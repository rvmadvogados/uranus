using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigman.BB.BBEntidades
{
    #region Request
    public class ListaConveniosRequest
    {
        public string dataMovimentoRetornoInicial { get; set; }
        public string dataMovimentoRetornoFinal { get; set; }
        public int codigoPrefixoAgencia { get; set; }
        public int numeroContaCorrente { get; set; }
        public int numeroCarteiraCobranca { get; set; }
        public int numeroVariacaoCarteiraCobranca { get; set; }
        public int numeroRegistroPretendido { get; set; }
        public int quantidadeRegistroPretendido { get; set; }
    }
    #endregion

    #region Response
    public class ListaConveniosResponse
    {
        public string indicadorContinuidade { get; set; }
        public int numeroUltimoRegistro { get; set; }
        public List<ListaRegistro> listaRegistro { get; set; }
        public int statusCode { get; set; }
        public ListaConveniosResponseErros mensagemErros { get; set; }
    }

    public class ListaRegistro
    {
        public string dataMovimentoRetorno { get; set; }
        public int numeroConvenio { get; set; }
        public string numeroTituloCobranca { get; set; }
        public int codigoComandoAcao { get; set; }
        public int codigoPrefixoAgencia { get; set; }
        public int numeroContaCorrente { get; set; }
        public int numeroCarteiraCobranca { get; set; }
        public int numeroVariacaoCarteiraCobranca { get; set; }
        public int tipoCobranca { get; set; }
        public string codigoControleParticipante { get; set; }
        public int codigoEspecieBoleto { get; set; }
        public string dataVencimentoBoleto { get; set; }
        public int valorBoleto { get; set; }
        public int codigoBancoRecebedor { get; set; }
        public int codigoPrefixoAgenciaRecebedora { get; set; }
        public string dataCreditoPagamentoBoleto { get; set; }
        public int valorTarifa { get; set; }
        public int valorOutrasDespesasCalculadas { get; set; }
        public int valorJurosDesconto { get; set; }
        public double valorIofDesconto { get; set; }
        public int valorAbatimento { get; set; }
        public int valorDesconto { get; set; }
        public int valorRecebido { get; set; }
        public int valorJurosMora { get; set; }
        public int valorOutrosValoresRecebidos { get; set; }
        public int valorAbatimentoNaoUtilizado { get; set; }
        public int valorLancamento { get; set; }
        public int codigoFormaPagamento { get; set; }
        public int codigoValorAjuste { get; set; }
        public double valorAjuste { get; set; }
        public int codigoAutorizacaoPagamentoParcial { get; set; }
        public int codigoCanalPagamento { get; set; }
        public string URL { get; set; }
        public string textoIdentificadorQRCode { get; set; }
        public int quantidadeDiasCalculo { get; set; }
        public double valorTaxaDesconto { get; set; }
        public double valorTaxaIOF { get; set; }
        public int naturezaRecebimento { get; set; }
        public int codigoTipoCobrancaComando { get; set; }
        public string dataLiquidacaoBoleto { get; set; }
    }

    public class ListaConveniosResponseErro
    {
        public string codigo { get; set; }
        public string versao { get; set; }
        public string mensagem { get; set; }
        public string ocorrencia { get; set; }
    }

    public class ListaConveniosResponseErros
    {
        public List<ListaConveniosResponseErro> erros { get; set; }
    }

    #endregion
}