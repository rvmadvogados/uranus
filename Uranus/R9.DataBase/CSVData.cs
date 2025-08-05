using R9.DataBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R9.DataBase
{
    public abstract class TaggedCSV
    {
        [Column]
        public string Tag { get; set; }
    }

    public static class CSVData
    {
        public static List<T> LoadListFromCSV<T>(string path, bool ignoreFirstLine, char separator = ';')
        {
            return LoadListFromCSV<T>(path, 1, false, separator: separator);
        }

        public static void Save(BaseModel item, string DestinationFile, CSVWriteMode mode, char separator = ';')
        {
            Type T = item.GetType();

            var props = T.GetProperties().Where(x => x.GetCustomAttributes(false).Where(y => y is Column).FirstOrDefault() != null);

            string LineToAdd = BuildNewLine(item, separator, props);

            FileInfo file = new FileInfo(DestinationFile);

            if (file.Exists && mode == CSVWriteMode.Overwrite)
            {
                file.Delete();                
            }

            if(!file.Exists)
            {
                LineToAdd = BuildHeader(separator, props) + Environment.NewLine + LineToAdd;
            }

            StreamWriter writer = new StreamWriter(DestinationFile, true);

            writer.WriteLine(LineToAdd);
            writer.Close();
        }

        private static string BuildNewLine(BaseModel item, char separator, IEnumerable<System.Reflection.PropertyInfo> props)
        {
            string LineToAdd = "";

            foreach (var prop in props)
            {
                if (!string.IsNullOrEmpty(LineToAdd))
                {
                    LineToAdd += separator;
                }

                LineToAdd += (prop.GetValue(item) ?? "").ToString();
            }

            return LineToAdd;
        }

        private static string BuildHeader(char separator, IEnumerable<System.Reflection.PropertyInfo> props)
        {
            string LineToAdd = "";

            foreach (var prop in props)
            {
                if (!string.IsNullOrEmpty(LineToAdd))
                {
                    LineToAdd += separator;
                }

                LineToAdd += (prop.Name).ToString();
            }

            return LineToAdd;
        }

        public static List<T> LoadListFromCSVString<T>(string csvData, char separator = ';', int linesToIgnore = 0)
        {
            var data = LoadDataTableFromCSVString<T>(csvData, separator, linesToIgnore: linesToIgnore);

            var list = Data.BuildObjects<T>(data);

            return list;
        }

        public static List<T> LoadListFromCSV<T>(string path, int linesToIgnore, bool ignoreLastLine, ModoNumeroDecimal modoDecimal = ModoNumeroDecimal.BRASILEIRO, char separator = ';')
        {
            var data = LoadDataTableFromCSV<T>(path, linesToIgnore, ignoreLastLine,separator, modoDecimal);

            var list = Data.BuildObjects<T>(data);

            return list;
        }

        public static List<T> LoadListFromCSV<T>(string data, ModoNumeroDecimal modoDecimal = ModoNumeroDecimal.BRASILEIRO, char separator = ';')
        {
            var morphedData = LoadDataTableFromCSV<T>(data, separator, modoDecimal);

            var list = Data.BuildObjects<T>(morphedData);

            return list;
        }

        public static T LoadItemromCSV<T>(string data, ModoNumeroDecimal modoDecimal = ModoNumeroDecimal.BRASILEIRO, char separator = ';')
        {
            var list = LoadListFromCSV<T>(data, modoDecimal);

            return list[0];
        }

        public static List<TaggedCSV> LoadTaggedListFromCSV<T>(string path, int linesToIgnore, bool ignoreLastLine, ModoNumeroDecimal modoDecimal = ModoNumeroDecimal.BRASILEIRO, char separator = ';') where T : TaggedCSV
        {
            var data = LoadDataTableFromCSV<T>(path, linesToIgnore, ignoreLastLine, separator, modoDecimal);

            var list = Data.BuildObjects<T>(data);

            return list.Cast<TaggedCSV>().ToList();
        }

        private static DataTable GetTemplateDataTable(Type T)
        {
            DataTable ret = new DataTable();

            var properties = T.GetProperties();

            foreach (var prop in properties)
            {   
                // ALTERADO POR MÁRCIO C.
                // PARA QUE POSSA ADD PROPRIEDADES QUE RECEBEM NULL
                if (prop.GetCustomAttributes(false).Where(a => a is Column && !(a is PK)).FirstOrDefault() != null)
                {
                    ret.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }

            }

            return ret;
        }

        public enum ModoNumeroDecimal
        {
            AMERICANO,
            BRASILEIRO
        }

        public enum CSVWriteMode
        {
            Append,
            Overwrite,
        }

        private static DataTable LoadDataTableFromCSV<T>(string data, char separator, ModoNumeroDecimal modoNumero = ModoNumeroDecimal.BRASILEIRO)
        {
            return ConstruirDataSet<T>(0, false, modoNumero, data, separator);
        }

        private static DataTable LoadDataTableFromCSVString<T>(string data, char separator, ModoNumeroDecimal modoNumero = ModoNumeroDecimal.BRASILEIRO, int linesToIgnore = 0)
        {
            return ConstruirDataSet<T>(linesToIgnore: linesToIgnore , ignoreLastLine: false, modoNumero:modoNumero, data:data, separator:separator);
        }

        private static DataTable LoadDataTableFromCSV<T>(string path, int linesToIgnore, bool ignoreLastLine, char separator, ModoNumeroDecimal modoNumero = ModoNumeroDecimal.BRASILEIRO )
        {
            StreamReader reader = new StreamReader(path);
            var data = reader.ReadToEnd();
            reader.Close();

            return ConstruirDataSet<T>(linesToIgnore, ignoreLastLine, modoNumero, data, separator);
        }

        private static DataTable ConstruirDataSet<T>(int linesToIgnore, bool ignoreLastLine, ModoNumeroDecimal modoNumero, string data, char separator)
        {
            Type type = typeof(T);
            var template = GetTemplateDataTable(type);

            var lines = data.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var ignoreCount = 0;
            var lineCount = 0;

            foreach (var line in lines)
            {
                lineCount++;

                if (ignoreCount < linesToIgnore)
                {
                    ignoreCount++;
                    continue;
                }
                else if (ignoreLastLine && lineCount == lines.Length)
                {
                    continue;
                }
                else
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        try
                        {
                            List<object> fieldData = ForgeObjectFromLine(modoNumero, template, line, separator);

                            if (type == typeof(TaggedCSV))
                            {
                                fieldData.Add(line);
                            }

                            template.Rows.Add(fieldData.ToArray());
                        }
                        catch
                        {

                        }
                    }
                }
            }

            return template;
        }



        private static List<object> ForgeObjectFromLine(ModoNumeroDecimal modoNumero, DataTable template, string line, char separator)
        {
            var fields = line.Trim().Split(new char[] { separator });

            var fieldData = new List<object>();

            for (int i = 0; i < fields.Length && i< template.Columns.Count; i++)
            {
                Type targetType = template.Columns[i].DataType;

                TypeConverter converter = TypeDescriptor.GetConverter(targetType);

                if (targetType == typeof(decimal))
                {
                    if (modoNumero == ModoNumeroDecimal.AMERICANO)
                    {
                        var morphedValue = decimal.Parse(fields[i].Replace('.', ';').Replace(',', '.').Replace(';', ','));
                        fieldData.Add(morphedValue);
                    }
                    else
                    {
                        var morphedValue = decimal.Parse(fields[i]);                     
                        fieldData.Add(morphedValue);
                    }
                }
                else
                {
                    try
                    {
                        var morphedValue = converter.ConvertFromString(fields[i]);
                        fieldData.Add(morphedValue);
                    }
                    catch
                    {
                        fieldData.Add(default(Type));
                    }
                }
            }
            return fieldData;
        }
    }
}

