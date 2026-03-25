using NReco.ImageGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Sigman.Common
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

        public static string OnlyNumbers(string number)
        {
            var digits = new Regex(@"[^\d]");
            return digits.Replace(number, "");
        }

        public static string IntervaloAnoMesPorExtenso(String DataInicial, String DataFinal)
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

        public static string GetNameUF(string UF)
        {
            var name = string.Empty;

            switch (UF)
            {
                case "AC":
                    name = "Acre";
                    break;

                case "AL":
                    name = "Alagoas";
                    break;

                case "AP":
                    name = "Amapá";
                    break;

                case "AM":
                    name = "Amazonas";
                    break;

                case "BA":
                    name = "Bahia";
                    break;

                case "CE":
                    name = "Ceará";
                    break;

                case "DF":
                    name = "Distrito Federal";
                    break;

                case "ES":
                    name = "Espírito Santo";
                    break;

                case "GO":
                    name = "Goiás";
                    break;

                case "MA":
                    name = "Maranhão";
                    break;

                case "MT":
                    name = "Mato Grosso";
                    break;

                case "MS":
                    name = "Mato Grosso do Sul";
                    break;

                case "MG":
                    name = "Minas Gerais";
                    break;

                case "PA":
                    name = "Pará";
                    break;

                case "PB":
                    name = "Paraíba";
                    break;

                case "PR":
                    name = "Paraná";
                    break;

                case "PE":
                    name = "Pernambuco";
                    break;

                case "PI":
                    name = "Piauí";
                    break;

                case "RJ":
                    name = "Rio de Janeiro";
                    break;

                case "RN":
                    name = "Rio Grande do Norte";
                    break;

                case "RS":
                    name = "Rio Grande do Sul";
                    break;

                case "RO":
                    name = "Rondônia";
                    break;

                case "RR":
                    name = "Roraima";
                    break;

                case "SC":
                    name = "Santa Catarina";
                    break;

                case "SP":
                    name = "São Paulo";
                    break;

                case "SE":
                    name = "Sergipe";
                    break;

                case "TO":
                    name = "Tocantins";
                    break;
            }

            return name;
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

                cep = number.ToString(@"00000-000");
            }

            return cep;
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
            value = value.Replace("%E0", "à");

            value = value.Replace("Ã§", "ç");
            value = value.Replace("Ã‡", "Ç");
            value = value.Replace("Ã¡", "á");
            value = value.Replace("Âº", "º");
            value = value.Replace("Ã³", "ó");
            value = value.Replace("Ã£", "ã");
            value = value.Replace("Ãƒ", "Ã");
            value = value.Replace("Ãº", "ú");
            value = value.Replace("Ã­", "í");
            value = value.Replace("Ã", "Í");
            value = value.Replace("Ãª", "ê");
            value = value.Replace("Ãµ", "õ");
            value = value.Replace("Í•", "Õ");
            return value;
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

        public static string GetLast(string s, int tail_length)
        {
            if (tail_length >= s.Length)
                return s;

            return s.Substring(s.Length - tail_length);
        }


        #region FuncoesPDF

        public static string GerarPDF(string htmlBase)
        {
            try
            {
                HtmlToImageConverter htmlToImageConv = new HtmlToImageConverter();
                var imageBytes = htmlToImageConv.GenerateImage(htmlBase, NReco.ImageGenerator.ImageFormat.Bmp.ToString());

                MemoryStream memoryStream = new MemoryStream();
                iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 23f, 23f, 20, 20f);

                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageBytes);
                image.ScaleToFit(820f, 1200f);
                image.SetDpi(300, 300);

                iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, memoryStream);
                document.Open();
                document.Add(image);
                document.Close();
                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();

                var base64 = Convert.ToBase64String(bytes);

                return base64;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static byte[] GerarPDFNReco(string htmlBase, int QuantidadeItems)
        {
            var conversorPDF = new NReco.PdfGenerator.HtmlToPdfConverter();

            conversorPDF.PageHeight = 230 + (QuantidadeItems * 6);
            conversorPDF.PageWidth = 130;

            conversorPDF.Margins = new NReco.PdfGenerator.PageMargins()
            {
                Bottom = 0,
                Left = 0,
                Right = 0,
                Top = 3,
            };

            var bytes = conversorPDF.GeneratePdf(htmlBase);

            return bytes;
        }
        #endregion
    }

    public static class Mail
    {
        //public static Boolean Send(String from, String to, String subject, String body, String name, String user, String pass)
        //{
        //    return Send(from, to, subject, body, name, user, pass, null);
        //}

        public static Boolean Send(String host, String to, String subject, String body, String name, String user, String pass, string port, List<Attachment> attach)
        {
            try
            {

                // Monta as Credenciais
//                NetworkCredential credential = new NetworkCredential("nfe@diluce.com.br", "Diluce!544");
//                NetworkCredential credential = new NetworkCredential("atendimento@artifex.com.br", "artifexfreios472");
                NetworkCredential credential = new NetworkCredential(user, pass);
                // 
                // Cria o Cliente SMTP:
                SmtpClient smtp = new SmtpClient();
//                smtp.Port = 587;
                smtp.Port = int.Parse(port);
                smtp.Host = host;
//                smtp.Host = "smtp.kinghost.net";


                smtp.Credentials = credential;
                smtp.EnableSsl = true;

                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                message.BodyEncoding = Encoding.UTF8;
                message.Priority = MailPriority.High;
                message.IsBodyHtml = true;
                message.Body = body;
                message.Subject = subject;
                MailAddressCollection mailTo = new MailAddressCollection();
                message.To.Add(to);
//                message.From = new MailAddress((!string.IsNullOrEmpty(from) ? from : credential.UserName), "NF_Diluce");
                message.From = new MailAddress(user, name);

                // Anexar arquivos
                if (attach != null)
                {
                    foreach (Attachment attachment in attach)
                    {
                        message.Attachments.Add(attachment);
                    }
                }

                // Envia Mensagem:
                smtp.Send(message);
                return true;

                //                // Monta as Credenciais
                //                NetworkCredential credential = new NetworkCredential();
                //                credential.UserName = user;
                //                credential.Password = pass;
                //                //credential.Domain = "rvmadvogados.com.br";

                //                // Cria o Cliente SMTP:
                //                SmtpClient smtp = new SmtpClient();
                //                smtp.Host = "smtp.kinghost.net";
                ////                smtp.Host = "smtp.gmail.com";
                //                smtp.Port = 465;
                ////                smtp.Port = 587;
                //                smtp.UseDefaultCredentials = true;
                //                smtp.EnableSsl = false;
                //                //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                //                smtp.Credentials = credential;

                //                // Monta Mensagem:
                //                MailMessage message = new MailMessage();
                //                message.BodyEncoding = Encoding.UTF8;
                //                message.Priority = MailPriority.High;
                //                message.IsBodyHtml = true;
                //                message.Body = body;
                //                message.Subject = subject;
                //                MailAddressCollection mailTo = new MailAddressCollection();
                //                message.To.Add(to);
                //                message.From = new MailAddress(from, name);

                //                // Anexar arquivos
                //                if (attach != null)
                //                {
                //                    foreach (Attachment attachment in attach)
                //                    {
                //                        message.Attachments.Add(attachment);
                //                    }
                //                }

                //                // Envia Mensagem:
                //                smtp.Send(message);
                //                return true;
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
}
