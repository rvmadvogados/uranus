using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Configuration;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;
using Uranus.Data.R9;
using Uranus.Domain;
using Uranus.Business;

namespace Uranus.Suite.Controllers
{
    public class RelatorioAgendaCompromissoController : Controller
    {
        // GET: RelatorioAgendaCompromisso
        public ActionResult Index(String FiltrarDataInicio = null, String FiltrarDataFim = null, List<int> FiltrarProfissional = null, List<int> FiltrarAssunto = null)
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                String nomesede = "";

                DateTime? dataInicio = null;
                DateTime? dataFim = null;

                if (FiltrarDataInicio != null && FiltrarDataInicio.Length > 0)
                {
                    dataInicio = DateTime.Parse(FiltrarDataInicio);
                }
                if (FiltrarDataFim != null && FiltrarDataInicio.Length > 0)
                {
                    dataFim = DateTime.Parse(FiltrarDataFim);
                }

                var ds = RelatorioAgendaCompromisso.Gerar(dataInicio, dataFim, ConfigurationManager.ConnectionStrings["APIR9"].ConnectionString);

                if (FiltrarProfissional != null && FiltrarProfissional.Count > 0 && FiltrarProfissional[0] > 0)
                {
                    RelatorioAgendaCompromisso.FiltrarPorProfissional(ds, FiltrarProfissional);
                }

                if (FiltrarAssunto != null && FiltrarAssunto.Count > 0 && FiltrarAssunto[0] > 0)
                {
                    RelatorioAgendaCompromisso.FiltrarPorAssunto(ds, FiltrarAssunto);
                }
                //else
                //{
                //    RelatorioPorSede.LimparRetorno(ds);
                //}

                var viewer = new Microsoft.Reporting.WebForms.ReportViewer();
                viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\rptRelatorioAgendaCompromisso.rdlc";
                viewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("dsRelatorioAgendaCompromisso", ds));

                viewer.SizeToReportContent = true;
                viewer.Width = Unit.Percentage(130);
                viewer.Height = Unit.Percentage(130);
                viewer.LocalReport.DisplayName = "Relatório Agenda de Compromissos";

                var textoData = "";
                var textoCabecalho = "";
                if (FiltrarDataInicio != "")
                {
                    textoData = $"Data Cadastro de {FiltrarDataInicio} a {FiltrarDataFim}";
                }

                textoCabecalho = "Período: " + textoData;

                ReportParameter[] parametros = new ReportParameter[1];
                parametros[0] = new ReportParameter("TextoCabecalho", textoCabecalho, false);
                viewer.LocalReport.SetParameters(parametros);

                ViewBag.ReportViewer = viewer;
                ViewBag.FiltrarProfissional = FiltrarProfissional;
                ViewBag.FiltrarAssunto = FiltrarAssunto;

                return View();
            }
        }
    }
}