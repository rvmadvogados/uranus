using System;
using System.Globalization;
using System.Linq;
using System.util;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class ClientesPagamentosPeriodoController : Controller
    {
        // GET: ClienteesNotas
        public ActionResult Index(Int64 FiltrarCodigo = 0, String FiltrarCliente = "", String FiltrarDataInicio = "", String FiltrarDataFim = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {

                var model = ClientesNotasParcelasBO.ListarPeriodo(FiltrarCodigo, FiltrarCliente, FiltrarDataInicio, FiltrarDataFim);
                ViewBag.FiltrarCodigo = FiltrarCodigo;
                ViewBag.FiltrarCliente = FiltrarCliente;
                ViewBag.FiltrarDataInicio = FiltrarDataInicio;
                ViewBag.FiltrarDataFim = FiltrarDataFim;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var parcela = ClientesNotasParcelasBO.ConsultarArray(Id);
            var result = new { codigo = "00", parcela = parcela };
            return Json(result);
        }


        [HttpPost]
        public JsonResult SalvarParcela(Int64 Id, Int64 IdClienteNota, Int32 Parcela, String Vencimento, String ValorParcela,
                                        String DataPagamento, String ValorPago, String Juros, String Desconto, String Saldo, String Observacao, Int32? IdDestino, Int32? IdFormaPagamento)

        {
            ClientesNotasPagamentos pagamento = new ClientesNotasPagamentos();
            pagamento.Id = 0;
            pagamento.IdClienteNota = IdClienteNota;
            pagamento.IdDestino = IdDestino;
            pagamento.IdFormaPagamento = IdFormaPagamento;
            if (DataPagamento != null && DataPagamento.Length > 0)
            {
                pagamento.DataPagamento = DateTime.Parse(DataPagamento);
            }

            if (ValorPago != null && ValorPago.Length > 0)
            {
                pagamento.ValorPago = Decimal.Parse(ValorPago);
            }

            if (Juros != null && Juros.Length > 0)
            {
                pagamento.Juros = Decimal.Parse(Juros);
            }

            if (Desconto != null && Desconto.Length > 0)
            {
                pagamento.Desconto = Decimal.Parse(Desconto);
            }

            pagamento.Observacao = Observacao;

            pagamento.DataCadastro = DateTime.Now;
            pagamento.NomeUsuarioCadastro = Sessao.Usuario.Nome;
            pagamento.DataAlteracao = DateTime.Now;
            pagamento.NomeUsuarioAlteracao = Sessao.Usuario.Nome;

            var IdPagamento = ClientesNotasPagamentosBO.Inserir(pagamento);

            var idBanco = 0;
            var proximaParcela = 0;
            decimal saldoParcela = 0;
            var notas = ClientesNotasBO.Consultar(IdClienteNota);
            var parcela = ClientesNotasParcelasBO.ConsultarParcelaBaixa(Id);
            parcela.Id = Id;
            parcela.IdClientesNota = IdClienteNota;
            parcela.Parcela = Parcela;
            parcela.Vencimento = DateTime.Parse(Vencimento);
            parcela.ValorParcela = Decimal.Parse(ValorParcela);
            parcela.IdDestino = IdDestino;
            parcela.IdFormapagamento = IdFormaPagamento;
            if (DataPagamento != null && DataPagamento.Length > 0)
            {
                parcela.DataPagamento = DateTime.Parse(DataPagamento);
            }

            if (Saldo != null && Saldo.Length > 0)
            {
                if (Decimal.Parse(Saldo) < 0)
                {
                    saldoParcela = Decimal.Parse(Saldo) * -1;
                    parcela.Saldo = 0;
                }
                else
                {
                    parcela.Saldo = Decimal.Parse(Saldo);
                }
            }


            if (ValorPago != null && ValorPago.Length > 0)
            {
                if (saldoParcela > 0)
                {
                    parcela.ValorPago = Decimal.Parse(ValorParcela);
                }
                else
                {
                    parcela.ValorPago = Decimal.Parse(ValorPago);
                }
            }

            if (Juros != null && Juros.Length > 0)
            {
                parcela.Juros = Decimal.Parse(Juros);
            }

            if (Desconto != null && Desconto.Length > 0)
            {
                parcela.Descontos = Decimal.Parse(Desconto);
            }

            parcela.Observacao = Observacao;

            parcela.DataAlteracao = DateTime.Now;
            parcela.NomeUsuarioAlteracao = Sessao.Usuario.Nome;

            //if (Id == 0)
            //    Id = ClientesNotasParcelasBO.Inserir(parcela);

            //parcela.Id = Id;
            ClientesNotasParcelasBO.Salvar(parcela);

            if (saldoParcela > 0)
            {
                proximaParcela = Parcela + 1;
                parcela = ClientesNotasParcelasBO.ConsultarProximaParcelaBaixa(IdClienteNota, proximaParcela);
                parcela.IdDestino = IdDestino;
                parcela.IdFormapagamento = IdFormaPagamento;
                if (DataPagamento != null && DataPagamento.Length > 0)
                {
                    parcela.DataPagamento = DateTime.Parse(DataPagamento);
                }

                parcela.Saldo = parcela.Saldo - saldoParcela;
                
                parcela.ValorPago = saldoParcela;

                parcela.Observacao = Observacao;

                parcela.DataAlteracao = DateTime.Now;
                parcela.NomeUsuarioAlteracao = Sessao.Usuario.Nome;

                ClientesNotasParcelasBO.Salvar(parcela);

            }
            FinanceiroReceitas financeiroreceitas = new FinanceiroReceitas();
            financeiroreceitas.Id = 0;
            financeiroreceitas.IdOrigem = IdDestino;
            financeiroreceitas.NumeroDocumento = notas.NumeroDocumento.ToString();

            financeiroreceitas.DataDocumento = DateTime.Parse(parcela.DataPagamento.ToString());

            financeiroreceitas.IdFinanceiroTipo = notas.IdFinanceiroTipo;
            financeiroreceitas.IdCentroCusto = notas.IdSede;
            financeiroreceitas.IdArea = notas.IdArea;
            financeiroreceitas.IdCliente = notas.IdCliente;
            financeiroreceitas.IdProcessoAcao = notas.IdProcessoAcao;
            financeiroreceitas.Valor = parcela.ValorPago;

            financeiroreceitas.ValorBruto = parcela.ValorPago;

            financeiroreceitas.IRRetido = 0;

            financeiroreceitas.DataPagamento = DateTime.Parse(parcela.DataPagamento.ToString());

            financeiroreceitas.Observacao = Observacao.Trim();

            var destino = FinanceiroOrigemBo.Consultar(int.Parse(financeiroreceitas.IdOrigem.ToString()));
            var nomecliente = ClientesBo.Consultar(int.Parse(notas.IdCliente.ToString())).Pessoas.Nome;
            if (destino.Tipo == "B" )
            {
                idBanco = int.Parse(destino.IdBanco.ToString());

                var complemento = "De: " + nomecliente + " - Documento número " + financeiroreceitas.NumeroDocumento;
                var idHistorico = 1;
                financeiroreceitas.IdBanco = idBanco;
                BancosLancamentos bancoslancamento = new BancosLancamentos();
                bancoslancamento.ID = 0;
                bancoslancamento.Data = financeiroreceitas.DataDocumento;
                bancoslancamento.IdBanco = int.Parse(idBanco.ToString());
                bancoslancamento.IdHistorico = idHistorico;
                bancoslancamento.Complemento = complemento;
                bancoslancamento.ValorDebito = 0;
                bancoslancamento.ValorCredito = financeiroreceitas.Valor;
                bancoslancamento.DataCadastro = DateTime.Now;
                bancoslancamento.NomeUsuarioCadastro = Sessao.Usuario.Nome;
                bancoslancamento.DataAlteracao = DateTime.Now;
                bancoslancamento.NomeUsuarioAlteracao = Sessao.Usuario.Nome;

                var IdBancosLancamento = BancosLancamentosBo.Inserir(bancoslancamento);

                financeiroreceitas.IdBancosLancamento = long.Parse(IdBancosLancamento.ToString());
            }
            else
            {
                Caixas caixas = new Caixas();
                caixas.ID = 0;
                caixas.Data = parcela.DataPagamento;
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

            var result = new { codigo = "00", IdClienteNota = IdClienteNota };
            return Json(result);

        }
    }
}