using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Printing;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Sigman.NFCe
{
    #region Entities
    #region NFCe
    public class FormasPagamento
    {
        public string forma_pagamento { get; set; }
        public string valor_pagamento { get; set; }
        //public string nome_credenciadora { get; set; }
        //public string bandeira_operadora { get; set; }
        //public string numero_autorizacao { get; set; }

    }

    public class ItemCupom
    {
        public string numero_item { get; set; }
        public string codigo_ncm { get; set; }
        public string quantidade_comercial { get; set; }
        public string quantidade_tributavel { get; set; }
        public string cfop { get; set; }
        public string valor_unitario_tributavel { get; set; }
        public string valor_unitario_comercial { get; set; }
        public string valor_bruto { get; set; }
        public string valor_desconto { get; set; }
        public string descricao { get; set; }
        public string codigo_produto { get; set; }
        public string icms_origem { get; set; }
        public string icms_situacao_tributaria { get; set; }
        public string unidade_comercial { get; set; }
        public string unidade_tributavel { get; set; }
        public string icms_aliquota { get; set; }
        public string icms_base_calculo { get; set; }
        public string icms_valor { get; set; }
        public string icms_modalidade_base_calculo { get; set; }
        public string valor_total_tributos { get; set; }
    }

    public class NFCeModel
    {
        public string cnpj_emitente { get; set; }
        public string modalidade_frete { get; set; }
        public DateTime data_emissao { get; set; }
        public string local_destino { get; set; }
        public string presenca_comprador { get; set; }
        public string natureza_operacao { get; set; }
        public string nome_destinatario { get; set; }
        public string cnpj_destinatario { get; set; }
        public string cpf_destinatario { get; set; }
        public string indicador_inscricao_estadual_destinatario { get; set; }
        public List<ItemCupom> itens { get; set; }
        public List<FormasPagamento> formas_pagamento { get; set; }
    }
    #endregion

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
        public string qrcode_url { get; set; }
        public string url_consulta_nf { get; set; }
    }

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

    public class Cancelamento
    {
        public string justificativa { get; set; }
    }

    public class CancelamentoModel
    {
        public string status_sefaz { get; set; }
        public string mensagem_sefaz { get; set; }
        public string status { get; set; }
        public string caminho_xml_cancelamento { get; set; }
        public string codigo { get; set; }
        public string mensagem { get; set; }
    }

    public class EnvioModel
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
        public string qrcode_url { get; set; }
        public string url_consulta_nf { get; set; }
        public bool contingencia_offline { get; set; }
        public bool contingencia_offline_efetivada { get; set; }
    }
    #endregion

    #region Interfaces
    public interface iNFCe
    {
        EnvioModel criarNFCe(NFCeModel nFCe, string numeroNFCe);

        ConsultaModel consultarNFCe(string numeroNFCe);

        CancelamentoModel cancelarNFCe(string numeroNFCe);

        bool enviarEmailNFCe(Emails emails, string numeroNFCe);

        bool inutilizacaoNFCe(Inutilizacao inutilizacao);
    }
    #endregion

    #region Geracao de Cupom
    public class NFCeFocus : iNFCe
    {
        private string url = string.Empty;
        private string token = string.Empty;

        public NFCeFocus(string url, String Token)
        {
            this.url = url;
            this.token = Token;
        }

        public EnvioModel criarNFCe(NFCeModel nFCe, string numeroNFCe)
        {
            //try
            //{
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);

                    var jasonxml = JsonConvert.SerializeObject(nFCe).ToString();
                    var parameters = "/v2/nfce?ref=" + numeroNFCe + "&token=" + token;
                    var content = new StringContent(JsonConvert.SerializeObject(nFCe).ToString(), Encoding.UTF8, "application/json");
                    var result = client.PostAsync(parameters, content).Result;

                    var resultContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    EnvioModel model = new EnvioModel();
                    model.mensagem_sefaz = url + "/v2/nfce?ref=" + numeroNFCe + "&token=" + token + " - " + ((int)result.StatusCode).ToString();
                    return model;

                //return JsonConvert.DeserializeObject<EnvioModel>(resultContent);
            }
            //}
            //catch (Exception ex)
            //{
            //    EnvioModel model = new EnvioModel();
            //    model.mensagem_sefaz = url + "/v2/nfce?ref=" + numeroNFCe + "&token=" + token + " - " + ex.Message;
            //    return model;
            //}
        }

        public ConsultaModel consultarNFCe(string numeroNFCe)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);

                    var result = client.GetAsync("/v2/nfce/" + numeroNFCe + "?completa=0&token=" + token).GetAwaiter().GetResult();
                    var resultContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        return JsonConvert.DeserializeObject<ConsultaModel>(resultContent);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public CancelamentoModel cancelarNFCe(string numeroNFCe)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);

                    var parameters = "/v2/nfce/" + numeroNFCe + "?token=" + token;

                    Cancelamento cancelamento = new Cancelamento();
                    cancelamento.justificativa = "Teste de cancelamento de nota";

                    //var content = new StringContent(JsonConvert.SerializeObject(cancelamento).ToString(), Encoding.UTF8, "application/json");
                    //var result = client.DeleteAsync(parameters, content).Result;


                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Delete,
                        RequestUri = new Uri(url + parameters),
                        Content = new StringContent(JsonConvert.SerializeObject(cancelamento), Encoding.UTF8, "application/json")
                    };
                    var response = client.SendAsync(request);

                    var result = response.Result;

                    var resultContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (result.StatusCode == HttpStatusCode.OK)
                    {
                        return JsonConvert.DeserializeObject<CancelamentoModel>(resultContent);
                    }
                    else
                    {
                        return JsonConvert.DeserializeObject<CancelamentoModel>(resultContent);
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool enviarEmailNFCe(Emails emails, string numeroNFCe)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);

                    var parameters = "/v2/nfce/" + numeroNFCe + "/email&token=" + token;
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

        public bool inutilizacaoNFCe(Inutilizacao inutilizacao)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);

                    var parameters = "/v2/nfce/inutilizacao?token=" + token;
                    var jsonInutilizacao = JsonConvert.SerializeObject(inutilizacao).ToString();
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


    }
    #endregion

    #region Impressora de Cupom
    public class CupomImpresso
    {
        public List<LinhaCupom> Linhas = new List<LinhaCupom>();

        public int LarguraCupom { get; }
        public int AlturaLinha { get; }

        public CupomImpresso(int larguraCupom, int alturaLinha)
        {
            LarguraCupom = larguraCupom;
            AlturaLinha = alturaLinha;
        }

        public void AdicionarLinha(string texto, EstiloLinhaCupom estilo)
        {
            Linhas.Add(new LinhaCupom
            {
                Texto = texto,
                Estilo = estilo
            });
        }

        public void AdicionarLinhaEmBranco()
        {
            Linhas.Add(new LinhaCupom());
        }

        public void AdicionarTracejado()
        {
            Linhas.Add(new LinhaCupom { Tracejado = true });
        }

        public void AdicionarImagem(Bitmap imagem, Rectangle posiçãoImagem)
        {
            Linhas.Add(new LinhaCupom
            {
                Imagem = imagem,
                Posição = posiçãoImagem,
            });
        }
    }

    public class LinhaCupom
    {
        public string Texto = string.Empty;
        public EstiloLinhaCupom Estilo;
        public bool Tracejado;

        public Bitmap Imagem;
        public Rectangle Posição;

    }

    public enum EstiloLinhaCupom
    {
        Regular,
        Negrito,
        Pequeno
    }


    public class ImpressoraDeCupom : PrintDocument
    {
        ///////Configurações de Fonte///////
        private Font fonteNegrita = new Font("Courier New", 7, FontStyle.Bold);
        private Font fonteRegular = new Font("Courier New", 7, FontStyle.Regular);
        private Font fontePequena = new Font("Courier New", 6, FontStyle.Bold);
        ////////////////////////////////////

        public CupomImpresso Cupom { get; }

        public ImpressoraDeCupom(CupomImpresso cupom, string nomeImpressora)
        {
            this.Cupom = cupom;

            PrinterSettings settings = new PrinterSettings();

            this.PrinterSettings.PrinterName = nomeImpressora; //settings.PrinterName;
            this.OriginAtMargins = false;
            this.PrintPage += new PrintPageEventHandler(printPage);
        }

        private void printPage(object send, PrintPageEventArgs e)
        {
            Graphics graphics = e.Graphics;
            int offset = 0;

            foreach (var linha in Cupom.Linhas)
            {
                if (linha.Tracejado)
                {
                    ///Imprime um tracejado

                    var grossuraDoTraço = offset; //Mudar isso se quiser trocar a grossura dos traços

                    graphics.DrawLine(Pens.Black, 0, offset, Cupom.LarguraCupom, grossuraDoTraço);
                    offset += Cupom.AlturaLinha;
                }
                else
                {
                    if (linha.Imagem != null)
                    {
                        offset += Cupom.AlturaLinha;

                        var novaPosição = new Rectangle(linha.Posição.X, offset, linha.Posição.Width, linha.Posição.Height);

                        graphics.DrawImage(linha.Imagem, novaPosição);

                        offset += linha.Posição.Height;

                    }
                    else if (string.IsNullOrEmpty(linha.Texto))
                    {
                        ///Linha em branco
                        ///Nada a fazer
                        offset += Cupom.AlturaLinha;
                    }
                    else
                    {
                        var estilo = fonteRegular;

                        switch (linha.Estilo)
                        {
                            case EstiloLinhaCupom.Regular:
                                estilo = fonteRegular;
                                offset += Cupom.AlturaLinha;
                                break;
                            case EstiloLinhaCupom.Negrito:
                                estilo = fonteNegrita;
                                offset += Cupom.AlturaLinha;
                                break;
                            case EstiloLinhaCupom.Pequeno:
                                estilo = fontePequena;
                                offset += 10;
                                break;
                        }

                        graphics.DrawString(linha.Texto, estilo, Brushes.Black, 0, offset);

                        if (linha.Estilo == EstiloLinhaCupom.Negrito)
                        {
                            offset += 5;
                        }

                    }
                }
            }
        }
    }
    #endregion
}