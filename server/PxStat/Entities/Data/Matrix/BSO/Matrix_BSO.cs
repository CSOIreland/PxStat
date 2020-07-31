using API;
using FluentValidation.Results;
using PxParser.Resources.Parser;
using PxStat.Data.Px;
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
        internal Matrix MatrixData { get; set; }

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
        private ADO Ado;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="ado"></param>
        internal Matrix_BSO(API.ADO ado)
        {
            Ado = ado;
        }
        #endregion

        /// <summary>
        /// Validates user input
        /// </summary>
        /// <param name="DTO"></param>
        /// <returns></returns>
        internal bool Validate(PxUpload_DTO DTO)
        {
            if (!ParsePxFile(DTO))
            {
                LogValidatorErrors(ParseValidatorResult);
                ResponseError = Error.GetValidationFailure(ParseValidatorResult.Errors);
                return true;
            }

            if (!PxSchemaIsValid())
            {
                LogValidatorErrors(SchemaValidatorResult);
                ResponseError = Error.GetValidationFailure(SchemaValidatorResult.Errors);
                return true;
            }

            MatrixData = GetMatrixData(PxDoc);

            if (!PxIntegrityIsValid(MatrixData))
            {
                LogValidatorErrors(IntegrityValidatorResult);
                ResponseError = Error.GetValidationFailure(IntegrityValidatorResult.Errors);
                return true;
            }

            if (!PxSettingsAreValid(MatrixData))
            {
                LogValidatorErrors(SettingsValidatorResult);
                ResponseError = Error.GetValidationFailure(SettingsValidatorResult.Errors);
                return true;
            }

            ResponseData = API.JSONRPC.success;
            return true;
        }

        /// <summary>
        /// Validates px settings
        /// </summary>
        /// <param name="theMatrix"></param>
        /// <returns></returns>
        internal bool PxSettingsAreValid(Matrix theMatrix)
        {
            SettingsValidatorResult = new PxSettingsValidator(Ado).Validate(theMatrix);
            return SettingsValidatorResult.IsValid;
        }

        /// <summary>
        /// Validates px integrity
        /// </summary>
        /// <param name="theMatrix"></param>
        /// <returns></returns>
        private bool PxIntegrityIsValid(Matrix theMatrix)
        {
            IntegrityValidatorResult = new PxIntegrityValidator(theMatrix.MainSpec).Validate(theMatrix);
            return IntegrityValidatorResult.IsValid;
        }

        /// <summary>
        /// Gets matrix data from a serialized matrix file
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private Matrix GetMatrixData(PxDocument doc)
        {
            return new Matrix(doc);
        }

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
        /// Parses the px file
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal bool ParsePxFile(PxUpload_DTO dto)
        {
            ParseValidatorResult = new ValidationResult();
            try
            {
                PxDoc = PxStatEngine.ParsePxInput(dto.MtrInput);
            }
            catch (Exception e)
            {
                ParseValidatorResult.Errors.Add(new ValidationFailure("MtrInput", Label.Get("px.parse")));
                ParseValidatorResult.Errors.Add(new ValidationFailure("MtrInput", e.Message));
            }

            return PxDoc != null;
        }

        /// <summary>
        /// Logs errors
        /// </summary>
        /// <param name="validatorResult"></param>
        private static void LogValidatorErrors(ValidationResult validatorResult)
        {
            foreach (var error in validatorResult.Errors)
            {
                Log.Instance.Debug(error.ErrorMessage);
            }
        }

        /// <summary>
        /// Get the latest release for a matrix
        /// </summary>
        /// <param name="theMatrix"></param>
        /// <returns></returns>
        internal Release_DTO GetLatestRelease(Matrix theMatrix)
        {
            Release_ADO releaseAdo = new Release_ADO(Ado);
            var releaseDTORead = new Release_DTO_Read() { MtrCode = theMatrix.Code };
            return Release_ADO.GetReleaseDTO(releaseAdo.ReadLatest(releaseDTORead));
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

        internal int CloneComment(int releaseCode, int RlsIdNew, string username)
        {
            Release_ADO releaseAdo = new Release_ADO(Ado);
            return releaseAdo.CloneComment(releaseCode, RlsIdNew, username);
        }

        /// <summary>
        /// Create a new release
        /// </summary>
        /// <param name="theMatrix"></param>
        /// <param name="releaseVersion"></param>
        /// <param name="releaseRevision"></param>
        /// <param name="grpCode"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        internal int CreateRelease(Matrix theMatrix, int releaseVersion, int releaseRevision, string grpCode, string username)
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
        /// Create a matrix object
        /// </summary>
        /// <param name="theMatrix"></param>
        /// <param name="releaseId"></param>
        /// <param name="username"></param>
        /// <param name="DTO"></param>
        internal void CreateMatrix(Matrix theMatrix, int releaseId, string username, dynamic DTO)
        {
            CreateMatrixForLanguage(releaseId, theMatrix, theMatrix.MainSpec, username, DTO);

            if (theMatrix.OtherLanguages != null)
            {
                var i = 0;
                foreach (var language in theMatrix.OtherLanguages)
                {
                    CreateMatrixForLanguage(releaseId, theMatrix, theMatrix.OtherLanguageSpec[i], username, DTO);
                    ++i;
                }

            }
        }

        /// <summary>
        /// Creates the metadata for a specific language (px files may contain metadata in more than one language)
        /// </summary>
        /// <param name="releaseId"></param>
        /// <param name="theMatrix"></param>
        /// <param name="languageSpec"></param>
        /// <param name="username"></param>
        /// <param name="DTO"></param>
        private void CreateMatrixForLanguage(int releaseId, Matrix theMatrix, Matrix.Specification languageSpec, string username, dynamic DTO)
        {
            Matrix_ADO matrixAdo = new Data.Matrix_ADO(Ado);
            var matrixRecordDTO = GetMatrixDto(theMatrix, languageSpec, DTO);

            languageSpec.MatrixId = matrixAdo.CreateMatrixRecord(matrixRecordDTO, releaseId, username);

            languageSpec.Frequency.FrequencyId = CreateFrequency(languageSpec.Frequency, languageSpec.MatrixId);

            // if I have contvariable I use the content of contvariable to access the values with that value to obtain the names of stat products 
            // and I use codes with same name to get the correspondent codes!
            // otherwise

            // if there is only one statistical product we use contents for the title and we set the code at zero 0

            CreateStatisticalProducts(languageSpec.Statistic, languageSpec.MatrixId);

            CreateClassifications(languageSpec.Classification, languageSpec.MatrixId);


        }


        /// <summary>
        /// Deletes a release based on release code
        /// </summary>
        /// <param name="rlsCode"></param>
        /// <param name="username"></param>
        internal void DeleteReleaseEntities(int rlsCode, string username)
        {
            Matrix_ADO matrixAdo = new Data.Matrix_ADO(new API.ADO());
            matrixAdo.Delete(rlsCode, username);

        }

        /// <summary>
        /// Creates classifications
        /// </summary>
        /// <param name="classifications"></param>
        /// <param name="matrixId"></param>
        private void CreateClassifications(IList<ClassificationRecordDTO_Create> classifications, int matrixId)
        {
            Matrix_ADO matrixAdo = new Data.Matrix_ADO(Ado);

            DataTable variablesTable = new DataTable();
            variablesTable.Columns.Add("VRB_CODE");
            variablesTable.Columns.Add("VRB_VALUE");
            variablesTable.Columns.Add("VRB_CLS_ID");

            foreach (var classification in classifications)
            {
                classification.ClassificationId = matrixAdo.CreateClassificationRecord(classification, matrixId);
                foreach (var v in classification.Variable)
                {
                    variablesTable.Rows.Add(new Object[] { v.Code, v.Value, classification.ClassificationId });
                }
            }
            matrixAdo.CreateVariableRecordBulk(variablesTable);

            //we need to get the variable ID's for the new variables, so we need to read them for each classificatio
            foreach (var classification in classifications)
            {
                classification.Variable = matrixAdo.ReadClassificationVariables(classification.ClassificationId);

            }

        }

        /// <summary>
        /// Creates a frequency object and persists it to the database via bulk upload
        /// </summary>
        /// <param name="frequency"></param>
        /// <param name="matrixId"></param>
        /// <returns></returns>
        private int CreateFrequency(FrequencyRecordDTO_Create frequency, int matrixId)
        {
            Matrix_ADO matrixAdo = new Data.Matrix_ADO(Ado);

            frequency.FrequencyId = matrixAdo.CreateFrequencyRecord(frequency, matrixId);

            DataTable dtPeriods = new DataTable();

            dtPeriods.Columns.Add("PRD_CODE");
            dtPeriods.Columns.Add("PRD_VALUE");
            dtPeriods.Columns.Add("PRD_FRQ_ID");

            foreach (var p in frequency.Period)
            {

                DataRow dr = dtPeriods.NewRow();
                dr["PRD_CODE"] = p.Code;
                dr["PRD_VALUE"] = p.Value;
                dr["PRD_FRQ_ID"] = frequency.FrequencyId;
                dtPeriods.Rows.Add(dr);

            }

            matrixAdo.CreatePeriodRecordBulk(dtPeriods);

            //read the period id's for the uploaded periods
            List<dynamic> periodData = matrixAdo.ReadPeriodsByMatrixId(matrixId).ToList();

            foreach (var p in frequency.Period)
            {
                p.FrequencyPeriodId = periodData.Where(x => x.PrdCode == p.Code).FirstOrDefault().PrdId;
            }

            return frequency.FrequencyId;
        }

        /// <summary>
        /// Gets a Matrix_DTO object from a Matrix and Specification
        /// </summary>
        /// <param name="theMatrix"></param>
        /// <param name="spec"></param>
        /// <param name="DTO"></param>
        /// <returns></returns>
        private Matrix_DTO GetMatrixDto(Matrix theMatrix, Matrix.Specification spec, dynamic DTO)
        {
            return new Matrix_DTO()
            {
                MtrCode = theMatrix.Code,
                FrmType = theMatrix.FormatType,
                FrmVersion = theMatrix.FormatVersion,
                MtrOfficialFlag = theMatrix.IsOfficialStatistic,
                LngIsoCode = spec.Language,
                MtrNote = spec.NotesAsString,
                CprValue = spec.Source,
                MtrTitle = spec.Contents,
                MtrInput = DTO.MtrInput
            };
        }

        /// <summary>
        /// Creates an ID for statistic
        /// </summary>
        /// <param name="statistics"></param>
        /// <param name="matrixId"></param>
        private void CreateStatisticalProducts(IList<StatisticalRecordDTO_Create> statistics, int matrixId)
        {
            Matrix_ADO matrixAdo = new Data.Matrix_ADO(Ado);
            foreach (var stat in statistics)
            {
                stat.StatisticalProductId = matrixAdo.CreateStatisticalRecord(stat, matrixId);
            }
        }



    }
}
