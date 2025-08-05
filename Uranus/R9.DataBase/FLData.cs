using R9.DataBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static R9.DataBase.CSVData;

namespace R9.DataBase
{
    public static class FLData
    {
        public static List<T> LoadListFromFLString<T>(string FLData)
        {
            var data = LoadDataTableFromFLString<T>(FLData);

            var list = Data.BuildObjects<T>(data);

            return list;
        }

        public static DataTable GetTemplateDataTable(Type T)
        {
            DataTable ret = new DataTable();

            var properties = T.GetProperties();

            foreach (var prop in properties)
            {
                if (prop.GetCustomAttributes(false).Where(a => a is Column && !(a is PK)).FirstOrDefault() != null)
                {
                    ret.Columns.Add(prop.Name, prop.PropertyType);
                }
            }

            return ret;
        }

        private static DataTable LoadDataTableFromFL<T>(string data, ModoNumeroDecimal modoNumero = ModoNumeroDecimal.BRASILEIRO)
        {
            return ConstruirDataSet<T>(0, false, modoNumero, data);
        }

        private static DataTable LoadDataTableFromFLString<T>(string data, ModoNumeroDecimal modoNumero = ModoNumeroDecimal.BRASILEIRO)
        {
            return ConstruirDataSet<T>(linesToIgnore: 0, ignoreLastLine: false, modoNumero: modoNumero, data: data);
        }

        private static DataTable ConstruirDataSet<T>(int linesToIgnore, bool ignoreLastLine, ModoNumeroDecimal modoNumero, string data)
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
                            List<object> fieldData = ForgeObjectFromLine<T>(modoNumero, template, line);                            

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

        private static List<object> ForgeObjectFromLine<T>(ModoNumeroDecimal modoNumero, DataTable template, string line)
        {
            var type = typeof(T);            
            var fields = SliceColumns(line, type);
            var fieldData = new List<object>();

            for (int i = 0; i < fields.Count && i < template.Columns.Count; i++)
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

        private static List<string> SliceColumns(string line, Type type)
        {
            List<string> ret = new List<string>();

            var props = type.GetProperties();

            foreach (var prop in props)
            {
                var colData = prop.GetCustomAttributes(false).Where(a => a is Column).FirstOrDefault() as Column;

                if (colData!= null)
                {
                    try
                    {
                        ret.Add(line.Substring(colData.Start, colData.Size));
                    }
                    catch(Exception ex)
                    {

                    }
                }
            }

            return ret;
        }
    }
}
