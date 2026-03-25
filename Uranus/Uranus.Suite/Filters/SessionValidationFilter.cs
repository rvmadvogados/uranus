using System.Web;
using System.Web.Mvc;
using Microsoft.Owin;

namespace Uranus.Suite.Filters
{
    public class SessionValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            if (controllerName == "Login" 
                || controllerName == "Mobile" 
                || controllerName == "Configurar2FA")
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            if (!Sessao.IsSessionValid)
            {
                var authManager = filterContext.HttpContext.GetOwinContext().Authentication;
                authManager.SignOut("ApplicationCookie");

                filterContext.Result = new RedirectResult("~/Login");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
