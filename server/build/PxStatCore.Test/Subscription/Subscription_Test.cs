using Xunit;
using PxStat.Security;
using PxStat.Resources;
using PxStatCore.Test;
using PxStat.Subscription;
using PxStat;

namespace PxStatXUnit.Tests
{
    [Collection("PxStatXUnit")]
    public class Subscription_Test
    {
        [Fact]
        public void SubscriptionRead_Basic()
        {
            Helper.SetupTests();
            Subscription_BSO sbso = new Subscription_BSO();
            var result=sbso.TableRead(AppServicesHelper.StaticADO);
            Assert.True(result.Count>=0);
        }

        [Fact]
        public void FirebaseHasAtLeastOneAccount()
        {
            Helper.SetupTests();
            var list=AppServicesHelper.Firebase.GetAllUsers();
            Assert.True(list.Count > 0);
        }

    }
}
