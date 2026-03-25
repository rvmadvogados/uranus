using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Sigman.NFe
{
    #region Entities
    #region NFe
    public class Item
    {
        public int numero_item { get; set; }
        public string codigo_produto { get; set; }
        public string descricao { get; set; }
        public int cfop { get; set; }
        public string unidade_comercial { get; set; }
        public string quantidade_comercial { get; set; }
        public decimal valor_unitario_comercial { get; set; }
        public decimal valor_unitario_tributavel { get; set; }
        public string unidade_tributavel { get; set; }
        public int codigo_ncm { get; set; }
        public string quantidade_tributavel { get; set; }
        public decimal valor_bruto { get; set; }
        public string icms_situacao_tributaria { get; set; }
        public int icms_origem { get; set; }
        public string pis_situacao_tributaria { get; set; }
        public string cofins_situacao_tributaria { get; set; }
        public int inclui_no_total { get; set; }
        public string icms_aliquota { get; set; }
        public decimal icms_base_calculo { get; set; }
        public decimal icms_valor_total { get; set; }
        public decimal icms_base_calculo_st { get; set; }
        public decimal icms_valor_total_st { get; set; }
        public decimal valor_produtos { get; set; }
        public decimal valor_frete { get; set; }
        public decimal valor_seguro { get; set; }
        public decimal valor_desconto { get; set; }
        public decimal valor_ipi { get; set; }
        public decimal valor_pis { get; set; }
        public decimal valor_cofins { get; set; }
        public decimal valor_outras_despesas { get; set; }
        public decimal valor_total { get; set; }
        public decimal icms_modalidade_base_calculo { get; set; }
        
    }

    public class NFeModel
    {
        public string natureza_operacao { get; set; }
        public string data_emissao { get; set; }
        public int tipo_documento { get; set; }
        public int local_destino { get; set; }
        public int finalidade_emissao { get; set; }
        public int consumidor_final { get; set; }
        public int presenca_comprador { get; set; }
        public string data_entrada_saida { get; set; }
        public string cnpj_emitente { get; set; }
        public string cpf_emitente { get; set; }
        public string nome_emitente { get; set; }
        public string nome_fantasia_emitente { get; set; }
        public string logradouro_emitente { get; set; }
        public string numero_emitente { get; set; }
        public string complemento_emitente { get; set; }
        public string bairro_emitente { get; set; }
        public string municipio_emitente { get; set; }
        public string uf_emitente { get; set; }
        public string cep_emitente { get; set; }
        public int regime_tributario_emitente { get; set; }
        public string inscricao_estadual_emitente { get; set; }
        public string nome_destinatario { get; set; }
        public string cnpj_destinatario { get; set; }
        public string cpf_destinatario { get; set; }
        public string inscricao_estadual_destinatario { get; set; }
        public string telefone_destinatario { get; set; }
        public string logradouro_destinatario { get; set; }
        public string numero_destinatario { get; set; }
        public string complemento_destinatario { get; set; }
        public string bairro_destinatario { get; set; }
        public string municipio_destinatario { get; set; }
        public string uf_destinatario { get; set; }
        public string pais_destinatario { get; set; }
        public int cep_destinatario { get; set; }
        public int indicador_inscricao_estadual_destinatario { get; set; }
        public decimal valor_frete { get; set; }
        public decimal valor_seguro { get; set; }
        public decimal valor_total { get; set; }
        public decimal valor_produtos { get; set; }
        public int modalidade_frete { get; set; }
        public List<Item> items { get; set; }
    }
    #endregion

    #region Carta de Correção
    public class CartaCorrecaoModel
    {
        public string status_sefaz { get; set; }
        public string mensagem_sefaz { get; set; }
        public string status { get; set; }
        public string caminho_xml_carta_correcao { get; set; }
        public string caminho_pdf_carta_correcao { get; set; }
        public int numero_carta_correcao { get; set; }
    }
    #endregion

    #region Consulta NFe
    public class ProtocoloCancelamento
    {
        public string versao { get; set; }
        public string ambiente { get; set; }
        public string versao_aplicativo { get; set; }
        public string codigo_orgao { get; set; }
        public string status { get; set; }
        public string motivo { get; set; }
        public string chave_nfe { get; set; }
        public string tipo_evento { get; set; }
        public string descricao_evento { get; set; }
        public DateTime data_evento { get; set; }
        public string numero_protocolo { get; set; }
    }

    public class ProtocoloCartaCorrecao
    {
        public string versao { get; set; }
        public string ambiente { get; set; }
        public string versao_aplicativo { get; set; }
        public string codigo_orgao { get; set; }
        public string status { get; set; }
        public string motivo { get; set; }
        public string chave_nfe { get; set; }
        public string tipo_evento { get; set; }
        public string descricao_evento { get; set; }
        public DateTime data_evento { get; set; }
        public string numero_protocolo { get; set; }
    }

    public class RequisicaoCancelamento
    {
        public string versao { get; set; }
        public string id_tag { get; set; }
        public string codigo_orgao { get; set; }
        public string ambiente { get; set; }
        public string cnpj { get; set; }
        public string chave_nfe { get; set; }
        public DateTime data_evento { get; set; }
        public string tipo_evento { get; set; }
        public string numero_sequencial_evento { get; set; }
        public string versao_evento { get; set; }
        public string descricao_evento { get; set; }
        public string protocolo { get; set; }
        public string justificativa { get; set; }
    }

    public class RequisicaoCartaCorrecao
    {
        public string versao { get; set; }
        public string id_tag { get; set; }
        public string codigo_orgao { get; set; }
        public string ambiente { get; set; }
        public string cnpj { get; set; }
        public string chave_nfe { get; set; }
        public DateTime data_evento { get; set; }
        public string tipo_evento { get; set; }
        public string numero_sequencial_evento { get; set; }
        public string versao_evento { get; set; }
        public string descricao_evento { get; set; }
        public string correcao { get; set; }
        public string condicoes_uso { get; set; }
    }

    public class ConsultaModel
    {
        public string cnpj_emitente { get; set; }
        public string @ref { get; set; }
        public string status { get; set; }
        public string status_sefaz { get; set; }
        public string mensagem_sefaz { get; set; }
        public string chave_nfe { get; set; }
        public string numero { get; set; }
        public string serie { get; set; }
        public string caminho_xml_nota_fiscal { get; set; }
        public string caminho_danfe { get; set; }
        public string caminho_xml_carta_correcao { get; set; }
        public string caminho_pdf_carta_correcao { get; set; }
        public int numero_carta_correcao { get; set; }

        public RequisicaoCancelamento requisicao_cancelamento { get; set; }
        public ProtocoloCancelamento protocolo_cancelamento { get; set; }
        public RequisicaoCartaCorrecao requisicao_carta_correcao { get; set; }
        public ProtocoloCartaCorrecao protocolo_carta_correcao { get; set; }
    }
    #endregion

    public class Emails
    {
        public List<string> emails { get; set; }
    }

    public class Inutilizacao
    {
        public string cnpj { get; set; }
        public string serie { get; set; }
        public string numero_inicial { get; set; }
        public string numero_final { get; set; }
        public string justificativa { get; set; }
    }

    public class Importacao
    {
        public string cnpj_emitente { get; set; }
        public string @ref { get; set; }
        public string status { get; set; }
        public string status_sefaz { get; set; }
        public string mensagem_sefaz { get; set; }
        public string chave_nfe { get; set; }
        public string numero { get; set; }
        public string serie { get; set; }
        public string caminho_xml_nota_fiscal { get; set; }
        public string caminho_danfe { get; set; }
        public string codigo { get; set; }
        public string mensagem { get; set; }
    }
    #endregion

    #region Interfaces
    public interface iNFe
    {
        bool criarNFe(NFeModel nFe, string numeroNFe);

        ConsultaModel consultarNFe(string numeroNFe);

        bool cancelarNFe(string numeroNFe);

        bool criarCartaCorrecaoNFe(CartaCorrecaoModel cartaCorrecao, string numeroNFe);

        bool enviarEmailNFe(Emails emails, string numeroNFe);

        bool inutilizacaoNFe(Inutilizacao inutilizacao);

        Importacao importacaoNFe(string xmlData, string numeroNFe);
    }
    #endregion

    public class NFeFocus : iNFe
    {
        private string url = string.Empty;
        private string token = string.Empty;

        public NFeFocus(string url, String Token)
        {
            this.url = url;
            this.token = Token;
        }

        public bool criarNFe(NFeModel nFe, string numeroNFe)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    // hMHl8iDfPINfxSx4xeDGkt6VWNNJqtEU
                    var parameters = "/v2/nfe?ref=" + numeroNFe + "&token=" + token;
                    var content = new StringContent(JsonConvert.SerializeObject(nFe).ToString(), Encoding.UTF8, "application/json");
                    var jasonxml = JsonConvert.SerializeObject(nFe).ToString();
                    var result = client.PostAsync(parameters, content).Result;

                    if (result.StatusCode == HttpStatusCode.OK || result.StatusCode == HttpStatusCode.Accepted)
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

        public ConsultaModel consultarNFe(string numeroNFe)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);

                    var result = client.GetAsync("/v2/nfe/" + numeroNFe + "?token=" + token).GetAwaiter().GetResult();
                    var resultContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    //if (result.StatusCode == HttpStatusCode.OK)
                    //{
                    //    return JsonConvert.DeserializeObject<ConsultaModel>(resultContent);
                    //}
                    //else
                    //{
                        return JsonConvert.DeserializeObject<ConsultaModel>(resultContent);
                    //}
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool cancelarNFe(string numeroNFe)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);

                    var result = client.DeleteAsync("/v2/nfe/" + numeroNFe).GetAwaiter().GetResult();

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

        public bool criarCartaCorrecaoNFe(CartaCorrecaoModel cartaCorrecao, string numeroNFe)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);

                    var parameters = "/v2/nfe/" + numeroNFe + "/carta_correcao&token=" + token;
                    var content = new StringContent(JsonConvert.SerializeObject(cartaCorrecao).ToString(), Encoding.UTF8, "application/json");
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

        public bool enviarEmailNFe(Emails emails, string numeroNFe)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);

                    var parameters = "/v2/nfe/" + numeroNFe + "/email&token=" + token;
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

        public bool inutilizacaoNFe(Inutilizacao inutilizacao)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);

                    var parameters = "/v2/nfe/inutilizacao&token=" + token;
                    var content = new StringContent(JsonConvert.SerializeObject(inutilizacao).ToString(), Encoding.UTF8, "application/json");
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

        public Importacao importacaoNFe(string xmlData, string numeroNFe)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);

                    var result = client.GetAsync("/v2/nfe/importacao?ref=" + numeroNFe + "&token=" + token).GetAwaiter().GetResult();
                    var resultContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    return JsonConvert.DeserializeObject<Importacao>(resultContent);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}