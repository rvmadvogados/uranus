namespace Sigman.Domain.Entities
{
    public class Empresas
    {
        public int ID { get; set; }
        public string Nome { get; set; }
        public string NomeFantasia { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Cep { get; set; }
        public string Cnpj { get; set; }
        public string InscricaoEstadual { get; set; }
        public string Fone { get; set; }
        public string Cnae { get; set; }
        public string InscricaoMunicipal { get; set; }
        public string CodigoMunicipio { get; set; }
        public string Regime { get; set; }
        public decimal? PercentualIcms { get; set; }
        public string HorarioVerao { get; set; }
        public bool? Parametrizacao { get; set; }
        public string Email { get; set; }
        public string TokenHomologacao { get; set; }
        public string TokenProducao { get; set; }
        public string Url_homologacao { get; set; }
        public string Url_Producao { get; set; }
        public string PastaNotas { get; set; }
        public string NomeImpressoraNfe { get; set; }
        public string NomeImpressoraCupom { get; set; }
        public string SMTPEndereco { get; set; }
        public string SMTPUsuario { get; set; }
        public string SMTPSenha { get; set; }
        public string SMTPPorta { get; set; }
        public string SMTPRemetente { get; set; }
        public long? NossoNumero { get; set; }
        public int? IdTipo { get; set; }
        public string Ambiente { get; set; }
        public string NomeLogo { get; set; }
        public string NomeLogoPedido { get; set; }
        public int? RegimeTributarioSN { get; set; }
        public decimal PercentualTotalTributosSN { get; set; }
    }
}
