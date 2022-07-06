using API;
using PxParser.Resources.Parser;
using PxStat.Resources;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PxStat.Data
{

    public class PxFileBuilder
    {
        public string Create(IDmatrix matrix, IMetaData metaData, string lngIsoCode)
        {
            var sb = new StringBuilder();
            var baseKeywordList = new List<IPxKeywordElement>();
            var dataKeywordList = new List<IPxKeywordElement>();
            //Is there a statistics dimension?
            bool contVariable = (matrix.Dspecs[lngIsoCode].Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_STATISTIC).Count()) > 0;

            baseKeywordList.Add(CreatePxElement("CHARSET", metaData.GetPxDefaultCharSet()));
            baseKeywordList.Add(CreatePxElement("AXIS-VERSION", metaData.GetPxDefaultAxisVersion()));

            baseKeywordList.Add(CreatePxElement("LANGUAGE", lngIsoCode));
            if (matrix.Dspecs.Count > 1)
            {
                var langsList = new PxListOfValues(matrix.Languages.ToList());
                baseKeywordList.Add(CreatePxElementUnquoted("LANGUAGES", langsList.ToPxQuotedString()));

            }



            if (matrix.Release != null)
            {
                if (matrix.Release.RlsLiveDatetimeFrom != default)
                {
                    baseKeywordList.Add(CreatePxElement("LAST-UPDATED", (matrix.Release.RlsLiveDatetimeFrom).ToString(Utility.GetCustomConfig("APP_PX_DATE_TIME_FORMAT"))));
                }
            }
            baseKeywordList.Add(CreatePxElement("OFFICIAL-STATISTICS", matrix.IsOfficialStatistic ? metaData.GetPxTrue() : metaData.GetPxFalse()));

            var statDim = matrix.Dspecs[lngIsoCode].Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_STATISTIC).FirstOrDefault();

            int numberOfDecimalPlaces = 0;
            if (statDim != null)
            {
                if (statDim.Variables.Count > 0)
                {
                    baseKeywordList.Add(CreatePxElement("DECIMALS", statDim.Variables[0].Decimals));
                    numberOfDecimalPlaces = statDim.Variables[0].Decimals;
                }
                else
                {
                    baseKeywordList.Add(CreatePxElement("DECIMALS", matrix.Decimals));
                    numberOfDecimalPlaces = matrix.Decimals;
                }
            }

            baseKeywordList.Add(CreatePxElement("MATRIX", matrix.Code));

            //Separate out the stub and heading items

            Dictionary<string, PxListOfValues> heading = getHeadingDefault(matrix.Dspecs[lngIsoCode].Dimensions.ToList());
            Dictionary<string, PxListOfValues> stub = getStubDefault(matrix.Dspecs[lngIsoCode].Dimensions.ToList(), heading);

            //Get the count of values in the heading
            int headingCount = 1;
            foreach (var v in heading)
            {
                headingCount *= v.Value.Values.Count;
            }

            var mainSpecDoc = new PxDocument(GetKeywordsForSpec(matrix, metaData, matrix.Dspecs[lngIsoCode], stub, heading));


            List<PxDocument> otherSpecDocs = new List<PxDocument>();

            if (matrix.Dspecs.Count > 1)
            {
                foreach (var spec in matrix.Dspecs)
                {
                    if (spec.Key != lngIsoCode)
                    {
                        string lng = spec.Key;
                        heading = getHeadingDefault(matrix.Dspecs[lng].Dimensions.ToList());
                        stub = getStubDefault(matrix.Dspecs[lng].Dimensions.ToList(), heading);

                        otherSpecDocs.Add(new PxDocument(GetKeywordsForSpec(matrix, metaData, matrix.Dspecs[lng], stub, heading)));
                    }
                }
            }

            if (matrix.Cells != null)
            {
                string defaultVal = Configuration_BSO.GetCustomConfig(ConfigType.server, "px.confidential-value");
                List<string> dataCells = new List<string>();
                foreach (var c in matrix.Cells)
                {
                    var v = c.GetType();
                    string sval;
                    if (v.Name.Equals("Double"))
                    {
                        // Format number of decimal places
                        sval = FormatDecimalPlaces(c, numberOfDecimalPlaces);
                    }
                    else
                    {
                        sval = Convert.ToString(c);
                    }
                    if ((string.IsNullOrEmpty(sval) || sval.Equals(defaultVal)))
                    {
                        dataCells.Add(defaultVal);
                    }
                    else
                    {
                        dataCells.Add(sval);
                    }
                }

                if (dataCells.Count > 0)
                {
                    var codesList = new PxListOfValues(dataCells);
                    dataKeywordList.Add(CreatePxElementUnquoted("DATA", codesList.ToPxDataString(headingCount)));

                }
            }

            var baseDoc = new PxDocument(baseKeywordList);
            var dataDoc = new PxDocument(dataKeywordList);


            sb.Append(baseDoc.ToPxString());
            sb.Append(mainSpecDoc.ToPxString());
            foreach (var doc in otherSpecDocs)
                sb.Append(doc.ToPxString());
            sb.Append(dataDoc.ToPxString());


            return new Resources.Xlsx().ConvertStringToUtf8Bom(sb.ToString());
        }

        private List<IPxKeywordElement> GetKeywordsForSpec(IDmatrix matrix, IMetaData metaData, IDspec spec, Dictionary<string, PxListOfValues> stub, Dictionary<string, PxListOfValues> heading, bool forceStatisticEntry = false)
        {
            var keywordList = new List<IPxKeywordElement>();

            bool contVariable = spec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_STATISTIC).FirstOrDefault().Variables.Count() > 0 || forceStatisticEntry;

            string lngTag;

            if (spec.Language != Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code"))
                lngTag = spec.Language != null ? "[" + spec.Language + "]" : "";
            else
                lngTag = "";

            if (matrix.Release != null)
            {
                keywordList.Add(CreatePxElement("SUBJECT-AREA" + lngTag, !String.IsNullOrEmpty(matrix.Release.PrcValue) ? matrix.Release.PrcValue : Label.Get("default.subject-area")));
                keywordList.Add(CreatePxElement("SUBJECT-CODE" + lngTag, !String.IsNullOrEmpty(matrix.Release.PrcValue) ? matrix.Release.PrcCode.ToString() : Label.Get("default.subject-code"))); ////
            }
            else
            {
                keywordList.Add(CreatePxElement("SUBJECT-AREA" + lngTag, Label.Get("default.subject-area")));
                keywordList.Add(CreatePxElement("SUBJECT-CODE" + lngTag, Label.Get("default.subject-code")));
            }

            keywordList.Add(CreatePxElement("DESCRIPTION" + lngTag, spec.Title)); /////

            if (spec.Title != null)
                keywordList.Add(CreatePxElement("TITLE" + lngTag, spec.Title));
            else
                keywordList.Add(CreatePxElement("TITLE" + lngTag, getContentString(spec))); ////

            keywordList.Add(CreatePxElement("CONTENTS" + lngTag, spec.Contents)); ////

            //Units
            //var distinctUnits = theSpec.Statistic.Select(o => o.Unit).Distinct();
            var distinctUnits = spec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_STATISTIC).FirstOrDefault().Variables.Select(x => x.Unit).Distinct();

            //Units

            if (distinctUnits.ElementAt(0) != null)
                keywordList.Add(CreatePxElement("UNITS" + lngTag, distinctUnits.ElementAt(0)));
            else
                keywordList.Add(CreatePxElement("UNITS" + lngTag, Utility.GetCustomConfig("APP_PX_DEFAULT_UNITS")));

            //Create the Stub entries
            List<string> stubs = new List<string>();
            foreach (KeyValuePair<string, PxListOfValues> pair in stub)
            {
                stubs.Add(pair.Key);
            }
            var stubsList = new PxListOfValues(stubs);
            keywordList.Add(CreatePxElementUnquoted("STUB" + lngTag, stubsList.ToPxQuotedString()));


            //Create the Heading entries
            List<string> headings = new List<string>();
            foreach (KeyValuePair<string, PxListOfValues> pair in heading)
            {
                headings.Add(pair.Key);
            }
            var headingsList = new PxListOfValues(headings);
            keywordList.Add(CreatePxElementUnquoted("HEADING" + lngTag, headingsList.ToPxQuotedString()));

            //CONTVARIABLE
            if (contVariable)
            {
                keywordList.Add(CreatePxElement("CONTVARIABLE" + lngTag, spec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_STATISTIC).FirstOrDefault().Value)); ////
            }

            foreach (IStatDimension dim in spec.Dimensions)
            {
                PxListOfValues px = new PxListOfValues(dim.Variables.Select(x => x.Value).ToList());
                keywordList.Add(CreatePxElementUnquoted("VALUES" + lngTag, dim.Value, px.ToPxQuotedString("VALUES", Convert.ToInt32(metaData.GetPxMultilineCharLimit()))));
            }

            //Create the Timeval

            var periodList = new PxListOfValues(spec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_TIME).FirstOrDefault().Variables.Select(x => x.Code).ToList());
            var part2 = spec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_TIME).FirstOrDefault().Code;

            keywordList.Add(CreatePxElementUnquoted("TIMEVAL" + lngTag, spec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_TIME).FirstOrDefault().Value, spec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_TIME).FirstOrDefault().Code + ',' + periodList.ToPxQuotedString())); ////

            //We don't need CODES for the time dimension because we already have TIMEVAL
            foreach (var dim in spec.Dimensions.Where(x => !x.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_TIME)))
            {
                PxListOfValues plv = new PxListOfValues(dim.Variables.Select(x => x.Code).ToList());
                KeyValuePair<string, PxListOfValues> kvp = new KeyValuePair<string, PxListOfValues>(dim.Code, plv);
                keywordList.Add(CreatePxElementUnquoted("CODES" + lngTag, dim.Value, kvp.Value.ToPxQuotedString("CODES", Convert.ToInt32(metaData.GetPxMultilineCharLimit()))));
            }

            //Domain
            foreach (var cls in spec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_CLASSIFICATION))
            {
                keywordList.Add(CreatePxElement("DOMAIN" + lngTag, cls.Value, new PxStringValue(cls.Code))); ////

            }

            //Create the MAP data
            foreach (var dim in spec.Dimensions)
            {
                if (!String.IsNullOrEmpty(dim.GeoUrl))
                    keywordList.Add(CreatePxElement("MAP" + lngTag, dim.Value, new PxStringValue(dim.GeoUrl))); ////
            }

            if (contVariable)
            {
                var sDim = spec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_STATISTIC).FirstOrDefault();
                foreach (var vrb in sDim.Variables)
                {
                    keywordList.Add(CreatePxElement("UNITS" + lngTag, vrb.Value, vrb.Unit)); ////
                }

            }

            //Refperiod and Baseperiod
            if (contVariable)
            {
                foreach (var stat in spec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_STATISTIC))
                {
                    foreach (var vrb in stat.Variables)
                    {
                        keywordList.Add(CreatePxElement("REFPERIOD" + lngTag, vrb.Value, " ")); ////
                    }
                }
                foreach (var stat in spec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_STATISTIC))
                {
                    foreach (var vrb in stat.Variables)
                    {
                        keywordList.Add(CreatePxElement("BASEPERIOD" + lngTag, vrb.Value, " ")); ////
                    }
                }
            }

            foreach (var cls in spec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_CLASSIFICATION))
            {
                var elimVrb = cls.Variables.Where(x => x.Elimination).FirstOrDefault();
                if (elimVrb != null)
                {
                    keywordList.Add(CreatePxElement("ELIMINATION" + lngTag, cls.Value, elimVrb.Value));
                }
            }

            keywordList.Add(CreatePxElement("SOURCE" + lngTag, spec.Source));
            keywordList.Add(CreatePxElement("NOTE" + lngTag, String.Join(",", spec.Notes), false));

            var statDim = spec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_STATISTIC).FirstOrDefault();


            foreach (var vrb in statDim.Variables)
            {
                if (vrb.Decimals > 0)
                {
                    List<string> statistics = new List<string>();
                    if (contVariable)
                        statistics.Add(spec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_STATISTIC).FirstOrDefault().Code);

                    statistics.Add(vrb.Value);
                    var statsList = new PxListOfValues(statistics);

                    IPxKeywordElement pxOut = CreatePxElementUnquotedIndividually("PRECISION" + lngTag, statsList.ToPxQuotedString(), vrb.Decimals.ToString());

                    keywordList.Add(pxOut); ////
                }
            }


            return keywordList;
        }

        private string getContentString(IDspec theSpec)
        {
            string content = theSpec.Contents;
            IEnumerable<string> clsList = new List<string>();
            clsList = theSpec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_CLASSIFICATION).Select(x => x.Value);
            if (clsList.Count() > 0) content = content + " (" + String.Join(",", clsList) + ")";

            return content;
        }

        private IPxKeywordElement CreatePxElementUnquotedIndividually(string key, string subKey, string value)
        {

            PxKeywordElement pk = new PxKeywordElement(new PxKey(key, (new PxSubKey(subKey))), new PxStringValue(value));
            return pk;
        }

        private Dictionary<string, PxListOfValues> getStubDefault(IList<StatDimension> dimensions, Dictionary<string, PxListOfValues> heading)
        {
            Dictionary<string, PxListOfValues> headingDict = new Dictionary<string, PxListOfValues>();

            foreach (var dim in dimensions)
            {
                if (!heading.ContainsKey(dim.Value))
                    headingDict.Add(dim.Value, new PxListOfValues(dim.Variables.Select(x => x.Value).ToList()));
            }

            return headingDict;
        }

        private Dictionary<string, PxListOfValues> getHeadingDefault(IList<StatDimension> dimensions)
        {
            Dictionary<string, PxListOfValues> headingDict = new Dictionary<string, PxListOfValues>();

            headingDict.Add(dimensions[dimensions.Count - 1].Value, new PxListOfValues((dimensions[dimensions.Count - 1]).Variables.Select(x => x.Value).ToList()));


            return headingDict;
        }

        private IPxKeywordElement CreatePxElementUnquoted(string key, string subKey, string value)
        {

            PxKeywordElement pk = new PxKeywordElement(new PxKey(key, new PxSubKey(subKey)), new PxStringValue(value));

            return pk;
        }

        private IPxKeywordElement CreatePxElementUnquoted(string key, string value)
        {
            return new PxKeywordElement(new PxKey(key), new PxStringValue(value));
        }

        private IPxKeywordElement CreatePxElement(string key, string value, bool removeHtml = true)
        {
            return new PxKeywordElement(new PxKey(key), new PxQuotedValue(value, removeHtml));
        }

        private IPxKeywordElement CreatePxElement(string key, short value)
        {
            return new PxKeywordElement(new PxKey(key), new PxLiteralValue(value));
        }

        private IPxKeywordElement CreatePxElement(string key, string subKey, string value)
        {
            return new PxKeywordElement(new PxKey(key, new PxSubKey(subKey)), new PxQuotedValue(value));
        }

        private string ConvertStringToUtf8Bom(string source)
        {
            var data = Encoding.UTF8.GetBytes(source);
            var result = Encoding.UTF8.GetPreamble().Concat(data).ToArray();
            var encoder = new UTF8Encoding(true);

            return encoder.GetString(result);
        }

        public string FormatDecimalPlaces(double value, int numberOfDecimalPlaces)
        {
            if (numberOfDecimalPlaces == 0)
            {
                return Convert.ToString(value);
            }

            switch (numberOfDecimalPlaces)
            {
                case 1:
                    return String.Format("{0:0.0}", value);
                case 2:
                    return String.Format("{0:0.00}", value);
                case 3:
                    return String.Format("{0:0.000}", value);
                case 4:
                    return String.Format("{0:0.0000}", value);
                case 5:
                    return String.Format("{0:0.00000}", value);
                case 6:
                    return String.Format("{0:0.000000}", value);
            }
            return Convert.ToString(value);
        }
    }
}
