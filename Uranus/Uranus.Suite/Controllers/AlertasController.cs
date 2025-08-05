using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class AlertasController : Controller
    {
        [HttpPost]
        public JsonResult Inserir(Int32? IdAcao, String NumeroProcesso, String Mensagem, Int32? IdPessoa)
        {
            if (IdAcao.HasValue && IdAcao.Value == 0) 
            {
                ProcessosAcoes idacao = ProcessosAcoesBo.ConsultarNumeroProcesso(NumeroProcesso);
                IdAcao = idacao.IdProcesso;
            }

            Alertas alerta = new Alertas();

            if (IdAcao.HasValue)
            {
                alerta.IdAcao = IdAcao.Value;
            }

            alerta.NumeroProcesso = NumeroProcesso;
            alerta.Mensagem = Mensagem.Trim();
            alerta.DataHora = DateTime.Now;
            alerta.IdUsuario = Sessao.Usuario.ID;
            alerta.Lido = false;

            if (IdPessoa.HasValue)
            {
                alerta.IdPessoa = IdPessoa.Value;
            }

            AlertasBo.Inserir(alerta);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(String IdAlertas)
        {
            if (!String.IsNullOrEmpty(IdAlertas))
            {
                var id = IdAlertas.Split(',');

                for (int i = 0; i < id.Length; i++)
                {
                    Alertas alerta = AlertasBo.Consultar(Int64.Parse(id[i]));
                    alerta.Lido = true;

                    AlertasBo.Salvar(alerta);
                }
            }

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Consultar(Int32? IdAcao, String NumeroProcesso, String Tipo, Int32? IdPessoa)
        {
            if (IdAcao.HasValue && IdAcao.Value == 0)
            {
                ProcessosAcoes acao = ProcessosAcoesBo.ConsultarNumeroProcesso(NumeroProcesso);
                IdAcao = (acao != null ? acao.IdProcesso.Value : 0);
            }

            var alerta = AlertasBo.Listar((IdAcao.HasValue ? IdAcao.Value : IdAcao), (IdPessoa.HasValue ? IdPessoa.Value : IdPessoa));

            if (IdAcao.HasValue)
            {
                var acao = ProcessosBo.Buscar(IdAcao.Value);

                foreach (var autor in acao.ProcessosAutores)
                {
                    alerta.AddRange(AlertasBo.Listar(null, autor.Clientes.IDPessoa));
                }

                foreach (var reu in acao.ProcessosPartes)
                {
                    alerta.AddRange(AlertasBo.Listar(null, reu.Clientes.IDPessoa));
                }
            }

            var alertas = String.Empty;

            if (alerta.Count > 0)
            {
                alertas += "<table class='table table-striped' style='width: 100% !important; font-size: 12px !important;'>";
                alertas += "    <thead>";
                alertas += "        <tr>";
                alertas += "            <th>Data</th>";
                alertas += "            <th>Hora</th>";
                alertas += "            <th>Usuário</th>";
                alertas += "            <th>Número Processo</th>";
                alertas += "            <th>Mensagem</th>";
                alertas += "            <th class='right " + (Tipo == "P" ? string.Empty : "hidden") + "'>Excluir</th>";
                alertas += "        </tr>";
                alertas += "    </thead>";
                alertas += "    <tbody>";

                for (int i = 0; i < alerta.Count; i++)
                {
                    alertas += "        <tr>";
                    alertas += "            <th scope='row'>" + alerta[i].DataHora.ToString("dd/MM/yyyy") + "</th>";
                    alertas += "            <td>" + alerta[i].DataHora.ToString("HH:mm") + "</td>";
                    alertas += "            <td>" + alerta[i].Usuarios.Nome + "</td>";
                    alertas += "            <td>" + (alerta[i].NumeroProcesso == "Cadastro" ? alerta[i].NumeroProcesso : NumeroProcesso) + "</td>";
                    alertas += "            <td>" + alerta[i].Mensagem + "</td>";
                    alertas += "            <td class='right " + (Tipo == "P" ? string.Empty : "hidden") + "' style='margin: 0px !important; padding: 0px !important;'><div class='checkbox-item'><input type='checkbox' id='alerta_" + alerta[i].Id.ToString() + "_" + i.ToString() + "' name='alertas[]' value='" + alerta[i].Id.ToString() + "'><label for='alerta_" + alerta[i].Id.ToString() + "_" + i.ToString() + "'></label></div></td>";
                    alertas += "        </tr>";
                }

                alertas += "    </tbody>";
                alertas += "</table>";
            }
            var result = new { codigo = "00", alertas = alertas };
            return Json(result);
        }
    }
}