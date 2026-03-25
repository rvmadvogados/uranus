using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Sigman.Common;
using Sigman.Domain.Entities;
//using Sigman.NFe;

namespace Sigman.NFSe
{
    #region Entities
    #region NFSe
    public class Endereco
    {
        public string logradouro { get; set; }
        public string numero { get; set; }
        public string complemento { get; set; }
        public string bairro { get; set; }
        public string codigo_municipio { get; set; }
        public string cidade { get; set; }
        public string uf { get; set; }
        public string cep { get; set; }
    }

    public class Prestador
    {
        public string cnpj { get; set; }
        public string inscricao_municipal { get; set; }
        public string codigo_municipio { get; set; }
    }

    public class Servico
    {
        public decimal valor_servicos { get; set; }
        public int aliquota { get; set; }
        public string discriminacao { get; set; }
        public bool iss_retido { get; set; }
        public decimal valor_iss { get; set; }
        public decimal valor_iss_retido { get; set; }
        public string item_lista_servico { get; set; }
        public string codigo_tributario_municipio { get; set; }
        public string codigo_cnae { get; set; }
        public decimal valor_pis { get; set; }
        public decimal valor_cofins { get; set; }
    }

    public class Tomador
    {
        public string cpf { get; set; }
        public string cnpj { get; set; }
        public string razao_social { get; set; }
        public string email { get; set; }
        public string inscricao_municipal { get; set; }
        public Endereco endereco { get; set; }
    }

    public class Nacional
    {
        public string regime_tributario_simples_nacional { get; set; }
        public decimal percentual_total_tributos_simples_nacional { get; set; }
        public string codigo_municipio { get; set; }
        public Int32 tipo_retencao_iss { get; set; }
        public Int32 tipo_retencao_pis_confins { get; set; }
        public Int32 percentual_aliquota_relativa_municipio { get; set; }
    }

    public class NFSeModel
    {
        public DateTime data_emissao { get; set; }
        public string natureza_operacao { get; set; }
        public string regime_especial_tributacao { get; set; }
        public Boolean optante_simples_nacional { get; set; }
        public Boolean incentivador_cultural { get; set; }
        public Prestador prestador { get; set; }
        public Tomador tomador { get; set; }
        public Nacional nacional { get; set; }
        public Servico servico { get; set; }
        public List<Duplicatas> duplicatas { get; set; }
    }

    public class Duplicatas
    {
        public string numero { get; set; }
        public string data_vencimento { get; set; }
        public decimal valor { get; set; }
    }

    #endregion

    public class RetornoMensagem
    {
        public string status { get; set; }
        public string mensagem { get; set; }
        public string NumeroChaveNfe { get; set; }
        public string Numero { get; set; }
    }

    public class ConsultaModel
    {
        public string cnpj_prestador { get; set; }
        public string @ref { get; set; }
        public string numero_rps { get; set; }
        public string serie_rps { get; set; }
        public string status { get; set; }
        public string numero { get; set; }
        public string codigo_verificacao { get; set; }
        public DateTime data_emissao { get; set; }
        public string url { get; set; }
        public string url_danfse { get; set; }
        public string caminho_xml_nota_fiscal { get; set; }
        public string caminho_xml_carta_correcao { get; set; }
        public string caminho_xml_cancelamento { get; set; }
        public List<Erro> erros { get; set; }
    }

    #region Cancelamento
    public class Cancelamento
    {
        public string justificativa { get; set; }
    }

    public class Erro
    {
        public string codigo { get; set; }
        public string mensagem { get; set; }
        public object correcao { get; set; }
    }

    public class CancelaModel
    {
        public string status_sefaz { get; set; }
        public string mensagem_sefaz { get; set; }
        public string status { get; set; }
        public string caminho_xml { get; set; }
    }
    #endregion
    public class Emails
    {
        public List<string> emails { get; set; }
    }

    public class EnvioModel
    {
        public string cnpj_prestador { get; set; }
        public string @ref { get; set; }
        public string status { get; set; }
        public string codigo { get; set; }
        public string mensagem { get; set; }
        public string caminho_danfe { get; set; }
    }
    #endregion

    #region Interfaces
    public interface iNFSe
    {
        EnvioModel criarNFSe(NFSeModel nFSe, string numeroNFSe);

        ConsultaModel consultarNFSe(string numeroNFSe);

        ConsultaModel consultarNFSeDanfe(string numeroNFSe);

        CancelaModel CancelarNFS(string numeroNFe, string Justificativa);

        bool enviarEmailNFSe(Emails emails, string numeroNFSe);
    }
    #endregion

    public class NFSeFocus : iNFSe
    {
        private string url = string.Empty;
        private string token = string.Empty;

        public NFSeFocus(string url, String Token)
        {
            this.url = url;
            this.token = Token;
        }

        public EnvioModel criarNFSe(NFSeModel nFSe, string numeroNFSe)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);


                    var parameters = "/v2/nfse?ref=" + numeroNFSe + "&token=" + token;
                    var content = new StringContent(JsonConvert.SerializeObject(nFSe).ToString(), Encoding.UTF8, "application/json");
                    var result = client.PostAsync(parameters, content).Result;
                    var jasonxml = JsonConvert.SerializeObject(nFSe).ToString();
                    var resultContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    return JsonConvert.DeserializeObject<EnvioModel>(resultContent);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public RetornoMensagem EmitirNFS(DadosNFS dados)
        {
            RetornoMensagem retornoMensagem = new RetornoMensagem();
            var mensagemtry = "";
            try
            {
                var mensagemErro = string.Empty;
                var vencimentoparcela = string.Empty;

                #region Emitir NFeMontar

                //#if (DEBUG)
                //                    NFSeFocus NFSeFocus = new NFSeFocus(empresa.Url_homologacao, empresa.TokenHomologacao);
                //#else
                //                    NFSeFocus NFSeFocus = new NFSeFocus(empresa.Url_Producao, empresa.TokenProducao);
                //#endif

                NFSeFocus NFSeFocus = new NFSeFocus(null, null);
                if (dados.empresa.Ambiente == "H")
                {
                    NFSeFocus = new NFSeFocus(dados.empresa.Url_homologacao, dados.empresa.TokenHomologacao);
                }
                else
                {
                    NFSeFocus = new NFSeFocus(dados.empresa.Url_Producao, dados.empresa.TokenProducao);
                }

                //NFSeFocus NFSeFocus = new NFeNFSeFocusFocus();

                string NumeroNota = "";
                var NumeroReferencia = long.Parse(dados.notaservico.NumeroReferencia.ToString());
                //var NumeroNota = long.Parse(dados.notaservico.NumeroNota.ToString());
                NFSeModel nFSeModel = new NFSeModel();
                nFSeModel.data_emissao = DateTime.Now;

                //natureza_operacao
                //1: Tributação no município;
                //2: Tributação fora do município;
                //3: Isenção;
                //4: Imune;
                //5: Exigibilidade suspensa por decisão judicial;
                //6: Exigibilidade suspensa por procedimento administrativo.
                nFSeModel.natureza_operacao = "1";

                //regime_especial_tributacao
                //1: Microempresa municipal;
                //2: Estimativa;
                //3: Sociedade de profissionais;
                //4: Cooperativa;
                //5: MEI - Simples Nacional;
                //6: ME EPP- Simples Nacional.
                nFSeModel.regime_especial_tributacao = "6";

                //optante_simples_nacional(*): (Boolean) Informar true (verdadeiro) ou false (falso) se a empresa for optante pelo Simples Nacional.Campo ignorado por alguns municípios
                nFSeModel.optante_simples_nacional = true;

                nFSeModel.nacional = new Nacional();
                nFSeModel.nacional.regime_tributario_simples_nacional = dados.empresa.RegimeTributarioSN.ToString();
                nFSeModel.nacional.percentual_total_tributos_simples_nacional = dados.empresa.PercentualTotalTributosSN;

                nFSeModel.nacional.tipo_retencao_iss = 1;
                if (dados.notaservico.ReterIssqn == "S")
                {
                    nFSeModel.nacional.tipo_retencao_iss = 2;
                    nFSeModel.nacional.percentual_aliquota_relativa_municipio = dados.notaservico.PercentualIssqn.Value;
                }

                nFSeModel.nacional.tipo_retencao_pis_confins = 2;


                //incentivador_cultural: (Boolean) Informe true (verdadeiro) ou false (falso). Valor padrão: false. Campo ignorado em alguns municípios.
                nFSeModel.incentivador_cultural = false;

                nFSeModel.prestador = new Prestador();

                nFSeModel.prestador.cnpj = dados.empresa.Cnpj;
                nFSeModel.prestador.inscricao_municipal = dados.empresa.InscricaoMunicipal;
                nFSeModel.prestador.codigo_municipio = dados.empresa.CodigoMunicipio;

                nFSeModel.tomador = new Tomador();
                nFSeModel.tomador.razao_social = dados.cliente.Nome;

                var cpfcnpj = Util.OnlyNumbers(dados.cliente.CpfCnpj);

                if (cpfcnpj.Length == 11)
                {
                    nFSeModel.tomador.cpf = Util.FormatCPF(cpfcnpj);
                }
                else if (cpfcnpj.Length == 14)
                {
                    nFSeModel.tomador.cnpj = Util.FormatCNPJ(cpfcnpj);
                }
                nFSeModel.tomador.email = dados.clientesemails.Trim();
                if (dados.cliente.InscricaoMunicipal != null && dados.cliente.InscricaoMunicipal.Trim().Length > 0)
                {
                    nFSeModel.tomador.inscricao_municipal = dados.cliente.InscricaoMunicipal;
                }
                //nFSeModel.tomador.inscricao_municipal = "00405922";

                nFSeModel.tomador.endereco = new Endereco();
                nFSeModel.tomador.endereco.logradouro = dados.clienteEndereco.Endereco;
                nFSeModel.tomador.endereco.numero = dados.clienteEndereco.Numero;
                if (dados.clienteEndereco.Complemento != null && dados.clienteEndereco.Complemento.Trim().Length > 0)
                {
                    nFSeModel.tomador.endereco.complemento = dados.clienteEndereco.Complemento;
                }
                nFSeModel.tomador.endereco.bairro = dados.clienteEndereco.Bairro;
                nFSeModel.tomador.endereco.codigo_municipio = dados.clienteEndereco.CodigoMunicipio;
                nFSeModel.tomador.endereco.cidade = dados.clienteEndereco.Municipio;
                nFSeModel.tomador.endereco.uf = dados.clienteEndereco.Estado;
                nFSeModel.tomador.endereco.cep = dados.clienteEndereco.CEP.Trim();

                nFSeModel.servico = new Servico();
                nFSeModel.servico.valor_servicos = dados.notaservico.ValorServico.Value;
                nFSeModel.servico.aliquota = 0;
                nFSeModel.servico.discriminacao = dados.notaservico.DescricaoServico;
                if (dados.notaservico.ReterIssqn == "N")
                {
                    nFSeModel.servico.iss_retido = false;
                    nFSeModel.servico.valor_iss = dados.notaservico.ValorIssqn.Value;
                    nFSeModel.servico.valor_iss_retido = 0;
                }
                else
                {
                    nFSeModel.servico.iss_retido = true;
                    nFSeModel.servico.valor_iss = 0;
                    nFSeModel.servico.valor_iss_retido = dados.notaservico.ValorIssqn.Value;
                }
                
                if (dados.empresa.NomeFantasia == "Sigman")
                {
                    nFSeModel.servico.item_lista_servico = "010701";
                }
                else
                {
                    nFSeModel.servico.item_lista_servico = "140101";
                }
                //nFSeModel.servico.codigo_tributario_municipio = "140100100";  10700100
                nFSeModel.servico.codigo_cnae = dados.empresa.Cnae;

                if (dados.duplicatasnfs != null)
                {
                    nFSeModel.duplicatas = new List<Duplicatas>();
                    foreach (var item in dados.duplicatasnfs)
                    {
                        Duplicatas duplicata = new Duplicatas();
                        if (item.Vencimento.Value >= DateTime.Now)
                        {
                            vencimentoparcela = item.Vencimento.Value.ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            vencimentoparcela = DateTime.Now.ToString("yyyy-MM-dd");

                        }
                        duplicata.numero = item.Parcela.ToString().PadLeft(3, '0');
                        duplicata.data_vencimento = vencimentoparcela;
                        duplicata.valor = item.ValorParcela.Value;

                        nFSeModel.duplicatas.Add(duplicata);
                    }
                }

                #endregion

                #region nFeAprovar

                var resultadoEnvioNota = Task.Run(() => NFSeFocus.criarNFSe(nFSeModel, Util.AddLeadingZeros(NumeroReferencia, 10)));
                resultadoEnvioNota.Wait(5000);

                //Thread.Sleep(10000);

                //var NFeSaida = NFSeFocus.consultarNFe(Util.AddLeadingZeros(NumeroNota, 10));

                bool retornoconsulta = false;
                var NFS = Task.Run(() => NFSeFocus.consultarNFSe(Util.AddLeadingZeros(NumeroReferencia, 10)));
                NFS.Wait(5000);

                if (NFS.Result.status != "processando_autorizacao")
                {
                    retornoconsulta = true;
                }
                else
                {
                    while (!retornoconsulta)
                    {
                        NFS = Task.Run(() => NFSeFocus.consultarNFSe(Util.AddLeadingZeros(NumeroReferencia, 10)));
                        NFS.Wait(5000);
                        if (NFS.Result.status != "processando_autorizacao")
                        {
                            retornoconsulta = true;
                        }
                    }
                }

                if (NFS != null && NFS.Result.status == "autorizado")
                {
                    var url = string.Empty;
                    NumeroNota = NFS.Result.numero.PadLeft(6, '0');
          //          NumeroNota = NumeroNota.Replace("00000", "");
                    retornoMensagem.Numero = NumeroNota;
                    //var chavenfe = NFeSaida.Result.chave_nfe.Replace("NFe", "");

                    url = dados.URL + NFS.Result.caminho_xml_nota_fiscal;

                    mensagemtry = $"url: ({url})";

                    using (WebClient client = new WebClient())
                    {
                        var xmlData = client.DownloadData(url);
                        var xmlCode = Encoding.UTF8.GetString(xmlData);

                        mensagemtry = $"url: ({string.Format(@"{0}\{1}", string.Concat(dados.CaminhoXml), string.Concat("NFSn-", NumeroNota, ".xml"))})";

                        System.IO.File.WriteAllText(string.Format(@"{0}\{1}", string.Concat(dados.CaminhoXml), string.Concat("NFSn-", NumeroNota, ".xml")), xmlCode.ToString());

                        string htmlContent = "";
                        if (NFS.Result.url_danfse != null)
                        {
                            mensagemtry = $"url: ({NFS.Result.url_danfse})";
                            client.DownloadFile(NFS.Result.url_danfse, string.Format(@"{0}\{1}", string.Concat(dados.CaminhoDanfe), string.Concat("NFSn-", NumeroNota, ".pdf")));

                            htmlContent = client.DownloadString(NFS.Result.url_danfse);
                        }
                        else
                        {
                            mensagemtry = $"url: ({resultadoEnvioNota.Result.caminho_danfe})";
                            htmlContent = client.DownloadString(resultadoEnvioNota.Result.caminho_danfe);
                            htmlContent = client.DownloadString(string.Format(@"{0}\{1}", string.Concat(dados.CaminhoXml), resultadoEnvioNota.Result.caminho_danfe));
                        }

                        mensagemtry = $"html: ({htmlContent})";

                        //var htmlNS = Sigman.Common.Util.URLEncoding(htmlContent);

                        //mensagemtry = $"url: ({htmlNS})";


                        retornoMensagem.mensagem = mensagemtry;
                        retornoMensagem.status = NFS.Result.status;
                        return retornoMensagem;

                        //var danfe = Sigman.Common.Util.GerarPDF(htmlNS);

                        ////mensagemtry = $"danfe: ({danfe})";
                        ////                        mensagemtry = $"url: ({string.Format(@"{0}\{1}", string.Concat(dados.CaminhoDanfe), string.Concat("NFS-", Util.AddLeadingZeros(NumeroNota, 6), ".pdf"))})";

                        ////mensagemtry = $"url: ({Convert.FromBase64String(danfe)})";

                        //System.IO.File.WriteAllBytes(string.Format(@"{0}\{1}", string.Concat(dados.CaminhoDanfe), string.Concat("NFS-", NumeroNota, ".pdf")), Convert.FromBase64String(danfe));

                        // mensagemtry = "Linha 06";

                    }

                    #region Enviar E-mail 
                    List<Attachment> attach = new List<Attachment>();

                    var nomeArquivoPdf = string.Format(@"{0}\{1}", string.Concat(dados.CaminhoDanfe), string.Concat("NFSn-", NumeroNota, ".pdf"));
                    if (System.IO.File.Exists(nomeArquivoPdf))
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(nomeArquivoPdf);
                        MemoryStream ms = new MemoryStream(bytes);
                        Attachment attachment = new Attachment(ms, string.Concat("NFS-", NumeroNota, ".pdf"));
                        attach.Add(attachment);
                    }

                    var nomeArquivoXml = string.Format(@"{0}\{1}", string.Concat(dados.CaminhoXml), string.Concat("NFSn-", NumeroNota, ".xml"));
                    if (System.IO.File.Exists(nomeArquivoXml))
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(nomeArquivoXml);
                        MemoryStream ms = new MemoryStream(bytes);
                        Attachment attachment = new Attachment(ms, string.Concat("NFS-", NumeroNota, ".xml"));
                        attach.Add(attachment);
                    }

                    //#region Boleto
                    //foreach (var parcela in ClientesNotasParcelasBO.Listar(notafiscal.ID))
                    //{
                    //    if (parcela.Status == "N" && parcela.NossoNumero == "BOLETO")
                    //    {
                    //        var boleto = GerarBoleto(empresa, cliente, clienteEndereco, parcela, string.Format("{0}-{1}", Util.AddLeadingZeros(NumeroNota, 6), Util.AddLeadingZeros(parcela.Parcela.Value, 2)));
                    //        if (boleto != null)
                    //        {
                    //            //byte[] bytes = System.IO.File.ReadAllBytes(boleto);
                    //            MemoryStream ms = new MemoryStream(boleto);
                    //            Attachment attachment = new Attachment(ms, string.Format("Boleto-{0}-{1}.pdf", Util.AddLeadingZeros(NumeroNota, 6), Util.AddLeadingZeros(parcela.Parcela.Value, 2)));
                    //            attach.Add(attachment);
                    //        }
                    //    }
                    //}
                    //#endregion

                    string emailNota = null;
                    emailNota = dados.clientesemails;


                    if (emailNota != null)
                    {
                        var assunto = "Nota Fiscal " + dados.empresa.NomeFantasia + string.Concat("NFS-", NumeroNota);
                        var corpoEmail = "<h3>Segue em anexo Nota fiscal número " + string.Concat("NFS - ", NumeroNota) + "</h3>";
                        Mail.Send(dados.empresa.SMTPEndereco, emailNota, assunto, corpoEmail, dados.empresa.SMTPRemetente, dados.empresa.SMTPUsuario, dados.empresa.SMTPSenha, dados.empresa.SMTPPorta, attach);
                    }

                    retornoMensagem.status = NFS.Result.status;
                    //retornoMensagem.mensagem = NFS.Result.mensagem_sefaz;
                    //retornoMensagem.NumeroChaveNfe = NFS.Result. chave_nfs.Replace("NFe", "");
                    #endregion

                }
                else
                {
                    retornoMensagem.status = NFS.Result.status;

                    var mensagens = string.Empty;

                    foreach (var erro in NFS.Result.erros)
                    {
                        mensagens += erro.mensagem + " - " + erro.correcao;
                    }

                    retornoMensagem.mensagem = mensagens;
                    retornoMensagem.NumeroChaveNfe = "";
                }

                #endregion

                return retornoMensagem;
            }
            catch (Exception ex)
            {
                retornoMensagem.status = "Erro_Estrutura";
                retornoMensagem.mensagem = $"Problema nos campos da Nota de Serviço: {mensagemtry} / {ex.Message}";
                retornoMensagem.NumeroChaveNfe = "";
                return retornoMensagem;
            }
        }

        public ConsultaModel consultarNFSe(string numeroNFSe)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);

                    var result = client.GetAsync("/v2/nfse/" + numeroNFSe + "?token=" + token).GetAwaiter().GetResult();
                    var resultContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    //if (result.StatusCode == HttpStatusCode.OK)
                    //{
                    //    return JsonConvert.DeserializeObject<ConsultaModel>(resultContent);
                    //}
                    //else
                    //{
                    //    return null;
                    //}
                    return JsonConvert.DeserializeObject<ConsultaModel>(resultContent);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public ConsultaModel consultarNFSeDanfe(string numeroNFSe)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);

                    var result = client.GetAsync("/v2/nfse/" + numeroNFSe + "?token=" + token + "&url_danfse").GetAwaiter().GetResult();
                    var resultContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    return JsonConvert.DeserializeObject<ConsultaModel>(resultContent);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public CancelaModel CancelarNFS(string numeroNFSe, string Justificativa)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);

                    //var result = client.DeleteAsync("/v2/nfse/" + numeroNFSe).GetAwaiter().GetResult();
                    //var resultContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    //return JsonConvert.DeserializeObject<CancelamentoModel>(resultContent);

                    Cancelamento cancelamento = new Cancelamento();
                    cancelamento.justificativa = Justificativa;

                    var parameters = "/v2/nfse/" + numeroNFSe + "?token=" + token;

                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Delete,
                        RequestUri = new Uri(url + parameters),
                        Content = new StringContent(JsonConvert.SerializeObject(cancelamento), Encoding.UTF8, "application/json")
                    };
                    var response = client.SendAsync(request);

                    var result = response.Result;

                    var resultContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    return JsonConvert.DeserializeObject<CancelaModel>(resultContent);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool enviarEmailNFSe(Emails emails, string numeroNFSe)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);

                    var parameters = "/v2/nfse/" + numeroNFSe + "/email&token=" + token;
                    var content = new StringContent(JsonConvert.SerializeObject(emails).ToString(), Encoding.UTF8, "application/json");
                    var result = client.PostAsync(parameters, content).Result;

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}