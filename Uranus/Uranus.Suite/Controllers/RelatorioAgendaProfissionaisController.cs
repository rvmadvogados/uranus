using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Configuration;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;
using Uranus.Data.R9;

namespace Uranus.Suite.Controllers
{
    public class RelatorioAgendaProfissionaisController : Controller
    {
        // GET: RelatorioAgendaProfissionais
        public ActionResult Index(String FiltrarDataInicio = null, String FiltrarDataFim = null, String FiltrarCumprimentoInicio = null, String FiltrarCumprimentoFim = null, List<int> FiltrarProfissionais = null, List<int> FiltrarUsuarios = null, List<int> FiltrarTiposEventos = null, Int32 FiltrarTipo = 1)
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                if (FiltrarDataInicio == null && FiltrarDataFim == null && FiltrarCumprimentoInicio == null && FiltrarCumprimentoFim == null)
                {
                    FiltrarDataInicio = DateTime.Now.ToString("dd/MM/yyyy");
                    FiltrarDataFim = DateTime.Now.ToString("dd/MM/yyyy");
                    FiltrarCumprimentoInicio = DateTime.Now.ToString("dd/MM/yyyy");
                    FiltrarCumprimentoFim = DateTime.Now.ToString("dd/MM/yyyy");
                }

                DateTime? dataInicio = null;
                DateTime? dataFim = null;
                DateTime? cumprimentoInicio = null;
                DateTime? cumprimentoFim = null;

                if (FiltrarDataInicio != "")
                {
                    dataInicio = DateTime.Parse(FiltrarDataInicio);
                }

                if (FiltrarDataFim != "")
                {
                    dataFim = DateTime.Parse(FiltrarDataFim);
                }
                if (FiltrarCumprimentoInicio != "")
                {
                    cumprimentoInicio = DateTime.Parse(FiltrarCumprimentoInicio); ;
                }
                if (FiltrarCumprimentoFim != "")
                {
                    cumprimentoFim = DateTime.Parse(FiltrarCumprimentoFim); ;
                }

                if (FiltrarTipo == 1)
                {
                    var ds = RelatorioAgendasProfissionais.Gerar(dataInicio, dataFim, cumprimentoInicio, cumprimentoFim, ConfigurationManager.ConnectionStrings["APIR9"].ConnectionString);
                    //var ds = RelatorioAgendasProfissionais.Gerar((FiltrarDataInicio != null ? DateTime.Parse(FiltrarDataInicio) : null), (FiltrarDataFim != null ? DateTime.Parse(FiltrarDataFim) : null), (FiltrarCumprimentoInicio != null ? DateTime.Parse(FiltrarCumprimentoInicio) : null), (FiltrarCumprimentoFim != null ? DateTime.Parse(FiltrarCumprimentoFim) : null), ConfigurationManager.ConnectionStrings["APIR9"].ConnectionString);

                    if (FiltrarProfissionais != null && FiltrarProfissionais.Count > 0 && FiltrarProfissionais[0] > 0)
                    {
                        RelatorioAgendasProfissionais.FiltrarPorProfissional(ds, FiltrarProfissionais);
                    }

                    if (FiltrarUsuarios != null && FiltrarUsuarios.Count > 0 && FiltrarUsuarios[0] > 0)
                    {
                        RelatorioAgendasProfissionais.FiltrarPorUsuario(ds, FiltrarUsuarios);
                    }

                    if (FiltrarTiposEventos != null && FiltrarTiposEventos.Count > 0 && FiltrarTiposEventos[0] > 0)
                    {
                        RelatorioAgendasProfissionais.FiltrarPorEvento(ds, FiltrarTiposEventos);
                    }

                    var viewer = new Microsoft.Reporting.WebForms.ReportViewer();
                    viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                    viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\rptRelatorioAgendaProfissionais.rdlc";
                    viewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("dsRelatorioAgendaProfissionais", ds));

                    viewer.SizeToReportContent = true;
                    viewer.Width = Unit.Percentage(130);
                    viewer.Height = Unit.Percentage(130);
                    viewer.LocalReport.DisplayName = "Relatório Agenda de Profisiionais";

                    var textoData = "";
                    var textoCumprimento = "";
                    var textoCabecalho = "";

                    if (FiltrarDataInicio != "")
                    {
                        textoData = $"Data Cadastro de {FiltrarDataInicio} a {FiltrarDataFim}";
                    }

                    if (FiltrarCumprimentoInicio != "")
                    {
                        textoCumprimento = $"Data Cumprimento Prazo de {FiltrarCumprimentoInicio} a {FiltrarCumprimentoFim}";
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

                    if (FiltrarDataInicio == DateTime.Now.ToString("dd/MM/yyyy") &&
                        FiltrarDataFim == DateTime.Now.ToString("dd/MM/yyyy") &&
                        FiltrarCumprimentoInicio == DateTime.Now.ToString("dd/MM/yyyy") &&
                        FiltrarCumprimentoFim == DateTime.Now.ToString("dd/MM/yyyy"))
                    {
                        ViewBag.ReportViewer = viewer;
                    }
                    else
                    {
                        ViewBag.ReportViewer = viewer;
                        ViewBag.FiltrarDataInicio = FiltrarDataInicio;
                        ViewBag.FiltrarDataFim = FiltrarDataFim;
                        ViewBag.FiltrarCumprimentoInicio = FiltrarCumprimentoInicio;
                        ViewBag.FiltrarCumprimentoFim = FiltrarCumprimentoFim;
                        ViewBag.FiltrarProfissionais = FiltrarProfissionais;
                        ViewBag.FiltrarUsuarios = FiltrarUsuarios;
                        ViewBag.FiltrarTiposEventos = FiltrarTiposEventos;
                    }
                }
                else
                {
                    var ds = RelatorioAgendasProfissionais.Gerar(dataInicio, dataFim, cumprimentoInicio, cumprimentoFim, ConfigurationManager.ConnectionStrings["APIR9"].ConnectionString);
                    //var ds = RelatorioAgendasProfissionais.Gerar((FiltrarDataInicio != null ? DateTime.Parse(FiltrarDataInicio) : null), (FiltrarDataFim != null ? DateTime.Parse(FiltrarDataFim) : null), (FiltrarCumprimentoInicio != null ? DateTime.Parse(FiltrarCumprimentoInicio) : null), (FiltrarCumprimentoFim != null ? DateTime.Parse(FiltrarCumprimentoFim) : null), ConfigurationManager.ConnectionStrings["APIR9"].ConnectionString);

                    if (FiltrarProfissionais != null && FiltrarProfissionais.Count > 0 && FiltrarProfissionais[0] > 0)
                    {
                        RelatorioAgendasProfissionais.FiltrarPorProfissional(ds, FiltrarProfissionais);
                    }

                    if (FiltrarUsuarios != null && FiltrarUsuarios.Count > 0 && FiltrarUsuarios[0] > 0)
                    {
                        RelatorioAgendasProfissionais.FiltrarPorUsuario(ds, FiltrarUsuarios);
                    }

                    if (FiltrarTiposEventos != null && FiltrarTiposEventos.Count > 0 && FiltrarTiposEventos[0] > 0)
                    {
                        RelatorioAgendasProfissionais.FiltrarPorEvento(ds, FiltrarTiposEventos);
                    }

                    var viewer = new Microsoft.Reporting.WebForms.ReportViewer();
                    viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                    viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\rptRelatorioAgendaProfissionaisAnalitico.rdlc";
                    viewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("dsRelatorioAgendaProfissionaisAnalitico", ds));

                    viewer.SizeToReportContent = true;
                    viewer.Width = Unit.Percentage(130);
                    viewer.Height = Unit.Percentage(130);
                    viewer.LocalReport.DisplayName = "Relatório Agenda de Profisiionais - Analítico";

                    var textoData = "";
                    var textoCumprimento = "";
                    var textoCabecalho = "";

                    if (FiltrarDataInicio != "")
                    {
                        textoData = $"Data Cadastro de {FiltrarDataInicio} a {FiltrarDataFim}";
                    }

                    if (FiltrarCumprimentoInicio != "")
                    {
                        textoCumprimento = $"Data Cumprimento Prazo de {FiltrarCumprimentoInicio} a {FiltrarCumprimentoFim}";
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

                    if (FiltrarDataInicio == DateTime.Now.ToString("dd/MM/yyyy") &&
                        FiltrarDataFim == DateTime.Now.ToString("dd/MM/yyyy") &&
                        FiltrarCumprimentoInicio == DateTime.Now.ToString("dd/MM/yyyy") &&
                        FiltrarCumprimentoFim == DateTime.Now.ToString("dd/MM/yyyy"))
                    {
                        ViewBag.ReportViewer = viewer;
                    }
                    else
                    {
                        ViewBag.ReportViewer = viewer;
                        ViewBag.FiltrarDataInicio = FiltrarDataInicio;
                        ViewBag.FiltrarDataFim = FiltrarDataFim;
                        ViewBag.FiltrarCumprimentoInicio = FiltrarCumprimentoInicio;
                        ViewBag.FiltrarCumprimentoFim = FiltrarCumprimentoFim;
                        ViewBag.FiltrarProfissionais = FiltrarProfissionais;
                        ViewBag.FiltrarTiposEventos = FiltrarTiposEventos;
                    }
                }
            }

            return View();
        }
    }
}