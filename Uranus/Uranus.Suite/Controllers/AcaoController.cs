using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;
using System.Linq;

namespace Uranus.Suite.Controllers
{
    public class AcaoController : Controller
    {

        public ActionResult GetAcaoList(string search, bool filter = false, int page = 1)
        {
            var acoes = AcaoBo.GetAcoesList(search, filter);
            var total = acoes.Count();

            if (!(string.IsNullOrEmpty(search) || string.IsNullOrWhiteSpace(search)))
            {
                acoes = acoes.Where(x => PessoasBo.ConverteNome(x.text).ToLower().StartsWith(PessoasBo.ConverteNome(search).ToLower())).Take(page * 10).ToList();
            }

            return Json(new { acoes = acoes, total = total }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Index(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = AcaoBo.Listar(search);
                ViewBag.search = search;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var acao = AcaoBo.ConsultarArray(Id);
            var result = new { codigo = "00", acao = acao };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String Nome)
        {
            ProcessosCadastroAcao acao = new ProcessosCadastroAcao();
            acao.ID = Id;
            acao.Acao = Nome.Trim();

            if (Id == 0)
                Id = AcaoBo.Inserir(acao);

            acao.ID = Id;
            AcaoBo.Salvar(acao);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = AcaoBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar()
        {
            var acoes = AcaoBo.Consultar();
            var result = new { codigo = "00", acoes = acoes };
            return Json(result);
        }
    }
}