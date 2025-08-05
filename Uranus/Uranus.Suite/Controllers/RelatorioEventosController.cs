using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Configuration;
using Uranus.Data.R9;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;

namespace Uranus.Suite.Controllers
{

    public class RelatorioEventosController : Controller
    {
        //GET: RelatorioEventos

        //private Models.UranusDataset ds = Models.DatasetRelatorio.ObterExemplo();

        public ActionResult Index(String FiltrarDataInicio = null, String FiltrarDataFim = null, String TipoRelatorio = null, List<int> FiltrarProfissionais = null, List<int> FiltrarTiposEventos = null)
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                PrepararDatas(ref FiltrarDataInicio, ref FiltrarDataFim);

                if (VerificarDatasPreenchidas(FiltrarDataInicio, FiltrarDataFim))
                {
                    PrepararRelatório(FiltrarDataInicio, FiltrarDataFim, TipoRelatorio, FiltrarProfissionais, FiltrarTiposEventos);
                }

                return View();
            }
        }

        private static bool VerificarDatasPreenchidas(string FiltrarDataInicio, string FiltrarDataFim)
        {
            return FiltrarDataInicio.Length > 0 && FiltrarDataFim.Length > 0;
        }

        private static void PrepararDatas(ref string FiltrarDataInicio, ref string FiltrarDataFim)
        {
            if (FiltrarDataInicio == null)
            {
                FiltrarDataInicio = DateTime.Now.ToString("dd/MM/yyyy");
            }
            if (FiltrarDataFim == null)
            {
                FiltrarDataFim = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }

        private void PrepararRelatório(string FiltrarDataInicio, string FiltrarDataFim, string TipoRelatorio, List<int> FiltrarProfissionais, List<int> FiltrarTiposEventos)
        {
            if (TipoRelatorio == "2")
            {
                var ds = RelatórioEventos.Gerar(DateTime.Parse(FiltrarDataInicio), DateTime.Parse(FiltrarDataFim), ConfigurationManager.ConnectionStrings["APIR9"].ConnectionString);

                if (FiltrarProfissionais != null && FiltrarProfissionais.Count > 0 && FiltrarProfissionais[0] > 0)
                {
                    RelatórioEventos.FiltrarPorProfissional(ds, FiltrarProfissionais);
                }

                if (FiltrarTiposEventos != null && FiltrarTiposEventos.Count > 0 && FiltrarTiposEventos[0] > 0)
                {
                    RelatórioEventos.FiltrarPorEvento(ds, FiltrarTiposEventos);
                }

                var viewer = new Microsoft.Reporting.WebForms.ReportViewer();
                viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\rptRelatorioEventos.rdlc";
                viewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("dsRelatorioEventos", ds));

                //// Exportação do Relatório ////
                viewer.SizeToReportContent = true;
                viewer.Width = Unit.Percentage(130);
                viewer.Height = Unit.Percentage(130);
                viewer.LocalReport.DisplayName = "Relatório de Eventos";
                ////                        ////

                var textoData = $"De {FiltrarDataInicio} a {FiltrarDataFim}";

                ReportParameter[] parametros = new ReportParameter[1];
                parametros[0] = new ReportParameter("TextoCabecalho", textoData, false);
                viewer.LocalReport.SetParameters(parametros);
                ViewBag.ReportViewer = viewer;
                ViewBag.FiltrarDataInicio = FiltrarDataInicio;
                ViewBag.FiltrarDataFim = FiltrarDataFim;
                ViewBag.TipoRelatorio = TipoRelatorio;
                ViewBag.FiltrarProfissionais = FiltrarProfissionais;
                ViewBag.FiltrarTiposEventos = FiltrarTiposEventos;
            }
            else
            {
                var ds = RelatorioEventosPorCliente.Gerar(DateTime.Parse(FiltrarDataInicio), DateTime.Parse(FiltrarDataFim), ConfigurationManager.ConnectionStrings["APIR9"].ConnectionString);

                if (FiltrarProfissionais != null && FiltrarProfissionais.Count > 0 && FiltrarProfissionais[0] > 0)
                {
                    RelatorioEventosPorCliente.FiltrarPorProfissional(ds, FiltrarProfissionais);
                }

                if (FiltrarTiposEventos != null && FiltrarTiposEventos.Count > 0 && FiltrarTiposEventos[0] > 0)
                {
                    RelatorioEventosPorCliente.FiltrarPorEvento(ds, FiltrarTiposEventos);
                }

                var viewer = new Microsoft.Reporting.WebForms.ReportViewer();
                viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\rptRelatorioEventosPorCliente.rdlc";
                viewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("dsRelatorioEventosPorCliente", ds));

                //// Exportação do Relatório ////
                viewer.SizeToReportContent = true;
                viewer.Width = Unit.Percentage(130);
                viewer.Height = Unit.Percentage(130);
                viewer.LocalReport.DisplayName = "Relatório de Eventos";
                ////                        ////

                var textoData = $"De {FiltrarDataInicio} a {FiltrarDataFim}";

                ReportParameter[] parametros = new ReportParameter[1];
                parametros[0] = new ReportParameter("TextoCabecalho", textoData, false);
                viewer.LocalReport.SetParameters(parametros);

                ViewBag.ReportViewer = viewer;
                ViewBag.FiltrarDataInicio = FiltrarDataInicio;
                ViewBag.FiltrarDataFim = FiltrarDataFim;
                ViewBag.TipoRelatorio = TipoRelatorio;
                ViewBag.FiltrarProfissionais = FiltrarProfissionais;
                ViewBag.FiltrarTiposEventos = FiltrarTiposEventos;

            }

        }

        //public ActionResult Imprimir(String FiltrarDataInicio = null, String FiltrarDataFim = null, String FiltrarProfissionais = null, String FiltrarTiposEventos = null)
        //{
        //    if (FiltrarDataInicio.Length > 0 && FiltrarDataFim.Length > 0)
        //    {
        //        var profissionaisFiltrados = new List<int>();

        //        foreach (var item in FiltrarProfissionais.Split(','))
        //        {
        //            profissionaisFiltrados.Add(int.Parse(item));
        //        }

        //        var tiposFiltrados = new List<int>();

        //        foreach (var item in FiltrarTiposEventos.Split(','))
        //        {
        //            tiposFiltrados.Add(int.Parse(item));
        //        }

        //        var ds = RelatórioEventos.Gerar(DateTime.Parse(FiltrarDataInicio), DateTime.Parse(FiltrarDataFim), ConfigurationManager.ConnectionStrings["APIR9"].ConnectionString);

        //        if (profissionaisFiltrados[0] > 0 && profissionaisFiltrados.Count > 0)
        //        {
        //            RelatórioEventos.FiltrarPorProfissional(ds, profissionaisFiltrados);
        //        }

        //        if (tiposFiltrados[0] > 0 && tiposFiltrados.Count > 0)
        //        {
        //            RelatórioEventos.FiltrarPorEvento(ds, tiposFiltrados);
        //        }


        //        var viewer = new Microsoft.Reporting.WebForms.ReportViewer();
        //        viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
        //        viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\EventosReports.rdlc";
        //        viewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("dsRelatoriosEventos", ds));

        //        var textoData = $"De {FiltrarDataInicio} a {FiltrarDataFim}";

        //        ReportParameter[] parametros = new ReportParameter[1];
        //        parametros[0] = new ReportParameter("TextoCabecalho", textoData, false);
        //        viewer.LocalReport.SetParameters(parametros);

        //        viewer.SizeToReportContent = true;
        //        viewer.Width = System.Web.UI.WebControls.Unit.Percentage(100);
        //        viewer.Height = System.Web.UI.WebControls.Unit.Percentage(100);

        //        ViewBag.ReportViwer = viewer;
        //    }

        //    return View();
        //}

        //private void LocalReport_SubreportProcessing(object sender, Microsoft.Reporting.WebForms.SubreportProcessingEventArgs e)
        //{
        //    //            e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("DataSetRelatoriosDetalhe", (System.Data.DataTable)ds.Endereco));
        //}
    }
}