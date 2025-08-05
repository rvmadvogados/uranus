using System;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Common;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class EmpresaController : Controller
    {
        [HttpPost]
        public JsonResult Consultar()
        {
            var empresa = EmpresaBo.ConsultarArray();
            var result = new { codigo = "00", empresa = empresa };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String CNPJ, String Nome, String CEP, String Logradouro, String Numero, String Complemento, String Bairro, String Estado, String Cidade, String Telefone)
        {
            try
            {
                Empresa empresa = new Empresa();
                empresa.ID = Id;
                empresa.Cnpj = Util.OnlyNumbers(CNPJ.Trim());
                empresa.Nome = Nome.Trim();
                empresa.Cep = Util.OnlyNumbers(CEP.Trim());
                empresa.Endereco = Logradouro.Trim();
                empresa.Numero = Numero.Trim();
                empresa.Complemento = Complemento.Trim();
                empresa.Bairro = Bairro.Trim();
                empresa.Estado = Estado.Trim();
                empresa.Cidade = Cidade.Trim();
                empresa.Fone = Util.OnlyNumbers(Telefone.Trim());

                EmpresaBo.Salvar(empresa);

                var result = new { codigo = "00" };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { codigo = "99" };
                return Json(result);
            }
        }

        public JsonResult Listar()
        {
            var empresas = EmpresaBo.Consultar();
            var result = new { codigo = "00", empresas = empresas };
            return Json(result);
        }
    }
}