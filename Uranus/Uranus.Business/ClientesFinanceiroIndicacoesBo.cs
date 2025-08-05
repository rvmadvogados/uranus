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
    public class ClientesFinanceiroIndicacoesBo : IRequiresSessionState
    {
        public static Int64 Inserir(ClientesFinanceiroIndicacoes clientesfinanceiroindicacoes)
        {
            using (var context = new UranusEntities())
            {
                context.ClientesFinanceiroIndicacoes.Add(clientesfinanceiroindicacoes);
                context.SaveChanges();

                return clientesfinanceiroindicacoes.Id;
            }
        }

        public static void Salvar(ClientesFinanceiroIndicacoes clientesfinanceiroindicacoes)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(clientesfinanceiroindicacoes).State = EntityState.Modified;
                context.Entry(clientesfinanceiroindicacoes).Property(x => x.DataCadastro).IsModified = false;
                context.Entry(clientesfinanceiroindicacoes).Property(x => x.NomeUsuarioCadastro).IsModified = false;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int64 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var clientesfinanceiroindicacoes = context.ClientesFinanceiroIndicacoes.Find(id);
                    context.ClientesFinanceiroIndicacoes.Attach(clientesfinanceiroindicacoes);
                    context.ClientesFinanceiroIndicacoes.Remove(clientesfinanceiroindicacoes);
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

        public static List<ClientesFinanceiroIndicacoes> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.ClientesFinanceiroIndicacoes.Include("Clientes")
                                                                              .Include("Clientes.Pessoas")
                                orderby d.Clientes.Pessoas.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.ClientesFinanceiroIndicacoes.Include("Clientes")
                                                                              .Include("Clientes.Pessoas")
                                where d.Clientes.Pessoas.Nome.Contains(search)
                                orderby d.Clientes.Pessoas.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static ClientesFinanceiroIndicacoes Consultar(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ClientesFinanceiroIndicacoes
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.ClientesFinanceiroIndicacoes.Include("Clientes")
                                                                           .Include("Clientes.Pessoas")
                                                                           .Include("Profissionais")
                                                                           .Include("Profissionais.Pessoas")
                                                                           .Include("Parceiros")
                                                                           .Include("Parceiros")
                             where d.Id == Id
                            select new
                            {
                                Id = d.Id,
                                IdCliente = d.IdCliente,
                                NomeCliente = d.Clientes.Pessoas.Nome,
                                Tipo = d.Tipo,
                                Caixa = d.Caixa,
                                PercentualNF = d.PercentualNF,
                                IdProfissional = d.IdProfissional,
                                NomeProfissional = d.Profissionais.Pessoas.Nome,
                                IdIndicacao1 = d.IdIndicacao1,
                                NomeIndicacao1 = d.Parceiros.Nome,
                                PercentualIndicacao1 = d.PercentualIndicacao1,
                                IdIndicacao2 = d.IdIndicacao2,
                                NomeIndicacao2 = d.Parceiros.Nome,
                                PercentualIndicacao2 = d.PercentualIndicacao2,
                                TipoComissao = d.TipoComissao,
                                DataCadastro = (d.DataCadastro != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataCadastro), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataCadastro), 2) + "/" + SqlFunctions.DatePart("year", d.DataCadastro).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataCadastro), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataCadastro), 2) : string.Empty),
                                UsuarioCadastro = d.NomeUsuarioCadastro,
                                DataAlteracao = (d.DataAlteracao != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataAlteracao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataAlteracao), 2) + "/" + SqlFunctions.DatePart("year", d.DataAlteracao).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataAlteracao), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataAlteracao), 2) : string.Empty),
                                UsuarioAlteracao = d.NomeUsuarioAlteracao,
                            }).ToList()
                            .Select(x => new
                            {
                                Id = x.Id,
                                IdCliente = x.IdCliente,
                                NomeCliente = x.NomeCliente,
                                Tipo = x.Tipo,
                                Caixa = (x.Caixa != null ? x.Caixa.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : "0,00"),
                                PercentualNF = (x.PercentualNF != null ? x.PercentualNF.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : "0,00"),
                                IdProfissional = x.IdProfissional,
                                NomeProfissional = x.NomeProfissional,
                                IdIndicacao1 = x.IdIndicacao1,
                                NomeIndicacao1 = x.NomeIndicacao1,
                                PercentualIndicacao1 = (x.PercentualIndicacao1 != null ? x.PercentualIndicacao1.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : "0,00"),
                                IdIndicacao2 = x.IdIndicacao2,
                                NomeIndicacao2 = x.NomeIndicacao2,
                                PercentualIndicacao2 = (x.PercentualIndicacao2 != null ? x.PercentualIndicacao2.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : "0,00"),
                                TipoComissao = x.TipoComissao,
                                DataCadastro = x.DataCadastro,
                                UsuarioCadastro = x.UsuarioCadastro,
                                DataAlteracao = x.DataAlteracao,
                                UsuarioAlteracao = x.UsuarioAlteracao,
                            });

                return query.ToArray();

            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ClientesFinanceiroIndicacoes.Include("Clientes")
                                                                               .Include("Clientes.Pessoas")
                            orderby d.Clientes.Pessoas.Nome ascending
                            select new
                            {
                                Id = d.Id,
                                Nome = d.Clientes.Pessoas.Nome
                            };

                return query.ToArray();
            }
        }
    }
}