using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class EventosBo : IRequiresSessionState
    {

        public class Select2Model
        {
            public string id { get; set; }
            public string text { get; set; }
        }

        public static List<Select2Model> GetEventosList(string search, bool filter)
        {
            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 360;

               // search = PessoasBo.ConverteNome(search);

                var query = (from d in context.ProcessosEventos
                             where d.Descricao.Contains(search)
                             orderby d.Descricao ascending
                             select new
                             {
                                 id = (filter ? d.Descricao.Trim() + d.Codigo : d.ID.ToString()),
                                 text = d.Descricao + d.Codigo
                             }).AsEnumerable().Select(B => new Select2Model()
                             {
                                 id = B.id.ToString(),
                                 text = B.text
                             });

                var xxx = query.ToList();

                return query.ToList();
            }
        }

        public static Int32 Inserir(ProcessosEventos evento)
        {
            using (var context = new UranusEntities())
            {
                context.ProcessosEventos.Add(evento);
                context.SaveChanges();

                return evento.ID;
            }
        }

        public static void Salvar(ProcessosEventos evento)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(evento).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var evento = context.ProcessosEventos.Find(id);
                    context.ProcessosEventos.Attach(evento);
                    context.ProcessosEventos.Remove(evento);
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

        public static List<ProcessosEventos> Listar(string codigo, string nome)
        {
            using (var context = new UranusEntities())
            {
                nome = PessoasBo.ConverteNome(nome);

                if (String.IsNullOrEmpty(codigo.Trim()) && String.IsNullOrEmpty(nome.Trim()))
                {
                    var query = from d in context.ProcessosEventos
                                orderby d.Descricao ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.ProcessosEventos
                                where d.Codigo.Contains(codigo)
                                where d.NomeBusca.Contains(nome)
                                orderby d.Descricao ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static ProcessosEventos Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosEventos
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosEventos
                            where d.ID == Id
                            select new
                            {
                                Id = d.ID,
                                Codigo = d.Codigo,
                                Nome = d.Descricao,
                                Texto = d.Texto,
                                Tipo = d.Tipo,
                                Aplicativo = d.Aplicativo,
                                WhatsApp = d.WhatsApp
                            };

                return query.ToArray();
            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosEventos
                            orderby d.Descricao ascending
                            select new
                            {
                                Id = d.ID,
                                Codigo = d.Codigo,
                                Nome = d.Descricao.Trim() + " (" + d.Codigo.Trim() + ")"
                                //Nome = d.Descricao + " (" + d.Codigo.Trim() + ")"
                            };

                return query.ToArray();
            }
        }
    }
}