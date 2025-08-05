using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class TiposAgendasController : Controller
    {
        public ActionResult Index(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = TiposAgendasBo.Listar(search);
                ViewBag.search = search;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var tipo = TiposAgendasBo.ConsultarArray(Id);
            var result = new { codigo = "00", tipo = tipo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String Nome)
        {
            AgendasTipos tipo = new AgendasTipos();
            tipo.ID = Id;
            tipo.Nome = Nome.Trim();

            if (Id == 0)
                Id = TiposAgendasBo.Inserir(tipo);

            tipo.ID = Id;
            TiposAgendasBo.Salvar(tipo);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = TiposAgendasBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar()
        {
            var tipos = TiposAgendasBo.Consultar();
            var result = new { codigo = "00", tipos = tipos };
            return Json(result);
        }
    }
}