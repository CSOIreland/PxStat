using API;
using System;
using System.Collections.Generic;

namespace Px5Migrator
{
    internal class DataReader
    {
        internal IDmatrix ReadReleaseDmatrix(IADO ado, int mtrId)
        {
            IDmatrix dmatrix = new Dmatrix();
            try
            {



                dmatrix.CreatedDateString = DateTime.Now.ToString("yyyyMMdd HH:mm");
                dmatrix.CreatedDateTime = DateTime.Now;

                Migration_ADO dAdo = new Migration_ADO();

                var dmatrixData = dAdo.DataMatrixReadByMatrixId(ado, mtrId);

                if (!dmatrixData.hasData) return dmatrix;

                dmatrix.Code = dmatrixData.data[0].MtrCode;
                dmatrix.FormatVersion = dmatrixData.data[0].FrmVersion;
                dmatrix.IsOfficialStatistic = dmatrixData.data[0].MtrOfficialFlag;
                dmatrix.FormatType = dmatrixData.data[0].FrmType;
                dmatrix.Languages = new List<string>();
                dmatrix.Languages.Add(dmatrixData.data[0].LngIsoCode);
                dmatrix.Copyright.CprCode = dmatrixData.data[0].CprCode;
                dmatrix.Copyright.CprValue = dmatrixData.data[0].CprValue;
                dmatrix.Copyright.CprUrl = dmatrixData.data[0].CprUrl;
                dmatrix.Id = dmatrixData.data[0].MtrId;
                dmatrix.Language = dmatrixData.data[0].LngIsoCode;



                IDspec dspec = new Dspec();
                dspec.Dimensions = new List<StatDimension>();
                dspec.Notes = new List<string>();

                if (dmatrix.Cells == null)
                {
                    var dbMatrixData = dAdo.GetFieldDataForMatrix(ado, dmatrix.Id);
                    dmatrix.Cells = (List<dynamic>)DeserializeData(dbMatrixData);
                    // dmatrix.Decimals = (short)dbMatrixData.data[0].MtdDecimal;
                }

                dmatrix.Dspecs = new Dictionary<string, Dspec>();

                IDspec spec = new Dspec();
                spec.Dimensions = new List<StatDimension>();
                spec.Contents = dmatrixData.data[0].MtrTitle;
                spec.CopyrightUrl = dmatrixData.data[0].CprUrl;
                spec.Language = dmatrix.Language;
                spec.MatrixCode = dmatrix.Code;
                spec.MatrixId = dmatrix.Id;
                spec.Notes = new List<string>() { dmatrixData.data[0].MtrNote };
                spec.Source = dmatrix.Copyright.CprValue;
                spec.Title = dmatrixData.data[0].MtrTitle;

                spec.ContentVariable = "Statistic";





                var dbDimensions = dAdo.GetDimensionsForMatrix(ado, dmatrix.Id);

                if (dbDimensions.hasData)
                {
                    int previousLambda = dmatrix.Cells.Count;

                    foreach (var dim in dbDimensions.data)
                    {
                        StatDimension statDim = new StatDimension()
                        {
                            Code = dim.MdmCode,
                            Role = dim.DmrCode,
                            Sequence = dim.MdmSequence,
                            Value = dim.MdmValue,
                            Id = dim.MdmId,
                            Variables = new List<IDimensionVariable>()
                        };

                        var dbVariables = dAdo.GetItemsForDimension(ado, statDim.Id);
                        if (dbVariables.hasData)
                            foreach (var vrb in dbVariables.data)
                            {
                                DimensionVariable dv = new DimensionVariable()
                                {
                                    Code = vrb.DmtCode,
                                    Value = vrb.DmtValue,
                                    Sequence = vrb.DmtSequence,
                                    Elimination = vrb.DmtEliminationFlag,
                                    Decimals = (short)vrb.DmtDecimals,
                                    Unit = vrb.DmtUnit.Equals(DBNull.Value) ? null : vrb.DmtUnit
                                };
                                statDim.Variables.Add(dv);
                            }
                        statDim.Lambda = previousLambda / statDim.Variables.Count;
                        statDim.PreviousLambda = previousLambda;
                        previousLambda = statDim.Lambda;

                        spec.Dimensions.Add(statDim);
                    }


                }
                dmatrix.Dspecs.Add(dmatrix.Language, (Dspec)spec);
                return dmatrix;
            }
            catch (Exception ex)
            {
                Log.Instance.Error($"Failed to migrate matrix: id={mtrId}");
                Log.Instance.Error($"Error message: {ex.Message}");
                return dmatrix;
            }
        }

        private IEnumerable<dynamic> DeserializeData(ADO_readerOutput result)
        {
            return Utility.JsonDeserialize_IgnoreLoopingReference<List<dynamic>>(result.data[0].MtdData);

        }
    }
}
