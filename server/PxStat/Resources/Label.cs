using API;
using Newtonsoft.Json.Linq;


// Keep it under the PxStat namespace because it's globally used
namespace PxStat
{
    // Static variable that must be initialized at run time.
    static public class Label
    {
        // Mandatory language merged with the default one (fall back)
        internal static readonly dynamic mergedInstance;
        //The en (English) is the mandatory language, always existing
        internal static readonly dynamic mandatoryInstance = Utility.JsonDeserialize_IgnoreLoopingReference(Properties.Resources.ResourceManager.GetString("en"));
        //The APP_DEFAULT_LANGUAGE is configurable and may not exist
        internal static readonly dynamic defaultInstance = Utility.JsonDeserialize_IgnoreLoopingReference(Properties.Resources.ResourceManager.GetString(Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE")));



        /// <summary>
        /// Initiate Label, merge the default language with the mandatory English one which is always available (fall back)
        /// 
        /// Static constructor is called at most one time, before any instance constructor is invoked or member is accessed.
        /// </summary>
        /// <returns></returns>
        static Label()
        {
            // Initiate the merged label with the mandatory one
            mergedInstance = mandatoryInstance;
            mergedInstance.Merge(defaultInstance, new JsonMergeSettings
            {
                // union array values together to avoid duplicates
                MergeArrayHandling = MergeArrayHandling.Union
            });
        }

        /// <summary>
        /// Get the Label in the mergedInstance
        /// </summary>
        static public string Get(string label)
        {
            if (string.IsNullOrEmpty(label))
                return "";
            try
            {
                string[] properties = label.Split('.');
                dynamic output = mergedInstance;
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