using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Common;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class ClientesController : Controller
    {
        public ActionResult GetClientsList(string search, bool filter = false, int page = 1)
        {
            var clientes = ClientesBo.GetClientsList(search, filter);
            var total = clientes.Count();

            if (!(string.IsNullOrEmpty(search) || string.IsNullOrWhiteSpace(search)))
            {
                clientes = clientes.Where(x => PessoasBo.ConverteNome(x.text).ToLower().StartsWith(PessoasBo.ConverteNome(search).ToLower())).Take(page * 10).ToList();
            }

            return Json(new { clientes = clientes, total = total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index(string FiltrarNome = "", string FiltrarCpf = "", string FiltrarFone = "", string FiltrarEmail = "", string FiltrarDataNascimento = "")
        {

            var filtrarcpf = FiltrarCpf.Replace("-", "").Replace(".", "").Replace("/", "").Replace(" ", "").Replace(" ", "");
            var filtrarfone = FiltrarFone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(".", "").Replace(" ", "");
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {

                var model = ClientesBo.ListarFiltros(FiltrarNome, filtrarcpf, filtrarfone, FiltrarEmail, FiltrarDataNascimento);
                ViewBag.FiltrarNome = FiltrarNome;
                ViewBag.FiltrarCpf = FiltrarCpf;
                ViewBag.FiltrarFone = FiltrarFone;
                ViewBag.FiltrarEmail = FiltrarEmail;
                ViewBag.FiltrarDataNascimento = FiltrarDataNascimento;
                return View(model);
            }
        }

        public ActionResult Timeline()
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var cliente = ClientesBo.ConsultarArray(Id);

            var arquivo = "Nenhum arquivo selecionado";
            var file = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}.pdf", "Cliente-" + Id.ToString()));
            if (System.IO.File.Exists(file))
            {
                arquivo = "<a style='cursor: pointer;' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}.pdf", "Cliente-" + Id.ToString()) + '"' + ");'>" + String.Format("{0}.pdf", "Cliente-" + Id.ToString()) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;<a id='ButtonClientePDFExcluir' class='btn btn-danger btn-xs' onclick='ExcluirPDF()'>Apagar</a>&nbsp;&nbsp;<label id='LabelClienteFileExcluirConfirmacao' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse PDF?&nbsp;&nbsp;&nbsp;</label><a id='acceptFile' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirPDFSim(" + '"' + String.Format("{0}.pdf", "Cliente-" + Id.ToString()) + '"' + ")'>Sim</a><a id='go_backFile' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirPDFNao()'>Não</a>";
            }

            var result = new { codigo = "00", cliente = cliente, arquivo = arquivo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, Int32 IdPessoa, String Nome, String CPFCNPJ, String RG, String Profissao, String Nacionalidade, String CEP, String Logradouro, String Numero,
                        String Complemento, String Bairro, String Estado, String Cidade, String IM, String IE, String CNAE, String DataNascimento, String EstadoCivil, String NomeMae,
                        String SenhaMeuINSS, String Observacao, Int32? IdSede, String Cliente, String Etiqueta, String LGPDAutorizado, HttpPostedFileBase LGPDArquivo,
                        Int64? IdPreCadastro, String Telefone, String Email)
        {
            //#region Auditoria
            //Auditoria auditoria = new Auditoria();
            //auditoria.DataHora = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            //auditoria.Modulo = "Cliente";
            //auditoria.Tipo = "Dados Básicos";
            //auditoria.Acao = "Alterado";
            //auditoria.Log = String.Format("<b>Nome</b>: {0};<b>CPFCNPJ</b>: {1};<b>RG</b>: {2};;<b>Profissao</b>: {3};;<b>Nacionalidade</b>: {4};<b>CEP</b>: {5};<b>Logradouro</b>: {6};<b>Número</b>: {7};<b>Complemento</b>: {8};<b>Bairro</b>: {9};<b>Estado</b>: {10};<b>Cidade</b>: {11};<b>IM</b>: {12};<b>IE</b>: {13};<b>CNAE</b>: {14};<b>Indicação</b>: {15};", Nome, CPFCNPJ, RG, Profissao, Nacionalidade, CEP, Logradouro, Numero, Complemento, Bairro, Estado, Cidade, IM, IE, CNAE, Indicacao);
            //auditoria.Usuario = Sessao.Usuario.Nome;

            //if (Id == 0)
            //{
            //    auditoria.Acao = "Inserido";
            //}

            //AuditoriaBo.Inserir(auditoria);
            //#endregion

            Pessoas pessoa = new Pessoas();
            pessoa.ID = IdPessoa;
            pessoa.Nome = Nome.Trim();

            if (Util.IsNumeric(Util.OnlyNumbers(CPFCNPJ.Trim())))
            {
                pessoa.CpfCnpj = Util.OnlyNumbers(CPFCNPJ.Trim());
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
            pessoa.DataNascimento = DataNascimento.Trim();
            pessoa.EstadoCivil = EstadoCivil.Trim();
            pessoa.NomeMae = NomeMae.Trim();
            if (Cliente == "S")
            {
                pessoa.Cliente = true;
            }
            else
            {
                pessoa.Cliente = false;
            }

            if (Etiqueta == "S")
            {
                pessoa.Etiqueta = true;
            }
            else
            {
                pessoa.Etiqueta = false;
            }

            pessoa.LGPDAutorizado = LGPDAutorizado;

            Clientes cliente = new Clientes();
            cliente.ID = Id;
            cliente.IDPessoa = IdPessoa;
            cliente.InscricaoMunicipal = IM;
            cliente.InscricaoEstadual = IE;
            cliente.Cnae = CNAE;
            cliente.SenhaMeuInss = SenhaMeuINSS;
            cliente.Profissao = Profissao;
            cliente.Nacionalidade = Nacionalidade;

            if (IdSede != null)
            {
                cliente.IdSede = IdSede;
            }

            //cliente.Vinculo = Vinculo;
            //cliente.IdIndicacaoTipo = IdTipoIndicacao;

            //var tipo = TiposIndicacoesBo.Consultar((IdTipoIndicacao ?? 0))?.Tipo;

            //cliente.Indicacao = (tipo == 1 ? Indicacao : null);
            //cliente.IdParceiro = (tipo == 2 ? IdParceiro : null);
            //cliente.IdProfissional = (tipo == 3 ? IdProfissional : null);
            //cliente.IdCliente = (tipo == 5 ? IdCliente : null);

            cliente.Observacao = Observacao;
            cliente.DataCadastro = DateTime.Now;
            cliente.NomeUsuarioCadastro = Sessao.Usuario.Nome;
            cliente.DataAlteracao = DateTime.Now;
            cliente.NomeUsuarioAlteracao = Sessao.Usuario.Nome;
            cliente.IdUsuarioCadastro = Sessao.Usuario.ID;

            if (Id == 0)
            {
                IdPessoa = PessoasBo.Inserir(pessoa);

                cliente.IDPessoa = IdPessoa;
                Id = ClientesBo.Inserir(cliente);
            }

            pessoa.ID = IdPessoa;
            PessoasBo.Salvar(pessoa);

            cliente.ID = Id;
            ClientesBo.Salvar(cliente);

            ClientesBo.AjustarVinculo(cliente.ID);

            #region Arquivo PDF
            var url = String.Empty;
            if (LGPDArquivo != null && LGPDArquivo.ContentLength > 0)
            {
                var file = Path.Combine(Server.MapPath("~/Uploads"), String.Format("Cliente-{0}.pdf", Id.ToString()));
                LGPDArquivo.SaveAs(file);
            }
            #endregion

            // ClientesBo.Vincular();

            if (IdPreCadastro.HasValue && IdPreCadastro.Value > 0)
            {
                var preCadastro = AppPreCadastroBo.Consultar(IdPreCadastro.Value);

                if (preCadastro != null)
                {
                    preCadastro.IdCliente = Id;
                    AppPreCadastroBo.Salvar(preCadastro);

                    Fones telefone = new Fones();
                    telefone.IDPessoa = IdPessoa;
                    telefone.Numero = Util.OnlyNumbers(Telefone);
                    telefone.Principal = true;

                    TelefonesBo.Inserir(telefone);

                    Email email = new Email();
                    email.IDPessoa = IdPessoa;
                    email.Email1 = Email;
                    email.Principal = true;

                    EmailsBo.Inserir(email);
                }
            }

            var result = new { codigo = "00", id = Id, idPessoa = IdPessoa };
            return Json(result);
        }

        [HttpPost]
        public JsonResult SalvarIndicacao(Int32 Id, String Vinculo, Int32? IdTipoIndicacao, String Indicacao, Int32? IdParceiro, Int32? IdProfissional, Int32? IdCliente)
        {
            var cliente = ClientesBo.ConsultarIndicacao(Id);
            cliente.Vinculo = Vinculo;
            cliente.IdIndicacaoTipo = IdTipoIndicacao;

            var tipo = TiposIndicacoesBo.Consultar((IdTipoIndicacao ?? 0))?.Tipo;

            cliente.Indicacao = (tipo == 1 ? Indicacao : null);
            cliente.IdParceiro = (tipo == 2 ? IdParceiro : null);
            cliente.IdProfissional = (tipo == 3 ? IdProfissional : null);
            cliente.IdCliente = (tipo == 5 ? IdCliente : null);

            var nomeIndicacao = "";

            if (IdParceiro != null)
            {
                nomeIndicacao = ParceirosBo.Consultar(int.Parse(IdParceiro.ToString())).Nome;
            }
            if (IdProfissional != null)
            {
                nomeIndicacao = ProfissionaisBo.Consultar(int.Parse(IdProfissional.ToString())).Pessoas.Nome;
            }
            if (IdCliente != null)
            {
                nomeIndicacao = ClientesBo.Consultar(int.Parse(IdCliente.ToString())).Pessoas.Nome;
            }

            cliente.NomeIndicacao = nomeIndicacao;
            if (cliente.DataCadastroIndicacao == null)
            {
                cliente.DataCadastroIndicacao = DateTime.Now;
                cliente.UsuarioCadastroIndicacao = Sessao.Usuario.Nome;
            }
            cliente.DataAlteracaoIndicacao = DateTime.Now;
            cliente.UsuarioAlteracaoIndicacao = Sessao.Usuario.Nome;

            ClientesBo.SalvarIndicacao(cliente);
            ClientesBo.AjustarVinculo(cliente.ID);

            var result = new { codigo = "00", id = Id };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            List<Fones> telefone = TelefonesBo.Listar(Id);
            List<Email> email = EmailsBo.Listar(Id);
            Clientes cliente = ClientesBo.Buscar(Id);
            List<Clientes> clienteindicacoes = ClientesBo.BuscarIndicacao(int.Parse(cliente.ID.ToString()));

            string mensagens = string.Empty;

            mensagens += (cliente.Agendas.Count > 0 ? "<b>Agendamentos</b><br/>" : string.Empty);
            foreach (var agenda in cliente.Agendas)
            {
                mensagens += (mensagens.Length > 0 ? "<br/>" : string.Empty) + "Data: " + agenda.Data.ToString("dd/MM/yyyy") + " Hora: " + agenda.Hora;
            }

            mensagens += (cliente.ProcessosAutores.Count > 0 ? (mensagens.Length > 0 ? "<br/><br/>" : string.Empty) + "<b>Processos Autor</b><br/>" : string.Empty);
            foreach (var autor in cliente.ProcessosAutores)
            {
                foreach (var acao in autor.Processos.ProcessosAcoes)
                {
                    mensagens += (mensagens.Length > 0 ? "<br/>" : string.Empty) + "Número Processo: " + acao.NumeroProcesso;
                }
            }

            mensagens += (cliente.ProcessosPartes.Count > 0 ? (mensagens.Length > 0 ? "<br/><br/>" : string.Empty) + "<b>Processos Réu</b><br/>" : string.Empty);
            foreach (var reu in cliente.ProcessosPartes)
            {
                foreach (var acao in reu.Processos.ProcessosAcoes)
                {
                    mensagens += (mensagens.Length > 0 ? "<br/>" : string.Empty) + "Número Processo: " + acao.NumeroProcesso;
                }
            }

            mensagens += (cliente.Pessoas.Profissionais.Count > 0 ? (mensagens.Length > 0 ? "<br/><br/>" : string.Empty) + "<b>Indicação Profissional</b><br/>" : string.Empty);
            foreach (var profissional in cliente.Pessoas.Profissionais)
            {
                mensagens += (mensagens.Length > 0 ? "<br/>" : string.Empty) + "Nome: " + profissional.Pessoas.Nome;
            }

            mensagens += (clienteindicacoes.Count > 0 ? (mensagens.Length > 0 ? "<br/><br/>" : string.Empty) + "<b>Indicação Cliente</b><br/>" : string.Empty);
            foreach (var clienteIndicacao in clienteindicacoes)
            {
                mensagens += (mensagens.Length > 0 ? "<br/>" : string.Empty) + "Nome: " + clienteIndicacao.Pessoas.Nome;
            }

            var codigo = ClientesBo.Excluir(Id);

            if (codigo == "00")
            {
                Auditoria auditoria;

                foreach (var item in telefone)
                {
                    auditoria = new Auditoria();
                    auditoria.DataHora = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                    auditoria.Modulo = "Cadastro";
                    auditoria.Tipo = "Telefone";
                    auditoria.Acao = "Excluído";
                    auditoria.Log = String.Format("<b>Nome</b>: {0};<b>Numero</b>: {1};<b>Ramal</b>: {2};<b>TipoAparelho</b>: {3};<b>TipoLinha</b>: {4};<b>Status</b>: {5};<b>Principal</b>: {6};", cliente.Pessoas.Nome, item.Numero, item.Ramal, (item.CF == "C" ? "Celular" : "Fixo"), item.Tipo, item.Status, (item.Principal ? "Sim" : "Não"));
                    auditoria.Usuario = Sessao.Usuario.Nome;

                    AuditoriaBo.Inserir(auditoria);
                }

                foreach (var item in email)
                {
                    auditoria = new Auditoria();
                    auditoria.DataHora = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                    auditoria.Modulo = "Cadastro";
                    auditoria.Tipo = "Email";
                    auditoria.Acao = "Excluído";
                    auditoria.Log = String.Format("<b>Nome</b>: {0};<b>Email</b>: {1};<b>Status</b>: {2};<b>Principal</b>: {3};", cliente.Pessoas.Nome, item.Email1, (item.Ativo ? "Ativo" : "Inativo"), (item.Principal ? "Sim" : "Não"));
                    auditoria.Usuario = Sessao.Usuario.Nome;

                    AuditoriaBo.Inserir(auditoria);
                }

                auditoria = new Auditoria();
                auditoria.DataHora = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                auditoria.Modulo = "Cadastro";
                auditoria.Tipo = "Dados Básicos";
                auditoria.Acao = "Alterado";
                auditoria.Log = String.Format("<b>Nome</b>: {0};<b>CPFCNPJ</b>: {1};<b>RG</b>: {2};<b>CEP</b>: {3};<b>Logradouro</b>: {4};<b>Número</b>: {5};<b>Complemento</b>: {6};<b>Bairro</b>: {7};<b>Estado</b>: {8};<b>Cidade</b>: {9};<b>IM</b>: {10};<b>IE</b>: {11};<b>CNAE</b>: {12};<b>Indicação</b>: {13};", cliente.Pessoas.Nome, cliente.Pessoas.CpfCnpj, cliente.Pessoas.RG, cliente.Pessoas.Cep, cliente.Pessoas.Endereco, cliente.Pessoas.Numero, cliente.Pessoas.Complemento, cliente.Pessoas.Bairro, cliente.Pessoas.Estado, cliente.Pessoas.Municipio, cliente.InscricaoMunicipal, cliente.InscricaoEstadual, cliente.Cnae, cliente.Indicacao);
                auditoria.Usuario = Sessao.Usuario.Nome;

                AuditoriaBo.Inserir(auditoria);
            }

            var result = new { codigo = codigo, mensagens = mensagens };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar(string search = "")
        {
            var clientes = ClientesBo.Listar(search);

            var result = new { codigo = "00", clientes = clientes };
            return Json(result);
        }

        [HttpPost]
        public JsonResult ValidarDadosTelefonicos(Int64 IdPessoa)
        {
            try
            {
                var existe = TelefonesBo.Validar(IdPessoa);

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
                    telefones += "                  <input type='text' id='" + String.Format("ClienteDadoTelefonico{0}Numero", (i + 1)) + "' name='ClienteDadoTelefonicoNumero[]' class='form-control' placeholder='Informe o Número' data-msg-required='Por favor, insira o número.' data-rule-required='true' maxlength='14' value='" + (telefone.Count >= (i + 1) ? Util.FormatPhone(telefone[i].Numero) : String.Empty) + "' />";
                    telefones += "               </div>";
                    telefones += "            </div>";
                    //telefones += "            <div class='col-md-4'>";
                    //telefones += "               <div class='form-group'>";
                    //telefones += "                  <label class='titlefield'>Ramal </label>";
                    //telefones += "                  <input type='text' id='" + String.Format("ClienteDadoTelefonico{0}Ramal", (i + 1)) + "' name='ClienteDadoTelefonicoRamal[]' class='form-control' placeholder='Informe o Ramal' maxlength='10' value='" + (telefone.Count >= (i + 1) ? telefone[i].Ramal : String.Empty) + "' />";
                    //telefones += "               </div>";
                    //telefones += "            </div>";
                    //telefones += "            <div class='col-md-4'>";
                    //telefones += "               <div class='form-group'>";
                    //telefones += "                  <label class='titlefield'>Tipo Aparelho <span class='required'>*</span></label>";
                    //telefones += "                  <select id='" + String.Format("ClienteDadoTelefonico{0}TipoAparelho", (i + 1)) + "' name='ClienteDadoTelefonicoTipoAparelho[]' class='form-control' data-msg-required='Por favor, selecione o Tipo de Aparelho.' data-rule-required='true'>";
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
                    //telefones += "                  <select id='" + String.Format("ClienteDadoTelefonico{0}TipoLinha", (i + 1)) + "' name='ClienteDadoTelefonicoTipoLinha[]' class='form-control' data-msg-required='Por favor, selecione o Tipo de Linha.' data-rule-required='true'>";
                    //telefones += "                     <option value='' " + (telefone.Count == 0 ? "selected" : String.Empty) + " style='background-color: #eaeaea; font-weight: bold;'>selecione</option>";
                    //telefones += "                     <option value='Pessoal' " + (telefone.Count >= (i + 1) ? (telefone[i].Tipo == "Pessoal" ? "selected" : String.Empty) : String.Empty) + ">Pessoal</option>";
                    //telefones += "                     <option value='Comercial' " + (telefone.Count >= (i + 1) ? (telefone[i].Tipo == "Comercial" ? "selected" : String.Empty) : String.Empty) + ">Comercial</option>";
                    //telefones += "                  </select>";
                    //telefones += "               </div>";
                    //telefones += "            </div>";
                    //telefones += "            <div class='col-md-4'>";
                    //telefones += "               <div class='form-group'>";
                    //telefones += "                  <label class='titlefield'>Status <span class='required'>*</span></label>";
                    //telefones += "                  <select id='" + String.Format("ClienteDadoTelefonico{0}Status", (i + 1)) + "' name='ClienteDadoTelefonicoStatus[]' class='form-control' data-msg-required='Por favor, selecione o Status.' data-rule-required='true'>";
                    //telefones += "                     <option value='' " + (telefone.Count == 0 ? "selected" : String.Empty) + " style='background-color: #eaeaea; font-weight: bold;'>selecione</option>";
                    //telefones += "                     <option value='Ativo' " + (telefone.Count >= (i + 1) ? (telefone[i].Status == "Ativo" ? "selected" : String.Empty) : String.Empty) + ">Ativo</option>";
                    //telefones += "                     <option value='Inativo' " + (telefone.Count >= (i + 1) ? (telefone[i].Status == "Inativo" ? "selected" : String.Empty) : String.Empty) + ">Inativo</option>";
                    //telefones += "                  </select>";
                    //telefones += "               </div>";
                    //telefones += "            </div>";
                    telefones += "            <div class='col-md-4'>";
                    telefones += "               <div class='form-group'>";
                    telefones += "                  <label class='titlefield'>Principal <span class='required'>*</span></label>";
                    telefones += "                  <select id='" + String.Format("ClienteDadoTelefonico{0}Principal", (i + 1)) + "' name='ClienteDadoTelefonicoPrincipal[]' class='form-control' data-msg-required='Por favor, selecione o telefone Principal.' data-rule-required='true'>";
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
                    telefones += "                   <textarea id='" + String.Format("ClienteDadoTelefonico{0}Observacao", (i + 1)) + "' name='ClienteDadoTelefonicoObservacao[]' class='form-control' rows='1' placeholder='Descreva-se aqui ...' maxlength='50'>" + (telefone.Count >= (i + 1) ? telefone[i].Observacao : String.Empty) + "</textarea>";
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
                    telefones += "                  <label id='" + String.Format("LabelDadoTelefonico{0}ExcluirConfirmacao", (i + 1)) + "' style='font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente excluir esse Telefone?</label><br/>";
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
                #region Auditoria
                //Clientes cliente = ClientesBo.Buscar(IdPessoa);

                //Auditoria auditoria = new Auditoria();
                //auditoria.DataHora = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                //auditoria.Modulo = "Cliente";
                //auditoria.Tipo = "Telefone";
                //auditoria.Acao = "Alterado";
                //auditoria.Log = String.Format("<b>Nome</b>: {0};<b>Numero</b>: {1};<b>Ramal</b>: {2};<b>TipoAparelho</b>: {3};<b>TipoLinha</b>: {4};<b>Status</b>: {5};<b>Principal</b>: {6};<b>Observação</b>: {7};", cliente.Pessoas.Nome, Numero, Ramal, (TipoAparelho == "C" ? "Celular" : "Fixo"), TipoLinha, Status, (Principal == "1" ? "Sim" : "Não"), Observacao);
                //auditoria.Usuario = Sessao.Usuario.Nome;

                //if (Id == 0)
                //{
                //    auditoria.Acao = "Inserido";
                //}

                //AuditoriaBo.Inserir(auditoria);
                #endregion

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
                #region Auditoria
                //Fones telefone = TelefonesBo.Buscar(Id);

                //if (telefone != null)
                //{
                //    Auditoria auditoria = new Auditoria();
                //    auditoria.DataHora = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                //    auditoria.Modulo = "Cliente";
                //    auditoria.Tipo = "Telefone";
                //    auditoria.Acao = "Excluído";
                //    auditoria.Log = String.Format("<b>Nome</b>: {0};<b>Numero</b>: {1};<b>Ramal</b>: {2};<b>TipoAparelho</b>: {3};<b>TipoLinha</b>: {4};<b>Status</b>: {5};<b>Principal</b>: {6};", telefone.Pessoas.Nome, telefone.Numero, telefone.Ramal, (telefone.CF == "C" ? "Celular" : "Fixo"), telefone.Tipo, telefone.Status, (telefone.Principal ? "Sim" : "Não"));
                //    auditoria.Usuario = Sessao.Usuario.Nome;

                //    AuditoriaBo.Inserir(auditoria);
                //}
                #endregion

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
        public JsonResult ValidarDadosEmails(Int64 IdPessoa)
        {
            try
            {
                var existe = EmailsBo.Validar(IdPessoa);

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
                    emails += "                  <input type='text' id='" + String.Format("ClienteDadoEmail{0}Email", (i + 1)) + "' name='ClienteDadoEmailEmail[]' class='form-control' placeholder='Informe o E-mail' data-msg-required='Por favor, insira o e-=mail.' data-rule-required='true' maxlength='100' value='" + (email.Count >= (i + 1) ? email[i].Email1 : String.Empty) + "' />";
                    emails += "               </div>";
                    emails += "            </div>";
                    //emails += "            <div class='col-md-2'>";
                    //emails += "               <div class='form-group'>";
                    //emails += "                  <label class='titlefield'>Status <span class='required'>*</span></label>";
                    //emails += "                  <select id='" + String.Format("ClienteDadoEmail{0}Status", (i + 1)) + "' name='ClienteDadoEmailStatus[]' class='form-control' data-msg-required='Por favor, selecione o Status.' data-rule-required='true'>";
                    //emails += "                     <option value='' " + (email.Count == 0 ? "selected" : String.Empty) + " style='background-color: #eaeaea; font-weight: bold;'>selecione</option>";
                    //emails += "                     <option value='1' " + (email.Count >= (i + 1) ? (email[i].Ativo ? "selected" : String.Empty) : String.Empty) + ">Ativo</option>";
                    //emails += "                     <option value='0' " + (email.Count >= (i + 1) ? (!email[i].Ativo ? "selected" : String.Empty) : String.Empty) + ">Inativo</option>";
                    //emails += "                  </select>";
                    //emails += "               </div>";
                    //emails += "            </div>";
                    emails += "            <div class='col-md-2'>";
                    emails += "               <div class='form-group'>";
                    emails += "                  <label class='titlefield'>Principal <span class='required'>*</span></label>";
                    emails += "                  <select id='" + String.Format("ClienteDadoEmail{0}Principal", (i + 1)) + "' name='ClienteDadoEmailPrincipal[]' class='form-control' data-msg-required='Por favor, selecione o e-mail Principal.' data-rule-required='true'>";
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
                    emails += "                  <label id='" + String.Format("LabelDadoEmail{0}ExcluirConfirmacao", (i + 1)) + "' style='font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente excluir esse E-mail?</label><br/>";
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
                #region Auditoria
                //Clientes cliente = ClientesBo.Buscar(IdPessoa);

                //Auditoria auditoria = new Auditoria();
                //auditoria.DataHora = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                //auditoria.Modulo = "Cliente";
                //auditoria.Tipo = "Email";
                //auditoria.Acao = "Alterado";
                //auditoria.Log = String.Format("<b>Nome</b>: {0};<b>Email</b>: {1};<b>Status</b>: {2};<b>Principal</b>: {3};<b>Observacao</b>: {4};", cliente.Pessoas.Nome, Email, (Status == "1" ? "Ativo" : "Inativo"), (Principal == "1" ? "Sim" : "Não"), Observacao);
                //auditoria.Usuario = Sessao.Usuario.Nome;

                //if (Id == 0)
                //{
                //    auditoria.Acao = "Inserido";
                //}

                //AuditoriaBo.Inserir(auditoria);
                #endregion

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
                #region Auditoria
                //Email email = EmailsBo.Buscar(Id);

                //if (email != null)
                //{
                //    Auditoria auditoria = new Auditoria();
                //    auditoria.DataHora = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                //    auditoria.Modulo = "Cliente";
                //    auditoria.Tipo = "Email";
                //    auditoria.Acao = "Excluído";
                //    auditoria.Log = String.Format("<b>Nome</b>: {0};<b>Email</b>: {1};<b>Status</b>: {2};<b>Principal</b>: {3};", email.Pessoas.Nome, email.Email1, (email.Ativo ? "Ativo" : "Inativo"), (email.Principal ? "Sim" : "Não"));
                //    auditoria.Usuario = Sessao.Usuario.Nome;

                //    AuditoriaBo.Inserir(auditoria);
                //}
                #endregion

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

        [HttpPost]
        public JsonResult ExcluirPDF(String Arquivo)
        {
            try
            {
                var file = Path.Combine(Server.MapPath("~/Uploads"), Arquivo);
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
        public JsonResult ValidarCPFCNPJExistente(String CPFCNPJ, Int64? IdCliente = 0)
        {
            try
            {
                var response = "success";

                var cliente = ClientesBo.ConsultarCPFCNPJ(Util.OnlyNumbers(CPFCNPJ), IdCliente.Value);
                if (cliente.Count > 0)
                {
                    response = "error";
                }

                var result = new { response = response };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult BuscarEmails(Int32 Id)
        {
            var idPessoa = ClientesBo.Consultar(Id).IDPessoa;
            var clientes = ClientesBo.ConsultarEmails(idPessoa);
            var result = new { codigo = "00", clientes = clientes };
            return Json(result);
        }

        [HttpPost]
        public JsonResult ConsultarSede(Int32 Id)
        {
            var sede = "";
            var idSede = ClientesBo.Consultar(Id).IdSede;
            if (idSede != null)
            {
                var sedes = SedesBo.Consultar(int.Parse(idSede.ToString()));
                sede = sedes.Nome;
            }
            var result = new { codigo = "00", sede = sede };
            return Json(result);
        }

    }
}