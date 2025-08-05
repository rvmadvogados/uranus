using System;
using System.Globalization;
using System.Linq;
using System.util;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class FornecedoresPagamentosPeriodoController : Controller
    {
        // GET: FornecedoresNotas
        public ActionResult Index(Int64 FiltrarCodigo = 0, String FiltrarFornecedor = "", String FiltrarDataInicio = "", String FiltrarDataFim = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {

                var model = FornecedoresNotasParcelasBo.ListarPeriodo(FiltrarCodigo, FiltrarFornecedor, FiltrarDataInicio, FiltrarDataFim);
                ViewBag.FiltrarCodigo = FiltrarCodigo;
                ViewBag.FiltrarFornecedor = FiltrarFornecedor;
                ViewBag.FiltrarDataInicio = FiltrarDataInicio;
                ViewBag.FiltrarDataFim = FiltrarDataFim;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var parcela = FornecedoresNotasParcelasBo.ConsultarArray(Id);
            var result = new { codigo = "00", parcela = parcela };
            return Json(result);
        }


        [HttpPost]
        public JsonResult SalvarParcela(Int64 Id, Int64 IdFornecedorNota, Int32 Parcela, String Vencimento, String ValorParcela,
            String DataPagamento, String ValorPago, String Juros, String Desconto, String Saldo,
            String Observacao)

        {
            FornecedoresNotasParcelas parcela = new FornecedoresNotasParcelas();
            parcela.Id = Id;
            parcela.IdFornecedoresNota = IdFornecedorNota;
            parcela.Parcela = Parcela;
            parcela.Vencimento = DateTime.Parse(Vencimento);
            parcela.ValorParcela = Decimal.Parse(ValorParcela);

            if (DataPagamento != null && DataPagamento.Length > 0)
            {
                parcela.DataPagamento = DateTime.Parse(DataPagamento);
            }

            if (ValorPago != null && ValorPago.Length > 0)
            {
                parcela.ValorPago = Decimal.Parse(ValorPago);
            }

            if (Juros != null && Juros.Length > 0)
            {
                parcela.Juros = Decimal.Parse(Juros);
            }

            if (Desconto != null && Desconto.Length > 0)
            {
                parcela.Descontos = Decimal.Parse(Desconto);
            }

            if (Saldo != null && Saldo.Length > 0)
            {
                parcela.Saldo = Decimal.Parse(Saldo);
            }

            parcela.Observacao = Observacao;

            parcela.DataCadastro = DateTime.Now;
            parcela.NomeUsuarioCadastro = Sessao.Usuario.Nome;
            parcela.DataAlteracao = DateTime.Now;
            parcela.NomeUsuarioAlteracao = Sessao.Usuario.Nome;

            if (Id == 0)
                Id = FornecedoresNotasParcelasBo.Inserir(parcela);

            parcela.Id = Id;
            FornecedoresNotasParcelasBo.Salvar(parcela);

            var result = new { codigo = "00", IdFornecedorNota = IdFornecedorNota };
            return Json(result);
        }
    }
}