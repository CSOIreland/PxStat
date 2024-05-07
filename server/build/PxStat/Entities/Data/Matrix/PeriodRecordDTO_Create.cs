using System;

namespace PxStat.Data
{
    public class PeriodRecordDTO_Create : IEquatable<PeriodRecordDTO_Create>
    {
        public int FrequencyPeriodId { get; set; }

        public string Value { get; set; }
        public string Code { get; set; }

        public long SortId { get; internal set; }
        public int Id { get; internal set; }
        /// <summary>
        /// Get a hash of this object
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
        /// Tests if two periods are equal in terms of code and value
        /// </summary>
        /// <param name="otherPeriod"></param>
        /// <returns></returns>
        public Boolean Equals(PeriodRecordDTO_Create otherPeriod)
        {
            if (!otherPeriod.GetType().Equals(typeof(PeriodRecordDTO_Create))) return false;
            var other = (PeriodRecordDTO_Create)otherPeriod;
            return this.Value.Equals(other.Value) && this.Code.Equals(other.Code);
        }


    }
}
