using API;
using Moq;
using Xunit;
using PxStat.Config;
using PxStat.Report;
using PxStatCore.Test;
using System.Collections.Generic;
using System.Dynamic;
using System.Configuration;
using PxStat;
using AngleSharp;

namespace PxStatXUnit.Tests
{
    [Collection("PxStatXUnit")]
    public class Configuration_Test
    {
        Mock<IADO> ado = new Mock<IADO>();
        Mock<Config_BSO> cBso = new Mock<Config_BSO>();
        Mock<Config_VLD_Create> cVldCreate = new Mock<Config_VLD_Create>();
        string? username;

        public Configuration_Test()
        {
            Helper.SetupTests();
            username = ApiServicesHelper.Configuration.GetSection("Test:SamAccountName").Value;
        }

        [Fact]
        public void TestReadAppConfiguration()
        {
            Config_ADO cAdo = new Config_ADO(ado.Object);
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Read", It.IsAny<List<ADO_inputParams>>())).Returns(GetReaderOutput("APP"));
            cBso.Setup(c => c.GetCache()).Verifiable();
            var output = cAdo.Read(GetReadDTO());
            Assert.NotNull(output);
        }

        [Fact]
        public void TestReadAppConfigurationNoVersion()
        {
            Config_ADO cAdo = new Config_ADO(ado.Object);
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Read", It.IsAny<List<ADO_inputParams>>())).Returns(GetReaderOutput("APP"));
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Read_Version", It.IsAny<List<ADO_inputParams>>())).Returns(GetLatestVersion());
            cBso.Setup(c => c.GetCache()).Verifiable();
            var output = cAdo.Read(GetReadDTO());
            Assert.NotNull(output);
        }

        [Fact]
        public void TestReadApiConfiguration()
        {
            Config_ADO cAdo = new Config_ADO(ado.Object);
            ado.Setup(a => a.ExecuteReaderProcedure("Api_Settings_Read", It.IsAny<List<ADO_inputParams>>())).Returns(GetReaderOutput("API"));
            cBso.Setup(c => c.GetCache()).Verifiable();
            var dto = GetReadDTO();
            dto.type = "API";
            var output = cAdo.Read(dto);
            Assert.NotNull(output);
        }

        [Fact]
        public void TestReadApiConfigurationNoVersion()
        {
            Config_ADO cAdo = new Config_ADO(ado.Object);
            ado.Setup(a => a.ExecuteReaderProcedure("Api_Settings_Read", It.IsAny<List<ADO_inputParams>>())).Returns(GetReaderOutput("API"));
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Read_Version", It.IsAny<List<ADO_inputParams>>())).Returns(GetLatestVersion());
            cBso.Setup(c => c.GetCache()).Verifiable();
            var dto = GetReadDTO();
            dto.type = "API";
            dto.version = "";
            var output = cAdo.Read(dto);
            Assert.NotNull(output);
        }

        [Fact]
        public void TestCreateConfiguration()
        {
            Config_ADO cAdo = new Config_ADO(ado.Object);
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Read", It.IsAny<List<ADO_inputParams>>())).Returns(GetReaderOutput("APP"));
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Read_Version", It.IsAny<List<ADO_inputParams>>())).Returns(GetLatestVersion());
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Insert_Version", It.IsAny<List<ADO_inputParams>>())).Returns(GetReaderOutput("APP"));
            var retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;
            ado.Setup(a => a.ExecuteNonQueryProcedure("App_Setting_Deploy_Update", It.IsAny<List<ADO_inputParams>>(), ref retParam)).Verifiable();
            cBso.Setup(c => c.GetCache()).Verifiable();
            string v = cAdo.Create(GetCreateDTO(), cVldCreate.Object, username);
            Assert.Equal("2", v);
        }

        [Fact]
        public void TestCreateConfigurationNoVersion()
        {
            Config_ADO cAdo = new Config_ADO(ado.Object);
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Read", It.IsAny<List<ADO_inputParams>>())).Returns(GetReaderOutput("APP"));
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Read_Version", It.IsAny<List<ADO_inputParams>>())).Returns(GetLatestVersion());
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Read_Version", It.IsAny<List<ADO_inputParams>>())).Returns(GetLatestVersion());
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Insert_Version", It.IsAny<List<ADO_inputParams>>())).Returns(GetReaderOutput("APP"));
            var retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;
            ado.Setup(a => a.ExecuteNonQueryProcedure("App_Setting_Deploy_Update", It.IsAny<List<ADO_inputParams>>(), ref retParam)).Verifiable();
            cBso.Setup(c => c.GetCache()).Verifiable();
            var dto = GetCreateDTO();
            dto.version = null;
            string v = cAdo.Create(GetCreateDTO(), cVldCreate.Object, username);
            Assert.Equal("2", v);
        }

        [Fact]
        public void TestCreateConfigurationNoDescription()
        {
            Config_ADO cAdo = new Config_ADO(ado.Object);
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Read", It.IsAny<List<ADO_inputParams>>())).Returns(GetReaderOutput("APP"));
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Read_Version", It.IsAny<List<ADO_inputParams>>())).Returns(GetLatestVersion());
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Read_Version", It.IsAny<List<ADO_inputParams>>())).Returns(GetLatestVersion());
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Insert_Version", It.IsAny<List<ADO_inputParams>>())).Returns(GetReaderOutput("APP"));
            var retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;
            ado.Setup(a => a.ExecuteNonQueryProcedure("App_Setting_Deploy_Update", It.IsAny<List<ADO_inputParams>>(), ref retParam)).Verifiable();
            cBso.Setup(c => c.GetCache()).Verifiable();
            var dto = GetCreateDTO();
            dto.description = null;
            string v = cAdo.Create(GetCreateDTO(), cVldCreate.Object, username);
            Assert.Equal("2", v);
        }

        [Fact]
        public void TestCreateConfigurationNoVersionAndDescription()
        {
            Config_ADO cAdo = new Config_ADO(ado.Object);
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Read", It.IsAny<List<ADO_inputParams>>())).Returns(GetReaderOutput("APP"));
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Read_Version", It.IsAny<List<ADO_inputParams>>())).Returns(GetLatestVersion());
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Read_Version", It.IsAny<List<ADO_inputParams>>())).Returns(GetLatestVersion());
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Insert_Version", It.IsAny<List<ADO_inputParams>>())).Returns(GetReaderOutput("APP"));
            var retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;
            ado.Setup(a => a.ExecuteNonQueryProcedure("App_Setting_Deploy_Update", It.IsAny<List<ADO_inputParams>>(), ref retParam)).Verifiable();
            cBso.Setup(c => c.GetCache()).Verifiable();
            var dto = GetCreateDTO();
            dto.version = null;
            dto.description = null;
            var output = cAdo.Create(dto, cVldCreate.Object, username);
            string v = cAdo.Create(GetCreateDTO(), cVldCreate.Object, username);
            Assert.Equal("2", v);
        }

        [Fact]
        public void TestCreateAPIConfiguration()
        {
            Config_ADO cAdo = new Config_ADO(ado.Object);
            ado.Setup(a => a.ExecuteReaderProcedure("Api_Settings_Read", It.IsAny<List<ADO_inputParams>>())).Returns(GetReaderOutput("API"));
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Read_Version", It.IsAny<List<ADO_inputParams>>())).Returns(GetLatestVersion());
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Read_Version", It.IsAny<List<ADO_inputParams>>())).Returns(GetLatestVersion());
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Insert_Version", It.IsAny<List<ADO_inputParams>>())).Returns(GetReaderOutput("API"));
            var dto = GetCreateDTO();
            dto.type = "API";
            var v = cAdo.CreateAPI(dto, username);
            Assert.Equal("2", v);
        }

        [Fact]
        public void TestCreateAPIConfigurationNoVersion()
        {
            Config_ADO cAdo = new Config_ADO(ado.Object);
            ado.Setup(a => a.ExecuteReaderProcedure("Api_Settings_Read", It.IsAny<List<ADO_inputParams>>())).Returns(GetReaderOutput("API"));
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Read_Version", It.IsAny<List<ADO_inputParams>>())).Returns(GetLatestVersion());
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Read_Version", It.IsAny<List<ADO_inputParams>>())).Returns(GetLatestVersion());
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Insert_Version", It.IsAny<List<ADO_inputParams>>())).Returns(GetReaderOutput("API"));
            var dto = GetCreateDTO();
            dto.version = null;
            dto.type = "API";
            var v = cAdo.CreateAPI(dto, username);
            Assert.Equal("2", v);
        }

        [Fact]
        public void TestCreateAPIConfigurationNoDescription()
        {
            Config_ADO cAdo = new Config_ADO(ado.Object);
            ado.Setup(a => a.ExecuteReaderProcedure("Api_Settings_Read", It.IsAny<List<ADO_inputParams>>())).Returns(GetReaderOutput("API"));
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Read_Version", It.IsAny<List<ADO_inputParams>>())).Returns(GetLatestVersion());
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Read_Version", It.IsAny<List<ADO_inputParams>>())).Returns(GetLatestVersion());
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Insert_Version", It.IsAny<List<ADO_inputParams>>())).Returns(GetReaderOutput("API"));
            var dto = GetCreateDTO();
            dto.description = null;
            dto.type = "API";
            var v = cAdo.CreateAPI(dto, username);
            Assert.Equal("2", v);
        }

        [Fact]
        public void TestCreateAPIConfigurationNoVersionAndDescription()
        {
            Config_ADO cAdo = new Config_ADO(ado.Object);
            ado.Setup(a => a.ExecuteReaderProcedure("Api_Settings_Read", It.IsAny<List<ADO_inputParams>>())).Returns(GetReaderOutput("API"));
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Read_Version", It.IsAny<List<ADO_inputParams>>())).Returns(GetLatestVersion());
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Read_Version", It.IsAny<List<ADO_inputParams>>())).Returns(GetLatestVersion());
            ado.Setup(a => a.ExecuteReaderProcedure("App_Settings_Insert_Version", It.IsAny<List<ADO_inputParams>>())).Returns(GetReaderOutput("API"));
            var dto = GetCreateDTO();
            dto.version = null;
            dto.description = null;
            dto.type = "API";
            var v = cAdo.CreateAPI(dto, username);
            Assert.Equal("2", v);
        }


        private Config_DTO_Read GetReadDTO()
        {
            Config_DTO_Read dto = new Config_DTO_Read();
            dto.version = "1.0";
            dto.type = "APP";
            dto.name = "Test";
            return dto;
        }

        private Config_DTO_Create GetCreateDTO()
        {
            Config_DTO_Create dto = new Config_DTO_Create();
            dto.version = "1.0";
            dto.name = "Test";
            dto.value = "value";
            dto.description = "description";
            return dto;
        }

        private ADO_readerOutput GetReaderOutput(string type)
        {
            ADO_readerOutput output = new ADO_readerOutput();

            if (type.Equals("APP"))
            {
                dynamic o = new ExpandoObject();
                output.data.Add(o);
                output.hasData = true;
            }
            return output;
        }

        private ADO_readerOutput GetLatestVersion()
        {
            ADO_readerOutput output = new ADO_readerOutput();
            dynamic o = new ExpandoObject();
            o.ASV_VERSION = "1.0";
            output.data.Add(o);
            output.hasData = true;
            return output;
        }
    }
}
