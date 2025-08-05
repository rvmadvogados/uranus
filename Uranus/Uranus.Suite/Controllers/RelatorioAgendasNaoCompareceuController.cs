using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Configuration;
using Uranus.Data.R9;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;


namespace Uranus.Suite.Controllers
{
    public class RelatorioAgendasNaoCompareceuController : Controller
    {
        //GET: RelatorioEventos
        public ActionResult Index(String FiltrarDataInicio = null, String FiltrarDataFim = null, List<int> FiltrarSedes = null)
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                if (FiltrarDataInicio == null)
                {
                    FiltrarDataInicio = DateTime.Now.ToString("dd/MM/yyyy");
                }
                if (FiltrarDataFim == null)
                {
                    FiltrarDataFim = DateTime.Now.ToString("dd/MM/yyyy");
                }

                if (FiltrarDataInicio.Length > 0 && FiltrarDataFim.Length > 0)
                {
                    var ds = RelatorioAgendaNaoCompareceu.Gerar(DateTime.Parse(FiltrarDataInicio), DateTime.Parse(FiltrarDataFim), ConfigurationManager.ConnectionStrings["APIR9"].ConnectionString);

                    if (FiltrarSedes != null && FiltrarSedes.Count > 0 && FiltrarSedes[0] > 0)
                    {
                        RelatorioAgendaNaoCompareceu.FiltrarPorSede(ds, FiltrarSedes);
                    }

                    var viewer = new Microsoft.Reporting.WebForms.ReportViewer();
                    viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                    viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\rptRelatorioAgendasNaoCompareceu.rdlc";
                    viewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("dsRelatorioAgendasNaoCompareceu", ds));

                    viewer.SizeToReportContent = true;
                    viewer.Width = Unit.Percentage(130);
                    viewer.Height = Unit.Percentage(130);
                    viewer.LocalReport.DisplayName = "Relatório Agendas Não Compareceu";

                    var textoData = $"De {FiltrarDataInicio} a {FiltrarDataFim}";

                    ReportParameter[] parametros = new ReportParameter[1];
                    parametros[0] = new ReportParameter("TextoCabecalho", textoData, false);
                    viewer.LocalReport.SetParameters(parametros);

                    ViewBag.ReportViewer = viewer;
                    ViewBag.FiltrarDataInicio = FiltrarDataInicio;
                    ViewBag.FiltrarDataFim = FiltrarDataFim;
                    ViewBag.FiltrarSedes = FiltrarSedes;
                }

                return View();
            }
        }
    }
}