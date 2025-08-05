using System;
using System.Globalization;
using System.Linq;
using System.util;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class FornecedoresNotasController : Controller
    {
        // GET: FornecedoresNotas
        public ActionResult Index(string FiltrarCPFCNPJ = "", string FiltrarFornecedor = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = FornecedoresNotasBo.Listar(FiltrarCPFCNPJ.Replace(".", string.Empty).Replace("/", string.Empty).Replace("-", string.Empty), FiltrarFornecedor);
                ViewBag.FiltrarCPFCNPJ = FiltrarCPFCNPJ;
                ViewBag.FiltrarFornecedor = FiltrarFornecedor;
                return View(model);
            }
        }

        #region Notas
        [HttpPost]
        public JsonResult Consultar(Int64 Id)
        {
            var nota = FornecedoresNotasBo.ConsultarArray(Id);
            var result = new { codigo = "00", nota = nota };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int64 Id, Int32 IdEmpresa, Int32 IdFornecedor, String NumeroDocumento, String DataEmissao, String Total, Int32 Plano, String ValorParcela, String Observacao, 
                                 String PrimeiroVencimento, Int32 IdContrato, Int32 IdAcao, Int32 IdSede, Int32 IdFormaPagamento)
        {
            FornecedoresNotas nota = new FornecedoresNotas();
            nota.Id = Id;
            nota.IdEmpresa = IdEmpresa;
            nota.IdFornecedor = IdFornecedor;
            nota.NumeroDocumento = NumeroDocumento;
            nota.Plano = Plano;
            nota.Observacao = Observacao;
            nota.IdContrato = IdContrato;
            nota.IdAcao = IdAcao;
            nota.IdSede = IdSede;
            nota.IdFormaPagamento = IdFormaPagamento; 

            if (DataEmissao != null && DataEmissao.Trim().Length > 0)
            {
                nota.DataEmissao = DateTime.Parse(DataEmissao);
            }

            if (PrimeiroVencimento != null && PrimeiroVencimento.Trim().Length > 0)
            {
                nota.PrimeiroVencimento = DateTime.Parse(PrimeiroVencimento);
            }

            if (Total != null && Total.Trim().Length > 0)
            {
                nota.Total = Decimal.Parse(Total);
            }

            if (ValorParcela != null && ValorParcela.Trim().Length > 0)
            {
                nota.ValorParcela = Decimal.Parse(ValorParcela);
            }

            nota.DataCadastro = DateTime.Now;
            nota.NomeUsuarioCadastro = Sessao.Usuario.Nome;
            nota.DataAlteracao = DateTime.Now;
            nota.NomeUsuarioAlteracao = Sessao.Usuario.Nome;

            if (Id == 0)
            {
                Id = FornecedoresNotasBo.Inserir(nota);
                FornecedoresNotasBo.GerarNotaParcelas(Id);
            }

            nota.Id = Id;
            FornecedoresNotasBo.Salvar(nota);
            var result = new { codigo = "00", id = Id };
            return Json(result);
        }

        [HttpPost]
        public JsonResult CarregarNota(Int64 Id)
        {
            try
            {
                var nota = FornecedoresNotasBo.Consultar(Id);
                String visualizar = String.Empty;

                if (nota != null)
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
                    visualizar += "         <label class='titlefield'>" + nota.DataEmissao.Value.ToString("dd/MM/yyyy") + "</label>";
                    visualizar += "      </div>";
                    visualizar += "   </div>";
                    visualizar += "   <div class='col-md-3'>";
                    visualizar += "      <div class='form-group'>";
                    visualizar += "         <label class='titlefield' style='width: 100%; text-align: right !important;'><b>Orçamento Total R$</b></label><br />";
                    visualizar += "         <label style='width: 100%; text-align: right !important; font-size: 18px !important; font-weight: bold !important; color: #5B77A7;'>" + String.Format("{0:##,##0.00}", nota.Total) + "</label>";
                    visualizar += "      </div>";
                    visualizar += "   </div>";
                    visualizar += "   <div class='col-md-1'>";
                    visualizar += "      <div class='form-group'>";
                    visualizar += "         <label class='titlefield'><b>Plano</b></label><br />";
                    visualizar += "         <label class='titlefield'>" + nota.Plano + "</label>";
                    visualizar += "      </div>";
                    visualizar += "   </div>";
                    visualizar += "</div>";
                    visualizar += "<div class='row'>";
                    visualizar += "   <div class='col-md-6'>";
                    visualizar += "      <div class='form-group'>";
                    visualizar += "         <label class='titlefield'><b>Nome Fornecedor</b></label><br />";
                    visualizar += "         <label class='titlefield'>" + nota.Fornecedores.Nome + "</label>";
                    visualizar += "      </div>";
                    visualizar += "   </div>";
                    visualizar += "</div>";
                    visualizar += "<div class='row'>";
                    visualizar += "   <div class='col-md-12'>";
                    visualizar += "      <div class='form-group'>";
                    visualizar += "         <label class='titlefield'><b>Observação</b></label><br />";
                    visualizar += "         <label class='titlefield'>" + nota.Observacao + "</label>";
                    visualizar += "      </div>";
                    visualizar += "   </div>";
                    visualizar += "</div>";
                    visualizar += "<div class='row'>";
                    visualizar += "   <div class='col-md-3'>";
                    visualizar += "      <div class='form-group'>";
                    visualizar += "         <label class='titlefield'><b>Data Cadastro</b></label><br />";
                    visualizar += "         <label class='titlefield'>" + nota.DataCadastro + "</label>";
                    visualizar += "      </div>";
                    visualizar += "   </div>";
                    visualizar += "   <div class='col-md-3'>";
                    visualizar += "      <div class='form-group'>";
                    visualizar += "         <label class='titlefield'><b>Usuário Cadastro</b></label><br />";
                    visualizar += "         <label class='titlefield'>" + nota.NomeUsuarioCadastro + "</label>";
                    visualizar += "      </div>";
                    visualizar += "   </div>";
                    visualizar += "   <div class='col-md-3'>";
                    visualizar += "      <div class='form-group'>";
                    visualizar += "         <label class='titlefield'><b>Data Alterção</b></label><br />";
                    visualizar += "         <label class='titlefield'>" + nota.DataAlteracao + "</label>";
                    visualizar += "      </div>";
                    visualizar += "   </div>";
                    visualizar += "   <div class='col-md-3'>";
                    visualizar += "      <div class='form-group'>";
                    visualizar += "         <label class='titlefield'><b>Usuário Alterção</b></label><br />";
                    visualizar += "         <label class='titlefield'>" + nota.NomeUsuarioAlteracao + "</label>";
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

                    foreach (var item in nota.FornecedoresNotasParcelas)
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
                        visualizar += "   <div class='col-md-2'>";
                        visualizar += "      <div class='form-group'>";

                        if (i == 0)
                        {
                            visualizar += "         <label class='titlefield'><b>Data Pagamento</b></label><br />";
                        }

                        visualizar += "         <label class='titlefield'>" + item.DataPagamento.Value.ToString("dd/MM/yyyy") + "</label>";
                        visualizar += "      </div>";
                        visualizar += "   </div>";
                        visualizar += "   <div class='col-md-2'>";
                        visualizar += "      <div class='form-group'>";

                        if (i == 0)
                        {
                            visualizar += "         <label class='titlefield'><b>Valor Pago R$</b></label><br />";
                        }

                        visualizar += "         <label class='titlefield'>" + String.Format("{0:##,##0.00}", item.ValorPago) + "</label>";
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
                var parcela = FornecedoresNotasParcelasBo.Listar(IdNota);
                String parcelas = String.Empty;

                int ParcelasQuantidade = (parcela.Count > 0 ? (parcela.Count + 1) : 1);

                for (int i = 0; i < ParcelasQuantidade; i++)
                {
                    parcelas += "<input id='" + String.Format("HiddenParcelaNotaDuplicata{0}", (i + 1)) + "' name='HiddenParcelaNotaDuplicata[]' type='hidden' value='" + (parcela.Count >= (i + 1) ? parcela[i].Id.ToString() : "0") + "' />";
                    parcelas += "<div class='accordion' id='accordionKits' style='border-bottom: 1px solid rgba(0,0,0,.125) !important;'>";
                    parcelas += "   <div class='card z-depth-0 bordered'>";
                    parcelas += "      <div class='card-header' id='" + String.Format("ParcelaNotaDuplicata{0}heading", (i + 1)) + "'>";
                    parcelas += "         <h5 class='mb-0'>";
                    //parcelas += "            <button class='btn btn-link' type='button' data-toggle='collapse' data-target='#" + String.Format("ParcelaNotaDuplicata{0}collapse", (i + 1)) + "' aria-expanded='true' aria-controls='" + String.Format("ParcelaNotaDuplicata{0}collapse", (i + 1)) + "'>";
                    //parcelas += "            " + String.Format("Parcela #{0}", (i + 1));
                    //parcelas += "            </button>";
                    parcelas += "            <button class='btn btn-link' type='button' data-toggle='collapse' data-target='#" + String.Format("ParcelaNotaDuplicata{0}collapse", (i + 1)) + "' aria-expanded='true' aria-controls='" + String.Format("ParcelaNotaDuplicata{0}collapse", (i + 1)) + "'>";

                    parcelas += (parcela.Count >= (i + 1) ? String.Format("{0}", String.Format(@" 
                                                        <table cellspacing='5' style='font-size: 12px;'>
                                                            <tr>
                                                                <td width='80'>{0}</td>
                                                                <td width='150'><b>Vencimento:</b> {1}</td>
                                                                <td width='150'><b>Valor:</b> {2}</td>
                                                                <td width='150'><b>Dt Pagto:</b> {3}</td>
                                                                <td width='150'><b>Valor Pago:</b> {4}</td>
                                                            </tr>
                                                        </table>"
                                                        , String.Format("Parcela #{0}", (i + 1)), (parcela[i].Vencimento != null ? parcela[i].Vencimento.Value.ToString("dd/MM/yyyy") : string.Empty), (parcela[i].ValorParcela != null ? string.Format("R$ {0:##,##0.00}", parcela[i].ValorParcela) : "0,00").PadLeft(10, ' '), (parcela[i].DataPagamento != null ? parcela[i].DataPagamento.Value.ToString("dd/MM/yyyy") : string.Empty), (parcela[i].ValorPago != null ? string.Format("R$ {0:##,##0.00}", parcela[i].ValorPago) : "0,00").PadLeft(10, ' '))) : String.Format("Parcela #{0}", (i + 1)));

                    parcelas += "            </button>";
                    parcelas += "         </h5>";
                    parcelas += "      </div>";
                    parcelas += "      <div id='" + String.Format("ParcelaNotaDuplicata{0}collapse", (i + 1)) + "' class='collapse' aria-labelledby='" + String.Format("ParcelaNotaDuplicata{0}heading", (i + 1)) + "' data-parent='#accordionKits'>";
                    parcelas += "         <div class='card-body'>";
                    parcelas += "            <div class='row'>";
                    parcelas += "               <div class='col-md-2'>";
                    parcelas += "                  <div class='form-group'>";
                    parcelas += "                     <label class='titlefield'>Parcela</label>";
                    parcelas += "                     <input type='text' id='" + String.Format("ParcelaNotaDuplicata{0}Parcela", (i + 1)) + "' name='ParcelaNotaDuplicataParcela[]' class='form-control' value='" + (i + 1) + "' readonly />";
                    parcelas += "                  </div>";
                    parcelas += "               </div>";
                    parcelas += "               <div class='col-md-2'>";
                    parcelas += "                  <div class='form-group'>";
                    parcelas += "                     <label class='titlefield'>Data de Vencimento <span class='required'>*</span></label>";
                    parcelas += "                     <input id='" + String.Format("ParcelaNotaDuplicata{0}DataVencimento", (i + 1)) + "' name='ParcelaNotaDuplicataDataVencimento[]' class='form-control' placeholder='Selecione uma Data' type='text' style='height: 40px !important; cursor: pointer;' value='" + (parcela.Count >= (i + 1) ? parcela[i].Vencimento.Value.ToString("dd/MM/yyyy") : string.Empty) + "' readonly />";
                    parcelas += "                     <label id='" + String.Format("ParcelaNotaDuplicata{0}DataVencimentoValidate", (i + 1)) + "' name='ParcelaNotaDuplicataDataVencimentoValidate[]' for='" + String.Format("ParcelaNotaDuplicata{0}DataVencimento", (i + 1)) + "' class='error hidden'>Por favor, selecionar uma Data.</label>";
                    parcelas += "                  </div>";
                    parcelas += "               </div>";
                    parcelas += "               <div class='col-md-2'>";
                    parcelas += "                  <div class='form-group'>";
                    parcelas += "                     <label class='titlefield'>Valor da Parcela R$ <span class='required'>*</span></label>";
                    parcelas += "                     <input id='" + String.Format("ParcelaNotaDuplicata{0}ValorParcela", (i + 1)) + "' name='ParcelaNotaDuplicataValorParcela[]' class='form-control' placeholder='Informe um Valor' type='text' style='height: 40px !important; cursor: pointer;' value='" + (parcela.Count >= (i + 1) ? parcela[i].ValorParcela.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : string.Empty) + "' onkeyup='CalcularParcela(" + (i + 1) + ")' />";
                    parcelas += "                     <label id='" + String.Format("ParcelaNotaDuplicata{0}ValorParcelaValidate", (i + 1)) + "' name='ParcelaNotaDuplicataValorParcelaValidate[]' for='" + String.Format("ParcelaNotaDuplicata{0}ValorParcela", (i + 1)) + "' class='error hidden'>Por favor, informar um Valor.</label>";
                    parcelas += "                  </div>";
                    parcelas += "               </div>";
                    parcelas += "               <div class='col-md-2'>";
                    parcelas += "                  <div class='form-group'>";
                    parcelas += "                     <label class='titlefield'>Data de Pagamento</label>";
                    parcelas += "                     <input id='" + String.Format("ParcelaNotaDuplicata{0}DataPagamento", (i + 1)) + "' name='ParcelaNotaDuplicataDataPagamento[]' class='form-control' placeholder='Selecione uma Data' type='text' style='height: 40px !important; cursor: pointer;' value='" + (parcela.Count >= (i + 1) && parcela[i].DataPagamento != null ? parcela[i].DataPagamento.Value.ToString("dd/MM/yyyy") : string.Empty) + "' readonly />";
                    parcelas += "                  </div>";
                    parcelas += "               </div>";
                    parcelas += "            </div>";
                    parcelas += "            <div class='row'>";
                    parcelas += "               <div class='col-md-2'>";
                    parcelas += "                  <div class='form-group'>";
                    parcelas += "                     <label class='titlefield'>Valor da Pago R$</label>";
                    parcelas += "                     <input id='" + String.Format("ParcelaNotaDuplicata{0}ValorPago", (i + 1)) + "' name='ParcelaNotaDuplicataValorPago[]' class='form-control' placeholder='Informe um Valor' type='text' style='height: 40px !important; cursor: pointer;' value='" + (parcela.Count >= (i + 1) && parcela[i].ValorPago != null ? parcela[i].ValorPago.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : string.Empty) + "' onkeyup='CalcularParcela(" + (i + 1) + ")' />";
                    parcelas += "                  </div>";
                    parcelas += "               </div>";
                    parcelas += "               <div class='col-md-2'>";
                    parcelas += "                  <div class='form-group'>";
                    parcelas += "                     <label class='titlefield'>Juros R$</label>";
                    parcelas += "                     <input id='" + String.Format("ParcelaNotaDuplicata{0}Juros", (i + 1)) + "' name='ParcelaNotaDuplicataJuros[]' class='form-control' placeholder='Informe um Valor' type='text' style='height: 40px !important; cursor: pointer;' value='" + (parcela.Count >= (i + 1) && parcela[i].Juros != null ? parcela[i].Juros.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : string.Empty) + "' onkeyup='CalcularParcela(" + (i + 1) + ")' />";
                    parcelas += "                  </div>";
                    parcelas += "               </div>";
                    parcelas += "               <div class='col-md-2'>";
                    parcelas += "                  <div class='form-group'>";
                    parcelas += "                     <label class='titlefield'>Desconto R$</label>";
                    parcelas += "                     <input id='" + String.Format("ParcelaNotaDuplicata{0}Desconto", (i + 1)) + "' name='ParcelaNotaDuplicataDesconto[]' class='form-control' placeholder='Informe um Valor' type='text' style='height: 40px !important; cursor: pointer;' value='" + (parcela.Count >= (i + 1) && parcela[i].Descontos != null ? parcela[i].Descontos.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : string.Empty) + "' onkeyup='CalcularParcela(" + (i + 1) + ")' />";
                    parcelas += "                  </div>";
                    parcelas += "               </div>";
                    parcelas += "               <div class='col-md-2'>";
                    parcelas += "                  <div class='form-group'>";
                    parcelas += "                     <label class='titlefield'>Saldo R$</label>";
                    parcelas += "                     <input id='" + String.Format("ParcelaNotaDuplicata{0}Saldo", (i + 1)) + "' name='ParcelaNotaDuplicataSaldo[]' class='form-control' placeholder='Informe um Valor' type='text' style='height: 40px !important; cursor: pointer;' value='" + (parcela.Count >= (i + 1) && parcela[i].Saldo != null ? parcela[i].Saldo.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : string.Empty) + "' readonly />";
                    parcelas += "                  </div>";
                    parcelas += "               </div>";
                    parcelas += "            </div>";
                    parcelas += "            <div class='row'>";
                    parcelas += "               <div class='col-md-12'>";
                    parcelas += "                  <div class='form-group'>";
                    parcelas += "                     <label class='titlefield'>Observação</label>";
                    parcelas += "                     <textarea id='" + String.Format("ParcelaNotaDuplicata{0}Observacao", (i + 1)) + "' name='ParcelaNotaDuplicataObservacao[]' rows=3 class='form-control' placeholder='Informe aqui a Observação'>" + (parcela.Count >= (i + 1) ? parcela[i].Observacao : string.Empty) + "</textarea>";
                    parcelas += "                  </div>";
                    parcelas += "               </div>";
                    parcelas += "            </div>";
                    parcelas += "            <div class='row'>";
                    parcelas += "               <div class='col-md-3'>";
                    parcelas += "                  <div class='form-group'>";
                    parcelas += "                     <label class='titlefield'>Data Cadastro</label>";
                    parcelas += "                     <input type='text' id='" + String.Format("ParcelaNotaDuplicata{0}DataCadastro", (i + 1)) + "' name='ParcelaNotaDuplicataDataCadastro[]' class='form-control' value='" + (parcela.Count >= (i + 1) && parcela[i].DataCadastro != null ? parcela[i].DataCadastro.Value.ToString("dd/MM/yyyy") : string.Empty) + "' readonly />";
                    parcelas += "                  </div>";
                    parcelas += "               </div>";
                    parcelas += "               <div class='col-md-3'>";
                    parcelas += "                  <div class='form-group'>";
                    parcelas += "                     <label class='titlefield'>Usuário Cadastro</label>";
                    parcelas += "                     <input type='text' id='" + String.Format("ParcelaNotaDuplicata{0}UsuarioCadastro", (i + 1)) + "' name='ParcelaNotaDuplicataUsuarioCadastro[]' class='form-control' value='" + (parcela.Count >= (i + 1) ? parcela[i].NomeUsuarioCadastro : string.Empty) + "' readonly />";
                    parcelas += "                  </div>";
                    parcelas += "               </div>";
                    parcelas += "               <div class='col-md-3'>";
                    parcelas += "                  <div class='form-group'>";
                    parcelas += "                     <label class='titlefield'>Data Alteração</label>";
                    parcelas += "                     <input type='text' id='" + String.Format("ParcelaNotaDuplicata{0}DataAlteracao", (i + 1)) + "' name='ParcelaNotaDuplicataDataAlteracao[]' class='form-control' value='" + (parcela.Count >= (i + 1) && parcela[i].DataAlteracao != null ? parcela[i].DataAlteracao.Value.ToString("dd / MM / yyyy") : string.Empty) + "'readonly />";
                    parcelas += "                  </div>";
                    parcelas += "               </div>";
                    parcelas += "               <div class='col-md-3'>";
                    parcelas += "                  <div class='form-group'>";
                    parcelas += "                     <label class='titlefield'>Usuário Alteração</label>";
                    parcelas += "                     <input type='text' id='" + String.Format("ParcelaNotaDuplicata{0}UsuarioAlteracao", (i + 1)) + "' name='ParcelaNotaDuplicataUsuarioAlteracao[]' class='form-control' value='" + (parcela.Count >= (i + 1) ? parcela[i].NomeUsuarioAlteracao : string.Empty) + "' readonly />";
                    parcelas += "                  </div>";
                    parcelas += "               </div>";
                    parcelas += "            </div>";
                    parcelas += "            <hr />";
                    parcelas += "            <div class='row'>";
                    parcelas += "               <div class='col-md-4'>";
                    parcelas += "                  <button type='button' id='" + String.Format("ButtonParcelaNotaDuplicata{0}Incluir", (i + 1)) + "' name='" + String.Format("ButtonParcelaNotaDuplicata{0}Incluir", (i + 1)) + "' class='btn btn-primary' data-loading-text='Salvando...' onclick='ButtonParcelaNotaDuplicataSalvar_Click(" + (i + 1) + ");'>";
                    parcelas += "                     Salvar";
                    parcelas += "                  </button>";
                    parcelas += "               </div>";
                    parcelas += "               <div class='col-md-4' style='text-align: center;'>";
                    parcelas += "               </div>";
                    parcelas += "               <div class='col-md-4' style='text-align: right;'>";

                    if (parcela.Count >= (i + 1) && parcela[i].DataPagamento == null && parcela[i].ValorPago == null)
                    {
                        parcelas += "                  <button type='button' id='" + String.Format("ButtonParcelaNotaDuplicata{0}Excluir", (i + 1)) + "' name='" + String.Format("ButtonParcelaNotaDuplicata{0}Excluir", (i + 1)) + "' class='btn btn-danger' data-loading-text='Apagando...' onclick='ButtonParcelaNotaDuplicataExcluir_Click(" + (i + 1) + ");' " + (parcela.Count >= (i + 1) ? String.Empty : "disabled='disabled'") + ">";
                        parcelas += "                     Apagar";
                        parcelas += "                  </button>";
                    }

                    parcelas += "               </div>";
                    parcelas += "            </div>";
                    parcelas += "         </div>";
                    parcelas += "      </div>";
                    parcelas += "   </div>";
                    parcelas += "</div>";

                    parcelas += "<script>";
                    parcelas += "   $('#ParcelaNotaDuplicata" + (i + 1) + "DataVencimento').datepicker({";
                    parcelas += "      language: 'pt-BR',";
                    parcelas += "      autoclose: true,";
                    parcelas += "      todayHighlight: true,";
                    parcelas += "      orientation: 'bottom',";
                    parcelas += "      clearBtn: true";
                    parcelas += "   });";
                    parcelas += "   $('#ParcelaNotaDuplicata" + (i + 1) + "DataPagamento').datepicker({";
                    parcelas += "      language: 'pt-BR',";
                    parcelas += "      autoclose: true,";
                    parcelas += "      todayHighlight: true,";
                    parcelas += "      orientation: 'bottom',";
                    parcelas += "      clearBtn: true";
                    parcelas += "   });";
                    parcelas += "   $('#ParcelaNotaDuplicata" + (i + 1) + "ValorParcela').maskMoney({ thousands: '.', decimal: ',', symbolStay: true });";
                    parcelas += "   $('#ParcelaNotaDuplicata" + (i + 1) + "ValorPago').maskMoney({ thousands: '.', decimal: ',', symbolStay: true });";
                    parcelas += "   $('#ParcelaNotaDuplicata" + (i + 1) + "Juros').maskMoney({ thousands: '.', decimal: ',', symbolStay: true });";
                    parcelas += "   $('#ParcelaNotaDuplicata" + (i + 1) + "Desconto').maskMoney({ thousands: '.', decimal: ',', symbolStay: true });";
                    parcelas += "   $('#ParcelaNotaDuplicata" + (i + 1) + "Saldo').maskMoney({ thousands: '.', decimal: ',', symbolStay: true });";
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
        public JsonResult SalvarParcela(Int64 Id, Int64 IdNotaDuplicata, Int32 Parcela, String Vencimento, String ValorParcela, 
            String DataPagamento, String ValorPago, String Juros, String Desconto, String Saldo,
            String Observacao)
        {
            FornecedoresNotasParcelas parcela = new FornecedoresNotasParcelas();
            parcela.Id = Id;
            parcela.IdFornecedoresNota = IdNotaDuplicata;
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

            var result = new { codigo = "00", idNotaDuplicata = IdNotaDuplicata };
            return Json(result);
        }

        [HttpPost]
        public JsonResult ExcluirParcela(Int64 IdNotaDuplicata)
        {
            try
            {
                FornecedoresNotasParcelasBo.Excluir(IdNotaDuplicata);

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