using System.Collections.Generic;
using System.Linq;

namespace Px5Migrator
{
    internal class Frequency_BSO
    {


        internal List<string> ReadAll(IMetaData metaData)
        {
            List<string> configList = metaData.GetFrequencyCodes().Split(',').ToList();
            List<string> frqList = new List<string>();
            foreach (var v in configList)
            {
                string[] item = v.Split('/');
                if (item.Length < 2) return null;
                frqList.Add(item[0]);
            }

            return frqList;
        }

    }
}
