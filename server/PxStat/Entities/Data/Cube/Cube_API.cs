using API;
using FluentValidation;
using FluentValidation.Results;
using PxStat.JsonQuery;
using PxStat.Resources;
using System.Net;

namespace PxStat.Data
{
    /// <summary>
    /// Cube APIs are used for reading data and metadata.
    /// </summary>

    [AllowAPICall]
    public class Cube_API
    {
        [Analytic]
        [AllowHEADrequest]
        public static dynamic PxAPIv1(IRequest restfulRequest)
        {
            return new Cube_BSO_ReadPxApiV1().Read(restfulRequest);
        }

        private static dynamic RunQuery()
        {
            JsonStatQuery jq = new JsonStatQuery();
            jq.Class = Constants.C_JSON_STAT_QUERY_CLASS;

            return null;
        }

        /// <summary>
        /// Reads a live dataset based on specific criteria. 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Analytic]
        [AllowHEADrequest]
        //Internal cache management
        public static dynamic ReadDataset(IRequest request)
        {
            //For RESTful requests...
            //A pre-validation. If a validation problem is found then an output containing the error will be created and returned immediately
            //The validation rules depend on PxStat, hence they are asserted here
            if (request.GetType().Equals(typeof(RESTful_API)))
            {
                var vldOutput = ValidateRest<Cube_VLD_REST_ReadDataset>(request, new Cube_VLD_REST_ReadDataset());
                if (vldOutput != null) return vldOutput;
            }
            else if (request.GetType().Equals(typeof(Head_API)))
            {
                return new Cube_BSO_ReadDatasetHEAD(request).Read().Response;
            }
            return new Cube_BSO_ReadDataset(request).Read().Response;
        }


        /// <summary>
        /// Creates a pre-validation of the RESTful request
        /// You must supply a generic validator and a RESTful_API
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="restfulRequest"></param>
        /// <param name="Validation"></param>
        /// <returns>Returns a RESTful_Output with error details if validation fails. This can be returned to the user. Othwerwise returns null</returns>

        internal static RESTful_Output ValidateRest<V>(IRequest restfulRequest, V Validation) where V : AbstractValidator<IRequest>
        {
            ValidationResult result = Validation.Validate(restfulRequest);
            if (!result.IsValid)
            {
                foreach (var e in result.Errors)
                {
                    Log.Instance.Debug(e.ErrorMessage);
                }
                RESTful_Output output = new RESTful_Output();
                output.statusCode = HttpStatusCode.BadRequest;
                output.response = Label.Get("error.invalid");
                return output;
            }
            return null;
        }

        /// <summary>
        /// Reads the metadata for a live dataset based on specific criteria.
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        [AllowHEADrequest]
        public static dynamic ReadMetadata(IRequest request)
        {
            return new Cube_BSO_ReadMetadata(request).Read().Response;
        }


        /// <summary>
        /// Reads any Dataset (including pre-release data) based on Release Code and other criteria. 
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>

        public static dynamic ReadPreDataset(JSONRPC_API jsonrpcRequest)
        {
            return new Cube_BSO_ReadPreDataset(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Reads the metadata for a non live dataset based on Release code and other criteria. 
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>

        public static dynamic ReadPreMetadata(JSONRPC_API jsonrpcRequest)
        {
            return new Cube_BSO_ReadPreMetadata(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Returns a Collection of JsonStat items.
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        /// 

        public static dynamic ReadCollection(IRequest jsonrpcRequest)
        {
            return new Cube_BSO_ReadCollection(jsonrpcRequest, true).Read().Response;
        }

        public static dynamic ReadMetaCollection(JSONRPC_API jsonrpcRequest)
        {
            return new Cube_BSO_ReadCollection(jsonrpcRequest, true).Read().Response;
        }


    }

}
