using System;
using System.Web.Mvc;
using Uranus.Business;

namespace Uranus.Suite.Controllers
{
    public class AuditoriaController : Controller
    {
        // GET: Auditoria
        public ActionResult Index(string data = "", string numeroProcesso = "", string nome = "", string modulo = "", string tipo = "", string acao = "", string usuario = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = AuditoriaBo.Listar(data, numeroProcesso, modulo, tipo, acao, usuario, nome);
                ViewBag.Data = data;
                ViewBag.numeroProcesso = numeroProcesso;
                ViewBag.Modulo = modulo;
                ViewBag.Tipo = tipo;
                ViewBag.Acao = acao;
                ViewBag.Usuario = usuario;
                ViewBag.Nome = nome;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int64 Id)
        {
            var auditoria = AuditoriaBo.ConsultarArray(Id);
            var result = new { codigo = "00", auditoria = auditoria };
            return Json(result);
        }

    }
}