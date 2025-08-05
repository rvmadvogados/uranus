using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class FinanceiroTipoController : Controller
    {
        public ActionResult Index(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = FinanceiroTipoBo.Listar(search);
                ViewBag.search = search;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var financeirotipo = FinanceiroTipoBo.ConsultarArray(Id);
            var result = new { codigo = "00", financeirotipo = financeirotipo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String Nome, String Tipo)
        {
            FinanceiroTipo financeirotipo = new FinanceiroTipo();
            financeirotipo.Id = Id;
            financeirotipo.Nome = Nome.Trim();
            financeirotipo.Tipo = Tipo;

            if (Id == 0)
                Id = FinanceiroTipoBo.Inserir(financeirotipo);

            financeirotipo.Id = Id;
            FinanceiroTipoBo.Salvar(financeirotipo);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = FinanceiroTipoBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar(string Tipo)
        {
            var financeirotipo = FinanceiroTipoBo.Consultar(Tipo);
            var result = new { codigo = "00", financeirotipo = financeirotipo };
            return Json(result);
        }
    }
}
