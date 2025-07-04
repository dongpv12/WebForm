using System;
using System.Data;
using System.Data.SqlClient;
using WebForm.Common;
using WebForm.Models;

namespace WebForm.DataAccess
{
    public class SymbolDA
    {
        public decimal Create(Symbol_Notify_Info request)
        {
            try
            {
                var spParameter = new SqlParameter[15];

                #region Set param

                var parameter = new SqlParameter("@Symbol", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.Symbol
                };
                spParameter[0] = parameter;

                parameter = new SqlParameter("@Name", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.Name
                };
                spParameter[1] = parameter;

                parameter = new SqlParameter("@Issue", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.Issue
                };
                spParameter[2] = parameter;

                parameter = new SqlParameter("@Description", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.Description
                };
                spParameter[3] = parameter;

                parameter = new SqlParameter("@Status", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.Status
                };
                spParameter[4] = parameter;

                parameter = new SqlParameter("@F_PRICE_Exp", SqlDbType.Decimal)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.F_PRICE_Exp
                };
                spParameter[5] = parameter;

                parameter = new SqlParameter("@T_PRICE_Exp", SqlDbType.Decimal)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.T_PRICE_Exp
                };
                spParameter[6] = parameter;


                parameter = new SqlParameter("@F_PRICE_Target", SqlDbType.Decimal)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.F_PRICE_Target
                };
                spParameter[7] = parameter;

                parameter = new SqlParameter("@T_PRICE_Target", SqlDbType.Decimal)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.T_PRICE_Target
                };
                spParameter[8] = parameter;


                parameter = new SqlParameter("@T_Pause", SqlDbType.Decimal)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.T_Pause
                };
                spParameter[9] = parameter;

                parameter = new SqlParameter("@DoanhThu", SqlDbType.Decimal)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.DoanhThu
                };
                spParameter[10] = parameter;

                parameter = new SqlParameter("@LoiNhuan", SqlDbType.Decimal)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.LoiNhuan
                };

                spParameter[11] = parameter;
                parameter = new SqlParameter("@IsSpecial", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.IsSpecial
                };
                

                spParameter[12] = parameter;


                parameter = new SqlParameter("@Price", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.Price
                };


                spParameter[13] = parameter;

                parameter = new SqlParameter("@P_Return", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output,
                    Value = -1
                };
                spParameter[14] = parameter;

                #endregion

                SqlHelper.ExecuteNonQuery(ConfigInfo.ConnectString, CommandType.StoredProcedure, "PROC_SYM_INSERT",
                    spParameter);

                return Convert.ToDecimal(spParameter[14].Value);
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

                SqlHelper.ExecuteNonQuery(ConfigInfo.ConnectString, CommandType.StoredProcedure, "PROC_SYM_DELETE",
                    spParameter);

                return 1;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return -1;
            }
        }

        public decimal Edit(Symbol_Notify_Info model)
        {
            try
            {
                var spParameter = new SqlParameter[16];

                #region Set param

                var parameter = new SqlParameter("@Id", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Id
                };
                spParameter[0] = parameter;

                parameter = new SqlParameter("@Symbol", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Symbol
                };
                spParameter[1] = parameter;

                parameter = new SqlParameter("@Name", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Name
                };
                spParameter[2] = parameter;

                parameter = new SqlParameter("@Issue", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Issue
                };
                spParameter[3] = parameter;

                parameter = new SqlParameter("@Description", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Description
                };
                spParameter[4] = parameter;

                parameter = new SqlParameter("@Status", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Status
                };
                spParameter[5] = parameter;

                parameter = new SqlParameter("@F_PRICE_Exp", SqlDbType.Decimal)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.F_PRICE_Exp
                };
                spParameter[6] = parameter;

                parameter = new SqlParameter("@T_PRICE_Exp", SqlDbType.Decimal)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.T_PRICE_Exp
                };
                spParameter[7] = parameter;


                parameter = new SqlParameter("@F_PRICE_Target", SqlDbType.Decimal)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.F_PRICE_Target
                };
                spParameter[8] = parameter;

                parameter = new SqlParameter("@T_PRICE_Target", SqlDbType.Decimal)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.T_PRICE_Target
                };
                spParameter[9] = parameter;


                parameter = new SqlParameter("@T_Pause", SqlDbType.Decimal)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.T_Pause
                };
                spParameter[10] = parameter;


                parameter = new SqlParameter("@DoanhThu", SqlDbType.Decimal)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.DoanhThu
                };
                spParameter[11] = parameter;

                parameter = new SqlParameter("@LoiNhuan", SqlDbType.Decimal)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.LoiNhuan
                };
                spParameter[12] = parameter;


                parameter = new SqlParameter("@IsSpecial", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.IsSpecial
                };

                spParameter[13] = parameter;
                parameter = new SqlParameter("@Price", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Price
                };


                spParameter[14] = parameter;

                parameter = new SqlParameter("@P_Return", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output,
                    Value = -1
                };
                spParameter[15] = parameter;

                #endregion

                SqlHelper.ExecuteNonQuery(ConfigInfo.ConnectString, CommandType.StoredProcedure, "PROC_SYM_UPDATE",
                    spParameter);

                return Convert.ToDecimal(spParameter[15].Value);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return -1;
            }
        }

        public DataSet Search(SearchSymbolRequest model, ref int pTotal)
        {
            try
            {
                var spParameter = new SqlParameter[4];

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



                parameter = new SqlParameter("@P_CODE", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Code
                };
                spParameter[2] = parameter;



                parameter = new SqlParameter("@P_TOTAL", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output,
                    Value = -1
                };
                spParameter[3] = parameter;

                #endregion

                var ds = SqlHelper.ExecuteDataset(ConfigInfo.ConnectString, CommandType.StoredProcedure,
                    "PROC_SYMBOL_SEARCH", spParameter);

                pTotal = Convert.ToInt32(spParameter[3].Value);

                return ds;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return new DataSet();
            }
        }

        public DataSet SymbolGetAll()
        {
            try
            {
                var ds = SqlHelper.ExecuteDataset(ConfigInfo.ConnectString, CommandType.StoredProcedure,
                    "PROC_SYM_GETALL");

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