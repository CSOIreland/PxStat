using API;
using Newtonsoft.Json;
using PxStat.Security;
using PxStat.System.Settings;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Static class used for maintaining lists, dictionaries etc for keywords. 
    /// The objects originate in json files, however we use a singleton so that we don't have to deserialize every time 
    /// we need to refer to the same list
    /// </summary>
    internal static class Keyword_BSO_ResourceFactory
    {
        internal static Dictionary<string, List<Synonym>> synonymLists = new Dictionary<string, List<Synonym>>();
        internal static Dictionary<string, Dictionary<string, string>> morphologyDictionaries = new Dictionary<string, Dictionary<string, string>>();
        internal static Dictionary<string, dynamic> Keywords = new Dictionary<string, dynamic>();
        /// <summary>
        /// Get a List<> of Synonym objects
        /// </summary>
        /// <param name="lngIsoCode"></param>
        /// <returns></returns>
        internal static List<Synonym> GetSynonyms(string lngIsoCode)
        {
            if (!synonymLists.ContainsKey(lngIsoCode))
            {
                //This is the first time using this resource for this language, so we must deserialize, add to our collection and return the object
                string json = Properties.Resources.ResourceManager.GetString(Utility.GetCustomConfig("APP_INTERNATIONALISATION_SYNONYM_FILE") + lngIsoCode);
                if (json != null)
                {

                    List<Synonym> SynonymList = Utility.JsonDeserialize_IgnoreLoopingReference<List<Synonym>>(json);

                    synonymLists.Add(lngIsoCode, SynonymList);
                    return SynonymList;
                }
                else return new List<Synonym>();
            }
            //object exists already, so no need to deserialize
            return synonymLists[lngIsoCode];
        }

        /// <summary>
        /// Get the full list of synonym dictionaries. 
        /// Use any existing ones and create new ones where they don't already exist
        /// </summary>
        /// <returns></returns>
        internal static Dictionary<string, List<Synonym>> GetAllSynonymSets()
        {
            ResourceSet resourceSet = Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry entry in resourceSet)
            {
                string resourceKey = entry.Key.ToString();
                if (resourceKey.Contains(Utility.GetCustomConfig("APP_INTERNATIONALISATION_SYNONYM_FILE")))
                {
                    //create a new synonym list and add it to the list if it doesn't exist
                    string lngIsoCode = resourceKey.Substring(Utility.GetCustomConfig("APP_INTERNATIONALISATION_SYNONYM_FILE").Length, 2);
                    //Check that the resource file corresponds with a language in the system
                    Language_BSO lBso = new Language_BSO();
                    if (lBso.Read(lngIsoCode).LngIsoCode != null)
                        GetSynonyms(lngIsoCode);
                }
            }
            //return synonym lists for all languages
            return synonymLists;
        }

        /// <summary>
        /// A morphology dictionary is used where a language needs to use a dictionary in order to find singulars and plurals of nouns
        /// This is typically the case in more highly inflected languages
        /// </summary>
        /// <param name="lngIsoCode"></param>
        /// <returns></returns>
        internal static Dictionary<string, string> GetMorphology(string lngIsoCode)
        {
            if (!morphologyDictionaries.ContainsKey(lngIsoCode))
            {
                //This is the first time using this resource for this language, so we must deserialize, add to our collection and return the object
                string json = Properties.Resources.ResourceManager.GetString(Utility.GetCustomConfig("APP_INTERNATIONALISATION_MORPHOLOGY_FILE") + lngIsoCode);
                if (json != null)
                {
                    Dictionary<string, string> morphology = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    morphologyDictionaries.Add(lngIsoCode, morphology);
                    return morphology;
                }
                else return new Dictionary<string, string>();

            }
            //object exists already, so no need to deserialize
            return morphologyDictionaries[lngIsoCode];

        }

        /// <summary>
        /// Get the appropriate Keyword class for each language. This is based on a Json file.
        /// The Keyword file defines a number of parsing rules specifically for that language.
        /// </summary>
        /// <param name="lngIsoCode"></param>
        /// <returns></returns>
        internal static dynamic GetKeyword(string lngIsoCode, Dictionary<string, dynamic> kwords = null)
        {
            if (kwords != null) return kwords[lngIsoCode];

            if (!Keywords.ContainsKey(lngIsoCode))
            {
                lngIsoCode = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");
                dynamic keywordInstance = Utility.JsonDeserialize_IgnoreLoopingReference(Properties.Resources.ResourceManager.GetString(Utility.GetCustomConfig("APP_INTERNATIONALISATION_KEYWORD_FILE") + lngIsoCode));
                if (!Keywords.ContainsKey(lngIsoCode))
                    Keywords.Add(lngIsoCode, keywordInstance);
                return keywordInstance;
            }
            return Keywords[lngIsoCode];
        }

    }
}
