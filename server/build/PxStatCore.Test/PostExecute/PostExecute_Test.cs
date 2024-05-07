using PxStatCore.Test;
using PxStat.System.Navigation;
using Moq;
using Moq.Protected;
using PxStat.Config;
using API;

namespace PxStatXUnit.Tests
{
    [Collection("PxStatXUnit")]
    public class PostExecute_Test : IDisposable
    {
        [Fact]
        public void Product_Update()
        {
            Helper.SetupTests();
            Dictionary<string, object> paramaters = new()
            {
                { "SbjCode", "1" },
                { "PrcCode", "OIIAATEST" },
                { "PrcValue", "Agriculture Output, Input and Income" },
                { "PrcCodeNew", "OIIAA" },
                { "LngIsoCode", "en" }
            };
            var request = Helper.GetRequest("PxStat.System.Navigation.Product_API.Update", paramaters);
            Product_BSO_Update pBso = new Product_BSO_Update(request);
            var result = pBso.Update();
            Assert.NotNull(result);
        }

        [Fact]
        public void Config_Create()
        {
            Helper.SetupTests();
            Dictionary<string, object> paramaters = new()
            {
                { "Type", "API" },
                { "Name", "API_TEST" },
                { "Value", "" }
            };
            var request = Helper.GetRequest("PxStat.Config.Config_API.Create", paramaters);
            var mock = new Mock<Config_BSO_Create>(request);
            mock.Protected().Setup<bool>("Execute").Returns(true); 
            mock.Setup<bool>(x => x.PostExecute()).Returns(true);
            var result = mock.Object.Create();
            Assert.NotNull(result);
        }

        [Fact]
        public void Config_Update()
        {
            Helper.SetupTests();
            Dictionary<string, object> paramaters = new()
            {
                { "Type", "API" },
                { "Name", "API_TEST" },
                { "Value", "" }
            };
            var request = Helper.GetRequest("PxStat.Config.Config_API.Update", paramaters);
            var mock = new Mock<Config_BSO_Create>(request);
            mock.Protected().Setup<bool>("Execute").Returns(true);
            mock.Setup<bool>(x => x.PostExecute()).Returns(true);
            var result = mock.Object.Create();
            Assert.NotNull(result);
        }
        public void Dispose()
        {
            // Revert update
            Dictionary<string, object> paramaters = new()
            {
                { "SbjCode", "1" },
                { "PrcCode", "OIIAATEST" },
                { "PrcValue", "Agriculture Output, Input and Income" },
                { "PrcCodeNew", "OIIAA" },
                { "LngIsoCode", "en" }
            };
            var request = Helper.GetRequest("PxStat.System.Navigation.Product_API.Update", paramaters);
            Product_BSO_Update pBso = new Product_BSO_Update(request);
        }
    }      
}
