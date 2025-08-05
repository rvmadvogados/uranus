using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class EventoAutomaticoController : Controller
    {
        // GET: EventoAutomatico
        public ActionResult Index()
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = ProcessosAcoesBo.ListarEventosAutomaticos();
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id)
        {
            ProcessosAcoes processosAcoes = ProcessosAcoesBo.Consultar(Id);
            processosAcoes.EventoAutomatico = true;

            ProcessosAcoesBo.Salvar(processosAcoes);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Gerar(String Ids, Int32 IdEvento, String Descricao)
        {
            foreach (var id in Ids.Split(';'))
            {
                ProcessosAcoesEventos evento = new ProcessosAcoesEventos();
                if (id != "on")
                {
                    evento.IdProcessosAcao = int.Parse(id);
                    evento.IdProcessosEvento = IdEvento;
                    evento.Descricao = Descricao;
                    evento.Data = DateTime.Now;
                    evento.IdUsuario = Sessao.Usuario.ID;

                    ProcessosAcoesEventosBo.Inserir(evento);
                }
            }

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            ProcessosAcoes processosAcoes = ProcessosAcoesBo.Consultar(Id);
            processosAcoes.EventoAutomatico = false;

            ProcessosAcoesBo.Salvar(processosAcoes);

            var result = new { codigo = "00" };
            return Json(result);
        }
    }
}