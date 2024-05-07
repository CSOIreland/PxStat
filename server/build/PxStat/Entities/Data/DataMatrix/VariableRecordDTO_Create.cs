using System;

namespace PxStat.Data
{
    public class VariableRecordDTO_Create : IEquatable<VariableRecordDTO_Create>
    {


        public int ClassificationVariableId { get; set; }
        public string Value { get; set; }
        public string Code { get; set; }

        public int Id { get; set; }

        public long SortId { get; internal set; }

        public bool EliminationFlag { get; internal set; }


        public VariableRecordDTO_Create() { }
        public VariableRecordDTO_Create(dynamic vrb)
        {
            if (vrb.VrbCode != null)
                this.Code = vrb.VrbCode;
            if (vrb.VrbValue != null)
                this.Value = vrb.VrbValue;
        }

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
        public Boolean Equals(VariableRecordDTO_Create otherVariable)
        {
            if (!otherVariable.GetType().Equals(typeof(VariableRecordDTO_Create))) return false;
            var other = (VariableRecordDTO_Create)otherVariable;
            return this.Value.Equals(other.Value) && this.Code.Equals(other.Code);
        }
    }
}
