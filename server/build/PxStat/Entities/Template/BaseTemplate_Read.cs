using AngleSharp.Io;
using API;
using FluentValidation;
using PxStat.Resources;
using PxStat.Security;
using System;
using System.Net;
using System.Web;
using PxStat;
using System.Reflection;
using Microsoft.AspNetCore.HttpOverrides;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http.Metadata;
using System.Linq;

namespace PxStat.Template
{
    /// <summary>
    /// Base Abstract class to allow for the template method pattern of Create, Read, Update and Delete objects from our model
    /// 
    /// </summary>
    public abstract class BaseTemplate_Read<T, V> : BaseTemplate<T, V>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        /// <param name="validator"></param>
        protected BaseTemplate_Read(IRequest request, IValidator<T> validator) : base(request, validator)
        {
        }

        /// <summary>
        /// Success
        /// </summary>
        protected override void OnExecutionSuccess()
        {

            Log.Instance.Debug("Record Read");
            if (!Enum.IsDefined(typeof(HttpStatusCode), Response.statusCode)) 
                Response.statusCode = HttpStatusCode.OK;
            
            //If a cache DTO exists, then we have been given a directive to store the results in the cache
            if (cDTO != null )
                AppServicesHelper.CacheD.Store_BSO<dynamic>(cDTO.Namespace, cDTO.ApiName, cDTO.Method, DTO, Response.data, cDTO.TimeLimit, cDTO.Cas + cDTO.Domain);

        }

        /// <summary>
        /// Error
        /// </summary>
        protected override void OnExecutionError()
        {
            if (!Enum.IsDefined(typeof(HttpStatusCode), Response.statusCode))
                Response.statusCode =HttpStatusCode.NotFound;
            Log.Instance.Debug("No record read");
        }

        protected void FormatForRestful(IResponseOutput output)
        {
            
            if (Request.GetType().Equals(typeof(RESTful_API)))
            {

                if (Response.error != null)
                {
                    if(Response.statusCode==0)
                        Response.statusCode =  HttpStatusCode.InternalServerError;
                    Response.data = RESTful.FormatRestfulError(Response, null, HttpStatusCode.NoContent, Response.statusCode);
                }
                else
                    if (!Enum.IsDefined(typeof(HttpStatusCode), Response.statusCode)) 
                        Response.statusCode = HttpStatusCode.OK; 
                //This is the default mimetype, may be amended before output by the calling BSO
                if (Response.mimeType == null)
                    Response.mimeType = Configuration_BSO.GetStaticConfig("APP_JSON_MIMETYPE");

                if (Response.data != null)
                {
                    Response.response = Response.data.ToString();                   
                    //We need to change e.g. an Excel file to a byte array
                    string data = Response.data.ToString();
                    if (data.Contains(";base64,"))
                    {
                        var base64Splits = data.Split(new[] { ";base64," }, StringSplitOptions.None);
                        var dataSplits = base64Splits[0].Split(new[] { "data:" }, StringSplitOptions.None);

                        // Override MimeType & Data
                        Response.mimeType = dataSplits[1];
                        Response.data = base64Splits[1];
                        Response.response = Utility.DecodeBase64ToByteArray(Response.data);
                    }
                }
                else
                {
                    Response.response = "";
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <returns></returns>
        public BaseTemplate_Read<T, V> Read()
        {
            try
            {
                
                //Run the parameters through the cleanse process
                dynamic cleansedParams;

                //If the API has the IndividualCleanseNoHtml attribute then parameters are cleansed individually
                //Any of these parameters whose corresponding DTO property contains the NoHtmlStrip attribute will not be cleansed of HTML tags

                bool isKeyValueParameters = Resources.Cleanser.TryParseJson<dynamic>(Request.parameters.ToString(), out dynamic canParse);

                if (API.MethodReader.MethodHasAttribute(Request.method, "NoCleanseDto"))
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
                        if (API.MethodReader.MethodHasAttribute(Request.method, "IndividualCleanseNoHtml"))
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

                if (API.MethodReader.MethodHasAttribute(Request.method,"NoDemo") && Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "security.demo"))
                {
                    if (!IsAdministrator() && !API.MethodReader.MethodHasAttribute(Request.method, "TokenSecure"))
                    {
                        OnAuthenticationFailed();
                        return this;
                    }
                }


                //HEAD requests are only allowed for methods with an attribute of [AllowHEADrequest]
                if (Request.method.Equals("HEAD"))
                {
                    if (!API.MethodReader.MethodHasAttribute(Request.method, "AllowHEADrequest"))
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
                    Log.Instance.Debug("Session cookie: " + Request.sessionCookie.Value);

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
                Ado.StartTransaction();
                // TODO This can be removed when the new tracing is tested
                // Trace_BSO_Create.Execute(Ado, Request, SamAccountName);
                Ado.CommitTransaction();



                if (API.MethodReader.MethodHasAttribute(Request.method,"Analytic"))
                {
                    //Create the analytic data if required
                    Ado.StartTransaction();

                    //Test for analytic attribute is in the method (phew!)
                    try
                    {
                        Security.Analytic_BSO_Create.Create(Ado, DTO, Request);
                    }
                    catch
                    {
                        Ado.RollbackTransaction();
                    }


                    Ado.CommitTransaction();
                }

                //See if there's a cache in the process
                if (API.MethodReader.MethodHasAttribute(Request.method, "CacheRead"))
                {
                    cDTO = new CacheMetadata("CacheRead", Request.method, DTO);
                    MemCachedD_Value cache = AppServicesHelper.CacheD.Get_BSO<dynamic>(cDTO.Namespace, cDTO.ApiName, cDTO.Method, DTO);
                    if (cache.hasData)
                    {
                        Response.data = cache.data;
                        if (!Enum.IsDefined(typeof(HttpStatusCode), Response.statusCode)) 
                            Response.statusCode = HttpStatusCode.OK;
                        return this;
                    }
                }

                // The Actual Read should happen here by the specific class!
                if (!Execute())
                {
                    OnExecutionError();
                }

                
                if (!PostExecute())
                {
                    OnExecutionError();
                }

                FormatForRestful(Response);
                OnExecutionSuccess();
                
                return this;
            }
            catch (UnmatchedParametersException unmatchException)
            {
                Log.Instance.Debug(unmatchException);
                OnDTOValidationError();
                Response.statusCode = HttpStatusCode.BadRequest;
                return this;
            }
            catch (FormatException formatException)
            {
                //A FormatException error has been caught, log the error and return a message to the caller
                Log.Instance.Error(formatException);
                Response.error = formatException.Message ?? Label.Get("error.schema");
                Response.statusCode = HttpStatusCode.InternalServerError;
                return this;
            }
            catch (Exception ex)
            {
                //An error has been caught,  log the error and return a message to the caller
                Log.Instance.Error(ex);
                Response.error = Label.Get("error.exception");
                Response.statusCode = HttpStatusCode.InternalServerError;
                return this;
            }
            finally
            {
                Dispose();
            }
        }


    }
}
