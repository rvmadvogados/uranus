using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Common;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class DocumentosController : Controller
    {
        public ActionResult Index(string FiltrarDataValor = "", string FiltrarModelo = "", string FiltrarCliente = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = DocumentosBo.Listar(FiltrarDataValor, FiltrarModelo, FiltrarCliente);
                ViewBag.FiltrarDataValor = FiltrarDataValor;
                ViewBag.FiltrarModelo = FiltrarModelo;
                ViewBag.FiltrarCliente = FiltrarCliente;
                return View(model);
            }
        }

        public PartialViewResult View(Int64 Id)
        {
            var model = DocumentosBo.Consultar(Id);

            JsonResult result = Json(model);
            result.MaxJsonLength = 2147483644;

            return PartialView(result);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Export(Int64 Id)
        {
            // Put your html tempelate here

            string HTMLContent = Regex.Replace(DocumentosBo.Consultar(Id).Documento.Replace(Environment.NewLine, "<br />"), @"\r\n?|\n", "<br />").Replace("<br /><br />", "<br />").Replace("font-size:12px", "font-size:8px").Replace("</p>", "</p><br />");
            //string HTMLContent = Regex.Replace(DocumentosBo.Consultar(Id).Documento.Replace(Environment.NewLine, "<br />"), @"\r\n?|\n", "<br />").Replace("<br /><br />", "<br />").Replace("<br />", string.Empty).Replace("font-size:12px", "font-size:8px").Replace("</p>", "</p><br />");

            MemoryStream memoryStream = new MemoryStream();
            TextReader textReader = new StringReader(HTMLContent);

            // 1: create object of a itextsharp document class  
            Document document = new Document(PageSize.A4, 40, 40, 110, 165);

            // 2: we create a itextsharp pdfwriter that listens to the document and directs a XML-stream to a file  
            PdfWriter PdfWriter = PdfWriter.GetInstance(document, memoryStream);
            PdfWriter.CloseStream = false;
            PdfWriter.ViewerPreferences = PdfWriter.PageModeUseOutlines;

            // Our custom Header and Footer is done using Event Handler
            TwoColumnHeaderFooter PageEventHandler = new TwoColumnHeaderFooter();
            PdfWriter.PageEvent = PageEventHandler;

            // 3: we create a worker parse the document  
            HTMLWorker htmlWorker = new HTMLWorker(document);

            // 4: we open document and start the worker on the document  
            document.Open();

            htmlWorker.StartDocument();

            // 5: parse the html into the document  
            htmlWorker.Parse(textReader);

            // 6: close the document and the worker  
            htmlWorker.EndDocument();
            htmlWorker.Close();
            document.Close();

            memoryStream.Flush(); //Always catches me out
            memoryStream.Position = 0; //Not sure if this is required

            byte[] imageBytes = memoryStream.ToArray();

            // Convert byte[] to Base64 String
            string base64String = Convert.ToBase64String(imageBytes);

            var doc = new { codigo = "00", documento = base64String };

            JsonResult result = Json(doc);
            result.MaxJsonLength = 2147483644;
            
            return result;
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult ExportarWord(Int64 Id)
        {
            // Put your html tempelate here
            //string HTMLContent = Regex.Replace(DocumentosBo.Consultar(Id).Documento.Replace(Environment.NewLine, "<br />"), @"\r\n?|\n", "<br />").Replace("<br /><br />", "<br />");
            //string HTMLContent = Regex.Replace(DocumentosBo.Consultar(Id).Documento.Replace(Environment.NewLine, "<br />"), @"\r\n?|\n", "<br />").Replace("<br>", "<p>");
            string HTMLContent = DocumentosBo.Consultar(Id).Documento;

            //            byte[] imageBytes = WordHelper.CriarDocumento(HTMLContent, Properties.Resources.cabecalho, Properties.Resources.rodape);

            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string imagePath = Path.Combine(basePath, "Content", "Images", "cabecalho.png");
            System.Drawing.Image img = System.Drawing.Image.FromFile(imagePath);
//            System.Drawing.Image img = System.Drawing.Image.FromFile(@"C:\ProjetosRVM\Uranus\Uranus.Suite\Content\Images\cabecalho.png");
            Bitmap bmp = new Bitmap(img);

            byte[] imageBytes = WordHelper.CriarDocumento(HTMLContent, bmp, Properties.Resources.rodape);

            // Convert byte[] to Base64 String
            string base64String = Convert.ToBase64String(imageBytes);

            var result = new { codigo = "00", documento = base64String };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Consultar(Int64 Id)
        {
            var documento = DocumentosBo.ConsultarArray(Id);
            var result = new { codigo = "00", documento = documento };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int64 Id, Int32 IdModelo, Int32 IdCliente, Int32? IdProfissional, String ClausulaPrimeira,
                                 Int32? ClausulaSegundaBeneficios, String ClausulaSegundaValor, String ClausulaSegundaParcelas,
                                 String ClausulaSegundaVencimentoDia, String ClausulaSegundaVencimento, Int32? ClausulaSegundaPercentual, String EspecificacaoProcuracao,
                                 String CidadeEstado, String DataPorExtenso)
        {
            var Modelo = Uri.UnescapeDataString(ModelosBo.Consultar(IdModelo).Modelo ?? String.Empty);

            var Cliente = ClientesBo.Consultar(IdCliente);

            var Profissional = ProfissionaisBo.Consultar(IdProfissional ?? 0);

            if (!String.IsNullOrEmpty(Modelo))
            {
                if (Cliente?.Pessoas.Nome != null)
                {
                    Modelo = Modelo.Replace("#ClienteNome", Cliente.Pessoas.Nome);
                }

                if (Cliente?.Nacionalidade != null && Cliente?.Nacionalidade.Trim().Length > 0)
                {
                    Modelo = Modelo.Replace("#ClienteNacionalidade", Cliente.Nacionalidade);
                }

                if (Cliente?.Pessoas.EstadoCivil != null && Cliente?.Pessoas.EstadoCivil.Trim().Length > 0)
                {
                    Modelo = Modelo.Replace("#ClienteEstadoCivil", Cliente.Pessoas.EstadoCivil);
                }

                if (Cliente?.Profissao != null && Cliente?.Profissao.Trim().Length > 0)
                {
                    Modelo = Modelo.Replace("#ClienteProfissao", Cliente.Profissao);
                }

                if (Cliente?.Pessoas.CpfCnpj != null && Cliente?.Pessoas.CpfCnpj.Trim().Length > 0)
                {
                    Modelo = Modelo.Replace("#ClienteCPF", Util.FormatCPF(Cliente.Pessoas.CpfCnpj));
                }

                if (Cliente?.Pessoas.Endereco != null && Cliente?.Pessoas.Endereco.Trim().Length > 0)
                {
                    Modelo = Modelo.Replace("#ClienteEndereco", String.Format("{0}, {1}{2}", Cliente.Pessoas.Endereco.Trim(), Cliente.Pessoas.Numero.Trim(), (Cliente.Pessoas.Complemento?.Length > 0 ? ", " + Cliente.Pessoas.Complemento.Trim() : String.Empty)));
                }

                if (Cliente?.Pessoas.Bairro != null && Cliente?.Pessoas.Bairro.Trim().Length > 0)
                {
                    Modelo = Modelo.Replace("#ClienteBairro", Cliente.Pessoas.Bairro.Trim());
                }

                if (Cliente?.Pessoas.Municipio != null && Cliente?.Pessoas.Municipio.Trim().Length > 0)
                {
                    Modelo = Modelo.Replace("#ClienteCidade", Cliente.Pessoas.Municipio.Trim());
                }

                if (Cliente?.Pessoas.Estado != null && Cliente?.Pessoas.Estado.Trim().Length > 0)
                {
                    Modelo = Modelo.Replace("#ClienteEstado", Cliente.Pessoas.Estado.Trim());
                }

                if (Cliente?.Pessoas.Cep != null && Cliente?.Pessoas.Cep.Trim().Length > 0)
                {
                    Modelo = Modelo.Replace("#ClienteCEP", Util.FormatCEP(Cliente.Pessoas.Cep));
                }

                if (!String.IsNullOrEmpty(ClausulaPrimeira))
                {
                    Modelo = Modelo.Replace("#ClausulaPrimeira", ClausulaPrimeira);
                }

                if (ClausulaSegundaBeneficios != null)
                {
                    Modelo = Modelo.Replace("#ClausulaSegundaBeneficios", String.Format("{0} ({1}) {2}", ClausulaSegundaBeneficios, Util.IntegerToExtensive(ClausulaSegundaBeneficios.Value), (ClausulaSegundaBeneficios == 1 ? "benefício" : "benefícios")));
                }

                if (!String.IsNullOrEmpty(ClausulaSegundaValor))
                {
                    var valor = Decimal.Parse(ClausulaSegundaValor.Replace("R$ ", String.Empty));

                    Modelo = Modelo.Replace("#ClausulaSegundaValor", String.Format("{0} ({1})", ClausulaSegundaValor, Util.EscreverExtenso(valor).ToLowerInvariant()));
                }

                if (ClausulaSegundaParcelas != null)
                {
                    Modelo = Modelo.Replace("#ClausulaSegundaParcelas", ClausulaSegundaParcelas);
                }

                if (ClausulaSegundaVencimentoDia != null)
                {
                    Modelo = Modelo.Replace("#ClausulaSegundaVencimentoDia", ClausulaSegundaVencimentoDia);
                }

                if (!String.IsNullOrEmpty(ClausulaSegundaVencimento))
                {
                    Modelo = Modelo.Replace("#ClausulaSegundaVencimento", ClausulaSegundaVencimento);
                }

                if (ClausulaSegundaPercentual != null)
                {
                    Modelo = Modelo.Replace("#ClausulaSegundaPercentual", String.Format("{0} ({1} por cento)", ClausulaSegundaPercentual, Util.IntegerToExtensive(ClausulaSegundaPercentual.Value)));
                }

                if (!String.IsNullOrEmpty(EspecificacaoProcuracao))
                {
                    Modelo = Modelo.Replace("#EspecificacaoProcuracao", EspecificacaoProcuracao);
                }

                if (!String.IsNullOrEmpty(CidadeEstado))
                {
                    Modelo = Modelo.Replace("#CidadeEstado", CidadeEstado);
                }

                if (!String.IsNullOrEmpty(DataPorExtenso))
                {
                    Modelo = Modelo.Replace("#DataPorExtenso", Util.DateByExtension(DateTime.Parse(DataPorExtenso)));
                }

                if (Profissional?.Pessoas.Nome != null)
                {
                    Modelo = Modelo.Replace("#ProfissionalNome", Profissional.Pessoas.Nome);
                }
                Modelo = Modelo.Replace("#usuario", Sessao.Usuario.Nome);
            }

            Documentos documento = new Documentos();
            documento.Id = Id;
            documento.IdModelo = IdModelo;
            documento.IdCliente = IdCliente;
            documento.IdProfissional = IdProfissional ?? null;
            documento.ClienteNome = Cliente?.Pessoas?.Nome ?? null;
            documento.ClienteNacionalidade = Cliente?.Nacionalidade ?? null;
            documento.ClienteEstadoCivil = Cliente?.Pessoas?.EstadoCivil ?? null;
            documento.ClienteProfissao = Cliente?.Profissao ?? null;
            documento.ClienteCPF = (Cliente?.Pessoas?.CpfCnpj != null ?  Util.FormatCPF(Cliente.Pessoas.CpfCnpj) : null);
            documento.ClienteEndereco = (Cliente?.Pessoas?.Endereco != null && Cliente?.Pessoas.Endereco.Trim().Length > 0 ? String.Format("{0}, {1}{2}", Cliente.Pessoas.Endereco.Trim(), Cliente.Pessoas.Numero.Trim(), (Cliente.Pessoas.Complemento?.Length > 0 ? ", " + Cliente.Pessoas.Complemento.Trim() : String.Empty)) : null);
            documento.ClienteBairro = (Cliente?.Pessoas?.Bairro != null && Cliente?.Pessoas.Bairro.Trim().Length > 0 ? Cliente.Pessoas.Bairro.Trim() : null);
            documento.ClienteCidade = (Cliente?.Pessoas?.Municipio != null && Cliente?.Pessoas.Municipio.Trim().Length > 0 ? Cliente.Pessoas.Municipio.Trim() : null);
            documento.ClienteEstado = (Cliente?.Pessoas?.Estado != null && Cliente?.Pessoas.Estado.Trim().Length > 0 ? Cliente.Pessoas.Estado.Trim() : null);
            documento.ClienteCEP = (Cliente?.Pessoas?.Cep != null && Cliente?.Pessoas.Cep.Trim().Length > 0 ? Util.FormatCEP(Cliente.Pessoas.Cep) : null);
            documento.ClausulaPrimeira = (ClausulaPrimeira.Trim().Length > 0 ? ClausulaPrimeira : null);
            documento.ClausulaSegundaBeneficios = ClausulaSegundaBeneficios;
            documento.ClausulaSegundaValor = ClausulaSegundaValor;
            documento.ClausulaSegundaParcelas = ClausulaSegundaParcelas;
            documento.ClausulaSegundaVencimentoDia = ClausulaSegundaVencimentoDia;
            documento.ClausulaSegundaVencimento = (ClausulaSegundaVencimento.Trim().Length > 0 ? ClausulaSegundaVencimento : null);
            documento.ClausulaSegundaPercentual = ClausulaSegundaPercentual;
            documento.EspecificacaoProcuracao = (EspecificacaoProcuracao.Trim().Length > 0 ? EspecificacaoProcuracao : null);
            documento.CidadeEstado = (CidadeEstado.Trim().Length > 0 ? CidadeEstado : null);

            if (!String.IsNullOrEmpty(DataPorExtenso))
            {
                documento.DataPorExtenso =  DateTime.Parse(DataPorExtenso);
            }

            documento.ProfissionalNome = (Profissional != null ? Profissional.Pessoas.Nome : null);
            documento.Documento = Modelo;
            documento.DataCadastro = DateTime.Now;
            documento.NomeUsuarioCadastro = Sessao.Usuario.Nome;
            documento.DataAlteracao = DateTime.Now;
            documento.NomeUsuarioAlteracao = Sessao.Usuario.Nome;

            if (Id == 0)
                Id = DocumentosBo.Inserir(documento);

            documento.Id = Id;
            DocumentosBo.Salvar(documento);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int64 Id)
        {
            var codigo = DocumentosBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

    }

    public class TwoColumnHeaderFooter : PdfPageEventHelper
    {
        // This is the contentbyte object of the writer
        PdfContentByte cb;

        // we will put the final number of pages in a template
        PdfTemplate template;

        // this is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;

        // we override the onOpenDocument method
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                template = cb.CreateTemplate(50, 50);
            }
            catch (DocumentException de)
            {
            }
            catch (System.IO.IOException ioe)
            {
            }
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);
            //iTextSharp.text.Image image = Image.GetInstance(HttpContext.Current.Server.MapPath("~/Content/Images/cabecalho.png"));
            //image.SetAbsolutePosition(0, 732);
            //cb.AddImage(image);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);
            int pageN = writer.PageNumber;
            String text = "Página " + pageN + " de ";
            float len = bf.GetWidthPoint(text, 8);
            iTextSharp.text.Rectangle pageSize = document.PageSize;
            cb.SetRGBColorFill(100, 100, 100);
            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.SetTextMatrix(pageSize.GetRight(100), pageSize.GetBottom(150));
            cb.ShowText(text);
            cb.EndText();
            cb.AddTemplate(template, pageSize.GetRight(100) + len, pageSize.GetBottom(150));

            //iTextSharp.text.Image image = Image.GetInstance(HttpContext.Current.Server.MapPath("~/Content/Images/rodape.png"));
            //image.ScaleAbsolute(595f, 143f);
            //image.SetAbsolutePosition(0, 0);
            //cb.AddImage(image);
        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
            template.BeginText();
            template.SetFontAndSize(bf, 8);
            template.SetTextMatrix(0, 0);
            template.ShowText("" + (writer.PageNumber));
            template.EndText();
        }
    }
}