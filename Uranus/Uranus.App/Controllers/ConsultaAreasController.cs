using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.App.Controllers
{
    public class ConsultaAreasController : ApiController
    {
        public class ConsultaAreasResponse
        {
            public List<AreasResponse> Areas { get; set; }
            public string Status { get; set; }
        }

        public class AreasResponse
        {
            public int Id { get; set; }

            public string Nome { get; set; }
        }

        // GET api/consultaareas
        public ConsultaAreasResponse Get(string token)
        {
            ConsultaAreasResponse response = new ConsultaAreasResponse();

            try
            {
                if (TokenController.Validar(token))
                {
                    var areas = Listar();

                    if (areas != null && areas.Count > 0)
                    {
                        List<AreasResponse> areasResponse = new List<AreasResponse>();

                        foreach (var area in areas)
                        {
                            AreasResponse areaResponse = new AreasResponse();
                            areaResponse.Id = area.ID;
                            areaResponse.Nome = area.AreaAtuacao;

                            areasResponse.Add(areaResponse);
                        }

                        response.Areas = areasResponse;
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

        public static List<ProcessosAreas> Listar()
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAreas
                            orderby d.AreaAtuacao
                            select d;

                return query.ToList();
            }
        }
    }
}
