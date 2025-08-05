using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Configuration;
using Uranus.Data.R9;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;
using Uranus.Business;

namespace Uranus.Suite.Controllers
{

    public class RelatorioCancelamentoAgendaController : Controller
    {
        //GET: RelatorioEventos

        //private Models.UranusDataset ds = Models.DatasetRelatorio.ObterExemplo();

        public ActionResult Index(String FiltrarDataInicio = "", String FiltrarDataFim = "", String FiltrarDataCancelamentoInicio = "", String FiltrarDataCancelamentoFim = "", List<int> FiltrarProfissionais = null, string FiltrarCliente = null, String FiltrarTipo = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                //if (FiltrarDataInicio == null && FiltrarDataFim == null && FiltrarDataCancelamentoInicio == null && FiltrarDataCancelamentoFim == null)
                //{
                //    FiltrarDataInicio = DateTime.Now.ToString("dd/MM/yyyy");
                //    FiltrarDataFim = DateTime.Now.ToString("dd/MM/yyyy");
                //    FiltrarDataCancelamentoInicio = DateTime.Now.ToString("dd/MM/yyyy");
                //    FiltrarDataCancelamentoFim = DateTime.Now.ToString("dd/MM/yyyy");
                //}

                //DateTime? dataInicio = null;
                //DateTime? dataFim = null;
                //DateTime? dataCancelamentoInicio = null;
                //DateTime? dataCancelamentoFim = null;

                //if (FiltrarDataInicio != "")
                //{
                //    dataInicio = DateTime.Parse(FiltrarDataInicio);
                //}

                //if (FiltrarDataFim != "")
                //{
                //    dataFim = DateTime.Parse(FiltrarDataFim);
                //}
                //if (FiltrarDataCancelamentoInicio != "")
                //{
                //    dataCancelamentoInicio = DateTime.Parse(FiltrarDataCancelamentoInicio);
                //}
                //if (FiltrarDataCancelamentoFim != "")
                //{
                //    dataCancelamentoFim = DateTime.Parse(FiltrarDataCancelamentoFim);
                //}

                    
                var ds = RelatorioCancelamentoAgendaBo.Gerar(FiltrarDataInicio, FiltrarDataFim, FiltrarDataCancelamentoInicio, FiltrarDataCancelamentoFim, FiltrarTipo);
                if (FiltrarProfissionais != null && FiltrarProfissionais.Count > 0 && FiltrarProfissionais[0] > 0)
                {
                    RelatorioCancelamentoAgendaBo.FiltrarPorProfissional(ds, FiltrarProfissionais);
                }

                if (FiltrarCliente != null && FiltrarCliente.Length > 0)
                {
                    RelatorioCancelamentoAgendaBo.FiltrarPorCliente(ds, FiltrarCliente);
                }

                var viewer = new Microsoft.Reporting.WebForms.ReportViewer();
                viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\rptRelatorioCancelamentoAgenda.rdlc";
                viewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("dsRelatorioCancelamentoAgenda", ds));

                // Exportação do Relatório ////
                viewer.SizeToReportContent = true;
                viewer.Width = Unit.Percentage(130);
                viewer.Height = Unit.Percentage(130);
                viewer.LocalReport.DisplayName = "Relatório Agendas de Ausentes e Cancelados";
                //                        ////

                var textoData = "";
                var textoCumprimento = "";
                var textoCabecalho = "";

                if (FiltrarDataInicio != "")
                {
                    textoData = $"Data Agenda de {FiltrarDataInicio} a {FiltrarDataFim}";
                }

                if (FiltrarDataCancelamentoInicio != "")
                {
                    textoCumprimento = $"Data Cancelamento de {FiltrarDataCancelamentoInicio} a {FiltrarDataCancelamentoFim}";
                }

                if (textoData != "")
                {
                    textoCabecalho = textoData;
                    if (textoCumprimento != "")
                    {
                        textoCabecalho += " / " + textoCumprimento;
                    }
                }
                else
                {
                    if (textoCumprimento != "")
                    {
                        textoCabecalho = textoCumprimento;
                    }
                }

                ReportParameter[] parametros = new ReportParameter[1];
                parametros[0] = new ReportParameter("TextoCabecalho", textoCabecalho, false);
                viewer.LocalReport.SetParameters(parametros);

                //if (FiltrarDataInicio == DateTime.Now.ToString("dd/MM/yyyy") &&
                //    FiltrarDataFim == DateTime.Now.ToString("dd/MM/yyyy") &&
                //    FiltrarDataCancelamentoInicio == DateTime.Now.ToString("dd/MM/yyyy") &&
                //    FiltrarDataCancelamentoFim == DateTime.Now.ToString("dd/MM/yyyy"))
                //{
                    ViewBag.ReportViewer = viewer;
                //}
                //else
                //{
                    ViewBag.ReportViewer = viewer;
                    ViewBag.FiltrarDataInicio = FiltrarDataInicio;
                    ViewBag.FiltrarDataFim = FiltrarDataFim;
                    ViewBag.FiltrarDataCancelamentoInicio = FiltrarDataCancelamentoInicio;
                    ViewBag.FiltrarDataCancelamentoFim = FiltrarDataCancelamentoFim;
                    ViewBag.FiltrarProfissionais = FiltrarProfissionais;
                    ViewBag.FiltrarCliente = FiltrarCliente;
                    ViewBag.FiltrarTipo = FiltrarTipo;
                //}

                return View();
            }
        }


        //private void PrepararRelatório(string FiltrarDataInicio, string FiltrarDataFim, string FiltrarDataCancelamentoInicio, string FiltrarDataCancelamentoFim, List<int> FiltrarProfissionais, List<int> FiltrarClientes)
        //{
        //    var ds = RelatorioCancelamentoAgenda.Gerar(DateTime.Parse(FiltrarDataInicio), DateTime.Parse(FiltrarDataFim), DateTime.Parse(FiltrarDataCancelamentoInicio), DateTime.Parse(FiltrarDataCancelamentoFim), ConfigurationManager.ConnectionStrings["APIR9"].ConnectionString);

        //    if (FiltrarProfissionais != null && FiltrarProfissionais.Count > 0 && FiltrarProfissionais[0] > 0)
        //    {
        //        RelatorioCancelamentoAgenda.FiltrarPorProfissional(ds, FiltrarProfissionais);
        //    }

        //    if (FiltrarClientes != null && FiltrarClientes.Count > 0 && FiltrarClientes[0] > 0)
        //    {
        //        RelatorioCancelamentoAgenda.FiltrarPorCliente(ds, FiltrarClientes);
        //    }

        //    var viewer = new Microsoft.Reporting.WebForms.ReportViewer();
        //    viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
        //    viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\rptRelatorioCancelamentoAgenda.rdlc";
        //    viewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("dsRelatorioCancelamentoAgenda", ds));

        //    //// Exportação do Relatório ////
        //    viewer.SizeToReportContent = true;
        //    viewer.Width = Unit.Percentage(130);
        //    viewer.Height = Unit.Percentage(130);
        //    viewer.LocalReport.DisplayName = "Relatório de Cancelamento de Agendas";
        //    ////                        ////

        //    var textoData = "";
        //    var textoCumprimento = "";
        //    var textoCabecalho = "";

        //    if (FiltrarDataInicio != "")
        //    {
        //        textoData = $"Data Agenda de {FiltrarDataInicio} a {FiltrarDataFim}";
        //    }

        //    if (FiltrarDataCancelamentoInicio != "")
        //    {
        //        textoCumprimento = $"Data Cancelamento de {FiltrarDataCancelamentoInicio} a {FiltrarDataCancelamentoFim}";
        //    }

        //    if (textoData != "")
        //    {
        //        textoCabecalho = textoData;
        //        if (textoCumprimento != "")
        //        {
        //            textoCabecalho += " / " + textoCumprimento;
        //        }
        //    }
        //    else
        //    {
        //        if (textoCumprimento != "")
        //        {
        //            textoCabecalho = textoCumprimento;
        //        }
        //    }

        //    ReportParameter[] parametros = new ReportParameter[1];
        //    parametros[0] = new ReportParameter("TextoCabecalho", textoCabecalho, false);
        //    viewer.LocalReport.SetParameters(parametros);

        //    if (FiltrarDataInicio == DateTime.Now.ToString("dd/MM/yyyy") &&
        //        FiltrarDataFim == DateTime.Now.ToString("dd/MM/yyyy") &&
        //        FiltrarDataCancelamentoInicio == DateTime.Now.ToString("dd/MM/yyyy") &&
        //        FiltrarDataCancelamentoFim == DateTime.Now.ToString("dd/MM/yyyy"))
        //    {
        //        ViewBag.ReportViewer = viewer;
        //    }
        //    else
        //    {
        //        ViewBag.ReportViewer = viewer;
        //        ViewBag.FiltrarDataInicio = FiltrarDataInicio;
        //        ViewBag.FiltrarDataFim = FiltrarDataFim;
        //        ViewBag.FiltrarDataCancelamentoInicio = FiltrarDataCancelamentoInicio;
        //        ViewBag.FiltrarDataCancelamentoFim = FiltrarDataCancelamentoFim;
        //        ViewBag.FiltrarProfissionais = FiltrarProfissionais;
        //        ViewBag.FiltrarClientes = FiltrarClientes;
        //    }

        //}

    }
}