using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sigman.BB.BBEntidades
{
    #region Request
    public class ConsultaPixRequest
    {
        // Número do título de cobrança.
        public string id { get; set; }

        // Número do convênio.
        public long numeroConvenio { get; set; }
    }
    #endregion

    #region Response
    public class ConsultaPixResponse
    {
        public string id { get; set; }
        public string dataRegistroTituloCobranca { get; set; }
        public int agenciaBeneficiario { get; set; }
        public int contaBeneficiario { get; set; }
        public decimal valorOriginalTituloCobranca { get; set; }
        public string validadeTituloCobranca { get; set; }
        public PixConsultaPix pix { get; set; }
        public QrCodeConsultaPix qrCode { get; set; }
        public int statusCode { get; set; }
        public ConsultaPixResponseErros mensagemErros { get; set; }
    }

    public class PixConsultaPix
    {
        public int valorRecebido { get; set; }
        public string timestamp { get; set; }
        public string chave { get; set; }
        public string textoRetorno { get; set; }
        public int idInstituicaoPagador { get; set; }
        public int agenciaPagador { get; set; }
        public int contaPagador { get; set; }
        public int tipoPessoaPagador { get; set; }
        public int idPagador { get; set; }
    }

    public class QrCodeConsultaPix
    {
        public string url { get; set; }
        public string txId { get; set; }
        public string emv { get; set; }
        public int tipo { get; set; }
    }

    public class ConsultaPixResponseErro
    {
        public string codigo { get; set; }
        public string versao { get; set; }
        public string mensagem { get; set; }
        public string ocorrencia { get; set; }
    }

    public class ConsultaPixResponseErros
{
        public List<ConsultaPixResponseErro> erros { get; set; }
    }
    #endregion
}