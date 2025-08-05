using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class ProfissoesController : Controller
    {
        public ActionResult Index(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = ProfissoesBo.Listar(search);
                ViewBag.search = search;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var profissao = ProfissoesBo.ConsultarArray(Id);
            var result = new { codigo = "00", profissao = profissao };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String Nome, String Agenda)
        {
            Profissoes profissao = new Profissoes();
            profissao.Id = Id;
            profissao.Nome = Nome.Trim();
            if (Id == 0)
                Id = ProfissoesBo.Inserir(profissao);

            profissao.Id = Id;
            ProfissoesBo.Salvar(profissao);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = ProfissoesBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar()
        {
            var profissao = ProfissoesBo.Consultar();
            var result = new { codigo = "00", profissao = profissao };
            return Json(result);
        }
        [HttpPost]
        public JsonResult ListarClientes()
        {
            var profissao = ProfissoesBo.ConsultarClientes();
            var result = new { codigo = "00", profissao = profissao };
            return Json(result);
        }
    }
}