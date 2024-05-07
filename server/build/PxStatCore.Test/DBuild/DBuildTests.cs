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
    }
}
