namespace Sigman.Domain.Entities
{
    public class ClientesEnderecos
    {
        public long ID { get; set; }
        public int IdCliente { get; set; }
        public string CEP { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Estado { get; set; }
        public string Municipio { get; set; }
        public string CodigoMunicipio { get; set; }
        public bool Principal { get; set; }

        public virtual Clientes Clientes { get; set; }
    }
}
