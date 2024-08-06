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
            bool result = validationRules.ValidateCreateConfig("config.client.json", GetFile(@"\Resources\test1.json"), "APP");
            Assert.True(result);
        }

        [Fact]
        public void TestValidFirebaseConfigClientConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("FirebaseKey.json", GetFile(@"\Resources\test2.json"), "APP");
            Assert.True(result);
        }

        [Fact]
        public void TestValidChartOptionsClientConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.client.json", GetFile(@"\Resources\test3.json"), "APP");
            Assert.True(result);
        }

        [Fact]
        public void TestValidDatatableClientConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.client.json", GetFile(@"\Resources\test4.json"), "APP");
            Assert.True(result);
        }

        [Fact]
        public void TestInvalidColoursClientConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.client.json", GetFile(@"\Resources\test5.json"), "APP");
            Assert.False(result);
        }

        [Fact]
        public void TestValidFirebaseConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("FirebaseKey.json", GetFile(@"\Resources\test6.json"), "APP");
            Assert.True(result);
        }

        [Fact]
        public void TestInvalidFirebaseConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("FirebaseKey.json", GetFile(@"\Resources\test7.json"), "APP");
            Assert.False(result);
        }

        [Fact]
        public void TestValidGlobalConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.global.json", GetFile(@"\Resources\test8.json"), "APP");
            Assert.True(result);
        }

        [Fact]
        public void TestInvalidGlobalConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.global.json", GetFile(@"\Resources\test9.json"), "APP");
            Assert.False(result);
        }

        [Fact]
        public void TestValidAnalyticalGlobalConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.global.json", GetFile(@"\Resources\test9.json"), "APP");
            Assert.False(result);
        }

        [Fact]
        public void TestValidServerConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.server.json", GetFile(@"\Resources\test10.json"), "APP");
            Assert.True(result);
        }

        [Fact]
        public void TestInvalidServerConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.server.json", GetFile(@"\Resources\test11.json"), "APP");
            Assert.False(result);
        }

        [Fact]
        public void TestInvalidSocialConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.client.json", GetFile(@"\Resources\test12.json"), "APP");
            Assert.False(result);
        }

        [Fact]
        public void TestInvalidIsoCodeConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.client.json", GetFile(@"\Resources\test13.json"), "APP");
            Assert.False(result);
        }

        [Fact]
        public void TestInvalidEmailClientConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.client.json", GetFile(@"\Resources\test14.json"), "APP");
            Assert.False(result);
        }

        [Fact]
        public void TestInvalidURLClientConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.client.json", GetFile(@"\Resources\test15.json"), "APP");
            Assert.False(result);
        }

        [Fact]
        public void TestInvalidTimeConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.global.json", GetFile(@"\Resources\test16.json"), "APP");
            Assert.False(result);
        }

        [Fact]
        public void TestValidReplaceTooltipLabelConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.client.json", GetFile(@"\Resources\test17.json"), "APP");
            Assert.True(result);
        }

        [Fact]
        public void TestInvalidReplaceTooltipLabelConfig()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.client.json", GetFile(@"\Resources\test18.json"), "APP");
            Assert.False(result);
        }

        [Fact]
        public void TestValidClientConfigDemo()
        {
            Config_VLD_Create validationRules = new Config_VLD_Create();
            bool result = validationRules.ValidateCreateConfig("config.client.json", GetFile(@"\Resources\test19.json"), "APP");
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
