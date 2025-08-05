using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class RHCargosNiveisBo : IRequiresSessionState
    {
        public static Int32 Inserir(RHCargosNiveis cargos)
        {
            using (var context = new UranusEntities())
            {
                context.RHCargosNiveis.Add(cargos);
                context.SaveChanges();

                return cargos.Id;
            }
        }

        public static void Salvar(RHCargosNiveis cargos)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(cargos).State = EntityState.Modified;
                context.Entry(cargos).Property(x => x.DataCadastro).IsModified = false;
                context.Entry(cargos).Property(x => x.UsuarioCadastro).IsModified = false;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var cargos = context.RHCargosNiveis.Find(id);
                    context.RHCargosNiveis.Attach(cargos);
                    context.RHCargosNiveis.Remove(cargos);
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

        public static List<RHCargosNiveis> Listar(string search)
        {
            using (var context = new UranusEntities())
            {

                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.RHCargosNiveis
                                orderby d.Nome ascending
                                select d;

                    return query.Distinct().ToList();
                }
                else
                {
                    var query = from d in context.RHCargosNiveis
                                where d.Nome.Contains(search)
                                orderby d.Nome ascending
                                select d;

                    return query.Distinct().ToList();
                }
            }
        }

        public static RHCargosNiveis Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.RHCargosNiveis
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.RHCargosNiveis
                            where d.Id == Id
                            select new
                            {
                                Id = d.Id,
                                Nome = d.Nome,
                                Descricao = d.Descricao,
                                Atribuicoes = d.Atribuicoes,
                                IdAreaResponsavel = d.IdAreaResponsavel,
                                IdProximoCargo = d.IdProximoCargo,
                                DataCadastro = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataCadastro), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataCadastro), 2) + "/" + SqlFunctions.DatePart("year", d.DataCadastro).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataCadastro), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataCadastro), 2),
                                UsuarioCadastro = d.UsuarioCadastro,
                                DataAlteracao = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataAlteracao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataAlteracao), 2) + "/" + SqlFunctions.DatePart("year", d.DataAlteracao).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataAlteracao), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataAlteracao), 2),
                                UsuarioAlteracao = d.UsuarioAlteracao
                            };

                return query.ToArray();
            }
        }

        public static Array ListarArray()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.RHCargosNiveis
                            orderby d.Nome ascending
                            select new
                            {
                                Id = d.Id,
                                Nome = d.Nome,
                            };

                return query.ToArray();
            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.RHCargosNiveis
                            orderby d.Nome ascending
                            select new
                            {
                                Id = d.Id,
                                Nome = d.Nome
                            };

                return query.ToArray();
            }
        }

        public static RHCargosNiveis ConsultarNome(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.RHCargosNiveis
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static List<RHCargosNiveis> ListarNiveis(Int32 IdCargo)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.RHCargosNiveis
                            where d.IdCargo == IdCargo
                            orderby d.Nome ascending
                            select d;

                return query.ToList();
            }
        }
    }
}