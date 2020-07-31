using PxStat.Build;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using static PxStat.Data.Matrix;

namespace PxStat.Data
{
    internal class Sdmx
    {
        internal string GetSDMXdata(Matrix theMatrix, string LngIsoCode)
        {
            Specification theSpec = theMatrix.GetSpecFromLanguage(LngIsoCode);

            Build_BSO bso = new Build_BSO();
            List<DataItem_DTO> dataItems = bso.GetMatrixDataItems(theMatrix, LngIsoCode, theSpec, false, true);


            //sort the data in SCP order
            foreach (var item in dataItems)
            {
                item.sortWord = item.statistic.Code + '/';

                foreach (var cls in item.classifications)
                {
                    item.sortWord = item.sortWord + cls.Code + '/' + cls.Variable[0].Code;
                }
                item.sortWord = item.sortWord + item.period.Code;
            }

            var slist = dataItems.OrderBy(x => x.sortWord);

            int counter = 0;
            List<SdmxItem> sdmxList = new List<SdmxItem>();
            SdmxItem sdmx = new SdmxItem();
            foreach (var item in slist)
            {
                if (counter == 0 || counter % theSpec.Frequency.Period.Count == 0)
                {

                    if (sdmx.Period != null) sdmxList.Add(sdmx);

                    sdmx = new SdmxItem() { Statistic = item.statistic, Classifications = item.classifications, Period = new List<PeriodRecordDTO_Create>(), Value = item.dataValue, PeriodValue = new Dictionary<string, dynamic>() };
                    sdmx.Period.Add(item.period);
                    sdmx.PeriodValue.Add(item.period.Code, item.dataValue);
                }
                else
                {
                    sdmx.Period.Add(item.period);
                    sdmx.PeriodValue.Add(item.period.Code, item.dataValue);
                }
                counter++;
            }
            sdmxList.Add(sdmx);


            XNamespace messageNS = "http://www.sdmx.org/resources/sdmxml/schemas/v2_0/message";
            XNamespace c = "http://www.sdmx.org/resources/sdmxml/schemas/v2_0/common";
            XNamespace xml = "http://www.w3.org/XML/1998/namespace";
            XNamespace ss = "http://www.sdmx.org/resources/sdmxml/schemas/v2_0/data/structurespecific";
            XNamespace xsi = "http://www.sdmx.org/resources/sdmxml/schemas/v2_0/data/structurespecific";
            XNamespace g = "http://www.sdmx.org/resources/sdmxml/schemas/v2_0/generic";


            XElement id = new XElement(g + "ID", theMatrix.Code);
            XElement test = new XElement(g + "Test", "false");
            XElement prepared = new XElement(g + "Prepared", theMatrix.CreationDateTime.ToString("yyyy-MM-ddTHH:mm"));
            XAttribute senderID = new XAttribute("ID", "CSO");
            XAttribute senderNameAttr = new XAttribute(xml + "lang", LngIsoCode);
            XElement senderName = new XElement(g + "Name", theSpec.Source, senderNameAttr);
            XElement sender = new XElement(g + "Sender", senderName, senderID);

            XElement header = new XElement(g + "Header",
               id,
               test,
               prepared,
               sender
               );


            XElement keyfamilyref = new XElement(g + "KeyFamilyRef", "REF");
            XElement series = new XElement(g + "Series");
            foreach (SdmxItem item in sdmxList)
            {
                XElement seriesKey = new XElement(g + "SeriesKey");
                seriesKey.Add(new XElement(g + "Value", new XAttribute("concept", theSpec.ContentVariable), new XAttribute("value", item.Statistic.Code)));

                seriesKey.Add(new XElement(g + "Value", new XAttribute("concept", theSpec.Frequency.Value), new XAttribute("value", theSpec.Frequency.Code)));

                foreach (var cls in item.Classifications)
                {
                    seriesKey.Add(new XElement(g + "Value", new XAttribute("concept", cls.Value), new XAttribute("value", cls.Variable[0].Code)));
                }
                series.Add(seriesKey);

                XElement attributes = new XElement(g + "Attributes");
                attributes.Add(new XElement(g + "Values", new XAttribute("concept", "UNIT_VALUE"), new XAttribute("value", item.Statistic.Unit)));
                series.Add(attributes);

                foreach (var per in item.Period)
                {
                    XElement Obs = new XElement(g + "Obs");
                    Obs.Add(new XElement(g + "Time", per.Code));
                    Obs.Add(new XElement(g + "ObsValue", new XAttribute("value", item.PeriodValue[per.Code])));
                    Obs.Add(new XElement(g + "Attributes", new XElement("Value", new XAttribute("concept", "OBS_STATUS"), new XAttribute("value", "A"))));
                    series.Add(Obs);
                }
            }

            XElement annotations = new XElement(c + "Annotations");

            if (theSpec.Notes != null)
            {
                foreach (string note in theSpec.Notes)
                {
                    annotations.Add(new XElement(c + "AnnotationText", new XAttribute(xml + "lang", LngIsoCode), note));
                }
            }


            XElement dataset = new XElement("DataSet", new XAttribute(XNamespace.Xmlns + "g", g), new XAttribute(XNamespace.Xmlns + "c", c), keyfamilyref, series, annotations);



            XElement root = new XElement(g + "GenericData", header, dataset);


            XmlDocument document = new XmlDocument();

            XmlDeclaration xmldecl;
            xmldecl = document.CreateXmlDeclaration("1.0", null, null);
            xmldecl.Encoding = "UTF-8";
            xmldecl.Standalone = "yes";



            document.Load(new StringReader(root.ToString()));

            XmlElement rt = document.DocumentElement;
            document.InsertBefore(xmldecl, rt);



            StringBuilder builder = new StringBuilder();
            using (XmlTextWriter writer = new XmlTextWriter(new StringWriter(builder)))
            {
                writer.Formatting = Formatting.Indented;
                document.Save(writer);
            }

            return builder.ToString();


            // return  root.ToString();
        }
    }

    internal class SdmxItem
    {
        internal StatisticalRecordDTO_Create Statistic { get; set; }
        internal List<ClassificationRecordDTO_Create> Classifications { get; set; }
        internal List<PeriodRecordDTO_Create> Period { get; set; }
        internal Dictionary<string, dynamic> PeriodValue { get; set; }
        internal dynamic Value { get; set; }
    }
}
