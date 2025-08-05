using System;
using System.Web;
using System.Web.SessionState;
using Uranus.Suite.Controllers;

namespace Uranus.Suite
{
    /// <summary>
    /// Summary description for KeepSessionAlive
    /// </summary>
    public class KeepSessionAlive : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            context.Response.Cache.SetNoStore();
            context.Response.Cache.SetNoServerCaching();

            if (Sessao.Usuario != null)
            {
                DashboardController.ConnectedUsers();
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}