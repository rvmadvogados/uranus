using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Configuration;
using Uranus.Data.R9;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;


namespace Uranus.Suite.Controllers
{
    public class RelatorioCadastroClientesController : Controller
    {
        //GET: RelatorioEventos
        public ActionResult Index(String FiltrarDataInicio = null, String FiltrarDataFim = null, List<int> FiltrarSedes = null, String FiltrarUsuario = null, String FiltrarCliente = null, string FiltrarProfissao = null)
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                DateTime? dataInicio = null;
                DateTime? dataFim = null;
                Int32? usuario = null;
                Int32? cliente = null;
                Int32? profissao = null;

                if (FiltrarDataInicio != null && FiltrarDataInicio.Length > 0)
                {
                    dataInicio = DateTime.Parse(FiltrarDataInicio);
                }
                if (FiltrarDataFim != null && FiltrarDataInicio.Length > 0)
                {
                    dataFim = DateTime.Parse(FiltrarDataFim);
                }
                if (FiltrarUsuario != null && FiltrarUsuario.Length > 0)
                {
                    usuario = int.Parse(FiltrarUsuario);
                }
                if (FiltrarCliente != null && FiltrarCliente.Length > 0)
                {
                    cliente = int.Parse(FiltrarCliente);
                }
                if (FiltrarProfissao != null && FiltrarProfissao.Length > 0)
                {
                    profissao = int.Parse(FiltrarProfissao);
                }


                var ds = RelatorioCadastroClientes.Gerar(dataInicio, dataFim, usuario, cliente, profissao, ConfigurationManager.ConnectionStrings["APIR9"].ConnectionString);

                if (FiltrarSedes != null && FiltrarSedes.Count > 0 && FiltrarSedes[0] > 0)
                {
                    RelatorioCadastroClientes.FiltrarPorSede(ds, FiltrarSedes);
                }

                var viewer = new Microsoft.Reporting.WebForms.ReportViewer();
                viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\rptRelatorioCadastroClientes.rdlc";
                viewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("dsRelatorio", ds));

                viewer.SizeToReportContent = true;
                viewer.Width = Unit.Percentage(130);
                viewer.Height = Unit.Percentage(130);
                viewer.LocalReport.DisplayName = "Relatório Cadastro de Clientes";

                var textoData = "";
                if (FiltrarDataInicio != null && FiltrarDataFim != null)
                {
                    textoData = $"De {FiltrarDataInicio} a {FiltrarDataFim}";
                }
                else
                {
                    textoData = $"Todos";
                }

                ReportParameter[] parametros = new ReportParameter[1];
                parametros[0] = new ReportParameter("TextoCabecalho", textoData, false);
                viewer.LocalReport.SetParameters(parametros);

                ViewBag.ReportViewer = viewer;
                ViewBag.FiltrarDataInicio = FiltrarDataInicio;
                ViewBag.FiltrarDataFim = FiltrarDataFim;
                ViewBag.FiltrarSedes = FiltrarSedes;
                //                }

                return View();
            }
        }
    }
}