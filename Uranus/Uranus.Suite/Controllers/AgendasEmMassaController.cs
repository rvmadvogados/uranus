using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class AgendasEmMassaController : Controller
    {
        public ActionResult Index(string FiltrarSede = "", string FiltrarProfissional = "", string FiltrarPeriodo = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = AgendasEmMassaBo.Listar(FiltrarSede, FiltrarProfissional, FiltrarPeriodo);
                ViewBag.FiltrarSede = FiltrarSede;
                ViewBag.FiltrarProfissional = FiltrarProfissional;
                ViewBag.FiltrarPeriodo = FiltrarPeriodo;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Salvar(String DataInicio, String DataFim, String HoraInicio, String HoraFim, Int32 Assunto, Int32 Sede, Int32 Profissional, String Intervalo, String SedeNome, String ProfissionalNome, string MotivoConsulta)
        {
            var datainicio = DateTime.Parse(DataInicio);
            var datafim = DateTime.Parse(DataFim);
            var consultaperiodo = AgendasBo.ConsultarAgendaPeriodo(datainicio, datafim, Profissional);

            if (consultaperiodo.Count > 0)
            {
                var result = new { codigo = "90" };
                return Json(result);
            }
            else
            {
                try
                {
                    AgendasMassa agendasMassa = new AgendasMassa();
                    agendasMassa.DataInicio = DataInicio;
                    agendasMassa.DataFim = DataFim;
                    agendasMassa.HoraInicio = HoraInicio;
                    agendasMassa.HoraFim = HoraFim;
                    agendasMassa.IdTipo = Assunto;
                    agendasMassa.IdSede = Sede;
                    agendasMassa.IdProfissional = Profissional;
                    agendasMassa.Intervalo = (Intervalo == "S" ? true : false);
                    agendasMassa.IdUsuario = Sessao.Usuario.ID;
                    agendasMassa.DataCadastro = DateTime.Now;

                    agendasMassa.DataPeriodo = DateTime.Parse(DataInicio);
                    agendasMassa.MotivoConsulta = MotivoConsulta;

                    var IdAgendaMassa = AgendasEmMassaBo.Inserir(agendasMassa);

                    var dateFirst = DateTime.Parse(DataInicio);
                    var dateLast = DateTime.Parse(DataFim);
                    var days = (dateLast - dateFirst).TotalDays + 1;
                    var timeFirst = DateTime.Parse(HoraInicio).Hour;
                    var timeLast = DateTime.Parse(HoraFim).Hour;
                    var hours = (DateTime.Parse(HoraFim) - DateTime.Parse(HoraInicio)).TotalHours - (Intervalo == "S" && (timeFirst <= 12) && (timeLast >= 12) ? 1 : 0) + 1;

                    var criadas = 0;

                    for (int d = 1; d < days + 1; d++)
                    {
                        if (dateFirst.DayOfWeek != DayOfWeek.Saturday && dateFirst.DayOfWeek != DayOfWeek.Sunday)
                        {
                            for (int h = 0; h < hours + 1; h++)
                            {
                                if (Intervalo == "N" || (Intervalo == "S" && timeFirst != 12))
                                {
                                    Agendas agenda = new Agendas();
                                    agenda.Data = dateFirst;
                                    agenda.Hora = String.Format("{0}:00:00", timeFirst.ToString().PadLeft(2, '0'));
                                    agenda.IdTipo = Assunto;
                                    agenda.IdSede = Sede;
                                    agenda.IdProfissional = Profissional;
                                    agenda.IdUsuario = Sessao.Usuario.ID;
                                    agenda.Encaixe = "N";
                                    agenda.Cancelou = false;
                                    agenda.DataCancelamento = null;
                                    agenda.MotivoConsulta = MotivoConsulta;

                                    var Id = AgendasBo.Inserir(agenda);

                                    AgendasMassaHistorico historico = new AgendasMassaHistorico();
                                    historico.IdAgendaMassa = IdAgendaMassa;
                                    historico.Data = dateFirst.ToShortDateString();
                                    historico.Hora = String.Format("{0}:00", timeFirst.ToString().PadLeft(2, '0'));
                                    historico.Sede = SedeNome;
                                    historico.Sala = null;
                                    historico.Profissional = ProfissionalNome;

                                    if (Id > 0)
                                    {
                                        historico.Status = "Agenda criada com sucesso.";
                                        historico.IdAgenda = Id;
                                        criadas++;
                                    }
                                    else
                                    {
                                        historico.Status = "Profissional já possui agenda nesta data e hora.";
                                        historico.IdAgenda = null;
                                    }

                                    AgendasEmMassaBo.InserirHistorico(historico);
                                }

                                timeFirst++;
                            }

                            timeFirst = timeFirst - int.Parse(hours.ToString()) - 1;
                        }

                        dateFirst = dateFirst.AddDays(1);
                    }

                    agendasMassa.Criadas = criadas;
                    AgendasEmMassaBo.Salvar(agendasMassa);

                    var result = new { codigo = "00" };
                    return Json(result);
                }
                catch (Exception ex)
                { 
                    var result = new { codigo = "80" };
                    return Json(result);
                }

            }
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var agendas = AgendasEmMassaBo.Listar(Id);

            foreach (var item in agendas)
            {
                if (item.IdAgenda != null)
                {
                    AgendasController.EnviarAgenda(Int32.Parse(item.IdAgenda.ToString()), "Agenda cancelada para o profissional ");

                    AgendasBo.Excluir(Int32.Parse(item.IdAgenda.ToString()));
                }
            }

            var codigo = AgendasEmMassaBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var historico = AgendasEmMassaBo.ListarArray(Id);
            var result = new { codigo = "00", historico = historico };
            return Json(result);
        }
    }
}