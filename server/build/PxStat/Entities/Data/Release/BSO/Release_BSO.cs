using API;
using PxStat.DataStore;
using PxStat.DBuild;
using PxStat.Resources;
using PxStat.Security;
using PxStat.System.Navigation;
using PxStat.Workflow;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using YamlDotNet.Core.Tokens;

namespace PxStat.Data
{
    internal class Release_BSO : IDisposable
    {
        internal string ErrorMessage { get; set; } = "";

        private IADO _ado;
        internal Release_BSO(IADO ado)
        {
            _ado = ado;
        }

        public void Dispose()
        {
        }

        internal bool IsLiveWithWip(int rlsCode, string samAccountName)
        {
            return GetWipForLive(rlsCode, samAccountName) != null;
        }



        internal dynamic GetWipForLive(int rlsCode, string samAccountName)
        {
            Release_ADO rAdo = new Release_ADO(_ado);
            if (!rAdo.IsLiveNow(rlsCode))
            {
                return null;
            }

            dynamic query = rAdo.Read(rlsCode, samAccountName);
            if (query == null)
            {
                return null;
            }
            query = rAdo.ReadLatest(new Release_DTO_Read() { MtrCode = query.MtrCode });
            if (query == null)
            {
                return null;
            }
            if (rAdo.IsWip(query.RlsCode))
            {
                return query;
            }

            return null;
        }

        internal dynamic GetPendingLiveForLive(int rlsCode, string samAccountName)
        {
            Release_ADO rAdo = new Release_ADO(_ado);
            if (!rAdo.IsLiveNow(rlsCode))
            {
                return null;
            }

            dynamic query = rAdo.Read(rlsCode, samAccountName);
            if (query == null)
            {
                return null;
            }
            query = rAdo.ReadPendingLive(new Release_DTO_Read() { MtrCode = query.MtrCode });
            if (query == null)
            {
                return null;
            }
            if (rAdo.IsLiveNext(query.RlsCode))
            {
                return query;
            }

            return null;
        }

        public int CreateRelease(IDmatrix matrix,  DBuild_DTO_UpdatePublish dto, string ccnUsername)
        {

            Release_DTO latestRelease = new();
            Matrix_BSO mBso = new Matrix_BSO(_ado);
            Release_ADO releaseAdo=new Release_ADO(_ado);
            string grpCode;

            // Check if a WIP Release already exists for the Matrix to Upload
            latestRelease = mBso.GetLatestRelease(matrix);
            grpCode = latestRelease.GrpCode;


            // Check if this Release already has a pending WorkflowRequest 
            if (latestRelease != null && new WorkflowRequest_ADO().IsCurrent(_ado, latestRelease.RlsCode))
            {
                
                this.ErrorMessage = String.Format(Label.Get("error.workflow", dto.LngIsoCode), dto.MtrCode);
                return 0;
            }

            // Check if this Release has another pending live release
            if (latestRelease != null && new Release_ADO(_ado).IsLiveNext(latestRelease.RlsCode))
            {
                this.ErrorMessage = String.Format(Label.Get("px.pendinglive", dto.LngIsoCode), dto.MtrCode);
                return 0;
            }


            var newAdo = new ADO("defaultConnection");

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
                            this.ErrorMessage = Label.Get("error.release.locked", dto.LngIsoCode);
                            return 0;
                        }
                    }
                    dAdo.DatasetLockUpdate(matrix.Code, DateTime.Now);
                    newAdo.CommitTransaction();
                }
                catch
                {
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
                this.ErrorMessage = Label.Get("error.publish", dto.LngIsoCode);
                return 0;
            }


            int releaseId;
            if (latestRelease != null)
            {
                if (latestRelease.RlsLiveFlag)
                {
                    releaseId = mBso.CloneRelease(latestRelease.RlsCode, grpCode, ccnUsername);
                    mBso.CloneComment(latestRelease.RlsCode, releaseId, ccnUsername);
                }
                else
                {
                    if (latestRelease.GrpCode != grpCode)
                        releaseId = releaseAdo.IncrementRevision(latestRelease.RlsCode, ccnUsername, grpCode);
                    else
                        releaseId = releaseAdo.IncrementRevision(latestRelease.RlsCode, ccnUsername);

                    new Matrix_ADO(_ado).Delete(latestRelease.RlsCode, ccnUsername);
                }
            }
            else
            {
                releaseId = mBso.CreateRelease(matrix, 0, 1, grpCode, ccnUsername);
            }

            newAdo = new ADO("defaultConnection");
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
            dataWriter.CreateAndLoadDataFieldDB(_ado, matrix, ccnUsername, releaseId);
            dataWriter.CreateAndLoadMetadata(_ado, matrix);
            Keyword_Release_BSO_CreateMandatory krBSO = new();



            krBSO.Create(_ado, matrix, releaseId, ccnUsername);

            krBSO.RemoveDupes(_ado, releaseId);

            swLoad.Stop();
            return releaseId;

        }

       

    }
}
