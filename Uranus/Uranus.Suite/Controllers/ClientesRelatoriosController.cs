using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Uranus.Business;

namespace Uranus.Site.Controllers
{
    public class ClientesRelatoriosController : Controller
    {
        //GET: RelatorioClientes
        public ActionResult Index(Int32 FiltrarRelatorio = 0, Int32 FiltrarEmpresa = 0, String FiltrarCliente = "", String FiltrarDataInicio = null, String FiltrarDataFim = null)
        {
            //if (Sessao.Usuario == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            //else
            //{
                FiltrarEmpresa = 1;
                var nomeEmpresa = "";

                if (FiltrarEmpresa > 0)
                {
                    nomeEmpresa = EmpresaBo.Consultar(FiltrarEmpresa).Nome;
                }

                if (FiltrarDataInicio == null)
                {
                    FiltrarDataInicio = DateTime.Now.ToString("dd/MM/yyyy");
                }
                if (FiltrarDataFim == null)
                {
                    FiltrarDataFim = DateTime.Now.ToString("dd/MM/yyyy");
                }
                
                Int32 FiltrarCodigo = 0;

                if (FiltrarCliente.Length > 0)
                {
                    FiltrarCodigo = int.Parse(FiltrarCliente.Substring(0, FiltrarCliente.IndexOf('-')).Trim());
                }

                if (FiltrarDataInicio != null && FiltrarDataInicio.Length > 0 && FiltrarDataFim.Length > 0)
                {
                    if (FiltrarRelatorio == 0 || FiltrarRelatorio == 1)
                    {
                        var ds = ClientesRelatoriosBO.GerarEmissao(FiltrarDataInicio, FiltrarDataFim);

                        if (FiltrarCodigo > 0)
                        {
                            var Cliente = new List<int> { FiltrarCodigo };

                            ClientesRelatoriosBO.FiltrarPorCliente(ds, Cliente);
                        }

                        var viewer = new Microsoft.Reporting.WebForms.ReportViewer();
                        viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                        viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\rptReceberEmAbertoEmissao.rdlc";
                        viewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("dsReceberEmAbertoEmissao", ds));

                        viewer.SizeToReportContent = true;
                        viewer.Width = Unit.Percentage(130);
                        viewer.Height = Unit.Percentage(130);
                        viewer.LocalReport.DisplayName = "Relatório à Receber por Data de Emissão";

                        var textoData = $"De {FiltrarDataInicio} a {FiltrarDataFim}";

                        ReportParameter[] parametros = new ReportParameter[1];
                        parametros[0] = new ReportParameter("TextoCabecalho", textoData, false);
                        viewer.LocalReport.SetParameters(parametros);

                        ViewBag.ReportViewer = viewer;
                        ViewBag.FiltrarDataInicio = FiltrarDataInicio;
                        ViewBag.FiltrarDataFim = FiltrarDataFim;
                        ViewBag.FiltrarRelatorio = FiltrarRelatorio;
                        ViewBag.FiltrarCliente = FiltrarCliente;
                    }
                    else
                    {
                        if (FiltrarRelatorio == 2)
                        {
                            var ds = ClientesRelatoriosBO.GerarVencimento(FiltrarDataInicio, FiltrarDataFim);

                            if (FiltrarCodigo > 0)
                            {
                                var Cliente = new List<int> { FiltrarCodigo };

                                ClientesRelatoriosBO.FiltrarPorClienteVencimento(ds, Cliente);
                            }

                            var viewer = new Microsoft.Reporting.WebForms.ReportViewer();
                            viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                            viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\rptReceberEmAbertoVencimento.rdlc";
                            viewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("dsReceberEmAbertoVencimento", ds));

                            viewer.SizeToReportContent = true;
                            viewer.Width = Unit.Percentage(130);
                            viewer.Height = Unit.Percentage(130);
                            viewer.LocalReport.DisplayName = "Relatório à Receber por Data de Vencimento";

                            var textoData = $"De {FiltrarDataInicio} a {FiltrarDataFim}";
                             
                            ReportParameter[] parametros = new ReportParameter[1];
                            parametros[0] = new ReportParameter("TextoCabecalho", textoData, false);
                            viewer.LocalReport.SetParameters(parametros);

                            ViewBag.ReportViewer = viewer;
                            ViewBag.FiltrarDataInicio = FiltrarDataInicio;
                            ViewBag.FiltrarDataFim = FiltrarDataFim;
                            ViewBag.FiltrarRelatorio = FiltrarRelatorio;
                            ViewBag.FiltrarCliente = FiltrarCliente;
                        }
                        else
                        {
                            if (FiltrarRelatorio == 3)
                            {
                                var ds = ClientesRelatoriosBO.GerarPagas(FiltrarEmpresa, FiltrarDataInicio, FiltrarDataFim);

                                if (FiltrarCodigo > 0)
                                {
                                    var Cliente = new List<int> { FiltrarCodigo };

                                    ClientesRelatoriosBO.FiltrarPorClientePagas(ds, Cliente);
                                }

                                var viewer = new Microsoft.Reporting.WebForms.ReportViewer();
                                viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                                viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\rptReceberPagas.rdlc";
                                viewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("dsReceberPagas", ds));

                                viewer.SizeToReportContent = true;
                                viewer.Width = Unit.Percentage(130);
                                viewer.Height = Unit.Percentage(130);
                                viewer.LocalReport.DisplayName = "Relatório à Receber - Pagas";

                                var textoData = $"De {FiltrarDataInicio} a {FiltrarDataFim}";

                                ReportParameter[] parametros = new ReportParameter[1];
                                parametros[0] = new ReportParameter("TextoCabecalho", textoData, false);
                                viewer.LocalReport.SetParameters(parametros);

                                ViewBag.ReportViewer = viewer;
                                ViewBag.FiltrarDataInicio = FiltrarDataInicio;
                                ViewBag.FiltrarDataFim = FiltrarDataFim;
                                ViewBag.FiltrarRelatorio = FiltrarRelatorio;
                                ViewBag.FiltrarCliente = FiltrarCliente;
                            }
                        }

                    }
                }
            //}
            return View();
        }
    }
}
