using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace R9.DataBase
{
    public static class DataBaseExtension
    {
        private static string[] NUMEROS_SINGULAR = new string[] { "MILHÃO", "MIL ", "" };
        private static string[] NUMEROS_PLURAL = new string[] { "MILHÕES", "MIL ", "" };

        #region Daisy-Chaining

        public static DateTime? ToOMNIDateTime(this string texto)
        {
            DateTime oRetorno = DateTime.Today;

            if (!string.IsNullOrEmpty(texto))
            {
                if (texto.Length == 8)
                {
                    string tempData = string.Concat(texto.Substring(4, 4), "-", texto.Substring(2, 2), "-", texto.Substring(0, 2));
                    if (DateTime.TryParse(tempData, out oRetorno))
                        return oRetorno;
                }
                else
                {
                    if(texto.Length == 10)
                    {
                        string tempData = string.Concat(texto.Substring(6, 4), "-", texto.Substring(3, 2), "-", texto.Substring(0, 2));
                        if (DateTime.TryParse(tempData, out oRetorno))
                            return oRetorno;
                    }
                }
            }

            return null;
        }

        public static string GetIndexedToString<T>(this List<T> list, int Index)
        {
            if(Index < list.Count)
            {
                return list[Index].ToString();
            }
            else
            {
                return "";
            }
        }

        public static T GetIndexOrDefault<T>(this List<T> list, int Index)
        {
            if (Index < list.Count)
            {
                return list[Index];
            }
            else
            {
                return default(T);
            }
        }

        public static string DumpParameters(this List<SqlParameter> list)
        {
            string ret = "";

            foreach(var item in list)
            {
                if(ret != "")
                {
                    ret += ",";
                }

                ret += item.ParameterName + " = " + item.Value + Environment.NewLine;
            }

            return ret;
        }

        public static List<SqlParameter> Add(this List<SqlParameter> list, string parameterName, object value)
        {
            if (!parameterName.Contains('@'))
            {
                parameterName = "@" + parameterName;
            }

            object ox = value.ToDB();

            list.Add(new SqlParameter(parameterName, value.ToDB()));

            return list;
        }

        public static List<NpgsqlParameter> Add(this List<NpgsqlParameter> list, string parameterName, object value)
        {
            if (!parameterName.Contains(':'))
            {
                parameterName = ":" + parameterName;
            }

            object ox = value.ToDB();

            list.Add(new NpgsqlParameter(parameterName, value.ToDB()));

            return list;
        }
        #endregion Daisy-Chaining        
        

        public static T Clone<T>(this T ObjetoBase) where T: BaseModel
        {
            var novoObjeto = Activator.CreateInstance<T>();

            var props = ObjetoBase.GetType().GetProperties().Where(x=> x.GetCustomAttributes().Where(y=> y is Column).FirstOrDefault() != null);

            foreach (PropertyInfo sourcePropertyInfo in props)
            {
                PropertyInfo destPropertyInfo = novoObjeto.GetType().GetProperty(sourcePropertyInfo.Name);

                destPropertyInfo.SetValue(
                    novoObjeto,
                    sourcePropertyInfo.GetValue(ObjetoBase, null),
                    null);
            }

            novoObjeto.Context = ObjetoBase.Context;

            return novoObjeto;
        }

        public static object ToDB(this object objeto)
        {
            return objeto ?? DBNull.Value;
        }

        public static string Higienizar(this string stringOriginal, bool celular)
        {
            string numeroLimpo = new string(stringOriginal.ToCharArray().Where(x => char.IsDigit(x)).ToArray());

            if(celular == true)
            {
                switch (numeroLimpo.Length)
                {
                    case 8:
                        numeroLimpo = "0519" + numeroLimpo;
                        break;
                    case 9:
                        numeroLimpo = "051" + numeroLimpo;
                        break;
                    case 10:
                        numeroLimpo = "0" + numeroLimpo.Substring(0, 2) + "9" + numeroLimpo.Substring(2);
                        break;
                    case 11:
                        numeroLimpo = "0" + numeroLimpo;
                        break;
                    default:
                        numeroLimpo = ("Impossível arrumar.");
                        break;
                }

                return numeroLimpo;
            }
            else
            {
                switch (numeroLimpo.Length)
                {
                    case 8:
                        numeroLimpo = "0510" + numeroLimpo;
                        break;
                    case 9:
                        numeroLimpo = "051" + numeroLimpo;
                        break;
                    case 10:
                        numeroLimpo = "0" + numeroLimpo.Substring(0, 2) + "9" + numeroLimpo.Substring(2);
                        break;
                    case 11:
                        numeroLimpo = "0" + numeroLimpo;
                        break;
                    default:
                        numeroLimpo = ("Impossível arrumar.");
                        break;
                }

                return numeroLimpo;
            }
        }

        public static bool IsEmpty<T>(this List<T> list)
        {
            return list.Count == 0;
        }

        public static string FromDBToString(this object objeto)
        {
            if (objeto == DBNull.Value)
            {
                return "";
            }
            else
            {
                return (objeto??"").ToString();
            }
        }

        public static string Truncate(this string value, int maxLength, bool AutoEllipsis = false)
        {
            if (AutoEllipsis)
            {
                if (string.IsNullOrEmpty(value)) return value;
                return value.Length <= maxLength ? value : value.Substring(0, maxLength - 3) + "...";
            }
            else
            {
                if (string.IsNullOrEmpty(value)) return value;
                return value.Length <= maxLength ? value : value.Substring(0, maxLength);
            }
        }

        //public static string Truncate(this string value, int maxLength)
        //{
        //    if (string.IsNullOrEmpty(value)) return value;
        //    return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        //}

        public static T FromDB<T>(this object objeto)
        {
            try
            {
                if (objeto == DBNull.Value)
                {
                    return default(T);
                }
                else
                {
                    return (T)objeto;
                }
            }
            catch
            {
                throw new InvalidCastException("Tipo de conversão inválida: parâmetro é do tipo " + typeof(object).Name + ", não é possível convertar para " + typeof(T).Name);
            }
        }

        public static string NullIfEmpty(this string item)
        {
            if(string.IsNullOrEmpty(item))
            {
                return null;
            }
            else
            {
                return item;
            }
        }

        public static int? ToNullableInt(this string item)
        {
            if (int.TryParse(item, out int valor))
            {
                return valor;
            }
            else
            {
                return null;
            }
        }

        public static decimal? ToNullableDecimal(this string item)
        {
            if (decimal.TryParse(item, out decimal valor))
            {
                return valor;
            }
            else
            {
                return null;
            }
        }

        public static string NullifyIfEmpty(this string objeto)
        {   
                if (objeto == "")
                {
                    return null;
                }
                else
                {
                    return objeto;
                }            
        }

        /// <summary>
        /// Extension methods that returns true of there is any kind of result value
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool HasData(this DataSet data)
        {
            return (data != null && data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0);
        }

        /// <summary>
        /// Transforma valor recebido em número extenso
        /// </summary>
        /// <param name="valor">O valor recebido não pode ser negativo ou zero e ultrapassar a casa dos milhões</param>
        /// <returns>Retorna uma string escrito o valor por extenso</returns>
        public static string Monetiza(this decimal valor)
        {
            if (valor <= 0 | valor > 100000000)
            {
                return "Valor não aceito.";
            }

            else
            {
                string strValor = valor.ToString("000000000.00");
                string ValorPorExtenso = string.Empty;

                for (int i = 0; i <= 6; i += 3)
                {
                    var dec = Convert.ToDecimal(strValor.Substring(i, 3));

                    var novaParte = EscrevaParte(dec);

                    if (ValorPorExtenso != "" && novaParte != "")
                    {
                        ValorPorExtenso = ValorPorExtenso.Replace("*E ", ", ");
                        ValorPorExtenso += "*E ";
                    }

                    ValorPorExtenso += novaParte;

                    if (dec > 1)
                    {
                        ValorPorExtenso += " " + NUMEROS_PLURAL[Math.DivRem(i, 3, out int rem)];
                    }
                    else if (dec == 1)
                    {
                        ValorPorExtenso += " " + NUMEROS_SINGULAR[Math.DivRem(i, 3, out int rem)];
                    }

                    if (i == 6)
                    {
                        ValorPorExtenso = ValorPorExtenso.Replace("*E ", "E ");
                        ValorPorExtenso += "REAIS* ";

                        dec = Convert.ToDecimal(strValor.Substring(10, 2));

                        if (dec > 1)
                        {
                            ValorPorExtenso += " E* " + EscrevaParte(dec) + " CENTAVOS.";
                        }
                        else if (dec == 1)
                        {
                            ValorPorExtenso += " E* UM CENTAVO.";
                        }

                        dec = Convert.ToDecimal(strValor.Substring(0, 9));

                        if (dec == 0)
                        {
                            ValorPorExtenso = ValorPorExtenso.Replace(" E* ", "");
                            ValorPorExtenso = ValorPorExtenso.Replace("REAIS* ", "");
                        }
                        else
                        {
                            ValorPorExtenso = ValorPorExtenso.Replace(" E* ", " E ");
                        }

                        ValorPorExtenso = ValorPorExtenso.Replace("REAIS* ", "REAIS");
                    }
                }

                return ValorPorExtenso;
            }
        }

        /// <summary>
        /// Este metódo coleta 3 casas decimais e as escreve por extenso
        /// </summary>
        /// <param name="valor">O valor recebido não pode ser negativo ou zero</param>
        /// <returns>Retorna o valor por extenso da centena, dezena e unidade</returns>
        private static string EscrevaParte(decimal valor)
        {
            if (valor <= 0)
            {
                return string.Empty;
            }

            else
            {
                string MontaNumero = string.Empty;

                if (valor > 0 && valor < 1)
                {
                    valor *= 100;
                }
                string strValor = valor.ToString("000");

                int Centena = Convert.ToInt32(strValor.Substring(0, 1));
                int Dezena = Convert.ToInt32(strValor.Substring(1, 1));
                int Unidade = Convert.ToInt32(strValor.Substring(2, 1));

                switch (Centena)
                {
                    case 1:
                        if (Dezena + Unidade == 0)
                        {
                            MontaNumero += "CEM";
                        }
                        else
                        {
                            MontaNumero += "CENTO";
                        }
                        break;

                    case 2:
                        MontaNumero += "DUZENTOS";
                        break;

                    case 3:
                        MontaNumero += "TREZENTOS";
                        break;

                    case 4:
                        MontaNumero += "QUATROCENTOS";
                        break;

                    case 5:
                        MontaNumero += "QUINHENTOS";
                        break;

                    case 6:
                        MontaNumero += "SEISCENTOS";
                        break;

                    case 7:
                        MontaNumero += "SETECENTOS";
                        break;

                    case 8:
                        MontaNumero += "OITOCENTOS";
                        break;

                    case 9:
                        MontaNumero += "NOVECENTOS";
                        break;
                }

                if (Centena > 0 && (Dezena > 0 || Unidade > 0))
                {
                    MontaNumero += " E ";

                }

                switch (Dezena)
                {
                    case 1:
                        MontaNumero = DeterminaDezena(MontaNumero, Centena, Unidade);
                        break;
                    case 2:
                        MontaNumero += "VINTE";
                        break;
                    case 3:
                        MontaNumero += "TRINTA";
                        break;
                    case 4:
                        MontaNumero += "QUARENTA";
                        break;
                    case 5:
                        MontaNumero += "CINQUENTA";
                        break;
                    case 6:
                        MontaNumero += "SESSENTA";
                        break;
                    case 7:
                        MontaNumero += "SETENTA";
                        break;
                    case 8:
                        MontaNumero += "OITENTA";
                        break;
                    case 9:
                        MontaNumero += "NOVENTA";
                        break;
                }

                if (Dezena > 1 && Unidade > 0)
                {
                    MontaNumero += " E ";
                }

                if (Dezena != 1)
                {
                    MontaNumero = MontaUnidade(MontaNumero, Unidade);
                }


                return MontaNumero;
            }
        }

        /// <summary>
        /// Metódo para retornar as unidades.
        /// </summary>
        /// <param name="MontaNumero">Tem que ser uma string</param>
        /// <param name="Unidade">Tem que ser uma unidade de número</param>
        /// <returns>Retorna a unidade por extenso</returns>
        private static string MontaUnidade(string MontaNumero, int Unidade)
        {
            switch (Unidade)
            {
                case 1:
                    MontaNumero += "UM";
                    break;
                case 2:
                    MontaNumero += "DOIS";
                    break;
                case 3:
                    MontaNumero += "TRÊS";
                    break;
                case 4:
                    MontaNumero += "QUATRO";
                    break;
                case 5:
                    MontaNumero += "CINCO";
                    break;
                case 6:
                    MontaNumero += "SEIS";
                    break;
                case 7:
                    MontaNumero += "SETE";
                    break;
                case 8:
                    MontaNumero += "OITO";
                    break;
                case 9:
                    MontaNumero += "NOVE";
                    break;
            }

            return MontaNumero;
        }

        /// <summary>
        /// Metódo para retornar as dezenas de 10 á 19, pois dentro desses valores, o valor por extenso é diferente dos outros
        /// </summary>
        /// <param name="MontaNumero">Tem que ser uma string</param>
        /// <param name="Centena">Tem que ser um inteiro</param>
        /// <param name="Unidade">tem que ser uma unidade</param>
        /// <returns>Retorna o valor por extenso das dezenas de 10 á 19</returns>
        private static string DeterminaDezena(string MontaNumero, int Centena, int Unidade)
        {
            switch (Unidade)
            {
                case 0:
                    MontaNumero += "DEZ";
                    break;

                case 1:
                    MontaNumero += "ONZE";
                    break;

                case 2:
                    MontaNumero += "DOZE";
                    break;

                case 3:
                    MontaNumero += "TREZE";
                    break;

                case 4:
                    MontaNumero += "QUATORZE";
                    break;

                case 5:
                    MontaNumero += "QUINZE";
                    break;

                case 6:
                    MontaNumero += "DEZESSEIS";
                    break;

                case 7:
                    MontaNumero += "DEZESSETE";
                    break;

                case 8:
                    MontaNumero += "DEZOITO";
                    break;

                case 9:
                    MontaNumero += "DEZENOVE";
                    break;
            }

            return MontaNumero;
        }

        public static string SafeToString<T>(this Nullable<T> objeto) where T: struct
        {
            if(!objeto.HasValue)
            {
                return string.Empty;
            }
            else
            {
                return objeto.Value.ToString();
            }
        }

        public static string SafeToString(this decimal? objeto, string format) 
        {
            if (!objeto.HasValue)
            {
                return string.Empty;
            }
            else
            {
                return objeto.Value.ToString(format);
            }
        }

        public static decimal FromMoney(this string valor)
        {
            return decimal.Parse(valor.Replace("$", "").Replace("R", ""));
        }
    }    
}
