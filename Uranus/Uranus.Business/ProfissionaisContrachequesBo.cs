using System;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.SqlServer;
using System.Globalization;

namespace Uranus.Business
{
    public class ProfissionaisContrachequesBo : IRequiresSessionState
    {

        public static Int32 Inserir(ProfissionaisContracheques profissionaiscontracheques)
        {
            using (var context = new UranusEntities())
            {
                context.ProfissionaisContracheques.Add(profissionaiscontracheques);
                context.SaveChanges();

                return profissionaiscontracheques.Id;
            }
        }


        public static void Salvar(ProfissionaisContracheques profissionaiscontracheques)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(profissionaiscontracheques).State = EntityState.Modified;
                context.Entry(profissionaiscontracheques).Property(x => x.DataCadastro).IsModified = false;
                context.Entry(profissionaiscontracheques).Property(x => x.UsuarioCadastro).IsModified = false;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var profissionaiscontracheques = context.ProfissionaisContracheques.Find(id);
                    context.ProfissionaisContracheques.Attach(profissionaiscontracheques);
                    context.ProfissionaisContracheques.Remove(profissionaiscontracheques);
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

        public static List<ProfissionaisContracheques> Listar(string search, Int32 mes, Int32 ano)
        {
            using (var context = new UranusEntities())
            {
                if (String.IsNullOrEmpty(search.Trim())&& mes == 0 && ano == 0)
                {
                    var query = from d in context.ProfissionaisContracheques.Include("Profissionais")
                                                                        .Include("Profissionais.Pessoas")
                                orderby d.Ano, d.Mes descending
                                select d;

                    return query.ToList();
                }
                else
                {
                    if (search.Trim() != "")
                    {
                        var query = from d in context.ProfissionaisContracheques.Include("Profissionais")
                                                                         .Include("Profissionais.Pessoas")
                                    where d.Profissionais.Pessoas.Nome.Contains(search)
                                    orderby d.Ano, d.Mes descending
                                    select d;

                        return query.ToList();
                    }
                    else
                    {
                        var query = from d in context.ProfissionaisContracheques.Include("Profissionais")
                                                                         .Include("Profissionais.Pessoas")
                                    where d.Ano == ano && d.Mes == mes
                                    orderby d.Ano, d.Mes descending
                                    select d;

                        return query.ToList();
                    }
                }
            }
        }

        public static List<ProfissionaisContracheques> Listar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisContracheques
                            orderby d.Profissionais.Pessoas.Nome ascending
                            select d;

                return query.ToList();
            }
        }

        
        public static List<ProfissionaisContracheques> Listar(Int32 IdProfissional)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisContracheques
                            where d.IdProfissional == IdProfissional
                            select d;

                return query.ToList();
            }
        }

        public static ProfissionaisContracheques Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisContracheques.Include("Profissionais")
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
                var query = from d in context.ProfissionaisContracheques.Include("Profissionais")
                                                                    .Include("Profissionais.Pessoas")
                            where d.Id == Id
                            select new
                            {
                                Id = d.Id,
                                IdProfissional = d.IdProfissional,
                                Nome = d.Profissionais.Pessoas.Nome,
                                Ano = d.Ano,
                                Mes = d.Mes,
                                NomeArquivo = d.NomeArquivo,
                                DataCadastro = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataCadastro), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataCadastro), 2) + "/" + SqlFunctions.DatePart("year", d.DataCadastro).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataCadastro), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataCadastro), 2),
                                UsuarioCadastro = d.UsuarioCadastro,
                                DataAlteracao = DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataAlteracao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataAlteracao), 2) + "/" + SqlFunctions.DatePart("year", d.DataAlteracao).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataAlteracao), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataAlteracao), 2),
                                UsuarioAlteracao = d.UsuarioAlteracao
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
                    var query = from d in context.ProfissionaisContracheques.Include("Profissionais")
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
                    var query = from d in context.ProfissionaisContracheques.Include("Profissionais")
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

        public static ProfissionaisContracheques Buscar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProfissionaisContracheques.Include("Profissionais")
                                                                    .Include("Profissionais.Pessoas")
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static List<ProfissionaisContracheques> ListarContracheques(Int32 IdProfissional)
        {
            using (var context = new UranusEntities())
            {
                    var query = from d in context.ProfissionaisContracheques
                                where d.IdProfissional == IdProfissional
                                orderby d.Ano descending, d.Mes descending
                                select d;

                    return query.Take(12).ToList();
            }
        }

    }
}