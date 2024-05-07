using Xunit;
using PxStat.Security;
using PxStat.Resources;
using PxStatCore.Test;
using PxStat.Subscription;
using PxStat;
using API;
using PxStat.Workflow;
using Newtonsoft.Json.Linq;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace PxStatXUnit.Tests
{
    [Collection("PxStatXUnit")]
    public class LocalUserTests
    {
        [Fact]
        public void TestCreateLocal()
        {
            Helper.SetupTests();
            Dictionary<string, object> param = new()
            {
                {"CcnEmail","joemurphy@abc.def" },
                {"CcnDisplayName","Joe Murphy" },
                {"PrvCode","ADMINISTRATOR" },
                {"CcnNotificationFlag",true },
                {"LngIsoCode","en" },
                {"CcnUsername","joemurphy@abc.def" }
            };

            JSONRPC_API request = Helper.GetRequest("PxStat.Security.Account_API.CreateLocal", param);
            var result = new Account_BSO_CreateLocal(request).Create().Response;

            //New local account has been created
            Assert.True(result.data.Equals(ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"]));

            param = new()
            {
                {"CcnUsername","joemurphy@abc.def" }
            };

            request = Helper.GetRequest("PxStat.Security.Account_API.Read", param);
            result = new Account_BSO_Read(request).Read().Response;

            //We can read the newly created account
            Assert.True(result.data[0].CcnEmail.Equals("joemurphy@abc.def"));

            param = new()
            {
                {"CcnUsername","joemurphy@abc.def" }
            };

            request = Helper.GetRequest("PxStat.Security.Account_API.Delete", param);
            result = new Account_BSO_Delete(request).Delete().Response;

            //New account has been deleted
            Assert.True(result.data.Equals(ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"]));

            param = new()
            {
                {"CcnUsername","joemurphy@abc.def" }
            };

            request = Helper.GetRequest("PxStat.Security.Account_API.Read", param);
            result = new Account_BSO_Read(request).Read().Response;

            //Deleted account cannot be read
            Assert.Null(result.data);
        }

       
        public void ReadCreatedLocal()
        {
            Helper.SetupTests();
            Dictionary<string, object> param = new()
            {
                {"CcnUsername","joemurphy@abc.def" }
            };

            JSONRPC_API request = Helper.GetRequest("PxStat.Security.Account_API.Read", param);
            var result = new Account_BSO_Read(request).Read().Response;
            Assert.True(result.data[0].CcnEmail.Equals("joemurphy@abc.def"));
        }


        
        public void TestDeleteLocal()
        {
            Helper.SetupTests();
            Dictionary<string, object> param = new()
            {
                {"CcnUsername","joemurphy@abc.def" }
            };

            JSONRPC_API request = Helper.GetRequest("PxStat.Security.Account_API.Delete", param);
            var result = new Account_BSO_Delete(request).Delete().Response;

            Assert.True(result.data.Equals(ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"]));
        }

    
        public void ReadDeletedLocal()
        {
            Helper.SetupTests();
            Dictionary<string, object> param = new()
            {
                {"CcnUsername","joemurphy@abc.def" }
            };

            JSONRPC_API request = Helper.GetRequest("PxStat.Security.Account_API.Read", param);
            var result = new Account_BSO_Read(request).Read().Response;;
            Assert.Null(result.data);
        }




    }
}
