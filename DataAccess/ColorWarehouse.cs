using System;
using System.Data;
using System.Data.SqlClient;
using WebForm.Common;
using WebForm.Models;

namespace WebForm.DataAccess
{
    public class ColorWarehouseDA
    {
        public decimal Create(ColorWarehouseRequest request)
        {
            try
            {
                var spParameter = new SqlParameter[6];

                #region Set param
                var parameter = new SqlParameter("@P_Name", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.Name
                };
                spParameter[0] = parameter;

                parameter = new SqlParameter("@P_Description", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.Description
                };
                spParameter[1] = parameter;

                parameter = new SqlParameter("@P_Avatar", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.Avatar
                };
                spParameter[2] = parameter;

                parameter = new SqlParameter("@P_CreateDate", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                };
                spParameter[3] = parameter;


                parameter = new SqlParameter("@P_Lstord", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.Lstord
                };
                spParameter[4] = parameter;


                parameter = new SqlParameter("@P_Return", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output,
                    Value = -1
                };
                spParameter[5] = parameter;
                #endregion

                SqlHelper.ExecuteNonQuery(ConfigInfo.ConnectString, CommandType.StoredProcedure,
                    "PROC_COLOR_INSERT",
                    spParameter);

                return Convert.ToDecimal(spParameter[5].Value);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return -1;
            }
        }

        public decimal Delete(int id)
        {
            try
            {
                var spParameter = new SqlParameter[1];

                #region Set param

                var parameter = new SqlParameter("@P_Id", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Input,
                    Value = id
                };
                spParameter[0] = parameter;

                #endregion

                SqlHelper.ExecuteNonQuery(ConfigInfo.ConnectString, CommandType.StoredProcedure,
                    "PROC_COLOR_DELETE",
                    spParameter);

                return 1;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return -1;
            }
        }

        public decimal Edit(ColorWarehouse model)
        {
            try
            {
                var spParameter = new SqlParameter[6];

                #region Set param

                var parameter = new SqlParameter("@P_Id", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Id
                };
                spParameter[0] = parameter;

                parameter = new SqlParameter("@P_Name", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Name
                };
                spParameter[1] = parameter;

                parameter = new SqlParameter("@P_Avatar", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Avatar
                };
                spParameter[2] = parameter;

                parameter = new SqlParameter("@P_Description", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Description
                };
                spParameter[3] = parameter;

                parameter = new SqlParameter("@P_Lstord", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Lstord
                };
                spParameter[4] = parameter;

                parameter = new SqlParameter("@P_Return", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output,
                    Value = -1
                };
                spParameter[5] = parameter;

                #endregion

                SqlHelper.ExecuteNonQuery(ConfigInfo.ConnectString, CommandType.StoredProcedure,
                    "PROC_COLOR_EDIT",
                    spParameter);

                return Convert.ToDecimal(spParameter[5].Value);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return -1;
            }
        }

        public DataSet Search(SearchColorWarehouseRequest model, ref int pTotal)
        {
            try
            {
                var spParameter = new SqlParameter[3];

                #region Set param

                var parameter = new SqlParameter("@P_START", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Start
                };
                spParameter[0] = parameter;

                parameter = new SqlParameter("@P_END", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.End
                };
                spParameter[1] = parameter;

                parameter = new SqlParameter("@P_TOTAL", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output,
                    Value = -1
                };
                spParameter[2] = parameter;

                #endregion

                var ds = SqlHelper.ExecuteDataset(ConfigInfo.ConnectString, CommandType.StoredProcedure,
                    "PROC_COLOR_SEARCH", spParameter);

                pTotal = Convert.ToInt32(spParameter[2].Value);

                return ds;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return new DataSet();
            }
        }

        public DataSet GetById(int id)
        {
            try
            {
                var spParameter = new SqlParameter[1];

                var parameter = new SqlParameter("@P_Id", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Input,
                    Value = id
                };
                spParameter[0] = parameter;


                var ds = SqlHelper.ExecuteDataset(ConfigInfo.ConnectString, CommandType.StoredProcedure,
                    "PROC_COLOR_GET_BY_ID", spParameter);

                return ds;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return new DataSet();
            }
        }
    }
}