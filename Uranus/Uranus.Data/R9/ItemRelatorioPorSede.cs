using R9.DataBase;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Uranus.Data.R9
{
    public class ItemRelatorioPorSede : BaseModel
    {
        [Column] public int IdCliente { get; set; }
        [Column] public int IdProcesso { get; set; }
        [Column] public string NumeroProcesso { get; set; }
        [Column] public DateTime DataAcao { get; set; }
        [Column] public string NomeAutor { get; set; }
        [Column] public string NomeReu { get; set; }
        [Column] public string Area { get; set; }
        [Column] public string Status { get; set; }
        [Column] public int IdSede { get; set; }
        [Column] public string Acao { get; set; }
        [Column] public int IdAcao { get; set; }
        [Column] public int IdArea { get; set; }
        [Column] public int IdJuizo { get; set; }
    }

    public class ItemRelatorioPorSedeAdapter
    {
        public List<ItemRelatorioPorSede> Gerar(DateTime? dataInício, DateTime? dataFim, string Cliente, String Sedes, string Acoes, string Areas, string Juizos, string conexão)
        {
            return RelatorioPorSede.Gerar(dataInício, dataFim, Cliente, Sedes, Acoes, Areas, Juizos, conexão);
        }
    }

    public static class RelatorioPorSede
    {
        public static List<ItemRelatorioPorSede> Gerar(DateTime? dataInício, DateTime? dataFim, string Cliente, string Sedes, string Acoes, string Areas, string Juizos, string conexão)
        {
            DataContext context = new DataContext(conexão);

            var parameters = new List<SqlParameter>
            {
                { "@dataInicio", dataInício },
                { "@dataFim", dataFim },
                { "@Cliente", Cliente },
                { "@Sedes", Sedes },
                { "@Acoes", Acoes },
                { "@Areas", Areas },
                { "@Juizos", Juizos }

            };

            return global::R9.DataBase.Data.BuildAllObjects<ItemRelatorioPorSede>(context, $"SELECT * FROM dbo.fnRelatorioPorSede(@dataInicio, @dataFim, @Cliente, @Sedes, @Acoes, @Areas, @Juizos)", parameters);

        }

        public static void LimparRetorno(List<ItemRelatorioPorSede> listaBase)
        {
                listaBase.Clear();
        }
    }

}
