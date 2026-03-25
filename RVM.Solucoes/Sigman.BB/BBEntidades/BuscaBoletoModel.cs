using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sigman.BB.BBEntidades
{
    #region Request
    public class BuscaBoletoRequest
    {
        // Número do título de cobrança.
        public string id { get; set; }

        // Número do convênio.
        public long numeroConvenio { get; set; }
    }
    #endregion

    #region Response
    public class BuscaBoletoResponse
    {
        public string codigoLinhaDigitavel { get; set; }
        public string textoEmailPagador { get; set; }
        public string textoMensagemBloquetoTitulo { get; set; }
        public int codigoTipoMulta { get; set; }
        public int codigoCanalPagamento { get; set; }
        public int numeroContratoCobranca { get; set; }
        public int codigoTipoInscricaoSacado { get; set; }
        public long numeroInscricaoSacadoCobranca { get; set; }
        public int codigoEstadoTituloCobranca { get; set; }
        public int codigoTipoTituloCobranca { get; set; }
        public int codigoModalidadeTitulo { get; set; }
        public string codigoAceiteTituloCobranca { get; set; }
        public int codigoPrefixoDependenciaCobrador { get; set; }
        public int codigoIndicadorEconomico { get; set; }
        public string numeroTituloCedenteCobranca { get; set; }
        public int codigoTipoJuroMora { get; set; }
        public string dataEmissaoTituloCobranca { get; set; }
        public string dataRegistroTituloCobranca { get; set; }
        public string dataVencimentoTituloCobranca { get; set; }
        public int valorOriginalTituloCobranca { get; set; }
        public int valorAtualTituloCobranca { get; set; }
        public int valorPagamentoParcialTitulo { get; set; }
        public int valorAbatimentoTituloCobranca { get; set; }
        public int percentualImpostoSobreOprFinanceirasTituloCobranca { get; set; }
        public int valorImpostoSobreOprFinanceirasTituloCobranca { get; set; }
        public int valorMoedaTituloCobranca { get; set; }
        public int percentualJuroMoraTitulo { get; set; }
        public int valorJuroMoraTitulo { get; set; }
        public int percentualMultaTitulo { get; set; }
        public int valorMultaTituloCobranca { get; set; }
        public int quantidadeParcelaTituloCobranca { get; set; }
        public string dataBaixaAutomaticoTitulo { get; set; }
        public string textoCampoUtilizacaoCedente { get; set; }
        public string indicadorCobrancaPartilhadoTitulo { get; set; }
        public string nomeSacadoCobranca { get; set; }
        public string textoEnderecoSacadoCobranca { get; set; }
        public string nomeBairroSacadoCobranca { get; set; }
        public string nomeMunicipioSacadoCobranca { get; set; }
        public string siglaUnidadeFederacaoSacadoCobranca { get; set; }
        public int numeroCepSacadoCobranca { get; set; }
        public int valorMoedaAbatimentoTitulo { get; set; }
        public string dataProtestoTituloCobranca { get; set; }
        public int codigoTipoInscricaoSacador { get; set; }
        public long numeroInscricaoSacadorAvalista { get; set; }
        public string nomeSacadorAvalistaTitulo { get; set; }
        public int percentualDescontoTitulo { get; set; }
        public string dataDescontoTitulo { get; set; }
        public int valorDescontoTitulo { get; set; }
        public int codigoDescontoTitulo { get; set; }
        public int percentualSegundoDescontoTitulo { get; set; }
        public string dataSegundoDescontoTitulo { get; set; }
        public int valorSegundoDescontoTitulo { get; set; }
        public int codigoSegundoDescontoTitulo { get; set; }
        public int percentualTerceiroDescontoTitulo { get; set; }
        public string dataTerceiroDescontoTitulo { get; set; }
        public int valorTerceiroDescontoTitulo { get; set; }
        public int codigoTerceiroDescontoTitulo { get; set; }
        public string dataMultaTitulo { get; set; }
        public int numeroCarteiraCobranca { get; set; }
        public int numeroVariacaoCarteiraCobranca { get; set; }
        public int quantidadeDiaProtesto { get; set; }
        public int quantidadeDiaPrazoLimiteRecebimento { get; set; }
        public string dataLimiteRecebimentoTitulo { get; set; }
        public string indicadorPermissaoRecebimentoParcial { get; set; }
        public string textoCodigoBarrasTituloCobranca { get; set; }
        public int codigoOcorrenciaCartorio { get; set; }
        public int valorImpostoSobreOprFinanceirasRecebidoTitulo { get; set; }
        public int valorAbatimentoTotal { get; set; }
        public int valorJuroMoraRecebido { get; set; }
        public int valorDescontoUtilizado { get; set; }
        public int valorPagoSacado { get; set; }
        public int valorCreditoCedente { get; set; }
        public int codigoTipoLiquidacao { get; set; }
        public string dataCreditoLiquidacao { get; set; }
        public string dataRecebimentoTitulo { get; set; }
        public int codigoPrefixoDependenciaRecebedor { get; set; }
        public int codigoNaturezaRecebimento { get; set; }
        public string numeroIdentidadeSacadoTituloCobranca { get; set; }
        public string codigoResponsavelAtualizacao { get; set; }
        public int codigoTipoBaixaTitulo { get; set; }
        public int valorMultaRecebido { get; set; }
        public int valorReajuste { get; set; }
        public int valorOutroRecebido { get; set; }
        public int codigoIndicadorEconomicoUtilizadoInadimplencia { get; set; }
        public int statusCode { get; set; }
        public BuscaBoletoResponseErrors mensagemErros { get; set; }
    }

    public class BuscaBoletoResponseError
    {
        [JsonProperty("code")]
        public string codigo { get; set; }

        [JsonProperty("message")]
        public string mensagem { get; set; }
    }

    public class BuscaBoletoResponseErrors
    {
        public List<BuscaBoletoResponseError> errors { get; set; }
    }
    #endregion
}