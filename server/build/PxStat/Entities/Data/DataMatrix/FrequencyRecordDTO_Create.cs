using PxStat.Data;
using System.Collections.Generic;
using System;

namespace PxStat.Data
{
    /// <summary>
    /// class
    /// </summary>
    public class FrequencyRecordDTO_Create : IEquatable<FrequencyRecordDTO_Create>
    {
        public List<PeriodRecordDTO_Create> Period { get; set; }

        public int FrequencyId { get; set; }

        public string Value { get; set; }

        public string Code { get; set; }

        public int IdMultiplier { get; internal set; }


        /// <summary>
        /// Get a hash for this object
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
        /// Tests if two frequencies are equal in terms of code and value
        /// </summary>
        /// <param name="otherFrequency"></param>
        /// <returns></returns>
        public Boolean Equals(FrequencyRecordDTO_Create otherFrequency)
        {
            if (!otherFrequency.GetType().Equals(typeof(FrequencyRecordDTO_Create))) return false;
            var other = (FrequencyRecordDTO_Create)otherFrequency;
            return this.Value.Equals(other.Value) && this.Code.Equals(other.Code);
        }
    }
}
