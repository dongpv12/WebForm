using System.Data;
using System.Data.SqlClient;
using WebForm.Common;
using WebForm.Models;

namespace WebForm
{
    public class TVChartUserConfigDA
    {
        public DataSet TVChartUserConfig_GetAll()
        {
            try
            {
                var spParameter = new SqlParameter[0];
                var ds = SqlHelper.ExecuteDataset(ConfigInfo.ConnectString, CommandType.StoredProcedure,
                   "PROC_CHART_CONFIG_GETALL", spParameter);
                return ds;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return new DataSet();
            }
        }

        public int TVChartUserConfig_Add(TVChartUserConfig request)
        {
            int result = -1;
            try
            {

                var spParameter = new SqlParameter[10];

                #region Set param
                var parameter = new SqlParameter("@p_name", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.name
                };
                spParameter[0] = parameter;

                parameter = new SqlParameter("@p_content", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.content
                };
                spParameter[1] = parameter;

                parameter = new SqlParameter("@p_symbol", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.symbol
                };
                spParameter[2] = parameter;

                parameter = new SqlParameter("@p_resolution", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.resolution
                };
                spParameter[3] = parameter;


                parameter = new SqlParameter("@p_timestamp", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.timestamp
                };
                spParameter[4] = parameter;

                parameter = new SqlParameter("@p_userid", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.userid
                };
                spParameter[5] = parameter;

                parameter = new SqlParameter("@p_username", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.username
                };
                spParameter[6] = parameter;

                parameter = new SqlParameter("@p_createddate", SqlDbType.DateTime)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.createddate
                };
                spParameter[7] = parameter;

                parameter = new SqlParameter("@p_createdby", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.createdby
                };
                spParameter[8] = parameter;


                parameter = new SqlParameter("@P_Return", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output,
                    Value = -1
                };
                spParameter[9] = parameter;
                #endregion

                SqlHelper.ExecuteNonQuery(ConfigInfo.ConnectString, CommandType.StoredProcedure,
                    "PROC_CHART_CONFIG_INSERT",
                    spParameter);

                return Convert.ToInt16(spParameter[9].Value);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
            return result;
        }


        public int TVChartUserConfig_Update(TVChartUserConfig request)
        {
            int result = -1;
            try
            {

                var spParameter = new SqlParameter[11];

                #region Set param
                var parameter = new SqlParameter("@p_id", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.id
                };
                spParameter[0] = parameter;

                parameter = new SqlParameter("@p_name", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.name
                };
                spParameter[1] = parameter;

                parameter = new SqlParameter("@p_content", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.content
                };
                spParameter[2] = parameter;

                parameter = new SqlParameter("@p_symbol", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.symbol
                };
                spParameter[3] = parameter;

                parameter = new SqlParameter("@p_resolution", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.resolution
                };
                spParameter[4] = parameter;


                parameter = new SqlParameter("@p_timestamp", SqlDbType.BigInt)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.timestamp
                };
                spParameter[5] = parameter;

                parameter = new SqlParameter("@p_userid", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.userid
                };
                spParameter[6] = parameter;

                parameter = new SqlParameter("@p_username", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.username
                };
                spParameter[7] = parameter;

                parameter = new SqlParameter("@p_createddate", SqlDbType.DateTime)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.createddate
                };
                spParameter[8] = parameter;

                parameter = new SqlParameter("@p_createdby", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.createdby
                };
                spParameter[9] = parameter;


                parameter = new SqlParameter("@P_Return", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output,
                    Value = -1
                };
                spParameter[10] = parameter;
                #endregion

                SqlHelper.ExecuteNonQuery(ConfigInfo.ConnectString, CommandType.StoredProcedure,
                    "PROC_CHART_CONFIG_UPDATE",
                    spParameter);

                return Convert.ToInt16(spParameter[10].Value);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
            return result;
        }

        public int TVChartUserConfig_Remove(int id, string username)
        {
            int result = -1;
            try
            {

                var spParameter = new SqlParameter[3];

                #region Set param
                var parameter = new SqlParameter("@p_id", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Input,
                    Value = id
                };
                spParameter[0] = parameter;

                parameter = new SqlParameter("@p_name", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = username
                };
                spParameter[1] = parameter;

                parameter = new SqlParameter("@P_Return", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output,
                    Value = -1
                };
                spParameter[2] = parameter;

                #endregion

                SqlHelper.ExecuteNonQuery(ConfigInfo.ConnectString, CommandType.StoredProcedure,
                    "PROC_CHART_CONFIG_DELETE",
                    spParameter);

                return Convert.ToInt16(spParameter[2].Value);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
            }
            return result;
        }
    }
}
