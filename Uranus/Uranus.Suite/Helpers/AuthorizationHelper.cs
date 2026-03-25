using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Uranus.Suite.Helpers
{
    public static class AuthorizationHelper
    {
        public static bool HasClaim(string claimType, string claimValue)
        {
            var user = HttpContext.Current?.User as ClaimsPrincipal;
            if (user == null || Sessao.Usuario == null)
                return false;

            if (user.HasClaim(claimType, claimValue))
                return true;

            if (claimType == "submenu" && claimValue.Contains(":"))
            {
                var menuPrincipal = claimValue.Split(':')[0];
                if (user.HasClaim("menu", menuPrincipal))
                    return true;
            }

            return false;
        }

     
        public static bool HasMenuAccess(string menuName)
        {
            return HasClaim("menu", menuName);
        }

     
        public static bool HasSubmenuAccess(string submenuName)
        {
            return HasClaim("submenu", submenuName);
        }

        public static bool HasAnyOfClaims(string claimType, params string[] claimValues)
        {
            foreach (var claimValue in claimValues)
            {
                if (HasClaim(claimType, claimValue))
                    return true;
            }
            return false;
        }
    }

    public static class HtmlAuthorizationExtensions
    {

        public static MvcHtmlString IfAuthorized(this HtmlHelper helper, string claimType, string claimValue, string content)
        {
            if (AuthorizationHelper.HasClaim(claimType, claimValue))
            {
                return MvcHtmlString.Create(content);
            }
            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString IfHasMenu(this HtmlHelper helper, string menuName, string content)
        {
            return helper.IfAuthorized("menu", menuName, content);
        }

        public static MvcHtmlString IfHasSubmenu(this HtmlHelper helper, string submenuName, string content)
        {
            return helper.IfAuthorized("submenu", submenuName, content);
        }
    }
}