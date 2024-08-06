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
using PxStat.Data.Px;
using PxStat.JsonStatSchema;
using PxStat;
using PxStat.DataStore;
using Newtonsoft.Json.Linq;
using PxStat.Entities.DBuild;
using System.Linq;

namespace PxStatXUnit.Tests
{
    [Collection("PxStatXUnit")]
    public class DBuildTests
    {
        [Fact]
        public void TestBuildReadDataset()
        {
            Helper.SetupTests();
            dynamic parameters = new TestMatrixFactory().TestBuildReadDataset();
            DBuild_DTO_Update dto = new DBuild_DTO_Update(parameters);

            DBuild_BSO bso = new DBuild_BSO();
            //Parse the px document
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(dto.MtrInput);
            var pxDocument = pxManualParser.Parse();

            //validate the px document
            PxSchemaValidator psv = new PxSchemaValidator();
            var pxValidation = psv.Validate(pxDocument);
            if (!pxValidation.IsValid)
            {
                Assert.True(false);
            }

            IDmatrix dmatrix = bso.ReadDataset(pxDocument, dto);

            //148
            var dtim = dmatrix.Dspecs["en"].Dimensions.Where(x => x.Role.Equals("TIME")).First();
            Assert.NotNull(dtim);
            var newperiod = dtim.Variables.Where(x => x.Code.Equals("202301")).FirstOrDefault();
            Assert.NotNull(newperiod);
            Assert.True(dmatrix.Cells.Count == 148);

            CubeQuery_DTO query = null;
            using (var b = new DBuild_BSO())
            {
                query = b.MapReadToQueryDto(dto, dmatrix);
                var per= query.jStatQuery.Dimensions["TLIST(M1)"].Category.Index.First();
                Assert.NotNull(per);
                Assert.True(per == "202301");
            }


        }

        [Fact]
        public void TestMapValidator()
        {
            //Currently DSpecs can map validate themselves...

            /*      
             *      spec.Value.ValidateMaps(true);
                    if (spec.Value.ValidationErrors == null) continue;
                    if (spec.Value.ValidationErrors.Count > 0)
                    {
                        response.error = spec.Value.ValidationErrors[0].ErrorMessage;
                        return response;
            }

            DBuild_BSO bso = new DBuild_BSO();
            //Parse the px document
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(dto.MtrInput);
            var pxDocument = pxManualParser.Parse();
            Dmatrix dmr = new Dmatrix();*/
        }

        [Fact]
        public void TestSortVariablesInDimensionTest1()
        {
            Helper.SetupTests();
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.ReOrderVariablesTest1();
            CubeQuery_DTO dto = new CubeQuery_DTO(parameters);


            //Parse the px document
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(tmf.TestPxfileMim04PcsHeadingStub());
            var pxDocument = pxManualParser.Parse();

            Assert.True(pxDocument != null);

            //validate the px document
            PxSchemaValidator psv = new PxSchemaValidator();
            var pxValidation = psv.Validate(pxDocument);
            if (!pxValidation.IsValid)
            {
                Assert.True(false);
            }
            IDmatrix matrix = new Dmatrix(pxDocument, new PxUpload_DTO() { FrqCodeTimeval = "TLIST(M1)", FrqValueTimeval = "Month" }, AppServicesHelper.StaticADO);
            Assert.True(matrix != null);
            DBuild_BSO dbBso = new();

            //var queriedMatrixBefore= new DataReader().QueryDataset(dto, matrix);

            Dictionary<int, int> changeDictionary = new Dictionary<int, int>();
            changeDictionary.Add(1, 3);
            changeDictionary.Add(2, 2);
            changeDictionary.Add(3, 1);

            Dmatrix delta=(Dmatrix)dbBso.SortVariablesInDimension(matrix, changeDictionary, 3);
            var queriedMatrix = new DataReader().QueryDataset(dto, delta);

            Assert.True(queriedMatrix != null);

            Assert.True(queriedMatrix.Cells.ToList()[0].Value.Equals(159.8));

            Assert.True(true);
        }

        [Fact]
        public void TestSortVariablesInDimensionTest2()
        {
            Helper.SetupTests();
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.ReOrderVariablesTest1();
            CubeQuery_DTO dto = new CubeQuery_DTO(parameters);


            //Parse the px document
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(tmf.TestPxfileMim04PcsHeadingStub());
            var pxDocument = pxManualParser.Parse();

            Assert.True(pxDocument != null);

            //validate the px document
            PxSchemaValidator psv = new PxSchemaValidator();
            var pxValidation = psv.Validate(pxDocument);
            if (!pxValidation.IsValid)
            {
                Assert.True(false);
            }
            IDmatrix matrix = new Dmatrix(pxDocument, new PxUpload_DTO() { FrqCodeTimeval = "TLIST(M1)", FrqValueTimeval = "Month" }, AppServicesHelper.StaticADO);
            Assert.True(matrix != null);
            DBuild_BSO dbBso = new();

           // var queriedMatrixBefore= new DataReader().QueryDataset(dto, matrix);

            Dictionary<int, int> changeDictionary = new Dictionary<int, int>();
            changeDictionary.Add(1, 2);
            changeDictionary.Add(2, 3);
            changeDictionary.Add(3, 1);

            Dmatrix delta = (Dmatrix)dbBso.SortVariablesInDimension(matrix, changeDictionary, 3);
            var queriedMatrix = new DataReader().QueryDataset(dto, delta);

            Assert.True(queriedMatrix != null);

            Assert.True(queriedMatrix.Cells.ToList()[0].Value.Equals(159.8));

            Assert.True(true);
        }

        [Fact]
        public void TestSortVariablesInDimensionTest3()
        {
            Helper.SetupTests();
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.ReOrderVariablesTest1();
            CubeQuery_DTO dto = new CubeQuery_DTO(parameters);


            //Parse the px document
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(tmf.TestPxfileMim04PcsHeadingStub());
            var pxDocument = pxManualParser.Parse();

            Assert.True(pxDocument != null);

            //validate the px document
            PxSchemaValidator psv = new PxSchemaValidator();
            var pxValidation = psv.Validate(pxDocument);
            if (!pxValidation.IsValid)
            {
                Assert.True(false);
            }
            IDmatrix matrix = new Dmatrix(pxDocument, new PxUpload_DTO() { FrqCodeTimeval = "TLIST(M1)", FrqValueTimeval = "Month" }, AppServicesHelper.StaticADO);
            Assert.True(matrix != null);
            DBuild_BSO dbBso = new();

            // var queriedMatrixBefore= new DataReader().QueryDataset(dto, matrix);

            Dictionary<int, int> changeDictionary = new Dictionary<int, int>();
            changeDictionary.Add(1, 2);
            changeDictionary.Add(2, 3);
            changeDictionary.Add(3, 1);

            Dmatrix delta = (Dmatrix)dbBso.SortVariablesInDimension(matrix, changeDictionary, 1);
            var queriedMatrix = new DataReader().QueryDataset(dto, delta);

            Assert.True(queriedMatrix != null);

            Assert.True(queriedMatrix.Cells.ToList()[0].Value.Equals(159.8));

            Assert.True(true);
        }

        [Fact]
        public void TestSortVariablesInDimensionTest4()
        {
            Helper.SetupTests();
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.ReOrderVariablesTest1();
            CubeQuery_DTO dto = new CubeQuery_DTO(parameters);


            //Parse the px document
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(tmf.TestPxfileMim04PcsHeadingStub());
            var pxDocument = pxManualParser.Parse();

            Assert.True(pxDocument != null);

            //validate the px document
            PxSchemaValidator psv = new PxSchemaValidator();
            var pxValidation = psv.Validate(pxDocument);
            if (!pxValidation.IsValid)
            {
                Assert.True(false);
            }
            IDmatrix matrix = new Dmatrix(pxDocument, new PxUpload_DTO() { FrqCodeTimeval = "TLIST(M1)", FrqValueTimeval = "Month" }, AppServicesHelper.StaticADO);
            Assert.True(matrix != null);
            DBuild_BSO dbBso = new();

            // var queriedMatrixBefore= new DataReader().QueryDataset(dto, matrix);

            Dictionary<int, int> changeDictionary = new Dictionary<int, int>();
            changeDictionary.Add(1, 1);
            changeDictionary.Add(2, 2);
            changeDictionary.Add(3, 3);

            Dmatrix delta = (Dmatrix)dbBso.SortVariablesInDimension(matrix, changeDictionary, 1);
            var queriedMatrix = new DataReader().QueryDataset(dto, delta);

            Assert.True(queriedMatrix != null);

            Assert.True(queriedMatrix.Cells.ToList()[0].Value.Equals(159.8));

            Assert.True(true);
        }

        [Fact]
        public void TestSortVariablesInDimensionTest5()
        {
            Helper.SetupTests();
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.ReOrderVariablesTest1();
            CubeQuery_DTO dto = new CubeQuery_DTO(parameters);


            //Parse the px document
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(tmf.TestPxfileMim04PcsHeadingStub());
            var pxDocument = pxManualParser.Parse();

            Assert.True(pxDocument != null);

            //validate the px document
            PxSchemaValidator psv = new PxSchemaValidator();
            var pxValidation = psv.Validate(pxDocument);
            if (!pxValidation.IsValid)
            {
                Assert.True(false);
            }
            IDmatrix matrix = new Dmatrix(pxDocument, new PxUpload_DTO() { FrqCodeTimeval = "TLIST(M1)", FrqValueTimeval = "Month" }, AppServicesHelper.StaticADO);
            Assert.True(matrix != null);
            DBuild_BSO dbBso = new();

            // var queriedMatrixBefore= new DataReader().QueryDataset(dto, matrix);

            Dictionary<int, int> changeDictionary = new Dictionary<int, int>();
            changeDictionary.Add(1, 2);
            changeDictionary.Add(2, 1);

            Dmatrix delta = (Dmatrix)dbBso.SortVariablesInDimension(matrix, changeDictionary, 2);
            var queriedMatrix = new DataReader().QueryDataset(dto, delta);

            Assert.True(queriedMatrix != null);

            Assert.True(queriedMatrix.Cells.ToList()[0].Value.Equals(159.8));

            Assert.True(true);
        }

        [Fact]
        public void TestSortVariablesInDimensionTest6()
        {
            Helper.SetupTests();
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.ReOrderVariablesTest6();
            CubeQuery_DTO dto = new CubeQuery_DTO(parameters);


            //Parse the px document
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(tmf.TestPxfileMim04PcsHeadingStub());
            var pxDocument = pxManualParser.Parse();

            Assert.True(pxDocument != null);

            //validate the px document
            PxSchemaValidator psv = new PxSchemaValidator();
            var pxValidation = psv.Validate(pxDocument);
            if (!pxValidation.IsValid)
            {
                Assert.True(false);
            }
            IDmatrix matrix = new Dmatrix(pxDocument, new PxUpload_DTO() { FrqCodeTimeval = "TLIST(M1)", FrqValueTimeval = "Month" }, AppServicesHelper.StaticADO);
            Assert.True(matrix != null);
            DBuild_BSO dbBso = new();

            // var queriedMatrixBefore= new DataReader().QueryDataset(dto, matrix);

            Dictionary<int, int> changeDictionary = new Dictionary<int, int>();
            changeDictionary.Add(1, 2);
            changeDictionary.Add(2, 3);
            changeDictionary.Add(3, 1);

            Dmatrix delta = (Dmatrix)dbBso.SortVariablesInDimension(matrix, changeDictionary, 1);
            var queriedMatrix = new DataReader().QueryDataset(dto, delta);

            Assert.True(queriedMatrix != null);

            Assert.True(queriedMatrix.Cells.ToList()[0].Value.Equals(113.5));

            Assert.True(true);
        }

        [Fact]
        public void TestBuildReadTemplateByRelease()
        {
            Helper.SetupTests();
            dynamic parameters = GetParameters();
            DBuild_DTO_ReadTemplateByRelease dto = new DBuild_DTO_ReadTemplateByRelease(parameters);
            var ado = AppServicesHelper.StaticADO;
            Release_ADO adoRelease = new Release_ADO(ado);
            string? SamAccountName = ApiServicesHelper.Configuration.GetSection("Test:SamAccountName").Value;
            Release_DTO dtoRelease = Release_ADO.GetReleaseDTO(adoRelease.Read(dto.RlsCode, SamAccountName));

            // Get the matrix from the ReleaseDTO
            IDmatrix dmatrix = new Dmatrix();
            dmatrix = dmatrix.GetMultiLanguageMatrixFromRelease(ado, dtoRelease.MtrCode, dtoRelease);

            FlatTableBuilder ftb = new FlatTableBuilder();

            DataTable dt = ftb.GetMatrixDataTableCodesOnly(dmatrix, dto.LngIsoCode, true, true);

            dynamic result = new ExpandoObject();
            result.FrqValue = "";
            result.template = ftb.GetCsv(dt, "\"", null, true);
            result.MtrCode = dmatrix.Code;

            Assert.NotNull(result);
        }

        [Fact]
        public void TestBuildReadDatasetByRelease()
        {
            Helper.SetupTests();
            dynamic parameters = GetParameters();
            DBuild_DTO_UpdateByRelease dto = new DBuild_DTO_UpdateByRelease(parameters);
            var ado = AppServicesHelper.StaticADO;
            Release_ADO adoRelease = new Release_ADO(ado);
            string? SamAccountName = ApiServicesHelper.Configuration.GetSection("Test:SamAccountName").Value;
            Release_DTO dtoRelease = Release_ADO.GetReleaseDTO(adoRelease.Read(dto.RlsCode, SamAccountName));

            // Get the matrix from the ReleaseDTO
            IDmatrix dmatrix = new Dmatrix();
            dmatrix = dmatrix.GetMultiLanguageMatrixFromRelease(ado, dtoRelease.MtrCode, dtoRelease);

            FlatTableBuilder ftb = new FlatTableBuilder();
            DataTable? dt = null;

            if (!parameters.Labels)
            {
                dt = ftb.GetMatrixDataTableCodesOnly(dmatrix, "en", true);
            }
            else
            {
                dt = ftb.GetMatrixDataTableCodesAndLabels(dmatrix, "en", true);
            }
            Assert.NotNull(dt);
        }

        [Fact]
        public void TestBuildUpdateByRelease()
        {
            Helper.SetupTests();

            IResponseOutput Response = new JSONRPC_Output();
            dynamic parameters = GetParameters();
            DBuild_DTO_UpdateByRelease dto = new DBuild_DTO_UpdateByRelease(parameters);
            dto.Dspecs = new List<DSpec_DTO>();
            var ado = AppServicesHelper.StaticADO;
            string? SamAccountName = ApiServicesHelper.Configuration.GetSection("Test:SamAccountName").Value;

            // Check permissions
            DBuild_BSO dbso = new DBuild_BSO();
            if (!dbso.HasBuildPermission(SamAccountName, "update"))
            {
                Response.error = "error.privilege";
                Assert.Fail("Response has the error: " + Response.error);
            }

            Release_ADO adoRelease = new Release_ADO(ado);
            Release_DTO dtoRelease = Release_ADO.GetReleaseDTO(adoRelease.Read(parameters.RlsCode, SamAccountName));

            // Get the matrix from the ReleaseDTO
            IDmatrix dmatrix = new Dmatrix();
            dmatrix = dmatrix.GetMultiLanguageMatrixFromRelease(ado, dtoRelease.MtrCode, dtoRelease);

            Response = dbso.BsoUpdateByRelease(dmatrix, Response, dto, ado, null, dto.LngIsoCode);
            Assert.NotNull(Response.data);
        }

        [Fact]
        public void TestFilterMatrix()
        {
            IDmatrix dmatrix = new Dmatrix();
            dmatrix.Release = new Release_DTO();
            dmatrix.Release.RlsLiveDatetimeFrom = DateTime.Now;
            dmatrix.Release.PrcCode = "P1";
            DBuild_BSO dbso = new DBuild_BSO();
            Dspec specEn = new Dspec();
            specEn.Language = "en";
            specEn.PrcValue = "P1";
            dmatrix.Dspecs.Add("en", specEn);
            Dspec specGa = new Dspec();
            specGa.Language = "ga";
            specGa.PrcValue = "Gailge";
            dmatrix.Dspecs.Add("ga", specGa);
            IDmatrix result = dbso.FilterMatrix(dmatrix);
            Assert.NotNull(result);
            Assert.True(result.Dspecs["en"].PrcValue.Equals("n/a"));
            Assert.True(result.Release.PrcCode.Equals("NA"));
            Assert.True(result.Dspecs["ga"].PrcValue.Equals("n/b"));
        }

        private dynamic GetParameters()
        {
            dynamic parameters = new ExpandoObject();
            parameters.RlsCode = 202;
            parameters.Labels = true;
            parameters.LngIsoCode = "en";
            parameters.MtrCode = "AES50";
            parameters.MtrOfficialFlag = true;
            parameters.WrqDatetime = DateTime.Now;
            string? username = ApiServicesHelper.Configuration.GetSection("Test:SamAccountName").Value;
            parameters.CcnUsername = username;
            parameters.AprToken = "";
            parameters.Data = null;
            parameters.Dimension = null;
            parameters.Dspecs = new List<DSpec_DTO>();
            parameters.FrmType = "PX";
            parameters.FrmVersion = "2013";
            parameters.FrqCodeTimeval = "";
            parameters.FrqValueTimeval = "";
            parameters.Elimination = "{ \"test\": \"test\" }";
            parameters.Map = "{ \"test\": \"test\" }";
            parameters.CprCode = "";
            return parameters;
        }
    }
}
