using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class SedesController : Controller
    {
        public ActionResult Index(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = SedesBo.Listar(search);
                ViewBag.search = search;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var sede = SedesBo.ConsultarArray(Id);
            var result = new { codigo = "00", sede = sede };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String Nome, String Agenda)
        {
            Sedes sede = new Sedes();
            sede.ID = Id;
            sede.Nome = Nome.Trim();
            if (Agenda == "S")
            {
                sede.Agenda = true;
            }
            else
            {
                sede.Agenda = false;
            }

            if (Id == 0)
                Id = SedesBo.Inserir(sede);

            sede.ID = Id;
            SedesBo.Salvar(sede);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = SedesBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar()
        {
            var sedes = SedesBo.Consultar();
            var result = new { codigo = "00", sedes = sedes };
            return Json(result);
        }
        [HttpPost]
        public JsonResult ListarClientes()
        {
            var sedes = SedesBo.ConsultarClientes();
            var result = new { codigo = "00", sedes = sedes };
            return Json(result);
        }
    }
}