using System;
using System.Linq;
using System.util;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class FornecedoresController : Controller
    {
        // GET: Fornecedores

        public ActionResult GetProviderList(string search, bool filter = false, int page = 1)
        {
            var fornecedores = FornecedoresBo.GetProviderList(search, filter);
            var total = fornecedores.Count();

            if (!(string.IsNullOrEmpty(search) || string.IsNullOrWhiteSpace(search)))
            {
                fornecedores = fornecedores.Where(x => x.text.ToLower().StartsWith(search.ToLower())).Take(page * 10).ToList();
            }

            return Json(new { fornecedores = fornecedores, total = total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = FornecedoresBo.Listar(search);
                ViewBag.search = search;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var fornecedor = FornecedoresBo.ConsultarArray(Id);
            var result = new { codigo = "00", fornecedor = fornecedor };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Buscar()
        {
            var fornecedores = FornecedoresBo.Consultar();
            var result = new { codigo = "00", fornecedores = fornecedores };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String TipoPessoa, String CPFCNPJ, String Nome, String RG, String IE, String IM, String CNAE, String Ativo, String Vendedor, String LimiteCredito, String Estrangeiro, String CEP, String Logradouro, String Numero, String Complemento, String Bairro, String Municipio, String Estado)
        {
            Fornecedores fornecedor = new Fornecedores();
            fornecedor.ID = Id;
            fornecedor.TipoPessoa = TipoPessoa;
            fornecedor.CpfCnpj = Common.Util.OnlyNumbers(CPFCNPJ);
            fornecedor.Nome = Nome.Trim();

            //fornecedor.CodigoMunicipio = MunicipiosBo.Consultar(Municipio);

            if (Ativo == "S")
            {
                fornecedor.Ativo = true;
            }
            else
            {
                fornecedor.Ativo = false;
            }

            if (RG != null && RG.Length > 0)
            {
                fornecedor.RG = Int64.Parse(Common.Util.OnlyNumbers(RG));
            }

            fornecedor.InscricaoEstadual = IE;
            fornecedor.InscricaoMunicipal = IM;
            fornecedor.Cnae = CNAE;

            fornecedor.Vendedor = Vendedor.Trim();
            fornecedor.LimiteCredito = Decimal.Parse((LimiteCredito.Trim().Length > 0 ? LimiteCredito : "0,00"));

            if (Estrangeiro == "S")
            {
                fornecedor.Estrangeiro = false;
            }
            else
            {
                fornecedor.Estrangeiro = true;
            }

            fornecedor.Cep = CEP.Trim();
            fornecedor.Endereco = Logradouro.Trim();
            fornecedor.Numero = Numero.Trim();
            fornecedor.Complemento = Complemento.Trim();
            fornecedor.Bairro = Bairro.Trim();
            fornecedor.Municipio = Municipio.Trim();
            fornecedor.Estado = Estado.Trim();

            fornecedor.DataCadastro = DateTime.Now;

            if (Id == 0)
                Id = FornecedoresBo.Inserir(fornecedor);

            fornecedor.ID = Id;
            FornecedoresBo.Salvar(fornecedor);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = FornecedoresBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        public ActionResult GetFornecedoresListRel(string search, int page = 1)
        {
            var fornecedores = FornecedoresBo.GetFornecedoresListRel(search);
            var total = fornecedores.Count();

            if (!(string.IsNullOrEmpty(search) || string.IsNullOrWhiteSpace(search)))
            {
                //    clientes = clientes.Where(x => x.text.ToLower().StartsWith(search.ToLower())).Take(page * 10).ToList();
            }

            return Json(new { fornecedores = fornecedores, total = total }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult BuscarEnderecos(Int32 Id)
        {
            var fornecedores = FornecedoresBo.ConsultarEnderecos(Id);
            var result = new { codigo = "00", fornecedores = fornecedores };
            return Json(result);
        }

        [HttpPost]
        public JsonResult BuscarEmails(Int32 Id)
        {
            var fornecedores = FornecedoresBo.ConsultarEmails(Id);
            var result = new { codigo = "00", fornecedores = fornecedores };
            return Json(result);
        }

        [HttpPost]
        public JsonResult ValidarDadosTelefonicos(Int64 IdFornecedor)
        {
            try
            {
                var existe = FornecedoresFonesBo.Validar(IdFornecedor);

                if (existe)
                {
                    var result = new { response = "success" };
                    return Json(result);
                }
                else
                {
                    var result = new { response = "empty" };
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult CarregarDadosTelefonicos(Int64 IdFornecedor)
        {
            try
            {
                var telefone = FornecedoresFonesBo.Listar(IdFornecedor);
                String telefones = String.Empty;

                int TelefonesQuantidade = (telefone.Count > 0 ? (telefone.Count + 1) : 1);

                for (int i = 0; i < TelefonesQuantidade; i++)
                {
                    telefones += "<input id='" + String.Format("HiddenDadoTelefonico{0}", (i + 1)) + "' name='HiddenDadoTelefonico[]' type='hidden' value='" + (telefone.Count >= (i + 1) ? telefone[i].ID.ToString() : "0") + "' />";

                    telefones += "<div class='accordion' id='accordionTelefones' style='border-bottom: 1px solid rgba(0,0,0,.125) !important;'>";
                    telefones += "   <div class='card z-depth-0 bordered'>";
                    telefones += "      <div class='card-header' id='" + String.Format("DadoTelefonico{0}heading", (i + 1)) + "'>";
                    telefones += "         <h5 class='mb-0'>";
                    telefones += "            <button class='btn btn-link' type='button' data-toggle='collapse' data-target='#" + String.Format("DadoTelefonico{0}collapse", (i + 1)) + "' aria-expanded='true' aria-controls='" + String.Format("DadoTelefonico{0}collapse", (i + 1)) + "'>";
                    telefones += "            " + String.Format("Telefone #{0}", (i + 1));
                    telefones += (telefone.Count >= (i + 1) ? String.Format(" - {0} - {1}{2}", Common.Util.FormatPhone(telefone[i].Numero), telefone[i].Status, (telefone[i].Principal.Value ? " - Principal" : String.Empty)) : String.Empty);
                    telefones += "            </button>";
                    telefones += "         </h5>";
                    telefones += "      </div>";
                    telefones += "      <div id='" + String.Format("DadoTelefonico{0}collapse", (i + 1)) + "' class='collapse' aria-labelledby='" + String.Format("DadoTelefonico{0}heading", (i + 1)) + "' data-parent='#accordionTelefones'>";
                    telefones += "         <div class='card-body'>";
                    telefones += "         <div class='row'>";
                    telefones += "            <div class='col-md-4'>";
                    telefones += "               <div class='form-group'>";
                    telefones += "                  <label class='titlefield'>Número <span class='required'>*</span></label>";
                    telefones += "                  <input type='text' id='" + String.Format("FornecedorDadoTelefonico{0}Numero", (i + 1)) + "' name='FornecedorDadoTelefonicoNumero[]' class='form-control' placeholder='Informe o Número' data-msg-required='Por favor, insira o número.' data-rule-required='true' maxlength='14' value='" + (telefone.Count >= (i + 1) ? Common.Util.FormatPhone(telefone[i].Numero) : String.Empty) + "' />";
                    telefones += "               </div>";
                    telefones += "            </div>";
                    telefones += "            <div class='col-md-4'>";
                    telefones += "               <div class='form-group'>";
                    telefones += "                  <label class='titlefield'>Ramal </label>";
                    telefones += "                  <input type='text' id='" + String.Format("FornecedorDadoTelefonico{0}Ramal", (i + 1)) + "' name='FornecedorDadoTelefonicoRamal[]' class='form-control' placeholder='Informe o Ramal' maxlength='10' value='" + (telefone.Count >= (i + 1) ? telefone[i].Ramal : String.Empty) + "' />";
                    telefones += "               </div>";
                    telefones += "            </div>";
                    telefones += "            <div class='col-md-4'>";
                    telefones += "               <div class='form-group'>";
                    telefones += "                  <label class='titlefield'>Tipo Aparelho <span class='required'>*</span></label>";
                    telefones += "                  <select id='" + String.Format("FornecedorDadoTelefonico{0}TipoAparelho", (i + 1)) + "' name='FornecedorDadoTelefonicoTipoAparelho[]' class='form-control' data-msg-required='Por favor, selecione o Tipo de Aparelho.' data-rule-required='true'>";
                    telefones += "                     <option value='' " + (telefone.Count == 0 ? "selected" : String.Empty) + " style='background-color: #eaeaea; font-weight: bold;'>selecione</option>";
                    telefones += "                     <option value='C' " + (telefone.Count >= (i + 1) ? (telefone[i].CF == "C" ? "selected" : String.Empty) : String.Empty) + ">Celular</option>";
                    telefones += "                     <option value='F' " + (telefone.Count >= (i + 1) ? (telefone[i].CF == "F" ? "selected" : String.Empty) : String.Empty) + ">Fixo</option>";
                    telefones += "                  </select>";
                    telefones += "               </div>";
                    telefones += "            </div>";
                    telefones += "         </div>";
                    telefones += "         <div class='row'>";
                    telefones += "            <div class='col-md-4'>";
                    telefones += "               <div class='form-group'>";
                    telefones += "                  <label class='titlefield'>Tipo Linha <span class='required'>*</span></label>";
                    telefones += "                  <select id='" + String.Format("FornecedorDadoTelefonico{0}TipoLinha", (i + 1)) + "' name='FornecedorDadoTelefonicoTipoLinha[]' class='form-control' data-msg-required='Por favor, selecione o Tipo de Linha.' data-rule-required='true'>";
                    telefones += "                     <option value='' " + (telefone.Count == 0 ? "selected" : String.Empty) + " style='background-color: #eaeaea; font-weight: bold;'>selecione</option>";
                    telefones += "                     <option value='Pessoal' " + (telefone.Count >= (i + 1) ? (telefone[i].Tipo == "Pessoal" ? "selected" : String.Empty) : String.Empty) + ">Pessoal</option>";
                    telefones += "                     <option value='Comercial' " + (telefone.Count >= (i + 1) ? (telefone[i].Tipo == "Comercial" ? "selected" : String.Empty) : String.Empty) + ">Comercial</option>";
                    telefones += "                  </select>";
                    telefones += "               </div>";
                    telefones += "            </div>";
                    telefones += "            <div class='col-md-4'>";
                    telefones += "               <div class='form-group'>";
                    telefones += "                  <label class='titlefield'>Status <span class='required'>*</span></label>";
                    telefones += "                  <select id='" + String.Format("FornecedorDadoTelefonico{0}Status", (i + 1)) + "' name='FornecedorDadoTelefonicoStatus[]' class='form-control' data-msg-required='Por favor, selecione o Status.' data-rule-required='true'>";
                    telefones += "                     <option value='' " + (telefone.Count == 0 ? "selected" : String.Empty) + " style='background-color: #eaeaea; font-weight: bold;'>selecione</option>";
                    telefones += "                     <option value='Ativo' " + (telefone.Count >= (i + 1) ? (telefone[i].Status == "Ativo" ? "selected" : String.Empty) : String.Empty) + ">Ativo</option>";
                    telefones += "                     <option value='Inativo' " + (telefone.Count >= (i + 1) ? (telefone[i].Status == "Inativo" ? "selected" : String.Empty) : String.Empty) + ">Inativo</option>";
                    telefones += "                  </select>";
                    telefones += "               </div>";
                    telefones += "            </div>";
                    telefones += "            <div class='col-md-4'>";
                    telefones += "               <div class='form-group'>";
                    telefones += "                  <label class='titlefield'>Principal <span class='required'>*</span></label>";
                    telefones += "                  <select id='" + String.Format("FornecedorDadoTelefonico{0}Principal", (i + 1)) + "' name='FornecedorDadoTelefonicoPrincipal[]' class='form-control' data-msg-required='Por favor, selecione o telefone Principal.' data-rule-required='true'>";
                    telefones += "                     <option value='' " + (telefone.Count == 0 ? "selected" : String.Empty) + " style='background-color: #eaeaea; font-weight: bold;'>selecione</option>";
                    telefones += "                     <option value='1' " + (telefone.Count >= (i + 1) ? (telefone[i].Principal.Value ? "selected" : String.Empty) : String.Empty) + ">Sim</option>";
                    telefones += "                     <option value='0' " + (telefone.Count >= (i + 1) ? (!telefone[i].Principal.Value ? "selected" : String.Empty) : String.Empty) + ">Não</option>";
                    telefones += "                  </select>";
                    telefones += "               </div>";
                    telefones += "            </div>";
                    telefones += "         </div>";
                    telefones += "         <div class='row'>";
                    telefones += "            <div class='col-md-12'>";
                    telefones += "               <div class='form-group'>";
                    telefones += "                  <label class='titlefield'>Observação</label>";
                    telefones += "                   <textarea id='" + String.Format("FornecedorDadoTelefonico{0}Observacao", (i + 1)) + "' name='FornecedorDadoTelefonicoObservacao[]' class='form-control' rows='3' placeholder='Descreva-se aqui ...'> " + (telefone.Count >= (i + 1) ? telefone[i].Observacao : String.Empty) + "</textarea>";
                    telefones += "               </div>";
                    telefones += "            </div>";
                    telefones += "         </div>";
                    telefones += "         <div class='row'>";
                    telefones += "            <div class='col-md-4'>";
                    telefones += "               <button type='button' id='" + String.Format("ButtonDadoTelefonico{0}Incluir", (i + 1)) + "' name='" + String.Format("ButtonDadoTelefonico{0}Incluir", (i + 1)) + "' class='btn btn-primary' data-loading-text='Salvando...' onclick='ButtonDadoTelefonicoSalvar_Click(" + (i + 1) + ");'>";
                    telefones += "                  Salvar";
                    telefones += "               </button>";
                    telefones += "            </div>";
                    telefones += "            <div class='col-md-4' style='text-align: center;'>";
                    telefones += "            </div>";
                    telefones += "            <div class='col-md-4' style='text-align: right;'>";
                    telefones += "               <button type='button' id='" + String.Format("ButtonDadoTelefonico{0}Excluir", (i + 1)) + "' name='" + String.Format("ButtonDadoTelefonico{0}Excluir", (i + 1)) + "' class='btn btn-danger' data-loading-text='Apagando...' onclick='ButtonDadoTelefonicoExcluir_Click(" + (i + 1) + ");' " + (telefone.Count >= (i + 1) ? String.Empty : "disabled='disabled'") + ">";
                    telefones += "                  Apagar";
                    telefones += "               </button>";
                    telefones += "            </div>";
                    telefones += "         </div>";
                    telefones += "         </div>";
                    telefones += "      </div>";
                    telefones += "   </div>";
                    telefones += "</div>";
                }

                var result = new { response = "success", telefones = telefones, count = TelefonesQuantidade };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult SalvarDadosTelefonicos(Int32 Id, Int32 IdFornecedor, String Numero, String Ramal, String TipoAparelho, String TipoLinha, String Status, String Principal, String Observacao)
        {
            try
            {
                FornecedoresFones telefone = new FornecedoresFones();
                telefone.ID = Id;
                telefone.IdFornecedor = IdFornecedor;
                telefone.Numero = Common.Util.OnlyNumbers(Numero);
                telefone.Ramal = Ramal;
                telefone.CF = TipoAparelho;
                telefone.Tipo = TipoLinha;
                telefone.Status = Status;
                telefone.Principal = (Principal == "1" ? true : false);
                telefone.Observacao = Observacao;

                if (Id == 0)
                    Id = FornecedoresFonesBo.Inserir(telefone);

                telefone.ID = Id;
                FornecedoresFonesBo.Salvar(telefone);

                var result = new { response = "success" };
                return Json(result);
            }
            catch (Exception ex)
            {
                String error = "error";
                String message = ex.InnerException.ToString();

                if (message.Contains("chave duplicada no objeto"))
                    error = "warning";

                var result = new { response = error };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult ExcluirDadosTelefonicos(Int32 Id)
        {
            try
            {
                FornecedoresFonesBo.Excluir(Id);

                var result = new { response = "removed" };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult ValidarDadosEmails(Int64 IdFornecedor)
        {
            try
            {
                var existe = FornecedoresEmailBo.Validar(IdFornecedor);

                if (existe)
                {
                    var result = new { response = "success" };
                    return Json(result);
                }
                else
                {
                    var result = new { response = "empty" };
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult CarregarDadosEmails(Int64 IdFornecedor)
        {
            try
            {
                var email = FornecedoresEmailBo.Listar(IdFornecedor);
                String emails = String.Empty;

                int EmailsQuantidade = (email.Count > 0 ? (email.Count + 1) : 1);

                for (int i = 0; i < EmailsQuantidade; i++)
                {
                    emails += "<input id='" + String.Format("HiddenDadoEmail{0}", (i + 1)) + "' name='HiddenDadoEmail[]' type='hidden' value='" + (email.Count >= (i + 1) ? email[i].ID.ToString() : "0") + "' />";

                    emails += "<div class='accordion' id='accordionEmails' style='border-bottom: 1px solid rgba(0,0,0,.125) !important;'>";
                    emails += "   <div class='card z-depth-0 bordered'>";
                    emails += "      <div class='card-header' id='" + String.Format("DadoEMail{0}heading", (i + 1)) + "'>";
                    emails += "         <h5 class='mb-0'>";
                    emails += "            <button class='btn btn-link' type='button' data-toggle='collapse' data-target='#" + String.Format("DadoEMail{0}collapse", (i + 1)) + "' aria-expanded='true' aria-controls='" + String.Format("DadoEMail{0}collapse", (i + 1)) + "'>";
                    emails += "            " + String.Format("E-Mail #{0}", (i + 1));
                    emails += (email.Count >= (i + 1) ? String.Format(" - {0} - {1}{2}", email[i].Email, (email[i].Ativo ? "Ativo" : "Inativo"), (email[i].Principal.Value ? " - Principal" : String.Empty)) : String.Empty);
                    emails += "            </button>";
                    emails += "         </h5>";
                    emails += "      </div>";
                    emails += "      <div id='" + String.Format("DadoEMail{0}collapse", (i + 1)) + "' class='collapse' aria-labelledby='" + String.Format("DadoEMail{0}heading", (i + 1)) + "' data-parent='#accordionEmails'>";
                    emails += "         <div class='card-body'>";
                    emails += "            <div class='row'>";
                    emails += "               <div class='col-md-6'>";
                    emails += "                  <div class='form-group'>";
                    emails += "                     <label class='titlefield'>E-mail <span class='required'>*</span></label>";
                    emails += "                     <input type='text' id='" + String.Format("FornecedorDadoEmail{0}Email", (i + 1)) + "' name='FornecedorDadoEmailEmail[]' class='form-control' placeholder='Informe o E-mail' data-msg-required='Por favor, insira o e-mail.' data-rule-required='true' maxlength='100' value='" + (email.Count >= (i + 1) ? email[i].Email : String.Empty) + "' />";
                    emails += "                  </div>";
                    emails += "               </div>";
                    emails += "               <div class='col-md-3'>";
                    emails += "                  <div class='form-group'>";
                    emails += "                     <label class='titlefield'>Status <span class='required'>*</span></label>";
                    emails += "                     <select id='" + String.Format("FornecedorDadoEmail{0}Status", (i + 1)) + "' name='FornecedorDadoEmailStatus[]' class='form-control' data-msg-required='Por favor, selecione o Status.' data-rule-required='true'>";
                    emails += "                        <option value='' " + (email.Count == 0 ? "selected" : String.Empty) + " style='background-color: #eaeaea; font-weight: bold;'>selecione</option>";
                    emails += "                        <option value='1' " + (email.Count >= (i + 1) ? (email[i].Ativo ? "selected" : String.Empty) : String.Empty) + ">Ativo</option>";
                    emails += "                        <option value='0' " + (email.Count >= (i + 1) ? (!email[i].Ativo ? "selected" : String.Empty) : String.Empty) + ">Inativo</option>";
                    emails += "                     </select>";
                    emails += "                  </div>";
                    emails += "               </div>";
                    emails += "               <div class='col-md-3'>";
                    emails += "                  <div class='form-group'>";
                    emails += "                     <label class='titlefield'>Principal <span class='required'>*</span></label>";
                    emails += "                     <select id='" + String.Format("FornecedorDadoEmail{0}Principal", (i + 1)) + "' name='FornecedorDadoEmailPrincipal[]' class='form-control' data-msg-required='Por favor, selecione o e-mail Principal.' data-rule-required='true'>";
                    emails += "                        <option value='' " + (email.Count == 0 ? "selected" : String.Empty) + " style='background-color: #eaeaea; font-weight: bold;'>selecione</option>";
                    emails += "                        <option value='1' " + (email.Count >= (i + 1) ? (email[i].Principal.Value ? "selected" : String.Empty) : String.Empty) + ">Sim</option>";
                    emails += "                        <option value='0' " + (email.Count >= (i + 1) ? (!email[i].Principal.Value ? "selected" : String.Empty) : String.Empty) + ">Não</option>";
                    emails += "                     </select>";
                    emails += "                  </div>";
                    emails += "               </div>";
                    emails += "            </div>";
                    emails += "            <div class='row'>";
                    emails += "               <div class='col-md-4'>";
                    emails += "                  <button type='button' id='" + String.Format("ButtonDadoEmail{0}Incluir", (i + 1)) + "' name='" + String.Format("ButtonDadoEmail{0}Incluir", (i + 1)) + "' class='btn btn-primary' data-loading-text='Salvando...' onclick='ButtonDadoEmailSalvar_Click(" + (i + 1) + ");'>";
                    emails += "                     Salvar";
                    emails += "                  </button>";
                    emails += "               </div>";
                    emails += "               <div class='col-md-4' style='text-align: center;'></div>";
                    emails += "               <div class='col-md-4' style='text-align: right;'>";
                    emails += "                  <button type='button' id='" + String.Format("ButtonDadoEmail{0}Excluir", (i + 1)) + "' name='" + String.Format("ButtonDadoEmail{0}Excluir", (i + 1)) + "' class='btn btn-danger' data-loading-text='Apagando...' onclick='ButtonDadoEmailExcluir_Click(" + (i + 1) + ");' " + (email.Count >= (i + 1) ? String.Empty : "disabled='disabled'") + ">";
                    emails += "                     Apagar";
                    emails += "                  </button>";
                    emails += "               </div>";
                    emails += "            </div>";
                    emails += "         </div>";
                    emails += "      </div>";
                    emails += "   </div>";
                    emails += "</div>";
                }

                var result = new { response = "success", emails = emails, count = EmailsQuantidade };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult SalvarDadosEmails(Int32 Id, Int32 IdFornecedor, String Email, String Status, String Principal)
        {
            try
            {
                FornecedoresEmail email = new FornecedoresEmail();
                email.ID = Id;
                email.IDFornecedor = IdFornecedor;
                email.Email = Email;
                email.Ativo = (Status == "1" ? true : false);
                email.Principal = (Principal == "1" ? true : false);

                if (Id == 0)
                    Id = FornecedoresEmailBo.Inserir(email);

                email.ID = Id;
                FornecedoresEmailBo.Salvar(email);

                var result = new { response = "success" };
                return Json(result);
            }
            catch (Exception ex)
            {
                String error = "error";
                String message = ex.InnerException.ToString();

                if (message.Contains("chave duplicada no objeto"))
                    error = "warning";

                var result = new { response = error };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult ExcluirDadosEmails(Int32 Id)
        {
            try
            {
                FornecedoresEmailBo.Excluir(Id);

                var result = new { response = "removed" };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }
    }
}