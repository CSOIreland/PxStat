using FluentValidation.Results;
using System.Collections.Generic;

namespace PxStat.Data
{
    public interface IDspec
    {
        string Contents { get; set; }
        string ContentVariable { get; set; }
        string CopyrightUrl { get; set; }
        short Decimals { get; set; }
        ICollection<StatDimension> Dimensions { get; set; }
        string Language { get; set; }
        string MatrixCode { get; set; }
        int MatrixId { get; set; }
        ICollection<string> Notes { get; set; }
        string Source { get; set; }
        ICollection<KeyValuePair<string, ICollection<string>>> TimeVals { get; set; }
        bool TimeValsDefined { get; set; }
        string Title { get; set; }
        List<ValidationFailure> ValidationErrors { get; set; }
        ICollection<KeyValuePair<string, ICollection<string>>> Values { get; set; }
        string GetNotesAsString();
        int GetCellCount();
        string PrcValue { get; set; }
        string SbjValue { get; set; }   
    }
}
