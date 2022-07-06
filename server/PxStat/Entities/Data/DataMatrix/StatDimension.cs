using System;
using System.Collections.Generic;
using System.Linq;

namespace PxStat.Data
{

    public class StatDimension : IStatDimension
    {
        //The sequence in which this dimension occurs in the ordering
        public int Sequence { get; set; }
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
        public List<IDimensionVariable> Variables { get; set; } = new List<IDimensionVariable>();
        //The Matrix Id of the matrix that contains this dimension
        public int MatrixId { get; set; }
        public int Id { get; set; }
        //The calculated size of each block of similar data for this dimension in a sorted 2D matrix
        //Used for the fractal query
        public int Lambda { get; set; }
        //The Lambda of the previous dimension in a sorted 2D matrix
        public int PreviousLambda { get; set; }
        //For the fractal query, which ordinals (sequences) are being queried?
        public List<int> QueryDimensionOrdinals { get; set; }
        //For the fractal query, which cells would be selected if only this dimension was being queried?
        public List<int> QualifyingOrdinals { get; set; }

        /// <summary>
        /// Get the hash for this object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Value.GetHashCode() ^ this.Code.GetHashCode();// ^ this.Sequence.GetHashCode();
        }

        /// <summary>
        /// Tests if two dimensions are entirely equal in terms of code and value
        /// </summary>
        /// <param name="otherDimension"></param>
        /// <returns></returns>
        public Boolean Equals(IStatDimension otherDimension)
        {
            if (Object.ReferenceEquals(this, otherDimension)) return true;
            if (!otherDimension.GetType().Equals(typeof(StatDimension))) return false;
            var other = (StatDimension)otherDimension;
            //if (!this.Value.Equals(other.Value) || !this.Code.Equals(other.Code) || !this.Sequence.Equals(other.Sequence)) return false;
            if (!this.Value.Equals(other.Value) || !this.Code.Equals(other.Code)) return false;
            return true;
        }

        /// <summary>
        /// Tests if two dimensions are entirely equal in terms of value and other measures. Used for comparing dimensions across languages.
        /// </summary>
        /// <param name="otherDimension"></param>
        /// <returns></returns>
        public bool IsEquivalent(IStatDimension otherDimension)
        {
            if (!otherDimension.GetType().Equals(typeof(StatDimension))) return false;
            var other = (StatDimension)otherDimension;

            if (this.Code != null && other.Code != null)
                if (!this.Code.Equals(other.Code)) return false;
            if (this.Code == null && other.Code != null) return false;
            if (this.Code != null && other.Code == null) return false;
            if (this.Role != null && other.Role != null)
                if (!this.Role.Equals(other.Role)) return false;
            if (this.Role == null && other.Role != null) return false;
            if (this.Role != null && other.Role == null) return false;
            if (!this.Sequence.Equals(other.Sequence)) return false;
            if (!this.GeoFlag.Equals(other.GeoFlag)) return false;
            if (this.GeoUrl != null && other.GeoUrl != null)
            {
                if (!this.GeoUrl.Equals(other.GeoUrl)) return false;
            }
            if (this.GeoUrl == null && other.GeoUrl != null) return false;
            if (this.GeoUrl != null && other.GeoUrl == null) return false;

            if (this.Variables != null && otherDimension.Variables != null)
            {
                List<string> vCodesThis = this.Variables.Select(x => x.Code).ToList();
                List<string> vCodesOther = otherDimension.Variables.Select(x => x.Code).ToList();
                foreach (var vrbCode in vCodesThis)
                {
                    if (!vCodesOther.Contains(vrbCode)) return false;
                }
            }
            if (this.Variables == null && otherDimension.Variables != null) return false;
            if (this.Variables != null && otherDimension.Variables == null) return false;

            return true;
        }
    }
}
