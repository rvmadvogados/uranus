using Newtonsoft.Json;
using Sigman.BB.BBEntidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Sigman.BB
{
    public class BBCobranca
    {
        private string urlAuth = string.Empty;
        private string urlApi = string.Empty;
        private string gwDevAppKey = string.Empty;
        private string client_id = string.Empty;
        private string client_secret = string.Empty;
        private string tokenBasic = string.Empty;
        private string grant_type = "client_credentials";
        private string scope = "cob.read cob.write pix.read pix.write cobrancas.boletos-requisicao cobrancas.boletos-info";
        private TokenBearer tokenBearer = new TokenBearer();

        public class TokenBearer
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }
        }

        public BBCobranca(string url, string gwDevAppKey, string client_id, string client_secret)
        {
            this.urlAuth = $"https://oauth.{url}/oauth/token";
            this.urlApi = $"https://api.{url}/cobrancas/v2/";
            this.gwDevAppKey = gwDevAppKey;
            this.client_id = client_id;
            this.client_secret = client_secret;
            this.tokenBasic = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{client_id}:{client_secret}"));
        }

        private bool Autenticar()
        {
            try
            {
                var result = Task.Run(() => ObterToken()).Result;
                if (result != null && !String.IsNullOrEmpty(result.AccessToken))
                {
                    return true;
                }
            }
            catch (Exception ex) {

                var error = ex;
            }

            return false;
        }

        public async Task<TokenBearer> ObterToken()
        {
            using (HttpClientHandler httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = CertificateValidationCallBack;
                using (HttpClient client = new HttpClient(httpClientHandler))
                {
                    client.DefaultRequestHeaders.Remove("Authorization");
                    client.DefaultRequestHeaders.Add("Authorization", tokenBasic);

                    var form = new Dictionary<string, string>
                    {
                        {"grant_type", grant_type},
                        {"scope", scope},
                    };

                    HttpResponseMessage tokenResponse = await client.PostAsync(urlAuth, new FormUrlEncodedContent(form));
                    var jsonContent = await tokenResponse.Content.ReadAsStringAsync();
                    tokenBearer = JsonConvert.DeserializeObject<TokenBearer>(jsonContent);
                    return tokenBearer;
                }
            }
        }

        private static bool CertificateValidationCallBack(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                if (chain != null && chain.ChainStatus != null)
                {
                    foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
                    {
                        if ((certificate.Subject == certificate.Issuer) &&
                           (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot))
                        {
                            // Self-signed certificates with an untrusted root are valid. 
                            continue;
                        }
                        else
                        {
                            if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
                            {
                                // If there are any other errors in the certificate chain, the certificate is invalid,
                                // so the method returns false.
                                return false;
                            }
                        }
                    }
                }

                // When processing reaches this line, the only errors in the certificate chain are 
                // untrusted root errors for self-signed certificates. These certificates are valid
                // for default Exchange server installations, so return true.
                return true;
            }


            /* overcome localhost and 127.0.0.1 issue */
            if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch) != 0)
            {
                if (certificate.Subject.Contains("localhost"))
                {
                    HttpRequestMessage castSender = sender as HttpRequestMessage;
                    if (null != castSender)
                    {
                        if (castSender.RequestUri.Host.Contains("127.0.0.1"))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;

        }

        public string GetQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

            return String.Join("&", properties.ToArray());
        }

        // Listar Boletos de Cobrança
        public ListaBoletosResponse ListarBoletos(ListaBoletosRequest lista)
        {
            try
            {
                if (Autenticar())
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(urlApi);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenBearer.AccessToken);

                        var parameters = $"boletos?gw-dev-app-key={this.gwDevAppKey}&{GetQueryString(lista)}";
                        var result = client.GetAsync(parameters).Result;

                        var resultContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                        if (result.StatusCode == HttpStatusCode.OK) // 200
                        {
                            ListaBoletosResponse boletos = JsonConvert.DeserializeObject<ListaBoletosResponse>(resultContent);
                            boletos.statusCode = (int)result.StatusCode;
                            return boletos;
                        }
                        else
                        {
                            ListaBoletosResponse boletos = new ListaBoletosResponse();
                            boletos.statusCode = (int)result.StatusCode;
                            boletos.mensagemErros = JsonConvert.DeserializeObject<ListaBoletosResponseErros>(resultContent);
                            return boletos;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Registrar Boleto de Cobrança
        public RegistraBoletoResponse RegistrarBoleto(RegistraBoletoRequest boleto)
        {
            try
            {
                if (Autenticar())
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(urlApi);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenBearer.AccessToken);

                        var json = JsonConvert.SerializeObject(boleto,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

                        var parameters = $"boletos?gw-dev-app-key={this.gwDevAppKey}";
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var result = client.PostAsync(parameters, content).Result;

                        var resultContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                        if (result.StatusCode == HttpStatusCode.OK || result.StatusCode == HttpStatusCode.Created) // 201
                        {
                            RegistraBoletoResponse boletos = JsonConvert.DeserializeObject<RegistraBoletoResponse>(resultContent);
                            boletos.statusCode = (int)result.StatusCode;
                            return boletos;
                        }
                        else
                        {
                            RegistraBoletoResponse boletos = new RegistraBoletoResponse();
                            boletos.statusCode = (int)result.StatusCode;
                            boletos.mensagemErros = JsonConvert.DeserializeObject<RegistraBoletoResponseErros>(resultContent);
                            return boletos;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Buscar Boleto de Cobrança
        public BuscaBoletoResponse BuscarBoleto(BuscaBoletoRequest boleto)
        {
            try
            {
                if (Autenticar())
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(urlApi);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenBearer.AccessToken);

                        var parameters = $"boletos/{boleto.id}?gw-dev-app-key={this.gwDevAppKey}&numeroConvenio={boleto.numeroConvenio}";
                        var result = client.GetAsync(parameters).Result;

                        var resultContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                        if (result.StatusCode == HttpStatusCode.OK) // 200
                        {
                            BuscaBoletoResponse boletos = JsonConvert.DeserializeObject<BuscaBoletoResponse>(resultContent);
                            boletos.statusCode = (int)result.StatusCode;
                            return boletos;
                        }
                        else
                        {
                            BuscaBoletoResponse boletos = new BuscaBoletoResponse();
                            boletos.statusCode = (int)result.StatusCode;
                            boletos.mensagemErros = JsonConvert.DeserializeObject<BuscaBoletoResponseErrors>(resultContent);
                            return boletos;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Aterar Boleto de Cobrança
        public AlteraBoletoResponse AlterarBoleto(string id, AlteraBoletoRequest boleto)
        {
            try
            {
                if (Autenticar())
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(urlApi);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenBearer.AccessToken);

                        var parameters = $"boletos/{1}?gw-dev-app-key={this.gwDevAppKey}";
                        var method = "PATCH";
                        var httpVerb = new HttpMethod(method);
                        var httpRequestMessage =
                            new HttpRequestMessage(httpVerb, parameters)
                            {
                                Content = new StringContent(JsonConvert.SerializeObject(boleto).ToString(), Encoding.UTF8, "application/json")
                            };

                        var result = client.SendAsync(httpRequestMessage);

                        var resultContent = result.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                        if (result.Result.StatusCode == HttpStatusCode.OK) // 200
                        {
                            AlteraBoletoResponse boletos = JsonConvert.DeserializeObject<AlteraBoletoResponse>(resultContent);
                            boletos.statusCode = (int)result.Result.StatusCode;
                            return boletos;
                        }
                        else
                        {
                            AlteraBoletoResponse boletos = new AlteraBoletoResponse();
                            boletos.statusCode = (int)result.Result.StatusCode;
                            boletos.mensagemErros = JsonConvert.DeserializeObject<AlteraBoletoResponseErrors>(resultContent);
                            return boletos;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Baixar ou Cancelar Boleto de Cobrança
        public BaixaCancelaBoletoResponse BaixarCancelarBoleto(string id, string tipo, BaixaCancelaBoletoRequest boleto)
        {
            try
            {
                if (Autenticar())
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(urlApi);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenBearer.AccessToken);

                        var parameters = $"boletos/{id}/{tipo}?gw-dev-app-key={this.gwDevAppKey}";
                        var content = new StringContent(JsonConvert.SerializeObject(boleto).ToString(), Encoding.UTF8, "application/json");
                        var result = client.PostAsync(parameters, content).Result;

                        var resultContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                        if (result.StatusCode == HttpStatusCode.OK) // 200
                        {
                            BaixaCancelaBoletoResponse boletos = JsonConvert.DeserializeObject<BaixaCancelaBoletoResponse>(resultContent);
                            boletos.statusCode = (int)result.StatusCode;
                            return boletos;
                        }
                        else
                        {
                            BaixaCancelaBoletoResponse boletos = new BaixaCancelaBoletoResponse();
                            boletos.statusCode = (int)result.StatusCode;
                            boletos.mensagemErros = JsonConvert.DeserializeObject<BaixaCancelaBoletoResponseErrors>(resultContent);
                            return boletos;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Cancelar Pix de Cobrança
        public CancelaPixResponse CancelarPix(string id, CancelaPixRequest boleto)
        {
            try
            {
                if (Autenticar())
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(urlApi);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenBearer.AccessToken);

                        var parameters = $"boletos/{id}/cancelar-pix?gw-dev-app-key={this.gwDevAppKey}";
                        var content = new StringContent(JsonConvert.SerializeObject(boleto).ToString(), Encoding.UTF8, "application/json");
                        var result = client.PostAsync(parameters, content).Result;

                        var resultContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                        if (result.StatusCode == HttpStatusCode.OK) // 200
                        {
                            CancelaPixResponse boletos = JsonConvert.DeserializeObject<CancelaPixResponse>(resultContent);
                            boletos.statusCode = (int)result.StatusCode;
                            return boletos;
                        }
                        else
                        {
                            CancelaPixResponse boletos = new CancelaPixResponse();
                            boletos.statusCode = (int)result.StatusCode;
                            boletos.mensagemErros = JsonConvert.DeserializeObject<CancelaPixResponseErros>(resultContent);
                            return boletos;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Gerar Pix de Cobrança
        public GeraPixResponse GerarPix(string id, GeraPixRequest boleto)
        {
            try
            {
                if (Autenticar())
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(urlApi);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenBearer.AccessToken);

                        var parameters = $"boletos/{id}/gerar-pix?gw-dev-app-key={this.gwDevAppKey}";
                        var content = new StringContent(JsonConvert.SerializeObject(boleto).ToString(), Encoding.UTF8, "application/json");
                        var result = client.PostAsync(parameters, content).Result;

                        var resultContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                        if (result.StatusCode == HttpStatusCode.OK) // 200
                        {
                            GeraPixResponse boletos = JsonConvert.DeserializeObject<GeraPixResponse>(resultContent);
                            boletos.statusCode = (int)result.StatusCode;
                            return boletos;
                        }
                        else
                        {
                            GeraPixResponse boletos = new GeraPixResponse();
                            boletos.statusCode = (int)result.StatusCode;
                            boletos.mensagemErros = JsonConvert.DeserializeObject<GeraPixResponseErros>(resultContent);
                            return boletos;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Consultar Pix de Cobrança
        public ConsultaPixResponse ConsultarPix(ConsultaPixRequest boleto)
        {
            try
            {
                if (Autenticar())
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(urlApi);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenBearer.AccessToken);

                        var parameters = $"boletos/{boleto.id}/pix?gw-dev-app-key={this.gwDevAppKey}&numeroConvenio={boleto.numeroConvenio}";
                        var result = client.GetAsync(parameters).Result;

                        var resultContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                        if (result.StatusCode == HttpStatusCode.OK) // 200
                        {
                            ConsultaPixResponse boletos = JsonConvert.DeserializeObject<ConsultaPixResponse>(resultContent);
                            boletos.statusCode = (int)result.StatusCode;
                            return boletos;
                        }
                        else
                        {
                            ConsultaPixResponse boletos = new ConsultaPixResponse();
                            boletos.statusCode = (int)result.StatusCode;
                            boletos.mensagemErros = JsonConvert.DeserializeObject<ConsultaPixResponseErros>(resultContent);
                            return boletos;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Listar Convênios
        public ListaConveniosResponse ListarConvenios(string id, ListaConveniosRequest convenio)
        {
            try
            {
                if (Autenticar())
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(urlApi);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenBearer.AccessToken);

                        var parameters = $"convenios/{id}/listar-retorno-movimento?gw-dev-app-key={this.gwDevAppKey}";
                        var content = new StringContent(JsonConvert.SerializeObject(convenio).ToString(), Encoding.UTF8, "application/json");
                        var result = client.PostAsync(parameters, content).Result;

                        var resultContent = result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                        if (result.StatusCode == HttpStatusCode.OK) // 200
                        {
                            ListaConveniosResponse boletos = JsonConvert.DeserializeObject<ListaConveniosResponse>(resultContent);
                            boletos.statusCode = (int)result.StatusCode;
                            return boletos;
                        }
                        else
                        {
                            ListaConveniosResponse boletos = new ListaConveniosResponse();
                            boletos.statusCode = (int)result.StatusCode;
                            boletos.mensagemErros = JsonConvert.DeserializeObject<ListaConveniosResponseErros>(resultContent);
                            return boletos;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}