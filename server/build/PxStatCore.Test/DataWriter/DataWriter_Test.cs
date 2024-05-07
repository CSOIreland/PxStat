using API;
using Moq;
using PxStat;
using PxStat.Data;
using PxStat.DataStore;
using PxStat.JsonQuery;
using PxStat.JsonStatSchema;
using PxStatCore.Test;
using System.Dynamic;
using System.Text;

/// <summary>
/// For these tests to work you need to setup a user environment variable UNIT_TEST_SQL_CONNECTION_STRING
/// whose value is the database connection string, for the environment, where you want the unit test to run.
/// </summary>
namespace PxStatXUnit.Tests
{
    [Collection("PxStatXUnit")]
    public class DataWriter_Test
    {
        Mock<IADO> ado = new Mock<IADO>();

        public DataWriter_Test() 
        {
            Helper.SetupTests();
            ado.Setup(a => a.ExecuteReaderProcedure(It.IsAny<string>(), It.IsAny<List<ADO_inputParams>>())).Returns(GetReaderOutput());
        }
       
        [Fact]
        public void TestCreateAndLoadMetadataWithCubeQuery()
        {
            IDataWriter dataWriter = new DataWriter();
            string rfolder = Helper.GetResourceFolder();
            IDmatrix matrix = GetMatrix(rfolder + "HPM02.b64");
            string groupCode = "LMTEST";
            string username = ApiServicesHelper.Configuration.GetSection("Test:SamAccountName").Value; ;
            int releaseVersion = 0;
            int releaseRevision = 1;
            Release_DTO releaseDTO = new Release_DTO()
            { 
                RlsVersion = releaseVersion,
                RlsRevision = releaseRevision,
                GrpCode = groupCode,
                RlsLiveDatetimeFrom = DateTime.Now,
                RlsLiveDatetimeTo = DateTime.MaxValue
            };
            Release_ADO releaseADO = new Release_ADO(ado.Object);
            int releaseId = releaseADO.Create(releaseDTO, username);
            dataWriter.CreateAndLoadDataField(ado.Object, matrix, username, releaseId);
            dataWriter.CreateAndLoadMetadata(ado.Object, matrix);

            PxStat.DataStore.IDataReader dataReader = new DataReader();
            string cubeQueryJSON = GetFromFile(rfolder + "HPM02CubeQuery.json");
            string cubeQueryJSONExtension = GetFromFile(rfolder + "HPM02CubeQueryExtension.json");
            CubeQuery_DTO cubeQuery = new CubeQuery_DTO();
            JsonStatQuery jsonStatQuery = Utility.JsonDeserialize_IgnoreLoopingReference<JsonStatQuery>(cubeQueryJSON);
            JsonStatQueryExtension jsonStatQueryExtension = Utility.JsonDeserialize_IgnoreLoopingReference<JsonStatQueryExtension>(cubeQueryJSON);
            cubeQuery.jStatQuery = jsonStatQuery;
            cubeQuery.jStatQueryExtension = jsonStatQueryExtension;
            var latestRelease = releaseADO.ReadID(releaseId, username);
            Release_DTO release = Release_ADO.GetReleaseDTO(latestRelease);
            IDmatrix result = dataReader.QueryDataset(ado.Object,  cubeQuery, release);
            Assert.NotNull(result);
        }
        


        private string GetBase64InputFromFile(string filename)
        {
            
            var encodedString = File.ReadAllText(filename);
            byte[] data = Convert.FromBase64String(encodedString);
            return Encoding.UTF8.GetString(data);
        }

        private IDmatrix GetMatrix(string filename)
        {
            var input = GetBase64InputFromFile(filename);
            var ado = new Mock<IADO>();
            var ado_readerOutput = new Mock<ADO_readerOutput>();
            ado.Setup(a => a.ExecuteReaderProcedure(It.IsAny<string>(), It.IsAny<List<ADO_inputParams>>())).Returns(ado_readerOutput.Object);
         


            DmatrixFactory dmatrixFactory = new DmatrixFactory(ado.Object);
            PxUpload_DTO dto = new PxUpload_DTO();
            dto.GrpCode = "LMTEST";
            dto.LngIsoCode = "en";
            dto.FrqValueTimeval = "Month";
            dto.FrqCodeTimeval = "TLIST(M1)";
            dto.Signature = "9bceb07b3010e47259a266edd3e76577";
            dto.Overwrite = true;
            dto.MtrInput = input;
            IDmatrix dmatrix = dmatrixFactory.CreateDmatrix(dto);
            dmatrix.Copyright.CprValue = "Central Statistics Office, Ireland";
            return dmatrix;
        }

        private string GetFromFile(string filename)
        {
            var text = File.ReadAllText( filename);
            return text;
        }

        private void RemoveMatrix(IADO ado, int releaseCode, string username, int matrixId)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name ="@RlsCode",value= releaseCode},
                new ADO_inputParams() {name ="@CcnUsername",value= username},
                new ADO_inputParams() {name ="@MtrId",value= matrixId}
            };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };
            ado.ExecuteNonQueryProcedure("Data_Matrix_Remove", inputParams, ref returnParam);
        }

        [Fact]
        public void TestCreateAndLoadMetadataWithQueryMatrix()
        {
            IDataWriter dataWriter = new DataWriter();
            string rfolder = Helper.GetResourceFolder();
            IDmatrix matrix = GetMatrix(rfolder + "HPM02.b64");
            string groupCode = "LMTEST";
            string? username = ApiServicesHelper.Configuration.GetSection("Test:SamAccountName").Value;
            int releaseVersion = 0;
            int releaseRevision = 1;
            Release_DTO releaseDTO = new Release_DTO()
            {
                RlsVersion = releaseVersion,
                RlsRevision = releaseRevision,
                GrpCode = groupCode,
                RlsLiveDatetimeFrom = DateTime.Now,
                RlsLiveDatetimeTo = DateTime.MaxValue
            };
            Release_ADO releaseADO = new Release_ADO(ado.Object);
            int releaseId = releaseADO.Create(releaseDTO, username);
            dataWriter.CreateAndLoadDataField(ado.Object, matrix, username, releaseId);
            dataWriter.CreateAndLoadMetadata(ado.Object, matrix);

            PxStat.DataStore.IDataReader dataReader = new DataReader();
            var latestRelease = releaseADO.ReadID(releaseId, username);
            Release_DTO release = Release_ADO.GetReleaseDTO(latestRelease);
            matrix.Release = release;
            IDmatrix result = dataReader.QueryDataset(ado.Object,  matrix, "en");
            Assert.NotNull(result);           
        }

        private ADO_readerOutput GetReaderOutput()
        {
            ADO_readerOutput output = new ADO_readerOutput();
            dynamic o = new ExpandoObject();
            o.MtrCode = "HPM02";
            o.RlsCode = 1;
            o.RlsVersion = 1;
            o.RlsRevision = 1;
            o.RlsLiveFlag = false;
            o.RlsLiveDatetimeFrom = DateTime.Now;
            o.RlsLiveDatetimeTo = DateTime.Now;
            o.RlsExceptionalFlag = false;
            o.RlsReservationFlag = false;
            o.RlsArchiveFlag = false;
            o.RlsExperimentalFlag = false;
            o.RlsAnalyticalFlag = false;
            o.FrmVersion = "";
            o.MtrOfficialFlag = false;
            o.FrmType = "";
            o.LngIsoCode = "en";
            o.CprCode = "";
            o.CprValue = "";
            o.CprUrl = "";
            o.MtrId = 1;
            o.MtrNote = "";
            o.SbjValue = "";
            List<object> d = new List<object>();
            d.Add(1);
            d.Add(2);
            d.Add(3);
            d.Add(4);
            string data = Utility.JsonSerialize_IgnoreLoopingReference(d);
            o.MtdData = data;
            o.MdmCode = "HPM02";
            o.MdmId = 1;
            o.MdmSequence = 1;
            o.MdmValue = "Test";
            o.MdmGeoFlag = false;
            o.MdmGeoUrl = "";
            o.DmrCode = "CLASSIFICATION";
            o.DmtCode = "Code";
            o.DmtValue = "Value";
            o.DmtSequence = 1;
            o.DmtEliminationFlag = false;
            o.DmtDecimals = DBNull.Value;
            o.DmtUnit = DBNull.Value;
            o.RsnValueExternal = "Test";
            output.data.Add(o);
            output.hasData = true;
            return output;
        }
    }
}
