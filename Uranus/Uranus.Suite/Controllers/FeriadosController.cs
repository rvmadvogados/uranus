using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class FeriadosController : Controller
    {
        // GET: Feriados
        public ActionResult Index(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = FeriadosBo.Listar(search);
                ViewBag.search = search;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var feriado = FeriadosBo.ConsultarArray(Id);
            var result = new { codigo = "00", feriado = feriado };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String Data, String Nome)
        {
            string[] datas = Data.Replace(" ", "").Split('-');
            DateTime datainicial = DateTime.Parse(datas[0]);
            DateTime datafinal = DateTime.Parse(datas[1]);

            var dias = (datafinal - datainicial).TotalDays;

            for (int i = 0; i <= dias; i++)
            {
                Feriados feriado = new Feriados();
                feriado.ID = Id;
                feriado.Data = datainicial.AddDays(i);
                feriado.Descricao = Nome.Trim();

                if (Id == 0)
                    Id = FeriadosBo.Inserir(feriado);

                feriado.ID = Id;
                FeriadosBo.Salvar(feriado);
                
                Id = 0;
            }

            Sessao.FeriadosRecesso = FeriadosBo.Buscar();

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = FeriadosBo.Excluir(Id);

            Sessao.FeriadosRecesso = FeriadosBo.Buscar();

            var result = new { codigo = codigo };
            return Json(result);
        }

        public static string Desabilitar()
        {
            var desabilitados = string.Empty;

            var feriados = FeriadosBo.Consultar();

            foreach (var feriado in feriados)
            {
                desabilitados += (desabilitados.Length > 0 ? ", " : string.Empty) + string.Format("'{0}'", feriado.Data.Value.ToString("dd/MM/yyyy"));
            }

            return desabilitados;
        }

        [HttpPost]
        public JsonResult Buscar()
        {
            var feriados = FeriadosBo.Buscar();
            var result = new { codigo = "00", feriados = feriados };
            return Json(result);
        }
    }
}