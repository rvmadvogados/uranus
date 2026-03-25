using Newtonsoft.Json;
using Sigman.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Uranus.Business;
using Uranus.Common;
using Uranus.Domain;
using Uranus.Suite.ViewModels;
using Uranus.Suite.Filters;

namespace Uranus.Suite.Controllers
{
    [RequireSubmenu("Controladoria:Perfis")]
    public class PerfisDeAcessoController : Controller
    {
        private readonly string _authApiUrl = ConfigurationManager.AppSettings["AuthApiUrl"];

        public async Task<ActionResult> Index()
        {
     if (Sessao.Usuario == null)
       return RedirectToAction("Index", "Login");

            List<PerfilDeAcessoViewModel> perfis = new List<PerfilDeAcessoViewModel>();
     using (var client = new HttpClient())
     {
 client.BaseAddress = new Uri(_authApiUrl);
       client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Sessao.Token);

                var response = await client.GetAsync("roles"); 
     if (response.IsSuccessStatusCode)
    {
         var json = await response.Content.ReadAsStringAsync();
   perfis = JsonConvert.DeserializeObject<List<PerfilDeAcessoViewModel>>(json);
          }
          }
       perfis = perfis.OrderBy(p => p.Role?.ToLowerInvariant()).ToList();
            return View(perfis);
        }

 public async Task<ActionResult> Create()
        {
    if (Sessao.Usuario == null)
         return RedirectToAction("Index", "Login");

          List<dynamic> availableClaims = new List<dynamic>();
      try
            {
    using (var client = new HttpClient())
     {
   client.BaseAddress = new Uri(_authApiUrl);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Sessao.Token);





          var response = await client.GetAsync("roles/available-claims");
             


      if (response.IsSuccessStatusCode)
          {
      var json = await response.Content.ReadAsStringAsync();
               availableClaims = JsonConvert.DeserializeObject<List<dynamic>>(json);

    }
         else
         {
             var errorMsg = await response.Content.ReadAsStringAsync();

  }
            }
            }
  catch (Exception ex)
      {


  }

ViewBag.AvailableClaims = availableClaims;


            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(PerfilDeAcessoViewModel model)
        {




 if (Sessao.Usuario == null)
      return RedirectToAction("Index", "Login");

            if (string.IsNullOrWhiteSpace(model?.Role))
         {

       return View(model);
     }

     try
      {
          // Obter todas as claims disponíveis para mapear o Type
            var claimsData = new List<dynamic>();
   using (var claimsClient = new HttpClient())
    {
        claimsClient.BaseAddress = new Uri(_authApiUrl);
     claimsClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Sessao.Token);

          var claimsResponse = await claimsClient.GetAsync("roles/available-claims");
         if (claimsResponse.IsSuccessStatusCode)
        {
     var claimsJson = await claimsResponse.Content.ReadAsStringAsync();
         claimsData = JsonConvert.DeserializeObject<List<dynamic>>(claimsJson);
    }
       }

 var claimObjects = new List<object>();
            if (model.Claims != null && model.Claims.Count > 0)
       {
        foreach (var claimId in model.Claims)
    {
            if (!string.IsNullOrWhiteSpace(claimId))
    {
          // Procurar a claim pelos dados disponíveis para obter o Type
        var availableClaim = claimsData?.FirstOrDefault(c =>
        (string.Equals((c.Id ?? c.id)?.ToString(), claimId, StringComparison.OrdinalIgnoreCase) ||
       string.Equals((c.Value ?? c.value)?.ToString(), claimId, StringComparison.OrdinalIgnoreCase)));

        if (availableClaim != null)
     {
             var claimType = availableClaim.Type ?? availableClaim.type ?? "claim";
         var claimValue = availableClaim.Value ?? availableClaim.value ?? claimId;

             claimObjects.Add(new
   {
 Type = claimType,
     Value = claimValue
    });


       }
     }
  }
        }



                var perfil = new
            {
  Role = model.Role,
         Claims = claimObjects
                };

         var jsonContent = JsonConvert.SerializeObject(perfil);


         using (var client = new HttpClient())
    {
          client.BaseAddress = new Uri(_authApiUrl);
  client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Sessao.Token);

     var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
     var response = await client.PostAsync("roles", content);



           if (response.IsSuccessStatusCode)
      {

       return RedirectToAction("Index");
             }

            var errorMsg = await response.Content.ReadAsStringAsync();

           ModelState.AddModelError("", $"Erro ao criar perfil: {errorMsg}");
        }
            }
            catch (Exception ex)
{


    ModelState.AddModelError("", $"Erro ao criar perfil: {ex.Message}");
  }

            // Se houver erro, recarregar as claims disponíveis
         List<dynamic> availableClaims = new List<dynamic>();
       try
{
     using (var client = new HttpClient())
    {
       client.BaseAddress = new Uri(_authApiUrl);
           client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Sessao.Token);

  var response = await client.GetAsync("roles/available-claims");
        if (response.IsSuccessStatusCode)
      {
      var json = await response.Content.ReadAsStringAsync();
         availableClaims = JsonConvert.DeserializeObject<List<dynamic>>(json);
    }
                }
      }
            catch (Exception ex)
            {

          }

    ViewBag.AvailableClaims = availableClaims;

            return View(model);
        }

        public async Task<ActionResult> Filtrar(string search, string status)
        {
         if (Sessao.Usuario == null)
       return RedirectToAction("Index", "Login");

 List<PerfilDeAcessoViewModel> perfis = new List<PerfilDeAcessoViewModel>();
            try
 {
              using (var client = new HttpClient())
       {
    client.BaseAddress = new Uri(_authApiUrl);
     client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Sessao.Token);

     var response = await client.GetAsync("roles");
         if (response.IsSuccessStatusCode)
           {
               var json = await response.Content.ReadAsStringAsync();
           perfis = JsonConvert.DeserializeObject<List<PerfilDeAcessoViewModel>>(json);
                 }
    else
    {
            return PartialView("_PerfisDeAcessoGrid", new List<PerfilDeAcessoViewModel>());
   }
                }

             if (!string.IsNullOrWhiteSpace(search))
      {
            perfis = perfis.Where(p => p.Role != null && p.Role.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
      }

              if (!string.IsNullOrWhiteSpace(status) && status != "T")
 {
         bool ativo = status == "A";
             perfis = perfis.Where(p => p.IsActive == ativo).ToList();
                }

       perfis = perfis.OrderBy(p => p.Role?.ToLowerInvariant()).ToList();

   return PartialView("_PerfisDeAcessoGrid", perfis);
            }
     catch (Exception ex)
   {

      return PartialView("_PerfisDeAcessoGrid", new List<PerfilDeAcessoViewModel>());
            }
        }

        public async Task<ActionResult> Edit(string id)
        {
       if (Sessao.Usuario == null)
                return RedirectToAction("Index", "Login");

            PerfilDeAcessoViewModel perfil = null;
     using (var client = new HttpClient())
            {
    client.BaseAddress = new Uri(_authApiUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Sessao.Token);

          var response = await client.GetAsync($"roles/{id}");
        if (response.IsSuccessStatusCode)
        {
           var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(json);

        var claims = new List<string>();
      if (result.selectedClaimIds != null)
  {
     foreach (var idClaim in result.selectedClaimIds)
   {
          claims.Add(idClaim.ToString());
         }
 }

                    perfil = new PerfilDeAcessoViewModel
     {
                  Id = result.id,
          Role = result.name,
       Claims = claims
   };
            }
        else
     {
     return HttpNotFound();
    }
       }

   List<dynamic> availableClaims = new List<dynamic>();
            using (var client = new HttpClient())
            {
     client.BaseAddress = new Uri(_authApiUrl);
          client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Sessao.Token);

      var response = await client.GetAsync("roles/available-claims");
     if (response.IsSuccessStatusCode)
     {
        var json = await response.Content.ReadAsStringAsync();
availableClaims = JsonConvert.DeserializeObject<List<dynamic>>(json);
                }
   }
   ViewBag.AvailableClaims = availableClaims;

     return View(perfil);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(PerfilDeAcessoViewModel model)
        {





            if (Sessao.Usuario == null)
                return RedirectToAction("Index", "Login");

            if (string.IsNullOrWhiteSpace(model?.Role) || string.IsNullOrWhiteSpace(model?.Id))
        {

       return View(model);
     }

       try
   {
         // Obter todas as claims disponíveis para mapear o Type
    var claimsData = new List<dynamic>();
      using (var claimsClient = new HttpClient())
          {
         claimsClient.BaseAddress = new Uri(_authApiUrl);
           claimsClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Sessao.Token);

              var claimsResponse = await claimsClient.GetAsync("roles/available-claims");
             if (claimsResponse.IsSuccessStatusCode)
          {
      var claimsJson = await claimsResponse.Content.ReadAsStringAsync();
            claimsData = JsonConvert.DeserializeObject<List<dynamic>>(claimsJson);
        }
        }

 var claimObjects = new List<object>();
                if (model.Claims != null && model.Claims.Count > 0)
          {
      foreach (var claimId in model.Claims)
       {
       if (!string.IsNullOrWhiteSpace(claimId))
   {
    // Procurar a claim pelos dados disponíveis para obter o Type
 var availableClaim = claimsData?.FirstOrDefault(c =>
             (string.Equals((c.Id ?? c.id)?.ToString(), claimId, StringComparison.OrdinalIgnoreCase) ||
  string.Equals((c.Value ?? c.value)?.ToString(), claimId, StringComparison.OrdinalIgnoreCase)));

          if (availableClaim != null)
        {
       var claimType = availableClaim.Type ?? availableClaim.type ?? "claim";
          var claimValue = availableClaim.Value ?? availableClaim.value ?? claimId;

           claimObjects.Add(new
      {
         Type = claimType,
  Value = claimValue
 });


            }
    }
      }
     }



                var perfil = new
      {
      Role = model.Role,
     Claims = claimObjects
              };

    var jsonContent = JsonConvert.SerializeObject(perfil);


     using (var client = new HttpClient())
         {
     client.BaseAddress = new Uri(_authApiUrl);
              client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Sessao.Token);

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
      var response = await client.PutAsync($"roles/{model.Id}", content);



          if (response.IsSuccessStatusCode)
                 {

return RedirectToAction("Index");
          }

          var errorMsg = await response.Content.ReadAsStringAsync();

          ModelState.AddModelError("", $"Erro ao atualizar perfil: {errorMsg}");
           }
        }
      catch (Exception ex)
      {


         ModelState.AddModelError("", $"Erro ao atualizar perfil: {ex.Message}");
 }

       // Se houver erro, recarregar o modelo e as claims disponíveis
  List<dynamic> availableClaims = new List<dynamic>();
         try
            {
                using (var client = new HttpClient())
        {
     client.BaseAddress = new Uri(_authApiUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Sessao.Token);

             var response = await client.GetAsync("roles/available-claims");
                    if (response.IsSuccessStatusCode)
          {
             var json = await response.Content.ReadAsStringAsync();
         availableClaims = JsonConvert.DeserializeObject<List<dynamic>>(json);
}
    }
          }
  catch (Exception ex)
            {

            }

            ViewBag.AvailableClaims = availableClaims;

        return View(model);
        }

[HttpPost]
        public async Task<JsonResult> AtivarInativar(string id, bool ativar)
   {
            if (Sessao.Usuario == null)
                return Json(new { codigo = "99", mensagem = "Usuário não autenticado" });

try
            {
           using (var client = new HttpClient())
   {
      client.BaseAddress = new Uri(_authApiUrl);
       client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Sessao.Token);

  if (!ativar)
     {
       var checkUsersResponse = await client.GetAsync($"roles/{id}/users");
   if (checkUsersResponse.IsSuccessStatusCode)
        {
var usersJson = await checkUsersResponse.Content.ReadAsStringAsync();
               var users = JsonConvert.DeserializeObject<List<string>>(usersJson);

                if (users != null && users.Count > 0)
       {
    return Json(new
    {
       codigo = "98",
             mensagem = $"Não é possível inativar este perfil pois existem {users.Count} usuário(s) associado(s) a ele. Remova os usuários primeiro."
  });
          }
        }
           }

              var activateData = new { isActive = ativar };
  var content = new StringContent(JsonConvert.SerializeObject(activateData), Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"roles/{id}/activate", content);

     if (response.IsSuccessStatusCode)
          {
               string acao = ativar ? "ativado" : "inativado";
      return Json(new
            {
     codigo = "00",
     mensagem = $"Perfil {acao} com sucesso!"
       });
          }
      else
               {
    var errorContent = await response.Content.ReadAsStringAsync();
       return Json(new
     {
         codigo = "97",
     mensagem = "Erro ao alterar status do perfil: " + errorContent
     });
          }
      }
            }
    catch (Exception ex)
      {
         return Json(new
    {
            codigo = "99",
    mensagem = "Erro inesperado: " + ex.Message
                });
            }
     }

        public async Task<ActionResult> Delete(string id)
        {
     if (Sessao.Usuario == null)
       return RedirectToAction("Index", "Login");

       using (var client = new HttpClient())
            {
          client.BaseAddress = new Uri(_authApiUrl);
  client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Sessao.Token);

       var response = await client.DeleteAsync($"roles/{id}");
                if (response.IsSuccessStatusCode)
    {
   return RedirectToAction("Index");
           }
    else
    {
         var errorMsg = await response.Content.ReadAsStringAsync();
             ModelState.AddModelError("", $"Erro ao excluir perfil: {errorMsg}");
       }
 }

      return RedirectToAction("Index");
      }
    }
}
