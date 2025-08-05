using System;
using System.Data.SqlClient;
using System.IO;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Data;
using Uranus.Suite;

namespace Uranus.WebManager.Controllers
{
    public class ColaboradoresFeriasController : Controller
    {
        public ActionResult Index(string AnoInicio = "", string AnoFim = "", string Cidade = "", DateTime? vencimento = null)
        {

            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return View();
            }
        }

        public ActionResult GerarFerias(Int32 IdProfissional, String DataInicio, String DataFim, String DiasFerias, String DiasRecesso)
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                try
                {
                    Int32 diasFerias = 0;
                    Int32 diasRecesso = 0;

                    DateTime? dataInicio = null;
                    DateTime? dataFim = null;
                    var Usuario = Sessao.Usuario.Nome;

                    if (DataInicio != null && DataInicio.Trim().Length > 0)
                    {
                        dataInicio = DateTime.Parse(DataInicio);
                    }
                    if (DataFim != null && DataFim.Trim().Length > 0)
                    {
                        dataFim = DateTime.Parse(DataFim);
                    }

                    if (DiasFerias != null && DiasFerias.Trim().Length > 0)
                    {
                        diasFerias = int.Parse(DiasFerias);
                    }
                    if (DiasRecesso != null && DiasRecesso.Trim().Length > 0)
                    {
                        diasRecesso = int.Parse(DiasRecesso);
                    }

                    if (dataInicio != null && dataFim != null && (diasFerias > 0 || diasRecesso > 0))
                    {
                        using (var context = new UranusEntities())
                        {
                            SqlParameter param1 = new SqlParameter("@IdProfissional", IdProfissional);
                            SqlParameter param2 = new SqlParameter("@DataInicio", dataInicio);
                            SqlParameter param3 = new SqlParameter("@DataFim", dataFim);
                            SqlParameter param4 = new SqlParameter("@DiasFerias", diasFerias);
                            SqlParameter param5 = new SqlParameter("@DiasRecesso", diasRecesso);
                            SqlParameter param6 = new SqlParameter("@Usuario", Usuario);
                            context.Database.ExecuteSqlCommand("stpGeraRecessoFerias @IdProfissional, @DataInicio, @DataFim, @DiasFerias, @DiasRecesso, @Usuario", param1, param2, param3, param4, param5, param6);
                        }
                    }

                    var result = new { codigo = "00" };
                    return Json(result);
                }
                catch (Exception ex)
                {
                    var resultError = new { codigo = "99", error = ex.Message.ToString() };
                    return Json(resultError);
                }


            }
        }

    }
}
