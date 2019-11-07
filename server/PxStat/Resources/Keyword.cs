using System.Collections.Generic;
using API;
using Newtonsoft.Json.Linq;

namespace PxStat
{
    public class Keyword
    {
        // Mandatory language merged with the default one (fall back)

        internal dynamic mandatoryKeywordInstance { get; set; }
        internal dynamic defaultKeywordInstance
        {
            get; private set;
        }
        internal dynamic mergedInstance { get; set; }


        internal Keyword(string lngIsoCode)
        {
            if ((Properties.Resources.ResourceManager.GetString("keyword_" + lngIsoCode)) == null) return;

            mandatoryKeywordInstance = Utility.JsonDeserialize_IgnoreLoopingReference(Properties.Resources.ResourceManager.GetString("keyword_" + lngIsoCode));

            defaultKeywordInstance = Utility.JsonDeserialize_IgnoreLoopingReference(Properties.Resources.ResourceManager.GetString("keyword_" + Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE")));
            // Initiate the merged label with the mandatory one
            mergedInstance = mandatoryKeywordInstance;
            mergedInstance.Merge(defaultKeywordInstance, new JsonMergeSettings
            {
                // union array values together to avoid duplicates
                MergeArrayHandling = MergeArrayHandling.Union
            });
        }


        internal string Get(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return "";
            try
            {
                string[] properties = keyword.Split('.');
                dynamic output = mergedInstance;
                foreach (string property in properties)
                {
                    if (output[property] != null)
                        output = output[property];
                    else
                        // Keyword not found, return the keyword parameter for feedback
                        return keyword;
                }

                return output.ToString();
            }
            catch
            {
                return keyword;
            }
        }



        internal List<string> GetStringList(string keyword)
        {
            List<string> sList = new List<string>();
            if (string.IsNullOrEmpty(keyword))
            {
                return sList;
            }

            try
            {
                string[] properties = keyword.Split('.');
                dynamic output = mergedInstance;
                foreach (string property in properties)
                {
                    if (output[property] != null)
                    {
                        output = output[property];
                    }

                    else
                    // Keyword not found, return the keyword parameter for feedback
                    {
                        sList.Add(keyword);
                        return sList;
                    }
                }

                foreach (var s in output)
                {
                    sList.Add(s.ToString());
                }
                return sList;
            }
            catch
            {
                return sList;
            }
        }
    }
}
