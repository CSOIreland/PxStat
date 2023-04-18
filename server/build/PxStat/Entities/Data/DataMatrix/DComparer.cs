using System.Collections.Generic;

namespace PxStat.Data
{

    public class StatDimensionComparer : IEqualityComparer<StatDimension>
    {
        public bool Equals(StatDimension left, StatDimension right)
        {
            if (!left.Value.Equals(right.Value) || !left.Code.Equals(right.Code) || !left.Sequence.Equals(right.Sequence)) return false;

            return true;
        }

        public int GetHashCode(StatDimension dim)
        {
            return dim.Value.GetHashCode() ^ dim.Code.GetHashCode() ^ dim.Sequence.GetHashCode();
        }
    }


    public class DimensionVariableComparer : IEqualityComparer<IDimensionVariable>
    {
        public bool Equals(DimensionVariable left, DimensionVariable right)
        {
            if (!left.Value.Equals(right.Value) || !left.Code.Equals(right.Code)) return false;

            return true;
        }

        public bool Equals(IDimensionVariable left, IDimensionVariable right)
        {
            if (!left.Value.Equals(right.Value) || !left.Code.Equals(right.Code)) return false;

            return true;
        }

        public int GetHashCode(DimensionVariable vrb)
        {
            return vrb.Value.GetHashCode() ^ vrb.Code.GetHashCode();
        }

        public int GetHashCode(IDimensionVariable vrb)
        {
            return vrb.Value.GetHashCode() ^ vrb.Code.GetHashCode();
        }

    }
}
