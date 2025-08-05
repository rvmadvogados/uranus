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
    public class RHCargosSalariosBo : IRequiresSessionState
    {
        public static Int32 Inserir(RHCargosSalarios cargossalarios)
        {
            using (var context = new UranusEntities())
            {
                context.RHCargosSalarios.Add(cargossalarios);
                context.SaveChanges();

                return cargossalarios.Id;
            }
        }

        public static void Salvar(RHCargosSalarios cargossalarios)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(cargossalarios).State = EntityState.Modified;
                context.Entry(cargossalarios).Property(x => x.DataCadastro).IsModified = false;
                context.Entry(cargossalarios).Property(x => x.UsuarioCadastro).IsModified = false;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var cargossalarios = context.RHCargosSalarios.Find(id);
                    context.RHCargosSalarios.Attach(cargossalarios);
                    context.RHCargosSalarios.Remove(cargossalarios);
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

        public static RHCargosSalarios Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.RHCargosSalarios
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.RHCargosSalarios.Include("Clientes")
                                                                 .Include("Clientes.Pessoas")
                                                                 .Include("FinanceirOrigem")
                                                                 .Include("FinanceiroTipo")
                                                                 .Include("Sedes")
                                                                 .Include("ProcessosAreas")
                                                                 .Include("Bancos")
                                                                 .Include("NotasServicos")
                             where d.Id == Id
                             select new
                             {
                                 Id = d.Id,
                                 IdCargoNivel = d.IdCargoNivel,
                                 Salario = d.Salario,
                                 DataInicio = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataInicio), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataInicio), 2) + "/" + SqlFunctions.DatePart("year", d.DataInicio).ToString(),
                                 DataFim = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataFim), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataFim), 2) + "/" + SqlFunctions.DatePart("year", d.DataFim).ToString(),
                                 MotivoMudanca = d.MotivoMudanca,
                                 DataCadastro = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataCadastro), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataCadastro), 2) + "/" + SqlFunctions.DatePart("year", d.DataCadastro).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataCadastro), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataCadastro), 2),
                                 UsuarioCadastro = d.UsuarioCadastro,
                                 DataAlteracao = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataAlteracao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataAlteracao), 2) + "/" + SqlFunctions.DatePart("year", d.DataAlteracao).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataAlteracao), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataAlteracao), 2),
                                 UsuarioAlteracao = d.UsuarioAlteracao
                             }).ToList()
                            .Select(x => new
                            {
                                Id = x.Id,
                                IdCargoNivel = x.IdCargoNivel,
                                Salario = (x.Salario != null ? x.Salario.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : "0,00"),
                                DataInicio = x.DataInicio,
                                DataFim = x.DataFim,
                                MotivoMudanca = x.MotivoMudanca,
                                DataCadastro = x.DataCadastro,
                                UsuarioCadastro = x.UsuarioCadastro,
                                DataAlteracao = x.DataAlteracao,
                                UsuarioAlteracao = x.UsuarioAlteracao
                            });

                return query.ToArray();

            }
        }

        public static List<RHCargosSalarios> Listar(Int64 IdCargo)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.RHCargosSalarios
                            where d.IdCargoNivel == IdCargo
                            orderby d.DataInicio descending
                            select d;

                return query.ToList();
            }
        }




        //public static Array Consultar()
        //{
        //    using (var context = new UranusEntities())
        //    {
        //        var query = from d in context.RHCargosSalarios
        //                    select new
        //                    {
        //                        Id = d.Id,
        //                        Nome = d.Nome
        //                    };

        //        return query.ToArray();
        //    }
        //}


    }
}