using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace R9.DataBase
{

    /// <summary>
    /// Classe para gerenciamento de múltiplas conexões de banco;
    /// ~T
    /// </summary>
    public class DataContext
    {
        /// <summary>
        /// Connection String atual deste contexto; Representa a conexão de banco a ser utilizada.
        /// </summary>
        public string ConnectionString { get; set; }

        public int? Timeout { get; set; }

        public string ParameterSeparator { get; set; } = "@";

        /// <summary>
        /// Inicializa um novo DataContext com a string de conexão passada como parâmetro.
        /// </summary>
        /// <param name="Connection">A string de conexão a utilizar para este objeto</param>
        public DataContext(string Connection)
        {
            this.ConnectionString = Connection;
        }

        public object ExecuteQuery(string v, object parametros)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executa a consulta SQL passada como parâmetro usando a string de conexão deste objeto.
        /// Parâmetros podem ser passados para a consulta, opcionalmente.
        /// </summary>
        /// <param name="query">A consulta a ser executada</param>
        /// <param name="parametros">Parâmetros da consulta. Opcional.</param>
        /// <returns>Um dataset contendo os dados obtidos do banco de dados, 
        /// ou um dataset vazio, caso não haja resultados.</returns>
        public DataSet ExecuteQuery(string query, IEnumerable<IDataParameter> parametros = null)
        {
            DataSet ds = SqlDataSetFactory(query, ConnectionString, parametros);

            return ds;
        }

        public DataSet ExecutarQuery(string query, IEnumerable<IDataParameter> parametros = null)
        {
            return ExecuteQuery(query, parametros);
        }

        /// <summary>
        /// Executa uma consulta SQL e retorna o primeiro valor da primeira linha como um objeto do tipo T.
        /// </summary>
        /// <typeparam name="T">O tipo do dado a ser retornado</typeparam>
        /// <param name="query">A consulta a ser executada. Somente o primeiro valor da primeira linha será considerado</param>
        /// <param name="parametros">Lista de parâmetros a ser incluidos na consulta. Opcional</param>
        /// <returns>O primeiro valor, da primeira linha, convertido para o tipo T, se possível.</returns>
        public T ExecuteScalar<T>(string query, IEnumerable<IDataParameter> parametros = null)
        {
            var comando = SqlCommandFactory(query, ConnectionString, parametros);

            comando.Connection.Open();

            object resultado = comando.ExecuteScalar();

            comando.Connection.Close();

            T retorno = (T)(resultado != null &&  resultado != DBNull.Value ? resultado : default(T));
            return retorno;
        }

        public T ExecutaScalar<T>(string query, IEnumerable<IDataParameter> parametros = null)
        {
            return ExecuteScalar<T>(query, parametros);
        }

        public virtual IDataParameter GetDataParameter(string name, object value)
        {
            return new SqlParameter(name, value.ToDB());
        }

        /// <summary>
        /// Executa uma non-query no banco de dados (update, insert ou delete).
        /// </summary>
        /// <param name="nonQuery">A non-query a ser executada</param>
        /// <param name="parameters"></param>
        public virtual void ExecuteNonQuery(string nonQuery, IEnumerable<IDataParameter> parameters = null)
        {
            using (var command = SqlCommandFactory(nonQuery, ConnectionString, parameters, Timeout))
            {
                command.Connection.Open();
                command.ExecuteNonQuery();
                command.Connection.Close();
            }
        }

        public void ExecutaNonQuery(string nonQuery, IEnumerable<IDataParameter> parameters = null)
        {
            ExecuteNonQuery(nonQuery, parameters);
        }

        public void ExecuteProcedure(string query, IEnumerable<IDataParameter> parameters = null, int procedureTimeout = 120)
        {
            var command = SqlCommandFactory(query, ConnectionString, parameters);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = procedureTimeout;            

            command.Connection.Open();
            command.ExecuteNonQuery();
            command.Connection.Close();
        }

        public void SafeExecuteProcedure(string query, IEnumerable<IDataParameter> parameters = null, int procedureTimeout = 120)
        {
            try
            {
                var command = SqlCommandFactory(query, ConnectionString, parameters);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = procedureTimeout;

                command.Connection.Open();
                command.ExecuteNonQuery();
                command.Connection.Close();
            }
            catch
            {

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <param name="procedureTimeout"></param>
        /// <returns></returns>
        public virtual DataSet ExecuteReturningProcedure(string query, IEnumerable<IDataParameter> parameters = null, int procedureTimeout = 120)
        {
            var command = SqlCommandFactory(query, ConnectionString, parameters);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = procedureTimeout;

            DataSet ds = new DataSet();

            SqlDataAdapter da = new SqlDataAdapter((SqlCommand) command);

            da.Fill(ds);
            return ds;
        }

        /// <summary>
        /// Uma Factory para DataSets. Gerencia o uso das conexões de forma implícita, sem
        /// necessidade de controle manual por parte do programador. Responsável por receber
        /// uma query e uma conexão, e devolver um DataSet com o resultado.
        /// </summary>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
        /// <param name="query">A query a ser executada</param>
        /// <param name="connection">A conexão em que a Query deve ser executada</param>
        /// <param name="parameters"></param>
        /// <returns>O DataSet com os dados relativos ao resultado da Query</returns>
        protected virtual DataSet SqlDataSetFactory(string query, string connection, IEnumerable<IDataParameter> parameters)
        {
            DbCommand command = SqlCommandFactory(query, connection, parameters);

            DataSet ds = new DataSet();

            SqlDataAdapter da = new SqlDataAdapter((SqlCommand) command);

            da.Fill(ds);

            return ds;
        }

        /// <summary>
        /// Factory de comandos para SQL. 
        /// </summary>
        /// <param name="query">query a ser executada sobre o banco</param>
        /// <param name="connectionString">a conexão a ser utilizada pela query</param>
        /// <param name="parameters">Parametros da consulta SQL, se houver.</param>
        /// <returns></returns>
        protected virtual DbCommand SqlCommandFactory(string query, string connectionString, IEnumerable<IDataParameter> parameters = null, int? Timeout = null)
        {
            DbConnection connection = new SqlConnection(connectionString);
            DbCommand command = connection.CreateCommand();
            command.CommandTimeout = 9000000;

            if(!query.Contains(" "))
            {
                command.CommandType = CommandType.StoredProcedure;
            }

            if(Timeout.HasValue)
            { 
                command.CommandTimeout = Timeout.Value;
            }

            command.CommandText = query;

            if(parameters != null)
            {
                foreach (var parametro in parameters)
                {
                    command.Parameters.Add(parametro);
                }
            }

            return command;
        } 
    }
}
