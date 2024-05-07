using API;
using Moq;
using Xunit;
using PxStat.Config;
using PxStat.Report;
using PxStatCore.Test;
using System.Collections.Generic;
using System.Dynamic;
using PxStat;
using DeviceDetectorNET.Cache;
using PxStat.Resources;

namespace PxStatCore.Test.BBCode
{
    [Collection("PxStatXUnit")]
    public class BBCodeTagReader_Test
    {
        [Fact]
        public void TestBasicBold()
        {
            Helper.SetupTests();
            BBCodeTagReader btr = new BBCodeTagReader("This is <b>a simple</b> read");
            btr.parseTag("<b>", "</b>");
            Assert.True(btr.Positions.Count == 1);
            Assert.True(btr.Positions[0].Start == 8);
            Assert.True(btr.Positions[0].End == 19);
            btr.ValidateResult();
            Assert.True(btr.IsValid);
        }

        [Fact]
        public void TestBasicBoldNegativeEn()
        {
            Helper.SetupTests();
            BBCodeTagReader btr = new BBCodeTagReader("This is <b>a simple read","en");
            btr.parseTag("<b>", "</b>");
            Assert.False(btr.IsValid);
        }

        [Fact]
        public void TestBasicBoldNegativeGa()
        {
            Helper.SetupTests();
            BBCodeTagReader btr = new BBCodeTagReader("This is <b>a simple read","ga");
            btr.parseTag("<b>", "</b>");
            Assert.False(btr.IsValid);
        }

        [Fact]
        public void TestBasicBoldNoOpeningNegativeEn()
        {
            Helper.SetupTests();
            BBCodeTagReader btr = new BBCodeTagReader("This is </b>a simple read", "en");
            btr.parseTag("<b>", "</b>");
            Assert.False(btr.IsValid);
        }

        [Fact]
        public void TestBasicBoldNoOpeningNegativeGa()
        {
            Helper.SetupTests();
            BBCodeTagReader btr = new BBCodeTagReader("This is </b>a simple read", "ga");
            btr.parseTag("<b>", "</b>");
            Assert.False(btr.IsValid);
        }

        [Fact]
        public void TestEmptyString()
        {
            Helper.SetupTests();
            BBCodeTagReader btr = new BBCodeTagReader("");
            btr.parseTag("<b>", "</b>");
            btr.parseTag("<u>", "</u>");
            Assert.True(btr.Positions.Count() == 0);
            Assert.True(btr.IsValid);
        }


        [Fact]
        public void TestDoubleBold()
        {
            Helper.SetupTests();
            BBCodeTagReader btr = new BBCodeTagReader("This is <b>a simple</b> read with <b>two</b> bold items ");
            btr.parseTag("<b>", "</b>");
            Assert.True(btr.Positions.Count == 2);
            Assert.True(btr.Positions[0].Start == 8);
            Assert.True(btr.Positions[0].End == 19);
            Assert.True(btr.Positions[1].Start == 34);
            Assert.True(btr.Positions[1].End == 40);
            btr.ValidateResult();
            Assert.True(btr.IsValid);
        }

        [Fact]
        public void TestDoubleBoldNegativeEn()
        {
            Helper.SetupTests();
            BBCodeTagReader btr = new BBCodeTagReader("This is <b>a simple read with <b>two</b> consecutive bold start items ","en");
            btr.parseTag("<b>", "</b>");
            Assert.False(btr.IsValid);
        }

        [Fact]
        public void TestDoubleBoldNegativeGa()
        {
            Helper.SetupTests();
            BBCodeTagReader btr = new BBCodeTagReader("This is <b>a simple read with <b>two</b> consecutive bold start items ", "ga");
            btr.parseTag("<b>", "</b>");
            Assert.False(btr.IsValid);
        }

        [Fact]
        public void TestDoubleBoldCloseNegativeEn()
        {
            Helper.SetupTests();
            BBCodeTagReader btr = new BBCodeTagReader("This is <b>a simple read with </b>two</b> consecutive bold end items ","en");
            btr.parseTag("<b>", "</b>");
            Assert.False(btr.IsValid);
        }

        [Fact]
        public void TestDoubleBoldCloseNegativeGa()
        {
            Helper.SetupTests();
            BBCodeTagReader btr = new BBCodeTagReader("This is <b>a simple read with </b>two</b> consecutive bold end items ","ga");
            btr.parseTag("<b>", "</b>");
            Assert.False(btr.IsValid);
        }

        [Fact]
        public void TestStartsWithClosingTagNegativeEn()
        {
            Helper.SetupTests();
            BBCodeTagReader btr = new BBCodeTagReader("This is </b>a simple read that <b>two</b> starts with a closing tag", "en" );
            btr.parseTag("<b>", "</b>");
            Assert.False(btr.IsValid);
        }

        [Fact]
        public void TestStartsWithClosingTagNegativeGa()
        {
            Helper.SetupTests();
            BBCodeTagReader btr = new BBCodeTagReader("This is </b>a simple read that <b>two</b> starts with a closing tag","ga");
            btr.parseTag("<b>", "</b>");
            Assert.False(btr.IsValid);
        }

        [Fact]
        public void TestBasicBoldEdgeEnds()
        {
            Helper.SetupTests();
            BBCodeTagReader btr = new BBCodeTagReader("<b>This is a simple read</b>");
            btr.parseTag("<b>", "</b>");
            Assert.True(btr.Positions.Count == 1);
            Assert.True(btr.Positions[0].Start == 0);
            Assert.True(btr.Positions[0].End == 24);
            btr.ValidateResult();
            Assert.True(btr.IsValid);
        }

        [Fact]
        public void TestBasicBoldEdgeEndsNegativeEn()
        {
            Helper.SetupTests();
            BBCodeTagReader btr = new BBCodeTagReader("</b>This is a simple read<b>", "en"  );
            btr.parseTag("<b>", "</b>");
            Assert.False(btr.IsValid);
        }

        [Fact]
        public void TestBasicBoldEdgeEndsNegativeGa()
        {
            Helper.SetupTests();
            BBCodeTagReader btr = new BBCodeTagReader("</b>This is a simple read<b>","ga");
            btr.parseTag("<b>", "</b>");
            Assert.False(btr.IsValid);
        }

        [Fact]
        public void TestNotFound()
        {
            Helper.SetupTests();
            BBCodeTagReader btr = new BBCodeTagReader("This is <b>a simple</b> read with <b>two</b> bold items");
            btr.parseTag("<u>", "</u>");
            Assert.True(btr.Positions.Count == 0);
            btr.ValidateResult();
            Assert.True(btr.IsValid);
        }
        [Fact]
        public void TestDoubleParseBasic()
        {
            Helper.SetupTests();
            BBCodeTagReader btr = new BBCodeTagReader("This is <b>a simple</b> read with <u>two</u> different items ");
            btr.parseTag("<b>", "</b>");
            btr.parseTag("<u>","</u>");
            Assert.True(btr.Positions.Count == 2);
            Assert.True(btr.Positions[0].Start == 8);
            Assert.True(btr.Positions[0].End == 19);
            Assert.True(btr.Positions[0].StartTag.Equals("<b>"));
            Assert.True(btr.Positions[0].EndTag.Equals("</b>"));
            Assert.True(btr.Positions[1].Start == 34);
            Assert.True(btr.Positions[1].End == 40);
            Assert.True(btr.Positions[1].StartTag.Equals("<u>"));
            Assert.True(btr.Positions[1].EndTag.Equals("</u>"));
            Assert.True(btr.IsValid);
            btr.RemoveTags();
            Assert.True(btr.Positions.Count == 2);
            Assert.True(btr.Positions[0].Start == 8);
            Assert.True(btr.Positions[0].End == 16);
            Assert.True(btr.Positions[0].StartTag.Equals("<b>"));
            Assert.True(btr.Positions[0].EndTag.Equals("</b>"));
            Assert.True(btr.Positions[1].Start == 27);
            Assert.True(btr.Positions[1].End == 30);
            Assert.True(btr.Positions[1].StartTag.Equals("<u>"));
            Assert.True(btr.Positions[1].EndTag.Equals("</u>"));
            btr.ValidateResult();
            Assert.True(btr.IsValid);

        }

        [Fact]
        public void TestDoubleParseNested()
        {
            Helper.SetupTests();
            BBCodeTagReader btr = new BBCodeTagReader("This is <b>a simple<u> read with </u>two</b> different items ");
            btr.parseTag("<b>", "</b>");
            btr.parseTag("<u>", "</u>");

            Assert.True(btr.IsValid);
            btr.RemoveTags();
            btr.ValidateResult();
            Assert.True(btr.IsValid);

        }


        [Fact]
        public void TestDoubleParseOverlapNegativeEn()
        {
            Helper.SetupTests();
            BBCodeTagReader btr = new BBCodeTagReader("This is <b>a simple<u> read with </b>two</u> different items ","en");
            btr.parseTag("<b>", "</b>");
            btr.parseTag("<u>", "</u>");
            
            Assert.True(btr.IsValid);
            btr.RemoveTags();
            btr.ValidateResult();
            Assert.False(btr.IsValid);

        }

        [Fact]
        public void TestDoubleParseOverlapNegativeGa()
        {
            Helper.SetupTests();
            BBCodeTagReader btr = new BBCodeTagReader("This is <b>a simple<u> read with </b>two</u> different items ","ga");
            btr.parseTag("<b>", "</b>");
            btr.parseTag("<u>", "</u>");

            Assert.True(btr.IsValid);
            btr.RemoveTags();
            btr.ValidateResult();
            Assert.False(btr.IsValid);

        }
    }
}
