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
    public class TestCsvDynamicObjects
    {
        [Fact]
        public void TestAddNewPeriod()
        {
            Helper.SetupTests();

            TestMatrixFactory tmf = new TestMatrixFactory();
            List<List<string>> inputList = tmf.GetCsvAsListNewPeriod();
            DBuild_BSO bid = new DBuild_BSO();
            List<Dictionary<string, object>> dlist = bid.GetInputDynamicList(inputList);

            Dmatrix matrix = tmf.GetGoodMatrixSingleSpec();

            StatDimension tdim = matrix.Dspecs["en"].Dimensions.Where(x => x.Role.Equals("TIME")).FirstOrDefault();
            tdim.Variables.Add(new DimensionVariable { Code = "detv3_code", Value = "detv3_value", Sequence = tdim.Variables.Count + 1 });

            matrix.Dspecs["en"] = (Dspec)bid.LoadDspec(matrix);

            matrix.Dspecs["en"].GetDimensionsAsDictionary();

            Dictionary<int, object> dict = bid.DictionaryWithOrdinal(matrix, dlist, "en");

            Assert.True(dlist.Count == 9);

            Assert.True(dlist[6]["VALUE"].Equals("7"));
            Assert.True((double)dict[19] == 1);
            Assert.True((double)dict[20] == 2);
            Assert.True((double)dict[21] == 3);

        }

        [Fact]
        public void TestUpdateMetadataFromDtoPopulateDTO()
        {
            Helper.SetupTests();

            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.GetDTOBuildParameters();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            Assert.True(dto.Dspecs.Count == 1);
            Assert.True(dto.Dspecs[0].StatDimensions.Count == 2);
            Assert.True(dto.MtrInput == "abcde");
            Assert.True(dto.FrqCodeTimeval == "TLIST(M1)");
            Assert.True(dto.FrqValueTimeval == "Month");
            Assert.True(dto.MtrCode == "MIM04");
            Assert.True(dto.Format.FrmVersion == "2013");
            Assert.True(dto.Format.FrmType == "PX");
            Assert.True(dto.MtrOfficialFlag);
            Assert.True(dto.CprCode == "CSO");

            Assert.True(dto.Dspecs[0].Language == "en");
            Assert.True(dto.Dspecs[0].MtrTitle == "Industrial Production Volume and Turnover Indices (Base 2015=100) by Industry Sector NACE Rev 2, statistical indicator and Month");
            Assert.True(dto.Dspecs[0].MtrNote == "This is a note");

            var dimensions = dto.Dspecs[0].StatDimensions.ToList();
            Assert.True(dimensions[0].Code == "STATISTIC");
            Assert.True(dimensions[0].Role == "STATISTIC");

            Assert.True(dimensions[1].Value == "Month");
            Assert.True(dimensions[1].Role == "TIME");
            Assert.True(dimensions[1].Variables.Count == 1);

            Assert.True(dimensions[1].Variables[0].Code == "2020100");
            Assert.True(dimensions[1].Variables[0].Value == "2020M100");


            Assert.True(dto.Elimination.ContainsKey("NACE2"));
            Assert.True(dto.Map.ContainsKey("NACE2"));



        }


        [Fact]
        public void TestFullChangeRequestOneLanguageOneAddedPeriod()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.GetFullRequest();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IDmatrix dmatrix = new Dmatrix();

            DBuild_BSO bid = new DBuild_BSO();
            var dlist = bid.GetInputDynamicList(dto.ChangeData);

            // Get PxDocument from dto
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(dto.MtrInput);
            var pxDocument = pxManualParser.Parse();
            IUpload_DTO uDto = new PxUpload_DTO();

            dmatrix = dmatrix.GetDmatrixFromPxDocument(pxDocument,  uDto);
           
            DMatrix_VLD validator = new DMatrix_VLD();
            var result = validator.Validate(dmatrix);
            if (!result.IsValid)
            {
                var errors = result.Errors;
            }

            Assert.True(result.IsValid);

            var updateOrdinals = bid.UpdateMatrixWithNewMetadata(ref dmatrix, dto,  moqCpr.Object);

            dmatrix.Cells = bid.MergeOldAndNewCells(dmatrix, updateOrdinals);


            result = validator.Validate(dmatrix);

            Assert.True(result.IsValid);

            Assert.True(dmatrix.Cells.ToList()[27].Equals(".."));
            Assert.True(dmatrix.Cells.ToList()[29].Equals(".."));
            Assert.True(dmatrix.Cells.ToList()[12].Equals(".."));
            Assert.True(dmatrix.Cells.ToList()[14].Equals(".."));
            Assert.True(dmatrix.Cells.ToList()[26].Equals(157.7));
            Assert.True(dmatrix.Cells.ToList()[11].Equals(151.1));


        }

        [Fact]
        public void TestFullChangeRequestOneLanguageInsertedAndAddedPeriods()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.GetFullRequestInsertedAndAddedPeriods();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IDmatrix dmatrix = new Dmatrix();

            DBuild_BSO bid = new DBuild_BSO();
            var dlist = bid.GetInputDynamicList(dto.ChangeData);

            // Get PxDocument from dto
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(dto.MtrInput);
            var pxDocument = pxManualParser.Parse();

            IUpload_DTO uDto = new PxUpload_DTO();

            dmatrix = dmatrix.GetDmatrixFromPxDocument(pxDocument,  uDto);
           
            DMatrix_VLD validator = new DMatrix_VLD();
            var result = validator.Validate(dmatrix);

            Assert.True(result.IsValid);

            var updateOrdinals = bid.UpdateMatrixWithNewMetadata(ref dmatrix, dto, moqCpr.Object);

            dmatrix.Cells = bid.MergeOldAndNewCells(dmatrix, updateOrdinals);


            result = validator.Validate(dmatrix);

            Assert.True(result.IsValid);

            Assert.True(dmatrix.Cells.ToList()[6].Equals(".."));
            Assert.True(dmatrix.Cells.ToList()[8].Equals(".."));
            Assert.True(dmatrix.Cells.ToList()[15].Equals(".."));
            Assert.True(dmatrix.Cells.ToList()[17].Equals(".."));
            Assert.True(dmatrix.Cells.ToList()[24].Equals(".."));
            Assert.True(dmatrix.Cells.ToList()[26].Equals(".."));
            Assert.True(dmatrix.Cells.ToList()[33].Equals(".."));
            Assert.True(dmatrix.Cells.ToList()[35].Equals(".."));
            Assert.True(dmatrix.Cells.ToList()[32].Equals(157.7));
            Assert.True(dmatrix.Cells.ToList()[14].Equals(151.1));

        }

        [Fact]
        public void TestFullChangeRequestTwoLanguagesAddedPeriod()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.GetFullRequestAddedPeriodTwoLanguages();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IDmatrix dmatrix = new Dmatrix();

            DBuild_BSO bid = new DBuild_BSO();
            var dlist = bid.GetInputDynamicList(dto.ChangeData);

            // Get PxDocument from dto
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(dto.MtrInput);
            var pxDocument = pxManualParser.Parse();

            IUpload_DTO uDto = new PxUpload_DTO() { FrqValueTimeval = dto.FrqValueTimeval, LngIsoCode = "en", FrqCodeTimeval = dto.FrqCodeTimeval };

            dmatrix = dmatrix.GetDmatrixFromPxDocument(pxDocument,  uDto);
           
            DMatrix_VLD validator = new DMatrix_VLD();
            var result = validator.Validate(dmatrix);

            Assert.True(result.IsValid);

            var updateOrdinals = bid.UpdateMatrixWithNewMetadata(ref dmatrix, dto, moqCpr.Object);

            dmatrix.Cells = bid.MergeOldAndNewCells(dmatrix, updateOrdinals);


            result = validator.Validate(dmatrix);

            Assert.True(result.IsValid);

            Assert.True(dmatrix.Cells.ToList()[3].Equals(2272691));
            Assert.True(dmatrix.Cells.ToList()[7].Equals(2354422));
            Assert.True(dmatrix.Cells.ToList()[12].Equals(2315551));
            Assert.True(dmatrix.Cells.ToList()[16].Equals(2407433));
            Assert.True(dmatrix.Cells.ToList()[2].Equals(".."));
            Assert.True(dmatrix.Cells.ToList()[5].Equals(".."));
            Assert.True(dmatrix.Cells.ToList()[0].Equals(2272699));
            Assert.True(dmatrix.Cells.ToList()[14].Equals(".."));

        }

        [Fact]
        public void TestAmendedNonVariableMetadata()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.GetAmendedNonVariableMetadata();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IDmatrix dmatrix = new Dmatrix();

            DBuild_BSO bid = new DBuild_BSO();
            var dlist = bid.GetInputDynamicList(dto.ChangeData);

            // Get PxDocument from dto
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(dto.MtrInput);
            var pxDocument = pxManualParser.Parse();

            IUpload_DTO uDto = new PxUpload_DTO() { FrqValueTimeval = dto.FrqValueTimeval, LngIsoCode = "en", FrqCodeTimeval = dto.FrqCodeTimeval };

            dmatrix = dmatrix.GetDmatrixFromPxDocument(pxDocument, uDto);
            DMatrix_VLD validator = new DMatrix_VLD();
            var result = validator.Validate(dmatrix);

            Assert.True(result.IsValid);

            var updateOrdinals = bid.UpdateMatrixWithNewMetadata(ref dmatrix, dto,  moqCpr.Object);

            dmatrix.Cells = bid.MergeOldAndNewCells(dmatrix, updateOrdinals);


            result = validator.Validate(dmatrix);

            Assert.True(result.IsValid);
            Assert.True(dmatrix.Code.Equals("MIM04x"));
            Assert.True(dmatrix.Copyright.CprValue.Equals("Department of Health and Children"));
            Assert.False(dmatrix.IsOfficialStatistic);
            Assert.True(dmatrix.Dspecs["en"].Title.Equals("Amended Industrial Production Volume and Turnover Indices (Base 2015=100) by Industry Sector NACE Rev 2, statistical indicator and Month"));
            Assert.True(dmatrix.Dspecs["en"].NotesAsString.Equals("This is NOT a note"));
            Assert.True(dmatrix.Dspecs["en"].Dimensions.Where(x => x.Role.Equals("STATISTIC")).First().Value.Equals("STATISTICxx"));
        }
        [Fact]
        public void TestAmendedNonVariableMetadataTwoLanguages()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.GetAmendedNonVariableMetadataTwoLanguages();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IDmatrix dmatrix = new Dmatrix();

            DBuild_BSO bid = new DBuild_BSO();
            var dlist = bid.GetInputDynamicList(dto.ChangeData);

            // Get PxDocument from dto
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(dto.MtrInput);
            var pxDocument = pxManualParser.Parse();

            IUpload_DTO uDto = new PxUpload_DTO() { FrqValueTimeval = dto.FrqValueTimeval, LngIsoCode = "en", FrqCodeTimeval = dto.FrqCodeTimeval }; ;

            dmatrix = dmatrix.GetDmatrixFromPxDocument(pxDocument,  uDto);
            DMatrix_VLD validator = new DMatrix_VLD();
            var result = validator.Validate(dmatrix);

            Assert.True(result.IsValid);

            var updateOrdinals = bid.UpdateMatrixWithNewMetadata(ref dmatrix, dto,  moqCpr.Object);

            dmatrix.Cells = bid.MergeOldAndNewCells(dmatrix, updateOrdinals);


            result = validator.Validate(dmatrix);

            Assert.True(result.IsValid);
            Assert.True(dmatrix.Code.Equals("E3002Yx"));
            Assert.True(dmatrix.Copyright.CprValue.Equals("Department of Health and Children"));
            Assert.True(dmatrix.IsOfficialStatistic);
            Assert.True(dmatrix.Dspecs["en"].Title.Equals("Amended Industrial Production Volume and Turnover Indices (Base 2015=100) by Industry Sector NACE Rev 2, statistical indicator and Month"));
            Assert.True(dmatrix.Dspecs["en"].NotesAsString.Equals("This is NOT a note"));
            Assert.True(dmatrix.Dspecs["en"].Dimensions.Where(x => x.Role.Equals("STATISTIC")).First().Value.Equals("STATISTICyy"));

            Assert.True(dmatrix.Dspecs["ga"].Title.Equals("Rud eile as gaeilge"));
            Assert.True(dmatrix.Dspecs["ga"].NotesAsString.Equals("Níl nóta é seo in aon chor"));
            Assert.True(dmatrix.Dspecs["ga"].Dimensions.Where(x => x.Role.Equals("STATISTIC")).First().Value.Equals("STATISTICyy"));


        }
        

        [Fact]
        public void TestValidateMatrixFromPxDoc()
        {
            Helper.SetupTests();

            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.GetFullRequest();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IDmatrix dmatrix = new Dmatrix();

            // Get PxDocument from dto
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(dto.MtrInput);
            var pxDocument = pxManualParser.Parse();

            IUpload_DTO uDto = new PxUpload_DTO();

            dmatrix = dmatrix.GetDmatrixFromPxDocument(pxDocument,  uDto);
           
            DMatrix_VLD validator = new DMatrix_VLD();
            var result = validator.Validate(dmatrix);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void TestAmendRequestDataGoodRequest()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.AmendDataSingleLanguageOk();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IDmatrix dmatrix = new Dmatrix();

            DBuild_BSO bid = new DBuild_BSO();
            var dlist = bid.GetInputDynamicList(dto.ChangeData);

            // Get PxDocument from dto
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(dto.MtrInput);
            var pxDocument = pxManualParser.Parse();

            IUpload_DTO uDto = new PxUpload_DTO() { FrqValueTimeval = dto.FrqValueTimeval, LngIsoCode = "en", FrqCodeTimeval = dto.FrqCodeTimeval };

            dmatrix = dmatrix.GetDmatrixFromPxDocument(pxDocument,  uDto);
            DMatrix_VLD validator = new DMatrix_VLD();
            var result = validator.Validate(dmatrix);

            Assert.True(result.IsValid);

            var updateOrdinals = bid.UpdateMatrixWithNewMetadata(ref dmatrix, dto,  moqCpr.Object);

            dmatrix.Cells = bid.MergeOldAndNewCells(dmatrix, updateOrdinals);


            result = validator.Validate(dmatrix);

            dmatrix.Dspecs["en"].GetDimensionsAsDictionary();



            Dictionary<int, object> dict = bid.DictionaryWithOrdinal(dmatrix, dlist, "en");

            var amendedData = bid.MergeBuildCells(dict, dmatrix);

            Assert.True(amendedData[0].value.Equals(11));
            Assert.True(amendedData[1].value.Equals(108.8));
            Assert.True(amendedData[35].value.Equals(33));
            Assert.True(amendedData[14].value.Equals(151.1));
            Assert.True(amendedData[6].value.Equals(".."));
            Assert.True(amendedData[33].value.Equals(".."));

        }

        [Fact]
        public void TestDataGoodRequestMultipleLanguage()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.AmendDataTwoLanguagesOk();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IDmatrix dmatrix = new Dmatrix();

            DBuild_BSO bid = new DBuild_BSO();
            var dlist = bid.GetInputDynamicList(dto.ChangeData);

            // Get PxDocument from dto
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(dto.MtrInput);
            var pxDocument = pxManualParser.Parse();

            IUpload_DTO uDto = new PxUpload_DTO() { FrqValueTimeval = dto.FrqValueTimeval, LngIsoCode = "en", FrqCodeTimeval = dto.FrqCodeTimeval };

            dmatrix = dmatrix.GetDmatrixFromPxDocument(pxDocument,  uDto);
            DMatrix_VLD validator = new DMatrix_VLD();
            var result = validator.Validate(dmatrix);

            Assert.True(result.IsValid);

            var updateOrdinals = bid.UpdateMatrixWithNewMetadata(ref dmatrix, dto,  moqCpr.Object);

            dmatrix.Cells = bid.MergeOldAndNewCells(dmatrix, updateOrdinals);


            result = validator.Validate(dmatrix);

            dmatrix.Dspecs["en"].GetDimensionsAsDictionary();



            Dictionary<int, object> dict = bid.DictionaryWithOrdinal(dmatrix, dlist, "en");

            var amendedData = bid.MergeBuildCells(dict, dmatrix);



            Assert.True(amendedData[0].value.Equals(111));
            Assert.True(amendedData[10].value.Equals(222));
            Assert.True(amendedData[8].value.Equals(333));
            Assert.True(amendedData[15].value.Equals(444));
            Assert.True(amendedData[1].value.Equals(2354428));
            Assert.True(amendedData[2].value.Equals(".."));
        }

        [Fact]
        public void TestValidateRequestDataNonReferencingHeader()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.AmendDataNonReferencingHeader();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IDmatrix dmatrix = new Dmatrix();

            DBuild_BSO bid = new DBuild_BSO();
            var dlist = bid.GetInputDynamicList(dto.ChangeData);

            // Get PxDocument from dto
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(dto.MtrInput);
            var pxDocument = pxManualParser.Parse();

            IUpload_DTO uDto = new PxUpload_DTO() { FrqValueTimeval = dto.FrqValueTimeval, LngIsoCode = "en", FrqCodeTimeval = dto.FrqCodeTimeval };

            dmatrix = dmatrix.GetDmatrixFromPxDocument(pxDocument,  uDto);
            DMatrix_VLD validator = new DMatrix_VLD();
            var result = validator.Validate(dmatrix);

            Assert.True(result.IsValid);

            var updateOrdinals = bid.UpdateMatrixWithNewMetadata(ref dmatrix, dto, moqCpr.Object);

            dmatrix.Cells = bid.MergeOldAndNewCells(dmatrix, updateOrdinals);


            result = validator.Validate(dmatrix);

            dmatrix.Dspecs["en"].GetDimensionsAsDictionary();



            Dictionary<int, object> dict = bid.DictionaryWithOrdinal(dmatrix, dlist, "en");
            Assert.True(dict == null);

        }

        [Fact]
        public void TestValidateRequestDataNonReferencingVariables()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.AmendDataSingleLanguageNonReferencingVariables();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IDmatrix dmatrix = new Dmatrix();

            DBuild_BSO bid = new DBuild_BSO();
            var dlist = bid.GetInputDynamicList(dto.ChangeData);

            // Get PxDocument from dto
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(dto.MtrInput);
            var pxDocument = pxManualParser.Parse();

            IUpload_DTO uDto = new PxUpload_DTO() { FrqValueTimeval = dto.FrqValueTimeval, LngIsoCode = "en", FrqCodeTimeval = dto.FrqCodeTimeval };

            dmatrix = dmatrix.GetDmatrixFromPxDocument(pxDocument,  uDto);
            DMatrix_VLD validator = new DMatrix_VLD();
            var result = validator.Validate(dmatrix);

            Assert.True(result.IsValid);

            var updateOrdinals = bid.UpdateMatrixWithNewMetadata(ref dmatrix, dto,  moqCpr.Object);

            dmatrix.Cells = bid.MergeOldAndNewCells(dmatrix, updateOrdinals);


            result = validator.Validate(dmatrix);

            dmatrix.Dspecs["en"].GetDimensionsAsDictionary();

            //We can use dlist as the basis for the output report

            Dictionary<int, object> dict = bid.DictionaryWithOrdinal(dmatrix, dlist, "en");

            var amendedData = bid.MergeBuildCells(dict, dmatrix);

            Assert.True(amendedData[0].value.Equals(77.8));
            Assert.True(amendedData[1].value.Equals(108.8));
            Assert.True(amendedData[35].value.Equals(333));
            Assert.True(amendedData[14].value.Equals(151.1));
            Assert.True(amendedData[6].value.Equals(".."));
            Assert.True(amendedData[33].value.Equals(".."));
        }

        [Fact]
        public void TestRequestDataDuplicateData()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.AmendDataSingleLanguageDuplicateData();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IDmatrix dmatrix = new Dmatrix();

            DBuild_BSO bid = new DBuild_BSO();
            var dlist = bid.GetInputDynamicList(dto.ChangeData);

            // Get PxDocument from dto
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(dto.MtrInput);
            var pxDocument = pxManualParser.Parse();

            IUpload_DTO uDto = new PxUpload_DTO() { FrqValueTimeval = dto.FrqValueTimeval, LngIsoCode = "en", FrqCodeTimeval = dto.FrqCodeTimeval };

            dmatrix = dmatrix.GetDmatrixFromPxDocument(pxDocument,  uDto);
            DMatrix_VLD validator = new DMatrix_VLD();
            var result = validator.Validate(dmatrix);

            Assert.True(result.IsValid);

            var updateOrdinals = bid.UpdateMatrixWithNewMetadata(ref dmatrix, dto,  moqCpr.Object);

            dmatrix.Cells = bid.MergeOldAndNewCells(dmatrix, updateOrdinals);


            result = validator.Validate(dmatrix);

            dmatrix.Dspecs["en"].GetDimensionsAsDictionary();

            //We can use dlist as the basis for the output report

            Dictionary<int, object> dict = bid.DictionaryWithOrdinal(dmatrix, dlist, "en");

            var amendedData = bid.MergeBuildCells(dict, dmatrix);

            Assert.True((bool)dlist[2]["duplicate"]);
            Assert.True((bool)dlist[1]["duplicate"]);
            Assert.True(dlist[2]["ordinal"].Equals(-1));
            Assert.True(amendedData[0].value.Equals(111));
            Assert.True(amendedData[1].value.Equals(108.8));
            Assert.True(amendedData[35].value.Equals(".."));
            Assert.True(amendedData[14].value.Equals(151.1));
            Assert.True(amendedData[6].value.Equals(".."));
            Assert.True(amendedData[33].value.Equals(".."));
        }

        [Fact]
        public void TestValidateRequestMissingData()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.AmendDataMissingData();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IDmatrix dmatrix = new Dmatrix();

            DBuild_BSO bid = new DBuild_BSO();
            var dlist = bid.GetInputDynamicList(dto.ChangeData);

            // Get PxDocument from dto
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(dto.MtrInput);
            var pxDocument = pxManualParser.Parse();

            IUpload_DTO uDto = new PxUpload_DTO() { FrqValueTimeval = dto.FrqValueTimeval, LngIsoCode = "en", FrqCodeTimeval = dto.FrqCodeTimeval };

            dmatrix = dmatrix.GetDmatrixFromPxDocument(pxDocument,  uDto);
            DMatrix_VLD validator = new DMatrix_VLD();
            var result = validator.Validate(dmatrix);

            Assert.True(result.IsValid);

            var updateOrdinals = bid.UpdateMatrixWithNewMetadata(ref dmatrix, dto, moqCpr.Object);

            dmatrix.Cells = bid.MergeOldAndNewCells(dmatrix, updateOrdinals);


            result = validator.Validate(dmatrix);

            dmatrix.Dspecs["en"].GetDimensionsAsDictionary();

            //We can use dlist as the basis for the output report

            Dictionary<int, object> dict = bid.DictionaryWithOrdinal(dmatrix, dlist, "en");

            var amendedData = bid.MergeBuildCells(dict, dmatrix);

            Assert.True(dlist.Count == 6);
        }

        [Fact]
        public void TestValidateRequestIncompleteHeader()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.AmendDataIncompleteHeader();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IDmatrix dmatrix = new Dmatrix();

            DBuild_BSO bid = new DBuild_BSO();
            var dlist = bid.GetInputDynamicList(dto.ChangeData);

            Assert.True(dlist.Count == 0);
        }
        [Fact]
        public void TestParseMtrInputNoTimeDimensionsInPxOrParameters()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.ParseMtrInputNoTimevalButTimevalInParams();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IDmatrix dmatrix = new Dmatrix();

            DBuild_BSO bid = new DBuild_BSO();
            var dlist = bid.GetInputDynamicList(dto.ChangeData);

            // Get PxDocument from dto
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(dto.MtrInput);
            var pxDocument = pxManualParser.Parse();

            IUpload_DTO uDto = new PxUpload_DTO();// { FrqValueTimeval = dto.FrqValueTimeval, LngIsoCode = "en", FrqCodeTimeval = dto.FrqCodeTimeval };

            dmatrix = dmatrix.GetDmatrixFromPxDocument(pxDocument,  uDto);

            Assert.True(dmatrix.Dspecs["en"].Dimensions.Where(x => x.Role.Equals("TIME")).Count() == 0);
        }

        [Fact]
        public void TestParseMtrInputNoTimeDimensionsButInParameters()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.ParseMtrInputNoTimevalButTimevalInParams();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IDmatrix dmatrix = new Dmatrix();

            DBuild_BSO bid = new DBuild_BSO();
            var dlist = bid.GetInputDynamicList(dto.ChangeData);

            // Get PxDocument from dto
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(dto.MtrInput);
            var pxDocument = pxManualParser.Parse();

            IUpload_DTO uDto = new PxUpload_DTO() { FrqValueTimeval = dto.FrqValueTimeval, LngIsoCode = "en", FrqCodeTimeval = dto.FrqCodeTimeval };

            dmatrix = dmatrix.GetDmatrixFromPxDocument(pxDocument,  uDto);

            Assert.True(dmatrix.Dspecs["en"].Dimensions.Where(x => x.Role.Equals("TIME")).Count() == 1);

            Assert.True(dmatrix.Dspecs["en"].Dimensions.Where(x => x.Role.Equals("TIME")).First().Code.Equals("TLIST(A1)"));
        }

        [Fact]
        public void TestMapsAndEliminations()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.TestMapsAndEliminations();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IDmatrix dmatrix = new Dmatrix();

            DBuild_BSO bid = new DBuild_BSO();
            var dlist = bid.GetInputDynamicList(dto.ChangeData);

            // Get PxDocument from dto
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(dto.MtrInput);
            var pxDocument = pxManualParser.Parse();

            IUpload_DTO uDto = new PxUpload_DTO() { FrqValueTimeval = dto.FrqValueTimeval, LngIsoCode = "en", FrqCodeTimeval = dto.FrqCodeTimeval };

            dmatrix = dmatrix.GetDmatrixFromPxDocument(pxDocument,  uDto);
           
            DMatrix_VLD validator = new DMatrix_VLD();
            var result = validator.Validate(dmatrix);

            Assert.True(result.IsValid);

            var updateOrdinals = bid.UpdateMatrixWithNewMetadata(ref dmatrix, dto,  moqCpr.Object);

            dmatrix.Cells = bid.MergeOldAndNewCells(dmatrix, updateOrdinals);


            result = validator.Validate(dmatrix);

            dmatrix.Dspecs["en"].GetDimensionsAsDictionary();

            //We can use dlist as the basis for the output report

            Dictionary<int, object> dict = bid.DictionaryWithOrdinal(dmatrix, dlist, "en");

            var amendedData = bid.MergeBuildCells(dict, dmatrix);

            var dimitem = dmatrix.Dspecs["en"].Dimensions.Where(x => x.Code.Equals("NACE2")).First();
            var varitem = dimitem.Variables.Where(x => x.Code.Equals("10")).First();
            Assert.True(varitem.Elimination);
            Assert.True(dimitem.GeoFlag);
            Assert.True(dimitem.GeoUrl.Equals("https://cdn.cso.ie/static/map/en/mapTestXXX/Province_BoundariesTopo.json"));
        }

        [Fact]
        public void TestBsoBuildUpdateBSO()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.AmendDataSingleLanguageOk();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);
            string? Url = ApiServicesHelper.Configuration.GetSection("Test:Url").Value;
            DBuild_BSO dbso = new DBuild_BSO();
            ICopyright icpr = new Copyright_DTO_Read()
           
            {
                CprCode = "CSO",
                CprUrl = Url,
                CprValue = "Central Statistics Office, Ireland"
            };

            var response = dbso.BsoUpdate(new JSONRPC_Output(),dto,null, false, icpr);

            IDmatrix dmatrix = dbso.refMatrix;

            var cells = dmatrix.Cells.ToList();

            Assert.True(cells[0].Equals(11));
            Assert.True(cells[1].Equals(108.8));
            Assert.True(cells[35].Equals(33));
            Assert.True(cells[14].Equals(151.1));
            Assert.True(cells[6].Equals(".."));
            Assert.True(cells[33].Equals(".."));
        }

        [Fact]
        public void TestValidateCreateOk()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.TestVldCreateOk();
            DBuild_DTO_Create dto = new DBuild_DTO_Create(parameters);
            DBuild_VLD_Create vld = new DBuild_VLD_Create();
            var result = vld.Validate(dto);
            if(!result.IsValid)
            {
                var errors = result.Errors;
            }
            Assert.True(result.IsValid);
        }

        [Fact]
        public void TestCreateSpecsAreNotValidDifferentDimensionCodes()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.TestCreateSpecsAreNotValidDifferentDimensionCodes();
            DBuild_DTO_Create dto = new DBuild_DTO_Create(parameters);
            DBuild_VLD_Create vld = new DBuild_VLD_Create();
            var result = vld.Validate(dto);
            Assert.False(result.IsValid);
            Assert.Contains("Invalid Dimensions", result.Errors.Select(x => x.ErrorMessage).ToList());
        }


        [Fact]
        public void TestCreateSpecsAreNotValidUmatchedVariables()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.TestCreateSpecsAreNotValidDifferentDimensionCodes();
            DBuild_DTO_Create dto = new DBuild_DTO_Create(parameters);
            DBuild_VLD_Create vld = new DBuild_VLD_Create();
            var result = vld.Validate(dto);
            Assert.False(result.IsValid);
            Assert.Contains("Invalid Dimensions", result.Errors.Select(x => x.ErrorMessage).ToList());
        }


        [Fact]
        public void TestCreateSpecsAreNotValidNonUniqueLanguage()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.TestCreateSpecsAreNotValidNonUniqueLanguage();
            DBuild_DTO_Create dto = new DBuild_DTO_Create(parameters);
            DBuild_VLD_Create vld = new DBuild_VLD_Create();
            var result = vld.Validate(dto);
            Assert.False(result.IsValid);
            Assert.Contains("Non unique language", result.Errors.Select(x => x.ErrorMessage).ToList());
        }


        [Fact]
        public void TestCreateSpecsAreNotValidNoMainLanguage()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.TestCreateSpecsAreNotValidNoMainLanguage();
            DBuild_DTO_Create dto = new DBuild_DTO_Create(parameters);
            DBuild_VLD_Create vld = new DBuild_VLD_Create();
            var result = vld.Validate(dto);
            Assert.False(result.IsValid);
            Assert.Contains("Main language not contained in any dimension", result.Errors.Select(x => x.ErrorMessage).ToList());
        }


        [Fact]
        public void TestCreateSpecsAreNotValidRepeatedPeriod()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.TestCreateSpecsAreNotValidRepeatedPeriod();
            DBuild_DTO_Create dto = new DBuild_DTO_Create(parameters);
            DBuild_VLD_Create vld = new DBuild_VLD_Create();
            var result = vld.Validate(dto);
            Assert.False(result.IsValid);
            Assert.Contains("Periods repeated in request", result.Errors.Select(x => x.ErrorMessage).ToList());
        }
        [Fact]
        public void TestReadDataset()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.TestReadDataset();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            DBuild_BSO dbso = new DBuild_BSO();

            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(dto.MtrInput);
            var pxDocument = pxManualParser.Parse();

            IDmatrix dmatrix = dbso.ReadDataset(pxDocument, dto);



            var cells = dmatrix.Cells.ToList();

            Assert.True(cells[0].Equals(77.8));
            Assert.True(cells[3].Equals(114.4));
            Assert.True(cells[6].Equals(91.7));
            Assert.True(cells[9].Equals(117.3));
            Assert.True(cells[11].Equals(157.7));
        }

        [Fact]
        public void TestReadDatasetResultAsCsv()
        {
            Helper.SetupTests();
            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.TestReadDataset();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            DBuild_BSO dbso = new DBuild_BSO(); 

            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(dto.MtrInput);
            var pxDocument = pxManualParser.Parse();

            IDmatrix dmatrix = dbso.ReadDataset(pxDocument, dto);

            FlatTableBuilder ftb = new FlatTableBuilder();

            DataTable dt = ftb.GetMatrixDataTableCodesOnly(dmatrix, dto.Dspecs[0].Language, true);

            dynamic result = new ExpandoObject();
            result.csv = ftb.GetCsv(dt, "\"");

            Assert.True(result.csv != null);
        }

        [Fact]
        public void TestUpdateWithNoNewVariables()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.TestUpdateWithNoNewVariables();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);
            string? Url = ApiServicesHelper.Configuration.GetSection("Test:Url").Value;
            DBuild_BSO dbso = new DBuild_BSO();
            ICopyright icpr = new Copyright_DTO_Read()
            {
                CprCode = "CSO",
                CprUrl = Url,
                CprValue = "Central Statistics Office, Ireland"
            };

            var response = dbso.BsoUpdate(new JSONRPC_Output(), dto, null, false, icpr);

            IDmatrix dmatrix = dbso.refMatrix;



            var cells = dmatrix.Cells.ToList();
            Assert.True(cells.Count == 24);
            Assert.True(cells[0].Equals(77.8));
            Assert.True(cells[1].Equals(108.8));
            Assert.True(cells[6].Equals(1));
            Assert.True(cells[11].Equals(151.1));
            Assert.True(cells[20].Equals(".."));
            Assert.True(cells[23].Equals(157.7));

        }


        [Fact]
        public void TestInvalidUpdateHeaderWithNonExistentDimension()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.TestInvalidUpdateHeaderWithNonExistentDimension();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IDmatrix dmatrix = new Dmatrix();
            IResponseOutput response = new JSONRPC_Output();


            DBuild_BSO bid = new DBuild_BSO();
            var rsp = bid.BsoUpdate(response, dto,  null, false);

            Assert.True(rsp.error.Equals("Validation failed. Please review your input."));
        }

        [Fact]
        public void TestGoodUpdateUsingBSO()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.TestGoodUpdateUsingBSO();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IResponseOutput response = new JSONRPC_Output();


            DBuild_BSO bid = new DBuild_BSO();
            var rsp = bid.BsoUpdate(response, dto,  null, false, new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            DMatrix_VLD validator = new DMatrix_VLD();
            var result = validator.Validate(bid.refMatrix);
            Assert.True(result.IsValid);
            Assert.True(bid.refMatrix.Cells.Count == 728);
        }

        [Fact]
        public void TestInvalidUpdateWithDuplicateHeader()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.TestInvalidUpdateWithDuplicateHeader();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IDmatrix dmatrix = new Dmatrix();
            IResponseOutput response = new JSONRPC_Output();


            DBuild_BSO bid = new DBuild_BSO();
            var rsp = bid.BsoUpdate(response, dto, null, false);

            Assert.True(rsp.error.Equals("Validation failed. Please review your input."));
        }

        [Fact]
        public void TestUpdateWithNoNewData()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.TestUpdateWithNoNewData();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IResponseOutput response = new JSONRPC_Output();


            DBuild_BSO bid = new DBuild_BSO();
            var rsp = bid.BsoUpdate(response, dto, null, false, new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            DMatrix_VLD validator = new DMatrix_VLD();
            var result = validator.Validate(bid.refMatrix);
            Assert.True(result.IsValid);
            Assert.True(bid.refMatrix.Cells.Count == 728);
        }

        //
        [Fact]
        public void TestUpdateWithOneDupe()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.TestUpdateWithOneDupe();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IResponseOutput response = new JSONRPC_Output();


            DBuild_BSO bid = new DBuild_BSO();
            var rsp = bid.BsoUpdate(response, dto, null, false, new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            DMatrix_VLD validator = new DMatrix_VLD();
            var result = validator.Validate(bid.refMatrix);
            Assert.True(result.IsValid);
            List<dynamic> dupeReport = rsp.data.report[8];
            Assert.True(dupeReport[4]);
            Assert.False(dupeReport[5]);

        }

        [Fact]
        public void TestUpdateWithNoNewAnything()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.TestUpdateWithNoNewAnything();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IResponseOutput response = new JSONRPC_Output();


            DBuild_BSO bid = new DBuild_BSO();
            var rsp = bid.BsoUpdate(response, dto,null, false, new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            DMatrix_VLD validator = new DMatrix_VLD();
            var result = validator.Validate(bid.refMatrix);
            Assert.True(result.IsValid);

            Assert.True(rsp.data.report.Count == 0);

        }
        //TestUpdateMainMetadata
        [Fact]
        public void TestUpdateMainMetadata()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.TestUpdateMainMetadata();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IResponseOutput response = new JSONRPC_Output();


            DBuild_BSO bid = new DBuild_BSO();
            var rsp = bid.BsoUpdate(response, dto,  null, false, new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            DMatrix_VLD validator = new DMatrix_VLD();
            var result = validator.Validate(bid.refMatrix);
            Assert.True(result.IsValid);

            Assert.True(bid.refMatrix.Code.Equals("AVA30zz"));
            Assert.True(bid.refMatrix.Dspecs["en"].Title.Equals("Average Age of Holderzz"));
            Assert.True(bid.refMatrix.Dspecs["en"].NotesAsString.Equals("This is a new note"));

        }

        [Fact]
        public void TestCreateMapsAndEliminations()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.TestCreateMapsAndEliminations();
            DBuild_DTO_Create dto = new DBuild_DTO_Create(parameters);

            IResponseOutput response = new JSONRPC_Output();


            DBuild_BSO bid = new DBuild_BSO();
            ICopyright cpr = new Copyright_DTO_Read()
            {
                CprCode = "DHA",
                CprValue = "Department of Health and Children",
                CprUrl = "https://www.gov.ie/en/organisation/department-of-health/"
            };

            var dmatrix = bid.Create(dto, cpr);
            DMatrix_VLD validator = new DMatrix_VLD();
            var result = validator.Validate(dmatrix);

            Assert.True(dmatrix.Dspecs["en"].Dimensions.ToList()[2].Variables.ToList()[0].Elimination);
            Assert.False(dmatrix.Dspecs["en"].Dimensions.ToList()[1].Variables.ToList()[0].Elimination);
            Assert.True(dmatrix.Dspecs["en"].Dimensions.ToList()[2].GeoUrl.Equals("www.maps.ie"));
            Assert.True(dmatrix.Dspecs["en"].Dimensions.ToList()[2].GeoFlag);

            Assert.True(result.IsValid);
        }
        /// <summary>
        /// Update with changed notes and changed elimination
        /// </summary>
        [Fact]
        public void TestUpdateAmendNotesAndElimination()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.TestUpdateAmendNotesAndElmination();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IResponseOutput response = new JSONRPC_Output();


            DBuild_BSO bid = new DBuild_BSO();
            var rsp = bid.BsoUpdate(response, dto, null, false, new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            DMatrix_VLD validator = new DMatrix_VLD();
            var result = validator.Validate(bid.refMatrix);
            Assert.True(result.IsValid);
            Assert.Contains(".xxx", bid.refMatrix.Dspecs["en"].NotesAsString);
            Assert.True(bid.refMatrix.Dspecs["en"].Dimensions.ToList()[2].Variables[1].Elimination);
            Assert.False(bid.refMatrix.Dspecs["en"].Dimensions.ToList()[2].Variables[0].Elimination);

            Assert.True(bid.refMatrix.Dspecs["en"].Dimensions.ToList()[2].Variables.Where(x => x.Elimination).Count() == 1);


        }

        /// <summary>
        /// Update with changed notes for 2 languages
        /// </summary>
        [Fact]
        public void TestUpdateAmendNotestTwoLanguages()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.TestUpdateAmendNotestTwoLanguages();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IResponseOutput response = new JSONRPC_Output();


            DBuild_BSO bid = new DBuild_BSO();
            var rsp = bid.BsoUpdate(response, dto, null, false, new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            DMatrix_VLD validator = new DMatrix_VLD();
            var result = validator.Validate(bid.refMatrix);
            Assert.True(result.IsValid);
            Assert.True(bid.refMatrix.Dspecs["en"].NotesAsString.Equals("This is a note"));
            Assert.True(bid.refMatrix.Dspecs["ga"].NotesAsString.Equals("Is nóta é seo"));

        }

        /// <summary>
        /// Update general metadata
        /// </summary>
        [Fact]
        public void TestUpdateMostMetadata()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.TestUpdateMostMetadata();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IResponseOutput response = new JSONRPC_Output();


            DBuild_BSO bid = new DBuild_BSO();
            var rsp = bid.BsoUpdate(response, dto,  null, false, new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            DMatrix_VLD validator = new DMatrix_VLD();
            var result = validator.Validate(bid.refMatrix);
            Assert.True(result.IsValid);
            Assert.True(bid.refMatrix.Dspecs["en"].NotesAsString.Equals("xxx"));
            Assert.True(bid.refMatrix.Code == "UA05X");
            var theDim = bid.refMatrix.Dspecs["en"].Dimensions.Where(x => x.Role.Equals("TIME")).FirstOrDefault();
            Assert.True(theDim.Code == "TLIST(M1)");
            Assert.True(theDim.Value == "Dayxx");
            theDim = bid.refMatrix.Dspecs["en"].Dimensions.Where(x => x.Role.Equals("STATISTIC")).FirstOrDefault();
            Assert.True(theDim.Value == "Statisticxxx");
            Assert.True(bid.refMatrix.Dspecs["en"].Title == "Number and Location (based on PPSN allocations data) of Arrivals from Ukrainexxx");
            Assert.True(bid.refMatrix.Copyright.CprValue == "Department of Health and Children");
            Assert.False(bid.refMatrix.IsOfficialStatistic);

        }

        /// <summary>
        /// Update general metadata
        /// </summary>
        [Fact]
        public void TestUpdateMostMetadataMultiLanguage()
        {
            Helper.SetupTests();

            var moqCpr = new Mock<Copyright_BSO>();
            moqCpr.Setup(m => m.Read(It.IsAny<string>())).Returns(new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.TestUpdateMostMetadataMultiLanguage();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            IResponseOutput response = new JSONRPC_Output();


            DBuild_BSO bid = new DBuild_BSO();
            var rsp = bid.BsoUpdate(response, dto,  null, false, new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" });

            DMatrix_VLD validator = new DMatrix_VLD();
            var result = validator.Validate(bid.refMatrix);
            Assert.True(result.IsValid);
            Assert.True(bid.refMatrix.Dspecs["en"].NotesAsString.Equals("xxxx"));
            Assert.True(bid.refMatrix.Code == "ADR04X");
            var theDim = bid.refMatrix.Dspecs["en"].Dimensions.Where(x => x.Role.Equals("TIME")).FirstOrDefault();
            Assert.True(theDim.Code == "TLIST(M1)");
            Assert.True(theDim.Value == "CensusYearxxx");
            theDim = bid.refMatrix.Dspecs["en"].Dimensions.Where(x => x.Role.Equals("STATISTIC")).FirstOrDefault();
            Assert.True(theDim.Value == "Statisticxxx");
            Assert.True(bid.refMatrix.Dspecs["en"].Title == "1996 Percentage Change in Population 1991 - 1996xxx");
            Assert.True(bid.refMatrix.Copyright.CprValue == "Department of Health and Children");
            Assert.False(bid.refMatrix.IsOfficialStatistic);
            Assert.True(bid.refMatrix.Dspecs["en"].NotesAsString == "xxxx");
            Assert.True(bid.refMatrix.Dspecs["ga"].NotesAsString.Equals("yyyy")); ;
            theDim = bid.refMatrix.Dspecs["ga"].Dimensions.Where(x => x.Role.Equals("TIME")).FirstOrDefault();
            Assert.True(theDim.Value == "BliainDaonáirimhyyy");
            theDim = bid.refMatrix.Dspecs["ga"].Dimensions.Where(x => x.Role.Equals("STATISTIC")).FirstOrDefault();
            Assert.True(theDim.Value == "Statisticyyy");
            Assert.True(bid.refMatrix.Dspecs["ga"].Title == "1996 Athrú Daonra mar Chéatadán 1991 - 1996yyy"); //yyyy
        }
    }
}
