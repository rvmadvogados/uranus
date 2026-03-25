using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Suite.Filters;

namespace Uranus.Suite.Controllers
{
    [RequireMenu("Processos")]
    public class PartesController : Controller
    {
        [HttpPost]
        public JsonResult Listar(Int32 IdProcesso)
        {
            var partes = ProcessosPartesBo.Consultar(IdProcesso);
            var result = new { codigo = "00", partes = partes };
            return Json(result);
        }
    }
}