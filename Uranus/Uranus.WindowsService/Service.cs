using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceProcess;
using System.Threading;
using System.Timers;
using System.Xml;
using System.Xml.Linq;
using Uranus.Business;
using Uranus.Common;
using Uranus.Domain;
using Timer = System.Timers.Timer;
//using iTextSharp.text;
//using iTextSharp.text.pdf;

namespace Uranus.WindowsService
{
    public partial class Service : ServiceBase
    {
        public const int Retest = 11;

        private Timer _timerSpreadshee;
        private Timer _timerWebServiceWEBJUR;
        private Timer _timerWebServiceOAB;
        private Timer _timerEmails;

        bool executando = true;

        string Log = "Application";
        string Source = "Uranus.WindowsService";

        private List<Email> _emails = new List<Email>();
        private string _hostname = "pop.gmail.com"; // Host do seu servidor POP3. Por exemplo, pop.gmail.com para o servidor do Gmail.
        private int _port = 995; // Porta utilizada pelo host. Por exemplo, 995 para o servidor do Gmail.
        private bool _useSsl = true; // Indicação se o servidor POP3 utiliza SSL para autenticação. Por exemplo, o servidor do Gmail utiliza SSL, então, "true".
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string fullPath;

        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
#if (!DEBUG)
                using (EventLog eventLog = new EventLog(Log))
                {
                    eventLog.Source = Source;
                    eventLog.WriteEntry("Iniciando a aplicação.", EventLogEntryType.Information);
                }
#endif

            if (ConfigurationManager.AppSettings["ActivateSpreadsheet"] == "1")
                SetupProcessingTimerSpreadsheet();

            if (ConfigurationManager.AppSettings["ActivateWebServiceWEBJUR"] == "1")
                SetupProcessingTimerWebServiceWEBJUR();

            if (ConfigurationManager.AppSettings["ActivateWebServiceOAB"] == "1")
                SetupProcessingTimerWebServiceOAB();

            if (ConfigurationManager.AppSettings["ActivateEmails"] == "1")
                SetupProcessingTimerEmails();

            if (ConfigurationManager.AppSettings["ActivateDocumentosVencidos"] == "1")
                SetupProcessingTimerDocs();

            if (ConfigurationManager.AppSettings["ActivateAniversariantes"] == "1")
                SetupProcessingTimerAniversariantes();

            if (ConfigurationManager.AppSettings["ActivateAniversariantesCasa"] == "1")
                SetupProcessingTimerAniversariantesCasa();

            if (ConfigurationManager.AppSettings["ActivateGerarPeriodoAquisitivo"] == "1")
                SetupProcessingTimerPeriodoAquisitivo();

            //if (ConfigurationManager.AppSettings["ActivateGerarBoleto"] == "1")
            //    SetupProcessingTimerGerarBoleto();
        }

        protected override void OnStop()
        {
            using (EventLog eventLog = new EventLog(Log))
            {
                eventLog.Source = Source;
                eventLog.WriteEntry("Finalizando a aplicação.", EventLogEntryType.Information);
            }
        }

        #region Planilhas E-PROC
        private void SetupProcessingTimerSpreadsheet()
        {
            _timerSpreadshee = new Timer(int.Parse(ConfigurationManager.AppSettings["TimeSpreadsheet"]) * 60000);
            _timerSpreadshee.AutoReset = true;
            _timerSpreadshee.Enabled = true;
            _timerSpreadshee.Elapsed += new ElapsedEventHandler(OnTimedEventSpreadsheet);
            _timerSpreadshee.Start();
        }

        private void OnTimedEventSpreadsheet(object source, ElapsedEventArgs e)
        {
            ReadSpreadsheet();
        }

        public void ReadSpreadsheet()
        {
            try
            {
                string path = ConfigurationManager.AppSettings["PathUpload"];
                string target = @"Planilhas";
                string targetPath = String.Format(@"{0}\{1}", path, target);

                string targetNew = String.Format(@"{0}\{1}", target, "Novas");
                string targetProcessing = String.Format(@"{0}\{1}", target, "Processando");
                string targetFinished = String.Format(@"{0}\{1}", target, "Finalizadas");
                string targetError = String.Format(@"{0}\{1}", target, "Erradas");

                string targetFullPath = String.Format(@"{0}\{1}", path, targetNew);

                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                    Directory.CreateDirectory(String.Format(@"{0}\{1}", targetPath, "Novas"));
                    Directory.CreateDirectory(String.Format(@"{0}\{1}", targetPath, "Processando"));
                    Directory.CreateDirectory(String.Format(@"{0}\{1}", targetPath, "Finalizadas"));
                    Directory.CreateDirectory(String.Format(@"{0}\{1}", targetPath, "Erradas"));
                }

                int countProcessing = Directory.GetFiles(String.Format(@"{0}\{1}", path, targetProcessing), "*", SearchOption.AllDirectories).Length;

                if (countProcessing == 0)
                {
                    int countNew = Directory.GetFiles(String.Format(@"{0}\{1}", path, targetNew), "*", SearchOption.AllDirectories).Length;

                    if (countNew > 0)
                    {
                        FileSystemInfo fileInfo = new DirectoryInfo(targetFullPath).GetFileSystemInfos().OrderBy(file => file.CreationTime).First();

                        File.Move(fileInfo.FullName, String.Format(@"{0}\{1}\{2}", path, targetProcessing, fileInfo.Name)); // Try to move

                        var response = SetFileToDataBase(String.Format(@"{0}\{1}\{2}", path, targetProcessing, fileInfo.Name));

                        if (response == "Success")
                        {
                            File.Move(String.Format(@"{0}\{1}\{2}", path, targetProcessing, fileInfo.Name), String.Format(@"{0}\{1}\{2}", path, targetFinished, fileInfo.Name));
                        }
                        else
                        {
                            File.Move(String.Format(@"{0}\{1}\{2}", path, targetProcessing, fileInfo.Name), String.Format(@"{0}\{1}\{2}", path, targetError, fileInfo.Name));
                        }
                    }
                }
                else
                {
                    FileSystemInfo fileInfo = new DirectoryInfo(String.Format(@"{0}\{1}", path, targetProcessing)).GetFileSystemInfos().OrderBy(file => file.CreationTime).First();

                    var response = SetFileToDataBase(String.Format(@"{0}\{1}\{2}", path, targetProcessing, fileInfo.Name));

                    if (response == "Success")
                    {
                        File.Move(String.Format(@"{0}\{1}\{2}", path, targetProcessing, fileInfo.Name), String.Format(@"{0}\{1}\{2}", path, targetFinished, fileInfo.Name));
                    }
                    else
                    {
                        File.Move(String.Format(@"{0}\{1}\{2}", path, targetProcessing, fileInfo.Name), String.Format(@"{0}\{1}\{2}", path, targetError, fileInfo.Name));
                    }
                }
            }
            catch (Exception ex)
            {
#if (!DEBUG)
                    using (EventLog eventLog = new EventLog(Log))
                    {
                        eventLog.Source = Source;
                        eventLog.WriteEntry(String.Format("Método ReadSpreadsheet, Erro: {0}", ex.Message), EventLogEntryType.Error);
                    }
#endif
            }
        }

        protected string SetFileToDataBase(string file)
        {
            var response = string.Empty;

            try
            {
                using (var stream = File.Open(file, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream))
                    {
                        do
                        {
                            int line = 0;
                            object[] item;
                            ProcessosEventosPendentes pendente;
                            var pendentes = new System.Collections.Generic.List<ProcessosEventosPendentes>();

                            while (reader.Read()) //Each ROW
                            {
                                if (line >= 2)
                                {
                                    if (reader.GetValue(0) == null)
                                    {
                                        break;
                                    }

                                    item = new object[9];

                                    for (int column = 0; column < reader.FieldCount; column++)
                                    {
                                        item[column] = reader.GetValue(column);
                                    }

                                    if (item.Length > 0)
                                    {
                                        var DataEnvioRequisicao = item[3].ToString();
                                        var DataPrazoInicio = item[4].ToString();
                                        var DataPrazoFinal = item[5].ToString();

#if (!DEBUG)
                                        if (Util.IsDate(DataEnvioRequisicao))
                                        {
                                            DataEnvioRequisicao = DateTime.Parse(item[3].ToString()).ToString("dd/MM/yyyy HH:mm:ss");
                                        }

                                        if (Util.IsDate(DataPrazoInicio))
                                        {
                                            DataPrazoInicio = DateTime.Parse(item[4].ToString()).ToString("dd/MM/yyyy HH:mm:ss");
                                        }

                                        if (Util.IsDate(DataPrazoFinal))
                                        {
                                            DataPrazoFinal = DateTime.Parse(item[5].ToString()).ToString("dd/MM/yyyy HH:mm:ss");
                                        }
#endif

                                        pendente = new ProcessosEventosPendentes();
                                        pendente.Id = 0;
                                        pendente.Origem = "E-PROC";
                                        pendente.Integrado = false;
                                        pendente.Processo = item[0].ToString();
                                        //pendente.Orgao = item[1].ToString();
                                        pendente.Partes = item[1].ToString().Replace("\n\r", " ");
                                        //pendente.Classe = item[3].ToString();
                                        //pendente.Assunto = item[4].ToString();
                                        pendente.EventoPrazo = item[2].ToString();
                                        pendente.DataEnvioRequisicao = DataEnvioRequisicao;
                                        pendente.DataPrazoInicio = DataPrazoInicio;
                                        pendente.DataPrazoFinal = DataPrazoFinal;

                                        pendentes.Add(pendente);
                                    }
                                }

                                line++;
                            }

                            var distinctItems = pendentes
                                .GroupBy(p => new
                                {
                                    p.Origem,
                                    p.Processo,
                                    p.Orgao,
                                    p.Partes,
                                    p.Classe,
                                    p.Assunto,
                                    p.EventoPrazo,
                                    p.DataEnvioRequisicao,
                                    p.DataPrazoInicio,
                                    p.DataPrazoFinal
                                })
                                .Select(c => c.First()).ToList();

                            response = ProcessosEventosPendentesBo.Inserir("E-PROC", distinctItems);

                        } while (reader.NextResult()); //Move to NEXT SHEET
                    }
                }
            }
            catch (Exception ex)
            {
#if (!DEBUG)
                    using (EventLog eventLog = new EventLog(Log))
                    {
                        eventLog.Source = Source;
                        eventLog.WriteEntry(String.Format("Método SetDataBase, Erro: {0}", ex.Message), EventLogEntryType.Error);
                    }
#endif
            }

            return response;
        }
        #endregion

        #region WebService WEBJUR
        private void SetupProcessingTimerWebServiceWEBJUR()
        {
            _timerWebServiceWEBJUR = new Timer(int.Parse(ConfigurationManager.AppSettings["TimeWebServiceWEBJUR"]) * 60000);
            _timerWebServiceWEBJUR.AutoReset = true;
            _timerWebServiceWEBJUR.Enabled = true;
            _timerWebServiceWEBJUR.Elapsed += new ElapsedEventHandler(OnTimedEventWebServiceWEBJUR);
            _timerWebServiceWEBJUR.Start();
        }

        private void OnTimedEventWebServiceWEBJUR(object source, ElapsedEventArgs e)
        {
            ReadWebServiceWEBJUR();
        }

        /// <summary>
        /// Execute a Soap WebService call
        /// </summary>
        public void ReadWebServiceWEBJUR()
        {
            try
            {
                HttpWebRequest request = CreateWebRequestWEBJUR();

                var envelope = String.Empty;
                envelope += "<?xml version=" + '"' + "1.0" + '"' + " encoding=" + '"' + "utf-8" + '"' + "?>";
                envelope += "<soap:Envelope xmlns:xsi=" + '"' + "http://www.w3.org/2001/XMLSchema-instance" + '"' + " xmlns:xsd=" + '"' + "http://www.w3.org/2001/XMLSchema" + '"' + " xmlns:soap=" + '"' + "http://schemas.xmlsoap.org/soap/envelope/" + '"' + ">";
                envelope += "    <soap:Body>";
                envelope += "        <getPublicacoesNaoExportadas xmlns=" + '"' + "http://tempuri.org/" + '"' + ">";
                envelope += "            <strUsuario>" + ConfigurationManager.AppSettings["WebServiceUserWEBJUR"] + "</strUsuario>";
                envelope += "            <strSenha>" + ConfigurationManager.AppSettings["WebServicePassWEBJUR"] + "</strSenha>";
                envelope += "            <intCodGrupo>" + ConfigurationManager.AppSettings["WebServiceGroupWEBJUR"] + "</intCodGrupo>";
                envelope += "        </getPublicacoesNaoExportadas>";
                envelope += "    </soap:Body>";
                envelope += "</soap:Envelope>";

                XmlDocument soapEnvelopeXml = new XmlDocument();
                soapEnvelopeXml.LoadXml(String.Format(@"{0}", envelope));

                using (Stream stream = request.GetRequestStream())
                {
                    soapEnvelopeXml.Save(stream);
                }

                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        string soapResult = rd.ReadToEnd();

                        XDocument xDoc = XDocument.Load(new StringReader(soapResult));
                        var xml = (xDoc.Descendants((XNamespace)"http://schemas.xmlsoap.org/soap/envelope/" + "Body")
                                       .First()
                                       .FirstNode)
                                       .ToString()
                                       .Replace(@"<getPublicacoesNaoExportadasResponse xmlns=""http://tempuri.org/"">", string.Empty)
                                       .Replace("</getPublicacoesNaoExportadasResponse>", string.Empty);

                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(xml);

                        var publications = SetXmlToDataBase(xmlDoc);

#if (!DEBUG)
                            if (publications.Length > 0 && publications != "Error")
                            {
                                WriteWebService(publications);
                            }
#endif
                    }
                }
            }
            catch (Exception ex)
            {
#if (!DEBUG)
                    using (EventLog eventLog = new EventLog(Log))
                    {
                        eventLog.Source = Source;
                        eventLog.WriteEntry(String.Format("Método ReadWebService, Erro: {0}", ex.Message), EventLogEntryType.Error);
                    }
#endif

                for (int i = 0; i < Retest; i++)
                {
                    Thread.Sleep(7200000);
                    ReadWebServiceWEBJUR();
                }
            }
        }

        protected string SetXmlToDataBase(XmlDocument xmlDoc)
        {
            var response = string.Empty;

            try
            {
                ProcessosEventosPendentes pendente;
                var pendentes = new System.Collections.Generic.List<ProcessosEventosPendentes>();
                var auditoria = String.Empty;

                XmlDocument xmlDocAux = new XmlDocument();

                XmlElement root = xmlDoc.DocumentElement;
                XmlNodeList elemList = root.GetElementsByTagName("publicacao");
                for (int i = 0; i < elemList.Count; i++)
                {
                    xmlDocAux.LoadXml("<publicacao>" + elemList[i].InnerXml.ToString() + "</publicacao>");

                    XmlNodeList orgFields = xmlDocAux.SelectNodes("//publicacao");

                    LogPrazos logprazos = new LogPrazos();
                    logprazos.Data = DateTime.Now;
                    logprazos.Origem = "WEBJUR";
                    logprazos.Texto = orgFields.ToString();
                    var Id =  ProcessosEventosPendentesBo.InserirLog(logprazos);

                    foreach (XmlNode org in orgFields)
                    {
                        pendente = new ProcessosEventosPendentes();
                        pendente.Id = 0;
                        pendente.Origem = "WEBJUR";
                        pendente.Integrado = false;
                        pendente.anoPublicacao = org.ChildNodes[0].InnerText.Trim();
                        pendente.codPublicacao = org.ChildNodes[1].InnerText.Trim();
                        pendente.edicaoDiario = org.ChildNodes[2].InnerText.Trim();
                        pendente.descricaoDiario = org.ChildNodes[3].InnerText.Trim();
                        pendente.paginaInicial = org.ChildNodes[4].InnerText.Trim();
                        pendente.paginaFinal = org.ChildNodes[5].InnerText.Trim();
                        pendente.dataPublicacao = org.ChildNodes[6].InnerText.Trim();
                        pendente.dataDivulgacao = org.ChildNodes[7].InnerText.Trim();
                        pendente.dataCadastro = org.ChildNodes[8].InnerText.Trim();
                        pendente.numeroProcesso = org.ChildNodes[9].InnerText.Trim();
                        pendente.ufPublicacao = org.ChildNodes[10].InnerText.Trim();
                        pendente.cidadePublicacao = org.ChildNodes[11].InnerText.Trim();
                        pendente.orgaoDescricao = org.ChildNodes[12].InnerText.Trim();
                        pendente.varaDescricao = org.ChildNodes[13].InnerText.Trim();
                        pendente.despachoPublicacao = org.ChildNodes[14].InnerText.Trim();
                        pendente.processoPublicacao = org.ChildNodes[15].InnerText.Trim();
                        pendente.publicacaoCorrigida = org.ChildNodes[16].InnerText.Trim();
                        pendente.codVinculo = org.ChildNodes[17].InnerText.Trim();
                        pendente.nomeVinculo = org.ChildNodes[18].InnerText.Trim();
                        pendente.OABNumero = org.ChildNodes[19].InnerText.Trim();
                        pendente.OABEstado = org.ChildNodes[20].InnerText.Trim();

                        pendentes.Add(pendente);

                        WriteWebServiceAudit(String.Format("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};{20};{21};{22};{23}",
                            org.ChildNodes[0].InnerText.Trim(),
                            org.ChildNodes[1].InnerText.Trim(),
                            org.ChildNodes[2].InnerText.Trim(),
                            org.ChildNodes[3].InnerText.Trim(),
                            org.ChildNodes[4].InnerText.Trim(),
                            org.ChildNodes[5].InnerText.Trim(),
                            org.ChildNodes[6].InnerText.Trim(),
                            org.ChildNodes[7].InnerText.Trim(),
                            org.ChildNodes[8].InnerText.Trim(),
                            org.ChildNodes[9].InnerText.Trim(),
                            org.ChildNodes[10].InnerText.Trim(),
                            org.ChildNodes[11].InnerText.Trim(),
                            org.ChildNodes[12].InnerText.Trim(),
                            org.ChildNodes[13].InnerText.Trim(),
                            org.ChildNodes[14].InnerText.Trim(),
                            org.ChildNodes[15].InnerText.Trim(),
                            org.ChildNodes[16].InnerText.Trim(),
                            org.ChildNodes[17].InnerText.Trim(),
                            org.ChildNodes[18].InnerText.Trim(),
                            org.ChildNodes[19].InnerText.Trim(),
                            org.ChildNodes[20].InnerText.Trim(),
                            org.ChildNodes[21].InnerText.Trim(),
                            org.ChildNodes[22].InnerText.Trim(),
                            org.ChildNodes[23].InnerText.Trim()
                        ));
                    }

                }

                var distinctItems = pendentes
                    .GroupBy(p => new
                    {
                        p.Origem,
                        p.anoPublicacao,
                        p.codPublicacao,
                        p.edicaoDiario,
                        p.descricaoDiario,
                        p.paginaInicial,
                        p.paginaFinal,
                        p.dataPublicacao,
                        p.dataDivulgacao,
                        p.dataCadastro,
                        p.numeroProcesso,
                        p.ufPublicacao,
                        p.cidadePublicacao,
                        p.orgaoDescricao,
                        p.varaDescricao,
                        p.despachoPublicacao,
                        p.processoPublicacao,
                        p.codVinculo,
                        p.nomeVinculo,
                        p.OABNumero,
                        p.OABEstado
                    })
                    .Select(c => c.First()).ToList();

                response = ProcessosEventosPendentesBo.Inserir("WEBJUR", distinctItems);

            }
            catch (Exception ex)
            {
#if (!DEBUG)
                    using (EventLog eventLog = new EventLog(Log))
                    {
                        eventLog.Source = Source;
                        eventLog.WriteEntry(String.Format("Método SetDataBase, Erro: {0}", ex.Message), EventLogEntryType.Error);
                    }
#endif
            }

            return response;
        }

        public void WriteWebService(string publications)
        {
            HttpWebRequest request = CreateWebResponseWEBJUR();

            var envelope = String.Empty;
            envelope += "<?xml version=" + '"' + "1.0" + '"' + " encoding=" + '"' + "utf-8" + '"' + "?>";
            envelope += "<soap:Envelope xmlns:xsi=" + '"' + "http://www.w3.org/2001/XMLSchema-instance" + '"' + " xmlns:xsd=" + '"' + "http://www.w3.org/2001/XMLSchema" + '"' + " xmlns:soap=" + '"' + "http://schemas.xmlsoap.org/soap/envelope/" + '"' + ">";
            envelope += "    <soap:Body>";
            envelope += "        <setPublicacoes xmlns=" + '"' + "http://tempuri.org/" + '"' + ">";
            envelope += "            <strUsuario>" + ConfigurationManager.AppSettings["WebServiceUserWEBJUR"] + "</strUsuario>";
            envelope += "            <strSenha>" + ConfigurationManager.AppSettings["WebServicePassWEBJUR"] + "</strSenha>";
            envelope += "            <strPublicacoes>" + publications + "</strPublicacoes>";
            envelope += "        </setPublicacoes>";
            envelope += "    </soap:Body>";
            envelope += "</soap:Envelope>";

            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(String.Format(@"{0}", envelope));

            using (Stream stream = request.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    string soapResult = rd.ReadToEnd();

#if (!DEBUG)
                        using (EventLog eventLog = new EventLog(Log))
                        {
                            eventLog.Source = Source;
                            eventLog.WriteEntry(String.Format("Método ReadWebService, Erro: {0}", soapResult), EventLogEntryType.Information);
                        }
#endif
                }
            }
        }

        public void WriteWebServiceAudit(string auditoria)
        {
            string path = ConfigurationManager.AppSettings["PathUpload"];
            string target = @"ServicosWeb";
            string targetPath = String.Format(@"{0}\{1}", path, target);

            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            string file = String.Format(@"{0}\SERVICOSWEB_WEBJUR_{1}.csv", targetPath, DateTime.Now.ToString("yyyyMMdd"));

            // This text is added only once to the file.
            if (!File.Exists(file))
            {
                // Create a file to write to.
                string createText = "anoPublicacao;codPublicacao;edicaoDiario;descricaoDiario;paginaInicial;paginaFinal;dataPublicacao;dataDivulgacao;dataCadastro;numeroProcesso;ufPublicacao;cidadePublicacao;orgaoDescricao;varaDescricao;despachoPublicacao;processoPublicacao;codVinculo;nomeVinculo;OABNumero;OABEstado;identificacaoCadastro;codIntegracao;publicacaoExportada" + Environment.NewLine;
                File.WriteAllText(file, createText);
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            string appendText = auditoria + Environment.NewLine;
            File.AppendAllText(file, appendText);
        }

        /// <summary>
        /// Create a soap webrequest to [Url]
        /// </summary>
        /// <returns></returns>
        public static HttpWebRequest CreateWebRequestWEBJUR()
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(@"http://webjur.com.br/WebjurServices/wsPublicacao.asmx?op=getPublicacoesNaoExportadas");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        public static HttpWebRequest CreateWebResponseWEBJUR()
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(@"http://webjur.com.br/WebjurServices/wsPublicacao.asmx?op=setPublicacoes");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }
        #endregion

        #region WebService OAB
        private void SetupProcessingTimerWebServiceOAB()
        {
            _timerWebServiceOAB = new Timer(int.Parse(ConfigurationManager.AppSettings["TimeWebServiceOAB"]) * 60000);
            _timerWebServiceOAB.AutoReset = true;
            _timerWebServiceOAB.Enabled = true;
            _timerWebServiceOAB.Elapsed += new ElapsedEventHandler(OnTimedEventWebServiceOAB);
            _timerWebServiceOAB.Start();
        }

        private void OnTimedEventWebServiceOAB(object source, ElapsedEventArgs e)
        {
            ReadWebServiceOAB();
        }

        /// <summary>
        /// Execute a Soap WebService call
        /// </summary>
        public void ReadWebServiceOAB()
        {
            try
            {
                var profissionais = ProfissionaisBo.Listar();

                var chave = ConfigurationManager.AppSettings["WebServiceKeyOAB"];
                var data = ConfigurationManager.AppSettings["WebServiceInitialDate"];
                var dataInicial = (data.Length > 0 ? ConfigurationManager.AppSettings["WebServiceInitialDate"] : DateTime.Now.AddDays(-1).ToString("yyyyMMdd"));
                var dataFinal = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");

                foreach (var item in profissionais)
                {
                    if (item.OAB != null && Util.IsNumeric(item.OAB))
                    {
                        var oab = string.Format("{0:000000}", long.Parse(Util.OnlyNumbers(item.OAB)));
                        foreach (int tribunal in Enum.GetValues(typeof(Tribunais)))
                        {
                            HttpWebRequest request = CreateWebRequestOAB();

                            var envelope = String.Empty;
                            envelope += "<?xml version=" + '"' + "1.0" + '"' + " encoding=" + '"' + "utf-8" + '"' + "?>";
                            envelope += "<soap:Envelope xmlns:xsi=" + '"' + "http://www.w3.org/2001/XMLSchema-instance" + '"' + " xmlns:xsd=" + '"' + "http://www.w3.org/2001/XMLSchema" + '"' + " xmlns:soap=" + '"' + "http://schemas.xmlsoap.org/soap/envelope/" + '"' + ">";
                            envelope += "    <soap:Body>";
                            envelope += "        <wsNotasExpedienteAdvogadoTribunal xmlns=" + '"' + "http://microsoft.com/webservices/" + '"' + ">";
                            envelope += "            <numeroOAB>" + oab + "</numeroOAB>";
                            envelope += "            <idTribunal>" + tribunal + "</idTribunal>";
                            envelope += "            <dtInicial>" + dataInicial + "</dtInicial>";
                            envelope += "            <dtFinal>" + dataFinal + "</dtFinal>";
                            envelope += "            <chave>" + chave + "</chave>";
                            envelope += "        </wsNotasExpedienteAdvogadoTribunal>";
                            envelope += "    </soap:Body>";
                            envelope += "</soap:Envelope>";

                            XmlDocument soapEnvelopeXml = new XmlDocument();
                            soapEnvelopeXml.LoadXml(String.Format(@"{0}", envelope));

                            using (Stream stream = request.GetRequestStream())
                            {
                                soapEnvelopeXml.Save(stream);
                            }

                            using (WebResponse response = request.GetResponse())
                            {
                                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                                {
                                    string soapResult = rd.ReadToEnd();

                                    XDocument xDoc = XDocument.Load(new StringReader(soapResult));
                                    var json = (xDoc.Descendants((XNamespace)"http://schemas.xmlsoap.org/soap/envelope/" + "Body")
                                                   .First()
                                                   .FirstNode)
                                                   .ToString()
                                                   .Replace(@"<wsNotasExpedienteAdvogadoTribunalResult>", string.Empty)
                                                   .Replace("</wsNotasExpedienteAdvogadoTribunalResult>", string.Empty)
                                                   .Replace(@"<wsNotasExpedienteAdvogadoTribunalResponse xmlns=""http://microsoft.com/webservices/"">", string.Empty)
                                                   .Replace("</wsNotasExpedienteAdvogadoTribunalResponse>", string.Empty)
                                                   .Replace("\r\n", string.Empty).Trim();

                                    List<ProcessosOAB> processos = JsonConvert.DeserializeObject<List<ProcessosOAB>>(json);
                                    List<ProcessosEventosPendentes> pendentes = new List<ProcessosEventosPendentes>();

                                    LogPrazos logprazos = new LogPrazos();
                                    logprazos.Data = DateTime.Now;
                                    logprazos.Origem = "OAB";
                                    logprazos.Texto = processos.ToString();
                                    var Id = ProcessosEventosPendentesBo.InserirLog(logprazos);

                                    foreach (var processo in processos)
                                    {
                                        ProcessosEventosPendentes pendente = new ProcessosEventosPendentes();

                                        pendente.Id = 0;
                                        pendente.Origem = "OAB";
                                        pendente.Integrado = false;

                                        var numero = processo.numeroProcesso.Replace("(CNJ)", "").Replace("AIRO", "").Replace("(ELETRÔNICO)", "").Replace("Pedido de Esclarecimento", "").Replace("Precatório", "").Replace("AIRR", "").Replace("PROCESSO", "").Replace("Nº", "").Replace("ATOrd", "").Replace("AP", "").Replace("ATSum", "").Replace("ConPag", "").Replace(":", "").Replace("PetCiv", "").Replace("RORSum", "").Replace("ROT", "").Replace("RR", "").Replace("P", "").Replace("(AUTA 433)", "").Replace("RS", "").Replace("rocesso", "").Replace("048) ", "").Replace("Ag", "").Trim();
                                        var index = numero.IndexOf(" (");
                                        pendente.numeroProcesso = Util.OnlyNumbers(numero.Substring((index >= 0 ? index : 0)).Trim());

                                        pendente.comarca = processo.comarca;
                                        pendente.vara = processo.vara;
                                        pendente.tituloAcao = processo.tituloAcao;
                                        pendente.numeroNota = processo.numeroNota;
                                        pendente.ata = processo.ata;
                                        pendente.descricao = processo.descricao;
                                        pendente.dtdisponibilizacao = processo.dtdisponibilizacao;
                                        pendente.IdAdvogadoStatus = processo.IdAdvogadoStatus;
                                        pendente.NumeroProcessoOriginal = processo.numeroProcesso;

                                        pendentes.Add(pendente);
                                    }

                                    if (pendentes.Count > 0)
                                    {
                                        ProcessosEventosPendentesBo.Inserir("OAB", pendentes);
                                    }
                                }
                            }

                        }
                    }

                }

                if (data.Length > 0)
                {
                    AddOrUpdateAppSettings("WebServiceInitialDate", "");
                }
            }
            catch (Exception ex)
            {
#if (!DEBUG)
                    using (EventLog eventLog = new EventLog(Log))
                    {
                        eventLog.Source = Source;
                        eventLog.WriteEntry(String.Format("Método ReadWebService, Erro: {0}", ex.Message), EventLogEntryType.Error);
                    }
#endif

                for (int i = 0; i < Retest; i++)
                {
                    Thread.Sleep(7200000);
                    ReadWebServiceOAB();
                }
            }
        }

        public static HttpWebRequest CreateWebRequestOAB()
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(@"https://wsoab.oabrs.org.br/wsoabexterno/Service.asmx?op=wsNotasExpedienteAdvogadoTribunal");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        public enum Tribunais
        {
            [Description("Justiça Estadual/RS")]
            JusticaEstadual = 1,
            [Description("Justiça do Trabalho/RS")]
            JusticadoTrabalho = 2,
            [Description("Superior Tribunal de Justiça(RS)")]
            SuperiorTribunaldeJustica = 3,
            [Description("Tribunal Regional Federal 4ª Região(RS, SC e PR)")]
            TribunalRegionalFederal4Regiao = 4,
            [Description("Tribunal Superior do Trabalho(RS)")]
            TribunalSuperiordoTrabalho = 5,
            [Description("Supremo Tribunal Federal(RS)")]
            SupremoTribunalFederal = 6,
            [Description("Justiça Federal - RS")]
            JusticaFederal = 7,
            [Description("Tribunal de Justiça Militar - RS")]
            TribunaldeJusticaMilitar = 8,
            [Description("Tribunal Regional Eleitoral - RS")]
            TribunalRegionalEleitoral = 9
        }

        public class ProcessosOAB
        {
            public string comarca { get; set; }
            public string vara { get; set; }
            public string tituloAcao { get; set; }
            public string numeroNota { get; set; }
            public string numeroProcesso { get; set; }
            public string ata { get; set; }
            public string descricao { get; set; }
            public string dtdisponibilizacao { get; set; }
            public string IdAdvogadoStatus { get; set; }
        }

        public static void AddOrUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
        #endregion

        #region E-Mails OAB
        private void SetupProcessingTimerEmails()
        {
            _timerEmails = new Timer(int.Parse(ConfigurationManager.AppSettings["TimeEmails"]) * 60000);
            _timerEmails.AutoReset = true;
            _timerEmails.Enabled = true;
            _timerEmails.Elapsed += new ElapsedEventHandler(OnTimedEventEmails);
            _timerEmails.Start();
        }

        private void OnTimedEventEmails(object source, ElapsedEventArgs e)
        {
            ReadEmails();
        }

        public void ReadEmails()
        {
            try
            {
                using (var client = new OpenPop.Pop3.Pop3Client())
                {
                    _username = string.Format("recent:{0}", ConfigurationManager.AppSettings["EmailUser"]); // Usuário do servidor POP3. Por exemplo, seuemail@gmail.com.
                    _password = ConfigurationManager.AppSettings["EmailPass"]; // Senha do servidor POP3.

                    client.Connect(_hostname, _port, _useSsl);
                    client.Authenticate(_username, _password);
                    int messageCount = client.GetMessageCount();
                    _emails.Clear();

                    for (int i = messageCount; i > 0; i--)
                    {
                        var popEmail = client.GetMessage(i);

                        if (popEmail.Headers.Subject.Contains("[Teste]"))
                        {
                            var popText = popEmail.FindFirstPlainTextVersion();
                            var popHtml = popEmail.FindFirstHtmlVersion();

                            string mailText = string.Empty;
                            string mailHtml = string.Empty;
                            if (popText != null)
                                mailText = popText.GetBodyAsText();
                            if (popHtml != null)
                                mailHtml = popHtml.GetBodyAsText();

                            _emails.Add(new Email()
                            {
                                Id = popEmail.Headers.MessageId,
                                Assunto = popEmail.Headers.Subject,
                                De = popEmail.Headers.From.Address,
                                Para = string.Join("; ", popEmail.Headers.To.Select(to => to.Address)),
                                Data = popEmail.Headers.DateSent,
                                ConteudoTexto = mailText,
                                ConteudoHtml = !string.IsNullOrWhiteSpace(mailHtml) ? mailHtml : mailText
                            });
                        }
                    }

                    var distinctItems = _emails
                        .GroupBy(p => new
                        {
                            p.Assunto,
                            p.De,
                            p.Para,
                            p.ConteudoTexto,
                            p.ConteudoHtml
                        })
                        .Select(d => d.First()).OrderBy(d => d.Data).ToList();

                }
            }
            catch (Exception ex)
            {
#if (!DEBUG)
                    using (EventLog eventLog = new EventLog(Log))
                    {
                        eventLog.Source = Source;
                        eventLog.WriteEntry(String.Format("Método ReadSpreadsheet, Erro: {0}", ex.Message), EventLogEntryType.Error);
                    }
#endif
            }
        }

        #region Entity
        public class Email
        {
            public string Id { get; set; }
            public string Assunto { get; set; }
            public string De { get; set; }
            public string Para { get; set; }
            public DateTime Data { get; set; }
            public string ConteudoTexto { get; set; }
            public string ConteudoHtml { get; set; }
        }
        #endregion
        #endregion

        #region Documentos Vencidos
        private void SetupProcessingTimerDocs()
        {
            _timerEmails = new Timer(int.Parse(ConfigurationManager.AppSettings["TimeDocumentos"]) * 55);
            _timerEmails.AutoReset = true;
            _timerEmails.Enabled = true;
            _timerEmails.Elapsed += new ElapsedEventHandler(TimeDocumentos);
            _timerEmails.Start();
        }

        private void TimeDocumentos(object source, ElapsedEventArgs e)
        {
            var datahora = DateTime.Now;
            if (datahora.Day == 1 && datahora.Hour == 6 && datahora.Minute == 0 && executando)
            {
                executando = false;
                ReadDocumentos();
                executando = true;
            }
        }

        public void ReadDocumentos()
        {
            try
            {
                int ano = DateTime.Now.Year;
                int mes = DateTime.Now.Month;

                var documentosvencidos = ProfissionaisDocumentosBo.ListarDocumentosVencidos(ano, mes);
                if (documentosvencidos.Count > 0)
                {
                    string HTMLContent = string.Empty;
                    HTMLContent += "  <HTML>";
                    HTMLContent += "  <HEAD>";
                    HTMLContent += "      <TITLE>";
                    HTMLContent += "          Documentos Vencidos";
                    HTMLContent += "      </TITLE>";

                    HTMLContent += "      <style>";

                    HTMLContent += "          .center {";
                    HTMLContent += "                  text-align: center !important;";
                    HTMLContent += "                  padding-top: 5px;";
                    HTMLContent += "                  padding-bottom: 5px;";
                    HTMLContent += "          }";

                    HTMLContent += "          .fonte {";
                    HTMLContent += "                  font-family: Tahoma, 'Times New Roman', Arial, Helvetica, sans - serif";
                    HTMLContent += "                  text-align: center! important;";
                    HTMLContent += "                  padding-top: 10px;";
                    HTMLContent += "                  padding-bottom: 10px;";
                    HTMLContent += "          }";

                    HTMLContent += "          td {";
                    HTMLContent += "                font-size: 12px !important;";
                    HTMLContent += "                text-align: left ;";
                    HTMLContent += "          }";

                    HTMLContent += "      </style>";

                    HTMLContent += "  </HEAD>";
                    HTMLContent += "  <BODY>";
                    HTMLContent += "      <TABLE border='0' align='center' style='width: 100%'>";
                    HTMLContent += "          <tr>";
                    HTMLContent += "              <th class='center' style='font-size: 20px;'> <bDocumentos Vencidos</b> </th>"; // width='200'
                    HTMLContent += "          </tr>";
                    HTMLContent += "      </TABLE>";

                    HTMLContent += "      <TABLE style='width: 100%;  border: 1px solid black;' align= 'center'>";
                    HTMLContent += "          <tr style= 'border: 1px solid'>";
                    HTMLContent += "              <td width=500 style='text-align: left !important'> <b> Nome</b ></td>";
                    HTMLContent += "              <td width=200 style='text-align: left !important'> <b>Tipo Documento</b></td>";
                    HTMLContent += "              <td width=200 style='text-align: left !important'> <b> Número Documento </td>";
                    HTMLContent += "              <td width=200 style='text-align: left !important'> <b> Data Validade </td>";
                    HTMLContent += "          </tr>";


                    foreach (var item in documentosvencidos)
                    {
                        HTMLContent += "          <tr>"; //style='border-top: 1px solid'
                        HTMLContent += "              <td style='text-align: left'>" + item.Profissionais.Pessoas.Nome + "</td>";
                        HTMLContent += "              <td style='text-align:left' !important>" + item.TipoDocumento + " </td>";
                        HTMLContent += "              <td style='text-align:left' !important>" + item.NumeroDocumento + "</td>";
                        HTMLContent += "              <td style='text-align:left' !important>" + (item.DataValidade.HasValue ? item.DataValidade.Value.ToString("dd/MM/yyyy") : string.Empty) + "</td>";
                        HTMLContent += "          </tr>";

                    }
                    HTMLContent += "      </TABLE>";


                    HTMLContent += "  </BODY >";
                    HTMLContent += "  </HTML >";
                    var result = new { codigo = "00", html = HTMLContent };


                    var titulo = "Vencimento de Documentos ";
                    var destinatario = "rh@rvmadvogados.com.br";
                    var resposta = EnviarEmail(titulo, "Sistema", HTMLContent, destinatario);

                }
            }
            catch (Exception ex)
            {
#if (!DEBUG)
                    using (EventLog eventLog = new EventLog(Log))
                    {
                        eventLog.Source = Source;
                        eventLog.WriteEntry(String.Format("Método ReadSpreadsheet, Erro: {0}", ex.Message), EventLogEntryType.Error);
                    }
#endif
            }
        }


        //private List<Email> _emails = new List<Email>();
        //private string _hostname = "pop.gmail.com"; // Host do seu servidor POP3. Por exemplo, pop.gmail.com para o servidor do Gmail.
        //private int _port = 995; // Porta utilizada pelo host. Por exemplo, 995 para o servidor do Gmail.
        //private bool _useSsl = true; // Indicação se o servidor POP3 utiliza SSL para autenticação. Por exemplo, o servidor do Gmail utiliza SSL, então, "true".
        //private string _username = string.Format("recent:{0}", ConfigurationManager.AppSettings["EmailUser"]); // Usuário do servidor POP3. Por exemplo, seuemail@gmail.com.
        //private string _password = ConfigurationManager.AppSettings["EmailPass"]; // Senha do servidor POP3.
        #endregion


        #region Aniversariantes
        private void SetupProcessingTimerAniversariantes()
        {
            _timerEmails = new Timer(int.Parse(ConfigurationManager.AppSettings["TimeAniversariantes"]) * 55);
            _timerEmails.AutoReset = true;
            _timerEmails.Enabled = true;
            _timerEmails.Elapsed += new ElapsedEventHandler(TimeAniversariantes);
            _timerEmails.Start();
        }

        private void TimeAniversariantes(object source, ElapsedEventArgs e)
        {
            var datahora = DateTime.Now;
            if (datahora.Hour == 8 && datahora.Minute == 0 && executando)
            {
                executando = false;
                ReadAniversariantes();
                executando = true;
            }
        }

        public void ReadAniversariantes()
        {
            try
            {
                int dia = DateTime.Now.Day;
                int mes = DateTime.Now.Month;

                var aniversariantes = ProfissionaisBo.ListarAniversariantes(dia, mes);

                if (aniversariantes.Count > 0)
                {
                    string HTMLContent = string.Empty;
                    HTMLContent += "  <HTML>";
                    HTMLContent += "  <HEAD>";
                    HTMLContent += "      <TITLE>";
                    HTMLContent += "          Documentos Vencidos";
                    HTMLContent += "      </TITLE>";

                    HTMLContent += "      <style>";

                    HTMLContent += "          .center {";
                    HTMLContent += "                  text-align: center !important;";
                    HTMLContent += "                  padding-top: 5px;";
                    HTMLContent += "                  padding-bottom: 5px;";
                    HTMLContent += "          }";

                    HTMLContent += "          .fonte {";
                    HTMLContent += "                  font-family: Tahoma, 'Times New Roman', Arial, Helvetica, sans - serif";
                    HTMLContent += "                  text-align: center! important;";
                    HTMLContent += "                  padding-top: 10px;";
                    HTMLContent += "                  padding-bottom: 10px;";
                    HTMLContent += "          }";

                    HTMLContent += "          td {";
                    HTMLContent += "                font-size: 12px !important;";
                    HTMLContent += "                text-align: left ;";
                    HTMLContent += "          }";

                    HTMLContent += "      </style>";

                    HTMLContent += "  </HEAD>";
                    HTMLContent += "  <BODY>";
                    HTMLContent += "      <TABLE border='0' align='center' style='width: 100%'>";
                    HTMLContent += "          <tr>";
                    HTMLContent += "              <th class='center' style='font-size: 20px;'> <bDocumentos Vencidos</b> </th>"; // width='200'
                    HTMLContent += "          </tr>";
                    HTMLContent += "      </TABLE>";

                    HTMLContent += "      <TABLE style='width: 100%;  border: 1px solid black;' align= 'center'>";
                    HTMLContent += "          <tr style= 'border: 1px solid'>";
                    HTMLContent += "              <td width=500 style='text-align: left !important'> <b> Nome</b ></td>";
                    HTMLContent += "              <td width=200 style='text-align: left !important'> <b>Tipo Documento</b></td>";
                    HTMLContent += "              <td width=200 style='text-align: left !important'> <b> Número Documento </td>";
                    HTMLContent += "              <td width=200 style='text-align: left !important'> <b> Data Validade </td>";
                    HTMLContent += "          </tr>";


                    foreach (var item in aniversariantes)
                    {
                        HTMLContent += "          <tr>"; //style='border-top: 1px solid'
                        HTMLContent += "              <td style='text-align: left'>" + item.Pessoas.Nome + "</td>";
                        HTMLContent += "              <td style='text-align:left' !important>" + (item.DataNascimento.HasValue ? item.DataNascimento.Value.ToString("dd/MM") : string.Empty) + "</td>";
                        HTMLContent += "          </tr>";

                    }
                    HTMLContent += "      </TABLE>";


                    HTMLContent += "  </BODY >";
                    HTMLContent += "  </HTML >";
                    var result = new { codigo = "00", html = HTMLContent };

                    var titulo = "Aniversariantes da Semana ";
                    var destinatario = "";
                    foreach (var item in aniversariantes)
                    {
                        var emails = EmailsBo.ListarEmail(item.IDPessoa);
                        destinatario = emails.Email1;
                        var resposta = EnviarEmail(titulo, "Sistema", HTMLContent, destinatario);

                        string HTMLColaboradores = string.Empty;
                        HTMLColaboradores += "  <HTML>";
                        HTMLColaboradores += "  <HEAD>";
                        HTMLColaboradores += "      <TITLE>";
                        HTMLColaboradores += "          Documentos Vencidos";
                        HTMLColaboradores += "      </TITLE>";

                        HTMLColaboradores += "      <style>";

                        HTMLColaboradores += "          .center {";
                        HTMLColaboradores += "                  text-align: center !important;";
                        HTMLColaboradores += "                  padding-top: 5px;";
                        HTMLColaboradores += "                  padding-bottom: 5px;";
                        HTMLColaboradores += "          }";

                        HTMLColaboradores += "          .fonte {";
                        HTMLColaboradores += "                  font-family: Tahoma, 'Times New Roman', Arial, Helvetica, sans - serif";
                        HTMLColaboradores += "                  text-align: center! important;";
                        HTMLColaboradores += "                  padding-top: 10px;";
                        HTMLColaboradores += "                  padding-bottom: 10px;";
                        HTMLColaboradores += "          }";

                        HTMLColaboradores += "          td {";
                        HTMLColaboradores += "                font-size: 12px !important;";
                        HTMLColaboradores += "                text-align: left ;";
                        HTMLColaboradores += "          }";

                        HTMLColaboradores += "      </style>";

                        HTMLColaboradores += "  </HEAD>";
                        HTMLColaboradores += "  <BODY>";
                        HTMLColaboradores += "      <TABLE border='0' align='center' style='width: 100%'>";
                        HTMLColaboradores += "          <tr>";
                        HTMLColaboradores += "              <th class='center' style='font-size: 20px;'> <bDocumentos Vencidos</b> </th>"; // width='200'
                        HTMLColaboradores += "          </tr>";
                        HTMLColaboradores += "      </TABLE>";

                        HTMLColaboradores += "      <TABLE style='width: 100%;  border: 1px solid black;' align= 'center'>";
                        HTMLColaboradores += "          <tr style= 'border: 1px solid'>";
                        HTMLColaboradores += "              <td width=500 style='text-align: left !important'> <b> Nome</b ></td>";
                        HTMLColaboradores += "              <td width=200 style='text-align: left !important'> <b>Tipo Documento</b></td>";
                        HTMLColaboradores += "              <td width=200 style='text-align: left !important'> <b> Número Documento </td>";
                        HTMLColaboradores += "              <td width=200 style='text-align: left !important'> <b> Data Validade </td>";
                        HTMLColaboradores += "          </tr>";


                        foreach (var item1 in aniversariantes)
                        {
                            HTMLColaboradores += "          <tr>"; //style='border-top: 1px solid'
                            HTMLColaboradores += "              <td style='text-align: left'>" + item.Pessoas.Nome + "</td>";
                            HTMLColaboradores += "              <td style='text-align:left' !important>" + (item.DataNascimento.HasValue ? item.DataNascimento.Value.ToString("dd/MM") : string.Empty) + "</td>";
                            HTMLColaboradores += "          </tr>";

                        }
                        HTMLColaboradores += "      </TABLE>";


                        HTMLColaboradores += "  </BODY >";
                        HTMLColaboradores += "  </HTML >";

                        var colaboradores = ProfissionaisBo.ListarColaboradores(item.ID);
                        foreach (var item1 in colaboradores)
                        {
                            var emailscolaboradores = EmailsBo.ListarEmail(item1.IDPessoa);
                            destinatario = emailscolaboradores.Email1;
                            resposta = EnviarEmail(titulo, "Sistema", HTMLColaboradores, destinatario);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
#if (!DEBUG)
                    using (EventLog eventLog = new EventLog(Log))
                    {
                        eventLog.Source = Source;
                        eventLog.WriteEntry(String.Format("Método ReadSpreadsheet, Erro: {0}", ex.Message), EventLogEntryType.Error);
                    }
#endif
            }
        }

        #endregion

        #region AniversariantesCasa
        private void SetupProcessingTimerAniversariantesCasa()
        {
            _timerEmails = new Timer(int.Parse(ConfigurationManager.AppSettings["TimeAniversariantes"]) * 55);
            _timerEmails.AutoReset = true;
            _timerEmails.Enabled = true;
            _timerEmails.Elapsed += new ElapsedEventHandler(TimeAniversariantesCasa);
            _timerEmails.Start();
        }

        private void TimeAniversariantesCasa(object source, ElapsedEventArgs e)
        {
            var datahora = DateTime.Now;
            if (datahora.Hour == 8 && datahora.Minute == 0 && executando)
            {
                executando = false;
                ReadAniversariantesCasa();
                executando = true;
            }
        }

        public void ReadAniversariantesCasa()
        {
            try
            {
                int dia = DateTime.Now.Day;
                int mes = DateTime.Now.Month;

                var aniversariantescasa = ProfissionaisBo.ListarAniversariantesCasa(dia, mes);

                if (aniversariantescasa.Count > 0)
                {
                    string HTMLContent = string.Empty;
                    HTMLContent += "  <HTML>";
                    HTMLContent += "  <HEAD>";
                    HTMLContent += "      <TITLE>";
                    HTMLContent += "          Documentos Vencidos";
                    HTMLContent += "      </TITLE>";

                    HTMLContent += "      <style>";

                    HTMLContent += "          .center {";
                    HTMLContent += "                  text-align: center !important;";
                    HTMLContent += "                  padding-top: 5px;";
                    HTMLContent += "                  padding-bottom: 5px;";
                    HTMLContent += "          }";

                    HTMLContent += "          .fonte {";
                    HTMLContent += "                  font-family: Tahoma, 'Times New Roman', Arial, Helvetica, sans - serif";
                    HTMLContent += "                  text-align: center! important;";
                    HTMLContent += "                  padding-top: 10px;";
                    HTMLContent += "                  padding-bottom: 10px;";
                    HTMLContent += "          }";

                    HTMLContent += "          td {";
                    HTMLContent += "                font-size: 12px !important;";
                    HTMLContent += "                text-align: left ;";
                    HTMLContent += "          }";

                    HTMLContent += "      </style>";

                    HTMLContent += "  </HEAD>";
                    HTMLContent += "  <BODY>";
                    HTMLContent += "      <TABLE border='0' align='center' style='width: 100%'>";
                    HTMLContent += "          <tr>";
                    HTMLContent += "              <th class='center' style='font-size: 20px;'> <bDocumentos Vencidos</b> </th>"; // width='200'
                    HTMLContent += "          </tr>";
                    HTMLContent += "      </TABLE>";

                    HTMLContent += "      <TABLE style='width: 100%;  border: 1px solid black;' align= 'center'>";
                    HTMLContent += "          <tr style= 'border: 1px solid'>";
                    HTMLContent += "              <td width=500 style='text-align: left !important'> <b> Nome</b ></td>";
                    HTMLContent += "              <td width=200 style='text-align: left !important'> <b>Tipo Documento</b></td>";
                    HTMLContent += "              <td width=200 style='text-align: left !important'> <b> Número Documento </td>";
                    HTMLContent += "              <td width=200 style='text-align: left !important'> <b> Data Validade </td>";
                    HTMLContent += "          </tr>";


                    foreach (var item in aniversariantescasa)
                    {
                        HTMLContent += "          <tr>"; //style='border-top: 1px solid'
                        HTMLContent += "              <td style='text-align: left'>" + item.Pessoas.Nome + "</td>";
                        HTMLContent += "              <td style='text-align:left' !important>" + (item.DataNascimento.HasValue ? item.DataNascimento.Value.ToString("dd/MM") : string.Empty) + "</td>";
                        HTMLContent += "          </tr>";

                    }
                    HTMLContent += "      </TABLE>";


                    HTMLContent += "  </BODY >";
                    HTMLContent += "  </HTML >";
                    var result = new { codigo = "00", html = HTMLContent };

                    var titulo = "AniversariantesCasa da Semana ";
                    var destinatario = "";
                    foreach (var item in aniversariantescasa)
                    {
                        var emails = EmailsBo.ListarEmail(item.IDPessoa);
                        destinatario = emails.Email1;
                        var resposta = EnviarEmail(titulo, "Sistema", HTMLContent, destinatario);

                        string HTMLColaboradores = string.Empty;
                        HTMLColaboradores += "  <HTML>";
                        HTMLColaboradores += "  <HEAD>";
                        HTMLColaboradores += "      <TITLE>";
                        HTMLColaboradores += "          Documentos Vencidos";
                        HTMLColaboradores += "      </TITLE>";

                        HTMLColaboradores += "      <style>";

                        HTMLColaboradores += "          .center {";
                        HTMLColaboradores += "                  text-align: center !important;";
                        HTMLColaboradores += "                  padding-top: 5px;";
                        HTMLColaboradores += "                  padding-bottom: 5px;";
                        HTMLColaboradores += "          }";

                        HTMLColaboradores += "          .fonte {";
                        HTMLColaboradores += "                  font-family: Tahoma, 'Times New Roman', Arial, Helvetica, sans - serif";
                        HTMLColaboradores += "                  text-align: center! important;";
                        HTMLColaboradores += "                  padding-top: 10px;";
                        HTMLColaboradores += "                  padding-bottom: 10px;";
                        HTMLColaboradores += "          }";

                        HTMLColaboradores += "          td {";
                        HTMLColaboradores += "                font-size: 12px !important;";
                        HTMLColaboradores += "                text-align: left ;";
                        HTMLColaboradores += "          }";

                        HTMLColaboradores += "      </style>";

                        HTMLColaboradores += "  </HEAD>";
                        HTMLColaboradores += "  <BODY>";
                        HTMLColaboradores += "      <TABLE border='0' align='center' style='width: 100%'>";
                        HTMLColaboradores += "          <tr>";
                        HTMLColaboradores += "              <th class='center' style='font-size: 20px;'> <bDocumentos Vencidos</b> </th>"; // width='200'
                        HTMLColaboradores += "          </tr>";
                        HTMLColaboradores += "      </TABLE>";

                        HTMLColaboradores += "      <TABLE style='width: 100%;  border: 1px solid black;' align= 'center'>";
                        HTMLColaboradores += "          <tr style= 'border: 1px solid'>";
                        HTMLColaboradores += "              <td width=500 style='text-align: left !important'> <b> Nome</b ></td>";
                        HTMLColaboradores += "              <td width=200 style='text-align: left !important'> <b>Tipo Documento</b></td>";
                        HTMLColaboradores += "              <td width=200 style='text-align: left !important'> <b> Número Documento </td>";
                        HTMLColaboradores += "              <td width=200 style='text-align: left !important'> <b> Data Validade </td>";
                        HTMLColaboradores += "          </tr>";


                        foreach (var item1 in aniversariantescasa)
                        {
                            HTMLColaboradores += "          <tr>"; //style='border-top: 1px solid'
                            HTMLColaboradores += "              <td style='text-align: left'>" + item.Pessoas.Nome + "</td>";
                            HTMLColaboradores += "              <td style='text-align:left' !important>" + (item.DataNascimento.HasValue ? item.DataNascimento.Value.ToString("dd/MM") : string.Empty) + "</td>";
                            HTMLColaboradores += "          </tr>";

                        }
                        HTMLColaboradores += "      </TABLE>";


                        HTMLColaboradores += "  </BODY >";
                        HTMLColaboradores += "  </HTML >";

                        var colaboradores = ProfissionaisBo.ListarColaboradores(item.ID);
                        foreach (var item1 in colaboradores)
                        {
                            var emailscolaboradores = EmailsBo.ListarEmail(item1.IDPessoa);
                            destinatario = emailscolaboradores.Email1;
                            resposta = EnviarEmail(titulo, "Sistema", HTMLColaboradores, destinatario);
                        }

                    }

                }
            }
            catch (Exception ex)
            {
#if (!DEBUG)
                    using (EventLog eventLog = new EventLog(Log))
                    {
                        eventLog.Source = Source;
                        eventLog.WriteEntry(String.Format("Método ReadSpreadsheet, Erro: {0}", ex.Message), EventLogEntryType.Error);
                    }
#endif
            }
        }

        #endregion


        #region PeriodoAquisitivo
        private void SetupProcessingTimerPeriodoAquisitivo()
        {
            _timerEmails = new Timer(int.Parse(ConfigurationManager.AppSettings["TimeAniversariantes"]) * 55);
            _timerEmails.AutoReset = true;
            _timerEmails.Enabled = true;
            _timerEmails.Elapsed += new ElapsedEventHandler(TimerPeriodoAquisitivo);
            _timerEmails.Start();
        }

        private void TimerPeriodoAquisitivo(object source, ElapsedEventArgs e)
        {
            var datahora = DateTime.Now;
            if (datahora.Hour == 6 && datahora.Minute == 0 && executando)
            {
                executando = false;
                ReadDocumentos();
                executando = true;
            }
            if (datahora.Hour == 8 && datahora.Minute == 0 && executando)
            {
                executando = false;
                ReadTimerPeriodoAquisitivo();
                executando = true;
            }
        }

        public void ReadTimerPeriodoAquisitivo()
        {
            try
            {
                var data = DateTime.Parse(DateTime.Now.ToString("dd-MM-yyyy"));
                var dataInicio = data.AddDays(1);
                var dataFim = dataInicio.AddYears(1);


                var Periodoaquisitivo = ProfissionaisPeriodosAquisitivoBo.ListarPeriodoVencido(data);

                foreach (var item in Periodoaquisitivo)
                {
                    var Id = 0;
                    ProfissionaisPeriodosAquisitivo periodoaquisitivo = new ProfissionaisPeriodosAquisitivo();
                    periodoaquisitivo.Id = Id;
                    periodoaquisitivo.IdProfissional = item.IdProfissional;
                    periodoaquisitivo.IdProfissional = item.IdProfissional;
                    periodoaquisitivo.DataInicio = dataInicio;
                    periodoaquisitivo.DataFim = dataFim;
                    periodoaquisitivo.Dias = item.Dias;
                    periodoaquisitivo.DiasGozados = 0;
                    periodoaquisitivo.DataCadastro = DateTime.Now;
                    periodoaquisitivo.UsuarioCadastro = "Sistema";
                    periodoaquisitivo.DataAlteracao = DateTime.Now;
                    periodoaquisitivo.UsuarioAlteracao = "Sistema";

                    if (Id == 0)
                    {
                        Id = ProfissionaisPeriodosAquisitivoBo.Inserir(periodoaquisitivo);
                    }

                    periodoaquisitivo.Id = Id;
                    ProfissionaisPeriodosAquisitivoBo.Salvar(periodoaquisitivo);

                }
            }
            catch (Exception ex)
            {
#if (!DEBUG)
                    using (EventLog eventLog = new EventLog(Log))
                    {
                        eventLog.Source = Source;
                        eventLog.WriteEntry(String.Format("Método ReadSpreadsheet, Erro: {0}", ex.Message), EventLogEntryType.Error);
                    }
#endif
            }
        }

        #endregion


        public static string EnviarEmail(String Titulo, String NomeProfissional, String mensagemEmail, String destinatario)
        {
            var motivo = string.Empty;

            var empresa = EmpresaBo.Buscar(1);

            var to = string.Empty;
            var name = NomeProfissional;
            var emailremetente = empresa.Email;
            var passwordremetente = empresa.Senha;

            to = destinatario;


            if (!Mail.Send(emailremetente, passwordremetente, to, Titulo, mensagemEmail))
            {
                motivo = "Ocorreu uma falha ao enviar os E-mails. Tente novamente mais tarde ou entre em contato com o administrador do sistema.";
            }
            else
            {
                motivo = "Email enviado com sucesso";
            }

            return motivo;
        }

        public static string EnviarEmailBoleto(String Titulo, String NomeProfissional, String mensagemEmail, String destinatario, Object attach)
        {
            var motivo = string.Empty;

            var empresa = EmpresaBo.Buscar(1);

            var to = string.Empty;
            var name = NomeProfissional;
            var emailremetente = empresa.Email;
            var passwordremetente = empresa.Senha;

            to = destinatario;


            if (!Mail.SendBoleto(emailremetente, passwordremetente, to, Titulo, mensagemEmail, attach))
            {
                motivo = "Ocorreu uma falha ao enviar os E-mails. Tente novamente mais tarde ou entre em contato com o administrador do sistema.";
            }
            else
            {
                motivo = "Email enviado com sucesso";
            }

            return motivo;
        }
    }

}
