using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Business
{
    public class IniciaisBo : IRequiresSessionState
    {
        public static Int32 Inserir(TemplatesIniciaisAposentadoria inicial)
        {
            using (var context = new UranusEntities())
            {
                context.TemplatesIniciaisAposentadoria.Add(inicial);
                context.SaveChanges();

                return inicial.IdTemplate;
            }
        }

        public static void Salvar(TemplatesIniciaisAposentadoria inicial)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(inicial).State = EntityState.Modified;
                context.Entry(inicial).Property(x => x.DataCadastro).IsModified = false;
                context.Entry(inicial).Property(x => x.NomeUsuarioCadastro).IsModified = false;
                context.SaveChanges();
            }
        }
        public static string Excluir(Int64 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var inicial = context.TemplatesIniciaisAposentadoria.Find(id);
                    context.TemplatesIniciaisAposentadoria.Attach(inicial);
                    context.TemplatesIniciaisAposentadoria.Remove(inicial);
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

        public static List<TemplatesIniciaisAposentadoria> Listar(string FiltrarData, string FiltrarCliente)
        {
            using (var context = new UranusEntities())
            {
                if (FiltrarData != null && FiltrarData.Length == 10)
                {
                    var data = DateTime.Parse(FiltrarData);

                    var query = from d in context.TemplatesIniciaisAposentadoria.Include("TemplatesIniciaisAposentadoriaPeriodos")
                                where EntityFunctions.TruncateTime(d.DataCadastro) == EntityFunctions.TruncateTime(data)
                                where d.ClienteNome.Contains(FiltrarCliente)
                                orderby d.IdTemplate descending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.TemplatesIniciaisAposentadoria.Include("TemplatesIniciaisAposentadoriaPeriodos")
                                where d.ClienteNome.Contains(FiltrarCliente)
                                orderby d.IdTemplate descending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static TemplatesIniciaisAposentadoria Consultar(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.TemplatesIniciaisAposentadoria
                            where d.IdTemplate == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.TemplatesIniciaisAposentadoria
                            where d.IdTemplate == Id
                            select new
                            {
                                Id = d.IdTemplate,
                                DataCadastro = (d.DataCadastro != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataCadastro), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataCadastro), 2) + "/" + SqlFunctions.DatePart("year", d.DataCadastro).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataCadastro), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataCadastro), 2) : string.Empty),
                                IdProfissional = d.IdProfissional,
                                IdCliente = d.IdCliente,
                                UsuarioCadastro = d.NomeUsuarioCadastro,
                                DataAlteracao = (d.DataAlteracao != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataAlteracao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataAlteracao), 2) + "/" + SqlFunctions.DatePart("year", d.DataAlteracao).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataAlteracao), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataAlteracao), 2) : string.Empty),
                                UsuarioAlteracao = d.NomeUsuarioAlteracao,
                                ClausulaPrimeira = d.VaraNumero,
                                VaraCidade = d.VaraCidade,
                                ProcessoDataProtocolo = d.ProcessoDataProtocolo,
                                RequerimentoNumero = d.RequerimentoNumero,
                                ProcessoAtividade = d.ProcessoAtividade,
                                ProcessoReafirmacaoDER = d.ProcessoReafirmacaoDER,
                                ProcessoGratuidade = d.ProcessoGratuidade,
                                ProcessoValorCausa = d.ProcessoValorCausa,
                                ProcessoRodapeData = d.ProcessoRodapeData,
                                ProcessoAdvogado = d.ProcessoAdvogado,
                                ProcessoImagem = d.ProcessoImagem,
                                ProcessoOAB = d.ProcessoOAB,
                                ProcessoResumoPeriodo = d.ProcessoResumoPeriodo,
                                ProcessoResumoPeriodosDER = d.ProcessoResumoPeriodosDER,
                                ClienteNome = d.ClienteNome,
                                ClienteNacionalidade = d.ClienteNacionalidade,
                                ClienteEstadoCivil = d.ClienteEstadoCivil,
                                ClienteProfissao = d.ClienteProfissao,
                                ClienteCPF = d.ClienteCPF,
                                ClienteEndereco = d.ClienteEndereco,
                                ClienteBairro = d.ClienteBairro,
                                ClienteCidade = d.ClienteCidade,
                                ClienteEstado = d.ClienteEstado,
                                ClienteCep = d.ClienteCep,
                                ClienteEmail = d.ClienteEmail,
                                ProcessoExcel = d.ProcessoExcel,
                            };

                return query.ToArray();
            }
        }

        public static Int32 InserirPeriodo(TemplatesIniciaisAposentadoriaPeriodos periodo)
        {
            using (var context = new UranusEntities())
            {
                context.TemplatesIniciaisAposentadoriaPeriodos.Add(periodo);
                context.SaveChanges();

                return periodo.IDPeriodo;
            }
        }
        public static void SalvarPeriodos(TemplatesIniciaisAposentadoriaPeriodos periodo)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(periodo).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
        public static string ExcluirPeriodos(Int64 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var templatesiniciaisaposentadoriaperiodos = context.TemplatesIniciaisAposentadoriaPeriodos.Find(id);
                    context.TemplatesIniciaisAposentadoriaPeriodos.Attach(templatesiniciaisaposentadoriaperiodos);
                    context.TemplatesIniciaisAposentadoriaPeriodos.Remove(templatesiniciaisaposentadoriaperiodos);
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
    }
}