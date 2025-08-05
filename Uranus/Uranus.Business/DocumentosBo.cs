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
    public class DocumentosBo : IRequiresSessionState
    {
        public static Int64 Inserir(Documentos documento)
        {
            using (var context = new UranusEntities())
            {
                context.Documentos.Add(documento);
                context.SaveChanges();

                return documento.Id;
            }
        }

        public static void Salvar(Documentos documento)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(documento).State = EntityState.Modified;
                context.Entry(documento).Property(x => x.DataCadastro).IsModified = false;
                context.Entry(documento).Property(x => x.NomeUsuarioCadastro).IsModified = false;
                context.SaveChanges();
            }
        }

        public static string Excluir(Int64 id)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    var documento = context.Documentos.Find(id);
                    context.Documentos.Attach(documento);
                    context.Documentos.Remove(documento);
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

        public static List<Documentos> Listar(string FiltrarData, string FiltrarModelo, string FiltrarCliente)
        {
            using (var context = new UranusEntities())
            {
                if (FiltrarData != null && FiltrarData.Length == 10)
                {
                    var data = DateTime.Parse(FiltrarData);

                    var query = from d in context.Documentos.Include("Modelos")
                                                            .Include("Clientes")
                                                            .Include("Clientes.Pessoas")
                                                            .Include("Profissionais")
                                                            .Include("Profissionais.Pessoas")
                                where EntityFunctions.TruncateTime(d.DataCadastro) == EntityFunctions.TruncateTime(data)
                                where d.Modelos.Nome.Contains(FiltrarModelo)
                                where d.Clientes.Pessoas.Nome.Contains(FiltrarCliente)
                                orderby d.Id descending
                                select d;

                    return query.ToList();
                }
                else
                {
                    var query = from d in context.Documentos.Include("Modelos")
                                                            .Include("Clientes")
                                                            .Include("Clientes.Pessoas")
                                                            .Include("Profissionais")
                                                            .Include("Profissionais.Pessoas")
                                where d.Modelos.Nome.Contains(FiltrarModelo)
                                where d.Clientes.Pessoas.Nome.Contains(FiltrarCliente)
                                orderby d.Id descending
                                select d;

                    return query.ToList();
                }
            }
        }

        public static Documentos Consultar(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Documentos
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Array ConsultarArray(Int64 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Documentos
                            where d.Id == Id
                            select new
                            {
                                Id = d.Id,
                                IdModelo = d.IdModelo,
                                IdCliente = d.IdCliente,
                                IdProfissional = d.IdProfissional,
                                ClienteNome = d.ClienteNome,
                                ClausulaPrimeira = d.ClausulaPrimeira,
                                ClausulaSegundaBeneficios = d.ClausulaSegundaBeneficios,
                                ClausulaSegundaValor = d.ClausulaSegundaValor,
                                ClausulaSegundaParcelas = d.ClausulaSegundaParcelas,
                                ClausulaSegundaVencimentoDia = d.ClausulaSegundaVencimentoDia,
                                ClausulaSegundaVencimento = d.ClausulaSegundaVencimento,
                                ClausulaSegundaPercentual = d.ClausulaSegundaPercentual,
                                EspecificacaoProcuracao = d.EspecificacaoProcuracao,
                                CidadeEstado = d.CidadeEstado,
                                DataPorExtenso = (d.DataPorExtenso != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataPorExtenso), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataPorExtenso), 2) + "/" + SqlFunctions.DatePart("year", d.DataPorExtenso).ToString() : string.Empty),
                                DataCadastro = (d.DataCadastro != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataCadastro), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataCadastro), 2) + "/" + SqlFunctions.DatePart("year", d.DataCadastro).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataCadastro), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataCadastro), 2) : string.Empty),
                                UsuarioCadastro = d.NomeUsuarioCadastro,
                                DataAlteracao = (d.DataAlteracao != null ? DbFunctions.Right("0" + SqlFunctions.DatePart("day", d.DataAlteracao), 2) + "/" + DbFunctions.Right("0" + SqlFunctions.DatePart("month", d.DataAlteracao), 2) + "/" + SqlFunctions.DatePart("year", d.DataAlteracao).ToString() + " " + DbFunctions.Right("0" + SqlFunctions.DatePart("hour", d.DataAlteracao), 2) + ":" + DbFunctions.Right("0" + SqlFunctions.DatePart("minute", d.DataAlteracao), 2) : string.Empty),
                                UsuarioAlteracao = d.NomeUsuarioAlteracao
                            };

                return query.ToArray();
            }
        }
    }
}