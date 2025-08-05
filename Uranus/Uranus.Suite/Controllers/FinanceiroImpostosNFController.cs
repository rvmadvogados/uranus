using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class FinanceiroImpostosNFController : Controller
    {
        public ActionResult Index(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = FinanceiroImpostosNFBo.Listar(search);
                ViewBag.search = search;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var financeiroimpostosnf = FinanceiroImpostosNFBo.ConsultarArray(Id);
            var result = new { codigo = "00", financeiroimpostosnf = financeiroimpostosnf };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String Nome, String Valor)
        {
            FinanceiroImpostosNF financeiroimpostosnf = new FinanceiroImpostosNF();
            financeiroimpostosnf.Id = Id;
            financeiroimpostosnf.Nome = Nome.Trim();

            if (Valor != null && Valor.Trim().Length > 0)
            {
                financeiroimpostosnf.Valor = Decimal.Parse(Valor);
            }

            if (Id == 0)
                Id = FinanceiroImpostosNFBo.Inserir(financeiroimpostosnf);

            financeiroimpostosnf.Id = Id;
            FinanceiroImpostosNFBo.Salvar(financeiroimpostosnf);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = FinanceiroImpostosNFBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar()
        {
            var financeiroimpostosnf = FinanceiroImpostosNFBo.Consultar();
            var result = new { codigo = "00", financeiroimpostosnf = financeiroimpostosnf };
            return Json(result);
        }
    }
}