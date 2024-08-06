using API;
using PxStat.Resources;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PxStat.Data
{
    /// <summary>
    /// IADO methods for Matrix
    /// </summary>
    internal class Matrix_IADO : DataAdaptor
    {
        /// <summary>
        /// class variable
        /// </summary>
        private IADO ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ado"></param>
        public Matrix_IADO(IADO ado)
        {
            this.ado = ado;
        }

        public Matrix_IADO()
        {
        }

        /// <summary>
        /// Load a single data field for e.g. a fractal query
        /// </summary>
        /// <param name="data"></param>
        /// <param name="matrixId"></param>
        /// <returns></returns>
        internal int LoadDataField(string data, int matrixId)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams(){ name="@MtrId",value=matrixId},
                new ADO_inputParams(){ name="@MatrixData", value=data}
            };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_Matrix_LoadSingleField", inputParams, ref returnParam);

            return (int)returnParam.value;

        }

        /// <summary>
        /// Create the new DMatrix
        /// </summary>
        /// <param name="data"></param>
        /// <param name="matrixId"></param>
        /// <returns></returns>
        internal int CreateNewMatrix(IDmatrix dmatrix, string username, int releaseId, string lngIsoCode)
        {
            
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name ="@MtrCode",value= dmatrix.Code},
                new ADO_inputParams() {name ="@MtrTitle",value= dmatrix.Dspecs[lngIsoCode].Title},
                new ADO_inputParams() {name ="@LngIsoCode",value= lngIsoCode},
                new ADO_inputParams() {name ="@FrmType",value= dmatrix.FormatType},
                new ADO_inputParams() {name ="@FrmVersion",value= dmatrix.FormatVersion},
                new ADO_inputParams() {name ="@CprValue",value= dmatrix.Copyright.CprValue},
                new ADO_inputParams() {name ="@MtrRlsId",value= releaseId},
                new ADO_inputParams() {name ="@userName",value= username}
            };
            if(dmatrix.Dspecs[lngIsoCode].NotesAsString!=null)
            {
                inputParams.Add(new ADO_inputParams() { name = "@MtrNote", value = dmatrix.Dspecs[lngIsoCode].NotesAsString });
            }
            if (dmatrix.MtrInput != null)
                inputParams.Add(new ADO_inputParams() { name = "@MtrInput", value = dmatrix.MtrInput });

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };
            ado.ExecuteNonQueryProcedure("Data_Matrix_Create", inputParams, ref returnParam);

            return (int)returnParam.value;
        }
    }
}
