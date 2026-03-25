using System.Collections.Generic;

namespace Sigman.BB.BBEntidades
{
    #region Request
    public class RegistraBoletoRequest
    {
        public int numeroConvenio { get; set; }
        public int numeroCarteira { get; set; }
        public int numeroVariacaoCarteira { get; set; }
        public int codigoModalidade { get; set; }
        public string dataEmissao { get; set; }
        public string dataVencimento { get; set; }
        public decimal valorOriginal { get; set; }
        public decimal valorAbatimento { get; set; }
        public int quantidadeDiasProtesto { get; set; }
        public int quantidadeDiasNegativacao { get; set; }
        public int orgaoNegativador { get; set; }
        public string indicadorAceiteTituloVencido { get; set; }
        public int numeroDiasLimiteRecebimento { get; set; }
        public string codigoAceite { get; set; }
        public int codigoTipoTitulo { get; set; }
        public string descricaoTipoTitulo { get; set; }
        public string indicadorPermissaoRecebimentoParcial { get; set; }
        public string numeroTituloBeneficiario { get; set; }
        public string campoUtilizacaoBeneficiario { get; set; }
        public string numeroTituloCliente { get; set; }
        public string mensagemBloquetoOcorrencia { get; set; }
        public Desconto desconto { get; set; }
        public SegundoDesconto segundoDesconto { get; set; }
        public TerceiroDesconto terceiroDesconto { get; set; }
        public JurosMora jurosMora { get; set; }
        public Multa multa { get; set; }
        public Pagador pagador { get; set; }
        public BeneficiarioFinal beneficiarioFinal { get; set; }
        public string indicadorPix { get; set; }
    }

    public class BeneficiarioFinal
    {
        public int tipoInscricao { get; set; }
        public long numeroInscricao { get; set; }
        public string nome { get; set; }
    }

    public class Desconto
    {
        public int tipo { get; set; }
        public string dataExpiracao { get; set; }
        public int porcentagem { get; set; }
        public int valor { get; set; }
    }

    public class JurosMora
    {
        public int tipo { get; set; }
        public int porcentagem { get; set; }
        public int valor { get; set; }
    }

    public class Multa
    {
        public int tipo { get; set; }
        public string data { get; set; }
        public int porcentagem { get; set; }
        public int valor { get; set; }
    }

    public class Pagador
    {
        public int tipoInscricao { get; set; }
        public long numeroInscricao { get; set; }
        public string nome { get; set; }
        public string endereco { get; set; }
        public int cep { get; set; }
        public string cidade { get; set; }
        public string bairro { get; set; }
        public string uf { get; set; }
        public string telefone { get; set; }
    }

    public class SegundoDesconto
    {
        public string dataExpiracao { get; set; }
        public int porcentagem { get; set; }
        public int valor { get; set; }
    }

    public class TerceiroDesconto
    {
        public string dataExpiracao { get; set; }
        public int porcentagem { get; set; }
        public int valor { get; set; }
    }
    #endregion

    #region Response
    public class RegistraBoletoResponse
    {
        public string numero { get; set; }
        public int numeroCarteira { get; set; }
        public int numeroVariacaoCarteira { get; set; }
        public int codigoCliente { get; set; }
        public string linhaDigitavel { get; set; }
        public string codigoBarraNumerico { get; set; }
        public int numeroContratoCobranca { get; set; }
        public Beneficiario beneficiario { get; set; }
        public QrCode qrCode { get; set; }
        public int statusCode { get; set; }
        public RegistraBoletoResponseErros mensagemErros { get; set; }
    }

    public class Beneficiario
    {
        public int agencia { get; set; }
        public int contaCorrente { get; set; }
        public int tipoEndereco { get; set; }
        public string logradouro { get; set; }
        public string bairro { get; set; }
        public string cidade { get; set; }
        public int codigoCidade { get; set; }
        public string uf { get; set; }
        public int cep { get; set; }
        public string indicadorComprovacao { get; set; }
    }

    public class QrCode
    {
        public string url { get; set; }
        public string txId { get; set; }
        public string emv { get; set; }
    }

    public class RegistraBoletoResponseErro
    {
        public string codigo { get; set; }
        public string versao { get; set; }
        public string ocorrencia { get; set; }
        public string mensagem { get; set; }
    }

    public class RegistraBoletoResponseErros
    {
        public List<RegistraBoletoResponseErro> erros { get; set; }
    }
    #endregion
}