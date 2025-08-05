using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Common;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class ColaboradoresController : Controller
    {
        public ActionResult GetProfissionaisList(string search, bool filter = false, int page = 1)
        {
            var profissionais = ColaboradoresBo.GetProfisionaisList(search, filter);
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
                var model = ColaboradoresBo.Listar(search);
                ViewBag.search = search;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var colaborador = ColaboradoresBo.ConsultarArray(Id);
            //var result = new { codigo = "00", colaborador = colaborador };

            var fotoarquivo = "Nenhum arquivo selecionado";
            var aniversarioarquivo = "Nenhum arquivo selecionado";
            var aniversariocasaarquivo = "Nenhum arquivo selecionado";

            var file = Path.Combine(Server.MapPath("~/RH/Fotos"), String.Format("{0}.pdf", "Foto-" + Id.ToString()));
            if (System.IO.File.Exists(file))
            {
                fotoarquivo = "<a style='cursor: pointer;' onclick='VisualizarFotoPDF(" + '"' + String.Format("/RH/Fotos/{0}.pdf", "Foto-" + Id.ToString()) + '"' + ");'>" + String.Format("{0}.pdf", "Foto-" + Id.ToString()) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;<a id='ButtonFotoPDFExcluir' class='btn btn-danger btn-xs' onclick='ExcluirFotoPDF()'>Apagar</a>&nbsp;&nbsp;<label id='LabelFotoFileExcluirConfirmacao' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse PDF?&nbsp;&nbsp;&nbsp;</label><a id='acceptFile' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirFotoPDFSim(" + '"' + String.Format("{0}.pdf", "Foto-" + Id.ToString()) + '"' + ")'>Sim</a><a id='go_backFile' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirFotoPDFNao()'>Não</a>";
            }

            var aniversariofile = Path.Combine(Server.MapPath("~/RH/Fotos"), String.Format("{0}.pdf", "Aniversario-" + Id.ToString()));
            if (System.IO.File.Exists(aniversariofile))
            {
                aniversarioarquivo = "<a style='cursor: pointer;' onclick='VisualizarAniversarioPDF(" + '"' + String.Format("/RH/Fotos/{0}.pdf", "Aniversario-" + Id.ToString()) + '"' + ");'>" + String.Format("{0}.pdf", "Aniversario-" + Id.ToString()) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;<a id='ButtonAniversarioPDFExcluir' class='btn btn-danger btn-xs' onclick='ExcluirAniversarioPDF()'>Apagar</a>&nbsp;&nbsp;<label id='LabelAniversarioFileExcluirConfirmacao' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse arquivo?&nbsp;&nbsp;&nbsp;</label><a id='acceptFile' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirAniversarioPDFSim(" + '"' + String.Format("{0}.pdf", "Aniversario-" + Id.ToString()) + '"' + ")'>Sim</a><a id='go_backFile' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirAniversarioPDFNao()'>Não</a>";
            }

            var aniversariocasafile = Path.Combine(Server.MapPath("~/RH/Fotos"), String.Format("{0}.pdf", "AniversarioCasa-" + Id.ToString()));
            if (System.IO.File.Exists(aniversariocasafile))
            {
                aniversariocasaarquivo = "<a style='cursor: pointer;' onclick='VisualizarAniversarioCasaPDF(" + '"' + String.Format("/RH/Fotos/{0}.pdf", "AniversarioCasa-" + Id.ToString()) + '"' + ");'>" + String.Format("{0}.pdf", "AniversarioCasa-" + Id.ToString()) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;<a id='ButtonAniversarioCasaPDFExcluir' class='btn btn-danger btn-xs' onclick='ExcluirAniversarioCasaPDF()'>Apagar</a>&nbsp;&nbsp;<label id='LabelAniversarioCasaFileExcluirConfirmacao' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse arquivo?&nbsp;&nbsp;&nbsp;</label><a id='acceptFile' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirAniversarioCasaPDFSim(" + '"' + String.Format("{0}.pdf", "AniversarioCasa-" + Id.ToString()) + '"' + ")'>Sim</a><a id='go_backFile' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirAniversarioCasaPDFNao()'>Não</a>";
            }

            var result = new { codigo = "00", colaborador = colaborador, fotoarquivo = fotoarquivo, aniversarioarquivo = aniversarioarquivo, aniversariocasaarquivo = aniversariocasaarquivo };

            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, Int32 IdPessoa, Int32 IdTipoProfissional, String Nome, String CPF, String RG, String OAB, String PIS, String Logradouro, String Numero, String Complemento, String CEP, String Bairro, String Estado,
                                 String Cidade, Int32? IdCargo, Int32? IdSede, Int32? IdArea, string TipoContrato, string Status, string Banco, string Agencia, string Conta, string ChavePix, Int32? IdUsuario, string DataNascimento,
                                 HttpPostedFileBase FotoArquivo, HttpPostedFileBase AniversarioArquivo, HttpPostedFileBase AniversarioCasaArquivo, string Salario)
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

            Profissionais colaborador = new Profissionais();
            colaborador.ID = Id;
            colaborador.IDPessoa = IdPessoa;
            colaborador.IDTipoProfissional = IdTipoProfissional;
            colaborador.OAB = OAB;
            // colaborador.PIS = PIS;
            colaborador.IdUsuario = IdUsuario;
            colaborador.IdCargo = IdCargo;
            colaborador.IdSede = IdSede;
            colaborador.IdArea = IdArea;
            colaborador.TipoContrato = TipoContrato;

            colaborador.Status = (Status == "Ativo" ? true : false);
            colaborador.Banco = Banco;
            colaborador.Agencia = Agencia;
            colaborador.Conta = Conta;
            colaborador.ChavePix = ChavePix;
            if (DataNascimento != null && DataNascimento.Trim().Length > 0)
            {
                colaborador.DataNascimento = DateTime.Parse(DataNascimento);
            }

            if (Salario != null && Salario.Trim().Length > 0)
            {
                colaborador.Salario = Decimal.Parse(Salario);
            }

            if (Id == 0)
            {
                IdPessoa = PessoasBo.Inserir(pessoa);

                colaborador.IDPessoa = IdPessoa;
                Id = ColaboradoresBo.Inserir(colaborador);
            }

            pessoa.ID = IdPessoa;
            PessoasBo.Salvar(pessoa);

            colaborador.ID = Id;
            ColaboradoresBo.Salvar(colaborador);

            if (FotoArquivo != null && FotoArquivo.ContentLength > 0)
            {
                var file = Path.Combine(Server.MapPath("~/RH/Fotos"), String.Format("Foto-{0}.pdf", Id.ToString()));
                FotoArquivo.SaveAs(file);
            }

            if (AniversarioArquivo != null && AniversarioArquivo.ContentLength > 0)
            {
                var file = Path.Combine(Server.MapPath("~/RH/Fotos"), String.Format("Aniversario-{0}.pdf", Id.ToString()));
                AniversarioArquivo.SaveAs(file);
            }

            if (AniversarioCasaArquivo != null && AniversarioCasaArquivo.ContentLength > 0)
            {
                var file = Path.Combine(Server.MapPath("~/RH/Fotos"), String.Format("AniversarioCasa-{0}.pdf", Id.ToString()));
                AniversarioCasaArquivo.SaveAs(file);
            }

            var result = new
            {
                codigo = "00",
                colaborador = colaborador,
                pessoa = pessoa
            };

            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = ColaboradoresBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        public JsonResult ExcluirFotoPDF(String Arquivo)
        {
            try
            {
                var file = Path.Combine(Server.MapPath("~/RH/Fotos"), Arquivo);
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }

                var result = new { response = "success" };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        public JsonResult ExcluirAniversarioPDF(String Arquivo)
        {
            try
            {
                var file = Path.Combine(Server.MapPath("~/RH/Fotos"), Arquivo);
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }

                var result = new { response = "success" };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        public JsonResult ExcluirAniversarioCasaPDF(String Arquivo)
        {
            try
            {
                var file = Path.Combine(Server.MapPath("~/RH/Fotos"), Arquivo);
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }

                var result = new { response = "success" };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult Listar(String Data)
        {
            var profissionais = ColaboradoresBo.ListarArray(Data);
            var result = new { codigo = "00", profissionais = profissionais };
            return Json(result);
        }

        // Telefone
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
                    telefones += (telefone.Count >= (i + 1)
                        ? String.Format(" - {0} - {1} - {2}{3}",
                            Util.FormatPhone(telefone[i].Numero),
                            telefone[i].Tipo ?? "Tipo não informado",
                            telefone[i].Status ?? "Status não informado",
                            telefone[i].Principal ? " - Principal" : String.Empty)
                        : String.Empty);
                    telefones += "         </a>";

                    telefones += "      </h4>";
                    telefones += "   </div>";
                    telefones += "   <div class='panel-collapse collapse' id='" + String.Format("DadoTelefonico{0}collapse", (i + 1)) + "' role='tabpanel' aria-labelledby='" + String.Format("DadoTelefonico{0}heading", (i + 1)) + "' aria-expanded='false' style='height: 0px;'>";
                    telefones += "      <div class='accordion-body panel-body'>";
                    telefones += "         <div class='row'>";
                    telefones += "            <div class='col-md-3'>";
                    telefones += "               <div class='form-group'>";
                    telefones += "                  <label class='titlefield'>Número <span class='required'>*</span></label>";
                    telefones += "                  <input type='text' id='" + String.Format("ColaboradorDadoTelefonico{0}Numero", (i + 1)) + "' name='ColaboradorDadoTelefonicoNumero[]' class='form-control' placeholder='Informe o Número' data-msg-required='Por favor, insira o número.' data-rule-required='true' maxlength='14' value='" + (telefone.Count >= (i + 1) ? Util.FormatPhone(telefone[i].Numero) : String.Empty) + "' />";
                    telefones += "               </div>";
                    telefones += "            </div>";
                    telefones += "            <div class='col-md-3'>";
                    telefones += "               <div class='form-group'>";
                    telefones += "                  <label class='titlefield'>Tipo Linha <span class='required'>*</span></label>";
                    telefones += "                  <select id='" + String.Format("ColaboradorDadoTelefonico{0}TipoLinha", (i + 1)) + "' name='ColaboradorDadoTelefonicoTipoLinha[]' class='form-control' data-msg-required='Por favor, selecione o Tipo de Linha.' data-rule-required='true'>";
                    telefones += "                     <option value='' " + (telefone.Count == 0 ? "selected" : String.Empty) + " style='background-color: #eaeaea; font-weight: bold;'>selecione</option>";
                    telefones += "                     <option value='Pessoal' " + (telefone.Count >= (i + 1) ? (telefone[i].Tipo == "Pessoal" ? "selected" : String.Empty) : String.Empty) + ">Pessoal</option>";
                    telefones += "                     <option value='Profissional' " + (telefone.Count >= (i + 1) ? (telefone[i].Tipo == "Profissional" ? "selected" : String.Empty) : String.Empty) + ">Profissional</option>";
                    telefones += "                     <option value='Contato Emergência' " + (telefone.Count >= (i + 1) ? (telefone[i].Tipo == "Contato Emergência" ? "selected" : String.Empty) : String.Empty) + ">Contato Emergência</option>";
                    telefones += "                     <option value='Residencial' " + (telefone.Count >= (i + 1) ? (telefone[i].Tipo == "Residencial" ? "selected" : String.Empty) : String.Empty) + ">Residencial</option>";
                    telefones += "                  </select>";
                    telefones += "               </div>";
                    telefones += "            </div>";
                    telefones += "            <div class='col-md-3'>";
                    telefones += "               <div class='form-group'>";
                    telefones += "                  <label class='titlefield'>Principal <span class='required'>*</span></label>";
                    telefones += "                  <select id='" + String.Format("ColaboradorDadoTelefonico{0}Principal", (i + 1)) + "' name='ColaboradorDadoTelefonicoPrincipal[]' class='form-control' data-msg-required='Por favor, selecione o telefone Principal.' data-rule-required='true'>";
                    telefones += "                     <option value='' " + (telefone.Count == 0 ? "selected" : String.Empty) + " style='background-color: #eaeaea; font-weight: bold;'>selecione</option>";
                    telefones += "                     <option value='1' " + (telefone.Count >= (i + 1) ? (telefone[i].Principal ? "selected" : String.Empty) : String.Empty) + ">Sim</option>";
                    telefones += "                     <option value='0' " + (telefone.Count >= (i + 1) ? (!telefone[i].Principal ? "selected" : String.Empty) : String.Empty) + ">Não</option>";
                    telefones += "                  </select>";
                    telefones += "               </div>";
                    telefones += "            </div>";

                    telefones += "            <div class='col-md-3'>";
                    telefones += "               <div class='form-group'>";
                    telefones += "                  <label class='titlefield'>Status <span class='required'>*</span></label>";
                    telefones += "                  <select id='" + String.Format("ColaboradorDadoTelefonico{0}Status", (i + 1)) + "' name='ColaboradorDadoTelefonicoStatus[]' class='form-control' data-msg-required='Por favor, selecione o Status.' data-rule-required='true'>";
                    telefones += "                     <option value='' " + (telefone.Count == 0 ? "selected" : String.Empty) + " style='background-color: #eaeaea; font-weight: bold;'>selecione</option>";
                    telefones += "                     <option value='Ativo' " + (telefone.Count >= (i + 1) ? (telefone[i].Status == "Ativo" ? "selected" : String.Empty) : String.Empty) + ">Ativo</option>";
                    telefones += "                     <option value='Inativo' " + (telefone.Count >= (i + 1) ? (telefone[i].Status == "Inativo" ? "selected" : String.Empty) : String.Empty) + ">Inativo</option>";
                    telefones += "                  </select>";
                    telefones += "               </div>";
                    telefones += "            </div>";

                    telefones += "         </div>";
                    telefones += "         <div class='row'>";
                    telefones += "            <div class='col-md-12'>";
                    telefones += "               <div class='form-group'>";
                    telefones += "                  <label class='titlefield'>Observação</label>";
                    telefones += "                   <textarea id='" + String.Format("ColaboradorDadoTelefonico{0}Observacao", (i + 1)) + "' name='ColaboradorDadoTelefonicoObservacao[]' class='form-control' rows='1' placeholder='Descreva-se aqui ...' maxlength='50'> " + (telefone.Count >= (i + 1) ? telefone[i].Observacao : String.Empty) + "</textarea>";
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
        public JsonResult SalvarDadosTelefonicos(Int32 Id, Int32 IdPessoa, String Numero, String TipoLinha, String Principal, String Observacao, String Status)
        {
            try
            {
                Fones telefone = new Fones();
                telefone.ID = Id;
                telefone.IDPessoa = IdPessoa;
                telefone.Numero = Util.OnlyNumbers(Numero);
                telefone.Status = (Status != "undefined" ? Status : "Ativo");
                telefone.Tipo = (TipoLinha != "undefined" ? TipoLinha : null);
                telefone.Principal = (Principal == "1" ? true : false);
                telefone.Observacao = Observacao.Trim();

                if (Id == 0)
                    Id = TelefonesBo.Inserir(telefone);

                telefone.ID = Id;
                TelefonesBo.Salvar(telefone);

                var result = new { response = "success", dados = telefone };
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

        // Email
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
                    emails += (email.Count >= (i + 1) ? String.Format(" - {0} - {1}{2}", email[i].Email1, email[i].Tipo, (email[i].Ativo ? " - Ativo" : " - Inativo"), (email[i].Principal ? " - Principal" : String.Empty)) : String.Empty);
                    emails += "         </a>";
                    emails += "      </h4>";
                    emails += "   </div>";
                    emails += "   <div class='panel-collapse collapse' id='" + String.Format("DadoEmail{0}collapse", (i + 1)) + "' role='tabpanel' aria-labelledby='" + String.Format("DadoEmail{0}heading", (i + 1)) + "' aria-expanded='false' style='height: 0px;'>";
                    emails += "      <div class='accordion-body panel-body'>";
                    emails += "         <div class='row'>";
                    emails += "            <div class='col-md-3'>";
                    emails += "               <div class='form-group'>";
                    emails += "                  <label class='titlefield'>E-mail <span class='required'>*</span></label>";
                    emails += "                  <input type='text' id='" + String.Format("ColaboradorDadoEmail{0}Email", (i + 1)) + "' name='ColaboradorDadoEmailEmail[]' class='form-control' placeholder='Informe o E-mail' data-msg-required='Por favor, insira o e-=mail.' data-rule-required='true' maxlength='100' value='" + (email.Count >= (i + 1) ? email[i].Email1 : String.Empty) + "' />";
                    emails += "               </div>";
                    emails += "            </div>";
                    emails += "            <div class='col-md-3'>";
                    emails += "               <div class='form-group'>";
                    emails += "                  <label class='titlefield'>Principal <span class='required'>*</span></label>";
                    emails += "                  <select id='" + String.Format("ColaboradorDadoEmail{0}Principal", (i + 1)) + "' name='ColaboradorDadoEmailPrincipal[]' class='form-control' data-msg-required='Por favor, selecione o e-mail Principal.' data-rule-required='true'>";
                    emails += "                     <option value='' " + (email.Count == 0 ? "selected" : String.Empty) + " style='background-color: #eaeaea; font-weight: bold;'>selecione</option>";
                    emails += "                     <option value='1' " + (email.Count >= (i + 1) ? (email[i].Principal ? "selected" : String.Empty) : String.Empty) + ">Sim</option>";
                    emails += "                     <option value='0' " + (email.Count >= (i + 1) ? (!email[i].Principal ? "selected" : String.Empty) : String.Empty) + ">Não</option>";
                    emails += "                  </select>";
                    emails += "               </div>";
                    emails += "            </div>";
                    emails += "            <div class='col-md-3'>";
                    emails += "               <div class='form-group'>";
                    emails += "                  <label class='titlefield'>Tipo E-mail <span class='required'>*</span></label>";
                    emails += "                  <select id='" + String.Format("ColaboradorDadoEmail{0}Tipo", (i + 1)) + "' name='ColaboradorDadoEmailTipo[]' class='form-control' data-msg-required='Por favor, selecione o Tipo.' data-rule-required='true'>";
                    emails += "                     <option value='' " + (email.Count == 0 ? "selected" : String.Empty) + " style='background-color: #eaeaea; font-weight: bold;'>selecione</option>";
                    emails += "                     <option value='Pessoal' " + (email.Count >= (i + 1) ? (email[i].Tipo == "Pessoal" ? "selected" : String.Empty) : String.Empty) + ">Pessoal</option>";
                    emails += "                     <option value='Profissional' " + (email.Count >= (i + 1) ? (email[i].Tipo == "Profissional" ? "selected" : String.Empty) : String.Empty) + ">Profissional</option>";
                    emails += "                  </select>";
                    emails += "               </div>";
                    emails += "            </div>";

                    emails += "            <div class='col-md-3'>";
                    emails += "               <div class='form-group'>";
                    emails += "                  <label class='titlefield'>Status <span class='required'>*</span></label>";
                    emails += "                  <select id='" + String.Format("ColaboradorDadoEmail{0}Status", (i + 1)) + "' name='ColaboradorDadoEmailStatus[]' class='form-control' data-msg-required='Por favor, selecione o status do e-mail.' data-rule-required='true'>";
                    emails += "                     <option value='' " + (email.Count == 0 ? "selected" : String.Empty) + " font-weight: bold;'>selecione</option>";
                    emails += "                     <option value='1' " + (email.Count >= (i + 1) ? (email[i].Ativo ? "selected" : String.Empty) : String.Empty) + ">Ativo</option>";
                    emails += "                     <option value='0' " + (email.Count >= (i + 1) ? (!email[i].Ativo ? "selected" : String.Empty) : String.Empty) + ">Inativo</option>";
                    emails += "                  </select>";
                    emails += "               </div>";
                    emails += "            </div>";

                    emails += "         </div>";
                    emails += "         <div class='row'>";
                    emails += "            <div class='col-md-12'>";
                    emails += "               <div class='form-group'>";
                    emails += "                  <label class='titlefield'>Observação</label>";
                    emails += "                   <textarea id='" + String.Format("ColaboradorDadoEmail{0}Observacao", (i + 1)) + "' name='ColaboradorDadoEmailObservacao[]' class='form-control' rows='1' placeholder='Descreva-se aqui ...' maxlength='50'>" + (email.Count >= (i + 1) ? email[i].Observacao : String.Empty) + "</textarea>";
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
        public JsonResult SalvarDadosEmails(Int32 Id, Int32 IdPessoa, String Email, String Status, String Principal, String Observacao, String Tipo)
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
                email.Tipo = Tipo.Trim();

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

        // Documentos
        [HttpPost]
        public JsonResult CarregarDadosDocumentos(Int32 IdProfissional)
        {
            try
            {
                var documento = ProfissionaisDocumentosBo.ListarDocumentos(IdProfissional);
                String documentos = String.Empty;

                int DocumentosQuantidade = (documento.Count > 0 ? (documento.Count + 1) : 1);

                for (int i = 0; i < DocumentosQuantidade; i++)
                {


                    // Início
                    documentos += "<input id='" + String.Format("HiddenDadoDocumento{0}", (i + 1)) + "' name='HiddenDadoDocumento[]' type='hidden' value='" + (documento.Count > i ? documento[i].Id.ToString() : "0") + "' />";
                    documentos += "<div class='accordion panel panel-default'>";
                    documentos += "   <div class='accordion-heading panel-heading' id='" + String.Format("DadoDocumento{0}heading", (i + 1)) + "' role='tab'>";
                    documentos += "      <h4 class='accordion-title panel-title'>";
                    documentos += "         <a href='" + String.Format("#DadoDocumento{0}collapse", (i + 1)) + "' aria-expanded='false' data-toggle='collapse' aria-controls='" + String.Format("DadoDocumento{0}collapse", (i + 1)) + "' data-parent='#DadoDocumentoAccordion' role='button' class='collapsed'>";

                    // Adicionar o título do documento e os detalhes
                    if (documento.Count >= (i + 1))
                    {
                        string dataEmissao = documento[i].DataEmissao.HasValue
                            ? documento[i].DataEmissao.Value.ToString("dd/MM/yyyy")
                            : "Data de emissão não informada";
                        string dataValidade = documento[i].DataValidade.HasValue
                            ? documento[i].DataValidade.Value.ToString("dd/MM/yyyy")
                            : "Data de validade não informada";
                        string tipoDocumento = !string.IsNullOrEmpty(documento[i].TipoDocumento)
                            ? documento[i].TipoDocumento
                            : "Tipo não informado";

                        documentos += String.Format("Documento #{0} - Emissão: {1} - Validade: {2} - {3}",
                            (i + 1),
                            dataEmissao,
                            dataValidade,
                            tipoDocumento);
                    }
                    else
                    {
                        documentos += String.Format("Documento #{0}", (i + 1));
                    }
                    documentos += "         </a>";
                    documentos += "      </h4>";
                    documentos += "   </div>";
                    documentos += "   <div class='panel-collapse collapse' id='" + String.Format("DadoDocumento{0}collapse", (i + 1)) + "' role='tabpanel'  aria-labelledby='" + String.Format("DadoDocumento{0}heading", (i + 1)) + "' aria-expanded='false' style='height: 0px;'>";
                    documentos += "      <div class='accordion-body panel-body'>";

                    // FileUploader

                    documentos += "         <div class='row'>";
                    documentos += "            <div class='col-md-12'>";
                    documentos += "               <div class='form-group'>";
                    documentos += "                  <label class='titlefield'>PDF Documento<label class='titlefieldcomment'> <span class='red'>(tamanho máximo de 10MB por arquivo)</label></label>";
                    documentos += "                  <div id='" + String.Format("ColaboradorDocumentos{0}Upload", (i + 1)) + (documento.Count == 0 ? 1 : documento[0].Id) + "' class='upload'>";
                    documentos += "                     <input type='button' class='uploadButton' value='Selecione o arquivo' onclick='$(" + '"' + String.Format("#ColaboradorDocumentos{0}DocumentosFileUpload", (i + 1)) + '"' + ").click();' />";
                    documentos += "                     <input type='file' id='" + String.Format("ColaboradorDocumentos{0}DocumentosFileUpload", (i + 1)) + "' name='" + String.Format("ColaboradorDocumentos{0}DocumentosFileUpload", (i + 1)) + "' accept='application/pdf' value='10485760' />";

                    if (documento.Count - 1 >= i)
                    {
                        var FileName = Path.Combine(Server.MapPath("~/RH/Documentos"), String.Format("Documento-{0}.pdf", documento[(i)].Id));
                        if (System.IO.File.Exists(FileName))
                        {
                            documentos += "                     <span id='" + String.Format("ColaboradorDocumentos{0}DocumentosFileName", (i + 1)) + "'><a style='cursor: pointer;' onclick='VisualizarDocumentoPDF(" + '"' + String.Format("/RH/Documentos/Documento-{0}.pdf", documento[i].Id) + '"' + ");''>" + String.Format("Documento-{0}.pdf", documento[i].Id) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a id='" + String.Format("ButtonDocumentoPDF{0}Excluir", (i + 1)) + "' class='btn btn-danger btn-xs' onclick='ExcluirDocumentoPDF(1, " + (i + 1) + ")'>Apagar</a>&nbsp;&nbsp;&nbsp;<label id='" + String.Format("LabelDocumentoFile{0}ExcluirConfirmacao", (i + 1)) + "' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse PDF?&nbsp;&nbsp;&nbsp;&nbsp;</label><a id='" + String.Format("acceptDocumento1File{0}", (i + 1)) + "' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirDocumentoPDFSim(1, " + (i + 1) + ", " + '"' + String.Format("Documento-{0}.pdf", documento[i].Id.ToString()) + '"' + ")'>Sim</a>&nbsp;<a id='" + String.Format("go_backDocumento1File{0}", (i + 1)) + "' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirDocumentoPDFNao(1, " + (i + 1) + ")''>Não</a></span>";
                        }
                        else
                        {
                            documentos += "                     <span id='" + String.Format("ColaboradorDocumentos{0}DocumentosFileName", (i + 1)) + "'>Nenhum arquivo encontrado</span>";
                        }
                    }
                    else
                    {
                        documentos += "                     <span id='" + String.Format("ColaboradorDocumentos{0}DocumentosFileName", (i + 1)) + "'>Nenhum arquivo selecionado</span>";
                    }

                    documentos += "                  </div>";
                    documentos += "                  <label id='" + String.Format("ColaboradorDocumentos{0}FileUploadValidate", (i + 1)) + "' name='" + String.Format("ColaboradorDocumentos{0}FileUploadValidate", (i + 1)) + "' for='" + String.Format("ColaboradorDocumentos{0}FileUpload1", (i + 1)) + "' class='error hidden'>Por favor, informe um arquivo com até 10MB.</label>";
                    documentos += "               </div>";
                    documentos += "            </div>";
                    documentos += "         </div>";



                    documentos += "         <div class='row'>";
                    documentos += "            <div class='col-md-3'>";
                    documentos += "               <div class='form-group'>";
                    documentos += "                  <label class='titlefield'>Data de Emissão <span class='required'>*</span></label>";
                    documentos += "                  <input type='date' class='form-control' placeholder='Informe a Data de Emissão'  data-msg-required='Por favor, insira a Data de Emissão.' data-rule-required='true' id='" + String.Format("ColaboradorDocumentos{0}DataEmissao", (i + 1)) + "' name='ColaboradorDocumentosDataEmissao[]'  value='" + (documento.Count > i ? documento[i].DataEmissao.Value.ToString("yyyy-MM-dd") : String.Empty) + "' />";
                    documentos += "               </div>";
                    documentos += "            </div>";
                    documentos += "            <div class='col-md-3'>";
                    documentos += "               <div class='form-group'>";
                    documentos += "                  <label class='titlefield'>Data de Val. do documento <span class='required'>*</span></label>";
                    documentos += "                  <input type='date' data-msg-required='Por favor, insira a Val. do documento.' data-rule-required='true' class='form-control' placeholder='Informe a Data de Validade do documento'  id='" + String.Format("ColaboradorDocumentos{0}DataValidade", (i + 1)) + "' name='ColaboradorDocumentosDataValidade[]'  value='" + (documento.Count > i ? documento[i].DataValidade.Value.ToString("yyyy-MM-dd") : String.Empty) + "' />";
                    documentos += "               </div>";
                    documentos += "            </div>";
                    documentos += "            <div class='col-md-3'>";
                    documentos += "               <div class='form-group'>";
                    documentos += "                  <label class='titlefield'>Tipo de documento <span class='required'>*</span></label>";
                    documentos += "                  <select data-msg-required='Por favor, insira o Tipo de documento.' data-rule-required='true' id='" + String.Format("ColaboradorDocumentos{0}TipoDocumento", (i + 1)) + "' name='ColaboradorDocumentosTipoDocumento[]' class='form-control placeholder'>";
                    documentos += "                     <option value='' style='background-color: #eaeaea;'>Selecione o Tipo de Documento</option>";
                    documentos += "                     <option value='RG'" + (documento.Count >= (i + 1) ? (documento[i].TipoDocumento == "RG" ? " selected" : String.Empty) : String.Empty) + ">RG</option>";
                    documentos += "                     <option value='CPF'" + (documento.Count >= (i + 1) ? (documento[i].TipoDocumento == "CPF" ? " selected" : String.Empty) : String.Empty) + ">CPF</option>";
                    documentos += "                     <option value='OAB'" + (documento.Count >= (i + 1) ? (documento[i].TipoDocumento == "OAB" ? " selected" : String.Empty) : String.Empty) + ">OAB</option>";
                    documentos += "                     <option value='Diploma'" + (documento.Count >= (i + 1) ? (documento[i].TipoDocumento == "Diploma" ? " selected" : String.Empty) : String.Empty) + ">Diploma</option>";
                    documentos += "                     <option value='Outros'" + (documento.Count >= (i + 1) ? (documento[i].TipoDocumento == "Outros" ? " selected" : String.Empty) : String.Empty) + ">Outros</option>";
                    documentos += "                  </select>";
                    documentos += "               </div>";
                    documentos += "            </div>";
                    documentos += "            <div class='col-md-4'>";
                    documentos += "               <div class='form-group'>";
                    documentos += "                  <label class='titlefield'>Número Documento <span class='required'>*</span></label>";
                    documentos += "                  <input type='text' id='" + String.Format("ColaboradorDocumentos{0}NumeroDocumento", (i + 1)) + "' name='ColaboradorDocumentosNumeroDocumento[]' class='form-control' value='" + (documento.Count >= (i + 1) ? documento[i].NumeroDocumento : String.Empty) + "' />";
                    documentos += "               </div>";
                    documentos += "            </div>";
                    documentos += "         </div>";
                    documentos += "         <div class='row'>";
                    documentos += "            <div class='col-md-12'>";
                    documentos += "               <div class='form-group'>";
                    documentos += "                  <label class='titlefield'>Descrição <span class='red'>(Limite de 250 caracteres)</span></label>";
                    documentos += "                  <textarea class='form-control' placeholder='Informe uma Observação'id='" + String.Format("ColaboradorDocumentos{0}Observacao", (i + 1)) + "' name='ColaboradorDocumentosObservacao[]' rows='3' maxlength='250'>" + (documento.Count >= (i + 1) ? documento[i].Observacao : String.Empty) + "</textarea>";
                    documentos += "               </div>";
                    documentos += "            </div>";
                    documentos += "         </div>";

                    documentos += "            <div class='row'>";
                    documentos += "                <div class='col-md-3'>";
                    documentos += "                    <div class='form-group'>";
                    documentos += "                        <label class='titlefield'>Data Cadastro</label>";
                    documentos += "                        <input type='text' class='form-control' id='" + String.Format("ColaboradorDocumentos{0}DataCadastro", (i + 1)) + "' name='ColaboradorDocumentosDataCadastro[]' readonly  value='" + (documento.Count > i ? documento[i].DataCadastro.Value.ToString("yyyy-MM-dd") : String.Empty) + "'/>";
                    documentos += "                    </div>";
                    documentos += "                </div>";
                    documentos += "                <div class='col-md-3'>";
                    documentos += "                    <div class='form-group'>";
                    documentos += "                        <label class='titlefield'>Usuário Cadastro</label>";
                    documentos += "                        <input type='text' class='form-control' id='" + String.Format("ColaboradorDocumentos{0}UsuarioCadastro", (i + 1)) + "' name='ColaboradorDocumentosUsuarioCadastro[]' readonly  value='" + (documento.Count > i ? documento[i].UsuarioCadastro : String.Empty) + "'/>";
                    documentos += "                    </div>";
                    documentos += "                </div>";
                    documentos += "                <div class='col-md-3'>";
                    documentos += "                    <div class='form-group'>";
                    documentos += "                        <label class='titlefield'>Data Alteração</label>";
                    documentos += "                        <input type='text' class='form-control' id='" + String.Format("ColaboradorDocumentos{0}DataAlteracao", (i + 1)) + "' name='ColaboradorDocumentosDataAlteracao[]' readonly  value='" + (documento.Count > i ? documento[i].DataAlteracao.Value.ToString("yyyy-MM-dd") : String.Empty) + "'/>";
                    documentos += "                    </div>";
                    documentos += "                </div>";
                    documentos += "                <div class='col-md-3'>";
                    documentos += "                    <div class='form-group'>";
                    documentos += "                        <label class='titlefield'>Usuário Alteração</label>";
                    documentos += "                        <input type='text' class='form-control' id='" + String.Format("ColaboradorDocumentos{0}UsuarioAlteracao", (i + 1)) + "' name='ColaboradorDocumentosUsuarioAlteracao[]' readonly  value='" + (documento.Count > i ? documento[i].UsuarioAlteracao : String.Empty) + "'/>";
                    documentos += "                    </div>";
                    documentos += "                </div>";
                    documentos += "            </div>";


                    // Rodapé
                    documentos += "         <div class='row'>";
                    documentos += "            <div class='col-md-4'>";
                    documentos += "               <button type='button' id='" + String.Format("ButtonColaboradorDocumentos{0}Incluir", (i + 1)) + "' name='" + String.Format("ButtonColaboradorDocumentos{0}Incluir", (i + 1)) + "' class='btn btn-primary' data-loading-text='Salvando...' onclick='ButtonDocSalvar_Click(" + (i + 1) + ")'>";
                    documentos += "                  Salvar";
                    documentos += "               </button>";
                    documentos += "            </div>";
                    documentos += "            <div class='col-md-3' style='text-align: center;'>";
                    documentos += "            </div>";
                    documentos += "            <div class='col-md-5' style='text-align: right;'>";
                    documentos += "               <button type='button' id='" + String.Format("ButtonColaboradorDocumentos{0}Excluir", (i + 1)) + "' name='" + String.Format("ButtonColaboradorDocumentos{0}Excluir", (i + 1)) + "' class='btn btn-danger' data-loading-text='Apagando...' onclick='ButtonDocumentoExcluir_Click(" + (i + 1) + ");' " + (documento.Count >= (i + 1) ? String.Empty : "disabled='disabled'") + ">";
                    documentos += "                  Apagar";
                    documentos += "               </button>";
                    documentos += "               <div id='" + String.Format("ButtonColaboradorDocumentos{0}ExcluirConfirmacao", (i + 1)) + "' style='display:none'>";
                    documentos += "                  <label id='" + String.Format("LabelColaboradorDocumentos{0}ExcluirConfirmacao", (i + 1)) + "' style='font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente excluir esse documento?</label><br />";
                    documentos += "                  <button id='" + String.Format("acceptDocumento{0}", (i + 1)) + "' class='btn btn-default btn-xs' value='true'>Sim</button>";
                    documentos += "                  <button id='" + String.Format("go_backDocumento{0}", (i + 1)) + "' class='btn btn-primary btn-xs' value='false'>Não</button>";
                    documentos += "               </div>";
                    documentos += "            </div>";
                    documentos += "         </div>";

                    documentos += "      </div>";
                    documentos += "   </div>";
                    documentos += "</div>";

                    documentos += "<script>";
                    documentos += "    $('" + String.Format("#ColaboradorDocumentos{0}DocumentosFileUpload", (i + 1)) + "').change(function (e) {";
                    documentos += "        $in = $(this);";
                    documentos += "         $in.next().html($in.val().split('\\\\').pop());"; // Note o uso de barras duplas para escapar a barra invertida // $in.next().html($in.val());";
                    documentos += "    });";
                    documentos += "</script>";

                }

                var result = new
                {
                    response = "success",
                    documentos = documentos,
                    count = DocumentosQuantidade
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new
                {
                    response = "error"
                };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult SalvarDadosDocumentos(Int32 Id, Int32 IdProfissional, String TipoDocumento, String NumeroDocumento, String DataEmissao, String DataValidade, String Observacao, HttpPostedFileBase DocumentoArquivo)
        {

            var mensagemEmail = String.Empty;
            var mensagem = string.Empty;
            ProfissionaisDocumentos documento = new ProfissionaisDocumentos();
            documento.Id = Id;
            documento.IdProfissional = IdProfissional;

            documento.TipoDocumento = TipoDocumento;
            documento.NumeroDocumento = NumeroDocumento;

            if (DataEmissao != null && DataEmissao.Trim().Length > 0)
            {
                documento.DataEmissao = DateTime.Parse(DataEmissao);
            }

            if (DataValidade != null && DataValidade.Trim().Length > 0)
            {
                documento.DataValidade = DateTime.Parse(DataValidade);
            }

            documento.Observacao = Observacao.Trim();


            documento.DataCadastro = DateTime.Now;
            documento.UsuarioCadastro = Sessao.Usuario.Nome;
            documento.DataAlteracao = DateTime.Now;
            documento.UsuarioAlteracao = Sessao.Usuario.Nome;
            if (Id == 0)
            {
                Id = ProfissionaisDocumentosBo.Inserir(documento);
            }

            documento.Id = Id;
            ProfissionaisDocumentosBo.Salvar(documento);

            var url = String.Empty;
            if (DocumentoArquivo != null && DocumentoArquivo.ContentLength > 0)
            {
                var file = Path.Combine(Server.MapPath("~/RH/Documentos"), String.Format("Documento-{0}.pdf", Id.ToString()));
                DocumentoArquivo.SaveAs(file);
                mensagemEmail = " com Documento anexado";
            }

            var nomeProfissional = ProfissionaisBo.Consultar(IdProfissional);
            var titulo = nomeProfissional.Pessoas.Nome + "Incluiu um documento";
            mensagemEmail = "Documento " + TipoDocumento + " incluído " + mensagemEmail;
            var resposta = EnviarEmail(titulo, nomeProfissional.Pessoas.Nome, mensagemEmail);
            mensagem = "Documento Salvo com sucesso - " + resposta;

            var result = new { codigo = "00", mensagem = mensagem };

            return Json(result);
        }

        public JsonResult ExcluirDadosDocumentos(Int32 Id)
        {
            var codigo = ProfissionaisSolicitacoesBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult ExcluirDocumentoPDF(String Arquivo)
        {
            try
            {
                var file = Path.Combine(Server.MapPath("~/RH/Documentos"), Arquivo);
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }

                var result = new { response = "success" };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        // Contratos
        [HttpPost]
        public JsonResult CarregarDadosContratos(Int32 IdProfissional)
        {
            try
            {
                var contratos = ProfissionaisContratosBo.ListarContratos(IdProfissional);
                String htmlContratos = String.Empty;

                int ContratosQuantidade = (contratos.Count > 0 ? (contratos.Count + 1) : 1);

                for (int i = 0; i < ContratosQuantidade; i++)
                {
                    // Início
                    htmlContratos += "<input id='" + String.Format("HiddenDadoContrato{0}", (i + 1)) + "' name='HiddenDadoContrato[]' type='hidden' value='" + (contratos.Count >= (i + 1) ? contratos[i].Id.ToString() : "0") + "' />";
                    htmlContratos += "<div class='accordion panel panel-default'>";
                    htmlContratos += "   <div class='accordion-heading panel-heading' id='" + String.Format("DadoContrato{0}heading", (i + 1)) + "' role='tab'>";
                    htmlContratos += "      <h4 class='accordion-title panel-title'>";
                    htmlContratos += "         <a href='" + String.Format("#DadoContrato{0}collapse", (i + 1)) + "' aria-expanded='false' data-toggle='collapse' aria-controls='" + String.Format("DadoContrato{0}collapse", (i + 1)) + "' data-parent='#DadoContratoAccordion' role='button' class='collapsed'>";
                    htmlContratos += "            " + String.Format("Contrato #{0}", (i + 1));
                    htmlContratos += (contratos.Count >= (i + 1)
                        ? String.Format(" - {0} - {1}",
                            contratos[i].DataInicio.HasValue
                                ? contratos[i].DataInicio.Value.ToString("dd/MM/yyyy")
                                : "Data Início não informada",
                            contratos[i].TipoContrato ?? "Tipo não informado")
                        : String.Empty);
                    htmlContratos += "         </a>";

                    htmlContratos += "      </h4>";
                    htmlContratos += "   </div>";
                    htmlContratos += "   <div class='panel-collapse collapse' id='" + String.Format("DadoContrato{0}collapse", (i + 1)) + "' role='tabpanel' aria-labelledby='" + String.Format("DadoContrato{0}heading", (i + 1)) + "' aria-expanded='false' style='height: 0px;'>";
                    htmlContratos += "      <div class='accordion-body panel-body'>";
                    htmlContratos += "         <div class='row'>";
                    htmlContratos += "            <div class='col-md-3'>";
                    htmlContratos += "               <div class='form-group'>";
                    htmlContratos += "                  <label class='titlefield'>Data de Início <span class='required'>*</span></label>";
                    htmlContratos += "                  <input type='date' data-msg-required='Por favor, insira a Data de Início.' data-rule-required='true' id='" + String.Format("DadoContratoDataInicio{0}", (i + 1)) + "' name='DadoContratoDataInicio[]' class='form-control' placeholder='Informe a Data de Início' value='" + (contratos.Count > i ? contratos[i].DataInicio.Value.ToString("yyyy-MM-dd") : String.Empty) + "' />";
                    htmlContratos += "               </div>";
                    htmlContratos += "            </div>";
                    htmlContratos += "            <div class='col-md-3'>";
                    htmlContratos += "               <div class='form-group'>";
                    htmlContratos += "                  <label class='titlefield'>Data de Fim </label>";
                    htmlContratos += "                  <input type='date' id='" + String.Format("DadoContratoDataFim{0}", (i + 1)) + "' name='DadoContratoDataFim[]' class='form-control' placeholder='Informe a Data de Fim' value='" + (contratos.Count >= (i + 1) && contratos[i].DataFim.HasValue ? contratos[i].DataFim.Value.ToString("dd/MM/yyyy") : String.Empty) + "' />";
                    htmlContratos += "               </div>";
                    htmlContratos += "            </div>";
                    htmlContratos += "            <div class='col-md-6'>";
                    htmlContratos += "               <div class='form-group'>";
                    htmlContratos += "                  <label class='titlefield'>Tipo de Contrato <span class='required'>*</span></label>";
                    htmlContratos += "                  <select data-msg-required='Por favor, insira o Tipo de Contrato.' data-rule-required='true' id='" + String.Format("DadoContratoTipoContrato{0}", (i + 1)) + "' name='DadoContratoTipoContrato[]' class='form-control' data-msg-required='Por favor, selecione um tipo de contrato.' data-rule-required='true'>";
                    htmlContratos += "                     <option value='' style='background-color: #eaeaea;'>Selecione</option>";
                    htmlContratos += "                     <option value='CLT' " + (contratos.Count >= (i + 1) && contratos[i].TipoContrato == "CLT" ? "selected" : String.Empty) + ">CLT</option>";
                    htmlContratos += "                     <option value='Advogado' " + (contratos.Count >= (i + 1) && contratos[i].TipoContrato == "Advogado" ? "selected" : String.Empty) + ">Advogado</option>";
                    htmlContratos += "                  </select>";
                    htmlContratos += "               </div>";
                    htmlContratos += "            </div>";
                    htmlContratos += "         </div>";

                    htmlContratos += "         <div class='row'>";

                    //FileUploader
                    htmlContratos += "         <div class='row'>";
                    htmlContratos += "            <div class='col-md-12'>";
                    htmlContratos += "               <div class='form-group'>";
                    htmlContratos += "                  <label class='titlefield'>PDF Contrato<label class='titlefieldcomment'> <span class='red'>(tamanho máximo de 10MB por arquivo)</label></label>";
                    htmlContratos += "                  <div id='" + String.Format("DadoContrato{0}Upload", (i + 1)) + (contratos.Count == 0 ? 1 : contratos[0].Id) + "' class='upload'>";
                    htmlContratos += "                     <input type='button' class='uploadButton' value='Selecione o arquivo' onclick='$(" + '"' + String.Format("#DadoContrato{0}ContratoFileUpload", (i + 1)) + '"' + ").click();' />";
                    htmlContratos += "                     <input type='file' id='" + String.Format("DadoContrato{0}ContratoFileUpload", (i + 1)) + "' name='" + String.Format("DadoContrato{0}ContratoFileUpload", (i + 1)) + "' accept='application/pdf' value='10485760' />";

                    if (contratos.Count - 1 >= i)
                    {
                        var FileName = Path.Combine(Server.MapPath("~/RH/Contratos"), String.Format("Contrato-{0}.pdf", contratos[(i)].Id));
                        if (System.IO.File.Exists(FileName))
                        {
                            htmlContratos += "                     <span id='" + String.Format("DadoContrato{0}ContratoFileName", (i + 1)) + "'><a style='cursor: pointer;' onclick='VisualizarContratoPDF(" + '"' + String.Format("/RH/Contratos/Contrato-{0}.pdf", contratos[i].Id) + '"' + ");''>" + String.Format("Contrato-{0}.pdf", contratos[i].Id) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a id='" + String.Format("ButtonContratoPDF{0}Excluir", (i + 1)) + "' class='btn btn-danger btn-xs' onclick='ExcluirContratoPDF(1, " + (i + 1) + ")'>Apagar</a>&nbsp;&nbsp;&nbsp;<label id='" + String.Format("LabelContratoFile{0}ExcluirConfirmacao", (i + 1)) + "' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse PDF?&nbsp;&nbsp;&nbsp;&nbsp;</label><a id='" + String.Format("acceptContrato1File{0}", (i + 1)) + "' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirContratoPDFSim(1, " + (i + 1) + ", " + '"' + String.Format("Contrato-{0}.pdf", contratos[i].Id.ToString()) + '"' + ")'>Sim</a>&nbsp;<a id='" + String.Format("go_backContrato1File{0}", (i + 1)) + "' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirContratoPDFNao(1, " + (i + 1) + ")''>Não</a></span>";
                        }
                        else
                        {
                            htmlContratos += "                     <span id='" + String.Format("DadoContrato{0}ContratoFileName", (i + 1)) + "'>Nenhum arquivo encontrado</span>";
                        }
                    }
                    else
                    {
                        htmlContratos += "                     <span id='" + String.Format("DadoContrato{0}ContratoFileName", (i + 1)) + "'>Nenhum arquivo selecionado</span>";
                    }

                    htmlContratos += "                  </div>";
                    htmlContratos += "                  <label id='" + String.Format("DadoContrato{0}FileUploadValidate", (i + 1)) + "' name='" + String.Format("DadoContrato{0}FileUploadValidate", (i + 1)) + "' for='" + String.Format("DadoContrato{0}FileUpload1", (i + 1)) + "' class='error hidden'>Por favor, informe um arquivo com até 10MB.</label>";
                    htmlContratos += "               </div>";
                    htmlContratos += "            </div>";
                    htmlContratos += "         </div>";

                    htmlContratos += "            <div class='row'>";
                    htmlContratos += "                <div class='col-md-3'>";
                    htmlContratos += "                    <div class='form-group'>";
                    htmlContratos += "                        <label class='titlefield'>Data Cadastro</label>";
                    htmlContratos += "                        <input type='text' class='form-control' id='" + String.Format("DadoContratoDataCadastro{0}", (i + 1)) + "' name='DadoContratoDataCadastro[]' readonly   value='" + (contratos.Count > i ? contratos[i].DataCadastro.Value.ToString("yyyy-MM-dd") : String.Empty) + "'/>";
                    htmlContratos += "                    </div>";
                    htmlContratos += "                </div>";
                    htmlContratos += "                <div class='col-md-3'>";
                    htmlContratos += "                    <div class='form-group'>";
                    htmlContratos += "                        <label class='titlefield'>Usuário Cadastro</label>";
                    htmlContratos += "                        <input type='text' class='form-control'  id='" + String.Format("DadoContratoUsuarioCadastro{0}", (i + 1)) + "' name='DadoContratoUsuarioCadastro[]' readonly   value='" + (contratos.Count > i ? contratos[i].UsuarioCadastro : String.Empty) + "'/>";
                    htmlContratos += "                    </div>";
                    htmlContratos += "                </div>";
                    htmlContratos += "                <div class='col-md-3'>";
                    htmlContratos += "                    <div class='form-group'>";
                    htmlContratos += "                        <label class='titlefield'>Data Alteração</label>";
                    htmlContratos += "                        <input type='text' class='form-control'  id='" + String.Format("DadoContratoDataAlteracao{0}", (i + 1)) + "' name='DadoContratoDataAlteracao[]' readonly   value='" + (contratos.Count > i ? contratos[i].DataAlteracao.Value.ToString("yyyy-MM-dd") : String.Empty) + "'/>";
                    htmlContratos += "                    </div>";
                    htmlContratos += "                </div>";
                    htmlContratos += "                <div class='col-md-3'>";
                    htmlContratos += "                    <div class='form-group'>";
                    htmlContratos += "                        <label class='titlefield'>Usuário Alteração</label>";
                    htmlContratos += "                        <input type='text' class='form-control'  id='" + String.Format("DadoContratoUsuarioAlteracao{0}", (i + 1)) + "' name='DadoContratoUsuarioAlteracao[]' readonly  value='" + (contratos.Count > i ? contratos[i].UsuarioAlteracao : String.Empty) + "'/>";
                    htmlContratos += "                    </div>";
                    htmlContratos += "                </div>";
                    htmlContratos += "            </div>";



                    htmlContratos += "         <div class='row'>";
                    htmlContratos += "            <div class='col-md-4'>";
                    htmlContratos += "               <button type='button' id='" + String.Format("ButtonDadoContrato{0}Incluir", (i + 1)) + "' name='" + String.Format("ButtonDadoContrato{0}Incluir", (i + 1)) + "' class='btn btn-primary' data-loading-text='Salvando...' onclick='ButtonDadoContratoSalvar_Click(" + (i + 1) + ");'>";
                    htmlContratos += "                  Salvar";
                    htmlContratos += "               </button>";
                    htmlContratos += "            </div>";
                    htmlContratos += "            <div class='col-md-3' style='text-align: center;'>";
                    htmlContratos += "            </div>";
                    htmlContratos += "            <div class='col-md-5' style='text-align: right;'>";
                    htmlContratos += "               <button type='button' id='" + String.Format("ButtonDadoContrato{0}Excluir", (i + 1)) + "' name='" + String.Format("ButtonDadoContrato{0}Excluir", (i + 1)) + "' class='btn btn-danger' data-loading-text='Apagando...' onclick='ButtonDadoContratoExcluir_Click(" + (i + 1) + ");' " + (contratos.Count >= (i + 1) ? String.Empty : "disabled='disabled'") + ">";
                    htmlContratos += "                  Apagar";
                    htmlContratos += "               </button>";
                    htmlContratos += "               <div id='" + String.Format("ButtonDadoContrato{0}ExcluirConfirmacao", (i + 1)) + "' style='display:none'>";
                    htmlContratos += "                  <label id='" + String.Format("LabelDadoContrato{0}ExcluirConfirmacao", (i + 1)) + "' style='font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente excluir este Contrato?</label><br />";
                    htmlContratos += "                  <button id='" + String.Format("acceptContrato{0}", (i + 1)) + "' class='btn btn-default btn-xs' value='true'>Sim</button>";
                    htmlContratos += "                  <button id='" + String.Format("go_backContrato{0}", (i + 1)) + "' class='btn btn-primary btn-xs' value='false'>Não</button>";
                    htmlContratos += "               </div>";
                    htmlContratos += "            </div>";
                    htmlContratos += "         </div>";
                    htmlContratos += "      </div>";
                    htmlContratos += "   </div>";
                    htmlContratos += "</div>";

                    htmlContratos += "<script>";
                    htmlContratos += "    $('" + String.Format("#DadoContrato{0}ContratoFileUpload", (i + 1)) + "').change(function (e) {";
                    htmlContratos += "        $in = $(this);";
                    htmlContratos += "         $in.next().html($in.val().split('\\\\').pop());"; // Note o uso de barras duplas para escapar a barra invertida // $in.next().html($in.val());";
                    htmlContratos += "    });";
                    htmlContratos += "</script>";


                }

                var result = new { response = "success", contratos = htmlContratos, count = ContratosQuantidade };

                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new
                {
                    response = "error"
                };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult SalvarDadosContratos(Int32 Id, Int32 IdProfissional, String DataInicio, String DataFim, String TipoContrato, HttpPostedFileBase ContratoArquivo)
        {
            try
            {

                var mensagemEmail = String.Empty;
                var mensagem = string.Empty;
                ProfissionaisContratos contrato = new ProfissionaisContratos();

                contrato.Id = Id;
                contrato.IdProfissional = IdProfissional;

                if (DataInicio != null && DataInicio.Trim().Length > 0)
                {
                    contrato.DataInicio = DateTime.Parse(DataInicio);
                }

                if (DataFim != null && DataFim.Trim().Length > 0)
                {
                    contrato.DataFim = DateTime.Parse(DataFim);
                }

                contrato.TipoContrato = TipoContrato;
                // contrato.IdSede = IdSede;
                contrato.DataCadastro = DateTime.Now;
                contrato.UsuarioCadastro = Sessao.Usuario.Nome;
                contrato.DataAlteracao = DateTime.Now;
                contrato.UsuarioAlteracao = Sessao.Usuario.Nome;

                if (Id == 0)
                    Id = ProfissionaisContratosBo.Inserir(contrato);

                contrato.Id = Id;
                ProfissionaisContratosBo.Salvar(contrato);

                var url = String.Empty;
                if (ContratoArquivo != null && ContratoArquivo.ContentLength > 0)
                {
                    var file = Path.Combine(Server.MapPath("~/RH/Contratos"), String.Format("Contrato-{0}.pdf", Id.ToString()));
                    ContratoArquivo.SaveAs(file);
                }

                var result = new
                {
                    response = "success",
                    dados = contrato
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                String error = "error";
                String message = ex.InnerException?.ToString() ?? ex.Message;

                if (message.Contains("chave duplicada no objeto"))
                    error = "warning";

                var result = new
                {
                    response = error,
                    message = ex.Message
                };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult ExcluirDadosContratos(Int32 Id)
        {
            try
            {
                ProfissionaisContratosBo.Excluir(Id);

                var result = new
                {
                    response = "removed"
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new
                {
                    response = "error"
                };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult ExcluirContratoPDF(String Arquivo)
        {
            try
            {
                var file = Path.Combine(Server.MapPath("~/RH/Contratos"), Arquivo);
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }

                var result = new { response = "success" };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        // Solicitações
        [HttpPost]
        public JsonResult CarregarDadosSolicitacoes(Int32 IdProfissional)
        {
            try
            {
                var solicitacoes = ProfissionaisSolicitacoesEspeciaisBo.ListarSolicitacoesEspeciais(IdProfissional);
                String solicitacoesHtml = String.Empty;

                int solicitacoesQuantidade = (solicitacoes.Count > 0 ? (solicitacoes.Count + 1) : 1);

                for (int i = 0; i < solicitacoesQuantidade; i++)
                {

                    // Início
                    solicitacoesHtml += "<input id='" + String.Format("HiddenSolicitacao{0}", (i + 1)) + "' name='HiddenSolicitacao[]' type='hidden' value='" + (solicitacoes.Count > i ? solicitacoes[i].Id.ToString() : "0") + "' />";
                    solicitacoesHtml += "<div class='accordion panel panel-default'>";
                    solicitacoesHtml += "   <div class='accordion-heading panel-heading' id='" + String.Format("Solicitacao{0}heading", (i + 1)) + "' role='tab'>";



                    solicitacoesHtml += "      <h4 class='accordion-title panel-title'>";
                    solicitacoesHtml += "         <a href='" + String.Format("#Solicitacao{0}collapse", (i + 1)) + "' aria-expanded='false' data-toggle='collapse' aria-controls='" + String.Format("Solicitacao{0}collapse", (i + 1)) + "' data-parent='#DadosolicitacoesAccordion' role='button' class='collapsed'>";
                    solicitacoesHtml += "            " + String.Format("Solicitação #{0}", (i + 1));

                    // Adicionar os detalhes de Data, Data de Aprovação e Status
                    if (solicitacoes.Count > i)
                    {
                        string data = solicitacoes[i].Data.HasValue ? solicitacoes[i].Data.Value.ToString("dd/MM/yyyy") : "N/A";
                        string dataAprovacao = solicitacoes[i].DataAprovacao.HasValue ? solicitacoes[i].DataAprovacao.Value.ToString("dd/MM/yyyy") : "N/A";
                        string status = !string.IsNullOrEmpty(solicitacoes[i].Status) ? solicitacoes[i].Status : "Não especificado";

                        solicitacoesHtml += $" - Pedido: {data} - {status} em {dataAprovacao}";
                    }

                    solicitacoesHtml += "         </a>";
                    solicitacoesHtml += "      </h4>";





                    solicitacoesHtml += "   </div>";
                    solicitacoesHtml += "   <div class='panel-collapse collapse' id='" + String.Format("Solicitacao{0}collapse", (i + 1)) + "' role='tabpanel'  aria-labelledby='" + String.Format("Solicitacao{0}heading", (i + 1)) + "' aria-expanded='false' style='height: 0px;'>";
                    solicitacoesHtml += "      <div class='accordion-body panel-body'>";

                    // Formulário
                    solicitacoesHtml += "         <div class='row'>";
                    solicitacoesHtml += "            <div class='col-md-4'>";
                    solicitacoesHtml += "               <div class='form-group'>";
                    solicitacoesHtml += "                  <label class='titlefield'>Data da Solicitação <span class='required'>*</span></label>";
                    solicitacoesHtml += "                  <input type='date'  data-msg-required='Por favor, insira a Data da Solicitação.' data-rule-required='true' class='form-control' placeholder='Informe a Data' id='" + String.Format("ColaboradorDadoSolicitacao{0}DataSolicitacao", (i + 1)) + "' name='ColaboradorDadoSolicitacaoDataSolicitacao[]' value='" + (solicitacoes.Count > i ? solicitacoes[i].Data.Value.ToString("yyyy-MM-dd") : String.Empty) + "' readonly />";
                    solicitacoesHtml += "               </div>";
                    solicitacoesHtml += "            </div>";
                    solicitacoesHtml += "            <div class='col-md-4'>";
                    solicitacoesHtml += "               <div class='form-group'>";
                    solicitacoesHtml += "                  <label class='titlefield'>Data da Aprovação/Reprovação <span class='required'>*</span></label>";
                    //  id='" + String.Format("DadoContratoDataInicio{0}", (i + 1)) + "' name='DadoContratoDataInicio[]'
                    solicitacoesHtml += "                  <input type='date' data-msg-required='Por favor, insira a Data da Aprovação/Reprovação.' data-rule-required='true' class='form-control' placeholder='Informe a Data' id='" + String.Format("ColaboradorDadoSolicitacao{0}DataAprovReprov", (i + 1)) + "' name='ColaboradorDadoSolicitacaoDataAprovReprov[]' value='" + (solicitacoes.Count > i ? solicitacoes[i].DataAprovacao.Value.ToString("yyyy-MM-dd") : String.Empty) + "' readonly />";
                    solicitacoesHtml += "               </div>";
                    solicitacoesHtml += "            </div>";
                    solicitacoesHtml += "            <div class='col-md-4'>";
                    solicitacoesHtml += "               <div class='form-group'>";
                    solicitacoesHtml += "                  <label class='titlefield'>Status <span class='required'>*</span></label>";
                    solicitacoesHtml += "                  <select  id='" + String.Format("ColaboradorDadoSolicitacao{0}Status", (i + 1)) + "' name='ColaboradorDadoSolicitacaoStatus[]' class='form-control placeholder'  data-msg-required='Por favor, insira o Status.' data-rule-required='true'>";
                    solicitacoesHtml += "                     <option value='' style='background-color: #eaeaea;'>Selecione o Status</option>";
                    solicitacoesHtml += "                     <option value='Em Análise'" + (solicitacoes.Count > i && solicitacoes[i].Status == "Em Análise" ? " selected" : String.Empty) + ">Em Análise</option>";
                    solicitacoesHtml += "                     <option value='Aprovada'" + (solicitacoes.Count > i && solicitacoes[i].Status == "Aprovada" ? " selected" : String.Empty) + ">Aprovada</option>";
                    solicitacoesHtml += "                     <option value='Reprovada'" + (solicitacoes.Count > i && solicitacoes[i].Status == "Reprovada" ? " selected" : String.Empty) + ">Reprovada</option>";
                    solicitacoesHtml += "                  </select>";
                    solicitacoesHtml += "               </div>";
                    solicitacoesHtml += "            </div>";
                    solicitacoesHtml += "         </div>";

                    solicitacoesHtml += "         <div class='row'>";
                    solicitacoesHtml += "            <div class='col-md-12'>";
                    solicitacoesHtml += "               <div class='form-group'>";
                    solicitacoesHtml += "                  <label class='titlefield'>Solicitação <span class='required'>*</span> <span class='red'>(Limite de 250 caracteres)</span></label>";
                    solicitacoesHtml += "                  <textarea class='form-control' placeholder='Informe a Solicitacao'  id='" + String.Format("ColaboradorDadoSolicitacao{0}Solicitacao", (i + 1)) + "' name='ColaboradorDadoSolicitacaoSolicitacao[]' rows='3' maxlength='250'  data-msg-required='Por favor, insira a Solicitação.' data-rule-required='true'>" + (solicitacoes.Count > i ? solicitacoes[i].Solicitacao : String.Empty) + "</textarea>";
                    solicitacoesHtml += "               </div>";
                    solicitacoesHtml += "            </div>";
                    solicitacoesHtml += "         </div>";

                    solicitacoesHtml += "         <div class='row'>";
                    solicitacoesHtml += "            <div class='col-md-12'>";
                    solicitacoesHtml += "               <div class='form-group'>";
                    solicitacoesHtml += "                  <label class='titlefield'>Justificativa da aprovação/reprovação <span class='red'>(Limite de 250 caracteres)</span></label>";
                    solicitacoesHtml += "                  <textarea class='form-control' placeholder='Informe uma justificativa' id='" + String.Format("ColaboradorDadoSolicitacao{0}Justificativa", (i + 1)) + "' name='ColaboradorDadoSolicitacaoJustificativa[]' rows='3' maxlength='250' data-msg-required='Por favor, insira o Status.' data-rule-required='true'>" + (solicitacoes.Count > i ? solicitacoes[i].Justificativa : String.Empty) + "</textarea>";
                    solicitacoesHtml += "               </div>";
                    solicitacoesHtml += "            </div>";
                    solicitacoesHtml += "         </div>";


                    solicitacoesHtml += "            <div class='row'>";
                    solicitacoesHtml += "                <div class='col-md-3'>";
                    solicitacoesHtml += "                    <div class='form-group'>";
                    solicitacoesHtml += "                        <label class='titlefield'>Data Cadastro</label>";
                    solicitacoesHtml += "                        <input type='text' class='form-control' id='" + String.Format("ColaboradorDadoSolicitacao{0}DataCadastro", (i + 1)) + "' name='ColaboradorDadoSolicitacaoDataCadastro[]' readonly  value='" + (solicitacoes.Count > i ? solicitacoes[i].DataCadastro.Value.ToString("yyyy-MM-dd") : String.Empty) + "'/>";
                    solicitacoesHtml += "                    </div>";
                    solicitacoesHtml += "                </div>";
                    solicitacoesHtml += "                <div class='col-md-3'>";
                    solicitacoesHtml += "                    <div class='form-group'>";
                    solicitacoesHtml += "                        <label class='titlefield'>Usuário Cadastro</label>";
                    solicitacoesHtml += "                        <input type='text' class='form-control' id='" + String.Format("ColaboradorDadoSolicitacao{0}UsuarioCadastro", (i + 1)) + "' name='ColaboradorDadoSolicitacaoUsuarioCadastro[]' readonly  value='" + (solicitacoes.Count > i ? solicitacoes[i].UsuarioCadastro : String.Empty) + "'/>";
                    solicitacoesHtml += "                    </div>";
                    solicitacoesHtml += "                </div>";
                    solicitacoesHtml += "                <div class='col-md-3'>";
                    solicitacoesHtml += "                    <div class='form-group'>";
                    solicitacoesHtml += "                        <label class='titlefield'>Data Alteração</label>";
                    solicitacoesHtml += "                        <input type='text' class='form-control' id='" + String.Format("ColaboradorDadoSolicitacao{0}DataAlteracao", (i + 1)) + "' name='ColaboradorDadoSolicitacaoDataAlteracao[]' readonly  value='" + (solicitacoes.Count > i ? solicitacoes[i].DataAlteracao.Value.ToString("yyyy-MM-dd") : String.Empty) + "'/>";
                    solicitacoesHtml += "                    </div>";
                    solicitacoesHtml += "                </div>";
                    solicitacoesHtml += "                <div class='col-md-3'>";
                    solicitacoesHtml += "                    <div class='form-group'>";
                    solicitacoesHtml += "                        <label class='titlefield'>Usuário Alteração</label>";
                    solicitacoesHtml += "                        <input type='text' class='form-control' id='" + String.Format("ColaboradorDadoSolicitacao{0}UsuarioAlteracao", (i + 1)) + "' name='ColaboradorDadoSolicitacaoUsuarioAlteracao[]' readonly  value='" + (solicitacoes.Count > i ? solicitacoes[i].UsuarioAlteracao : String.Empty) + "'/>";
                    solicitacoesHtml += "                    </div>";
                    solicitacoesHtml += "                </div>";
                    solicitacoesHtml += "            </div>";

                    // Rodapé  
                    solicitacoesHtml += "         <div class='row'>";
                    solicitacoesHtml += "            <div class='col-md-4'>";
                    solicitacoesHtml += "               <button type='button' id='ButtonSolicitacao" + (i + 1) + "Incluir' name='ButtonSolicitacao" + (i + 1) + "Incluir' class='btn btn-primary' data-loading-text='Salvando...' onclick='ButtonSolicitacaoSalvar_Click(" + (i + 1) + ");'>";
                    solicitacoesHtml += "                  Salvar";
                    solicitacoesHtml += "               </button>";
                    solicitacoesHtml += "            </div>";
                    solicitacoesHtml += "            <div class='col-md-3' style='text-align: center;'>";
                    solicitacoesHtml += "            </div>";
                    solicitacoesHtml += "            <div class='col-md-5' style='text-align: right;'>";
                    solicitacoesHtml += "               <button type='button' id='" + String.Format("ButtonSolicitacao{0}Excluir", (i + 1)) + "' name='" + String.Format("ButtonSolicitacao{0}Excluir", (i + 1)) + "' class='btn btn-danger' data-loading-text='Apagando...' onclick='ButtonSolicitacaoExcluir_Click(" + (i + 1) + ");' " + (solicitacoes.Count > i ? String.Empty : "disabled='disabled'") + ">";
                    solicitacoesHtml += "                  Apagar";
                    solicitacoesHtml += "               </button>";
                    solicitacoesHtml += "               <div id='" + String.Format("ButtonSolicitacao{0}ExcluirConfirmacao", (i + 1)) + "' style='display:none'>";
                    solicitacoesHtml += "                  <label id='" + String.Format("LabelSolicitacao{0}ExcluirConfirmacao", (i + 1)) + "' style='font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente excluir essa solicitação?</label><br />";
                    solicitacoesHtml += "                  <button id='" + String.Format("accept{0}", (i + 1)) + "' class='btn btn-default btn-xs' value='true'>Sim</button>";
                    solicitacoesHtml += "                  <button id='" + String.Format("go_back{0}", (i + 1)) + "' class='btn btn-primary btn-xs' value='false'>Não</button>";
                    solicitacoesHtml += "               </div>";
                    solicitacoesHtml += "            </div>";
                    solicitacoesHtml += "         </div>";

                    solicitacoesHtml += "      </div>";
                    solicitacoesHtml += "   </div>";
                    solicitacoesHtml += "</div>";
                }

                var result = new
                {
                    response = "success",
                    solicitacoes = solicitacoesHtml,
                    count = solicitacoesQuantidade
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new
                {
                    response = "error",
                    message = ex.Message
                };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult SalvarDadosSolicitacoesEspeciais(Int64 Id, Int32 IdProfissional, string Data, string Solicitacao, string DataAprovacao, string Status, string Justificativa)
        {

            try
            {
                var mensagemEmail = String.Empty;
                var mensagem = string.Empty;

                var solicitacao = new ProfissionaisSolicitacoesEspeciais();
                solicitacao.Id = Id;
                solicitacao.IdProfissional = IdProfissional;

                if (Data != null && Data.Trim().Length > 0)
                {
                    solicitacao.Data = DateTime.Parse(Data);
                }

                solicitacao.Solicitacao = Solicitacao;

                if (DataAprovacao != null && DataAprovacao.Trim().Length > 0)
                {
                    solicitacao.DataAprovacao = DateTime.Parse(DataAprovacao);
                }

                solicitacao.Status = Status;
                solicitacao.Justificativa = Justificativa;

                solicitacao.DataCadastro = DateTime.Now;
                solicitacao.UsuarioCadastro = Sessao.Usuario.Nome;
                solicitacao.DataAlteracao = DateTime.Now;
                solicitacao.UsuarioAlteracao = Sessao.Usuario.Nome;

                if (Id == 0)
                    Id = ProfissionaisSolicitacoesEspeciaisBo.Inserir(solicitacao);

                solicitacao.Id = Id;

                ProfissionaisSolicitacoesEspeciaisBo.Salvar(solicitacao);

                var nomeProfissional = ProfissionaisBo.Consultar(IdProfissional);
                var titulo = nomeProfissional.Pessoas.Nome + "registrou ausencia";
                mensagemEmail = "Solicitação de Materiais e Afins no dia" + Data + mensagemEmail;
                var resposta = EnviarEmail(titulo, nomeProfissional.Pessoas.Nome, mensagemEmail);

                return Json(new
                {
                    response = "success",
                    dados = solicitacao
                });
            }
            catch (Exception ex)
            {

                String error = "error";
                String message = ex.InnerException?.ToString() ?? ex.Message;

                if (message.Contains("chave duplicada no objeto"))
                    error = "warning";

                return Json(new
                {
                    response = "error",
                    message = ex.Message
                });
            }

            //try
            //{
            //    var solicitacao = new ProfissionaisSolicitacoesEspeciais();
            //    solicitacao.Id = Id;
            //    solicitacao.IdProfissional = IdProfissional;

            //    if (Data != null && Data.Trim().Length > 0)
            //    {
            //        solicitacao.Data = DateTime.Parse(Data);
            //    }

            //    solicitacao.Solicitacao = Solicitacao;

            //    if (DataAprovacao != null && DataAprovacao.Trim().Length > 0)
            //    {
            //        solicitacao.DataAprovacao = DateTime.Parse(DataAprovacao);
            //    }

            //    solicitacao.Status = Status;
            //    solicitacao.Justificativa = Justificativa;

            //    if (Id == 0)
            //        Id = ProfissionaisSolicitacoesEspeciaisBo.Inserir(solicitacao);

            //    solicitacao.Id = Id;

            //    ProfissionaisSolicitacoesEspeciaisBo.Salvar(solicitacao);

            //    return Json(new
            //    {
            //        response = "success",
            //        dados = solicitacao
            //    });
            //}
            //catch (Exception ex)
            //{

            //    String error = "error";
            //    String message = ex.InnerException?.ToString() ?? ex.Message;

            //    if (message.Contains("chave duplicada no objeto"))
            //        error = "warning";

            //    return Json(new
            //    {
            //        response = "error",
            //        message = ex.Message
            //    });
            //}
        }

        [HttpPost]
        public JsonResult ExcluirDadosSolicitacoesEspeciais(Int64 Id)
        {
            try
            {
                ProfissionaisSolicitacoesEspeciaisBo.Excluir(Id);

                var result = new { response = "removed" };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new
                {
                    response = "error",
                    message = ex.Message
                };
                return Json(result);
            }
        }

        // Avaliações 
        [HttpPost]
        public JsonResult CarregarDadosAvaliacao(Int32 IdProfissional)
        {
            try
            {
                var avaliacoes = ProfissionaisAvaliacoesBo.ListarAAvaliacoes(IdProfissional);
                String htmlAvaliacoes = String.Empty;

                int quantidadeAvaliacoes = avaliacoes.Count > 0 ? (avaliacoes.Count + 1) : 1;

                for (int i = 0; i < quantidadeAvaliacoes; i++)
                {
                    // Style
                    htmlAvaliacoes += "<style>";
                    htmlAvaliacoes += "    div." + String.Format("DadoAvaliacoes{0}Upload", (i + 1)) + " {";
                    htmlAvaliacoes += "        background-color: #fff;";
                    htmlAvaliacoes += "        border: 1px solid #ccc;";
                    htmlAvaliacoes += "        display: inline-block;";
                    htmlAvaliacoes += "        height: 38px;";
                    htmlAvaliacoes += "        padding: 3px 40px 3px 3px;";
                    htmlAvaliacoes += "        position: relative;";
                    htmlAvaliacoes += "        width: 100% !important;";
                    htmlAvaliacoes += "    }";
                    htmlAvaliacoes += "        div." + String.Format("DadoAvaliacoes{0}Upload", (i + 1)) + ":hover {";
                    htmlAvaliacoes += "            opacity: 0.95;";
                    htmlAvaliacoes += "        }";
                    htmlAvaliacoes += "        div." + String.Format("DadoAvaliacoes{0}Upload", (i + 1)) + " input[type='file'] {";
                    htmlAvaliacoes += "            display: input-block;";
                    htmlAvaliacoes += "            width: 100%;";
                    htmlAvaliacoes += "            height: 30px;";
                    htmlAvaliacoes += "            opacity: 0;";
                    htmlAvaliacoes += "            cursor: pointer;";
                    htmlAvaliacoes += "            position: absolute;";
                    htmlAvaliacoes += "            left: 0;";
                    htmlAvaliacoes += "        }";
                    htmlAvaliacoes += "    ." + String.Format("DadoAvaliacoes{0}UploadButton", (i + 1)) + " {";
                    htmlAvaliacoes += "        background-color: #333333;";
                    htmlAvaliacoes += "        border: none;";
                    htmlAvaliacoes += "        color: #FFF;";
                    htmlAvaliacoes += "        cursor: pointer;";
                    htmlAvaliacoes += "        display: inline-block;";
                    htmlAvaliacoes += "        height: 30px;";
                    htmlAvaliacoes += "        margin-right: 15px;";
                    htmlAvaliacoes += "        width: auto;";
                    htmlAvaliacoes += "        padding: 0 20px;";
                    htmlAvaliacoes += "        box-sizing: content-box;";
                    htmlAvaliacoes += "    }";
                    htmlAvaliacoes += "        ." + String.Format("DadoAvaliacoes{0}UploadButton", (i + 1)) + ":hover {";
                    htmlAvaliacoes += "            background-color: #3D3D3D;";
                    htmlAvaliacoes += "        }";
                    htmlAvaliacoes += "    ." + String.Format("DadoAvaliacoes{0}FileName", (i + 1)) + " {";
                    htmlAvaliacoes += "        font-family: Arial;";
                    htmlAvaliacoes += "        font-size: 14px;";
                    htmlAvaliacoes += "    }";
                    htmlAvaliacoes += "    ." + String.Format("DadoAvaliacoes{0}Upload", (i + 1)) + " + ." + String.Format("DadoAvaliacoes{0}UploadButton", (i + 1)) + " {";
                    htmlAvaliacoes += "        height: 40px;";
                    htmlAvaliacoes += "    }";
                    htmlAvaliacoes += "</style>";

                    htmlAvaliacoes += "<input id='" + String.Format("HiddenDadoAvaliacao{0}", (i + 1)) + "' name='HiddenDadoAvaliacao[]' type='hidden' value='" + (avaliacoes.Count >= (i + 1) ? avaliacoes[i].Id.ToString() : "0") + "' />";
                    htmlAvaliacoes += "<div class='accordion panel panel-default'>";
                    htmlAvaliacoes += "   <div class='accordion-heading panel-heading' id='" + String.Format("DadoAvaliacao{0}heading", (i + 1)) + "' role='tab'>";
                    htmlAvaliacoes += "      <h4 class='accordion-title panel-title'>";
                    htmlAvaliacoes += "          <a href='#Avaliacao" + (i + 1) + "collapse' aria-expanded='false' data-toggle='collapse' aria-controls='Avaliacao" + (i + 1) + "collapse' data-parent='#AvaliacaoAccordion' role='button' class='collapsed'>";
                    htmlAvaliacoes += "            " + "Avaliação #" + (i + 1);
                    htmlAvaliacoes += (avaliacoes.Count >= (i + 1)
                        ? $" - {avaliacoes[i].Avaliador ?? "Avaliador não informado"} - {avaliacoes[i].Data?.ToString("dd/MM/yyyy") ?? "Data não informada"}"
                        : string.Empty);
                    htmlAvaliacoes += "         </a>";

                    htmlAvaliacoes += "      </h4>";
                    htmlAvaliacoes += "   </div>";
                    htmlAvaliacoes += "   <div class='panel-collapse collapse' id='Avaliacao" + (i + 1) + "collapse' role='tabpanel' aria-labelledby='Avaliacao" + (i + 1) + "heading' aria-expanded='false' style='height: 0px;'>";
                    htmlAvaliacoes += "      <div class='accordion-body panel-body'>";


                    htmlAvaliacoes += "         <div class='row'>";
                    htmlAvaliacoes += "            <div class='col-md-6'>";
                    htmlAvaliacoes += "               <div class='form-group'>";
                    htmlAvaliacoes += "                  <label class='titlefield'>Data <span class='required'>*</span></label>";
                    htmlAvaliacoes += "                  <input type='date' data-msg-required='Por favor, insira a Data.' data-rule-required='true' id='" + String.Format("DadoAvaliacao{0}DataAvaliacao", (i + 1)) + "' name='DadoAvaliacoesDataAvaliacao[]' class='form-control' placeholder='Informe a Data'  value='" + (avaliacoes.Count > i ? avaliacoes[i].Data.Value.ToString("yyyy-MM-dd") : String.Empty) + "' />";
                    htmlAvaliacoes += "               </div>";
                    htmlAvaliacoes += "            </div>";
                    htmlAvaliacoes += "            <div class='col-md-6'>";
                    htmlAvaliacoes += "               <div class='form-group'>";
                    htmlAvaliacoes += "                  <label class='titlefield'>Nome do Avaliador <span class='required'>*</span></label>";
                    htmlAvaliacoes += "                  <input type='text' data-msg-required='Por favor, insira o Avaliador.' data-rule-required='true' id='" + String.Format("DadoAvaliacao{0}Avaliador", (i + 1)) + "' name='DadoAvaliacaoAvaliador[]' class='form-control' placeholder='Informe o Avaliador' value='" + (avaliacoes.Count >= (i + 1) ? avaliacoes[i].Avaliador : string.Empty) + "' />";
                    htmlAvaliacoes += "               </div>";
                    htmlAvaliacoes += "            </div>";
                    htmlAvaliacoes += "         </div>";
                    htmlAvaliacoes += "         <div class='row'>";
                    htmlAvaliacoes += "            <div class='col-md-12'>";
                    htmlAvaliacoes += "               <div class='form-group'>";
                    htmlAvaliacoes += "                  <label class='titlefield'>Descrição da Avaliação <span class='red'>(Limite de 250 caracteres)</span></label>";
                    htmlAvaliacoes += "                  <textarea class='form-control' data-msg-required='Por favor, insira a Descrição.' data-rule-required='true'  id='" + String.Format("DadoAvaliacao{0}DescricaoAvaliacao", (i + 1)) + "' name='DadoAvaliacaoDescricaoAvaliacao[]' rows='3' maxlength='250' placeholder='Informe uma Descrição'>" + (avaliacoes.Count >= (i + 1) ? avaliacoes[i].Descricao : string.Empty) + "</textarea>";
                    htmlAvaliacoes += "               </div>";
                    htmlAvaliacoes += "            </div>";
                    htmlAvaliacoes += "         </div>";
                    // htmlAvaliacoes += "         <div class='row'>";
                    // htmlAvaliacoes += "            <div class='col-md-12'>";

                    // FileUploader
                    htmlAvaliacoes += "         <div class='row'>";
                    htmlAvaliacoes += "            <div class='col-md-12'>";
                    htmlAvaliacoes += "               <div class='form-group'>";
                    htmlAvaliacoes += "                  <label class='titlefield'>PDF Avaliação<label class='titlefieldcomment'> <span class='red'>(tamanho máximo de 10MB por arquivo)</label></label>";
                    htmlAvaliacoes += "                  <div id='" + String.Format("DadoAvaliacoes{0}Upload", (i + 1)) + (avaliacoes.Count == 0 ? 1 : avaliacoes[0].Id) + "' class='upload'>";
                    htmlAvaliacoes += "                     <input type='button' class='uploadButton' value='Selecione o arquivo' onclick='$(" + '"' + String.Format("#DadoAvaliacoes{0}AvaliacaoFileUpload", (i + 1)) + '"' + ").click();' />";
                    htmlAvaliacoes += "                     <input type='file' id='" + String.Format("DadoAvaliacoes{0}AvaliacaoFileUpload", (i + 1)) + "' name='" + String.Format("DadoAvaliacoes{0}AvaliacaoFileUpload", (i + 1)) + "' accept='application/pdf' value='10485760' />";

                    if (avaliacoes.Count - 1 >= i)
                    {
                        var FileName = Path.Combine(Server.MapPath("~/RH/Avaliacoes"), String.Format("Avaliacao-{0}.pdf", avaliacoes[(i)].Id));
                        if (System.IO.File.Exists(FileName))
                        {
                            htmlAvaliacoes += "                     <span id='" + String.Format("DadoAvaliacoes{0}AvaliacaoFileName", (i + 1)) + "'><a style='cursor: pointer;' onclick='VisualizarAvaliacaoPDF(" + '"' + String.Format("/RH/Avaliacoes/Avaliacao-{0}.pdf", avaliacoes[i].Id) + '"' + ");''>" + String.Format("Avaliacao-{0}.pdf", avaliacoes[i].Id) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a id='" + String.Format("ButtonAvaliacaoPDF{0}Excluir", (i + 1)) + "' class='btn btn-danger btn-xs' onclick='ExcluirAvaliacaoPDF(1, " + (i + 1) + ")'>Apagar</a>&nbsp;&nbsp;&nbsp;<label id='" + String.Format("LabelAvaliacaoFile{0}ExcluirConfirmacao", (i + 1)) + "' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse PDF?&nbsp;&nbsp;&nbsp;&nbsp;</label><a id='" + String.Format("acceptAvaliacao1File{0}", (i + 1)) + "' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirAvaliacaoPDFSim(1, " + (i + 1) + ", " + '"' + String.Format("Contrato-{0}.pdf", avaliacoes[i].Id.ToString()) + '"' + ")'>Sim</a>&nbsp;<a id='" + String.Format("go_backAvaliacao1File{0}", (i + 1)) + "' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirAvaliacaoPDFNao(1, " + (i + 1) + ")''>Não</a></span>";
                        }
                        else
                        {
                            htmlAvaliacoes += "                     <span id='" + String.Format("DadoAvaliacoes{0}AvaliacaoFileName", (i + 1)) + "'>Nenhum arquivo encontrado</span>";
                        }
                    }
                    else
                    {
                        htmlAvaliacoes += "                     <span id='" + String.Format("DadoAvaliacoes{0}AvaliacaoFileName", (i + 1)) + "'>Nenhum arquivo selecionado</span>";
                    }

                    htmlAvaliacoes += "                  </div>";
                    htmlAvaliacoes += "                  <label id='" + String.Format("DadoAvaliacoes{0}FileUploadValidate", (i + 1)) + "' name='" + String.Format("DadoAvaliacoes{0}FileUploadValidate", (i + 1)) + "' for='" + String.Format("DadoAvaliacoes{0}FileUpload1", (i + 1)) + "' class='error hidden'>Por favor, informe um arquivo com até 10MB.</label>";

                    htmlAvaliacoes += "               </div>";
                    htmlAvaliacoes += "            </div>";
                    htmlAvaliacoes += "         </div>";


                    htmlAvaliacoes += "            <div class='row'>";
                    htmlAvaliacoes += "                <div class='col-md-3'>";
                    htmlAvaliacoes += "                    <div class='form-group'>";
                    htmlAvaliacoes += "                        <label class='titlefield'>Data Cadastro</label>";
                    htmlAvaliacoes += "                        <input type='text' class='form-control' id='" + String.Format("DadoAvaliacao{0}DataCadastro", (i + 1)) + "' name='DadoAvaliacaoDataCadastro[]' readonly  value='" + (avaliacoes.Count > i ? avaliacoes[i].DataCadastro.Value.ToString("yyyy-MM-dd") : String.Empty) + "'/>";
                    htmlAvaliacoes += "                    </div>";
                    htmlAvaliacoes += "                </div>";
                    htmlAvaliacoes += "                <div class='col-md-3'>";
                    htmlAvaliacoes += "                    <div class='form-group'>";
                    htmlAvaliacoes += "                        <label class='titlefield'>Usuário Cadastro</label>";
                    htmlAvaliacoes += "                        <input type='text' class='form-control' id='" + String.Format("DadoAvaliacao{0}UsuarioCadastro", (i + 1)) + "' name='DadoAvaliacaoUsuarioCadastro[]' readonly  value='" + (avaliacoes.Count > i ? avaliacoes[i].UsuarioCadastro : String.Empty) + "'/>";
                    htmlAvaliacoes += "                    </div>";
                    htmlAvaliacoes += "                </div>";
                    htmlAvaliacoes += "                <div class='col-md-3'>";
                    htmlAvaliacoes += "                    <div class='form-group'>";
                    htmlAvaliacoes += "                        <label class='titlefield'>Data Alteração</label>";
                    htmlAvaliacoes += "                        <input type='text' class='form-control' id='" + String.Format("DadoAvaliacao{0}DataAlteracao", (i + 1)) + "' name='DadoAvaliacaoDataAlteracao[]' readonly  value='" + (avaliacoes.Count > i ? avaliacoes[i].DataAlteracao.Value.ToString("yyyy-MM-dd") : String.Empty) + "'/>";
                    htmlAvaliacoes += "                    </div>";
                    htmlAvaliacoes += "                </div>";
                    htmlAvaliacoes += "                <div class='col-md-3'>";
                    htmlAvaliacoes += "                    <div class='form-group'>";
                    htmlAvaliacoes += "                        <label class='titlefield'>Usuário Alteração</label>";
                    htmlAvaliacoes += "                        <input type='text' class='form-control' id='" + String.Format("DadoAvaliacao{0}UsuarioAlteracao", (i + 1)) + "' name='DadoAvaliacaoUsuarioAlteracao[]' readonly value='" + (avaliacoes.Count > i ? avaliacoes[i].UsuarioAlteracao : String.Empty) + "'/>";
                    htmlAvaliacoes += "                    </div>";
                    htmlAvaliacoes += "                </div>";
                    htmlAvaliacoes += "            </div>";



                    htmlAvaliacoes += "         <div class='row'>";
                    htmlAvaliacoes += "            <div class='col-md-4'>";
                    htmlAvaliacoes += "               <button type='button' id='" + String.Format("ButtonDadoAvaliacoes{0}Incluir", (i + 1)) + "' name='" + String.Format("ButtonDadoAvaliacoes{0}Salvar", (i + 1)) + "' class='btn btn-primary' data-loading-text='Salvando...' onclick='ButtonDadoAvaliacaoSalvar_Click(" + (i + 1) + ");'>";
                    htmlAvaliacoes += "                  Salvar";
                    htmlAvaliacoes += "               </button>";
                    htmlAvaliacoes += "            </div>";
                    htmlAvaliacoes += "            <div class='col-md-3' style='text-align: center;'>";
                    htmlAvaliacoes += "            </div>";
                    htmlAvaliacoes += "            <div class='col-md-5' style='text-align: right;'>";
                    htmlAvaliacoes += "               <button type='button' id='" + String.Format("ButtonDadoAvaliacoes{0}Excluir", (i + 1)) + "' name='" + String.Format("ButtonDadoAvaliacoes{0}Excluir", (i + 1)) + "' class='btn btn-danger' data-loading-text='Apagando...' onclick='ButtonDadoAvaliacaoExcluir_Click(" + (i + 1) + ");' " + (avaliacoes.Count >= (i + 1) ? String.Empty : "disabled='disabled'") + ">";
                    htmlAvaliacoes += "                  Apagar";
                    htmlAvaliacoes += "               </button>";
                    htmlAvaliacoes += "               <div id='" + String.Format("ButtonDadoAvaliacoes{0}ExcluirConfirmacao", (i + 1)) + "' style='display:none'>";
                    htmlAvaliacoes += "                  <label id='" + String.Format("LabelDadoAvaliacoes{0}ExcluirConfirmacao", (i + 1)) + "' style='font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente excluir esta Ausência?</label><br />";
                    htmlAvaliacoes += "                  <button id='" + String.Format("accept{0}", (i + 1)) + "' class='btn btn-default btn-xs' value='true'>Sim</button>";
                    htmlAvaliacoes += "                  <button id='" + String.Format("go_back{0}", (i + 1)) + "' class='btn btn-default btn-xs' value='false'>Não</button>";
                    htmlAvaliacoes += "               </div>";
                    htmlAvaliacoes += "            </div>";
                    htmlAvaliacoes += "         </div>";

                    htmlAvaliacoes += "      </div>";
                    htmlAvaliacoes += "   </div>";
                    htmlAvaliacoes += "</div>";

                    htmlAvaliacoes += "<script>";
                    htmlAvaliacoes += "    $('" + String.Format("#DadoAvaliacoes{0}AvaliacaoFileUpload", (i + 1)) + "').change(function (e) {";
                    htmlAvaliacoes += "        $in = $(this);";
                    htmlAvaliacoes += "         $in.next().html($in.val().split('\\\\').pop());"; // Note o uso de barras duplas para escapar a barra invertida // $in.next().html($in.val());";
                    htmlAvaliacoes += "    });";
                    htmlAvaliacoes += "</script>";
                }

                var result = new
                {
                    response = "success",
                    avaliacoes = htmlAvaliacoes,
                    count = quantidadeAvaliacoes
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new
                {
                    response = "error",
                    message = ex.Message
                };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult SalvarDadosAvaliacao(Int32 Id, Int32 IdProfissional, String Data, String Avaliador, String Descricao, HttpPostedFileBase AvaliacaoArquivo)
        {

            try
            {


                ProfissionaisAvaliacoes avaliacao = new ProfissionaisAvaliacoes();
                avaliacao.Id = Id;
                avaliacao.IdProfissional = IdProfissional;
                avaliacao.Avaliador = Avaliador;
                avaliacao.Descricao = Descricao;
                if (Data != null && Data.Trim().Length > 0)
                {
                    avaliacao.Data = DateTime.Parse(Data);
                }
                avaliacao.DataCadastro = DateTime.Now;
                avaliacao.UsuarioCadastro = Sessao.Usuario.Nome;
                avaliacao.DataAlteracao = DateTime.Now;
                avaliacao.UsuarioAlteracao = Sessao.Usuario.Nome;

                if (Id == 0)
                    Id = ProfissionaisAvaliacoesBo.Inserir(avaliacao);

                avaliacao.Id = Id;
                ProfissionaisAvaliacoesBo.Salvar(avaliacao);

                var url = String.Empty;
                if (AvaliacaoArquivo != null && AvaliacaoArquivo.ContentLength > 0)
                {
                    var file = Path.Combine(Server.MapPath("~/RH/Avaliacoes"), String.Format("Avaliacao-{0}.pdf", Id.ToString()));
                    AvaliacaoArquivo.SaveAs(file);
                }
                var result = new
                {
                    response = "success",
                    dados = avaliacao
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                String error = "error";
                String message = ex.InnerException?.ToString() ?? ex.Message;

                if (message.Contains("chave duplicada no objeto"))
                    error = "warning";

                var result = new
                {
                    response = error,
                    message = ex.Message
                };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult ExcluirDadosAvaliacao(Int32 Id)
        {
            try
            {
                ProfissionaisAvaliacoesBo.Excluir(Id);

                var result = new
                {
                    response = "removed"
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new
                {
                    response = "error",
                    message = ex.Message
                };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult ExcluirAvaliacaoPDF(String Arquivo)
        {
            try
            {
                var file = Path.Combine(Server.MapPath("~/RH/Avaliacoes"), Arquivo);
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }

                var result = new { response = "success" };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        // Ausencias
        [HttpPost]
        public JsonResult CarregarDadosAusencias(Int32 IdProfissional)
        {
            try
            {
                var ausencias = ProfissionaisAusenciasBo.ListarAusencias(IdProfissional);
                String ausenciasHtml = String.Empty;

                int AusenciasQuantidade = (ausencias.Count > 0 ? (ausencias.Count + 1) : 1);

                for (int i = 0; i < AusenciasQuantidade; i++)
                {

                    ausenciasHtml += "<input id='" + String.Format("HiddenDadoAusencia{0}", (i + 1)) + "' name='HiddenDadoAusencia[]' type='hidden' value='" + (ausencias.Count >= (i + 1) ? ausencias[i].Id.ToString() : "0") + "' />";
                    ausenciasHtml += "<div class='accordion panel panel-default'>";
                    ausenciasHtml += "   <div class='accordion-heading panel-heading' id='" + String.Format("DadoAusencia{0}heading", (i + 1)) + "' role='tab'>";
                    ausenciasHtml += "      <h4 class='accordion-title panel-title'>";
                    //ausenciasHtml += "         <a href='" + String.Format("#DadoAusencia{0}collapse", (i + 1)) + "' aria-expanded='false' data-toggle='collapse' aria-controls='" + String.Format("DadoAusencia{0}collapse", (i + 1)) + "' data-parent='#AusenciaAccordion' role='button' class='collapsed'>";
                    //ausenciasHtml += "            " + String.Format("Ausência #{0}", (i + 1));
                    //ausenciasHtml += "         </a>";



                    ausenciasHtml += "         <a href='" + String.Format("#DadoAusencia{0}collapse", (i + 1)) + "' aria-expanded='false' data-toggle='collapse' aria-controls='" + String.Format("DadoAusencia{0}collapse", (i + 1)) + "' data-parent='#AusenciaAccordion' role='button' class='collapsed'>";
                    ausenciasHtml += "            " + String.Format("Ausência #{0}", (i + 1));

                    // Adicionar os detalhes de Data de Início, Data de Fim e Abonado
                    if (ausencias.Count > i)
                    {
                        string dataInicio = ausencias[i].DataInicio.HasValue ? ausencias[i].DataInicio.Value.ToString("dd/MM/yyyy") : "N/A";
                        string dataFim = ausencias[i].DataFim.HasValue ? ausencias[i].DataFim.Value.ToString("dd/MM/yyyy") : "N/A";
                        string abonado = ausencias[i].Abonado == true ? "Sim" : "Não";

                        ausenciasHtml += $" - {dataInicio} a {dataFim} - Abonado: {abonado}";
                    }

                    ausenciasHtml += "         </a>";



                    ausenciasHtml += "      </h4>";
                    ausenciasHtml += "   </div>";
                    ausenciasHtml += "   <div class='panel-collapse collapse' id='" + String.Format("DadoAusencia{0}collapse", (i + 1)) + "' role='tabpanel' aria-labelledby='" + String.Format("DadoAusencia{0}heading", (i + 1)) + "' aria-expanded='false' style='height: 0px;'>";
                    ausenciasHtml += "      <div class='accordion-body panel-body'>";


                    ausenciasHtml += "         <div class='row'>";
                    ausenciasHtml += "            <div class='col-md-4'>";
                    ausenciasHtml += "               <div class='form-group'>";
                    ausenciasHtml += "                  <label class='titlefield'>Data de Início <span class='required'>*</span></label>";
                    ausenciasHtml += "                  <input type='date' id='" + String.Format("DadoAusencias{0}DataInicio", (i + 1)) + "' name='DadoAusenciasDataInicio[]' class='form-control' placeholder='Informe a Data de Início' data-msg-required='Por favor, insira a Data de Início.' data-rule-required='true' value='" + (ausencias.Count > i ? ausencias[i].DataInicio.Value.ToString("yyyy-MM-dd") : String.Empty) + "' />";
                    ausenciasHtml += "               </div>";
                    ausenciasHtml += "            </div>";
                    ausenciasHtml += "            <div class='col-md-4'>";
                    ausenciasHtml += "               <div class='form-group'>";
                    ausenciasHtml += "                  <label class='titlefield'>Data de Fim <span class='required'>*</span></label>";
                    ausenciasHtml += "                  <input type='date' id='" + String.Format("DadoAusencias{0}DataFim", (i + 1)) + "' name='DadoAusenciasDataFim[]' class='form-control' placeholder='Informe a Data de Fim' data-msg-required='Por favor, insira a Data de Fim.' data-rule-required='true' value='" + (ausencias.Count >= (i + 1) ? ausencias[i].DataFim.Value.ToString("yyyy-MM-dd") : String.Empty) + "' />";
                    ausenciasHtml += "               </div>";
                    ausenciasHtml += "            </div>";
                    ausenciasHtml += "            <div class='col-md-4'>";
                    ausenciasHtml += "               <div class='form-group'>";
                    ausenciasHtml += "                  <label class='titlefield'>Abonado <span class='required'>*</span></label>";
                    ausenciasHtml += "                  <select id='" + String.Format("DadoAusencias{0}Abonado", (i + 1)) + "' name='DadoAusenciasAbonado[]' class='form-control' data-msg-required='Por favor, selecione se foi Abonado.' data-rule-required='true'>";
                    ausenciasHtml += "                     <option value=''>Selecione</option>";
                    ausenciasHtml += "                     <option value='true'" + (ausencias.Count > i && ausencias[i].Abonado == true ? " selected" : "") + ">Sim</option>";
                    ausenciasHtml += "                     <option value='false'" + (ausencias.Count > i && ausencias[i].Abonado == false ? " selected" : "") + ">Não</option>";
                    ausenciasHtml += "                  </select>";
                    ausenciasHtml += "               </div>";
                    ausenciasHtml += "            </div>";
                    ausenciasHtml += "         </div>";

                    // FileUploader

                    ausenciasHtml += "         <div class='row'>";
                    ausenciasHtml += "            <div class='col-md-12'>";
                    ausenciasHtml += "               <div class='form-group'>";
                    ausenciasHtml += "                  <label class='titlefield'>PDF Atestado<label class='titlefieldcomment'> <span class='red'>(tamanho máximo de 10MB por arquivo)</label></label>";
                    ausenciasHtml += "                  <div id='" + String.Format("DadoAusencias{0}Upload", (i + 1)) + (ausencias.Count == 0 ? 1 : ausencias[0].Id) + "' class='upload'>";
                    ausenciasHtml += "                     <input type='button' class='uploadButton' value='Selecione o arquivo' onclick='$(" + '"' + String.Format("#DadoAusencias{0}AusenciaFileUpload", (i + 1)) + '"' + ").click();' />";
                    ausenciasHtml += "                     <input type='file' id='" + String.Format("DadoAusencias{0}AusenciaFileUpload", (i + 1)) + "' name='" + String.Format("DadoAusencias{0}AusenciaFileUpload", (i + 1)) + "' accept='application/pdf' value='10485760' />";

                    if (ausencias.Count - 1 >= i)
                    {
                        var FileName = Path.Combine(Server.MapPath("~/RH/Atestados"), String.Format("Atestado-{0}.pdf", ausencias[(i)].Id));
                        if (System.IO.File.Exists(FileName))
                        {
                            ausenciasHtml += "                     <span id='" + String.Format("DadoAusencias{0}AusenciaFileName", (i + 1)) + "'><a style='cursor: pointer;' onclick='VisualizarAusenciaPDF(" + '"' + String.Format("/RH/Atestados/Atestado-{0}.pdf", ausencias[i].Id) + '"' + ");''>" + String.Format("Atestado-{0}.pdf", ausencias[i].Id) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a id='" + String.Format("ButtonAusenciaPDF{0}Excluir", (i + 1)) + "' class='btn btn-danger btn-xs' onclick='ExcluirAusenciaPDF(1, " + (i + 1) + ")'>Apagar</a>&nbsp;&nbsp;&nbsp;<label id='" + String.Format("LabelAusenciaFile{0}ExcluirConfirmacao", (i + 1)) + "' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse PDF?&nbsp;&nbsp;&nbsp;&nbsp;</label><a id='" + String.Format("acceptAusencia1File{0}", (i + 1)) + "' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirAusenciaPDFSim(1, " + (i + 1) + ", " + '"' + String.Format("Atestado-{0}.pdf", ausencias[i].Id.ToString()) + '"' + ")'>Sim</a>&nbsp;<a id='" + String.Format("go_backAusencia1File{0}", (i + 1)) + "' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirAusenciaPDFNao(1, " + (i + 1) + ")''>Não</a></span>";
                        }
                        else
                        {
                            ausenciasHtml += "                     <span id='" + String.Format("DadoAusencias{0}AusenciaFileName", (i + 1)) + "'>Nenhum arquivo encontrado</span>";
                        }
                    }
                    else
                    {
                        ausenciasHtml += "                     <span id='" + String.Format("DadoAusencias{0}AusenciaFileName", (i + 1)) + "'>Nenhum arquivo selecionado</span>";
                    }

                    ausenciasHtml += "                  </div>";
                    ausenciasHtml += "                  <label id='" + String.Format("DadoAusencias{0}FileUploadValidate", (i + 1)) + "' name='" + String.Format("DadoAusencias{0}FileUploadValidate", (i + 1)) + "' for='" + String.Format("DadoAusencias{0}FileUpload1", (i + 1)) + "' class='error hidden'>Por favor, informe um arquivo com até 10MB.</label>";
                    ausenciasHtml += "               </div>";
                    ausenciasHtml += "            </div>";
                    ausenciasHtml += "         </div>";

                    ausenciasHtml += "         <div class='row'>";
                    ausenciasHtml += "            <div class='col-md-12'>";
                    ausenciasHtml += "               <div class='form-group'>";
                    ausenciasHtml += "                  <label class='titlefield'>Motivo da Ausência</label>";
                    ausenciasHtml += "                   <textarea id='" + String.Format("DadoAusencias{0}Motivo", (i + 1)) + "' name='DadoAusenciasMotivo[]' class='form-control' rows='3' placeholder='Informe uma Observação' maxlength='250'>" + (ausencias.Count >= (i + 1) ? ausencias[i].MotivoAusencia : String.Empty) + "</textarea>";
                    ausenciasHtml += "               </div>";
                    ausenciasHtml += "            </div>";
                    ausenciasHtml += "         </div>";

                    ausenciasHtml += "            <div class='row'>";
                    ausenciasHtml += "                <div class='col-md-3'>";
                    ausenciasHtml += "                    <div class='form-group'>";
                    ausenciasHtml += "                        <label class='titlefield'>Data Cadastro</label>";
                    ausenciasHtml += "                        <input type='text' class='form-control' id='" + String.Format("DadoAusencias{0}DataCadastro", (i + 1)) + "' name='DadoAusenciasDataCadastro[]' readonly   value='" + (ausencias.Count > i ? ausencias[i].DataCadastro.Value.ToString("yyyy-MM-dd") : String.Empty) + "' />";
                    ausenciasHtml += "                    </div>";
                    ausenciasHtml += "                </div>";
                    ausenciasHtml += "                <div class='col-md-3'>";
                    ausenciasHtml += "                    <div class='form-group'>";
                    ausenciasHtml += "                        <label class='titlefield'>Usuário Cadastro</label>";
                    ausenciasHtml += "                        <input type='text' class='form-control' id='" + String.Format("DadoAusencias{0}UsuarioCadastro", (i + 1)) + "' name='DadoAusenciasUsuarioCadastro[]' readonly   value='" + (ausencias.Count > i ? ausencias[i].UsuarioCadastro : String.Empty) + "'/>";
                    ausenciasHtml += "                    </div>";
                    ausenciasHtml += "                </div>";
                    ausenciasHtml += "                <div class='col-md-3'>";
                    ausenciasHtml += "                    <div class='form-group'>";
                    ausenciasHtml += "                        <label class='titlefield'>Data Alteração</label>";
                    ausenciasHtml += "                        <input type='text' class='form-control' id='" + String.Format("DadoAusencias{0}DataAlteracao", (i + 1)) + "' name='DadoAusenciasDataAlteracao[]' readonly  value='" + (ausencias.Count > i ? ausencias[i].DataAlteracao.Value.ToString("yyyy-MM-dd") : String.Empty) + "' />";
                    ausenciasHtml += "                    </div>";
                    ausenciasHtml += "                </div>";
                    ausenciasHtml += "                <div class='col-md-3'>";
                    ausenciasHtml += "                    <div class='form-group'>";
                    ausenciasHtml += "                        <label class='titlefield'>Usuário Alteração</label>";
                    ausenciasHtml += "                        <input type='text' class='form-control' id='" + String.Format("DadoAusencias{0}UsuarioAlteracao", (i + 1)) + "' name='DadoAusenciasUsuarioAlteracao[]' readonly   value='" + (ausencias.Count > i ? ausencias[i].UsuarioAlteracao : String.Empty) + "'/>";
                    ausenciasHtml += "                    </div>";
                    ausenciasHtml += "                </div>";
                    ausenciasHtml += "            </div>";

                    ausenciasHtml += "         <div class='row'>";
                    ausenciasHtml += "            <div class='col-md-4'>";
                    ausenciasHtml += "               <button type='button' id='" + String.Format("ButtonDadoAusencia{0}Incluir", (i + 1)) + "' name='" + String.Format("ButtonDadoAusencia{0}Salvar", (i + 1)) + "' class='btn btn-primary' data-loading-text='Salvando...' onclick='ButtonDadoAusenciaSalvar_Click(" + (i + 1) + ");'>";
                    ausenciasHtml += "                  Salvar";
                    ausenciasHtml += "               </button>";
                    ausenciasHtml += "            </div>";
                    ausenciasHtml += "            <div class='col-md-3' style='text-align: center;'>";
                    ausenciasHtml += "            </div>";
                    ausenciasHtml += "            <div class='col-md-5' style='text-align: right;'>";
                    ausenciasHtml += "               <button type='button' id='" + String.Format("ButtonDadoAusencia{0}Excluir", (i + 1)) + "' name='" + String.Format("ButtonDadoAusencia{0}Excluir", (i + 1)) + "' class='btn btn-danger' data-loading-text='Apagando...' onclick='ButtonDadoAusenciaExcluir_Click(" + (i + 1) + ");' " + (ausencias.Count >= (i + 1) ? String.Empty : "disabled='disabled'") + ">";
                    ausenciasHtml += "                  Apagar";
                    ausenciasHtml += "               </button>";
                    ausenciasHtml += "               <div id='" + String.Format("ButtonDadoAusencia{0}ExcluirConfirmacao", (i + 1)) + "' style='display:none'>";
                    ausenciasHtml += "                  <label id='" + String.Format("LabelDadoAusencia{0}ExcluirConfirmacao", (i + 1)) + "' style='font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente excluir esta Ausência?</label><br />";
                    ausenciasHtml += "                  <button id='" + String.Format("acceptAusencia{0}", (i + 1)) + "' class='btn btn-default btn-xs' value='true'>Sim</button>";
                    ausenciasHtml += "                  <button id='" + String.Format("go_backAusencia{0}", (i + 1)) + "' class='btn btn-primary btn-xs' value='false'>Não</button>";
                    ausenciasHtml += "               </div>";

                    //documentos += "               <div id='" + String.Format("ButtonColaboradorAusencias{0}ExcluirConfirmacao", (i + 1)) + "' style='display:none'>";
                    //documentos += "                  <label id='" + String.Format("LabelColaboradorDocumentos{0}ExcluirConfirmacao", (i + 1)) + "' style='font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente excluir esse documento?</label><br />";
                    //documentos += "                  <button id='" + String.Format("accept{0}", (i + 1)) + "' class='btn btn-default btn-xs' value='true'>Sim</button>";
                    //documentos += "                  <button id='" + String.Format("go_back{0}", (i + 1)) + "' class='btn btn-primary btn-xs' value='false'>Não</button>";
                    //documentos += "               </div>";



                    ausenciasHtml += "            </div>";
                    ausenciasHtml += "         </div>";

                    ausenciasHtml += "      </div>";
                    ausenciasHtml += "   </div>";
                    ausenciasHtml += "</div>";


                    ausenciasHtml += "<script>";
                    ausenciasHtml += "    $('" + String.Format("#DadoAusencias{0}AusenciaFileUpload", (i + 1)) + "').change(function (e) {";
                    ausenciasHtml += "        $in = $(this);";
                    ausenciasHtml += "         $in.next().html($in.val().split('\\\\').pop());"; // Note o uso de barras duplas para escapar a barra invertida // $in.next().html($in.val());";
                    ausenciasHtml += "    });";
                    ausenciasHtml += "</script>";

                }

                // return Json(new { Success = true, Html = ausenciasHtml });


                var result = new { response = "success", ausencias = ausenciasHtml, count = AusenciasQuantidade };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Erro ao carregar os dados de ausências: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SalvarDadosAusencias(Int32 Id, Int32 IdProfissional, String DataInicio, String DataFim, Boolean? Abonado, String MotivoAusencia, HttpPostedFileBase AtestadoArquivo)
        {
            try
            {
                var mensagemEmail = String.Empty;
                var mensagem = string.Empty;
                DateTime? DataInicioDate = null;
                if (DataInicio != null && DataInicio.Trim().Length > 0)
                {
                    DataInicioDate = DateTime.Parse(DataInicio);
                }

                DateTime? DataFimDate = null;
                if (DataFim != null && DataFim.Trim().Length > 0)
                {
                    DataFimDate = DateTime.Parse(DataFim);
                }

                ProfissionaisAusencias ausencia = new ProfissionaisAusencias();
                ausencia.Id = Id;
                ausencia.IdProfissional = IdProfissional;
                ausencia.DataInicio = DataInicioDate;
                ausencia.DataFim = DataFimDate;
                ausencia.Abonado = Abonado;
                ausencia.MotivoAusencia = MotivoAusencia?.Trim();

                ausencia.DataCadastro = DateTime.Now;
                ausencia.UsuarioCadastro = Sessao.Usuario.Nome;
                ausencia.DataAlteracao = DateTime.Now;
                ausencia.UsuarioAlteracao = Sessao.Usuario.Nome;

                if (Id == 0)
                    Id = ProfissionaisAusenciasBo.Inserir(ausencia);

                ausencia.Id = Id;
                ProfissionaisAusenciasBo.Salvar(ausencia);

                var url = String.Empty;
                if (AtestadoArquivo != null && AtestadoArquivo.ContentLength > 0)
                {
                    var file = Path.Combine(Server.MapPath("~/RH/Atestados"), String.Format("Atestado-{0}.pdf", Id.ToString()));
                    AtestadoArquivo.SaveAs(file);
                    mensagemEmail = " com Atestado anexado";
                }

                var nomeProfissional = ProfissionaisBo.Consultar(IdProfissional);
                var titulo = nomeProfissional.Pessoas.Nome + "registrou ausencia";
                mensagemEmail = "Ausência registrada no periodo de " + DataInicio + " à " + DataFim + mensagemEmail;
                var resposta = EnviarEmail(titulo, nomeProfissional.Pessoas.Nome, mensagemEmail);
                mensagem = "Ausência Salva com sucesso - " + resposta;

                var result = new
                {
                    response = "success",
                    dados = ausencia
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                String error = "error";
                String message = ex.InnerException?.ToString() ?? ex.Message;

                if (message.Contains("chave duplicada no objeto"))
                    error = "warning";

                var result = new
                {
                    response = error,
                    message = ex.Message
                };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult ExcluirDadosAusencias(Int32 Id)
        {
            try
            {
                ProfissionaisAusenciasBo.Excluir(Id);

                var result = new { response = "removed" };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error", message = ex.Message };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult ExcluirAusenciaPDF(String Arquivo)
        {
            try
            {
                var file = Path.Combine(Server.MapPath("~/RH/Atestados"), Arquivo);
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }

                var result = new { response = "success" };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        // Recesso
        [HttpPost]
        public JsonResult CarregarDadosRecessos(Int32 IdProfissional)
        {
            try
            {
                var recesso = ProfissionaisSolicitacoesBo.ListarSolicitacoes(IdProfissional);
                String recessos = String.Empty;

                int recessosQuantidade = (recesso.Count > 0 ? (recesso.Count + 1) : 1);

                for (int i = 0; i < recessosQuantidade; i++)
                {
                    recessos += "<input id='" + String.Format("HiddenRecesso{0}", (i + 1)) + "' name='HiddenRecesso[]' type='hidden' value='" + (recesso.Count > i ? recesso[i].Id.ToString() : "0") + "' />";
                    recessos += "<div class='accordion panel panel-default'>";
                    recessos += "   <div class='accordion-heading panel-heading' id='" + String.Format("Recesso{0}heading", (i + 1)) + "' role='tab'>";
                    recessos += "      <h4 class='accordion-title panel-title'>";
                    recessos += "         <a href='" + String.Format("#Recesso{0}collapse", (i + 1)) + "' aria-expanded='false' data-toggle='collapse' aria-controls='" + String.Format("Recesso{0}collapse", (i + 1)) + "' data-parent='#DadosolicitacoesAccordion' role='button' class='collapsed'>";
                    recessos += "            " + String.Format("Recesso #{0}", (i + 1));
                    recessos += (recesso.Count >= (i + 1) ? String.Format(" - {0} {1}{2}", recesso[i].DataInicio.Value.ToString("yyyy-MM-dd"), " - " + recesso[i].DataInicio.Value.ToString("yyyy-MM-dd"), " - " + recesso[i].Saldo, recesso[i].Status) : String.Empty);
                    recessos += "         </a>";
                    recessos += "      </h4>";
                    recessos += "   </div>";
                    recessos += "   <div class='panel-collapse collapse' id='" + String.Format("Recesso{0}collapse", (i + 1)) + "' role='tabpanel'  aria-labelledby='" + String.Format("Recesso{0}heading", (i + 1)) + "' aria-expanded='false' style='height: 0px;'>";
                    recessos += "      <div class='accordion-body panel-body'>";

                    // Formulário de recesso
                    recessos += "         <div class='row'>";
                    recessos += "            <div class='col-md-3'>";
                    recessos += "               <div class='form-group'>";
                    recessos += "                  <label class='titlefield'>Data de Início <span class='required'>*</span></label>";
                    recessos += "                  <input type='date' data-msg-required='Por favor, insira a Data de Início.' data-rule-required='true' class='form-control' placeholder='Informe a Data' id='" + String.Format("ColaboradorDadoRecesso{0}DataInicio", (i + 1)) + "' name='ColaboradorDadoRecessoDataInicio[]' readonly value='" + (recesso.Count > i ? recesso[i].DataInicio.Value.ToString("yyyy-MM-dd") : String.Empty) + "'/>";
                    recessos += "               </div>";
                    recessos += "            </div>";

                    //recessos += "            <div class='col-md-4'>";
                    //recessos += "               <div class='form-group'>";
                    //recessos += "                  <label class='titlefield'>Data de Fim <span class='required'>*</span></label>";
                    //recessos += "                  <input type='date' data-msg-required='Por favor, insira a Data de Fim.' data-rule-required='true' class='form-control' placeholder='Informe a Data' id='" + String.Format("ColaboradorDadoRecesso{0}DataFim", (i + 1)) + "' name='ColaboradorDadoRecessoDataFim[]' value='" + (recesso.Count > i ? recesso[i].DataInicio.Value.ToString("yyyy-MM-dd") : String.Empty) + "'/>";
                    //recessos += "               </div>";
                    //recessos += "            </div>";

                    recessos += "            <div class='col-md-3'>";
                    recessos += "               <div class='form-group'>";
                    recessos += "                  <label class='titlefield'>Dias<span class='required'>*</span></label>";
                    recessos += "                  <input type='text' data-msg-required='Por favor, insira quantidade de Dias.' data-rule-required='true' class='form-control' placeholder='Informe a Qnt de Dias' id='" + String.Format("ColaboradorDadoRecesso{0}Dias", (i + 1)) + "' name='ColaboradorDadoRecessoDias[]' readonly value='" + (recesso.Count > i ? recesso[i].Dias.Value.ToString("yyyy-MM-dd") : String.Empty) + "'/>";
                    recessos += "               </div>";
                    recessos += "            </div>";

                    recessos += "            <div class='col-md-3'>";
                    recessos += "               <div class='form-group'>";
                    recessos += "                  <label class='titlefield'>Saldo <span class='required'>*</span></label>";
                    recessos += "                  <input type='text' id='" + String.Format("ColaboradorDadoRecesso{0}Saldo", (i + 1)) + "' name='ColaboradorDadoRecessoSaldo[]' class='form-control' placeholder='Informe o E-mail' data-msg-required='Por favor, insira o Saldo.' data-rule-required='true' maxlength='100' readonly value='" + (recesso.Count >= (i + 1) ? recesso[i].Saldo : 0) + "' />";
                    recessos += "               </div>";
                    recessos += "            </div>";

                    recessos += "            <div class='col-md-3'>";
                    recessos += "               <div class='form-group'>";
                    recessos += "                  <label class='titlefield'>Status <span class='required'>*</span></label>";
                    recessos += "                  <select  id='" + String.Format("ColaboradorDadoRecesso{0}Status", (i + 1)) + "' name='ColaboradorDadoRecessoStatus[]' class='form-control placeholder'  data-msg-required='Por favor, insira o Status.' data-rule-required='true'>";
                    recessos += "                     <option value='' style='background-color: #eaeaea;'>Selecione o Status</option>";
                    recessos += "                     <option value='Em Análise'" + (recesso.Count > i && recesso[i].Status == "Em Análise" ? " selected" : String.Empty) + ">Em Análise</option>";
                    recessos += "                     <option value='Aprovada'" + (recesso.Count > i && recesso[i].Status == "Aprovada" ? " selected" : String.Empty) + ">Aprovada</option>";
                    recessos += "                     <option value='Reprovada'" + (recesso.Count > i && recesso[i].Status == "Reprovada" ? " selected" : String.Empty) + ">Reprovada</option>";
                    recessos += "                  </select>";
                    recessos += "               </div>";
                    recessos += "            </div>";
                    recessos += "         </div>";


                    //recessos += "            <div class='col-md-4'>";
                    //recessos += "               <div class='form-group'>";
                    //recessos += "                  <label class='titlefield'>Data da Solicitação<span class='required'>*</span></label>";
                    //recessos += "                  <input type='date'  data-msg-required='Por favor, insira a Data da Solicitação.' data-rule-required='true' class='form-control' placeholder='Informe a Data' id='" + String.Format("ColaboradorDadoRecesso{0}DataSolicitacao", (i + 1)) + "' name='ColaboradorDadoRecessoDataSolicitacao[]' value='" + (recesso.Count > i ? recesso[i].DataCadastro.Value.ToString("yyyy-MM-dd") : String.Empty) + "' />";
                    //recessos += "               </div>";
                    //recessos += "            </div>";
                    //recessos += "            <div class='col-md-4'>";
                    //recessos += "               <div class='form-group'>";
                    //recessos += "                  <label class='titlefield'>Status <span class='required'>*</span></label>";
                    //recessos += "                  <select  id='" + String.Format("ColaboradorDadoRecesso{0}Status", (i + 1)) + "' name='ColaboradorDadoRecessoStatus[]' class='form-control placeholder'  data-msg-required='Por favor, insira o Status.' data-rule-required='true'>";
                    //recessos += "                     <option value='' style='background-color: #eaeaea;'>Selecione o Status</option>";
                    //recessos += "                     <option value='Em Análise'" + (recesso.Count > i && recesso[i].Status == "Em Análise" ? " selected" : String.Empty) + ">Em Análise</option>";
                    //recessos += "                     <option value='Aprovada'" + (recesso.Count > i && recesso[i].Status == "Aprovada" ? " selected" : String.Empty) + ">Aprovada</option>";
                    //recessos += "                     <option value='Reprovada'" + (recesso.Count > i && recesso[i].Status == "Reprovada" ? " selected" : String.Empty) + ">Reprovada</option>";
                    //recessos += "                  </select>";
                    //recessos += "               </div>";
                    //recessos += "            </div>";


                    recessos += "         <div class='row'>";
                    recessos += "            <div class='col-md-12'>";
                    recessos += "               <div class='form-group'>";
                    recessos += "                  <label class='titlefield'>Justificativa<span class='red'>(Limite de 250 caracteres)</span></label>";
                    recessos += "                  <textarea class='form-control' placeholder='Informe uma justificativa' id='" + String.Format("ColaboradorDadoRecesso{0}Justificativa", (i + 1)) + "' name='ColaboradorDadoRecessoJustificativa[]' rows='3' maxlength='250' data-msg-required='Por favor, insira o Status.' data-rule-required='true'>" + (recesso.Count > i ? recesso[i].Justificativa : String.Empty) + "</textarea>";
                    recessos += "               </div>";
                    recessos += "            </div>";
                    recessos += "         </div>";

                    recessos += "         <div class='row'>";
                    recessos += "            <div class='col-md-3'>";
                    recessos += "               <div class='form-group'>";
                    recessos += "                  <label class='titlefield'>Data de Aprovação <span class='required'>*</span></label>";
                    recessos += "                  <input type='date' data-msg-required='Por favor, insira a Data de Aprovação.' data-rule-required='true' class='form-control' placeholder='Informe a Data' id='" + String.Format("ColaboradorDadoRecesso{0}DataAprovacao", (i + 1)) + "' name='ColaboradorDadoRecessoDataAprovacao[]' value='" + (recesso.Count > i ? recesso[i].DataAprovacao.Value.ToString("yyyy-MM-dd") : String.Empty) + "'/>";
                    recessos += "               </div>";
                    recessos += "            </div>";

                    recessos += "            <div class='col-md-3'>";
                    recessos += "               <div class='form-group'>";
                    recessos += "                  <label class='titlefield'>Solicitação Especial <span class='required'>*</span></label>";
                    recessos += "                  <input type='text'class='form-control' placeholder='Solicitação Especial' id='" + String.Format("ColaboradorDadoRecesso{0}Especial", (i + 1)) + "' name='ColaboradorDadoRecessoDataEspecial[]' readonly value='" + (recesso.Count > i ? (recesso[i].SolicitacaoEspecial.HasValue ? (recesso[i].SolicitacaoEspecial.Value ? "Sim" : "Não") : String.Empty) : String.Empty) + "' /> ";
                    recessos += "               </div>";
                    recessos += "            </div>";

                    recessos += "            <div class='col-md-3'>";
                    recessos += "               <div class='form-group'>";
                    recessos += "                  <label class='titlefield'>Periodo Aquisitivo<span class='required'>*</span></label>";
                    recessos += "                  <input type='text' id='" + String.Format("ColaboradorDadoRecesso{0}IdPeriodoAquisitivo", (i + 1)) + "' name='ColaboradorDadoRecessoIdPeriodoAquisitivo[]' class='form-control' placeholder='Informe o Periodo Aquisitivo' data-msg-required='Por favor, insira o Saldo.' data-rule-required='true' maxlength='100' readonly value='" + (recesso.Count >= (i + 1) ? recesso[i].IdPeriodoAquisitivo : 0) + "' />";
                    recessos += "               </div>";
                    recessos += "            </div>";

                    recessos += "         </div>";


                    recessos += "            <div class='row'>";
                    recessos += "                <div class='col-md-3'>";
                    recessos += "                    <div class='form-group'>";
                    recessos += "                        <label class='titlefield'>Data Cadastro</label>";
                    recessos += "                        <input type='text' class='form-control' id='" + String.Format("ColaboradorDadoRecesso{0}DataCadastro", (i + 1)) + "' name='ColaboradorDadoRecessoDataCadastro[]' readonly  value='" + (recesso.Count > i ? recesso[i].DataCadastro.Value.ToString("yyyy-MM-dd") : String.Empty) + "'/>";
                    recessos += "                    </div>";
                    recessos += "                </div>";
                    recessos += "                <div class='col-md-3'>";
                    recessos += "                    <div class='form-group'>";
                    recessos += "                        <label class='titlefield'>Usuário Cadastro</label>";
                    recessos += "                        <input type='text' class='form-control' id='" + String.Format("ColaboradorDadoRecesso{0}UsuarioCadastro", (i + 1)) + "' name='ColaboradorDadoRecessoUsuarioCadastro[]' readonly  value='" + (recesso.Count > i ? recesso[i].UsuarioCadastro : String.Empty) + "'/>";
                    recessos += "                    </div>";
                    recessos += "                </div>";
                    recessos += "                <div class='col-md-3'>";
                    recessos += "                    <div class='form-group'>";
                    recessos += "                        <label class='titlefield'>Data Alteração</label>";
                    recessos += "                        <input type='text' class='form-control' id='" + String.Format("ColaboradorDadoRecesso{0}DataAlteracao", (i + 1)) + "' name='ColaboradorDadoRecessoDataAlteracao[]' readonly  value='" + (recesso.Count > i ? recesso[i].DataAlteracao.Value.ToString("yyyy-MM-dd") : String.Empty) + "'/>";
                    recessos += "                    </div>";
                    recessos += "                </div>";
                    recessos += "                <div class='col-md-3'>";
                    recessos += "                    <div class='form-group'>";
                    recessos += "                        <label class='titlefield'>Usuário Alteração</label>";
                    recessos += "                        <input type='text' class='form-control' id='" + String.Format("ColaboradorDadoRecesso{0}UsuarioAlteracao", (i + 1)) + "' name='ColaboradorDadoRecessoUsuarioAlteracao[]' readonly  value='" + (recesso.Count > i ? recesso[i].UsuarioAlteracao : String.Empty) + "'/>";
                    recessos += "                    </div>";
                    recessos += "                </div>";
                    recessos += "            </div>";



                    //<div class="row">
                    //    <div class="col-md-3">
                    //        <div class="form-group">
                    //            <label class="titlefield">Data Cadastro</label>
                    //            <input type="text" class="form-control" id="SolicitcaoFeriasDataCadastro" name="SolicitcaoFeriasDataCadastro" readonly />
                    //        </div>
                    //    </div>
                    //    <div class="col-md-3">
                    //        <div class="form-group">
                    //            <label class="titlefield">Usuário Cadastro</label>
                    //            <input type="text" class="form-control" id="SolicitcaoFeriasUsuarioCadastro" name="SolicitcaoFeriasUsuarioCadastro" readonly />
                    //        </div>
                    //    </div>
                    //    <div class="col-md-3">
                    //        <div class="form-group">
                    //            <label class="titlefield">Data Alteração</label>
                    //            <input type="text" class="form-control" id="SolicitcaoFeriasDataAlteracao" name="SolicitcaoFeriasDataAlteracao" readonly />
                    //        </div>
                    //    </div>
                    //    <div class="col-md-3">
                    //        <div class="form-group">
                    //            <label class="titlefield">Usuário Alteração</label>
                    //            <input type="text" class="form-control" id="SolicitcaoFeriasUsuarioAlteracao" name="SolicitcaoFeriasUsuarioAlteracao" readonly />
                    //        </div>
                    //    </div>
                    //</div>









                    recessos += "         <div class='row'>";
                    recessos += "            <div class='col-md-4'>";
                    recessos += "               <button type='button' id='" + String.Format("ButtonDadoRecesso{0}Incluir", (i + 1)) + "' name='" + String.Format("ButtonDadoRecesso{0}Salvar", (i + 1)) + "' class='btn btn-primary' data-loading-text='Salvando...' onclick='ButtonDadosRecessosSalvar_Click(" + (i + 1) + ");'>";
                    recessos += "                  Salvar";
                    recessos += "               </button>";
                    recessos += "            </div>";
                    recessos += "            <div class='col-md-3' style='text-align: center;'>";
                    recessos += "            </div>";
                    recessos += "            <div class='col-md-5' style='text-align: right;'>";
                    recessos += "               <button type='button' id='" + String.Format("ButtonDadoRecesso{0}Excluir", (i + 1)) + "' name='" + String.Format("ButtonDadoRecesso{0}Excluir", (i + 1)) + "' class='btn btn-danger' data-loading-text='Apagando...' onclick='ButtonDadoRecessoExcluir_Click(" + (i + 1) + ");' " + (recesso.Count > i ? String.Empty : "disabled='disabled'") + ">";
                    recessos += "                  Apagar";
                    recessos += "               </button>";
                    recessos += "               <div id='" + String.Format("ButtonDadoRecesso{0}ExcluirConfirmacao", (i + 1)) + "' style='display:none'>";
                    recessos += "                  <label id='" + String.Format("LabelDadoRecesso{0}ExcluirConfirmacao", (i + 1)) + "' style='font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente excluir essa solicitação?</label><br />";
                    recessos += "                  <button id='" + String.Format("accept{0}", (i + 1)) + "' class='btn btn-default btn-xs' value='true'>Sim</button>";
                    recessos += "                  <button id='" + String.Format("go_back{0}", (i + 1)) + "' class='btn btn-primary btn-xs' value='false'>Não</button>";
                    recessos += "               </div>";
                    recessos += "            </div>";
                    recessos += "         </div>";

                    recessos += "      </div>";
                    recessos += "   </div>";
                    recessos += "</div>";
                }

                var result = new
                {
                    response = "success",
                    recessos = recessos,
                    count = recessosQuantidade
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new
                {
                    response = "error",
                    message = ex.Message
                };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult SalvarDadosRecessos(Int32 Id, Int32 IdProfissional, Int32 IdContrato, String DataSolicitacao, String DataAprovacao, String Status, String DataInicio, String Saldo, String Dias, String Justificativa, String Especial, Int32 IdPeriodoAquisitivo)
        {
            try
            {
                var mensagemEmail = String.Empty;
                var mensagem = string.Empty;
                var codigo = "00";

                ProfissionaisSolicitacoes solicitacao = new ProfissionaisSolicitacoes();
                solicitacao.Id = Id;

                solicitacao.IdProfissional = IdProfissional;

                if (IdContrato > 0)
                {
                    solicitacao.IdContrato = IdContrato;
                }
                else
                {
                    solicitacao.IdContrato = ProfissionaisContratosBo.ConsultarContrato().Id;
                }

                if (DataSolicitacao != null && DataSolicitacao.Trim().Length > 0)
                {
                    solicitacao.DataSolicitacao = DateTime.Parse(DataSolicitacao);
                }

                if (DataAprovacao != null && DataAprovacao.Trim().Length > 0)
                {
                    solicitacao.DataAprovacao = DateTime.Parse(DataAprovacao);
                }

                solicitacao.Status = Status.Trim();

                if (DataInicio != null && DataInicio.Trim().Length > 0)
                {
                    solicitacao.DataInicio = DateTime.Parse(DataInicio);
                }

                if (Saldo != null && Saldo.Trim().Length > 0)
                {
                    solicitacao.Saldo = int.Parse(Saldo);
                }

                if (Dias != null && Dias.Trim().Length > 0)
                {
                    solicitacao.Dias = int.Parse(Dias);
                }

                solicitacao.Justificativa = Justificativa.Trim();
                solicitacao.IdPeriodoAquisitivo = IdPeriodoAquisitivo;

                solicitacao.SolicitacaoEspecial = (Especial == "S" ? true : false);

                solicitacao.DataCadastro = DateTime.Now;
                solicitacao.UsuarioCadastro = Sessao.Usuario.Nome;
                solicitacao.DataAlteracao = DateTime.Now;
                solicitacao.UsuarioAlteracao = Sessao.Usuario.Nome;

                if (Id == 0)
                {
                    Id = ProfissionaisSolicitacoesBo.Inserir(solicitacao);
                }

                solicitacao.Id = Id;
                ProfissionaisSolicitacoesBo.Salvar(solicitacao);

                var nomeProfissional = ProfissionaisBo.Consultar(IdProfissional);
                var titulo = "Solicitação de Férias/Recesso de: ";
                mensagemEmail = "Solicitado dia " + DataSolicitacao + ", " + Dias + " dias para inicio dia " + DataInicio;
                var resposta = EnviarEmail(titulo, nomeProfissional.Pessoas.Nome, mensagemEmail);
                mensagem = "Solicitação Salva com sucesso - " + resposta;

                var result = new { codigo = codigo, mensagem = mensagem };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new
                {
                    response = "error",
                    message = ex.Message
                };
                return Json(result);
            }
        }

        public static string EnviarEmail(String Titulo, String NomeProfissional, String mensagemEmail)
        {
            var motivo = string.Empty;

            var empresa = EmpresaBo.Buscar(1);

            var menssage = string.Empty;
            var to = string.Empty;
            var name = NomeProfissional;
            var emailremetente = empresa.Email;
            var passwordremetente = empresa.Senha;
            Titulo = Titulo + NomeProfissional;

            var email = UsuariosBo.Buscar(Sessao.Usuario.ID);

            to = "";

            menssage += "<!DOCTYPE html>";
            menssage += "<html>";
            menssage += "  <head>";
            menssage += "    <style>";
            menssage += "      #customers {";
            menssage += "        font-family: 'Trebuchet MS', Arial, Helvetica, sans-serif;";
            menssage += "        border - collapse: collapse;";
            menssage += "        width: 100 %;";
            menssage += "      }";
            menssage += "      #customers td, #customers th {";
            menssage += "        border: 1px solid #DDDDDD;";
            menssage += "        padding: 8px;";
            menssage += "      }";
            menssage += "      #customers tr:nth-child(even){ background-color: #EDEDED; }";
            menssage += "      #customers tr:hover { background-color: #EED58F; }";
            menssage += "      #customers th {";
            menssage += "        padding-top: 12px;";
            menssage += "        padding-bottom: 12px;";
            menssage += "        text-align: left;";
            menssage += "        background-color: #ECBD4F;";
            menssage += "        color: #333333;";
            menssage += "        border: 1px solid black";
            menssage += "        border-collapse: collapse";
            menssage += "      }";
            menssage += "    </style>";
            menssage += "  </head>";
            menssage += "  <body>";
            menssage += "    <h3>";
            menssage += "      Solicitado por: " + NomeProfissional;
            menssage += "    </h3>";
            menssage += "    <table id='customers'>";
            menssage += "      <tr>";
            menssage += "        <td>" + mensagemEmail;
            menssage += "      </tr>";

            if (!string.IsNullOrEmpty(menssage))
            {
                menssage += "    </table>";
                menssage += "  </body>";
                menssage += "</html>";

                if (!Mail.Send(emailremetente, passwordremetente, to, Titulo, menssage))
                {
                    motivo = "Ocorreu uma falha ao enviar os E-mails. Tente novamente mais tarde ou entre em contato com o administrador do sistema.";
                }
                else
                {
                    motivo = "Email enviado com sucesso";
                }
            }


            return motivo;
        }

    }

}