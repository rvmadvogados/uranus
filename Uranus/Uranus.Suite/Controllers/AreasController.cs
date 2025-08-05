using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class AreasController : Controller
    {
        public ActionResult Index(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = AreasBo.Listar(search);
                ViewBag.search = search;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var area = AreasBo.ConsultarArray(Id);
            var result = new { codigo = "00", area = area };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String Nome)
        {
            ProcessosAreas area = new ProcessosAreas();
            area.ID = Id;
            area.AreaAtuacao = Nome.Trim();

            if (Id == 0)
                Id = AreasBo.Inserir(area);

            area.ID = Id;
            AreasBo.Salvar(area);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = AreasBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar()
        {
            var areas = AreasBo.Consultar();
            var result = new { codigo = "00", areas = areas };
            return Json(result);
        }
    }
}