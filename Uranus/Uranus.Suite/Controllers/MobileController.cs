using System.Web.Mvc;
using Uranus.Suite.Filters;

namespace Uranus.Suite.Controllers
{
    [RequireMenu("AgendaDeCompromissos")]
    public class MobileController : Controller
    {
        // GET: Mobile
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Agendas()
        {
            return View();
        }

    }
}