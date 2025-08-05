using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Uranus.Common;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.App.Controllers
{
    public class ConsultaEventosController : ApiController
    {
        #region EventosRequest
        public class Channel
        {
            public string type { get; set; }
            public string id { get; set; }
        }

        public class Contact
        {
            public string type { get; set; }
            public string name { get; set; }
            public int uid { get; set; }
            public string key { get; set; }
            public Fields fields { get; set; }
        }

        public class Data
        {
            public string id { get; set; }
            public string token { get; set; }
            public string cpfcnpj { get; set; }
            //public string telefone { get; set; }
        }

        public class Fields
        {
            public string cnpjcpf { get; set; }
            //public string telefone { get; set; }
            public string token { get; set; }
        }

        public class EventosRequest
        {
            public string text { get; set; }
            public Contact contact { get; set; }
            public Channel channel { get; set; }
            public Data data { get; set; }
            public int id { get; set; }
            public int clienteId { get; set; }
            public string origin { get; set; }
        }
        #endregion

        #region EventosResponse
        public class Attachment
        {
            public string position { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public string url { get; set; }
        }

        public class EventosResponse
        {
            public string type { get; set; }
            public string text { get; set; }
            public List<Attachment> attachments { get; set; }
        }
        #endregion

        // POST api/consultaeventos
        public EventosResponse Post([FromBody] EventosRequest request)
        {
            EventosResponse response = new EventosResponse();

            try
            {
                var re = Request;
                var headers = re.Headers;
                string token = string.Empty;

                if (headers.Contains("mz-integration"))
                {
                    token = headers.GetValues("mz-integration").First();
                }

                if (TokenController.Validar(token))
                {
                    var evento = Listar(int.Parse(request.data.id));

                    if (evento != null)
                    {
                        response.type = "INFORMATION";
                        response.text = string.Format("Evento {0} - {1}", evento.Data.Value.ToString("dd/MM/yyyy"), (((HttpUtility.HtmlDecode(Util.StripHTML(Uri.UnescapeDataString(evento.Descricao)))).Replace("\r\n", " ")).Replace("\n", string.Empty)).Replace("\r", string.Empty));

                        //response.Eventos = processosResponse;
                        //response.Status = "Sucesso";
                    }
                    else
                    {
                        //response.Status = "Insucesso";
                    }

                    return response;
                }
                else
                {
                    //response.Status = "Token Inválido";
                    return response;
                }
            }
            catch (System.Exception ex)
            {
                //response.Status = "Falha";
                return response;
            }
        }

        public static ProcessosAcoesEventos Listar(int idacao)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAcoesEventos
                            where d.IdProcessosAcao == idacao
                            where d.ProcessosEventos.WhatsApp == true
                            orderby d.ID descending
                            select d;

                return query.ToList().Take(1).FirstOrDefault();
            }
        }
    }
}
