using System;
using System.Linq;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Common;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class ProfissionaisController : Controller
    {
        public ActionResult GetProfissionaisList(string search, bool filter = false, int page = 1)
        {
            var profissionais = ProfissionaisBo.GetProfisionaisList(search, filter);
            var total = profissionais.Count();

            if (!(string.IsNullOrEmpty(search) || string.IsNullOrWhiteSpace(search)))
            {
                profissionais = profissionais.Where(x => PessoasBo.ConverteNome(x.text).ToLower().StartsWith(PessoasBo.ConverteNome(search).ToLower())).Take(page * 10).ToList();
            }

            return Json(new { profissionais = profissionais, total = total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = ProfissionaisBo.Listar(search);
                ViewBag.search = search;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var profissional = ProfissionaisBo.ConsultarArray(Id);
            var result = new { codigo = "00", profissional = profissional };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, Int32 IdPessoa, String Nome, Int32 IdTipoProfissional, String OAB, String CPF, String RG, String CEP, String Logradouro, String Numero, String Complemento, String Bairro, String Estado, String Cidade, Int32? IdUsuario,
                                 Int32?  IdCargo, Int32? IdSede, Int32? IdArea, string TipoContrato, string Status, string Banco, string Agencia, string Conta, string ChavePix, string DataNascimento, String Salario)

        {
            Pessoas pessoa = new Pessoas();
            pessoa.ID = IdPessoa;
            pessoa.Nome = Nome.Trim();
            if (Util.IsNumeric(Util.OnlyNumbers(CPF.Trim())))
            {
                pessoa.CpfCnpj = Util.OnlyNumbers(CPF.Trim());
            }
            if (Util.IsNumeric(RG.Trim()))
            {
                pessoa.RG = RG.Trim();
            }
            pessoa.Cep = Util.OnlyNumbers(CEP.Trim());
            pessoa.Endereco = Logradouro.Trim();
            pessoa.Numero = Numero.Trim();
            pessoa.Complemento = Complemento.Trim();
            pessoa.Bairro = Bairro.Trim();
            pessoa.Estado = Estado.Trim();
            pessoa.Municipio = Cidade.Trim();

            Profissionais profissional = new Profissionais();
            profissional.ID = Id;
            profissional.IDPessoa = IdPessoa;
            profissional.IDTipoProfissional = IdTipoProfissional;
            profissional.OAB = OAB;
            profissional.IdUsuario = IdUsuario;
            profissional.IdCargo = IdCargo;
            profissional.IdSede = IdSede;
            profissional.IdArea = IdArea;
            profissional.TipoContrato = TipoContrato;

            profissional.Status = (Status == "Ativo" ? true : false);
            profissional.Banco = Banco;
            profissional.Agencia = Agencia;
            profissional.Conta = Conta;
            profissional.ChavePix = ChavePix;
            if (DataNascimento != null && DataNascimento.Trim().Length > 0)
            {
                profissional.DataNascimento = DateTime.Parse(DataNascimento);
            }
            if (Salario != null && Salario.Trim().Length > 0)
            {
                profissional.Salario = Decimal.Parse(Salario);
            }

            if (Id == 0)
            {
                IdPessoa = PessoasBo.Inserir(pessoa);

                profissional.IDPessoa = IdPessoa;
                Id = ProfissionaisBo.Inserir(profissional);
            }

            pessoa.ID = IdPessoa;
            PessoasBo.Salvar(pessoa);

            profissional.ID = Id;
            ProfissionaisBo.Salvar(profissional);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = ProfissionaisBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar(String Data, bool Todos = true)
        {
            var profissionais = ProfissionaisBo.ListarArray(Data, (Sessao.Usuario.Nivel.Value != 5 && Sessao.Usuario.Nivel.Value != 4 && !Todos ? Sessao.Usuario.ID : 0));

            var result = new { codigo = "00", profissionais = profissionais };
            return Json(result);
        }

        [HttpPost]
        public JsonResult CarregarDadosTelefonicos(Int64 IdPessoa)
        {
            try
            {
                var telefone = TelefonesBo.Listar(IdPessoa);
                String telefones = String.Empty;

                int TelefonesQuantidade = (telefone.Count > 0 ? (telefone.Count + 1) : 1);

                for (int i = 0; i < TelefonesQuantidade; i++)
                {
                    telefones += "<input id='" + String.Format("HiddenDadoTelefonico{0}", (i + 1)) + "' name='HiddenDadoTelefonico[]' type='hidden' value='" + (telefone.Count >= (i + 1) ? telefone[i].ID.ToString() : "0") + "' />";
                    telefones += "<div class='accordion panel panel-default'>";
                    telefones += "   <div class='accordion-heading panel-heading' id='" + String.Format("DadoTelefonico{0}heading", (i + 1)) + "' role='tab'>";
                    telefones += "      <h4 class='accordion-title panel-title'>";
                    telefones += "         <a href='" + String.Format("#DadoTelefonico{0}collapse", (i + 1)) + "' aria-expanded='false' data-toggle='collapse' aria-controls='" + String.Format("DadoTelefonico{0}collapse", (i + 1)) + "' data-parent='#DadoTelefonicoAccordion' role='button' class='collapsed'>";
                    telefones += "            " + String.Format("Telefone #{0}", (i + 1));
                    telefones += (telefone.Count >= (i + 1) ? String.Format(" - {0} - {1}{2}", Util.FormatPhone(telefone[i].Numero), telefone[i].Status, (telefone[i].Principal ? " - Principal" : String.Empty)) : String.Empty);
                    telefones += "         </a>";
                    telefones += "      </h4>";
                    telefones += "   </div>";
                    telefones += "   <div class='panel-collapse collapse' id='" + String.Format("DadoTelefonico{0}collapse", (i + 1)) + "' role='tabpanel' aria-labelledby='" + String.Format("DadoTelefonico{0}heading", (i + 1)) + "' aria-expanded='false' style='height: 0px;'>";
                    telefones += "      <div class='accordion-body panel-body'>";
                    telefones += "         <div class='row'>";
                    telefones += "            <div class='col-md-4'>";
                    telefones += "               <div class='form-group'>";
                    telefones += "                  <label class='titlefield'>Número <span class='required'>*</span></label>";
                    telefones += "                  <input type='text' id='" + String.Format("ProfissionalDadoTelefonico{0}Numero", (i + 1)) + "' name='ProfissionalDadoTelefonicoNumero[]' class='form-control' placeholder='Informe o Número' data-msg-required='Por favor, insira o número.' data-rule-required='true' maxlength='14' value='" + (telefone.Count >= (i + 1) ? Util.FormatPhone(telefone[i].Numero) : String.Empty) + "' />";
                    telefones += "               </div>";
                    telefones += "            </div>";
                    //telefones += "            <div class='col-md-4'>";
                    //telefones += "               <div class='form-group'>";
                    //telefones += "                  <label class='titlefield'>Ramal </label>"; 
                    //telefones += "                  <input type='text' id='" + String.Format("ProfissionalDadoTelefonico{0}Ramal", (i + 1)) + "' name='ProfissionalDadoTelefonicoRamal[]' class='form-control' placeholder='Informe o Ramal' maxlength='10' value='" + (telefone.Count >= (i + 1) ? telefone[i].Ramal : String.Empty) + "' />";
                    //telefones += "               </div>";
                    //telefones += "            </div>";
                    //telefones += "            <div class='col-md-4'>";
                    //telefones += "               <div class='form-group'>";
                    //telefones += "                  <label class='titlefield'>Tipo Aparelho <span class='required'>*</span></label>";
                    //telefones += "                  <select id='" + String.Format("ProfissionalDadoTelefonico{0}TipoAparelho", (i + 1)) + "' name='ProfissionalDadoTelefonicoTipoAparelho[]' class='form-control' data-msg-required='Por favor, selecione o Tipo de Aparelho.' data-rule-required='true'>";
                    //telefones += "                     <option value='' " + (telefone.Count == 0 ? "selected" : String.Empty) + " style='background-color: #eaeaea; font-weight: bold;'>selecione</option>";
                    //telefones += "                     <option value='C' " + (telefone.Count >= (i + 1) ? (telefone[i].CF == "C" ? "selected" : String.Empty) : String.Empty) + ">Celular</option>";
                    //telefones += "                     <option value='F' " + (telefone.Count >= (i + 1) ? (telefone[i].CF == "F" ? "selected" : String.Empty) : String.Empty) + ">Fixo</option>";
                    //telefones += "                  </select>";
                    //telefones += "               </div>";
                    //telefones += "            </div>";
                    //telefones += "         </div>";
                    //telefones += "         <div class='row'>";
                    //telefones += "            <div class='col-md-4'>";
                    //telefones += "               <div class='form-group'>";
                    //telefones += "                  <label class='titlefield'>Tipo Linha <span class='required'>*</span></label>";
                    //telefones += "                  <select id='" + String.Format("ProfissionalDadoTelefonico{0}TipoLinha", (i + 1)) + "' name='ProfissionalDadoTelefonicoTipoLinha[]' class='form-control' data-msg-required='Por favor, selecione o Tipo de Linha.' data-rule-required='true'>";
                    //telefones += "                     <option value='' " + (telefone.Count == 0 ? "selected" : String.Empty) + " style='background-color: #eaeaea; font-weight: bold;'>selecione</option>";
                    //telefones += "                     <option value='Pessoal' " + (telefone.Count >= (i + 1) ? (telefone[i].Tipo == "Pessoal" ? "selected" : String.Empty) : String.Empty) + ">Pessoal</option>";
                    //telefones += "                     <option value='Comercial' " + (telefone.Count >= (i + 1) ? (telefone[i].Tipo == "Comercial" ? "selected" : String.Empty) : String.Empty) + ">Comercial</option>";
                    //telefones += "                  </select>";
                    //telefones += "               </div>";
                    //telefones += "            </div>";
                    //telefones += "            <div class='col-md-4'>";
                    //telefones += "               <div class='form-group'>";
                    //telefones += "                  <label class='titlefield'>Status <span class='required'>*</span></label>";
                    //telefones += "                  <select id='" + String.Format("ProfissionalDadoTelefonico{0}Status", (i + 1)) + "' name='ProfissionalDadoTelefonicoStatus[]' class='form-control' data-msg-required='Por favor, selecione o Status.' data-rule-required='true'>";
                    //telefones += "                     <option value='' " + (telefone.Count == 0 ? "selected" : String.Empty) + " style='background-color: #eaeaea; font-weight: bold;'>selecione</option>";
                    //telefones += "                     <option value='Ativo' " + (telefone.Count >= (i + 1) ? (telefone[i].Status == "Ativo" ? "selected" : String.Empty) : String.Empty) + ">Ativo</option>";
                    //telefones += "                     <option value='Inativo' " + (telefone.Count >= (i + 1) ? (telefone[i].Status == "Inativo" ? "selected" : String.Empty) : String.Empty) + ">Inativo</option>";
                    //telefones += "                  </select>";
                    //telefones += "               </div>";
                    //telefones += "            </div>";
                    telefones += "            <div class='col-md-4'>";
                    telefones += "               <div class='form-group'>";
                    telefones += "                  <label class='titlefield'>Principal <span class='required'>*</span></label>";
                    telefones += "                  <select id='" + String.Format("ProfissionalDadoTelefonico{0}Principal", (i + 1)) + "' name='ProfissionalDadoTelefonicoPrincipal[]' class='form-control' data-msg-required='Por favor, selecione o telefone Principal.' data-rule-required='true'>";
                    telefones += "                     <option value='' " + (telefone.Count == 0 ? "selected" : String.Empty) + " style='background-color: #eaeaea; font-weight: bold;'>selecione</option>";
                    telefones += "                     <option value='1' " + (telefone.Count >= (i + 1) ? (telefone[i].Principal ? "selected" : String.Empty) : String.Empty) + ">Sim</option>";
                    telefones += "                     <option value='0' " + (telefone.Count >= (i + 1) ? (!telefone[i].Principal ? "selected" : String.Empty) : String.Empty) + ">Não</option>";
                    telefones += "                  </select>";
                    telefones += "               </div>";
                    telefones += "            </div>";
                    telefones += "         </div>";
                    telefones += "         <div class='row'>";
                    telefones += "            <div class='col-md-12'>";
                    telefones += "               <div class='form-group'>";
                    telefones += "                  <label class='titlefield'>Observação</label>";
                    telefones += "                   <textarea id='" + String.Format("ProfissionalDadoTelefonico{0}Observacao", (i + 1)) + "' name='ProfissionalDadoTelefonicoObservacao[]' class='form-control' rows='1' placeholder='Descreva-se aqui ...' maxlength='50'> " + (telefone.Count >= (i + 1) ? telefone[i].Observacao : String.Empty) + "</textarea>";
                    telefones += "               </div>";
                    telefones += "            </div>";
                    telefones += "         </div>";
                    telefones += "         <div class='row'>";
                    telefones += "            <div class='col-md-4'>";
                    telefones += "               <button type='button' id='" + String.Format("ButtonDadoTelefonico{0}Incluir", (i + 1)) + "' name='" + String.Format("ButtonDadoTelefonico{0}Incluir", (i + 1)) + "' class='btn btn-primary' data-loading-text='Salvando...' onclick='ButtonDadoTelefonicoSalvar_Click(" + (i + 1) + ");'>";
                    telefones += "                  Salvar";
                    telefones += "               </button>";
                    telefones += "            </div>";
                    telefones += "            <div class='col-md-3' style='text-align: center;'>";
                    telefones += "            </div>";
                    telefones += "            <div class='col-md-5' style='text-align: right;'>";
                    telefones += "               <button type='button' id='" + String.Format("ButtonDadoTelefonico{0}Excluir", (i + 1)) + "' name='" + String.Format("ButtonDadoTelefonico{0}Excluir", (i + 1)) + "' class='btn btn-danger' data-loading-text='Apagando...' onclick='ButtonDadoTelefonicoExcluir_Click(" + (i + 1) + ");' " + (telefone.Count >= (i + 1) ? String.Empty : "disabled='disabled'") + ">";
                    telefones += "                  Apagar";
                    telefones += "               </button>";
                    telefones += "               <div id='" + String.Format("ButtonDadoTelefonico{0}ExcluirConfirmacao", (i + 1)) + "' style='display:none'>";
                    telefones += "                  <label id='" + String.Format("LabelDadoTelefonico{0}ExcluirConfirmacao", (i + 1)) + "' style='font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente excluir esse Telefone?</label><br />";
                    telefones += "                  <button id='" + String.Format("accept{0}", (i + 1)) + "' class='btn btn-default btn-xs' value='true'>Sim</button>";
                    telefones += "                  <button id='" + String.Format("go_back{0}", (i + 1)) + "' class='btn btn-primary btn-xs' value='false'>Não</button>";
                    telefones += "               </div>";
                    telefones += "            </div>";
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
        public JsonResult SalvarDadosTelefonicos(Int32 Id, Int32 IdPessoa, String Numero, String Ramal, String TipoAparelho, String TipoLinha, String Status, String Principal, String Observacao)
        {
            try
            {
                Fones telefone = new Fones();
                telefone.ID = Id;
                telefone.IDPessoa = IdPessoa;
                telefone.Numero = Util.OnlyNumbers(Numero);
                telefone.Ramal = (Ramal != "undefined" ? Ramal : null);
                telefone.CF = (TipoAparelho != "undefined" ? TipoAparelho : null);
                telefone.Tipo = (TipoLinha != "undefined" ? TipoLinha : null);
                telefone.Status = (Status != "undefined" ? Status : "Ativo");
                telefone.Principal = (Principal == "1" ? true : false);
                telefone.Observacao = Observacao.Trim();

                if (Id == 0)
                    Id = TelefonesBo.Inserir(telefone);

                telefone.ID = Id;
                TelefonesBo.Salvar(telefone);

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
                TelefonesBo.Excluir(Id);

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
        public JsonResult CarregarDadosEmails(Int64 IdPessoa)
        {
            try
            {
                var email = EmailsBo.Listar(IdPessoa);
                String emails = String.Empty;

                int EmailsQuantidade = (email.Count > 0 ? (email.Count + 1) : 1);

                for (int i = 0; i < EmailsQuantidade; i++)
                {
                    emails += "<input id='" + String.Format("HiddenDadoEmail{0}", (i + 1)) + "' name='HiddenDadoEmail[]' type='hidden' value='" + (email.Count >= (i + 1) ? email[i].ID.ToString() : "0") + "' />";
                    emails += "<div class='accordion panel panel-default'>";
                    emails += "   <div class='accordion-heading panel-heading' id='" + String.Format("DadoEmail{0}heading", (i + 1)) + "' role='tab'>";
                    emails += "      <h4 class='accordion-title panel-title'>";
                    emails += "         <a href='" + String.Format("#DadoEmail{0}collapse", (i + 1)) + "' aria-expanded='false' data-toggle='collapse' aria-controls='" + String.Format("DadoEmail{0}collapse", (i + 1)) + "' data-parent='#DadoEmailAccordion' role='button' class='collapsed'>";
                    emails += "            " + String.Format("E-mail #{0}", (i + 1));
                    emails += (email.Count >= (i + 1) ? String.Format(" - {0} - {1}{2}", email[i].Email1, (email[i].Ativo ? "Ativo" : "Inativo"), (email[i].Principal ? " - Principal" : String.Empty)) : String.Empty);
                    emails += "         </a>";
                    emails += "      </h4>";
                    emails += "   </div>";
                    emails += "   <div class='panel-collapse collapse' id='" + String.Format("DadoEmail{0}collapse", (i + 1)) + "' role='tabpanel' aria-labelledby='" + String.Format("DadoEmail{0}heading", (i + 1)) + "' aria-expanded='false' style='height: 0px;'>";
                    emails += "      <div class='accordion-body panel-body'>";
                    emails += "         <div class='row'>";
                    emails += "            <div class='col-md-8'>";
                    emails += "               <div class='form-group'>";
                    emails += "                  <label class='titlefield'>E-mail <span class='required'>*</span></label>";
                    emails += "                  <input type='text' id='" + String.Format("ProfissionalDadoEmail{0}Email", (i + 1)) + "' name='ProfissionalDadoEmailEmail[]' class='form-control' placeholder='Informe o E-mail' data-msg-required='Por favor, insira o e-=mail.' data-rule-required='true' maxlength='100' value='" + (email.Count >= (i + 1) ? email[i].Email1 : String.Empty) + "' />";
                    emails += "               </div>";
                    emails += "            </div>";
                    //emails += "            <div class='col-md-2'>";
                    //emails += "               <div class='form-group'>";
                    //emails += "                  <label class='titlefield'>Status <span class='required'>*</span></label>";
                    //emails += "                  <select id='" + String.Format("ProfissionalDadoEmail{0}Status", (i + 1)) + "' name='ProfissionalDadoEmailStatus[]' class='form-control' data-msg-required='Por favor, selecione o Status.' data-rule-required='true'>";
                    //emails += "                     <option value='' " + (email.Count == 0 ? "selected" : String.Empty) + " style='background-color: #eaeaea; font-weight: bold;'>selecione</option>";
                    //emails += "                     <option value='1' " + (email.Count >= (i + 1) ? (email[i].Ativo ? "selected" : String.Empty) : String.Empty) + ">Ativo</option>";
                    //emails += "                     <option value='0' " + (email.Count >= (i + 1) ? (!email[i].Ativo ? "selected" : String.Empty) : String.Empty) + ">Inativo</option>";
                    //emails += "                  </select>";
                    //emails += "               </div>";
                    //emails += "            </div>";
                    emails += "            <div class='col-md-2'>";
                    emails += "               <div class='form-group'>";
                    emails += "                  <label class='titlefield'>Principal <span class='required'>*</span></label>";
                    emails += "                  <select id='" + String.Format("ProfissionalDadoEmail{0}Principal", (i + 1)) + "' name='ProfissionalDadoEmailPrincipal[]' class='form-control' data-msg-required='Por favor, selecione o e-mail Principal.' data-rule-required='true'>";
                    emails += "                     <option value='' " + (email.Count == 0 ? "selected" : String.Empty) + " style='background-color: #eaeaea; font-weight: bold;'>selecione</option>";
                    emails += "                     <option value='1' " + (email.Count >= (i + 1) ? (email[i].Principal ? "selected" : String.Empty) : String.Empty) + ">Sim</option>";
                    emails += "                     <option value='0' " + (email.Count >= (i + 1) ? (!email[i].Principal ? "selected" : String.Empty) : String.Empty) + ">Não</option>";
                    emails += "                  </select>";
                    emails += "               </div>";
                    emails += "            </div>";
                    emails += "         </div>";
                    emails += "         <div class='row'>";
                    emails += "            <div class='col-md-12'>";
                    emails += "               <div class='form-group'>";
                    emails += "                  <label class='titlefield'>Observação</label>";
                    emails += "                   <textarea id='" + String.Format("ClienteDadoEmail{0}Observacao", (i + 1)) + "' name='ClienteDadoEmailObservacao[]' class='form-control' rows='1' placeholder='Descreva-se aqui ...' maxlength='50'>" + (email.Count >= (i + 1) ? email[i].Observacao : String.Empty) + "</textarea>";
                    emails += "               </div>";
                    emails += "            </div>";
                    emails += "         </div>";
                    emails += "         <div class='row'>";
                    emails += "            <div class='col-md-4'>";
                    emails += "               <button type='button' id='" + String.Format("ButtonDadoEmail{0}Incluir", (i + 1)) + "' name='" + String.Format("ButtonDadoEmail{0}Incluir", (i + 1)) + "' class='btn btn-primary' data-loading-text='Salvando...' onclick='ButtonDadoEmailSalvar_Click(" + (i + 1) + ");'>";
                    emails += "                  Salvar";
                    emails += "               </button>";
                    emails += "            </div>";
                    emails += "            <div class='col-md-3' style='text-align: center;'>";
                    emails += "            </div>";
                    emails += "            <div class='col-md-5' style='text-align: right;'>";
                    emails += "               <button type='button' id='" + String.Format("ButtonDadoEmail{0}Excluir", (i + 1)) + "' name='" + String.Format("ButtonDadoEmail{0}Excluir", (i + 1)) + "' class='btn btn-danger' data-loading-text='Apagando...' onclick='ButtonDadoEmailExcluir_Click(" + (i + 1) + ");' " + (email.Count >= (i + 1) ? String.Empty : "disabled='disabled'") + ">";
                    emails += "                  Apagar";
                    emails += "               </button>";
                    emails += "               <div id='" + String.Format("ButtonDadoEmail{0}ExcluirConfirmacao", (i + 1)) + "' style='display:none'>";
                    emails += "                  <label id='" + String.Format("LabelDadoEmail{0}ExcluirConfirmacao", (i + 1)) + "' style='font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente excluir esse E-mail?</label><br />";
                    emails += "                  <button id='" + String.Format("accept{0}", (i + 1)) + "' class='btn btn-default btn-xs' value='true'>Sim</button>";
                    emails += "                  <button id='" + String.Format("go_back{0}", (i + 1)) + "' class='btn btn-primary btn-xs' value='false'>Não</button>";
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
        public JsonResult SalvarDadosEmails(Int32 Id, Int32 IdPessoa, String Email, String Status, String Principal, String Observacao)
        {
            try
            {
                Email email = new Email();
                email.ID = Id;
                email.IDPessoa = IdPessoa;
                email.Email1 = Email.Trim().ToLower();
                email.Ativo = (Status == "1" || Status == "undefined" ? true : false);
                email.Principal = (Principal == "1" ? true : false);
                email.Observacao = Observacao.Trim();

                if (Id == 0)
                    Id = EmailsBo.Inserir(email);

                email.ID = Id;
                EmailsBo.Salvar(email);

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
                EmailsBo.Excluir(Id);

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