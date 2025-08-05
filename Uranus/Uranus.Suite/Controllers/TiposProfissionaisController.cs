using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class TiposProfissionaisController : Controller
    {
        public ActionResult Index(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = TiposProfissionaisBo.Listar(search);
                ViewBag.search = search;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var tipo = TiposProfissionaisBo.ConsultarArray(Id);
            var result = new { codigo = "00", tipo = tipo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String Nome)
        {
            ProfissionaisTipos tipo = new ProfissionaisTipos();
            tipo.ID = Id;
            tipo.Nome = Nome.Trim();

            if (Id == 0)
                Id = TiposProfissionaisBo.Inserir(tipo);

            tipo.ID = Id;
            TiposProfissionaisBo.Salvar(tipo);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = TiposProfissionaisBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar()
        {
            var tipos = TiposProfissionaisBo.Consultar();
            var result = new { codigo = "00", tipos = tipos };
            return Json(result);
        }
    }
}