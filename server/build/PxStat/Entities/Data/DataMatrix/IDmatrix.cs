using API;
using FluentValidation.Results;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;

namespace PxStat.Data
{
    /// <summary>
    /// Abstraction of the Dmatrix class
    /// </summary>
    /// 

    public interface IDmatrix
    {
        ICollection<dynamic> Cells { get; set; }
        string CellsString { get; set; }
        string Code { get; set; }
        ICopyright Copyright { get; set; }
        DateTime CreatedDateTime { get; set; }
        string CreatedDateString { get; set; }
        Dictionary<string, Dspec> Dspecs { get; set; }
        ICollection<string> Languages { get; set; }
        Release_DTO Release { get; set; }
        string FormatType { get; set; }
        string FormatVersion { get; set; }
        string MtrInput { get; set; }
        bool IsOfficialStatistic { get; set; }
        int Id { get; set; }
        short Decimals { get; set; }
        string Units { get; set; }
        List<bool> ComparisonReport { get; set; }
        string Language { get; set; }
        IDmatrix GetDmatrixFromPxDocument(IDocument document,  IUpload_DTO uploadDto,List<PxUpload_DTO> dspecs=null);
        IDmatrix GetMultiLanguageMatrixFromRelease(IADO ado, string mtrCode, Release_DTO rDto);
        IDmatrix GetMultiLanguageMatrixFromRelease(IADO ado, Release_DTO rDto);
        bool Validate();
        public ValidationResult ValidationResult { get; set; }

    }
}
