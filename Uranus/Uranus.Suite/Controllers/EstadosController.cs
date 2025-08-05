using System.IO;
using System.Web.Mvc;
using Uranus.Business;

namespace Uranus.Suite.Controllers
{
    public class EstadosController : Controller
    {
        [HttpPost]
        public JsonResult Listar()
        {
            string path = Path.Combine(Server.MapPath("~/Models/Estados.json"));

            var estados = EstadosBo.ListarArray(path);
            var result = new { codigo = "00", estados = estados };
            return Json(result);
        }
    }
}