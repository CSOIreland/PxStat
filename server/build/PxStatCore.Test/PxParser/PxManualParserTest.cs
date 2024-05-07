using PxParser.Resources.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.IO;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2013.Excel;
using PxStat.Properties;
using PxStatCore.Test;

namespace PxStatXUnit.Tests
{
    [Collection("PxStatXUnit")]
    public class PxManualParserTest
    {
        private const string EQUALS = "=";
        [Theory]
        [InlineData("DATA=\r\n28612 61598 27369 62484 34930 70051 38721;\r\n", true)]
        [InlineData("DATA=\r\n28612\r\n61598\r\n27369\r\n62484\r\n34930\r\n70051\r\n38721;\r\n", true)]
        [InlineData("DATA=\r\n\"single\",\"0-4\",306443 290460 596903 301373 284917 586290 296138 280460 576598 290726 275596 566322 285378 270765 556143 282059 267532 549591 283338 269260 552598 281565 267572 549137 274909 261653 536562 267596 254791 522387 258990 246712 505702 251440 239689 491129 247885 235951 483836 245098 233666 478764 243176 231842 475018 242615 230750 473365 241928 229573 471501 243242 230272 473514 248066 234372 482438 254176 241051 495227 265130 251159 516289 277148 262619 539767 290330 275635 565965 301091 285607 586698 310189 293395 603584 312287 296326 608613 310539 295172 605711 298680 283639 582319 282416 269074 551490 265400 253132 518532 251848 239508 491356 240440 228276 468716;\r\n", true)]
        [InlineData("DATA=\r\n\"00 Sweden\",\"20\",456 223 327\r\n\"00 Sweden\",\"21\",613 305 464\r\n\"00 Sweden\",\"22\",835 325 511\r\n\"01 Stockholm county\",\"20\",590 480 741\r\n\"01 Stockholm county\",\"21\",771 668 993\r\n\"01 Stockholm county\",\"22\",924 672 906\r\n\"0114 Upplands Väsby\",\"20\",14 38 22 43\r\n\"0114 Upplands Väsby\",\"21\",21 36 34 57\r\n\"0114 Upplands Väsby\",\"22\",26 36 34 34\r\n\"0115 Vallentuna\",\"20\",17 20 22 30\r\n\"0115 Vallentuna\",\"21\",13 21 29 42\r\n\"..\",\"22\",20 23 33 35;\r\n", true)]
        public void TestDataParser(string input, bool expected)
        {
            Helper.SetupTests();

            PxManualParser parser = new PxManualParser(input);

            // Remove , and ; characters and "\r\n" from input as they will not be shown when the document is parsed
            var values = input.Split('=').ToList<string>();
            if (values.Count > 0)
            {
                input = values[0].Replace("\r\n", "") + "=" + values[1].Replace("\r\n", " ");
            }
            input = input.Replace(",", " ").Replace(";\r\n", "").Replace("\r\n", "").Replace("= ", "=");

            PxDocument pxDocument = parser.Parse();
            Assert.Equal(pxDocument != null, expected);
            Assert.Equal(1, pxDocument.Keywords.Count);
            PxKey key = pxDocument.Keywords[0].Key;
            var data = pxDocument.GetData(key.ToString());
            var value = pxDocument.Keywords[0].Element;

            // Replace "" with " for dot quoted strings
            var valueString = value.ToPxValue().Replace("\"\"", "\"");
            Assert.Equal(input, key + EQUALS + valueString + "; ");
        }

        [Theory]
        [InlineData("DATA=\r\n76 80\r\n75 70\r\n72 79\r\n55 68\r\n45 43\r\n38 47\r\n33 17\r\n15 4\r\n8 8\r\n3 3\r\n4 3\r\n\"less than 1\" \"less than 1\";\r\n", true)]

        public void TestDataParserWithStrings(string input, bool expected)
        {
            Helper.SetupTests();
            PxManualParser parser = new PxManualParser(input);

            // Remove , and ; characters and "\r\n" from input as they will not be shown when the document is parsed
            var values = input.Split('=').ToList<string>();
            if (values.Count > 0)
            {
                input = values[0].Replace("\r\n", "") + "=" + values[1].Replace("\r\n", " ");
            }
            input = input.Replace(",", " ").Replace(";\r\n", "").Replace("\r\n", "").Replace("= ", "=").Replace("\"", "");

            PxDocument pxDocument = parser.Parse();
            Assert.Equal(expected, pxDocument != null);
            Assert.Equal(1, pxDocument.Keywords.Count);
            PxKey key = pxDocument.Keywords[0].Key;
            var data = pxDocument.GetData(key.ToString());
            var value = pxDocument.Keywords[0].Element;
            var valueString = value.ToPxValue();
            Assert.Equal(input, key + EQUALS + valueString + "; ");
        }
        [Theory]
        [InlineData("DATA=\r\n28612 61598 27369 62484 34930 70051 38721\r\n;", true)]
        public void TestDataParserWithCarriageReturnLineFeed(string input, bool expected)
        {
            Helper.SetupTests();
            PxManualParser parser = new PxManualParser(input);

            // Remove , and ; characters and "\r\n" from input as they will not be shown when the document is parsed
            var values = input.Split('=').ToList<string>();
            if (values.Count > 0)
            {
                input = values[0].Replace("\r\n", "") + "=" + values[1].Replace("\r\n", " ");
            }
            input = input.Replace(",", " ").Replace(";\r\n", "").Replace("\r\n", "").Replace("= ", "=");

            PxDocument pxDocument = parser.Parse();
            Assert.Equal(expected, pxDocument != null);
            Assert.Equal(1, pxDocument.Keywords.Count);
            PxKey key = pxDocument.Keywords[0].Key;
            var data = pxDocument.GetData(key.ToString());
            var value = pxDocument.Keywords[0].Element;

            // Replace "" with " for dot quoted strings
            var valueString = value.ToPxValue().Replace("\"\"", "\"");
            Assert.Equal(input, key + EQUALS + valueString + " ;");
        }
        [Theory]
        [InlineData("CHARSET=\"UTF-16\";\r\n", true)]
        [InlineData("AXIS-VERSION=\"2000\";\r\n", true)]
        [InlineData("LANGUAGE=\"en\";\r\n", true)]
        [InlineData("AUTOPEN=\"YES\";\r\n", true)]
        [InlineData("CFPRICES=\"C\";\r\n", true)]
        [InlineData("LAST-UPDATED=\"19990318 18:12\";\r\n", true)]
        [InlineData("CREATION-DATE=\"20100718 19:14\";\r\n", true)]
        [InlineData("CODEPAGE=\"iso-8859-1\";\r\n", true)]
        [InlineData("CONTACT(\"value\")=\"xx\";\r\n", true)]
        [InlineData("CONTVARIABLE=\"Statistic\";\r\n", true)]
        [InlineData("COPYRIGHT=\"YES\";\r\n", true)]
        [InlineData("DATABASE=\"CSO Databank\";\r\n", true)]
        [InlineData("DATANOTESUM=\".\";\r\n", true)]
        [InlineData("DATASYMBOL1=\"one\";\r\n", true)]
        [InlineData("DATASYMBOL2=\"two\";\r\n", true)]
        [InlineData("DATASYMBOL3=\"three\";\r\n", true)]
        [InlineData("DATASYMBOL4=\"four\";\r\n", true)]
        [InlineData("DATASYMBOL5=\"five\";\r\n", true)]
        [InlineData("DATASYMBOL6=\"six\";\r\n", true)]
        [InlineData("DATASYMBOLNIL=\"-\";\r\n", true)]
        [InlineData("DATASYMBOLSUM=\"sum\";\r\n", true)]
        [InlineData("DATASYMBOLSUM[sv]=\"summa\";\r\n", true)]
        [InlineData("DAYADJ=\"NO\";\r\n", true)]
        [InlineData("DESCRIPTIONDEFAULT=\"YES\";\r\n", true)]
        [InlineData("DIRECTORY-PATH=\".\";\r\n", true)]
        [InlineData("FIRST-PUBLISHED=\"20130224 20:55\";\r\n", true)]
        [InlineData("INFO=\"Some information\";\r\n", true)]
        [InlineData("INFOFILE=\"DB_DH\";\r\n", true)]
        [InlineData("LINK=\"Link name\";\r\n", true)]
        [InlineData("META-ID=\"RT:12\";\r\n", true)]
        [InlineData("NEXT-UPDATE=\"20070224 22:54\";\r\n", true)]
        [InlineData("NOTE=\"\";\r\n", true)]
        [InlineData("NOTE=\"Test English note\";\r\n", true)]
        [InlineData("NOTE[sv]=\"Test svensk note\";\r\n", true)]
        [InlineData("NOTEX=\"Mandatory English note\";\r\n", true)]
        [InlineData("OFFICIAL-STATISTICS=\"YES\";\r\n", true)]
        [InlineData("SEAADJ=\"YES\";\r\n", true)]
        [InlineData("SOURCE=\"Central Statistics Office, Ireland\";\r\n", true)]
        [InlineData("STOCKFA=\"S\";\r\n", true)]
        [InlineData("SURVEY=\"Survey name\";\r\n", true)]
        [InlineData("SYNONYMS=\"Inflation\";\r\n", true)]
        [InlineData("TABLEID=\"08811\";\r\n", true)]
        [InlineData("UPDATE-FREQUENCY=\"12\";\r\n", true)]
        [InlineData("VARIABLE-TYPE=\"\";\r\n", true)]
        public void TestBasicParser(string input, bool expected)
        {
            Helper.SetupTests();
            PxManualParser parser = new PxManualParser(input);
            PxDocument pxDocument = parser.Parse();
            Assert.Equal(expected, pxDocument != null);
            Assert.Equal(1, pxDocument.Keywords.Count);
            PxKey key = pxDocument.Keywords[0].Key;
            var value = pxDocument.GetStringElementValue(key.Identifier);
            input = input.Replace("\"", "").Replace("\r\n", "");
            var keyString = key.ToPxString().Replace("\"", "");
            var valueString = value.ToString().Replace("\"", "");
            Assert.Equal(input, keyString + EQUALS + valueString + ";");
        }
        [Theory]
        [InlineData("LANGUAGES=\"sv\",\"en\";\r\n", true)]
        [InlineData("ATTRIBUTE-ID=\"ObsStatus\",\"ObsConf\";\r\n", true)]
        [InlineData("ATTRIBUTE-TEXT=\"Observation status\",\"Observation confidence\";\r\n", true)]
        [InlineData("ATTRIBUTE-TEXT[\"sv\"]=\"Status\",\"Tillit\";\r\n", true)]
        [InlineData("ATTRIBUTES=\"A\",\"F\";\r\n", true)]
        [InlineData("CODES(\"region\")=\"AL\",\"AT\";\r\n", true)]
        [InlineData("TIMEVAL(\"time\")=TLIST(A1),\"1994\",\"1995\",\"1996\";\r\n", true)]
        [InlineData("HEADING=\"Year\",\"Statistic\";\r\n", true)]
        [InlineData("STUB=\"Species\";\r\n", true)]
        [InlineData("PARTITIONED(\"age\")=\"municipality\",1,4, \"Test\";\r\n", true)]
        [InlineData("VALUES(\"Industry Sector NACE Rev 2\")=\"Food; products(10)\",\"Meat; and meat products(101)\";\r\n", true)]
        [InlineData("VALUES(\"Statistic\")=\"Infant Mortality\",\"Deaths of Infants under One Year per 1,000 Live Births\",\"Neonatal Mortality\",\r\n\"Deaths of Infants under 28 Days per 1,000 Live Births\";\r\n", true)]
        [InlineData("VALUES(\"Procedure\")=\"All procedures\",\"Extirpation, excision & destruction of intracranial lesion\";\r\n", true)]
        public void TestListParser(string input, bool expected)
        {
            Helper.SetupTests();
            PxManualParser parser = new PxManualParser(input);
            PxDocument pxDocument = parser.Parse();
            string language = pxDocument.Keywords[0].Key.Language;
            Assert.Equal(expected, pxDocument != null);
            Assert.Equal(1, pxDocument.Keywords.Count);
            PxKey key = pxDocument.Keywords[0].Key;
            var value = pxDocument.Keywords[0].Element;
            input = input.Replace("\"", "").Replace("\r\n", "");
            var keyString = key.ToPxString().Replace("\"", "");
            Assert.Equal(input, keyString + EQUALS + value + ";");
        }
        [Theory]
        [InlineData("DECIMALS=2;\r\n", true)]
        [InlineData("CONFIDENTIAL=0;\r\n", true)]
        [InlineData("DEFAULT-GRAPH=1;\r\n", true)]
        [InlineData("HIERARCHYLEVELS(\"Country\")=4;\r\n", true)]
        [InlineData("HIERARCHYLEVELSOPEN(\"Country\")=4;\r\n", true)]
        [InlineData("PRECISION(\"variable name\",\"value name\")=2;\r\n", true)]
        [InlineData("PRESTEXT(\"region\")=2;\r\n", true)]
        [InlineData("ROUNDING=0;\r\n", true)]
        [InlineData("SHOWDECIMALS=2;\r\n", true)]
        public void TestNumberParser(string input, bool expected)
        {
            Helper.SetupTests();
            PxManualParser parser = new PxManualParser(input);
            PxDocument pxDocument = parser.Parse();
            Assert.Equal(expected, pxDocument != null);
            Assert.Equal(1, pxDocument.Keywords.Count);
            input = input.Replace("\r\n", "");
            PxKey key = pxDocument.Keywords[0].Key;
            var value = pxDocument.GetShortElementValue(key.Identifier);
            Assert.Equal(input, key + EQUALS + value + ";");
        }
        [Theory]
        [InlineData("PRECISION(\"Statistic\",\"Death Rate Registered per 1,000 Estimated Population\")=2;\r\n", true)]
        public void TestPrecisionWithCommas(string input, bool expected)
        {
            Helper.SetupTests();
            PxManualParser parser = new PxManualParser(input);
            PxDocument pxDocument = parser.Parse();
            Assert.Equal(pxDocument != null, expected);
            Assert.Equal(1, pxDocument.Keywords.Count);
            input = input.Replace("\r\n", "");
            PxKey key = pxDocument.Keywords[0].Key;
            var value = pxDocument.GetShortElementValue(key.Identifier);
            Assert.Equal(input, key + EQUALS + value + ";");
        }
        [Theory]
        [InlineData("LAST-UPDATED(\"value\")=\"19990318 18:12\";\r\n", true)]
        [InlineData("BASEPERIOD(\"Trade Surplus (Exports Minus Imports)\")=\" \";\r\n", true)]
        [InlineData("CFPRICES(\"value\")=\"C\";\r\n", true)]
        [InlineData("CONTACT=\"Maria Svensson, SCB, +4619176800, +4619176900, maria.svensson@scb.se\";\r\n", true)]
        [InlineData("DATANOTECELL(\" * \", \"20\", \"*\", \"BE0101F2\", \"*\")=\"Ae\";\r\n", true)]
        [InlineData("DAYADJ(\"value\")=\"NO\";\r\n", true)]
        [InlineData("DOMAIN(\"Age group\")=\"C02076V02508\";\r\n", true)]
        [InlineData("DOMAINCOLUMN(\"region\")=\"YES\";\r\n", true)]
        [InlineData("ELIMINATION(\"variable name\")=\"value name\";\r\n", true)]
        [InlineData("HIERARCHIES(\"Country\")=\"parent\",\"parent\":\"child\";\r\n", true)]
        [InlineData("KEYS(\"age\")=\"VALUES\";\r\n", true)]
        [InlineData("MAP(\"region\")=\"Sweeden_municipality\";\r\n", true)]
        [InlineData("META-ID(\"VARIABLE\")=\"V:167\";\r\n", true)]
        [InlineData("SEAADJ(\"value\")=\"NO\";\r\n", true)]
        [InlineData("STOCKFA(\"value\")=\"S\";\r\n", true)]
        [InlineData("VALUENOTE(\"typ\",\"Inmigrated\")=\"Request value note for inmigrated.\";\r\n", true)]
        public void TestSubKeyParser(string input, bool expected)
        {
            Helper.SetupTests();
            PxManualParser parser = new PxManualParser(input);
            PxDocument pxDocument = parser.Parse();
            Assert.Equal(expected, pxDocument != null);
            Assert.Equal(1, pxDocument.Keywords.Count);
            PxKey key = pxDocument.Keywords[0].Key;
            var value = pxDocument.GetStringElementValue(key.Identifier);
            input = input.Replace("\"", "").Replace("\r\n", "");
            var keyString = key.ToPxString().Replace("\"", "");
            var valueString = value.ToString().Replace("\"", "");
            Assert.Equal(input, keyString + EQUALS + valueString + ";");
        }
        [Theory]
        [InlineData("NOTE(\"marital status\")=\"Marital status is dependent on the registration in the census. Married persons\"\r\n\"living together are rendered as married. Other persons living together are reg\"\r\n\"arded as single.\";\r\n", true)]
        [InlineData("NOTEX=\"Some note\";\r\n", true)]
        [InlineData("NOTEX=\"Some not\"\r\n\"e\";\r\n", true)]
        [InlineData("PX-SERVER=\"test.ie\";\r\n", true)]
        [InlineData("BASEPERIOD(\"var4val1\")=\"2000\";\r\n", true)]
        [InlineData("BASEPERIOD[sv]=\"1980\";\r\n", true)]
        [InlineData("CONTACT(\"value\")=\"xx\";\r\n", true)]
        [InlineData("CELLNOTE(\" * \",\" * \",\"Örebro\", \"1995\")=\"Lekebergs kommun has been excluded from Örebro\";\r\n", true)]
        [InlineData("CELLNOTEX(\" * \",\" * \",\"Örebro\", \"1995\")=\"Lekebergs kommun has been excluded from Örebro\";\r\n", true)]
        [InlineData("DATANOTE(\"tid\", \"2010\")=\"*\";\r\n", true)]
        [InlineData("REFPERIOD(\"value\")=\" \";\r\n", true)]
        [InlineData("VALUENOTE[sv](\"typ\",\"Inflyttade\")=\"Frivillig fotnot för inflyttade.\";\r\n", true)]
        [InlineData("VALUENOTEX(\"test double equals\")=\"Testing value containing an = character.\";\r\n", true)]
        [InlineData("UNITS(\"Deaths of Infants under One Year per 1,000 Live Births\")=\"Rate\";\r\n", true)]
        [InlineData("REFPERIOD(\"Deaths of Infants under One Year per 1,000 Live Births\")=\" \";\r\n", true)]
        [InlineData("BASEPERIOD(\"Deaths of Infants under 28 Days per 1,000 Live Births\")=\" \";\r\n", true)]

        public void TestMultilineParser(string input, bool expected)
        {
            Helper.SetupTests();
            PxManualParser parser = new PxManualParser(input);
            PxDocument pxDocument = parser.Parse();
            Assert.Equal(pxDocument != null, expected);
            Assert.Equal(1, pxDocument.Keywords.Count);
            PxKey key = pxDocument.Keywords[0].Key;
            var value = pxDocument.GetStringElementValue(key.Identifier);
            input = input.Replace("\"\r\n\"", " ").Replace("\r\n", "");
            input = input.Replace("\"", "");
            var keyString = key.ToPxString().Replace("\"", "");
            var valueString = value.ToString().Replace("\"", "");
            Assert.Equal(input, keyString + EQUALS + valueString + ";");
        }
        [Theory]
        [InlineData("TITLE=\"This is a title\";\r\n", true)]
        [InlineData("LAST-UPDATED(\"value\")=\"20200925 15:03\";\r\n", true)]
        public void TestDefaultParser(string input, bool expected)
        {
            Helper.SetupTests();
            PxManualParser parser = new PxManualParser(input);
            PxDocument pxDocument = parser.Parse();
            Assert.Equal(expected, pxDocument != null);
            Assert.Equal(1, pxDocument.Keywords.Count);
            input = input.Replace("\r\n", "");
            PxKey key = pxDocument.Keywords[0].Key;
            var value = pxDocument.GetStringElementValue(key.Identifier);
            Assert.Equal(input, key + EQUALS + "\"" + value + "\"" + ";");
        }




        [Fact]
        public void TestPreProcessInput()
        {
            Helper.SetupTests();
            string input = File.ReadAllText(Helper.GetResourceFolder() + "\\MIM04.px");
            int count = input.Length;
            PxManualParser parser = new PxManualParser(input);
            string result = parser.PreProcessInput(input);
            Assert.True(count == result.Length);
        }


    }
}
