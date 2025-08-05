using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Common;
using Uranus.Domain;
using System.Configuration;


namespace Uranus.Suite.Controllers
{
    public class IniciaisController : Controller
    {
        public ActionResult Index(string FiltrarDataValor = "", string FiltrarCliente = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = IniciaisBo.Listar(FiltrarDataValor, FiltrarCliente);
                ViewBag.FiltrarDataValor = FiltrarDataValor;
                ViewBag.FiltrarCliente = FiltrarCliente;
                return View(model);
            }
        }

        public PartialViewResult View(Int32 Id)
        {

            var model = TemplateInicialController.GerarInicialHTML(Id, ConfigurationManager.ConnectionStrings["APIR9"].ConnectionString);

            JsonResult result = Json(model);

            result.MaxJsonLength = 2147483644;

            return PartialView(result);

        }

        public JsonResult Export(Int32 Id)
        {
            var bytes = TemplateInicialController.GerarInicialPDF(Id, ConfigurationManager.ConnectionStrings["APIR9"].ConnectionString);
            var result = new { codigo = "00", inicial = Convert.ToBase64String(bytes)};
            return Json(result);
        }


        [HttpPost]
        public JsonResult Consultar(Int64 Id)
        {
            var inicial = IniciaisBo.ConsultarArray(Id);
            var result = new { codigo = "00", inicial = inicial };
            return Json(result);
        }

        public JsonResult Salvar(Int32 Id, Int32 IdCliente, Int32? IdProfissional, String VaraNumero, String VaraCidade, String ProcessoDataProtocolo, String RequerimentoNumero,
                                 String ProcessoAtividade, String ProcessoReafirmacaoDER, String ProcessoGratuidade, String ProcessoValorCausa, String ProcessoRodapeData,
                                 String ProcessoResumoPeriodo, String ProcessoResumoPeriodosDER, String ProcessoExcel, String ProcessoImagem)
        {
            var Cliente = ClientesBo.Consultar(IdCliente);
            var Email = EmailsBo.ListarEmail(Cliente.IDPessoa);

            var Profissional = ProfissionaisBo.Consultar(IdProfissional ?? 0);

            //if (!String.IsNullOrEmpty(Modelo))
            //{
            //    if (Cliente?.Pessoas.Nome != null)
            //    {
            //        Modelo = Modelo.Replace("#ClienteNome", Cliente.Pessoas.Nome);
            //    }

            //    if (Cliente?.Nacionalidade != null && Cliente?.Nacionalidade.Trim().Length > 0)
            //    {
            //        Modelo = Modelo.Replace("#ClienteNacionalidade", Cliente.Nacionalidade);
            //    }

            //    if (Cliente?.Pessoas.EstadoCivil != null && Cliente?.Pessoas.EstadoCivil.Trim().Length > 0)
            //    {
            //        Modelo = Modelo.Replace("#ClienteEstadoCivil", Cliente.Pessoas.EstadoCivil);
            //    }

            //    if (Cliente?.Profissao != null && Cliente?.Profissao.Trim().Length > 0)
            //    {
            //        Modelo = Modelo.Replace("#ClienteProfissao", Cliente.Profissao);
            //    }

            //    if (Cliente?.Pessoas.CpfCnpj != null && Cliente?.Pessoas.CpfCnpj.Trim().Length > 0)
            //    {
            //        Modelo = Modelo.Replace("#ClienteCPF", Util.FormatCPF(Cliente.Pessoas.CpfCnpj));
            //    }

            //    if (Cliente?.Pessoas.Endereco != null && Cliente?.Pessoas.Endereco.Trim().Length > 0)
            //    {
            //        Modelo = Modelo.Replace("#ClienteEndereco", String.Format("{0}, {1}{2}", Cliente.Pessoas.Endereco.Trim(), Cliente.Pessoas.Numero.Trim(), (Cliente.Pessoas.Complemento?.Length > 0 ? ", " + Cliente.Pessoas.Complemento.Trim() : String.Empty)));
            //    }

            //    if (Cliente?.Pessoas.Bairro != null && Cliente?.Pessoas.Bairro.Trim().Length > 0)
            //    {
            //        Modelo = Modelo.Replace("#ClienteBairro", Cliente.Pessoas.Bairro.Trim());
            //    }

            //    if (Cliente?.Pessoas.Municipio != null && Cliente?.Pessoas.Municipio.Trim().Length > 0)
            //    {
            //        Modelo = Modelo.Replace("#ClienteCidade", Cliente.Pessoas.Municipio.Trim());
            //    }

            //    if (Cliente?.Pessoas.Estado != null && Cliente?.Pessoas.Estado.Trim().Length > 0)
            //    {
            //        Modelo = Modelo.Replace("#ClienteEstado", Cliente.Pessoas.Estado.Trim());
            //    }

            //    if (Cliente?.Pessoas.Cep != null && Cliente?.Pessoas.Cep.Trim().Length > 0)
            //    {
            //        Modelo = Modelo.Replace("#ClienteCEP", Util.FormatCEP(Cliente.Pessoas.Cep));
            //    }

            //    if (!String.IsNullOrEmpty(ClausulaPrimeira))
            //    {
            //        Modelo = Modelo.Replace("#ClausulaPrimeira", ClausulaPrimeira);
            //    }

            //    if (ClausulaSegundaBeneficios != null)
            //    {
            //        Modelo = Modelo.Replace("#ClausulaSegundaBeneficios", String.Format("{0} ({1}) {2}", ClausulaSegundaBeneficios, Util.IntegerToExtensive(ClausulaSegundaBeneficios.Value), (ClausulaSegundaBeneficios == 1 ? "benefício" : "benefícios")));
            //    }

            //    if (!String.IsNullOrEmpty(ClausulaSegundaValor))
            //    {
            //        var valor = Decimal.Parse(ClausulaSegundaValor.Replace("R$ ", String.Empty));

            //        Modelo = Modelo.Replace("#ClausulaSegundaValor", String.Format("{0} ({1})", ClausulaSegundaValor, Util.EscreverExtenso(valor).ToLowerInvariant()));
            //    }

            //    if (ClausulaSegundaParcelas != null)
            //    {
            //        Modelo = Modelo.Replace("#ClausulaSegundaParcelas", ClausulaSegundaParcelas);
            //    }

            //    if (ClausulaSegundaVencimentoDia != null)
            //    {
            //        Modelo = Modelo.Replace("#ClausulaSegundaVencimentoDia", ClausulaSegundaVencimentoDia);
            //    }

            //    if (!String.IsNullOrEmpty(ClausulaSegundaVencimento))
            //    {
            //        Modelo = Modelo.Replace("#ClausulaSegundaVencimento", ClausulaSegundaVencimento);
            //    }

            //    if (ClausulaSegundaPercentual != null)
            //    {
            //        Modelo = Modelo.Replace("#ClausulaSegundaPercentual", String.Format("{0} ({1} por cento)", ClausulaSegundaPercentual, Util.IntegerToExtensive(ClausulaSegundaPercentual.Value)));
            //    }

            //    if (!String.IsNullOrEmpty(CidadeEstado))
            //    {
            //        Modelo = Modelo.Replace("#CidadeEstado", CidadeEstado);
            //    }

            //    if (!String.IsNullOrEmpty(DataPorExtenso))
            //    {
            //        Modelo = Modelo.Replace("#DataPorExtenso", Util.DateByExtension(DateTime.Parse(DataPorExtenso)));
            //    }

            //    if (Profissional?.Pessoas.Nome != null)
            //    {
            //        Modelo = Modelo.Replace("#ProfissionalNome", Profissional.Pessoas.Nome);
            //    }
            //}
            TemplatesIniciaisAposentadoria inicial = new TemplatesIniciaisAposentadoria();
            inicial.IdTemplate = Id;
            inicial.IdProfissional = IdProfissional ?? null;
            inicial.IdCliente = IdCliente;
            inicial.ClienteNome = Cliente?.Pessoas?.Nome ?? null;
            inicial.ClienteNacionalidade = Cliente?.Nacionalidade ?? null;
            inicial.ClienteEstadoCivil = Cliente?.Pessoas?.EstadoCivil ?? null;
            inicial.ClienteProfissao = Cliente?.Profissao ?? null;
            inicial.ClienteCPF = (Cliente?.Pessoas?.CpfCnpj != null ? Util.FormatCPF(Cliente.Pessoas.CpfCnpj) : null);
            inicial.ClienteEndereco = (Cliente?.Pessoas?.Endereco != null && Cliente?.Pessoas.Endereco.Trim().Length > 0 ? String.Format("{0}, {1}{2}", Cliente.Pessoas.Endereco.Trim(), Cliente.Pessoas.Numero.Trim(), (Cliente.Pessoas.Complemento?.Length > 0 ? ", " + Cliente.Pessoas.Complemento.Trim() : String.Empty)) : null);
            inicial.ClienteBairro = (Cliente?.Pessoas?.Bairro != null && Cliente?.Pessoas.Bairro.Trim().Length > 0 ? Cliente.Pessoas.Bairro.Trim() : null);
            inicial.ClienteCidade = (Cliente?.Pessoas?.Municipio != null && Cliente?.Pessoas.Municipio.Trim().Length > 0 ? Cliente.Pessoas.Municipio.Trim() : null);
            inicial.ClienteEstado = (Cliente?.Pessoas?.Estado != null && Cliente?.Pessoas.Estado.Trim().Length > 0 ? Cliente.Pessoas.Estado.Trim() : null);
            inicial.ClienteCep = (Cliente?.Pessoas?.Cep != null && Cliente?.Pessoas.Cep.Trim().Length > 0 ? Util.FormatCEP(Cliente.Pessoas.Cep) : null);
            inicial.ClienteEmail = (Email?.Email1 != null && Email?.Email1.Trim() != "" ? Email?.Email1 : null);
            inicial.VaraNumero = (VaraNumero.Trim().Length > 0 ? VaraNumero : null);
            inicial.VaraCidade = VaraCidade;
            inicial.ProcessoDataProtocolo = ProcessoDataProtocolo;
            inicial.RequerimentoNumero = RequerimentoNumero;
            inicial.ProcessoAtividade = ProcessoAtividade;
            inicial.ProcessoReafirmacaoDER = (ProcessoReafirmacaoDER == "1" ? true : false);
            inicial.ProcessoGratuidade = (ProcessoGratuidade == "1" ? true : false);
            inicial.ProcessoValorCausa = ProcessoValorCausa;
            inicial.ProcessoRodapeData = ProcessoRodapeData;
            inicial.ProcessoAdvogado = (Profissional != null ? Profissional.Pessoas.Nome : null);
            inicial.ProcessoImagem = ProcessoImagem;
            inicial.ProcessoOAB = (Profissional != null ? Profissional.OAB : null);
            inicial.ProcessoResumoPeriodo = ProcessoResumoPeriodo;
            inicial.ProcessoResumoPeriodosDER = ProcessoResumoPeriodosDER;
            inicial.ProcessoExcel = ProcessoExcel;
            inicial.DataCadastro = DateTime.Now;
            inicial.NomeUsuarioCadastro = Sessao.Usuario.Nome;
            inicial.DataAlteracao = DateTime.Now;
            inicial.NomeUsuarioAlteracao = Sessao.Usuario.Nome;




            if (Id == 0)
                Id = IniciaisBo.Inserir(inicial);

            inicial.IdTemplate = Id;
            IniciaisBo.Salvar(inicial);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = IniciaisBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult CarregarPeriodosIniciais(Int32 IdTemplate)
        {
            try
            {
                var periodo = IniciaisPeriodosBO.Listar(IdTemplate);
                String periodos = String.Empty;

                int PeriodosQuantidade = (periodo.Count > 0 ? (periodo.Count + 1) : 1);

                for (int i = 0; i < PeriodosQuantidade; i++)
                {
                    periodos += "<input id='" + String.Format("HiddenDadoPeriodo{0}", (i + 1)) + "' name='HiddenDadoPeriodo[]' type='hidden' value='" + (periodo.Count >= (i + 1) ? periodo[i].IDPeriodo.ToString() : "0") + "' />";
                    periodos += "<div class='accordion' id='accordionPeriodos' style='border-bottom: 1px solid rgba(0,0,0,.125) !important;'>";
                    //periodos += "   <div class='card z-depth-0 bordered'>";
                    periodos += "   <div class='accordion-heading panel-heading' id='" + String.Format("DadoPeriodo{0}heading", (i + 1)) + "' role='tab'>";
                    periodos += "      <div class='card-header' id='" + String.Format("DadoPeriodo{0}heading", (i + 1)) + "'>";
                    //periodos += "         <h5 class='mb-0'>";
                    periodos += "      <h4 class='accordion-title panel-title'>";
                    //periodos += "            <button class='btn btn-link' type='button' data-toggle='collapse' data-target='#" + String.Format("DadoPeriodo{0}collapse", (i + 1)) + "' aria-expanded='true' aria-controls='" + String.Format("DadoPeriodo{0}collapse", (i + 1)) + "'>";
                    //periodos += "            " + String.Format("Período {0}", (i + 1));
                    //periodos += (periodo.Count >= (i + 1) ? String.Format(" - {0} - {1}", periodo[i].Periodo, periodo[i].Empresa) : String.Empty);
                    //periodos += "            </button>";
                    periodos += "         <a href='" + String.Format("#DadoPeriodo{0}collapse", (i + 1)) + "' aria-expanded='false' data-toggle='collapse' aria-controls='" + String.Format("DadoPeriodo{0}collapse", (i + 1)) + "' data-parent='#DadoPeriodoAccordion' role='button' class='collapsed'>";
                    periodos += "            " + String.Format("Período {0} ", (i + 1));
                    periodos += (periodo.Count >= (i + 1) ? String.Format(" - {0}", periodo[i].Periodo) : String.Empty);
                    periodos += "         </a>";
                    periodos += "         </h4>";
                    periodos += "      </div>";
                    //periodos += "      <div id='" + String.Format("DadoPeriodo{0}collapse", (i + 1)) + "' class='collapse' aria-labelledby='" + String.Format("DadoPeriodo{0}heading", (i + 1)) + "' data-parent='#accordionPeriodos'>";
                    periodos += "      <div class='panel-collapse collapse' id='" + String.Format("DadoPeriodo{0}collapse", (i + 1)) + "' role='tabpanel' aria-labelledby='" + String.Format("DadoPeriodo{0}heading", (i + 1)) + "' aria-expanded='false' style='height: 0px;'>";
                    periodos += "         <div class='card-body'>";
                    periodos += "           <div class='row'>";
                    periodos += "              <div class='col-md-2'>";
                    periodos += "                 <div class='form-group'>";
                    periodos += "                    <label class='titlefield'>Periodo <span class='required'>*</span></label>";
                    periodos += "                    <input type='text' id='" + String.Format("DadoPeriodo{0}Periodo", (i + 1)) + "' name='DadoPeriodoPeriodo[]' class='form-control' placeholder='Informe o Período' value='" + (periodo.Count >= (i + 1) ? periodo[i].Periodo.ToString() : String.Empty) + "' />";
                    periodos += "                 </div>";
                    periodos += "              </div>";
                    periodos += "              <div class='col-md-8'>";
                    periodos += "                 <div class='form-group'>";
                    periodos += "                    <label class='titlefield'>Empresa <span class='required'>*</span></label>";
                    periodos += "                    <input type='text' id='" + String.Format("DadoPeriodo{0}Empresa", (i + 1)) + "' name='DadoPeriodoEmpresa[]' class='form-control' value='" + (periodo.Count >= (i + 1) ? periodo[i].Empresa?.ToString() : String.Empty) + "' />";
                    periodos += "                 </div>";
                    periodos += "              </div>";
                    periodos += "           </div>";
                    periodos += "           <div class='row'>";
                    periodos += "              <div class='col-md-12'>";
                    periodos += "                 <div class='form-group'>";
                    periodos += "                    <label class='titlefield'>Atividades <span class='required'>*</span></label>";
                    periodos += "                    <input type='text' id='" + String.Format("DadoPeriodo{0}Atividades", (i + 1)) + "' name='DadoPeriodoAtividades[]' class='form-control' value='" + (periodo.Count >= (i + 1) ? periodo[i].Atividades?.ToString() : String.Empty) + "' />";
                    periodos += "                 </div>";
                    periodos += "              </div>";
                    periodos += "           </div>";
                    periodos += "           <div class='row'>";
                    periodos += "              <div class='col-md-12'>";
                    periodos += "                 <div class='form-group'>";
                    periodos += "                    <label class='titlefield'>Enquadramento Legal <span class='required'>*</span></label>";
                    periodos += "                    <input type='text' id='" + String.Format("DadoPeriodo{0}EnquadramentoLegal", (i + 1)) + "' name='DadoPeriodoEnquadramentoLegal[]' class='form-control' value='" + (periodo.Count >= (i + 1) ? periodo[i].EnquadramentoLegal?.ToString() : String.Empty) + "' />";
                    periodos += "                 </div>";
                    periodos += "              </div>";
                    periodos += "           </div>";
                    periodos += "           <div class='row'>";
                    periodos += "              <div class='col-md-12'>";
                    periodos += "                 <div class='form-group'>";
                    periodos += "                    <label class='titlefield'>Provas <span class='required'>*</span></label>";
                    periodos += "                    <input type='text' id='" + String.Format("DadoPeriodo{0}Provas", (i + 1)) + "' name='DadoPeriodoProvas[]' class='form-control' value='" + (periodo.Count >= (i + 1) ? periodo[i].Provas?.ToString() : String.Empty) + "' />";
                    periodos += "                 </div>";
                    periodos += "              </div>";
                    periodos += "           </div>";
                    periodos += "            <div class='row'>";
                    periodos += "              <div class='col-md-12'>";
                    periodos += "                 <div class='form-group'>";
                    periodos += "                    <label class='titlefield'>Provas à Produzir <span class='required'>*</span></label>";
                    periodos += "                    <input type='text' id='" + String.Format("DadoPeriodo{0}ProvasAProduzir", (i + 1)) + "' name='DadoPeriodoProvasAProduzir[]' class='form-control' value='" + (periodo.Count >= (i + 1) ? periodo[i].ProvasAProduzir?.ToString() : String.Empty) + "' />";
                    periodos += "                 </div>";
                    periodos += "              </div>";
                    periodos += "           </div>";
                    periodos += "           <div class='row'>";
                    periodos += "              <div class='col-md-12'>";
                    periodos += "                 <div class='form-group'>";
                    periodos += "                    <label class='titlefield'>Observação <span class='required'>*</span></label>";
                    periodos += "                   <input type='text' id='" + String.Format("DadoPeriodo{0}OBS", (i + 1)) + "' name='DadoPeriodoOBS[]' class='form-control' value='" + (periodo.Count >= (i + 1) ? periodo[i].OBS?.ToString() : String.Empty) + "' />";
                    periodos += "                 </div>";
                    periodos += "              </div>";
                    periodos += "           </div>";
                    periodos += "           <div class='row'>";
                    periodos += "              <div class='col-md-4'>";
                    periodos += "                 <button type='button' id='" + String.Format("ButtonDadoPeriodo{0}Incluir", (i + 1)) + "' name='" + String.Format("ButtonDadoPeriodo{0}Incluir", (i + 1)) + "' class='btn btn-primary' data-loading-text='Salvando...' onclick='ButtonDadoPeriodoSalvar_Click(" + (i + 1) + ");'>";
                    periodos += "                    Salvar";
                    periodos += "                 </button>";
                    periodos += "              </div>";
                    periodos += "              <div class='col-md-4' style='text-align: center;'>";
                    periodos += "              </div>";
                    periodos += "              <div class='col-md-4' style='text-align: right;'>";
                    periodos += "                 <button type='button' id='" + String.Format("ButtonDadoPeriodo{0}Excluir", (i + 1)) + "' name='" + String.Format("ButtonDadoPeriodo{0}Excluir", (i + 1)) + "' class='btn btn-danger' data-loading-text='Apagando...' onclick='ButtonDadoPeriodoExcluir_Click(" + (i + 1) + ");' " + (periodo.Count >= (i + 1) ? String.Empty : "disabled='disabled'") + ">";
                    periodos += "                    Apagar";
                    periodos += "                 </button>";
                    periodos += "              </div>";
                    periodos += "           </div>";
                    periodos += "              <div class='col-md-4' style='text-align: center;'>";
                    periodos += "              </div>";
                    periodos += "              <div class='col-md-4' style='text-align: right;'>";
                    periodos += "              </div>";
                    periodos += "           </div>";
                    periodos += "         </div>";
                    periodos += "      </div>";
                    periodos += "   </div>";
                    periodos += "</div>";
                }
                var response = "success";
                //if (parcelas.Length == 0)
                //{
                //    response = "empty";
                //}

                var result = new { response = response, periodos = periodos, count = PeriodosQuantidade };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult SalvarPeriodos(Int32 Id, Int32 IdTemplate, string Periodo, String Empresa, String Atividades, String EnquadramentoLegal, String Provas, String ProvasAProduzir, String OBS)
        { 
            try
            {
                TemplatesIniciaisAposentadoriaPeriodos periodo = new TemplatesIniciaisAposentadoriaPeriodos();
                periodo.IDPeriodo = Id;
                        
                periodo.IDTemplate = IdTemplate;
                periodo.Periodo = Periodo;
                periodo.Empresa = Empresa;
                periodo.Atividades = Atividades;
                periodo.EnquadramentoLegal = EnquadramentoLegal;
                periodo.Provas = Provas;
                periodo.ProvasAProduzir = ProvasAProduzir;
                periodo.OBS = OBS;

                if (Id == 0)
                    Id = IniciaisBo.InserirPeriodo(periodo);

                IniciaisBo.SalvarPeriodos(periodo);

                var result = new { response = "success" };
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
        public JsonResult ExcluirDadosPeriodos(Int32 Id)
        {
            try
            {
            IniciaisBo.ExcluirPeriodos(Id);

            var result = new { response = "removed" };
            return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        //public class TwoColumnHeaderFooter : PdfPageEventHelper
        //{
        //    // This is the contentbyte object of the writer
        //    PdfContentByte cb;

        //    // we will put the final number of pages in a template
        //    PdfTemplate template;

        //    // this is the BaseFont we are going to use for the header / footer
        //    BaseFont bf = null;

        //    // we override the onOpenDocument method
        //    public override void OnOpenDocument(PdfWriter writer, Document document)
        //    {
        //        try
        //        {
        //            bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        //            cb = writer.DirectContent;
        //            template = cb.CreateTemplate(50, 50);
        //        }
        //        catch (DocumentException de)
        //        {
        //        }
        //        catch (System.IO.IOException ioe)
        //        {
        //        }
        //    }

        //    public override void OnStartPage(PdfWriter writer, Document document)
        //    {
        //        base.OnStartPage(writer, document);
        //        iTextSharp.text.Image image = Image.GetInstance(HttpContext.Current.Server.MapPath("~/Content/Images/cabecalho.png"));
        //        image.SetAbsolutePosition(0, 732);
        //        cb.AddImage(image);
        //    }

        //    public override void OnEndPage(PdfWriter writer, Document document)
        //    {
        //        base.OnEndPage(writer, document);
        //        int pageN = writer.PageNumber;
        //        String text = "Página " + pageN + " de ";
        //        float len = bf.GetWidthPoint(text, 8);
        //        iTextSharp.text.Rectangle pageSize = document.PageSize;
        //        cb.SetRGBColorFill(100, 100, 100);
        //        cb.BeginText();
        //        cb.SetFontAndSize(bf, 8);
        //        cb.SetTextMatrix(pageSize.GetRight(100), pageSize.GetBottom(195));
        //        cb.ShowText(text);
        //        cb.EndText();
        //        cb.AddTemplate(template, pageSize.GetRight(100) + len, pageSize.GetBottom(195));

        //        iTextSharp.text.Image image = Image.GetInstance(HttpContext.Current.Server.MapPath("~/Content/Images/rodape.png"));
        //        image.SetAbsolutePosition(0, 0);
        //        cb.AddImage(image);
        //    }

        //    public override void OnCloseDocument(PdfWriter writer, Document document)
        //    {
        //        base.OnCloseDocument(writer, document);
        //        template.BeginText();
        //        template.SetFontAndSize(bf, 8);
        //        template.SetTextMatrix(0, 0);
        //        template.ShowText("" + (writer.PageNumber));
        //        template.EndText();
        //    }
        //}
    }

}
