using API;
using PxParser.Resources.Parser;
using PxStat.Build;
using PxStat.Data.Px;
using PxStat.Security;
using PxStat.System.Navigation;
using PxStat.Template;
using PxStat.Workflow;
using System;
using System.Diagnostics;

namespace PxStat.Data
{
    /// <summary>
    /// Class for creating a Matrix from a file upload
    /// </summary>
    internal class Matrix_BSO_Create : BaseTemplate_Create<PxUpload_DTO, PxUploadValidator>
    {
        /// <summary>
        /// class variable
        /// </summary>
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
        internal Matrix_BSO_Create(JSONRPC_API request) : base(request, new PxUploadValidator())
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
            Stopwatch swMatrix = new Stopwatch();
            swMatrix.Start();

            var signature = Utility.GetMD5(Utility.GetCustomConfig("APP_SALSA") + Utility.JsonSerialize_IgnoreLoopingReference(DTO.GetSignatureDTO()));
            if (signature != DTO.Signature)
            {
                Response.error = Label.Get("error.validation");
                return false;
            }

            PxDoc = PxStatEngine.ParsePxInput(DTO.MtrInput);
            Matrix theMatrixData = new Matrix(PxDoc, DTO);
            Matrix_BSO mBso = new Matrix_BSO(Ado);
            Build_BSO buildBso = new Build_BSO();

            int releaseId;

            // Check if a WIP Release already exists for the Matrix to Upload
            var latestRelease = mBso.GetLatestRelease(theMatrixData);
            if (latestRelease != null && !DTO.Overwrite && releaseAdo.IsWip(latestRelease.RlsCode)) //
            {
                Group_DTO_Create dtoGroup = this.GetGroup(DTO.GrpCode);
                if (latestRelease.GrpCode != DTO.GrpCode)
                {
                    Response.data = String.Format(Label.Get("px.duplicate-different-group"), theMatrixData.Code, latestRelease.GrpName + " (" + latestRelease.GrpCode + ")", dtoGroup.GrpName + " (" + DTO.GrpCode + ")");
                }
                else
                {
                    Response.data = String.Format(Label.Get("px.duplicate"), theMatrixData.Code);
                }
                return true;
            }

            // Check if this Release already has a pending WorkflowRequest 
            if (latestRelease != null && new WorkflowRequest_ADO().IsCurrent(Ado, latestRelease.RlsCode))
            {
                Response.error = String.Format(Label.Get("px.workflow"), theMatrixData.Code);
                return false;
            }

            // Check if this Release has another pending live release
            if (latestRelease != null && new Release_ADO(Ado).IsLiveNext(latestRelease.RlsCode))
            {
                Response.error = String.Format(Label.Get("px.pendinglive"), theMatrixData.Code);
                return false;
            }


            if (latestRelease != null)
            {

                if (latestRelease.RlsLiveFlag)
                {
                    releaseId = mBso.CloneRelease(latestRelease.RlsCode, DTO.GrpCode, SamAccountName);
                }
                else
                {
                    if (latestRelease.GrpCode != DTO.GrpCode)
                        releaseId = releaseAdo.IncrementRevision(latestRelease.RlsCode, SamAccountName, DTO.GrpCode);
                    else
                        releaseId = releaseAdo.IncrementRevision(latestRelease.RlsCode, SamAccountName);

                    matrixAdo.Delete(latestRelease.RlsCode, SamAccountName);
                }
                // Clean up caching
                MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_DATA_COMPARE_READ_ADDITION + latestRelease.RlsCode);
                MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_DATA_COMPARE_READ_DELETION + latestRelease.RlsCode);
                MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_DATA_COMPARE_READ_AMENDMENT + latestRelease.RlsCode);
                MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_DATA_COMPARE_READ_PREVIOUS_RELEASE + latestRelease.RlsCode);

                MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_DATA_CUBE_READ_PRE_DATASET + latestRelease.RlsCode);
                MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_DATA_CUBE_READ_PRE_METADATA + latestRelease.RlsCode);
            }
            else
            {
                releaseId = mBso.CreateRelease(theMatrixData, 0, 1, DTO.GrpCode, SamAccountName);
            }

            mBso.CreateMatrix(theMatrixData, releaseId, SamAccountName, DTO);
            DTO = null;

            swMatrix.Stop();
            Log.Instance.Info(string.Format("Matrix object created in {0} ms", Math.Round((double)swMatrix.ElapsedMilliseconds)));

            Stopwatch swLoad = new Stopwatch();
            swLoad.Start();

            //Do a Cartesian join to correctly label each data point with its dimensions
            //Create bulk tables from this and load them to the database
            _ = buildBso.CreateAndLoadDataTables(Ado, theMatrixData, true);

            matrixAdo.MarkMatrixAsContainingData(theMatrixData.MainSpec.MatrixId, true);

            Keyword_Release_BSO_CreateMandatory krBSO = new Keyword_Release_BSO_CreateMandatory();

            krBSO.Create(Ado, releaseId, SamAccountName);

            swLoad.Stop();
            Log.Instance.Info(string.Format("Matrix loaded in DB in {0} ms", Math.Round((double)swLoad.ElapsedMilliseconds)));

            Response.data = JSONRPC.success;
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
