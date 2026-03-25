using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;

[assembly: OwinStartup(typeof(Uranus.Suite.Startup))]
namespace Uranus.Suite
{
    public class Startup
    {
        //public void Configuration(IAppBuilder app)
        //{
        //    app.UseCookieAuthentication(new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions
        //    {
        //        AuthenticationType = "ApplicationCookie",
        //        LoginPath = new PathString("/Login/Index")
        //    });
        //}
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                LoginPath = new PathString("/Login/Index"),

                ExpireTimeSpan = TimeSpan.FromMinutes(525600),
                SlidingExpiration = true,

                CookieHttpOnly = true,
                CookieSecure = CookieSecureOption.SameAsRequest,
                CookieName = "UranusAuthCookie"
            });
        }
    }
}