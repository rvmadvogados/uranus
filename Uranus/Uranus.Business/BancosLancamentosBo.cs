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
    public class BancosLancamentosBo : IRequiresSessionState
    {
        public static Int64 Inserir(BancosLancamentos bancoslancamentos)
        {
            using (var context = new UranusEntities())
            {
                context.BancosLancamentos.Add(bancoslancamentos);
                context.SaveChanges();

                return bancoslancamentos.ID;
            }
        }

        public static void Salvar(BancosLancamentos bancoslancamentos)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(bancoslancamentos).State = EntityState.Modified;
                context.Entry(bancoslancamentos).Property(x => x.DataCadastro).IsModified = false;
                context.Entry(bancoslancamentos).Property(x => x.NomeUsuarioCadastro).IsModified = false;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int64 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var bancoslancamentos = context.BancosLancamentos.Find(id);
                    context.BancosLancamentos.Attach(bancoslancamentos);
                    context.BancosLancamentos.Remove(bancoslancamentos);
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

        public static List<BancosLancamentos> Listar(Int32 FiltrarBanco, DateTime DataInicio, DateTime DataFim)
        {
            using (var context = new UranusEntities())
            {
                if (FiltrarBanco == 0)
                {
                    var query = from d in context.BancosLancamentos.Include("Bancos")
                                                                   .Include("Historicos")
                                where d.Data >= DataInicio
                                where d.Data <= DataFim
                                orderby d.Bancos.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.BancosLancamentos.Include("Bancos")
                                                                   .Include("Historicos")
                                where d.IdBanco == FiltrarBanco
                                where d.Data >= DataInicio
                                where d.Data <= DataFim
                                orderby d.Bancos.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static BancosLancamentos Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.BancosLancamentos
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.BancosLancamentos.Include("Bancos")
                             where d.ID == Id
                             select new
                             {
                                 Id = d.ID,
                                 Data = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.Data), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.Data), 2) + "/" + SqlFunctions.DatePart("year", d.Data).ToString(),
                                 IdBanco = d.IdBanco,
                                 IdHistorico = d.IdHistorico,
                                 Complemento = d.Complemento,
                                 ValorDebito = d.ValorDebito,
                                 ValorCredito = d.ValorCredito,
                                 DataCadastro = (d.DataCadastro != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataCadastro), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataCadastro), 2) + "/" + SqlFunctions.DatePart("year", d.DataCadastro).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataCadastro), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataCadastro), 2) : string.Empty),
                                 UsuarioCadastro = d.NomeUsuarioCadastro,
                                 DataAlteracao = (d.DataAlteracao != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataAlteracao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataAlteracao), 2) + "/" + SqlFunctions.DatePart("year", d.DataAlteracao).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataAlteracao), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataAlteracao), 2) : string.Empty),
                                 UsuarioAlteracao = d.NomeUsuarioAlteracao,
                             }).ToList()
                            .Select(x => new
                            {
                                Id = x.Id,
                                Data = x.Data,
                                IdBanco = x.IdBanco,
                                IdHistorico = x.IdHistorico,
                                Complemento = x.Complemento,
                                ValorDebito = (x.ValorDebito?.ToString("#,0.00", new CultureInfo("pt-BR")) ?? "0,00"),
                                ValorCredito = (x.ValorCredito?.ToString("#,0.00", new CultureInfo("pt-BR")) ?? "0,00"),
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
                var query = from d in context.BancosLancamentos
                            orderby d.IdBanco ascending
                            select new
                            {
                                Id = d.ID,
                                Nome = d.IdBanco
                            };

                return query.ToArray();
            }
        }
    }
}