using API;
using FluentValidation.Results;
using PxParser.Resources.Parser;
using PxStat.Data.Px;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

/// <summary>
/// This is to abstract out the Validate method and ultimately present it to 
/// (a) Matrix Create
/// (b) PxBuild validation requirements
/// </summary>
namespace PxStat.Data
{
    /// <summary>
    /// Delegate AsyncDeleteCaller
    /// </summary>
    /// <param name="rlsCode"></param>
    /// <param name="username"></param>
    public delegate void AsyncDeleteCaller(int rlsCode, string username);
    internal class Matrix_BSO
    {
        #region Properties
        /// <summary>
        /// class variable
        /// </summary>

        /// <summary>
        /// class variable
        /// </summary>
        internal dynamic ResponseError { get; set; }

        /// <summary>
        /// class variable 
        /// </summary>
        internal dynamic ResponseData { get; set; }
        /// <summary>
        /// class variable
        /// </summary>
        internal ValidationResult ParseValidatorResult { get; private set; }

        /// <summary>
        /// class variable
        /// </summary>
        internal ValidationResult SchemaValidatorResult { get; private set; }

        /// <summary>
        /// class variable
        /// </summary>
        internal ValidationResult IntegrityValidatorResult { get; private set; }

        /// <summary>
        /// class variable
        /// </summary>
        internal PxDocument PxDoc { get; set; }

        /// <summary>
        /// class variable
        /// </summary>
        internal ValidationResult SettingsValidatorResult { get; private set; }

        /// <summary>
        /// class variable
        /// </summary>
        private IADO Ado;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="ado"></param>
        internal Matrix_BSO(API.IADO ado)
        {
            Ado = ado;
        }
        #endregion




        /// <summary>
        /// Validates px schema
        /// </summary>
        /// <returns></returns>
        internal bool PxSchemaIsValid()
        {
            SchemaValidatorResult = new PxSchemaValidator(Ado).Validate(PxDoc); ;
            return SchemaValidatorResult.IsValid;
        }




        /// <summary>
        /// Get the latest release for a matrix
        /// </summary>
        /// <param name="theMatrix"></param>
        /// <returns></returns>
        internal Release_DTO GetLatestRelease(IDmatrix theMatrix)
        {
            Release_ADO releaseAdo = new Release_ADO(Ado);
            var releaseDTORead = new Release_DTO_Read() { MtrCode = theMatrix.Code };
            return Release_ADO.GetReleaseDTO(releaseAdo.ReadLatestIgnoreCancelled(releaseDTORead));
        }

        /// <summary>
        /// Get the latest release for a matrix
        /// </summary>
        /// <param name="theMatrix"></param>
        /// <returns></returns>
        internal Release_DTO GetLatestReleaseIncludingCancelled(IDmatrix theMatrix)
        {
            Release_ADO releaseAdo = new Release_ADO(Ado);
            var releaseDTORead = new Release_DTO_Read() { MtrCode = theMatrix.Code };
            return Release_ADO.GetReleaseDTO(releaseAdo.ReadLatestIncludingCancelled(releaseDTORead));
        }

        /// <summary>
        /// Clones a release
        /// </summary>
        /// <param name="releaseCode"></param>
        /// <param name="grpCode"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        internal int CloneRelease(int releaseCode, string grpCode, string username)
        {
            Release_ADO releaseAdo = new Release_ADO(Ado);
            //Clone the comment first
            return releaseAdo.Clone(releaseCode, grpCode, username);
        }

        internal int CloneReleaseToVersion(int releaseCode, int version, int revision, string username)
        {
            Release_ADO releaseAdo = new Release_ADO(Ado);
            //Clone the comment first
            return releaseAdo.CloneToVersion(releaseCode, version,revision, username);
        }

        internal int CloneComment(int releaseCode, int RlsIdNew, string username)
        {
            Release_ADO releaseAdo = new Release_ADO(Ado);
            return releaseAdo.CloneComment(releaseCode, RlsIdNew, username);
        }

        /// <summary>
        /// Create a new release for a Dmatrix
        /// </summary>
        /// <param name="theMatrix"></param>
        /// <param name="releaseVersion"></param>
        /// <param name="releaseRevision"></param>
        /// <param name="grpCode"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        internal int CreateRelease(IDmatrix theMatrix, int releaseVersion, int releaseRevision, string grpCode, string username)
        {
            Release_ADO releaseAdo = new Release_ADO(Ado);
            Release_DTO releaseDto = new Release_DTO()
            {
                GrpCode = grpCode,
                RlsLiveDatetimeFrom = DateTime.MinValue,
                RlsLiveDatetimeTo = DateTime.MaxValue,
                RlsRevision = releaseRevision,
                RlsVersion = releaseVersion
            };

            ValidationResult releaseValidatorResult = new ReleaseValidator().Validate(releaseDto);

            if (!releaseValidatorResult.IsValid)
            {
                Log.Instance.Debug(releaseValidatorResult.Errors);
                return 0;
            }

            return releaseAdo.Create(releaseDto, username);
        }





        /// <summary>
        /// Deletes a release based on release code
        /// </summary>
        /// <param name="rlsCode"></param>
        /// <param name="username"></param>
        internal void DeleteReleaseEntities(int rlsCode, string username)
        {
            using (Matrix_ADO matrixAdo = new Data.Matrix_ADO(AppServicesHelper.StaticADO))
            {
                matrixAdo.Delete(rlsCode, username);
            }

        }



    }
}
