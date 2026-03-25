using System.Collections.Generic;

namespace Sigman.BB.BBEntidades
{
    #region Request
    public class CancelaPixRequest
    {
        public int numeroConvenio { get; set; }
    }
    #endregion

    #region Response
    public class CancelaPixResponse
    {
        public Pix pix { get; set; }
        public QrCodeCancelaPix qrCode { get; set; }
        public int statusCode { get; set; }
        public CancelaPixResponseErros mensagemErros { get; set; }
    }

    public class Pix
    {
        public string chave { get; set; }
    }

    public class QrCodeCancelaPix
    {
        public string url { get; set; }
        public string txId { get; set; }
        public string emv { get; set; }
    }

    public class CancelaPixResponseErro
    {
        public string codigo { get; set; }
        public string versao { get; set; }
        public string mensagem { get; set; }
        public string ocorrencia { get; set; }
    }

    public class CancelaPixResponseErros
    {
        public List<CancelaPixResponseErro> erros { get; set; }
    }
    #endregion
}