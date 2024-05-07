using Xunit;
using PxStat.Security;
using API;
using PxStat.Resources;
using PxStatCore.Test;
using PxStat.Workflow;

namespace PxStatXUnit.Tests
{
    public  class ConfigurationTest
    {
        [Theory]
        [InlineData("workflow.embargo.time", "11:00:00", ConfigType.global)]
        [InlineData("build.import.moderator", true, ConfigType.global)]
        [InlineData("search.subject_word_multiplier", 100, ConfigType.server)]
        [InlineData("wrong.wrong", "wrong.wrong", ConfigType.server)]
        public void ConfigurationReadItem(string keyValue, dynamic valueValue,ConfigType configType)
        {
            Helper.InjectConfig();

            var val = Configuration_BSO.GetApplicationConfigItem(configType,keyValue);

            Assert.Equal(valueValue, val);
        }

        [Theory]
        [InlineData("APP_PX_DEFAULT_AXIS_VERSION", "2013")]
        public void ConfigurationReadStatic(string keyValue, dynamic valueValue)
        {
            Helper.InjectConfig();

            var val = Configuration_BSO.GetStaticConfig( keyValue);

            Assert.Equal(valueValue, val);
        }


    }
}
