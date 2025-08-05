using System;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.SqlServer;

namespace Uranus.Business
{
    public class ProfissionaisAvaliacoesBo : IRequiresSessionState
    {

        public static Int32 Inserir(ProfissionaisAvaliacoes profissionaisavaliacoes)
        {
            using (var context = new UranusEntities())
            {
                context.ProfissionaisAvaliacoes.Add(profissionaisavaliacoes);
                context.SaveChanges();

                return profissionaisavaliacoes.Id;
            }
        }


        public static void Salvar(ProfissionaisAvaliacoes profissionaisavaliacoes)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(profissionaisavaliacoes).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var profissionaisavaliacoes = context.ProfissionaisAvaliacoes.Find(id);
                    context.ProfissionaisAvaliacoes.Attach(profissionaisavaliacoes);
                    context.ProfissionaisAvaliacoes.Remove(profissionaisavaliacoes);
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

        public static List<ProfissionaisAvaliacoes> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.ProfissionaisAvaliacoes.Include("Profissionais")
                                                                    .Include("Profissionais.Pessoas")
                                orderby d.Profissionais.Pessoas.Nome ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.ProfissionaisAvaliacoes.Include("Profissionais")
                                                                         .Include("Profissionais.Pessoas")
                                where d.Profissionais.Pessoas.Nome.Contains(search)
                                orderby d.Profissionais.Pessoas.Nome ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static List<ProfissionaisAvaliacoes> Listar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisAvaliacoes
                            orderby d.Profissionais.Pessoas.Nome ascending
                            select d;

                return query.ToList();
            }
        }


        
        public static List<ProfissionaisAvaliacoes> ListarAAvaliacoes(Int32 IdProfissional)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisAvaliacoes
                            where d.IdProfissional == IdProfissional
                            select d;

                return query.ToList();
            }
        }

        public static ProfissionaisAvaliacoes Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisAvaliacoes.Include("Profissionais")
                                                                    .Include("Profissionais.Pessoas")
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisAvaliacoes.Include("Profissionais")
                                                                    .Include("Profissionais.Pessoas")
                            where d.Id == Id
                            select new
                            {
                                Id = d.Id,
                                IdProfissional = d.IdProfissional,
                                Nome = d.Profissionais.Pessoas.Nome,
                                Data = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.Data), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.Data), 2) + "/" + SqlFunctions.DatePart("year", d.Data).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.Data), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.Data), 2),
                                Avaliador = d.Avaliador,
                                Descricao = d.Descricao
                            };

                return query.ToArray();
            }
        }

        public static Array ListarArray(String Data)
        {
            if (!String.IsNullOrEmpty(Data) && Data.Length == 10)
            {
                var date = DateTime.Parse(Data);

                using (var context = new UranusEntities())
                {
                    var query = from d in context.ProfissionaisAvaliacoes.Include("Profissionais")
                                                                        .Include("Profissionais.Pessoas")
                                orderby d.Profissionais.Pessoas.Nome ascending
                                select new
                                {
                                    Id = d.Id,
                                    Nome = d.Profissionais.Pessoas.Nome,
                                };

                    return query.ToArray();
                }
            }
            else
            {
                using (var context = new UranusEntities())
                {
                    var query = from d in context.ProfissionaisAvaliacoes.Include("Profissionais")
                                                                        .Include("Profissionais.Pessoas")
                                orderby d.Profissionais.Pessoas.Nome ascending
                                select new
                                {
                                    Id = d.Id,
                                    Nome = d.Profissionais.Pessoas.Nome,
                                };

                    return query.ToArray();
                }
            }
        }

        public static ProfissionaisAvaliacoes Buscar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisAvaliacoes.Include("Profissionais")
                                                                    .Include("Profissionais.Pessoas")
                            select d;

                return query.FirstOrDefault();
            }
        }

    }
}