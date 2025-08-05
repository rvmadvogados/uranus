using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class FinanceiroDespesasController : Controller
    {
        public ActionResult Index(string FiltrarDataInicio = "", string FiltrarDataFim = "", string FiltrarFornecedor = "", Int32? FiltrarSede = null)
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = FinanceiroDespesasBo.Listar(FiltrarDataInicio, FiltrarDataFim, FiltrarFornecedor, FiltrarSede);
                ViewBag.FiltrarDataInicio = FiltrarDataInicio;
                ViewBag.FiltrarDataFim = FiltrarDataFim;
                ViewBag.FiltrarFornecedor = FiltrarFornecedor;
                ViewBag.FiltrarSede = FiltrarSede;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var financeirodespesas = FinanceiroDespesasBo.ConsultarArray(Id);
            var result = new { codigo = "00", financeirodespesas = financeirodespesas };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int64 Id, Int32 IdOrigem, String NumeroDocumento, String DataDocumento, Int32 IdFinanceiroTipo, Int32 IdCentroCusto, Int32 IdFornecedor, String Valor, 
                                 String DataPagamento, String Observacao, Int64 IdBancosLancamento, Int32 IdBanco)
        {
            FinanceiroDespesas financeirodespesas = new FinanceiroDespesas();
            financeirodespesas.Id = Id;
            financeirodespesas.IdOrigem = IdOrigem;
            financeirodespesas.NumeroDocumento = NumeroDocumento.Trim();

            if (DataDocumento != null && DataDocumento.Trim().Length > 0)
            {
                financeirodespesas.DataDocumento = DateTime.Parse(DataDocumento);
            }

            financeirodespesas.IdFinanceiroTipo = IdFinanceiroTipo;
            financeirodespesas.IdCentroCusto = IdCentroCusto;
            financeirodespesas.IdFornecedor = IdFornecedor;

            if (Valor != null && Valor.Trim().Length > 0)
            {
                financeirodespesas.Valor = Decimal.Parse(Valor);
            }

            if (DataPagamento != null && DataPagamento.Trim().Length > 0)
            {
                financeirodespesas.DataPagamento = DateTime.Parse(DataPagamento);
            }

            financeirodespesas.Observacao = Observacao.Trim();
            financeirodespesas.IdBancosLancamento = IdBancosLancamento;

            financeirodespesas.DataCadastro = DateTime.Now;
            financeirodespesas.NomeUsuarioCadastro = Sessao.Usuario.Nome;
            financeirodespesas.DataAlteracao = DateTime.Now;
            financeirodespesas.NomeUsuarioAlteracao = Sessao.Usuario.Nome;

            if (IdBancosLancamento != 0)
            {
                BancosLancamentosBo.Excluir(IdBancosLancamento);
                IdBancosLancamento = 0;
            }

            var destino = FinanceiroOrigemBo.Consultar(int.Parse(financeirodespesas.IdOrigem.ToString()));

            if (destino.Tipo == "B" && IdBanco == 0)
            {
                IdBanco = int.Parse(destino.IdBanco.ToString());
            }

            if (IdBanco > 0)
            {
                var complemento = "Documento número " + financeirodespesas.NumeroDocumento;
                var idHistorico = 1;
                financeirodespesas.IdBanco = IdBanco;
                BancosLancamentos bancoslancamento = new BancosLancamentos();
                bancoslancamento.ID = IdBancosLancamento;
                bancoslancamento.Data = financeirodespesas.DataDocumento;
                bancoslancamento.IdBanco = IdBanco;
                bancoslancamento.IdHistorico = idHistorico;
                bancoslancamento.Complemento = complemento;
                bancoslancamento.ValorDebito = financeirodespesas.Valor;
                bancoslancamento.ValorCredito = 0;
                bancoslancamento.DataCadastro = DateTime.Now;
                bancoslancamento.NomeUsuarioCadastro = Sessao.Usuario.Nome;
                bancoslancamento.DataAlteracao = DateTime.Now;
                bancoslancamento.NomeUsuarioAlteracao = Sessao.Usuario.Nome;

                if (IdBancosLancamento == 0)
                    IdBancosLancamento = BancosLancamentosBo.Inserir(bancoslancamento);

                bancoslancamento.ID = IdBancosLancamento;
                BancosLancamentosBo.Salvar(bancoslancamento);

                financeirodespesas.IdBancosLancamento = IdBancosLancamento;
            }

            if (Id == 0)
            {
                Id = FinanceiroDespesasBo.Inserir(financeirodespesas);
            }

            financeirodespesas.Id = Id;
            FinanceiroDespesasBo.Salvar(financeirodespesas);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = FinanceiroDespesasBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar()
        {
            var financeirodespesas = FinanceiroDespesasBo.Consultar();
            var result = new { codigo = "00", financeirodespesas = financeirodespesas };
            return Json(result);
        }
    }
}