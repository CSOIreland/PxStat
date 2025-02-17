using PxStat.Config;

namespace PxStatXUnit.Tests
{

    [Collection("PxStatXUnit")]
    public class Schema_Test
    {
        [Fact]
        public void TestValidClientConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.client.json", GetFile(@"\Resources\config.client.json"), "APP");
            Assert.True(result);
        }

        [Fact]
        public void TestValidChartOptionsClientConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.client.json", GetFile(@"\Resources\config.client.json"), "APP");
            Assert.True(result);
        }

        [Fact]
        public void TestValidDatatableClientConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.client.json", GetFile(@"\Resources\config.client.json"), "APP");
            Assert.True(result);
        }

        [Fact]
        public void TestValidColoursClientConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.client.json", GetFile(@"\Resources\config.client.json"), "APP");
            Assert.True(result);
        }

        [Fact]
        public void TestValidDisplayClientConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.client.json", GetFile(@"\Resources\config.client.json"), "APP");
            Assert.True(result);
        }


        [Fact]
        public void TestValidQuickLinksClientConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.client.json", GetFile(@"\Resources\config.client.json"), "APP");
            Assert.True(result);
        }
        [Fact]
        public void TestValidGlobalConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.global.json", GetFile(@"\Resources\config.global.json"), "APP");
            Assert.True(result);
        }

        [Fact]
        public void TestValidServerConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.server.json", GetFile(@"\Resources\config.server.json"), "APP");
            Assert.True(result);
        }

        [Fact]
        public void TestValidReplaceTooltipLabelConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.client.json", GetFile(@"\Resources\config.client.json"), "APP");
            Assert.True(result);
        }

        [Fact]
        public void TestValidClientConfigDemo()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.client.json", GetFile(@"\Resources\config.client.json"), "APP");
            Assert.True(result);
        }

        [Fact]
        public void TestValidNoSocialURL()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.client.json", GetFile(@"\Resources\config.client.json"), "APP");
            Assert.True(result);
        }

        private string GetFile(string filename)
        {
            string workingDirectory = Environment.CurrentDirectory;

            // This will get the current PROJECT directory
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;
            return File.ReadAllText(projectDirectory + filename);
        }
    }
}
