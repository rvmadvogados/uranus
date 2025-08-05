using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Common;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{

    public class ListarColModel
    {
        public ProfissionalModel Profissional { get; set; }
        public List<ProfissionaisSolicitacoes> SolicitacoesFerias { get; set; }
        public List<ProfissionaisDocumentos> Documentos { get; set; }
        public List<ProfissionaisAusencias> Ausencias { get; set; }
        public List<ProfissionaisSolicitacoesEspeciais> SolicitacosEspeciais { get; set; }
        public List<ProfissionaisContracheques> Contracheques { get; set; }

    }

    public class ProfissionalModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int SaldoPeriodo { get; set; }
        public int IdPeriodoAquisitivo { get; set; }
        public string Tipo { get; set; }
    }

    public class ColaboradoresSolicitacoesController : Controller
    {
        // GET: Colaboradoressolicitacoesferias
        public ActionResult Index()
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var profissional = ProfissionaisBo.Buscar(Sessao.Usuario.ID);
                var solicitacoesferias = ProfissionaisSolicitacoesBo.ListarSolicitacoes(profissional.ID);
                var periodoaquisitivo = ProfissionaisPeriodosAquisitivoBo.BuscarPeriodo(profissional.ID);
                var documentos = ProfissionaisDocumentosBo.ListarDocumentos(profissional.ID);
                var ausencias = ProfissionaisAusenciasBo.ListarAusencias(profissional.ID);
                var solicitacosespeciais = ProfissionaisSolicitacoesEspeciaisBo.ListarSolicitacoesEspeciais(profissional.ID);
                var contracheques = ProfissionaisContrachequesBo.ListarContracheques(profissional.ID);

                //ListarColModel model = new ListarColModel();
                //model.Profissional = new ProfissionalModel();
                //model.Profissional.Id = profissional.ID;
                //model.Profissional.Nome = profissional.Pessoas.Nome;

                ListarColModel model = new ListarColModel()
                {
                    Profissional = new ProfissionalModel
                    {
                        Id = profissional.ID,
                        Nome = profissional.Pessoas.Nome,
                        SaldoPeriodo = (periodoaquisitivo != null ? int.Parse((periodoaquisitivo.Dias - periodoaquisitivo.DiasGozados).ToString()) : 0),
                        IdPeriodoAquisitivo = (periodoaquisitivo != null ? periodoaquisitivo.Id : 0),
                        Tipo = (profissional != null && profissional.IDTipoProfissional == 6 ? "Advogado" : "Colaborador")
                    },
                    SolicitacoesFerias = solicitacoesferias,
                    Documentos = documentos,
                    Ausencias = ausencias,
                    SolicitacosEspeciais = solicitacosespeciais,
                    Contracheques = contracheques
                };

                return View(model);
            }
        }

        public JsonResult Consultar(Int32 Id)
        {
            var colaboradorsolicitacao = ProfissionaisSolicitacoesBo.ConsultarArray(Id);
            var result = new { codigo = "00", colaboradorsolicitacao = colaboradorsolicitacao };
            return Json(result);
        }

        [HttpPost]
        public JsonResult SalvarSolicitacaoFerias(Int32 Id, Int32 IdProfissional, Int32 IdContrato, String DataSolicitacao, String DataAprovacao, String Status, String DataInicio, String Saldo, String Dias, String Justificativa, String Especial,
                                                  Int32 IdPeriodoAquisitivo)
        {
            var mensagem = string.Empty;
            var codigo = "00";

            if (int.Parse(Dias) > int.Parse(Saldo) && Especial == "N")
            {
                mensagem = "Número de dias solicitados maior que o saldo. Para continuar com a solicitação, selecione Sim em solicitação especial";
            }
            else
            {

                ProfissionaisSolicitacoes solicitacao = new ProfissionaisSolicitacoes();
                solicitacao.Id = Id;

                solicitacao.IdProfissional = IdProfissional;

                if (IdContrato > 0)
                {
                    solicitacao.IdContrato = IdContrato;
                }
                else
                {
                    solicitacao.IdContrato = ProfissionaisContratosBo.ConsultarContrato().Id;
                }

                if (DataSolicitacao != null && DataSolicitacao.Trim().Length > 0)
                {
                    solicitacao.DataSolicitacao = DateTime.Parse(DataSolicitacao);
                }

                if (DataAprovacao != null && DataAprovacao.Trim().Length > 0)
                {
                    solicitacao.DataAprovacao = DateTime.Parse(DataAprovacao);
                }

                solicitacao.Status = Status.Trim();

                if (DataInicio != null && DataInicio.Trim().Length > 0)
                {
                    solicitacao.DataInicio = DateTime.Parse(DataInicio);
                }

                if (Saldo != null && Saldo.Trim().Length > 0)
                {
                    solicitacao.Saldo = int.Parse(Saldo);
                }

                if (Dias != null && Dias.Trim().Length > 0)
                {
                    solicitacao.Dias = int.Parse(Dias);
                }

                solicitacao.Justificativa = Justificativa.Trim();
                solicitacao.IdPeriodoAquisitivo = IdPeriodoAquisitivo;

                solicitacao.SolicitacaoEspecial = (Especial == "S" ? true : false);

                solicitacao.DataCadastro = DateTime.Now;
                solicitacao.UsuarioCadastro = Sessao.Usuario.Nome;
                solicitacao.DataAlteracao = DateTime.Now;
                solicitacao.UsuarioAlteracao = Sessao.Usuario.Nome;

                if (Id == 0)
                {
                    Id = ProfissionaisSolicitacoesBo.Inserir(solicitacao);
                }

                solicitacao.Id = Id;
                ProfissionaisSolicitacoesBo.Salvar(solicitacao);

                var nomeProfissional = ProfissionaisBo.Consultar(IdProfissional);
                var titulo = "Solicitação de Férias/Recesso de: ";
                var mensagemEmail = "Solicitado dia " + DataSolicitacao + ", " + Dias + " dias para inicio dia " + DataInicio;
                var resposta = EnviarEmail(titulo, nomeProfissional.Pessoas.Nome, mensagemEmail);
                mensagem = "Solicitação Salva com sucesso - " + resposta;
            }

            var result = new { codigo = codigo, mensagem = mensagem };
            return Json(result);
        }

        public JsonResult ExcluirSolicitacaoFerias(Int32 Id)
        {
            var codigo = ProfissionaisSolicitacoesBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }


        public JsonResult ConsultarDocumento(Int32 Id)
        {
            var colaboradordocumento = ProfissionaisDocumentosBo.ConsultarArray(Id);

            var arquivo = "Nenhum arquivo selecionado";
            var file = Path.Combine(Server.MapPath("~/RH/Documentos"), String.Format("{0}.pdf", "Documento-" + Id.ToString()));
            if (System.IO.File.Exists(file))
            {
                arquivo = "<a style='cursor: pointer;' onclick='VisualizarPDF(" + '"' + String.Format("/RH/Documentos/{0}.pdf", "Documento-" + Id.ToString()) + '"' + ");'>" + String.Format("{0}.pdf", "Documento-" + Id.ToString()) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;<a id='ButtonDocumentoPDFExcluir' class='btn btn-danger btn-xs' onclick='ExcluirPDF()'>Apagar</a>&nbsp;&nbsp;<label id='LabelDocumentoFileExcluirConfirmacao' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse PDF?&nbsp;&nbsp;&nbsp;</label><a id='acceptFile' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirPDFSim(" + '"' + String.Format("{0}.pdf", "Documento-" + Id.ToString()) + '"' + ")'>Sim</a><a id='go_backFile' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirPDFNao()'>Não</a>";
            }
            var result = new { codigo = "00", colaboradordocumento = colaboradordocumento, arquivo = arquivo };

            //            var result = new { codigo = "00", colaboradordocumento = colaboradordocumento };
            return Json(result);
        }

        [HttpPost]
        public JsonResult SalvarDocumentos(Int32 Id, Int32 IdProfissional, String TipoDocumento, String NumeroDocumento, String DataEmissao, String DataValidade, String Observacao, HttpPostedFileBase DocumentoArquivo)
        {

            var mensagemEmail = String.Empty;
            var mensagem = string.Empty;
            ProfissionaisDocumentos documento = new ProfissionaisDocumentos();
            documento.Id = Id;
            documento.IdProfissional = IdProfissional;

            documento.TipoDocumento = TipoDocumento;
            documento.NumeroDocumento = NumeroDocumento;

            if (DataEmissao != null && DataEmissao.Trim().Length > 0)
            {
                documento.DataEmissao = DateTime.Parse(DataEmissao);
            }

            if (DataValidade != null && DataValidade.Trim().Length > 0)
            {
                documento.DataValidade = DateTime.Parse(DataValidade);
            }

            documento.Observacao = Observacao.Trim();


            documento.DataCadastro = DateTime.Now;
            documento.UsuarioCadastro = Sessao.Usuario.Nome;
            documento.DataAlteracao = DateTime.Now;
            documento.UsuarioAlteracao = Sessao.Usuario.Nome;
            if (Id == 0)
            {
                Id = ProfissionaisDocumentosBo.Inserir(documento);
            }

            documento.Id = Id;
            ProfissionaisDocumentosBo.Salvar(documento);

            var url = String.Empty;
            if (DocumentoArquivo != null && DocumentoArquivo.ContentLength > 0)
            {
                var file = Path.Combine(Server.MapPath("~/RH/Documentos"), String.Format("Documento-{0}.pdf", Id.ToString()));
                DocumentoArquivo.SaveAs(file);
                mensagemEmail = " com Documento anexado";
            }

            var nomeProfissional = ProfissionaisBo.Consultar(IdProfissional);
            var titulo = nomeProfissional.Pessoas.Nome + "Incluiu um documento";
            mensagemEmail = "Documento " + TipoDocumento + " incluído " + mensagemEmail;
            var resposta = EnviarEmail(titulo, nomeProfissional.Pessoas.Nome, mensagemEmail);
            mensagem = "Documento Salvo com sucesso - " + resposta;

            var result = new { codigo = "00", mensagem = mensagem };

            return Json(result);
        }

        public JsonResult ExcluirDocumentos(Int32 Id)
        {
            var codigo = ProfissionaisSolicitacoesBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult ExcluirPDF(String Arquivo)
        {
            try
            {
                var file = Path.Combine(Server.MapPath("~/RH/Documentos"), Arquivo);
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

        public JsonResult ConsultarAusencia(Int32 Id)
        {
            var colaboradorausencia = ProfissionaisAusenciasBo.ConsultarArray(Id);

            var arquivo = "Nenhum arquivo selecionado";
            var file = Path.Combine(Server.MapPath("~/RH/Atestados"), String.Format("{0}.pdf", "Atestado-" + Id.ToString()));
            if (System.IO.File.Exists(file))
            {
                arquivo = "<a style='cursor: pointer;' onclick='VisualizarPDF(" + '"' + String.Format("/RH/Atestados/{0}.pdf", "Atestado-" + Id.ToString()) + '"' + ");'>" + String.Format("{0}.pdf", "Atestado-" + Id.ToString()) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;<a id='ButtonAtestadoPDFExcluir' class='btn btn-danger btn-xs' onclick='ExcluirPDFAtestado()'>Apagar</a>&nbsp;&nbsp;<label id='LabelAtestadoFileExcluirConfirmacao' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse PDF?&nbsp;&nbsp;&nbsp;</label><a id='acceptFile' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirPDFAtestadoSim(" + '"' + String.Format("{0}.pdf", "Atestado-" + Id.ToString()) + '"' + ")'>Sim</a><a id='go_backFile' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirPDFAtestadoNao()'>Não</a>";
            }
            var result = new { codigo = "00", colaboradorausencia = colaboradorausencia, arquivo = arquivo };

            return Json(result);
        }

        [HttpPost]
        public JsonResult SalvarAusencia(Int32 Id, Int32 IdProfissional, String DataInicio, String DataFim, String Abonado, String MotivoAusencia, HttpPostedFileBase AtestadoArquivo)
        {
            var mensagemEmail = String.Empty;
            var mensagem = string.Empty;
            ProfissionaisAusencias ausencia = new ProfissionaisAusencias();
            ausencia.Id = Id;
            ausencia.IdProfissional = IdProfissional;

            if (DataInicio != null && DataInicio.Trim().Length > 0)
            {
                ausencia.DataInicio = DateTime.Parse(DataInicio);
            }

            if (DataFim != null && DataFim.Trim().Length > 0)
            {
                ausencia.DataFim = DateTime.Parse(DataFim);
            }

            ausencia.MotivoAusencia = MotivoAusencia.Trim();

            ausencia.DataCadastro = DateTime.Now;
            ausencia.UsuarioCadastro = Sessao.Usuario.Nome;
            ausencia.DataAlteracao = DateTime.Now;
            ausencia.UsuarioAlteracao = Sessao.Usuario.Nome;
            if (Id == 0)
            {
                Id = ProfissionaisAusenciasBo.Inserir(ausencia);
            }

            ausencia.Id = Id;
            ProfissionaisAusenciasBo.Salvar(ausencia);
            
            var url = String.Empty;
            if (AtestadoArquivo != null && AtestadoArquivo.ContentLength > 0)
            {
                var file = Path.Combine(Server.MapPath("~/RH/Atestados"), String.Format("Atestado-{0}.pdf", Id.ToString()));
                AtestadoArquivo.SaveAs(file);
                mensagemEmail = " com Atestado anexado";
            }

            var nomeProfissional = ProfissionaisBo.Consultar(IdProfissional);
            var titulo = nomeProfissional.Pessoas.Nome + "registrou ausencia";
            mensagemEmail = "Ausência registrada no periodo de " + DataInicio + " à " + DataFim + mensagemEmail;
            var resposta = EnviarEmail(titulo, nomeProfissional.Pessoas.Nome, mensagemEmail);
            mensagem = "Ausência Salva com sucesso - " + resposta;

            var result = new { codigo = "00", mensagem = mensagem };
            return Json(result);
        }

        [HttpPost]
        public JsonResult ExcluirAusencias(Int32 Id)
        {
            var codigo = ProfissionaisSolicitacoesBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult ExcluirPDFAtestado(String Arquivo)
        {
            try
            {
                var file = Path.Combine(Server.MapPath("~/RH/Atestados"), Arquivo);
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

        public JsonResult ConsultarSolicitacaoEspecial(Int32 Id)
        {
            var colaboradorsolicitacaoespecial = ProfissionaisSolicitacoesEspeciaisBo.ConsultarArray(Id);

            var result = new { codigo = "00", colaboradorsolicitacaoespecial = colaboradorsolicitacaoespecial };

            return Json(result);
        }

        [HttpPost]
        public JsonResult SalvarSolicitacaoEspecial(Int64 Id, Int32 IdProfissional, String Data, String DataAprovacao, String Status, String Solicitacao, String Justificativa)
        {

            var mensagemEmail = String.Empty;
            var mensagem = string.Empty;

            ProfissionaisSolicitacoesEspeciais solicitacaoespecial = new ProfissionaisSolicitacoesEspeciais();
            solicitacaoespecial.Id = Id;
            solicitacaoespecial.IdProfissional = IdProfissional;

            if (Data != null && Data.Trim().Length > 0)
            {
                solicitacaoespecial.Data = DateTime.Parse(Data);
            }

            if (DataAprovacao != null && DataAprovacao.Trim().Length > 0)
            {
                solicitacaoespecial.DataAprovacao = DateTime.Parse(DataAprovacao);
            }

            solicitacaoespecial.Status = Status.Trim();
            solicitacaoespecial.Solicitacao = Solicitacao.Trim();
            solicitacaoespecial.Justificativa = Justificativa.Trim();

            solicitacaoespecial.DataCadastro = DateTime.Now;
            solicitacaoespecial.UsuarioCadastro = Sessao.Usuario.Nome;
            solicitacaoespecial.DataAlteracao = DateTime.Now;
            solicitacaoespecial.UsuarioAlteracao = Sessao.Usuario.Nome;
            if (Id == 0)
            {
                Id = ProfissionaisSolicitacoesEspeciaisBo.Inserir(solicitacaoespecial);
            }

            solicitacaoespecial.Id = Id;
            ProfissionaisSolicitacoesEspeciaisBo.Salvar(solicitacaoespecial);
             
            var nomeProfissional = ProfissionaisBo.Consultar(IdProfissional);
            var titulo = nomeProfissional.Pessoas.Nome + "registrou ausencia";
            mensagemEmail = "Solicitação especial: " + Solicitacao + " registrada no dia " + Data;
            var resposta = EnviarEmail(titulo, nomeProfissional.Pessoas.Nome, mensagemEmail);
            mensagem = "Ausência Salva com sucesso - " + resposta;

            var result = new { codigo = "00", mensagem = mensagem };

            return Json(result);
        }

        public JsonResult ExcluirSolicitacaoEspecial(Int64 Id)
        {
            var codigo = ProfissionaisSolicitacoesEspeciaisBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        public JsonResult ExcluirContracheque(Int32 Id)
        {
            var arquivo = ProfissionaisContrachequesBo.Consultar(Id).NomeArquivo;
            ExcluirPDFContracheque(arquivo);
            var codigo = ProfissionaisContrachequesBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult ExcluirPDFContracheque(String Arquivo)
        {
            try
            {
                var file = Path.Combine(Server.MapPath("~/RH/Contracheques"), Arquivo);
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

        public ActionResult DownloadContracheque(Int32 Id)
        {
            // Obtém o nome do arquivo a partir da consulta ao banco de dados.
            var arquivo = ProfissionaisContrachequesBo.Consultar(Id).NomeArquivo;
            var caminhoArquivo = Path.Combine(Server.MapPath("~/RH/Contracheques"), arquivo);

            // Verifica se o arquivo existe no servidor.
            //if (!System.IO.File.Exists(caminhoArquivo))
            //{
            //    return HttpNotFound("Arquivo não encontrado.");
            //}

            // Retorna o arquivo para download com o nome original.
            byte[] fileBytes = System.IO.File.ReadAllBytes(caminhoArquivo);
            string nomeArquivo = Path.GetFileName(caminhoArquivo);

            string base64String = Convert.ToBase64String(fileBytes);

            var doc = new { codigo = "00", documento = base64String, nomearquivo = nomeArquivo };


            JsonResult result = Json(doc);
            result.MaxJsonLength = 2147483644;

            return result;

        }

        public static string EnviarEmail(String Titulo, String NomeProfissional, String mensagemEmail)
        {
            var motivo = string.Empty;

            var empresa = EmpresaBo.Buscar(1);

            var menssage = string.Empty;
            var to = string.Empty;
            var name = NomeProfissional;
            var emailremetente = empresa.Email;
            var passwordremetente = empresa.Senha;
            Titulo = Titulo + NomeProfissional;

            var email = UsuariosBo.Buscar(Sessao.Usuario.ID);

            to = "rh@rvmadvogados.com.br";

            menssage += "<!DOCTYPE html>";
            menssage += "<html>";
            menssage += "  <head>";
            menssage += "    <style>";
            menssage += "      #customers {";
            menssage += "        font-family: 'Trebuchet MS', Arial, Helvetica, sans-serif;";
            menssage += "        border - collapse: collapse;";
            menssage += "        width: 100 %;";
            menssage += "      }";
            menssage += "      #customers td, #customers th {";
            menssage += "        border: 1px solid #DDDDDD;";
            menssage += "        padding: 8px;";
            menssage += "      }";
            menssage += "      #customers tr:nth-child(even){ background-color: #EDEDED; }";
            menssage += "      #customers tr:hover { background-color: #EED58F; }";
            menssage += "      #customers th {";
            menssage += "        padding-top: 12px;";
            menssage += "        padding-bottom: 12px;";
            menssage += "        text-align: left;";
            menssage += "        background-color: #ECBD4F;";
            menssage += "        color: #333333;";
            menssage += "        border: 1px solid black";
            menssage += "        border-collapse: collapse";
            menssage += "      }";
            menssage += "    </style>";
            menssage += "  </head>";
            menssage += "  <body>";
            menssage += "    <h3>";
            menssage += "      Solicitado por: " + NomeProfissional;
            menssage += "    </h3>";
            menssage += "    <table id='customers'>";
            menssage += "      <tr>";
            menssage += "        <td>" + mensagemEmail;
            menssage += "      </tr>";

            if (!string.IsNullOrEmpty(menssage))
            {
                menssage += "    </table>";
                menssage += "  </body>";
                menssage += "</html>";

                if (!Mail.Send(emailremetente, passwordremetente, to, Titulo, menssage))
                {
                    motivo = "Ocorreu uma falha ao enviar os E-mails. Tente novamente mais tarde ou entre em contato com o administrador do sistema.";
                }
                else
                {
                    motivo = "Email enviado com sucesso";
                }
            }


            return motivo;
        }

    }

}