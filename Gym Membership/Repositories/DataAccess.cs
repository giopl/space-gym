using Gym_Membership.Helpers;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace Gym_Membership.Repositories
{

    public class DataAccess
    {
        ILog log = log4net.LogManager.GetLogger(typeof(DataAccess));
        bool DoLogQueries = true;//false;//CIMS.Properties.Settings.Default.logQueries;



        private static string ConnectionStringLocal = ConfigurationHelper.GetEnvironment() == "PRODUCTION" ? 
            System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString 
            : ConfigurationHelper.GetEnvironment() == "BIZ" ? System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringBiz"].ConnectionString :
            System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        

        public SqlConnection SQlConn = new SqlConnection();

        
        /*Connect to Database*/
        public void DBConnect()
        {
            try
            {
                #if DEBUG
                                SQlConn = new SqlConnection(ConnectionStringLocal);
#elif (Release)
                                SQlConn = new SqlConnection(ConnectionStringRelease);

#elif (Local)
                                 SQlConn = new SqlConnection(ConnectionStringLocal);
#endif
                SQlConn.Open();
            }
            catch (Exception e)
            {
                try
                {
                    SQlConn.Close();
                    SQlConn.Dispose();
                    log.Error("DataAccess:DBConnect(): " + e.ToString());
                    throw e;
                }
                catch (Exception ee)
                {
                    log.Error("DataAccess:DBConnect(): " + e.ToString());
                    throw ee;
                }
            }
        }


        /*Disconnect from Database*/
        public void DBDisconnect()
        {
            try
            {

                SQlConn.Close();
                SQlConn.Dispose();
            }
            catch (Exception e)
            {
                log.Error("DataAccess:DBDisconnect(): " + e.ToString());
                throw e;
            }
        }


        /*CUD operations only */
        public int CreateUpdateDelete(String Command)
        {
            log.Info("DataAccess:CreateUpdateDelete(String)");
            /* if (UserSession.Current.ReadOnlyMode)
             {
                 log.Warn("[ReadOnlyMode] - Cannot Write to DB!");
                 return 0;
             }*/

            int RowsAffected = 0;
            using (SqlTransaction Transaction = SQlConn.BeginTransaction())
            {
                try
                {
                    SqlCommand Odc = new SqlCommand(Command, SQlConn, Transaction);
                    RowsAffected = Odc.ExecuteNonQuery();
                    Transaction.Commit();
                }
                catch (Exception e)
                {
                    try
                    {
                        Transaction.Rollback(); //if transaction is active rollback
                        log.Error("DataAccess:CreateUpdateDelete(): " + e.ToString());
                        throw e;

                    }
                    catch (Exception Ex)
                    {
                        throw Ex;
                        //if transaction is inactive, do nothing

                    }
                }

            }
            return RowsAffected;
        }


        /* Overload for CUD function which accepts query parameters*/
        /* Calls Stored Procedure Only.*/
        public int CreateUpdateDelete(String Command, List<SqlParameter> parameters)
        {
            log.Info("DataAccess:CreateUpdateDelete(String)");
            //SqlCommand c = new SqlCommand();
            //c.Parameters.Add()

            if (UserSession.Current.ReadOnlyMode)
            {
                log.Warn("[ReadOnlyMode] - Cannot Write to DB!");
                return 0;
            }

            int RowsAffected = 0;


            //using (SqlTransaction Transaction = SQlConn.BeginTransaction())
            // {
            try
            {
                SqlCommand Odc = new SqlCommand(Command, SQlConn);

                Odc.CommandType = CommandType.Text;

                foreach (var param in parameters)
                {

                    Odc.Parameters.Add(param.ParameterName, param.SqlDbType).Value = param.Value;
                }

                RowsAffected = Odc.ExecuteNonQuery();
                //Transaction.Commit();
            }
            catch (Exception e)
            {
                try
                {
                    //Transaction.Rollback(); //if transaction is active rollback
                    log.Error("DataAccess:CreateUpdateDelete(): " + e.ToString());
                    throw e;

                }
                catch (Exception Ex)
                {
                    throw Ex;
                    //if transaction is inactive, do nothing

                }
            }

            //}
            return RowsAffected;
        }


        /*Return a resultset from a select statement*/
        public SqlDataReader ReadQuery(String Command)
        {
            log.Info("DataAccess:ReadQuery");
            SqlDataReader Odr;
            using (SqlCommand Odc = new SqlCommand(Command, SQlConn))
            {
                try
                {

                    log.Debug("ReadQuery:QueryString:" + Command);
                    Odr = Odc.ExecuteReader();
                    return Odr;
                }
                catch (SqlException SqlEx)
                {
                    try
                    {
                        log.Error("[CountQuery] returned: " + SqlEx.ToString());
                        throw SqlEx;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }

            }

        }


        /*Return a resultset from a select statement with parameters,*/
        public SqlDataReader ReadQuery(String Command, List<SqlParameter> parameters)
        {
            log.Info("DataAccess:ReadQuery[parameter overload]");
            SqlDataReader Odr;
            using (SqlCommand Odc = new SqlCommand(Command, SQlConn))
            {
                try
                {
                    log.Debug("ReadQuery:QueryString:" + Command);
                    Odc.CommandType = CommandType.Text;

                    foreach (var param in parameters)
                    {
                        Odc.Parameters.Add(param);
                    }

                    Odr = Odc.ExecuteReader();
                    return Odr;
                }
                catch (SqlException SqlEx)
                {
                    try
                    {
                        log.Error("[ReadQuery] returned: " + SqlEx.ToString());
                        throw SqlEx;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }

            }

        }


        /*Return a resultset from a select statement with parameters,*/
        public SqlDataReader ReadQueryStoredProcedure(String Command, List<SqlParameter> parameters)
        {
            log.Info("DataAccess:ReadQuery[parameter overload]");
            SqlDataReader Odr;
            using (SqlCommand Odc = new SqlCommand(Command, SQlConn))
            {
                try
                {
                    log.Debug("ReadQuery:QueryString:" + Command);
                    Odc.CommandType = CommandType.StoredProcedure;

                    foreach (var param in parameters)
                    {
                        Odc.Parameters.Add(param);
                    }

                    Odr = Odc.ExecuteReader();
                    return Odr;
                }
                catch (SqlException SqlEx)
                {
                    try
                    {
                        log.Error("[ReadQuery] returned: " + SqlEx.ToString());
                        throw SqlEx;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }

            }

        }

        public SqlParameter CreateParameter(string name, SqlDbType datatype, object Value, int size = 0)
        {
            try
            {
                SqlParameter param;
                if (size == 0)
                {
                    param = new SqlParameter(name, datatype);
                }
                else
                {
                    param = new SqlParameter(name, datatype, size);
                }

                param.Value = Value == null ? DBNull.Value : Value;

                return param;
            }
            catch (Exception e)
            {
                log.Error("[DataAccces:CreateParameter] " + e.ToString());
                throw e;
            }
        }



        /*This set of functions read data from the Data Reader at the required position */
        public string ReadString(ref SqlDataReader reader, int position)
        {
            try
            {
                return reader.GetValue(position).Equals(DBNull.Value) ? String.Empty : reader.GetString(position);
            }
            catch (Exception e)
            {
                log.Error("DataAccess:ReadString : " + e.ToString());
                throw e;
            }
        }

        public string ReadString(ref SqlDataReader reader, ref int position)
        {
            try
            {
                return reader.GetValue(position).Equals(DBNull.Value) ? String.Empty : reader.GetString(position);
            }
            catch (Exception e)
            {
                log.Error("DataAccess:ReadString : " + e.ToString());
                throw e;
            }
            finally
            {
                ++position;
            }
        }



        public int ReadInt(ref SqlDataReader reader, int position)
        {
            try
            {
                return reader.GetValue(position).Equals(DBNull.Value) ? 0 : reader.GetInt32(position);
            }
            catch (Exception e)
            {
                log.Error("DataAccess:ReadInt : " + e.ToString());
                throw e;
            }
        }

        public int ReadInt(ref SqlDataReader reader, ref int position)
        {
            try
            {
                return reader.GetValue(position).Equals(DBNull.Value) ? 0 : reader.GetInt32(position);
            }
            catch (Exception e)
            {
                log.Error("DataAccess:ReadInt : " + e.ToString());
                throw e;
            }
            finally
            {
                ++position;
            }
        }



        public int ReadBigInt(ref SqlDataReader reader, ref int position)
        {
            try
            {
                return reader.GetValue(position).Equals(DBNull.Value) ? 0 : Convert.ToInt32(reader.GetInt64(position));
            }
            catch (Exception e)
            {
                log.Error("DataAccess:ReadInt : " + e.ToString());
                throw e;
            }
            finally
            {
                ++position;
            }
        }



        public double ReadDouble(ref SqlDataReader reader, int position)
        {
            try
            {
                return reader.GetValue(position).Equals(DBNull.Value) ? Convert.ToDouble(0) : reader.GetDouble(position);
            }
            catch (Exception e)
            {
                log.Error("DataAccess:ReadDouble : " + e.ToString());
                throw e;
            }
        }

        public double ReadDouble(ref SqlDataReader reader, ref int position)
        {
            try
            {
                return reader.GetValue(position).Equals(DBNull.Value) ? Convert.ToDouble(0) : reader.GetDouble(position);
            }
            catch (Exception e)
            {
                log.Error("DataAccess:ReadDouble : " + e.ToString());
                throw e;
            }
            finally
            {
                ++position;
            }
        }

        public Decimal ReadDecimal(ref SqlDataReader reader, int position)
        {
            try
            {
                return reader.GetValue(position).Equals(DBNull.Value) ? Convert.ToDecimal(0) : reader.GetDecimal(position);
            }
            catch (Exception e)
            {
                log.Error("DataAccess:ReadDecimal : " + e.ToString());
                throw e;
            }
        }

        public Decimal ReadDecimal(ref SqlDataReader reader, ref int position)
        {
            try
            {
                return reader.GetValue(position).Equals(DBNull.Value) ? Convert.ToDecimal(0) : reader.GetDecimal(position);
            }
            catch (Exception e)
            {
                log.Error("DataAccess:ReadDecimal : " + e.ToString());
                throw e;
            }
            finally
            {
                ++position;
            }
        }




        public DateTime ReadDate(ref SqlDataReader reader, int position)
        {
            try
            {
                return reader.GetValue(position).Equals(DBNull.Value) ? DateTime.MinValue : reader.GetDateTime(position);
            }
            catch (Exception e)
            {
                log.Error("DataAccess:ReadDate : " + e.ToString());
                throw e;
            }
        }



        public DateTime ReadDate(ref SqlDataReader reader, ref int position)
        {
            try
            {
                return reader.GetValue(position).Equals(DBNull.Value) ? DateTime.MinValue : reader.GetDateTime(position);
            }
            catch (Exception e)
            {
                log.Error("DataAccess:ReadDate : " + e.ToString());
                throw e;
            }
            finally
            {
                ++position;
            }
        }

        public int ReadEnum<T>(ref SqlDataReader reader, ref int position)
        {
            try
            {
                return reader.GetValue(position).Equals(DBNull.Value) ? 0 : (int)Enum.Parse(typeof(T), reader.GetString(position));
            }
            catch (Exception e)
            {
                log.Error("DataAccess:ReadDate : " + e.ToString());
                throw e;
            }
            finally
            {
                ++position;
            }

        }

        /// <summary>
        /// Return any type of data 
        /// TO USE IN .NET 4+ only
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public dynamic ReadDynamic(ref SqlDataReader reader, ref int position)
        {
            try
            {
                dynamic value;

                var readdata = reader.GetValue(position);
                switch (Type.GetTypeCode(readdata.GetType()))
                {
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Single:
                        if (readdata.Equals(DBNull.Value))
                        {
                            value = 0;
                        }
                        else
                        {
                            value = readdata;
                        }
                        break;
                    case TypeCode.String:
                        if (readdata.Equals(DBNull.Value))
                        {
                            value = String.Empty;
                        }
                        else
                        {
                            value = readdata;
                        }
                        break;
                    case TypeCode.DateTime:
                        if (readdata.Equals(DBNull.Value))
                        {
                            value = DateTime.MinValue;
                        }
                        else
                        {
                            value = readdata;
                        }
                        break;
                    default:
                        return DBNull.Value;

                }
                return value;
            }
            catch (Exception e)
            {
                log.Error("DataAccess:ReadDynamic : " + e.ToString());
                throw e;
            }
        }

        protected int InsertWithReturnId(String Command, List<SqlParameter> parameters)
        {

            log.Info("DataAccess:InsertWithReturnId(String)");
            if (UserSession.Current.ReadOnlyMode)
            {
                log.Warn("[ReadOnlyMode] - Cannot Write to DB!");
                return 0;
            }

            int LastId = 0;
            using (SqlTransaction Transaction = SQlConn.BeginTransaction())
            {
                try
                {
                    /*Insert the required data*/
                    SqlCommand Odc = new SqlCommand(Command, SQlConn, Transaction);
                    foreach (var param in parameters)
                    {
                        Odc.Parameters.Add(param);
                    }


                    //RowsAffected =  Odc.ExecuteNonQuery();


                    //string getIdQuery = " SELECT SCOPE_IDENTITY() ";
                    //Odc = new OdbcCommand(getIdQuery, Oc,Transaction);

                    var oVal = Odc.ExecuteScalar();

                    LastId = oVal.Equals(DBNull.Value) ? 0 : Convert.ToInt32(oVal);

                    Transaction.Commit();

                }
                catch (Exception e)
                {
                    try
                    {
                        Transaction.Rollback(); //if transaction is active rollback
                        log.Error("DataAccess:InsertWithReturnId(): " + e.ToString());
                        throw e;
                    }
                    catch (Exception Ex)
                    {
                        throw Ex;
                        //if transaction is inactive, do nothing

                    }
                }

            }
            return LastId;
        }


        protected void logQuery(string query, List<SqlParameter> parameters = null, [System.Runtime.CompilerServices.CallerMemberName] string method = "")
        {
            if (ConfigurationHelper.LogQueries())
            {


                StringBuilder parms = new StringBuilder();
                if (parameters != null && parameters.Count > 0)
                {
                    foreach (var p in parameters)
                    {
                        parms.Append(p.Value);
                        parms.Append("|");
                    }
                }

                parms.Length--;

                log.InfoFormat("method:[{0}] query: {1} params:{2}", method, query, parms);
            }
        }


        /*End of reader set*/
    }

}