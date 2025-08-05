using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Uranus.Common;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.App.Controllers
{
    public class ConsultaProcessosController : ApiController
    {
        #region ProcessosRequest
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

        public class Fields
        {
            public string token { get; set; }
            public string cnpjcpf { get; set; }
            //public string telefone { get; set; }
        }

        public class ProcessosRequest
        {
            public string text { get; set; }
            public Contact contact { get; set; }
            public Channel channel { get; set; }
            public int id { get; set; }
            public int clienteId { get; set; }
            public string origin { get; set; }
        }
        #endregion

        #region ProcessosResponse
        public class Callback
        {
            public string endpoint { get; set; }
            public Data data { get; set; }
        }

        public class Data
        {
            public string id { get; set; }
            public string token { get; set; }
            public string cpfcnpj { get; set; }
            //public string telefone { get; set; }
        }

        public class Item
        {
            public int number { get; set; }
            public string text { get; set; }
            public Callback callback { get; set; }
        }

        public class ProcessosResponse
        {
            public string type { get; set; }
            public string text { get; set; }
            public List<Item> items { get; set; }
        }
        #endregion

        // POST api/consultaareas
        public ProcessosResponse Post([FromBody] ProcessosRequest request)
        {
            ProcessosResponse response = new ProcessosResponse();

            try
            {
                var re = Request;
                var headers = re.Headers;
                string token = string.Empty;

                if (headers.Contains("mz-authorization"))
                {
                    token = headers.GetValues("mz-authorization").First();
                }

                if (TokenController.Validar(token))
                {
                    if (UtilCpfCnpj.IsValid(request.contact.fields.cnpjcpf))
                    {
                        var cliente = ConsultarCliente(Util.OnlyNumbers(request.contact.fields.cnpjcpf));

                        if (cliente != null)
                        {
                            var clienteTelefone = ConsultarTelefone(Util.OnlyNumbers(request.contact.fields.cnpjcpf), Util.OnlyNumbers(request.contact.key.Substring(2, 2) + "9" + request.contact.key.Substring(4, 8)));

                            if (clienteTelefone != null)
                            {
                                try
                                {
                                    var processos = Listar(cliente.ID);

                                    if (processos != null && processos.Count > 0)
                                    {
                                        response.type = "MENU";
                                        response.text = "Selecione o numero do processo:";
                                        response.items = new List<Item>();

                                        var i = 1;

                                        foreach (var processo in processos)
                                        {
                                            Item item = new Item();
                                            item.number = i;
                                            item.text = processo.NumeroProcesso;
                                            item.callback = new Callback();
                                            item.callback.endpoint = "https://uranusapi.rvmadvogados.com.br/api/consultaeventos";
                                            item.callback.data = new Data();
                                            item.callback.data.id = processo.ID.ToString();
                                            item.callback.data.token = request.contact.fields.token;
                                            item.callback.data.cpfcnpj = request.contact.fields.cnpjcpf;
                                            //item.callback.data.telefone = request.contact.fields.telefone;

                                            response.items.Add(item);
                                            i++;
                                        }

                                        //response.Processos = processosResponse;
                                        //response.Status = "Sucesso";
                                    }
                                    else
                                    {
                                        //response.Status = "Insucesso";
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    //response.Status = "Insucesso";
                                }
                            }
                            else
                            {
                                //response.Status = "Telefone não existe na base";
                            }
                        }
                        else
                        {
                            if (request.contact.fields.cnpjcpf.Length <= 14)
                            {
                                //response.Status = "Cpf não existe na base";
                            }
                            else
                            {
                                //response.Status = "Cnpj não existe na base";
                            }
                        }

                        if (string.IsNullOrEmpty(response.type))
                        {
                            response.type = "INFORMATION";
                            response.text = "Dados inconsistentes, estamos redirecionando para um atendente. Por favor, aguarde.";
                        }
                        return response;
                    }
                    else
                    {
                        //response.Status = "CPF ou CNPJ Inválido";

                        if (string.IsNullOrEmpty(response.type))
                        {
                            response.type = "INFORMATION";
                            response.text = "Dados inconsistentes, estamos redirecionando para um atendente. Por favor, aguarde.";
                        }

                        return response;
                    }
                }
                else
                {
                    //response.Status = "Token Inválido";

                    if (string.IsNullOrEmpty(response.type))
                    {
                        response.type = "INFORMATION";
                        response.text = "Dados inconsistentes, estamos redirecionando para um atendente. Por favor, aguarde.";
                    }

                    return response;
                }
            }
            catch (System.Exception ex)
            {
                //response.Status = "Falha";

                if (string.IsNullOrEmpty(response.type))
                {
                    response.type = "INFORMATION";
                    response.text = "Dados inconsistentes, estamos redirecionando para um atendente. Por favor, aguarde.";
                }
                return response;
            }
        }

        public static Clientes ConsultarCliente(string cpfcnpj)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Clientes.Include("Pessoas")
                            where d.Pessoas.CpfCnpj == cpfcnpj
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static Clientes ConsultarTelefone(string cpfcnpj, string telefone)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.Clientes
                            where d.Pessoas.CpfCnpj == cpfcnpj
                            where d.Pessoas.Fones.Where(x => (x.DDD + x.Numero) == telefone).Any()
                            select d;

                return query.FirstOrDefault();
            }
        }

        public static List<ProcessosAcoes> Listar(int idcliente)
        {
            using (var context = new UranusEntities())
            {
                var query = from d in context.ProcessosAcoes.Include("ProcessosAcoesEventos")
                            where d.Processos.ProcessosAutores.Where(x => x.IdCliente == idcliente).Any()
                            where d.ProcessosAcoesEventos.Where(w => w.ProcessosEventos.WhatsApp == true).Any()
                            select d;

                return query.ToList();
            }
        }
    }
}
