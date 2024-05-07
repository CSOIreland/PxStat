using Xunit;
using PxStat.Security;
using API;
using PxStatCore.Test;

namespace PxStatXUnit.Tests
{

    [Collection("PxStatXUnit")]
    public class Throttle_Test
    {

        [Fact]
        public void TestIsNotInWhitelist()
        {
            Helper.SetupTests();
            string? DataUrl = ApiServicesHelper.Configuration.GetSection("Test:DataUrl").Value;
            string? VisualUrl = ApiServicesHelper.Configuration.GetSection("Test:VisualUrl").Value;
            string[] whitelist = { "test.ie" };
            string host = "localhost";

            Assert.True(Throttle_BSO.IsNotInTheWhitelist(whitelist, host));

            host = "test.ie";
            Assert.False(Throttle_BSO.IsNotInTheWhitelist(whitelist, host));

            host = DataUrl;
            Assert.False(Throttle_BSO.IsNotInTheWhitelist(whitelist, host));

            host = VisualUrl;
            Assert.False(Throttle_BSO.IsNotInTheWhitelist(whitelist, host));

            host = null;
            Assert.True(Throttle_BSO.IsNotInTheWhitelist(whitelist, host));
        }

    }

}
