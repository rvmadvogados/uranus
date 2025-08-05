using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Common;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class UsuariosController : Controller
    {
        public ActionResult Index(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = UsuariosBo.Listar(search);
                ViewBag.search = search;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int64 Id)
        {
            var usuario = UsuariosBo.ConsultarArray(Id);
            var result = new { codigo = "00", usuario = usuario };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String Nome, String Usuario, Int32 Nivel, String Senha, String Ativo)
        {
            Usuarios usuario = new Usuarios();
            usuario.ID = Id;
            usuario.Nome = Nome.Trim();
            usuario.Login = Usuario.Trim();

            if (!String.IsNullOrEmpty(Senha))
            {
                usuario.Senha = Util.GerarHashMd5(Senha.Trim());
            }

            usuario.Nivel = Nivel;

            if (Ativo == "S")
            {
                usuario.Bloqueio = false;
            }
            else
            {
                usuario.Bloqueio = true;
            }

            if (Id == 0)
                Id = UsuariosBo.Inserir(usuario);

            usuario.ID = Id;
            UsuariosBo.Salvar(usuario);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = UsuariosBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar()
        {
            var usuarios = UsuariosBo.ListarArray();
            var result = new { codigo = "00", usuarios = usuarios };
            return Json(result);
        }

        [HttpPost]
        public JsonResult AlterarSenha(String SenhaAux)
        {
            Usuarios usuario = new Usuarios();
            usuario.ID = Sessao.Usuario.ID;
            usuario.Senha = Util.GerarHashMd5(SenhaAux);
            usuario.Nivel = Sessao.Usuario.Nivel;
            usuario.Bloqueio = Sessao.Usuario.Bloqueio;

            UsuariosBo.Salvar(usuario);

            var result = new { codigo = "00" };
            return Json(result);
        }
    }
}