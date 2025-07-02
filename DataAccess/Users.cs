using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using WebForm.Common;
using WebForm.Models;

namespace WebForm.DataAccess
{
    public class UsersDA
    {
        public bool CheckLogin(User model, ref DataSet ds)
        {
            try
            {
                var spParameter = new SqlParameter[3];
                var count = -1;
                #region Set param

                var parameter = new SqlParameter("@P_USERNAME", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.UserName
                };
                spParameter[0] = parameter;

                parameter = new SqlParameter("@P_PASSWORD", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Password
                };
                spParameter[1] = parameter;

                parameter = new SqlParameter("@P_RESULT", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output,
                    Value = -1
                };
                spParameter[2] = parameter;

                #endregion

                  ds = SqlHelper.ExecuteDataset(ConfigInfo.ConnectString, CommandType.StoredProcedure,
                    "PROC_CHECK_LOGIN", spParameter);

                count = Convert.ToInt32(spParameter[2].Value);
                return count > 0;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return false;
            }
        }


        public DataSet GetAllcode()
        {
            try
            {
                var ds = SqlHelper.ExecuteDataset(ConfigInfo.ConnectString, CommandType.StoredProcedure,
                    "PROC_NEWS_ALLCODE");

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