namespace PxStat.DBuild
{
    public class DimensionVariable_DTO
    {
        public string Code { get; set; }
        //Variable value


        public string Value { get; set; }
        //The order in which this appears in the variable list
        //Is this an elimination?
        public bool Elimination { get; set; }
        //If this is for a stat dimension and it can override the global decimal places, the value can be shown here:
        public short Decimals { get; set; }
        //If the dimension has a statistic role, this may show the unit of measurement
        public string Unit { get; set; }
        //For use during Comparison. Flags if this is a new variable
        public bool AmendFlag { get; set; }
        public bool IsEquivalent(DimensionVariable_DTO otherVariable)
        {
            if (!this.Code.Equals(otherVariable.Code)) return false;
            if (!this.Elimination.Equals(otherVariable.Elimination)) return false;
            if (!this.Decimals.Equals(otherVariable.Decimals)) return false;
            if (!this.AmendFlag.Equals(otherVariable.AmendFlag)) return false;
            return true;
        }
    }
}
