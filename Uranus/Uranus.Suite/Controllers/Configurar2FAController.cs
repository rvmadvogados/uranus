using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Uranus.Common;

namespace Uranus.Suite.Controllers
{
    public class Configurar2FAController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Garantir UTF-8 em todas as respostas
            Response.ContentEncoding = Encoding.UTF8;
            Response.HeaderEncoding = Encoding.UTF8;
            base.OnActionExecuting(filterContext);
        }
        
        public ActionResult Index()
        {
            // Garantir UTF-8 na view
            Response.ContentType = "text/html; charset=utf-8";
            
            // Verificar se tem token de setup
            var token = Session["Setup2FA_Token"]?.ToString();
            var usuario = Session["Setup2FA_Usuario"]?.ToString();
            
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(usuario))
            {
                return RedirectToAction("Index", "Login");
            }
            
            ViewBag.Token = token;
            ViewBag.Usuario = usuario;
            ViewBag.ApplicationName = ConfigurationManager.AppSettings["ApplicationName"];
            
            return View();
        }
        
        [HttpPost]
        public async Task<JsonResult> GerarQRCode(string usuario)
        {
            var token = Session["Setup2FA_Token"]?.ToString();
            
            if (string.IsNullOrEmpty(token))
            {
                return Json(new { response = "error", message = "Sessão expirada" });
            }
            
            try
            {
                using (var client = new HttpClient())
                {
                    var apiUrl = ConfigurationManager.AppSettings["AuthApiUrl"];
                    client.BaseAddress = new Uri(apiUrl);
                    client.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", token);
                    
                    var request = new { username = usuario };
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    
                    var response = await client.PostAsync("twofactor/enable", content);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var resultContent = await response.Content.ReadAsStringAsync();
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(resultContent);
                        
                        return Json(new { 
                            response = "success", 
                            qrCodeUri = result.qrCodeUri?.ToString(),
                            manualEntryKey = result.manualEntryKey?.ToString(),
                            message = result.message?.ToString()
                        });
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return Json(new { response = "error", message = errorContent });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { response = "error", message = ex.Message });
            }
        }
        
        [HttpPost]
        public async Task<JsonResult> VerificarCodigo(string usuario, string codigo)
        {
            var token = Session["Setup2FA_Token"]?.ToString();
            
            if (string.IsNullOrEmpty(token))
            {
                Response.StatusCode = 401;
                return Json(new { response = "error", message = "Sessão expirada. Por favor, faça login novamente." });
            }
            
            try
            {
                using (var client = new HttpClient())
                {
                    var apiUrl = ConfigurationManager.AppSettings["AuthApiUrl"];
                    client.BaseAddress = new Uri(apiUrl);
                    client.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", token);
                    
                    var request = new { username = usuario, code = codigo };
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    
                    var response = await client.PostAsync("twofactor/verify", content);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var resultContent = await response.Content.ReadAsStringAsync();
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(resultContent);
                        
                        if (data.recoveryCodes == null || data.recoveryCodes.Count == 0)
                        {
                            return Json(new { 
                                response = "error", 
                                message = "Erro ao gerar códigos de recuperação. Por favor, tente novamente." 
                            });
                        }
                        
                        // Converter para array de strings
                        var recoveryCodesArray = new System.Collections.Generic.List<string>();
                        foreach (var code in data.recoveryCodes)
                        {
                            if (code != null && !string.IsNullOrWhiteSpace(code.ToString()))
                            {
                                recoveryCodesArray.Add(code.ToString());
                            }
                        }
                        
                        if (recoveryCodesArray.Count == 0)
                        {
                            return Json(new { 
                                response = "error", 
                                message = "Nenhum código de recuperação foi gerado. Por favor, tente novamente." 
                            });
                        }
                        
                        System.Diagnostics.Debug.WriteLine($"Códigos de recuperação gerados: {recoveryCodesArray.Count}");
                        
                        Sessao.Token = token;
                        Sessao.Usuario = Business.UsuariosBo.BuscarUsuarioPorLogin(usuario);
                        
                        Session.Remove("Setup2FA_Token");
                        Session.Remove("Setup2FA_Usuario");
                        
                        return Json(new { 
                            response = "success",
                            recoveryCodes = recoveryCodesArray.ToArray(),
                            message = data.message?.ToString()
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        var errorData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(errorContent);
                        
                        Response.StatusCode = (int)response.StatusCode;
                        return Json(new { 
                            response = "error", 
                            message = errorData?.message?.ToString() ?? "Código inválido" 
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao verificar código: {ex.Message}");
                Response.StatusCode = 500;
                return Json(new { response = "error", message = $"Erro interno: {ex.Message}" });
            }
        }

        // Método de teste para verificar se o controller está acessível
        [HttpPost]
        public JsonResult TesteRota()
        {
            return Json(new { response = "success", message = "Controller funcionando" });
        }

        [HttpPost]
        public async Task<JsonResult> PularConfiguracao(string usuario)
        {
            System.Diagnostics.Debug.WriteLine($"[PULAR CONFIG] Método chamado. Usuario: {usuario}");
            
            var token = Session["Setup2FA_Token"]?.ToString();
            System.Diagnostics.Debug.WriteLine($"[PULAR CONFIG] Token na sessão: {(string.IsNullOrEmpty(token) ? "AUSENTE" : "PRESENTE")}");
            
            if (string.IsNullOrEmpty(token))
            {
                System.Diagnostics.Debug.WriteLine($"[PULAR CONFIG] Sessão expirada");
                Response.StatusCode = 401;
                return Json(new { response = "error", message = "Sessão expirada. Por favor, faça login novamente." });
            }
            
            System.Diagnostics.Debug.WriteLine($"[PULAR CONFIG] Iniciando chamada à API");
            
            try
            {
                using (var client = new HttpClient())
                {
                    var apiUrl = ConfigurationManager.AppSettings["AuthApiUrl"];
                    System.Diagnostics.Debug.WriteLine($"[PULAR CONFIG] API URL: {apiUrl}");
                    
                    client.BaseAddress = new Uri(apiUrl);
                    client.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", token);
                    
                    var request = new { username = usuario };
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(request);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    
                    System.Diagnostics.Debug.WriteLine($"[PULAR CONFIG] Chamando endpoint: {client.BaseAddress}twofactor/skip-authenticator-setup");
                    
                    var response = await client.PostAsync("twofactor/skip-authenticator-setup", content);
                    
                    System.Diagnostics.Debug.WriteLine($"[PULAR CONFIG] Status da resposta: {response.StatusCode}");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var resultContent = await response.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.WriteLine($"[PULAR CONFIG] Resposta da API: {resultContent}");
                        
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(resultContent);
                        
                        var newToken = data.token?.ToString();
                        
                        if (!string.IsNullOrEmpty(newToken))
                        {
                            System.Diagnostics.Debug.WriteLine($"[PULAR CONFIG] Atualizando token na sessão");
                            // Atualizar token na sessão com o novo token retornado pela API
                            Sessao.Token = newToken;
                            Sessao.Usuario = Business.UsuariosBo.BuscarUsuarioPorLogin(usuario);
                        }
                        
                        // Limpar sessão temporária
                        Session.Remove("Setup2FA_Token");
                        Session.Remove("Setup2FA_Usuario");
                        
                        System.Diagnostics.Debug.WriteLine($"[PULAR CONFIG] Sucesso!");
                        
                        return Json(new { 
                            response = "success",
                            message = data.message?.ToString() ?? "Sucesso. Usando autenticação por email."
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.WriteLine($"[PULAR CONFIG] Erro da API: {errorContent}");
                        
                        var errorData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(errorContent);
                        
                        Response.StatusCode = (int)response.StatusCode;
                        return Json(new { 
                            response = "error", 
                            message = errorData?.message?.ToString() ?? "Erro ao processar solicitação" 
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PULAR CONFIG] Exceção: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[PULAR CONFIG] Stack trace: {ex.StackTrace}");
                Response.StatusCode = 500;
                return Json(new { response = "error", message = $"Erro interno: {ex.Message}" });
            }
        }
    }
}
