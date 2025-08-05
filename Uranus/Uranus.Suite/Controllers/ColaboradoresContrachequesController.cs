using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Common;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{

    public class ColaboradoresContrachequesController : Controller
    {
        // GET: ColaboradoresContracheques
        public ActionResult Index(string search = "", Int32 mes = 0, Int32 ano = 0)
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = ProfissionaisContrachequesBo.Listar(search, mes, ano);

                return View(model);
            }
        }

        public JsonResult Consultar(Int32 Id)
        {
            var profissionaiscontracheques = ProfissionaisContrachequesBo.ConsultarArray(Id);
            var result = new { codigo = "00", profissionaiscontracheques = profissionaiscontracheques };
            return Json(result);
        }

        [HttpPost]
        public JsonResult SalvarContracheques(Int32 Id, Int32 IdProfissional, Int32 Ano, Int32 Mes, String NomeArquivo)
        {
            ProfissionaisContracheques contracheques = new ProfissionaisContracheques();

            contracheques.Id = Id;
            contracheques.IdProfissional = IdProfissional;

            contracheques.Ano = Ano;
            contracheques.Mes = Mes;

            contracheques.NomeArquivo = NomeArquivo.Trim();

            contracheques.DataCadastro = DateTime.Now;
            contracheques.UsuarioCadastro = Sessao.Usuario.Nome;
            contracheques.DataAlteracao = DateTime.Now;
            contracheques.UsuarioAlteracao = Sessao.Usuario.Nome;

            if (Id == 0)
            {
                Id = ProfissionaisContrachequesBo.Inserir(contracheques);
            }

            contracheques.Id = Id;
            ProfissionaisContrachequesBo.Salvar(contracheques);


            var nomeProfissional = ProfissionaisBo.Consultar(IdProfissional);
            var titulo = "RPA/Contra Cheque Disponível";
            var mensagemEmail = "Esta disponível na Área do Colaborador a RPA/Contra Cheque referente mês " + Mes.ToString() + "/" + Ano.ToString();
            //var resposta = EnviarEmail(titulo, IdProfissional, mensagemEmail);


            var result = new { codigo = "00" };
            return Json(result);
        }

        public JsonResult ExcluirContracheques(Int32 Id)
        {
            var contracheques = ProfissionaisContrachequesBo.Consultar(Id);
            var ano = contracheques.Ano;
            var mes = contracheques.Mes;
            var nomeArquivo = contracheques.NomeArquivo;

            var file = Path.Combine(Server.MapPath("~/RH/Contracheques"), nomeArquivo);
            if (System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);
            }

            var codigo = ProfissionaisContrachequesBo.Excluir(Id);


            var result = new { codigo = codigo };
            return Json(result);
        }

        //public JsonResult UploadContracheques(Int32 Ano, Int32 Mes)
        //{
        //    try
        //    {

        //        string path1 = "C:\\Contracheques\\Contracheques";
        //        string path2 = "C:\\Contracheques\\ContrachequesProcessados";
        //        string path3 = "C:\\Contracheques\\Erros";

        //        string[] allfiles = Directory.GetFiles(path1, "*.*", SearchOption.AllDirectories);

        //        foreach (string file in allfiles)
        //        {
        //            StreamReader Arquivo = new StreamReader(file);

        //            try
        //            {
        //                var nomeArquivo = Path.GetFileName(file);
        //                var nome = "";
                        
                        
        //                var IdProfissional = ProfissionaisBo.ConsultarNome(nome).ID;
                        
        //                byte[] fileBytes = System.IO.File.ReadAllBytes(file);

        //                using (var memoryStream = new MemoryStream(fileBytes))
        //                {
        //                    // Criar um objeto HttpPostedFileBase a partir do MemoryStream
        //                    var postedFile = new CustomPostedFile(memoryStream, "application/pdf", nomeArquivo);
        //                    nomeArquivo = Ano + "-" + Mes + "-" + nomeArquivo;

        //                    // Chamar o método SalvarContracheques com o arquivo carregado
        //                    SalvarContracheques(0, IdProfissional, Ano, Mes, nomeArquivo);

        //                    var url = String.Empty;


        //                    //if (ContrachequeArquivo != null && ContrachequeArquivo.ContentLength > 0)
        //                    //{
        //                    //    var file = Path.Combine(Server.MapPath("~/RH/Contracheques"), String.Format("Contracheque-{0}.pdf", Id.ToString()));
        //                    //    ContrachequeArquivo.SaveAs(file);
        //                    //}
        //                }

        //                Arquivo.Close();
        //                System.IO.File.Move(file, file.Replace(path1, path2));
        //            }
        //            catch (Exception ex)
        //            {
        //                Arquivo.Close();

        //                System.IO.File.Move(file, file.Replace(path1, path3));
        //            }
        //        }
        //        var result = new { codigo = "00" };
        //        return Json(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        var resultError = new { codigo = "99", error = ex.Message.ToString() };
        //        return Json(resultError);
        //    }
        //}

        //public class CustomPostedFile : HttpPostedFileBase
        //{
        //    private readonly Stream _stream;
        //    private readonly string _contentType;
        //    private readonly string _fileName;

        //    public CustomPostedFile(Stream stream, string contentType, string fileName)
        //    {
        //        _stream = stream;
        //        _contentType = contentType;
        //        _fileName = fileName;
        //    }

        //    public override int ContentLength => (int)_stream.Length;
        //    public override string ContentType => _contentType;
        //    public override string FileName => _fileName;
        //    public override Stream InputStream => _stream;
        //}

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

        public ActionResult FileUpload(HttpPostedFileBase file, string competencia)
        {
            try 
            {
                var memStream = new MemoryStream();
                file.InputStream.CopyTo(memStream);
                var ano = competencia.Substring(2, 4);
                var mes = competencia.Substring(0, 2);
                var nome = file.FileName.Replace("RPA ", "").Replace("rpa ", "").Replace(".PDF", "").Replace(".pdf", "").Replace("RECIBO ", "");
                var IdProfissional = ProfissionaisBo.ConsultarNome(nome).ID;
            //    var IdProfissional = 54;
                var nomeArquivo = ano + '-' + mes + '-' + file.FileName.Replace("RPA ", "").Replace(".PDF", "").Replace("RECIBO ", "");

                byte[] fileData = memStream.ToArray();

               // string path = ConfigurationManager.AppSettings["PathUploadContracheques"];
                var path = Path.Combine(Server.MapPath("~/RH/Contracheques"));

                System.IO.File.WriteAllBytes(string.Format(@"{0}\{1}", string.Concat(path), nomeArquivo), fileData);

                SalvarContracheques(0, IdProfissional, int.Parse(ano), int.Parse(mes), nomeArquivo);

            }
            catch (Exception exception)
            {
                return Json(new
                {
                    success = false,
                    response = "Ocorreu uma falha no envio desse Contracheque."
                });
            }

            return Json(new
            {
                success = true,
                response = "Sucesso ao enviar este Contracheque."
            });
        }


        public static string EnviarEmail(String Titulo, Int32 IdProfissional, String mensagemEmail)
        {
            var motivo = string.Empty;

            var empresa = EmpresaBo.Buscar(1);

            var menssage = string.Empty;
            var to = string.Empty;
            var name = "";
            var emailremetente = empresa.Email;
            var passwordremetente = empresa.Senha;

            var email = ProfissionaisBo.BuscarEmailProfissional(IdProfissional);

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
            menssage += "    <table id='customers'>";
            menssage += "      <tr>";
            menssage += "        <td>" + mensagemEmail;
            menssage += "      </tr>";

            if (!string.IsNullOrEmpty(menssage))
            {
                menssage += "    </table>";
                menssage += "  </body>";
                menssage += "</html>";

                foreach (var item in email.Pessoas.Email)
                {
                    to = item.Email1;
                    if (!Mail.Send(emailremetente, passwordremetente, to, Titulo + name, menssage))
                    {
                        motivo = "Ocorreu uma falha ao enviar os E-mails. Tente novamente mais tarde ou entre em contato com o administrador do sistema.";
                    }
                    else
                    {
                        motivo = "Email enviado com sucesso";
                    }
                }


            }


            return motivo;
        }

    }

}