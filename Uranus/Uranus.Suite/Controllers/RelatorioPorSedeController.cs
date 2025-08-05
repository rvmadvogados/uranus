using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Configuration;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;
using Uranus.Data.R9;
using Uranus.Domain;
using Uranus.Business;
using System.Linq;

namespace Uranus.Suite.Controllers
{
    public class RelatorioPorSedeController : Controller
    {
        // GET: RelatorioPorSede
        public ActionResult Index(List<int> FiltrarSedes = null, List<int> FiltrarAcao = null, List<int> FiltrarArea = null, List<int> FiltrarJuizo = null, String FiltrarDataInicio = "", String FiltrarDataFim = "", String FiltrarCliente = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                String nomesede = "";
                String nomejuizo = "";

                DateTime? dataInicio = null;
                DateTime? dataFim = null;

                var viewer = new Microsoft.Reporting.WebForms.ReportViewer();
                viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\rptRelatorioPorSede.rdlc";

                if (FiltrarSedes != null || FiltrarAcao != null || FiltrarArea != null || FiltrarJuizo != null || FiltrarDataInicio.Length > 0 || FiltrarDataFim.Length > 0 || FiltrarCliente.Length > 0)
                {

                    if (FiltrarDataInicio != null && FiltrarDataInicio.Length > 0)
                    {
                        dataInicio = DateTime.Parse(FiltrarDataInicio);
                    }
                    if (FiltrarDataFim != null && FiltrarDataFim.Length > 0)
                    {
                        dataFim = DateTime.Parse(FiltrarDataFim);
                    }

                    string sedes = FiltrarSedes != null ? string.Join(",", FiltrarSedes.Select(x => x.ToString())) : null;
                    string acoes = FiltrarAcao != null ? string.Join(",", FiltrarAcao.Select(x => x.ToString())) : null;
                    string area = FiltrarArea != null ? string.Join(",", FiltrarArea.Select(x => x.ToString())) : null;
                    string juizo = FiltrarJuizo != null ? string.Join(",", FiltrarJuizo.Select(x => x.ToString())) : null;
                    

                    var ds = RelatorioPorSede.Gerar(dataInicio, dataFim, FiltrarCliente, sedes, acoes, area, juizo, ConfigurationManager.ConnectionStrings["APIR9"].ConnectionString);

                    viewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("dsRelatorioPorSede", ds));
                }
                else
                {
                    List<ItemRelatorioPorSede> ds = new List<ItemRelatorioPorSede>();

                    viewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("dsRelatorioPorSede", ds));
                }

                viewer.SizeToReportContent = true;
                viewer.Width = Unit.Percentage(130);
                viewer.Height = Unit.Percentage(130);
                viewer.LocalReport.DisplayName = "Relatório de Processos";

                var textoCabecalho = "";
                if (nomesede != "")
                {
                    textoCabecalho = "Sede: " + nomesede;
                    if (nomejuizo != "")
                    {
                        textoCabecalho = textoCabecalho + " - Juizo: " + nomejuizo;
                    }
                }
                else
                {
                    if (nomejuizo != "")
                    {
                        textoCabecalho = "Juizo: " + nomejuizo;
                    }
                    else
                    {
                        textoCabecalho = " ";
                    }
                }

                ReportParameter[] parametros = new ReportParameter[1];
                parametros[0] = new ReportParameter("TextoCabecalho", textoCabecalho, false);
                viewer.LocalReport.SetParameters(parametros);
                ViewBag.ReportViewer = viewer;

                ViewBag.FiltrarSedes = FiltrarSedes;
                ViewBag.FiltrarAcao = FiltrarAcao;
                ViewBag.FiltrarArea = FiltrarArea;
                ViewBag.FiltrarJuizo = FiltrarJuizo;
                ViewBag.FiltrarDataInicio = FiltrarDataInicio;
                ViewBag.FiltrarDataFim = FiltrarDataFim;
                ViewBag.FiltrarCliente = FiltrarCliente;
                return View();
            }
        }
    }
}