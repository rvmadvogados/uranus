using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Uranus.Suite.Filters
{
    /// <summary>
    /// Filtro de autorização baseado em claims para proteger controllers e actions
    /// </summary>
    public class ClaimsAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string _claimType;
        private readonly string _claimValue;

        /// <summary>
        /// Construtor para autorização baseada em claim específica
        /// </summary>
        /// <param name="claimType">Tipo da claim (ex: "menu", "submenu")</param>
        /// <param name="claimValue">Valor da claim (ex: "Usuarios", "Cadastros")</param>
        public ClaimsAuthorizeAttribute(string claimType, string claimValue)
        {
            _claimType = claimType;
            _claimValue = claimValue;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!base.AuthorizeCore(httpContext))
            {
                return false;
            }

            if (Sessao.Usuario == null)
            {
                return false;
            }

            var user = httpContext.User as ClaimsPrincipal;
            if (user != null)
            {
                if (user.HasClaim(_claimType, _claimValue))
                {
                    return true;
                }

                // Se for uma claim de submenu, verifica se tem o menu pai
                //if (_claimType == "submenu" && _claimValue.Contains(":"))
                //{
                //    var menuPrincipal = _claimValue.Split(':')[0];
                //    if (user.HasClaim("menu", menuPrincipal))
                //    {
                //        return true;
                //    }
                //}
            }

            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // Usuário está logado mas não tem permissão
                filterContext.Result = new ViewResult
                {
                    ViewName = "~/Views/Shared/Unauthorized.cshtml"
                };
            }
            else
            {
                // Usuário não está logado, redireciona para login
                filterContext.Result = new RedirectResult("~/Login");
            }
        }
    }

    /// <summary>
    /// Atributo simplificado para menu
    /// </summary>
    public class RequireMenuAttribute : ClaimsAuthorizeAttribute
    {
        public RequireMenuAttribute(string menuName) : base("menu", menuName) { }
    }

    /// <summary>
    /// Atributo simplificado para submenu
    /// </summary>
    public class RequireSubmenuAttribute : ClaimsAuthorizeAttribute
    {
        public RequireSubmenuAttribute(string submenuName) : base("submenu", submenuName) { }
    }
}