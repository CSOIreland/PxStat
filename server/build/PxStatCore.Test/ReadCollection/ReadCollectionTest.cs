using API;
using Moq;
using PxStat.Data;
using PxStat.System.Settings;

namespace PxStatCore.Test.ReadCollection
{
    [Collection("PxStatXUnit")]
    public class CubeBsoCreate_Test
    {
        [Fact]
        public void TestMultipleReadCollections()
        {
            Helper.SetupTests();
            var lastWeek = DateTime.Now.AddDays(-7).Date.ToShortDateString();
            Dictionary<string, object> p = new()
            {
                {"DateFrom","10/07/2024" },
            };

            var request = Helper.GetRequest("PxStat.Data.Cube_API.ReadCollection", p);
            var result = new Cube_BSO_ReadCollection(request).Read().Response;
            Assert.NotNull(result.data);
        }
    }
}
