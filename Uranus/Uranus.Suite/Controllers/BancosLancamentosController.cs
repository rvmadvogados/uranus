using System;
using System.Data.Entity;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class BancosLancamentosController : Controller
    {
        // GET: BancosLancamentos
        public ActionResult Index(Int32 FiltrarBanco = 0, string FiltrarDataInicio = null, string FiltrarDataFim = null)
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {

                if (FiltrarDataInicio == null)
                {
                    FiltrarDataInicio = DateTime.Now.ToString("dd/MM/yyyy");
                }
                if (FiltrarDataFim == null)
                {
                    FiltrarDataFim = DateTime.Now.ToString("dd/MM/yyyy");
                }

                var dataInicio = DateTime.Parse(FiltrarDataInicio);
                var dataFim = DateTime.Parse(FiltrarDataFim);

                var model = BancosLancamentosBo.Listar(FiltrarBanco, dataInicio, dataFim.AddDays(1));
                ViewBag.FiltrarBanco = FiltrarBanco;
                ViewBag.FiltrarDataInicio = FiltrarDataInicio;
                ViewBag.FiltrarDataFim = FiltrarDataFim;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Consultar(Int64 Id)
        {
            var bancoslancamento = BancosLancamentosBo.ConsultarArray(Id);
            var result = new { codigo = "00", bancoslancamento = bancoslancamento };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int64 Id, String Data, Int32 IdBanco, Int32 IdHistorico, String Complemento, String ValorDebito, String ValorCredito)
        {


            BancosLancamentos bancoslancamento = new BancosLancamentos();
            bancoslancamento.ID = Id;
            bancoslancamento.Data = DateTime.Parse(Data);
            bancoslancamento.IdBanco = IdBanco;
            bancoslancamento.IdHistorico = IdHistorico;
            bancoslancamento.Complemento = Complemento;
            if (ValorDebito != null && ValorDebito.Trim().Length > 0)
            {
                bancoslancamento.ValorDebito = Decimal.Parse(ValorDebito);
            }
            if (ValorCredito != null && ValorCredito.Trim().Length > 0)
            {
                bancoslancamento.ValorCredito = Decimal.Parse(ValorCredito);
            }


            if (Id == 0)
                Id = BancosLancamentosBo.Inserir(bancoslancamento);

            bancoslancamento.ID = Id;
            BancosLancamentosBo.Salvar(bancoslancamento);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int64 Id)
        {
            var codigo = BancosLancamentosBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Listar()
        {
            var bancoslancamento = BancosLancamentosBo.Consultar();
            var result = new { codigo = "00", bancoslancamento = bancoslancamento };
            return Json(result);
        }
    }

    //using System;
    //using System.IO;
    //using OfxSharp;

    //class Program
    //    {
    //        static void Main()
    //        {
    //            // Caminho para o arquivo OFX
    //            string filePath = @"ArquivosOFX\arquivo.ofx";

    //            // Certifique-se de que o arquivo existe
    //            if (!File.Exists(filePath))
    //            {
    //                Console.WriteLine("Arquivo não encontrado.");
    //                return;
    //            }

    //            // Leitura e processamento do arquivo OFX
    //            try
    //            {
    //                string ofxContent = File.ReadAllText(filePath);

    //                // Criar um parser OFX
    //                OfxDocumentParser parser = new OfxDocumentParser();
    //                var ofxDocument = parser.Import(new StringReader(ofxContent));

    //                // Exemplo: Listar transações
    //                foreach (var transaction in ofxDocument.Transactions)
    //                {
    //                    Console.WriteLine($"Data: {transaction.Date}");
    //                    Console.WriteLine($"Tipo: {transaction.TransactionType}");
    //                    Console.WriteLine($"Valor: {transaction.Amount}");
    //                    Console.WriteLine($"Descrição: {transaction.Memo}");
    //                    Console.WriteLine("--------------------");
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                Console.WriteLine($"Erro ao processar o arquivo OFX: {ex.Message}");
    //            }
    //        }
    //    }

}