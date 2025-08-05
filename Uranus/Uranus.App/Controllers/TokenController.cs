using System.Linq;
using System.Web.Mvc;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.App.Controllers
{
    public class TokenController : Controller
    {
        public static bool Validar(string token)
        {
            bool resposta = false;
            
            if (Consultar(token) != null)
            {
                resposta = true;
            }

            return resposta;
        }

        public static AppConfiguracao Consultar(string token)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.AppConfiguracao
                            where d.Token == token
                            select d;

                return query.FirstOrDefault();
            }
        }
    }
}