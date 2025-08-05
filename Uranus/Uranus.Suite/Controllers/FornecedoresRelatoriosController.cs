using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.util;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class FornecedoresRelatoriosController : Controller
    {
        //GET: RelatorioFornecedores
        public ActionResult Index(Int32 FiltrarRelatorio = 0, String FiltrarFornecedor = "", String FiltrarDataInicio = null, String FiltrarDataFim = null)
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

                Int32 FiltrarCodigo = 0;

                if (FiltrarFornecedor.Length > 0)
                {
                    FiltrarCodigo = int.Parse(FiltrarFornecedor.Substring(0, FiltrarFornecedor.IndexOf('-')).Trim());
                }

                if (FiltrarDataInicio.Length > 0 && FiltrarDataFim.Length > 0)
                {
                    if (FiltrarRelatorio == 0 || FiltrarRelatorio == 1)
                    {
                        var ds = FornecedoresRelatoriosBo.GerarEmissao(FiltrarDataInicio, FiltrarDataFim);

                        if (FiltrarCodigo > 0)
                        {
                            var Fornecedor = new List<int> { FiltrarCodigo };

                            FornecedoresRelatoriosBo.FiltrarPorFornecedor(ds, Fornecedor);
                        }

                        var viewer = new Microsoft.Reporting.WebForms.ReportViewer();
                        viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                        viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\rptPagarEmAbertoEmissao.rdlc";
                        viewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("dsPagarEmAbertoEmissao", ds));

                        viewer.SizeToReportContent = true;
                        viewer.Width = Unit.Percentage(130);
                        viewer.Height = Unit.Percentage(130);
                        viewer.LocalReport.DisplayName = "Relatório à Pagar por Data de Emissão";

                        var textoData = $"De {FiltrarDataInicio} a {FiltrarDataFim}";

                        ReportParameter[] parametros = new ReportParameter[1];
                        parametros[0] = new ReportParameter("TextoCabecalho", textoData, false);
                        viewer.LocalReport.SetParameters(parametros);

                        ViewBag.ReportViewer = viewer;
                        ViewBag.FiltrarDataInicio = FiltrarDataInicio;
                        ViewBag.FiltrarDataFim = FiltrarDataFim;
                        ViewBag.FiltrarRelatorio = FiltrarRelatorio;
                        ViewBag.FiltrarFornecedor = FiltrarFornecedor;
                    }
                    else
                    {
                        if (FiltrarRelatorio == 2)
                        {
                            var ds = FornecedoresRelatoriosBo.GerarVencimento(FiltrarDataInicio, FiltrarDataFim);

                            if (FiltrarCodigo > 0)
                            {
                                var Fornecedor = new List<int> { FiltrarCodigo };

                                FornecedoresRelatoriosBo.FiltrarPorFornecedorVencimento(ds, Fornecedor);
                            }

                            var viewer = new Microsoft.Reporting.WebForms.ReportViewer();
                            viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\rptPagarEmAbertoVencimento.rdlc";
                            viewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("dsPagarEmAbertoVencimento", ds));

                            viewer.SizeToReportContent = true;
                            viewer.Width = Unit.Percentage(130);
                            viewer.Height = Unit.Percentage(130);
                            viewer.LocalReport.DisplayName = "Relatório à Pagar por Data de Vencimento";

                            var textoData = $"De {FiltrarDataInicio} a {FiltrarDataFim}";

                            ReportParameter[] parametros = new ReportParameter[1];
                            parametros[0] = new ReportParameter("TextoCabecalho", textoData, false);
                            viewer.LocalReport.SetParameters(parametros);

                            ViewBag.ReportViewer = viewer;
                            ViewBag.FiltrarDataInicio = FiltrarDataInicio;
                            ViewBag.FiltrarDataFim = FiltrarDataFim;
                            ViewBag.FiltrarRelatorio = FiltrarRelatorio;
                            ViewBag.FiltrarFornecedor = FiltrarFornecedor;
                        }
                        else
                        {
                            if (FiltrarRelatorio == 3)
                            {
                                var ds = FornecedoresRelatoriosBo.GerarPagas(FiltrarDataInicio, FiltrarDataFim);

                                if (FiltrarCodigo > 0)
                                {
                                    var Fornecedor = new List<int> { FiltrarCodigo };

                                    FornecedoresRelatoriosBo.FiltrarPorFornecedorPagas(ds, Fornecedor);
                                }

                                var viewer = new Microsoft.Reporting.WebForms.ReportViewer();
                                viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                                viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\rptPagarPagas.rdlc";
                                viewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("dsPagarPagas1", ds));

                                viewer.SizeToReportContent = true;
                                viewer.Width = Unit.Percentage(130);
                                viewer.Height = Unit.Percentage(130);
                                viewer.LocalReport.DisplayName = "Relatório à Pagar - Pagas";

                                var textoData = $"De {FiltrarDataInicio} a {FiltrarDataFim}";

                                ReportParameter[] parametros = new ReportParameter[1];
                                parametros[0] = new ReportParameter("TextoCabecalho", textoData, false);
                                viewer.LocalReport.SetParameters(parametros);

                                ViewBag.ReportViewer = viewer;
                                ViewBag.FiltrarDataInicio = FiltrarDataInicio;
                                ViewBag.FiltrarDataFim = FiltrarDataFim;
                                ViewBag.FiltrarRelatorio = FiltrarRelatorio;
                                ViewBag.FiltrarFornecedor = FiltrarFornecedor;
                            }
                            else
                            {
                                if (FiltrarRelatorio == 4)
                                {
                                    var ds = FornecedoresRelatoriosBo.GerarRolPagamentos(FiltrarDataInicio, FiltrarDataFim);

                                    if (FiltrarCodigo > 0)
                                    {
                                        var Fornecedor = new List<int> { FiltrarCodigo };

                                        FornecedoresRelatoriosBo.FiltrarPorFornecedorRolPagamentos(ds, Fornecedor);
                                    }

                                    var viewer = new Microsoft.Reporting.WebForms.ReportViewer();
                                    viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                                    viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\rptPagarRolPagamentos.rdlc";
                                    viewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("dsPagarRolPagamentos", ds));

                                    viewer.SizeToReportContent = true;
                                    viewer.Width = Unit.Percentage(130);
                                    viewer.Height = Unit.Percentage(130);
                                    viewer.LocalReport.DisplayName = "Relatório à Pagar - Rol de Pagamentos";

                                    var textoData = $"De {FiltrarDataInicio} a {FiltrarDataFim}";

                                    ReportParameter[] parametros = new ReportParameter[1];
                                    parametros[0] = new ReportParameter("TextoCabecalho", textoData, false);
                                    viewer.LocalReport.SetParameters(parametros);

                                    ViewBag.ReportViewer = viewer;
                                    ViewBag.FiltrarDataInicio = FiltrarDataInicio;
                                    ViewBag.FiltrarDataFim = FiltrarDataFim;
                                    ViewBag.FiltrarRelatorio = FiltrarRelatorio;
                                    ViewBag.FiltrarFornecedor = FiltrarFornecedor;
                                }
                            }
                        }
                    }
                }
            }
            return View();
        }
    }
}