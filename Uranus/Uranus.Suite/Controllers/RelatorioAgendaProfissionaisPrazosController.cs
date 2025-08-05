using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Configuration;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;
using Uranus.Data.R9;

namespace Uranus.Suite.Controllers
{
    public class RelatorioAgendaProfissionaisPrazosController : Controller
    {
        // GET: RelatorioAgendaProfissionaisPrazos
        public ActionResult Index(String FiltrarDataPrazo1Inicio = null, String FiltrarDataPrazo1Fim = null, String FiltrarDataPrazo2Inicio = null, String FiltrarDataPrazo2Fim = null, List<int> FiltrarProfissionais = null)
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                if (FiltrarDataPrazo1Inicio == null && FiltrarDataPrazo1Fim == null && FiltrarDataPrazo2Inicio == null && FiltrarDataPrazo2Fim == null)
                {
                    FiltrarDataPrazo1Inicio = DateTime.Now.ToString("dd/MM/yyyy");
                    FiltrarDataPrazo1Fim = DateTime.Now.ToString("dd/MM/yyyy");
                    FiltrarDataPrazo2Inicio = DateTime.Now.ToString("dd/MM/yyyy");
                    FiltrarDataPrazo2Fim = DateTime.Now.ToString("dd/MM/yyyy");
                }

                DateTime? dataPrazo1Inicio = null;
                DateTime? dataPrazo1Fim = null;
                DateTime? dataPrazo2Inicio = null;
                DateTime? dataPrazo2Fim = null;

                if (FiltrarDataPrazo1Inicio != "")
                {
                    dataPrazo1Inicio = DateTime.Parse(FiltrarDataPrazo1Inicio);
                }

                if (FiltrarDataPrazo1Fim != "")
                {
                    dataPrazo1Fim = DateTime.Parse(FiltrarDataPrazo1Fim);
                }
                if (FiltrarDataPrazo2Inicio != "")
                {
                    dataPrazo2Inicio = DateTime.Parse(FiltrarDataPrazo2Inicio);
                }
                if (FiltrarDataPrazo2Fim != "")
                {
                    dataPrazo2Fim = DateTime.Parse(FiltrarDataPrazo2Fim);
                }

                if (FiltrarProfissionais != null && FiltrarProfissionais.Count > 0 && FiltrarProfissionais[0] > 0)
                {
                    var ds = RelatorioAgendaProfissionaisPrazos.Gerar(dataPrazo1Inicio, dataPrazo1Fim, dataPrazo2Inicio, dataPrazo2Fim, ConfigurationManager.ConnectionStrings["APIR9"].ConnectionString);
                    //var ds = RelatorioAgendasProfissionais.Gerar((FiltrarDataPrazo1Inicio != null ? DateTime.Parse(FiltrarDataPrazo1Inicio) : null), (FiltrarDataPrazo1Fim != null ? DateTime.Parse(FiltrarDataPrazo1Fim) : null), (FiltrarDataPrazo2Inicio != null ? DateTime.Parse(FiltrarDataPrazo2Inicio) : null), (FiltrarDataPrazo2Fim != null ? DateTime.Parse(FiltrarDataPrazo2Fim) : null), ConfigurationManager.ConnectionStrings["APIR9"].ConnectionString);

                    if (FiltrarProfissionais != null && FiltrarProfissionais.Count > 0 && FiltrarProfissionais[0] > 0)
                    {
                        RelatorioAgendaProfissionaisPrazos.FiltrarPorProfissional(ds, FiltrarProfissionais);
                    }

                    var viewer = new Microsoft.Reporting.WebForms.ReportViewer();
                    viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                    viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\rptRelatorioAgendaProfissionaisPrazos.rdlc";
                    viewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("dsRelatorioAgendaProfissionaisPrazos", ds));

                    viewer.SizeToReportContent = true;
                    viewer.Width = Unit.Percentage(130);
                    viewer.Height = Unit.Percentage(130);
                    viewer.LocalReport.DisplayName = "Relatório Agenda de Profisiionais";

                    var textoData = "";
                    var textoCumprimento = "";
                    var textoCabecalho = "";

                    if (FiltrarDataPrazo1Inicio != "")
                    {
                        textoData = $"Data Prazo 1 de {FiltrarDataPrazo1Inicio} a {FiltrarDataPrazo1Fim}";
                    }

                    if (FiltrarDataPrazo2Inicio != "")
                    {
                        textoCumprimento = $"Data Prazo 2 de {FiltrarDataPrazo2Inicio} a {FiltrarDataPrazo2Fim}";
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

                    if (FiltrarDataPrazo1Inicio == DateTime.Now.ToString("dd/MM/yyyy") &&
                        FiltrarDataPrazo1Fim == DateTime.Now.ToString("dd/MM/yyyy") &&
                        FiltrarDataPrazo2Inicio == DateTime.Now.ToString("dd/MM/yyyy") &&
                        FiltrarDataPrazo2Fim == DateTime.Now.ToString("dd/MM/yyyy"))
                    {
                        ViewBag.ReportViewer = viewer;
                    }
                    else
                    {
                        ViewBag.ReportViewer = viewer;
                        ViewBag.FiltrarDataPrazo1Inicio = FiltrarDataPrazo1Inicio;
                        ViewBag.FiltrarDataPrazo1Fim = FiltrarDataPrazo1Fim;
                        ViewBag.FiltrarDataPrazo2Inicio = FiltrarDataPrazo2Inicio;
                        ViewBag.FiltrarDataPrazo2Fim = FiltrarDataPrazo2Fim;
                        ViewBag.FiltrarProfissionais = FiltrarProfissionais;
                    }
                    return View();
                }
                else
                {
                    var viewer = new Microsoft.Reporting.WebForms.ReportViewer();
                    ViewBag.ReportViewer = viewer;
                    return View();
                }
            }

        }
    }
}