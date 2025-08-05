using R9.DataBase;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Data.R9
{
    public class ItemRelatorioCadastroClientes : BaseModel
    {
        [Column] public DateTime DataCadastro { get; set; }
        [Column] public string NomeCliente { get; set; }
        [Column] public string Sede { get; set; }
        [Column] public string Usuario { get; set; }
        [Column] public int IdSede { get; set; }
        [Column] public string Cliente { get; set; }
        [Column] public int IdProfissao { get; set; }
        [Column] public string NomeProfissao { get; set; }
    }
    public class ItemRelatorioCadastroClientesAdapter
    {
        public List<ItemRelatorioCadastroClientes> Gerar(DateTime? dataInício, DateTime? dataFim, Int32? Usuario, Int32? Cliente, Int32? Profissao, string conexão)
        {
            return RelatorioCadastroClientes.Gerar(dataInício, dataFim, Usuario, Cliente, Profissao, conexão);
        }
    }

    public static class RelatorioCadastroClientes
    {
        public static List<ItemRelatorioCadastroClientes> Gerar(DateTime? dataInício, DateTime? dataFim, Int32? Usuario, Int32? Cliente, Int32? Profissao, string conexão)
        {
            DataContext context = new DataContext(conexão);

            var parameters = new List<SqlParameter>
            {
                { "@dataInicio", dataInício },
                { "@dataFim", dataFim },
                { "@usuario", Usuario },
                { "@cliente", Cliente },
                { "@Profissao", Profissao }
            };


            return global::R9.DataBase.Data.BuildAllObjects<ItemRelatorioCadastroClientes>(context, $"SELECT * FROM dbo.RelatorioCadastroClientes(@dataInicio, @dataFim, @usuario, @cliente, @Profissao)", parameters);
        }

        public static void FiltrarPorSede(List<ItemRelatorioCadastroClientes> listaBase, List<int> Sedes)
        {
            listaBase.RemoveAll(x => !Sedes.Contains(x.IdSede));
        }

    }
}
