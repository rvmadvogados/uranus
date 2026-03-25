using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigman.BB.BBEntidades
{
    #region Request
    public class BaixaCancelaBoletoRequest
    {
        public int numeroConvenio { get; set; }
    }
    #endregion

    #region Response
    public class BaixaCancelaBoletoResponse
    {
        public string numeroContratoCobranca { get; set; }
        public string dataBaixa { get; set; }
        public string horarioBaixa { get; set; }
        public int statusCode { get; set; }
        public BaixaCancelaBoletoResponseErrors mensagemErros { get; set; }
    }

    public class BaixaCancelaBoletoResponseError
    {
        [JsonProperty("code")]
        public string codigo { get; set; }

        [JsonProperty("message")]
        public string mensagem { get; set; }
    }

    public class BaixaCancelaBoletoResponseErrors
    {
        public List<BaixaCancelaBoletoResponseError> errors { get; set; }
    }
    #endregion
}