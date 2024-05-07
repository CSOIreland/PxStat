using API;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace PxStat.Resources
{
    /// <summary>
    /// A container for cache related metadata.
    /// </summary>
    public class CacheMetadata
    {
        #region Properties
        /// <summary>
        /// Namespace
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// API name
        /// </summary>
        public string ApiName { get; set; }

        /// <summary>
        /// Method
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// CAS - i.e. storage location for cache
        /// </summary>
        public string Cas { get; set; } = "";

        /// <summary>
        /// Domain
        /// </summary>
        public string Domain { get; set; } = "";

        /// <summary>
        /// Cache time limit
        /// </summary>
        public DateTime TimeLimit { get; set; } = new DateTime();

        /// <summary>
        /// List of CAS objects
        /// </summary>
        public List<Cas> CasList { get; set; }

        /// <summary>
        /// CAS Domain
        /// </summary>
        private string casDomain = "";
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="method"></param>
        /// <param name="dto"></param>
        public CacheMetadata(string attributeName, string method, dynamic dto)
        {
            //Find the namespace and method from the full method string
            List<string> parts = method.Split('.').ToList<string>();
            if (parts.Count < 3) return;


            Method = parts.Last();
            ApiName = parts[parts.Count - 2];
            for (int i = 0; i < (parts.Count - 2); i++)
            {
                Namespace = Namespace + parts[i];
                if (i < (parts.Count - 3)) Namespace = Namespace + '.';
            }

            //Get MethodInfo for  the fully qualified api
            Type type = Type.GetType(Namespace + '.' + ApiName);
            MethodInfo methodInfo = type.GetMethod(Method);

            string domainField = null;

            //Check which custom attributes are part of the api call
            foreach (dynamic attr in methodInfo.GetCustomAttributes(true))
            {
                if (MethodReader.DynamicHasProperty(attr, "CAS_REPOSITORY"))
                    Cas = attr.CAS_REPOSITORY;
                if (MethodReader.DynamicHasProperty(attr, "DOMAIN"))
                    domainField = attr.DOMAIN;
                if (MethodReader.DynamicHasProperty(attr, "CAS_REPOSITORY_DOMAIN_LIST"))
                    casDomain = attr.CAS_REPOSITORY_DOMAIN_LIST;
                if (MethodReader.DynamicHasProperty(attr, "EXPIRY_DATE_TIME_PROPERTY"))
                {
                    string dateString = MethodReader.GetStringValueOfProperty(dto, attr.EXPIRY_DATE_TIME_PROPERTY);
                    DateTime dateValue;
                    if (DateTime.TryParse(dateString, out dateValue))
                    {
                        this.TimeLimit = dateValue;
                    }
                }


            }

            //The cas + domain fields for the flush will be in a comma separated list
            List<string> casDomainList = casDomain.Split(',').ToList<string>();
            CasList = new List<Cas>();
            // We now need to split the cas and domain (they are separated by a '/' character)
            // and make a list of cas objects
            foreach (string cString in casDomainList)
            {
                string[] casArray = cString.Split('/');
                if (casArray.Length > 0)
                {
                    Cas cas = new Cas();
                    cas.CasRepository = casArray[0];
                    if (casArray.Length > 1)
                        cas.Domain = MethodReader.GetStringValueOfProperty(dto, casArray[1]);
                    else cas.Domain = "";
                    CasList.Add(cas);
                }
            }

            //Get the string value of the domain field
            if (domainField != null)
            {
                Domain = MethodReader.GetStringValueOfProperty(dto, domainField);
            }


        }


    }

    /// <summary>
    /// A class to act as a holder for Cas/domain pairs
    /// </summary>
    public class Cas
    {
        public string CasRepository { get; set; }
        public string Domain { get; set; }
        public Cas(string casRepository, string domain)
        {
            CasRepository = casRepository;
            Domain = domain;
        }

        public Cas() { }

        public static void RunCasFlush(string repository)
        {
           // bool enabled= Convert.ToBoolean(AppServicesHelper.ApiConfiguration.Settings["API_MEMCACHED_ENABLED"]);
           // if(!enabled) { return; }

            if (!AppServicesHelper.CacheD.CasRepositoryFlush(repository )) 
            {
                Thread.Sleep(1000);
                if (!AppServicesHelper.CacheD.CasRepositoryFlush(repository))
                    throw new IncompleteCasFlush();
            }
        }

    }
}