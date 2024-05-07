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
    public abstract class BaseTemplate_Delete<T, V> : BaseTemplate<T, V>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        /// <param name="validator"></param>
        protected BaseTemplate_Delete(JSONRPC_API request, IValidator<T> validator) : base(request, validator)
        {
        }

        /// <summary>
        /// On success
        /// </summary>
        protected override void OnExecutionSuccess()
        {
            Log.Instance.Debug("Record deleted");
            //See if there's a cache in the process. If so then we need to flush the cache.
            if (Resources.MethodReader.MethodHasAttribute(Request.method, Configuration_BSO.GetStaticConfig("APP_CACHE_FLUSH_ATTRIBUTE")))
            {
                cDTO = new CacheMetadata(Configuration_BSO.GetStaticConfig("APP_CACHE_FLUSH_ATTRIBUTE"), Request.method, DTO);
                foreach (Cas cas in cDTO.CasList)Cas.RunCasFlush(cas.CasRepository + cas.Domain);

            }
        }

        /// <summary>
        /// On error
        /// </summary>
        protected override void OnExecutionError()
        {
            Log.Instance.Debug("No record deleted");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <returns></returns>
        public BaseTemplate_Delete<T, V> Delete()
        {
            //HEAD requests are not allowed on action queries
            if (Request.method.Equals("HEAD"))
            {
                OnAuthenticationFailed();
                return this;
            }

            try
            {
                if (Resources.MethodReader.MethodHasAttribute(Request.method, "NoDemo") && Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "security.demo"))
                {
                    if (!IsAdministrator())
                    {
                        OnAuthenticationFailed();
                        return this;
                    }
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
                dynamic cleansedParams;

                //If the API has the IndividualCleanseNoHtml attribute then parameters are cleansed individually
                //Any of these parameters whose corresponding DTO property contains the NoHtmlStrip attribute will not be cleansed of HTML tags
                //if (Resources.MethodReader.MethodHasAttribute(Request.method, "IndividualCleanseNoHtml"))
                //{
                //    dynamic dto = GetDTO(Request.parameters);
                //    cleansedParams = Cleanser.Cleanse(Request.parameters, dto);
                //}
                //else
                //{
                //    cleansedParams = Cleanser.Cleanse(Request.parameters);
                //}

                bool isKeyValueParameters = Resources.Cleanser.TryParseJson<dynamic>(Request.parameters.ToString(), out dynamic canParse);

                if (Resources.MethodReader.MethodHasAttribute(Request.method, "NoCleanseDto"))
                {
                    cleansedParams = Request.parameters;
                }
                else
                {
                    if (!isKeyValueParameters)
                    {
                        cleansedParams = Resources.Cleanser.Cleanse(Request.parameters);
                    }
                    else
                    {
                        if (Resources.MethodReader.MethodHasAttribute(Request.method, "IndividualCleanseNoHtml"))
                        {
                            dynamic dto = GetDTO(Request.parameters);
                            cleansedParams = Resources.Cleanser.Cleanse(Request.parameters, dto);
                        }
                        else
                        {
                            cleansedParams = Resources.Cleanser.Cleanse(Request.parameters);
                        }
                    }
                }

                try
                {
                    DTO = GetDTO(cleansedParams);
                }
                catch
                {
                    throw new InputFormatException();
                }

                DTO = Resources.Sanitizer.Sanitize(DTO);

                DTOValidationResult = Validator.Validate(DTO);

                if (!DTOValidationResult.IsValid)
                {
                    OnDTOValidationError();
                    return this;
                }

                Ado.StartTransaction();

                //Create the trace now that we're sure we have a SamAccountName if it exists
                Trace_BSO_Create.Execute(Ado, Request, SamAccountName);

                // The Actual Creation should happen here by the specific class!
                if (!Execute())
                {
                    Ado.RollbackTransaction();
                    OnExecutionError();
                }
                Ado.CommitTransaction();

                if (!PostExecute())
                {
                    OnExecutionError();
                }
                OnExecutionSuccess();
                return this;
            }
            catch (FormatException formatException)
            {
                //A FormatException error has been caught, rollback the transaction, log the error and return a message to the caller
                Ado.RollbackTransaction();
                Log.Instance.Error(formatException);
                Response.error = formatException.Message ?? Label.Get("error.schema");
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
