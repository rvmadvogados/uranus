using Newtonsoft.Json;
using System;
using System.Web.Http;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.App.Controllers
{
    public class PreAgendamentoController : ApiController
    {
        public class PreAgendamentoRequest
        {
            public string CpfCnpj { get; set; }
            public string Turno { get; set; }
            public int? IdProfissional { get; set; }
            public int? IdArea { get; set; }
        }

        public class PreAgendamentoResponse
        {
            public long? Id { get; set; }
            public string Status { get; set; }
        }

        // POST api/preagendamento
        public PreAgendamentoResponse Post(string token, [FromBody] PreAgendamentoRequest request)
        {
            PreAgendamentoResponse response = new PreAgendamentoResponse();

            try
            {
                if (TokenController.Validar(token))
                {
                    var cliente = PreCadastroController.Consultar(request.CpfCnpj);
                    var agenda = JsonConvert.DeserializeObject<PreAgendamento>(JsonConvert.SerializeObject(request));
                    agenda.IdCliente = cliente.Id;
                    //agenda.Data = DateTime.Now();

                    var idAgendamento = Inserir(agenda);

                    if (idAgendamento > 0)
                    {
                        response.Id = idAgendamento;
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

        public static long Inserir(PreAgendamento agendamento)
        {
            using (var context = new UranusEntities())
            {
                context.PreAgendamento.Add(agendamento);
                context.SaveChanges();

                return agendamento.Id;
            }
        }
    }
}
