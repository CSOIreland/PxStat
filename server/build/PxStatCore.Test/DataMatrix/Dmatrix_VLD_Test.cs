using Xunit;
using PxStat.Security;
using API;
using PxStat.Resources;
using PxStatCore.Test;
using PxStat.Workflow;
using PxStat.Data;
using PxStat.DBuild;
using DocumentFormat.OpenXml.Spreadsheet;
using Moq;
using PxStat.System.Settings;
using System.Data;
using System.Dynamic;

namespace PxStatXUnit.Tests
{
    [Collection("PxStatXUnit")]
    public class Dmatrix_VLD_Test
    {
        TestMatrixFactory _tmf;
        public Mock<IPathProvider> pathProvider;
        public Mock<IADO> ado;


        [Fact]
        public void HappyTest()
        {
            Helper.SetupTests();


            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            DMatrix_VLD vld = new DMatrix_VLD();
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Count() == 0);
        }

        [Fact]
        public void HappyTestSingleSpec()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            DMatrix_VLD vld = new DMatrix_VLD();
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Count() == 0);
        }


        [Fact]
        public void TestNoMtrCodeFail()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Code = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Equals("'Code' must not be empty.")).Count() > 0);

        }

        [Fact]
        public void TestNoSpecTitleFail()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Dspecs[matrix.Language].Title = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Equals($"No spec title for spec {matrix.Dspecs[matrix.Language].Language}")).Count() > 0);

        }

        [Fact]
        public void TestNoSpecContentsFieldFail()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Dspecs[matrix.Language].Contents = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Equals($"No spec Contents field for spec {matrix.Dspecs[matrix.Language].Language}")).Count() > 0);
        }

        [Fact]
        public void TestNoSpecTitleFailSingleSpec()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            matrix.Dspecs[matrix.Language].Title = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Equals($"No spec title for spec {matrix.Dspecs[matrix.Language].Language}")).Count() > 0);

        } //No Source field found for spec {x.Language}

        [Fact]
        public void TestNoSpecSourceFieldFail()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Dspecs[matrix.Language].Source = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Equals($"No Source field found for spec {matrix.Dspecs[matrix.Language].Language}")).Count() > 0);
        }

        [Fact]
        public void TestNoSpecSourceFieldSingleSpec()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            matrix.Dspecs[matrix.Language].Source = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Equals($"No Source field found for spec {matrix.Dspecs[matrix.Language].Language}")).Count() > 0);

        }

        [Fact]
        public void TestNoSpecContentsFieldFailSingleSpec()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            matrix.Dspecs[matrix.Language].Contents = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Equals($"No spec Contents field for spec {matrix.Dspecs[matrix.Language].Language}")).Count() > 0);
        }

        [Fact]
        public void TestEmptyCopyrightFail()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Copyright = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Equals("'Copyright' must not be empty.")).Count() > 0);
        }

        [Fact]
        public void TestLanguagesEmptyFail()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Languages = new List<string>();
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Equals("At least one language must exist")).Count() > 0);
        }

        [Fact]
        public void TestLanguagesMissingFail()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Languages = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Equals("No Language list found in the matrix")).Count() > 0);
        }

        [Fact]
        public void TestWrongCellCount()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Cells.Add(99.9);
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Actual and expected count of values do not match")).Count() > 0);
        }

        [Fact]
        public void TestSpecsEmpty()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Dspecs = new Dictionary<string, Dspec>();
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Equals("There must be at least 1 spec in the matrix")).Count() > 0);
        }

        [Fact]
        public void TestSpecsNull()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Dspecs = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Equals("'Dspecs' must not be empty.")).Count() > 0);
        }

        [Fact]
        public void TestDimensionCodeDupe()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Dspecs["en"].Dimensions.ToList()[0].Code = matrix.Dspecs["en"].Dimensions.ToList()[1].Code;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Error en spec has duplicate dimension codes")).Count() > 0);
        }

        [Fact]
        public void TestDimensionValueDupe()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Dspecs["en"].Dimensions.ToList()[0].Value = matrix.Dspecs["en"].Dimensions.ToList()[1].Value;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Error en spec has duplicate dimension values")).Count() > 0);
        }

        [Fact]
        public void TestWrongTimeDimensionCount()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Dspecs["en"].Dimensions.ToList()[0].Role = "TIME";
            matrix.Dspecs["en"].Dimensions.ToList()[1].Role = "TIME";
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Error en spec has a wrong time dimension count")).Count() > 0);
        }

        [Fact]
        public void TestWrongStatDimensionCount()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Dspecs["en"].Dimensions.ToList()[0].Role = "STATISTIC";
            matrix.Dspecs["en"].Dimensions.ToList()[1].Role = "STATISTIC";
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Error en spec has a wrong time dimension count")).Count() > 0);
        }

        [Fact]
        public void TestWrongClassificationDimensionCount()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            foreach (var dim in matrix.Dspecs["en"].Dimensions)
            {
                dim.Role = "TIME";
            }
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Error en spec has a wrong classification dimension count")).Count() > 0);
        }

        [Fact]
        public void TestInvalidDimensionSequencing()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Dspecs["en"].Dimensions.ToList()[1].Sequence = matrix.Dspecs["en"].Dimensions.ToList()[0].Sequence;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Error en spec has invalid dimension sequencing")).Count() > 0);
        }

        [Fact]
        public void TestInconsistentDimensionCodes()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Dspecs["en"].Dimensions.ToList()[0].Code = "9999999";
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Inconsistent dimension codes across specs")).Count() > 0);
        }

        [Fact]
        public void TestEmptyVariables()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Dspecs["en"].Dimensions.ToList()[0].Variables = new List<IDimensionVariable>();
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("No variables found for dimension")).Count() > 0);
        }
        //Missing variable set in dimension
        [Fact]
        public void TestMissingVariablesSet()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Dspecs["en"].Dimensions.ToList()[0].Variables = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Missing variable set in dimension")).Count() > 0);
        }

        [Fact]
        public void TestMissingRoleInDimension()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Dspecs["en"].Dimensions.ToList()[0].Role = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains($"Dimension {matrix.Dspecs["en"].Dimensions.ToList()[0].Code} has no Role assigned to it")).Count() > 0);
        }
        [Fact]
        public void TestDimensionVariablesAreOrdered()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Dspecs["en"].Dimensions.ToList()[1].Variables[0].Sequence = 1;
            matrix.Dspecs["en"].Dimensions.ToList()[1].Variables[1].Sequence = 1;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains($"Variable sequencing error for dimension {matrix.Dspecs["en"].Dimensions.ToList()[1].Code}")).Count() > 0);
        }

        [Fact]
        public void TestDimensionCodeIsNull()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Dspecs["en"].Dimensions.ToList()[1].Code = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Dimension Code must not be empty")).Count() > 0);
        }

        [Fact]
        public void TestDimensionValueIsNull()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Dspecs["en"].Dimensions.ToList()[1].Value = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Dimension Value must not be empty")).Count() > 0);
        }

        [Fact]
        public void TestGeoUrlWithGeoFlag()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Dspecs["en"].Dimensions.ToList()[2].GeoFlag = true;
            matrix.Dspecs["en"].Dimensions.ToList()[2].GeoUrl = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("A Geo url must be provided when the Geo flag is set")).Count() > 0);
        }

        [Fact]
        public void TestVariableCodeIsNull()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Dspecs["en"].Dimensions.ToList()[2].Variables[0].Code = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Variable missing a variable code")).Count() > 0);
        }

        [Fact]
        public void TestVariableValueIsNull()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Dspecs["en"].Dimensions.ToList()[2].Variables[0].Value = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Variable missing a variable value")).Count() > 0);
        }

        [Fact]
        public void TestSpecsAreEquivalent()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixMultiLanguage();
            matrix.Dspecs["en"].Dimensions.ToList()[2].Code = "88888";
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Inconsistent dimension codes across specs")).Count() > 0);
        }

        [Fact]
        public void TestNoMtrCodeFailSingleSpec()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            matrix.Code = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Equals("'Code' must not be empty.")).Count() > 0);
        }

        [Fact]
        public void TestEmptyCopyrightFailSingleSpec()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            matrix.Copyright = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Equals("'Copyright' must not be empty.")).Count() > 0);
        }

        [Fact]
        public void TestLanguagesEmptyFailSingleSpec()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            matrix.Languages = new List<string>();
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Equals("At least one language must exist")).Count() > 0);
        }

        [Fact]
        public void TestLanguagesMissingFailSingleSpec()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            matrix.Languages = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Equals("No Language list found in the matrix")).Count() > 0);
        }

        [Fact]
        public void TestWrongCellCountSingleSpec()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            matrix.Cells.Add(99.9);
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Actual and expected count of values do not match")).Count() > 0);
        }

        [Fact]
        public void TestSpecsEmptySingleSpec()
        {
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            matrix.Dspecs = new Dictionary<string, Dspec>();
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Equals("There must be at least 1 spec in the matrix")).Count() > 0);
        }

        [Fact]
        public void TestSpecsNullSingleSpec()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            matrix.Dspecs = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Equals("'Dspecs' must not be empty.")).Count() > 0);
        }

        [Fact]
        public void TestDimensionCodeDupeSingleSpec()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            matrix.Dspecs["en"].Dimensions.ToList()[0].Code = matrix.Dspecs["en"].Dimensions.ToList()[1].Code;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Error en spec has duplicate dimension codes")).Count() > 0);
        }

        [Fact]
        public void TestDimensionValueDupeSingleSpec()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            matrix.Dspecs["en"].Dimensions.ToList()[0].Value = matrix.Dspecs["en"].Dimensions.ToList()[1].Value;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Error en spec has duplicate dimension values")).Count() > 0);
        }

        [Fact]
        public void TestWrongTimeDimensionCountSingleSpec()
        {
            Helper.SetupTests();

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            matrix.Dspecs["en"].Dimensions.ToList()[0].Role = "TIME";
            matrix.Dspecs["en"].Dimensions.ToList()[1].Role = "TIME";
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Error en spec has a wrong time dimension count")).Count() > 0);
        }

        [Fact]
        public void TestWrongStatDimensionCountSingleSpec()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            matrix.Dspecs["en"].Dimensions.ToList()[0].Role = "STATISTIC";
            matrix.Dspecs["en"].Dimensions.ToList()[1].Role = "STATISTIC";
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Error en spec has a wrong time dimension count")).Count() > 0);
        }

        [Fact]
        public void TestWrongClassificationDimensionCountSingleSpec()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            foreach (var dim in matrix.Dspecs["en"].Dimensions)
            {
                dim.Role = "TIME";
            }
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Error en spec has a wrong classification dimension count")).Count() > 0);
        }

        [Fact]
        public void TestInvalidDimensionSequencingSingleSpec()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            matrix.Dspecs["en"].Dimensions.ToList()[1].Sequence = matrix.Dspecs["en"].Dimensions.ToList()[0].Sequence;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Error en spec has invalid dimension sequencing")).Count() > 0);
        }



        [Fact]
        public void TestEmptyVariablesSingleSpec()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            matrix.Dspecs["en"].Dimensions.ToList()[0].Variables = new List<IDimensionVariable>();
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("No variables found for dimension")).Count() > 0);
        }
        //Missing variable set in dimension
        [Fact]
        public void TestMissingVariablesSetSingleSpec()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            matrix.Dspecs["en"].Dimensions.ToList()[0].Variables = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Missing variable set in dimension")).Count() > 0);
        }

        [Fact]
        public void TestMissingRoleInDimensionSingleSpec()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            matrix.Dspecs["en"].Dimensions.ToList()[0].Role = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains($"Dimension {matrix.Dspecs["en"].Dimensions.ToList()[0].Code} has no Role assigned to it")).Count() > 0);
        }
        [Fact]
        public void TestDimensionVariablesAreOrderedSingleSpec()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            matrix.Dspecs["en"].Dimensions.ToList()[1].Variables[0].Sequence = 1;
            matrix.Dspecs["en"].Dimensions.ToList()[1].Variables[1].Sequence = 1;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains($"Variable sequencing error for dimension {matrix.Dspecs["en"].Dimensions.ToList()[1].Code}")).Count() > 0);
        }

        [Fact]
        public void TestDimensionCodeIsNullSingleSpec()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            matrix.Dspecs["en"].Dimensions.ToList()[1].Code = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Dimension Code must not be empty")).Count() > 0);
        }

        [Fact]
        public void TestDimensionValueIsNullSingleSpec()
        {
            Helper.SetupTests();

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            matrix.Dspecs["en"].Dimensions.ToList()[1].Value = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Dimension Value must not be empty")).Count() > 0);
        }

        [Fact]
        public void TestGeoUrlWithGeoFlagSingleSpec()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            matrix.Dspecs["en"].Dimensions.ToList()[2].GeoFlag = true;
            matrix.Dspecs["en"].Dimensions.ToList()[2].GeoUrl = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("A Geo url must be provided when the Geo flag is set")).Count() > 0);
        }

        [Fact]
        public void TestVariableCodeIsNullSingleSpec()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            matrix.Dspecs["en"].Dimensions.ToList()[2].Variables[0].Code = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Variable missing a variable code")).Count() > 0);
        }

        [Fact]
        public void TestVariableValueIsNullSingleSpec()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            _tmf = new TestMatrixFactory();
            Dmatrix matrix = _tmf.GetGoodMatrixSingleSpec();
            matrix.Dspecs["en"].Dimensions.ToList()[2].Variables[0].Value = null;
            DMatrix_VLD vld = new DMatrix_VLD();
            
            var result = vld.Validate(matrix);
            Assert.True(result.Errors.Where(x => x.ErrorMessage.Contains("Variable missing a variable value")).Count() > 0);
        }
    }
}
