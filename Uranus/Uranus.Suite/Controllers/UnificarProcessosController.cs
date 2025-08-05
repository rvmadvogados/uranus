using System;
using System.Data.SqlClient;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Data;

namespace Uranus.Suite.Controllers
{
    public class UnificarProcessosController : Controller
    {
        // GET: UnificarProcessos
        public ActionResult Index()
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = ProcessosAcoesBo.Listar();
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult UnificarEventosProcesso(String Ids, Int32 IdOrigem, Int32 IdDestino)
        {
            try
            {
                Ids = Ids.Replace("on;", "");

                using (var context = new UranusEntities())
                {
                    SqlParameter param1 = new SqlParameter("@IdOrigem", IdOrigem);
                    SqlParameter param2 = new SqlParameter("@IdDestino", IdDestino);
                    SqlParameter param3 = new SqlParameter("@Ids", Ids);

                    context.Database.ExecuteSqlCommand("stpExcluirProcessos @IdOrigem, @IdDestino, @Ids", param1, param2, param3);

                }

                var result = new { response = "success" };
                return Json(result);

            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }


        [HttpPost]
        public JsonResult AlterarClientesAgenda(String Origem, String Destino)
        {

            try
            {
                using (var context = new UranusEntities())
                {
                    SqlParameter param1 = new SqlParameter("@NomeOrigem", Origem);
                    SqlParameter param2 = new SqlParameter("@NomeDestino", Destino);

                    context.Database.ExecuteSqlCommand("stpAlterarClienteAgenda @NomeOrigem, @NomeDestino", param1, param2);

                }

                var result = new { response = "success" };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult UnificarProcessosEventos(Int32 IdAcao)
        {
            try
            {
                var evento = ProcessosEventosBo.Visualizar(IdAcao);
                String eventos = String.Empty;
                int d = evento.Count;
                int i = 1;
                foreach (var item in evento)
                {
                    eventos += "<tr class='" + (i % 2 == 0 ? "webgrid-alternating-row" : "webgrid-row-style") + "'>";
                    eventos += "   <td>";
                    //eventos += "      <input type='checkbox' name='[checkTodos]' value=" + item.ID + " checked />" + (string.Format("{0} - {1}{2}", " Evento " + d, item.Data.Value.ToString("dd/MM/yyyy"), item.Descricao));
                    eventos += "      <input type='checkbox' name='checkLista[]' value=" + item.ID + " /> " + (string.Format("{0}-{1} {2}", " Evento " + d, item.Data.Value.ToString("dd/MM/yyyy"), item.Descricao));
                    eventos += "   </td>";
                    eventos += "</tr> ";
                    d--;
                    i++;
                }

                var result = new { response = "success", eventos = eventos };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }
    }
}