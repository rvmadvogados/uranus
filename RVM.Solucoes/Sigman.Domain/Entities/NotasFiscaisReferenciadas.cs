using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigman.Domain.Entities
{
    public class NotasFiscaisReferenciadas
    {
        public int ID { get; set; }
        public long IdNotaFiscal { get; set; }
        public string Tipo { get; set; }
        public string Chave { get; set; }
        public string CodigoUf { get; set; }
        public string AnoMes { get; set; }
        public string Cnpj { get; set; }
        public string ModeloNotaFiscal { get; set; }
        public string Serie { get; set; }
        public string NumeroNotaFiscal { get; set; }
        public string ModeloCupomFiscal { get; set; }
        public string NumeroCupomFiscal { get; set; }
        public string NumeroCO { get; set; }

        public virtual NotasFiscais NotasFiscais { get; set; }
    }
}
