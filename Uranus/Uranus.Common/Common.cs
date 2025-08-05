using System;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Uranus.Common
{
    public static class Util
    {
        public static string GerarHashMd5(string input)
        {
            MD5 md5Hash = MD5.Create();
            // Converter a String para array de bytes, que é como a biblioteca trabalha.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Cria-se um StringBuilder para recompôr a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop para formatar cada byte como uma String em hexadecimal
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public static bool IsNumeric(this string s)
        {
            float output;
            return float.TryParse(s, out output);
        }

        public static bool IsDate(Object obj)
        {
            string strDate = obj.ToString();
            try
            {
                DateTime dt = DateTime.Parse(strDate);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsDate(string date, string format)
        {
            DateTime parsedDate;
            bool isValidDate;

            isValidDate = DateTime.TryParseExact(date, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);

            return isValidDate;
        }

        public static string OnlyNumbers(string value)
        {
            var digits = new Regex(@"[^\d]");
            return digits.Replace(value, "");
        }

        public static string OnlyAlphaNumeric(string value)
        {
            var digits = new Regex("[^a-zA-Z0-9]");
            return digits.Replace(value, "");
        }

        public static string FormatPhone(string value)
        {
            string phone = string.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                // por omissão tem 10 ou menos dígitos
                string mask = "{0:(00) 0000-0000}";
                // converter o texto em número
                long number = Convert.ToInt64(OnlyNumbers(value));

                if (value.Length == 11)
                    mask = "{0:(00) 0000-00000}";

                phone = string.Format(mask, number);
            }

            return phone;
        }

        public static string FormatCNPJ(string value)
        {
            string cnpj = string.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                long number = Convert.ToInt64(OnlyNumbers(value));

                cnpj = number.ToString(@"00\.000\.000\/0000\-00");
            }

            return cnpj;
        }

        public static string FormatCPF(string value)
        {
            string cpf = string.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                long number = Convert.ToInt64(OnlyNumbers(value));

                cpf = number.ToString(@"000\.000\.000\-00");
            }

            return cpf;
        }

        public static string FormatCEP(string value)
        {
            string cep = string.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                long number = Convert.ToInt64(OnlyNumbers(value));

                cep = number.ToString(@"00\.000\-000");
            }

            return cep;
        }

        public static string IntervalYearMonByExtensive(String DataInicial, String DataFinal)
        {
            DateTime data_inicial = DateTime.Parse(DataInicial);
            DateTime data_final = DateTime.Now;
            if (DataFinal != "01/99/9999") data_final = DateTime.Parse(DataFinal);

            // obtém a diferença
            TimeSpan dif = data_final.Subtract(data_inicial);

            Int32 ano = (dif.Days / 365);
            Int32 mes = (dif.Days / 30);

            // exibe o resultado
            String resultado = String.Empty;
            if (ano > 0)
            {
                if (ano == 1)
                    resultado += ano + " Ano";
                else
                    resultado += ano + " Anos";
            }
            if (mes > 11)
            {
                mes = mes - (ano * 12);
            }
            if (ano > 0 && mes > 0) resultado += " e ";
            if (mes > 0)
            {
                if (mes == 1)
                    resultado += mes + " Mês";
                else
                    resultado += mes + " Meses";
            }

            return resultado;
        }

        public static int SearchforPossibleSchedulesCreated(string DataInicio, string DataFim, string HoraInicio, string HoraFim, bool Intervalo)
        {
            var dateFirst = DateTime.Parse(DataInicio);
            var dateLast = DateTime.Parse(DataFim);
            var days = (dateLast - dateFirst).TotalDays + 1;
            var hours = (DateTime.Parse(HoraFim) - DateTime.Parse(HoraInicio)).TotalHours - (Intervalo && (int.Parse(HoraInicio.Substring(0, HoraInicio.IndexOf(':'))) <= 12) && (int.Parse(HoraFim.Substring(0, HoraFim.IndexOf(':'))) >= 12) ? 1 : 0) + 1;
            var totalDays = 0;

            for (int i = 1; i < days + 1; i++)
            {
                if (dateFirst.DayOfWeek != DayOfWeek.Saturday && dateFirst.DayOfWeek != DayOfWeek.Sunday)
                {
                    totalDays++;
                }

                dateFirst = dateFirst.AddDays(1);
            }

            var total = (totalDays * int.Parse(hours.ToString()));

            return total;
        }

        public static DateTime NextDate(this DateTime from, DayOfWeek dayOfWeek)
        {
            int start = (int)from.DayOfWeek;
            int target = (int)dayOfWeek;
            if (target <= start)
                target += 7;
            return from.AddDays(target - start);
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static string GetOSVersion()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32S:
                    return "Win 3.1";

                case PlatformID.Win32Windows:
                    switch (Environment.OSVersion.Version.Minor)
                    {
                        case 0:
                            return "Win95";
                        case 10:
                            return "Win98";
                        case 90:
                            return "WinME";
                    }
                    break;

                case PlatformID.Win32NT:
                    switch (Environment.OSVersion.Version.Major)
                    {
                        case 3:
                            return "NT 3.51";
                        case 4:
                            return "NT 4.0";
                        case 5:
                            switch (Environment.OSVersion.Version.Minor)
                            {
                                case 0:
                                    return "Win2000";
                                case 1:
                                    return "WinXP";
                                case 2:
                                    return "Win2003";
                            }
                            break;

                        case 6:
                            switch (Environment.OSVersion.Version.Minor)
                            {
                                case 0:
                                    return "Vista/Win2008Server";
                                case 1:
                                    return "Win7/Win2008Server R2";
                                case 2:
                                    return "Win8/Win2012Server";
                                case 3:
                                    return "Win8.1/Win2012Server R2";
                            }
                            break;
                        case 10:  //this will only show up if the application has a manifest file allowing W10, otherwise a 6.2 version will be used
                            return "Windows 10";
                    }
                    break;

                case PlatformID.WinCE:
                    return "Win CE";

                case PlatformID.Unix:
                    return "Linux";

                case PlatformID.MacOSX:
                    return "Mac";

            }

            return "Unknown";
        }

        public static string GetWebBrowserName()
        {
            string WebBrowserName = string.Empty;
            try
            {
                WebBrowserName = HttpContext.Current.Request.Browser.Browser;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return WebBrowserName;
        }

        public static string FormatClient(string part)
        {
            string client = string.Empty;

            if (part.IndexOf("XRéu ") > 0)
            {
                client = part.Substring(0, part.IndexOf("XRéu ")).Replace("Autor ", string.Empty).Trim();
            }
            else if (part.IndexOf("XExecutado ") > 0)
            {
                client = part.Substring(0, part.IndexOf("XExecutado ")).Replace("Exequente ", string.Empty).Trim();
            }
            else if (part.IndexOf("XRecorrido ") > 0)
            {
                client = part.Substring(0, part.IndexOf("XRecorrido ")).Replace("Recorrente ", string.Empty).Trim();
            }
            else if (part.IndexOf("XRequerido ") > 0)
            {
                client = part.Substring(0, part.IndexOf("XRequerido ")).Replace("Requerente ", string.Empty).Trim();
            }
            else if (part.IndexOf("XImpetrado ") > 0)
            {
                client = part.Substring(0, part.IndexOf("XImpetrado ")).Replace("Impetrante ", string.Empty).Trim();
            }
            else if (part.IndexOf("XAgravado ") > 0)
            {
                client = part.Substring(0, part.IndexOf("XAgravado ")).Replace("Agravante ", string.Empty).Trim();
            }
            else if (part.IndexOf("XApelado ") > 0)
            {
                client = part.Substring(0, part.IndexOf("XApelado ")).Replace("Apelante ", string.Empty).Trim();
            }
            else
            {
                client = part;
            }

            return client;
        }

        public static string IntegerToExtensive(int valor)
        {
            string extenso = string.Empty;
            string separador = " e ";

            if (valor < 20)
            {
                extenso = RetorneValorString(valor);
            }

            if (valor > 19)
            {
                int len = valor.ToString().Length;

                if (len == 2)
                {
                    int ValorPrimario = int.Parse(valor.ToString().Substring(0, 1));
                    int ValorSecundario = int.Parse(valor.ToString().Substring(1, 1));
                    ValorPrimario = ValorPrimario * 10;
                    extenso = (RetorneValorString(ValorPrimario) + (ValorSecundario > 0 ? separador + RetorneValorString(ValorSecundario) : ""));
                }
                else if (len == 3)
                {
                    int ValorPrimario = int.Parse(valor.ToString().Substring(0, 1));
                    int ValorSecundario = int.Parse(valor.ToString().Substring(1, 1));
                    int ValorTerciario = int.Parse(valor.ToString().Substring(2, 1));

                    ValorPrimario = ValorPrimario * 100;
                    ValorSecundario = ValorSecundario * 10;

                    extenso = (RetorneValorString(ValorPrimario)
                                               + (ValorSecundario > 0 ? separador + RetorneValorString(ValorSecundario) : "")
                                               + (ValorTerciario > 0 ? separador + RetorneValorString(ValorTerciario) : ""));
                }
            }

            return extenso;
        }

        public static string RetorneValorString(int identificador)
        {
            switch (identificador)
            {
                case 1: return "Um";
                case 2: return "Dois";
                case 3: return "Tres";
                case 4: return "Quatro";
                case 5: return "Cinco";
                case 6: return "Seis";
                case 7: return "Sete";
                case 8: return "Oito";
                case 9: return "Nove";

                case 10: return "Dez";
                case 11: return "Onze";
                case 12: return "Doze";
                case 13: return "Treze";
                case 14: return "Quatorze";
                case 15: return "Quinze";
                case 16: return "Dezesseis";
                case 17: return "Dezessete";
                case 18: return "Dezoito";
                case 19: return "Dezenove";

                case 20: return "Vinte";
                case 30: return "Trinta";
                case 40: return "Quarenta";
                case 50: return "Cinquenta";
                case 60: return "Sessenta";
                case 70: return "Setenta";
                case 80: return "Oitenta";
                case 90: return "Noventa";

                case 100: return "Cem";
                case 200: return "Duzentos";
                case 300: return "Trezentos";
                case 400: return "Quatrocentos";
                case 500: return "Quinhentos";
                case 600: return "Seicentos";
                case 700: return "Setecentos";
                case 800: return "Oitocentos";
                case 900: return "Novecentos";

                default: return "Valor inválido";
            }

        }

        public static string DateByExtension(DateTime date)
        {
            CultureInfo culture = new CultureInfo("pt-BR");
            var day = date.Day;
            var month = date.ToString("MMMM");
            var year = date.Year;

            var extension = string.Format("{0} de {1} de {2}", AddLeadingZeros(day, 2), month, year);

            return extension;
        }

        public static string AddLeadingZeros(long value, int length)
        {
            var str = (value > 0 ? value : -value) + "";
            var zeros = "";
            for (var i = length - str.Length; i > 0; i--)
                zeros += "0";
            zeros += str;
            return value >= 0 ? zeros : "-" + zeros;
        }

        public static string EscreverExtenso(decimal valor)
        {
            if (valor <= 0 | valor >= 1000000000000000)
                return "Valor não suportado pelo sistema.";
            else
            {
                string strValor = valor.ToString("000000000000000.00");
                string valor_por_extenso = string.Empty;
                for (int i = 0; i <= 15; i += 3)
                {
                    valor_por_extenso += Escrever_Valor_Extenso(Convert.ToDecimal(strValor.Substring(i, 3)));
                    if (i == 0 & valor_por_extenso != string.Empty)
                    {
                        if (Convert.ToInt32(strValor.Substring(0, 3)) == 1)
                            valor_por_extenso += " TRILHÃO" + ((Convert.ToDecimal(strValor.Substring(3, 12)) > 0) ? " E " : string.Empty);
                        else if (Convert.ToInt32(strValor.Substring(0, 3)) > 1)
                            valor_por_extenso += " TRILHÕES" + ((Convert.ToDecimal(strValor.Substring(3, 12)) > 0) ? " E " : string.Empty);
                    }
                    else if (i == 3 & valor_por_extenso != string.Empty)
                    {
                        if (Convert.ToInt32(strValor.Substring(3, 3)) == 1)
                            valor_por_extenso += " BILHÃO" + ((Convert.ToDecimal(strValor.Substring(6, 9)) > 0) ? " E " : string.Empty);
                        else if (Convert.ToInt32(strValor.Substring(3, 3)) > 1)
                            valor_por_extenso += " BILHÕES" + ((Convert.ToDecimal(strValor.Substring(6, 9)) > 0) ? " E " : string.Empty);
                    }
                    else if (i == 6 & valor_por_extenso != string.Empty)
                    {
                        if (Convert.ToInt32(strValor.Substring(6, 3)) == 1)
                            valor_por_extenso += " MILHÃO" + ((Convert.ToDecimal(strValor.Substring(9, 6)) > 0) ? " E " : string.Empty);
                        else if (Convert.ToInt32(strValor.Substring(6, 3)) > 1)
                            valor_por_extenso += " MILHÕES" + ((Convert.ToDecimal(strValor.Substring(9, 6)) > 0) ? " E " : string.Empty);
                    }
                    else if (i == 9 & valor_por_extenso != string.Empty)
                        if (Convert.ToInt32(strValor.Substring(9, 3)) > 0)
                            valor_por_extenso += " MIL" + ((Convert.ToDecimal(strValor.Substring(12, 3)) > 0) ? " E " : string.Empty);
                    if (i == 12)
                    {
                        if (valor_por_extenso.Length > 8)
                            if (valor_por_extenso.Substring(valor_por_extenso.Length - 6, 6) == "BILHÃO" | valor_por_extenso.Substring(valor_por_extenso.Length - 6, 6) == "MILHÃO")
                                valor_por_extenso += " DE";
                            else
                                if (valor_por_extenso.Substring(valor_por_extenso.Length - 7, 7) == "BILHÕES" | valor_por_extenso.Substring(valor_por_extenso.Length - 7, 7) == "MILHÕES" | valor_por_extenso.Substring(valor_por_extenso.Length - 8, 7) == "TRILHÕES")
                                valor_por_extenso += " DE";
                            else
                                    if (valor_por_extenso.Substring(valor_por_extenso.Length - 8, 8) == "TRILHÕES")
                                valor_por_extenso += " DE";
                        if (Convert.ToInt64(strValor.Substring(0, 15)) == 1)
                            valor_por_extenso += " REAL";
                        else if (Convert.ToInt64(strValor.Substring(0, 15)) > 1)
                            valor_por_extenso += " REAIS";
                        if (Convert.ToInt32(strValor.Substring(16, 2)) > 0 && valor_por_extenso != string.Empty)
                            valor_por_extenso += " E ";
                    }
                    if (i == 15)
                        if (Convert.ToInt32(strValor.Substring(16, 2)) == 1)
                            valor_por_extenso += " CENTAVO";
                        else if (Convert.ToInt32(strValor.Substring(16, 2)) > 1)
                            valor_por_extenso += " CENTAVOS";
                }
                return valor_por_extenso;
            }
        }

        static string Escrever_Valor_Extenso(decimal valor)
        {
            if (valor <= 0)
                return string.Empty;
            else
            {
                string montagem = string.Empty;
                if (valor > 0 & valor < 1)
                {
                    valor *= 100;
                }
                string strValor = valor.ToString("000");
                int a = Convert.ToInt32(strValor.Substring(0, 1));
                int b = Convert.ToInt32(strValor.Substring(1, 1));
                int c = Convert.ToInt32(strValor.Substring(2, 1));
                if (a == 1) montagem += (b + c == 0) ? "CEM" : "CENTO";
                else if (a == 2) montagem += "DUZENTOS";
                else if (a == 3) montagem += "TREZENTOS";
                else if (a == 4) montagem += "QUATROCENTOS";
                else if (a == 5) montagem += "QUINHENTOS";
                else if (a == 6) montagem += "SEISCENTOS";
                else if (a == 7) montagem += "SETECENTOS";
                else if (a == 8) montagem += "OITOCENTOS";
                else if (a == 9) montagem += "NOVECENTOS";
                if (b == 1)
                {
                    if (c == 0) montagem += ((a > 0) ? " E " : string.Empty) + "DEZ";
                    else if (c == 1) montagem += ((a > 0) ? " E " : string.Empty) + "ONZE";
                    else if (c == 2) montagem += ((a > 0) ? " E " : string.Empty) + "DOZE";
                    else if (c == 3) montagem += ((a > 0) ? " E " : string.Empty) + "TREZE";
                    else if (c == 4) montagem += ((a > 0) ? " E " : string.Empty) + "QUATORZE";
                    else if (c == 5) montagem += ((a > 0) ? " E " : string.Empty) + "QUINZE";
                    else if (c == 6) montagem += ((a > 0) ? " E " : string.Empty) + "DEZESSEIS";
                    else if (c == 7) montagem += ((a > 0) ? " E " : string.Empty) + "DEZESSETE";
                    else if (c == 8) montagem += ((a > 0) ? " E " : string.Empty) + "DEZOITO";
                    else if (c == 9) montagem += ((a > 0) ? " E " : string.Empty) + "DEZENOVE";
                }
                else if (b == 2) montagem += ((a > 0) ? " E " : string.Empty) + "VINTE";
                else if (b == 3) montagem += ((a > 0) ? " E " : string.Empty) + "TRINTA";
                else if (b == 4) montagem += ((a > 0) ? " E " : string.Empty) + "QUARENTA";
                else if (b == 5) montagem += ((a > 0) ? " E " : string.Empty) + "CINQUENTA";
                else if (b == 6) montagem += ((a > 0) ? " E " : string.Empty) + "SESSENTA";
                else if (b == 7) montagem += ((a > 0) ? " E " : string.Empty) + "SETENTA";
                else if (b == 8) montagem += ((a > 0) ? " E " : string.Empty) + "OITENTA";
                else if (b == 9) montagem += ((a > 0) ? " E " : string.Empty) + "NOVENTA";
                if (strValor.Substring(1, 1) != "1" & c != 0 & montagem != string.Empty) montagem += " E ";
                if (strValor.Substring(1, 1) != "1")
                    if (c == 1) montagem += "UM";
                    else if (c == 2) montagem += "DOIS";
                    else if (c == 3) montagem += "TRÊS";
                    else if (c == 4) montagem += "QUATRO";
                    else if (c == 5) montagem += "CINCO";
                    else if (c == 6) montagem += "SEIS";
                    else if (c == 7) montagem += "SETE";
                    else if (c == 8) montagem += "OITO";
                    else if (c == 9) montagem += "NOVE";
                return montagem;
            }
        }

        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }


        public static string URLEncoding(string value)
        {
            value = value.Replace("%C7", "Ç");
            value = value.Replace("%E7", "ç");
            value = value.Replace("%E1", "á");
            value = value.Replace("%C1", "Á");
            value = value.Replace("%E9", "é");
            value = value.Replace("%C9", "É");
            value = value.Replace("%ED", "í");
            value = value.Replace("%CD", "Í");
            value = value.Replace("%F3", "ó");
            value = value.Replace("%D3", "Ó");
            value = value.Replace("%FA", "ú");
            value = value.Replace("%DA", "Ú");
            value = value.Replace("%F4", "ô");
            value = value.Replace("%D4", "Ô");
            value = value.Replace("%E3", "ã");
            value = value.Replace("%C3", "Ã");
            value = value.Replace("%F5", "õ");
            value = value.Replace("%D5", "Õ");
            value = value.Replace("%EA", "ê");
            value = value.Replace("%CA", "Ê");
            value = value.Replace("%E2", "â");
            value = value.Replace("%C2", "Â");

            return value;
        }

        public static string GerarToken()
        {
            var token = Guid.NewGuid().ToString();

            return token;
        }
    }

    public static class Mail
    {
        public static Boolean Send(String from, String Password, String to, String subject, String body)
        {
            return Send(from, Password, to, subject, body, null);
        }

        public static Boolean SendBoleto(String from, String Password, String to, String subject, String body, Object attach)
        {
            return Send(from, Password, to, subject, body, attach);
        }

        public static Boolean Send(String from, String Password, String to, String subject, String body, Object attach)
        {
            try
            {
                // Monta as Credenciais
                NetworkCredential credential = new NetworkCredential();
                credential.UserName = from;
                credential.Password = Password;
                //credential.UserName = "secretaria@rvmadvogados.com.br";
                //credential.Password = "ttwh pnge vwud uhco";
                //                credential.Password = "GDPB11@RVM";
                //credential.UserName = "contato@rvmadvogados.com.br";
                //credential.Password = "rvm92713070";
                //credential.Domain = "rvmadvogados.com.br";

                // Cria o Cliente SMTP:
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = true;
                smtp.EnableSsl = true;
                //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = credential;

                // Monta Mensagem:
                MailMessage message = new MailMessage();
                message.BodyEncoding = Encoding.UTF8;
                message.Priority = MailPriority.High;
                message.IsBodyHtml = true;
                message.Body = body;
                message.Subject = subject;
                MailAddressCollection mailTo = new MailAddressCollection();
                message.To.Add(to);
                message.From = new MailAddress(from, "RVM Advogados");

                // Anexar arquivo
                if (attach != null)
                    message.Attachments.Add((Attachment)attach);

                // Envia Mensagem:
                smtp.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static Boolean ValidateEmail(String email)
        {
            if (email.Length == 0)
                throw new ArgumentException("Um e-mail deve ser informado");

            string emailRegex = @"^(([^<>()[\]\\.,;áàãâäéèêëíìîïóòõôöúùûüç:\s@\""]+"
            + @"(\.[^<>()[\]\\.,;áàãâäéèêëíìîïóòõôöúùûüç:\s@\""]+)*)|(\"".+\""))@"
            + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|"
            + @"(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$";

            Regex rx = new Regex(emailRegex);
            return rx.IsMatch(email);
        }
    }

    public static class PerformanceInfo
    {
        [DllImport("psapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPerformanceInfo([Out] out PerformanceInformation PerformanceInformation, [In] int Size);

        [StructLayout(LayoutKind.Sequential)]
        public struct PerformanceInformation
        {
            public int Size;
            public IntPtr CommitTotal;
            public IntPtr CommitLimit;
            public IntPtr CommitPeak;
            public IntPtr PhysicalTotal;
            public IntPtr PhysicalAvailable;
            public IntPtr SystemCache;
            public IntPtr KernelTotal;
            public IntPtr KernelPaged;
            public IntPtr KernelNonPaged;
            public IntPtr PageSize;
            public int HandlesCount;
            public int ProcessCount;
            public int ThreadCount;
        }

        public static Int64 GetPhysicalAvailableMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalAvailable.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }
        }

        public static Int64 GetTotalMemoryInMiB()
        {
            PerformanceInformation pi = new PerformanceInformation();
            if (GetPerformanceInfo(out pi, Marshal.SizeOf(pi)))
            {
                return Convert.ToInt64((pi.PhysicalTotal.ToInt64() * pi.PageSize.ToInt64() / 1048576));
            }
            else
            {
                return -1;
            }
        }
    }

    public static class UtilCpfCnpj
    {
        public static bool IsValid(string cpfCnpj)
        {
            return (IsCpf(cpfCnpj) || IsCnpj(cpfCnpj));
        }

        private static bool IsCpf(string cpf)
        {
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            cpf = cpf.Trim().Replace(".", "").Replace("-", "");
            if (cpf.Length != 11)
                return false;

            for (int j = 0; j < 10; j++)
                if (j.ToString().PadLeft(11, char.Parse(j.ToString())) == cpf)
                    return false;

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            string digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cpf.EndsWith(digito);
        }

        private static bool IsCnpj(string cnpj)
        {
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            cnpj = cnpj.Trim().Replace(".", "").Replace("-", "").Replace("/", "");
            if (cnpj.Length != 14)
                return false;

            string tempCnpj = cnpj.Substring(0, 12);
            int soma = 0;

            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            int resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            string digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cnpj.EndsWith(digito);
        }
    }
}