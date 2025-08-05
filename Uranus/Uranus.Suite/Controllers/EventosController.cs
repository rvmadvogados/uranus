using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class EventosController : Controller
    {
        //public ActionResult GetEventosList(string search, bool filter = false, int page = 1)
        //{
        //    var clientes = EventosBo.GetEventosList(search, filter);
        //    var total = clientes.Count();

        //    if (!(string.IsNullOrEmpty(search) || string.IsNullOrWhiteSpace(search)))
        //    {
        //        clientes = clientes.Where(x => PessoasBo.ConverteNome(x.text).ToLower().StartsWith(PessoasBo.ConverteNome(search).ToLower())).Take(page * 10).ToList();
        //    }

        //    return Json(new { clientes = clientes, total = total }, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult Index(string FiltrarCodigo = "", string FiltrarNome = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = EventosBo.Listar(FiltrarCodigo, FiltrarNome);
                ViewBag.FiltrarCodigo = FiltrarCodigo;
                ViewBag.FiltrarNome = FiltrarNome;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32? Id)
        {
            var evento = EventosBo.ConsultarArray(Id ?? 0);
            var result = new { codigo = "00", evento = evento };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String Codigo, String Nome, String Texto, String Tipo, String Aplicativo, String WhatsApp)
        {
            ProcessosEventos evento = new ProcessosEventos();
            evento.ID = Id;
            evento.Codigo = Codigo.Trim();
            evento.Descricao = Nome.Trim();
            evento.Texto = Texto;
            evento.Tipo = Tipo;

            if (Aplicativo == "S")
            {
                evento.Aplicativo = true;
            }
            else
            {
                evento.Aplicativo = false;
            }

            if (WhatsApp == "S")
            {
                evento.WhatsApp = true;
            }
            else
            {
                evento.WhatsApp = false;
            }

            if (Id == 0)
                Id = EventosBo.Inserir(evento);

            evento.ID = Id;
            EventosBo.Salvar(evento);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = EventosBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar()
        {
            var eventos = EventosBo.Consultar();
            var result = new { codigo = "00", eventos = eventos };
            return Json(result);
        }
    }
}