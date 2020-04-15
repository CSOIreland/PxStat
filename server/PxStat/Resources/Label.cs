using API;
using Newtonsoft.Json.Linq;


// Keep it under the PxStat namespace because it's globally used
namespace PxStat
{
    static public class RequestLanguage
    {
        public static string LngIsoCode { get; set; }
    }
    // Static variable that must be initialized at run time.
    static public class Label
    {
        // Mandatory language merged with the default one (fall back)
        internal static readonly dynamic mergedInstance;
        internal static readonly dynamic gaMergedInstance;
        internal static readonly dynamic plMergedInstance;

        //The en (English) is the mandatory language, always existing
        internal static readonly dynamic mandatoryInstance = Utility.JsonDeserialize_IgnoreLoopingReference(Properties.Resources.ResourceManager.GetString("en"));
        //The APP_DEFAULT_LANGUAGE is configurable and may not exist
        internal static readonly dynamic defaultInstance = Utility.JsonDeserialize_IgnoreLoopingReference(Properties.Resources.ResourceManager.GetString(Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE")));

        internal static readonly dynamic gaInstance = Utility.JsonDeserialize_IgnoreLoopingReference(Properties.Resources.ResourceManager.GetString("ga"));
        internal static readonly dynamic plInstance = Utility.JsonDeserialize_IgnoreLoopingReference(Properties.Resources.ResourceManager.GetString("pl"));



        /// <summary>
        /// Initiate Label, merge the default language with the mandatory English one which is always available (fall back)
        /// 
        /// Static constructor is called at most one time, before any instance constructor is invoked or member is accessed.
        /// </summary>
        /// <returns></returns>
        static Label()
        {
            // Initiate the merged label with the mandatory one
            mergedInstance = Utility.JsonDeserialize_IgnoreLoopingReference(Utility.JsonSerialize_IgnoreLoopingReference(mandatoryInstance));
            mergedInstance.Merge(defaultInstance, new JsonMergeSettings
            {
                // union array values together to avoid duplicates
                MergeArrayHandling = MergeArrayHandling.Union
            });

            gaMergedInstance = Utility.JsonDeserialize_IgnoreLoopingReference(Utility.JsonSerialize_IgnoreLoopingReference(mandatoryInstance));
            gaMergedInstance.Merge(gaInstance, new JsonMergeSettings
            {
                // union array values together to avoid duplicates
                MergeArrayHandling = MergeArrayHandling.Union
            });

            plMergedInstance = Utility.JsonDeserialize_IgnoreLoopingReference(Utility.JsonSerialize_IgnoreLoopingReference(mandatoryInstance));
            plMergedInstance.Merge(plInstance, new JsonMergeSettings
            {
                // union array values together to avoid duplicates
                MergeArrayHandling = MergeArrayHandling.Union
            });


        }

        static public string GetFromRequestLanguage(string label)
        {
            return Get(label, RequestLanguage.LngIsoCode == null ? Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE") : RequestLanguage.LngIsoCode);
        }

        static public string Get(string label, string lngIsoCode)
        {
            if (lngIsoCode.Equals(Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE")))
                return Get(label);

            switch (lngIsoCode)
            {
                case "ga":
                    return ProcessGet(label, gaMergedInstance);
                case "pl":
                    return ProcessGet(label, plMergedInstance);

            }
            return label;
        }

        /// <summary>
        /// Get the Label in the mergedInstance
        /// </summary>
        static public string Get(string label)
        {
            return ProcessGet(label, mergedInstance);
        }

        private static string ProcessGet(string label, dynamic outputObject)
        {
            if (string.IsNullOrEmpty(label))
                return "";
            try
            {
                string[] properties = label.Split('.');
                dynamic output = outputObject;// mergedInstance;
                foreach (string property in properties)
                {
                    if (output[property] != null)
                        output = output[property];
                    else
                        // Label not found, return the Label parameter for feedback
                        return label;
                }

                return output.ToString();
            }
            catch
            {
                return label;
            }
        }
    }
}
