using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.SessionState;
using Uranus.Data;
using Uranus.Domain;
using Uranus.Domain.Entities;

namespace Uranus.Business
{
    public class ProcessosEventosPendentesBo : IRequiresSessionState
    {
        public static String Inserir(String origem, List<ProcessosEventosPendentes> pendentes)
        {
            string Log = "Application";
            string Source = "Uranus.WindowsService";
            string Publicacoes = string.Empty;

            try
            {
                List<ProcessosEventosPendentes> RemoveExisting = new List<ProcessosEventosPendentes>();

                foreach (ProcessosEventosPendentes item in pendentes)
                {
                    if (origem == "WEBJUR")
                    {
                        Publicacoes += (Publicacoes.Length > 0 ? ConfigurationManager.AppSettings["WebServiceParameterWEBJUR"] : string.Empty);
                        Publicacoes += item.codPublicacao;
                    }

                    var result = Consultar(
                        item.Origem,
                        item.Processo,
                        item.Orgao,
                        item.Partes,
                        item.Classe,
                        item.Assunto,
                        item.EventoPrazo,
                        item.DataEnvioRequisicao,
                        item.DataPrazoInicio,
                        item.DataPrazoFinal,
                        item.anoPublicacao,
                        item.codPublicacao,
                        item.edicaoDiario,
                        item.descricaoDiario,
                        item.paginaInicial,
                        item.paginaFinal,
                        item.dataPublicacao,
                        item.dataDivulgacao,
                        item.dataCadastro,
                        item.numeroProcesso,
                        item.ufPublicacao,
                        item.cidadePublicacao,
                        item.orgaoDescricao,
                        item.varaDescricao,
                        item.despachoPublicacao,
                        item.processoPublicacao,
                        item.publicacaoCorrigida,
                        item.codVinculo,
                        item.nomeVinculo,
                        item.OABNumero,
                        item.OABEstado,
                        item.comarca,
                        item.vara,
                        item.tituloAcao,
                        item.numeroNota,
                        item.ata,
                        item.descricao,
                        item.dtdisponibilizacao,
                        item.IdAdvogadoStatus
                    );

                    if (result != null)
                    {
                        RemoveExisting.Add(item);
                    }
                }

                foreach (ProcessosEventosPendentes item in RemoveExisting)
                {
                    pendentes.Remove(item);
                }

                if (pendentes.Count > 0)
                {
                    using (var context = new UranusEntities())
                    {
                        using (var transactionContext = context.Database.BeginTransaction())
                        {
                            context.Set<ProcessosEventosPendentes>().AddRange(pendentes);
                            context.SaveChanges();

                            transactionContext.Commit();
                        }
                    }

                    if (origem == "WEBJUR")
                    {
                        return Publicacoes;
                    }
                    else
                    {
                        return "Success";
                    }
                }
                else
                {
                    if (origem == "WEBJUR")
                    {
                        return Publicacoes;
                    }
                    else
                    {
                        return "Error";
                    }
                }
            }
            catch (Exception ex)
            {
#if (!DEBUG)
                    using (EventLog eventLog = new EventLog(Log))
                    {
                        eventLog.Source = Source;
                        eventLog.WriteEntry(String.Format("Método Insert, Erro: {0}", ex.Message), EventLogEntryType.Error);
                    }
#endif

                return "Error";
            }
        }

        public static void Salvar(ProcessosEventosPendentes evento)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(evento).State = EntityState.Modified;
                context.Entry(evento).Property(x => x.Origem).IsModified = false;
                context.Entry(evento).Property(x => x.Processo).IsModified = false;
                context.Entry(evento).Property(x => x.Orgao).IsModified = false;
                context.Entry(evento).Property(x => x.Partes).IsModified = false;
                context.Entry(evento).Property(x => x.Classe).IsModified = false;
                context.Entry(evento).Property(x => x.Assunto).IsModified = false;
                context.Entry(evento).Property(x => x.EventoPrazo).IsModified = false;
                context.Entry(evento).Property(x => x.DataEnvioRequisicao).IsModified = false;
                context.Entry(evento).Property(x => x.DataPrazoInicio).IsModified = false;
                context.Entry(evento).Property(x => x.DataPrazoFinal).IsModified = false;
                context.Entry(evento).Property(x => x.anoPublicacao).IsModified = false;
                context.Entry(evento).Property(x => x.codPublicacao).IsModified = false;
                context.Entry(evento).Property(x => x.edicaoDiario).IsModified = false;
                context.Entry(evento).Property(x => x.descricaoDiario).IsModified = false;
                context.Entry(evento).Property(x => x.paginaInicial).IsModified = false;
                context.Entry(evento).Property(x => x.paginaFinal).IsModified = false;
                context.Entry(evento).Property(x => x.dataPublicacao).IsModified = false;
                context.Entry(evento).Property(x => x.dataDivulgacao).IsModified = false;
                context.Entry(evento).Property(x => x.dataCadastro).IsModified = false;
                context.Entry(evento).Property(x => x.numeroProcesso).IsModified = false;
                context.Entry(evento).Property(x => x.ufPublicacao).IsModified = false;
                context.Entry(evento).Property(x => x.cidadePublicacao).IsModified = false;
                context.Entry(evento).Property(x => x.orgaoDescricao).IsModified = false;
                context.Entry(evento).Property(x => x.varaDescricao).IsModified = false;
                context.Entry(evento).Property(x => x.despachoPublicacao).IsModified = false;
                context.Entry(evento).Property(x => x.processoPublicacao).IsModified = false;
                context.Entry(evento).Property(x => x.publicacaoCorrigida).IsModified = false;
                context.Entry(evento).Property(x => x.codVinculo).IsModified = false;
                context.Entry(evento).Property(x => x.nomeVinculo).IsModified = false;
                context.Entry(evento).Property(x => x.OABNumero).IsModified = false;
                context.Entry(evento).Property(x => x.OABEstado).IsModified = false;
                context.Entry(evento).Property(x => x.comarca).IsModified = false;
                context.Entry(evento).Property(x => x.vara).IsModified = false;
                context.Entry(evento).Property(x => x.tituloAcao).IsModified = false;
                context.Entry(evento).Property(x => x.numeroNota).IsModified = false;
                context.Entry(evento).Property(x => x.ata).IsModified = false;
                context.Entry(evento).Property(x => x.descricao).IsModified = false;
                context.Entry(evento).Property(x => x.dtdisponibilizacao).IsModified = false;
                context.Entry(evento).Property(x => x.IdAdvogadoStatus).IsModified = false;
                context.Entry(evento).Property(x => x.NumeroProcessoOriginal).IsModified = false;

                context.SaveChanges();
            }
        }

        public static ProcessosEventosPendentes Consultar(
            String Origem,
            String Processo,
            String Orgao,
            String Partes,
            String Classe,
            String Assunto,
            String EventoPrazo,
            String DataEnvioRequisicao,
            String DataPrazoInicio,
            String DataPrazoFinal,
            String anoPublicacao,
            String codPublicacao,
            String edicaoDiario,
            String descricaoDiario,
            String paginaInicial,
            String paginaFinal,
            String dataPublicacao,
            String dataDivulgacao,
            String dataCadastro,
            String numeroProcesso,
            String ufPublicacao,
            String cidadePublicacao,
            String orgaoDescricao,
            String varaDescricao,
            String despachoPublicacao,
            String processoPublicacao,
            String publicacaoCorrigida,
            String codVinculo,
            String nomeVinculo,
            String OABNumero,
            String OABEstado,
            String comarca,
            String vara,
            String tituloAcao,
            String numeroNota,
            String ata,
            String descricao,
            String dtdisponibilizacao,
            String IdAdvogadoStatus
        )
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosEventosPendentes
                            where d.Origem == Origem
                            where d.Processo == Processo
                            where d.Orgao == Orgao
                            where d.Partes == Partes
                            where d.Classe == Classe
                            where d.Assunto == Assunto
                            where d.EventoPrazo == EventoPrazo
                            where d.DataEnvioRequisicao == DataEnvioRequisicao
                            where d.DataPrazoInicio == DataPrazoInicio
                            where d.DataPrazoFinal == DataPrazoFinal
                            where d.anoPublicacao == anoPublicacao
                            where d.codPublicacao == codPublicacao
                            where d.edicaoDiario == edicaoDiario
                            where d.descricaoDiario == descricaoDiario
                            where d.paginaInicial == paginaInicial
                            where d.paginaFinal == paginaFinal
                            where d.dataPublicacao == dataPublicacao
                            where d.dataDivulgacao == dataDivulgacao
                            where d.dataCadastro == dataCadastro
                            where d.numeroProcesso == numeroProcesso
                            where d.ufPublicacao == ufPublicacao
                            where d.cidadePublicacao == cidadePublicacao
                            where d.orgaoDescricao == orgaoDescricao
                            where d.varaDescricao == varaDescricao
                            where d.despachoPublicacao == despachoPublicacao
                            where d.processoPublicacao == processoPublicacao
                            where d.publicacaoCorrigida == publicacaoCorrigida
                            where d.codVinculo == codVinculo
                            where d.nomeVinculo == nomeVinculo
                            where d.OABNumero == OABNumero
                            where d.OABEstado == OABEstado
                            where d.comarca == comarca
                            where d.vara == vara
                            where d.tituloAcao == tituloAcao
                            where d.numeroNota == numeroNota
                            where d.ata == ata
                            where d.descricao == descricao
                            where d.dtdisponibilizacao == dtdisponibilizacao
                            where d.IdAdvogadoStatus == IdAdvogadoStatus
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static List<CaptureEProc> BuscarEProc(string search)
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.ProcessosEventosPendentes
                             where d.Origem == "E-PROC"
                             where d.Integrado == false
                             where d.Excluido != true
                             where d.Processo.Contains(search)
                             select d)
                            .GroupJoin(context.ProcessosAcoes, e => e.Processo, c => c.NumeroProcesso,
                            (ProcessosEventosPendentes, ProcessosAcoes) => new { E = ProcessosEventosPendentes, C = ProcessosAcoes.DefaultIfEmpty() })
                            .SelectMany(final => final.C, (final, ex) => new
                            {
                                Id = final.E.Id,
                                PublicationDate = (final.E.DataPrazoInicio != null ? final.E.DataPrazoInicio.Substring(0, 10) : string.Empty),
                                ProcessNumber = final.E.Processo,
                                Client = final.E.Partes,
                                Organ = final.E.Orgao,
                                Class = final.E.Classe,
                                Subject = final.E.Assunto,
                                IdProcessAction = ex.ID.ToString(),
                                HasProcess = (ex.NumeroProcesso == null ? false : true)
                            }).ToList()
                            .Select(x => new
                            {
                                Id = x.Id,
                                PublicationDate = x.PublicationDate,
                                ProcessNumber = x.ProcessNumber,
                                Client = BuscarCliente(x.ProcessNumber, x.Client),
                                Organ = x.Organ,
                                Class = x.Class,
                                Subject = x.Subject,
                                IdProcessAction = x.IdProcessAction,
                                HasProcess = x.HasProcess
                            }).ToList();

                List<CaptureEProc> captures = new List<CaptureEProc>();

                if (query.Count > 0)
                {
                    var jsonString = JsonConvert.SerializeObject(query);

                    captures = JsonConvert.DeserializeObject<List<CaptureEProc>>(jsonString);
                }

                return captures.Take(5000).OrderByDescending(x => x.HasProcess).ThenBy(x => x.PublicationDate).Distinct().ToList();
            }
        }

        public static List<CaptureWebJur> BuscarWebJur(string search)
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.ProcessosEventosPendentes
                             where d.Origem == "WEBJUR"
                             where d.Integrado == false
                             where d.Excluido != true
                             where d.numeroProcesso.Contains(search)
                             select d)
                            .GroupJoin(context.ProcessosAcoes, e => e.numeroProcesso, c => c.NumeroProcesso,
                            (ProcessosEventosPendentes, ProcessosAcoes) => new { E = ProcessosEventosPendentes, C = ProcessosAcoes.DefaultIfEmpty() })
                            .SelectMany(final => final.C, (final, ex) => new
                            {
                                Id = final.E.Id,
                                PublicationDate = (final.E.dataPublicacao != null ? final.E.dataPublicacao : string.Empty),
                                ProcessNumber = final.E.numeroProcesso,
                                Client = string.Empty,
                                Organ = final.E.descricaoDiario,
                                Stick = final.E.varaDescricao,
                                PublicationCode = final.E.codPublicacao,
                                IdProcessAction = ex.ID.ToString(),
                                HasProcess = (ex.NumeroProcesso == null ? false : true),
                                processoPublicacao = final.E.processoPublicacao,
                                orgaoDescricao = final.E.orgaoDescricao
                            }).ToList()
                            .Select(x => new
                            {
                                Id = x.Id,
                                PublicationDate = x.PublicationDate,
                                ProcessNumber = BuscarNumeroProcesso(x.ProcessNumber, x.orgaoDescricao, x.processoPublicacao),
                                Client = BuscarCliente(BuscarNumeroProcesso(x.ProcessNumber, x.orgaoDescricao, x.processoPublicacao), x.Client),
                                Organ = x.Organ,
                                Stick = x.Stick,
                                PublicationCode = x.PublicationCode,
                                IdProcessAction = x.IdProcessAction,
                                HasProcess = x.HasProcess
                            }).ToList();

                List<CaptureWebJur> captures = new List<CaptureWebJur>();

                if (query.Count > 0)
                {
                    var jsonString = JsonConvert.SerializeObject(query);

                    captures = JsonConvert.DeserializeObject<List<CaptureWebJur>>(jsonString);
                }

                return captures.Take(5000).OrderByDescending(x => x.HasProcess).ThenBy(x => x.PublicationDate).Distinct().ToList();
            }
        }

        public static List<CaptureOAB> BuscarOAB(string search)
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.ProcessosEventosPendentes
                             where d.Origem == "OAB"
                             where d.Integrado == false
                             where d.Excluido != true
                             where d.numeroProcesso.Contains(search)
                             select d)
                            .GroupJoin(context.ProcessosAcoes, e => e.numeroProcesso, c => c.NumeroProcesso,
                            (ProcessosEventosPendentes, ProcessosAcoes) => new { E = ProcessosEventosPendentes, C = ProcessosAcoes.DefaultIfEmpty() })
                            .SelectMany(final => final.C, (final, ex) => new
                            {
                                Id = final.E.Id,
                                PublicationDate = (final.E.dtdisponibilizacao != null ? final.E.dtdisponibilizacao : string.Empty),
                                ProcessNumber = final.E.numeroProcesso,
                                Client = string.Empty,
                                County = final.E.comarca,
                                Stick = final.E.vara,
                                IdProcessAction = ex.ID.ToString(),
                                HasProcess = (ex.NumeroProcesso == null ? false : true)
                            }).ToList()
                            .Select(x => new
                            {
                                Id = x.Id,
                                PublicationDate = x.PublicationDate,
                                ProcessNumber = x.ProcessNumber,
                                Client = BuscarCliente(x.ProcessNumber, x.Client),
                                County = x.County,
                                Stick = x.Stick,
                                IdProcessAction = x.IdProcessAction,
                                HasProcess = x.HasProcess
                            }).ToList();

                List<CaptureOAB> captures = new List<CaptureOAB>();

                if (query.Count > 0)
                {
                    var jsonString = JsonConvert.SerializeObject(query);

                    captures = JsonConvert.DeserializeObject<List<CaptureOAB>>(jsonString);
                }

                return captures.Take(5000).OrderByDescending(x => x.HasProcess).ThenBy(x => x.PublicationDate).Distinct().ToList();
            }
        }

        public static Array ConsultarArray(Int64 Id, Int16 StartDays, Int16 EndDays)
        {
            using (var context = new UranusEntities())
            {
                var query = (from d in context.ProcessosEventosPendentes
                             where d.Id == Id
                             select new
                             {
                                 Id = d.Id,
                                 Processo = d.Processo,
                                 DataPrazoInicio = (d.DataPrazoInicio != null ? d.DataPrazoInicio.Substring(0, 10) : null),
                                 DataPrazoFinal = (d.DataPrazoFinal != null ? d.DataPrazoFinal.Substring(0, 10) : null),
                                 EventoPrazo = d.EventoPrazo,
                                 numeroProcesso = d.numeroProcesso,
                                 ProcessoPublicacao = ("Data Publicação: " + d.dataPublicacao + ", Data Disponibilização: " + d.dataDivulgacao + ", " + d.processoPublicacao),
                                 Descricao = d.descricao,
                                 processoPublicacao = d.processoPublicacao,
                                 orgaoDescricao = d.orgaoDescricao
                             }).ToList()
                            .Select(x => new
                            {
                                Id = x.Id,
                                Processo = x.Processo,
                                DataPrazoInicio = (x.DataPrazoInicio != null ? AddBusinessDays(DateTime.Parse(x.DataPrazoInicio.Substring(0, 10)), StartDays) : String.Empty),
                                DataPrazoFinal = (x.DataPrazoInicio != null ? (DateDiff(AddBusinessDays(DateTime.Parse(x.DataPrazoInicio.Substring(0, 10)), StartDays), AddBusinessDays(DateTime.Parse(x.DataPrazoFinal.Substring(0, 10)), EndDays)) > 0 ? AddBusinessDays(DateTime.Parse(x.DataPrazoFinal.Substring(0, 10)), EndDays) : String.Empty) : String.Empty),
                                EventoPrazo = x.EventoPrazo,
                                numeroProcesso = BuscarNumeroProcesso(x.numeroProcesso, x.orgaoDescricao, x.processoPublicacao),
                                ProcessoPublicacao = x.ProcessoPublicacao,
                                Descricao = x.Descricao
                            });

                return query.ToArray();
            }
        }

        public static string AddBusinessDays(DateTime Date, Int16 Days)
        {
            using (var context = new UranusEntities())
            {
                return context.Database.SqlQuery<DateTime>("SELECT [dbo].[fnAdicionarDiasUteis](CAST('" + Date.ToString("yyyy-MM-dd") + "' AS DATE), " + Days + ")").FirstOrDefault().ToString("dd/MM/yyyy");
            }
        }

        public static double DateDiff(String StartDate, String EndDate)
        {
            double Days = (DateTime.Parse(EndDate) - DateTime.Parse(StartDate)).TotalDays;

            return Days;
        }

        public static ProcessosEventosPendentes Consultar(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosEventosPendentes
                            where d.Id == Id
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static string Excluir(ProcessosEventosPendentes evento)
        {
            try
            {
                using (var context = new UranusEntities())
                {
                    context.Entry(evento).State = EntityState.Modified;
                    context.Entry(evento).Property(x => x.Origem).IsModified = false;
                    context.Entry(evento).Property(x => x.Integrado).IsModified = false;
                    context.Entry(evento).Property(x => x.Processo).IsModified = false;
                    context.Entry(evento).Property(x => x.Orgao).IsModified = false;
                    context.Entry(evento).Property(x => x.Partes).IsModified = false;
                    context.Entry(evento).Property(x => x.Classe).IsModified = false;
                    context.Entry(evento).Property(x => x.Assunto).IsModified = false;
                    context.Entry(evento).Property(x => x.EventoPrazo).IsModified = false;
                    context.Entry(evento).Property(x => x.DataEnvioRequisicao).IsModified = false;
                    context.Entry(evento).Property(x => x.DataPrazoInicio).IsModified = false;
                    context.Entry(evento).Property(x => x.DataPrazoFinal).IsModified = false;
                    context.Entry(evento).Property(x => x.anoPublicacao).IsModified = false;
                    context.Entry(evento).Property(x => x.codPublicacao).IsModified = false;
                    context.Entry(evento).Property(x => x.edicaoDiario).IsModified = false;
                    context.Entry(evento).Property(x => x.descricaoDiario).IsModified = false;
                    context.Entry(evento).Property(x => x.paginaInicial).IsModified = false;
                    context.Entry(evento).Property(x => x.paginaFinal).IsModified = false;
                    context.Entry(evento).Property(x => x.dataPublicacao).IsModified = false;
                    context.Entry(evento).Property(x => x.dataDivulgacao).IsModified = false;
                    context.Entry(evento).Property(x => x.dataCadastro).IsModified = false;
                    context.Entry(evento).Property(x => x.numeroProcesso).IsModified = false;
                    context.Entry(evento).Property(x => x.ufPublicacao).IsModified = false;
                    context.Entry(evento).Property(x => x.cidadePublicacao).IsModified = false;
                    context.Entry(evento).Property(x => x.orgaoDescricao).IsModified = false;
                    context.Entry(evento).Property(x => x.varaDescricao).IsModified = false;
                    context.Entry(evento).Property(x => x.despachoPublicacao).IsModified = false;
                    context.Entry(evento).Property(x => x.processoPublicacao).IsModified = false;
                    context.Entry(evento).Property(x => x.publicacaoCorrigida).IsModified = false;
                    context.Entry(evento).Property(x => x.codVinculo).IsModified = false;
                    context.Entry(evento).Property(x => x.nomeVinculo).IsModified = false;
                    context.Entry(evento).Property(x => x.OABNumero).IsModified = false;
                    context.Entry(evento).Property(x => x.OABEstado).IsModified = false;
                    context.Entry(evento).Property(x => x.comarca).IsModified = false;
                    context.Entry(evento).Property(x => x.vara).IsModified = false;
                    context.Entry(evento).Property(x => x.tituloAcao).IsModified = false;
                    context.Entry(evento).Property(x => x.numeroNota).IsModified = false;
                    context.Entry(evento).Property(x => x.ata).IsModified = false;
                    context.Entry(evento).Property(x => x.descricao).IsModified = false;
                    context.Entry(evento).Property(x => x.dtdisponibilizacao).IsModified = false;
                    context.Entry(evento).Property(x => x.IdAdvogadoStatus).IsModified = false;

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

        public static string ExcluirTodos(String Origem)
        {
            var query = "UPDATE ProcessosEventosPendentes SET Excluido = 1, UsuarioExclusao = '', DataHoraExclusao = GETDATE() WHERE Origem = '" + Origem + "'";
            using (var context = new UranusEntities())
            {
                context.Database.ExecuteSqlCommand(query);
            }

            return "00";
        }

        public static string BuscarCliente(String numeroProcesso, String Nome)
        {
            using (var context = new UranusEntities())
            {
                ProcessosAcoes acao = ProcessosAcoesBo.BuscarNome(numeroProcesso);

                if (acao != null)
                {
                    var autores = acao.Processos.ProcessosAutores.ToList();

                    foreach (var autor in autores)
                    {
                        if (autor.Clientes.Pessoas.Cliente == true)
                        {
                            return autor.Clientes.Pessoas.Nome;
                        }
                    }

                    var partes = acao.Processos.ProcessosPartes.ToList();

                    foreach (var parte in partes)
                    {
                        if (parte.Clientes.Pessoas.Cliente == true)
                        {
                            return parte.Clientes.Pessoas.Nome;
                        }
                    }
                }
            }

            return Nome;
        }

        public static string BuscarNumeroProcesso(String numeroProcesso, String orgaoDescricao, String processoPublicacao)
        {
            if (orgaoDescricao != null && orgaoDescricao.Trim() == "CAPITAL 2º GRAU")
            {
                if (processoPublicacao.Contains("(CNJ") && !processoPublicacao.Contains("(CNJ)"))
                {
                    var info = processoPublicacao.Substring(processoPublicacao.IndexOf("(CNJ") + 4);
                    var value = info.Substring(0, info.IndexOf(")"));
                    var digits = new Regex(@"[^\d]");
                    return digits.Replace(value, "");
                }
            }
            else if (processoPublicacao != null && processoPublicacao.Contains("(CNJ") && !processoPublicacao.Contains("(CNJ)"))
            {
                var info = processoPublicacao.Substring(0, processoPublicacao.IndexOf("("));
                var digits = new Regex(@"[^\d]");
                return digits.Replace(info, "");
            }

            return numeroProcesso;
        }

        public static Int64 InserirLog(LogPrazos logprazos)
        {
            using (var context = new UranusEntities())
            {
                context.LogPrazos.Add(logprazos);
                context.SaveChanges();

                return logprazos.Id;
            }
        }
    }
}