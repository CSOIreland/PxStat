using API;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentValidation;
using PxStat.Resources;
using PxStat.Security;
using System;
using System.Linq;
using System.Reflection;

namespace PxStat.Template
{
    /// <summary>
    /// Base Abstract class to allow for the template method pattern of Create, Read, Update and Delete objects from our model
    /// 
    /// </summary>
    public abstract class BaseTemplate_Create<T, V> : BaseTemplate<T, V>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        /// <param name="validator"></param>
        public BaseTemplate_Create(JSONRPC_API request, IValidator<T> validator) : base(request, validator)
        {
        }

        /// <summary>
        /// Execution Success
        /// </summary>
        protected override void OnExecutionSuccess()
        {
            Log.Instance.Debug("Record created");
            //See if there's a cache in the process. If so then we need to flush the cache.
            if (CustomAttributeNames.Contains("PxStat.CacheFlush"))
            {
                cDTO = new CacheMetadata("CacheFlush", Request.method, DTO);
                foreach (Cas cas in cDTO.CasList)Cas.RunCasFlush(cas.CasRepository + cas.Domain);
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
                //Run the parameters through the cleanse process
                dynamic cleansedParams;


                //If the API has the IndividualCleanseNoHtml attribute then parameters are cleansed individually
                //Any of these parameters whose corresponding DTO property contains the NoHtmlStrip attribute will not be cleansed of HTML tags

                bool isKeyValueParameters = Resources.Cleanser.TryParseJson<dynamic>(Request.parameters.ToString(), out dynamic canParse);

                if (CustomAttributeNames.Contains("PxStat.NoCleanseDto"))
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
                        if (CustomAttributeNames.Contains("PxStat.IndividualCleanseNoHtml"))
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
                    OnDTOValidationError(CustomAttributeNames.Contains("PxStat.TokenSecure"));                  
                    return this;
                }


                // first of all, we check if user has the right to perform this operation!
                
                if (HasUserToBeAuthenticated()) 
                {
                    if (!IsUserAuthenticated( ) || !HasUserPrivilege())
                    {
                        OnAuthenticationFailed();
                        return this;
                    }
                }


                if (CustomAttributeNames.Contains("PxStat.NoDemo") && Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "security.demo"))
                {
                    if (!IsAdministrator() && !CustomAttributeNames.Contains("PxStat.TokenSecure"))
                    {
                        OnAuthenticationFailed();
                        return this;
                    }
                }

                //HEAD requests are not allowed on action queries
                if (Request.method.Equals("HEAD"))
                {
                    OnAuthenticationFailed();
                    return this;
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
                


                Ado.StartTransaction();

                // Create the trace now that we're sure we have a SamAccountName if it exists
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
