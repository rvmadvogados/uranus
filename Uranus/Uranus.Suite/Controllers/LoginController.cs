using System;
using Uranus.Business;
using Uranus.Common;
using System.Web.Mvc;
using System.Configuration;
using Uranus.Domain.Entities;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            if (Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("Index", "Mobile");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public JsonResult Validar(String Usuario, String Senha)
        {
            var usuario = UsuariosBo.Validar(Usuario, Util.GerarHashMd5(Senha));

            if (usuario != null)
            {
                var nivel = usuario.Nivel;
                Sessao.Aplicativo = ConfigurationManager.AppSettings["ApplicationName"];
                Sessao.Usuario = usuario;
                Sessao.Setting = SettingsBO.Consultar();

                Sessao.ProcessRowIndex = String.Empty;
                Sessao.ProcessNumber = String.Empty;
                Sessao.ClientName = String.Empty;
                Sessao.AreaType = String.Empty;
                Sessao.ProcessStatus = String.Empty;
                Sessao.Judgment = String.Empty;

                Sessao.FeriadosRecesso = FeriadosBo.Buscar();

                Connected conectado = new Connected();
                conectado.IP = Util.GetLocalIPAddress();
                conectado.SistemaOperacional = Util.GetOSVersion();
                conectado.Navegador = Util.GetWebBrowserName();

                Sessao.Conectado = conectado;

                DashboardController.ConnectedUsers();

                var result = new { response = "success", aplicativo = Sessao.Aplicativo, nivel = nivel };
                return Json(result);
            }
            else
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        public ActionResult DestroySession()
        {
            Sessao.Usuario = null;

            return RedirectToAction("Index", "Login");
        }

        public ActionResult Permission()
        {
            return View();
        }
    }
}