using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Common;
using Uranus.Domain;
using Uranus.Domain.Entities;
using System.Security.Claims;
using Microsoft.Owin.Security;
using System.Web;
using System.Linq;
using Uranus.Suite.ViewModels;

namespace Uranus.Suite.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            if (Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("Index", "Mobile");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<JsonResult> Validar(String Usuario, String Senha)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string authApiUrl = ConfigurationManager.AppSettings["AuthApiUrl"] + "auth/";
                    client.BaseAddress = new Uri(authApiUrl);

                    var loginData = new { username = Usuario, password = Senha };
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(loginData);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("login", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var resultContent = await response.Content.ReadAsStringAsync();
                        dynamic authResult = Newtonsoft.Json.JsonConvert.DeserializeObject(resultContent);

                        // Verificar se precisa configurar 2FA
                        if (authResult.requiresSetup != null && authResult.requiresSetup == true)
                        {
                            // Salvar token temporário para configuração
                            Session["Setup2FA_Token"] = authResult.setupToken?.ToString();
                            Session["Setup2FA_Usuario"] = Usuario;

                            var resultSetup = new
                            {
                                response = "requiresSetup",
                                message = authResult.message?.ToString() ?? "Você precisa configurar a autenticação de dois fatores"
                            };
                            return Json(resultSetup);
                        }

                        if (authResult.requiresTwoFactor != null && authResult.requiresTwoFactor == true)
                        {
                            Session["2FA_Usuario"] = Usuario;
                            Session["2FA_Senha"] = Senha;
                            
                            // Verificar método preferido
                            var preferredMethod = authResult.preferredMethod?.ToString() ?? "App";
                            var hasAuthenticator = authResult.hasAuthenticator ?? false;
                            var hasEmail = authResult.hasEmail ?? false;
                            var maskedEmail = authResult.maskedEmail?.ToString();
                            
                            var result2FA = new
                            {
                                response = "requires2fa",
                                preferredMethod = preferredMethod,
                                hasAuthenticator = hasAuthenticator,
                                hasEmail = hasEmail,
                                maskedEmail = maskedEmail,
                                message = authResult.message?.ToString() ?? "Por favor, forneça o código de autenticação de dois fatores"
                            };
                            return Json(result2FA);
                        }

                        var roles = authResult.usuario.roles;
                        var claims = authResult.usuario.claims;

                        Sessao.Aplicativo = ConfigurationManager.AppSettings["ApplicationName"];
                        Sessao.Usuario = UsuariosBo.BuscarUsuarioPorLogin(Usuario);
                        Sessao.Token = authResult.token.ToString();

                        Sessao.Setting = SettingsBO.Consultar();
                        Sessao.ProcessRowIndex = String.Empty;
                        Sessao.ProcessNumber = String.Empty;
                        Sessao.ClientName = String.Empty;
                        Sessao.AreaType = String.Empty;
                        Sessao.ProcessStatus = String.Empty;
                        Sessao.Judgment = String.Empty;
                        Sessao.FeriadosRecesso = FeriadosBo.Buscar();

                        Connected conectado = new Connected();
                        conectado.IP = Util.GetLocalIPAddress();
                        conectado.SistemaOperacional = Util.GetOSVersion();
                        conectado.Navegador = Util.GetWebBrowserName();
                        Sessao.Conectado = conectado;

                        DashboardController.ConnectedUsers();

                        
                        Sessao.Usuario.Nivel = 5;

                        var identity = new ClaimsIdentity("ApplicationCookie");
                        identity.AddClaim(new Claim(ClaimTypes.Name, Usuario));
                        foreach (var role in roles)
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, role.ToString()));
                        }

                        foreach (var claim in claims)
                        {
                            identity.AddClaim(new Claim(claim.type.ToString(), claim.value.ToString()));
                        }

                        Sessao.Claims = identity.Claims.Where(a=>a.Type =="sistema").
                            Select(a => new UsuarioClaimDTO { Tipo = a.Type, Valor = a.Value }).ToList();

                        var authManager = HttpContext.GetOwinContext().Authentication;
                        authManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);

                        var result = new { response = "success", aplicativo = Sessao.Aplicativo, nivel = 0 };
                        return Json(result);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        dynamic errorResult = null;
                        
                        try
                        {
                            errorResult = Newtonsoft.Json.JsonConvert.DeserializeObject(errorContent);
                            
                            // DEBUG - Log para verificar o que está vindo da API
                            System.Diagnostics.Debug.WriteLine($"[LOGIN ERROR] Response: {errorContent}");
                            System.Diagnostics.Debug.WriteLine($"[LOGIN ERROR] isLocked: {errorResult?.isLocked}");
                            System.Diagnostics.Debug.WriteLine($"[LOGIN ERROR] minutesRemaining: {errorResult?.minutesRemaining}");
                            System.Diagnostics.Debug.WriteLine($"[LOGIN ERROR] attemptsRemaining: {errorResult?.attemptsRemaining}");
                        }
                        catch (Exception debugEx)
                        {
                            System.Diagnostics.Debug.WriteLine($"[LOGIN ERROR - DESERIALIZATION FAILED] {debugEx.Message}");
                        }

                        // Verificar se é erro de bloqueio
                        if (errorResult?.isLocked == true)
                        {
                            var minutesRemaining = errorResult.minutesRemaining ?? 5;
                            var message = $"⏱️ Sua conta está temporariamente bloqueada. Tente novamente em {minutesRemaining} minuto(s).";
 
                            System.Diagnostics.Debug.WriteLine($"[LOGIN BLOCKED] Mensagem: {message}");
                
                            var result = new 
                            { 
                                response = "error",
                                locked = true,
                                minutesRemaining = minutesRemaining,
                                message = message
                            };
                            return Json(result);
                        }

                        // Verificar se há informações sobre tentativas restantes
                        int? attemptsRemaining = null;
                        if (errorResult?.attemptsRemaining != null)
                        {
                            attemptsRemaining = (int)errorResult.attemptsRemaining;
                        }

                        var errorMessage = errorResult?.message?.ToString() ?? "Usuário ou senha inválidos";

                        // Construir mensagem com informação de tentativas restantes
                        if (attemptsRemaining.HasValue && attemptsRemaining > 0)
                        {
                            errorMessage = $"{errorMessage} | ⚠️ Tentativas restantes: {attemptsRemaining}";
                            System.Diagnostics.Debug.WriteLine($"[LOGIN FAILED] Tentativas restantes: {attemptsRemaining}");
                        }
                        
                        System.Diagnostics.Debug.WriteLine($"[LOGIN FAILED] Mensagem: {errorMessage}");
                        System.Diagnostics.Debug.WriteLine($"[LOGIN FAILED] attemptsRemaining value: {attemptsRemaining}");
     
                        var resultError = new 
                        { 
                            response = "error", 
                            message = errorMessage,
                            attemptsRemaining = attemptsRemaining ?? 0
                        };
                        return Json(resultError);
                    }
                }
            }
            catch (Exception ex)
            {
                var result = new { response = "error", message = "Erro ap processar requisição. Ex: " + ex.Message  + ex.InnerException};
                return Json(result);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<JsonResult> ValidarCom2FA(String Usuario, String Senha, String Codigo2FA)
        {
            using (var client = new HttpClient())
            {
                string authApiUrl = ConfigurationManager.AppSettings["AuthApiUrl"] + "auth/";
                client.BaseAddress = new Uri(authApiUrl);

                var loginData = new
                {
                    username = Usuario,
                    password = Senha,
                    twoFactorCode = Codigo2FA
                };

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(loginData);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync("login-2fa", content);

                if (response.IsSuccessStatusCode)
                {
                    var resultContent = await response.Content.ReadAsStringAsync();
                    dynamic authResult = Newtonsoft.Json.JsonConvert.DeserializeObject(resultContent);

                    var roles = authResult.usuario.roles;
                    var claims = authResult.usuario.claims;

                    Sessao.Aplicativo = ConfigurationManager.AppSettings["ApplicationName"];
                    var usuarioDb = UsuariosBo.BuscarUsuarioPorLogin(Usuario);
                    Sessao.Usuario = usuarioDb;
                    Sessao.Token = authResult.token.ToString();

                    Sessao.Setting = SettingsBO.Consultar();
                    Sessao.ProcessRowIndex = String.Empty;
                    Sessao.ProcessNumber = String.Empty;
                    Sessao.ClientName = String.Empty;
                    Sessao.AreaType = String.Empty;
                    Sessao.ProcessStatus = String.Empty;
                    Sessao.Judgment = String.Empty;
                    Sessao.FeriadosRecesso = FeriadosBo.Buscar();

                    Connected conectado = new Connected();
                    conectado.IP = Util.GetLocalIPAddress();
                    conectado.SistemaOperacional = Util.GetOSVersion();
                    conectado.Navegador = Util.GetWebBrowserName();
                    Sessao.Conectado = conectado;

                    DashboardController.ConnectedUsers();

                    var nivel = 5;

                    var identity = new ClaimsIdentity("ApplicationCookie");
                    identity.AddClaim(new Claim(ClaimTypes.Name, Usuario));
                    foreach (var role in roles)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, role.ToString()));
                    }

                    foreach (var claim in claims)
                    {
                        identity.AddClaim(new Claim(claim.type.ToString(), claim.value.ToString()));
                    }

                    Sessao.Claims = identity.Claims.Where(a => a.Type == "sistema").
                      Select(a => new UsuarioClaimDTO { Tipo = a.Type, Valor = a.Value }).ToList();

                    var authManager = HttpContext.GetOwinContext().Authentication;
                    authManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);

                    var result = new { response = "success", aplicativo = Sessao.Aplicativo, nivel = nivel };
                    return Json(result);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    dynamic errorResult = null;
                    
                    try
                    {
                        errorResult = Newtonsoft.Json.JsonConvert.DeserializeObject(errorContent);
                    }
                    catch
                    {
                        // Ignorar erro de deserialização
                    }

                    // Verificar se é erro de bloqueio
                    if (errorResult?.isLocked == true)
                    {
                        var minutesRemaining = errorResult.minutesRemaining ?? 5;
                        var message = $"⏱️ Sua conta está temporariamente bloqueada. Tente novamente em {minutesRemaining} minuto(s).";
                        
                        var result = new 
                        { 
                            response = "error",
                            locked = true,
                            minutesRemaining = minutesRemaining,
                            message = message
                        };
                        return Json(result);
                    }

                    // Verificar se há informações sobre tentativas restantes
                    int? attemptsRemaining = null;
                    if (errorResult?.attemptsRemaining != null)
                    {
                        attemptsRemaining = (int)errorResult.attemptsRemaining;
                    }

                    var errorMessage = errorResult?.message?.ToString() ?? "Código de autenticação inválido";

                    // Construir mensagem com informação de tentativas restantes
                    if (attemptsRemaining.HasValue && attemptsRemaining > 0)
                    {
                        errorMessage = $"{errorMessage} | ⚠️ Tentativas restantes: {attemptsRemaining}";
                    }

                    var resultError = new 
                    { 
                        response = "error", 
                        message = errorMessage,
                        attemptsRemaining = attemptsRemaining
                    };
                    return Json(resultError);
                }
            }
        }

        [HttpPost]
        public async Task<JsonResult> SolicitarCodigoEmail(String Usuario)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string authApiUrl = ConfigurationManager.AppSettings["AuthApiUrl"];
                    client.BaseAddress = new Uri(authApiUrl);

                    var requestData = new { username = Usuario };
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    
                    // Endpoint correto: api/twofactor/send-email-code
                    var response = await client.PostAsync("twofactor/send-email-code", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var resultContent = await response.Content.ReadAsStringAsync();
                        dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(resultContent);
                        
                        return Json(new 
                        { 
                            response = "success", 
                            message = result.message?.ToString(),
                            email = result.email?.ToString()
                        });
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        dynamic errorResult = null;
                        
                        try
                        {
                            errorResult = Newtonsoft.Json.JsonConvert.DeserializeObject(errorContent);
                        }
                        catch { }

                        var errorMessage = errorResult?.message?.ToString() ?? "Erro ao enviar código por email";
                        return Json(new { response = "error", message = errorMessage });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { response = "error", message = "Erro ao processar requisição: " + ex.Message });
            }
        }

        //[HttpPost]
        //public JsonResult ValidarOld(String Usuario, String Senha)
        //{
        //    var usuario = UsuariosBo.Validar(Usuario, Util.GerarHashMd5(Senha));

        //    if (usuario != null)
        //    {
        //        var nivel = usuario.Nivel;
        //        Sessao.Aplicativo = ConfigurationManager.AppSettings["ApplicationName"];
        //        Sessao.Usuario = usuario;
        //        Sessao.Setting = SettingsBO.Consultar();

        //        Sessao.ProcessRowIndex = String.Empty;
        //        Sessao.ProcessNumber = String.Empty;
        //        Sessao.ClientName = String.Empty;
        //        Sessao.AreaType = String.Empty;
        //        Sessao.ProcessStatus = String.Empty;
        //        Sessao.Judgment = String.Empty;

        //        Sessao.FeriadosRecesso = FeriadosBo.Buscar();

        //        Connected conectado = new Connected();
        //        conectado.IP = Util.GetLocalIPAddress();
        //        conectado.SistemaOperacional = Util.GetOSVersion();
        //        conectado.Navegador = Util.GetWebBrowserName();

        //        Sessao.Conectado = conectado;

        //        DashboardController.ConnectedUsers();

        //        var result = new { response = "success", aplicativo = Sessao.Aplicativo, nivel = nivel };
        //        return Json(result);
        //    }
        //    else
        //    {
        //        var result = new { response = "error" };
        //        return Json(result);
        //    }
        //}

        public ActionResult DestroySession()
        {
            Sessao.Usuario = null;

            return RedirectToAction("Index", "Login");
        }

        public ActionResult Permission()
        {
            // Tentar obter informações do usuário da sessão
            if (Sessao.Usuario != null)
            {
                ViewBag.UserName = Sessao.Usuario.Nome;
            }
            
            // Tentar obter o role do usuário autenticado (via Claims)
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null && identity.IsAuthenticated)
            {
                var roleClaim = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                if (roleClaim != null)
                {
                    ViewBag.UserRole = roleClaim.Value;
                }
                
                // Se não tem nome do usuário na sessão, tenta pegar do claim
                if (ViewBag.UserName == null)
                {
                    var nameClaim = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                    if (nameClaim != null)
                    {
                        ViewBag.UserName = nameClaim.Value;
                    }
                }
            }
            
            return View();
        }
    }
}