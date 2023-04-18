using PxStat.System.Settings;
using System.Collections.Generic;

namespace PxStat.Data
{
    internal interface ICube_DTO_Read
    {
        IList<Dimension> dimension { get; set; }
        Format_DTO_Read Format { get; set; }
        string language { get; set; }
        string matrix { get; set; }
        int release { get; set; }
        Role role { get; set; }
    }
}