﻿using PxStat.System.Settings;
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
        Copyright_DTO_Create Copyright { get; set; }
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
    }
}