using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class ParceirosController : Controller
    {
        // GET: Parceiros
        public ActionResult Index(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = ParceirosBo.Listar(search);
                ViewBag.search = search;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var parceiro = ParceirosBo.ConsultarArray(Id);
            var result = new { codigo = "00", parceiro = parceiro };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String Nome, Int32? IdProfissional)
        {
            Parceiros parceiro = new Parceiros();
            parceiro.ID = Id;
            parceiro.Nome = Nome.Trim();
            parceiro.IdProfissional = IdProfissional;

            if (Id == 0)
                Id = ParceirosBo.Inserir(parceiro);

            parceiro.ID = Id;
            ParceirosBo.Salvar(parceiro);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = ParceirosBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar()
        {
            var parceiros = ParceirosBo.Consultar();
            var result = new { codigo = "00", parceiros = parceiros };
            return Json(result);
        }
    }
}