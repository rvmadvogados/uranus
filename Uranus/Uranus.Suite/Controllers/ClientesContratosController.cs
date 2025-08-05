using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class ClientesContratosController : Controller
    {
        // GET: ClientesContratos
        public ActionResult Index(string FiltrarDataInicio = "", string FiltrarDataFim = "", string FiltrarNumeroContrato = "", string FiltrarCliente = "", Int32? FiltrarSede = null, Int32? FiltrarArea = null)
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = ClientesContratosBo.Listar(FiltrarDataInicio, FiltrarDataFim, FiltrarNumeroContrato, FiltrarCliente, FiltrarSede, FiltrarArea);
                ViewBag.FiltrarDataInicio = FiltrarDataInicio;
                ViewBag.FiltrarDataFim = FiltrarDataFim;
                ViewBag.FiltrarNumeroContrato = FiltrarNumeroContrato;
                ViewBag.FiltrarCliente = FiltrarCliente;
                ViewBag.FiltrarSede = FiltrarSede;
                ViewBag.FiltrarArea = FiltrarArea;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int64 Id)
        {
            var clientescontratos = ClientesContratosBo.ConsultarArray(Id);

            var arquivo = "Nenhum arquivo selecionado";
            var file = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}.pdf", "Contrato-" + Id.ToString()));
            if (System.IO.File.Exists(file))
            {
                arquivo = "<a style='cursor: pointer;' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}.pdf", "Contrato-" + Id.ToString()) + '"' + ");'>" + String.Format("{0}.pdf", "Contrato-" + Id.ToString()) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;<a id='ButtonContratoPDFExcluir' class='btn btn-danger btn-xs' onclick='ExcluirPDF()'>Apagar</a>&nbsp;&nbsp;<label id='LabelClienteFileExcluirConfirmacao' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse PDF?&nbsp;&nbsp;&nbsp;</label><a id='acceptFile' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirPDFSim(" + '"' + String.Format("{0}.pdf", "Contrato-" + Id.ToString()) + '"' + ")'>Sim</a><a id='go_backFile' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirPDFNao()'>Não</a>";
            }

            var result = new { codigo = "00", clientescontratos = clientescontratos, arquivo = arquivo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String NumeroContrato, Int32 IdProfissional, Int32 IdCliente, Int32 IdArea, String Objeto, String Data, String ValorHonorarios, String SaldoHonorarios,
                                 String CondicaoPagamento, String NumeroMeses, String PrimeiroVencimento, String FormaPagamento, String ValorParcela, String Observacao, String DataPagamento,
                                 Int32? IdBanco, String NumeroProcesso, HttpPostedFileBase ContratoArquivo, Int32 IdAcao, Int32 IdAcaoEvento)
        {
            var codigo = "00";
            var mensagem = "";

            ClientesContratos clientescontratos = new ClientesContratos();
            clientescontratos.ID = Id;
            clientescontratos.NumeroContrato = NumeroContrato;
            clientescontratos.IdCliente = IdCliente;
            clientescontratos.IdProfissional = IdProfissional;
            clientescontratos.IdArea = IdArea;
            clientescontratos.IdAcao = IdAcao;
            clientescontratos.IdAcaoEvento = IdAcaoEvento;

            if (Data != null && Data.Trim().Length > 0)
            {
                clientescontratos.Data = DateTime.Parse(Data);
            }

            if (ValorHonorarios != null && ValorHonorarios.Trim().Length > 0)
            {
                clientescontratos.ValorHonorarios = Decimal.Parse(ValorHonorarios);
            }
            if (SaldoHonorarios != null && SaldoHonorarios.Trim().Length > 0)
            {
                clientescontratos.SaldoHonorarios = Decimal.Parse(SaldoHonorarios);
            }
            clientescontratos.CondicaoPagamento = CondicaoPagamento;

            if (NumeroMeses != null && NumeroMeses.Trim().Length > 0)
            {
                clientescontratos.NumeroMeses = int.Parse(NumeroMeses);
            }

            if (PrimeiroVencimento != null && PrimeiroVencimento.Trim().Length > 0)
            {
                clientescontratos.PrimeiroVencimento = DateTime.Parse(PrimeiroVencimento);
            }

            if (FormaPagamento != null && FormaPagamento.Trim().Length > 0)
            {
                clientescontratos.FormaPagamento = int.Parse(FormaPagamento);
            }

            if (ValorParcela != null && ValorParcela.Trim().Length > 0)
            {
                clientescontratos.ValorParcela = Decimal.Parse(ValorParcela);
            }
            clientescontratos.Observacao = Observacao;

            if (DataPagamento != null && DataPagamento.Trim().Length > 0)
            {
                clientescontratos.DataPagamento = DateTime.Parse(DataPagamento);
            }

            if (IdBanco == 0) 
            {
                clientescontratos.IdBanco = null;
            }
            else
            {
                clientescontratos.IdBanco = IdBanco;
            }
            clientescontratos.NumeroProcesso = NumeroProcesso;

            clientescontratos.DataCadastro = DateTime.Now;
            clientescontratos.NomeUsuarioCadastro = Sessao.Usuario.Nome;
            clientescontratos.DataAlteracao = DateTime.Now;
            clientescontratos.NomeUsuarioAlteracao = Sessao.Usuario.Nome;
            clientescontratos.IdUsuarioCadastro = Sessao.Usuario.ID;


            #region Ações Eventos

            var numeroProcesso = "";

            var area = AreasBo.Consultar(IdArea);
            var processoacao = AcaoBo.Consultar(IdAcao);

            var nomeFormaPagamento = "";
            if (FormaPagamento != null && FormaPagamento.Trim().Length > 0)
            {
                nomeFormaPagamento = ClientesContratosBo.ConsultarFormaPagamento(int.Parse(FormaPagamento)).Nome;
            }
            numeroProcesso = "Histórico " + area.AreaAtuacao;

            var acoes = ProcessosAcoesBo.ConsultarNumeroProcessoContrato(IdCliente, numeroProcesso);

            if (acoes != null)
            {
                Int32? IdProcessosAcao = null;
                if (processoacao != null)
                {
                    IdProcessosAcao = acoes.ID;
                }

                var descricao = "";
                descricao = "ÁREA: " + area.AreaAtuacao + "<br />";
                descricao = descricao + "OBJETO DO CONTRATO: " + processoacao.Acao + "<br />";
                if (NumeroMeses == null || NumeroMeses.Trim() == "1")
                {
                    descricao = descricao + "PAGAMENTO À VISTA " + "<br />";
                }
                else
                {
                    descricao = descricao + "PAGAMENTO PARCELADP EM " + NumeroMeses + " VEZES, A COMECAR NO DIA " + PrimeiroVencimento + "<br />";

                }
                descricao = descricao + "FORMA DE PAGAMENTO: " + nomeFormaPagamento + "<br />";

                if (SaldoHonorarios != null && SaldoHonorarios.Trim().Length > 0)
                {
                    descricao = descricao + "HONORÁRIOS A PAGAR NO FINAl DO PROCESSO: " + SaldoHonorarios + "<br />";
                }

                descricao = descricao + "OBS: " + Observacao + "<br />";

                ProcessosAcoesEventos acao = new ProcessosAcoesEventos();
                acao.ID = IdAcaoEvento;
                acao.IdProcessosAcao = int.Parse(IdProcessosAcao.ToString());
                acao.IdProcessosEvento = 28;
                acao.Data = clientescontratos.Data;
                acao.DataAlteracao = DateTime.Now;
                acao.Descricao = descricao;
                acao.DataCumprimentoPrazo = null;
                acao.PrazoEvento1 = null;
                acao.PrazoEvento2 = null;
                acao.IdProcessosEventoPendente = null;
                acao.IdUsuario = Sessao.Usuario.ID;
                acao.IdUsuarioAlteracao = Sessao.Usuario.ID;

                if (IdAcaoEvento == 0)
                {
                    acao.ID = ProcessosAcoesEventosBo.Inserir(acao);
                    IdAcaoEvento = acao.ID;
                }

                ProcessosAcoesEventosBo.Salvar(acao);
                #endregion



                clientescontratos.IdAcaoEvento = IdAcaoEvento;

                if (Id == 0)
                {
                    Id = ClientesContratosBo.Inserir(clientescontratos);
                }


                clientescontratos.ID = Id;

                ClientesContratosBo.Salvar(clientescontratos);

                #region Arquivo PDF
                var url = String.Empty;
                if (ContratoArquivo != null && ContratoArquivo.ContentLength > 0)
                {
                    var file = Path.Combine(Server.MapPath("~/Uploads"), String.Format("Contrato-{0}.pdf", Id.ToString()));
                    ContratoArquivo.SaveAs(file);
                }
                #endregion
            }
            else
            {
                mensagem = numeroProcesso + " não cadastrado ";
                codigo = "99";
            }

            var result = new { codigo = codigo, mensagem = mensagem};
            return Json(result);
        }

        [HttpPost]
        public JsonResult ConsultarAcao(Int32 Id, Int32 IdCliente)
        {
            var codigo = "";
            using (var context = new UranusEntities())
            {
                var numeroProcesso = "";

                var area = AreasBo.Consultar(Id);
                numeroProcesso = "Histórico " + area.AreaAtuacao;

                var acoes = ProcessosAcoesBo.ConsultarNumeroProcessoContrato(IdCliente, numeroProcesso);
                if (acoes != null)
                {
                    codigo = "00";
                } 
                else
                {
                    codigo = "99";

                }
            }

            var result = new { codigo = codigo };
            return Json(result);
        }



        public JsonResult FinanceiroAtualizar(Int32 Id)
        {
            var codigo = "00";
            try
            {
                using (var context = new UranusEntities())
                {
                    SqlParameter param1 = new SqlParameter("@Id", Id);
                    SqlParameter param2 = new SqlParameter("@Tipo", "I");
                    SqlParameter param3 = new SqlParameter("@Funcionario", Sessao.Usuario.Nome);

                    context.Database.ExecuteSqlCommand("stpFinanceiroContratos @Id, @Tipo, @Funcionario", param1, param2, param3);

                }
            }
            catch (Exception ex)
            {
                codigo = "99";
                throw;
            }

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            using (var context = new UranusEntities())
            {
                SqlParameter param1 = new SqlParameter("@Id", Id);
                SqlParameter param2 = new SqlParameter("@Tipo", "I");
                SqlParameter param3 = new SqlParameter("@Funcionario", Sessao.Usuario.Nome);

                context.Database.ExecuteSqlCommand("stpFinanceiroContratos @Id, @Tipo, @Funcionario", param1, param2, param3);
            }

            var codigo = ClientesContratosBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        public JsonResult ExcluirPDF(String Arquivo)
        {
            try
            {
                var file = Path.Combine(Server.MapPath("~/Uploads"), Arquivo);
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }

                var result = new { response = "success" };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult Listar()
        {
            var clientescontratos = ClientesContratosBo.Consultar();
            var result = new { codigo = "00", clientescontratos = clientescontratos };
            return Json(result);
        }

}
}