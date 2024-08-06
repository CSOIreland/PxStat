using API;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.HttpOverrides;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PxStat.Data;
using PxStat.Resources;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace PxStat.Template
{
    /// <summary>
    /// Base Abstract class to allow for the template method pattern of Create, Read, Update and Delete objects from our model
    /// 
    /// </summary>
    public abstract class BaseTemplate<T, V>
    {
        #region Properties
        /// <summary>
        /// IADO variable
        /// </summary>
        public IADO Ado { get; }

        /// <summary>
        /// Cache related metadata
        /// </summary>
        public CacheMetadata cDTO { get; set; }

        /// <summary>
        /// Account username
        /// </summary>
        public string SamAccountName { get; set; }

        /// <summary>
        /// Request passed into the API
        /// </summary>
        public IRequest Request { get; }

        /// <summary>
        /// Response passed back by API
        /// </summary>
        public IResponseOutput Response { get; set; }

        /// <summary>
        /// DTO created from request parameters
        /// </summary>
        protected T DTO { get; set; }

        /// <summary>
        /// Validator (Fluent Validation)
        /// </summary>
        protected IValidator<T> Validator { get; }

        /// <summary>
        /// Validation result
        /// </summary>
        protected ValidationResult DTOValidationResult { get; set; }

        protected AuthenticationType AuthenticationType { get; set; }

        protected string UserIdentifier { get; set; }

        public IList<string> CustomAttributeNames { get; set; }
       

        #endregion 

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        /// <param name="validator"></param>
        public  BaseTemplate(IRequest request, IValidator<T> validator)
        {

            Ado = AppServicesHelper.StaticADO;
            

            if (ActiveDirectory.IsAuthenticated(request.userPrincipal))
            {
                SamAccountName = request.userPrincipal.SamAccountName.ToString();
            }

            Request = request;
            if (Request.method.Equals("HEAD"))
            {
                Response = new RESTful_Output();
            }
            else
                Response = new JSONRPC_Output();
            Validator = validator;

            //Save some Reflection effort by getting all custom attributes together
            CustomAttributeNames=Resources.MethodReader.GetAllCustomAttributeNamesForMethod(Request.method);  
            

        }

        /// <summary>
        /// Dispose of the IADO for connection tidy-up
        /// </summary>
        protected void Dispose()
        {


            if (SamAccountName != null && AuthenticationType == AuthenticationType.local)
            {

                Login_BSO.ExtendSession(Ado, SamAccountName);

            }

            // Dispose the IADO
            Ado.Dispose();

            // To be reviewed... 
            PxStat.RequestLanguage.LngIsoCode = null;
        }

        /// <summary>
        /// Test if the current user is a moderator
        /// </summary>
        /// <returns></returns>
        protected bool IsModerator()
        {
            Account_BSO acBso = new Account_BSO();
            return acBso.IsModerator(Ado, SamAccountName);
        }

        /// <summary>
        /// Test if the user is a moderator
        /// </summary>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        protected bool IsModerator(string ccnUsername)
        {
            Account_BSO acBso = new Account_BSO();
            return acBso.IsModerator(Ado, ccnUsername);
        }

        /// <summary>
        /// Test if the current user is a power user
        /// </summary>
        /// <returns></returns>
        protected bool IsPowerUser()
        {
            Account_BSO acBso = new Account_BSO();
            return acBso.IsPowerUser(Ado, SamAccountName);
        }

        /// <summary>
        /// Test if any user is a power user
        /// </summary>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        protected bool IsPowerUser(string ccnUsername)
        {
            Account_BSO acBso = new Account_BSO();
            return acBso.IsPowerUser(Ado, ccnUsername);
        }

        /// <summary>
        /// Tests if the current user is an administrator
        /// </summary>
        /// <returns></returns>
        protected bool IsAdministrator()
        {
            Account_BSO acBso = new Account_BSO();
            return acBso.IsAdministrator(Ado, SamAccountName);
        }

        /// <summary>
        /// Tests if any user is an administrator
        /// </summary>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        protected bool IsAdministrator(string ccnUsername)
        {
            Account_BSO acBso = new Account_BSO();
            return acBso.IsAdministrator(Ado, ccnUsername);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool IsApprover()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tests if the current user has user privilege
        /// </summary>
        /// <returns></returns>
        protected bool HasUserPrivilege()
        {

            if (IsAdministrator() || HasPrivilege())
            {
                OnPrivilegeSuccess();
                return true;
            }

            OnPrivilegeError();
            return false;
        }

        public virtual bool PostExecute()
        {
            return true;
        }

        #region Abstract methods.
        // These methods must be overriden.
        abstract protected bool Execute();

        /// <summary>
        /// Return the current DTO
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        virtual protected T GetDTO(dynamic parameters = null)
        {
            dynamic copyParams;
            try
            {
                // Mapster would be better but autmapper used in HSM
                copyParams = parameters ?? Request.parameters;
                //ExpandoObject? mappedParamers = ((JObject)copyParams).ToObject<ExpandoObject>();
                return AutoMap.Mapper.Map<T>(copyParams);
            }
            catch (Exception ex)
            {
                Log.Instance.ErrorFormat("GetDTO error for {0}", typeof(T));
                if (ex.InnerException != null)
                {
                    if (ex.InnerException is FormatException)
                    {
                        throw ex.InnerException;
                    }
                    else
                    {
                        Log.Instance.Error(ex.InnerException);
                    }
                }
                else
                {
                    // throw ex;
                    return parameters != null ?
                    (T)Activator.CreateInstance(typeof(T), parameters) :
                    (T)Activator.CreateInstance(typeof(T), Request.parameters);
                }

                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        abstract protected void OnExecutionSuccess();

        /// <summary>
        /// 
        /// </summary>
        abstract protected void OnExecutionError();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        abstract protected bool HasPrivilege();

        #endregion

        #region Default methods.
        // These methods can be left as their default implementations.

        /// <summary>
        /// Validation fail
        /// </summary>
        virtual protected void OnDTOValidationError(bool isMachineReadable=false)
        {
            //parameter validation not ok - return an error and proceed no further
            Log.Instance.Debug("Validation failed: " + JsonConvert.SerializeObject(DTOValidationResult.Errors));

            if (!isMachineReadable)
            {
                Response.error = Label.Get("error.validation");
            }
            else
            {
                Response.error = DTOValidationResult.Errors;
            }
        }

        /// <summary>
        /// The request has been throttled for usage reasons
        /// </summary>
        virtual protected void OnThrottle()
        {
            Log.Instance.Debug("Request throttled");
            Response.error = Label.Get("error.throttled");
        }

        /// <summary>
        /// Privilege fail
        /// </summary>
        virtual protected void OnPrivilegeError()
        {
            //Security validation failed - return an error and proceed no further
            Log.Instance.Debug("Unauthorised access");
            Response.error = Label.Get("error.privilege");
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnAuthenticationSuccessful() { }

        virtual protected void OnAuthenticationFailed()
        {
            //Security validation failed - return an error and proceed no further
            Log.Instance.Debug("Unauthorised access");
            Response.error = Label.Get("error.authentication");
        }

        /// <summary>
        /// 
        /// </summary>
        virtual protected void OnPrivilegeSuccess()
        {
            Log.Instance.Debug("Valid privilege");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        virtual protected bool HasUserToBeAuthenticated()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        virtual protected bool IsUserAuthenticated()
        {
            if (Request.userPrincipal != null)
            {
                if (!ActiveDirectory.IsAuthenticated(Request.userPrincipal))
                {

                    OnAuthenticationFailed();
                    return false;
                }

                //check in case the account is locked
                Account_ADO aAdo = new Account_ADO();

                ADO_readerOutput response = aAdo.Read(Ado, new Account_DTO_Read() { CcnUsername = Request.userPrincipal.SamAccountName });
                if (!response.hasData)
                {
                    OnAuthenticationFailed();
                    return false;
                }
                if (response.data[0].CcnLockedFlag)
                {
                    OnAuthenticationFailed();
                    return false;
                }
                AuthenticationType = AuthenticationType.windows;
            }
            else if (!string.IsNullOrEmpty(Request.sessionCookie.Value) )
            {
                //This may be application authenticated, let's check..

                Response.error = null;


                    if (!String.IsNullOrEmpty(Request.sessionCookie.Value))
                    {

                        //Does the cookie correspond with a live token for a user? If so then return the user.


                        ADO_readerOutput user;
                        using (Login_BSO lBso = new Login_BSO())
                        {
                            user = lBso.ReadBySession(Request.sessionCookie.Value);
                            if (!user.hasData)
                            {

                                Response.error = Label.Get("error.authentication"); ;
                                return false;
                            }
                            else
                            {
                                SamAccountName = user.data[0].CcnUsername;
                                if (!HasUserPrivilege()) return false;
                            }
                        }



                        AuthenticationType = AuthenticationType.local;
                    }
                    else return false;
               

            }
            else if(CustomAttributeNames.Contains("PxStat.TokenSecure"))
            {
                //This may be a token verified API.

                //We're not authenticated conventionally, so there must be an apiToken in the Request header
                string apiToken = Request.requestHeaders["aprtoken"];
                string ccnUsername= Request.requestHeaders["ccnusername"];

                if (apiToken == null || ccnUsername == null) return false;

                //Check the calling ip is from an allowed subnet
                if (!IsIpAddressOnWhitelist(Request.ipAddress))
                    return false;

                //We must validate that the CcnUsername and token validate against the account data
                Account_ADO account_ADO = new Account_ADO();
                var response = account_ADO.ReadByApiTokenCcnUsername(Ado, ccnUsername, apiToken);
                if(response!=null)
                {
                    if(response.hasData)
                    {
                        SamAccountName = response.data[0].CcnUsername;
                       
                        return true;
                    }
                    return false;
                }
                return false;
            }
        
            else
            {
                return false;
            }

            OnAuthenticationSuccessful();
            return true;
        }

        public bool IsIpAddressOnWhitelist(string ipAddress)
        {
            IPNetwork ipnetwork;
            JArray addrList = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "security.tokenApiAccessIpMaskWhitelist");
            foreach (var item in addrList)
            {
                ipnetwork = new IPNetwork(IPAddress.Parse((string)item["prefix"]), (int)item["length"]);
                if (ipnetwork.Contains(IPAddress.Parse(ipAddress))) return true;
            }
            return false;
        }

    }

    public class InputFormatException : Exception
    {
        public InputFormatException() : base("Invalid format found in input parameters")
        {

        }
    }
}
#endregion