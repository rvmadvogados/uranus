using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class AcaoBo : IRequiresSessionState
    {

        public class Select2Model
        {
            public string id { get; set; }
            public string text { get; set; }
        }

        public static List<Select2Model> GetAcoesList(string search, bool filter)
        {
            using (var context = new UranusEntities())
            {
                context.Database.CommandTimeout = 360;

                search = PessoasBo.ConverteNome(search);

                var query = (from d in context.ProcessosCadastroAcao.Include("Pessoas")
                             where d.NomeBusca.Contains(search)
                             orderby d.Acao ascending
                             select new
                             {
                                 id = (filter ? d.Acao.Trim() : d.ID.ToString()),
                                 text = d.Acao
                             }).AsEnumerable().Select(B => new Select2Model()
                             {
                                 id = B.id.ToString(),
                                 text = B.text
                             });

                var xxx = query.ToList();

                return query.ToList();
            }
        }


        public static Int32 Inserir(ProcessosCadastroAcao acao)
        {
            using (var context = new UranusEntities())
            {
                context.ProcessosCadastroAcao.Add(acao);
                context.SaveChanges();

                return acao.ID;
            }
        }

        public static void Salvar(ProcessosCadastroAcao acao)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(acao).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int32 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var acao = context.ProcessosCadastroAcao.Find(id);
                    context.ProcessosCadastroAcao.Attach(acao);
                    context.ProcessosCadastroAcao.Remove(acao);
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

        public static List<ProcessosCadastroAcao> Listar(string search)
        {
            using (var context = new UranusEntities())
            {
                var nome = PessoasBo.ConverteNome(search);

                if (String.IsNullOrEmpty(search.Trim()))
                {
                    var query = from d in context.ProcessosCadastroAcao
                                orderby d.Acao ascending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.ProcessosCadastroAcao
                                where d.NomeBusca.Contains(nome)
                                orderby d.Acao ascending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static ProcessosCadastroAcao Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosCadastroAcao
                            where d.ID == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosCadastroAcao
                            where d.ID == Id
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Acao
                            };

                return query.ToArray();
            }
        }

        public static Array Consultar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosCadastroAcao
                            orderby d.Acao ascending
                            select new
                            {
                                Id = d.ID,
                                Nome = d.Acao
                            };

                return query.ToArray();
            }
        }
    }
}