﻿using Npgsql;
using QueryDB.Resources;
using System;
using System.Collections.Generic;
using System.Data;

namespace QueryDB.PostgreSQL
{
    /// <summary>
    /// 'PostgreSQL' adapter.
    /// </summary>
    internal class Adapter
    {
        /// <summary>
        /// Gets the 'PostgreSQL' data reader.
        /// </summary>
        /// <param name="cmdText">The text of the query.</param>
        /// <param name="connection">'PostgreSQL' connection.</param>
        /// <param name="commandType">Sql command type.</param>
        /// <returns>'PostgreSQL' data reader.</returns>
        internal NpgsqlDataReader GetPostgreSqlReader(string cmdText, NpgsqlConnection connection, CommandType commandType)
        {
            connection.Open();
            using (var sqlCommand = new NpgsqlCommand(cmdText, connection) { CommandType = commandType })
            {
                return sqlCommand.ExecuteReader();
            }
        }

        /// <summary>
        /// Retrieves records for 'Select' queries from the database.
        /// Converts column names to keys holding values, with multiple database rows returned into a list.
        /// </summary>
        /// <param name="selectSql">'Select' query.</param>
        /// <param name="connection">'PostgreSQL' Connection.</param>
        /// <param name="upperCaseKeys">Boolean parameter to return dictionary keys in uppercase.</param>
        /// <returns>List of data Dictionary with column names as keys holding values into a list for multiple rows of data.
        /// Note: Byte[] is returned as Base64 string.</returns>
        internal List<DataDictionary> FetchData(string selectSql, NpgsqlConnection connection, bool upperCaseKeys)
        {
            var dataList = new List<DataDictionary>();
            using (var reader = GetPostgreSqlReader(selectSql, connection, CommandType.Text))
            {
                while (reader.Read())
                {
                    var addRow = new DataDictionary();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string key = upperCaseKeys ? reader.GetName(i).ToUpper() : reader.GetName(i);
                        if (reader.GetValue(i) is byte[] value)
                            addRow.ReferenceData.Add(key, Convert.ToBase64String(value));
                        else
                            addRow.ReferenceData.Add(key, reader.GetValue(i).ToString());
                    }
                    dataList.Add(addRow);
                }
            }
            return dataList;
        }

        /// <summary>
        ///  Retrieves records for 'Select' queries from the database.
        /// </summary>
        /// <typeparam name="T">Object entity to return data mapped into.</typeparam>
        /// <param name="selectSql">'Select' query.</param>
        /// <param name="connection">'PostgreSQL' Connection.</param>
        /// <param name="strict">Enables fetch data only for object <T> properties existing in database query result.</param>
        /// <returns>List of data rows mapped into object entity into a list for multiple rows of data.</returns>
        internal List<T> FetchData<T>(string selectSql, NpgsqlConnection connection, bool strict) where T : new()
        {
            var dataList = new List<T>();
            using (var reader = GetPostgreSqlReader(selectSql, connection, CommandType.Text))
            {
                while (reader.Read())
                {
                    var addObjectRow = new T();
                    foreach (var prop in typeof(T).GetProperties())
                    {
                        if ((strict || Utils.ColumnExists(reader, prop.Name)) && !reader.IsDBNull(reader.GetOrdinal(prop.Name)))
                            prop.SetValue(addObjectRow, reader[prop.Name]);
                    }
                    dataList.Add(addObjectRow);
                }
            }
            return dataList;
        }
    }
}
