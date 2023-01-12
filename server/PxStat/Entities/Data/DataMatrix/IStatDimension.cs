using System;
using System.Collections.Generic;

namespace PxStat.Data
{
    public interface IStatDimension
    {
        string Code { get; set; }
        bool GeoFlag { get; set; }
        string GeoUrl { get; set; }
        int MatrixId { get; set; }
        string Role { get; set; }
        int Sequence { get; set; }
        string Value { get; set; }
        List<IDimensionVariable> Variables { get; set; }
        int Id { get; set; }
        int Lambda { get; set; }
        int PreviousLambda { get; set; }
        List<int> QueryDimensionOrdinals { get; set; }
        List<int> QualifyingOrdinals { get; set; }
        Dictionary<string, object> DictionaryVariables { get; set; }

        int GetHashCode();
        Boolean Equals(IStatDimension otherDimension);

    }
}
