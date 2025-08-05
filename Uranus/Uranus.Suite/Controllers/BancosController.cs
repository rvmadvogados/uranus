using System;
using System.Data.Entity;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class BancosController : Controller
    {
        // GET: Bancos
        public ActionResult Index(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = BancosBo.Listar(search);
                ViewBag.search = search;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int64 Id)
        {
            var banco = BancosBo.ConsultarArray(Id);
            var result = new { codigo = "00", banco = banco };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String Nome, Int32 NumeroBanco, String Agencia, String Conta, String Convenio, String Carteira)
        {
            Bancos banco = new Bancos();
            banco.ID = Id;
            banco.Nome = Nome.Trim();
            banco.NumeroBanco = NumeroBanco;
            banco.Agencia = Agencia;
            banco.Conta = Conta;
            banco.Convenio = Convenio;
            banco.Carteira = Carteira;

            if (Id == 0)
                Id = BancosBo.Inserir(banco);

            banco.ID = Id;
            BancosBo.Salvar(banco);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = BancosBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar()
        {
            var bancos = BancosBo.ConsultarBanco();
            var result = new { codigo = "00", bancos = bancos };
            return Json(result);
        }
    }
}