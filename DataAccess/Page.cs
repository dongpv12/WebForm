using System;
using System.Data;
using System.Data.SqlClient;
using WebForm.Common;
using WebForm.Models;

namespace WebForm.DataAccess
{
    public class PageDA
    {
        public decimal Create(PageRequest request)
        {
            try
            {
                var spParameter = new SqlParameter[10];

                #region Set param
                var parameter = new SqlParameter("@P_Title", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.Title
                };
                spParameter[0] = parameter;

                parameter = new SqlParameter("@P_Content", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.Content
                };
                spParameter[1] = parameter;

                parameter = new SqlParameter("@P_Tag", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.Tag
                };
                spParameter[2] = parameter;

                parameter = new SqlParameter("@P_CreateDate", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                };
                spParameter[3] = parameter;

                parameter = new SqlParameter("@P_CreateBy", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.CreateBy
                };
                spParameter[4] = parameter;

                parameter = new SqlParameter("@P_Return", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output,
                    Value = -1
                };
                spParameter[5] = parameter;
                #endregion

                SqlHelper.ExecuteNonQuery(ConfigInfo.ConnectString, CommandType.StoredProcedure, "PROC_PAGE_INSERT",
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

                SqlHelper.ExecuteNonQuery(ConfigInfo.ConnectString, CommandType.StoredProcedure, "PROC_PAGE_DELETE",
                    spParameter);

                return 1;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return -1;
            }
        }

        public decimal Edit(Page model)
        {
            try
            {
                var spParameter = new SqlParameter[9];

                #region Set param

                var parameter = new SqlParameter("@P_Id", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Id
                };
                spParameter[0] = parameter;

                parameter = new SqlParameter("@P_Title", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Title
                };
                spParameter[1] = parameter;

                parameter = new SqlParameter("@P_Content", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Content
                };
                spParameter[2] = parameter;

                parameter = new SqlParameter("@P_Tag", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Tag
                };
                spParameter[3] = parameter;

                parameter = new SqlParameter("@P_Return", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output,
                    Value = -1
                };
                spParameter[4] = parameter;
                #endregion

                SqlHelper.ExecuteNonQuery(ConfigInfo.ConnectString, CommandType.StoredProcedure, "PROC_PAGE_EDIT",
                    spParameter);

                return Convert.ToDecimal(spParameter[4].Value);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return -1;
            }
        }

        public DataSet Search(SearchProjectRequest model, ref int pTotal)
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
                    "PROC_PAGE_SEARCH", spParameter);

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
                    "PROC_PAGE_GET_BY_ID", spParameter);

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