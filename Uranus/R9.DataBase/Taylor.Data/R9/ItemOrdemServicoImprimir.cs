using R9.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taylor.Data.R9
{
    public class ItemOrdemServicoImprimir : BaseModel
    {
		[Column] public Int64 IdOrdem { get; set; }
		[Column] public Int64 NumeroOS { get; set; }
		[Column] public DateTime Data { get; set; }
		[Column] public string Veiculo { get; set; }
		[Column] public string Modelo { get; set; }
		[Column] public string Frota { get; set; }
		[Column] public Int32 AnoVeiculo { get; set; }
		[Column] public string Placa { get; set; }
		[Column] public string Motorista { get; set; }
		[Column] public string Fone { get; set; }
		[Column] public string Contato { get; set; }
		[Column] public string FoneContato { get; set; }
		[Column] public string HoraChegada { get; set; }
		[Column] public string HoraSaida { get; set; }
		[Column] public Decimal TotalPecas { get; set; }
		[Column] public Decimal ValorMaoObra { get; set; }
		[Column] public Decimal TotalDesconto { get; set; }
		[Column] public Decimal TotalLiquido { get; set; }
		[Column] public string Servico { get; set; }
		[Column] public string Diagnostico { get; set; }
		[Column] public string ServicoExecutado { get; set; }
		[Column] public string Observacao { get; set; }
		[Column] public Int32 IdCliente { get; set; }
        [Column] public string NomeCliente { get; set; }
        [Column] public string CpfCnpj { get; set; }
		[Column] public string Endereco { get; set; }
		[Column] public string Bairro { get; set; }
		[Column] public string Municipio { get; set; }
		[Column] public string Estado { get; set; }
		[Column] public string CEP { get; set; }

	}

	public class ItemOrdemServicoImprimirAdapter
    {
        public List<ItemOrdemServicoImprimir> Gerar(Int64 Id, string conexão)
        {
            return OrdemServicoImprimir.Gerar(Id, conexão);
        }
    }

    public static class OrdemServicoImprimir
    {
        public static List<ItemOrdemServicoImprimir> Gerar(Int64 Id, string conexão)
        {
            DataContext context = new DataContext(conexão);

            return global::R9.DataBase.Data.BuildAllObjects<ItemOrdemServicoImprimir>(context, $"SELECT * FROM dbo.OrdemServicoImprimir('{Id}' )");
        }

    }


}
