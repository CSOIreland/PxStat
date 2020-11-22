using API;
using Newtonsoft.Json.Linq;
using PxStat.Data;
using PxStat.Template;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace PxStat.Build
{
    /// <summary>
    /// Build a px file from json parameters
    /// </summary>
    internal class Build_BSO_Create : BaseTemplate_Read<Build_DTO, Build_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Build_BSO_Create(JSONRPC_API request) : base(request, new Build_VLD_Create())
        { }

        /// <summary>
        /// Test privileges
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

            Build_BSO pBso = new Build_BSO();

            if (!pBso.HasBuildPermission(Ado, SamAccountName, "create"))
            {
                Response.error = Label.Get("error.privilege");
                return false;
            }

            Matrix matrix = new Matrix(DTO)
            {
                MainSpec = new Matrix.Specification(DTO.matrixDto.LngIsoCode, DTO),
                TheLanguage = DTO.LngIsoCode,
                FormatType = DTO.Format.FrmType,
                FormatVersion = DTO.Format.FrmVersion
            };


            //Get the Specifications
            if (DTO.DimensionList.Count > 1)
            {
                IEnumerable<string> otherLanguages = from lng in DTO.DimensionList where lng.LngIsoCode != DTO.matrixDto.LngIsoCode select lng.LngIsoCode;
                matrix.OtherLanguageSpec = new List<Matrix.Specification>();
                List<string> otherLang = new List<string>();
                foreach (var lang in otherLanguages)
                {
                    matrix.OtherLanguageSpec.Add(new Matrix.Specification(lang, DTO));
                    otherLang.Add(lang);
                }
                matrix.OtherLanguages = otherLang;
            }

            //Create the blank csv with titles to enable the user to fill in their own data for the update



            matrix.Cells = GetBlankCells(matrix);


            //We should be able to validate the newly created matrix now...
            MatrixValidator mValidator = new MatrixValidator();
            if (!mValidator.Validate(matrix))
            {
                Response.error = Label.Get("error.validation");
                return false;
            }

            dynamic fileOutput = new ExpandoObject();

            switch (DTO.Format.FrmType)
            {
                case Resources.Constants.C_SYSTEM_PX_NAME:

                    List<dynamic> resultPx = new List<dynamic>();
                    resultPx.Add(matrix.GetPxObject(true));
                    Response.data = resultPx;
                    break;

                case Resources.Constants.C_SYSTEM_JSON_STAT_NAME:
                    dynamic result = new ExpandoObject();
                    List<JRaw> jsonData = new List<JRaw>();
                    jsonData.Add(new JRaw(Serialize.ToJson(matrix.GetJsonStatObject())));


                    if (matrix.OtherLanguageSpec != null)
                    {
                        foreach (Matrix.Specification s in matrix.OtherLanguageSpec)
                        {
                            matrix.MainSpec = s;
                            jsonData.Add(new JRaw(Serialize.ToJson(matrix.GetJsonStatObject())));
                        }
                    }

                    Response.data = jsonData;
                    break;

                default:
                    Response.error = Label.Get("error.invalid");
                    return false;

            }

            return true;
        }

        /// <summary>
        /// For the px file to be valid, there must be a time value and some data
        /// This supplies the blank data
        /// </summary>
        /// <param name="mtr"></param>
        /// <returns></returns>
        private List<dynamic> GetBlankCells(Matrix mtr)
        {
            List<dynamic> cells = new List<dynamic>();
            int cellCount = 0;
            foreach (var v in mtr.MainSpec.Classification)
            {
                cellCount = cellCount == 0 ? 1 : cellCount;
                cellCount = cellCount * v.Variable.Count;
            }


            cellCount = cellCount == 0 ? 1 : cellCount * mtr.MainSpec.Frequency.Period.Count;


            cellCount = cellCount * mtr.MainSpec.Statistic.Count;
            for (int i = 1; i <= cellCount; i++)
            {
                dynamic cell = new ExpandoObject();
                cell.TdtValue = "";
                cells.Add(cell);
            }
            if (cellCount == 0) return null;
            return cells;
        }
    }
}
