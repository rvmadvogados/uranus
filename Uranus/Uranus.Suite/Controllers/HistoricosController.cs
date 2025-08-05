using System;
using System.util;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class HistoricosController : Controller
    {
        // GET: Historicos
        public ActionResult Index(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = HistoricosBo.Listar(search);
                ViewBag.search = search;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var historico = HistoricosBo.ConsultarArray(Id);
            var result = new { codigo = "00", historico = historico };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String Nome, String Tipo)
        {
            Historicos historico = new Historicos();
            historico.Id = Id;
            historico.Nome = Nome.Trim();
            historico.Tipo = Tipo.Trim();

            if (Id == 0)
                Id = HistoricosBo.Inserir(historico);

            historico.Id = Id;
            HistoricosBo.Salvar(historico);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = HistoricosBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar()
        {
            var historicos = HistoricosBo.Consultar();
            var result = new { codigo = "00", historicos = historicos };
            return Json(result);


        }

    }
}