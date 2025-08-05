using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;
using Uranus.Domain.Entities;

namespace Uranus.Business
{
    public class RelatorioCancelamentoAgendaBo : IRequiresSessionState
    {
        public static List<AgendasCanceladosNaocompareceu> Gerar(string DataInício = "", string DataFim = "", string DataCancelamentoInicial = "", string DataCancelamentoFinal = "", string Tipo = "")
        {
            DateTime? datainicio = null;
            DateTime? datafim = null;
            DateTime? dataCancelamentoInicial = null;
            DateTime? dataCancelamentoFinal = null;

            if (DataInício.Length > 0)
            {
                datainicio = DateTime.Parse(DataInício);
            }
            if (DataFim.Length > 0)
            {
                datafim = DateTime.Parse(DataFim);
            }
            if (DataCancelamentoInicial.Length > 0)
            {
                dataCancelamentoInicial = DateTime.Parse(DataCancelamentoInicial);
            }
            if (DataCancelamentoFinal.Length > 0)
            {
                dataCancelamentoFinal = DateTime.Parse(DataCancelamentoFinal);
            }

            using (var context = new UranusEntities())
            {

                //var query = context.Database.SqlQuery<AgendasCanceladosNaocompareceu>($"stpRelatorioAgendaCancelamento_NaoCompareceu '{datainicio.ToString("yyyyMMdd")}', '{datafim.ToString("yyyyMMdd")}', '{dataCancelamentoInicial.ToString("yyyyMMdd")}', '{dataCancelamentoFinal.ToString("yyyyMMdd")}', '{Tipo}'").ToList();

                var query = context.Agendas.Include("Clientes")
                                           .Include("Clientes.Pessoas")
                                           .Include("Profissionais")
                                           .Include("Profissionais.Pessoas")
                                           .Include("Sede")
                                           .Include("AgendasTipos")
                                           .AsQueryable();
                if (datainicio != null)
                {
                    query = query.Where(x => x.Data >= datainicio && x.Data <= datafim);
                }
                //if (dataCancelamentoInicial != null)
                //{
                //    query = query.Where(x => x.DataCancelamento >= dataCancelamentoInicial && x.DataCancelamento <= dataCancelamentoFinal);
                //}
                if (Tipo == "Cancelado")
                {
                    query = query.Where(x => x.Cancelou == true);
                }
                if (Tipo == "Ausente")
                {
                    query = query.Where(x => x.Compareceu == true);
                }
                if (Tipo == "")
                {
                    query = query.Where(x => x.Cancelou == true || x.Compareceu == true);
                }

                var queryAux = query.Select(obj => new AgendasCanceladosNaocompareceu
                {
                    Data = obj.Data,
                    Hora = obj.Hora,
               //     IdCliente = int.Parse(obj.IdCliente.ToString()),
                    NomeCliente = obj.Clientes.Pessoas.Nome,
                    IdProfissional = obj.IdProfissional,
                    NomeProfissional = obj.Profissionais.Pessoas.Nome,
                    DataCancelamento = obj.DataCancelamento,
                    Descricao = obj.MotivoConsulta,
                    Sede = obj.Sedes.Nome,
                    Assunto = obj.AgendasTipos.Nome,
                    //Compareceu = obj.Compareceu, 
                    //Cancelou = obj.Cancelou,
                    Tipo = (obj.Compareceu == true ? "Ausente" : "Cancelado")
                }).ToList();
                return queryAux.OrderBy(x => x.Data).ToList();
                //return query.ToList().Select(obj => new AgendasCanceladosNaocompareceu
                //{
                //    Data = obj.Data
                //});
            }
        }

        public static void FiltrarPorCliente(List<AgendasCanceladosNaocompareceu> listaBase, string NomeCliente)
        {
            listaBase.RemoveAll(x => !NomeCliente.Contains(x.NomeCliente));
        }
        public static void FiltrarPorProfissional(List<AgendasCanceladosNaocompareceu> listaBase, List<int> idsProfissionais)
        {
            listaBase.RemoveAll(x => !idsProfissionais.Contains(x.IdProfissional));
        }
    }
}