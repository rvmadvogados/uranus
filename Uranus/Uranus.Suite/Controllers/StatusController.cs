using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Common;
using Uranus.Domain;
using Uranus.Suite;

namespace Taylor.Site.Controllers
{
    public class StatusController : Controller
    {
        // GET: Status
        public ActionResult Index(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = StatusBO.Listar(search);
                ViewBag.search = search;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var status = StatusBO.ConsultarArray(Id);
            var result = new { codigo = "00", status = status };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String Nome, String TipoStatus, String TipoAcao, String Ativo)
        {
            Status status = new Status();
            status.ID = Id;
            status.Nome = Nome.Trim();
            status.TipoStatus = TipoStatus.Trim();
            status.TipoAcao = TipoAcao.Trim();

            if (Ativo == "S")
            {
                status.Status1 = true;
            }
            else
            {
                status.Status1 = false;
            }

            if (Id == 0)
                Id = StatusBO.Inserir(status);

            status.ID = Id;
            StatusBO.Salvar(status);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = StatusBO.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar(String Tipo)
        {
            var status = StatusBO.Consultar(Tipo);
            var result = new { codigo = "00", status = status };
            return Json(result);
        }

        public static String BuscaTipoStatus(String value)
        {
            var tipo = String.Empty;

            if (value == "O")
            {
                tipo = "Orçamento";
            }
            else if (value == "P")
            {
                tipo = "Pedido";
            }
            else if (value == "S")
            {
                tipo = "Serviço";
            }
            else if (value == "N")
            {
                tipo = "Nota Fiscal";
            }
            else if (value == "D")
            {
                tipo = "Devolução";
            }

            return tipo;
        }

        public static String BuscaTipoAcao(String value)
        {
            var tipo = String.Empty;

            if (value == "NO")
            {
                tipo = "Novo Orçamento";
            }
            else if (value == "OA")
            {
                tipo = "Orçamento Em Andamento";
            }
            else if (value == "OC")
            {
                tipo = "Orçamento Perdido";
            }
            else if (value == "OFI")
            {
                tipo = "Orçamento Fechamento Imediato";
            }
            else if (value == "OMP")
            {
                tipo = "Orçamento Fechamento a Médio Prazo";
            }
            else if (value == "OLP")
            {
                tipo = "Orçamento Fechamento a Longo Prazo";
            }
            else if (value == "OF")
            {
                tipo = "Orçamento Fechado";
            }
            else if (value == "NP")
            {
                tipo = "Novo Pedido";
            }
            else if (value == "PEV")
            {
                tipo = "Pedido Enviado";
            }
            else if (value == "PEG")
            {
                tipo = "Pedido Entregue";
            }
            else if (value == "PEF")
            {
                tipo = "Pedido Fechado";
            }
            else if (value == "PC")
            {
                tipo = "Pedido Cancelado";
            }
            else if (value == "NFE")
            {
                tipo = "Nota Fiscal Emitida";
            }
            else if (value == "NFN")
            {
                tipo = "Nota Fiscal Nova";
            }
            else if (value == "NFL")
            {
                tipo = "Nota Fiscal Lida";
            }
            else if (value == "NFF")
            {
                tipo = "Nota Fiscal Fechada";
            }
            else if (value == "NFC")
            {
                tipo = "Nota Fiscal Cancelada";
            }
            else if (value == "NS")
            {
                tipo = "Nova Ordem de Seviço";
            }
            else if (value == "SEV")
            {
                tipo = "Ordem de Seviço Enviada";
            }
            else if (value == "SEG")
            {
                tipo = "Ordem de Seviço Entregue";
            }
            else if (value == "SC")
            {
                tipo = "Ordem de Seviço Cancelada";
            }
            else if (value == "SEF")
            {
                tipo = "Ordem de Seviço Fechada";
            }
            else if (value == "SEA")
            {
                tipo = "Ordem de Seviço Em Andamento";
            }

            return tipo;
        }
    }
}