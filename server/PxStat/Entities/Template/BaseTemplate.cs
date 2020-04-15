using API;
using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json;
using PxStat.Resources;
using PxStat.Security;
using System;

namespace PxStat.Template
{
    /// <summary>
    /// Base Abstract class to allow for the template method pattern of Create, Read, Update and Delete objects from our model
    /// 
    /// </summary>
    internal abstract class BaseTemplate<T, V>
    {
        #region Properties
        /// <summary>
        /// ADO variable
        /// </summary>
        protected ADO Ado { get; }

        /// <summary>
        /// Cache related metadata
        /// </summary>
        protected CacheMetadata cDTO { get; set; }

        /// <summary>
        /// Account username
        /// </summary>
        protected string SamAccountName { get; }

        /// <summary>
        /// Request passed into the API
        /// </summary>
        protected JSONRPC_API Request { get; }

        /// <summary>
        /// Response passed back by API
        /// </summary>
        public JSONRPC_Output Response { get; set; }

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

        #endregion 

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        /// <param name="validator"></param>
        protected BaseTemplate(JSONRPC_API request, IValidator<T> validator)
        {
            Ado = new ADO("defaultConnection");

            if (ActiveDirectory.IsAuthenticated(request.userPrincipal))
            {
                SamAccountName = request.userPrincipal.SamAccountName.ToString();
            }

            Request = request;
            Response = new JSONRPC_Output();
            Validator = validator;
            Trace_BSO_Create.Execute(Ado, request);

        }

        /// <summary>
        /// Dispose of the ADO for connection tidy-up
        /// </summary>
        protected void Dispose()
        {
            Ado.Dispose();
            PxStat.RequestLanguage.LngIsoCode = null;
        }

        /// <summary>
        /// Test if the current user is a moderator
        /// </summary>
        /// <returns></returns>
        protected bool IsModerator()
        {

            return Account_BSO_Read.IsModerator(Ado, SamAccountName);
        }

        /// <summary>
        /// Test if the user is a moderator
        /// </summary>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        protected bool IsModerator(string ccnUsername)
        {

            return Account_BSO_Read.IsModerator(Ado, ccnUsername);
        }

        /// <summary>
        /// Test if the current user is a power user
        /// </summary>
        /// <returns></returns>
        protected bool IsPowerUser()
        {
            return Account_BSO_Read.IsPowerUser(Ado, SamAccountName);
        }

        /// <summary>
        /// Test if any user is a power user
        /// </summary>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        protected bool IsPowerUser(string ccnUsername)
        {
            return Account_BSO_Read.IsPowerUser(Ado, ccnUsername);
        }

        /// <summary>
        /// Tests if the current user is an administrator
        /// </summary>
        /// <returns></returns>
        protected bool IsAdministrator()
        {

            return Account_BSO_Read.IsAdministrator(Ado, SamAccountName);
        }

        /// <summary>
        /// Tests if any user is an administrator
        /// </summary>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        protected bool IsAdministrator(string ccnUsername)
        {
            return Account_BSO_Read.IsAdministrator(Ado, ccnUsername);
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
            try
            {
                if (parameters != null)
                {
                    return (T)Activator.CreateInstance(typeof(T), parameters);
                }
                else return (T)Activator.CreateInstance(typeof(T), Request.parameters);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException is FormatException)
                    {
                        throw ex.InnerException;
                    }
                }

                throw ex;
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
        virtual protected void OnDTOValidationError()
        {
            //parameter validation not ok - return an error and proceed no further
            Log.Instance.Debug("Validation failed: " + JsonConvert.SerializeObject(DTOValidationResult.Errors));
            Response.error = Label.Get("error.validation");
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
            if (!ActiveDirectory.IsAuthenticated(Request.userPrincipal))
            {

                OnAuthenticationFailed();
                return false;
            }

            OnAuthenticationSuccessful();
            return true;
        }
        #endregion
    }
}
