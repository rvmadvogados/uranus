using R9.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace R9.DataBase
{
    public static class XLSXData
    {
        public static List<T> LoadListFromFile<T>(string FilePath) where T: BaseModel
        {
            var retorno = new List<T>();

            Type type = typeof(T);

            var attrib = type.GetCustomAttributes(false).Where(a => a is Table || a is Query).FirstOrDefault();

            if (attrib == null)
            {
                throw new ArgumentNullException("Table", "Não há tabela/query especificada para realizar o Insert/Update no banco de dados.");
            }

            var DataTable = BuildQuery(attrib, FilePath);

            return BuildObjectsXLS<T>(DataTable);
        }

        private static DataTable BuildQuery(object attrib, string FilePath)
        {
            string ConnectionString = SelectProvider(FilePath);

            var TableName = attrib as Table;

            string query = "SELECT * FROM [" + TableName.Name + "$]";

            DataTable table = BuildDataTable(ConnectionString, TableName, query);

            return table;
        }

        private static DataTable BuildDataTable(string ConnectionString, Table TableName, string query)
        {
            DataTable table = new DataTable(TableName.Name);

            OleDbConnection Connection = new OleDbConnection(ConnectionString);
            Connection.Open();

            OleDbDataAdapter Adapter = new OleDbDataAdapter();
            OleDbCommand selectCommand = new OleDbCommand(query, Connection);
            Adapter.SelectCommand = selectCommand;
            Adapter.Fill(table);
            Connection.Close();

            return table;
        }

        public static string SelectProvider(string FilePath)
        {
            string ConnectionString = "";

            if (FilePath.Trim().EndsWith(".xlsx"))
            {
                ConnectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";", FilePath);
            }
            else if (FilePath.Trim().EndsWith(".xls"))
            {
                ConnectionString = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\";", FilePath);
            }

            return ConnectionString;
        }

        public static List<T> BuildObjectsXLS<T>(DataTable ds)
        {
            Type type = typeof(T);

            PropertyInfo[] properties = type.GetProperties();

            List<T> returnedList = new List<T>();

            foreach (DataRow dr in ds.Rows)
            {
                T ret = Activator.CreateInstance<T>();
                returnedList.Add(ret);

                foreach (PropertyInfo prop in properties)
                {
                    var propertyName = prop.Name.Replace("_", " ");

                    if (ds.Columns.Contains(propertyName))
                    {
                        if (dr[propertyName] != DBNull.Value)
                        {
                            if (prop.PropertyType == typeof(decimal) && dr[propertyName] is double)
                            {
                                prop.SetValue(ret, (decimal)(double)dr[propertyName], null);
                            }
                            else if (prop.PropertyType == typeof(int) && dr[propertyName] is double)
                            {
                                prop.SetValue(ret, (int)(double)dr[propertyName], null);
                            }
                            else if (prop.PropertyType == typeof(string) && dr[propertyName] is double)
                            {
                                prop.SetValue(ret, dr[propertyName].ToString(), null);
                            }
                            else
                            {
                                prop.SetValue(ret, dr[propertyName], null);
                            }
                        }
                    }
                }
            }

            return returnedList;
        }
    }
}
