using PxStat;
using PxStat.Data;
using PxStat.Data.Px;
using PxStat.DataStore;
using PxStat.DBuild;
using PxStat.JsonStatSchema;
using PxStatCore.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace PxStatXUnit.Tests
{
    [Collection("PxStatXUnit")]
    public class QueryDataset_Test
    {
        [Fact]
        public void TestBasicQuery()
        {
            Helper.SetupTests();
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.CubeQueryDtoStringParameters();
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

            var statDim = matrix.Dspecs["en"].Dimensions.Where(x => x.Code.Equals("STATISTIC")).First();
            var timeDim = matrix.Dspecs["en"].Dimensions.Where(x => x.Code.Equals("TLIST(M1)")).First();
            var clsDim = matrix.Dspecs["en"].Dimensions.Where(x => x.Code.Equals("NACE2")).First();


            var queriedMatrix = new DataReader().QueryDataset(dto, matrix);
            Assert.True(queriedMatrix != null);

            

            Assert.True(statDim.Sequence>clsDim.Sequence);
            Assert.True(clsDim.Sequence<timeDim.Sequence);

            Assert.True(clsDim.Variables.Where(x => x.Code.Equals("10")).First().Sequence.Equals(1));
            Assert.True(clsDim.Variables.Where(x => x.Code.Equals("101")).First().Sequence.Equals(2));

            Assert.True(timeDim.Variables.Where(x => x.Code.Equals("2020M06")).First().Sequence.Equals(2));
            Assert.True(timeDim.Variables.Where(x => x.Code.Equals("2020M07")).First().Sequence.Equals(3));

            Assert.True(queriedMatrix.Cells.ToList()[0].Value.Equals(91.7));
            Assert.True(queriedMatrix.Cells.ToList()[1].Value.Equals(93.6));
            Assert.True(queriedMatrix.Cells.ToList()[2].Value.Equals(106.7));
            Assert.True(queriedMatrix.Cells.ToList()[3].Value.Equals(113.5));
        }

        [Fact]
        public void TestBasicQueryCodeOrderReversed()
        {
            Helper.SetupTests();
            TestMatrixFactory tmf = new TestMatrixFactory();
            dynamic parameters = tmf.CubeQueryDtoStringParameters();
            CubeQuery_DTO dto = new CubeQuery_DTO(parameters);


            //Parse the px document
            var pxManualParser = new PxParser.Resources.Parser.PxManualParser(tmf.TestPxfileMim04PcsHeadingStubReversed());
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

            var statDim = matrix.Dspecs["en"].Dimensions.Where(x => x.Code.Equals("STATISTIC")).First();
            var timeDim = matrix.Dspecs["en"].Dimensions.Where(x => x.Code.Equals("TLIST(M1)")).First();
            var clsDim = matrix.Dspecs["en"].Dimensions.Where(x => x.Code.Equals("NACE2")).First();


            var queriedMatrix = new DataReader().QueryDataset(dto, matrix);
            Assert.True(queriedMatrix != null);



            Assert.True(statDim.Sequence > clsDim.Sequence);
            Assert.True(clsDim.Sequence < timeDim.Sequence);

            Assert.True(clsDim.Variables.Where(x => x.Code.Equals("10")).First().Sequence.Equals(1));
            Assert.True(clsDim.Variables.Where(x => x.Code.Equals("101")).First().Sequence.Equals(2));

            Assert.True(timeDim.Variables.Where(x => x.Code.Equals("2020M06")).First().Sequence.Equals(2));
            Assert.True(timeDim.Variables.Where(x => x.Code.Equals("2020M07")).First().Sequence.Equals(3));

            Assert.True(queriedMatrix.Cells.ToList()[0].Value.Equals(91.7));
            Assert.True(queriedMatrix.Cells.ToList()[1].Value.Equals(93.6));
            Assert.True(queriedMatrix.Cells.ToList()[2].Value.Equals(106.7));
            Assert.True(queriedMatrix.Cells.ToList()[3].Value.Equals(113.5));
        }
    }
}
