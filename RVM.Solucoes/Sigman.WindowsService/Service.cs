using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Sigman.WindowsService
{
    public partial class Service : ServiceBase
    {
        public const int Retest = 11;

        private Timer _timerWebServiceEmails;

        string Log = "Application";
        string Source = "Syndicus.WindowsService";

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

            if (ConfigurationManager.AppSettings["ActivateWebServiceEmails"] == "1")
                SetupProcessingTimerWebServiceEmails();
        }

        protected override void OnStop()
        {
            using (EventLog eventLog = new EventLog(Log))
            {
                eventLog.Source = Source;
                eventLog.WriteEntry("Finalizando a aplicação.", EventLogEntryType.Information);
            }
        }


        #region WebService Emails
        private void SetupProcessingTimerWebServiceEmails()
        {
            _timerWebServiceEmails = new Timer(int.Parse(ConfigurationManager.AppSettings["TimeWebServiceEmails"]));
            _timerWebServiceEmails.AutoReset = true;
            _timerWebServiceEmails.Enabled = true;
            _timerWebServiceEmails.Elapsed += new ElapsedEventHandler(OnTimedEventWebServiceEmails);
            _timerWebServiceEmails.Start();
        }

        private void OnTimedEventWebServiceEmails(object source, ElapsedEventArgs e)
        {
            ReadWebServiceEmails();
        }

        /// <summary>
        /// Execute a Soap WebService call
        /// </summary>
        public void ReadWebServiceEmails()
        {
            try
            {
                //Buscar todos os e-mails que tem que ser enviados
                var emails = BoletoController.BuscarEmailsEnviar();

                //Fazer um laço, repetindo uma vez para cada e-mail

                var NomeCorretor = "";

                foreach (var email in emails.Take(600).ToList())
                {
                    var retorno = Mailer.Mail.Send(email.Remetente, email.EmailEndereco, email.EmailAssunto, email.EmailCorpo, email.SMTPEndereco, email.SMTPUsuario, email.SMTPSenha, email.SMTPPorta);

                    if (retorno != "Sucesso")
                    {
                        //NomeCorretor = email.Nome + " - Creci: " + email.Creci;
                        //CampanhaEmailsFalhas campanhaemailsfalhas = new CampanhaEmailsFalhas();
                        //campanhaemailsfalhas.Id = 0;
                        //campanhaemailsfalhas.Nome = NomeCorretor;
                        //campanhaemailsfalhas.Mensagem = retorno;

                        //CampanhaEmailsFalhasBO.Inserir(campanhaemailsfalhas);

                        email.Enviado = true;
                        email.EnviadoEm = DateTime.Now;

                        //marcar o e-mail como enviado
                        email.Save();
                    }
                    else
                    {
                        email.Enviado = true;
                        email.EnviadoEm = DateTime.Now;

                        //marcar o e-mail como enviado
                        email.Save();
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

                //for (int i = 0; i < Retest; i++)
                //{
                //    Thread.Sleep(7200000);
                //    ReadWebServiceEmails();
                //}
            }
        }
        #endregion


    }
}
