using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlToOpenXml;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;


//using DocumentFormat.OpenXml.Drawing;


namespace Uranus.Suite.Controllers
{
    public static class WordHelper
    {

        /// <summary>
        /// Criar um arquivo DOCX com base no texto HTML fornecido e nas imagens para cabeçalho/rodapé
        /// </summary>
        /// <param name="html">o conteúdo do arquivo</param>
        /// <param name="imagemCabeçalho">imagem para cabeçalho (precisa ser PNG)</param>
        /// <param name="imagemRodapé">imagem para rodapé (precisa ser PNG)</param>
        /// <param name="larguraCabeçalho">largura do cabeçalho em pontos </param>
        /// <param name="alturaCabeçalho">altura do cabeçalho em pontos</param>
        /// <param name="larguraRodapé">largura do rodapé em pontos </param>
        /// <param name="alturaRodapé">altura do rodapé em pontos </param>
        /// <returns></returns>
        public static byte[] CriarDocumento(string html,
            Bitmap imagemCabeçalho,
            Bitmap imagemRodapé,
            long larguraCabeçalho = 8000000L,
            long alturaCabeçalho = 1252000L,
            long larguraRodapé = 8000000L,
//            long alturaRodapé = 1352000L)
            long alturaRodapé = 1700000L)
        {
            using (MemoryStream generatedDocument = new MemoryStream())
            {
                using (WordprocessingDocument package = WordprocessingDocument.Create(
                       generatedDocument, WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = package.MainDocumentPart;
                    if (mainPart == null)
                    {
                        mainPart = package.AddMainDocumentPart();
                        new Document(new Body()).Save(mainPart);
                    }

                    HtmlConverter converter = new HtmlConverter(mainPart);
                    Body body = mainPart.Document.Body;

                    //html = Regex.Replace(html, @"\r\n?|\n", "<br />").Replace("<br />","<p>").Replace(" <br /><br />", "<br />");
                    //html = Regex.Replace(html .Replace(Environment.NewLine, "<br />"), @"\r\n?|\n", "<br />").Replace("<br /><br />", "<br />").Replace("<br />", string.Empty);
                    //html = "<p style='text-align:center'><strong>P R O C U R A &Ccedil; &Atilde; O</strong></p><p><strong>OUTORGANTE</strong>:&nbsp;Bruno Fernando Brandt, brasileiro, casado, rep. comercial, inscrito(a) no CPF sob n&ordm; 362.798.380-72, residente e domiciliado(a) na Av Getulio Vargas, 1146, ap 301, Bairro Menino Deus, na cidade de&nbsp;Porto Alegre/RS, CEP 90.150-004.</p><p><strong>OUTORGADOS</strong>: RENATO VON M&Uuml;HLEN, brasileiro, advogado inscrito na OAB/RS sob o n&ordm; 21.768, ANGELA VON M&Uuml;HLEN, brasileira, advogada inscrita na OAB/RS sob o n&ordm; 49.157 e OAB/SP sob o n&ordm; 323.478, LIANDRA FRACALOSSI, brasileira, advogada inscrita na OAB/RS sob o n&ordm; 71.325, PEDRO IN&Aacute;CIO VON AMELN FERREIRA E SILVA, brasileiro, advogado inscrito na OAB/RS sob o n&ordm; 69.018, SANDRA MENDON&Ccedil;A SUELLO DA SILVA, brasileira, advogada inscrita na OAB/RS sob o n&ordm; 81.139, EDUARDO MACHADO MILDNER, brasileiro, advogado inscrito na OAB/RS sob o n&ordm; 81.302, JAQUELINE VON M&Uuml;HLEN, brasileira, advogada inscrita na OAB/RS sob o n&ordm; 96.678, ALINE CEZAR BECKER, advogada inscrita na OAB/RS sob o n&ordm; 56.219, VALQUIRIA PETER BACELLAR, brasileira, advogada inscrita na OAB/RS sob o n&ordm; 105.793, JULIANE TEODORO, brasileira, advogada inscrita na OAB/RS sob o n&ordm; 109.537-A e OAB/SC sob o n&ordm; 40.108, ANA MARIA RODRIGUES TISSOT, brasileira, advogada inscrita na OAB/RS sob o n&ordm; 90.870, RAQUEL MARLENE SIMSEN, brasileira, advogada inscrita na OAB/RS sob o n&ordm; 114.428, LUANA BERTT&Eacute; JUCHEM, brasileira, advogada inscrita na OAB/RS sob o n&ordm; 119.405, ISADORA ALBUQUERQUE, brasileira, advogada, inscrita na OAB/RS sob o n&ordm; 102.644, RENATA DOS SANTOS, brasileira, advogada, inscrita na OAB/RS sob o n&ordm; 105.347, e BIANCA ALMEIDA DOS SANTOS, brasileira, advogada, inscrita na OAB/RS sob o n&ordm; 115.186, todos representando a sociedade RENATO VON M&Uuml;HLEN ADVOGADOS ASSOCIADOS, inscrita na OAB/RS sob n&ordm; 2844, com escrit&oacute;rio profissional na cidade de Porto Alegre/RS, na Rua dos Andradas, n&ordm; 1137, cj. 1107, Centro Hist&oacute;rico, fone/fax (51) 3226.2900.</p><p><strong>PODERES</strong>: Com todos os poderes em direito permitidos para o Foro em geral, qualquer grau ou Tribunal, bem como, agir em qualquer &oacute;rg&atilde;o ou reparti&ccedil;&atilde;o p&uacute;blica, federal, estadual ou municipal e, mais os poderes especiais de receber e dar quita&ccedil;&atilde;o, firmar recibos, conciliar, acordar, transigir, desistir, tudo requerer, ratificar, retificar, receber e sacar alvar&aacute;s, substabelecer no todo ou em parte, firmar compromissos em tudo mais que se fizer necess&aacute;rio para o bom e fiel cumprimento do presente mandato.</p><p><strong>ESPECIFICA&Ccedil;&Atilde;O</strong>: #EspecificacaoProcuracao.&nbsp;&nbsp;</p><p>#CidadeEstado, 31 de maio de 2022.</p><p>______________________________________________</p>";

                    //// Insert a paragraph and run (for image)
                    //Paragraph para = body.AppendChild(new Paragraph());
                    //A.Run run = para.AppendChild(new A.Run());

                    //// Insert the image
                    //InsertImage(run, "");

                    TextReader textReader = new StringReader(html);

                    //var paragraphs = converter.Parse(textReader.ReadToEnd().ToString());
                    var paragraphs = converter.Parse(html);

                    for (int i = 0; i < paragraphs.Count; i++)
                    {
                        var espaçoEsquerda = "1500";
                        var espaçoDireita = "1000";

                        if(i == 0)
                        {
                            ((Paragraph)paragraphs[i]).ParagraphProperties = new ParagraphProperties(
                            new Indentation() { Left = espaçoEsquerda, Right = espaçoDireita, Start = espaçoEsquerda, End = espaçoDireita },
                            new Justification() { Val = JustificationValues.Center });
                            body.Append(paragraphs[i]);
                        }
                        else
                        {
                            ((Paragraph)paragraphs[i]).ParagraphProperties = new ParagraphProperties(
                            new Indentation() { Left = espaçoEsquerda, Right = espaçoDireita, Start = espaçoEsquerda, End = espaçoDireita },
                            new Justification() { Val = JustificationValues.Both });
                            body.Append(paragraphs[i]);
                            //    var para = ((Paragraph)paragraphs[i]);

                            //    para.ParagraphProperties = new ParagraphProperties(
                            //    new Indentation() { Left = espaçoEsquerda, Right = espaçoDireita, Start = espaçoEsquerda, End = espaçoDireita },
                            //    new Justification() { Val = JustificationValues.Both });

                            //    var run = para.Elements<Run>().GetEnumerator();

                            //    var lastRead = run.Current;

                            //    while (true)
                            //    {
                            //        var hasNext = run.MoveNext();

                            //        if (hasNext)
                            //        {
                            //            lastRead = run.Current;
                            //        }
                            //        else
                            //        {
                            //            break;
                            //        }
                            //    }

                            //    var texts = lastRead.Elements<Text>().GetEnumerator();

                            //    var lastTextRead = texts.Current;

                            //    while (true)
                            //    {
                            //        var hasNext = texts.MoveNext();

                            //        if (hasNext)
                            //        {
                            //            lastTextRead = texts.Current;
                            //        }
                            //        else
                            //        {
                            //            break;
                            //        }
                            //    }

                            //    if (lastTextRead != null)
                            //    {
                            //        lastRead.Append(new Break());
                            //    }

                            //    body.Append(paragraphs[i]);
                        }



                    }

                        HeaderPart headerPart = mainPart.AddNewPart<HeaderPart>();
                    var headerImagePart = headerPart.AddImagePart(ImagePartType.Png);

                    using (MemoryStream headerStream = new MemoryStream())
                    {
                        imagemCabeçalho.Save(headerStream, ImageFormat.Png);

                        headerStream.Position = 0;

                        headerImagePart.FeedData(headerStream);
                    }

                    var headerImagePartId = headerPart.GetIdOfPart(headerImagePart);

                    var headerElement = new Header();
                    headerElement.AddChild(
                        new Paragraph(
                            new ParagraphProperties(new ParagraphStyleId() { Val = "Header" }),
                            new Run(BuildImageElement(headerImagePartId,
                            larguraCabeçalho,
                            alturaCabeçalho,
                            "{28A0092B-C50C-407E-A947-70E740481C1C}",
                            "cabeçalho",
                            0,
                            "50D07946"))
                            )
                        );

                    headerElement.Save(headerPart);

                    FooterPart footerPart = mainPart.AddNewPart<FooterPart>();
                    var footerImagePart = footerPart.AddImagePart(ImagePartType.Png);

                    using (MemoryStream footerStream = new MemoryStream())
                    {
                        imagemRodapé.Save(footerStream, ImageFormat.Png);

                        footerStream.Position = 0;

                        footerImagePart.FeedData(footerStream);
                    }

                    var footerImagePartId = footerPart.GetIdOfPart(footerImagePart);

                    var footerElement = new Footer();
                    footerElement.AddChild(
                        new Paragraph(
                            GerarRodapé(),
                            new Run(BuildImageElement(footerImagePartId,
                            larguraRodapé,
                            alturaRodapé,
                            "{28A0092B-C50C-407E-A947-70E740481C1D}",
                            "rodapé",
                            5,
                            "50D07947"))
                            )
                        );

                    footerElement.Save(footerPart);

                    body.Append(new SectionProperties(new SectionProperties(
                        new PageMargin()
                        {
                            Top = 1440,
                            Right = (UInt32Value)0UL,
                            Bottom = 2440,
                            Left = (UInt32Value)0UL,
                            Header = (UInt32Value)0UL,
                            Footer = (UInt32Value)0UL,
                            Gutter = (UInt32Value)0UL
                        },

                            //Top = 1440,
                            //Right = (UInt32Value)0UL,
                            //Bottom = 1440,
                            //Left = (UInt32Value)0UL,
                            //Header = (UInt32Value)0UL,
                            //Footer = (UInt32Value)0UL,
                            //Gutter = (UInt32Value)0UL


                        new HeaderReference()
                        {
                            Type = HeaderFooterValues.Default,
                            Id = mainPart.GetIdOfPart(headerPart)
                        },
                        new FooterReference()
                        {
                            Type = HeaderFooterValues.Default,
                            Id = mainPart.GetIdOfPart(footerPart)
                        }
                        )));

                    mainPart.Document.Save();
                }

                return generatedDocument.ToArray();
            }
        }

        private static ParagraphProperties GerarRodapé()
        {
            var novoEstiloDeParágrafo = new ParagraphProperties();

            SpacingBetweenLines espaçamento = new SpacingBetweenLines()
            {
                Line = "250",
                LineRule = LineSpacingRuleValues.Auto,
                Before = "0",
                After = "0"
            };

            novoEstiloDeParágrafo.Append(espaçamento);

            return novoEstiloDeParágrafo;
        }

        private static Drawing BuildImageElement(string relationshipId, long tamanhoX, long tamanhoY, string uri, string nome, UInt32 id, string idEdição)
        {
            var element = new Drawing(
                new DW.Inline(
                new DW.Extent() { Cx = tamanhoX, Cy = tamanhoY },
                new DW.EffectExtent()
                {
                    LeftEdge = 0L,
                    TopEdge = 0L,
                    RightEdge = 0L,
                    BottomEdge = 0L
                },
                new DW.DocProperties()
                {
                    Id = id + 1,
                    Name = nome,
                },
                    new DW.NonVisualGraphicFrameDrawingProperties(
                    new A.GraphicFrameLocks() { NoChangeAspect = true }),
                    new A.Graphic(
                        new A.GraphicData(
                            new PIC.Picture(
                                new PIC.NonVisualPictureProperties(
                                    new PIC.NonVisualDrawingProperties()
                                    {
                                        Id = id,
                                        Name = nome + ".png"
                                    },
                                new PIC.NonVisualPictureDrawingProperties()),
                                new PIC.BlipFill(
                                    new A.Blip(
                                        new A.BlipExtensionList(
                                            new A.BlipExtension()
                                            {
                                                Uri = uri
                                            })
                                    )
                                    {
                                        Embed = relationshipId,
                                        CompressionState =
                                        A.BlipCompressionValues.Print
                                    },
                                    new A.Stretch(
                                        new A.FillRectangle())),
                                        new PIC.ShapeProperties(
                                            new A.Transform2D(
                                                new A.Offset() { X = 0L, Y = 0L },
                                                new A.Extents() { Cx = tamanhoX, Cy = tamanhoY }),
                                                new A.PresetGeometry(
                                                    new A.AdjustValueList()
                                                )
                                                {
                                                    Preset = A.ShapeTypeValues.Rectangle
                                                }))
                        )
                        {
                            Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture"
                        })
                    )
                {
                    DistanceFromTop = (UInt32Value)0U,
                    DistanceFromBottom = (UInt32Value)0U,
                    DistanceFromLeft = (UInt32Value)0U,
                    DistanceFromRight = (UInt32Value)0U,
                    EditId = idEdição
                });

            return element;
        }

        // Method to insert an image
        private static void InsertImage(A.Run run, string imagePath)
        {
            ImagePart imagePart = null;
            using (FileStream stream = new FileStream(imagePath, FileMode.Open))
            {
                //imagePart = run.MainDocumentPart.AddImagePart(ImagePartType.Jpeg);
                imagePart.FeedData(stream);
            }

            // Create a relationship between the image and the document
            //string relationshipId = run.Document.MainDocumentPart.GetIdOfPart(imagePart);

            // Define the reference of the image within the document
            Drawing drawing = new Drawing();
            run.AppendChild(drawing);

            // Add an inline image with size adjustment
            //AddImageToBody(drawing, relationshipId, 300, 200); // Adjust dimensions as needed
        }

        // Method to add image to body with size adjustment
        private static void AddImageToBody(Drawing drawing, string relationshipId, long cx, long cy)
        {
            // Define image dimensions and positioning
            var element = new Drawing(
                new DW.Inline(
                    new DW.Extent() { Cx = cx, Cy = cy },
                    new DW.EffectExtent()
                    {
                        LeftEdge = 0L,
                        TopEdge = 0L,
                        RightEdge = 0L,
                        BottomEdge = 0L
                    },
                    new DW.DocProperties()
                    {
                        Id = (UInt32Value)1U,
                        Name = "Image"
                    },
                    new DW.NonVisualGraphicFrameDrawingProperties(
                        new A.GraphicFrameLocks() { NoChangeAspect = true }),
                    new A.Graphic(
                        new A.GraphicData(
                            new PIC.Picture(
                                new PIC.NonVisualPictureProperties(
                                    new PIC.NonVisualDrawingProperties()
                                    {
                                        Id = (UInt32Value)0U,
                                        Name = "New Image"
                                    },
                                    new PIC.NonVisualPictureDrawingProperties()),
                                new PIC.BlipFill(
                                    new A.Blip(
                                        new A.BlipExtensionList(
                                            new A.BlipExtension()
                                            {
                                                Uri =
                                                    "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                            })
                                    )
                                    {
                                        Embed = relationshipId,
                                        CompressionState =
                                            A.BlipCompressionValues.Print
                                    },
                                    new A.Stretch(
                                        new A.FillRectangle())),
                                new PIC.ShapeProperties(
                                    new A.Transform2D(
                                        new A.Offset() { X = 0L, Y = 0L },
                                        new A.Extents() { Cx = cx, Cy = cy }),
                                    new A.PresetGeometry(
                                        new A.AdjustValueList()
                                    )
                                    { Preset = A.ShapeTypeValues.Rectangle }))
                        )
                        { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                )
                { DistanceFromTop = (UInt32Value)0U, DistanceFromBottom = (UInt32Value)0U, DistanceFromLeft = (UInt32Value)0U, DistanceFromRight = (UInt32Value)0U });

            drawing.Append(element);
        }
    }
}
