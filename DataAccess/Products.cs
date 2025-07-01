using System;
using System.Data;
using System.Data.SqlClient;
using WebForm.Common;
using WebForm.Models;

namespace WebForm.DataAccess
{
    public class ProductDA
    {

        #region Group
        public decimal CreateGroup(GroupProductsRequest request)
        {
            try
            {
                var spParameter = new SqlParameter[3];

                #region Set param

                var parameter = new SqlParameter("@P_Name", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.GroupName
                };
                spParameter[0] = parameter;

                parameter = new SqlParameter("@P_Description", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.Description
                };
                spParameter[1] = parameter;

                parameter = new SqlParameter("@P_Return", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output,
                    Value = -1
                };
                spParameter[2] = parameter;

                #endregion

                SqlHelper.ExecuteNonQuery(ConfigInfo.ConnectString, CommandType.StoredProcedure, "PROC_GROUP_PRO_INSERT",
                    spParameter);

                return Convert.ToDecimal(spParameter[2].Value);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return -1;
            }
        }

        public decimal DeleteGroup(int id)
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

                SqlHelper.ExecuteNonQuery(ConfigInfo.ConnectString, CommandType.StoredProcedure, "PROC_GROUP_PRO_DELETE",
                    spParameter);

                return 1;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return -1;
            }
        }

        public decimal EditGroup(GroupProducts model)
        {
            try
            {
                var spParameter = new SqlParameter[4];

                #region Set param

                var parameter = new SqlParameter("@P_GroupId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.GroupId
                };
                spParameter[0] = parameter;

                parameter = new SqlParameter("@P_GroupName", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.GroupName
                };
                spParameter[1] = parameter;

                parameter = new SqlParameter("@P_Description", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Description
                };
                spParameter[2] = parameter;
 
                parameter = new SqlParameter("@P_Return", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output,
                    Value = -1
                };
                spParameter[3] = parameter;

                #endregion

                SqlHelper.ExecuteNonQuery(ConfigInfo.ConnectString, CommandType.StoredProcedure, "PROC_GROUP_PRO_EDIT",
                    spParameter);

                return Convert.ToDecimal(spParameter[3].Value);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return -1;
            }
        }

        public DataSet SearchGroup(SearchGroupProductRequest model, ref int pTotal)
        {
            try
            {
                var spParameter = new SqlParameter[5];

                #region Set param

                var parameter = new SqlParameter("@P_ORDER_BY", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.OrderBy
                };
                spParameter[0] = parameter;

                parameter = new SqlParameter("@P_ORDER_TYPE", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.OrderByType
                };
                spParameter[1] = parameter;

                parameter = new SqlParameter("@P_START", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Start
                };
                spParameter[2] = parameter;

                parameter = new SqlParameter("@P_END", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.End
                };
                spParameter[3] = parameter;

                parameter = new SqlParameter("@P_TOTAL", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output,
                    Value = -1
                };
                spParameter[4] = parameter;

                #endregion

                var ds = SqlHelper.ExecuteDataset(ConfigInfo.ConnectString, CommandType.StoredProcedure,
                    "PROC_GROUP_PRO_SEARCH", spParameter);

                pTotal = Convert.ToInt32(spParameter[4].Value);

                return ds;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return new DataSet();
            }
        }

        public DataSet GetGroupById(int id)
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
                    "PROC_GROUP_PRO_GET_BY_ID", spParameter);

                return ds;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return new DataSet();
            }
        }

        public DataSet GetAllGroup()
        {
            try
            {
                var ds = SqlHelper.ExecuteDataset(ConfigInfo.ConnectString, CommandType.StoredProcedure,
                    "PROC_GET_ALL_GROUP");

                return ds;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return new DataSet();
            }
        }
        #endregion


        #region Product
        public decimal CreateProduct(ProductsRequest request)
        {
            try
            {
                var spParameter = new SqlParameter[13];

                #region Set param

                var parameter = new SqlParameter("@P_FullName", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.FullName
                };
                spParameter[0] = parameter;

                parameter = new SqlParameter("@P_ShortName", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.ShortName
                };
                spParameter[1] = parameter;

                parameter = new SqlParameter("@P_GroupId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.GroupId
                };
                spParameter[2] = parameter;

                parameter = new SqlParameter("@P_Description", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.Description
                };
                spParameter[3] = parameter;
            
                parameter = new SqlParameter("@P_Detail", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.Detail
                };
                spParameter[4] = parameter;

                parameter = new SqlParameter("@P_Avatar", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.Avatar
                };
                spParameter[5] = parameter;

                parameter = new SqlParameter("@P_CoatingThickness", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.CoatingThickness
                };
                spParameter[6] = parameter;

                parameter = new SqlParameter("@P_Features1", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.Features1
                };
                spParameter[7] = parameter;

                parameter = new SqlParameter("@P_Features2", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.Features2
                };
                spParameter[8] = parameter;


                parameter = new SqlParameter("@P_Features3", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.Features3
                };
                spParameter[9] = parameter;

                parameter = new SqlParameter("@P_Features4", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.Features4
                };
                spParameter[10] = parameter;

                parameter = new SqlParameter("@P_Price", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.Price
                };
                spParameter[11] = parameter;


                parameter = new SqlParameter("@P_Return", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output,
                    Value = -1
                };
                spParameter[12] = parameter;

                #endregion

                SqlHelper.ExecuteNonQuery(ConfigInfo.ConnectString, CommandType.StoredProcedure, "PROC_PRODUCT_INSERT",
                    spParameter);

                return Convert.ToDecimal(spParameter[12].Value);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return -1;
            }
        }

        public decimal DeleteProduct(int id)
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

                SqlHelper.ExecuteNonQuery(ConfigInfo.ConnectString, CommandType.StoredProcedure, "PROC_PRODUCT_DELETE",
                    spParameter);

                return 1;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return -1;
            }
        }

        public decimal EditProduct(Products model)
        {
            try
            {
                var spParameter = new SqlParameter[14];

                #region Set param

                var parameter = new SqlParameter("@P_Id", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Id
                };
                spParameter[0] = parameter;

                parameter = new SqlParameter("@P_FullName", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.FullName
                };
                spParameter[1] = parameter;

                parameter = new SqlParameter("@P_ShortName", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.ShortName
                };
                spParameter[2] = parameter;


                parameter = new SqlParameter("@P_GroupId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.GroupId
                };
                spParameter[3] = parameter;

                parameter = new SqlParameter("@P_Description", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Description
                };
                spParameter[4] = parameter;

                parameter = new SqlParameter("@P_Detail", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Detail
                };
                spParameter[5] = parameter;

                parameter = new SqlParameter("@P_Avatar", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Avatar
                };
                spParameter[6] = parameter;

                parameter = new SqlParameter("@P_CoatingThickness", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.CoatingThickness
                };
                spParameter[7] = parameter;


                parameter = new SqlParameter("@P_Features1", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Features1
                };
                spParameter[8] = parameter;

                parameter = new SqlParameter("@P_Features2", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Features2
                };
                spParameter[9] = parameter;


                parameter = new SqlParameter("@P_Features3", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Features3
                };
                spParameter[10] = parameter;

                parameter = new SqlParameter("@P_Features4", SqlDbType.NVarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Features4
                };
                spParameter[11] = parameter;

                parameter = new SqlParameter("@P_Price", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Price
                };
                spParameter[12] = parameter;

                parameter = new SqlParameter("@P_Return", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output,
                    Value = -1
                };
                spParameter[13] = parameter;

                #endregion

                SqlHelper.ExecuteNonQuery(ConfigInfo.ConnectString, CommandType.StoredProcedure, "PROC_PRODUCT_EDIT",
                    spParameter);

                return Convert.ToDecimal(spParameter[13].Value);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return -1;
            }
        }

        public DataSet SearchProduct(SearchProductRequest model, ref int pTotal)
        {
            try
            {
                var spParameter = new SqlParameter[5];

                #region Set param

                var parameter = new SqlParameter("@P_ORDER_BY", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.OrderBy
                };
                spParameter[0] = parameter;

                parameter = new SqlParameter("@P_ORDER_TYPE", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.OrderByType
                };
                spParameter[1] = parameter;

                parameter = new SqlParameter("@P_START", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Start
                };
                spParameter[2] = parameter;

                parameter = new SqlParameter("@P_END", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.End
                };
                spParameter[3] = parameter;

                parameter = new SqlParameter("@P_TOTAL", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output,
                    Value = -1
                };
                spParameter[4] = parameter;

                #endregion

                var ds = SqlHelper.ExecuteDataset(ConfigInfo.ConnectString, CommandType.StoredProcedure,
                    "PROC_PRODUCT_SEARCH", spParameter);

                pTotal = Convert.ToInt32(spParameter[4].Value);

                return ds;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return new DataSet();
            }
        }

        public DataSet GetProductById(int id)
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
                    "PROC_PRODUCT_GET_BY_ID", spParameter);

                return ds;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return new DataSet();
            }
        }


        public DataSet GetProductPortal(SearchProductPortalRequest model, ref int pTotal)
        {
            try
            {
                var spParameter = new SqlParameter[6];

                #region Set param

                var parameter = new SqlParameter("@P_GROUP_ID", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.GroupId
                };
                spParameter[0] = parameter;

                  parameter = new SqlParameter("@P_ORDER_BY", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.OrderBy
                };
                spParameter[1] = parameter;

                parameter = new SqlParameter("@P_ORDER_TYPE", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.OrderByType
                };
                spParameter[2] = parameter;

                parameter = new SqlParameter("@P_START", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.Start
                };
                spParameter[3] = parameter;

                parameter = new SqlParameter("@P_END", SqlDbType.VarChar)
                {
                    Direction = ParameterDirection.Input,
                    Value = model.End
                };
                spParameter[4] = parameter;

                parameter = new SqlParameter("@P_TOTAL", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output,
                    Value = -1
                };
                spParameter[5] = parameter;

                #endregion

                var ds = SqlHelper.ExecuteDataset(ConfigInfo.ConnectString, CommandType.StoredProcedure,
                    "GET_PRODUCT_PORTAL", spParameter);

                pTotal = Convert.ToInt32(spParameter[5].Value);

                return ds;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                return new DataSet();
            }
        }
        #endregion

    }
}