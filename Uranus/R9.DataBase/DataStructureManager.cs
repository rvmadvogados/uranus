using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace R9.DataBase
{
    public static class DataStructureManager
    {
        public static List<Type> CheckedTables = new List<Type>();

        public static bool CheckTable<T>(T Item) where T : BaseModel
        {
            DataContext context = Item.Context;

            var type = typeof(T);

            if (CheckedTables.Contains(type)) return false; //Type already checked, do nothing.

            var builder = new SqlConnectionStringBuilder(context.ConnectionString);            

            Server server = new Server(builder.DataSource);
            server.ConnectionContext.LoginSecure = false;
            server.ConnectionContext.Login = builder.UserID;
            server.ConnectionContext.Password = builder.Password;

            Database database = server.Databases[builder.InitialCatalog];

            var tableName = Data.ParseTable(Item);

            foreach(Microsoft.SqlServer.Management.Smo.Table table in database.Tables)
            {
                if(table.Name == tableName)
                {
                    CheckedTables.Add(type);

                    return false;
                }
            }

            //Table wasn't found, so create it.
            var newTable = new Microsoft.SqlServer.Management.Smo.Table(database, tableName);

            //Use the properties as a base for the table.
            var properties = type.GetProperties();

            foreach(var property in properties)
            {
                if (property.GetCustomAttributes(false).Where(a => a is Column).FirstOrDefault() == null) continue;

                var newColumn = new Microsoft.SqlServer.Management.Smo.Column(newTable, property.Name, GetDataType(property));

                if (property.GetCustomAttributes(false).Where(a => a is ID).FirstOrDefault() != null)
                {
                    //Se é um ID, cria a chave primária.
                    newColumn.Identity = true;
                    Index index = new Index(newTable, "PK_" + newTable);
                    index.IndexKeyType = IndexKeyType.DriPrimaryKey;
                    index.IndexedColumns.Add(new IndexedColumn(index, property.Name));
                    newTable.Indexes.Add(index);
                }
                else
                {
                    //Verifica se é nullable.
                    if(Nullable.GetUnderlyingType(property.PropertyType) != null)
                    {
                        newColumn.Nullable = true;
                    }
                    else
                    {
                        newColumn.Nullable = false;
                    }
                }

                newTable.Columns.Add(newColumn);
            }

            var teste = newTable.Script();

            newTable.Create();
            
            CheckedTables.Add(type);
            return true;
        }

        private static DataType GetDataType(PropertyInfo property)
        {
            var propBaseType = property.PropertyType;

            if (Nullable.GetUnderlyingType(property.PropertyType) != null)
            {
                propBaseType = Nullable.GetUnderlyingType(property.PropertyType);
            }

            if (propBaseType == typeof(int))
            {
                return DataType.Int;
            }

            if (propBaseType == typeof(string))
            {
                return DataType.NVarChar(500);
            }

            if (propBaseType == typeof(decimal))
            {
                if(property.Name.Contains("CPF"))
                {
                    return DataType.Numeric(0,18);
                }
                else
                {
                    return DataType.Money;
                }
            }

            if (propBaseType == typeof(DateTime))
            {
                return DataType.DateTime;
            }

            if (propBaseType == typeof(long))
            {
                return DataType.BigInt;
            }

            if (propBaseType == typeof(bool))
            {
                return DataType.Bit;
            }

            return DataType.NVarChar(500);
        }
    }
}
