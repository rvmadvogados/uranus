using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using Uranus.Business;
using Uranus.Common;
using Uranus.Domain;
using Uranus.Domain.Entities;

namespace Uranus.Suite.Controllers
{
    public class ProcessosController : Controller, IRequiresSessionState
    {
        public ActionResult GetProcessList(string search, bool filter = false, int page = 1)
        {
            search = Util.OnlyAlphaNumeric(search);
            var acoes = ProcessosAcoesBo.GetProcessList(search, filter);
            var total = acoes.Count();

            if (!(string.IsNullOrEmpty(search) || string.IsNullOrWhiteSpace(search)))
            {
                acoes = acoes.Where(x => x.text.ToLower().StartsWith(search.ToLower())).Take(page * 10).ToList();
            }

            return Json(new { acoes = acoes, total = total }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index(string FiltrarProcesso = "", string FiltrarCliente = "", string FiltrarArea = "", string FiltrarStatus = "", string FiltrarVara = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                string url = HttpContext.Request.Url.AbsoluteUri;
                var uri = new Uri(url);
                string parameter = uri.Query.Replace("&amp;", "&").Replace("&amp%3b", "&").Replace("%2c3", string.Empty);
                Sessao.URLParameters = parameter;

                if (!String.IsNullOrEmpty(FiltrarProcesso) ||
                    !String.IsNullOrEmpty(FiltrarCliente) ||
                    !String.IsNullOrEmpty(FiltrarArea) ||
                    !String.IsNullOrEmpty(FiltrarStatus) ||
                    !String.IsNullOrEmpty(FiltrarVara))
                {
                    Sessao.ProcessNumber = FiltrarProcesso;
                    Sessao.ClientName = FiltrarCliente;
                    Sessao.AreaType = FiltrarArea;
                    Sessao.ProcessStatus = FiltrarStatus;
                    Sessao.Judgment = FiltrarVara;
                }
                else
                {
                    if (!string.IsNullOrEmpty(parameter))
                    {
                        Sessao.ProcessNumber = HttpUtility.ParseQueryString(parameter).Get("FiltrarProcesso") ?? string.Empty;
                        Sessao.ClientName = HttpUtility.ParseQueryString(parameter).Get("FiltrarCliente") ?? string.Empty;
                        Sessao.AreaType = HttpUtility.ParseQueryString(parameter).Get("FiltrarArea") ?? string.Empty;
                        Sessao.ProcessStatus = HttpUtility.ParseQueryString(parameter).Get("FiltrarStatus") ?? string.Empty;
                        Sessao.Judgment = HttpUtility.ParseQueryString(parameter).Get("FiltrarVara") ?? string.Empty;
                    }
                }

                var vara = string.Empty;

                if (FiltrarVara.Length > 0)
                {
                    var tamanho = Sessao.Judgment.LastIndexOf('(');

                    if (tamanho < 0)
                    {
                        tamanho = Sessao.Judgment.Length;
                    }

                    vara = Sessao.Judgment.Substring(0, tamanho).Trim();
                }

                JsonResult model = Json(ProcessosBo.Listar(Util.OnlyAlphaNumeric(Sessao.ProcessNumber), Sessao.ClientName, Sessao.AreaType, Sessao.ProcessStatus, vara));

                model.MaxJsonLength = 2147483644;

                ViewBag.FiltrarProcesso = Sessao.ProcessNumber;
                ViewBag.FiltrarCliente = Sessao.ClientName;
                ViewBag.FiltrarArea = Sessao.AreaType;
                ViewBag.FiltrarStatus = Sessao.ProcessStatus;
                ViewBag.FiltrarVara = Sessao.Judgment;

                return View(model);
            }
        }

        [HttpPost]
        public JsonResult LimparFiltroSessao()
        {
            Sessao.ProcessRowIndex = String.Empty;
            Sessao.ProcessNumber = String.Empty;
            Sessao.ClientName = String.Empty;
            Sessao.AreaType = String.Empty;
            Sessao.ProcessStatus = String.Empty;
            Sessao.Judgment = String.Empty;
            Sessao.URLParameters = String.Empty;

            var result = new { response = "success" };
            return Json(result);
        }

        public PartialViewResult View(Int32 Id)
        {
            var model = ProcessosBo.Buscar(Id);

            JsonResult result = Json(model);
            result.MaxJsonLength = 2147483644;

            return PartialView(result);
        }

        public ActionResult CapturarEProc(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = ProcessosEventosPendentesBo.BuscarEProc(Util.OnlyNumbers(search));

                ViewBag.search = search;
                return View(model);
            }
        }

        public ActionResult CapturarWebJur(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = ProcessosEventosPendentesBo.BuscarWebJur(Util.OnlyNumbers(search));

                ViewBag.search = search;
                return View(model);
            }
        }

        public ActionResult CapturarOab(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = ProcessosEventosPendentesBo.BuscarOAB(Util.OnlyNumbers(search));

                ViewBag.search = search;
                return View(model);
            }
        }

        #region Processos
        public ActionResult Agendar(string FiltrarCliente = "", string FiltrarProcesso = "", string FiltrarPrazoEvento1 = "", string FiltrarPrazoEvento2 = "", string FiltrarEvento = "", string FiltrarProfissional = "", string FiltrarCadastro = "", string FiltrarVara = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var idProfissional = 0;
                if ((Sessao.Usuario.Nivel != 5 && Sessao.Usuario.Nivel != 4) || Sessao.Usuario.Nivel == 0)
                {
                    FiltrarProfissional = Sessao.Usuario.Profissionais.FirstOrDefault()?.Pessoas.Nome ?? String.Empty;
                    idProfissional = ProfissionaisBo.Buscar(Sessao.Usuario.ID).ID;
                }

                var vara = string.Empty;

                if (FiltrarVara.Length > 0)
                {
                    var tamanho = FiltrarVara.LastIndexOf('(');

                    if (tamanho < 0)
                    {
                        tamanho = FiltrarVara.Length;
                    }

                    vara = FiltrarVara.Substring(0, tamanho).Trim();
                }

                var model = ProcessosAgendasProfissionaisBo.Listar(FiltrarCliente, Util.OnlyAlphaNumeric(FiltrarProcesso), FiltrarPrazoEvento1, FiltrarPrazoEvento2, FiltrarEvento, FiltrarProfissional, FiltrarCadastro, vara, idProfissional);

                ViewBag.FiltrarCliente = FiltrarCliente;
                ViewBag.FiltrarProcesso = FiltrarProcesso;
                ViewBag.FiltrarPrazoEvento1 = FiltrarPrazoEvento1;
                ViewBag.FiltrarPrazoEvento2 = FiltrarPrazoEvento2;
                ViewBag.FiltrarEvento = FiltrarEvento;
                ViewBag.FiltrarProfissional = FiltrarProfissional;
                ViewBag.FiltrarCadastro = FiltrarCadastro;
                ViewBag.FiltrarVara = FiltrarVara;
                return View(model);
            }
        }

        public ActionResult Enviar()
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Processar()
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public JsonResult Processamento()
        {
            string path = ConfigurationManager.AppSettings["PathUpload"];

            string targetNew = String.Format(@"{0}\{1}", path, "Novas");
            string targetProcessing = String.Format(@"{0}\{1}", path, "Processando");
            string targetFinished = String.Format(@"{0}\{1}", path, "Finalizadas");
            string targetError = String.Format(@"{0}\{1}", path, "Erradas");

            var processamento = string.Empty;

            foreach (var file in Directory.GetFiles(targetNew, "*", SearchOption.AllDirectories))
            {
                FileInfo fi = new FileInfo(file);
                processamento += "                <tr>";
                processamento += "                    <th scope='row'>" + Path.GetFileName(file) + "</th>";
                processamento += "                    <td>" + fi.LastWriteTime.ToString("dd/MM/yyyy HH:mm:ss") + "</td>";
                processamento += "                    <td style='color: blue !important;'><b>Nova</b></td>";
                processamento += "                </tr>";
            }

            foreach (var file in Directory.GetFiles(targetProcessing, "*", SearchOption.AllDirectories))
            {
                FileInfo fi = new FileInfo(file);
                processamento += "                <tr>";
                processamento += "                    <th scope='row'>" + Path.GetFileName(file) + "</th>";
                processamento += "                    <td>" + fi.LastWriteTime.ToString("dd/MM/yyyy HH:mm:ss") + "</td>";
                processamento += "                    <td style='color: orange !important;'><b>Processando</b></td>";
                processamento += "                </tr>";
            }

            foreach (var file in Directory.GetFiles(targetFinished, "*", SearchOption.AllDirectories))
            {
                FileInfo fi = new FileInfo(file);
                processamento += "                <tr>";
                processamento += "                    <th scope='row'>" + Path.GetFileName(file) + "</th>";
                processamento += "                    <td>" + fi.LastWriteTime.ToString("dd/MM/yyyy HH:mm:ss") + "</td>";
                processamento += "                    <td style='color: green !important;'><b>Finalizada</b></td>";
                processamento += "                </tr>";
            }

            foreach (var file in Directory.GetFiles(targetError, "*", SearchOption.AllDirectories))
            {
                FileInfo fi = new FileInfo(file);
                processamento += "                <tr>";
                processamento += "                    <th scope='row'>" + Path.GetFileName(file) + "</th>";
                processamento += "                    <td>" + fi.LastWriteTime.ToString("dd/MM/yyyy HH:mm:ss") + "</td>";
                processamento += "                    <td style='color: red !important;'><b>Errada</b></td>";
                processamento += "                </tr>";
            }

            var result = new { response = "success", processamento = processamento };
            return Json(result);
        }

        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase file)
        {
            try
            {
                var memStream = new MemoryStream();
                file.InputStream.CopyTo(memStream);

                byte[] fileData = memStream.ToArray();

                string path = ConfigurationManager.AppSettings["PathUpload"];

                System.IO.File.WriteAllBytes(string.Format(@"{0}\PLANILHA_E-PROC_{1}.xls", string.Concat(path, "\\Novas"), DateTime.Now.ToString("yyyyMMddHHmmssffff")), fileData);
            }
            catch (Exception exception)
            {
                return Json(new
                {
                    success = false,
                    response = "Ocorreu uma falha no envio dessa planilha do E-Proc."
                });
            }

            return Json(new
            {
                success = true,
                response = "Sucesso ao enviar esta planilha do E-Proc."
            });
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var processo = ProcessosBo.ConsultarArray(Id);
            var result = new { codigo = "00", processo = processo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, Int32? IdProfissional, String Status, String Objeto, String Observacao)
        {
            #region Auditoria
            Processos processo = ProcessosBo.Buscar(Id);
            Profissionais profissional = ProfissionaisBo.Consultar((IdProfissional != null ? IdProfissional.Value : 0));

            Auditoria auditoria = new Auditoria();
            auditoria.DataHora = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            auditoria.Modulo = "Processo";
            auditoria.Tipo = "Inicial";
            auditoria.Acao = "Alterado";
            auditoria.Log = String.Format("<b>Sede</b>: {0};<b>Profissional</b>: {1};<b>Status</b>: {2};<b>Objeto</b>: {3};<b>Observação</b>: {4};", (processo != null && processo?.ProcessosAutores.Count > 0 ? (processo?.ProcessosAutores.FirstOrDefault().Clientes?.Sedes?.Nome ?? string.Empty) : string.Empty), (profissional?.Pessoas?.Nome ?? String.Empty), Status, Objeto, Observacao);
            auditoria.Usuario = Sessao.Usuario.Nome;

            if (Id == 0)
            {
                auditoria.Acao = "Inserido";
            }

            AuditoriaBo.Inserir(auditoria);
            #endregion

            processo = new Processos();
            processo.ID = Id;

            if (IdProfissional != null)
            {
                processo.IdProfissionalResponsavel = IdProfissional.Value;
            }

            processo.Status = Status;
            processo.Objeto = Objeto.Trim();
            processo.Observacao = Observacao.Trim();
            processo.DataInclusao = DateTime.Now;

            if (Id == 0)
            {
                Id = ProcessosBo.Inserir(processo);
            }

            processo.ID = Id;
            ProcessosBo.Salvar(processo);

            var result = new { codigo = "00", id = Id };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            #region Auditoria
            Processos processo = ProcessosBo.Buscar(Id);
            //aqui ruy
            Auditoria auditoria = new Auditoria();
            auditoria.DataHora = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            auditoria.Modulo = "Processo";
            auditoria.Tipo = "Inicial";
            auditoria.Acao = "Excluído";
            auditoria.Log = String.Format("<b>Sede</b>: {0};<b>Profissional</b>: {1};<b>Status</b>: {2};<b>Objeto</b>: {3};<b>Observação</b>: {4};", (processo.ProcessosAutores?.FirstOrDefault()?.Clientes?.Sedes?.Nome ?? string.Empty), processo.Profissionais.Pessoas.Nome, processo.Status, (processo?.Objeto ?? string.Empty), (processo?.Observacao ?? string.Empty));
            auditoria.Usuario = Sessao.Usuario.Nome;

            AuditoriaBo.Inserir(auditoria);
            #endregion

            var codigo = ProcessosBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar(Int32 IdProcesso)
        {
            JsonResult processos = Json(ProcessosAcoesBo.Listar(IdProcesso));
            processos.MaxJsonLength = 2147483644;

            var result = new { codigo = "00", processos = processos };
            return Json(result);
        }

        [HttpPost]
        public JsonResult ListarAcoes(Int32 IdProcesso)
        {
            var idProcesso = ProcessosAcoesBo.Consultar(IdProcesso).IdProcesso;
            JsonResult processos = Json(ProcessosAcoesBo.Listar(long.Parse(idProcesso.ToString())));
            processos.MaxJsonLength = 2147483644;

            var result = new { codigo = "00", processos = processos };
            return Json(result);
        }

        [HttpPost]
        public JsonResult BuscarNumeroProcesso()
        {
            JsonResult processos = Json(ProcessosAcoesBo.ListarArray());
            processos.MaxJsonLength = 2147483644;

            var result = new { codigo = "00", processos = processos };
            return Json(result);
        }

        [HttpPost]
        public JsonResult ConsultarProcessoEvento(Int32 Id)
        {
            var evento = ProcessosEventosBo.ConsultarEventos(Id);

            ProcessosAgendaProfissional agenda = ProcessosAgendasProfissionaisBo.Consultar(Id);

            if (agenda != null)
            {
                evento[0].IdAgendaProfissional = agenda.ID;
                evento[0].IdProfissional = agenda.IdProfissional;
            }
            else
            {
                evento[0].IdAgendaProfissional = 0;
            }
            var arquivo1 = "Nenhum arquivo encontrado";
            var file1 = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-1.pdf", Util.OnlyNumbers(evento[0].NumeroProcesso), evento[0].Id.ToString()));
            if (System.IO.File.Exists(file1))
            {
                arquivo1 = "<a style='cursor: pointer;' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-1.pdf", Util.OnlyNumbers(evento[0].NumeroProcesso), evento[0].Id.ToString()) + '"' + ");'>" + String.Format("{0}-{1}-1.pdf", Util.OnlyNumbers(evento[0].NumeroProcesso), evento[0].Id.ToString()) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;<a id='" + String.Format("ButtonProcessoEventoPDF{0}Excluir1", "") + "' class='btn btn-danger btn-xs' onclick='ExcluirPDF(1, " + '"' + '"' + ")'>Apagar</a>&nbsp;&nbsp;<label id='" + String.Format("LabelProcessoEventoFile{0}ExcluirConfirmacao1", "") + "' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse PDF?&nbsp;&nbsp;&nbsp;</label><a id='" + String.Format("accept1File{0}", "") + "' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirPDFSim(1, " + '"' + '"' + ", " + '"' + String.Format("{0}-{1}-1.pdf", Util.OnlyNumbers(evento[0].NumeroProcesso), evento[0].Id.ToString()) + '"' + ")'>Sim</a><a id='" + String.Format("go_back1File{0}", "") + "' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirPDFNao(1, " + '"' + '"' + ")''>Não</a>";
            }

            var arquivo2 = "Nenhum arquivo encontrado";
            var file2 = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-2.pdf", Util.OnlyNumbers(evento[0].NumeroProcesso), evento[0].Id.ToString()));
            if (System.IO.File.Exists(file2))
            {
                arquivo2 = "<a style='cursor: pointer;' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-2.pdf", Util.OnlyNumbers(evento[0].NumeroProcesso), evento[0].Id.ToString()) + '"' + ");'>" + String.Format("{0}-{1}-2.pdf", Util.OnlyNumbers(evento[0].NumeroProcesso), evento[0].Id.ToString()) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;<a id='" + String.Format("ButtonProcessoEventoPDF{0}Excluir2", "") + "' class='btn btn-danger btn-xs' onclick='ExcluirPDF(2, " + '"' + '"' + ")'>Apagar</a>&nbsp;&nbsp;<label id='" + String.Format("LabelProcessoEventoFile{0}ExcluirConfirmacao2", "") + "' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse PDF?&nbsp;&nbsp;&nbsp;</label><a id='" + String.Format("accept2File{0}", "") + "' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirPDFSim(2, " + '"' + '"' + ", " + '"' + String.Format("{0}-{1}-2.pdf", Util.OnlyNumbers(evento[0].NumeroProcesso), evento[0].Id.ToString()) + '"' + ")'>Sim</a><a id='" + String.Format("go_back2File{0}", "") + "' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirPDFNao(2, " + '"' + '"' + ")''>Não</a>";
            }

            var arquivo3 = "Nenhum arquivo encontrado";
            var file3 = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-3.pdf", Util.OnlyNumbers(evento[0].NumeroProcesso), evento[0].Id.ToString()));
            if (System.IO.File.Exists(file3))
            {
                arquivo3 = "<a style='cursor: pointer;' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-3.pdf", Util.OnlyNumbers(evento[0].NumeroProcesso), evento[0].Id.ToString()) + '"' + ");'>" + String.Format("{0}-{1}-3.pdf", Util.OnlyNumbers(evento[0].NumeroProcesso), evento[0].Id.ToString()) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;<a id='" + String.Format("ButtonProcessoEventoPDF{0}Excluir3", "") + "' class='btn btn-danger btn-xs' onclick='ExcluirPDF(3, " + '"' + '"' + ")'>Apagar</a>&nbsp;&nbsp;<label id='" + String.Format("LabelProcessoEventoFile{0}ExcluirConfirmacao3", "") + "' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse PDF?&nbsp;&nbsp;&nbsp;</label><a id='" + String.Format("accept3File{0}", "") + "' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirPDFSim(3, " + '"' + '"' + ", " + '"' + String.Format("{0}-{1}-3.pdf", Util.OnlyNumbers(evento[0].NumeroProcesso), evento[0].Id.ToString()) + '"' + ")'>Sim</a><a id='" + String.Format("go_back3File{0}", "") + "' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirPDFNao(3, " + '"' + '"' + ")''>Não</a>";
            }

            var arquivo4 = "Nenhum arquivo encontrado";
            var file4 = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-4.pdf", Util.OnlyNumbers(evento[0].NumeroProcesso), evento[0].Id.ToString()));
            if (System.IO.File.Exists(file4))
            {
                arquivo4 = "<a style='cursor: pointer;' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-4.pdf", Util.OnlyNumbers(evento[0].NumeroProcesso), evento[0].Id.ToString()) + '"' + ");'>" + String.Format("{0}-{1}-4.pdf", Util.OnlyNumbers(evento[0].NumeroProcesso), evento[0].Id.ToString()) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;<a id='" + String.Format("ButtonProcessoEventoPDF{0}Excluir4", "") + "' class='btn btn-danger btn-xs' onclick='ExcluirPDF(4, " + '"' + '"' + ")'>Apagar</a>&nbsp;&nbsp;<label id='" + String.Format("LabelProcessoEventoFile{0}ExcluirConfirmacao4", "") + "' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse PDF?&nbsp;&nbsp;&nbsp;</label><a id='" + String.Format("accept4File{0}", "") + "' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirPDFSim(4, " + '"' + '"' + ", " + '"' + String.Format("{0}-{1}-4.pdf", Util.OnlyNumbers(evento[0].NumeroProcesso), evento[0].Id.ToString()) + '"' + ")'>Sim</a><a id='" + String.Format("go_back4File{0}", "") + "' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirPDFNao(4, " + '"' + '"' + ")''>Não</a>";
            }

            var arquivo5 = "Nenhum arquivo encontrado";
            var file5 = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-5.pdf", Util.OnlyNumbers(evento[0].NumeroProcesso), evento[0].Id.ToString()));
            if (System.IO.File.Exists(file5))
            {
                arquivo5 = "<a style='cursor: pointer;' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-5.pdf", Util.OnlyNumbers(evento[0].NumeroProcesso), evento[0].Id.ToString()) + '"' + ");'>" + String.Format("{0}-{1}-5.pdf", Util.OnlyNumbers(evento[0].NumeroProcesso), evento[0].Id.ToString()) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;<a id='" + String.Format("ButtonProcessoEventoPDF{0}Excluir5", "") + "' class='btn btn-danger btn-xs' onclick='ExcluirPDF(5, " + '"' + '"' + ")'>Apagar</a>&nbsp;&nbsp;<label id='" + String.Format("LabelProcessoEventoFile{0}ExcluirConfirmacao5", "") + "' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse PDF?&nbsp;&nbsp;&nbsp;</label><a id='" + String.Format("accept5File{0}", "") + "' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirPDFSim(5, " + '"' + '"' + ", " + '"' + String.Format("{0}-{1}-5.pdf", Util.OnlyNumbers(evento[0].NumeroProcesso), evento[0].Id.ToString()) + '"' + ")'>Sim</a><a id='" + String.Format("go_back5File{0}", "") + "' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirPDFNao(5, " + '"' + '"' + ")''>Não</a>";
            }

            var result = new { codigo = "00", evento = evento.ToArray(), arquivo1 = arquivo1, arquivo2 = arquivo2, arquivo3 = arquivo3, arquivo4 = arquivo4, arquivo5 = arquivo5 };
            return Json(result);
        }

        [HttpPost]
        public JsonResult CarregarProcessosAutores(Int64 IdProcesso)
        {
            try
            {
                var autor = ProcessosAutoresBo.Listar(IdProcesso);
                String autores = String.Empty;

                List<Customers> customers = new List<Customers>();
                Customers customer;

                int AutoresQuantidade = (autor.Count > 0 ? (autor.Count + 1) : 1);

                for (int i = 0; i < AutoresQuantidade; i++)
                {
                    customer = new Customers();
                    customer.IdCustomer = (autor.Count > 0 && i < autor.Count ? int.Parse(autor[i].IdCliente.ToString()) : 0);
                    customer.Customer = (autor.Count > 0 && i < autor.Count ? autor[i].Clientes.Pessoas.Nome : "");
                    customers.Add(customer);

                    autores += "<input id='" + String.Format("HiddenProcessoAutor{0}", (i + 1)) + "' name='HiddenProcessoAutor[]' type='hidden' value='" + (autor.Count >= (i + 1) ? autor[i].ID.ToString() : "0") + "' />";
                    autores += "<div class='accordion panel panel-default'>";
                    autores += "   <div class='accordion-heading panel-heading' id='" + String.Format("ProcessoAutor{0}heading", (i + 1)) + "' role='tab'>";
                    autores += "      <h4 class='accordion-title panel-title'>";
                    autores += "         <a href='" + String.Format("#ProcessoAutor{0}collapse", (i + 1)) + "' aria-expanded='false' data-toggle='collapse' aria-controls='" + String.Format("ProcessoAutor{0}collapse", (i + 1)) + "' data-parent='#ProcessoAutorAccordion' role='button' class='collapsed'>";
                    autores += "            " + String.Format("Autor #{0}", (i + 1));
                    autores += (autor.Count >= (i + 1) ? String.Format(" - {0}", autor[i].Clientes.Pessoas.Nome) : String.Empty);
                    autores += "         </a>";
                    autores += "      </h4>";
                    autores += "   </div>";
                    autores += "   <div class='panel-collapse collapse' id='" + String.Format("ProcessoAutor{0}collapse", (i + 1)) + "' role='tabpanel' aria-labelledby='" + String.Format("ProcessoAutor{0}heading", (i + 1)) + "' aria-expanded='false' style='height: 0px;'>";
                    autores += "      <div class='accordion-body panel-body'>";
                    autores += "         <div class='row'>";
                    autores += "            <div class='col-md-6'>";
                    autores += "               <div class='form-group'>";
                    autores += "                  <label class='titlefield'>Nome do Autor <span class='required'>*</span></label>";
                    autores += "                  <br />";
                    autores += "                  <select id='" + String.Format("ProcessoAutor{0}Cliente", (i + 1)) + "' name='ProcessoAutorCliente[]' class='select2_single form-control placeholder' data-msg-required='Por favor, selecione um Autor.' data-rule-required='true' style='width: 100% !important;'></select>";
                    autores += "                  <label id='" + String.Format("ProcessoAutor{0}ClienteValidate", (i + 1)) + "' name='ProcessoAutorClienteValidate[]' for='" + String.Format("ProcessoAutor{0}Cliente", (i + 1)) + "' class='error hidden'> Por favor, selecione um Autor.</label>";
                    autores += "               </div>";
                    autores += "            </div>";
                    autores += "         </div>";
                    autores += "         <div class='row'>";
                    autores += "            <div class='col-md-4'>";
                    autores += "               <button type='button' id='" + String.Format("ButtonProcessoAutor{0}Incluir", (i + 1)) + "' name='" + String.Format("ButtonProcessoAutor{0}Incluir", (i + 1)) + "' class='btn btn-primary' data-loading-text='Salvando...' onclick='ButtonProcessoAutorSalvar_Click(" + (i + 1) + ");'>";
                    autores += "                  Salvar";
                    autores += "               </button>";
                    autores += "            </div>";
                    autores += "            <div class='col-md-4' style='text-align: center;'>";
                    autores += "            </div>";
                    autores += "            <div class='col-md-4' style='text-align: right;'>";
                    autores += "               <button type='button' id='" + String.Format("ButtonProcessoAutor{0}Excluir", (i + 1)) + "' name='" + String.Format("ButtonProcessoAutor{0}Excluir", (i + 1)) + "' class='btn btn-danger' data-loading-text='Apagando...' onclick='ButtonProcessoAutorExcluir_Click(" + (i + 1) + ");' " + (autor.Count >= (i + 1) ? String.Empty : "disabled='disabled'") + ">";
                    autores += "                  Apagar";
                    autores += "               </button>";
                    autores += "               <div id='" + String.Format("ButtonProcessoAutor{0}ExcluirConfirmacao", (i + 1)) + "' style='display:none'>";
                    autores += "                  <label id='" + String.Format("LabelProcessoAutor{0}ExcluirConfirmacao", (i + 1)) + "' style='font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente excluir esse Autor?</label><br />";
                    autores += "                  <button id='" + String.Format("accept{0}", (i + 1)) + "' class='btn btn-default btn-xs' value='true'>Sim</button>";
                    autores += "                  <button id='" + String.Format("go_back{0}", (i + 1)) + "' class='btn btn-primary btn-xs' value='false'>Não</button>";
                    autores += "               </div>";
                    autores += "            </div>";
                    autores += "         </div>";
                    autores += "      </div>";
                    autores += "   </div>";
                    autores += "   <div class='alert alert-success hidden' id='processo-autor-alert-success-" + (i + 1) + "'>";
                    autores += "      <strong>SUCESSO:</strong> Cadastrado do autor realizado com sucesso.";
                    autores += "   </div>";
                    autores += "   <div class='alert alert-success hidden' id='processo-autor-alert-removed-" + (i + 1) + "'>";
                    autores += "      <strong>SUCESSO:</strong> Exclusão do autor realizado com sucesso.";
                    autores += "   </div>";
                    autores += "   <div class='alert alert-danger hidden' id='processo-autor-alert-error-" + (i + 1) + "'>";
                    autores += "      <strong>ERRO:</strong> Algo deu errado ao efetuar o cadastro do autor.";
                    autores += "   </div>";
                    autores += "   <div class='alert alert-danger hidden' id='processo-autor-alert-error2-" + (i + 1) + "'>";
                    autores += "      <strong>ERRO:</strong> Algo deu errado ao efetuar a exclusão do autor.";
                    autores += "   </div>";
                    autores += "</div>";
                }

                var result = new { response = "success", autores = autores, count = AutoresQuantidade, array = customers.ToArray() };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult SalvarProcessosAutores(Int32 Id, Int32 IdProcesso, Int32 IdCliente)
        {
            try
            {
                #region Processos
                Processos processo;

                var IdProcessoAux = IdProcesso;
                if (IdProcesso == 0)
                {
                    processo = new Processos();
                    processo.Status = "Ativo";
                    processo.DataInclusao = DateTime.Now;
                    IdProcesso = ProcessosBo.Inserir(processo);
                }

                processo = ProcessosBo.Buscar(IdProcesso);
                #endregion

                #region Clientes
                Clientes cliente = ClientesBo.Consultar(IdCliente);
                #endregion

                #region Auditoria
                Auditoria auditoria = new Auditoria();
                auditoria.DataHora = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                auditoria.Modulo = "Processo";
                auditoria.Tipo = "Autor";
                auditoria.Acao = "Alterado";
                auditoria.Log = String.Format("<b>Sede</b>: {0};<b>Profissional</b>: {1};<b>Status</b>: {2};<b>Cliente</b>: {3}", (processo != null && processo.ProcessosAutores.Count > 0 ? processo.ProcessosAutores.FirstOrDefault().Clientes.Sedes?.Nome : string.Empty), (processo != null ? processo.Profissionais?.Pessoas?.Nome : String.Empty), (processo != null ? processo.Status : String.Empty), cliente.Pessoas.Nome);
                auditoria.Usuario = Sessao.Usuario.Nome;

                if (Id == 0)
                {
                    auditoria.Acao = "Inserido";
                }

                AuditoriaBo.Inserir(auditoria);
                #endregion

                ProcessosAutores autor = new ProcessosAutores();
                autor.ID = Id;
                autor.IdProcesso = IdProcesso;
                autor.IdCliente = IdCliente;

                if (Id == 0)
                    Id = ProcessosAutoresBo.Inserir(autor);

                ProcessosAutoresBo.Salvar(autor);

                #region Sede
                var IdSede = 0;

                if (IdProcessoAux == 0)
                {
                    IdSede = (ProcessosAutoresBo.Listar(IdProcesso).FirstOrDefault()?.Clientes?.IdSede ?? 0);
                }
                #endregion

                var result = new { response = "success", idprocesso = IdProcesso, idSede = IdSede };
                return Json(result);
            }
            catch (Exception ex)
            {
                String error = "error";
                String message = ex.InnerException.ToString();

                if (message.Contains("chave duplicada no objeto"))
                    error = "warning";

                var result = new { response = error };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult ExcluirProcessosAutores(Int32 Id)
        {
            try
            {
                #region Auditoria
                ProcessosAutores autor = ProcessosAutoresBo.Buscar(Id);

                if (autor != null)
                {
                    Auditoria auditoria = new Auditoria();
                    auditoria.DataHora = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                    auditoria.Modulo = "Processo";
                    auditoria.Tipo = "Autor";
                    auditoria.Acao = "Excluído";
                    auditoria.Log = String.Format("<b>Sede</b>: {0};<b>Profissional</b>: {1};<b>Status</b>: {2};<b>Cliente</b>: {3};", autor.Clientes.Sedes.Nome, autor.Processos.Profissionais.Pessoas.Nome, autor.Processos.Status, autor.Clientes.Pessoas.Nome);
                    auditoria.Usuario = Sessao.Usuario.Nome;

                    AuditoriaBo.Inserir(auditoria);
                }
                #endregion

                ProcessosAutoresBo.Excluir(Id);

                var result = new { response = "removed" };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult CarregarProcessosPartesAdvogados(Int64 IdProcesso)
        {
            try
            {
                var parteAdvogado = ProcessosPartesAdvogadosBo.Listar(IdProcesso);
                String partesAdvogados = String.Empty;

                List<PartsLawyers> partsLawyers = new List<PartsLawyers>();
                PartsLawyers partLawyer;

                int PartesQuantidade = (parteAdvogado.Count > 0 ? (parteAdvogado.Count + 1) : 1);

                for (int i = 0; i < PartesQuantidade; i++)
                {
                    partLawyer = new PartsLawyers();
                    partLawyer.IdPart = (parteAdvogado.Count > 0 && i < parteAdvogado.Count ? (parteAdvogado[i].IdCliente ?? 0) : 0);
                    partLawyer.Part = (parteAdvogado.Count > 0 && i < parteAdvogado.Count ? (parteAdvogado[i].Clientes?.Pessoas?.Nome ?? string.Empty) : string.Empty);
                    partLawyer.Lawyer = string.Empty;
                    partsLawyers.Add(partLawyer);

                    partesAdvogados += "<input id='" + String.Format("HiddenProcessoParteReu{0}", (i + 1)) + "' name='HiddenProcessoParteReu[]' type='hidden' value='" + (parteAdvogado.Count >= (i + 1) ? parteAdvogado[i].ID.ToString() : "0") + "' />";
                    partesAdvogados += "<input id='" + String.Format("HiddenProcessoParteAdvogado{0}", (i + 1)) + "' name='HiddenProcessoParteAdvogado[]' type='hidden' value='" + (parteAdvogado.Count >= (i + 1) ? parteAdvogado[i].ID.ToString() : "0") + "' />";
                    partesAdvogados += "<div class='accordion panel panel-default'>";
                    partesAdvogados += "   <div class='accordion-heading panel-heading' id='" + String.Format("ProcessoParteAdvogado{0}heading", (i + 1)) + "' role='tab'>";
                    partesAdvogados += "      <h4 class='accordion-title panel-title'>";
                    partesAdvogados += "         <a href='" + String.Format("#ProcessoParteAdvogado{0}collapse", (i + 1)) + "' aria-expanded='false' data-toggle='collapse' aria-controls='" + String.Format("ProcessoParteAdvogado{0}collapse", (i + 1)) + "' data-parent='#ProcessoParteAdvogadoAccordion' role='button' class='collapsed'>";
                    partesAdvogados += "            " + String.Format("Réu #{0}", (i + 1));
                    partesAdvogados += (parteAdvogado.Count >= (i + 1) ? String.Format(" - {0}", (parteAdvogado[i].Clientes?.Pessoas?.Nome ?? string.Empty)) : String.Empty);
                    partesAdvogados += "         </a>";
                    partesAdvogados += "      </h4>";
                    partesAdvogados += "   </div>";
                    partesAdvogados += "   <div class='panel-collapse collapse' id='" + String.Format("ProcessoParteAdvogado{0}collapse", (i + 1)) + "' role='tabpanel' aria-labelledby='" + String.Format("ProcessoParteAdvogado{0}heading", (i + 1)) + "' aria-expanded='false' style='height: 0px;'>";
                    partesAdvogados += "      <div class='accordion-body panel-body'>";
                    partesAdvogados += "         <div class='row'>";
                    partesAdvogados += "            <div class='col-md-6'>";
                    partesAdvogados += "               <div class='form-group'>";
                    partesAdvogados += "                  <label class='titlefield'>Nome do Réu <span class='required'>*</span></label>";
                    partesAdvogados += "                  <br />";
                    partesAdvogados += "                  <select id='" + String.Format("ProcessoParteAdvogado{0}Parte", (i + 1)) + "' name='ProcessoParteAdvogadoParte[]' class='select2_single form-control placeholder' data-msg-required='Por favor, selecione um Réu.' data-rule-required='true' style='width: 100% !important;'></select>";
                    partesAdvogados += "                  <label id='" + String.Format("ProcessoParteAdvogado{0}ParteValidate", (i + 1)) + "' name='ProcessoParteAdvogadoParteValidate[]' for='" + String.Format("ProcessoParteAdvogado{0}Parte", (i + 1)) + "' class='error hidden'> Por favor, selecione um Réu.</label>";
                    partesAdvogados += "               </div>";
                    partesAdvogados += "            </div>";
                    partesAdvogados += "         </div>";
                    partesAdvogados += "         <div class='row'>";
                    partesAdvogados += "            <div class='col-md-4'>";
                    partesAdvogados += "               <button type='button' id='" + String.Format("ButtonProcessoParteAdvogado{0}Incluir", (i + 1)) + "' name='" + String.Format("ButtonProcessoParteAdvogado{0}Incluir", (i + 1)) + "' class='btn btn-primary' data-loading-text='Salvando...' onclick='ButtonProcessoParteAdvogadoSalvar_Click(" + (i + 1) + ");'>";
                    partesAdvogados += "                  Salvar";
                    partesAdvogados += "               </button>";
                    partesAdvogados += "            </div>";
                    partesAdvogados += "            <div class='col-md-4' style='text-align: center;'>";
                    partesAdvogados += "            </div>";
                    partesAdvogados += "            <div class='col-md-4' style='text-align: right;'>";
                    partesAdvogados += "               <button type='button' id='" + String.Format("ButtonProcessoParteAdvogado{0}Excluir", (i + 1)) + "' name='" + String.Format("ButtonProcessoParteAdvogado{0}Excluir", (i + 1)) + "' class='btn btn-danger' data-loading-text='Apagando...' onclick='ButtonProcessoParteAdvogadoExcluir_Click(" + (i + 1) + ");' " + (parteAdvogado.Count >= (i + 1) ? String.Empty : "disabled='disabled'") + ">";
                    partesAdvogados += "                  Apagar";
                    partesAdvogados += "               </button>";
                    partesAdvogados += "               <div id='" + String.Format("ButtonProcessoParteAdvogado{0}ExcluirConfirmacao", (i + 1)) + "' style='display:none'>";
                    partesAdvogados += "                  <label id='" + String.Format("LabelProcessoParteAdvogado{0}ExcluirConfirmacao", (i + 1)) + "' style='font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente excluir esse Autor?</label><br />";
                    partesAdvogados += "                  <button id='" + String.Format("accept{0}", (i + 1)) + "' class='btn btn-default btn-xs' value='true'>Sim</button>";
                    partesAdvogados += "                  <button id='" + String.Format("go_back{0}", (i + 1)) + "' class='btn btn-primary btn-xs' value='false'>Não</button>";
                    partesAdvogados += "               </div>";
                    partesAdvogados += "            </div>";
                    partesAdvogados += "         </div>";
                    partesAdvogados += "      </div>";
                    partesAdvogados += "   </div>";
                    partesAdvogados += "   <div class='alert alert-success hidden' id='processo-parte-advogados-alert-success-" + (i + 1) + "'>";
                    partesAdvogados += "      <strong>SUCESSO:</strong> Cadastrado do réu realizado com sucesso.";
                    partesAdvogados += "   </div>";
                    partesAdvogados += "   <div class='alert alert-success hidden' id='processo-parte-advogados-alert-removed-" + (i + 1) + "'>";
                    partesAdvogados += "      <strong>SUCESSO:</strong> Exclusão do réu realizado com sucesso.";
                    partesAdvogados += "   </div>";
                    partesAdvogados += "   <div class='alert alert-danger hidden' id='processo-parte-advogados-alert-error-" + (i + 1) + "'>";
                    partesAdvogados += "      <strong>ERRO:</strong> Algo deu errado ao efetuar o cadastro do réu.";
                    partesAdvogados += "   </div>";
                    partesAdvogados += "   <div class='alert alert-danger hidden' id='processo-parte-advogados-alert-error2-" + (i + 1) + "'>";
                    partesAdvogados += "      <strong>ERRO:</strong> Algo deu errado ao efetuar a exclusão do réu.";
                    partesAdvogados += "   </div>";
                    partesAdvogados += "</div>";
                }

                var result = new { response = "success", partesAdvogados = partesAdvogados, count = PartesQuantidade, array = partsLawyers.ToArray() };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult SalvarProcessosPartesAdvogados(Int32 Id, Int32 IdProcesso, Int32? IdParte, Int32? IdCliente, String Parte, String Advogado, String NumeroOab, String TelefoneComercial, String Celular, String Email)
        {
            try
            {
                #region Processos
                Processos processo;

                if (IdProcesso == 0)
                {
                    processo = new Processos();
                    processo.Status = "Ativo";
                    processo.DataInclusao = DateTime.Now;
                    IdProcesso = ProcessosBo.Inserir(processo);
                }

                processo = ProcessosBo.Buscar(IdProcesso);
                #endregion

                #region Cliente
                if (IdCliente == null)
                {
                    Pessoas pessoa = new Pessoas();
                    pessoa.Nome = Parte;

                    var IdPessoa = PessoasBo.Inserir(pessoa);

                    Clientes cliente = new Clientes();
                    cliente.IDPessoa = IdPessoa;

                    IdCliente = ClientesBo.Inserir(cliente);
                }
                #endregion

                #region Parte
                ProcessosPartes partes = new ProcessosPartes();

                if (IdParte != null && IdParte > 0)
                {
                    partes.ID = IdParte.Value;
                }

                partes.IdProcesso = IdProcesso;
                partes.NomeParte = Parte;
                partes.IdCliente = IdCliente;

                if (IdParte == null || IdParte == 0)
                {
                    IdParte = ProcessosPartesBo.Inserir(partes);
                }

                ProcessosPartesBo.Salvar(partes);

                partes = ProcessosPartesBo.Buscar(IdParte.Value);
                #endregion

                #region Auditoria
                Auditoria auditoria = new Auditoria();
                auditoria.DataHora = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                auditoria.Modulo = "Processo";
                auditoria.Tipo = "Réu e Advogado";
                auditoria.Acao = "Alterado";
                auditoria.Log = String.Format("<b>Sede</b>: {0};<b>Profissional</b>: {1};<b>Status</b>: {2};<b>Nome do Réu</b>: {3};<b>Nome do Advogado</b>: {4};<b>Número da OAB</b>: {5};<b>Telefone Comercial</b>: {6};<b>Celular</b>: {7};<b>E-mail</b>: {8};", (processo != null && processo.ProcessosAutores.Count > 0 ? processo.ProcessosAutores.FirstOrDefault().Clientes.Sedes?.Nome : String.Empty), (processo != null ? processo.Profissionais?.Pessoas?.Nome : String.Empty), (processo != null ? processo.Status : String.Empty), (partes != null ? partes.NomeParte : String.Empty), Advogado, NumeroOab, TelefoneComercial, Celular, Email);
                auditoria.Usuario = Sessao.Usuario.Nome;

                if (Id == 0)
                {
                    auditoria.Acao = "Inserido";
                }

                AuditoriaBo.Inserir(auditoria);
                #endregion

                ProcessosPartesAdvogados partesAdvogados = new ProcessosPartesAdvogados();
                partesAdvogados.ID = Id;
                partesAdvogados.IdProcessoParte = IdParte.Value;
                partesAdvogados.NomeAdvogado = Advogado;
                partesAdvogados.NumeroOab = NumeroOab;
                partesAdvogados.TelefoneComercial = Util.OnlyNumbers(TelefoneComercial);
                partesAdvogados.Celular = Util.OnlyNumbers(Celular);
                partesAdvogados.Email = Email;

                if (Id == 0)
                    Id = ProcessosPartesAdvogadosBo.Inserir(partesAdvogados);

                ProcessosPartesAdvogadosBo.Salvar(partesAdvogados);

                var result = new { response = "success", idprocesso = IdProcesso };
                return Json(result);
            }
            catch (Exception ex)
            {
                String error = "error";
                String message = ex.InnerException.ToString();

                if (message.Contains("chave duplicada no objeto"))
                    error = "warning";

                var result = new { response = error };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult ExcluirProcessosPartesAdvogados(Int32 Id)
        {
            try
            {
                //                ProcessosPartesAdvogados partesAdvogados = ProcessosPartesAdvogadosBo.Consultar(Id, 0);

                // Comentado porque estava dando erro
                //#region Auditoria
                //if (partesAdvogados != null)
                //{
                //    Auditoria auditoria = new Auditoria();
                //    auditoria.DataHora = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                //    auditoria.Modulo = "Processo";
                //    auditoria.Tipo = "Réu e Advogado";
                //    auditoria.Acao = "Excluído";
                //    auditoria.Log = String.Format("<b>Sede</b>: {0};<b>Profissional</b>: {1};<b>Status</b>: {2};<b>Nome do Réu</b>: {3};<b>Nome do Advogado</b>: {4};<b>Número da OAB</b>: {5};<b>Telefone Comercial</b>: {6};<b>Celular</b>: {7};<b>E-mail</b>: {8};", partesAdvogados.ProcessosPartes.Clientes.Sedes.Nome, partesAdvogados.ProcessosPartes.Processos.Profissionais.Pessoas.Nome, partesAdvogados.ProcessosPartes.Processos.Status, partesAdvogados.ProcessosPartes.NomeParte, partesAdvogados.NomeAdvogado, partesAdvogados.NumeroOab, partesAdvogados.TelefoneComercial, partesAdvogados.Celular, partesAdvogados.Email);
                //    auditoria.Usuario = Sessao.Usuario.Nome;

                //    AuditoriaBo.Inserir(auditoria);
                //}
                //#endregion

                //                ProcessosPartesAdvogadosBo.Excluir(Id);

                ProcessosPartesBo.Excluir(Id);

                var result = new { response = "removed" };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        public JsonResult CarregarProcessosAdvogados(Int32 IdProcesso, String NomeAdvogado)
        {
            try
            {
                var advogado = ProcessosPartesAdvogadosBo.Buscar(IdProcesso, NomeAdvogado);

                var result = new { response = "success", advogado = advogado };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult CarregarProcessosAcoes(Int64 IdProcesso)
        {
            try
            {
                var acao = ProcessosAcoesBo.Listar(IdProcesso);
                //                acao.RemoveAll(x => x.NumeroProcesso == "Histórico");

                String acoes = String.Empty;

                List<Actions> actions = new List<Actions>();
                Actions action;

                int AcoesQuantidade = (acao.Count > 0 ? (acao.Count + 1) : 1);

                for (int i = 0; i < AcoesQuantidade; i++)
                {
                    action = new Actions();
                    action.IdAction = (acao.Count > 0 && i < acao.Count && acao[i].IdAcao != null ? int.Parse(acao[i].IdAcao.ToString()) : 0);
                    action.IdStick = (acao.Count > 0 && i < acao.Count && acao[i].IdVara != null ? int.Parse(acao[i].IdVara.ToString()) : 0);
                    action.IdArea = (acao.Count > 0 && i < acao.Count && acao[i].IdProcessosArea != null ? int.Parse(acao[i].IdProcessosArea.ToString()) : 0);
                    actions.Add(action);

                    acoes += "<input id='" + String.Format("HiddenProcessoAcao{0}", (i + 1)) + "' name='HiddenProcessoAcao[]' type='hidden' value='" + (acao.Count >= (i + 1) ? acao[i].ID.ToString() : "0") + "' />";
                    acoes += "<div class='accordion panel panel-default'>";
                    acoes += "   <div class='accordion-heading panel-heading' id='" + String.Format("ProcessoAcao{0}heading", (i + 1)) + "' role='tab'>";
                    acoes += "      <h4 class='accordion-title panel-title'>";
                    acoes += "         <a href='" + String.Format("#ProcessoAcao{0}collapse", (i + 1)) + "' aria-expanded='false' data-toggle='collapse' aria-controls='" + String.Format("ProcessoAcao{0}collapse", (i + 1)) + "' data-parent='#ProcessoAcaoAccordion' role='button' class='collapsed'>";
                    acoes += "            " + String.Format("Ação #{0} ", (i + 1));
                    acoes += (acao.Count >= (i + 1) ? String.Format(" - {0}{1}", acao[i].NumeroProcesso, (!String.IsNullOrEmpty(acao[i].Status) ? String.Format(" - {0}", acao[i].Status) : String.Empty)) : String.Empty);
                    acoes += "         </a>";
                    acoes += "      </h4>";
                    acoes += "   </div>";
                    acoes += "   <div class='panel-collapse collapse' id='" + String.Format("ProcessoAcao{0}collapse", (i + 1)) + "' role='tabpanel' aria-labelledby='" + String.Format("ProcessoAcao{0}heading", (i + 1)) + "' aria-expanded='false' style='height: 0px;'>";
                    acoes += "      <div class='accordion-body panel-body'>";
                    acoes += "         <div class='row'>";
                    acoes += "            <div class='col-md-2'>";
                    acoes += "               <div class='form-group'>";
                    acoes += "                  <label class='titlefield'>Data Ação <span class='required'>*</span></label>";
                    acoes += "                  <div id='" + String.Format("ProcessoAcao{0}DataAcao", (i + 1)) + "' name='ProcessoAcaoDataAcao[]' class='input-group date'>";
                    acoes += "                     <input id='" + String.Format("ProcessoAcao{0}Data", (i + 1)) + "' name='ProcessoAcaoData[]' class='form-control placeholder' data-msg-required='Por favor, selecione uma Data Inicial.' data-rule-required='true' type='text' style='height: 40px !important;' readonly value='" + (acao.Count >= (i + 1) && acao[i].DataAcao != null ? acao[i].DataAcao.ToString().Substring(0, 10) : DateTime.Now.ToString("dd/MM/yyyy")) + "' />";
                    acoes += "                     <span class='input-group-addon' style='border-radius: 0px !important; padding: 11px 12px !important;'><i class='glyphicon glyphicon-calendar'></i></span>";
                    acoes += "                  </div>";
                    acoes += "               </div>";
                    acoes += "            </div>";
                    acoes += "            <div class='col-md-2' hidden>";
                    acoes += "               <div class='form-group'>";
                    acoes += "                  <label class='titlefield'>Evento Automático <span class='required'>*</span></label>";
                    acoes += "                  <br />";
                    acoes += "                  <div class='radio-item'>";
                    acoes += "                     <input type='radio' id='" + String.Format("ProcessoAcao{0}EventoAutomaticoSim", (i + 1)) + "' name='" + String.Format("ProcessoAcao{0}EventoAutomatico", (i + 1)) + "' value='S' " + (acao.Count >= (i + 1) && acao[i].EventoAutomatico != null && acao[i].EventoAutomatico.Value ? "checked" : string.Empty) + ">";
                    acoes += "                     <label for='" + String.Format("ProcessoAcao{0}EventoAutomaticoSim", (i + 1)) + "'>Ativo</label>";
                    acoes += "                  </div>";
                    acoes += "                  <div class='radio-item'>";
                    acoes += "                     <input type='radio' id='" + String.Format("ProcessoAcao{0}EventoAutomaticoNao", (i + 1)) + "' name='" + String.Format("ProcessoAcao{0}EventoAutomatico", (i + 1)) + "' value='N' " + (acao.Count >= (i + 1) && acao[i].EventoAutomatico != null && !acao[i].EventoAutomatico.Value ? "checked" : (acao.Count == i ? "checked" : string.Empty)) + ">";
                    acoes += "                     <label for='" + String.Format("ProcessoAcao{0}EventoAutomaticoNao", (i + 1)) + "'>Inativo</label>";
                    acoes += "                  </div>";
                    acoes += "               </div>";
                    acoes += "            </div>";
                    acoes += "            <div class='col-md-3'>";
                    acoes += "               <div class='form-group'>";
                    acoes += "                  <label class='titlefield'>Número do Processo </label>";
                    acoes += "                  <input type='text' id='" + String.Format("ProcessoAcao{0}NumeroProcesso", (i + 1)) + "' name='ProcessoAcaoNumeroProcesso[]' class='form-control placeholder' data-msg-required='Por favor, informe o Número do Processo.' maxlength='50' value='" + (acao.Count >= (i + 1) ? acao[i].NumeroProcesso : String.Empty) + "' />";
                    acoes += "               </div>";
                    acoes += "            </div>";
                    acoes += "            <div class='col-md-3'>";
                    acoes += "               <div class='form-group'>";
                    acoes += "                  <label class='titlefield'>Número do Processo Antigo </label>";
                    acoes += "                  <input type='text' id='" + String.Format("ProcessoAcao{0}NumeroProcessoAntigo", (i + 1)) + "' name='ProcessoAcaoNumeroProcessoAntigo[]' class='form-control placeholder' data-msg-required='Por favor, informe o Número do Processo.' maxlength='50' value='" + (acao.Count >= (i + 1) ? acao[i].NumeroProcessoAntigo : String.Empty) + "' />";
                    acoes += "               </div>";
                    acoes += "            </div>";

                    acoes += "            <div class='col-md-2'>";
                    acoes += "               <div class='form-group'>";
                    acoes += "                  <label class='titlefield'>Status do Processo <span class='required'>*</span></label>";
                    acoes += "                  <select id='" + String.Format("ProcessoAcao{0}Status", (i + 1)) + "' name='ProcessoAcaoStatus[]' class='form-control placeholder' data-msg-required='Por favor, selecione um Status.' data-rule-required='true'" + (Sessao.Usuario.Nivel != 5 && acao.Count >= (i + 1) && acao[i].Status == "Baixado" ? "disabled" : string.Empty) + ">";
                    acoes += "                     <option value=''>Selecione um Status</option>";
                    acoes += "                     <option value='Ativo'" + (acao.Count >= (i + 1) && acao[i].Status == "Ativo" ? " selected" : String.Empty) + ">Ativo</option>";
                    acoes += "                     <option value='Baixado'" + (acao.Count >= (i + 1) && acao[i].Status == "Baixado" ? " selected" : String.Empty) + ">Baixado</option> ";
                    acoes += "                  </select> ";
                    acoes += "               </div> ";
                    acoes += "            </div> ";

                    acoes += "         </div>";
                    acoes += "         <div class='row'>";
                    acoes += "            <div class='col-md-4'>";
                    acoes += "               <div class='form-group'>";
                    acoes += "                  <label class='titlefield'>Ação</label>";
                    acoes += "                  <br />";
                    acoes += "                  <select id='" + String.Format("ProcessoAcao{0}Acao", (i + 1)) + "' name='ProcessoAcaoAcao[]' class='select2_single form-control' style='width: 100% !important;'></select>";
                    acoes += "               </div>";
                    acoes += "            </div>";
                    acoes += "            <div class='col-md-4'>";
                    acoes += "               <div class='form-group'>";
                    acoes += "                  <label class='titlefield'>Juízo</label>";
                    acoes += "                  <br />";
                    acoes += "                  <select id='" + String.Format("ProcessoAcao{0}Vara", (i + 1)) + "' name='ProcessoAcaoVara[]' class='select2_single form-control' style='width: 100% !important;'> readonly value</select>";
                    acoes += "               </div>";
                    acoes += "            </div>";
                    acoes += "            <div class='col-md-4'>";
                    acoes += "               <div class='form-group'>";
                    acoes += "                  <label class='titlefield'>Área</label>";
                    acoes += "                  <br />";
                    acoes += "                  <select id='" + String.Format("ProcessoAcao{0}Area", (i + 1)) + "' name='ProcessoAcaoArea[]' class='select2_single form-control' style='width: 100% !important;'> readonly value</select>";
                    acoes += "               </div>";
                    acoes += "            </div>";
                    acoes += "         </div>";
                    acoes += "         <div class='row'>";
                    acoes += "            <div class='col-md-4'>";
                    acoes += "               <button type='button' id='" + String.Format("ButtonProcessoAcao{0}Incluir", (i + 1)) + "' name='" + String.Format("ButtonProcessoAcao{0}Incluir", (i + 1)) + "' class='btn btn-primary' data-loading-text='Salvando...' onclick='ButtonProcessoAcaoSalvar_Click(" + (i + 1) + ");'>";
                    acoes += "                  Salvar";
                    acoes += "               </button>";
                    acoes += "            </div>";
                    acoes += "            <div class='col-md-4' style='text-align: center;'>";
                    acoes += "            </div>";
                    acoes += "            <div class='col-md-4' style='text-align: right;'>";
                    acoes += "               <button type='button' id='" + String.Format("ButtonProcessoAcao{0}Excluir", (i + 1)) + "' name='" + String.Format("ButtonProcessoAcao{0}Excluir", (i + 1)) + "' class='btn btn-danger' data-loading-text='Apagando...' onclick='ButtonProcessoAcaoExcluir_Click(" + (i + 1) + ");' " + (acao.Count >= (i + 1) ? String.Empty : "disabled='disabled'") + ">";
                    acoes += "                  Apagar";
                    acoes += "               </button>";
                    acoes += "               <div id='" + String.Format("ButtonProcessoAcao{0}ExcluirConfirmacao", (i + 1)) + "' style='display:none'>";
                    acoes += "                  <label id='" + String.Format("LabelProcessoAcao{0}ExcluirConfirmacao", (i + 1)) + "' style='font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente excluir esse Evento?</label><br />";
                    acoes += "                  <button id='" + String.Format("accept{0}", (i + 1)) + "' class='btn btn-default btn-xs' value='true'>Sim</button>";
                    acoes += "                  <button id='" + String.Format("go_back{0}", (i + 1)) + "' class='btn btn-primary btn-xs' value='false'>Não</button>";
                    acoes += "               </div>";
                    acoes += "            </div>";
                    acoes += "         </div>";
                    acoes += "      </div>";
                    acoes += "   </div>";
                    acoes += "   <div class='alert alert-success hidden' id='processo-acao-alert-success-" + (i + 1) + "'>";
                    acoes += "      <strong>SUCESSO:</strong> Cadastrado da ação realizada com sucesso.";
                    acoes += "   </div>";
                    acoes += "   <div class='alert alert-success hidden' id='processo-acao-alert-removed-" + (i + 1) + "'>";
                    acoes += "      <strong>SUCESSO:</strong> Exclusão da ação realizada com sucesso.";
                    acoes += "   </div>";
                    acoes += "   <div class='alert alert-danger hidden' id='processo-acao-alert-error-" + (i + 1) + "'>";
                    acoes += "      <strong>ERRO:</strong> Algo deu errado ao efetuar o cadastro da ação.";
                    acoes += "   </div>";
                    acoes += "   <div class='alert alert-danger hidden' id='processo-acao-alert-error2-" + (i + 1) + "'>";
                    acoes += "      <strong>ERRO:</strong> Número do Processo já foi cadastrado em outra Ação.";
                    acoes += "   </div>";
                    acoes += "</div>";
                }

                var result = new { response = "success", acoes = acoes, count = AcoesQuantidade, array = actions.ToArray() };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult SalvarProcessosAcoes(Int32 Id, Int32 IdProcesso, String DataAcao, String NumeroProcesso, String NumeroProcessoAntigo, Int32? IdAcao, Int32? IdVara, Int32? IdArea, String Acao, String Vara, String Area, String EventoAutomatico, String Status)
        {
            try
            {
                if (Util.IsNumeric(NumeroProcesso))
                {
                    var dublicado = ProcessosAcoesBo.ConsultarDublicado(IdProcesso, NumeroProcesso);

                    if (dublicado.Count > 0)
                    {
                        String error = "warning";

                        var resultDouble = new { response = error };
                        return Json(resultDouble);

                    }
                }

                #region Ação
                ProcessosCadastroAcao acoes;
                if (IdAcao == null && !String.IsNullOrEmpty(Acao))
                {
                    acoes = new ProcessosCadastroAcao();
                    acoes.Acao = Acao;

                    IdAcao = AcaoBo.Inserir(acoes);
                }

                if (IdAcao != null)
                {
                    acoes = AcaoBo.Consultar(int.Parse(IdAcao.ToString()));
                }
                else
                {
                    acoes = new ProcessosCadastroAcao();
                }
                #endregion

                #region Vara
                ProcessosVara varas;
                if (IdVara == null && !String.IsNullOrEmpty(Vara))
                {
                    varas = new ProcessosVara();
                    varas.Vara = Vara;

                    IdVara = VarasBo.Inserir(varas);
                }

                if (IdVara != null)
                {
                    varas = VarasBo.Consultar(int.Parse(IdVara.ToString()));
                }
                else
                {
                    varas = new ProcessosVara();
                }
                #endregion

                #region Área
                ProcessosAreas areas;
                if (IdArea == null && !String.IsNullOrEmpty(Area))
                {
                    areas = new ProcessosAreas();
                    areas.AreaAtuacao = Area;

                    IdArea = AreasBo.Inserir(areas);
                }

                if (IdArea != null)
                {
                    areas = AreasBo.Consultar(int.Parse(IdArea.ToString()));
                }
                else
                {
                    areas = new ProcessosAreas();
                }
                #endregion

                #region Processos
                Processos processo = ProcessosBo.Buscar(IdProcesso);
                ProcessosAcoes acaoanterior = ProcessosBo.AcaoAnterior(Id);
                #endregion

                #region Auditoria

                var eventoAutomatico = false;

                if (EventoAutomatico == "S")
                {
                    eventoAutomatico = true;
                }

                if (acaoanterior == null)
                {
                    eventoAutomatico = false;
                }

                if (acaoanterior != null) 
                {
                    if (acaoanterior.EventoAutomatico != eventoAutomatico)
                    {
                        if (EventoAutomatico == "S")
                        {
                            eventoAutomatico = true;
                        }
                        else
                        {
                            eventoAutomatico = false;
                        }
                    }
                    else
                    {
                        eventoAutomatico = bool.Parse(acaoanterior.EventoAutomatico.ToString());
                    }
                }


                Auditoria auditoria = new Auditoria();
                auditoria.DataHora = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                auditoria.Modulo = "Processo";
                auditoria.Tipo = "Ação";
                auditoria.Acao = "Alterado";
                auditoria.Log = String.Format("<b>Sede</b>: {0};<b>Profissional</b>: {1};<b>Status</b>: {2};<b>Data Ação</b>: {3};<b>Número do Processo</b>: {4};<b>Ação</b>: {5};<b>Juízo</b>: {6};<b>Área</b>: {7};", processo?.ProcessosAutores?.FirstOrDefault()?.Clientes?.Sedes?.Nome ?? string.Empty, (processo.Profissionais != null ? processo.Profissionais.Pessoas.Nome : String.Empty), (processo != null ? processo.Status : String.Empty), DataAcao, NumeroProcesso, (acoes != null ? acoes.Acao : string.Empty), (varas != null ? varas.Vara : string.Empty), (areas != null ? areas.AreaAtuacao : string.Empty));
                auditoria.Usuario = Sessao.Usuario.Nome;

                if (Id == 0)
                {
                    auditoria.Acao = "Inserido";
                }

                AuditoriaBo.Inserir(auditoria);
                #endregion

                ProcessosAcoes acao = new ProcessosAcoes();
                acao.ID = Id;
                acao.IdProcesso = IdProcesso;
                acao.DataAcao = DateTime.Parse(DataAcao);
                if (NumeroProcesso != "")
                {
                    acao.NumeroProcesso = NumeroProcesso;
                }
                else
                {
                    acao.NumeroProcesso = "Histórico " + (IdArea != null ? AreasBo.ConsultarNome(IdArea.Value).AreaAtuacao : string.Empty);
                }
                acao.NumeroProcessoAntigo = NumeroProcessoAntigo;
                acao.IdAcao = IdAcao;
                acao.IdVara = IdVara;
                acao.IdProcessosArea = IdArea;

                acao.EventoAutomatico = eventoAutomatico;

                if (Id == 0)
                    Id = ProcessosAcoesBo.Inserir(acao);

                if (Status == "Baixado")
                {
                    var processosAcao = ProcessosAcoesBo.Consultar(Id);

                    if (processosAcao.ID > 0 && processosAcao.Status != "Baixado")
                    {
                        //SalvarProcessosEventos(0, null, DateTime.Now.ToString(), String.Empty, processosAcao.ID, 19, (processo.IdProfissionalResponsavel != null ? processo.IdProfissionalResponsavel.Value : 0), null, null, "Processo baixado na data do evento", null, null, null, null, null, 0, false);
                         SalvarProcessosEventos(0, null, DateTime.Now.ToString(), String.Empty, processosAcao.ID, 19, 0, null, null, "Processo baixado na data do evento", null, null, null, null, null, 0, false);
                   
                    }
                }

                acao.Status = Status;

                ProcessosAcoesBo.Salvar(acao);

                var result = new { response = "success", status = Status, id = Id };
                return Json(result);
            }
            catch (Exception ex)
            {
                String error = "error";
                String message = ex.InnerException.ToString();

                if (message.Contains("chave duplicada no objeto"))
                    error = "warning";

                var result = new { response = error };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult ExcluirProcessosAcoes(Int32 Id)
        {
            try
            {
                #region Auditoria
                //                Comentado por problema no ProcessosAcoesBO.Buscar
                ProcessosAcoes acao = ProcessosAcoesBo.Buscar(Id);

                if (acao != null)
                {
                    Auditoria auditoria = new Auditoria();
                    auditoria.DataHora = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                    auditoria.Modulo = "Processo";
                    auditoria.Tipo = "Ação";
                    auditoria.Acao = "Excluído";
                    auditoria.Log = String.Format("<b>Sede</b>: {0};<b>Profissional</b>: {1};<b>Status</b>: {2};<b>Data Ação</b>: {3};<b>Número do Processo</b>: {4};<b>Ação</b>: {5};<b>Juízo</b>: {6};<b>Área</b>: {7};", (acao.Processos.ProcessosAutores.FirstOrDefault()?.Clientes?.Sedes?.Nome ?? string.Empty), acao.Processos.Profissionais.Pessoas.Nome, acao.Processos.Status, acao.DataAcao, acao.NumeroProcesso, (acao.ProcessosCadastroAcao?.Acao ?? string.Empty), (acao.ProcessosVara?.Vara ?? string.Empty), (acao.ProcessosAreas?.AreaAtuacao ?? string.Empty));
                    auditoria.Usuario = Sessao.Usuario.Nome;

                    AuditoriaBo.Inserir(auditoria);
                }
                #endregion

                ProcessosAcoesBo.Excluir(Id);

                var result = new { response = "removed" };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult CarregarProcessosEventos(Int64 IdProcesso)
        {
            try
            {
                var evento = ProcessosEventosBo.Listar(IdProcesso);
                var numeroProcesso = string.Empty;
                var status = string.Empty;
                String eventos = String.Empty;

                List<Events> events = new List<Events>();
                Events eventAux;

                int d = evento.Count;
                int EventosQuantidade = (evento.Count > 0 ? (evento.Count) : 1);

                if (EventosQuantidade > 0)
                {
                    numeroProcesso = evento[0].ProcessosAcoes.NumeroProcesso;
                    status = evento[0].ProcessosAcoes.Status;
                }

                for (int i = 0; i < EventosQuantidade; i++)
                {
                    eventAux = new Events();
                    eventAux.IdAction = (evento.Count > 0 && i < evento.Count ? int.Parse(evento[i].IdProcessosAcao.ToString()) : 0);
                    eventAux.IdEvent = (evento.Count > 0 && i < evento.Count ? int.Parse(evento[i].IdProcessosEvento.ToString()) : 0);
                    events.Add(eventAux);

                    ProcessosAgendaProfissional agenda = null;

                    if (evento[i].ID>0)
                    {
                        agenda = ProcessosAgendasProfissionaisBo.Consultar((int)evento[i].ID);
                    }

                    string resumo = "";
                    if (i < (EventosQuantidade - 1))
                    {
                        if (evento[i].Descricao != null && Uri.UnescapeDataString(evento[i].Descricao).Length > 250)
                        {
                            resumo = Uri.UnescapeDataString(evento[i].Descricao.Substring(0, 250) ?? String.Empty).Replace("<br>", string.Empty).Replace("<br/>", string.Empty).Replace("<br />", string.Empty).Replace("3Cp>", string.Empty).Replace("3Cdiv>", string.Empty);
                        }
                        else
                        {
                            resumo = Uri.UnescapeDataString(evento[i].Descricao ?? String.Empty).Replace("<br>", string.Empty).Replace("<br/>", string.Empty).Replace("<br />", string.Empty).Replace("3Cp>", string.Empty).Replace("3Cdiv>", string.Empty);
                        }
                    }

                    eventos += "<input id='" + String.Format("HiddenIdAgenda{0}", evento[i].ID.ToString()) + "' name='HiddenIdAgenda[]' type='hidden' value='" + (agenda!=null? agenda.ID.ToString() : "0") + "' />";
                    eventos += "<input id='" + String.Format("HiddenProcessoEvento{0}", (i + 1)) + "' name='HiddenProcessoEvento[]' type='hidden' value='" + (evento.Count >= (i + 1) ? evento[i].ID.ToString() : "0") + "' />";
                    eventos += "<input type='hidden' id='" + String.Format("ProcessoEvento{0}DescricaoOriginal", (i + 1)) + "' name = '" + String.Format("ProcessoEvento{0}DescricaoOriginal", (i + 1)) + "' value=''>";
                    eventos += "<div class='accordion panel panel-default'>";
                    eventos += "   <div class='accordion-heading panel-heading' id='" + String.Format("ProcessoEvento{0}heading", (i + 1)) + "' role='tab'>";
                    eventos += "      <h4 class='accordion-title panel-title'>";
                    eventos += "         <a href='" + String.Format("#ProcessoEvento{0}collapse", (i + 1)) + "' aria-expanded='true' data-toggle='collapse' aria-controls='" + String.Format("ProcessoEvento{0}collapse", (i + 1)) + "' data-parent='#ProcessoEventoAccordion' role='button' class='collapsed' onclick='CarregarEvento(" + (evento.Count >= (i + 1) ? evento[i].ID.ToString() : "0") + ", " + eventAux.IdAction + ", " + eventAux.IdEvent + ");'>";
                    eventos += "            " + String.Format("Evento #{0}", Util.AddLeadingZeros(d, 3));
                    eventos += (evento.Count >= (i + 1) ? String.Format(" - {0} - {1}{2}{3}{4}", evento[i].ProcessosAcoes.NumeroProcesso, evento[i].ProcessosEventos.Descricao,
                                                                                                (evento[i].Data != null ? String.Format(" - {0}", DateTime.Parse(evento[i].Data.ToString()).ToString("dd/MM/yyyy")) : string.Empty),
                                                                                                (evento[i].PrazoEvento1 != null ? String.Format(" - {0}", DateTime.Parse(evento[i].PrazoEvento1.ToString()).ToString("dd/MM/yyyy")) : string.Empty),
                                                                                                (evento[i].PrazoEvento2 != null ? String.Format(" - {0}", DateTime.Parse(evento[i].PrazoEvento2.ToString()).ToString("dd/MM/yyyy")) : string.Empty),
                                                                                                 resumo) : String.Empty);

                    int fCount = Directory.GetFiles(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-*.pdf", Util.OnlyNumbers(evento[i].ProcessosAcoes.NumeroProcesso), evento[i].ID.ToString()), SearchOption.TopDirectoryOnly).Length;
                    if (fCount > 0)
                    {
                        eventos += "<img src='../Content/Images/clip.png' width='16' height='16'>&nbsp;";
                    }

                    eventos += "         </a>";
                    eventos += "      </h4>";
                    eventos += "   </div>";
                    eventos += "   <div class='panel-collapse collapse' id='" + String.Format("ProcessoEvento{0}collapse", (i + 1)) + "' role='tabpanel' aria-labelledby='" + String.Format("ProcessoEvento{0}heading", (i + 1)) + "' aria-expanded='false' style='height: 0px;'>";
                    eventos += "      <div class='accordion-body panel-body'>";
                    eventos += "         <div id='CarregarEventoDetalhe_" + (evento.Count >= (i + 1) ? evento[i].ID.ToString() : "0") + "' />";
                    eventos += "      </div>";
                    eventos += "   </div>";
                    eventos += "   <div class='alert alert-success hidden' id='processo-evento-alert-success-" + (evento.Count >= (i + 1) ? evento[i].ID.ToString() : "0") + "'>";
                    eventos += "      <strong>SUCESSO:</strong> Cadastrado do evento com sucesso.";
                    eventos += "   </div>";
                    eventos += "   <div class='alert alert-success hidden' id='processo-evento-alert-removed-" + (evento.Count >= (i + 1) ? evento[i].ID.ToString() : "0") + "'>";
                    eventos += "      <strong>SUCESSO:</strong> Exclusão do evento realizado com sucesso.";
                    eventos += "   </div>";
                    eventos += "   <div class='alert alert-danger hidden' id='processo-evento-alert-error-" + (evento.Count >= (i + 1) ? evento[i].ID.ToString() : "0") + "'>";
                    eventos += "      <strong>ERRO:</strong> Algo deu errado ao efetuar o cadastro do evento.";
                    eventos += "   </div>";
                    eventos += "   <div class='alert alert-danger hidden' id='processo-evento-alert-error2-" + (evento.Count >= (i + 1) ? evento[i].ID.ToString() : "0") + "'>";
                    eventos += "      <strong>ERRO:</strong> Não é possível executar a exclusão do evento, pois existe um agendamento vinculado a ele.";
                    eventos += "   </div>";
                    eventos += "   <div class='alert alert-danger hidden' id='processo-evento-alert-errorx-" + (evento.Count >= (i + 1) ? evento[i].ID.ToString() : "0") + "'>";
                    eventos += "      <strong>ERRO:</strong> Não é possível executar essa operação.";
                    eventos += "   </div>";
                    eventos += "</div>";
                    d--;
                }

                var result = new { response = "success", eventos = eventos, count = EventosQuantidade, array = events.ToArray(), numeroProcesso = numeroProcesso, status = status };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult BuscarProcessosAgendas(Int64 IdAcao)
        {
            try
            {
                var agenda = ProcessosAgendasProfissionaisBo.Visualizar(IdAcao);
                String agendas = String.Empty;

                int AgendasQuantidade = agenda.Count;

                agendas += "<table width='100%'>";
                agendas += "   <thead>";
                agendas += "      <tr>";
                agendas += "         <th>Número do Processo</th>";
                agendas += "      </tr>";
                agendas += "   </thead>";
                agendas += "   <tbody>";
                agendas += "      <tr>";
                agendas += "         <td>" + (AgendasQuantidade > 0 ? agenda[0].ProcessosAcoesEventos.ProcessosAcoes.NumeroProcesso : "Histórico") + "</td>";
                agendas += "      </tr>";
                agendas += "   </tbody>";
                agendas += "</table>";
                agendas += "<hr />";
                agendas += "<table width='100%'>";
                agendas += "   <thead>";
                agendas += "      <tr>";
                agendas += "         <th>Profissional</th>";
                agendas += "         <th>Data 1º Prazo</th>";
                agendas += "         <th>Data 2º Prazo</th>";
                agendas += "         <th>Evento</th>";
                agendas += "      </tr>";
                agendas += "   </thead>";
                agendas += "   <tbody>";

                for (int i = 0; i < AgendasQuantidade; i++)
                {
                    agendas += "      <tr " + (i % 2 == 0 ? "style='background-color: #EDEDED'" : string.Empty) + ">";
                    agendas += "         <td>" + (agenda[i].Profissionais.Pessoas != null ? agenda[i].Profissionais.Pessoas.Nome.Trim() : string.Empty) + "</td>";
                    agendas += "         <td>" + (agenda[i].ProcessosAcoesEventos.PrazoEvento1 != null ? agenda[i].ProcessosAcoesEventos.PrazoEvento1.Value.ToString("dd/MM/yyyy") : string.Empty) + "</td>";
                    agendas += "         <td>" + (agenda[i].ProcessosAcoesEventos.PrazoEvento2 != null ? agenda[i].ProcessosAcoesEventos.PrazoEvento2.Value.ToString("dd/MM/yyyy") : string.Empty) + "</td>";
                    agendas += "         <td>" + (agenda[i].ProcessosAcoesEventos.ProcessosEventos != null ? (string.Format("{0} ({1})", agenda[i].ProcessosAcoesEventos.ProcessosEventos.Descricao.Trim(), agenda[i].ProcessosAcoesEventos.ProcessosEventos.Codigo)) : string.Empty) + "</td>";
                    agendas += "      </tr>";
                }

                agendas += "   </tbody>";
                agendas += "</table>";

                var result = new { response = "success", agendas = agendas, count = AgendasQuantidade };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult BuscarProcessosEventos(Int64? IdAcao = 0)
        {
            try
            {
                var evento = ProcessosEventosBo.Visualizar(IdAcao.Value);
                var status = string.Empty;
                String eventos = String.Empty;
                int EventosQuantidade = evento.Count;

                List<Events> events = new List<Events>();
                Events eventAux;

                if (EventosQuantidade > 0)
                {
                    status = evento[0].ProcessosAcoes.Status;

                    eventos += "<table width='100%' style='margin-bottom: 10px !important;'>";
                    eventos += "   <thead>";
                    eventos += "      <tr>";
                    eventos += "         <th>Número do Processo</th>";
                    eventos += "      </tr>";
                    eventos += "   </thead>";
                    eventos += "   <tbody>";
                    eventos += "      <tr>";
                    eventos += "         <td>" + (EventosQuantidade > 0 ? evento[0].ProcessosAcoes.NumeroProcesso : "Histórico") + "</td>";
                    eventos += "      </tr>";
                    eventos += "   </tbody>";
                    eventos += "</table>";


                    eventos += "<input type='hidden' id='HiddenNumeroProcessoAux' value='" + (EventosQuantidade > 0 ? evento[0].ProcessosAcoes.NumeroProcesso : "") + "' />";
                    eventos += "<input type='hidden' id='HiddenIdProcessoAcaoAux' value='" + (EventosQuantidade > 0 ? evento[0].IdProcessosAcao.ToString() : "") + "' />";

                    int d = EventosQuantidade;
                    for (int i = 0; i < EventosQuantidade; i++)
                    {
                        eventAux = new Events();
                        eventAux.IdAction = (evento.Count > 0 && i < evento.Count ? int.Parse(evento[i].IdProcessosAcao.ToString()) : 0);
                        eventAux.IdEvent = (evento.Count > 0 && i < evento.Count ? int.Parse(evento[i].IdProcessosEvento.ToString()) : 0);
                        events.Add(eventAux);

                        string resumo = Util.StripHTML(HttpUtility.UrlDecode(Uri.UnescapeDataString(evento[i].Descricao ?? String.Empty)));
                        if (!string.IsNullOrEmpty(resumo) && resumo.Length > 250)
                        {
                            resumo = resumo.Substring(0, 250);
                        }

                        eventos += "<div class='accordion panel panel-default' style='margin-bottom: 0 !important;'>";
                        eventos += "   <div class='accordion-heading panel-heading-rvm' id='" + String.Format("ProcessoEventoView{0}heading", (i + 1)) + "' role='tab'>";
                        eventos += "      <h4 class='accordion-title panel-title'style='font-size: 14px !important; margin-top: 0px !important;'>";
                        //eventos += "         <a href='" + String.Format("#ProcessoEvento{0}collapse", (i + 1)) + "' aria-expanded='true' data-toggle='collapse' aria-controls='" + String.Format("ProcessoEvento{0}collapse", (i + 1)) + "' data-parent='#ProcessoEventoAccordion' role='button' class='collapsed' onclick='CarregarEvento(" + (evento.Count >= (i + 1) ? evento[i].ID.ToString() : "0") + ", " + eventAux.IdAction + ", " + eventAux.IdEvent + ");'>";
                        eventos += "         <a href='" + String.Format("#ProcessoEventoView{0}collapse", (i + 1)) + "' aria-expanded='false' data-toggle='collapse' aria-controls='" + String.Format("ProcessoEventoView{0}collapse", (i + 1)) + "' data-parent='#ProcessoEventoViewAccordion' role='button' class='collapsed' onclick='VisualizarEvento(" + evento[i].ID + ");'>";


                        eventos += "            " + String.Format("Evento {0} <b> {1} </b> - {2} - {3} ", Util.AddLeadingZeros(d, 3), (evento[i].Data != null ? String.Format(" - {0}", DateTime.Parse(evento[i].Data.ToString()).ToString("dd/MM/yyyy")) : string.Empty),
                                                                                                          (evento[i].ProcessosEventos.Descricao != null ? evento[i].ProcessosEventos.Descricao : string.Empty),
                                                                                                           resumo);

                        int fCount = Directory.GetFiles(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-*.pdf", Util.OnlyNumbers(evento[i].ProcessosAcoes.NumeroProcesso), evento[i].ID.ToString()), SearchOption.TopDirectoryOnly).Length;
                        if (fCount > 0)
                        {
                            eventos += "<img src='../Content/Images/clip.png' width='16' height='16'>&nbsp;";
                        }

                        eventos += "         </a>";
                        eventos += "      </h4>";
                        eventos += "   </div>";
                        eventos += "   <div class='panel-collapse collapse' id='" + String.Format("ProcessoEventoView{0}collapse", (i + 1)) + "' role='tabpanel' aria-labelledby='" + String.Format("ProcessoEventoView{0}heading", (i + 1)) + "' aria-expanded='false' style='height: 0px;'>";
                        eventos += "      <div class='accordion-body panel-body'>";
                        eventos += "         <div id='CarregarEventoDetalhe_" + evento[i].ID + "' />";
                        eventos += "      </div>";
                        eventos += "   </div>";
                        eventos += "</div>";
                        d--;
                    }
                }

                //var result = new { response = "success", eventos = eventos.Replace("<s ", string.Empty).Replace("</s>", string.Empty), count = EventosQuantidade, status = status };
                var result = new { response = "success", eventos = eventos, count = EventosQuantidade, status = status };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult CarregarProcessosEvento(Int64 IdEvento)
        {
            try
            {
                var evento = ProcessosEventosBo.VisualizarEvento(IdEvento);
                var idProfissional = evento.ProcessosAgendaProfissional.LastOrDefault(p => p.IdProfissional != null)?.IdProfissional ?? 0;

                String eventos = String.Empty;

                eventos += "<style>";
                eventos += "    div." + String.Format("ProcessoEvento{0}Upload", (evento != null && evento.ID == IdEvento ? IdEvento.ToString() : "0")) + " {";
                eventos += "        background-color: #fff;";
                eventos += "        border: 1px solid #ccc;";
                eventos += "        display: inline-block;";
                eventos += "        height: 38px;";
                eventos += "        padding: 3px 40px 3px 3px;";
                eventos += "        position: relative;";
                eventos += "        width: 100% !important;";
                eventos += "    }";
                eventos += "        div." + String.Format("ProcessoEvento{0}Upload", (evento != null && evento.ID == IdEvento ? IdEvento.ToString() : "0")) + ":hover {";
                eventos += "            opacity: 0.95;";
                eventos += "        }";
                eventos += "        div." + String.Format("ProcessoEvento{0}Upload", (evento != null && evento.ID == IdEvento ? IdEvento.ToString() : "0")) + " input[type='file'] {";
                eventos += "            display: input-block;";
                eventos += "            width: 100%;";
                eventos += "            height: 30px;";
                eventos += "            opacity: 0;";
                eventos += "            cursor: pointer;";
                eventos += "            position: absolute;";
                eventos += "            left: 0;";
                eventos += "        }";
                eventos += "    ." + String.Format("ProcessoEvento{0}UploadButton", (evento != null && evento.ID == IdEvento ? IdEvento.ToString() : "0")) + " {";
                eventos += "        background-color: #333333;";
                eventos += "        border: none;";
                eventos += "        color: #FFF;";
                eventos += "        cursor: pointer;";
                eventos += "        display: inline-block;";
                eventos += "        height: 30px;";
                eventos += "        margin-right: 15px;";
                eventos += "        width: auto;";
                eventos += "        padding: 0 20px;";
                eventos += "        box-sizing: content-box;";
                eventos += "    }";
                eventos += "        ." + String.Format("ProcessoEvento{0}UploadButton", (evento != null && evento.ID == IdEvento ? IdEvento.ToString() : "0")) + ":hover {";
                eventos += "            background-color: #3D3D3D;";
                eventos += "        }";
                eventos += "    ." + String.Format("ProcessoEvento{0}FileName", (evento != null && evento.ID == IdEvento ? IdEvento.ToString() : "0")) + " {";
                eventos += "        font-family: Arial;";
                eventos += "        font-size: 14px;";
                eventos += "    }";
                eventos += "    ." + String.Format("ProcessoEvento{0}Upload", (evento != null && evento.ID == IdEvento ? IdEvento.ToString() : "0")) + " + ." + String.Format("ProcessoEvento{0}UploadButton", (evento != null && evento.ID == IdEvento ? IdEvento.ToString() : "0")) + " {";
                eventos += "        height: 40px;";
                eventos += "    }";
                eventos += "</style>";

                eventos += "         <div class='row'>";
                eventos += "            <div class='col-md-4'>";
                eventos += "               <div class='form-group'>";
                eventos += "                  <label class='titlefield'>Número do Processo <span class='required'>*</span></label>";
                eventos += "                  <br />";
                eventos += "                  <select id='" + String.Format("ProcessoEvento{0}NumeroProcesso", IdEvento) + "' name='ProcessoEventoNumeroProcesso[]' class='select2_single form-control' style='width: 100% !important;'></select>";
                eventos += "                  <label id='" + String.Format("ProcessoEvento{0}NumeroProcessoValidate", IdEvento) + "' name='" + String.Format("ProcessoEvento{0}NumeroProcessoValidate", IdEvento) + "' for='" + String.Format("ProcessoEvento{0}NumeroProcesso", IdEvento) + "' class='error hidden'> Por favor, selecione um Número de Processo.</label>";
                eventos += "               </div>";
                eventos += "            </div>";
                eventos += "            <div class='col-md-8'>";
                eventos += "               <div class='form-group'>";
                eventos += "                  <label class='titlefield'>Evento <span class='required'>*</span></label>";
                eventos += "                  <br />";
                eventos += "                  <select id='" + String.Format("ProcessoEvento{0}Evento", IdEvento) + "' name='ProcessoEventoEvento[]' class='select2_single form-control' style='width: 100% !important;'></select>";
                eventos += "                  <label id='" + String.Format("ProcessoEvento{0}EventoValidate", IdEvento) + "' name='" + String.Format("ProcessoEvento{0}EventoValidate", IdEvento) + "' for='" + String.Format("ProcessoEvento{0}Evento", IdEvento) + "' class='error hidden'> Por favor, selecione um Evento.</label>";
                eventos += "               </div>";
                eventos += "            </div>";
                eventos += "         </div>";
                eventos += "         <div class='row'>";
                eventos += "            <div class='col-md-6'>";
                eventos += "               <div class='form-group'>";
                eventos += "                  <label class='titlefield'>Profissional</label>";
                eventos += "                  <br />";
                //eventos += "                  <select id='" + String.Format("ProcessoEvento{0}Profissional", IdEvento) + "' name='ProcessoEventoProfissional[]' class='select2_single form-control' style='width: 100% !important;'></select>";
                eventos +=   "<select id = '" + String.Format("ProcessoEvento{0}Profissional", IdEvento) + "' name = 'ProcessoEventoProfissional[]' class='select2_single form-control' style='width: 100% !important;'></select>";
                eventos += "                  <input type='hidden' id='" + String.Format("ProcessoEvento{0}IdProfissional", IdEvento) + "' value='" + idProfissional + "' />";
                eventos += "                  <!--<label id='" + String.Format("ProcessoEvento{0}ProfissionalValidate", IdEvento) + "' name='" + String.Format("ProcessoEvento{0}ProfissionalValidate", IdEvento) + "' for='" + String.Format("ProcessoEvento{0}Profissional", IdEvento) + "' class='error hidden'> Por favor, selecione um Profissional.</label>-->";
                eventos += "               </div>";
                eventos += "            </div>";
                eventos += "            <div class='col-md-3'>";
                eventos += "               <div class='form-group'>";
                eventos += "                  <label class='titlefield'>Data 1º Prazo</label>";
                eventos += "                  <div id='" + String.Format("ProcessoEvento{0}DataPrazoEvento1", IdEvento) + "' name='ProcessosEventoDataPrazoEvento1[]' class='input-group date'>";
                eventos += "                     <input id='" + String.Format("ProcessoEvento{0}PrazoEvento1", IdEvento) + "' name='ProcessoEventoPrazoEvento1[]' class='form-control placeholder' type='text' style='height: 40px !important;' readonly value='" + (evento != null && evento.ID == IdEvento && evento.PrazoEvento1 != null ? evento.PrazoEvento1.ToString().Substring(0, 10) : String.Empty) + "' />";
                eventos += "                     <span class='input-group-addon' style='border-radius: 0px !important; padding: 11px 12px !important;'><i class='glyphicon glyphicon-calendar'></i></span>";
                eventos += "                  </div>";
                eventos += "               </div>";
                eventos += "            </div>";
                eventos += "            <div class='col-md-3'>";
                eventos += "               <div class='form-group'>";
                eventos += "                  <label class='titlefield'>Data 2º Prazo</label>";
                eventos += "                  <div id='" + String.Format("ProcessoEvento{0}DataPrazoEvento2", IdEvento) + "' name='ProcessosEventoDataPrazoEvento2[]' class='input-group date'>";
                eventos += "                     <input id='" + String.Format("ProcessoEvento{0}PrazoEvento2", IdEvento) + "' name='ProcessoEventoPrazoEvento2[]' class='form-control placeholder' type='text' style='height: 40px !important;' readonly value='" + (evento != null && evento.ID == IdEvento && evento.PrazoEvento2 != null ? evento.PrazoEvento2.ToString().Substring(0, 10) : String.Empty) + "' />";
                eventos += "                     <span class='input-group-addon' style='border-radius: 0px !important; padding: 11px 12px !important;'><i class='glyphicon glyphicon-calendar'></i></span>";
                eventos += "                  </div>";
                eventos += "                  <label id='" + String.Format("ProcessoEvento{0}PrazoEvento2Validate", IdEvento) + "' name='" + String.Format("ProcessoEvento{0}PrazoEvento2Validate", IdEvento) + "' for='" + String.Format("ProcessoEvento{0}PrazoEvento2", IdEvento) + "' class='error hidden'>Por favor, informe um prazo maior que o primeiro prazo.</label>";
                eventos += "               </div>";
                eventos += "            </div>";
                eventos += "         </div>";
                eventos += "         <div class='row'>";
                eventos += "            <div class='col-md-12'>";
                eventos += "               <div class='form-group'>";
                eventos += "                  <label class='titlefield'>PDF <label class='titlefieldcomment'> <span class='red'>(tamanho máximo de 10MB por arquivo)</label></label>";
                eventos += "                  <div class='" + String.Format("ProcessoEvento{0}Upload", IdEvento) + "'>";
                eventos += "                     <input type='button' class='" + String.Format("ProcessoEvento{0}UploadButton", IdEvento) + "' value='Selecione o arquivo' onclick='$(" + '"' + String.Format("#ProcessoEvento{0}FileUpload1", IdEvento) + '"' + ").click();' />";
                eventos += "                     <input type='file' id='" + String.Format("ProcessoEvento{0}FileUpload1", IdEvento) + "' name='" + String.Format("ProcessoEvento{0}Upload1", IdEvento) + "' accept='application/pdf' value='10485760' />";

                if (evento != null && evento.ID == IdEvento)
                {
                    var file1 = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-1.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()));
                    if (System.IO.File.Exists(file1))
                    {
                        eventos += "                     <span id='" + String.Format("ProcessoEvento{0}File1Name", IdEvento) + "'><a style='cursor: pointer;' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-1.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()) + '"' + ");'>" + String.Format("{0}-{1}-1.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a id='" + String.Format("ButtonProcessoEventoPDF{0}Excluir1", IdEvento) + "' class='btn btn-danger btn-xs' onclick='ExcluirPDF(1, " + IdEvento + ")'>Apagar</a>&nbsp;&nbsp;&nbsp;<label id='" + String.Format("LabelProcessoEventoFile{0}ExcluirConfirmacao1", IdEvento) + "' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse PDF?&nbsp;&nbsp;&nbsp;&nbsp;</label><a id='" + String.Format("accept1File{0}", IdEvento) + "' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirPDFSim(1, " + IdEvento + ", " + '"' + String.Format("{0}-{1}-1.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()) + '"' + ")'>Sim</a>&nbsp;<a id='" + String.Format("go_back1File{0}", IdEvento) + "' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirPDFNao(1, " + IdEvento + ")''>Não</a></span>";
                    }
                    else
                    {
                        eventos += "                     <span id='" + String.Format("ProcessoEvento{0}File1Name", IdEvento) + "'>Nenhum arquivo encontrado</span>";
                    }
                }
                else
                {
                    eventos += "                     <span id='" + String.Format("ProcessoEvento{0}File1Name", IdEvento) + "'>Nenhum arquivo selecionado</span>";
                }

                eventos += "                  </div>";
                eventos += "                  <label id='" + String.Format("ProcessoEvento{0}FileUpload1Validate", IdEvento) + "' name='" + String.Format("ProcessoEvento{0}FileUpload1Validate", IdEvento) + "' for='" + String.Format("ProcessoEvento{0}FileUpload1", IdEvento) + "' class='error hidden'>Por favor, informe um arquivo com até 10MB.</label>";
                eventos += "                  <br />";

                eventos += "                  <div class='" + String.Format("ProcessoEvento{0}Upload", IdEvento) + "'>";
                eventos += "                     <input type='button' class='" + String.Format("ProcessoEvento{0}UploadButton", IdEvento) + "' value='Selecione o arquivo' onclick='$(" + '"' + String.Format("#ProcessoEvento{0}FileUpload2", IdEvento) + '"' + ").click();' />";
                eventos += "                     <input type='file' id='" + String.Format("ProcessoEvento{0}FileUpload2", IdEvento) + "' name='" + String.Format("ProcessoEvento{0}Upload2", IdEvento) + "' accept='application/pdf' value='10485760' />";

                if (evento != null && evento.ID == IdEvento)
                {
                    var file2 = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-2.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()));
                    if (System.IO.File.Exists(file2))
                    {
                        eventos += "                     <span id='" + String.Format("ProcessoEvento{0}File2Name", IdEvento) + "'><a style='cursor: pointer;' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-2.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()) + '"' + ");'>" + String.Format("{0}-{1}-2.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a id='" + String.Format("ButtonProcessoEventoPDF{0}Excluir2", IdEvento) + "' class='btn btn-danger btn-xs' onclick='ExcluirPDF(2, " + IdEvento + ")'>Apagar</a>&nbsp;&nbsp;&nbsp;<label id='" + String.Format("LabelProcessoEventoFile{0}ExcluirConfirmacao2", IdEvento) + "' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse PDF?&nbsp;&nbsp;&nbsp;&nbsp;</label><a id='" + String.Format("accept2File{0}", IdEvento) + "' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirPDFSim(2, " + IdEvento + ", " + '"' + String.Format("{0}-{1}-2.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()) + '"' + ")'>Sim</a>&nbsp;<a id='" + String.Format("go_back2File{0}", IdEvento) + "' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirPDFNao(2, " + IdEvento + ")''>Não</a></span>";
                    }
                    else
                    {
                        eventos += "                     <span id='" + String.Format("ProcessoEvento{0}File2Name", IdEvento) + "'>Nenhum arquivo encontrado</span>";
                    }
                }
                else
                {
                    eventos += "                     <span id='" + String.Format("ProcessoEvento{0}File2Name", IdEvento) + "'>Nenhum arquivo selecionado</span>";
                }

                eventos += "                  </div>";
                eventos += "                  <label id='" + String.Format("ProcessoEvento{0}FileUpload2Validate", IdEvento) + "' name='" + String.Format("ProcessoEvento{0}FileUpload2Validate", IdEvento) + "' for='" + String.Format("ProcessoEvento{0}FileUpload2", IdEvento) + "' class='error hidden'>Por favor, informe um arquivo com até 10MB.</label>";
                eventos += "                  <br />";

                eventos += "                  <div class='" + String.Format("ProcessoEvento{0}Upload", IdEvento) + "'>";
                eventos += "                     <input type='button' class='" + String.Format("ProcessoEvento{0}UploadButton", IdEvento) + "' value='Selecione o arquivo' onclick='$(" + '"' + String.Format("#ProcessoEvento{0}FileUpload3", IdEvento) + '"' + ").click();' />";
                eventos += "                     <input type='file' id='" + String.Format("ProcessoEvento{0}FileUpload3", IdEvento) + "' name='" + String.Format("ProcessoEvento{0}Upload3", IdEvento) + "' accept='application/pdf' value='10485760' />";

                if (evento != null && evento.ID == IdEvento)
                {
                    var file3 = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-3.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()));
                    if (System.IO.File.Exists(file3))
                    {
                        eventos += "                     <span id='" + String.Format("ProcessoEvento{0}File3Name", IdEvento) + "'><a style='cursor: pointer;' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-3.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()) + '"' + ");'>" + String.Format("{0}-{1}-3.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a id='" + String.Format("ButtonProcessoEventoPDF{0}Excluir3", IdEvento) + "' class='btn btn-danger btn-xs' onclick='ExcluirPDF(3, " + IdEvento + ")'>Apagar</a>&nbsp;&nbsp;&nbsp;<label id='" + String.Format("LabelProcessoEventoFile{0}ExcluirConfirmacao3", IdEvento) + "' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse PDF?&nbsp;&nbsp;&nbsp;&nbsp;</label><a id='" + String.Format("accept3File{0}", IdEvento) + "' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirPDFSim(3, " + IdEvento + ", " + '"' + String.Format("{0}-{1}-3.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()) + '"' + ")'>Sim</a>&nbsp;<a id='" + String.Format("go_back3File{0}", IdEvento) + "' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirPDFNao(3, " + IdEvento + ")''>Não</a></span>";
                    }
                    else
                    {
                        eventos += "                     <span id='" + String.Format("ProcessoEvento{0}File3Name", IdEvento) + "'>Nenhum arquivo encontrado</span>";
                    }
                }
                else
                {
                    eventos += "                     <span id='" + String.Format("ProcessoEvento{0}File3Name", IdEvento) + "'>Nenhum arquivo selecionado</span>";
                }

                eventos += "                  </div>";
                eventos += "                  <label id='" + String.Format("ProcessoEvento{0}FileUpload3Validate", IdEvento) + "' name='" + String.Format("ProcessoEvento{0}FileUpload3Validate", IdEvento) + "' for='" + String.Format("ProcessoEvento{0}FileUpload3", IdEvento) + "' class='error hidden'>Por favor, informe um arquivo com até 10MB.</label>";
                eventos += "                  <br />";

                eventos += "                  <div class='" + String.Format("ProcessoEvento{0}Upload", IdEvento) + "'>";
                eventos += "                     <input type='button' class='" + String.Format("ProcessoEvento{0}UploadButton", IdEvento) + "' value='Selecione o arquivo' onclick='$(" + '"' + String.Format("#ProcessoEvento{0}FileUpload4", IdEvento) + '"' + ").click();' />";
                eventos += "                     <input type='file' id='" + String.Format("ProcessoEvento{0}FileUpload4", IdEvento) + "' name='" + String.Format("ProcessoEvento{0}Upload4", IdEvento) + "' accept='application/pdf' value='10485760' />";

                if (evento != null && evento.ID == IdEvento)
                {
                    var file4 = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-4.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()));
                    if (System.IO.File.Exists(file4))
                    {
                        eventos += "                     <span id='" + String.Format("ProcessoEvento{0}File4Name", IdEvento) + "'><a style='cursor: pointer;' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-4.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()) + '"' + ");'>" + String.Format("{0}-{1}-4.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a id='" + String.Format("ButtonProcessoEventoPDF{0}Excluir4", IdEvento) + "' class='btn btn-danger btn-xs' onclick='ExcluirPDF(4, " + IdEvento + ")'>Apagar</a>&nbsp;&nbsp;&nbsp;<label id='" + String.Format("LabelProcessoEventoFile{0}ExcluirConfirmacao4", IdEvento) + "' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse PDF?&nbsp;&nbsp;&nbsp;&nbsp;</label><a id='" + String.Format("accept4File{0}", IdEvento) + "' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirPDFSim(4, " + IdEvento + ", " + '"' + String.Format("{0}-{1}-4.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()) + '"' + ")'>Sim</a>&nbsp;<a id='" + String.Format("go_back4File{0}", IdEvento) + "' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirPDFNao(4, " + IdEvento + ")''>Não</a></span>";
                    }
                    else
                    {
                        eventos += "                     <span id='" + String.Format("ProcessoEvento{0}File4Name", IdEvento) + "'>Nenhum arquivo encontrado</span>";
                    }
                }
                else
                {
                    eventos += "                     <span id='" + String.Format("ProcessoEvento{0}File4Name", IdEvento) + "'>Nenhum arquivo selecionado</span>";
                }

                eventos += "                  </div>";
                eventos += "                  <label id='" + String.Format("ProcessoEvento{0}FileUpload4Validate", IdEvento) + "' name='" + String.Format("ProcessoEvento{0}FileUpload4Validate", IdEvento) + "' for='" + String.Format("ProcessoEvento{0}FileUpload4", IdEvento) + "' class='error hidden'>Por favor, informe um arquivo com até 10MB.</label>";
                eventos += "                  <br />";

                eventos += "                  <div class='" + String.Format("ProcessoEvento{0}Upload", IdEvento) + "'>";
                eventos += "                     <input type='button' class='" + String.Format("ProcessoEvento{0}UploadButton", IdEvento) + "' value='Selecione o arquivo' onclick='$(" + '"' + String.Format("#ProcessoEvento{0}FileUpload5", IdEvento) + '"' + ").click();' />";
                eventos += "                     <input type='file' id='" + String.Format("ProcessoEvento{0}FileUpload5", IdEvento) + "' name='" + String.Format("ProcessoEvento{0}Upload5", IdEvento) + "' accept='application/pdf' value='10485760' />";

                if (evento != null && evento.ID == IdEvento)
                {
                    var file5 = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-5.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()));
                    if (System.IO.File.Exists(file5))
                    {
                        eventos += "                     <span id='" + String.Format("ProcessoEvento{0}File5Name", IdEvento) + "'><a style='cursor: pointer;' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-5.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()) + '"' + ");'>" + String.Format("{0}-{1}-5.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a id='" + String.Format("ButtonProcessoEventoPDF{0}Excluir5", IdEvento) + "' class='btn btn-danger btn-xs' onclick='ExcluirPDF(5, " + IdEvento + ")'>Apagar</a>&nbsp;&nbsp;&nbsp;<label id='" + String.Format("LabelProcessoEventoFile{0}ExcluirConfirmacao5", IdEvento) + "' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse PDF?&nbsp;&nbsp;&nbsp;&nbsp;</label><a id='" + String.Format("accept5File{0}", IdEvento) + "' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirPDFSim(5, " + IdEvento + ", " + '"' + String.Format("{0}-{1}-5.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()) + '"' + ")'>Sim</a>&nbsp;<a id='" + String.Format("go_back5File{0}", IdEvento) + "' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirPDFNao(5, " + IdEvento + ")''>Não</a></span>";
                    }
                    else
                    {
                        eventos += "                     <span id='" + String.Format("ProcessoEvento{0}File5Name", IdEvento) + "'>Nenhum arquivo encontrado</span>";
                    }
                }
                else
                {
                    eventos += "                     <span id='" + String.Format("ProcessoEvento{0}File5Name", IdEvento) + "'>Nenhum arquivo selecionado</span>";
                }

                eventos += "                  </div>";
                eventos += "                  <label id='" + String.Format("ProcessoEvento{0}FileUpload5Validate", IdEvento) + "' name='" + String.Format("ProcessoEvento{0}FileUpload5Validate", IdEvento) + "' for='" + String.Format("ProcessoEvento{0}FileUpload5", IdEvento) + "' class='error hidden'>Por favor, informe um arquivo com até 10MB.</label>";
                eventos += "                  <br />";

                eventos += "               </div>";
                eventos += "            </div>";
                eventos += "         </div>";
                eventos += "         <div class='row'>";
                eventos += "            <div class='col-md-12'>";
                eventos += "               <div class='form-group'>";
                eventos += "                  <label class='titlefield'>Descrição <label class='titlefieldcomment'> <span class='red'>(tamanho máximo do texto de 1MB)</span></label></label>";
                eventos += "                  <textarea id='" + String.Format("ProcessoEvento{0}Descricao", IdEvento) + "' name='" + String.Format("ProcessoEvento{0}Descricao", IdEvento) + "' class='form-control placeholder' placeholder='Informe aqui a Descrição'>" + (evento != null && evento.ID == IdEvento && evento.Descricao != null ? Uri.UnescapeDataString(evento.Descricao) : string.Empty) + "</textarea>";
                eventos += "                  <label id='" + String.Format("ProcessoEvento{0}DescricaoValidate", IdEvento) + "' name='" + String.Format("ProcessoEvento{0}DescricaoValidate", IdEvento) + "' for='" + String.Format("ProcessoEvento{0}Descricao", IdEvento) + "' class='error hidden'>Por favor, informe uma descrição com até 1MB.</label>";
                eventos += "               </div>";
                eventos += "            </div>";
                eventos += "         </div>";
                eventos += "         <div class='row'>";
                eventos += "            <div class='col-md-4'>";
                eventos += "               <button type='button' id='" + String.Format("ButtonProcessoEvento{0}Incluir", IdEvento) + "' name='" + String.Format("ButtonProcessoEvento{0}Incluir", IdEvento) + "' class='btn btn-primary' data-loading-text='Salvando...' onclick='ButtonProcessoEventoSalvar_Click(" + IdEvento + ");'>";
                eventos += "                  Salvar";
                eventos += "               </button>";
                eventos += "            </div>";
                eventos += "            <div class='col-md-4' style='text-align: center;'>";
                eventos += "            </div>";
                eventos += "            <div class='col-md-4' style='text-align: right;'>";
                eventos += "              <button type='button' id='" + String.Format("ButtonProcessoEvento{0}Excluir", IdEvento) + "' name='" + String.Format("ButtonProcessoEvento{0}Excluir", IdEvento) + "' class='btn btn-danger' data-loading-text='Apagando...' onclick='ButtonProcessoEventoExcluir_Click(" + IdEvento + ");' " + (evento != null && evento.ID == IdEvento ? String.Empty : "disabled='disabled'") + ">";
                eventos += "                 Apagar";
                eventos += "              </button>";
                eventos += "              <div id='" + String.Format("ButtonProcessoEvento{0}ExcluirConfirmacao", IdEvento) + "' style='display:none'>";
                eventos += "                 <label id='" + String.Format("LabelProcessoEvento{0}ExcluirConfirmacao", IdEvento) + "' style='font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente excluir esse Evento?</label><br />";
                eventos += "                 <button id='" + String.Format("accept{0}", IdEvento) + "' class='btn btn-default btn-xs' value='true'>Sim</button>";
                eventos += "                 <button id='" + String.Format("go_back{0}", IdEvento) + "' class='btn btn-primary btn-xs' value='false'>Não</button>";
                eventos += "              </div>";
                eventos += "            </div>";
                eventos += "         </div>";

                eventos += "<script>";
                eventos += "    $('" + String.Format("#ProcessoEvento{0}FileUpload1", (evento != null && evento.ID == IdEvento ? IdEvento.ToString() : "0")) + "').change(function (e) {";
                eventos += "        $in = $(this);";
                eventos += "        $in.next().html($in.val());";
                eventos += "    });";
                eventos += "    $('" + String.Format("#ProcessoEvento{0}FileUpload2", (evento != null && evento.ID == IdEvento ? IdEvento.ToString() : "0")) + "').change(function (e) {";
                eventos += "        $in = $(this);";
                eventos += "        $in.next().html($in.val());";
                eventos += "    });";
                eventos += "    $('" + String.Format("#ProcessoEvento{0}FileUpload3", (evento != null && evento.ID == IdEvento ? IdEvento.ToString() : "0")) + "').change(function (e) {";
                eventos += "        $in = $(this);";
                eventos += "        $in.next().html($in.val());";
                eventos += "    });";
                eventos += "    $('" + String.Format("#ProcessoEvento{0}FileUpload4", (evento != null && evento.ID == IdEvento ? IdEvento.ToString() : "0")) + "').change(function (e) {";
                eventos += "        $in = $(this);";
                eventos += "        $in.next().html($in.val());";
                eventos += "    });";
                eventos += "    $('" + String.Format("#ProcessoEvento{0}FileUpload5", (evento != null && evento.ID == IdEvento ? IdEvento.ToString() : "0")) + "').change(function (e) {";
                eventos += "        $in = $(this);";
                eventos += "        $in.next().html($in.val());";
                eventos += "    });";
                eventos += "</script>";

                //var result = new { response = "success", eventos = eventos.Replace("<s ", string.Empty).Replace("</s>", string.Empty) };
                var result = new { response = "success", eventos = eventos };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult BuscarProcessosEvento(Int64 IdEvento)
        {
            try
            {
                String profissional;
                var evento = ProcessosEventosBo.VisualizarEvento(IdEvento);
                String eventos = String.Empty;

                var Profissional = ProcessosAgendasProfissionaisBo.ProfissionalAgenda(evento.ID);
                profissional = Profissional != null && Profissional.Profissionais !=null ? Profissional.Profissionais.Pessoas.Nome.Trim() : string.Empty;

                eventos += "<table id='EventoDetalhe_" + evento.ID + "' width='100%'>";
                eventos += "   <thead>";
                eventos += "      <tr>";
                eventos += "         <th>Data</th>";
                eventos += "         <th>Evento</th>";
                eventos += "         <th>Agendado Para</th>";
                eventos += "         <th>Data 1º Prazo</th>";
                eventos += "         <th>Data 2º Prazo</th>";
                eventos += "         <th>Usuário</th>";
                eventos += "         <th>Usuário Alteração</th>";
                eventos += "         <th>Data Alteração</th>";
                eventos += "         <th>PDF</th>";
                eventos += "      </tr>";
                eventos += "   </thead>";
                eventos += "   <tbody>";
                eventos += "      <tr>";
                eventos += "         <td>" + (evento.Data != null ? evento.Data.Value.ToString("dd/MM/yyyy") : string.Empty) + "</td>";
                eventos += "         <td>" + evento.ProcessosEventos.Descricao + "</td>";
                eventos += "         <td>" + profissional + "</td>";
                eventos += "         <td>" + (evento.PrazoEvento1 != null ? evento.PrazoEvento1.Value.ToShortDateString() : string.Empty) + "</td>";
                eventos += "         <td>" + (evento.PrazoEvento2 != null ? evento.PrazoEvento2.Value.ToShortDateString() : string.Empty) + "</td>";
                eventos += "         <td>" + (evento.Usuarios != null ? evento.Usuarios.Nome : string.Empty) + "</td>";
                eventos += "         <td>" + (evento.Usuarios1 != null ? evento.Usuarios1.Nome : string.Empty) + "</td>";
                eventos += "         <td>" + (evento.DataAlteracao != null ? evento.DataAlteracao.Value.ToString("dd/MM/yyyy") : string.Empty) + "</td>";

                var files = String.Empty;

                var file = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-1.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()));
                if (System.IO.File.Exists(file))
                {
                    files += "         <a class='btn btn-default btn-xs' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-1.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()) + '"' + ");'>Visualizar 1</a>";
                }

                file = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-2.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()));
                if (System.IO.File.Exists(file))
                {
                    files += "         <a class='btn btn-default btn-xs' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-2.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()) + '"' + ");'>Visualizar 2</a>";
                }

                file = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-3.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()));
                if (System.IO.File.Exists(file))
                {
                    files += "         <a class='btn btn-default btn-xs' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-3.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()) + '"' + ");'>Visualizar 3</a>";
                }

                file = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-4.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()));
                if (System.IO.File.Exists(file))
                {
                    files += "         <a class='btn btn-default btn-xs' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-4.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()) + '"' + ");'>Visualizar 4</a>";
                }

                file = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-5.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()));
                if (System.IO.File.Exists(file))
                {
                    files += "         <a class='btn btn-default btn-xs' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-5.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()) + '"' + ");'>Visualizar 5</a>";
                }

                if (files.Length > 0)
                {
                    eventos += "         <td>";
                    eventos += "             " + files;
                    eventos += "         </td>";
                }
                else
                {
                    eventos += "         <td>Nenhum arquivo encontrado</td>";
                }

                eventos += "      </tr>";
                eventos += "   </tbody>";
                eventos += "</table>";
                eventos += "<br />";
                eventos += "<table id='EventoDescricao_" + evento.ID + "' width='100%'>";
                eventos += "   <thead>";
                eventos += "      <tr>";
                eventos += "         <th>Descrição</th>";
                eventos += "      </tr>";
                eventos += "   </thead>";
                eventos += "   <tbody>";
                eventos += "      <tr>";
                eventos += "         <td>" + Uri.UnescapeDataString(evento.Descricao ?? String.Empty) + "</td>";
                eventos += "         <td></td>";
                eventos += "      </tr>";
                eventos += "   </tbody>";
                eventos += "</table>";

                if (Sessao.Usuario.ID == evento.IdUsuario || Sessao.Usuario.ID == evento.IdUsuarioAlteracao || Sessao.Usuario.Nivel == 5)
                {
                    eventos += "<a id='ButtonAlterarEvento_" + evento.ID + "' name='ButtonAlterarEvento' class='btn btn-primary alterar-evento' data-backdrop='static' data-toggle='modal' data-target='#modalNovoEvento' onclick='AlterarEvento(" + evento.ID + ");'>Alterar</a>";
                }

                var result = new { response = "success", eventos = eventos };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult SalvarProcessosEventos(Int32 Id, Int64? IdProcessosEventoPendente, String Data, String Origem, Int32 IdProcessosAcao, 
            Int32 IdProcessosEvento, Int32 IdProfissional, String Prazo1, String Prazo2, String Descricao, 
            HttpPostedFileBase Arquivo1, HttpPostedFileBase Arquivo2, HttpPostedFileBase Arquivo3, HttpPostedFileBase Arquivo4, HttpPostedFileBase Arquivo5, 
            Int32 IdAgendaProfissional, Boolean Excluido = false)
        {
            try
            {
                if (IdProcessosEventoPendente == null)
                {
                    IdProcessosEventoPendente = 0;
                }

                #region Ação
                ProcessosAcoes acoes = ProcessosAcoesBo.Consultar(IdProcessosAcao);

                if (Id == 0 && acoes != null && acoes.Status == "Baixado")
                {
                    return Json(new { response = "warning" });
                }
                #endregion

                #region Evento
                ProcessosEventos eventos = ProcessosEventosBo.Consultar(IdProcessosEvento);
                #endregion

                #region Profissional
                Profissionais profissionais = ProfissionaisBo.Consultar(IdProfissional);
                #endregion

                #region Auditoria

                var nomeautor = ProcessosAcoesBo.BuscarNomeCliente(acoes.ID, "A");
                var nomereu = ProcessosAcoesBo.BuscarNomeCliente(acoes.ID, "R");

                var fileName = String.Empty;
                if (Arquivo1 != null && Arquivo1.ContentLength > 0)
                {
                    fileName += Path.GetFileName(Arquivo1.FileName);
                }

                if (Arquivo2 != null && Arquivo2.ContentLength > 0)
                {
                    fileName += (fileName.Length > 0 ? ", " : String.Empty) + Path.GetFileName(Arquivo2.FileName);
                }

                if (Arquivo3 != null && Arquivo3.ContentLength > 0)
                {
                    fileName += (fileName.Length > 0 ? ", " : String.Empty) + Path.GetFileName(Arquivo3.FileName);
                }

                if (Arquivo4 != null && Arquivo4.ContentLength > 0)
                {
                    fileName += (fileName.Length > 0 ? ", " : String.Empty) + Path.GetFileName(Arquivo4.FileName);
                }

                if (Arquivo5 != null && Arquivo5.ContentLength > 0)
                {
                    fileName += (fileName.Length > 0 ? ", " : String.Empty) + Path.GetFileName(Arquivo5.FileName);
                }

                if (fileName.Length == 0)
                {
                    fileName = "Nenhum arquivo pdf enviado para o servidor.";
                }

                Auditoria auditoria = new Auditoria();
                auditoria.DataHora = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                auditoria.Modulo = "Processo";
                auditoria.Tipo = "Evento";
                auditoria.Acao = "Alterado";
                auditoria.Log = String.Format("<b>Origem</b>: {0};<b>Número do Processo</b>: {1};<b>Evento</b>: {2};<b>Profissional</b>: {3};<b>Data 1º Prazo</b>: {4};<b>Data 2º Prazo</b>: {5};<b>Arquivo PDF/b>: {6};<b>Descrição/b>: {7};<b>Autor/b>: {8};<b>Réu/b>: {9};", Origem, acoes?.NumeroProcesso, eventos.Descricao, (profissionais != null ? profissionais.Pessoas.Nome : string.Empty), Prazo1, Prazo2, fileName, Descricao, nomeautor, nomereu);
                auditoria.Usuario = Sessao.Usuario.Nome;

                if (Id == 0)
                {
                    auditoria.Acao = "Inserido";
                }

                if (IdProcessosEventoPendente > 0)
                {
                    auditoria.Acao = "Capturado";
                }

                AuditoriaBo.Inserir(auditoria);
                #endregion

                #region Ações Eventos
                ProcessosAcoesEventos acao = new ProcessosAcoesEventos();
                acao.ID = Id;
                acao.IdProcessosAcao = IdProcessosAcao;
                acao.IdProcessosEvento = IdProcessosEvento;
                acao.Data = DateTime.Now;
                acao.DataAlteracao = DateTime.Now;
                acao.Descricao = Descricao;
                if (IdAgendaProfissional > 0 && Excluido)
                {
                    acao.DataCumprimentoPrazo = DateTime.Now;
                }

                if (!String.IsNullOrEmpty(Prazo1))
                {
                    acao.PrazoEvento1 = DateTime.Parse(Prazo1);
                }

                if (!String.IsNullOrEmpty(Prazo2))
                {
                    acao.PrazoEvento2 = DateTime.Parse(Prazo2);
                }

                acao.IdProcessosEventoPendente = IdProcessosEventoPendente;
                acao.IdUsuario = Sessao.Usuario.ID;
                acao.IdUsuarioAlteracao = Sessao.Usuario.ID;

                if (Id == 0 || IdProcessosEventoPendente > 0)
                    Id = ProcessosAcoesEventosBo.Inserir(acao);

                ProcessosAcoesEventosBo.Salvar(acao);
                #endregion

                #region Agenda Profissional
                //if (((IdAgendaProfissional == 0 && auditoria.Acao == "Alterado") || auditoria.Acao == "Capturado" || auditoria.Acao == "Inserido") && IdProfissional > 0 && !String.IsNullOrEmpty(Prazo1) && !Excluido)
                if (((IdAgendaProfissional == 0 && auditoria.Acao == "Alterado") || auditoria.Acao == "Capturado" || auditoria.Acao == "Inserido") && !Excluido)
                {
                    ProcessosAgendaProfissional agenda = new ProcessosAgendaProfissional();
                    agenda.ID = 0;
                    //agenda.IdProfissional = IdProfissional;
                    if (IdProfissional == 0)
                    {
                        agenda.IdProfissional = null;
                    }
                    else
                        agenda.IdProfissional = IdProfissional;

                    agenda.IdProcessosEvento = Id;
                    agenda.Excluido = false;
                    agenda.DataCadastro = DateTime.Now;
                    agenda.NomeUsuarioCadastro = Sessao.Usuario.Nome;

                    IdAgendaProfissional = ProcessosAgendasProfissionaisBo.Inserir(agenda);
                }
                //else if (IdAgendaProfissional != 0 && auditoria.Acao == "Alterado"  && !String.IsNullOrEmpty(Prazo1) && !Excluido)
                else if (IdAgendaProfissional != 0 && auditoria.Acao == "Alterado"  && !Excluido)
                {
                    ProcessosAgendaProfissional agenda = new ProcessosAgendaProfissional();
                    agenda.ID = IdAgendaProfissional;
                    //agenda.IdProfissional = IdProfissional;
                    if (IdProfissional == 0) {
                        agenda.IdProfissional = null;
                    }else
                        agenda.IdProfissional = IdProfissional;
                    agenda.IdProcessosEvento = Id;
                    agenda.Excluido = false;
                    agenda.DataCadastro = DateTime.Now;
                    agenda.NomeUsuarioCadastro = Sessao.Usuario.Nome;
                    agenda.DataAlteracao = DateTime.Now;
                    agenda.NomeUsuarioAlteracao = Sessao.Usuario.Nome;

                    ProcessosAgendasProfissionaisBo.Salvar(agenda);
                }
                else
                {
                    if (IdAgendaProfissional > 0 && Excluido)
                    {
                        ProcessosAgendaProfissional agenda = ProcessosAgendasProfissionaisBo.Buscar(IdAgendaProfissional);
                        agenda.Excluido = true;
                        ProcessosAgendasProfissionaisBo.Salvar(agenda);
                    }
                }


                #endregion

                #region Arquivo PDF
                var url = String.Empty;
                if (Arquivo1 != null && Arquivo1.ContentLength > 0)
                {
                    var file = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-1.pdf", Util.OnlyNumbers(acoes.NumeroProcesso), Id.ToString()));
                    Arquivo1.SaveAs(file);

                    if (System.IO.File.Exists(file))
                    {
                        url += "         <td><a class='btn btn-default btn-xs' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-1.pdf", Util.OnlyNumbers(acoes.NumeroProcesso), Id.ToString()) + '"' + ");'>Visualizar 1</a></td>";
                    }
                }

                if (Arquivo2 != null && Arquivo2.ContentLength > 0)
                {
                    var file = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-2.pdf", Util.OnlyNumbers(acoes.NumeroProcesso), Id.ToString()));
                    Arquivo2.SaveAs(file);

                    if (System.IO.File.Exists(file))
                    {
                        url += "         <td><a class='btn btn-default btn-xs' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-2.pdf", Util.OnlyNumbers(acoes.NumeroProcesso), Id.ToString()) + '"' + ");'>Visualizar 2</a></td>";
                    }
                }

                if (Arquivo3 != null && Arquivo3.ContentLength > 0)
                {
                    var file = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-3.pdf", Util.OnlyNumbers(acoes.NumeroProcesso), Id.ToString()));
                    Arquivo3.SaveAs(file);

                    if (System.IO.File.Exists(file))
                    {
                        url += "         <td><a class='btn btn-default btn-xs' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-3.pdf", Util.OnlyNumbers(acoes.NumeroProcesso), Id.ToString()) + '"' + ");'>Visualizar 3</a></td>";
                    }
                }

                if (Arquivo4 != null && Arquivo4.ContentLength > 0)
                {
                    var file = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-4.pdf", Util.OnlyNumbers(acoes.NumeroProcesso), Id.ToString()));
                    Arquivo4.SaveAs(file);

                    if (System.IO.File.Exists(file))
                    {
                        url += "         <td><a class='btn btn-default btn-xs' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-4.pdf", Util.OnlyNumbers(acoes.NumeroProcesso), Id.ToString()) + '"' + ");'>Visualizar 4</a></td>";
                    }
                }

                if (Arquivo5 != null && Arquivo5.ContentLength > 0)
                {
                    var file = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-5.pdf", Util.OnlyNumbers(acoes.NumeroProcesso), Id.ToString()));
                    Arquivo5.SaveAs(file);

                    if (System.IO.File.Exists(file))
                    {
                        url += "         <td><a class='btn btn-default btn-xs' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-5.pdf", Util.OnlyNumbers(acoes.NumeroProcesso), Id.ToString()) + '"' + ");'>Visualizar 5</a></td>";
                    }
                }

                if (url.Length == 0)
                {
                    url = "         <td>Nenhum arquivo encontrado</td>";
                }
                #endregion

                #region Eventos Pendentes
                if (IdProcessosEventoPendente > 0)
                {
                    ProcessosEventosPendentes evento = new ProcessosEventosPendentes();
                    evento.Id = IdProcessosEventoPendente.Value;
                    evento.Origem = Origem;
                    evento.Integrado = true;

                    ProcessosEventosPendentesBo.Salvar(evento);
                }
                #endregion

                var result = new { response = "success", id = Id, idagenda = IdAgendaProfissional, url = url, message="Dados cadastrados com sucesso." };
                return Json(result);
            }
            catch (Exception ex)
            {
                String error = "error";
                String message = (ex.InnerException != null ? ex.InnerException.ToString() : ex.Message.ToString());

                if (message.Contains("chave duplicada no objeto"))
                    error = "warning";

                var result = new { response = error };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult ExcluirProcessosEventos(Int32 Id)
        {
            try
            {
                // tratar retorno da mensagem
                ProcessosAcoesEventos evento = ProcessosAcoesEventosBo.Consultar(Id);

                if (Sessao.Usuario.Nivel >= 5)
                {
                    return ExcluirEvento(Id, evento);
                }
                else
                {
                    if (Sessao.Usuario.ID == evento.IdUsuario && evento.Data.Value > DateTime.Today.AddDays(-1))
                    {
                        return ExcluirEvento(Id, evento);
                    }
                    else
                    {
                        var result = new { response = "errorx" };
                        return Json(result);
                    }
                }
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        private JsonResult ExcluirEvento(int Id, ProcessosAcoesEventos evento)
        {
            #region Auditoria

            var fileName = string.Empty;

            if (evento != null)
            {
                var file = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString()));

                if (System.IO.File.Exists(file))
                {
                    fileName = String.Format("{0}-{1}.pdf", Util.OnlyNumbers(evento.ProcessosAcoes.NumeroProcesso), evento.ID.ToString());
                    System.IO.File.Delete(file);
                }

                Auditoria auditoria = new Auditoria();
                auditoria.DataHora = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                auditoria.Modulo = "Processo";
                auditoria.Tipo = "Ação";
                auditoria.Acao = "Excluído";
                auditoria.Log = String.Format("<b>Origem</b>: {0};<b>Número do Processo</b>: {1};<b>Evento</b>: {2};<b>Profissional</b>: {3};<b>Data 1º Prazo</b>: {4};<b>Data 2º Prazo</b>: {5};<b>Arquivo PDF/b>: {6};<b>Descrição/b>: {7};", string.Empty, evento.ProcessosAcoes.NumeroProcesso, evento.ProcessosEventos.Descricao, string.Empty, evento.PrazoEvento1, evento.PrazoEvento2, fileName, evento.Descricao);
                auditoria.Usuario = Sessao.Usuario.Nome;

                AuditoriaBo.Inserir(auditoria);
            }
            #endregion

            var response = ProcessosAcoesEventosBo.Excluir(Id);

            if (response == "00")
            {
                var result = new { response = "removed" };
                return Json(result);
            }
            else
            {
                var result = new { response = "error2" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult ConsultaAgendaProfissional(Int32 Id)
        {
            var agenda = ProcessosAgendasProfissionaisBo.ConsultarArray(Id);

            var jsonString = JsonConvert.SerializeObject(agenda);

            dynamic data = JObject.Parse(jsonString.Replace("[", "").Replace("]", ""));

            #region Arquivo PDF
            var pdf = String.Empty;
            var arquivo1 = "Nenhum arquivo encontrado";
            var arquivo2 = "Nenhum arquivo encontrado";
            var arquivo3 = "Nenhum arquivo encontrado";
            var arquivo4 = "Nenhum arquivo encontrado";
            var arquivo5 = "Nenhum arquivo encontrado";

            var file1 = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-1.pdf", Util.OnlyNumbers(data.NumeroProcesso.Value.ToString()), (data.IdProcessosEvento.Value != null ? data.IdProcessosEvento.Value.ToString() : string.Empty)));
            if (System.IO.File.Exists(file1))
            {
                pdf += "<a class='btn btn-default' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-1.pdf", Util.OnlyNumbers(data.NumeroProcesso.Value.ToString()), data.IdProcessosEvento.Value.ToString()) + '"' + ");'>Visualizar 1</a>";
                arquivo1 = "<a style='cursor: pointer;' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-1.pdf", Util.OnlyNumbers(data.NumeroProcesso.Value.ToString()), data.IdProcessosEvento.Value.ToString()) + '"' + ");'>" + String.Format("{0}-{1}-1.pdf", Util.OnlyNumbers(data.NumeroProcesso.Value.ToString()), data.IdProcessosEvento.Value.ToString()) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;<a id='" + String.Format("ButtonProcessoEventoPDF{0}Excluir1", "") + "' class='btn btn-danger btn-xs' onclick='ExcluirPDF(1, " + '"' + '"' + ")'>Apagar</a>&nbsp;&nbsp;<label id='" + String.Format("LabelProcessoEventoFile{0}ExcluirConfirmacao1", "") + "' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse PDF?&nbsp;&nbsp;&nbsp;</label><a id='" + String.Format("accept1File{0}", "") + "' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirPDFSim(1, " + '"' + '"' + ", " + '"' + String.Format("{0}-{1}-1.pdf", Util.OnlyNumbers(data.NumeroProcesso.Value.ToString()), data.IdProcessosEvento.Value.ToString()) + '"' + ")'>Sim</a><a id='" + String.Format("go_back1File{0}", "") + "' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirPDFNao(1, " + '"' + '"' + ")''>Não</a>";
            }

            var file2 = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-2.pdf", Util.OnlyNumbers(data.NumeroProcesso.Value.ToString()), (data.IdProcessosEvento.Value != null ? data.IdProcessosEvento.Value.ToString() : string.Empty)));
            if (System.IO.File.Exists(file2))
            {
                pdf += "<a class='btn btn-default' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-2.pdf", Util.OnlyNumbers(data.NumeroProcesso.Value.ToString()), data.IdProcessosEvento.Value.ToString()) + '"' + ");'>Visualizar 2</a>";
                arquivo2 = "<a style='cursor: pointer;' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-2.pdf", Util.OnlyNumbers(data.NumeroProcesso.Value.ToString()), data.IdProcessosEvento.Value.ToString()) + '"' + ");'>" + String.Format("{0}-{1}-2.pdf", Util.OnlyNumbers(data.NumeroProcesso.Value.ToString()), data.IdProcessosEvento.Value.ToString()) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;<a id='" + String.Format("ButtonProcessoEventoPDF{0}Excluir2", "") + "' class='btn btn-danger btn-xs' onclick='ExcluirPDF(2, " + '"' + '"' + ")'>Apagar</a>&nbsp;&nbsp;<label id='" + String.Format("LabelProcessoEventoFile{0}ExcluirConfirmacao2", "") + "' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse PDF?&nbsp;&nbsp;&nbsp;</label><a id='" + String.Format("accept2File{0}", "") + "' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirPDFSim(2, " + '"' + '"' + ", " + '"' + String.Format("{0}-{1}-2.pdf", Util.OnlyNumbers(data.NumeroProcesso.Value.ToString()), data.IdProcessosEvento.Value.ToString()) + '"' + ")'>Sim</a><a id='" + String.Format("go_back2File{0}", "") + "' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirPDFNao(2, " + '"' + '"' + ")''>Não</a>";
            }

            var file3 = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-3.pdf", Util.OnlyNumbers(data.NumeroProcesso.Value.ToString()), (data.IdProcessosEvento.Value != null ? data.IdProcessosEvento.Value.ToString() : string.Empty)));
            if (System.IO.File.Exists(file3))
            {
                pdf += "<a class='btn btn-default' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-3.pdf", Util.OnlyNumbers(data.NumeroProcesso.Value.ToString()), data.IdProcessosEvento.Value.ToString()) + '"' + ");'>Visualizar 3</a>";
                arquivo3 = "<a style='cursor: pointer;' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-3.pdf", Util.OnlyNumbers(data.NumeroProcesso.Value.ToString()), data.IdProcessosEvento.Value.ToString()) + '"' + ");'>" + String.Format("{0}-{1}-3.pdf", Util.OnlyNumbers(data.NumeroProcesso.Value.ToString()), data.IdProcessosEvento.Value.ToString()) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;<a id='" + String.Format("ButtonProcessoEventoPDF{0}Excluir3", "") + "' class='btn btn-danger btn-xs' onclick='ExcluirPDF(3, " + '"' + '"' + ")'>Apagar</a>&nbsp;&nbsp;<label id='" + String.Format("LabelProcessoEventoFile{0}ExcluirConfirmacao3", "") + "' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse PDF?&nbsp;&nbsp;&nbsp;</label><a id='" + String.Format("accept3File{0}", "") + "' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirPDFSim(3, " + '"' + '"' + ", " + '"' + String.Format("{0}-{1}-3.pdf", Util.OnlyNumbers(data.NumeroProcesso.Value.ToString()), data.IdProcessosEvento.Value.ToString()) + '"' + ")'>Sim</a><a id='" + String.Format("go_back3File{0}", "") + "' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirPDFNao(3, " + '"' + '"' + ")''>Não</a>";
            }

            var file4 = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-4.pdf", Util.OnlyNumbers(data.NumeroProcesso.Value.ToString()), (data.IdProcessosEvento.Value != null ? data.IdProcessosEvento.Value.ToString() : string.Empty)));
            if (System.IO.File.Exists(file4))
            {
                pdf += "<a class='btn btn-default' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-4.pdf", Util.OnlyNumbers(data.NumeroProcesso.Value.ToString()), data.IdProcessosEvento.Value.ToString()) + '"' + ");'>Visualizar 4</a>";
                arquivo4 = "<a style='cursor: pointer;' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-4.pdf", Util.OnlyNumbers(data.NumeroProcesso.Value.ToString()), data.IdProcessosEvento.Value.ToString()) + '"' + ");'>" + String.Format("{0}-{1}-4.pdf", Util.OnlyNumbers(data.NumeroProcesso.Value.ToString()), data.IdProcessosEvento.Value.ToString()) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;<a id='" + String.Format("ButtonProcessoEventoPDF{0}Excluir4", "") + "' class='btn btn-danger btn-xs' onclick='ExcluirPDF(4, " + '"' + '"' + ")'>Apagar</a>&nbsp;&nbsp;<label id='" + String.Format("LabelProcessoEventoFile{0}ExcluirConfirmacao4", "") + "' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse PDF?&nbsp;&nbsp;&nbsp;</label><a id='" + String.Format("accept4File{0}", "") + "' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirPDFSim(4, " + '"' + '"' + ", " + '"' + String.Format("{0}-{1}-4.pdf", Util.OnlyNumbers(data.NumeroProcesso.Value.ToString()), data.IdProcessosEvento.Value.ToString()) + '"' + ")'>Sim</a><a id='" + String.Format("go_back4File{0}", "") + "' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirPDFNao(4, " + '"' + '"' + ")''>Não</a>";
            }

            var file5 = Path.Combine(Server.MapPath("~/Uploads"), String.Format("{0}-{1}-5.pdf", Util.OnlyNumbers(data.NumeroProcesso.Value.ToString()), (data.IdProcessosEvento.Value != null ? data.IdProcessosEvento.Value.ToString() : string.Empty)));
            if (System.IO.File.Exists(file5))
            {
                pdf += "<a class='btn btn-default' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-5.pdf", Util.OnlyNumbers(data.NumeroProcesso.Value.ToString()), data.IdProcessosEvento.Value.ToString()) + '"' + ");'>Visualizar 5</a>";
                arquivo5 = "<a style='cursor: pointer;' onclick='VisualizarPDF(" + '"' + String.Format("/Uploads/{0}-{1}-5.pdf", Util.OnlyNumbers(data.NumeroProcesso.Value.ToString()), data.IdProcessosEvento.Value.ToString()) + '"' + ");'>" + String.Format("{0}-{1}-5.pdf", Util.OnlyNumbers(data.NumeroProcesso.Value.ToString()), data.IdProcessosEvento.Value.ToString()) + "</a>&nbsp;&nbsp;&nbsp;&nbsp;<a id='" + String.Format("ButtonProcessoEventoPDF{0}Excluir5", "") + "' class='btn btn-danger btn-xs' onclick='ExcluirPDF(5, " + '"' + '"' + ")'>Apagar</a>&nbsp;&nbsp;<label id='" + String.Format("LabelProcessoEventoFile{0}ExcluirConfirmacao5", "") + "' style='display: none; font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente apagar esse PDF?&nbsp;&nbsp;&nbsp;</label><a id='" + String.Format("accept5File{0}", "") + "' class='btn btn-default btn-xs' style='display:none' onclick='ExcluirPDFSim(5, " + '"' + '"' + ", " + '"' + String.Format("{0}-{1}-5.pdf", Util.OnlyNumbers(data.NumeroProcesso.Value.ToString()), data.IdProcessosEvento.Value.ToString()) + '"' + ")'>Sim</a><a id='" + String.Format("go_back5File{0}", "") + "' class='btn btn-primary btn-xs' style='display:none' onclick='ExcluirPDFNao(5, " + '"' + '"' + ")''>Não</a>";
            }

            if (pdf.Length == 0)
            {
                pdf = "Nenhum arquivo encontrado";
            }
            #endregion

            var result = new { codigo = "00", agenda = agenda, pdf = pdf, arquivo1 = arquivo1, arquivo2 = arquivo2, arquivo3 = arquivo3, arquivo4 = arquivo4, arquivo5 = arquivo5 };
            return Json(result);
        }

        [HttpPost]
        public JsonResult ExcluirAgendaProfissional(Int32 Id)
        {
            try
            {
                #region Auditoria
                ProcessosAgendaProfissional agenda = ProcessosAgendasProfissionaisBo.Buscar(Id);

                if (agenda != null)
                {
                    Auditoria auditoria = new Auditoria();
                    auditoria.DataHora = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                    auditoria.Modulo = "Processo";
                    auditoria.Tipo = "Agenda do Profissional";
                    auditoria.Acao = "Excluído";
                    auditoria.Log = String.Format("<b>Profissional</b>: {0};<b>Número do Processo</b>: {1};<b>Data 1º Prazo</b>: {2};<b>Data 2º Prazo</b>: {3};<b>Descrição/b>: {4};", agenda.Profissionais.Pessoas.Nome, agenda.ProcessosAcoesEventos.ProcessosAcoes.NumeroProcesso, agenda.ProcessosAcoesEventos.PrazoEvento1?.ToShortDateString(), agenda.ProcessosAcoesEventos.PrazoEvento2?.ToShortDateString(), agenda.ProcessosAcoesEventos.Descricao);
                    auditoria.Usuario = Sessao.Usuario.Nome;

                    AuditoriaBo.Inserir(auditoria);
                }
                #endregion

                var codigo = ProcessosAgendasProfissionaisBo.Excluir(Id);

                var result = new { codigo = codigo };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }
        #endregion

        [HttpPost]
        public JsonResult ConsultarEProc(Int32 Id)
        {
            Array processo = ProcessosEventosPendentesBo.ConsultarArray(Id, Int16.Parse(Sessao.Setting.Find(x => x.Name == "AddWorkingDays").Value), Int16.Parse(Sessao.Setting.Find(x => x.Name == "SubtractWorkingDays").Value));
            var result = new { codigo = "00", processo = processo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult ExcluirEProc(Int32 Id)
        {
            #region Auditoria
            ProcessosEventosPendentes processo = ProcessosEventosPendentesBo.Consultar(Id);
            processo.Excluido = true;
            processo.UsuarioExclusao = Sessao.Usuario.Nome;
            processo.DataHoraExclusao = DateTime.Now;

            Auditoria auditoria = new Auditoria();
            auditoria.DataHora = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            auditoria.Modulo = "Tratamento de Prazo";
            auditoria.Tipo = "Processos EPROC";
            auditoria.Acao = "Excluído";
            auditoria.Log = String.Format("<b>Data da Publicação</b>: {0};<b>Número Processo</b>: {1};<b>Cliente</b>: {2};<b>Órgão</b>: {3};<b>Assunto</b>: {4};<b>Descrição</b>: {5};", processo.DataEnvioRequisicao, processo.Processo, (processo.Partes == null ? string.Empty : Util.FormatClient(processo.Partes)), processo.Orgao, processo.Assunto, processo.EventoPrazo);
            auditoria.Usuario = Sessao.Usuario.Nome;

            AuditoriaBo.Inserir(auditoria);
            #endregion

            var codigo = ProcessosEventosPendentesBo.Excluir(processo);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult ConsultarWebJur(Int32 Id)
        {
            var processo = ProcessosEventosPendentesBo.ConsultarArray(Id, Int16.Parse(Sessao.Setting.Find(x => x.Name == "AddWorkingDays").Value), Int16.Parse(Sessao.Setting.Find(x => x.Name == "SubtractWorkingDays").Value));
            var result = new { codigo = "00", processo = processo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult ExcluirWebJur(Int32 Id)
        {
            #region Auditoria
            ProcessosEventosPendentes processo = ProcessosEventosPendentesBo.Consultar(Id);
            processo.Excluido = true;
            processo.UsuarioExclusao = Sessao.Usuario.Nome;
            processo.DataHoraExclusao = DateTime.Now;

            Auditoria auditoria = new Auditoria();
            auditoria.DataHora = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            auditoria.Modulo = "Tratamento de Prazo";
            auditoria.Tipo = "Processos WEBJUR";
            auditoria.Acao = "Excluído";
            auditoria.Log = String.Format("<b>Data</b>: {0};<b>Número Processo</b>: {1};<b>Órgão</b>: {2};<b>Juízo</b>: {3};<b>Código Publicação</b>: {4};<b>Descrição</b>: {5};", processo.dataPublicacao, processo.numeroProcesso, processo.descricaoDiario, processo.varaDescricao, processo.codPublicacao, processo.processoPublicacao);
            auditoria.Usuario = Sessao.Usuario.Nome;

            AuditoriaBo.Inserir(auditoria);
            #endregion

            var codigo = ProcessosEventosPendentesBo.Excluir(processo);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult ConsultarOab(Int32 Id)
        {
            var processo = ProcessosEventosPendentesBo.ConsultarArray(Id, Int16.Parse(Sessao.Setting.Find(x => x.Name == "AddWorkingDays").Value), Int16.Parse(Sessao.Setting.Find(x => x.Name == "SubtractWorkingDays").Value));
            var result = new { codigo = "00", processo = processo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult ExcluirOab(Int32 Id)
        {
            #region Auditoria
            ProcessosEventosPendentes processo = ProcessosEventosPendentesBo.Consultar(Id);
            processo.Excluido = true;
            processo.UsuarioExclusao = Sessao.Usuario.Nome;
            processo.DataHoraExclusao = DateTime.Now;

            Auditoria auditoria = new Auditoria();
            auditoria.DataHora = DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            auditoria.Modulo = "Tratamento de Prazo";
            auditoria.Tipo = "Processos OAB";
            auditoria.Acao = "Excluído";
            auditoria.Log = String.Format("<b>Data</b>: {0};<b>Número Processo</b>: {1};<b>Comarca</b>: {2};<b>Ação</b>: {3};<b>Juízo</b>: {4};<b>Descrição</b>: {5};", processo.dtdisponibilizacao, processo.numeroProcesso, processo.comarca, processo.tituloAcao, processo.vara, processo.descricao);
            auditoria.Usuario = Sessao.Usuario.Nome;

            AuditoriaBo.Inserir(auditoria);
            #endregion

            var codigo = ProcessosEventosPendentesBo.Excluir(processo);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
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
        public JsonResult ExcluirTodos(String Origem)
        {
            var codigo = ProcessosEventosPendentesBo.ExcluirTodos(Origem);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult ListarProcessosCliente(String Cliente)
        {
            var processos = ProcessosAcoesBo.ListarProcessosCliente(Cliente);

            var result = new { codigo = "00", processos = processos };
            return Json(result);
        }

        [HttpPost]
        public JsonResult ConsultarFinanceiro(Int32 IdProcesso)
        {

            var IdCliente = ProcessosBo.ConsultaCliente(IdProcesso).IdCliente;

            var financeiro = ProcessosBo.ListarFinanceiro(int.Parse(IdCliente.ToString()));


            var financeiros = String.Empty;

            if (financeiro.Count > 0)
            {
                financeiros += "<table class='table table-striped' style='width: 100% !important; font-size: 12px !important;'>";
                financeiros += "    <thead>";
                financeiros += "        <tr>";
                financeiros += "            <th>Número Documento</th>";
                financeiros += "            <th>Data Emissão</th>";
                financeiros += "            <th>Parcela</th>";
                financeiros += "            <th>Data Vencimento</th>";
                financeiros += "            <th>Valor Parcela</th>";
                financeiros += "            <th>Data Pagamento</th>";
                financeiros += "        </tr>";
                financeiros += "    </thead>";
                financeiros += "    <tbody>";

                for (int i = 0; i < financeiro.Count; i++)
                {
                    financeiros += "        <tr>";
                    financeiros += "            <th scope='row'>" + financeiro[i].NumeroDocumento + "</th>";
                    financeiros += "            <td>" + financeiro[i].DataEmissao.Value.ToString("dd/MM/yyyy") + "</td>";
                    financeiros += "            <td>" + financeiro[i].Parcela + "</td>";
                    financeiros += "            <td>" + financeiro[i].Vencimento.Value.ToString("dd/MM/yyyy") + "</td>";
                    financeiros += "            <td>" + financeiro[i].ValorParcela + "</td>";
                    financeiros += "            <td>" + (financeiro[i].DataPagamento != null ? financeiro[i].DataPagamento.Value.ToString("dd/MM/yyyy") : string.Empty) + "</td>";
                    financeiros += "        </tr>";
                }

                financeiros += "    </tbody>";
                financeiros += "</table>";
            }

            var result = new { codigo = "00", financeiros = financeiros };
            return Json(result);
        }

        [HttpPost]
        public JsonResult BuscarProcessosFinanceiro(Int32 IdCliente)
        {
            var processos = ProcessosAcoesBo.ListarArrayFinanceiro(IdCliente);

            var result = new { codigo = "00", processos = processos };
            return Json(result);
        }

    }
}