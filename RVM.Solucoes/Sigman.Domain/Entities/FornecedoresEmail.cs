namespace Sigman.Domain.Entities
{
    public class FornecedoresEmail
    {
        public int ID { get; set; }
        public int IDFornecedor { get; set; }
        public string Email { get; set; }
        public bool Ativo { get; set; }
        public int? Score { get; set; }
    }
}
