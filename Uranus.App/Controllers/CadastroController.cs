using System.Linq;
using System.Web.Http;
using Uranus.Common;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.App.Controllers
{
    public class CadastroController : ApiController
    {
        public class CadastroRequest
        {
            public string CpfCnpj { get; set; }
        }

        public class CadastroResponse
        {
            public string Status { get; set; }
        }

        // POST api/cadastro
        public CadastroResponse Post(string token, [FromBody] CadastroRequest request)
        {
            CadastroResponse response = new CadastroResponse();

            try
            {
                if (TokenController.Validar(token))
                {
                    if (UtilCpfCnpj.IsValid(request.CpfCnpj))
                    {
                        var cliente = Consultar(Util.OnlyNumbers(request.CpfCnpj));

                        if (cliente != null)
                        {
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
                        response.Status = "CPF ou CNPJ Inválido";
                        return response;
                    }
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

        public static Clientes Consultar(string CpfCnpj)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Clientes
                            where d.Pessoas.CpfCnpj == CpfCnpj
                            select d;

                return query.FirstOrDefault();
            }
        }
    }
}
