using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Common;
using Uranus.Domain;
using Uranus.Suite.Helpers;
using Uranus.Suite.Filters;

namespace Uranus.Suite.Controllers
{
    public class UsuarioViewModel
    {
        public int ID { get; set; }
        public string Nome { get; set; }
        public string Login { get; set; }
        public bool Bloqueio { get; set; }
        public int? Nivel { get; set; }
        public string PerfilDeAcesso { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool HasAuthenticator { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public int AccessFailedCount { get; set; }
        public bool EstaBloqueado { get; set; }
        public DateTimeOffset? DataFimBloqueio { get; set; } // Alterado para DateTimeOffset
        public bool LockoutExpirado { get; set; }
    }

    public class UsuariosController : Controller
    {
        [RequireSubmenu("Controladoria:Usuarios")]
        public async Task<ActionResult> Index(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var usuarios = UsuariosBo.Listar(search).ToList();
                var usuariosViewModel = new List<UsuarioViewModel>();

                foreach (var usuario in usuarios)
                {
                    string perfilDeAcesso = await ObterPerfilUsuario(usuario.Login);
                    var (twoFactorEnabled, hasAuthenticator) = await ObterStatus2FA(usuario.Login);
                    var (email, emailConfirmed, lastLogin, accessFailedCount, lockoutEnd) = await ObterInfoUsuario(usuario.Login);

                    bool estaBloqueado = false;
                    bool lockoutExpirou = false;

                    if (lockoutEnd.HasValue)
                    {
                        estaBloqueado = lockoutEnd.Value > DateTimeOffset.UtcNow;
                        lockoutExpirou = !estaBloqueado;
                        System.Diagnostics.Debug.WriteLine($"[Index] {usuario.Login}: lockoutEnd={lockoutEnd}, estaBloqueado={estaBloqueado}, lockoutExpirou={lockoutExpirou}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[Index] {usuario.Login}: lockoutEnd NULL");
                    }

                    usuariosViewModel.Add(new UsuarioViewModel
                    {
                        ID = usuario.ID,
                        Nome = usuario.Nome,
                        Login = usuario.Login,
                        Bloqueio = usuario.Bloqueio,
                        Nivel = usuario.Nivel,
                        PerfilDeAcesso = perfilDeAcesso ?? "Não definido",
                        TwoFactorEnabled = twoFactorEnabled,
                        HasAuthenticator = hasAuthenticator,
                        Email = email,
                        EmailConfirmed = emailConfirmed,
                        LastLoginDate = lastLogin,
                        AccessFailedCount = accessFailedCount,
                        EstaBloqueado = estaBloqueado,
                        DataFimBloqueio = lockoutEnd,
                        LockoutExpirado = lockoutExpirou
                    });
                }

                ViewBag.search = search;
                ViewBag.AuthApiUrl = ConfigurationManager.AppSettings["AuthApiUrl"];
                ViewBag.Token = Sessao.Token;
                return View(usuariosViewModel);
            }
        }

        private async Task<string> ObterPerfilUsuario(string login)
        {
            try
            {
                var apiUrl = ConfigurationManager.AppSettings["AuthApiUrl"];
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    if (!string.IsNullOrEmpty(Sessao.Token))
                    {
                        client.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Sessao.Token);
                    }

                    var response = await client.GetAsync($"roles/user/{login}");
                    if (response.IsSuccessStatusCode)
                    {
                        var perfil = await response.Content.ReadAsStringAsync();
                        return perfil?.Replace("\"", "");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }


        private async Task<(bool twoFactorEnabled, bool hasAuthenticator)> ObterStatus2FA(string login)
        {
            try
            {
                var apiUrl = ConfigurationManager.AppSettings["AuthApiUrl"];
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    if (!string.IsNullOrEmpty(Sessao.Token))
                    {
                        client.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Sessao.Token);
                    }

                    var response = await client.GetAsync($"twofactor/status/{login}");
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        dynamic status = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                        return ((bool)status.twoFactorEnabled, (bool)status.hasAuthenticator);
                    }
                }
            }
            catch { }
            return (false, false);
        }

        private async Task<(string email, bool emailConfirmed, DateTime? lastLogin, int accessFailedCount, DateTimeOffset? lockoutEnd)> ObterInfoUsuario(string login)
        {
            try
            {
                var apiUrl = ConfigurationManager.AppSettings["AuthApiUrl"];
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    if (!string.IsNullOrEmpty(Sessao.Token))
                    {
                        client.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Sessao.Token);
                    }

                    var response = await client.GetAsync($"users/{login}");
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.WriteLine($"[ObterInfoUsuario] JSON retornado da API para {login}: {json}");

                        dynamic userInfo = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                        var email = userInfo.email?.ToString() ?? "";
                        var emailConfirmed = (bool)(userInfo.emailConfirmed ?? false);

                        DateTime? lastLogin = null;
                        if (userInfo.lastLoginDate != null)
                        {
                            DateTime.TryParse(userInfo.lastLoginDate.ToString(), out DateTime parsedDate);
                            lastLogin = parsedDate;
                        }

                        var accessFailedCount = (int)(userInfo.accessFailedCount ?? 0);

                        // ?? NOVO: Extrair lockoutEnd com logging
                        DateTimeOffset? lockoutEnd = null;
                        if (userInfo.lockoutEnd != null)
                        {
                            string lockoutEndStr = userInfo.lockoutEnd.ToString();
                            System.Diagnostics.Debug.WriteLine($"[ObterInfoUsuario] lockoutEnd bruto para {login}: {lockoutEndStr}");

                            if (!string.IsNullOrEmpty(lockoutEndStr) && DateTimeOffset.TryParse(lockoutEndStr, out DateTimeOffset parsedLockout))
                            {
                                lockoutEnd = parsedLockout;
                                System.Diagnostics.Debug.WriteLine($"[ObterInfoUsuario] lockoutEnd parseado para {login}: {lockoutEnd}");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"[ObterInfoUsuario] FALHA ao parsear lockoutEnd para {login}. String: {lockoutEndStr}");
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"[ObterInfoUsuario] lockoutEnd é NULL para {login}");
                        }

                        System.Diagnostics.Debug.WriteLine($"[ObterInfoUsuario] Dados finais para {login}: email={email}, accessFailedCount={accessFailedCount}, lockoutEnd={lockoutEnd}");

                        return (email, emailConfirmed, lastLogin, accessFailedCount, lockoutEnd);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[ObterInfoUsuario] Erro ao chamar API para {login}. Status: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ObterInfoUsuario] EXCEPTION para {login}: {ex.Message}");
            }
            return ("", false, null, 0, null);
        }

        [HttpPost]
        //[RequireSubmenu("Controladoria:Usuarios")]
        public async Task<JsonResult> Consultar(Int64 Id)
        {
            var usuarios = UsuariosBo.ConsultarArray(Id);
            string perfilDeAcesso = null;

            if (usuarios.Length > 0)
            {
                var usuario = usuarios.GetValue(0);
                var loginProp = usuario.GetType().GetProperty("Login");
                var login = loginProp?.GetValue(usuario)?.ToString();

                try
                {
                    var apiUrl = ConfigurationManager.AppSettings["AuthApiUrl"];
                    using (var client = new System.Net.Http.HttpClient())
                    {
                        client.BaseAddress = new Uri(apiUrl);
                        if (!string.IsNullOrEmpty(Sessao.Token))
                        {
                            client.DefaultRequestHeaders.Authorization =
                                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Sessao.Token);
                        }

                        var response = await client.GetAsync($"roles/user/{login}");
                        if (response.IsSuccessStatusCode)
                        {
                            perfilDeAcesso = await response.Content.ReadAsStringAsync();
                        }
                    }
                }
                catch
                {
                    perfilDeAcesso = null;
                }
            }
            var result = new { codigo = "00", usuario = usuarios, perfilDeAcesso = perfilDeAcesso };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[RequireSubmenu("Controladoria:Usuarios")]
        public async Task<JsonResult> Salvar(Int32 Id, String Nome, String Usuario, String Senha, String Ativo, string PerfilDeAcesso, string Email = null)
        {
            Usuarios usuario = new Usuarios();
            usuario.ID = Id;
            usuario.Nome = Nome.Trim();
            usuario.Login = Usuario.Trim();

            if (!String.IsNullOrEmpty(Senha))
            {
                usuario.Senha = Util.GerarHashMd5(Senha.Trim());
            }

            usuario.Bloqueio = (Ativo != "S");

            bool identityCreated = true;
            string identityError = null;
            bool emailSaved = true;
            string emailError = null;

            // Normalizar e-mail: string vazia ou somente espa�os = null
            string emailFinal = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim().ToLower();

            // Validar unicidade do e-mail se informado
            if (!string.IsNullOrEmpty(emailFinal))
            {
                try
                {
                    var apiUrl = ConfigurationManager.AppSettings["AuthApiUrl"];
                    using (var client = new System.Net.Http.HttpClient())
                    {
                        client.BaseAddress = new Uri(apiUrl);
                        if (!string.IsNullOrEmpty(Sessao.Token))
                        {
                            client.DefaultRequestHeaders.Authorization =
                                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Sessao.Token);
                        }

                        // Verificar se j� existe outro usu�rio com esse e-mail
                        var response = await client.GetAsync($"users/check-email?email={Uri.EscapeDataString(emailFinal)}&excludeUsername={Uri.EscapeDataString(usuario.Login)}");
                        if (response.IsSuccessStatusCode)
                        {
                            var json = await response.Content.ReadAsStringAsync();
                            dynamic checkResult = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                            if (checkResult.exists == true)
                            {
                                var resultDuplicate = new { codigo = "95", mensagem = $"O e-mail '{emailFinal}' já está sendo usado por outro usuário." };
                                return Json(resultDuplicate);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log mas continue - valida��o � secund�ria
                    System.Diagnostics.Debug.WriteLine($"Erro ao validar unicidade de e-mail: {ex.Message}");
                }
            }

            if (Id == 0)
            {
                Id = UsuariosBo.Inserir(usuario);
                try
                {
                    var apiUrl = ConfigurationManager.AppSettings["AuthApiUrl"];
                    using (var client = new System.Net.Http.HttpClient())
                    {
                        client.BaseAddress = new Uri(apiUrl);
                        if (!string.IsNullOrEmpty(Sessao.Token))
                        {
                            client.DefaultRequestHeaders.Authorization =
                                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Sessao.Token);
                        }
                        var createUser = new
                        {
                            UserName = usuario.Login,
                            Password = Senha,
                            Role = PerfilDeAcesso,
                            Email = emailFinal // Pode ser null
                        };
                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(createUser);
                        var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
                        var response = await client.PostAsync("users/create", content);
                        if (!response.IsSuccessStatusCode)
                        {
                            identityCreated = false;
                            identityError = await response.Content.ReadAsStringAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    identityCreated = false;
                    identityError = ex.Message;
                }
            }
            else
            {
                try
                {
                    var apiUrl = ConfigurationManager.AppSettings["AuthApiUrl"];
                    using (var client = new System.Net.Http.HttpClient())
                    {
                        client.BaseAddress = new Uri(apiUrl);
                        if (!string.IsNullOrEmpty(Sessao.Token))
                        {
                            client.DefaultRequestHeaders.Authorization =
                                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Sessao.Token);
                        }

                        var updateEmailData = new
                        {
                            UserName = usuario.Login,
                            Email = emailFinal ?? "", // Se null, enviar string vazia para apagar
                            Password = !string.IsNullOrWhiteSpace(Senha) ? Senha.Trim() : "",
                            Role = !string.IsNullOrWhiteSpace(PerfilDeAcesso) ? PerfilDeAcesso.Trim() : ""
                        };

                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(updateEmailData);
                        var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");

                        var response = await client.PutAsync("users/update", content);

                        if (!response.IsSuccessStatusCode)
                        {
                            emailSaved = false;
                            var errorContent = await response.Content.ReadAsStringAsync();
                            emailError = $"Status: {response.StatusCode}, Erro: {errorContent}";
                        }
                    }
                }
                catch (Exception ex)
                {
                    emailSaved = false;
                    emailError = ex.Message;
                }
            }

            usuario.ID = Id;
            UsuariosBo.Salvar(usuario);

            if (!identityCreated)
            {
                var resultError = new { codigo = "97", mensagem = "Erro ao criar usuário na API: " + identityError };
                return Json(resultError);
            }

            if (!emailSaved)
            {
                var resultWarning = new { codigo = "96", mensagem = "Usuário salvo, mas erro ao salvar e-mail: " + emailError };
                return Json(resultWarning);
            }

            var result = new { codigo = "00", mensagem = emailFinal == null ? "Usuário salvo sem e-mail!" : "Usuário salvo com sucesso!" };
            return Json(result);
        }

        private void AtualizarPerfisIdentity(string login, string perfil, string senha)
        {
            var token = Sessao.Token;

            Task.Run(async () =>
            {
                try
                {
                    var apiUrl = AppSettingsHelper.AuthApiUrl;
                    using (var client = new System.Net.Http.HttpClient())
                    {
                        client.BaseAddress = new Uri(apiUrl);
                        if (!string.IsNullOrEmpty(token))
                        {
                            client.DefaultRequestHeaders.Authorization =
                                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        }

                        var updateUser = new
                        {
                            UserName = login,
                            Password = !string.IsNullOrEmpty(senha) ? senha : "",
                            Role = perfil
                        };

                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(updateUser);
                        var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");

                        var response = await client.PutAsync("users/update", content);

                        if (!response.IsSuccessStatusCode)
                        {

                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
        }

        [HttpPost]
        //[RequireSubmenu("Controladoria:Usuarios")]
        public async Task<JsonResult> Excluir()
        {
            // Tenta obter o Id do corpo da requisi��o (FormData ou JSON)
            int id = 0;
            if (Request.Form["Id"] != null)
            {
                int.TryParse(Request.Form["Id"], out id);
            }
            else if (Request.Params["Id"] != null)
            {
                int.TryParse(Request.Params["Id"], out id);
            }
            // Se n�o encontrou, retorna erro
            if (id == 0)
            {
                var resultError = new { codigo = "99", mensagem = "Id do usuário não informado." };
                return Json(resultError, JsonRequestBehavior.AllowGet);
            }

            var codigo = UsuariosBo.Excluir(id);
            string identityError = null;
            bool identityDisabled = true;

            // Buscar login do usu�rio
            var usuario = UsuariosBo.Consultar(id);
            if (usuario != null && !string.IsNullOrEmpty(usuario.Login))
            {
                try
                {
                    var apiUrl = ConfigurationManager.AppSettings["AuthApiUrl"];
                    using (var client = new System.Net.Http.HttpClient())
                    {
                        client.BaseAddress = new Uri(apiUrl);
                        if (!string.IsNullOrEmpty(Sessao.Token))
                        {
                            client.DefaultRequestHeaders.Authorization =
                                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Sessao.Token);
                        }
                        var disableUserData = new { UserName = usuario.Login };
                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(disableUserData);
                        var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
                        var response = await client.PostAsync("users/disable", content);
                        if (!response.IsSuccessStatusCode)
                        {
                            identityDisabled = false;
                            identityError = await response.Content.ReadAsStringAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    identityDisabled = false;
                    identityError = ex.Message;
                }
            }

            if (!identityDisabled)
            {
                var resultError = new { codigo = codigo, mensagem = "Erro ao desabilitar usuário na API de autenticação: " + identityError };
                return Json(resultError, JsonRequestBehavior.AllowGet);
            }

            var result = new { codigo = codigo };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //[RequireSubmenu("Controladoria:Usuarios")]
        public JsonResult Listar()
        {
            var usuarios = UsuariosBo.ListarArray();
            var result = new { codigo = "00", usuarios = usuarios };
            return Json(result);
        }


        [HttpPost]
        public async Task<JsonResult> AlterarSenha(String SenhaAux)
        {
            bool senhaIdentityAlterada = false;
            string erroIdentity = string.Empty;

            try
            {
                try
                {
                    var apiUrl = ConfigurationManager.AppSettings["AuthApiUrl"];
                    using (var client = new System.Net.Http.HttpClient())
                    {
                        client.BaseAddress = new Uri(apiUrl);

                        if (!string.IsNullOrEmpty(Sessao.Token))
                        {
                            client.DefaultRequestHeaders.Authorization =
                                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Sessao.Token);
                        }

                        var changePasswordData = new
                        {
                            UserName = Sessao.Usuario.Login,
                            NewPassword = SenhaAux
                        };

                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(changePasswordData);
                        var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");

                        var response = await client.PostAsync("users/change-password", content);

                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            var responseObj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseContent);

                            if (responseObj.success == true)
                            {
                                senhaIdentityAlterada = true;
                            }
                            else
                            {
                                erroIdentity = responseObj.error?.ToString() ?? "Erro desconhecido ao alterar senha no Identity";
                            }
                        }
                        else
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();

                            try
                            {
                                var errorObj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(errorContent);
                                erroIdentity = errorObj.error?.ToString() ?? errorContent;
                            }
                            catch
                            {
                                erroIdentity = errorContent;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    erroIdentity = ex.Message;
                }

                if (senhaIdentityAlterada)
                {
                    var result = new
                    {
                        codigo = "00",
                        mensagem = "Senha alterada com sucesso!"
                    };
                    return Json(result);
                }
                else
                {
                    var result = new
                    {
                        codigo = "98",
                        mensagem = $"Erro ao alterar senha no sistema de autenticação. Erro: {erroIdentity}. Por favor, tente novamente ou contate o administrador."
                    };
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                var result = new
                {
                    codigo = "99",
                    mensagem = $"Erro inesperado ao alterar senha: {ex.Message}"
                };
                return Json(result);
            }
        }


        [HttpPost]
        //[RequireSubmenu("Controladoria:Usuarios")]
        public async Task<JsonResult> Resetar2FA(string login)
        {
            try
            {
                if (Sessao.Usuario == null)
                {
                    return Json(new { codigo = "99", mensagem = "Usuário não autenticado" });
                }

                var apiUrl = ConfigurationManager.AppSettings["AuthApiUrl"];
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);

                    if (!string.IsNullOrEmpty(Sessao.Token))
                    {
                        client.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Sessao.Token);
                    }

                    var requestData = new { username = login };
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
                    var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("twofactor/admin/reset", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var resetResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseContent);

                        return Json(new
                        {
                            codigo = "00",
                            mensagem = $"2FA resetado com sucesso para o usuário {login}",
                            detalhes = resetResult
                        });
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        return Json(new
                        {
                            codigo = "98",
                            mensagem = "Você não tem permissão para realizar esta ação"
                        });
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return Json(new
                        {
                            codigo = "97",
                            mensagem = "Erro ao resetar 2FA: " + errorContent
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    codigo = "99",
                    mensagem = "Erro ao resetar 2FA: " + ex.Message
                });
            }
        }


        [HttpPost]
        [RequireSubmenu("Controladoria:Usuarios")]
        public async Task<JsonResult> ConsultarStatus2FA(string login)
        {
            try
            {
                if (Sessao.Usuario == null)
                {
                    return Json(new { codigo = "99", mensagem = "Usuário não autenticado" });
                }

                var apiUrl = ConfigurationManager.AppSettings["AuthApiUrl"];
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);

                    if (!string.IsNullOrEmpty(Sessao.Token))
                    {
                        client.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Sessao.Token);
                    }

                    var response = await client.GetAsync($"twofactor/status/{login}");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var status = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseContent);

                        return Json(new
                        {
                            codigo = "00",
                            twoFactorEnabled = (bool)status.twoFactorEnabled,
                            hasAuthenticator = (bool)status.hasAuthenticator,
                            recoveryCodesRemaining = (int)status.recoveryCodesRemaining
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            codigo = "97",
                            mensagem = "Erro ao consultar status do 2FA"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    codigo = "99",
                    mensagem = "Erro ao consultar status: " + ex.Message
                });
            }
        }


        [HttpPost]
        [RequireSubmenu("Controladoria:Usuarios")]
        public async Task<JsonResult> Ativar2FA(string login)
        {
            try
            {
                if (Sessao.Usuario == null)
                {
                    return Json(new { codigo = "99", mensagem = "Usuário não autenticado" });
                }

                //if (Sessao.Usuario.Nivel != 5)
                if (Sessao.Claims.FirstOrDefault(a => a.Tipo == "sistema" && a.Valor == "Ativar2FA") == null)
                {
                    return Json(new { codigo = "98", mensagem = "Você não tem permissão para ativar a verificação em duas etapas." });
                }

                var apiUrl = ConfigurationManager.AppSettings["AuthApiUrl"];
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);

                    if (!string.IsNullOrEmpty(Sessao.Token))
                    {
                        client.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Sessao.Token);
                    }

                    var requestData = new { username = login };
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
                    var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("twofactor/admin/enable", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new { codigo = "00", mensagem = "2FA ativado com sucesso" });
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        var statusCode = (int)response.StatusCode;
                        return Json(new
                        {
                            codigo = "97",
                            mensagem = $"Erro ao ativar 2FA (HTTP {statusCode}): {errorContent}",
                            statusCode = statusCode,
                            detalhes = errorContent
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { codigo = "99", mensagem = "Erro ao ativar 2FA: " + ex.Message, stackTrace = ex.StackTrace });
            }
        }

        /// <summary>
        /// Desativa 2FA para um usu�rio (sem desassociar)
        /// </summary>
        [HttpPost]
        [RequireSubmenu("Controladoria:Usuarios")]
        public async Task<JsonResult> Desativar2FA(string login)
        {
            try
            {
                if (Sessao.Usuario == null)
                {
                    return Json(new { codigo = "99", mensagem = "Usuário não autenticado" });
                }

                //                if (Sessao.Usuario.Nivel != 5)
                if (Sessao.Claims.FirstOrDefault(a => a.Tipo == "sistema" && a.Valor == "Ativar2FA") == null)
                {
                    return Json(new { codigo = "98", mensagem = "Você não tem permissão para desativar autenticação 2FA" });
                }

                var apiUrl = ConfigurationManager.AppSettings["AuthApiUrl"];
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);

                    if (!string.IsNullOrEmpty(Sessao.Token))
                    {
                        client.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Sessao.Token);
                    }

                    var requestData = new { username = login };
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
                    var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("twofactor/admin/disable", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new { codigo = "00", mensagem = "2FA desativado com sucesso" });
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return Json(new { codigo = "97", mensagem = "Erro ao desativar 2FA: " + errorContent });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { codigo = "99", mensagem = "Erro ao desativar 2FA: " + ex.Message });
            }
        }

        public async Task<JsonResult> ConsultarEmail(string login)
        {
            try
            {
                if (Sessao.Usuario == null)
                {
                    return Json(new { codigo = "99", mensagem = "Usuário não autenticado" });
                }

                var apiUrl = ConfigurationManager.AppSettings["AuthApiUrl"];
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);

                    if (!string.IsNullOrEmpty(Sessao.Token))
                    {
                        client.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Sessao.Token);
                    }

                    // Buscar informa��es do usu�rio
                    var response = await client.GetAsync($"users/{login}");

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var userResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseContent);

                        return Json(new
                        {
                            codigo = "00",
                            email = userResult.email?.ToString() ?? "",
                            emailConfirmed = (bool)(userResult.emailConfirmed ?? false)
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            codigo = "97",
                            mensagem = "Erro ao consultar e-mail"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    codigo = "99",
                    mensagem = "Erro ao consultar e-mail: " + ex.Message
                });
            }
        }
        public async Task<JsonResult> AtualizarEmail(string login, string email)
        {
            try
            {
                if (Sessao.Usuario == null)
                {
                    return Json(new { codigo = "99", mensagem = "Usuário não autenticado" });
                }

                if (!IsValidEmail(email))
                {
                    return Json(new
                    {
                        codigo = "98",
                        mensagem = "E-mail inválido"
                    });
                }

                var apiUrl = ConfigurationManager.AppSettings["AuthApiUrl"];
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);

                    if (!string.IsNullOrEmpty(Sessao.Token))
                    {
                        client.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Sessao.Token);
                    }

                    var updateData = new
                    {
                        UserName = login,
                        Email = email,
                        Password = "",
                        Role = ""
                    };

                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(updateData);
                    var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    var response = await client.PutAsync("users/update", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new
                        {
                            codigo = "00",
                            mensagem = "E-mail atualizado com sucesso"
                        });
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return Json(new
                        {
                            codigo = "97",
                            mensagem = "Erro ao atualizar e-mail: " + errorContent
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    codigo = "99",
                    mensagem = "Erro ao atualizar e-mail: " + ex.Message
                });
            }
        }


        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Novo m�todo para exportar usu�rios para migra��o
        [HttpPost]
        //[RequireSubmenu("Controladoria:Usuarios")]
        public JsonResult ExportarUsuariosParaMigracao()
        {
            try
            {
                if (Sessao.Usuario == null)
                {
                    return Json(new { codigo = "99", mensagem = "Usuário não autenticado" });
                }

                // Obt�m a lista de usu�rios
                var usuarios = UsuariosBo.ListarArray();

                // L�gica para exportar os usu�rios (pode variar conforme a necessidade)
                // Aqui estamos apenas convertendo para JSON como exemplo
                var jsonUsuarios = Newtonsoft.Json.JsonConvert.SerializeObject(usuarios);

                var result = new { codigo = "00", usuarios = jsonUsuarios };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { codigo = "99", mensagem = "Erro ao exportar usuários: " + ex.Message };
                return Json(result);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Ativar(Int32 Id)
        {
            string identityError = null;
            bool identityEnabled = true;
            var usuario = UsuariosBo.Consultar(Id);
            if (usuario != null && !string.IsNullOrEmpty(usuario.Login))
            {
                try
                {
                    var apiUrl = ConfigurationManager.AppSettings["AuthApiUrl"];
                    using (var client = new System.Net.Http.HttpClient())
                    {
                        client.BaseAddress = new Uri(apiUrl);
                        if (!string.IsNullOrEmpty(Sessao.Token))
                        {
                            client.DefaultRequestHeaders.Authorization =
                                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Sessao.Token);
                        }
                        var enableUserData = new { UserName = usuario.Login };
                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(enableUserData);
                        var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
                        var response = await client.PostAsync("users/enable", content);
                        if (!response.IsSuccessStatusCode)
                        {
                            identityEnabled = false;
                            identityError = await response.Content.ReadAsStringAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    identityEnabled = false;
                    identityError = ex.Message;
                }
            }
            if (identityEnabled)
            {
                UsuariosBo.Ativar(Id); // Bloqueio = false
            }
            if (!identityEnabled)
            {
                var resultError = new { codigo = "99", mensagem = "Erro ao ativar usuário na API de autenticação: " + identityError };
                return Json(resultError, JsonRequestBehavior.AllowGet);
            }
            var result = new { codigo = "00" };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // M�todo para listar perfis de acesso ativos
        [HttpGet]
        public async Task<JsonResult> ListarPerfisAtivos()
        {
            try
            {
                var apiUrl = ConfigurationManager.AppSettings["AuthApiUrl"];
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    if (!string.IsNullOrEmpty(Sessao.Token))
                    {
                        client.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Sessao.Token);
                    }

                    var response = await client.GetAsync("roles/roles-ativos");
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();

                        // Deserializar como uma lista de objetos din�micos
                        var perfisObj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(json);

                        // Extrair apenas os nomes dos perfis
                        var perfisNomes = new List<string>();
                        foreach (var perfil in perfisObj)
                        {
                            string roleName = perfil.Role ?? perfil.role ?? "";
                            if (!string.IsNullOrEmpty(roleName))
                            {
                                perfisNomes.Add(roleName);
                            }
                        }

                        return Json(new
                        {
                            codigo = "00",
                            perfis = perfisNomes
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            codigo = "97",
                            mensagem = "Erro ao carregar perfis de acesso"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    codigo = "99",
                    mensagem = "Erro ao carregar perfis: " + ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // ===== M�todos para Gerenciamento de Bloqueio (Lockout) =====

        [HttpPost]
        public async Task<JsonResult> ConsultarBloqueio(string login)
        {
            try
            {
                if (string.IsNullOrEmpty(login))
                {
                    return Json(new { codigo = "01", mensagem = "Login não fornecido" });
                }

                var apiUrl = ConfigurationManager.AppSettings["AuthApiUrl"];
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    if (!string.IsNullOrEmpty(Sessao.Token))
                    {
                        client.DefaultRequestHeaders.Authorization =
                  new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Sessao.Token);
                    }

                    var userData = new { UserName = login };
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(userData);
                    var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("users/lockout-status", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseJson = await response.Content.ReadAsStringAsync();
                        var lockoutInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseJson);

                        return Json(new
                        {
                            codigo = "00",
                            estaBloqueado = lockoutInfo.isLockedOut ?? false,
                            bloqueioAte = lockoutInfo.lockoutEnd ?? null,
                            tentativasRestantes = lockoutInfo.accessFailedCount ?? 0,
                            tentativasMaximas = lockoutInfo.maxFailedAttempts ?? 5,
                            duracaoBloqueio = lockoutInfo.lockoutDuration ?? 15
                        });
                    }
                    else
                    {
                        var errorJson = await response.Content.ReadAsStringAsync();
                        return Json(new { codigo = "97", mensagem = "Erro ao consultar bloqueio do usuário", erro = errorJson });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { codigo = "99", mensagem = "Erro ao consultar bloqueio: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> DesbloqueaUsuario(string login)
        {
            try
            {
                if (string.IsNullOrEmpty(login))
                {
                    return Json(new { codigo = "01", mensagem = "Login não fornecido" }, JsonRequestBehavior.AllowGet);
                }

                var apiUrl = ConfigurationManager.AppSettings["AuthApiUrl"];
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    if (!string.IsNullOrEmpty(Sessao.Token))
                    {
                        client.DefaultRequestHeaders.Authorization =
                              new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Sessao.Token);
                    }

                    var userData = new { UserName = login };
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(userData);
                    var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("users/unlock", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new
                        {
                            codigo = "00",
                            mensagem = "Usuário desbloqueado com sucesso!"
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();

                        // Tentar parsear como JSON ou usar o statuscode/reason
                        string errorMessage = errorContent;
                        try
                        {
                            if (!string.IsNullOrEmpty(errorContent))
                            {
                                dynamic errorObj = Newtonsoft.Json.JsonConvert.DeserializeObject(errorContent);
                                errorMessage = errorObj?.mensagem?.ToString() ??
                                       errorObj?.message?.ToString() ??
                                    errorContent;
                            }
                            else
                            {
                                errorMessage = $"Erro HTTP {response.StatusCode}: {response.ReasonPhrase}";
                            }
                        }
                        catch
                        {
                            if (string.IsNullOrEmpty(errorContent))
                            {
                                errorMessage = $"Erro HTTP {response.StatusCode}: {response.ReasonPhrase}";
                            }
                        }

                        return Json(new
                        {
                            codigo = "97",
                            mensagem = "Erro ao desbloquear usuário",
                            erro = errorMessage,
                            statusCode = (int)response.StatusCode
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { codigo = "99", mensagem = "Erro ao desbloquear usuário: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}