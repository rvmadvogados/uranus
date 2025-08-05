using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class PartesAdvogadosController : Controller
    {
        [HttpPost]
        public JsonResult Listar(Int32 IdProcesso)
        {
            var advogados = ProcessosPartesAdvogadosBo.Consultar(IdProcesso);
            var result = new { codigo = "00", advogados = advogados };
            return Json(result);
        }
    }
}