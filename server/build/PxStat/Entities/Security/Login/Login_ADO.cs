using API;
using System;
using System.Collections.Generic;

namespace PxStat.Security
{

    internal class Login_ADO
    {
        IADO ado;
        internal Login_ADO(IADO Ado)
        {
            ado = Ado;
        }
        internal int CreateLogin(Login_DTO_Create dto, string samAccountName, string token = null)
        {

            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@CcnUsernameCreator",value=samAccountName },
                new ADO_inputParams() {name= "@CcnUsername",value=dto.CcnUsername},

            };

            if (token != null)
                inputParamList.Add(new ADO_inputParams() { name = "@LgnToken1FA", value = token });


            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Security_Login_Create", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }


        internal int Logout(string session)
        {
            List<ADO_inputParams> paramList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@LgnSession",value=session  }
            };

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            ado.ExecuteNonQueryProcedure("Security_Login_Logout", paramList, ref retParam);

            return retParam.value;
        }

        internal int Update1FA(Login_DTO_Create1FA dto, string NewToken)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@LgnToken1FA",value=dto.LgnToken1Fa  },
                new ADO_inputParams() {name= "@Lgn1FA",value=Utility.GetSHA256(dto.Lgn1Fa) },
                new ADO_inputParams() {name= "@LgnNewToken",value=NewToken },
                new ADO_inputParams() {name= "@CcnEmail",value=dto.CcnEmail }
            };


            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Security_Login_Update1FA", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }


        internal int Update1FaTokenForUser(string CcnUsername, string token)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
               new ADO_inputParams() {name= "@CcnUsername",value=CcnUsername  },
                new ADO_inputParams() {name= "@LgnNewToken1FA",value=token}
            };

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Security_Login_UpdateToken1FA", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }



        internal bool ReadOpen1Fa(string ccnEmail)
        {
            List<ADO_inputParams> paramList = new List<ADO_inputParams>();
            paramList.Add(new ADO_inputParams() { name = "@CcnEmail", value = ccnEmail });

            return ado.ExecuteReaderProcedure("Security_Login_ReadOpen1Fa", paramList).hasData;
        }

        internal bool ReadOpen2Fa(string ccnUsername)
        {
            List<ADO_inputParams> paramList = new List<ADO_inputParams>();
            paramList.Add(new ADO_inputParams() { name = "@CcnUsername", value = ccnUsername });

            return ado.ExecuteReaderProcedure("Security_Login_ReadOpen2Fa", paramList).hasData;
        }

        internal ADO_readerOutput ReadBy1FaToken(string token, string ccnUsername)
        {

            List<ADO_inputParams> paramList = new List<ADO_inputParams>();
            paramList.Add(new ADO_inputParams() { name = "@LgnToken1Fa", value = token });

            paramList.Add(new ADO_inputParams() { name = "@CcnUsername", value = ccnUsername });

            return ado.ExecuteReaderProcedure("Security_Login_ReadBy1FaToken", paramList);

        }

        internal ADO_readerOutput ReadBy2FaToken(string token, string ccnUsername)
        {

            List<ADO_inputParams> paramList = new List<ADO_inputParams>();
            paramList.Add(new ADO_inputParams() { name = "@LgnToken2Fa", value = token });
            paramList.Add(new ADO_inputParams() { name = "@CcnUsername", value = ccnUsername });

            return ado.ExecuteReaderProcedure("Security_Login_ReadBy2FaToken", paramList);

        }


        internal ADO_readerOutput ReadBySession(string token)
        {

            List<ADO_inputParams> paramList = new List<ADO_inputParams>();
            paramList.Add(new ADO_inputParams() { name = "@LgnSession", value = token });

            return ado.ExecuteReaderProcedure("Security_Login_ReadBySession", paramList);

        }


        internal int Update2FA(Login_DTO_Create2FA dto, string lgn2FA)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@LgnToken2FA",value=dto.LgnToken2Fa},
                new ADO_inputParams() {name="@Lgn2FA",value=lgn2FA },
                new ADO_inputParams() {name="@CcnUsername",value=dto.CcnUsername  }
            };



            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Security_Login_Update2FA", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }
        internal int UpdateInvitationToken2Fa(string ccnUsername, string token)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@CcnUsername",value=ccnUsername},
                new ADO_inputParams() {name="@LgnToken2FA",value=token }
            };



            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Security_Login_Update2FaToken", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }



        internal ADO_readerOutput Validate1Fa(string CcnUsername, string Login1Fa)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@CcnUsername",value=CcnUsername},
                new ADO_inputParams() {name="@Lgn1FA",value= Utility.GetSHA256(Login1Fa) }
            };



            return ado.ExecuteReaderProcedure("Security_Login_Authenticate1FA", inputParamList);

        }



        internal bool ExtendSession(string CcnUsername, DateTime LgnSessionExpiry)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@CcnUsername",value=CcnUsername},
                new ADO_inputParams() {name="@LgnSessionExpiry",value=LgnSessionExpiry}
            };



            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Security_Login_ExtendSession", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value > 0;
        }

        internal bool CreateSession(string LgnSession, DateTime LgnSessionExpiry, string CcnUsername)
        {
            //Security_Login_CreateSession
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@LgnSession",value=LgnSession},
                new ADO_inputParams() {name="@LgnSessionExpiry",value=LgnSessionExpiry },
                new ADO_inputParams() {name="@CcnUsername",value=CcnUsername }
            };



            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Security_Login_CreateSession", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value > 0;
        }



    }
}
