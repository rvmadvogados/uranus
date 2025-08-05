using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using R9.DataBase;
using System.Collections.Concurrent;
using System.ComponentModel;
using Equin.ApplicationFramework;

namespace R9.DataBase
{
    public static class Data
    {
        /// <summary>
        /// Salva o item passado como parâmetro no banco de dados. 
        /// Gera um Insert se ID = 0, update caso contrário.
        /// </summary>
        /// <param name="item">O item a ser salvo no banco de dados</param>
        public static void Save(this BaseModel item, bool identityInsert = false, bool PkInsert = false, bool SaveCopy = false)
        {
            string _tableName;
            Dictionary<string, object> _ParamValues = new Dictionary<string, object>();
            PropertyInfo[] properties = item.GetType().GetProperties();

            _tableName = ParseTable(item);

            ParseParameters(item, _ParamValues, properties, identityInsert);

            #region insert/Update 
            if (item.HasID && !PkInsert && !SaveCopy)
            {
                GenerateUpdate(item, _ParamValues, _tableName, identityInsert);
            }
            else

            {
                GenerateInsert(item, _ParamValues, _tableName, identityInsert);
            }
            #endregion

        }

        public static void Save<T>(this List<T> Itens) where T : BaseModel
        {
            Itens.ForEach(x => x.Save());

            //foreach (var x in Itens) { x.Save(); }
        }

        public static void Save(List<BaseModel> bulkItems, bool identityInsert = false)
        {
            if (bulkItems.Count == 0)
            {
                return;
            }

            BaseModel item = bulkItems[0];            

            Dictionary<string, object> _ParamValues = new Dictionary<string, object>();
            PropertyInfo[] properties = item.GetType().GetProperties();

            var _tableName = ParseTable(item);

            ParseParameters(item, _ParamValues, properties, identityInsert);

            #region insert

            string insert = GenerateInsertBulkHeader(item, _ParamValues, _tableName, identityInsert);

            for (int i = 0; i < bulkItems.Count; i++)
            {
                ParseParameters(bulkItems[i], _ParamValues, properties, identityInsert);

                insert += GenerateInsertBulkValues(bulkItems[i], _ParamValues, identityInsert);

                if (i % 999 == 0 && i != 0)
                {
                    insert += ";";

                    if (i + 1 < bulkItems.Count)
                    {
                        insert += GenerateInsertBulkHeader(bulkItems[i], _ParamValues, _tableName, identityInsert);
                    }
                }
                else
                {
                    if (i + 1 < bulkItems.Count)
                    {
                        insert += ", ";
                    }
                    else
                    {
                        insert += ";";
                    }
                }
            }
            #endregion

            if (identityInsert)
            {
                insert = "SET IDENTITY_INSERT " + _tableName + " ON; " + insert + " ;" + "SET IDENTITY_INSERT " + _tableName + " OFF; ";
            }

            item.Context.ExecuteNonQuery(insert);
        }

        public static object LoadAll<T>(object conexãoSigefin, string v)
        {
            throw new NotImplementedException();
        }

        internal static string ParseTable(BaseModel item)
        {
            string _tableName = "";

            Table attrib = item.GetType().GetCustomAttributes(false).Where(a => a is Table).FirstOrDefault() as Table;

            if (attrib == null)
            {
                throw new ArgumentNullException("Table", "Não há tabela especificada para realizar o Insert/Update no banco de dados.");
            }           

            if (attrib.ReadOnly)
            {
                throw new InvalidOperationException("Não é possível realizar a atualização dos dados - a tabela especificada foi marcada como somente leitura nas classes de acesso.", null);
            }

            _tableName = attrib.Name;

            return _tableName;
        }

        private static void ParseParameters(BaseModel item, Dictionary<string, object> _ParamValues, PropertyInfo[] properties, bool identityInsert)
        {
            _ParamValues.Clear();

            #region ParseParameters
            foreach (var prop in properties)
            {
                if (prop.GetCustomAttributes(false).Where(a => a is Column).FirstOrDefault() != null)
                {
                    if (!identityInsert && prop.GetCustomAttributes(false).Where(a => a is Identity).FirstOrDefault() != null)
                    {
                        continue;
                    }

                    var value = prop.GetValue(item, null);

                    if (value != null)
                    {
                        if (value.GetType() == typeof(BaseModel))
                        {
                            //var baseModel = value as BaseModel;
                            //_ParamValues.Add("ID_"+ prop.Name, baseModel.ID);
                        }
                        else
                        {
                            if (value is DateTime)
                            {
                                if ((DateTime)value == new DateTime())
                                {
                                    _ParamValues.Add(prop.Name, DBNull.Value);
                                    continue;
                                }
                            }
                            _ParamValues.Add(prop.Name, value ?? DBNull.Value);
                        }
                    }
                    else
                    {
                        _ParamValues.Add(prop.Name, DBNull.Value);
                    }
                }
            }
            #endregion ParseParameters
        }


        /// <summary>
        /// Carrega um item único do banco de dados
        /// </summary>        
        public static T Load<T>(object ID, DataContext Context, T Refill = null) where T : BaseModel
        {
            Dictionary<string, object> keys = new Dictionary<string, object>();

            T mock = Activator.CreateInstance<T>();

            keys.Add(mock.PKNames[0], ID);

            return Load<T>(keys, Context, Refill);
        }

        /// <summary>
        /// Carrega um item único do banco de dados
        /// </summary>        
        public static T Load<T>(long ID, DataContext Context, T Refill = null) where T : BaseModel
        {
            Dictionary<string, object> keys = new Dictionary<string, object>();

            T mock = Activator.CreateInstance<T>();

            keys.Add(mock.PKNames[0], ID);

            return Load<T>(keys, Context, Refill);
        }

        /// <summary>
        /// Carrega um item único do banco de dados
        /// </summary>
        public static T Load<T>(DataContext Context, object key, T Refill = null) where T : BaseModel
        {
            Dictionary<string, object> keys = new Dictionary<string, object>();

            T mock = Activator.CreateInstance<T>();

            keys.Add(mock.PKNames[0], key);

            return Load<T>(keys, Context, Refill);
        }

        public static void Reload<T>(this T Item) where T : BaseModel
        {
            Dictionary<string, object> keys = new Dictionary<string, object>();

            keys.Add(Item.PKNames[0], Item.PKValues[0]);

            Load<T>(keys, Item.Context, Item);
            Item.ResetLists();
        }

        /// <summary>
        /// Carrega um item único do banco de dados
        /// </summary>        
        public static T Load<T>(Dictionary<string, object> keys, DataContext Context, T Refill) where T : BaseModel
        {
            T retorno = null;

            //Carrega o tipo especificado.
            Type type = typeof(T);

            var attrib = type.GetCustomAttributes(false).Where(a => a is Table).FirstOrDefault();

            if (attrib == null)
            {
                throw new ArgumentNullException("Table", "Não há tabela especificada para realizar o Insert/Update no banco de dados.");
            }

            var TableName = attrib as Table;

            T mock = Activator.CreateInstance<T>();

            var cache = false;

            if(mock is IR9Cacheable)
            {
                retorno = BuscarCache<T>((int)keys.Values.FirstOrDefault());

                if (retorno != null)
                {
                    cache = true;                    
                }
            }

            if (!cache)
            {
                string query = "SELECT * FROM " + TableName.Name + " WHERE ";
                List<SqlParameter> parameters = new List<SqlParameter>();

                foreach (KeyValuePair<string, object> kvp in keys)
                {
                    query += " " + kvp.Key + " = " + "@" + kvp.Key;
                    parameters.Add("@" + kvp.Key, kvp.Value);
                }

                retorno = QueryToObject<T>(Context, query, parameters, Refill);


                if(mock is IR9Cacheable && retorno != null)
                {
                    SalvarCache<T>(retorno);                    
                }
            }

            return retorno;
        }

        private static Dictionary<string, Dictionary<int, IR9Cacheable>> localCache = new Dictionary<string, Dictionary<int, IR9Cacheable>>();

        private static void SalvarCache<T>(T Objeto) where T : BaseModel
        {
            var nomeClasse = typeof(T).Name;

            if (!localCache.ContainsKey(nomeClasse))
            {
                localCache.Add(nomeClasse, new Dictionary<int, IR9Cacheable>());
            }

            if (localCache[nomeClasse].ContainsKey(((IR9Cacheable) Objeto).ID))
            {
                localCache[nomeClasse].Remove(((IR9Cacheable)Objeto).ID);
            }

            localCache[nomeClasse].Add(((IR9Cacheable)Objeto).ID, ((IR9Cacheable)Objeto));

            ((IR9Cacheable)Objeto).AtualizarValidadeCache();
        }

        private static T BuscarCache<T>(int ID) where T : BaseModel
        {
            var nomeClasse = typeof(T).Name;

            if (!localCache.ContainsKey(nomeClasse)) return null;

            if (!localCache[nomeClasse].ContainsKey(ID)) return null;

            if (localCache[nomeClasse][ID].ValidadeCache < DateTime.Now)
            {
                localCache[nomeClasse].Remove(ID);
                return null;
            }

            return (T)localCache[nomeClasse][ID];
        }

        public static T QueryToObject<T>(DataContext Context, string query, List<SqlParameter> parameters = null, T Refill = null) where T : BaseModel
        {
            DataSet ds = Context.ExecuteQuery(query, parameters);

            if (ds.Tables[0].Rows.Count == 0)
            {
                return default(T);
            }

            return BuildObjects<T>(Context, ds, Refill)[0];
        }

        public static List<T> BuildObjects<T>(DataContext Context, DataSet ds, T Refill = null) where T : BaseModel
        {
            Type type = typeof(T);

            PropertyInfo[] properties = type.GetProperties();

            List<T> returnedList = new List<T>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                T ret = MontarObjetoAvulso(Context, ds, Refill, properties, dr);


                returnedList.Add(ret);
            }

            return returnedList;
        }

        private static T MontarObjetoAvulso<T>(DataContext Context, DataSet ds, T Refill, PropertyInfo[] properties, DataRow dr) where T : BaseModel
        {
            T ret = Refill;

            if (ret == null)
            {
                ret = Activator.CreateInstance<T>();
            }
            ret.Context = Context;

            foreach (PropertyInfo prop in properties)
            {
                if (ds.Tables[0].Columns.Contains(prop.Name))
                {
                    if (dr[prop.Name] != DBNull.Value)
                    {
                        if (prop.PropertyType == typeof(long) && dr[prop.Name] is decimal)
                        {
                            prop.SetValue(ret, long.Parse(dr[prop.Name].ToString()), null);
                        }
                        else
                        {
                            prop.SetValue(ret, dr[prop.Name], null);
                        }
                    }
                }
            }
            BuildAutoLinks(Context, ret);
            return ret;
        }

        public static void RefreshLists<T>(this T item) where T : BaseModel
        {
            BuildAutoLinks<T>(item.Context, item);
        }

        private static void BuildAutoLinks<T>(DataContext Context, T ret) where T : BaseModel
        {
            var links = ret.GetAutoLinks();

            foreach (var link in links)
            {
                Type remoteType = (link.GetCustomAttributes().Where(x => x is Autolink).FirstOrDefault() as Autolink).ReferencedType;

                var typeData = typeof(Data);

                //remoteType é o tipo a ser buscado. A classe indicada por remoteType precisa ter uma propriedade marcada com FK e assinada com o tipo do objeto que está a chamando.

                var remoteProp = remoteType.GetProperties().Where(x => x.GetCustomAttributes().Where(y => y is FK && (y as FK).ReferencedType == ret.GetType()).FirstOrDefault() != null).FirstOrDefault();

                if (remoteProp != null)
                {
                    List<IDataParameter> parameters = new List<IDataParameter>();
                    string condition = $"{remoteProp.Name} = {Context.ParameterSeparator}{remoteProp.Name}";

                    var teste = remoteProp.GetCustomAttributes().Where(y => y is FK && (y as FK).ReferencedType == ret.GetType()).FirstOrDefault();

                    string OrderBy = (teste as FK).OrderBy;
                    parameters.Add(Context.GetDataParameter($"{remoteProp.Name}", ret.GetID()));

                    MethodInfo method = typeData.GetMethod(nameof(BuildGenericLazyList), BindingFlags.Static | BindingFlags.NonPublic);
                    method = method.MakeGenericMethod(new Type[] { remoteType });
                    object lazyList = method.Invoke(null, new object[] { Context, condition, parameters, OrderBy});

                    link.SetValue(ret, lazyList);
                }
            }
        }

        internal static Lazy<List<T>> BuildGenericSQLLazyList<T>(DataContext Context, string Condition, List<SqlParameter> parameters) where T : BaseModel
        {
            return new Lazy<List<T>>(() => Data.LoadAll<T>(Context, Condition, parameters));
        }

        internal static Lazy<List<T>> BuildGenericLazyList<T>(DataContext Context, string Condition, List<IDataParameter> parameters, string OrderBy = "") where T : BaseModel
        {
            return new Lazy<List<T>>(()=> Data.LoadAll<T>(Context, Condition + " " + OrderBy, parameters));
        }

        internal static Dictionary<int, T> BuildGenericList<T>() where T : IR9Cacheable
        {
            return new Dictionary<int, T>();
        }

        public static List<T> BuildObjects<T>(DataTable ds)
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
                    if (ds.Columns.Contains(prop.Name))
                    {
                        if (dr[prop.Name] != DBNull.Value)
                        {
                            if (prop.PropertyType == typeof(long) && dr[prop.Name] is decimal)
                            {
                                prop.SetValue(ret, long.Parse(dr[prop.Name].ToString()), null);
                            }
                            else
                            {
                                prop.SetValue(ret, dr[prop.Name], null);
                            }
                        }
                    }
                }
            }

            return returnedList;
        }

        public static void Delete(this BaseModel item)
        {
            var attrib = item.GetType().GetCustomAttributes(false).Where(a => a is Table).FirstOrDefault();

            if (attrib == null)
            {
                throw new ArgumentNullException("Table", "Não há tabela especificada para realizar o Insert/Update no banco de dados.");
            }

            var TableName = attrib as Table;

            string _tableName = TableName.Name;

            string query = "DELETE FROM " + _tableName + " WHERE ";

            List<SqlParameter> parameters = new List<SqlParameter>();

            string keysQuery = "";

            foreach (KeyValuePair<string, object> kvp in item.Keys)
            {
                if(!string.IsNullOrEmpty(keysQuery))
                {
                    keysQuery += " AND ";
                }

                keysQuery += " " + kvp.Key + " = " + "@" + kvp.Key + " ";
                parameters.Add("@" + kvp.Key, kvp.Value);
            }

            query += keysQuery;
             
            item.Context.ExecuteNonQuery(query, parameters);

            item.ResetKeys();
        }

        public static List<T> LoadAll<T>(DataContext Context, List<object> keys) where T : BaseModel
        {
            ConcurrentQueue<T> items = new ConcurrentQueue<T>();

            Parallel.ForEach(keys, (key) =>
            {
                items.Enqueue(Data.Load<T>(Context, key));
            });


            return items.ToList();
        }

        public static List<T> LoadAll<T>(DataContext Context, string Where = "", IEnumerable<IDataParameter> parametros = null, int? take = null) where T : BaseModel
        {
            //Carrega o tipo especificado.
            Type type = typeof(T);

            var attrib = type.GetCustomAttributes(false).Where(a => a is Table || a is Query).FirstOrDefault();

            if (attrib == null)
            {
                throw new ArgumentNullException("Table", "Não há tabela/query especificada para realizar o Insert/Update no banco de dados.");
            }

            string query = "";

            if (attrib is Table)
            {
                query = MontaQueryTable(Where, attrib, take);
            }
            else
            {
                query = MontaQueryQuery(Where, attrib);
            }

            return BuildAllObjects<T>(Context, query, parametros);
        }

        public static BindingListView<T> Pack<T>(List<T> lista)
        {
            return new BindingListView<T>(lista);
        }

        public static List<T> BuildAllObjects<T>(DataContext Context, string query, IEnumerable<IDataParameter> parametros = null) where T : BaseModel
        {
            DataSet ds = Context.ExecuteQuery(query, parametros);

            return BuildObjects<T>(Context, ds);
        }

        public static List<T> BuildObjects<T>(DataContext Context, string query, IEnumerable<IDataParameter> parametros = null)
        {
            DataSet ds = Context.ExecuteQuery(query, parametros);

            return BuildObjects<T>(ds.Tables[0]);
        }

        private static string MontaQueryQuery(string Where, object attrib)
        {
            string query;
            var QueryText = attrib as Query;

            query = QueryText.QueryText;

            if (!string.IsNullOrEmpty(Where))
            {
                query += " AND " + Where;
            }

            if (!string.IsNullOrEmpty(QueryText.OrderBy))
            {
                query += " ORDER BY " + QueryText.OrderBy;
            }

            return query;
        }

        private static string MontaQueryTable(string Where, object attrib, int? take)
        {
            var TableName = attrib as Table;

            string query = "SELECT * FROM " + TableName.Name;

            if(take.HasValue)
            {
                query = $"SELECT TOP {take.Value} * FROM " + TableName.Name;
            }


            if(TableName.NoLock)
            {
                query = query + " WITH (NOLOCK) ";
            }

            if (!string.IsNullOrEmpty(Where))
            {
                query += " WHERE " + Where;
            }

            return query;
        }

        private static void GenerateInsert(BaseModel item, Dictionary<string, object> _ParamValues, string _tableName, bool identityInsert = false)
        {
            string query = "INSERT INTO " + _tableName + " ( ";

            List<SqlParameter> parametros = new List<SqlParameter>();

            var pks = item.PKNames;

            if (pks.Count == 1)
            {
                foreach (var kvp in _ParamValues)
                {
                    if (kvp.Key != pks[0])
                    {
                        query += kvp.Key + ", ";
                        parametros.Add("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                    }
                }
            }
            else
            {
                foreach (var kvp in _ParamValues)
                {
                    query += kvp.Key + ", ";
                    parametros.Add("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                }
            }

            //Remove a vírgula e o espaço do final;
            query = query.Remove(query.Length - 2);

            query += " ) OUTPUT INSERTED." + pks[0] + " VALUES ( ";

            foreach (var kvp in _ParamValues)
            {
                if (pks.Count == 1 && kvp.Key == pks[0])
                {
                    continue;
                }
                else
                {
                    query += "@" + kvp.Key + ", ";
                }
            }

            //Remove a vírgula e o espaço do final;
            query = query.Remove(query.Length - 2);

            query += " ) ";

            if (identityInsert)
            {
                query = "SET IDENTITY_INSERT " + _tableName + " ON; " + query + " ;" + "SET IDENTITY_INSERT " + _tableName + " OFF; ";
            }

            object PKValue = item.Context.ExecuteScalar<object>(query, parametros);

            if (PKValue != null)
            {
                item.GetType().InvokeMember(pks[0],
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
                        Type.DefaultBinder, item, new object[] { PKValue });
            }
        }

        private static string GenerateInsertBulkHeader(BaseModel item, Dictionary<string, object> _ParamValues, string _tableName, bool identityInsert = false)
        {
            string query = "INSERT INTO " + _tableName + " ( ";

            var ids = item.IdentityNames;

            foreach (var kvp in _ParamValues)
            {
                if (!identityInsert && ids.Contains(kvp.Key))
                {
                    continue;
                }

                query += kvp.Key + ", ";
            }

            //Remove a vírgula e o espaço do final;
            query = query.Remove(query.Length - 2);

            query += " ) VALUES ";
            return query;
        }

        private static string GenerateInsertBulkValues(BaseModel item, Dictionary<string, object> _ParamValues, bool identityInsert = false)
        {
            string query = "(";

            var ids = item.IdentityNames;

            foreach (var kvp in _ParamValues)
            {
                if (!identityInsert)
                {
                    if (ids.Contains(kvp.Key))
                    {
                        continue;
                    }
                }

                query += FormatParameter(kvp.Value) + ", ";
            }

            //Remove a vírgula e o espaço do final;
            query = query.Remove(query.Length - 2);

            query += " ) ";

            return query;
        }

        private static string FormatParameter(object value)
        {
            object tempValue = value;

            if (tempValue == null || value is DBNull)
            {
                return "null";
            }
            else if (tempValue is string)
            {
                return "'" + ((string)tempValue).Replace("'", "''") + "'";
            }
            else if (tempValue is DateTime)
            {
                return "'" + ((DateTime)tempValue).ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }
            else if (tempValue is decimal || tempValue is float || tempValue is double)
            {
                return tempValue.ToString().Replace(",", ".");
            }
            else if (tempValue is bool)
            {
                return ((bool)tempValue) ? "1" : "0";
            }
            else
            {
                return value.ToString();
            }
        }

        private static void GenerateUpdate(BaseModel item, Dictionary<string, object> _ParamValues, string _tableName, bool IdentityInsert = false)
        {
            string query = "UPDATE " + _tableName + " SET  ";

            List<SqlParameter> parametros = new List<SqlParameter>();

            var keys = item.Keys;

            foreach (var kvp in _ParamValues)
            {
                if (!keys.ContainsKey(kvp.Key))
                {
                    query += kvp.Key + " = @" + kvp.Key + ", ";
                }

                parametros.Add("@" + kvp.Key, kvp.Value ?? kvp.Value);
            }

            //Remove a vírgula e o espaço do final;
            query = query.Remove(query.Length - 2);

            query += "  WHERE 1=1 ";

            foreach (var key in keys)
            {
                query += " AND " + key.Key + " = @" + key.Key;
            }

            if (IdentityInsert)
            {
                query = "SET IDENTITY_INSERT " + _tableName + " ON; " + query + " ;" + "SET IDENTITY_INSERT " + _tableName + " OFF; ";
            }

            item.Context.ExecuteNonQuery(query, parametros);
        }

        public static object LoadAll<T>()
        {
            throw new NotImplementedException();
        }
    }
}
