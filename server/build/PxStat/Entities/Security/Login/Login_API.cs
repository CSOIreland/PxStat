using API;
using System.Diagnostics;

namespace PxStat.Security
{
    [AllowAPICall]
    public class Login_API
    {

        public static dynamic Create1FA(JSONRPC_API requestApi)
        {
            return new Login_BSO_Create1FA(requestApi, true).Create().Response;
        }

        public static dynamic Logout(JSONRPC_API requestApi)
        {
            return new Login_BSO_Logout(requestApi).Update().Response;
        }


        public static dynamic Create2FA(JSONRPC_API requestApi)
        {
            return new Login_BSO_Create2FA(requestApi).Create().Response;
        }


        public static dynamic Login(JSONRPC_API requestApi)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var v = new Login_BSO_Login(requestApi).Read().Response;
            sw.Stop();
            Log.Instance.Debug("Login elapsed milliseconds: " + sw.ElapsedMilliseconds);
            return v;
        }

        public static dynamic ReadOpen1FA(JSONRPC_API requestApi)
        {
            return new Login_BSO_ReadOpen1FA(requestApi).Read().Response;

        }

        public static dynamic ReadOpen2FA(JSONRPC_API requestApi)
        {
            return new Login_BSO_ReadOpen2FA(requestApi).Read().Response;
        }

        public static dynamic InitiateUpdate1FA_Current(JSONRPC_API requestApi)
        {
            return new Login_BSO_InitiateUpdate1FA_Current(requestApi).Update().Response;
        }

        public static dynamic InitiateUpdate2FA_Current(JSONRPC_API requestApi)
        {
            return new Login_BSO_InitiateUpdate2FA_Current(requestApi).Update().Response;
        }

        public static dynamic Update2FA(JSONRPC_API requestApi)
        {
            return new Login_BSO_Update2FA(requestApi).Update().Response;
        }

        public static dynamic InitiateUpdate1FA(JSONRPC_API requestApi)
        {
            return new Login_BSO_InitiateUpdate1FA(requestApi).Update().Response;
        }

        public static dynamic InitiateUpdate2FA(JSONRPC_API requestApi)
        {
            return new Login_BSO_InitiateUpdate2FA(requestApi).Update().Response;
        }


        public static dynamic InitiateForgotten1FA(JSONRPC_API requestApi)
        {
            return new Login_BSO_InitiateForgotten1FA(requestApi).Update().Response;
        }

        public static dynamic InitiateForgotten2FA(JSONRPC_API requestApi)
        {
            return new Login_BSO_InitiateForgotten2FA(requestApi).Update().Response;
        }

        public static dynamic UpdateForgotten1FA(JSONRPC_API requestApi)
        {
            return new Login_BSO_Update1FA(requestApi).Update().Response;
        }

        public static dynamic Update1FA(JSONRPC_API requestApi)
        {
            return new Login_BSO_Update1FA(requestApi).Update().Response;
        }


    }
}
