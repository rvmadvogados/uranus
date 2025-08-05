using System.Linq;
using Newtonsoft.Json;
using System.Web.Http;
using Uranus.Data;
using Uranus.Domain;
using Uranus.Common;
using System.Web.Query.Dynamic;

namespace Uranus.App.Controllers
{
    public class PreCadastroController : ApiController
    {
        public class PreCadastroRequest
        {
            public string CpfCnpj { get; set; }
            public string Nome { get; set; }
            public string Telefone { get; set; }
            public string Email { get; set; }
        }

        public class PreCadastroResponse
        {
            public long? Id { get; set; }
            public string Status { get; set; }
        }

        // POST api/precadastro
        public PreCadastroResponse Post(string token, [FromBody] PreCadastroRequest request)
        {
            PreCadastroResponse response = new PreCadastroResponse();

            try
            {
                if (TokenController.Validar(token))
                {
                    if (UtilCpfCnpj.IsValid(request.CpfCnpj))
                    {
                        var cliente = Consultar(request.CpfCnpj);

                        if (cliente == null)
                        {
                            var idCadastro = Inserir(JsonConvert.DeserializeObject<PreCadastro>(JsonConvert.SerializeObject(request)));

                            if (idCadastro > 0)
                            {
                                response.Id = idCadastro;
                                response.Status = "Sucesso";
                            }
                            else
                            {
                                response.Status = "Insucesso";
                            }
                        }
                        else
                        {
                            response.Id = cliente.Id;
                            response.Status = "Sucesso";
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

        public static PreCadastro Consultar(string CpfCnpj)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.PreCadastro
                            where d.CpfCnpj == CpfCnpj
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static long Inserir(PreCadastro cadastro)
        {
            using (var context = new UranusEntities())
            {
                context.PreCadastro.Add(cadastro);
                context.SaveChanges();

                return cadastro.Id;
            }
        }
    }
}
