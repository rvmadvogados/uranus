using System;

namespace Sigman.Mailer
{
    public class ItemCampanha
    {
        public long Id { get; set; }
        public long IdCampanha { get; set; }
        public string CampanhaNome { get; set; }
        public DateTime? CampanhaData { get; set; }
        public string Acordo { get; set; }
        public string Cpf { get; set; }
        public string Nome { get; set; }
        public string Creci { get; set; }
        public string SMTPEndereco { get; set; }
        public string SMTPUsuario { get; set; }
        public string SMTPSenha { get; set; }
        public int SMTPPorta { get; set; }
        public string Remetente { get; set; }
        public string EmailAssunto { get; set; }
        public string EmailEndereco { get; set; }
        public string EmailCorpo { get; set; }
        public string Status { get; set; }
        public DateTime? EnviadoEm { get; set; }
        public bool Enviado { get; set; }
        public string Anexo { get; set; }
    }
}
