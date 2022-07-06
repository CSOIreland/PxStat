using API;
using FluentValidation;
using PxStat.Resources;
using PxStat.Security;
using System;

namespace PxStat.Template
{
    /// <summary>
    /// Base Abstract class to allow for the template method pattern of Create, Read, Update and Delete objects from our model
    /// 
    /// </summary>
    internal abstract class BaseTemplate_Create<T, V> : BaseTemplate<T, V>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        /// <param name="validator"></param>
        protected BaseTemplate_Create(JSONRPC_API request, IValidator<T> validator) : base(request, validator)
        {
        }

        /// <summary>
        /// Execution Success
        /// </summary>
        protected override void OnExecutionSuccess()
        {
            Log.Instance.Debug("Record created");
            //See if there's a cache in the process. If so then we need to flush the cache.
            if (MethodReader.MethodHasAttribute(Request.method, "CacheFlush"))
            {
                cDTO = new CacheMetadata("CacheFlush", Request.method, DTO);
                foreach (Cas cas in cDTO.CasList) MemCacheD.CasRepositoryFlush(cas.CasRepository + cas.Domain);
            }
        }

        /// <summary>
        /// Execution Error
        /// </summary>
        protected override void OnExecutionError()
        {
            Log.Instance.Debug("No record created");
        }

        /// <summary>
        /// Constructio
        /// </summary>
        /// <returns></returns>
        public BaseTemplate_Create<T, V> Create()
        {
            try
            {
                //HEAD requests are not allowed on action queries
                if (Request.GetType().Equals(typeof(Head_API)))
                {
                    OnAuthenticationFailed();
                    return this;
                }

                // first of all, we check if user has the right to perform this operation!
                if (HasUserToBeAuthenticated())
                {
                    if (!IsUserAuthenticated() || !HasUserPrivilege())
                    {

                        OnAuthenticationFailed();
                        return this;
                    }
                }
                //if we didn't attempt to authenticate and it's an external call then we still need to the the SamAccountName
                if (SamAccountName == null && Request.sessionCookie != null)
                {
                    using (var lBso = new Login_BSO())
                    {
                        //Does the cookie correspond with a live token for a user? 

                        var user = lBso.ReadBySession(Request.sessionCookie.Value);
                        if (user.hasData)
                        {

                            SamAccountName = user.data[0].CcnUsername;
                        }
                    }
                }
                //Run the parameters through the cleanse process
                dynamic cleansedParams;

                //If the API has the IndividualCleanseNoHtml attribute then parameters are cleansed individually
                //Any of these parameters whose corresponding DTO property contains the NoHtmlStrip attribute will not be cleansed of HTML tags
                if (Resources.MethodReader.MethodHasAttribute(Request.method, "IndividualCleanseNoHtml"))
                {
                    dynamic dto = GetDTO(Request.parameters);
                    cleansedParams = Cleanser.Cleanse(Request.parameters, dto);
                }
                else
                {
                    cleansedParams = Cleanser.Cleanse(Request.parameters);
                }
                try
                {
                    DTO = GetDTO(cleansedParams);
                }
                catch
                {
                    throw new InputFormatException();
                }

                DTO = Sanitizer.Sanitize(DTO);

                DTOValidationResult = Validator.Validate(DTO);

                if (!DTOValidationResult.IsValid)
                {
                    OnDTOValidationError();
                    return this;
                }

                Ado.StartTransaction();

                // The Actual Creation should happen here by the specific class!
                if (!Execute())
                {

                    Ado.RollbackTransaction();
                    OnExecutionError();
                }
                else
                {

                    Ado.CommitTransaction();
                    OnExecutionSuccess();
                }

                return this;
            }
            catch (FormatException formatException)
            {
                //A FormatException error has been caught, rollback the transaction, log the error and return a message to the caller
                Ado.RollbackTransaction();
                Log.Instance.Error(formatException);
                Response.error = Label.Get("error.schema");
                return this;
            }
            catch (InputFormatException inputError)
            {
                //An error has been caught, rollback the transaction, log the error and return a message to the caller
                Ado.RollbackTransaction();
                Log.Instance.Error(inputError);
                Response.error = Label.Get("error.schema");
                return this;
            }
            catch (Exception ex)
            {
                //An error has been caught, rollback the transaction, log the error and return a message to the caller
                Ado.RollbackTransaction();
                Log.Instance.Error(ex);
                Response.error = Label.Get("error.exception");
                return this;
            }
            finally
            {

                Dispose();
            }
        }
    }
}
