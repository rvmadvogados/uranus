namespace Sigman.Domain.Entities
{
    public class Transportadoras
    {
        public int ID { get; set; }
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Municipio { get; set; }
        public string UF { get; set; }
        public string Placa { get; set; }
        public string Cnpj { get; set; }
        public string InscricaoEstadual { get; set; }
        public string Fone { get; set; }
        public string Cep { get; set; }
        public string Contato { get; set; }
        public string Observacao { get; set; }
        public int? CodigoLegado { get; set; }
    }
}
