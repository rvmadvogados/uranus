using System.IO;
using System.Web.Mvc;
using Uranus.Business;

namespace Uranus.Suite.Controllers
{
    public class CidadesController : Controller
    {
        [HttpPost]
        public JsonResult Listar(string UF)
        {
            string path = Path.Combine(Server.MapPath("~/Models/Cidades.json"));

            var cidades = CidadesBo.ListarArray(path, UF);
            var result = new { codigo = "00", cidades = cidades };
            return Json(result);
        }
    }
}