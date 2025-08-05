using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Uranus.Business;
using Uranus.Data.R9;

namespace Uranus.Suite.Controllers
{
    public class RelatorioClientesIndicacaoController : Controller
    {
        //GET: RelatorioEventos

        public ActionResult GetClientsList(string search, bool filter = false, int page = 1)
        {
            var clientes = PessoasBo.GetClientsList(search, filter);
            var total = clientes.Count();

            if (!(string.IsNullOrEmpty(search) || string.IsNullOrWhiteSpace(search)))
            {
                clientes = clientes.Where(x => PessoasBo.ConverteNome(x.text).ToLower().StartsWith(PessoasBo.ConverteNome(search).ToLower())).Take(page * 10).ToList();
            }

            return Json(new { clientes = clientes, total = total }, JsonRequestBehavior.AllowGet);
        }
        

        public ActionResult Index(Int32 FiltrarIndicacao = 0, Int32? Profissionais = 0, Int32? Parceiros = 0, Int32? Clientes = 0)
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {

                var viewer = new Microsoft.Reporting.WebForms.ReportViewer();
                if (FiltrarIndicacao != 0 || Profissionais != 0 || Parceiros != 0 || Clientes != 0)
                {
                    //var ds = RelatorioClientesIndicacao.Gerar(ConfigurationManager.ConnectionStrings["APIR9"].ConnectionString);
                    var ds = RelatorioClientesIndicacaoBO.Gerar(FiltrarIndicacao, Profissionais, Parceiros, Clientes);

                    //if (FiltrarIndicacao != null && FiltrarIndicacao.Count > 0 && FiltrarIndicacao[0] > 0)
                    //{
                    //    RelatorioClientesIndicacaoBO.FiltrarPorIndicacao(ds, FiltrarIndicacao);
                    //}

                    //if (FiltrarCliente != null && FiltrarCliente != "")
                    //{
                    //    RelatorioClientesIndicacaoBO.FiltrarPorCliente(ds, FiltrarCliente);
                    //}

                    viewer.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Local;
                    viewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\rptRelatorioClientesIndicacao.rdlc";
                    viewer.LocalReport.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("dsRelatorioClientesIndicacao", ds));

                    viewer.SizeToReportContent = true;
                    viewer.Width = Unit.Percentage(130);
                    viewer.Height = Unit.Percentage(130);
                    viewer.LocalReport.DisplayName = "Relatório de Indicação";

                    //                    ReportParameter[] parametros = new ReportParameter[1];
                    //                    parametros[0] = new ReportParameter("TextoCabecalho", textoData, false);
                    //                    viewer.LocalReport.SetParameters(parametros);

                }
                ViewBag.ReportViewer = viewer;
                ViewBag.FiltrarIndicacao = FiltrarIndicacao;


                return View();
            }
        }
    }
}