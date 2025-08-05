using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Npgsql;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;

namespace R9.DataBase
{
    public class PostgresDataContext : DataContext
    {
        public PostgresDataContext(string Connection) : base(Connection)
        {
            ParameterSeparator = ":";
        }

        public override DataSet ExecuteReturningProcedure(string query, IEnumerable<IDataParameter> parameters = null, int procedureTimeout = 120)
        {
            var command = SqlCommandFactory(query, ConnectionString, parameters);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = procedureTimeout;

            DataSet ds = new DataSet();

            NpgsqlDataAdapter da = new NpgsqlDataAdapter((NpgsqlCommand)command);

            da.Fill(ds);

            return ds;
        }
        
        public override void ExecuteNonQuery(string nonQuery, IEnumerable<IDataParameter> parameters = null)
        {
            using (var command = SqlCommandFactory(nonQuery, ConnectionString, parameters, Timeout))
            {
                command.Connection.Open();
                command.ExecuteNonQuery();
                command.Connection.Close();
            }
        }

        protected override DbCommand SqlCommandFactory(string query, string connectionString, IEnumerable<IDataParameter> parameters = null, int? Timeout = default(int?))
        {
            DbConnection connection = new NpgsqlConnection(connectionString);
            DbCommand command = connection.CreateCommand();
            command.CommandTimeout = 9000000;

            if (!query.Contains(" "))
            {
                command.CommandType = CommandType.StoredProcedure;
            }

            if (Timeout.HasValue)
            {
                command.CommandTimeout = Timeout.Value;
            }

            command.CommandText = query;

            if (parameters != null)
            {
                foreach (var parametro in parameters)
                {
                    command.Parameters.Add(parametro);
                }
            }
            return command;
        }

        public override IDataParameter GetDataParameter(string name, object value)
        {
            return new NpgsqlParameter(name, value.ToDB());
        }

        protected override DataSet SqlDataSetFactory(string query, string connection, IEnumerable<IDataParameter> parameters)
        {
            DbCommand command = SqlCommandFactory(query, connection, parameters);

            DataSet ds = new DataSet();

            NpgsqlDataAdapter da = new NpgsqlDataAdapter((NpgsqlCommand)command);

            da.Fill(ds);

            return ds;
        }
    }
}
