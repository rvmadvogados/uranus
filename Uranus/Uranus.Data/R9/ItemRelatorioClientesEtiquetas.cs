using R9.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Data.R9
{
    public class ItemRelatorioClientesEtiquetas : BaseModel
    {
        [Column] public Int32 IdCliente { get; set; }
        [Column] public string NomeCliente { get; set; }
        [Column] public string Endereco { get; set; }
        [Column] public string Bairro { get; set; }
        [Column] public string Municipio { get; set; }
        [Column] public string Estado { get; set; }
        [Column] public string Cep { get; set; }
        [Column] public string Sede { get; set; }
        [Column] public int IdSede { get; set; }
        [Column] public DateTime? DataCadastro { get; set; }
    }

    public class ItemRelatorioClientesEtiquetasAdapter
    {
        public List<ItemRelatorioClientesEtiquetas> Gerar(string conexão)
        {
            return RelatorioClientesEtiquetas.Gerar(conexão);
        }
    }

    public static class RelatorioClientesEtiquetas
    {
        public static List<ItemRelatorioClientesEtiquetas> Gerar(string conexão)
        {
            DataContext context = new DataContext(conexão);

            return global::R9.DataBase.Data.BuildAllObjects<ItemRelatorioClientesEtiquetas>(context, $"SELECT * FROM dbo.RelatorioClientesEtiquetas()");
        }

        public static void FiltrarPorSede(List<ItemRelatorioClientesEtiquetas> listaBase, List<int> Sedes)
        {
            listaBase.RemoveAll(x => !Sedes.Contains(x.IdSede));
        }
    }
}
