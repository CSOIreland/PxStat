using PxStat.System.Navigation;
using System.Collections.Generic;

namespace PxStat
{
    public class Keyword
    {
        internal dynamic keywordInstance { get; set; }





        internal string Get(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return "";
            try
            {
                string[] properties = keyword.Split('.');
                dynamic output = keywordInstance;
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
                dynamic output = keywordInstance;
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
