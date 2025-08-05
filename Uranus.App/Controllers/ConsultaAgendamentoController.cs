using System.Linq;
using System.Web.Http;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.App.Controllers
{
    public class ConsultaAgendamentoController : ApiController
    {
        public class ConsultaAgendamentoRequest
        {
            public long Id { get; set; }
        }

        public class ConsultaAgendamentoResponse
        {
            public int IdProfissional { get; set; }

            public string Data { get; set; }

            public string Hora { get; set; }

            public string Status { get; set; }
        }

        // POST api/consultaagendamento
        public ConsultaAgendamentoResponse Post(string token, [FromBody] ConsultaAgendamentoRequest request)
        {
            ConsultaAgendamentoResponse response = new ConsultaAgendamentoResponse();

            try
            {
                if (TokenController.Validar(token))
                {
                    var agenda = Consultar(request.Id);

                    if (agenda != null)
                    {
                        response.IdProfissional = agenda.Agendas.IdProfissional;
                        response.Data = agenda.Agendas.Data.ToString("dd-MM-yyyy");
                        response.Hora = agenda.Agendas.Hora;
                        response.Status = "Sucesso";
                    }
                    else
                    {
                        response.Status = "Insucesso";
                    }

                    return response;
                }
                else
                {
                    response.Status = "Token Inválido";
                    return response;
                }
            }
            catch (System.Exception ex)
            {
                response.Status = "Falha";
                return response;
            }

        }

        public static PreAgendamento Consultar(long Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.PreAgendamento.Include("Agendas")
                            where d.Id == Id
                            where d.IdAgenda != null
                            select d;

                return query.FirstOrDefault();
            }
        }
    }
}
