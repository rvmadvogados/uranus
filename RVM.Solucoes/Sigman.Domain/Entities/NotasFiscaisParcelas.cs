using System;

namespace Sigman.Domain.Entities
{
    public class NotasFiscaisParcelas
    {
        public int ID { get; set; }
        public long IdNotaFiscal { get; set; }
        public int? Prazo { get; set; }
        public int Parcela { get; set; }
        public DateTime? Vencimento { get; set; }
        public decimal? ValorParcela { get; set; }
        public string EmitirBoleto { get; set; }

        public virtual NotasFiscais NotasFiscais { get; set; }
    }
}
