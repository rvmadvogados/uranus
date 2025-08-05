using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class VarasBo : IRequiresSessionState
    {
        public static Int32 Inserir(ProcessosVara vara)
        {
            using (var context = new UranusEntities())
            {
                context.ProcessosVara.Add(vara);
                context.SaveChanges();

                return vara.ID;
            }
        }

        public static void Salvar(ProcessosVara vara)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(vara).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var vara = context.ProcessosVara.Find(id);
                    context.ProcessosVara.Attach(vara);
                    context.ProcessosVara.Remove(vara);
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

        public static List<ProcessosVara> Listar(string nome, string sigla)
        {
            using (var context = new UranusEntities())
            {
                nome = PessoasBo.ConverteNome(nome);


                if (String.IsNullOrEmpty(nome.Trim()) && String.IsNullOrEmpty(sigla.Trim()))
                {
                    var query = from d in context.ProcessosVara
                                orderby d.Vara ascending
                                select d;

                    return query.ToList();
                }
                else if (!String.IsNullOrEmpty(nome.Trim()) && String.IsNullOrEmpty(sigla.Trim()))
                {
                    var query = from d in context.ProcessosVara
                                where d.NomeBusca.Contains(nome)
                                orderby d.Vara ascending
                                select d;

                    return query.ToList();
                }
                else if (String.IsNullOrEmpty(nome.Trim()) && !String.IsNullOrEmpty(sigla.Trim()))
                {
                    var query = from d in context.ProcessosVara
                                where d.Sigla.Contains(sigla)
                                orderby d.Vara ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.ProcessosVara
                                where d.NomeBusca.Contains(nome)
                                where d.Sigla.Contains(sigla)
                                orderby d.Vara ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static ProcessosVara Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosVara
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosVara
                            where d.ID == Id
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Vara,
                                Sigla = d.Sigla
                            };

                return query.ToArray();
            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosVara
                            orderby d.Vara ascending
                            select new
                            {
                                Id = d.ID,
                                Nome = string.Concat(d.Vara.Trim(), (d.Sigla.Trim() != null && d.Sigla.Length > 0 ? " (" + d.Sigla.Trim() + ")" : string.Empty)),
                                Sigla = d.Sigla.Trim()
                            };

                return query.ToArray();
            }
        }
    }
}