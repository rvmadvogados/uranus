using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class SalasController : Controller
    {
        public ActionResult Index(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = SalasBo.Listar(search);
                ViewBag.search = search;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var sala = SalasBo.ConsultarArray(Id);
            var result = new { codigo = "00", sala = sala };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String Nome, Int32 IdSede)
        {
            Salas sala = new Salas();
            sala.ID = Id;
            sala.Nome = Nome.Trim();
            sala.IDSede = IdSede;

            if (Id == 0)
                Id = SalasBo.Inserir(sala);

            sala.ID = Id;
            SalasBo.Salvar(sala);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = SalasBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar(Int32 IdSede)
        {
            var salas = SalasBo.ListarArray(IdSede);

            var result = new { codigo = "00", salas = salas };
            return Json(result);
        }
    }
}