using Sigman.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Suite;

namespace Uranus.WebManager.Controllers
{
    public class FinanceiroGestaoBoletosController : Controller
    {
        public ActionResult Index(string AnoInicio = "", string AnoFim = "", string Cidade = "", DateTime? vencimento = null)
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

        public ActionResult GerarRemessa()
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                try
                {

                    var empresa = EmpresaBo.Consultar(1);
                    var boletos = ClientesNotasParcelasBO.ListarBoletos("N");

                    foreach (var item in boletos)
                    {
                        // **** gerar boleto
                        var clientesnotas = ClientesNotasBO.Consultar(long.Parse(item.IdClientesNota.ToString()));
                        var clientes = ClientesBo.Consultar(int.Parse(clientesnotas.IdCliente.ToString()));


                        var clientesnotasparcelas = ClientesNotasParcelasBO.ListarParcela(item.Id);

                        clientesnotasparcelas.NossoNumero = "";
                        clientesnotasparcelas.CODBARRAS = "";
                        clientesnotasparcelas.Arquivo = "";
                        clientesnotasparcelas.Status = "G";
                        ClientesNotasParcelasBO.Salvar(clientesnotasparcelas);

                    }

                    //var empresa = EmpresaBo.Consultar(1);
                    //var competencia = "";
                    //string path1 = "C:\\CaixaSintrapostos\\RETORNO";
                    //string path2 = "C:\\CaixaSintrapostos\\RetornosProcessados";
                    //string path3 = "C:\\CaixaSintrapostos\\Erros";

                    //string[] allfiles = Directory.GetFiles(path1, "*.*", SearchOption.AllDirectories);

                    //foreach (string file in allfiles)
                    //{
                    //    StreamReader Arquivo = new StreamReader(file);

                    //    try
                    //    {
                    //        string linha;
                    //        int mescodigo = 0;
                    //        int anocodigo = 0;
                    //        int idEmpresa = 0;
                    //        string nossonumero = "";
                    //        DateTime? Datapag = null;
                    //        decimal Valor = 0;

                    //        while ((linha = Arquivo.ReadLine()) != null)
                    //        {

                    //            if (linha[13] == 'T')
                    //            {
                    //                mescodigo = 0;
                    //                anocodigo = 0;
                    //                idEmpresa = 0;
                    //                nossonumero = "";
                    //                Datapag = null;
                    //                Valor = 0;
                    //                nossonumero = (linha.Substring(39, 17)).Trim();
                    //                if (nossonumero != "")
                    //                {
                    //                    mescodigo = int.Parse(linha.Substring(75, 2));
                    //                    anocodigo = int.Parse(linha.Substring(77, 4));
                    //                    competencia = linha.Substring(75, 6);
                    //                    idEmpresa = int.Parse(BuscaCodigo(linha));
                    //                }
                    //            }

                    //            if (linha[13] == 'U')
                    //            {
                    //                Valor = decimal.Parse(linha.Substring(77, 15)) / 100;
                    //                if ((linha.Substring(145, 2)) != "00")
                    //                {
                    //                    Datapag = new DateTime(int.Parse(linha.Substring(149, 4)), int.Parse(linha.Substring(147, 2)), int.Parse(linha.Substring(145, 2)));

                    //                    //var pagamento = EmpresasPagamentosBO.ConsultarPagamento(Sessao.IdSindicato, nossonumero);

                    //                    //pagamento.DataPagamento = Datapag;

                    //                    //EmpresasPagamentosBO.Salvar(pagamento);
                    //                }

                    //            }
                    //        }

                    //        Arquivo.Close();

                    //        System.IO.File.Move(file, file.Replace(path1, path2));
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Arquivo.Close();

                    //        System.IO.File.Move(file, file.Replace(path1, path3));
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    var resultError = new { codigo = "99", error = ex.Message.ToString() };
                    return Json(resultError);
                }


                var result = new { codigo = "00" };
                return Json(result);
            }
        }

        public static string EnviarEmailBoleto()
        {
            var motivo = "Emails não enviados";
            var to = string.Empty;
            var name = string.Empty;
            var emailremetente = string.Empty;
            var passwordremetente = string.Empty;
            to = string.Empty;
            var Titulo = string.Empty;
            var mensagemEmail = string.Empty;
            var nomePdf = string.Empty;

            var empresa = EmpresaBo.Buscar(1);
            var boletos = ClientesNotasParcelasBO.ListarBoletos("G");

            Int64 idnota = 0;

            foreach (var item in boletos)
            {
                if (idnota == 0)
                {
                    idnota = long.Parse(item.IdClientesNota.HasValue.ToString());
                }
                else
                {
                    if (idnota != item.IdClientesNota)
                    {
                        idnota = long.Parse(item.IdClientesNota.HasValue.ToString());

                        //List<Attachment> attach = new List<Attachment>();
                        //byte[] bytes = System.IO.File.ReadAllBytes(boleto);
                        //MemoryStream ms = new MemoryStream(boleto);
                        //Attachment attachment = new Attachment(ms, string.Format("Boleto-{0}.pdf", nomePdf, 50) );
                        //attach.Add(attachment);
                        //if (!Uranus.Common.Mail.SendBoleto(emailremetente, passwordremetente, to, Titulo, mensagemEmail, attach))
                        //{
                        //    motivo = "Ocorreu uma falha ao enviar os E-mails. Tente novamente mais tarde ou entre em contato com o administrador do sistema.";
                        //}
                        //else
                        //{
                        //    motivo = "Email enviado com sucesso";
                        //}
                    }
                }
                // **** gerar pdf
                var clientesnotas = ClientesNotasBO.Consultar(long.Parse(item.IdClientesNota.ToString()));
                var clientes = ClientesBo.Consultar(int.Parse(clientesnotas.IdCliente.ToString()));

                var email = ClientesBo.ConsultarEmailsBoletos(clientes.IDPessoa);
                nomePdf = clientesnotas.NumeroDocumento.ToString();
                to = string.Empty;
                name = clientes.Pessoas.Nome;
                emailremetente = empresa.Email;
                passwordremetente = empresa.Senha;
                to = email.Email1;
                Titulo = "Boletos de RVMADVOGADOS";
                mensagemEmail = "Prezado Sr(a) " + clientes.Pessoas.Nome + ", Segue em anexo boletos de RVMAdvogados";

                return motivo;

            }
            return motivo;
        }



        public ActionResult GerarRetorno(Int32 Id)
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                try
                {
                    var empresa = EmpresaBo.Consultar(1);
                    var competencia = "";
                    string path1 = "C:\\CaixaSintrapostos\\RETORNO";
                    string path2 = "C:\\CaixaSintrapostos\\RetornosProcessados";
                    string path3 = "C:\\CaixaSintrapostos\\Erros";

                    string[] allfiles = Directory.GetFiles(path1, "*.*", SearchOption.AllDirectories);

                    foreach (string file in allfiles)
                    {
                        StreamReader Arquivo = new StreamReader(file);

                        try
                        {
                            string linha;
                            int mescodigo = 0;
                            int anocodigo = 0;
                            int idEmpresa = 0;
                            string nossonumero = "";
                            DateTime? Datapag = null;
                            decimal Valor = 0;

                            while ((linha = Arquivo.ReadLine()) != null)
                            {

                                if (linha[13] == 'T')
                                {
                                    mescodigo = 0;
                                    anocodigo = 0;
                                    idEmpresa = 0;
                                    nossonumero = "";
                                    Datapag = null;
                                    Valor = 0;
                                    nossonumero = (linha.Substring(39, 17)).Trim();
                                    if (nossonumero != "")
                                    {
                                        mescodigo = int.Parse(linha.Substring(75, 2));
                                        anocodigo = int.Parse(linha.Substring(77, 4));
                                        competencia = linha.Substring(75, 6);
                                        idEmpresa = int.Parse(BuscaCodigo(linha));
                                    }
                                }

                                if (linha[13] == 'U')
                                {
                                    Valor = decimal.Parse(linha.Substring(77, 15)) / 100;
                                    if ((linha.Substring(145, 2)) != "00")
                                    {
                                        Datapag = new DateTime(int.Parse(linha.Substring(149, 4)), int.Parse(linha.Substring(147, 2)), int.Parse(linha.Substring(145, 2)));

                                        //var pagamento = EmpresasPagamentosBO.ConsultarPagamento(Sessao.IdSindicato, nossonumero);

                                        //pagamento.DataPagamento = Datapag;

                                        //EmpresasPagamentosBO.Salvar(pagamento);
                                    }

                                }
                            }

                            Arquivo.Close();

                            System.IO.File.Move(file, file.Replace(path1, path2));
                        }
                        catch (Exception ex)
                        {
                            Arquivo.Close();

                            System.IO.File.Move(file, file.Replace(path1, path3));
                        }
                    }
                }
                catch (Exception ex)
                {
                    var resultError = new { codigo = "99", error = ex.Message.ToString() };
                    return Json(resultError);
                }


                var result = new { codigo = "00" };
                return Json(result);
            }
        }

        private static string BuscaCodigo(string linha)
        {
            string valor = linha.Substring(105, 5).Trim();
            if (valor == "")
            {
                valor = "0";
            }
            return valor;
        }

        //class Program
        //{
        //    static void Main(string[] args)
        //    {
        //        // Lista de strings Base64 dos arquivos PDF
        //        List<string> base64PdfFiles = new List<string>
        //    {
        //        "BASE64_PDF_1",
        //        "BASE64_PDF_2",
        //        "BASE64_PDF_3"
        //    };

        //        // Caminho do arquivo PDF resultante
        //        string outputFilePath = "resultado.pdf";

        //        // Mesclar PDFs
        //        UnirPdfBase64(base64PdfFiles, outputFilePath);
        //    }

        //    static void UnirPdfBase64(List<string> base64PdfFiles, string outputFilePath)
        //    {
        //        using (FileStream stream = new FileStream(outputFilePath, FileMode.Create))
        //        {
        //            using (Document document = new Document())
        //            {
        //                PdfCopy pdfCopy = new PdfCopy(document, stream);
        //                document.Open();

        //                foreach (var base64Pdf in base64PdfFiles)
        //                {
        //                    // Decodificar base64 para bytes
        //                    byte[] pdfBytes = Convert.FromBase64String(base64Pdf);

        //                    // Criar um leitor PDF com os bytes decodificados
        //                    using (PdfReader reader = new PdfReader(pdfBytes))
        //                    {
        //                        // Adicionar todas as páginas do PDF ao documento final
        //                        for (int page = 1; page <= reader.NumberOfPages; page++)
        //                        {
        //                            PdfImportedPage importedPage = pdfCopy.GetImportedPage(reader, page);
        //                            pdfCopy.AddPage(importedPage);
        //                        }
        //                    }
        //                }

        //                document.Close();
        //            }
        //        }

        //        Console.WriteLine($"PDF mesclado salvo em: {outputFilePath}");
        //    }
        //}

    }
}
