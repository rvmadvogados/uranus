using System;

namespace Sigman.Domain.Entities
{
    public class NotasServicosParcelas
    {
        public int ID { get; set; }
        public long IdNotaServico { get; set; }
        public int? Prazo { get; set; }
        public int Parcela { get; set; }
        public DateTime? Vencimento { get; set; }
        public decimal? ValorParcela { get; set; }
        public string EmitirBoleto { get; set; }

        public virtual NotasServicos NotasServicos { get; set; }
    }
}

