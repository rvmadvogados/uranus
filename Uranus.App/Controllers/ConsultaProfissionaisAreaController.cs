using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.App.Controllers
{
    public class ConsultaProfissionaisAreaController : ApiController
    {
        public class ConsultaProfissionaisAreaRequest
        {
            public int IdArea { get; set; }
        }

        public class ConsultaProfissionaisAreaResponse
        {
            public List<ProfissionaisAreaResponse> Profissionais { get; set; }

            public string Status { get; set; }
        }

        public class ProfissionaisAreaResponse
        {
            public int Id { get; set; }

            public string Nome { get; set; }
        }

        // POST api/consultaprofissionaisarea
        public ConsultaProfissionaisAreaResponse Post(string token, [FromBody] ConsultaProfissionaisAreaRequest request)
        {
            ConsultaProfissionaisAreaResponse response = new ConsultaProfissionaisAreaResponse();

            try
            {
                if (TokenController.Validar(token))
                {
                    var profissionais = Listar(request.IdArea);

                    if (profissionais != null)
                    {
                        List<ProfissionaisAreaResponse> profissionaisAreaResponse = new List<ProfissionaisAreaResponse>();

                        foreach (var profissional in profissionais)
                        {
                            ProfissionaisAreaResponse profissionalAreaResponse = new ProfissionaisAreaResponse();
                            profissionalAreaResponse.Id = profissional.ID;
                            profissionalAreaResponse.Nome = profissional.Pessoas.Nome;

                            profissionaisAreaResponse.Add(profissionalAreaResponse);
                        }

                        response.Profissionais = profissionaisAreaResponse;
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

        public static List<Profissionais> Listar(int Id)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Profissionais.Include("Pessoas")
                                                           .Include("Usuarios")
                            where d.IDTipoProfissional == 6
                            where d.Usuarios.Bloqueio == false
                            select d;

                return query.ToList();
            }
        }
    }
}
