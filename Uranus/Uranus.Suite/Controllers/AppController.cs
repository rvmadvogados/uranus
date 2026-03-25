using System;
using System.util;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;
using Uranus.Suite.Filters;

namespace Uranus.Suite.Controllers
{
    
    public class AppController : Controller
    {
        [RequireSubmenu("IntegracaoApp:PreCadastro")]
        public ActionResult PreCadastro()
        {
            var model = AppPreCadastroBo.Listar();

            return View(model);
        }

        [RequireSubmenu("IntegracaoApp:PreAgendamento")]
        public ActionResult PreAgendamento()
        {
            var model = AppPreAgendamentoBo.Listar();

            return View(model);
        }

        [RequireSubmenu("IntegracaoApp:GerarToken")]
        public ActionResult GerarToken()
        {
            var model = AppGerarTokenBo.Consultar();

            return View(model);
        }

        [HttpPost]
        public JsonResult Salvar()
        {
            var configuracao = AppGerarTokenBo.Consultar();
            var token = Uranus.Common.Util.GerarToken();

            if (configuracao == null)
            {
                configuracao = new AppConfiguracao();
                configuracao.Token = token;
                configuracao.DataHoraAlteracao = DateTime.Now;
                configuracao.NomeUsuario = Sessao.Usuario.Nome;

                AppGerarTokenBo.Inserir(configuracao);
            }
            else
            {
                configuracao.Token = token;
                configuracao.DataHoraAlteracao = DateTime.Now;
                configuracao.NomeUsuario = Sessao.Usuario.Nome;
                AppGerarTokenBo.Salvar(configuracao);
            }

            var result = new { codigo = "00" };
            return Json(result);
        }
    }
}