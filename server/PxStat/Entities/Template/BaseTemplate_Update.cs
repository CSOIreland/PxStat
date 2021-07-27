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
    internal abstract class BaseTemplate_Update<T, V> : BaseTemplate<T, V>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        /// <param name="validator"></param>
        protected BaseTemplate_Update(JSONRPC_API request, IValidator<T> validator) : base(request, validator)
        {
        }

        /// <summary>
        /// Success
        /// </summary>
        protected override void OnExecutionSuccess()
        {
            Log.Instance.Debug("Record updated");
            //See if there's a cache in the process. If so then we need to flush the cache.
            if (MethodReader.MethodHasAttribute(Request.method, "CacheFlush"))
            {
                cDTO = new CacheMetadata("CacheFlush", Request.method, DTO);
                foreach (Cas cas in cDTO.CasList) MemCacheD.CasRepositoryFlush(cas.CasRepository + cas.Domain);

            }
        }

        /// <summary>
        /// Error
        /// </summary>
        protected override void OnExecutionError()
        {
            Log.Instance.Debug("No record updated");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <returns></returns>
        public BaseTemplate_Update<T, V> Update()
        {
            try
            {

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

                    //Does the cookie correspond with a live token for a user? 
                    ADO_readerOutput user;
                    using (Login_BSO lBso = new Login_BSO())
                    {
                        user = lBso.ReadBySession(Request.sessionCookie.Value);
                        if (user.hasData)
                        {

                            SamAccountName = user.data[0].CcnUsername;
                        }
                    }


                }

                //Run the parameters through the cleanse process
                dynamic cleansedParams = Cleanser.Cleanse(Request.parameters);

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
                //An error has been caught, rollback the transaction, log the error and return a message to the caller
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
                Ado.RollbackTransaction();
                //An error has been caught, rollback the transaction, log the error and return a message to the caller
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
