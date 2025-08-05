using System.Web.Mvc;

namespace Uranus.App.Controllers
{
    public class HomeController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            return View();
        }
    }
}