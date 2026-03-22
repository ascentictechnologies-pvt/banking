using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Loan_CRM
{
    public static class DbHelper
    {
        /// <summary>
        /// Inserts, update delete Record without Transaction.
        /// </summary>
        /// <returns>Count of Effected Rows</returns>
        /// <param name="query">Query.</param>
        /// <param name="dbDataParameters">Db data parameters.</param>
        public static int InsertUpdateDelete(string query, List<Parameters> dbDataParameters = null)
        {
            QueryLog.Info(query, (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name);
            NpgsqlConnection npgsqlConnection = new NpgsqlConnection(Convert.ToString(ConfigurationManager.ConnectionStrings["DefaultConnection"]));
            NpgsqlCommand npgsqlCommand = new NpgsqlCommand();
            try
            {
                if (npgsqlConnection.State == ConnectionState.Closed || npgsqlConnection.State == ConnectionState.Broken)
                {
                    npgsqlConnection.Open();
                }
                npgsqlCommand = npgsqlConnection.CreateCommand();
                npgsqlCommand.CommandText = query;
                npgsqlCommand.CommandTimeout = 36000;
                if (dbDataParameters != null)
                    foreach (var item in dbDataParameters)
                    {
                        var parameter = npgsqlCommand.CreateParameter();
                        parameter.ParameterName = item.ParameterName;
                        parameter.Value = item.ParameterValue;
                        parameter.NpgsqlDbType = item.DbType;
                        npgsqlCommand.Parameters.Add(parameter);
                    }
                npgsqlCommand.Prepare();
                return npgsqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"DbHelper ==> {MethodBase.GetCurrentMethod().Name}");
                return 0;
            }
            finally
            {
                npgsqlCommand.Dispose();
                npgsqlConnection.Close();
            }
        }

        /// <summary>
        /// Selects Record Method.
        /// </summary>
        /// <returns>Data Table</returns>
        /// <param name="query">Query.</param>
        /// <param name="dbDataParameters">Db data parameters.</param>
        public static DataTable SelectMethod(string query, List<Parameters> dbDataParameters = null)
        {
            QueryLog.Info(query, (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name);
            NpgsqlConnection npgsqlConnection = new NpgsqlConnection(Convert.ToString(ConfigurationManager.ConnectionStrings["DefaultConnection"]));
            NpgsqlCommand npgsqlCommand = new NpgsqlCommand();
            NpgsqlDataAdapter npgsqlDataAdapter = new NpgsqlDataAdapter();
            try
            {
                if (npgsqlConnection.State == ConnectionState.Closed || npgsqlConnection.State == ConnectionState.Broken)
                {
                    npgsqlConnection.Open();
                }
                DataTable dataTable = new DataTable();
                npgsqlCommand = new NpgsqlCommand();
                npgsqlCommand = npgsqlConnection.CreateCommand();
                npgsqlCommand.CommandText = query;
                npgsqlCommand.CommandTimeout = 36000;
                if (dbDataParameters != null)
                    foreach (var item in dbDataParameters)
                    {
                        var parameter = npgsqlCommand.CreateParameter();
                        parameter.ParameterName = item.ParameterName;
                        parameter.Value = item.ParameterValue;
                        parameter.NpgsqlDbType = item.DbType;
                        npgsqlCommand.Parameters.Add(parameter);
                    }
                npgsqlCommand.Prepare();
                npgsqlDataAdapter = new NpgsqlDataAdapter(npgsqlCommand);
                npgsqlDataAdapter.Fill(dataTable);
                return dataTable;
            }
            catch (Exception ex)
            {
                logger.WriteErrorLogs($"{ex}\n{ex.StackTrace}", $"DbHelper ==> {MethodBase.GetCurrentMethod().Name}");
                return null;
            }
            finally
            {
                if (npgsqlDataAdapter != null)
                    npgsqlDataAdapter.Dispose();
                npgsqlCommand.Dispose();
                npgsqlConnection.Close();
            }
        }
    }
    public class Parameters
    {
        public string ParameterName { get; set; }
        public object ParameterValue { get; set; }
        public NpgsqlDbType DbType { get; set; }
    }
}