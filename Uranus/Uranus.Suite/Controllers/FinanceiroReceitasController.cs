using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class FinanceiroReceitasController : Controller
    {
        public ActionResult Index(string FiltrarDataInicio = "", string FiltrarDataFim = "", string FiltrarCliente = "", Int32? FiltrarSede = null)
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = FinanceiroReceitasBo.Listar(FiltrarDataInicio, FiltrarDataFim, FiltrarCliente, FiltrarSede);
                ViewBag.FiltrarDataInicio = FiltrarDataInicio;
                ViewBag.FiltrarDataFim = FiltrarDataFim;
                ViewBag.FiltrarCliente = FiltrarCliente;
                ViewBag.FiltrarSede = FiltrarSede;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var financeiroreceitas = FinanceiroReceitasBo.ConsultarArray(Id);
            var result = new { codigo = "00", financeiroreceitas = financeiroreceitas };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int64 Id, Int32 IdOrigem, String NumeroDocumento, String DataDocumento, Int32 IdFinanceiroTipo, Int32 IdCentroCusto, Int32 IdArea, Int32 IdCliente,
                                 String Valor, String DataPagamento, String Observacao, Int64? IdBancosLancamento, String ValorBruto, String IRRetido, Int32? IdBanco, Int32 IdProcessoAcao)

        {
            FinanceiroReceitas financeiroreceitas = new FinanceiroReceitas();
            financeiroreceitas.Id = Id;
            financeiroreceitas.IdOrigem = IdOrigem;
            financeiroreceitas.NumeroDocumento = NumeroDocumento.Trim();

            if (DataDocumento != null && DataDocumento.Trim().Length > 0)
            {
                financeiroreceitas.DataDocumento = DateTime.Parse(DataDocumento);
            }

            financeiroreceitas.IdFinanceiroTipo = IdFinanceiroTipo;
            financeiroreceitas.IdCentroCusto = IdCentroCusto;
            financeiroreceitas.IdArea = IdArea;
            financeiroreceitas.IdCliente = IdCliente;
            financeiroreceitas.IdProcessoAcao = IdProcessoAcao;

            financeiroreceitas.Valor = 0;
            if (Valor != null && Valor.Trim().Length > 0)
            {
                financeiroreceitas.Valor = Decimal.Parse(Valor);
            }

            financeiroreceitas.ValorBruto = 0;
            if (ValorBruto != null && ValorBruto.Trim().Length > 0)
            {
                financeiroreceitas.ValorBruto = Decimal.Parse(ValorBruto);
            }

            financeiroreceitas.IRRetido = 0;
            if (IRRetido != null && IRRetido.Trim().Length > 0)
            {
                financeiroreceitas.IRRetido = Decimal.Parse(IRRetido);
            }

            if (DataPagamento != null && DataPagamento.Trim().Length > 0)
            {
                financeiroreceitas.DataPagamento = DateTime.Parse(DataPagamento);
            }

            financeiroreceitas.Observacao = Observacao.Trim();

            if (IdBancosLancamento != null && IdBancosLancamento != 0)
            {
                BancosLancamentosBo.Excluir(long.Parse(IdBancosLancamento.ToString()));
                IdBancosLancamento = 0;
            }

            var destino = FinanceiroOrigemBo.Consultar(int.Parse(financeiroreceitas.IdOrigem.ToString()));
            var nomecliente = ClientesBo.Consultar(int.Parse(financeiroreceitas.IdCliente.ToString())).Pessoas.Nome;

            if (destino.Tipo == "B" && IdBanco == 0)
            {
                IdBanco = int.Parse(destino.IdBanco.ToString());
                if (IdBanco > 0)
                {
                    var complemento = "De: " + nomecliente + " - Documento número " + financeiroreceitas.NumeroDocumento;
                    var idHistorico = 1;
                    financeiroreceitas.IdBanco = IdBanco;
                    BancosLancamentos bancoslancamento = new BancosLancamentos();
                    bancoslancamento.ID = long.Parse(IdBancosLancamento.ToString());
                    bancoslancamento.Data = financeiroreceitas.DataDocumento;
                    bancoslancamento.IdBanco = int.Parse(IdBanco.ToString());
                    bancoslancamento.IdHistorico = idHistorico;
                    bancoslancamento.Complemento = complemento;
                    bancoslancamento.ValorDebito = 0;
                    bancoslancamento.ValorCredito = financeiroreceitas.Valor;
                    bancoslancamento.DataCadastro = DateTime.Now;
                    bancoslancamento.NomeUsuarioCadastro = Sessao.Usuario.Nome;
                    bancoslancamento.DataAlteracao = DateTime.Now;
                    bancoslancamento.NomeUsuarioAlteracao = Sessao.Usuario.Nome;

                    if (IdBancosLancamento == 0)
                        IdBancosLancamento = BancosLancamentosBo.Inserir(bancoslancamento);

                    bancoslancamento.ID = long.Parse(IdBancosLancamento.ToString());
                    BancosLancamentosBo.Salvar(bancoslancamento);

                    financeiroreceitas.IdBancosLancamento = long.Parse(IdBancosLancamento.ToString());
                }
            }
            else
            {
                Caixas caixas = new Caixas();
                caixas.ID = 0;
                caixas.Data = financeiroreceitas.DataDocumento;
                caixas.IdHistorico = 1;
                var complemento = "De: " + nomecliente + " - Documento número " + financeiroreceitas.NumeroDocumento;
                caixas.Descricao = complemento;
                caixas.ValorDebito = 0;
                caixas.ValorCredito = financeiroreceitas.Valor;
                caixas.IdOrigem = int.Parse(destino.IdCaixa.ToString());
                caixas.DataCadastro = DateTime.Now;
                caixas.NomeUsuarioCadastro = Sessao.Usuario.Nome;
                caixas.DataAlteracao = DateTime.Now;
                caixas.NomeUsuarioAlteracao = Sessao.Usuario.Nome;
                var IdCaixa = CaixasBo.Inserir(caixas);
                financeiroreceitas.IdCaixa = long.Parse(IdCaixa.ToString());
            }

            financeiroreceitas.DataCadastro = DateTime.Now;
            financeiroreceitas.NomeUsuarioCadastro = Sessao.Usuario.Nome;
            financeiroreceitas.DataAlteracao = DateTime.Now;
            financeiroreceitas.NomeUsuarioAlteracao = Sessao.Usuario.Nome;

            if (Id == 0)
            {
                financeiroreceitas.Nota = false;
                Id = FinanceiroReceitasBo.Inserir(financeiroreceitas);
            }

            financeiroreceitas.Id = Id;
            FinanceiroReceitasBo.Salvar(financeiroreceitas);



            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = FinanceiroReceitasBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar()
        {
            var financeiroreceitas = FinanceiroReceitasBo.Consultar();
            var result = new { codigo = "00", financeiroreceitas = financeiroreceitas };
            return Json(result);
        }

        public JsonResult ImportarReceita(Int64 Id)
        {
            var idNotaFiscal = FinanceiroReceitasBo.Importar(Id, Sessao.Usuario.Nome);
            var result = new { codigo = "00", idNotaFiscal = idNotaFiscal };
            return Json(result);
        }

    }
}