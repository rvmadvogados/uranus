using Newtonsoft.Json;
using Sigman.Domain.Entities;
using Sigman.NFSe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Uranus.Business;
using Uranus.Common;
using Uranus.Suite;

namespace Uranus.Site.Controllers
{
    public class NotasServicosController : Controller
    {
        #region Nota de Servico
        public ActionResult Index(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = NotasServicosBO.Listar("NS", search);
                ViewBag.search = search;
                return View(model);
            }
        }


        public ActionResult GetClientsList(string search, bool filter = false, int page = 1)
        {
            var clientes = ClientesBo.GetClientsList(search, filter);
            var total = clientes.Count();

            if (!(string.IsNullOrEmpty(search) || string.IsNullOrWhiteSpace(search)))
            {
                clientes = clientes.Where(x => x.text.ToLower().StartsWith(search.ToLower())).Take(page * 10).ToList();
            }

            return Json(new { clientes = clientes, total = total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProdutosSaida(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = NotasServicosBO.Listar("PEF", search);
                ViewBag.search = search;
                return View(model);
            }
        }

        public ActionResult ServicosSaida(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = NotasServicosBO.ListarReceitas("SEF", search);
                ViewBag.search = search;
                return View(model);
            }
        }

        //public PartialViewResult VisualizarSaida(Int64 Id)
        //{
        //    var model = NotasFiscaisBO.Consultar(Id);
        //    return PartialView(model);
        //}

        [HttpPost]
        public JsonResult ConsultarServico(Int64 Id)
        {
            var nota = NotasServicosBO.ConsultarArray(Id);
            var result = new { codigo = "00", nota = nota };
            return Json(result);
        }

        //[HttpPost]
        //public JsonResult CalcularNotaFiscal(Int64 Id)
        //{
        //    NotasFiscaisBO.CalcularClientesNotas(Id);
        //    NotasFiscaisBO.AlterarClientesNotas(Id);

        //    var nota = NotasFiscaisBO.ConsultarArray(Id);
        //    var result = new { codigo = "00", nota = nota };
        //    return Json(result);
        //}


        [HttpPost]
        public JsonResult SalvarServico(Int64 Id, Int32 IdEmpresa, Int64 NumeroNota, String IdReceita, String NumeroDocumento, String DataEmissao, Int32 IdStatus, Int32 IdCliente, Int32 IdClienteEmail, String DescricaoServico,
                                        String ValorServico, String ValorLiquido, String PercentualIssqn, String ValorIssqn, String PercentualIR, String ValorIR, String PercentualCofins, String ValorCofins, String PercentualPis, String ValorPis,
                                        String PercentualCSLL, String ValorCSLL, Int32 Plano, String LocalPagamento, String Observacao, Int32 IdFomraPagamento, String ReterIssqn, Int64? NumeroReferencia)
        {
            try
            {
                var codigo = "00";
                var status = StatusBO.Consultar(IdStatus).TipoAcao;

                long? numeronota = 0;
                if (NumeroNota > 0)
                {
                    numeronota = NumeroNota;
                }

                long? numeroreferencia = 0;
                if (NumeroReferencia > 0)
                {
                    numeroreferencia = NumeroReferencia;
                }
                else
                {
                    var numeroreferenciaNF = NotasServicosBO.ConsultarUltimaReferencia(IdEmpresa).NumeroReferencia;

                    if (numeroreferenciaNF != null && numeroreferenciaNF != 0)
                    {
                        numeroreferencia = numeroreferenciaNF + 1;
                    }
                    else
                    {
                        numeroreferencia = 1;
                    }
                }

                Uranus.Domain.NotasServicos NFSe = new Uranus.Domain.NotasServicos();
                NFSe.Id = Id;
                NFSe.IdEmpresa = IdEmpresa;
                NFSe.IdReceita = long.Parse(IdReceita);
                NFSe.NumeroNota = numeronota;
                NFSe.NumeroReferencia = numeroreferencia;
                if (NumeroDocumento.Length > 0)
                {
                    NFSe.NumeroDocumento = long.Parse(NumeroDocumento);
                }
                if (DataEmissao != null && DataEmissao.Trim().Length > 0)
                {
                    NFSe.Data = DateTime.Parse(DataEmissao);
                }
                NFSe.IdStatus = IdStatus;


                NFSe.IdCliente = IdCliente;
                NFSe.IdClienteEmail = IdClienteEmail;
                NFSe.DescricaoServico = DescricaoServico;

                NFSe.ValorServico = 0;
                if (ValorServico.Length > 0)
                {
                    NFSe.ValorServico = Decimal.Parse(ValorServico);
                }

                NFSe.ValorLiquido = 0;
                if (ValorLiquido.Length > 0)
                {
                    NFSe.ValorLiquido = Decimal.Parse(ValorLiquido);
                }

                NFSe.PercentualIssqn = 0;
                if (PercentualIssqn.Length > 0 && PercentualIssqn != "0,00")
                {
                    NFSe.PercentualIssqn = int.Parse(PercentualIssqn);
                }

                NFSe.ValorIssqn = 0;
                if (ValorIssqn.Length > 0 && ValorIssqn != "0,00")
                {
                    NFSe.ValorIssqn = Decimal.Parse(ValorIssqn);
                }

                NFSe.PercentualIR = 0;
                if (PercentualIR.Length > 0 && PercentualIR != "0,00")
                {
                    NFSe.PercentualIR = int.Parse(PercentualIR);
                }

                NFSe.ValorIR = 0;
                if (ValorIR.Length > 0 && ValorIR != "0,00")
                {
                    NFSe.ValorIR = Decimal.Parse(ValorIR);
                }

                NFSe.PercentualCofins = 0;
                if (PercentualCofins.Length > 0 && PercentualCofins != "0,00")
                {
                    NFSe.PercentualCofins = int.Parse(PercentualCofins);
                }

                NFSe.ValorCofins = 0;
                if (ValorCofins.Length > 0 && ValorCofins != "0,00")
                {
                    NFSe.ValorCofins = Decimal.Parse(ValorCofins);
                }

                NFSe.PercentualPis = 0;
                if (PercentualPis.Length > 0 && PercentualPis != "0,00")
                {
                    NFSe.PercentualPis = int.Parse(PercentualPis);
                }

                NFSe.ValorPis = 0;
                if (ValorPis.Length > 0 && ValorPis != "0,00")
                {
                    NFSe.ValorPis = Decimal.Parse(ValorPis);
                }

                NFSe.PercentualCSLL = 0;
                if (PercentualCSLL.Length > 0 && PercentualCSLL != "0,00")
                {
                    NFSe.PercentualCSLL = int.Parse(PercentualCSLL);
                }

                NFSe.ValorCSLL = 0;
                if (ValorCSLL.Length > 0 && ValorCSLL != "0,00")
                {
                    NFSe.ValorCSLL = Decimal.Parse(ValorCSLL);
                }

                NFSe.ReterIssqn = ReterIssqn;

                NFSe.Plano = Plano;

                if (LocalPagamento.Length > 0)
                {
                    NFSe.LocalPagamento = int.Parse(LocalPagamento);
                }
                NFSe.Observacao = Observacao;
                NFSe.IdFormaPagamento = IdFomraPagamento;
                if (status == "NS")
                {
                    codigo = "11";
                }

                if (status == "SC")
                {
                    NFSe.Cancelada = "C";
                }

                NFSe.DataCadastro = DateTime.Now;
                NFSe.NomeUsuarioCadastro = Sessao.Usuario.Nome;
                NFSe.DataAlteracao = DateTime.Now;
                NFSe.NomeUsuarioAlteracao = Sessao.Usuario.Nome;

                NotasServicosBO.Salvar(NFSe);

                //NotasServicosBO.SalvarParcelasNotasServicos(NFSe.Id);

                //NotasServicosBO.CalcularClientesServicos(NFSe.Id);
                //NotasServicosBO.AlteraClientesNotasServicos(NFSe.Id);

                var result = new { codigo = codigo };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { codigo = "99" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult CarregarParcelasNotaServico(Int64 IdNotaServico)
        {
            try
            {
                var parcela = NotasServicosBO.ListarParcelas(IdNotaServico);
                String parcelas = String.Empty;

                if (parcela.Count > 0)
                {
                    parcelas += "<div class='accordion' id='accordionParcelas' role='tablist' aria-multiselectable='true' style='border-bottom: 1px solid rgba(0,0,0,.125) !important;'>";
                    parcelas += "    <div class='card z-depth-0 bordered'>";
                    parcelas += "        <a class='card-header' role='tab' id='headingOneParcelas' data-toggle='collapse' data-parent='#accordionParcelas' href='#collapseOneParcelas' aria-expanded='true' aria-controls='collapseOne'>";
                    parcelas += "            <h5 class='mb-0'>";
                    parcelas += "                <button class='btn btn-link' type='button'>";
                    parcelas += "                    Visualizar Parcelas";
                    parcelas += "                </button>";
                    parcelas += "            </h5>";
                    parcelas += "        </a>";
                    parcelas += "        <div id='collapseOneParcelas' class='panel-collapse collapse in' role='tabpanel' aria-labelledby='headingOne'>";
                    parcelas += "            <div class='panel-body'>";
                    parcelas += "                <table class='table table-striped' style='width: 30% !important; font-size: 12px !important;'>";
                    parcelas += "                    <thead>";
                    parcelas += "                        <tr>";
                    parcelas += "                            <th>Parcela</th>";
                    parcelas += "                            <th>Vencimento</th>";
                    parcelas += "                            <th class='right'>Prazo</th>";
                    parcelas += "                            <th class='right'>Valor R$</th>";
                    parcelas += "                        </tr>";
                    parcelas += "                    </thead>";
                    parcelas += "                    <tbody>";

                    for (int i = 0; i < parcela.Count; i++)
                    {
                        parcelas += "                        <tr>";
                        parcelas += "                            <th scope='row'>" + Util.AddLeadingZeros(parcela[i].Parcela, 2) + "</th>";
                        parcelas += "                            <td>" + parcela[i].Vencimento.Value.ToString("dd/MM/yyyy") + "</td>";
                        parcelas += "                            <td class='right'>" + parcela[i].Prazo + "</td>";
                        parcelas += "                            <td class='right'>" + String.Format("{0:##,##0.00}", parcela[i].ValorParcela) + "</td>";
                        parcelas += "                        </tr>";
                    }

                    parcelas += "                    </tbody>";
                    parcelas += "                </table>";
                    parcelas += "            </div>";
                    parcelas += "        </div>";
                    parcelas += "    </div>";
                    parcelas += "</div>";
                }
                else
                {
                    parcelas = "As parcelas só poderam ser cadastradas após a Nota Fiscal ser salva.";
                }

                var result = new { response = "success", parcelas = parcelas };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        #region NFSe
        public ActionResult Gerenciar(string FiltrarNumero = "", string FiltrarCPFCNPJ = "", string FiltrarCliente = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = NotasServicosBO.ListarNotasServicos(FiltrarNumero, FiltrarCPFCNPJ.Replace(".", string.Empty).Replace("/", string.Empty).Replace("-", string.Empty), FiltrarCliente);
                ViewBag.FiltrarCPFCNPJ = FiltrarCPFCNPJ;
                ViewBag.FiltrarCliente = FiltrarCliente;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult EmitirNFS(Int64 Id, Int32 IdStatus)
        {
            try
            {
                var mensagemErro = string.Empty;
                var vencimentoparcela = string.Empty;
                var codigo = "00";
                var arquivo = string.Empty;
                var status = StatusBO.Consultar(IdStatus).TipoAcao;


                if (status == "NS")
                {
                    Uranus.Domain.NotasServicos notaservico = NotasServicosBO.ConsultarNFS(Id);
                    // var IdCliente = int.Parse(notafiscal.IdCliente.ToString());
                    Uranus.Domain.Empresa empresa = EmpresaBo.Buscar(int.Parse(notaservico.IdEmpresa.ToString()));
                    Uranus.Domain.Clientes cliente = ClientesBo.Consultar(notaservico.IdCliente.Value);
                    Uranus.Domain.Clientes pessoa = PessoasBo.ConsultarPessoa(cliente.ID);
                    string clienteTelefone = PessoasBo.BuscarForne(pessoa.ID).Numero;
                    Uranus.Domain.Email email = PessoasBo.ConsultarEmail(notaservico.IdClienteEmail.Value);
                    List<Uranus.Domain.NotasServicosParcelas> duplicatasnfs = NotasServicosBO.ListarParcelas(Id);

                    NFSeFocus nFsFocus = new NFSeFocus(null, null);
                    if (empresa.Ambiente == "H")
                    {
                        nFsFocus = new NFSeFocus(empresa.Url_homologacao, empresa.TokenHomologacao);
                    }
                    else
                    {
                        nFsFocus = new NFSeFocus(empresa.Url_Producao, empresa.TokenProducao);
                    }

                    var URL = empresa.Url_Producao;
                    if (empresa.Ambiente == "H")
                    {
                        URL = empresa.Url_homologacao;
                    }

                    string basePath = System.AppDomain.CurrentDomain.BaseDirectory.Replace("Uranus.Site\\", string.Empty);
                    string configurationPath = empresa.PastaNotas;
                    if (!Directory.Exists(basePath) || !Directory.Exists(string.Concat(basePath, configurationPath)))
                    {
                        Directory.CreateDirectory(string.Concat(basePath, configurationPath));
                    }
                    string fullPath = string.Concat(basePath, configurationPath);

                    if (!Directory.Exists(fullPath) || !Directory.Exists(string.Concat(fullPath, "\\NFS")))
                    {
                        Directory.CreateDirectory(string.Concat(fullPath, "\\NFS"));
                        Directory.CreateDirectory(string.Concat(fullPath, "\\NFS\\Xml"));
                        Directory.CreateDirectory(string.Concat(fullPath, "\\NFS\\Danfe"));
                    }

                    var CaminhoXml = string.Concat(fullPath + "\\NFS\\Xml");
                    var CaminhoDanfe = string.Concat(fullPath + "\\NFS\\Danfe");

                    #region DadosNFS
                    DadosNFS dados = new DadosNFS();

                    dados.notaservico = JsonConvert.DeserializeObject<Sigman.Domain.Entities.NotasServicos>(JsonConvert.SerializeObject(notaservico, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                    dados.empresa = JsonConvert.DeserializeObject<Sigman.Domain.Entities.Empresas>(JsonConvert.SerializeObject(empresa, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

//                    cliente.Emai.Clear();
                    dados.cliente = JsonConvert.DeserializeObject<Sigman.Domain.Entities.Clientes>(JsonConvert.SerializeObject(cliente, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                    dados.clienteTelefone = clienteTelefone;

                    dados.clientesemails = email.Email1;

                    foreach (var item in duplicatasnfs)
                    {
                        item.NotasServicos.NotasServicosParcelas.Clear();
                    }
                    dados.duplicatasnfs = JsonConvert.DeserializeObject<List<Sigman.Domain.Entities.NotasServicosParcelas>>(JsonConvert.SerializeObject(duplicatasnfs, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                    dados.CaminhoXml = CaminhoXml;
                    dados.CaminhoDanfe = CaminhoDanfe;
                    dados.URL = URL;
                    #endregion

                    RetornoMensagem retornoMensagem = nFsFocus.EmitirNFS(dados);

                    //using (WebClient client = new WebClient())
                    //{

                    //    var xmlData = client.DownloadData(URL);
                    //    var xmlCode = Encoding.UTF8.GetString(xmlData);

                    //}

                    if (retornoMensagem.status == "autorizado")
                    {
                        //var notafiscal = NotasFiscaisBO.ConsultarNFe(Id);
                        //                        notaservico.NumeroChaveNfe = retornoMensagem.NumeroChaveNfe;

                        notaservico.NumeroNota = long.Parse(retornoMensagem.Numero);
                        notaservico.IdStatus = 13;
                        NotasServicosBO.Salvar(notaservico);
                        var receita = FinanceiroReceitasBo.ConsultarReceita(long.Parse(notaservico.IdReceita.ToString()));
                        receita.IdNota = notaservico.Id;
                        NotasServicosBO.AlteraClientesNotasServicos(notaservico.Id);

                        arquivo = Imprimir(notaservico.Id, "NFS");
                    }
                    else if (retornoMensagem != null && retornoMensagem.status == "processando_autorizacao")
                    {
                        codigo = "11";
                        mensagemErro = retornoMensagem.mensagem;
                    }
                    else if (retornoMensagem != null && retornoMensagem.status == "cancelado")
                    {
                        codigo = "22";
                        mensagemErro = retornoMensagem.mensagem;
                    }
                    else if (retornoMensagem != null && retornoMensagem.status == "erro_autorizacao")
                    {
                        codigo = "33";
                        mensagemErro = retornoMensagem.mensagem;
                    }
                    else if (retornoMensagem != null && retornoMensagem.status == "denegado")
                    {
                        codigo = "44";
                        mensagemErro = retornoMensagem.mensagem;
                    }
                    else
                    {
                        codigo = "88";
                        mensagemErro = retornoMensagem.mensagem;
                    }

                }
                else if (status == "NFC")
                {
                    //                    NFeFocus nFeFocus = new NFeFocus();
                    //                    nFeFocus.cancelarNFe(NumeroNota.ToString());
                }

                var result = new { codigo = codigo, mensagemErro = mensagemErro, arquivo = arquivo };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { codigo = "99" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult EnviarNFS(long Id)
        {
            try
            {
                Uranus.Domain.NotasServicos nota = NotasServicosBO.Consultar(Id);
                Uranus.Domain.Empresa empresa = EmpresaBo.Consultar(nota.IdEmpresa);
                string configurationPath = empresa.PastaNotas;
                string basePath = System.AppDomain.CurrentDomain.BaseDirectory.Replace("Uranus.Site\\", string.Empty);
                string fullPath = string.Concat(basePath, configurationPath);

                List<Attachment> attach = new List<Attachment>();

                var nomeArquivoPdf = string.Format(@"{0}\{1}", string.Concat(fullPath + "\\NFS\\Danfe"), string.Concat("NFS-", Util.AddLeadingZeros(nota.NumeroNota.Value, 6), ".pdf"));
                if (System.IO.File.Exists(nomeArquivoPdf))
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(nomeArquivoPdf);
                    MemoryStream ms = new MemoryStream(bytes);
                    Attachment attachment = new Attachment(ms, string.Concat("NFe-", Util.AddLeadingZeros(nota.NumeroNota.Value, 6), ".pdf"));
                    attach.Add(attachment);
                }

                var nomeArquivoXml = string.Format(@"{0}\{1}", string.Concat(fullPath + "\\NFS\\Xml"), string.Concat("NFS-", Util.AddLeadingZeros(nota.NumeroNota.Value, 6), ".xml"));
                if (System.IO.File.Exists(nomeArquivoXml))
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(nomeArquivoXml);
                    MemoryStream ms = new MemoryStream(bytes);
                    Attachment attachment = new Attachment(ms, string.Concat("NFS-", Util.AddLeadingZeros(nota.NumeroNota.Value, 6), ".xml"));
                    attach.Add(attachment);
                }

                Uranus.Domain.Clientes cliente = ClientesBo.Consultar(nota.IdCliente.Value);
                Uranus.Domain.Email email = PessoasBo.ConsultarEmail(nota.IdClienteEmail.Value);

                var assunto = "Nota Fiscal " + string.Concat("NFS-", Util.AddLeadingZeros(long.Parse(nota.NumeroNota.ToString()), 6));
                var corpoEmail = "<h3>Segue em anexo Nota fiscal número " + string.Concat("NFS - ", Util.AddLeadingZeros(long.Parse(nota.NumeroNota.ToString()), 6)) + "</h3>";

                var emailremetente = empresa.Email;
                var passwordremetente = empresa.Senha;
                Mail.Send(emailremetente, passwordremetente, email.Email1, assunto, corpoEmail);

                if (Sigman.Common.Mail.Send(empresa.SMTPEndereco, email.Email1, assunto, corpoEmail, empresa.SMTPRemetente, empresa.SMTPUsuario, empresa.SMTPSenha, empresa.SMTPPorta, attach))
                {
                    var result = new { codigo = "00" };
                    return Json(result);
                }
                else
                {
                    var result = new { codigo = "88" };
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                var result = new { codigo = "99" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult ImprimirNFS(long Id)
        {
            try
            {
                string arquivo = Imprimir(Id, "NS");

                var result = new { codigo = "00", arquivo = arquivo };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { codigo = "99" };
                return Json(result);
            }
        }

        public string Imprimir(long Id, string Tipo)
        {
            Uranus.Domain.NotasServicos nota = NotasServicosBO.Consultar(Id);
            Uranus.Domain.Empresa empresa = EmpresaBo.Consultar(nota.IdEmpresa);
            string configurationPath = empresa.PastaNotas;
            string basePath = System.AppDomain.CurrentDomain.BaseDirectory.Replace("Uranus.Site\\", string.Empty);
            string fullPath = string.Concat(basePath, configurationPath);
            var nomeArquivoPdf = "";
            nomeArquivoPdf = string.Format(@"{0}\{1}", string.Concat(fullPath + "\\NFS\\Danfe"), string.Concat("NFSn-", Util.AddLeadingZeros(nota.NumeroNota.Value, 6), ".pdf"));


            string arquivo = string.Empty;

            if (System.IO.File.Exists(nomeArquivoPdf))
            {
                byte[] objByte = System.IO.File.ReadAllBytes(nomeArquivoPdf);
                arquivo = Convert.ToBase64String(objByte);
            }

            return arquivo;
        }

        //public byte[] GerarBoleto(Uranus.Domain.Empresas empresa, Uranus.Domain.Clientes cliente, Uranus.Domain.ClientesEnderecos clienteEndereco, Uranus.Domain.ClientesNotasParcelas parcela, string NumeroDocumento)
        //{
        //    var banco = BancosBO.Buscar();
        //    BoletoBancario boletos = new BoletoBancario();

        //    // Banco, Cedente, Conta Corrente
        //    boletos.Banco = Banco.Instancia(341);
        //    boletos.Banco.Cedente = new Cedente
        //    {
        //        CPFCNPJ = empresa.Cnpj,
        //        Nome = empresa.Nome,
        //        Observacoes = "",
        //        ContaBancaria = new ContaBancaria
        //        {
        //            Agencia = banco.Agencia,
        //            //DigitoAgencia = digitoAgencia,
        //            //OperacaoConta = operacaoConta,
        //            Conta = banco.Conta,
        //            //DigitoConta = banco.DigitoConta,
        //            CarteiraPadrao = banco.Carteira,
        //            VariacaoCarteiraPadrao = null,
        //            TipoCarteiraPadrao = (TipoCarteira)1,
        //            TipoFormaCadastramento = (TipoFormaCadastramento)1,
        //            TipoImpressaoBoleto = (TipoImpressaoBoleto)1,
        //            TipoDocumento = (TipoDocumento)1
        //        },
        //        Codigo = banco.Conta,
        //        CodigoDV = "0",
        //        //CodigoTransmissao = codigoTransmissao,
        //        Endereco = new Endereco
        //        {
        //            LogradouroEndereco = empresa.Endereco,
        //            LogradouroNumero = empresa.Numero,
        //            LogradouroComplemento = empresa.Complemento,
        //            Bairro = empresa.Bairro,
        //            Cidade = empresa.Cidade,
        //            UF = empresa.Estado,
        //            CEP = empresa.Cep
        //        }
        //    };
        //    boletos.Banco.FormataCedente();

        //    // Novo boleto:
        //    var boleto = new Boleto(boletos.Banco);

        //    var sacado = new Sacado
        //    {
        //        CPFCNPJ = cliente.CpfCnpj,
        //        Nome = cliente.Nome,
        //        Observacoes = "",
        //        Endereco = new Endereco
        //        {
        //            LogradouroEndereco = clienteEndereco.Endereco,
        //            LogradouroNumero = clienteEndereco.Numero,
        //            LogradouroComplemento = clienteEndereco.Complemento,
        //            Bairro = clienteEndereco.Bairro,
        //            Cidade = clienteEndereco.Municipio,
        //            UF = clienteEndereco.Estado,
        //            CEP = clienteEndereco.CEP
        //        }
        //    };
        //    boleto.Sacado = sacado;

        //    boleto.NumeroDocumento = NumeroDocumento;
        //    //boleto.NumeroControleParticipante = nrControle;
        //    boleto.NossoNumero = EmpresasBO.BuscarNossoNumero(empresa.ID).ToString();
        //    boleto.DataEmissao = DateTime.Now;
        //    boleto.DataProcessamento = DateTime.Now;
        //    boleto.DataVencimento = parcela.Vencimento.Value;
        //    boleto.ValorTitulo = parcela.ValorParcela.Value;
        //    boleto.Aceite = "S";
        //    boleto.EspecieDocumento = TipoEspecieDocumento.DM;
        //    //boleto.EspecieDocumento = Utils.ToEnum<TipoEspecieDocumento>(siglaEspecieDocumento, true, TipoEspecieDocumento.OU);

        //    //boleto.DataDesconto = dataDesconto;
        //    //boleto.ValorDesconto = valorDesconto;

        //    //boleto.DataMulta = dataMulta;
        //    //boleto.PercentualMulta = percMulta;
        //    //boleto.ValorMulta = valorMulta;

        //    //boleto.DataJuros = dataJuros;
        //    //boleto.PercentualJurosDia = percJuros;
        //    //boleto.ValorJurosDia = valorJuros;

        //    boleto.MensagemInstrucoesCaixa = "PREFERENCIALMENTE NAS CASAS LOTÉRICAS ATÉ O VALOR LIMITE.";
        //    //boleto.MensagemArquivoRemessa = mensagemRemessa;
        //    //boleto.CodigoInstrucao1 = instrucao1;
        //    //boleto.ComplementoInstrucao1 = instrucao1Aux;
        //    //boleto.CodigoInstrucao2 = instrucao2;
        //    //boleto.ComplementoInstrucao2 = instrucao2Aux;
        //    //boleto.CodigoInstrucao3 = instrucao3;
        //    //boleto.ComplementoInstrucao3 = instrucao3Aux;

        //    //boleto.CodigoProtesto = (TipoCodigoProtesto)codigoProtesto;
        //    //boleto.DiasProtesto = diasProtesto;

        //    //boleto.CodigoBaixaDevolucao = (TipoCodigoBaixaDevolucao)codigoBaixaDevolucao;
        //    //boleto.DiasBaixaDevolucao = diasBaixaDevolucao;


        //    boleto.TextHTML = "<h1>Aqui fica o texto da empresa para o seu cliente, que precisa ser formatado em HTML.</h1>";

        //    boleto.ValidarDados();
        //    boletos.Boleto = boleto;

        //    HtmlToImageConverter htmlToImageConv = new HtmlToImageConverter();
        //    var imageBytes = htmlToImageConv.GenerateImage(boletos.MontaHtml(), NReco.ImageGenerator.ImageFormat.Bmp.ToString());

        //    MemoryStream memoryStream = new MemoryStream();
        //    iTextSharp.text.Document document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 23f, 23f, 20, 20f);

        //    iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageBytes);
        //    image.ScaleToFit(820f, 1200f);
        //    image.SetDpi(300, 300);

        //    iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, memoryStream);
        //    document.Open();
        //    document.Add(image);
        //    document.Close();
        //    byte[] bytes = memoryStream.ToArray();
        //    memoryStream.Close();

        //    parcela.Status = "E";
        //    parcela.Arquivo = Convert.ToBase64String(bytes);
        //    parcela.NossoNumero = boleto.NossoNumero;

        //    ClientesNotasParcelasBO.Salvar(parcela);

        //    return bytes;
        //}
        #endregion

        private void SendToPrinter(string fileToPrint, string printerName)
        {
            using (Process p = new Process())
            {
                p.StartInfo = new ProcessStartInfo()
                {
                    CreateNoWindow = true,
                    Verb = "print",
                    FileName = fileToPrint
                };
                p.Start();
            }
        }

        public ActionResult CancelarNotaServico(Int64 IdNota, String Justificativa)
        {
            try
            {
                if (Sessao.Usuario == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    var notaservico = NotasServicosBO.Consultar(IdNota);
                    var numeronota = long.Parse(notaservico.NumeroNota.ToString());
                    var numnota = "NFSC-" + numeronota.ToString().PadLeft(10, '0');
                    var numeroreferencia = long.Parse(notaservico.NumeroReferencia.ToString());

                    Uranus.Domain.Empresa empresa = EmpresaBo.Consultar(notaservico.IdEmpresa);
                    //#if (DEBUG)
                    //                    NFCeFocus nFCeFocus = new NFCeFocus(empresa.Url_homologacao, empresa.TokenHomologacao);
                    //#else
                    //                    NFCeFocus nFCeFocus = new NFCeFocus(empresa.Url_Producao, empresa.TokenProducao);
                    //#endif

                    NFSeFocus nFsFocus = new NFSeFocus(null, null);
                    if (empresa.Ambiente == "H")
                    {
                        nFsFocus = new NFSeFocus(empresa.Url_homologacao, empresa.TokenHomologacao);
                    }
                    else
                    {
                        nFsFocus = new NFSeFocus(empresa.Url_Producao, empresa.TokenProducao);
                    }

                    var nota = new NFSeModel();

                    var resultadoCancelaNota = nFsFocus.CancelarNFS(Util.AddLeadingZeros(numeroreferencia, 10).ToString(), Justificativa);


                    var NFS = Task.Run(() => nFsFocus.consultarNFSe(Util.AddLeadingZeros(numeroreferencia, 10)));
                    NFS.Wait(5000);

                    var codigo = "00";
                    var mensagemErro = string.Empty;

                    if (NFS != null && NFS.Result.status == "cancelado")
                    {
                        //mensagemErro = (resultadoCancelaNota.mensagem_sefaz ?? resultadoCancelaNota.mensagem);

                        var numerocupom = numeronota;
                        var urlxml = "https://api.focusnfe.com.br" + NFS.Result.caminho_xml_cancelamento;
                        if (empresa.Ambiente == "H")
                        {
                            urlxml = "https://homologacao.focusnfe.com.br" + NFS.Result.caminho_xml_cancelamento;
                        }

                        using (WebClient client = new WebClient())
                        {
                            var xmlData = client.DownloadData(urlxml);
                            var xmlCode = Encoding.UTF8.GetString(xmlData);

                            string basePath = System.AppDomain.CurrentDomain.BaseDirectory.Replace("Uranus.Site\\", string.Empty);
                            string configurationPath = empresa.PastaNotas;
                            if (!Directory.Exists(basePath) || !Directory.Exists(string.Concat(basePath, configurationPath)))
                            {
                                Directory.CreateDirectory(string.Concat(basePath, configurationPath));
                            }
                            ////        //                            string configurationPath = ConfigurationManager.AppSettings["PathUploadImportacao"];
                            string fullPath = string.Concat(basePath, configurationPath);

                            System.IO.File.WriteAllText(string.Format(@"{0}\{1}", string.Concat(fullPath, "\\NFS\\XML"), string.Concat(numnota, ".xml")), xmlCode.ToString());

                        }
                    }
                    //else if (resultadoEnvioNota != null && resultadoEnvioNota.status == "erro_cancelamento")
                    else
                    {
                        codigo = "88";
                        var mensagens = string.Empty;

                        foreach (var erro in NFS.Result.erros)
                        {
                            mensagens += erro.mensagem + " - ";
                        }

                        mensagemErro = mensagens;
                    }
                    //else if (resultadoEnvioNota != null && resultadoEnvioNota.codigo == "nao_encontrado")
                    //{
                    //    codigo = "89";
                    //    mensagemErro = resultadoEnvioNota.mensagem;
                    //}

                    var result = new { codigo = codigo, mensagem = mensagemErro };
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                var result = new { codigo = "99", mensagem = ex.Message };
                return Json(result);
            }
        }
        #endregion
    }
}
