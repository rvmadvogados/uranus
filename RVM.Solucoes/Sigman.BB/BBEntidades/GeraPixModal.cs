using System.Collections.Generic;

namespace Sigman.BB.BBEntidades
{
    #region Request
    public class GeraPixRequest
    {
        public int numeroConvenio { get; set; }
    }
    #endregion

    #region Response
    public class GeraPixResponse
    {
        public PixGeraPix pix { get; set; }
        public QrCodeGeraPix qrCode { get; set; }
        public int statusCode { get; set; }
        public GeraPixResponseErros mensagemErros { get; set; }
    }

    public class PixGeraPix
    {
        public string chave { get; set; }
    }

    public class QrCodeGeraPix
    {
        public string url { get; set; }
        public string txId { get; set; }
        public string emv { get; set; }
    }

    public class GeraPixResponseErro
    {
        public string codigo { get; set; }
        public string versao { get; set; }
        public string mensagem { get; set; }
        public string ocorrencia { get; set; }
    }

    public class GeraPixResponseErros
    {
        public List<GeraPixResponseErro> erros { get; set; }
    }
    #endregion
}