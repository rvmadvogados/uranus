using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class ModelosController : Controller
    {
        public ActionResult Index(string FiltrarNome = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = ModelosBo.Listar(FiltrarNome);
                ViewBag.FiltrarNome = FiltrarNome;
                return View(model);
            }
        }

        public PartialViewResult View(Int32 Id)
        {
            var model = ModelosBo.Consultar(Id);

            JsonResult result = Json(model);
            result.MaxJsonLength = 2147483644;

            return PartialView(result);
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var modelo = ModelosBo.ConsultarArray(Id);
            var result = new { codigo = "00", modelo = modelo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String Nome, String Modelo)
        {
            Modelos modelo = new Modelos();
            modelo.Id = Id;
            modelo.Nome = Nome.Trim();
            modelo.Modelo = Modelo;

            if (Id == 0)
                Id = ModelosBo.Inserir(modelo);

            modelo.Id = Id;
            ModelosBo.Salvar(modelo);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = ModelosBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar()
        {
            var modelos = ModelosBo.Consultar();
            var result = new { codigo = "00", modelos = modelos };
            return Json(result);
        }
    }
}