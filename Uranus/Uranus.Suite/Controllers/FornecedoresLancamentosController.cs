using System;
using System.Globalization;
using System.Linq;
using System.util;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class FornecedoresLancamentosController : Controller
    {
        // GET: FornecedoresLancamentos
        public ActionResult Index(string FiltrarCPFCNPJ = "", string FiltrarFornecedor = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = FornecedoresLancamentosBo.Listar(FiltrarCPFCNPJ.Replace(".", string.Empty).Replace("/", string.Empty).Replace("-", string.Empty), FiltrarFornecedor);
                ViewBag.FiltrarCPFCNPJ = FiltrarCPFCNPJ;
                ViewBag.FiltrarFornecedor = FiltrarFornecedor;
                return View(model);
            }
        }

        #region Notas
        [HttpPost]
        public JsonResult Consultar(Int64 Id)
        {
            var lancamento = FornecedoresLancamentosBo.ConsultarArray(Id);
            var result = new { codigo = "00", lancamento = lancamento };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int64 Id, Int32 IdFornecedor, String NumeroDocumento, String DataEmissao, String Total, Int32 Plano, String Observacao, 
                                 String PrimeiroVencimento, Int32 IdContrato, Int32 IdAcao)
        {
            FornecedoresLancamentos lancamento = new FornecedoresLancamentos();
            lancamento.Id = Id;
            lancamento.IdFornecedor = IdFornecedor;
            lancamento.NumeroDocumento = NumeroDocumento;
            lancamento.Plano = Plano;
            lancamento.Observacao = Observacao;

            if (DataEmissao != null && DataEmissao.Trim().Length > 0)
            {
                lancamento.DataEmissao = DateTime.Parse(DataEmissao);
            }

            if (PrimeiroVencimento != null && PrimeiroVencimento.Trim().Length > 0)
            {
                lancamento.PrimeiroVencimento = DateTime.Parse(PrimeiroVencimento);
            }

            if (Total != null && Total.Trim().Length > 0)
            {
                lancamento.Valor = Decimal.Parse(Total);
            }

            if (Id == 0)
            {
                Id = FornecedoresLancamentosBo.Inserir(lancamento);
                FornecedoresLancamentosBo.GerarNotaParcelas(Id);
            }

            lancamento.Id = Id;
            FornecedoresLancamentosBo.Salvar(lancamento);
            var result = new { codigo = "00", id = Id };
            return Json(result);
        }

        [HttpPost]
        public JsonResult CarregarNota(Int64 Id)
        {
            try
            {
                var lancamento = FornecedoresLancamentosBo.Consultar(Id);
                String visualizar = String.Empty;

                if (lancamento != null)
                {
                    visualizar += "<div class='row'>";
                    visualizar += "   <div class='col-md-12'>";
                    visualizar += "      <br />";
                    visualizar += "      <h2>Dados da Nota</h2>";
                    visualizar += "      <hr />";
                    visualizar += "   </div>";
                    visualizar += "</div>";
                    visualizar += "<div class='row'>";
                    visualizar += "   <div class='col-md-3'>";
                    visualizar += "      <div class='form-group'>";
                    visualizar += "         <label class='titlefield'><b>Data</b></label><br />";
                    visualizar += "         <label class='titlefield'>" + lancamento.DataEmissao.Value.ToString("dd/MM/yyyy") + "</label>";
                    visualizar += "      </div>";
                    visualizar += "   </div>";
                    visualizar += "   <div class='col-md-3'>";
                    visualizar += "      <div class='form-group'>";
                    visualizar += "         <label class='titlefield' style='width: 100%; text-align: right !important;'><b>Orçamento Total R$</b></label><br />";
                    visualizar += "         <label style='width: 100%; text-align: right !important; font-size: 18px !important; font-weight: bold !important; color: #5B77A7;'>" + String.Format("{0:##,##0.00}", lancamento.Valor) + "</label>";
                    visualizar += "      </div>";
                    visualizar += "   </div>";
                    visualizar += "   <div class='col-md-1'>";
                    visualizar += "      <div class='form-group'>";
                    visualizar += "         <label class='titlefield'><b>Plano</b></label><br />";
                    visualizar += "         <label class='titlefield'>" + lancamento.Plano + "</label>";
                    visualizar += "      </div>";
                    visualizar += "   </div>";
                    visualizar += "</div>";
                    visualizar += "<div class='row'>";
                    visualizar += "   <div class='col-md-6'>";
                    visualizar += "      <div class='form-group'>";
                    visualizar += "         <label class='titlefield'><b>Nome Corretor</b></label><br />";
                    visualizar += "         <label class='titlefield'>" + lancamento.Fornecedores.Nome + "</label>";
                    visualizar += "      </div>";
                    visualizar += "   </div>";
                    visualizar += "</div>";
                    visualizar += "<div class='row'>";
                    visualizar += "   <div class='col-md-12'>";
                    visualizar += "      <div class='form-group'>";
                    visualizar += "         <label class='titlefield'><b>Observação</b></label><br />";
                    visualizar += "         <label class='titlefield'>" + lancamento.Observacao + "</label>";
                    visualizar += "      </div>";
                    visualizar += "   </div>";
                    visualizar += "</div>";

                    visualizar += "<div class='row'>";
                    visualizar += "   <div class='col-md-12'>";
                    visualizar += "      <br />";
                    visualizar += "      <h2>Parcelas da Nota</h2>";
                    visualizar += "      <hr />";
                    visualizar += "   </div>";
                    visualizar += "</div>";

                    int i = 0;

                    foreach (var item in lancamento.FornecedoresLancamentosParcelas)
                    {
                        visualizar += "<div class='row'>";
                        visualizar += "   <div class='col-md-2'>";
                        visualizar += "      <div class='form-group'>";

                        if (i == 0)
                        {
                            visualizar += "         <label class='titlefield'><b>Parcela</b></label><br />";
                        }

                        visualizar += "         <label class='titlefield'>" + item.Parcela + "</label>";
                        visualizar += "      </div>";
                        visualizar += "   </div>";
                        visualizar += "   <div class='col-md-2'>";
                        visualizar += "      <div class='form-group'>";

                        if (i == 0)
                        {
                            visualizar += "         <label class='titlefield'><b>Vencimento</b></label><br />";
                        }

                        visualizar += "         <label class='titlefield'>" + item.Vencimento.Value.ToString("dd/MM/yyyy") + "</label>";
                        visualizar += "      </div>";
                        visualizar += "   </div>";
                        visualizar += "   <div class='col-md-2'>";
                        visualizar += "      <div class='form-group'>";

                        if (i == 0)
                        {
                            visualizar += "         <label class='titlefield'><b>Valor R$</b></label><br />";
                        }

                        visualizar += "         <label class='titlefield'>" + String.Format("{0:##,##0.00}", item.ValorParcela) + "</label>";
                        visualizar += "      </div>";
                        visualizar += "   </div>";

                        i++;
                    }

                }

                var result = new { response = "success", visualizar = visualizar };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult CarregarParcelas(Int64 IdNota)
        {
            try
            {
                var parcela = FornecedoresLancamentosParcelasBo.Listar(IdNota);
                String parcelas = String.Empty;

                int ParcelasQuantidade = (parcela.Count > 0 ? (parcela.Count + 1) : 1);

                for (int i = 0; i < ParcelasQuantidade; i++)
                {
                    parcelas += "<input id='" + String.Format("HiddenFornecedoresLancamentosDuplicata{0}", (i + 1)) + "' name='HiddenFornecedoresLancamentosDuplicata[]' type='hidden' value='" + (parcela.Count >= (i + 1) ? parcela[i].Id.ToString() : "0") + "' />";
                    parcelas += "<div class='accordion' id='accordionKits' style='border-bottom: 1px solid rgba(0,0,0,.125) !important;'>";
                    parcelas += "   <div class='card z-depth-0 bordered'>";
                    parcelas += "      <div class='card-header' id='" + String.Format("FornecedoresLancamentosDuplicata{0}heading", (i + 1)) + "'>";
                    parcelas += "         <h5 class='mb-0'>";
                    parcelas += "            <button class='btn btn-link' type='button' data-toggle='collapse' data-target='#" + String.Format("FornecedoresLancamentosDuplicata{0}collapse", (i + 1)) + "' aria-expanded='true' aria-controls='" + String.Format("FornecedoresLancamentosDuplicata{0}collapse", (i + 1)) + "'>";
                    parcelas += "            " + String.Format("Parcela #{0}", (i + 1));
                    parcelas += "            </button>";
                    parcelas += "         </h5>";
                    parcelas += "      </div>";
                    parcelas += "      <div id='" + String.Format("FornecedoresLancamentosDuplicata{0}collapse", (i + 1)) + "' class='collapse' aria-labelledby='" + String.Format("FornecedoresLancamentosDuplicata{0}heading", (i + 1)) + "' data-parent='#accordionKits'>";
                    parcelas += "         <div class='card-body'>";
                    parcelas += "            <div class='row'>";
                    parcelas += "               <div class='col-md-2'>";
                    parcelas += "                  <div class='form-group'>";
                    parcelas += "                     <label class='titlefield'>Parcela</label>";
                    parcelas += "                     <input type='text' id='" + String.Format("FornecedoresLancamentosDuplicata{0}Parcela", (i + 1)) + "' name='FornecedoresLancamentosDuplicataParcela[]' class='form-control' value='" + (i + 1) + "' readonly />";
                    parcelas += "                  </div>";
                    parcelas += "               </div>";
                    parcelas += "               <div class='col-md-2'>";
                    parcelas += "                  <div class='form-group'>";
                    parcelas += "                     <label class='titlefield'>Data de Vencimento <span class='required'>*</span></label>";
                    parcelas += "                     <input id='" + String.Format("FornecedoresLancamentosDuplicata{0}DataVencimento", (i + 1)) + "' name='FornecedoresLancamentosDuplicataDataVencimento[]' class='form-control' placeholder='Selecione uma Data' type='text' style='height: 40px !important; cursor: pointer;' value='" + (parcela.Count >= (i + 1) ? parcela[i].Vencimento.Value.ToString("dd/MM/yyyy") : string.Empty) + "' readonly />";
                    parcelas += "                     <label id='" + String.Format("FornecedoresLancamentosDuplicata{0}DataVencimentoValidate", (i + 1)) + "' name='FornecedoresLancamentosDuplicataDataVencimentoValidate[]' for='" + String.Format("FornecedoresLancamentosDuplicata{0}DataVencimento", (i + 1)) + "' class='error hidden'>Por favor, selecionar uma Data.</label>";
                    parcelas += "                  </div>";
                    parcelas += "               </div>";
                    parcelas += "               <div class='col-md-2'>";
                    parcelas += "                  <div class='form-group'>";
                    parcelas += "                     <label class='titlefield'>Valor da Parcela R$ <span class='required'>*</span></label>";
                    parcelas += "                     <input id='" + String.Format("FornecedoresLancamentosDuplicata{0}ValorParcela", (i + 1)) + "' name='FornecedoresLancamentosDuplicataValorParcela[]' class='form-control' placeholder='Informe um Valor' type='text' style='height: 40px !important; cursor: pointer;' value='" + (parcela.Count >= (i + 1) ? parcela[i].ValorParcela.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : string.Empty) + "' onkeyup='CalcularParcela(" + (i + 1) + ")' />";
                    parcelas += "                     <label id='" + String.Format("FornecedoresLancamentosDuplicata{0}ValorParcelaValidate", (i + 1)) + "' name='FornecedoresLancamentosDuplicataValorParcelaValidate[]' for='" + String.Format("FornecedoresLancamentosDuplicata{0}ValorParcela", (i + 1)) + "' class='error hidden'>Por favor, informar um Valor.</label>";
                    parcelas += "                  </div>";
                    parcelas += "               </div>";
                    parcelas += "            </div>";
                    parcelas += "            <div class='row'>";
                    parcelas += "               <div class='col-md-12'>";
                    parcelas += "                  <div class='form-group'>";
                    parcelas += "                     <label class='titlefield'>Observação</label>";
                    parcelas += "                     <textarea id='" + String.Format("FornecedoresLancamentosDuplicata{0}Observacao", (i + 1)) + "' name='FornecedoresLancamentosDuplicataObservacao[]' rows=3 class='form-control' placeholder='Informe aqui a Observação'>" + (parcela.Count >= (i + 1) ? parcela[i].Observacao : string.Empty) + "</textarea>";
                    parcelas += "                  </div>";
                    parcelas += "               </div>";
                    parcelas += "            </div>";
                    parcelas += "            <hr />";
                    parcelas += "            <div class='row'>";
                    parcelas += "               <div class='col-md-4'>";
                    parcelas += "                  <button type='button' id='" + String.Format("ButtonFornecedoresLancamentosDuplicata{0}Incluir", (i + 1)) + "' name='" + String.Format("ButtonFornecedoresLancamentosDuplicata{0}Incluir", (i + 1)) + "' class='btn btn-primary' data-loading-text='Salvando...' onclick='ButtonFornecedoresLancamentosDuplicataSalvar_Click(" + (i + 1) + ");'>";
                    parcelas += "                     Salvar";
                    parcelas += "                  </button>";
                    parcelas += "               </div>";
                    parcelas += "               <div class='col-md-4' style='text-align: center;'>";
                    parcelas += "               </div>";
                    parcelas += "               <div class='col-md-4' style='text-align: right;'>";

                    parcelas += "                  <button type='button' id='" + String.Format("ButtonFornecedoresLancamentosDuplicata{0}Excluir", (i + 1)) + "' name='" + String.Format("ButtonFornecedoresLancamentosDuplicata{0}Excluir", (i + 1)) + "' class='btn btn-danger' data-loading-text='Apagando...' onclick='ButtonFornecedoresLancamentosDuplicataExcluir_Click(" + (i + 1) + ");' " + (parcela.Count >= (i + 1) ? String.Empty : "disabled='disabled'") + ">";
                    parcelas += "                     Apagar";
                    parcelas += "                  </button>";

                    parcelas += "               </div>";
                    parcelas += "            </div>";
                    parcelas += "         </div>";
                    parcelas += "      </div>";
                    parcelas += "   </div>";
                    parcelas += "</div>";

                    parcelas += "<script>";
                    parcelas += "   $('#FornecedoresLancamentosDuplicata" + (i + 1) + "DataVencimento').datepicker({";
                    parcelas += "      language: 'pt-BR',";
                    parcelas += "      autoclose: true,";
                    parcelas += "      todayHighlight: true,";
                    parcelas += "      orientation: 'bottom',";
                    parcelas += "      clearBtn: true";
                    parcelas += "   });";
                    parcelas += "   $('#FornecedoresLancamentosDuplicata" + (i + 1) + "ValorParcela').maskMoney({ thousands: '.', decimal: ',', symbolStay: true });";
                    parcelas += "</script> ";
                }

                var result = new { response = "success", parcelas = parcelas, count = ParcelasQuantidade };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult SalvarParcela(Int64 Id, Int64 IdFornecedoresLancamentos, Int32 Parcela, String Vencimento, String ValorParcela, String Observacao)
        {
            FornecedoresLancamentosParcelas parcela = new FornecedoresLancamentosParcelas();
            parcela.Id = Id;
            parcela.IdFornecedoresLancamentos = IdFornecedoresLancamentos;
            parcela.Parcela = Parcela;
            parcela.Vencimento = DateTime.Parse(Vencimento);
            parcela.ValorParcela = Decimal.Parse(ValorParcela);


            parcela.Observacao = Observacao;

            if (Id == 0)
                Id = FornecedoresLancamentosParcelasBo.Inserir(parcela);

            parcela.Id = Id;
            FornecedoresLancamentosParcelasBo.Salvar(parcela);

            var result = new { codigo = "00", idFornecedoresLancamentos = IdFornecedoresLancamentos };
            return Json(result);
        }

        [HttpPost]
        public JsonResult ExcluirParcela(Int64 IdNotaDuplicata)
        {
            try
            {
                FornecedoresLancamentosParcelasBo.Excluir(IdNotaDuplicata);

                var result = new { response = "removed" };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }
        #endregion
    }
}