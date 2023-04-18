using API;
using Autofac;
using PxStat.Data;
using PxStat.JsonStatSchema;
using PxStat.Resources;
using PxStat.System.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PxStat.DataStore
{

    public class DataReader : IDataReader
    {

        IADO _ado;
        string _lngIsoCode;
        IDspec _spec;

        IContainer Container;


        public DataReader()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ADO>().As<IADO>().WithParameter("connectionName", "defaultConnection");
            Container = builder.Build();
        }

        /// <summary>
        /// Query the data source based on a query object
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="query"></param>
        /// <param name="rDto"></param>
        /// <returns></returns>
        public IDmatrix QueryDataset(IADO ado, IMetaData metaData, CubeQuery_DTO query, Release_DTO rDto)
        {
            _ado = ado;
            _lngIsoCode = query.jStatQueryExtension.extension.Language.Code;
            IDmatrix matrix = new MatrixFactory().Get(ado, rDto);

            //Load the matrix from the data and metaData
            ReadDmatrix(ref matrix, ado, metaData, matrix.Release, _lngIsoCode);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            //Run the query against the matrix
            IDmatrix queriedMatrix = RunFractalQuery(matrix, query);

            sw.Stop();
            return matrix;
        }

        public IDmatrix QueryDataset(CubeQuery_DTO query, IDmatrix matrix)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            matrix.Dspecs[matrix.Language] = (Dspec)LoadDspec(matrix);

            //Send empty data back if the query is referring to wrong variables
            if (!ValidateQueryForMatrix(query, matrix.Dspecs[matrix.Language]))
            {
                return null;
            }
            else
            {

                //Run the query against the matrix
                IDmatrix queriedMatrix = RunFractalQuery(matrix, query);
            }
            sw.Stop();
            return matrix;
        }



        public bool ValidateQueryForMatrix(CubeQuery_DTO query, Dspec spec)
        {
            bool isOk = true;



            //Check for non-existing dimensions
            foreach (var d in query.jStatQuery.Dimensions)
            {
                if (spec.Dimensions.Select(x => x.Code == d.Key).ToList().Where(x => x == true).Count() == 0) return false;
            }





            return isOk;
        }

        /// <summary>
        /// Query the database with a related matrix having possibly reduced metaData.
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="queryMatrix"></param>
        /// <returns></returns>
        public IDmatrix QueryDataset(IADO ado, IMetaData metaData, IDmatrix queryMatrix, string queryLngIsoCode)
        {
            IDmatrix matrix = new MatrixFactory().Get(ado, queryMatrix.Release);

            //Load the matrix from the data and metaData
            ReadDmatrix(ref matrix, ado, metaData, matrix.Release, queryLngIsoCode);

            //Run the query against the matrix
            IDmatrix queriedMatrix = RunFractalQuery(matrix, queryMatrix.Dspecs[queryLngIsoCode]);
            return matrix;
        }

        public IDmatrix GetDataset(IADO ado, IMetaData metaData, string lngIsoCode, Release_DTO rDto)
        {
            _ado = ado;
            IDmatrix matrix = new MatrixFactory().Get(ado, rDto);

            //Load the matrix from the data and metaData
            ReadDmatrix(ref matrix, ado, metaData, matrix.Release, lngIsoCode);
            return matrix;
        }


        /// <summary>
        /// Run the query against the matrix. Return a subset of the data and metaData depending on the query
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private IDmatrix RunFractalQuery(IDmatrix matrix, CubeQuery_DTO query)
        {
            if (_spec == null) _spec = matrix.Dspecs[matrix.Language];
            //For each dimension, what variables are being queried? The result is stored in the QueryDimensionOrdinals
            //property of each dimension
            ApplyQueryToDimensions(ref _spec, query);

            //For each dimension, what ordinals of Cells would be selected if only that dimension was being queried?
            Parallel.ForEach(_spec.Dimensions, dim =>
            {
                dim.QualifyingOrdinals = GetQualifyingOrdinals(dim, matrix.Cells.Count);
                List<IDimensionVariable> queriedVariableList = new List<IDimensionVariable>();
                foreach (int s in dim.QueryDimensionOrdinals)
                    queriedVariableList.Add(dim.Variables[s - 1]);
                //We can also take the opportunity to amend the dimension to make it correspond to the query:
                dim.Variables = queriedVariableList;
            });


            //Keep this code for debugging :-)

            //foreach (var dim in _spec.Dimensions)
            //{
            //    dim.QualifyingOrdinals = GetQualifyingOrdinals(dim, matrix.Cells.Count);
            //    List<IDimensionVariable> queriedVariableList = new List<IDimensionVariable>();
            //    foreach (int s in dim.QueryDimensionOrdinals)
            //        queriedVariableList.Add(dim.Variables[s - 1]);
            //    //We can also take the opportunity to amend the dimension to make it correspond to the query:
            //    dim.Variables = queriedVariableList;
            //}


            //Now do a linq join of the QueryDimensionOrdinals properties of each dimension
            //This will result in a list of Cells ordinals that correspond to the query
            var selectedCells = GetJoinedQueriedOrdinals(matrix);
            List<dynamic> filteredCells = new List<dynamic>();


            //Express the matrix cells as a list of ordered objects
            var sequenceCells = from pair in matrix.Cells.ToList().Select((value, index) => new { value, index })
                                select new SequencedCell() { Sequence = pair.index, Value = pair.value };


            //Get the cells corresponding to the selecte sequences
            var output = from sCell in sequenceCells
                         join cell in selectedCells
                         on sCell.Sequence equals cell
                         orderby sCell.Sequence
                         select new
                         {
                             sCell.Value
                         };


            matrix.Cells = output.Select(x => x.Value).ToList();


            return matrix;
        }

        public IDmatrix RunFractalQueryMetadata(IDmatrix matrix, CubeQuery_DTO query)
        {
            if (_spec == null) _spec = matrix.Dspecs[matrix.Language];
            //For each dimension, what variables are being queried? The result is stored in the QueryDimensionOrdinals
            //property of each dimension
            ApplyQueryToDimensions(ref _spec, query);

            //For each dimension, what ordinals of Cells would be selected if only that dimension was being queried?
            Parallel.ForEach(_spec.Dimensions, dim =>
            {
                dim.QualifyingOrdinals = GetQualifyingOrdinals(dim, matrix.Cells.Count);
                List<IDimensionVariable> queriedVariableList = new List<IDimensionVariable>();
                foreach (int s in dim.QueryDimensionOrdinals)
                    queriedVariableList.Add(dim.Variables[s - 1]);
                //We can also take the opportunity to amend the dimension to make it correspond to the query:
                dim.Variables = queriedVariableList;
            });

            return matrix;
        }


        /// <summary>
        /// Run fractal query using another matrix as query metaData
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="querySpec"></param>
        /// <returns></returns>
        public IDmatrix RunFractalQuery(IDmatrix matrix, IDspec querySpec)
        {

            //For each dimension, what variables are being queried? The result is stored in the QueryDimensionOrdinals
            //property of each dimension
            ApplyQueryToDimensions(ref _spec, querySpec);

            //For each dimension, what ordinals of Cells would be selected if only that dimension was being queried?
            Parallel.ForEach(_spec.Dimensions, dim =>
            {
                dim.QualifyingOrdinals = GetQualifyingOrdinals(dim, matrix.Cells.Count);
                List<IDimensionVariable> queriedVariableList = new List<IDimensionVariable>();
                foreach (int s in dim.QueryDimensionOrdinals)
                    queriedVariableList.Add(dim.Variables[s - 1]);
                //We can also take the opportunity to amend the dimension to make it correspond to the query:
                dim.Variables = queriedVariableList;
            });


            //Now do a linq join of the QueryDimensionOrdinals properties of each dimension
            //This will result in a list of Cells ordinals that correspond to the query
            var selectedCells = GetJoinedQueriedOrdinals(matrix);

            var sequenceCells = from pair in matrix.Cells.ToList().Select((value, index) => new { value, index })
                                select new SequencedCell() { Sequence = pair.index, Value = pair.value };

            //foreach (int s in selectedCells)
            //    filteredCells.Add(matrix.Cells.ToList()[s]);

            var output = from sCell in sequenceCells
                         join cell in selectedCells
                         on sCell.Sequence equals cell
                         orderby sCell.Sequence
                         select new
                         {
                             sCell.Value
                         };

            // result = new List<int>(output);

            matrix.Cells = output.Select(x => x.Value).ToList();


            return matrix;
        }

        /// <summary>
        /// Run fractal query using another matrix as query metaData. This version just returns the list of selected ordinals
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="querySpec"></param>
        /// <returns></returns>
        public List<int> RunFractalQueryOrdinalsOnly(IDmatrix matrix, IDspec querySpec,string lngIsoCode)
        {
            _spec = matrix.Dspecs[lngIsoCode];
            //For each dimension, what variables are being queried? The result is stored in the QueryDimensionOrdinals
            //property of each dimension
            ApplyQueryToDimensions(ref _spec, querySpec);

            //For each dimension, what ordinals of Cells would be selected if only that dimension was being queried?
            //Parallel.ForEach(_spec.Dimensions, dim =>
            //{
            //    dim.QualifyingOrdinals = GetQualifyingOrdinals(dim, matrix.Cells.Count);
            //    List<IDimensionVariable> queriedVariableList = new List<IDimensionVariable>();
            //    foreach (int s in dim.QueryDimensionOrdinals)
            //        queriedVariableList.Add(dim.Variables[s - 1]);
                
            //});

            foreach( var dim in _spec.Dimensions)
            {
                dim.QualifyingOrdinals = GetQualifyingOrdinals(dim, matrix.Cells.Count);
                List<IDimensionVariable> queriedVariableList = new List<IDimensionVariable>();
                foreach (int s in dim.QueryDimensionOrdinals)
                    queriedVariableList.Add(dim.Variables[s - 1]);
            }


            //Now do a linq join of the QueryDimensionOrdinals properties of each dimension
            //This will result in a list of Cells ordinals that correspond to the query
            var selectedCells = GetJoinedQueriedOrdinals(matrix);

            return selectedCells;
        }

        public IDmatrix RunFractalQuery(IDmatrix matrix, IDspec querySpec, string lngIsoCode)
        {

            //For each dimension, what variables are being queried? The result is stored in the QueryDimensionOrdinals
            //property of each dimension
            IDspec refSpec = matrix.Dspecs[lngIsoCode];
            refSpec = LoadDspec(matrix);
            querySpec = LoadDspec(querySpec);
            ApplyQueryToDimensions(ref refSpec, querySpec);

            //For each dimension, what ordinals of Cells would be selected if only that dimension was being queried?
            Parallel.ForEach(refSpec.Dimensions, dim =>
            {
                dim.QualifyingOrdinals = GetQualifyingOrdinals(dim, matrix.Cells.Count);
                List<IDimensionVariable> queriedVariableList = new List<IDimensionVariable>();
                foreach (int s in dim.QueryDimensionOrdinals)
                    queriedVariableList.Add(dim.Variables[s - 1]);
                // We can also take the opportunity to amend the dimension to make it correspond to the query:
                dim.Variables = queriedVariableList;
            });


            //foreach (var dim in refSpec.Dimensions)
            //{
            //    dim.QualifyingOrdinals = GetQualifyingOrdinals(dim, matrix.Cells.Count);
            //    List<IDimensionVariable> queriedVariableList = new List<IDimensionVariable>();
            //    foreach (int s in dim.QueryDimensionOrdinals)
            //        queriedVariableList.Add(dim.Variables[s - 1]);
            //    //We can also take the opportunity to amend the dimension to make it correspond to the query:
            //    dim.Variables = queriedVariableList;
            //}



            //Now do a linq join of the QueryDimensionOrdinals properties of each dimension
            //This will result in a list of Cells ordinals that correspond to the query
            var selectedCells = GetJoinedQueriedOrdinals(matrix);
            List<dynamic> filteredCells = new List<dynamic>();

            var sequenceCells = from pair in matrix.Cells.ToList().Select((value, index) => new { value, index })
                                select new SequencedCell() { Sequence = pair.index, Value = pair.value };

            //foreach (int s in selectedCells)
            //    filteredCells.Add(matrix.Cells.ToList()[s]);

            var output = from sCell in sequenceCells
                         join cell in selectedCells
                         on sCell.Sequence equals cell
                         orderby sCell.Sequence
                         select new
                         {
                             sCell.Value
                         };

            // result = new List<int>(output);

            matrix.Cells = output.Select(x => x.Value).ToList();


            return matrix;
        }



        //Each dimension has a list of cells ordinals that would hold true if it were the only dimension queried
        //Here we get an actual list of what cells have been queried by doing a progressive linq join on all of them
        private List<int> GetJoinedQueriedOrdinals(IDmatrix matrix)
        {
            if (_lngIsoCode == null) _lngIsoCode = matrix.Language;
            List<int> result = new List<int>();
            result.AddRange(Enumerable.Range(0, matrix.Cells.Count));

            foreach (var dim in matrix.Dspecs[_lngIsoCode].Dimensions)
            {

                var output = result.Join(dim.QualifyingOrdinals,
                    res => res,
                    ord => ord,
                    (res, ord) => res);

                result = new List<int>(output);

            }
            return result;
        }

        //Gets a list of cells ordinals that hold true if the dimension were the only one being queried
        private List<int> GetQualifyingOrdinals(IStatDimension dim, int mcount)
        {
            List<int> qualifyingOrdinals = new List<int>();

            //iterates through the dataset
            int p_from = 0;

            //keeps a count of which item we're looking at in our current dimension
            int stepper = 1;

            while (p_from < mcount)
            {
                //If we've come upon a new section of the previous lambda (i.e. start counting our items again from the start)
                if (p_from % dim.PreviousLambda == 0)
                    stepper = 1;

                //If this is an item in the query..
                if (dim.QueryDimensionOrdinals.Contains(stepper))
                    qualifyingOrdinals.Add(p_from);

                p_from++;

                //If we've come upon a new section of our current lambda, move the count forwards
                if (p_from % dim.Lambda == 0)
                    stepper++;

            }


            return qualifyingOrdinals;
        }

        /// <summary>
        /// Converts the query item entries for each dimension from an array of codes to an array of ordinals
        /// </summary>
        /// <param name="dto"></param>
        private void ApplyQueryToDimensions(ref IDspec spec, CubeQuery_DTO dto)
        {


            Parallel.ForEach(spec.Dimensions, d =>
            {
                var codes = dto.jStatQuery.Id;
                if (codes.Contains(d.Code))
                {
                    d.QueryDimensionOrdinals = new List<int>();
                    // There is a query defined for this dimension
                    List<string> itemStringList = new List<string>();

                    if (dto.jStatQuery.Dimensions.ContainsKey(d.Code))
                        itemStringList = (dto.jStatQuery.Dimensions[d.Code].Category.Index).ToList();

                    //Convert these to a list of integers
                    //If the list of integers is empty, we presume that all items for the dimension are required (same as not specifying the dim code in the query)
                    if (itemStringList.Count > 0)
                    {
                        foreach (var item in itemStringList)
                        {
                            //if a valid variable is found in the query then add it, otherwise ignore
                            if (d.Variables.Where(x => x.Code == item).Count() > 0)
                                d.QueryDimensionOrdinals.Add((d.Variables.Where(x => x.Code == item).FirstOrDefault().Sequence));

                        }
                    }
                    else d.QueryDimensionOrdinals = d.Variables.Select(x => x.Sequence).ToList();
                }
                else
                    d.QueryDimensionOrdinals = d.Variables.Select(x => x.Sequence).ToList();

                //ordinals may or may not have been added according to their sequence, so we must sort them
                d.QueryDimensionOrdinals.Sort();
            });

            //Keep the following code for debugging

            //foreach (var d in spec.Dimensions)
            //{
            //    var codes = dto.jStatQuery.Id;
            //    if (codes.Contains(d.Code))
            //    {
            //        d.QueryDimensionOrdinals = new List<int>();
            //        // There is a query defined for this dimension
            //        List<string> itemStringList = new List<string>();

            //        if (dto.jStatQuery.Dimensions.ContainsKey(d.Code))
            //            itemStringList = (dto.jStatQuery.Dimensions[d.Code].Category.Index).ToList();

            //        //Convert these to a list of integers
            //        //If the list of integers is empty, we presume that all items for the dimension are required (same as not specifying the dim code in the query)
            //        if (itemStringList.Count > 0)
            //        {
            //            foreach (var item in itemStringList)
            //            {
            //                //if a valid variable is found in the query then add it, otherwise ignore
            //                if (d.Variables.Where(x => x.Code == item).Count() > 0)
            //                    d.QueryDimensionOrdinals.Add((d.Variables.Where(x => x.Code == item).FirstOrDefault().Sequence));

            //            }
            //        }
            //        else d.QueryDimensionOrdinals = d.Variables.Select(x => x.Sequence).ToList();
            //    }
            //    else
            //        d.QueryDimensionOrdinals = d.Variables.Select(x => x.Sequence).ToList();

            //    //ordinals may or may not have been added according to their sequence, so we must sort them
            //    d.QueryDimensionOrdinals.Sort();

            //}



        }


        /// <summary>
        /// Converts the query item entries for each dimension from an array of codes to an array of ordinals
        /// This version is for where we query by another matrix/spec
        /// </summary>
        /// <param name="dto"></param>
        private void ApplyQueryToDimensions(ref IDspec spec, IDspec querySpec)
        {


            Parallel.ForEach(spec.Dimensions, d =>
            {

                if (querySpec.Dimensions.Select(x => x.Code).ToList().Contains(d.Code))
                {
                    d.QueryDimensionOrdinals = new List<int>();
                    // There is a query defined for this dimension
                    var qDimension = querySpec.Dimensions.Where(x => x.Code == d.Code).FirstOrDefault();
                    List<string> itemStringList = qDimension.Variables.Select(x => x.Code).ToList();
                    //Convert these to a list of integers
                    foreach (var item in itemStringList)
                    {
                        if (d.Variables.Where(x => x.Code == item).Count() > 0)
                        {
                            d.QueryDimensionOrdinals.Add((d.Variables.Where(x => x.Code == item).FirstOrDefault().Sequence));
                        }
                    }
                }
                else
                    d.QueryDimensionOrdinals = d.Variables.Select(x => x.Sequence).ToList();
            });

            //foreach (var d in spec.Dimensions)
            //{
            //    if (querySpec.Dimensions.Select(x => x.Code).ToList().Contains(d.Code))
            //    {
            //        d.QueryDimensionOrdinals = new List<int>();
            //        // There is a query defined for this dimension
            //        var qDimension = querySpec.Dimensions.Where(x => x.Code == d.Code).FirstOrDefault();
            //        List<string> itemStringList = qDimension.Variables.Select(x => x.Code).ToList();
            //        //Convert these to a list of integers
            //        foreach (var item in itemStringList)
            //        {
            //            if (d.Variables.Where(x => x.Code == item).Count() > 0)
            //            {
            //                d.QueryDimensionOrdinals.Add((d.Variables.Where(x => x.Code == item).FirstOrDefault().Sequence));
            //            }
            //        }
            //    }
            //    else
            //        d.QueryDimensionOrdinals = d.Variables.Select(x => x.Sequence).ToList();
            //}
        }

        public IDmatrix GetNonLiveData(IADO ado, IMetaData metaData, string lngIsoCode, Release_DTO rDto)
        {
            DataStore_ADO dAdo = new DataStore_ADO();
            IDmatrix matrix = ReadReleaseDmatrix(ado, metaData, rDto, lngIsoCode);
            matrix.Release.RlsCode = rDto.RlsCode;
            matrix.CreatedDateString = rDto.RlsLiveDatetimeFrom.ToString(metaData.GetPxDataTimeFormat());
            matrix.CreatedDateTime = rDto.RlsLiveDatetimeFrom;



            return matrix;
        }

        public IDmatrix GetLiveData(IADO ado, IMetaData metaData, string mtrCode, string lngIsoCode, Release_DTO rDto)
        {

            DataStore_ADO dAdo = new DataStore_ADO();
            IDmatrix matrix = ReadLiveDataset(mtrCode, lngIsoCode, rDto);
            matrix.Release.RlsCode = rDto.RlsCode;
            matrix.CreatedDateString = rDto.RlsLiveDatetimeFrom.ToString(metaData.GetPxDataTimeFormat());
            matrix.CreatedDateTime = rDto.RlsLiveDatetimeFrom;

            return matrix;
        }

        private IDmatrix ReadReleaseDmatrix(IADO ado, IMetaData metaData, Release_DTO releaseDto, string lngIsoCode)
        {
            IDmatrix dmatrix = new DmatrixFactory().CreateDmatrix();

            // start by reading the Matrix for tha release and language
            dmatrix.Release = releaseDto;
            dmatrix.CreatedDateString = releaseDto.RlsLiveDatetimeFrom.ToString(metaData.GetPxDataTimeFormat());
            dmatrix.CreatedDateTime = releaseDto.RlsLiveDatetimeFrom;

            DataStore_ADO dAdo = new DataStore_ADO();

            var dmatrixData = dAdo.ReadDMatrixByRelease(ado, releaseDto.RlsCode, lngIsoCode);

            if (!dmatrixData.hasData) return dmatrix;

            dmatrix.Code = dmatrixData.data[0].MtrCode;
            dmatrix.FormatVersion = dmatrixData.data[0].FrmVersion;
            dmatrix.IsOfficialStatistic = dmatrixData.data[0].MtrOfficialFlag;
            dmatrix.FormatType = dmatrixData.data[0].FrmType;
            dmatrix.Languages = new List<string>();
            dmatrix.Languages.Add(lngIsoCode);
            dmatrix.Release = releaseDto;
            dmatrix.Copyright.CprCode = dmatrixData.data[0].CprCode;
            dmatrix.Copyright.CprValue = dmatrixData.data[0].CprValue;
            dmatrix.Copyright.CprUrl = dmatrixData.data[0].CprUrl;
            dmatrix.Id = dmatrixData.data[0].MtrId;
            dmatrix.Language = lngIsoCode;



            IDspec dspec = new DSpecFactory().CreateDspec();
            dspec.Dimensions = new List<StatDimension>();
            dspec.Notes = new List<string>();

            if (dmatrix.Cells == null)
            {
                var dbMatrixData = dAdo.GetFieldDataForMatrix(ado, dmatrix.Id);
                dmatrix.Cells = (List<dynamic>)new MatrixFactory().DeserializeData(dbMatrixData);
                // dmatrix.Decimals = (short)dbMatrixData.data[0].MtdDecimal;
            }

            dmatrix.Dspecs = new Dictionary<string, Dspec>();

            IDspec spec = new Dspec();
            spec.Dimensions = new List<StatDimension>();
            spec.Contents = dmatrixData.data[0].MtrTitle;
            spec.CopyrightUrl = dmatrixData.data[0].CprUrl;
            spec.Language = lngIsoCode;
            spec.MatrixCode = dmatrix.Code;
            spec.MatrixId = dmatrix.Id;
            if (dmatrixData.data[0].MtrNote != null)
            {
                if (Cleanser.TryCast<List<string>>(dmatrixData.data[0].MtrNote, out List<string> result))
                {
                    spec.Notes = result;
                }
                else
                {
                    spec.Notes = new List<string>() { dmatrixData.data[0].MtrNote };
                }

            }
            spec.Source = dmatrix.Copyright.CprValue;
            spec.Title = dmatrixData.data[0].MtrTitle;

            spec.ContentVariable = Utility.GetCustomConfig("APP_CSV_STATISTIC");

            //Get the language appropriate version of the Product Value for this spec
            if (dmatrix.Release?.PrcCode != null)
            {
                
                    Product_ADO pAdo = new Product_ADO(ado);
                    Product_DTO pDTO = new Product_DTO() { LngIsoCode = spec.Language, PrcCode = dmatrix.Release.PrcCode };
                    var pResult = pAdo.Read(pDTO);
                    if (pResult.Count > 0)
                    {
                        spec.PrcValue = pResult[0].PrcValue;
                    }
                
            }

            //Get the language appropriate version of the Subject Value for this spec
            if(dmatrix.Release?.SbjCode !=null)
            {
                Subject_ADO sAdo = new Subject_ADO(ado);
                Subject_DTO sDto = new Subject_DTO() { SbjCode = dmatrix.Release.SbjCode, LngIsoCode = spec.Language };
                var sResult = sAdo.Read(sDto);
                if(sResult.Count > 0)
                {
                    spec.SbjValue= sResult[0].SbjValue;
                }
            }



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
                        GeoFlag = dim.MdmGeoFlag.Equals(DBNull.Value) ? null : dim.MdmGeoFlag,
                        GeoUrl = dim.MdmGeoUrl.Equals(DBNull.Value) ? null : dim.MdmGeoUrl,
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
            dmatrix.Dspecs.Add(lngIsoCode, (Dspec)spec);
            return dmatrix;
        }


        private void ReadDmatrix(ref IDmatrix dmatrix, IADO ado, IMetaData metaData, Release_DTO releaseDto, string lngIsoCode)
        {
            // start by reading the Matrx for tha release and language
            dmatrix.Release.RlsCode = releaseDto.RlsCode;
            dmatrix.CreatedDateString = releaseDto.RlsLiveDatetimeFrom.ToString(metaData.GetPxDataTimeFormat());
            dmatrix.CreatedDateTime = releaseDto.RlsLiveDatetimeFrom;

            DataStore_ADO dAdo = new DataStore_ADO();

            var dmatrixData = dAdo.ReadDMatrixByRelease(ado, releaseDto.RlsCode, lngIsoCode);

            if (!dmatrixData.hasData) return;

            dmatrix.Code = dmatrixData.data[0].MtrCode;
            dmatrix.FormatVersion = dmatrixData.data[0].FrmVersion;
            dmatrix.IsOfficialStatistic = dmatrixData.data[0].MtrOfficialFlag;
            dmatrix.FormatType = dmatrixData.data[0].FrmType;
            dmatrix.Languages.Add(dmatrixData.data[0].LngIsoCode);
            dmatrix.Release = releaseDto;
            dmatrix.Copyright.CprCode = dmatrixData.data[0].CprCode;
            dmatrix.Copyright.CprValue = dmatrixData.data[0].CprValue;
            dmatrix.Copyright.CprUrl = dmatrixData.data[0].CprUrl;
            dmatrix.Id = dmatrixData.data[0].MtrId;
            dmatrix.Language = dmatrixData.data[0].LngIsoCode;



            IDspec dspec = new DSpecFactory().CreateDspec();
            dspec.Dimensions = new List<StatDimension>();
            dspec.Notes = new List<string>();
            if (!dmatrixData.data[0]?.MtrNote.Equals(DBNull.Value))
                dspec.Notes.Add(dmatrixData.data[0].MtrNote);

            if (dmatrix.Release?.PrcCode != null)
            {

                Product_ADO pAdo = new Product_ADO(ado);
                Product_DTO pDTO = new Product_DTO() { LngIsoCode = dspec.Language, PrcCode = dmatrix.Release.PrcCode };
                var pResult = pAdo.Read(pDTO);
                if (pResult.Count > 0)
                {
                    dspec.PrcValue = pResult[0].PrcValue;
                }

            }

            if (dmatrix.Release?.SbjCode != null)
            {
                Subject_ADO sAdo = new Subject_ADO(ado);
                Subject_DTO sDTO = new Subject_DTO() { LngIsoCode = dspec.Language, SbjCode = dmatrix.Release.SbjCode };
                var sResult = sAdo.Read(sDTO);
                if(sResult.Count > 0)
                {
                    dspec.SbjValue= sResult[0].SbjValue; 
                }
            }


            if (dmatrix.Cells == null)
            {
                var dbMatrixData = dAdo.GetFieldDataForMatrix(ado, dmatrix.Id);
                dmatrix.Cells = (List<dynamic>)new MatrixFactory().DeserializeData(dbMatrixData);
                // dmatrix.Decimals = (short)dbMatrixData.data[0].MtdDecimal;
            }

            LoadDspec(ref dspec, ado, dmatrix, dmatrix.Cells.Count);

            dmatrix.Dspecs.Add(lngIsoCode, (Dspec)dspec);

            _spec = dspec;

            var reason = new ReasonRelease_ADO().Read(ado, new ReasonRelease_DTO_Read() { RlsCode = dmatrix.Release.RlsCode, LngIsoCode = lngIsoCode });


            if (reason.hasData)
            {
                foreach (var r in reason.data)
                {
                    dmatrix.Release.Reasons.Add(r.RsnValueExternal);
                }

            }


        }

        /// <summary>
        /// Loads the extra spec properties for a fractal query
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private IDspec LoadDspec(IDmatrix matrix)
        {
            IDspec spec = matrix.Dspecs[matrix.Language];
            int previousLambda = matrix.Cells.Count;
            foreach (IStatDimension dim in spec.Dimensions)
            {
                dim.Lambda = previousLambda / dim.Variables.Count;
                dim.PreviousLambda = previousLambda;
                previousLambda = dim.Lambda;
            }

            return spec;
        }

        private IDspec LoadDspec(IDspec spec)
        {
            int previousLambda = 1;
            foreach (var dim in spec.Dimensions)
            {
                previousLambda = previousLambda * dim.Variables.Count;
            }
            foreach (IStatDimension dim in spec.Dimensions)
            {
                dim.Lambda = previousLambda / dim.Variables.Count;
                dim.PreviousLambda = previousLambda;
                previousLambda = dim.Lambda;
            }

            return spec;
        }

        //Loads a spec from the data store
        private void LoadDspec(ref IDspec dspec, IADO ado, IDmatrix matrix, int cellCount)
        {
            dspec.Language = matrix.Language;
            dspec.Contents = matrix.Code;
            dspec.CopyrightUrl = matrix.Copyright.CprUrl;
            dspec.Source = matrix.Copyright.CprValue;
            dspec.Title = matrix.Code;
            dspec.ContentVariable = Label.Get("default.statistic");
            dspec.Dimensions = new List<StatDimension>();

            DataStore_ADO dAdo = new DataStore_ADO();
            var dbDimensions = dAdo.GetDimensionsForMatrix(ado, matrix.Id);

            if (dbDimensions.hasData)
            {
                int previousLambda = cellCount;

                foreach (var dim in dbDimensions.data)
                {
                    StatDimension statDim = new StatDimension()
                    {
                        Code = dim.MdmCode,
                        Role = dim.DmrCode,
                        Sequence = dim.MdmSequence,
                        Value = dim.MdmValue,
                        Id = dim.MdmId,
                        GeoFlag = dim.MdmGeoFlag.Equals(DBNull.Value) ? null : dim.MdmGeoFlag,
                        GeoUrl = dim.MdmGeoUrl.Equals(DBNull.Value) ? null : dim.MdmGeoUrl,
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
                                Decimals = vrb.DmtDecimals.Equals(DBNull.Value) ? (short)0 : (short)vrb.DmtDecimals,
                                Unit = vrb.DmtUnit.Equals(DBNull.Value) ? null : vrb.DmtUnit
                            };
                            statDim.Variables.Add(dv);
                        }
                    statDim.Lambda = previousLambda / statDim.Variables.Count;
                    statDim.PreviousLambda = previousLambda;
                    previousLambda = statDim.Lambda;

                    dspec.Dimensions.Add(statDim);
                }

            }


        }

        public IDmatrix ReadLiveDataset(string mtrCode, string lngIsoCode, Release_DTO rDto, bool metadataOnly = false)
        {

            DataStore_ADO dAdo = new DataStore_ADO();
            IDmatrix matrix = new DmatrixFactory().CreateDmatrix();

            if (rDto != null)
            {
                using (var ado = new ADO("defaultConnection"))
                {
                    var reasons = new ReasonRelease_ADO().Read(ado, new ReasonRelease_DTO_Read() { RlsCode = rDto.RlsCode, LngIsoCode = lngIsoCode });
                    if (reasons.hasData)
                    {
                        foreach (var r in reasons.data)
                        {
                            rDto.Reasons = new List<string>();
                            rDto.Reasons.Add(r.RsnValueExternal);
                        }
                    }
                }
            }

            ADO_readerOutput output = null;

            using (var scope = Container.BeginLifetimeScope())
            {
                var ado = scope.Resolve<IADO>();
                output = dAdo.DataMatrixReadLive(ado, mtrCode, lngIsoCode);

            }


            if (!output.hasData)
                return matrix;

            matrix.Code = output.data[0].MtrCode;
            matrix.FormatVersion = output.data[0].FrmVersion;
            matrix.IsOfficialStatistic = output.data[0].MtrOfficialFlag;
            matrix.FormatType = output.data[0].FrmType;
            matrix.Languages = new List<string>();
            matrix.Languages.Add(output.data[0].LngIsoCode);
            matrix.Release = rDto;
            matrix.Copyright.CprCode = output.data[0].CprCode;
            matrix.Copyright.CprValue = output.data[0].CprValue;
            matrix.Copyright.CprUrl = output.data[0].CprUrl;
            matrix.Id = output.data[0].MtrId;
            matrix.Language = output.data[0].LngIsoCode;

            matrix.Dspecs = new Dictionary<string, Dspec>();

            IDspec spec = new Dspec();
            spec.Dimensions = new List<StatDimension>();
            spec.Contents = output.data[0].MtrTitle;
            spec.CopyrightUrl = output.data[0].CprUrl;
            spec.Language = output.data[0].LngIsoCode;
            spec.MatrixCode = matrix.Code;
            spec.MatrixId = matrix.Id;
            spec.Notes = new List<string>();

            if (!output.data[0]?.MtrNote.Equals(DBNull.Value))
            {
                spec.Notes.Add(output.data[0].MtrNote);
            }
            spec.Source = matrix.Copyright.CprValue;
            spec.Title = output.data[0].MtrTitle;

            spec.ContentVariable = Utility.GetCustomConfig("APP_CSV_STATISTIC");

            if (matrix.Release?.PrcCode != null)
            {
                using (var ado = new ADO("defaultConnection"))
                {
                    Product_ADO pAdo = new Product_ADO(ado);
                    Product_DTO pDTO = new Product_DTO() { LngIsoCode = spec.Language, PrcCode = matrix.Release.PrcCode };
                    var pResult = pAdo.Read(pDTO);
                    if(pResult.Count>0)
                    {
                        spec.PrcValue = pResult[0].PrcValue;
                    }
                }
            }

            if(matrix.Release?.SbjCode !=null)
            {
                using (var ado=new ADO("defaultConnection"))
                {
                    Subject_ADO sAdo = new Subject_ADO(ado);
                    Subject_DTO sDTO = new Subject_DTO() { LngIsoCode = spec.Language, SbjCode = matrix.Release.SbjCode };
                    var sResult = sAdo.Read(sDTO);
                    if(sResult.Count>0)
                    {
                        spec.SbjValue = sResult[0].SbjValue;
                    }

                }
            }

                using (var scope = Container.BeginLifetimeScope())
            {
                var ado = scope.Resolve<IADO>();
                int previousLambda = 0;
                if (!metadataOnly)
                {
                    if (matrix.Cells == null)
                    {
                        var dbMatrixData = dAdo.GetFieldDataForMatrix(ado, matrix.Id);
                        matrix.Cells = (List<dynamic>)new MatrixFactory().DeserializeData(dbMatrixData);
                        // dmatrix.Decimals = (short)dbMatrixData.data[0].MtdDecimal;
                    }
                    previousLambda = matrix.Cells.Count;
                }
                var dbDimensions = dAdo.GetDimensionsForMatrix(ado, matrix.Id);

                if (dbDimensions.hasData)
                {


                    foreach (var dim in dbDimensions.data)
                    {
                        StatDimension statDim = new StatDimension()
                        {
                            Code = dim.MdmCode,
                            Role = dim.DmrCode,
                            Sequence = dim.MdmSequence,
                            Value = dim.MdmValue,
                            Id = dim.MdmId,
                            GeoFlag = dim.MdmGeoFlag.Equals(DBNull.Value) ? null : dim.MdmGeoFlag,
                            GeoUrl = dim.MdmGeoUrl.Equals(DBNull.Value) ? null : dim.MdmGeoUrl,
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

            }
            matrix.Dspecs.Add(lngIsoCode, (Dspec)spec);
            return matrix;
        }

        public IDmatrix ReadNonLiveDataset(string lngIsoCode, Release_DTO rDto, string ccnUsername, bool metadataOnly = false)
        {

            DataStore_ADO dAdo = new DataStore_ADO();
            IDmatrix matrix = new DmatrixFactory().CreateDmatrix();

            ADO_readerOutput output = null;

            using (var ado = new ADO("defaultConnection"))
            {
                var reasons = new ReasonRelease_ADO().Read(ado, new ReasonRelease_DTO_Read() { RlsCode = rDto.RlsCode, LngIsoCode = lngIsoCode });
                if (reasons.hasData)
                {
                    foreach (var r in reasons.data)
                    {
                        rDto.Reasons = new List<string>();
                        rDto.Reasons.Add(r.RsnValueExternal);
                    }
                }
            }

            using (var scope = Container.BeginLifetimeScope())
            {
                var ado = scope.Resolve<IADO>();
                output = dAdo.DataMatrixRead(ado, rDto.RlsCode, lngIsoCode, ccnUsername);

            }


            if (!output.hasData)
                return matrix;

            matrix.Code = output.data[0].MtrCode;
            matrix.FormatVersion = output.data[0].FrmVersion;
            matrix.IsOfficialStatistic = output.data[0].MtrOfficialFlag;
            matrix.FormatType = output.data[0].FrmType;
            matrix.Languages = new List<string>();
            matrix.Languages.Add(output.data[0].LngIsoCode);
            matrix.Release = rDto;
            matrix.Copyright.CprCode = output.data[0].CprCode;
            matrix.Copyright.CprValue = output.data[0].CprValue;
            matrix.Copyright.CprUrl = output.data[0].CprUrl;
            matrix.Id = output.data[0].MtrId;
            matrix.Language = output.data[0].LngIsoCode;

            matrix.Dspecs = new Dictionary<string, Dspec>();

            IDspec spec = new Dspec();
            spec.Dimensions = new List<StatDimension>();
            spec.Contents = output.data[0].MtrTitle;
            spec.CopyrightUrl = output.data[0].CprUrl;
            spec.Language = output.data[0].LngIsoCode;
            spec.MatrixCode = matrix.Code;
            spec.MatrixId = matrix.Id;
            if (output.data[0].MtrNote != null)
            {
                if (Cleanser.TryCast<List<string>>(output.data[0].MtrNote, out List<string> result))
                {
                    spec.Notes = result;
                }

            }

            spec.Source = matrix.Copyright.CprValue;
            spec.Title = output.data[0].MtrTitle;

            spec.ContentVariable = Utility.GetCustomConfig("APP_CSV_STATISTIC");

            if (matrix.Release?.PrcCode != null)
            {
                using (var ado = new ADO("defaultConnection"))
                {
                    Product_ADO pAdo = new Product_ADO(ado);
                    Product_DTO pDTO = new Product_DTO() { LngIsoCode = spec.Language, PrcCode = matrix.Release.PrcCode };
                    var pResult = pAdo.Read(pDTO);
                    if (pResult.Count > 0)
                    {
                        spec.PrcValue = pResult[0].PrcValue;
                    }
                }
            }

            if(matrix.Release?.SbjCode!=null)
            {
                using (var ado=new ADO("defaultConnection"))
                {
                    Subject_ADO sAdo = new Subject_ADO(ado);
                    Subject_DTO sDTO = new Subject_DTO() { LngIsoCode = spec.Language, SbjCode = matrix.Release.SbjCode };
                    var sResult = sAdo.Read(sDTO);  
                    if(sResult.Count > 0)
                    {
                        spec.SbjValue= sResult[0].SbjValue;
                    }
                }

            }

            using (var scope = Container.BeginLifetimeScope())
            {
                var ado = scope.Resolve<IADO>();
                int previousLambda = 0;
                if (!metadataOnly)
                {
                    if (matrix.Cells == null)
                    {
                        var dbMatrixData = dAdo.GetFieldDataForMatrix(ado, matrix.Id);
                        matrix.Cells = (List<dynamic>)new MatrixFactory().DeserializeData(dbMatrixData);
                        // dmatrix.Decimals = (short)dbMatrixData.data[0].MtdDecimal;
                    }
                    previousLambda = matrix.Cells.Count;
                }
                var dbDimensions = dAdo.GetDimensionsForMatrix(ado, matrix.Id);

                if (dbDimensions.hasData)
                {


                    foreach (var dim in dbDimensions.data)
                    {
                        StatDimension statDim = new StatDimension()
                        {
                            Code = dim.MdmCode,
                            Role = dim.DmrCode,
                            Sequence = dim.MdmSequence,
                            Value = dim.MdmValue,
                            Id = dim.MdmId,
                            GeoFlag = dim.MdmGeoFlag.Equals(DBNull.Value) ? null : dim.MdmGeoFlag,
                            GeoUrl = dim.MdmGeoUrl.Equals(DBNull.Value) ? null : dim.MdmGeoUrl,
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

            }
            matrix.Dspecs.Add(lngIsoCode, (Dspec)spec);
            return matrix;
        }


    }

    internal class SequencedCell
    {
        public dynamic Value { get; set; }
        public int Sequence { get; set; }
    }

}
