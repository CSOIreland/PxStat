using API;
using System.Collections.Generic;
using System.IO;

namespace Px5Migrator
{
    public class PxKeywordDictionary
    {
        static SortedDictionary<string, string> KeywordTypes = new SortedDictionary<string, string>();
        static SortedDictionary<string, string> KeywordExceptions = new SortedDictionary<string, string>();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        static public void GetTypesFromFile(PxDocument file)
        {

            foreach (var k in file.Keywords)
            {
                if (KeywordTypes.ContainsKey(k.Key.Identifier))
                {
                    string theType = KeywordTypes[k.Key.Identifier];
                    if (theType == k.Element.GetType().Name)
                    {
                        continue;
                    }
                    else
                    {
                        if (!KeywordExceptions.ContainsKey(k.Key.Identifier))
                        {
                            KeywordExceptions.Add(k.Key.Identifier, string.Format("{0} different from previous {1}", k.Element.GetType().Name, theType));
                        }

                    }
                }
                else
                {
                    KeywordTypes.Add(k.Key.Identifier, k.Element.GetType().Name);
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileNameToSaveWithTypes"></param>
        /// <param name="fileNameToSaveWithExceptions"></param>
        static public void SaveToFile(string fileNameToSaveWithTypes, string fileNameToSaveWithExceptions)

        {
            string resultFileNamesTypes = SaveDictionaryToFile(fileNameToSaveWithTypes, KeywordTypes);
            string resultFileNamesExceptions = SaveDictionaryToFile(fileNameToSaveWithExceptions, KeywordExceptions);

            Log.Instance.DebugFormat("All Keywords saved to new file {0}", resultFileNamesTypes);
            Log.Instance.DebugFormat("All Exceptions saved to new file {0}", resultFileNamesExceptions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileNameToSaveWithTypes"></param>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        private static string SaveDictionaryToFile(string fileNameToSaveWithTypes, SortedDictionary<string, string> dictionary)
        {
            string resultFileNamesTypes = new FileInfo(fileNameToSaveWithTypes).FullName;

            var f = File.CreateText(resultFileNamesTypes);

            foreach (var k in dictionary)
            {
                f.WriteLine("Identifier:{0} Type:{1}", k.Key, k.Value.ToString());
            }

            f.Close();

            return resultFileNamesTypes;
        }
    }
}
