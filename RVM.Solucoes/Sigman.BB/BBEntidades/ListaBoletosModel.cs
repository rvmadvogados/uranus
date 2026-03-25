using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sigman.BB.BBEntidades
{
    #region Request
    public class ListaBoletosRequest
    {
        // Situação do boleto.Campo obrigatoriamente MAIÚSCULO.
        // Domínios: A - Em ser B - Baixados/Protestados/Liquidados
        public string indicadorSituacao { get; set; }

        // Número da conta caução. 
        // Domínio:
        // 1 - Compõe Garantia;
        // 2 - Não Compõe Garantia;
        // 4 - Não Compõe Garantia(vencimento superior a 180 dias);
        // 5 - Não Compõe Garantia(Vedado);
        // 6 - Em análise;
        // 7 - Em análise;
        // 8 - Não Compõe Garantia.
        public int? contaCaucao { get; set; }

        // Número da agência do beneficiário, sem o dígito verificador.
        // Ex: 452. CAMPO OBRIGATÓRIO.
        public int agenciaBeneficiario { get; set; }

        // Número da conta do beneficiário, sem o dígito verificador.
        // Ex: 123873. CAMPO OBRIGATÓRIO.
        public long contaBeneficiario { get; set; }

        // Número da carteira do convênio de cobrança.
        // Ex: 17
        public int? carteiraConvenio { get; set; }

        // Número da variação da carteira do convênio de cobrança.
        // Ex: 35
        public int? variacaoCarteiraConvenio { get; set; }

        // Código para identificar a característica dos boletos dentro das modalidades de cobrança existentes no BB.
        // Domínio:
        // 1 - SIMPLES COM REGISTRO
        // 2 - SIMPLES SEM REGISTRO
        // 4 - VINCULADA
        // 6 - DESCONTADA
        // 8 - FINANCIADA VENDOR
        public int? modalidadeCobranca { get; set; }

        // CNPJ do pagador
        // Ex: 123456789012
        public long? cnpjPagador { get; set; }

        // Dígito verificador do CNPJ do pagador
        // Ex: 12
        public int? digitoCNPJPagador { get; set; }

        // CPF do pagador sem o dígito.
        // Ex: 711285901
        public long? cpfPagador { get; set; }

        // Dígito verificador do CPF do pagador.
        // Ex: 82
        public int? digitoCPFPagador { get; set; }

        // Data inicial de vencimento do boleto que delimita o período da consulta.
        // Campo não obrigatório.
        // Se informado Data Início, deixando em branco a Data Fim, o sistema deve assumir a data atual como Data Fim.
        // Ex: 22.04.2020
        public string dataInicioVencimento { get; set; }

        // Data final de vencimento do boleto que delimita o período da consulta - deverá ser maior que a data de início.
        // Campo não obrigatório.
        // Se informado, deverá ser preenchido dataInicioVencimento.
        // Ex: 28.04.2020
        public string dataFimVencimento { get; set; }

        // Data inicial do registro do boleto que delimita o período da consulta.
        // Ex: 22.04.2020
        public string dataInicioRegistro { get; set; }

        // Data final do registro do boleto que delimita o período da consulta - deverá ser maior que a data de início.
        // Campo não obrigatório.
        // Se informado, deverá ser preenchido dataInicioRegistro.
        // Ex: 28.04.2020
        public string dataFimRegistro { get; set; }

        // Delimita o período da consulta de boletos liquidados, baixados ou protestados, caso seja informado,
        // no campo codigoEstadoTituloCobranca os códigos 05, 06, 07 ou 09.
        // Ex: 22.04.2020
        public string dataInicioMovimento { get; set; }

        // Data final do movimento que delimita o período da consulta de boletos liquidados, baixados ou protestados, caso seja informado,
        // no campo codigoEstadoTituloCobranca os códigos 05, 06, 07 ou 09. Data fim deverá ser maior que a data de início.
        // Campo não obrigatório.
        // Se informado, deverá ser preenchido dataInicioMovimento.
        // Ex: 28.04.2020
        public string dataFimMovimento { get; set; }

        // Código da situação atual do boleto.
        // Domínios:
        // 01 - NORMAL
        // 02 - MOVIMENTO CARTORIO
        // 03 - EM CARTORIO
        // 04 - TITULO COM OCORRENCIA DE CARTORIO
        // 05 - PROTESTADO ELETRONICO
        // 06 - LIQUIDADO
        // 07 - BAIXADO
        // 08 - TITULO COM PENDENCIA DE CARTORIO
        // 09 - TITULO PROTESTADO MANUAL
        // 10 - TITULO BAIXADO/PAGO EM CARTORIO
        // 11 - TITULO LIQUIDADO/PROTESTADO
        // 12 - TITULO LIQUID/PGCRTO
        // 13 - TITULO PROTESTADO AGUARDANDO BAIXA
        // 14 - TITULO EM LIQUIDACAO
        // 15 - TITULO AGENDADO
        // 16 - TITULO CREDITADO
        // 17 - PAGO EM CHEQUE - AGUARD.LIQUIDACAO 1
        // 8 - PAGO PARCIALMENTE CREDITADO
        // 80 - EM PROCESSAMENTO(ESTADO TRANSITÓRIO)
        public int? codigoEstadoTituloCobranca { get; set; }

        // Indica se o Boleto está vencido ou não.
        // Campo obrigatoriamente MAIÚSCULO.
        // Domínio: S para boletos vencidos N para boletos não vencidos
        public string boletoVencido { get; set; }

        // Somente deve ser utilizado em caso de pesquisas que retornem mais de 300 boletos.
        // Caso o campo RPST "Indicador Continuidade" retorne com o valor "S", o usuário deve informar o conteúdo do campo RPST "Numero Ultimo Registro"
        // a partir do qual será iniciada nova consulta.
        public long? indice { get; set; }
    }
    #endregion

    #region Response
    public class ListaBoletosResponse
    {
        public string indicadorContinuidade { get; set; }
        public List<Boleto> boletos { get; set; }
        public int quantidadeRegistros { get; set; }
        public int proximoIndice { get; set; }
        public int statusCode { get; set; }
        public ListaBoletosResponseErros mensagemErros { get; set; }
    }

    public class Boleto
    {
        public string numeroBoletoBB { get; set; }
        public string dataRegistro { get; set; }
        public string dataVencimento { get; set; }
        public double valorOriginal { get; set; }
        public int carteiraConvenio { get; set; }
        public int variacaoCarteiraConvenio { get; set; }
        public int codigoEstadoTituloCobranca { get; set; }
        public string estadoTituloCobranca { get; set; }
        public int contrato { get; set; }
        public string dataMovimento { get; set; }
        public string dataCredito { get; set; }
        public double valorAtual { get; set; }
        public double valorPago { get; set; }

    }

    public class ListaBoletosResponseErro
    {
        [JsonProperty("codigoMensagem")]
        public string codigo { get; set; }

        [JsonProperty("versaoMensagem")]
        public string versao { get; set; }

        [JsonProperty("codigoRetorno")]
        public string ocorrencia { get; set; }

        [JsonProperty("textoMensagem")]
        public string mensagem { get; set; }
    }

    public class ListaBoletosResponseErros
    {
        public List<ListaBoletosResponseErro> erros { get; set; }
    }
    #endregion
}