using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class VarasController : Controller
    {
        public ActionResult Index(string FiltrarNome = "", string FiltrarSigla = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = VarasBo.Listar(FiltrarNome, FiltrarSigla);
                ViewBag.FiltrarNome = FiltrarNome;
                ViewBag.FiltrarSigla = FiltrarSigla;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var vara = VarasBo.ConsultarArray(Id);
            var result = new { codigo = "00", vara = vara };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String Nome, String Sigla)
        {
            ProcessosVara vara = new ProcessosVara();
            vara.ID = Id;
            vara.Vara = Nome.Trim();
            vara.Sigla = Sigla.Trim();

            if (Id == 0)
                Id = VarasBo.Inserir(vara);

            vara.ID = Id;
            VarasBo.Salvar(vara);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = VarasBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar()
        {
            var varas = VarasBo.Consultar();
            var result = new { codigo = "00", varas = varas };
            return Json(result);
        }
    }
}