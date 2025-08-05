using Newtonsoft.Json;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using Uranus.Common;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.App.Controllers
{
    public class UsuarioController : ApiController
    {
        public class UsuarioRequest
        {
            public string Login { get; set; }
            public string Senha { get; set; }
        }

        public class UsuarioResponse
        {
            public long? Id { get; set; }
            public string Status { get; set; }
        }

        // POST api/usuario
        public UsuarioResponse Post(string token, [FromBody] UsuarioRequest request)
        {
            UsuarioResponse response = new UsuarioResponse();

            try
            {
                if (TokenController.Validar(token))
                {
                    if (UtilCpfCnpj.IsValid(request.Login))
                    {
                        var usuario = Consultar(request.Login);

                        if (usuario == null)
                        {
                            var idCadastro = Inserir(JsonConvert.DeserializeObject<PessoasUsuariosAplicativo>(JsonConvert.SerializeObject(request)));

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
                            response.Id = usuario.Id;
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

        // PUT api/usuario
        public UsuarioResponse Put(string token, [FromBody] UsuarioRequest request)
        {
            UsuarioResponse response = new UsuarioResponse();

            try
            {
                if (TokenController.Validar(token))
                {
                    if (UtilCpfCnpj.IsValid(request.Login))
                    {
                        var usuario = Consultar(request.Login);

                        if (usuario != null)
                        {
                            try
                            {
                                usuario.Senha = request.Senha;
                                Salvar(usuario);
                                response.Id = usuario.Id;
                                response.Status = "Sucesso";
                            }
                            catch (System.Exception ex)
                            {
                                response.Status = "Insucesso";
                            }
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

        // GET api/usuario
        public UsuarioResponse GET(string token, [FromBody] UsuarioRequest request)
        {
            UsuarioResponse response = new UsuarioResponse();

            try
            {
                if (TokenController.Validar(token))
                {
                    if (UtilCpfCnpj.IsValid(request.Login))
                    {
                        var usuario = Consultar(request.Login);

                        if (usuario != null)
                        {
                            try
                            {
                                if (usuario.Senha == request.Senha)
                                {
                                    response.Id = usuario.Id;
                                    response.Status = "Sucesso";
                                }
                                else
                                {
                                    response.Status = "Insucesso";
                                }
                            }
                            catch (System.Exception ex)
                            {
                                response.Status = "Insucesso";
                            }
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

        public static PessoasUsuariosAplicativo Consultar(string Login)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.PessoasUsuariosAplicativo
                            where d.Login == Login
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static long Inserir(PessoasUsuariosAplicativo usuario)
        {
            using (var context = new UranusEntities())
            {
                context.PessoasUsuariosAplicativo.Add(usuario);
                context.SaveChanges();

                return usuario.Id;
            }
        }

        public static void Salvar(PessoasUsuariosAplicativo usuario)
        {
            using (var context = new UranusEntities())
            {
                context.Entry(usuario).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}
