using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class RHAreasController : Controller
    {
        public ActionResult Index(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = RHAreasBo.Listar(search);
                ViewBag.search = search;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var area = RHAreasBo.ConsultarArray(Id);
            var result = new { codigo = "00", area = area };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String Nome)
        {
            RHAreas area = new RHAreas();
            area.Id = Id;
            area.Nome = Nome.Trim();

            if (Id == 0)
                Id = RHAreasBo.Inserir(area);

            area.Id = Id;
            RHAreasBo.Salvar(area);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = RHAreasBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar()
        {
            var areas = RHAreasBo.Consultar();
            var result = new { codigo = "00", areas = areas };
            return Json(result);
        }
    }
}