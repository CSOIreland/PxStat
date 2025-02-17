using API;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Ganss.Xss;
using Newtonsoft.Json.Schema;
using PxStat.Data;
using PxStat.Security;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace PxStat.Resources
{
    internal class XlsxClosedXL
    {
        XLWorkbook workbook;
        internal XlsxClosedXL(MemoryStream xlstream)
        {
            workbook = new XLWorkbook(xlstream);

        }

        internal XlsxClosedXL()
        {
            workbook = new XLWorkbook();
        }

        internal MemoryStream InsertTableSheet(DataTable dt, string title)
        {
            var sheet = workbook.Worksheets.Add(title);
            var table = sheet.Cell(1, 1).InsertTable(dt, title, true);
            sheet.Columns().AdjustToContents();
            MemoryStream output = new MemoryStream();
            workbook.SaveAs(output);
            return output;
        }

        public XLWorkbook GetWorkbook()
        {
            return workbook;
        }

        /// <summary>
        /// Parse a specific html tag from the note string and return the start index and the length of the
        /// tag content to be used to decorate with rich text
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="index"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        public int[] ParseTag(string[] tag, int[] index, ref string note)
        {
            if (String.IsNullOrEmpty(note))
            {
                return index;
            }

            int startCount = Regex.Matches(note, tag[0]).Count;
            int endCount = Regex.Matches(note, tag[1]).Count;

            // Start and end tags have to match other wise there is an error 
            if (startCount != endCount)
            {
                Log.Instance.Error("The opening and closing tags for " + tag[0] + " do not match");
                return index;
            }

            // Get the start index of the html tag and the end end index of the html tag,
            // remove the tags from the note string and add the recalculated indexes to the index array
            index = new int[startCount * 2];
            int startIndex = 0;
            int endIndex = 0;
            int j = 0;

            for (int i = 0; i < startCount; i++)
            {
                startIndex = note.IndexOf(tag[0], startIndex, StringComparison.Ordinal);
                endIndex = note.IndexOf(tag[1], endIndex, StringComparison.Ordinal) - 3;
                note = note.Remove(startIndex, 3); // 3, for example, the length of the start tag: <b>
                note = note.Remove(endIndex, 4); // 4, for example, the length of the end tag: </b>

                // If there are other tags in the string before the startIndex
                int tags = Regex.Matches(note.Substring(0, startIndex), "<[biu]>").Count;
                if (tags > 0)
                {
                    startIndex -= tags * 7; // 7 for example, the length of the start and end tags: <b> </b>
                    endIndex -= tags * 7;
                }
                if (startIndex >= 0)
                {
                    // If there are other tags in the string between the startIndex and endIndex
                    tags = Regex.Matches(note.Substring(startIndex, endIndex - startIndex), "<[biu]>").Count;
                    if (tags > 0)
                    {
                        endIndex -= tags * 7; // 7 for example, the length of the start and end tags: <b> </b>
                    }

                    index[j++] = startIndex;
                    index[j] = endIndex - startIndex;
                    j++;
                }
            }
            return index;
        }

        /// <summary>
        /// Parse the note string and add rich text where there are specific html tags
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        public IXLWorksheet Parse(IXLWorksheet sheet, int row, int column, string note,string lngIsoCode=null)
        {

            BBCodeTagReader btr = new BBCodeTagReader(note,lngIsoCode);
            btr.parseTag("<b>", "</b>");
            btr.parseTag("<i>", "</i>");
            btr.parseTag("<u>", "</u>");
            btr.RemoveTags();
            sheet.Cell(row, column).Value = btr.Output;
            ProcessRichTextForTag(ref sheet, row, column, btr);

            return sheet;
        }

        /// <summary>
        /// Process the rich text for the tag using the index array to find the start index of the substring and the size of the substring
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="index"></param>
        /// <param name="tag"></param>
        private static void ProcessRichTextForTag(ref IXLWorksheet sheet, int row, int column, int[] index, string tag)
        {
            for (int i = 0; i < index.Length; i = i + 2)
            {
                if (sheet.Cell(row, column).GetRichText().Length < (i + 2)) return;
               
                if (index[i + 1] == 0) return;
                switch (tag)
                {
                    case "b":
                        
                        sheet.Cell(row, column).GetRichText().Substring(index[i], index[i + 1]).SetBold(true);
                        break;
                    case "i":
                        sheet.Cell(row, column).GetRichText().Substring(index[i], index[i + 1]).SetItalic(true);
                        break;
                    case "u":
                        sheet.Cell(row, column).GetRichText().Substring(index[i], index[i + 1]).SetUnderline();
                        break;
                    default:
                        //TODO - move to translation on cdn
                        Log.Instance.Error("Tag " + tag + " is not found, so cannot be processed");
                        break;
                }
            }
        }

        private static void ProcessRichTextForTag(ref IXLWorksheet sheet, int row, int column, BBCodeTagReader btr)
        {
            if(!btr.IsValid ) return;

            // Newlines to be interpreted as actual breaks rather than displayed as <br>
            sheet.Cell(row,column).Style.Alignment.SetWrapText(true);

            foreach (var pair in btr.Positions)
            {
                switch (pair.StartTag)

                {
                    case "<b>":
                        if (pair.End > pair.Start)
                        {
                            sheet.Cell(row, column).GetRichText().Substring(pair.Start, pair.End - pair.Start).SetBold(true);
                        }
                        break;

                    case "<i>":
                        if (pair.End > pair.Start)
                        {
                            sheet.Cell(row, column).GetRichText().Substring(pair.Start, pair.End - pair.Start).SetItalic(true);
                        }
                        break;

                    case "<u>":
                        if (pair.End > pair.Start)
                        {
                            sheet.Cell(row, column).GetRichText().Substring(pair.Start, pair.End - pair.Start).SetUnderline();
                        }
                        break;

                    default:
                        return;

                }
            }

        }

        internal MemoryStream CreateAboutPage(IDmatrix matrix, string title, string lngIsoCode, CultureInfo ci = null)
        {
            IDspec spec ;
            if (!matrix.Dspecs.ContainsKey(lngIsoCode))
            {
                lngIsoCode=Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
            }
            spec = matrix.Dspecs[lngIsoCode];

            string noteString = "";
            if (spec.Notes != null)
            {
                noteString = spec.GetNotesAsString();
                noteString = new BBCode().Transform(noteString, true);

                //Do this in case somebody tries to go under the radar by using html codes..
                //pvalue = pvalue.Replace("&lt;", "<");
                //pvalue = pvalue.Replace("&gt;", ">");

                noteString = HttpUtility.HtmlDecode(noteString);

                //If we don't sanitize natively then be default we use the HtmlSanitizer library to delete any script tags etc
                //We pass in the list of allowed tags - which is empty in our case - nothing allowed!

                //First iteration - nuke all scripts
                HtmlSanitizer sanitizer = new HtmlSanitizer();
                noteString = sanitizer.Sanitize(noteString);

                //Second iteration - remove all other tags but keep their contents
                sanitizer = new HtmlSanitizer();
                sanitizer.AllowedTags.Add("b");
                sanitizer.AllowedTags.Add("i");
                sanitizer.AllowedTags.Add("u");
                sanitizer.KeepChildNodes = true;
                noteString = sanitizer.Sanitize(noteString);

                //Allow end users to see tags instead of codes - the sanitizer will have replaced real signs with html codes
                //pvalue = pvalue.Replace("\u00A0", " ");
                //pvalue = pvalue.Replace("&gt;", ">");
                //pvalue = pvalue.Replace("&amp;", "&");

                noteString = HttpUtility.HtmlDecode(noteString);
            }


            if (matrix.Release != null)
            {
                if (matrix.Release.RlsLiveFlag && matrix.Release.RlsLiveDatetimeFrom < DateTime.Now)
                {
                    string Href = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "url.api.restful") + '/' +
                        string.Format(Configuration_BSO.GetStaticConfig("APP_RESTFUL_DATASET"), Configuration_BSO.GetStaticConfig("APP_READ_DATASET_API"), matrix.Code, Constants.C_SYSTEM_XLSX_NAME, Constants.C_SYSTEM_XLSX_VERSION, spec.Language), Type = Configuration_BSO.GetStaticConfig("APP_XLSX_MIMETYPE");
                }
            }

            var sheet = workbook.AddWorksheet(title);
            byte[] image = GetImage(Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "url.logo"));
            
            if (image != null && image.Length > 0)
            {
                sheet.AddPicture(new MemoryStream(image)).MoveTo(sheet.Cell("A1"));
            }

            sheet.Columns("B").Width = 100;
            int row = 8;
            int counter = 0;
            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.table", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.FontSize = 13;
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 1).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Cell(row + counter, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Cell(row + counter, 1).Style.Border.BottomBorderColor = XLColor.FromArgb(0xA2B8E1);// XLColor.AirForceBlue;   //( "A2B8E1");
            sheet.Cell(row + counter, 2).Style.Border.BottomBorderColor = XLColor.FromArgb(0xA2B8E1);
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.code", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = matrix.Code;
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.name", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = spec.Title;
            counter++;

            var dimTime = spec.Dimensions.Where(x => x.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_TIME)).FirstOrDefault();
            if (dimTime != null)
            {
                sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.frequency", lngIsoCode);
                sheet.Cell(row + counter, 1).Style.Font.Bold = true;
                sheet.Cell(row + counter, 2).Value =Label.Get("xlsx." + GetTlistCode(dimTime.Code, dimTime.Value),lngIsoCode);
                counter++;
            }
            

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.last-updated", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            //sheet.Cell(row + counter, 1).DataType = XLDataType.Text;
            if (matrix.Release != null)
                sheet.Cell(row + counter, 2).SetValue(Convert.ToString(matrix.Release.RlsLiveDatetimeFrom.ToString(ci ?? CultureInfo.InvariantCulture)));
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.note", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;

            // Parse the noteString to add some RichText, if needed
            Parse(sheet, row + counter, 2, noteString,lngIsoCode);

            sheet.Cell(row + counter,2).Style.Alignment.WrapText = true;
            counter++;

            if (matrix.Release != null)
            {
                if (matrix.Release.RlsLiveFlag && matrix.Release.RlsLiveDatetimeFrom < DateTime.Now)
                {
                    string Href = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "url.api.restful") + '/' +
                        string.Format(Configuration_BSO.GetStaticConfig("APP_RESTFUL_DATASET"), Configuration_BSO.GetStaticConfig("APP_READ_DATASET_API"), matrix.Code, Constants.C_SYSTEM_XLSX_NAME, Constants.C_SYSTEM_XLSX_VERSION, spec.Language), Type = Configuration_BSO.GetStaticConfig("APP_XLSX_MIMETYPE");

                    sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.url");
                    sheet.Cell(row + counter, 1).Style.Font.Bold = true;
                    sheet.Cell(row + counter, 2).Value = Href;
                    sheet.Cell(row + counter, 2).SetHyperlink(new XLHyperlink(Href));
                }
            }
            counter += 2;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.product", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.FontSize = 13;
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 1).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Cell(row + counter, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Cell(row + counter, 1).Style.Border.BottomBorderColor = XLColor.FromArgb(0xA2B8E1);
            sheet.Cell(row + counter, 2).Style.Border.BottomBorderColor = XLColor.FromArgb(0xA2B8E1);
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.code", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            if (matrix.Release != null)
                sheet.Cell(row + counter, 2).Value = matrix.Release.PrcCode;
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.name", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            if (matrix.Release != null)
                sheet.Cell(row + counter, 2).Value = spec.PrcValue;
            counter += 2;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.contacts", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.FontSize = 13;
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 1).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Cell(row + counter, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Cell(row + counter, 1).Style.Border.BottomBorderColor = XLColor.FromArgb(0xA2B8E1);
            sheet.Cell(row + counter, 2).Style.Border.BottomBorderColor = XLColor.FromArgb(0xA2B8E1);
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.name", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            if (matrix.Release != null)
                sheet.Cell(row + counter, 2).Value = matrix.Release.GrpContactName;
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.email", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            if (matrix.Release != null)
                sheet.Cell(row + counter, 2).Value = matrix.Release.GrpContactEmail;
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.phone", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            if (matrix.Release != null)
                sheet.Cell(row + counter, 2).Value = matrix.Release.GrpContactPhone;
            counter += 2;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.copyright", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.FontSize = 13;
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 1).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Cell(row + counter, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Cell(row + counter, 1).Style.Border.BottomBorderColor = XLColor.FromArgb(0xA2B8E1);
            sheet.Cell(row + counter, 2).Style.Border.BottomBorderColor = XLColor.FromArgb(0xA2B8E1);
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.code", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = matrix.Copyright.CprCode;
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.name", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = matrix.Copyright.CprValue;
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.url", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = matrix.Copyright.CprUrl;
            sheet.Cell(row + counter, 2).SetHyperlink(new XLHyperlink(matrix.Copyright.CprUrl));
            counter += 2;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.properties", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.FontSize = 13;
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 1).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Cell(row + counter, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Cell(row + counter, 1).Style.Border.BottomBorderColor = XLColor.FromArgb(0xA2B8E1);
            sheet.Cell(row + counter, 2).Style.Border.BottomBorderColor = XLColor.FromArgb(0xA2B8E1);
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.official-statistics", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = matrix.IsOfficialStatistic ? Label.Get("xlsx.yes", lngIsoCode) : Label.Get("xlsx.no", lngIsoCode);
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.exceptional", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            if (matrix.Release != null)
                sheet.Cell(row + counter, 2).Value = matrix.Release.RlsExceptionalFlag ? Label.Get("xlsx.yes", lngIsoCode) : Label.Get("xlsx.no", lngIsoCode);
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.archived", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            if (matrix.Release != null)
                sheet.Cell(row + counter, 2).Value = matrix.Release.RlsArchiveFlag ? Label.Get("xlsx.yes", lngIsoCode) : Label.Get("xlsx.no", lngIsoCode);
            counter++;

            string wildCardFlag = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "dataset.analytical.label");
            sheet.Cell(row + counter, 1).Value = Label.Get(wildCardFlag, lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            if (matrix.Release != null)
                sheet.Cell(row + counter, 2).Value = matrix.Release.RlsAnalyticalFlag ? Label.Get("xlsx.yes", lngIsoCode) : Label.Get("xlsx.no", lngIsoCode);
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.experimental", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            if (matrix.Release != null)
                sheet.Cell(row + counter, 2).Value = matrix.Release.RlsExperimentalFlag ? Label.Get("xlsx.yes", lngIsoCode) : Label.Get("xlsx.no", lngIsoCode);
            counter += 2;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.language", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.FontSize = 13;
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 1).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Cell(row + counter, 2).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
            sheet.Cell(row + counter, 1).Style.Border.BottomBorderColor = XLColor.FromArgb(0xA2B8E1);
            sheet.Cell(row + counter, 2).Style.Border.BottomBorderColor = XLColor.FromArgb(0xA2B8E1);
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.iso-code", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = spec.Language;
            counter++;

            sheet.Cell(row + counter, 1).Value = Label.Get("xlsx.iso-name", lngIsoCode);
            sheet.Cell(row + counter, 1).Style.Font.Bold = true;
            sheet.Cell(row + counter, 2).Value = new Language_BSO().Read(spec.Language).LngIsoName;


            sheet.Columns("A").Width = 20;
            MemoryStream output = new MemoryStream();
            workbook.SaveAs(output);
            return output;
        }

        private string GetTlistCode(string timeCode,string timeValue)
        {
            var codes=Configuration_BSO.GetStaticConfig("APP_PX_FREQUENCY_CODES").ToString().Split(',');
            Dictionary<string, string> cMap = new();
            foreach(string s in codes)
            {
                var keyVal=s.Split('/');
                if(keyVal.Length ==2)
                    cMap.Add(keyVal[0], keyVal[1]);
            }
            if(cMap.ContainsKey(timeCode))
                return cMap[timeCode];

            return timeValue;
        }

        private byte[] GetImage(string url)
        {

            try
            {

                byte[] imageData = null;

                var socketHandler = new SocketsHttpHandler()
                {
                    ConnectCallback = async (context, cancellationToken) =>
                    {
                        // Use DNS to look up the IP addresses of the target host:
                        // - IP v4: AddressFamily.InterNetwork
                        // - IP v6: AddressFamily.InterNetworkV6
                        // - IP v4 or IP v6: AddressFamily.Unspecified
                        // note: this method throws a SocketException when there is no IP address for the host
                        var entry = await Dns.GetHostEntryAsync(context.DnsEndPoint.Host, AddressFamily.InterNetwork, cancellationToken);

                        // Open the connection to the target host/port
                        var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

                        // Turn off Nagle's algorithm since it degrades performance in most HttpClient scenarios.
                        socket.NoDelay = true;

                        try
                        {
                            await socket.ConnectAsync(entry.AddressList, context.DnsEndPoint.Port, cancellationToken);

                            // If you want to choose a specific IP address to connect to the server
                            // await socket.ConnectAsync(
                            //    entry.AddressList[Random.Shared.Next(0, entry.AddressList.Length)],
                            //    context.DnsEndPoint.Port, cancellationToken);

                            // Return the NetworkStream to the caller
                            return new NetworkStream(socket, ownsSocket: true);
                        }
                        catch
                        {
                            socket.Dispose();
                            throw;
                        }
                    }

                };
                using (var httpClient = new HttpClient(socketHandler))
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", Configuration_BSO.GetStaticConfig("APP_USER_AGENT"));
                    imageData = GetImageData(httpClient, url).Result;
                }                
                return imageData;
            }
            catch(Exception ex) 
            {
                    Log.Instance.Error("Failed to get image from url: " + url + " " + ex.Message);
                    return null;
            }

        }

        public static async Task<byte[]> GetImageData(HttpClient httpClient, string url)
        {
            byte[] b;

            var httpResult = await httpClient.GetAsync(url);
            using var resultStream = await httpResult.Content.ReadAsStreamAsync();

            using (BinaryReader br = new BinaryReader(resultStream))
            {
                b = br.ReadBytes((int)resultStream.Length);
            }
            return b;

        }

        internal void Dispose()
        {
            workbook.Dispose();
        }
    }
}
