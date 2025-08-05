using iTextSharp.text.html.simpleparser;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Uranus.Data.R9;
using R9.DataBase;

using System.Web.Mvc;
using Uranus.Business;

namespace Uranus.Suite.Controllers
{
    public static class TemplateInicialController
    {
        public static void Instalar(string conexão)
        {
            R9.DataBase.DataStructureManager.CheckTable<TemplateInicialAposentadoria>(new TemplateInicialAposentadoria());
            R9.DataBase.DataStructureManager.CheckTable<TemplateInicialPeríodo>(new TemplateInicialPeríodo());
        }
        public static byte[] GerarInicialPDF(int IDTemplate, string conexão)
        {
            return GerarInicial(R9.DataBase.Data.Load<TemplateInicialAposentadoria>(new DataContext(conexão), IDTemplate));
        }

        public static string GerarInicialHTML(int IDTemplate, string conexão)
        {
            return GerarHTML(R9.DataBase.Data.Load<TemplateInicialAposentadoria>(new DataContext(conexão), IDTemplate));
        }

        private static byte[] GerarInicial(TemplateInicialAposentadoria template)
        {
            string htmlBase = GerarHTML(template);

            return GerarPDFNReco(htmlBase);
        }

        private static string GerarHTML(TemplateInicialAposentadoria template)
        {
            var período = R9.DataBase.Data.LoadAll<TemplateInicialPeríodo>(template.Context, $"IDTemplate = {template.IDTemplate}");

            var processoExcel = Uri.UnescapeDataString(template.ProcessoExcel);
            processoExcel = processoExcel.Replace("font-size:14px", "font-size:8px");

            var htmlBase = Properties.Resources.TemplateInicialAposentadoria
                .Replace("#ClienteNome", HttpUtility.HtmlEncode(template.ClienteNome))
                .Replace("#ClienteNacionalidade", HttpUtility.HtmlEncode(template.ClienteNacionalidade))
                .Replace("#ClienteEstadoCivil", HttpUtility.HtmlEncode(template.ClienteEstadoCivil))
                .Replace("#ClienteProfissao", HttpUtility.HtmlEncode(template.ClienteProfissao))
                .Replace("#ClienteCPF", HttpUtility.HtmlEncode(template.ClienteCPF))
                .Replace("#ClienteEmail", HttpUtility.HtmlEncode(template.ClienteEmail))
                .Replace("#ClienteEndereco", HttpUtility.HtmlEncode(template.ClienteEndereco))
                .Replace("#ClienteBairro", HttpUtility.HtmlEncode(template.ClienteBairro))
                .Replace("#ClienteCidade", HttpUtility.HtmlEncode(template.ClienteCidade))
                .Replace("#ClienteEstado", HttpUtility.HtmlEncode(template.ClienteEstado))
                .Replace("#ClienteCEP", HttpUtility.HtmlEncode(template.ClienteCEP))
                .Replace("#ResumoPeriodo", HttpUtility.HtmlEncode(template.ProcessoResumoPeríodo))
                .Replace("#ProcessoDataProtocolo", HttpUtility.HtmlEncode(template.ProcessoDataProtocolo))
                .Replace("#RequerimentoNúmero", HttpUtility.HtmlEncode(template.RequerimentoNúmero))
                .Replace("#ProcessoImagem", HttpUtility.HtmlEncode(template.ProcessoImagem.ToString()))
                .Replace("#QuadrosSingularPlural", MontarQuadroSingularPlurar(template, período))
                .Replace("#DetalhamentoAtividades", MontarDetalhamento(template, período))
                .Replace("#ResumoPeríodos", HttpUtility.HtmlEncode(template.ProcessoResumoPeríodosDER))
                .Replace("#ReafirmarADERPedido", MontarReafirmação(template))
                .Replace("#Gratuidade", MontarGratuidade(template))
                .Replace("#ProcessoValorCausa", HttpUtility.HtmlEncode(template.ProcessoValorCausa))
                .Replace("#ProcessoRodapeData", HttpUtility.HtmlEncode(template.ProcessoRodapeData))
                .Replace("#ProcessoAdvogado", HttpUtility.HtmlEncode(template.ProcessoAdvogado))
                .Replace("#ProcessoOAB", HttpUtility.HtmlEncode(template.ProcessoOAB))
                .Replace("#VaraNumero", HttpUtility.HtmlEncode(template.VaraNumero))
                //.Replace("#ProcessoExcel", processoExcel)
                .Replace("#VaraCidade", HttpUtility.HtmlEncode(template.VaraCidade));
            return htmlBase;
        }

        private static byte[] GerarPDFNReco(string htmlBase)
        {
            var conversorPDF = new NReco.PdfGenerator.HtmlToPdfConverter();

            var imgCabeçalhoBase64 = Properties.Resources.cabeçalho;
            var imgRodapéBase64 = Properties.Resources.rodape;

            conversorPDF.PageHeight = 297;
            conversorPDF.PageWidth = 210;
            conversorPDF.Margins = new NReco.PdfGenerator.PageMargins()
            {
                Bottom = 50,
                Left = 20,
                Right = 0,
                Top = 40,
            };

            conversorPDF.PageHeaderHtml = $"<div>{imgCabeçalhoBase64}</div>";
            conversorPDF.PageFooterHtml = $"<div style='text-align:right;'>página <span class='page'></span> de <span class='topage'></span> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</div><div>{imgRodapéBase64}</div>";
            var bytes = conversorPDF.GeneratePdf(htmlBase);

            return bytes;
        }

        //private static byte[] GerarPDF(string htmlBase)
        //{
        //    MemoryStream memoryStream = new MemoryStream();
        //    TextReader textReader = new StringReader(htmlBase);

        //    // 1: create object of a itextsharp document class  
        //    Document document = new Document(PageSize.A4, 50, 50, 110, 180);

        //    //iTextSharp.text.Image image = Image.GetInstance("http://localhost:63117/Content/Images/cabecalho.png");
        //    //image.SetAbsolutePosition(0, 732);



        //    // 2: we create a itextsharp pdfwriter that listens to the document and directs a XML-stream to a file  
        //    PdfWriter PdfWriter = PdfWriter.GetInstance(document, memoryStream);
        //    PdfWriter.CloseStream = false;
        //    PdfWriter.ViewerPreferences = PdfWriter.PageModeUseOutlines;

        //    // Our custom Header and Footer is done using Event Handler
        //    TwoColumnHeaderFooter PageEventHandler = new TwoColumnHeaderFooter();
        //    PdfWriter.PageEvent = PageEventHandler;

        //    // 3: we create a worker parse the document  
        //    HTMLWorker htmlWorker = new HTMLWorker(document);

        //    // 4: we open document and start the worker on the document  
        //    document.Open();
        //    //document.Add(image);
        //    htmlWorker.StartDocument();

        //    // 5: parse the html into the document  
        //    htmlWorker.Parse(textReader);

        //    // 6: close the document and the worker  
        //    htmlWorker.EndDocument();
        //    htmlWorker.Close();
        //    document.Close();



        //    memoryStream.Flush(); //Always catches me out
        //    memoryStream.Position = 0; //Not sure if this is required

        //    byte[] imageBytes = memoryStream.ToArray();

        //    return imageBytes;
        //}

        private static string MontarGratuidade(TemplateInicialAposentadoria template)
        {
            if (template.ProcessoGratuidade)
            {
                return "<br />A Parte Autora &eacute; pessoa de poucos recursos, n&atilde;o tendo condi&ccedil;&otilde;es de pagar as custas desta demanda sem que disto resulte preju&iacute;zo de seu sustento pr&oacute;prio e de sua fam&iacute;lia, raz&atilde;o pela qual requer, desde j&aacute;, seja deferida a GRATUIDADE DA JUSTI&Ccedil;A, nos termos do C&oacute;digo de Processo Civil vigentes e da Lei n&ordm; 1.060/1950.";
            }

            return "";
        }

        private static string MontarReafirmação(TemplateInicialAposentadoria template)
        {
            if (template.ProcessoReafirmacaoDER)
            {
                return "<li><b>REAFIRMAR a DER</b>, caso necess&aacute;rio, para fins de concess&atilde;o do benef&iacute;cio de aposentadoria na forma mais vantajosa;<br /><br /></li>";
            }

            return "";
        }

        private static string MontarDetalhamento(TemplateInicialAposentadoria template, List<TemplateInicialPeríodo> período)
        {
            var htmlTotal = "";

            foreach (var item in período)
            {
                htmlTotal += Properties.Resources.TemplateInicialAposentadoriaQuadro
                    .Replace("#Periodo", item.Periodo)
                    .Replace("#Empresa", item.Empresa)
                    .Replace("#Atividades", item.Atividades)
                    .Replace("#EnquadramentoLegal", item.EnquadramentoLegal)
                    .Replace("#Provas", item.Provas)
                    .Replace("#ProvasAProduzir", item.ProvasAProduzir)
                    .Replace("#Observações", item.OBS);
            }

            return htmlTotal;

        }

        private static string MontarQuadroSingularPlurar(TemplateInicialAposentadoria template, List<TemplateInicialPeríodo> período)
        {
            if (período.Count > 1)
            {
                return "<br />Os quadros abaixo apresentam os per&iacute;odos que a Parte Autora pretende o reconhecimento de atividade especial, bem como a legisla&ccedil;&atilde;o aplic&aacute;vel, e a respectiva prova: <br />";
            }
            else
            {
                return "<br />O quadro abaixo apresenta o per&iacute;odo que a Parte Autora pretende o reconhecimento de atividade especial, bem como a legisla&ccedil;&atilde;o aplic&aacute;vel, e a respectiva prova: <br />";
            }
        }

    }
}