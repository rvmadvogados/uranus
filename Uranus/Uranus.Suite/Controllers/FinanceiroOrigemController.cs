using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class FinanceiroOrigemController : Controller
    {
        public ActionResult Index(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = FinanceiroOrigemBo.Listar(search);
                ViewBag.search = search;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var financeiroorigem = FinanceiroOrigemBo.ConsultarArray(Id);
            var result = new { codigo = "00", financeiroorigem = financeiroorigem };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String Nome, string Tipo, Int32? IdCaixa, Int32? IdBanco)
        {
            FinanceiroOrigem financeiroorigem = new FinanceiroOrigem();
            financeiroorigem.Id = Id;
            financeiroorigem.Nome = Nome.Trim();
            financeiroorigem.Tipo = Tipo.Trim();

            if (Tipo == "C")
            {
                financeiroorigem.IdCaixa = IdCaixa;
                financeiroorigem.IdBanco = null;
            }
            else
            {
                if (Tipo == "B")
                {
                    financeiroorigem.IdCaixa = null;
                    financeiroorigem.IdBanco = IdBanco;
                }
                else
                {
                    financeiroorigem.IdCaixa = null;
                    financeiroorigem.IdBanco = null;
                }
            }

            if (Id == 0)
                Id = FinanceiroOrigemBo.Inserir(financeiroorigem);

            financeiroorigem.Id = Id;
            FinanceiroOrigemBo.Salvar(financeiroorigem);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = FinanceiroOrigemBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar()
        {
            var financeiroorigem = FinanceiroOrigemBo.Consultar();
            var result = new { codigo = "00", financeiroorigem = financeiroorigem };
            return Json(result);
        }
    }
}