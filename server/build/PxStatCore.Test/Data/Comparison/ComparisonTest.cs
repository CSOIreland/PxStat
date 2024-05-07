using API;
using PxStat.Resources;
using PxStatCore.Test;
using PxStat.Workflow;
using Moq;
using PxStat.Data;
using PxStat.System.Settings;
using DocumentFormat.OpenXml.Spreadsheet;

namespace PxStatXUnit.Tests
{
    [Collection ("PxStatXUnit")]
    public class ComparisonTest
    {
        [Fact]
        public void Amendment_Duplicate_Matrixes()
        {
            Helper.SetupTests();

            var en = LanguageManager.GetLanguage("en");

            DCompare_BSO bso = new DCompare_BSO();

            TestMatrixFactory tmf = new TestMatrixFactory();

            IDmatrix dmleft = tmf.GetGoodMatrixSingleSpec();
            IDmatrix dmright = tmf.GetGoodMatrixSingleSpec();
            IDmatrix dmleftRef = tmf.GetGoodMatrixSingleSpec();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            en = LanguageManager.GetLanguage("en");

            IDmatrix result = bso.CompareDmatrixAmendment(dmleft, dmright, dmleftRef);

            Assert.True(result.ComparisonReport != null);
            Assert.True(result.ComparisonReport.Where(x => x == true).ToList().Count == 0);
        }

        [Fact]
        public void Amendment_Duplicate_MatrixesMultiLanguage()
        {
            Helper.SetupTests();

            DCompare_BSO bso = new DCompare_BSO();

            TestMatrixFactory tmf = new TestMatrixFactory();

            IDmatrix dmleft = tmf.GetGoodMatrixMultiLanguage();
            IDmatrix dmright = tmf.GetGoodMatrixMultiLanguage();
            IDmatrix dmleftRef = tmf.GetGoodMatrixMultiLanguage();

           

            IDmatrix result = bso.CompareDmatrixAmendment(dmleft, dmright, dmleftRef);


            Assert.True(result.ComparisonReport != null);
            Assert.True(result.ComparisonReport.Where(x => x == true).ToList().Count == 0);
            
        }

        [Fact]
        public void Amendment_StandardAmendments_Matrixes()
        {
            Helper.SetupTests();

            DCompare_BSO bso = new DCompare_BSO();

            TestMatrixFactory tmf = new TestMatrixFactory();

            IDmatrix dmleft = tmf.GetGoodMatrixSingleSpec();
            IDmatrix dmright = tmf.GetGoodMatrixSingleSpecModifiedData();
            IDmatrix dmleftRef = tmf.GetGoodMatrixSingleSpec();

            

            IDmatrix result = bso.CompareDmatrixAmendment(dmleft, dmright, dmleftRef);


            Assert.True(result.ComparisonReport != null);
            Assert.True(result.ComparisonReport.Where(x => x == true).ToList().Count == 2);
            Assert.True(result.ComparisonReport[3] == true);
            Assert.True(result.ComparisonReport[11] == true);
        }

        [Fact]
        public void Addition_NoAddition()
        {
            Helper.SetupTests();

            DCompare_BSO bso = new DCompare_BSO();

            TestMatrixFactory tmf = new TestMatrixFactory();

            IDmatrix dmleft = tmf.GetGoodMatrixSingleSpec();
            IDmatrix dmright = tmf.GetGoodMatrixSingleSpec();

            IDmatrix result = bso.CompareDmatrixAddDelete(dmleft, dmright, "en");


            Assert.True(result.ComparisonReport != null);
            Assert.True(result.ComparisonReport.Where(x => x == true).ToList().Count == 0);
        }

        [Fact]
        public void Addition_AddPeriod()
        {
            Helper.SetupTests();

            DCompare_BSO bso = new DCompare_BSO();

            TestMatrixFactory tmf = new TestMatrixFactory();

            IDmatrix dmleft = tmf.GetGoodMatrixSingleSpecAddedPeriodPlusData();
            IDmatrix dmright = tmf.GetGoodMatrixSingleSpec();

            IDmatrix result = bso.CompareDmatrixAddDelete(dmleft, dmright, "en");


            Assert.True(result.ComparisonReport != null);
            Assert.True(result.ComparisonReport.Where(x => x == true).ToList().Count == 9);
            Assert.True(result.ComparisonReport[18]);
            Assert.False(result.ComparisonReport[17]);
        }

        [Fact]
        public void Subtraction_RemovePeriod()
        {
            Helper.SetupTests();

            DCompare_BSO bso = new DCompare_BSO();

            TestMatrixFactory tmf = new TestMatrixFactory();

            IDmatrix dmleft = tmf.GetGoodMatrixSingleSpecAddedPeriodPlusData();
            IDmatrix dmright = tmf.GetGoodMatrixSingleSpec();

            IDmatrix result = bso.CompareDmatrixAddDelete(dmleft, dmright, "en");


            Assert.True(result.ComparisonReport != null);
            Assert.True(result.ComparisonReport.Where(x => x == true).ToList().Count == 9);
            Assert.True(result.ComparisonReport[18]);
            Assert.False(result.ComparisonReport[17]);
        }

        [Fact]
        public void AmendmentWithAdditionOnly()
        {
            Helper.SetupTests();

            DCompare_BSO bso = new DCompare_BSO();

            TestMatrixFactory tmf = new TestMatrixFactory();

            IDmatrix dmleft = tmf.GetGoodMatrixSingleSpecAddedPeriodPlusData();
            IDmatrix dmright = tmf.GetGoodMatrixSingleSpec();
            IDmatrix dmleftRef = tmf.GetGoodMatrixSingleSpecAddedPeriodPlusData();

            

            IDmatrix result = bso.CompareDmatrixAmendment(dmleft, dmright, dmleftRef);


            Assert.True(result.ComparisonReport != null);
            Assert.True(result.ComparisonReport.Where(x => x == true).ToList().Count == 0);

        }

        [Fact]
        public void AmendmentWithNoNewMetadata()
        {
            Helper.SetupTests();

            DCompare_BSO bso = new DCompare_BSO();

            TestMatrixFactory tmf = new TestMatrixFactory();

            IDmatrix dmleft = tmf.GetGoodMatrixSingleSpecAddedPeriodPlusData();
            IDmatrix dmright = tmf.GetGoodMatrixSingleSpec();
            IDmatrix dmleftRef = tmf.GetGoodMatrixSingleSpecAddedPeriodPlusData();

            int counter = 0;
            double newval = 100;
            List<dynamic> newCells =new List<dynamic>();
            foreach(var cell in dmright.Cells)
            {
                if (counter == 3 || counter == 5 || counter == 12 | counter == 15)
                {
                    newCells.Add(newval);
                    newval = newval - 1;
                }
                else newCells.Add(cell);
                counter++;
            }
            dmright.Cells = newCells;

            IDmatrix result = bso.CompareDmatrixAmendment(dmleft, dmright, dmleftRef);


            Assert.True(result.ComparisonReport != null);
            Assert.True(result.ComparisonReport.Where(x => x == true).ToList().Count == 4);
            Assert.True(result.ComparisonReport.ToList()[3]);
            Assert.True(result.ComparisonReport.ToList()[5]);
            Assert.True(result.ComparisonReport.ToList()[12]);
            Assert.True(result.ComparisonReport.ToList()[15]);
            Assert.False(result.ComparisonReport.ToList()[2]);
            Assert.False(result.ComparisonReport.ToList()[4]);
        }

        [Fact]
        public void AmendmentWithSubtractionOnly()
        {
            Helper.SetupTests();

            DCompare_BSO bso = new DCompare_BSO();

            TestMatrixFactory tmf = new TestMatrixFactory();

            IDmatrix dmright = tmf.GetGoodMatrixSingleSpecAddedPeriodPlusData();
            IDmatrix dmleft = tmf.GetGoodMatrixSingleSpec();
            IDmatrix dmleftRef = tmf.GetGoodMatrixSingleSpec();


            IDmatrix result = bso.CompareDmatrixAmendment(dmleft, dmright, dmleftRef);


            Assert.True(result.ComparisonReport != null);
            Assert.True(result.ComparisonReport.Where(x => x == true).ToList().Count == 0);

        }


        [Fact]
        public void Addition_AddPeriodMultiLanguage()
        {
            Helper.SetupTests();

            DCompare_BSO bso = new DCompare_BSO();

            TestMatrixFactory tmf = new TestMatrixFactory();

            IDmatrix dmleft = tmf.GetGoodMatrixMultiLanguageWithAddedPeriodAndData();
            IDmatrix dmright = tmf.GetGoodMatrixMultiLanguage();

            IDmatrix result = bso.CompareDmatrixAddDelete(dmleft, dmright, "en");


            Assert.True(result.ComparisonReport != null);
            Assert.True(result.ComparisonReport.Where(x => x == true).ToList().Count == 9);
            Assert.True(result.ComparisonReport[18]);
            Assert.False(result.ComparisonReport[17]);
        }
    }
}
