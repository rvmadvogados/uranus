using System;
using System.Web.Mvc;
using Uranus.Business;


namespace Uranus.Suite.Controllers
{
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