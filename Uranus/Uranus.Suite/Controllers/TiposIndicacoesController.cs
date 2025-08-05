using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class TiposIndicacoesController : Controller
    {
        // GET: TiposIndicacoes
        public ActionResult Index(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = TiposIndicacoesBo.Listar(search);
                ViewBag.search = search;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var indicacao = TiposIndicacoesBo.ConsultarArray(Id);
            var result = new { codigo = "00", indicacao = indicacao };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String Nome, Int32 Tipo)
        {
            ProcessosIndicacoesTipos indicacao = new ProcessosIndicacoesTipos();
            indicacao.ID = Id;
            indicacao.Nome = Nome.Trim();
            indicacao.Tipo = Tipo;

            if (Id == 0)
                Id = TiposIndicacoesBo.Inserir(indicacao);

            indicacao.ID = Id;
            TiposIndicacoesBo.Salvar(indicacao);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = TiposIndicacoesBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar()
        {
            var indicacoes = TiposIndicacoesBo.Consultar();
            var result = new { codigo = "00", indicacoes = indicacoes };
            return Json(result);
        }
    }
}