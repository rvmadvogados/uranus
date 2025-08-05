using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class ClientesFinanceiroIndicacoesController : Controller
    {
        public ActionResult Index(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = ClientesFinanceiroIndicacoesBo.Listar(search);
                ViewBag.search = search;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int64 Id)
        {
            var clientesfinanceiroindicacoes = ClientesFinanceiroIndicacoesBo.ConsultarArray(Id);
            var result = new { codigo = "00", clientesfinanceiroindicacoes = clientesfinanceiroindicacoes };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int64 Id, Int32 IdCliente, string Tipo, String Caixa, String PercentualNF, Int32? IdProfissional, String PercentualProfissional, Int32? IdIndicacao1, String PercentualIndicacao1, Int32? IdIndicacao2,
                                 String PercentualIndicacao2, String TipoComissao, Int32? IdColaborador, String PercentualColaborador)
        {
            ClientesFinanceiroIndicacoes clientesfinanceiroindicacoes = new ClientesFinanceiroIndicacoes();
            clientesfinanceiroindicacoes.Id = Id;
            clientesfinanceiroindicacoes.IdCliente = IdCliente;

            clientesfinanceiroindicacoes.Tipo = 0;
            //if (Tipo != null && Tipo.Trim().Length > 0)
            //{
            //    clientesfinanceiroindicacoes.Tipo = int.Parse(Tipo);
            //}

            if (Caixa != null && Caixa.Trim().Length > 0)
            {
                clientesfinanceiroindicacoes.Caixa = Decimal.Parse(Caixa);
            }

            if (PercentualNF != null && PercentualNF.Trim().Length > 0)
            {
                clientesfinanceiroindicacoes.PercentualNF = Decimal.Parse(PercentualNF);
            }

            clientesfinanceiroindicacoes.IdProfissional = IdProfissional;

            clientesfinanceiroindicacoes.PercentualProfissional = 0;
            if (PercentualProfissional != null && PercentualProfissional.Trim().Length > 0)
            {
                clientesfinanceiroindicacoes.PercentualProfissional = Decimal.Parse(PercentualProfissional);
            }

            clientesfinanceiroindicacoes.IdIndicacao1 = IdIndicacao1;

            clientesfinanceiroindicacoes.PercentualIndicacao1 = 0;
            if (PercentualIndicacao1 != null && PercentualIndicacao1.Trim().Length > 0)
            {
                clientesfinanceiroindicacoes.PercentualIndicacao1 = Decimal.Parse(PercentualIndicacao1);
            }

            clientesfinanceiroindicacoes.IdIndicacao2 = IdIndicacao2;

            clientesfinanceiroindicacoes.PercentualIndicacao2 = 0;
            if (PercentualIndicacao2 != null && PercentualIndicacao2.Trim().Length > 0)
            {
                clientesfinanceiroindicacoes.PercentualIndicacao2 = Decimal.Parse(PercentualIndicacao2);
            }


            clientesfinanceiroindicacoes.TipoComissao = TipoComissao;

            clientesfinanceiroindicacoes.IdColaborador = IdColaborador;

            clientesfinanceiroindicacoes.PercentualColaborador = 0;
            if (PercentualColaborador != null && PercentualColaborador.Trim().Length > 0)
            {
                clientesfinanceiroindicacoes.PercentualColaborador = Decimal.Parse(PercentualColaborador);
            }

            clientesfinanceiroindicacoes.DataCadastro = DateTime.Now;
            clientesfinanceiroindicacoes.NomeUsuarioCadastro = Sessao.Usuario.Nome;
            clientesfinanceiroindicacoes.DataAlteracao = DateTime.Now;
            clientesfinanceiroindicacoes.NomeUsuarioAlteracao = Sessao.Usuario.Nome;

            if (Id == 0)
                Id = ClientesFinanceiroIndicacoesBo.Inserir(clientesfinanceiroindicacoes);

            clientesfinanceiroindicacoes.Id = Id;
            ClientesFinanceiroIndicacoesBo.Salvar(clientesfinanceiroindicacoes);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int64 Id)
        {
            var codigo = ClientesFinanceiroIndicacoesBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar()
        {
            var clientesfinanceiroindicacoes = ClientesFinanceiroIndicacoesBo.Consultar();
            var result = new { codigo = "00", clientesfinanceiroindicacoes = clientesfinanceiroindicacoes };
            return Json(result);
        }
    }
}