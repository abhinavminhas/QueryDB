﻿using Oracle.ManagedDataAccess.Client;
using QueryDB.Resources;
using System.Collections.Generic;
using System.Data;

namespace QueryDB.Oracle
{
    /// <summary>
    /// 'Oracle' adapter.
    /// </summary>
    internal class Adapter
    {
        /// <summary>
        /// Gets the 'Oracle' data reader.
        /// </summary>
        /// <param name="cmdText">The text of the query.</param>
        /// <param name="connection">'Oracle' connection.</param>
        /// <param name="commandType">Sql command type.</param>
        /// <returns>'Oracle' data reader.</returns>
        internal OracleDataReader GetOracleReader(string cmdText, OracleConnection connection, CommandType commandType)
        {
            connection.Open();
            using (var sqlCommand = new OracleCommand(cmdText, connection) { CommandType = commandType })
            {
                return sqlCommand.ExecuteReader();
            }
        }

        /// <summary>
        /// Retrieves records for 'Select' queries from the database.
        /// Converts column names to keys holding values, with multiple database rows returned into a list.
        /// </summary>
        /// <param name="selectSql">'Select' query.</param>
        /// <param name="connection">Oracle Connection.</param>
        /// <param name="upperCaseKeys">Boolean parameter to return dictionary keys in uppercase.</param>
        /// <returns>List of data Dictionary with column names as keys holding values into a list for multiple rows of data.</returns>
        internal List<DataDictionary> FetchData(string selectSql, OracleConnection connection, bool upperCaseKeys)
        {
            var dataList = new List<DataDictionary>();
            using (var reader = GetOracleReader(selectSql, connection, CommandType.Text))
            {
                while (reader.Read())
                {
                    var addedRow = new DataDictionary();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (upperCaseKeys)
                            addedRow.ReferenceData.Add(reader.GetName(i).ToUpper(), reader.GetValue(i).ToString());
                        else
                            addedRow.ReferenceData.Add(reader.GetName(i), reader.GetValue(i).ToString());
                    }
                    dataList.Add(addedRow);
                }
            }
            return dataList;
        }
    }
}