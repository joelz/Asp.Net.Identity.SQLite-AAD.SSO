﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AspNet.Identity.SQLite
{
    public class SQLiteDatabase : IDisposable
    {
        private SQLiteConnection _connection;

        /// Default constructor which uses the "DefaultConnection" connectionString
        /// </summary>
        public SQLiteDatabase()
            : this("DefaultConnection")
        {
        }

        /// <summary>
        /// Constructor which takes the connection string name
        /// </summary>
        /// <param name="connectionStringName"></param>
        public SQLiteDatabase(string connectionStringName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            _connection = new SQLiteConnection(connectionString);
        }

        /// <summary>
        /// Executes a non-query SQLite statement
        /// </summary>
        /// <param name="commandText">The SQLite query to execute</param>
        /// <param name="parameters">Optional parameters to pass to the query</param>
        /// <returns>The count of records affected by the SQLite statement</returns>
        public int Execute(string commandText, Dictionary<string, object> parameters)
        {
            int result = 0;

            if (String.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try
            {
                EnsureConnectionOpen();
                var command = CreateCommand(commandText, parameters);
                result = command.ExecuteNonQuery();
            }
            finally
            {
                _connection.Close();
            }

            return result;
        }

        /// <summary>
        /// Executes a SQLite query that returns a single scalar value as the result.
        /// </summary>
        /// <param name="commandText">The SQLite query to execute</param>
        /// <param name="parameters">Optional parameters to pass to the query</param>
        /// <returns></returns>
        public object QueryValue(string commandText, Dictionary<string, object> parameters)
        {
            object result = null;

            if (String.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try
            {
                EnsureConnectionOpen();
                var command = CreateCommand(commandText, parameters);
                result = command.ExecuteScalar();
            }
            finally
            {
                EnsureConnectionClosed();
            }

            return result;
        }

        /// <summary>
        /// Executes a SQL query that returns a list of rows as the result.
        /// </summary>
        /// <param name="commandText">The SQLite query to execute</param>
        /// <param name="parameters">Parameters to pass to the SQLite query</param>
        /// <returns>A list of a Dictionary of Key, values pairs representing the 
        /// ColumnName and corresponding value</returns>
        public List<Dictionary<string, string>> Query(string commandText, Dictionary<string, object> parameters)
        {
            List<Dictionary<string, string>> rows = null;
            if (String.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try
            {
                EnsureConnectionOpen();
                var command = CreateCommand(commandText, parameters);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    rows = new List<Dictionary<string, string>>();
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, string>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var columnName = reader.GetName(i);
                            var columnValue = reader.IsDBNull(i) ? null : reader[i].ToString();
                            row.Add(columnName, columnValue);
                        }
                        rows.Add(row);
                    }
                }
            }
            finally
            {
                EnsureConnectionClosed();
            }

            return rows;
        }

        /// <summary>
        /// Opens a connection if not open
        /// </summary>
        private void EnsureConnectionOpen()
        {
            var retries = 3;
            if (_connection.State == ConnectionState.Open)
            {
                return;
            }
            else
            {
                while (retries >= 0 && _connection.State != ConnectionState.Open)
                {
                    _connection.Open();
                    retries--;
                    Thread.Sleep(30);
                }
            }
        }

        /// <summary>
        /// Closes a connection if open
        /// </summary>
        public void EnsureConnectionClosed()
        {
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        /// <summary>
        /// Creates a SQLiteCommand with the given parameters
        /// </summary>
        /// <param name="commandText">The SQLite query to execute</param>
        /// <param name="parameters">Parameters to pass to the SQLite query</param>
        /// <returns></returns>
        private SQLiteCommand CreateCommand(string commandText, Dictionary<string, object> parameters)
        {
            SQLiteCommand command = _connection.CreateCommand();
            command.CommandText = commandText;
            AddParameters(command, parameters);

            return command;
        }

        /// <summary>
        /// Adds the parameters to a SQLite command
        /// </summary>
        /// <param name="commandText">The SQLite query to execute</param>
        /// <param name="parameters">Parameters to pass to the SQLite query</param>
        private static void AddParameters(SQLiteCommand command, Dictionary<string, object> parameters)
        {
            if (parameters == null)
            {
                return;
            }

            foreach (var param in parameters)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = param.Key;
                parameter.Value = param.Value ?? DBNull.Value;
                command.Parameters.Add(parameter);
            }
        }

        /// <summary>
        /// Helper method to return query a string value 
        /// </summary>
        /// <param name="commandText">The SQLite query to execute</param>
        /// <param name="parameters">Parameters to pass to the SQLite query</param>
        /// <returns>The string value resulting from the query</returns>
        public string GetStrValue(string commandText, Dictionary<string, object> parameters)
        {
            string value = QueryValue(commandText, parameters) as string;
            return value;
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
