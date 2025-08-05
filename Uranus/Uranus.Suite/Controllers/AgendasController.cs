using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Common;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class AgendasController : Controller
    {
        // GET: Agenda
        public ActionResult Index()
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Send()
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Period()
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var agenda = AgendasBo.ConsultarArray(Id);
            var result = new { codigo = "00", agenda = agenda };
            return Json(result);
        }

        public Boolean Consultar(Agendas agenda)
        {
            if (AgendasBo.Consultar(agenda.Data, agenda.Hora, agenda.IdTipo, agenda.IdSede, agenda.IdProfissional, agenda.IdUsuario) == null)
            {
                return false;
            }

            return true;
        }

        public Boolean ConsultarSala(Agendas agenda)
        {
            if (AgendasBo.ConsultarSala(agenda.Id, agenda.Data, agenda.Hora, agenda.IdTipo, agenda.IdSede, agenda.IdSala) == null)
            {
                return false;
            }

            return true;
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String Data, String Hora, Int32 IdTipo, Int32 IdSede, Int32? IdSala, Int32 IdProfissional, Int32? IdCliente, String Valor, String Descricao, 
                                 String Cliente, String Telefone, String Encaixe, String Compareceu, Int32? IdTipoIndicacao, String Indicacao, Int32? IdParceiro, Int32? IdProfissionalIndicacao, 
                                 String HoraEncaixe, String Cancelou, Int32? IdFormaPagamento, Int32? IdBanco, Int64? IdReceita, Int32? IdArea)
        {
            #region Auditoria
            //if (IdCliente != null || !String.IsNullOrEmpty(Cliente) || (IdCliente == null && String.IsNullOrEmpty(Cliente) && Id > 0 && AgendasBo.Consultar(Id).IdCliente != null))
            //{
            //    AgendasTipos TipoAgendaAux = TiposAgendasBo.Consultar(IdTipo);
            //    Sedes SedeAux = SedesBo.Consultar(IdSede);
            //    int salaid = (IdSala == null ? 0 : int.Parse(IdSala.ToString()));
            //    Salas SalaAux = SalasBo.Consultar(salaid);
            //    Profissionais Profissional = ProfissionaisBo.Consultar(IdProfissional);
            //    int clienteid = (IdCliente == null ? 0 : int.Parse(IdCliente.ToString()));
            //    Clientes ClienteAux = ClientesBo.Consultar(clienteid);

            //    Auditoria auditoria = new Auditoria();
            //    auditoria.DataHora = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            //    auditoria.Modulo = "Agenda";

            //    if (IdCliente == null && String.IsNullOrEmpty(Cliente))
            //    {
            //        auditoria.Tipo = "Liberada";
            //    }
            //    else
            //    {
            //        auditoria.Tipo = "Agendada";
            //    }

            //    auditoria.Acao = "Alterado";
            //    auditoria.Log = String.Format("<b>Data</b>: {0};<b>Hora</b>: {1};<b>Tipo Agenta</b>: {2};<b>Sede</b>: {3};<b>Sala</b>: {4};<b>Profissional</b>: {5};<b>Cliente</b>: {6};<b>Indicacao</b>: {7};<b>Telefone</b>: {8};<b>Valor</b>: {9};<b>Encaixe</b>: {10};<b>Descrição</b>: {11};<b>Compareceu</b>: {12}", Data, Hora, TipoAgendaAux.Nome, SedeAux.Nome, (SalaAux != null ? SalaAux.Nome : String.Empty), Profissional.Pessoas.Nome, (IdCliente != null ? ClienteAux.Pessoas.Nome : Cliente), Indicacao, Telefone, Valor, (Encaixe == "S" ? "Sim" : "Não"), Descricao, (Compareceu == "S" ? "Não" : "Sim"));
            //    auditoria.Usuario = Sessao.Usuario.Nome;

            //    if (Id == 0)
            //    {
            //        auditoria.Acao = "Inserido";
            //    }

            //    AuditoriaBo.Inserir(auditoria);
            //}
            #endregion

            var IdOld = Id;
            var IdClienteOld = IdCliente;

            Agendas agenda = new Agendas();
            agenda.Id = Id;
            agenda.Data = DateTime.Parse(Data);
            agenda.Hora = Hora; //String.Format("{0}:00", Hora);
            agenda.IdTipo = IdTipo;
            agenda.IdSede = IdSede;
            agenda.IdSala = IdSala;
            agenda.IdProfissional = IdProfissional;
            agenda.IdCliente = IdCliente;
            agenda.ValorConsulta = (!String.IsNullOrEmpty(Valor) ? Decimal.Parse(Valor.Replace("R$ ", String.Empty)) : 0);
            agenda.MotivoConsulta = Descricao;
            agenda.IdUsuario = Sessao.Usuario.ID;
            agenda.Encaixe = (Encaixe == null ? "N" : Encaixe);
            agenda.Compareceu = (Compareceu == "N" ? false : true);
            

            agenda.Cancelou = (Cancelou == "S" ? true : false);

            if (Cancelou == "S")
            {
                agenda.DataCancelamento = DateTime.Now;
            }

            agenda.DataCadastro = DateTime.Now;
            agenda.NomeUsuarioCadastro = Sessao.Usuario.Nome;
            agenda.DataAlteracao = DateTime.Now;
            agenda.NomeUsuarioAlteracao = Sessao.Usuario.Nome;
            agenda.IdFormaPagamento = IdFormaPagamento;
            if (IdBanco != null)
            {
                agenda.IdBanco = IdBanco;
            }

            if (IdArea > 0)
            {
                agenda.IdArea = IdArea;
            }

            if (IdReceita == null)
            {
                IdReceita = 0;
            }
            agenda.IdReceita = IdReceita;

            var title = String.Empty;

            var codigo = "00";
            if (Id == 0 && Consultar(agenda))
            {
                codigo = "99";
            }
            else
            {
                if (IdSala != null && ConsultarSala(agenda))
                {
                    codigo = "-80";
                }
                else
                {
                    if (Id == 0)
                    {
                        Id = AgendasBo.Inserir(agenda);
                        EnviarAgenda(Id, "Agenda incluída para o profissional ");
                    }

                    if (Id > 0)
                    {
                        if (agenda.ValorConsulta > 0 && IdFormaPagamento == null && IdBanco == null && (IdArea == null || IdArea == 0))
                        {
                            codigo = "-60";
                        }
                        else
                        { 
                            if (agenda.ValorConsulta > 0)
                            {
                                var idOrigem = 0;
                                if (agenda.IdBanco > 0)
                                {
                                    idOrigem = FinanceiroOrigemBo.ConsultarOregiemBanco(int.Parse(agenda.IdBanco.ToString())).Id;
                                }
                                else
                                {
                                    idOrigem = FinanceiroOrigemBo.ConsultarOregiemSede(int.Parse(agenda.IdSede.ToString())).Id;
                                }
                                FinanceiroReceitas receita = new FinanceiroReceitas();

                                receita.Id = long.Parse(agenda.IdReceita.ToString());
                                receita.IdOrigem = idOrigem;
                                receita.NumeroDocumento = "";
                                receita.DataDocumento = DateTime.Now;
                                receita.IdFinanceiroTipo = 14;
                                receita.IdCentroCusto = agenda.IdSede;
                                receita.IdArea = agenda.IdArea;
                                receita.IdCliente = agenda.IdCliente;
                                receita.ValorBruto = agenda.ValorConsulta;
                                receita.IRRetido = 0;
                                receita.Valor = agenda.ValorConsulta;
                                receita.DataPagamento = agenda.Data;
                                receita.Observacao = null;
                                receita.Nota = false;

                                if (agenda.IdBanco > 0)
                                {
                                    receita.IdBanco = agenda.IdBanco;
                                }
                                else
                                {
                                    receita.IdCaixa = agenda.IdSede;
                                }
                                receita.IdBancosLancamento = null;
                                receita.DataCadastro = DateTime.Now;
                                receita.NomeUsuarioCadastro = Sessao.Usuario.Nome;
                                receita.DataAlteracao = DateTime.Now;
                                receita.NomeUsuarioAlteracao = Sessao.Usuario.Nome;


                                if (agenda.IdReceita == 0)
                                {
                                    IdReceita = FinanceiroReceitasBo.Inserir(receita);
                                }
                                agenda.IdReceita = IdReceita;
                                receita.Id = long.Parse(agenda.IdReceita.ToString());
                                FinanceiroReceitasBo.Salvar(receita);
                            }


                            agenda.Id = Id;
                            AgendasBo.Salvar(agenda);

                            if (IdOld > 0)
                            {
                                if (Cancelou == "S")
                                {
                                    EnviarAgenda(Id, "Agenda cancelada para o profissional ");
                                }
                                else
                                {
                                    EnviarAgenda(Id, "Agenda alterada para o profissional ");
                                }
                            }

                            var agendaTitulo = AgendasBo.Consultar(Id);

                            if (agendaTitulo != null)
                            {
                                title = String.Format("{0}{1} - {2} - {3}{4}", agendaTitulo.Profissionais.Pessoas.Nome, (agendaTitulo.Clientes != null ? String.Format(" - {0}", agendaTitulo.Clientes.Pessoas.Nome) : String.Empty), agendaTitulo.AgendasTipos.Nome, agendaTitulo.Sedes.Nome, (agendaTitulo.Salas != null ? String.Format(" - {0}", agendaTitulo.Salas.Nome) : String.Empty));
                            }
                        }
                    }
                    else
                    {
                        codigo = Id.ToString();
                        Id = 0;
                    }
                }
            }

            string evento = string.Empty;

            if (!String.IsNullOrEmpty(HoraEncaixe))
            {
                var agendado = AgendasBo.Consultar(Id);

                Agendas agendaEncaixe = new Agendas();
                agendaEncaixe.Id = 0;
                agendaEncaixe.Data = agendado.Data;
                agendaEncaixe.Hora = HoraEncaixe;
                agendaEncaixe.IdTipo = agendado.IdTipo;
                agendaEncaixe.IdSede = agendado.IdSede;
                agendaEncaixe.IdSala = agendado.IdSala;
                agendaEncaixe.IdProfissional = agendado.IdProfissional;
                agendaEncaixe.IdUsuario = Sessao.Usuario.ID;
                agendaEncaixe.Encaixe = "S";

                var EventoId = AgendasBo.Inserir(agendaEncaixe);

                evento = "{ \"id\": " + EventoId + ", \"title\": \"" + string.Format("{0} - {1} - {2}{3}", agendado.Profissionais.Pessoas.Nome, agendado.AgendasTipos.Nome, agendado.Sedes.Nome, (agendado.Salas != null ? string.Format(" - {0}", agendado.Salas.Nome) : string.Empty)) + "\"";

                var started = DateTime.Parse(string.Format("{0} {1}", agendado.Data.ToString("yyyy-MM-dd"), HoraEncaixe));
                var ended = started.AddHours(1);
                ended = started.AddMinutes(15);

                evento = evento + ", \"start\": \"" + started.ToString("yyyy-MM-ddTHH:mm:00") + "\", \"end\": \"" + ended.ToString("yyyy-MM-ddTHH:mm:00") + "\"";

                evento = evento + ", \"allDay\": false ";

                evento = evento + ", \"color\": \"#D5F5E3\" }";

            }

            var result = new { codigo = codigo, id = Id, title = title, evento = evento };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            Agendas agenda = AgendasBo.Consultar(Id);

            if (Sessao.Usuario.Nivel == 5 || agenda.NomeUsuarioCadastro == null || agenda.NomeUsuarioCadastro == Sessao.Usuario.Nome)
            {

                #region Auditoria
                //if (agenda.Clientes != null)
                //{
                //    Auditoria auditoria = new Auditoria();
                //    auditoria.DataHora = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                //    auditoria.Modulo = "Agenda";
                //    auditoria.Tipo = "Cancelada";
                //    auditoria.Acao = "Excluído";
                //    auditoria.Log = String.Format("<b>Data</b>: {0};<b>Hora</b>: {1};<b>Tipo Agenta</b>: {2};<b>Sede</b>: {3};<b>Sala</b>: {4};<b>Profissional</b>: {5};<b>Cliente</b>: {6};<b>Indicacao</b>: {7};<b>Valor</b>: {8};<b>Encaixe</b>: {9};<b>Descrição</b>: {10};<b>Compareceu</b>: {11};", agenda.Data.ToString("dd/MM/yyyy"), agenda.Hora.Substring(0, 5), agenda.AgendasTipos.Nome, agenda.Sedes.Nome, (agenda.Salas != null ? agenda.Salas.Nome : String.Empty), agenda.Profissionais.Pessoas.Nome, agenda.Clientes.Pessoas.Nome, agenda.Clientes.Indicacao, agenda.ValorConsulta, (agenda.Encaixe == "S" ? "Sim" : "Não"), agenda.MotivoConsulta, (agenda.Compareceu == true ? "Sim" : "Não"));
                //    auditoria.Usuario = Sessao.Usuario.Nome;

                //    AuditoriaBo.Inserir(auditoria);
                //}
                #endregion

                EnviarAgenda(Id, "Agenda cancelada para o profissional ");
                var codigo = AgendasBo.Excluir(Id);

                var result = new { codigo = codigo };
                return Json(result);
            }
            else
            {

                // conferir mensagem de exclusão da agenda
                var result = new { codigo = "Não é possível executar essa operação" };
                return Json(result);

            }
        }

        [HttpPost]
        public string Eventos(int? IdSede, int? IdProfissional, int? IdCliente, int? IdTipo, DateTime? DataInicial, DateTime? DataFinal)
        {
            var model = AgendasBo.Listar(DataInicial, DataFinal).AsQueryable();

            if (IdSede > 0)
                model = model.Where(d => d.IdSede == IdSede);

            if (IdProfissional > 0)
                model = model.Where(d => d.IdProfissional == IdProfissional);

            if (IdCliente > 0)
                model = model.Where(d => d.IdCliente == IdCliente);

            if (IdTipo > 0)
                model = model.Where(d => d.IdTipo == IdTipo);

            var sb = new StringBuilder();
            int count = 0;

            foreach (var item in model)
            {
                if (count > 0)
                    sb.Append(", ");

                string title = $"{item.NomeProfissional}{(string.IsNullOrEmpty(item.NomeCliente) ? "" : $" - {item.NomeCliente}")} - {item.NomeTipo} - {item.NomeSede}{(string.IsNullOrEmpty(item.NomeSala) ? "" : $" - {item.NomeSala}")}";

                sb.Append("{ ");
                sb.Append($"\"id\": {item.Id}, ");
                sb.Append($"\"title\": \"{title}\"");

                if (string.IsNullOrEmpty(item.Hora))
                {
                    sb.Append($", \"start\": \"{item.Data:yyyy-MM-dd}\"");
                }
                else
                {
                    var started = DateTime.Parse($"{item.Data:yyyy-MM-dd} {item.Hora}");
                    var ended = item.Encaixe == "S" ? started.AddMinutes(15) : started.AddHours(1);

                    sb.Append($", \"start\": \"{started:yyyy-MM-ddTHH:mm:00}\", \"end\": \"{ended:yyyy-MM-ddTHH:mm:00}\"");
                }

                string color = "#E1DCE6";

                if (Sessao.Usuario.ID == item.IdUsuario)
                {
                    color = item.IdCliente > 0 ? "#F2D7D5" : "#D5F5E3";
                }
                else if (item.IdCliente == null)
                {
                    color = "#D6EAF8";
                }

                sb.Append($", \"color\": \"{color}\" }}");

                count++;
            }

            return $"[{sb}]";
        }
        //[HttpPost]
        //public string Eventos(Int32? IdSede, Int32? IdProfissional, Int32? IdCliente, Int32? IdTipo, DateTime? DataInicial, DateTime? DataFinal)
        //{
        //    string result = string.Empty;

        //    var model = AgendasBo.Listar(DataInicial, DataFinal);

        //    //if (Sessao.Usuario.Nivel == 1 || Sessao.Usuario.Nivel == 0)
        //    //{
        //    //    model = model.Where(d => d.IdUsuario == Sessao.Usuario.ID || d.Profissionais.IdUsuario == Sessao.Usuario.ID).ToList();
        //    //}
        //    //else if (Sessao.Usuario.Nivel > 1)
        //    //{
        //    if (IdSede != null && IdSede > 0)
        //    {
        //        model = model.Where(d => d.IdSede == IdSede).ToList();
        //    }

        //    if (IdProfissional != null && IdProfissional > 0)
        //    {
        //        model = model.Where(d => d.IdProfissional == IdProfissional).ToList();
        //    }

        //    if (IdCliente != null && IdCliente > 0)
        //    {
        //        model = model.Where(d => d.IdCliente == IdCliente).ToList();
        //    }

        //    if (IdTipo != null && IdTipo > 0)
        //    {
        //        model = model.Where(d => d.IdTipo == IdTipo).ToList();
        //    }
        //    //}

        //    Int32 index = 0;

        //    foreach (var item in model)
        //    {
        //        if (!string.IsNullOrEmpty(result))
        //        {
        //            result = result + ", ";
        //        }
        //        index++;
        //        if (index == 2015)
        //        {
        //            var xxx = string.Empty;
        //        }
        //        if (item.Id == 32392)
        //        {
        //            var xxx = string.Empty;
        //        }

        //        result = result + "{ \"id\": " + item.Id + ", \"title\": \"" + string.Format("{0}{1} - {2} - {3}{4}", item.Profissionais.Pessoas.Nome, (item.Clientes != null ? string.Format(" - {0}", item.Clientes.Pessoas.Nome) : string.Empty), item.AgendasTipos.Nome, item.Sedes.Nome, (item.Salas != null ? string.Format(" - {0}", item.Salas.Nome) : string.Empty)) + "\"";

        //        if (string.IsNullOrEmpty(item.Hora))
        //        {
        //            result = result + ", \"start\": \"" + item.Data.ToString("yyyy-MM-dd") + "\"";
        //        }
        //        else
        //        {
        //            var started = DateTime.Parse(string.Format("{0} {1}", item.Data.ToString("yyyy-MM-dd"), item.Hora));
        //            var ended = started.AddHours(1);
        //            if (item.Encaixe == "S")
        //            {
        //                ended = started.AddMinutes(15);
        //            }

        //            result = result + ", \"start\": \"" + started.ToString("yyyy-MM-ddTHH:mm:00") + "\", \"end\": \"" + ended.ToString("yyyy-MM-ddTHH:mm:00") + "\"";
        //        }

        //        if (Sessao.Usuario.ID == item.IdUsuario && item.IdCliente != null && item.IdCliente > 0)
        //        {
        //            result = result + ", \"color\": \"#F2D7D5\" }";
        //        }
        //        else if (Sessao.Usuario.ID == item.IdUsuario && item.IdCliente == null)
        //        {
        //            result = result + ", \"color\": \"#D5F5E3\" }";
        //        }
        //        else if (Sessao.Usuario.ID != item.IdUsuario && item.IdCliente == null)
        //        {
        //            result = result + ", \"color\": \"#D6EAF8\" }";
        //        }
        //        else
        //        {
        //            result = result + ", \"color\": \"#E1DCE6\" }";
        //        }
        //    }

        //    return string.Format("[{0}]", result);
        //}

        [HttpPost]
        public JsonResult EnviarSemanal()
        {
            var codigo = "00";
            var motivo = string.Empty;

            var profissional = ProfissionaisBo.Buscar(Sessao.Usuario.ID);

            if (profissional != null)
            {
                var Emails = profissional.Pessoas.Email;

                if (Emails.Count > 0)
                {
                    var mails = Emails.Where(x => x.Ativo).ToList();

                    if (mails.Count > 0)
                    {
                        var DataInicial = Util.NextDate(DateTime.Now, DayOfWeek.Saturday).ToShortDateString();
                        var DataFinal = DateTime.Parse(DataInicial).AddDays(7).ToShortDateString();
                        var agendas = AgendasBo.Listar(DateTime.Parse(DataInicial), DateTime.Parse(DataFinal));

                        var resposta = Enviar(agendas, mails, "Agendas da próxima semana para o profissional ");

                        codigo = resposta[0];
                        motivo = resposta[1];
                    }
                    else
                    {
                        codigo = "70";
                        motivo = "Profissional (" + profissional.Pessoas.Nome + ") sem e-mail ativo, por esse motivo não é possível enviar E-mails.";
                    }
                }
                else
                {
                    codigo = "70";
                    motivo = "Profissional (" + profissional.Pessoas.Nome + ") sem e-mail cadastrado, por esse motivo não é possível enviar E-mails.";
                }
            }
            else
            {
                codigo = "70";
                motivo = "Usuário (" + Sessao.Usuario.Nome + ") sem vínculo com o cadastro de profissionais, por esse motivo não é possível enviar E-mails.";
            }

            var result = new { codigo = codigo, motivo = motivo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult EnviarPeriodo(String DataInicial, String DataFinal, Int32? IdProfissional)
        {
            var codigo = "00";
            var motivo = string.Empty;

            var profissional = ProfissionaisBo.Buscar(Sessao.Usuario.ID);

            if (profissional != null)
            {
                var Emails = profissional.Pessoas.Email;

                if (Emails.Count > 0)
                {
                    var mails = Emails.Where(x => x.Ativo).ToList();

                    if (mails.Count > 0)
                    {
                        if (DataInicial == null || DataInicial.Length == 0)
                        {
                            DataInicial = Util.NextDate(DateTime.Now, DayOfWeek.Saturday).ToShortDateString();
                        }

                        if (DataFinal == null || DataFinal.Length == 0)
                        {
                            DataFinal = DateTime.Parse(DataInicial).AddDays(7).ToShortDateString();
                        }

                        var agendas = AgendasBo.Listar(DateTime.Parse(DataInicial), DateTime.Parse(DataFinal));

                        if (IdProfissional != null && IdProfissional > 0)
                        {
                            agendas.RemoveAll(x => x.IdProfissional != IdProfissional);
                        }

                        var resposta = Enviar(agendas, mails, "Agendas da próxima semana para o profissional ");

                        codigo = resposta[0];
                        motivo = resposta[1];
                    }
                    else
                    {
                        codigo = "70";
                        motivo = "Profissional (" + profissional.Pessoas.Nome + ") sem e-mail ativo, por esse motivo não é possível enviar E-mails.";
                    }
                }
                else
                {
                    codigo = "70";
                    motivo = "Profissional (" + profissional.Pessoas.Nome + ") sem e-mail cadastrado, por esse motivo não é possível enviar E-mails.";
                }
            }
            else
            {
                codigo = "70";
                motivo = "Usuário (" + Sessao.Usuario.Nome + ") sem vínculo com o cadastro de profissionais, por esse motivo não é possível enviar E-mails.";
            }

            var result = new { codigo = codigo, motivo = motivo };
            return Json(result);
        }

        public static void EnviarAgenda(Int32 Id, String Titulo)
        {
            var profissional = ProfissionaisBo.Buscar(Sessao.Usuario.ID);

            if (profissional != null)
            {
                var Emails = profissional.Pessoas.Email;

                if (Emails.Count > 0)
                {
                    var mails = Emails.Where(x => x.Ativo).ToList();

                    if (mails.Count > 0)
                    {
                        var agenda = AgendasBo.Listar(Id);

                        Enviar(agenda, mails, Titulo);
                    }
                }
            }
        }

        public static List<string> Enviar(List<Agendas> agendas, List<Email> mails, String Titulo)
        {
            var resposta = new List<string>();
            var codigo = "00";
            var motivo = string.Empty;

            var empresa = EmpresaBo.Buscar(1);

            var pessoas = new List<string>();

            foreach (var item in agendas)
            {
                if (item.Profissionais.Pessoas.Email.Count == 0)
                {
                    if (!pessoas.Contains(item.Profissionais.Pessoas.Nome))
                    {
                        pessoas.Add(item.Profissionais.Pessoas.Nome);
                    }
                }
                else
                {
                    var mailsAux = item.Profissionais.Pessoas.Email.Where(x => x.Ativo).ToList();
                    if (mailsAux.Count == 0)
                    {
                        if (!pessoas.Contains(item.Profissionais.Pessoas.Nome))
                        {
                            pessoas.Add(item.Profissionais.Pessoas.Nome);
                        }
                    }
                }
            }

            if (pessoas.Count == 0)
            {
                var profissionalId = 0;
                var menssage = string.Empty;
                var to = string.Empty;
                var name = string.Empty;
                var emailremetente = empresa.Email;
                var passwordremetente = empresa.Senha;

                var email = UsuariosBo.Buscar(Sessao.Usuario.ID);

                foreach (var item in agendas)
                {
                    if (profissionalId != 0 && profissionalId != item.IdProfissional)
                    {
                        menssage += "    </table>";
                        menssage += "  </body>";
                        menssage += "</html>";

                        Mail.Send(emailremetente, passwordremetente, to, Titulo + name, menssage);

                        profissionalId = 0;
                        menssage = string.Empty;
                    }

                    if (profissionalId == 0)
                    {
                        to = mails[0].Email1 + "," + item.Profissionais.Pessoas.Email.Where(x => x.Ativo).FirstOrDefault().Email1;
                        name = item.Profissionais.Pessoas.Nome;

                        menssage += "<!DOCTYPE html>";
                        menssage += "<html>";
                        menssage += "  <head>";
                        menssage += "    <style>";
                        menssage += "      #customers {";
                        menssage += "        font-family: 'Trebuchet MS', Arial, Helvetica, sans-serif;";
                        menssage += "        border - collapse: collapse;";
                        menssage += "        width: 100 %;";
                        menssage += "      }";
                        menssage += "      #customers td, #customers th {";
                        menssage += "        border: 1px solid #DDDDDD;";
                        menssage += "        padding: 8px;";
                        menssage += "      }";
                        menssage += "      #customers tr:nth-child(even){ background-color: #EDEDED; }";
                        menssage += "      #customers tr:hover { background-color: #EED58F; }";
                        menssage += "      #customers th {";
                        menssage += "        padding-top: 12px;";
                        menssage += "        padding-bottom: 12px;";
                        menssage += "        text-align: left;";
                        menssage += "        background-color: #ECBD4F;";
                        menssage += "        color: #333333;";
                        menssage += "        border: 1px solid black";
                        menssage += "        border-collapse: collapse";
                        menssage += "      }";
                        menssage += "    </style>";
                        menssage += "  </head>";
                        menssage += "  <body>";
                        menssage += "    <h3>";
                        menssage += "      Agendado por: " + Sessao.Usuario.Nome;
                        menssage += "    </h3>";
                        menssage += "    <table id='customers'>";
                        menssage += "      <tr>";
                        menssage += "        <th>Data</th>";
                        menssage += "        <th>Hora</th>";
                        menssage += "        <th>Assunto</th>";
                        menssage += "        <th>Sede</th>";
                        menssage += "        <th>Sala</th>";
                        menssage += "        <th>Cliente</th>";
                        menssage += "        <th>Cancelou</th>";
                        menssage += "        <th>Descrição</th>";
                        menssage += "      </tr>";

                        profissionalId = item.IdProfissional;
                    }

                    menssage += "      <tr>";
                    menssage += "        <td>" + item.Data.ToShortDateString() + "</td>";
                    menssage += "        <td>" + (item.Hora != null ? item.Hora.Substring(0, 5) : string.Empty) + "</td>";
                    menssage += "        <td>" + item.AgendasTipos.Nome + "</td>";
                    menssage += "        <td>" + item.Sedes.Nome + "</td>";
                    menssage += "        <td>" + (item.Salas != null ? item.Salas.Nome : string.Empty) + "</td>";
                    menssage += "        <td>" + (item.Clientes != null ? item.Clientes.Pessoas.Nome : string.Empty) + "</td>";
                    menssage += "        <td>" + (item.Cancelou != null && item.Cancelou.Value ? "Sim" : "Não") + "</td>";
                    menssage += "        <td>" + (item.MotivoConsulta?.Length > 0 ? item.MotivoConsulta : string.Empty) + "</td>";
                    menssage += "      </tr>";

                }

                if (!string.IsNullOrEmpty(menssage))
                {
                    menssage += "    </table>";
                    menssage += "  </body>";
                    menssage += "</html>";

                    if (!Mail.Send(emailremetente, passwordremetente, to, Titulo + name, menssage))
                    {
                        codigo = "70";
                        motivo = "Ocorreu uma falha ao enviar os E-mails. Tente novamente mais tarde ou entre em contato com o administrador do sistema.";
                    }
                }
            }
            else
            {
                codigo = "70";
                if (pessoas.Count == 1)
                {
                    motivo = "Profissional listado abaixo está sem e-mail cadastrado ou inativo, por esse motivo não é possível enviar E-mails.<br /><br />";
                }
                else
                {
                    motivo = "Profissionais listados abaixo estão sem e-mails cadastrados ou inativos, por esse motivo não é possível enviar E-mails.<br /><br />";
                }

                for (int i = 0; i < pessoas.Count; i++)
                {
                    motivo += pessoas[i] + "<br />";
                }
            }

            resposta.Add(codigo);
            resposta.Add(motivo);

            return resposta;
        }

        [HttpPost]
        public JsonResult VisualizarPeriodo(String DataInicial, String DataFinal, Int32? IdProfissional, Int32? IdCliente, Int32? IdSede)
        {
            if (IdSede != null)
            {
                var sede = SedesBo.Consultar(int.Parse(IdSede.ToString()));
                if (sede.Nome.Trim().Count() == 0)
                {
                    IdSede = null;
                }
            }
            if (!String.IsNullOrEmpty(DataInicial) && !String.IsNullOrEmpty(DataFinal) && (DateTime.Parse(DataFinal).Date - DateTime.Parse(DataInicial).Date).Days >= 0)
            {
                var agenda = AgendasBo.ListarPeriodoAux(DateTime.Parse(DataInicial), DateTime.Parse(DataFinal), IdProfissional, IdCliente, IdSede);
                String agendas = String.Empty;

                if (agenda.Count > 0)
                {
                    agendas += "<table class='table table-striped' style='width: 100% !important; font-size: 12px !important;'>";
                    agendas += "    <thead>";
                    agendas += "        <tr>";
                    agendas += "            <th>Data</th>";
                    agendas += "            <th>Hora</th>";
                    agendas += "            <th>Encaixe</th>";
                    agendas += "            <th>Assunto</th>";
                    agendas += "            <th>Local de Atendimento</th>";
                    agendas += "            <th>Cliente</th>";
                    agendas += "            <th>Nome Profissional</th>";
                    agendas += "        </tr>";
                    agendas += "    </thead>";
                    agendas += "    <tbody>";

                    for (int i = 0; i < agenda.Count; i++)
                    {
                        agendas += "        <tr>";
                        agendas += "            <th scope='row'>" + agenda[i].Data.ToString("dd/MM/yyyy") + "</th>";
                        agendas += "            <td>" + agenda[i].Hora + "</td>";
                        agendas += "            <td>" + (agenda[i].Encaixe == "S" ? "Sim" : "Não") + "</td>";
                        agendas += "            <td>" + agenda[i].AgendasTipos.Nome + "</td>";
                        agendas += "            <td>" + agenda[i].Sedes.Nome + "</td>";
                        agendas += "            <td>" + (agenda[i].Clientes?.Pessoas?.Nome ?? String.Empty) + "</td>";
                        agendas += "            <td>" + (agenda[i].Profissionais?.Pessoas?.Nome ?? String.Empty) + "</td>";
                        agendas += "        </tr>";
                    }

                    agendas += "    </tbody>";
                    agendas += "</table>";
                }
                else
                {
                    agendas = "Nenhuma agenda encontrada para esse período e profissional ou cliente.";
                }

                var result = new { codigo = "00", agendas = agendas };
                return Json(result);
            }
            else if (!String.IsNullOrEmpty(DataInicial) && !String.IsNullOrEmpty(DataFinal) && (DateTime.Parse(DataFinal).Date - DateTime.Parse(DataInicial).Date).Days < 0)
            {
                var result = new { codigo = "99", motivo = "A Data Final deve ser maior que a Data Inicial." };
                return Json(result);
            }
            else
            {
                var result = new { codigo = "99", motivo = "É necessário selecionar um período para efetuar o filtro." };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult BuscarAgendaProfissionais(DateTime Data, String HoraInicial, String HoraFinal, String Profissionais)
        {
            String agendasLiberadas = "1";
            String agendas = String.Empty;

            foreach (var item in Profissionais.Split(','))
            {
                var agenda = AgendasBo.BuscarAgendaProfissionais(Data, HoraInicial, HoraFinal, int.Parse(item));

                agendas += "        <tr>";
                agendas += "           <th scope='row'>" + ProfissionaisBo.Consultar(int.Parse(item)).Pessoas.Nome + "</th>";

                if (agenda.Count > 0)
                {
                    agendas += "           <td class='red'>Indisponível</td>";
                    agendasLiberadas = "0";
                }
                else
                {
                    agendas += "           <td class='green'>Livre</td>";
                }

                agendas += "        </tr>";
            }

            var result = new { codigo = "00", agendas = agendas, agendasLiberadas = agendasLiberadas };
            return Json(result);
        }

        [HttpPost]
        public JsonResult SalvarAgendaProfissionais(DateTime Data, String Hora, Int32 Assunto, Int32 Sede, Int32? Sala, String Profissionais, String Descricao)
        {
            try
            {
                foreach (var item in Profissionais.Split(','))
                {
                    Agendas agenda = new Agendas();
                    agenda.Data = Data;
                    agenda.Hora = Hora;
                    agenda.IdTipo = Assunto;
                    agenda.IdSede = Sede;

                    if (Sala != null)
                    {
                        agenda.IdSala = Sala;
                    }

                    agenda.IdProfissional = Int32.Parse(item);
                    agenda.MotivoConsulta = Descricao;
                    agenda.Encaixe = "N";
                    agenda.IdUsuario = Sessao.Usuario.ID;
                    agenda.DataCadastro = DateTime.Now;
                    agenda.Cancelou = false;

                    AgendasBo.Inserir(agenda);
                }

                var result = new { codigo = "00" };
                return Json(result);

            }
            catch (Exception ex)
            {
                var result = new { codigo = "99" };
                return Json(result);
            }
        }
    }
}