using API;
using PxStat.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using static PxStat.Data.Matrix;

namespace PxStat.Data
{
    /// <summary>
    /// Compare BSO class
    /// </summary>
    internal class Compare_BSO
    {
        /// <summary>
        /// Compare Releases and find data points that have changed
        /// </summary>
        /// <param name="Ado"></param>
        /// <param name="dtoRight"></param>
        /// <param name="dtoLeft"></param>
        /// <returns></returns>
        internal Matrix CompareAmendment(ADO Ado, Compare_DTO_Read dtoRight, Compare_DTO_Read dtoLeft)
        {
            bool totalChange = false;


            Release_DTO lDto = new Release_DTO();
            Release_DTO rDto = new Release_DTO();

            lDto.RlsCode = dtoLeft.RlsCode;
            rDto.RlsCode = dtoRight.RlsCode;

            Matrix leftMatrix = new Matrix(Ado, lDto, dtoLeft.LngIsoCode);
            Matrix rightMatrix = new Matrix(Ado, rDto, dtoRight.LngIsoCode);


            Specification spec = new Data.Matrix.Specification();
            spec.Statistic = leftMatrix.MainSpec.Statistic.Intersect(rightMatrix.MainSpec.Statistic).ToList<StatisticalRecordDTO_Create>();
            spec.Frequency = new FrequencyRecordDTO_Create();
            spec.Frequency.Code = rightMatrix.MainSpec.Frequency.Code;
            spec.Frequency.Value = rightMatrix.MainSpec.Frequency.Value;
            spec.Frequency.Period = new List<PeriodRecordDTO_Create>();
            spec.Frequency.Period = leftMatrix.MainSpec.Frequency.Period.Intersect(rightMatrix.MainSpec.Frequency.Period).ToList<PeriodRecordDTO_Create>();



            //We now get the added variables. Note that this only applies where the classifications themselves have not been added
            //Classifications will contain (1) Brand new classifications, (2) classifications that have had variables added to them
            List<ClassificationRecordDTO_Create> intersectCls = leftMatrix.MainSpec.Classification.Intersect(rightMatrix.MainSpec.Classification).ToList<ClassificationRecordDTO_Create>();

            //if there are different classifications in each matrix (something added or deleted) then return all false for amendments
            if (intersectCls != null)
            {
                if (intersectCls.Count != leftMatrix.MainSpec.Classification.Count || intersectCls.Count != rightMatrix.MainSpec.Classification.Count)
                    totalChange = true;
            }
            else totalChange = true;

            spec.Classification = new List<ClassificationRecordDTO_Create>();

            foreach (ClassificationRecordDTO_Create cls in intersectCls)
            {
                ClassificationRecordDTO_Create otherCls = rightMatrix.MainSpec.Classification.Where(x => x.Equals(cls)).FirstOrDefault();//lefttoright
                List<VariableRecordDTO_Create> newVars = cls.Variable.Intersect(otherCls.Variable).ToList<VariableRecordDTO_Create>();

                if (newVars.Count > 0)
                {
                    cls.Variable = newVars;
                    spec.Classification.Add(cls);
                }
                if (newVars.Count == 0)
                    totalChange = true;
            }

            //If there are no periods or statistics in common then everything has changed and this is flagged
            if (spec.Frequency.Period.Count == 0 || spec.Statistic.Count == 0)
                totalChange = true;

            leftMatrix.MainSpec = spec;

            //If there are no periods in common then nothing could have been amended - prepare for a graceful exit
            if (spec.Frequency.Period.Count > 0)
                rightMatrix.MainSpec = spec;

            //Get the matrix based on a database read of the DTO RlsCode
            Matrix_ADO mAdo = new Matrix_ADO(Ado);


            //Get the data for leftMatrix
            //Get the data for rightMatrix
            //Compare and flag where they're not equal
            //return the matrix

            leftMatrix.TimeFilterWasApplied = true;
            rightMatrix.TimeFilterWasApplied = true;
            leftMatrix.StatFilterWasApplied = true;
            rightMatrix.StatFilterWasApplied = true;
            foreach (var cls in leftMatrix.MainSpec.Classification)
            {
                cls.ClassificationFilterWasApplied = true;
            }

            if (!totalChange)
            {

                leftMatrix = new Cube_ADO(Ado).ReadCubeData(leftMatrix);
            }

            rightMatrix = new Cube_ADO(Ado).ReadCubeData(rightMatrix);




            int counter = 0;
            foreach (var cell in rightMatrix.Cells)
            {
                //If there are no periods in common then nothing could have been amended.
                //Similarly, if there was a difference in the number of classifications
                if (totalChange)
                {
                    cell.WasAmendment = false;
                }
                else
                {
                    if (cell.TdtValue.Equals(DBNull.Value)) cell.TdtValue = Configuration_BSO.GetCustomConfig("px.confidential-value");
                    if (leftMatrix.Cells.ElementAt(counter).TdtValue.Equals(DBNull.Value)) leftMatrix.Cells.ElementAt(counter).TdtValue = Configuration_BSO.GetCustomConfig("px.confidential-value");

                    if (cell.TdtValue != leftMatrix.Cells.ElementAt(counter).TdtValue)
                        cell.WasAmendment = true;
                    else
                        cell.WasAmendment = false;
                }

                counter++;
            }



            return rightMatrix;

        }
        /// <summary>
        /// Compare Releases and find data points that have either been added or deleted
        /// </summary>
        /// <param name="Ado"></param>
        /// <param name="dtoRight"></param>
        /// <param name="dtoLeft"></param>
        /// <returns></returns>
        internal Matrix CompareAddDelete(ADO Ado, Compare_DTO_Read dtoRight, Compare_DTO_Read dtoLeft)
        {

            Release_DTO lDto = new Release_DTO();
            Release_DTO rDto = new Release_DTO();

            lDto.RlsCode = dtoLeft.RlsCode;
            rDto.RlsCode = dtoRight.RlsCode;


            Matrix leftMatrix = new Matrix(Ado, lDto, dtoLeft.LngIsoCode);
            Matrix rightMatrix = new Matrix(Ado, rDto, dtoRight.LngIsoCode);


            Specification spec = new Specification();


            spec.Classification = leftMatrix.MainSpec.Classification.Except(rightMatrix.MainSpec.Classification).ToList<ClassificationRecordDTO_Create>();

            bool test = leftMatrix.MainSpec.Statistic.Equals(rightMatrix.MainSpec.Statistic);

            spec.Statistic = leftMatrix.MainSpec.Statistic.Except(rightMatrix.MainSpec.Statistic).ToList<StatisticalRecordDTO_Create>();


            spec.Frequency = new FrequencyRecordDTO_Create();
            spec.Frequency.Period = new List<PeriodRecordDTO_Create>();
            spec.Frequency.Period = leftMatrix.MainSpec.Frequency.Period.Except(rightMatrix.MainSpec.Frequency.Period).ToList<PeriodRecordDTO_Create>();




            //Classifications will contain (1) Brand new classifications, (2) classifications that have had variables added to them
            List<ClassificationRecordDTO_Create> intersectCls = leftMatrix.MainSpec.Classification.Intersect(rightMatrix.MainSpec.Classification).ToList<ClassificationRecordDTO_Create>();
            foreach (ClassificationRecordDTO_Create cls in intersectCls)
            {
                ClassificationRecordDTO_Create otherCls = rightMatrix.MainSpec.Classification.Where(x => x.Equals(cls)).FirstOrDefault();
                List<VariableRecordDTO_Create> newVars = cls.Variable.Except(otherCls.Variable).ToList<VariableRecordDTO_Create>();

                ClassificationRecordDTO_Create newCls = new ClassificationRecordDTO_Create();
                newCls.Code = cls.Code;
                newCls.Value = cls.Value;
                newCls.Variable = new List<VariableRecordDTO_Create>();

                if (newVars.Count > 0)
                {
                    newCls.Variable = newVars;
                    spec.Classification.Add(newCls);
                }
            }
            leftMatrix.TimeFilterWasApplied = true;

            leftMatrix.StatFilterWasApplied = true;
            foreach (var cls in leftMatrix.MainSpec.Classification)
            {
                cls.ClassificationFilterWasApplied = true;
            }


            //Get the matrix based on a database read of the DTO RlsCode
            Matrix_ADO mAdo = new Matrix_ADO(Ado);


            //Get the metadata for the left matrix as a list of DataItem_DTO
            List<DataItem_DTO> resDataLeft = GetCellMetadata(Ado, dtoLeft.LngIsoCode, dtoLeft.RlsCode, dtoRight.RlsCode, leftMatrix.MainSpec);

            //Get the data for the added-to or deleted-from matrix
            leftMatrix = new Cube_ADO(Ado).ReadCubeData(leftMatrix);

            //Get the WasAmendment flag set on the list of items. This is calculated by referring to the extra dimensions in spec
            resDataLeft = GetFlaggedItemsAddDelete(resDataLeft, spec, leftMatrix);


            //This is the final result of Add/Delete
            leftMatrix = GetFlaggedMatrix(leftMatrix, resDataLeft);

            return leftMatrix;

        }
        /// <summary>
        /// Get a list of datapoints with their associated dimension metadata
        /// </summary>
        /// <param name="Ado"></param>
        /// <param name="lngIsoCode"></param>
        /// <param name="RlsCodeLeft"></param>
        /// <param name="RlsCodeRight"></param>
        /// <param name="spec"></param>
        /// <returns></returns>
        private List<DataItem_DTO> GetCellMetadata(ADO Ado, string lngIsoCode, int RlsCodeLeft, int RlsCodeRight, Specification spec)
        {
            Matrix_ADO mAdo = new Matrix_ADO(Ado);


            List<DimensionValue_DTO> dimensions = new List<DimensionValue_DTO>();


            DimensionValue_DTO sVal = new DimensionValue_DTO();
            sVal.dimType = DimensionType.STATISTIC;
            foreach (StatisticalRecordDTO_Create stat in spec.Statistic)
            {
                DimensionDetail_DTO detail = new DimensionDetail_DTO();
                detail.key = stat.Code;
                detail.value = stat.Value;
                detail.dimensionValue = sVal;
                sVal.details.Add(detail);
            }
            dimensions.Add(sVal);


            DimensionValue_DTO pVal = new DimensionValue_DTO();
            pVal.dimType = DimensionType.PERIOD;
            foreach (PeriodRecordDTO_Create per in spec.Frequency.Period)
            {
                DimensionDetail_DTO detail = new DimensionDetail_DTO();
                detail.key = per.Code;
                detail.value = per.Value;
                detail.dimensionValue = pVal;
                pVal.details.Add(detail);
            }
            dimensions.Add(pVal);

            foreach (ClassificationRecordDTO_Create cls in spec.Classification)
            {
                DimensionValue_DTO cVal = new DimensionValue_DTO();
                cVal.dimType = DimensionType.CLASSIFICATION;
                cVal.code = cls.Code;
                cVal.value = cls.Value;
                foreach (var vrb in cls.Variable)
                {
                    DimensionDetail_DTO detail = new DimensionDetail_DTO();
                    detail.key = vrb.Code;
                    detail.value = vrb.Value;
                    detail.dimensionValue = cVal;
                    cVal.details.Add(detail);
                }
                dimensions.Add(cVal);

            }

            var graph = CartesianProduct(dimensions[0].details.ToArray());

            for (int i = 1; i < dimensions.Count; i++)
            {
                graph = CartesianProduct(graph.ToArray(), dimensions[i].details.ToArray());
            }
            List<DataItem_DTO> itemList = new List<DataItem_DTO>();
            //int counter = 0;
            foreach (var item in graph)
            {
                DataItem_DTO dto = new DataItem_DTO();
                populateDataItem(ref dto, item);
                itemList.Add(dto);

            }


            return itemList;

        }
        /// <summary>
        ///  Get cartesian join of all of the elements in inputs
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        internal static IEnumerable<object[]> CartesianProduct(params object[][] inputs)
        {
            return inputs.Aggregate(
                (IEnumerable<object[]>)new object[][] { new object[0] },
                (soFar, input) =>
                    from prevProductItem in soFar
                    from item in input
                    select prevProductItem.Concat(new object[] { item }).ToArray());
        }
        /// <summary>
        /// Recursive function to package the graph of dimensions into a list of DataItem_DTO
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="item"></param>
        private void populateDataItem(ref DataItem_DTO dto, object[] item)
        {
            foreach (var subItem in item)
            {
                if (subItem.GetType().IsArray)
                {
                    populateDataItem(ref dto, (object[])subItem);
                }
                else
                {
                    var v = (DimensionDetail_DTO)subItem;
                    switch (v.dimensionValue.dimType)
                    {
                        case DimensionType.STATISTIC:
                            dto.statistic.Code = v.key;
                            dto.statistic.Value = v.value;
                            break;
                        case DimensionType.PERIOD:
                            dto.period.Code = v.key;
                            dto.period.Value = v.value;
                            break;
                        case DimensionType.CLASSIFICATION:
                            var cls = new ClassificationRecordDTO_Create();
                            cls.Code = v.dimensionValue.code;
                            cls.Value = v.dimensionValue.value;
                            var vrb = new VariableRecordDTO_Create();
                            vrb.Code = v.key;
                            vrb.Value = v.value;
                            cls.Variable = new List<VariableRecordDTO_Create>();
                            cls.Variable.Add(vrb);
                            dto.classifications.Add(cls);
                            break;

                    }
                }

            }
        }


        /// <summary>
        /// For Add and Delete - mark items that have been amended
        /// </summary>
        /// <param name="dataItems"></param>
        /// <param name="theSpec"></param>
        /// <returns></returns>
        internal List<DataItem_DTO> GetFlaggedItemsAddDelete(List<DataItem_DTO> dataItems, Specification theSpec, Matrix theMatrixData)
        {
            int counter = 0;
            //Now mark any ADDED datapoint
            foreach (DataItem_DTO dataItem in dataItems)
            {
                dataItem.dataValue = theMatrixData.Cells.ElementAt(counter).TdtValue.Equals(DBNull.Value) ? Configuration_BSO.GetCustomConfig("px.confidential-value") : theMatrixData.Cells.ElementAt(counter).TdtValue;


                counter++;
                //if a new classification was added then we must mark everything as amended
                if (theSpec.Classification.Count > 0)
                {

                    foreach (ClassificationRecordDTO_Create cls in dataItem.classifications)
                    {

                        //cls will have just one variable - is that a new variable? If so, then flag the data point
                        ClassificationRecordDTO_Create specCls = theSpec.Classification.Where(x => x.Code == cls.Code).FirstOrDefault();
                        if (specCls != null && specCls.Variable.Where(x => x.Code == cls.Variable.FirstOrDefault().Code).Count() > 0)
                        {
                            dataItem.wasAmendment = true;

                            //the datapoint has been marked as amended, we can now just go straight to the next data point the the foreach loop
                            continue;
                        }

                    }

                }
                //If the datapoint has a new statistic then we mark the datapoint as amended
                if (theSpec.Statistic.Count > 0)
                {
                    if (theSpec.Statistic.Where(x => x.Code == dataItem.statistic.Code).Count() > 0)
                    {
                        dataItem.wasAmendment = true;
                        //we're in a foreach loop, so there's no point in checking the period - the datapoint is already marked and we move on to the next data point

                        continue;
                    }
                }

                //If the datapoint has a new period then we mark the datapoint as amended
                if (theSpec.Frequency.Period.Count > 0)
                {
                    if (theSpec.Frequency.Period.Where(x => x.Code == dataItem.period.Code).Count() > 0) dataItem.wasAmendment = true;
                }


            }

            return dataItems;
        }



        /// <summary>
        /// Add the Cells to the matrix, complete with the wasAmendment flag
        /// </summary>
        /// <param name="theMatrixData"></param>
        /// <param name="dataItems"></param>
        /// <returns></returns>
        internal Matrix GetFlaggedMatrix(Matrix theMatrixData, List<DataItem_DTO> dataItems)
        {
            dataItems = dataItems.OrderBy(x => x.id).ToList<DataItem_DTO>();


            theMatrixData.Cells = new List<dynamic>();

            foreach (var v in dataItems)
            {
                dynamic cell = new ExpandoObject();
                cell.TdtValue = v.dataValue;
                cell.WasAmendment = v.wasAmendment;
                ((IList)theMatrixData.Cells).Add(cell);
            }


            return theMatrixData;
        }



    }





}
