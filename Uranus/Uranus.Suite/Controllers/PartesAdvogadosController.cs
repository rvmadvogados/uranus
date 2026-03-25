using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;
using Uranus.Suite.Filters;

namespace Uranus.Suite.Controllers
{
    [RequireMenu("Processos")]
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