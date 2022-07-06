using System;

namespace PxStat.Data
{
    /// <summary>
    /// This contains the variables for each dimension
    /// </summary>
    /// 

    public class DimensionVariable : IDimensionVariable
    {
        //Variable Code
        public string Code { get; set; }
        //Variable value
        public string Value { get; set; }
        //The order in which this appears in the variable list
        public int Sequence { get; set; }
        //Is this an elimination?
        public bool Elimination { get; set; }
        //If this is for a stat dimension and it can override the global decimal places, the value can be shown here:
        public short Decimals { get; set; }
        //If the dimension has a statistic role, this may show the unit of measurement
        public string Unit { get; set; }
        //For use during Comparison. Flags if this is a new variable
        public bool AmendFlag { get; set; }


        /// <summary>
        /// Get a hash code for the object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            // Get the hash codes
            int hashValue = this.Value.GetHashCode();
            int hashCode = this.Code.GetHashCode();
            // Calculate the hash code for the object.
            return hashValue ^ hashCode;
        }

        /// <summary>
        /// Tests if two variables are equal in terms of code and value
        /// </summary>
        /// <param name="otherVariable"></param>
        /// <returns></returns>
        public Boolean Equals(IDimensionVariable otherVariable)
        {
            if (!otherVariable.GetType().Equals(typeof(DimensionVariable))) return false;
            var other = (DimensionVariable)otherVariable;
            return this.Value.Equals(other.Value) && this.Code.Equals(other.Code);
        }

        /// <summary>
        /// Tests if two variables are equal in terms of code only. Used for comparing across languages
        /// </summary>
        /// <param name="otherVariable"></param>
        /// <returns></returns>
        public bool IsEquivalent(IDimensionVariable otherVariable)
        {
            if (!otherVariable.GetType().Equals(typeof(DimensionVariable))) return false;
            var other = (DimensionVariable)otherVariable;

            return this.Code.Equals(other.Code);
        }

    }
}
