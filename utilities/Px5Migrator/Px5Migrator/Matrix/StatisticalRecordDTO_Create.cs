using System;

namespace Px5Migrator
{
    public class StatisticalRecordDTO_Create : IEquatable<StatisticalRecordDTO_Create>
    {

        public int StatisticalProductId { get; set; }

        public string Value { get; set; }
        public string Code { get; set; }
        public string Unit { get; set; }

        public short Decimal { get; set; }

        public long SortId { get; internal set; }
        public int Id { get; internal set; }
        public StatisticalRecordDTO_Create(dynamic stat)
        {
            if (stat.SttCode != null)
                this.Code = stat.SttCode;
            if (stat.SttValue != null)
                this.Value = stat.SttValue;
            if (stat.SttUnit != null)
                this.Unit = stat.SttUnit;
            if (stat.SttDecimal != null)
            {
                int check = stat.SttDecimal;
                if (check > 32767 || check < -32767)
                {
                    throw new FormatException();
                }
                this.Decimal = stat.SttDecimal;
            }
        }

        /// <summary>
        /// blank constructor
        /// </summary>
        public StatisticalRecordDTO_Create() { }

        /// <summary>
        /// Tests if two statistic records have the same Decimal and Unit
        /// </summary>
        /// <param name="comp"></param>
        /// <returns></returns>
        internal bool IsEquivalent(StatisticalRecordDTO_Create comp)
        {

            if (this.Decimal != comp.Decimal) return false;
            if (this.Unit != comp.Unit) return false;
            return true;
        }


        /// <summary>
        /// Tests if two statistical records are entirely similar
        /// </summary>
        /// <param name="otherStatistic"></param>
        /// <returns></returns>
        public Boolean Equals(StatisticalRecordDTO_Create otherStatistic)
        {
            if (!otherStatistic.GetType().Equals(typeof(StatisticalRecordDTO_Create))) return false;
            var other = (StatisticalRecordDTO_Create)otherStatistic;
            if (!IsEquivalent(other)) return false;
            if (!this.Value.Equals(other.Value)) return false;

            return true;
        }

        /// <summary>
        /// Get the hash for this object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            // Get the hash codes
            int hashValue = this.Value.GetHashCode();
            // Calculate the hash code for the object.
            return hashValue;
        }
    }
}
