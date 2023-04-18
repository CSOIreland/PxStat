using System.Collections.Generic;

namespace PxStat.DBuild
{
    public class StatDimension_DTO
    {
        //The type of dimension, i.e. statistic, time or classification
        public string Role { get; set; }
        //The dimensions code
        public string Code { get; set; }
        //The dimension name
        public string Value { get; set; }
        //Set to true if this dimension contains Geo information
        public bool GeoFlag { get; set; }
        //If the dimension contains Geo information, this will contain the Geo url
        public string GeoUrl { get; set; }
        //The list of dimension variables
        public List<DimensionVariable_DTO> Variables { get; set; } = new List<DimensionVariable_DTO>();
        public bool IsEquivalent(StatDimension_DTO otherDimension)
        {
            if (this.Variables.Count != otherDimension.Variables.Count) return false;
            int varcount = this.Variables.Count;
            for (int i = 0; i < varcount; i++)
            {
                if (!this.Variables[i].IsEquivalent(otherDimension.Variables[i])) return false;
            }
            if (!this.Role.Equals(otherDimension.Role)) return false;
            if (!this.Code.Equals(otherDimension.Code)) return false;
            if (!this.GeoFlag.Equals(otherDimension.GeoFlag)) return false;
            return true;
        }
    }
}
