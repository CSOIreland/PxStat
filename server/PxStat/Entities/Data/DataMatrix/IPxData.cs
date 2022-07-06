using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxStat.Data
{
    public interface IPxData
    {
        string GetFormatType();
        string GetIsOfficialStatistic();
        string GetFrequencyCodes();

    }
}
