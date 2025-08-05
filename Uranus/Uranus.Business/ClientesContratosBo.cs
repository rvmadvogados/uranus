using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class ClientesContratosBo : IRequiresSessionState
    {
        public static Int32 Inserir(ClientesContratos clientescontratos)
        {
            using (var context = new UranusEntities())
            {
                context.ClientesContratos.Add(clientescontratos);
                context.SaveChanges();

                return clientescontratos.ID;
            }
        }

        public static void Salvar(ClientesContratos clientescontratos)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(clientescontratos).State = EntityState.Modified;
                context.Entry(clientescontratos).Property(x => x.IdClientesNotas).IsModified = false;
                context.Entry(clientescontratos).Property(x => x.IdClientesNotasParcelas).IsModified = false;
                context.Entry(clientescontratos).Property(x => x.IdReceita).IsModified = false;
                context.Entry(clientescontratos).Property(x => x.DataCadastro).IsModified = false;
                context.Entry(clientescontratos).Property(x => x.NomeUsuarioCadastro).IsModified = false;
                context.Entry(clientescontratos).Property(x => x.DataFinanceiro).IsModified = false;
                context.Entry(clientescontratos).Property(x => x.NomeUsuarioFinanceiro).IsModified = false;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var clientescontratos = context.ClientesContratos.Find(id);
                    context.ClientesContratos.Attach(clientescontratos);
                    context.ClientesContratos.Remove(clientescontratos);
                    context.SaveChanges();
                }

                return "00";
            }
            catch (Exception ex)
            {
                String error = "99";
                String message = ex.InnerException.ToString();

                if (message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
                    error = "98";

                return error;
            }
        }

        public static List<ClientesContratos> Listar(string DataInicio = "", string DataFim = "", string FiltrarNumeroContrato = "", string FiltrarNomeCliente = "", Int32? FiltrarSede = null, Int32? FiltrarArea = null)
        {
            using (var context = new UranusEntities())
            {

                var query = context.ClientesContratos.Include("Clientes")
                                                     .Include("Clientes.Pessoas")
                                                     .Include("Clientes.Sedes")
                                                     .Include("ProcessosAreas")
                                                     .Include("Profissionais")
                                                     .Include("Profissionais.Pessoas")
                                                     .Include("ProcessosCadastroAcao")
                                   .AsQueryable();

                query = query.Where(x => x.NumeroContrato.Contains(FiltrarNumeroContrato));
                query = query.Where(x => x.Clientes.Pessoas.Nome.Contains(FiltrarNomeCliente));

                if (FiltrarSede != null)
                {
                    query = query.Where(x => x.Clientes.IdSede == FiltrarSede);
                }
                if (FiltrarArea != null)
                {
                    query = query.Where(x => x.IdArea == FiltrarArea);
                }

                if (DataInicio != "")
                {
                    var dataInicio = DateTime.Parse(DataInicio);
                    query = query.Where(x => x.Data >= dataInicio);
                }
                if (DataFim != "")
                {
                    var dataFim = DateTime.Parse(DataFim);
                    query = query.Where(x => x.Data <= dataFim);
                }

                query = query.AsQueryable().OrderBy(x => x.Clientes.Pessoas.Nome);

                return query.Take(500).ToList();
            }
        }

        public static ClientesContratos Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ClientesContratos
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.ClientesContratos.Include("Clientes")
                                                                .Include("Clientes.Pessoas")
                                                                .Include("Clientes.Sedes")
                                                                .Include("ProcessosAreas")
                                                                .Include("Profissionais")
                                                                .Include("Profissionais.Pessoas")
                                                                .Include("Bancos")
                                                                .Include("ProcessosCadastroAcao")
                             where d.ID == Id
                             select new
                             {
                                 Id = d.ID,
                                 NumeroContrato = d.NumeroContrato,
                                 IdProfissional = d.IdProfissional,
                                 NomeProfissional = d.Profissionais.Pessoas.Nome,
                                 IdCliente = d.IdCliente,
                                 NomeCliente = d.Clientes.Pessoas.Nome,
                                 NomeSede = d.Clientes.Sedes.Nome,
                                 IdArea = d.IdArea,
                                 NomeArea = d.ProcessosAreas.AreaAtuacao,
                                 IdAcao = d.IdAcao,
                                 NomeAcao = d.ProcessosCadastroAcao.Acao,
                                 Data = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.Data), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.Data), 2) + "/" + SqlFunctions.DatePart("year", d.Data).ToString(),
                                 ValorHonorarios = d.ValorHonorarios,
                                 SaldoHonorarios = d.SaldoHonorarios,
                                 CondicaoPagamento = d.CondicaoPagamento,
                                 NumeroMeses = d.NumeroMeses,
                                 PrimeiroVencimento = (d.PrimeiroVencimento != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.PrimeiroVencimento), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.PrimeiroVencimento), 2) + "/" + SqlFunctions.DatePart("year", d.PrimeiroVencimento).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.PrimeiroVencimento), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.PrimeiroVencimento), 2) : string.Empty),
                                 FormaPagamento = d.FormaPagamento,
                                 ValorParcela = d.ValorParcela,
                                 Observacao = d.Observacao,
                                 DataPagamento = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataPagamento), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataPagamento), 2) + "/" + SqlFunctions.DatePart("year", d.DataPagamento).ToString(),
                                 IdClientesNotas = d.IdClientesNotas,
                                 IdClientesNotasParcelas = d.IdClientesNotasParcelas,
                                 IdReceita = d.IdReceita,
                                 IdBanco = d.IdBanco,
                                 NomeBanco= d.Bancos.Nome,
                                 NumeroProcesso = d.NumeroProcesso,
                                 DataCadastro = (d.DataCadastro != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataCadastro), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataCadastro), 2) + "/" + SqlFunctions.DatePart("year", d.DataCadastro).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataCadastro), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataCadastro), 2) : string.Empty),
                                 UsuarioCadastro = d.NomeUsuarioCadastro,
                                 DataAlteracao = (d.DataAlteracao != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataAlteracao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataAlteracao), 2) + "/" + SqlFunctions.DatePart("year", d.DataAlteracao).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataAlteracao), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataAlteracao), 2) : string.Empty),
                                 UsuarioAlteracao = d.NomeUsuarioAlteracao,
                                 DataFinanceiro = (d.DataFinanceiro != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataFinanceiro), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataFinanceiro), 2) + "/" + SqlFunctions.DatePart("year", d.DataFinanceiro).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataFinanceiro), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataFinanceiro), 2) : string.Empty),
                                 UsuarioFinanceiro = d.NomeUsuarioFinanceiro,
                                 NomeIndicacao = d.Clientes.NomeIndicacao,
                                 IdAcaoEvento = d.IdAcaoEvento,
                             }).ToList()
                            .Select(x => new
                            {
                                Id = x.Id,
                                NumeroContrato = x.NumeroContrato,
                                IdProfissional = x.IdProfissional,
                                NomeProfissional = x.NomeProfissional,
                                IdCliente = x.IdCliente,
                                NomeCliente = x.NomeCliente,
                                NomeSede = x.NomeSede,
                                IdArea = x.IdArea,
                                NomeArea = x.NomeArea,
                                IdAcao = x.IdAcao,
                                NomeAcao = x.NomeAcao,
                                Data = x.Data,
                                ValorHonorarios = (x.ValorHonorarios?.ToString("#,0.00", new CultureInfo("pt-BR")) ?? "0,00"),
                                SaldoHonorarios = (x.SaldoHonorarios?.ToString("#,0.00", new CultureInfo("pt-BR")) ?? "0,00"),
                                CondicaoPagamento = x.CondicaoPagamento,
                                NumeroMeses = x.NumeroMeses,
                                PrimeiroVencimento = x.PrimeiroVencimento,
                                FormaPagamento = x.FormaPagamento,
                                ValorParcela = (x.ValorParcela?.ToString("#,0.00", new CultureInfo("pt-BR")) ?? "0,00"),
                                Observacao = x.Observacao,
                                DataPagamento = x.DataPagamento,
                                IdClientesNotas = x.IdClientesNotas,
                                IdClientesNotasParcelas = x.IdClientesNotasParcelas,
                                IdReceita = x.IdReceita,
                                IdBanco = x.IdBanco,
                                NomeBanco = x.NomeBanco,
                                NumeroProcesso = x.NumeroProcesso,
                                DataCadastro = x.DataCadastro,
                                UsuarioCadastro = x.UsuarioCadastro,
                                DataAlteracao = x.DataAlteracao,
                                UsuarioAlteracao = x.UsuarioAlteracao,
                                DataFinanceiro = x.DataFinanceiro,
                                UsuarioFinanceiro = x.UsuarioFinanceiro,
                                NomeIndicacao = x.NomeIndicacao,
                                IdAcaoEvento = x.IdAcaoEvento,
                            });

                return query.ToArray();
            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ClientesContratos.Include("Clientes")
                                                               .Include("Pessoas")
                            orderby d.IdCliente ascending
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Clientes.Pessoas.Nome
                            };

                return query.ToArray();
            }
        }


        public static FormaPagamentos ConsultarFormaPagamento(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.FormaPagamentos
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        
    }
}


