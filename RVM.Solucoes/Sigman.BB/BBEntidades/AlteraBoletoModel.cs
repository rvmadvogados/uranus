using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sigman.BB.BBEntidades
{
    #region Request
    public class AlteraBoletoRequest
    {
        public int numeroConvenio { get; set; }
        public string indicadorNovaDataVencimento { get; set; }
        public AlteracaoData alteracaoData { get; set; }
        public string indicadorAtribuirDesconto { get; set; }
        public DescontoAlteraBoleto desconto { get; set; }
        public string indicadorAlterarDesconto { get; set; }
        public AlteracaoDesconto alteracaoDesconto { get; set; }
        public string indicadorAlterarDataDesconto { get; set; }
        public AlteracaoDataDesconto alteracaoDataDesconto { get; set; }
        public string indicadorProtestar { get; set; }
        public Protesto protesto { get; set; }
        public string indicadorSustacaoProtesto { get; set; }
        public string indicadorCancelarProtesto { get; set; }
        public string indicadorIncluirAbatimento { get; set; }
        public Abatimento abatimento { get; set; }
        public string indicadorAlterarAbatimento { get; set; }
        public AlteracaoAbatimento alteracaoAbatimento { get; set; }
        public string indicadorCobrarJuros { get; set; }
        public Juros juros { get; set; }
        public string indicadorDispensarJuros { get; set; }
        public string indicadorCobrarMulta { get; set; }
        public MultaAlteraBoleto multa { get; set; }
        public string indicadorDispensarMulta { get; set; }
        public string indicadorNegativar { get; set; }
        public Negativacao negativacao { get; set; }
        public string indicadorAlterarSeuNumero { get; set; }
        public AlteracaoSeuNumero alteracaoSeuNumero { get; set; }
        public string indicadorAlterarEnderecoPagador { get; set; }
        public AlteracaoEndereco alteracaoEndereco { get; set; }
        public string indicadorAlterarPrazoBoletoVencido { get; set; }
        public AlteracaoPrazo alteracaoPrazo { get; set; }
    }

    public class Abatimento
    {
        public int valorAbatimento { get; set; }
    }

    public class AlteracaoAbatimento
    {
        public int novoValorAbatimento { get; set; }
    }

    public class AlteracaoData
    {
        public string novaDataVencimento { get; set; }
    }

    public class AlteracaoDataDesconto
    {
        public string novaDataLimitePrimeiroDesconto { get; set; }
        public string novaDataLimiteSegundoDesconto { get; set; }
        public string novaDataLimiteTerceiroDesconto { get; set; }
    }

    public class AlteracaoDesconto
    {
        public int tipoPrimeiroDesconto { get; set; }
        public int novoValorPrimeiroDesconto { get; set; }
        public int novoPercentualPrimeiroDesconto { get; set; }
        public string novaDataLimitePrimeiroDesconto { get; set; }
        public int tipoSegundoDesconto { get; set; }
        public int novoValorSegundoDesconto { get; set; }
        public int novoPercentualSegundoDesconto { get; set; }
        public string novaDataLimiteSegundoDesconto { get; set; }
        public int tipoTerceiroDesconto { get; set; }
        public int novoValorTerceiroDesconto { get; set; }
        public int novoPercentualTerceiroDesconto { get; set; }
        public string novaDataLimiteTerceiroDesconto { get; set; }
    }

    public class AlteracaoEndereco
    {
        public string enderecoPagador { get; set; }
        public string bairroPagador { get; set; }
        public string cidadePagador { get; set; }
        public string UFPagador { get; set; }
        public int CEPPagador { get; set; }
    }

    public class AlteracaoPrazo
    {
        public int quantidadeDiasAceite { get; set; }
    }

    public class AlteracaoSeuNumero
    {
        public string codigoSeuNumero { get; set; }
    }

    public class DescontoAlteraBoleto
    {
        public int tipoPrimeiroDesconto { get; set; }
        public int valorPrimeiroDesconto { get; set; }
        public int percentualPrimeiroDesconto { get; set; }
        public string dataPrimeiroDesconto { get; set; }
        public int tipoSegundoDesconto { get; set; }
        public int valorSegundoDesconto { get; set; }
        public int percentualSegundoDesconto { get; set; }
        public string dataSegundoDesconto { get; set; }
        public int tipoTerceiroDesconto { get; set; }
        public int valorTerceiroDesconto { get; set; }
        public int percentualTerceiroDesconto { get; set; }
        public string dataTerceiroDesconto { get; set; }
    }

    public class Juros
    {
        public int tipoJuros { get; set; }
        public int valorJuros { get; set; }
        public int taxaJuros { get; set; }
    }

    public class MultaAlteraBoleto
    {
        public int tipoMulta { get; set; }
        public int valorMulta { get; set; }
        public string dataInicioMulta { get; set; }
        public int taxaMulta { get; set; }
    }

    public class Negativacao
    {
        public int quantidadeDiasNegativacao { get; set; }
        public int tipoNegativacao { get; set; }
    }

    public class Protesto
    {
        public int quantidadeDiasProtesto { get; set; }
    }
    #endregion

    #region Response
    public class AlteraBoletoResponse
    {
        public int numeroContratoCobranca { get; set; }
        public string dataAtualizacao { get; set; }
        public string horarioAtualizacao { get; set; }
        public int statusCode { get; set; }
        public AlteraBoletoResponseErrors mensagemErros { get; set; }
    }

    public class AlteraBoletoResponseError
    {
        [JsonProperty("code")]
        public string codigo { get; set; }

        [JsonProperty("message")]
        public string mensagem { get; set; }
    }

    public class AlteraBoletoResponseErrors
    {
        public List<AlteraBoletoResponseError> errors { get; set; }
    }
    #endregion
}