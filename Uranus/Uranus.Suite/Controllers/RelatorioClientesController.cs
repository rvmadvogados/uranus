using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Configuration;
using Uranus.Data.R9;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;

namespace Uranus.Suite.Controllers
{
    public class RelatorioClientesController : Controller
    {
        //GET: RelatorioEventos
        public ActionResult Index(List<int> FiltrarSedes = null)
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var ds = RelatorioClientesEtiquetas.Gerar(ConfigurationManager.ConnectionStrings["APIR9"].ConnectionString);

                if (FiltrarSedes != null && FiltrarSedes.Count > 0 && FiltrarSedes[0] > 0)
                {
                    RelatorioClientesEtiquetas.FiltrarPorSede(ds, FiltrarSedes);
                }

                var viewer = new Microsoft.Reporting.WebForms.ReportViewer();
                viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\rptEtiquetasClientes.rdlc";
                viewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("dsEtiquetasClientes", ds));

                viewer.SizeToReportContent = true;
                viewer.Width = Unit.Pixel(950);
                viewer.Height = Unit.Percentage(130);
                viewer.LocalReport.DisplayName = "Relatório Etiquetas de Clientes";

                //                    var textoData = $"De {FiltrarDataInicio} a {FiltrarDataFim}";

                //                ReportParameter[] parametros = new ReportParameter[1];
                //      parametros[0] = new ReportParameter("TextoCabecalho", textoData, false);
                //                viewer.LocalReport.SetParameters(parametros);

                ViewBag.ReportViewer = viewer;
                ViewBag.FiltrarSedes = FiltrarSedes;

                return View();
            }
        }
    }
}