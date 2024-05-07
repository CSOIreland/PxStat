using API;
using Microsoft.Extensions.Configuration;
using PxParser.Resources.Parser;
using PxStat.Build;
using PxStat.DataStore;
using PxStat.DBuild;
using PxStat.Resources;
using PxStat.Security;
using PxStat.System.Navigation;
using PxStat.Template;
using PxStat.Workflow;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

namespace PxStat.Data
{
    /// <summary>
    /// Class for creating a Matrix from a file upload
    /// </summary>
    public class Matrix_BSO_Create : BaseTemplate_Create<PxUpload_DTO, PxUploadValidator>
    {
        /// <summary>
        /// latestRelease
        /// </summary>
        private Release_DTO latestRelease;

        /// <summary>
        /// class variable
        /// </summary>
        /// 
        Matrix_ADO matrixAdo;

        /// <summary>
        /// class variable
        /// </summary>
        Release_ADO releaseAdo;

        /// <summary>
        /// class property
        /// </summary>
        private PxDocument PxDoc { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        public Matrix_BSO_Create(JSONRPC_API request) : base(request, new PxUploadValidator())
        {
            matrixAdo = new Matrix_ADO(Ado);
            releaseAdo = new Release_ADO(Ado);
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsModerator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            Build_BSO buildBso = new Build_BSO();

            if (!buildBso.HasBuildPermission(Ado, SamAccountName, "import"))
            {
                Response.error = Label.Get("error.privilege");
                return false;
            }

            Stopwatch swMatrix = new Stopwatch();
            swMatrix.Start();

            var signature = Utility.GetMD5(Configuration_BSO.GetStaticConfig ("APP_SALSA") + Utility.JsonSerialize_IgnoreLoopingReference(DTO.GetSignatureDTO()));
            if (signature != DTO.Signature)
            {
                Response.error = Label.Get("error.validation");
                return false;
            }

      
            // Create the matrix from the Matrix Factory
            DmatrixFactory dmatrixFactory = new DmatrixFactory(Ado);
           
            IDmatrix matrix = dmatrixFactory.CreateDmatrix(DTO);


            Matrix_BSO mBso = new Matrix_BSO(Ado);

            // Check if a WIP Release already exists for the Matrix to Upload
            latestRelease = mBso.GetLatestRelease(matrix);
            if (latestRelease != null && !DTO.Overwrite && releaseAdo.IsWip(latestRelease.RlsCode)) //
            {
                Group_DTO_Create dtoGroup = this.GetGroup(DTO.GrpCode);
                if (latestRelease.GrpCode != DTO.GrpCode)
                {
                    Response.data = String.Format(Label.Get("px.duplicate-different-group",DTO.LngIsoCode), matrix.Code, latestRelease.GrpName + " (" + latestRelease.GrpCode + ")", dtoGroup.GrpName + " (" + DTO.GrpCode + ")");
                }
                else
                {
                    Response.data = String.Format(Label.Get("px.duplicate",DTO.LngIsoCode), matrix.Code);
                }
                return true;
            }

            // Check if this Release already has a pending WorkflowRequest 
            if (latestRelease != null && new WorkflowRequest_ADO().IsCurrent(Ado, latestRelease.RlsCode))
            {
                Response.error = String.Format(Label.Get("error.workflow", DTO.LngIsoCode), matrix.Code);
                return false;
            }

            // Check if this Release has another pending live release
            if (latestRelease != null && new Release_ADO(Ado).IsLiveNext(latestRelease.RlsCode))
            {
                Response.error = String.Format(Label.Get("px.pendinglive", DTO.LngIsoCode), matrix.Code);
                return false;
            }


            var newAdo= new ADO("defaultConnection");
            
            //Check if the matrix code is locked in the dataset table
            using (DatasetAdo dAdo = new DatasetAdo(newAdo))
            {
               
               
                try
                {
                    newAdo.StartTransaction();
                    ADO_readerOutput dResult = dAdo.ReadDatasetLocked(matrix.Code);
                    if (dResult.hasData)
                    {
                        DateTime lockedTime = dResult.data[0].DttDatetimeLocked.Equals(DBNull.Value) ? default : (DateTime)dResult.data[0].DttDatetimeLocked;
                        if (lockedTime.AddMinutes(Configuration_BSO.GetApplicationConfigItem(ConfigType.server, "release.lockTimeMinutes")) > DateTime.Now)
                        {
                            Response.error = Label.Get("error.release.locked", DTO.LngIsoCode);
                            return false;
                        }
                    }
                    dAdo.DatasetLockUpdate(matrix.Code, DateTime.Now);
                    newAdo.CommitTransaction();
                }
                catch  { 
                    newAdo.RollbackTransaction();
                    throw;
                }
                
            }


            //Sort the time dimension variables by default. Bring the data values with you!

            //Do the time dimensions need sorting? (and hence the related datapoints)
            var timeDimension = matrix.Dspecs[matrix.Language].Dimensions.Where(x => x.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_TIME)).First();

            if (timeDimension != null)
            {
                DBuild_BSO bid = new DBuild_BSO();
                if (!bid.AreVariablesSequential(timeDimension))
                {
                    timeDimension.Variables = timeDimension.Variables.OrderBy(x => x.Code).ToList();

                    Dictionary<int, int> sequenceDictionary = new Dictionary<int, int>();
                    int counter = 1;
                    foreach (var vrb in timeDimension.Variables)
                    {
                        sequenceDictionary.Add(vrb.Sequence, counter);
                        counter++;
                    }
                    matrix = bid.SortVariablesInDimension(matrix, sequenceDictionary, timeDimension.Sequence);
                }
            }
            else
            {

                Response.error = Label.Get("error.invalid", DTO.LngIsoCode);
                return false;
            }


            int releaseId;
            if (latestRelease != null)
            {
                if (latestRelease.RlsLiveFlag)
                {
                    releaseId = mBso.CloneRelease(latestRelease.RlsCode, DTO.GrpCode, SamAccountName);
                    mBso.CloneComment(latestRelease.RlsCode, releaseId, SamAccountName);
                }
                else
                {
                    if (latestRelease.GrpCode != DTO.GrpCode)
                        releaseId = releaseAdo.IncrementRevision(latestRelease.RlsCode, SamAccountName, DTO.GrpCode);
                    else
                        releaseId = releaseAdo.IncrementRevision(latestRelease.RlsCode, SamAccountName);

                    matrixAdo.Delete(latestRelease.RlsCode, SamAccountName);
                }
            }
            else
            {
                releaseId = mBso.CreateRelease(matrix, 0, 1, DTO.GrpCode, SamAccountName);
            }

            newAdo=new ADO("defaultConnection");
            newAdo.StartTransaction();
            // Update the lock on the matrix code in the dataset table
            using (DatasetAdo dAdo = new DatasetAdo(newAdo))
            {
                try
                {
                    dAdo.DatasetLockUpdate(matrix.Code, default);
                    newAdo.CommitTransaction();
                }
                catch
                {
                    newAdo.RollbackTransaction();
                    throw;
                }
            }

            Stopwatch swLoad = new Stopwatch();
            swLoad.Start();

            // Write matrix and associated metaData to the database
            IDataWriter dataWriter = new DataWriter();
            dataWriter.CreateAndLoadDataField(Ado, matrix, SamAccountName, releaseId);
            dataWriter.CreateAndLoadMetadata(Ado, matrix);
            Keyword_Release_BSO_CreateMandatory krBSO = new();



            krBSO.Create(Ado, matrix, releaseId, SamAccountName);

            krBSO.RemoveDupes(Ado, releaseId);

            swLoad.Stop();
            Log.Instance.Info(string.Format("matrix loaded in db in {0} ms", Math.Round((double)swLoad.ElapsedMilliseconds)));

            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];// JSONRPC.success;
            return true;
        }

        public override bool PostExecute()
        {
            if (latestRelease != null)
            {
                // Clean up caching
                Cas.RunCasFlush(Resources.Constants.C_CAS_DATA_COMPARE_READ_ADDITION + latestRelease.RlsCode);
                Cas.RunCasFlush(Resources.Constants.C_CAS_DATA_COMPARE_READ_DELETION + latestRelease.RlsCode);
                Cas.RunCasFlush(Resources.Constants.C_CAS_DATA_COMPARE_READ_AMENDMENT + latestRelease.RlsCode);
                Cas.RunCasFlush(Resources.Constants.C_CAS_DATA_CUBE_READ_PRE_DATASET + latestRelease.RlsCode);
                Cas.RunCasFlush(Resources.Constants.C_CAS_DATA_CUBE_READ_PRE_METADATA + latestRelease.RlsCode);
            }
            return true;
        }

        /// <summary>
        /// Read a Group
        /// </summary>
        /// <param name="groupCode"></param>
        /// <returns></returns>
        private Group_DTO_Create GetGroup(string groupCode)
        {
            Group_ADO gAdo = new Security.Group_ADO();
            Group_DTO_Read gDto = new Group_DTO_Read();
            gDto.GrpCode = DTO.GrpCode;
            ADO_readerOutput readGroupOutput = gAdo.Read(Ado, gDto);

            Group_DTO_Create readGroup = new Group_DTO_Create();
            readGroup.GrpCode = groupCode;
            readGroup.GrpName = readGroupOutput.data[0].GrpName;

            return readGroup;
        }
    }
}
