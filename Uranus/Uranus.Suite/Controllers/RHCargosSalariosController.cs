using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Uranus.Business;
using Uranus.Common;
using Uranus.Data;
using Uranus.Domain;

namespace Uranus.Suite.Controllers
{
    public class RHCargosSalariosController : Controller
    {
        public ActionResult Index(string search = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var model = RHCargosBo.Listar(search);
                ViewBag.search = search;
                return View(model);
            }
        }

        [HttpPost]
        public JsonResult Listar()
        {
            var cargos = RHCargosBo.ListarArray();
            var result = new { codigo = "00", cargos = cargos };
            return Json(result);
        }

        [HttpPost]
        public JsonResult ListarCargos()
        {
            var cargos = RHCargosBo.ListarCargos();
            var result = new { codigo = "00", cargos = cargos };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Consultar(Int32 Id)
        {
            var cargos = RHCargosBo.ConsultarArray(Id);
            var result = new { codigo = "00", cargos = cargos };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Salvar(Int32 Id, String Nome, String Descricao, String Atribuicoes, Int32? IdAreaResponsavel, Int32? IdProximoCargo)

        {
            RHCargos cargos = new RHCargos();
            cargos.Id = Id;
            cargos.Nome = Nome.Trim();
            cargos.Descricao = Descricao;
            cargos.Atribuicoes = Atribuicoes.Trim();
            cargos.IdAreaResponsavel = IdAreaResponsavel;
            cargos.IdProximoCargo = IdProximoCargo;

            cargos.DataCadastro = DateTime.Now;
            cargos.UsuarioCadastro = Sessao.Usuario.Nome;
            cargos.DataAlteracao = DateTime.Now;
            cargos.UsuarioAlteracao = Sessao.Usuario.Nome;

            if (Id == 0)
            {
                Id = RHCargosBo.Inserir(cargos);

            }

            cargos.Id = Id;
            RHCargosBo.Salvar(cargos);

            var result = new { codigo = "00" };
            return Json(result);
        }

        [HttpPost]
        public JsonResult Excluir(Int32 Id)
        {
            var codigo = RHCargosBo.Excluir(Id);

            var result = new { codigo = codigo };
            return Json(result);
        }

        [HttpPost]
        public JsonResult CarregarDadosNiveis(Int32 IdCargo)
        {
            try
            {
                var nivel = RHCargosNiveisBo.ListarNiveis(IdCargo);
                String niveis = String.Empty;

                int SalariosQuantidade = (nivel.Count > 0 ? (nivel.Count + 1) : 1);

                for (int i = 0; i < SalariosQuantidade; i++)
                {
                    niveis += "<input id='" + String.Format("HiddenDadosNivel{0}", (i + 1)) + "' name='HiddenDadosNivel[]' type='hidden' value='" + (nivel.Count >= (i + 1) ? nivel[i].Id.ToString() : "0") + "' />";
                    niveis += "<div class='accordion panel panel-default'>";
                    niveis += "   <div class='accordion-heading panel-heading' id='" + String.Format("DadosNivel{0}heading", (i + 1)) + "' role='tab'>";
                    niveis += "      <h4 class='accordion-title panel-title'>";
                    niveis += "         <a href='" + String.Format("#DadosNivel{0}collapse", (i + 1)) + "' aria-expanded='false' data-toggle='collapse' aria-controls='" + String.Format("DadosNivel{0}collapse", (i + 1)) + "' data-parent='#DadosNiveisAccordion' role='button' class='collapsed'>";
                    niveis += "            " + String.Format("Nível #{0}", (i + 1));
                    niveis += (nivel.Count >= (i + 1) ? String.Format(" - {0} ", (nivel[i].Nome != null ? nivel[i].Nome : string.Empty)) : String.Empty);

                    niveis += "         </a>";
                    niveis += "      </h4>";
                    niveis += "   </div>";
                    niveis += "   <div class='panel-collapse collapse' id='" + String.Format("DadosNivel{0}collapse", (i + 1)) + "' role='tabpanel' aria-labelledby='" + String.Format("DadosNivel{0}heading", (i + 1)) + "' aria-expanded='false' style='height: 0px;'>";
                    niveis += "      <div class='accordion-body panel-body'>";
                    niveis += "         <div class='row'>";
                    niveis += "            <div class='col-md-8'>";
                    niveis += "               <div class='form-group'>";
                    niveis += "                  <label class='titlefield'>Nome <span class='required'>*</span></label>";
                    niveis += "                  <input type='text' id='" + String.Format("DadosNivel{0}Nome", (i + 1)) + "' name='DadosNivelNome[]' class='form-control' placeholder='Informe nome do cargo' data-msg-required='Por favor, insira o nome.' data-rule-required='true' value='" + (nivel.Count >= (i + 1) ? nivel[i].Nome : string.Empty) + "' />";
                    niveis += "               </div>";
                    niveis += "            </div>";
                    niveis += "         </div>";
                    niveis += "         <div class='row'>";
                    niveis += "            <div class='col-md-12'>";
                    niveis += "               <div class='form-group'>";
                    niveis += "                  <label class='titlefield'>Desrição</label>";
                    niveis += "                   <textarea id='" + String.Format("DadosNivel{0}Descricao", (i + 1)) + "' name='DadosNivelDescricao[]' class='form-control' rows='3'> " + (nivel.Count >= (i + 1) ? nivel[i].Descricao : String.Empty) + "</textarea>";
                    niveis += "               </div>";
                    niveis += "            </div>";
                    niveis += "         </div>";
                    niveis += "         <div class='row'>";
                    niveis += "            <div class='col-md-12'>";
                    niveis += "               <div class='form-group'>";
                    niveis += "                  <label class='titlefield'>Atribuições</label>";
                    niveis += "                   <textarea id='" + String.Format("DadosNivel{0}Atribuicoes", (i + 1)) + "' name='DadosNivelAtribuicoes[]' class='form-control' rows='3'> " + (nivel.Count >= (i + 1) ? nivel[i].Atribuicoes : String.Empty) + "</textarea>";
                    niveis += "               </div>";
                    niveis += "            </div>";
                    niveis += "         </div>";
                    niveis += "         <div class='row'>";
                    niveis += "            <div class='col-md-4'>";
                    niveis += "               <button type='button' id='" + String.Format("ButtonDadosNivel{0}Incluir", (i + 1)) + "' name='" + String.Format("ButtonDadosNivel{0}Incluir", (i + 1)) + "' class='btn btn-primary' data-loading-text='Salvando...' onclick='ButtonDadosNivelSalvar_Click(" + (i + 1) + ");'>";
                    niveis += "                  Salvar";
                    niveis += "               </button>";
                    niveis += "            </div>";
                    niveis += "            <div class='col-md-3' style='text-align: center;'>";
                    niveis += "            </div>";
                    niveis += "            <div class='col-md-5' style='text-align: right;'>";
                    niveis += "               <button type='button' id='" + String.Format("ButtonDadosNivel{0}Excluir", (i + 1)) + "' name='" + String.Format("ButtonDadosNivel{0}Excluir", (i + 1)) + "' class='btn btn-danger' data-loading-text='Apagando...' onclick='ButtonDadosNivelExcluir_Click(" + (i + 1) + ");' " + (nivel.Count >= (i + 1) ? String.Empty : "disabled='disabled'") + ">";
                    niveis += "                  Apagar";
                    niveis += "               </button>";
                    niveis += "               <div id='" + String.Format("ButtonDadosNivel{0}ExcluirConfirmacao", (i + 1)) + "' style='display:none'>";
                    niveis += "                  <label id='" + String.Format("LabelDadosNivel{0}ExcluirConfirmacao", (i + 1)) + "' style='font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente excluir esse Salário?</label><br />";
                    niveis += "                  <button id='" + String.Format("accept{0}", (i + 1)) + "' class='btn btn-default btn-xs' value='true'>Sim</button>";
                    niveis += "                  <button id='" + String.Format("go_back{0}", (i + 1)) + "' class='btn btn-primary btn-xs' value='false'>Não</button>";
                    niveis += "               </div>";
                    niveis += "            </div>";
                    niveis += "         </div>";
                    niveis += "      </div>";
                    niveis += "   </div>";
                    niveis += "</div>";
                }

                var result = new { response = "success", niveis = niveis, count = SalariosQuantidade };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult SalvarDadosNiveis(Int32 Id, Int32 IdCargo, String Nome, String Descricao, String Atribuicoes) //, Int32? IdAreaResponsavel, Int32? IdProximoCargo
        {
            try
            {
                RHCargosNiveis cargonivel = new RHCargosNiveis();
                cargonivel.Id = Id;
                cargonivel.IdCargo = IdCargo;
                cargonivel.Nome = Nome.Trim();
                cargonivel.Descricao = Descricao;
                cargonivel.Atribuicoes = Atribuicoes.Trim();
                //cargonivel.IdAreaResponsavel = IdAreaResponsavel;
                //cargonivel.IdProximoCargo = IdProximoCargo;

                cargonivel.DataCadastro = DateTime.Now;
                cargonivel.UsuarioCadastro = Sessao.Usuario.Nome;
                cargonivel.DataAlteracao = DateTime.Now;
                cargonivel.UsuarioAlteracao = Sessao.Usuario.Nome;

                if (Id == 0)
                    Id = RHCargosNiveisBo.Inserir(cargonivel);

                cargonivel.Id = Id;
                RHCargosNiveisBo.Salvar(cargonivel);

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
        public JsonResult ExcluirDadosNiveis(Int32 Id)
        {
            try
            {
                RHCargosNiveisBo.Excluir(Id);

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
        public JsonResult CarregarDadosSalarios(Int32 IdCargo)
        {
            try
            {
                var salario = RHCargosSalariosBo.Listar(IdCargo);
                String salarios = String.Empty;

                int SalariosQuantidade = (salario.Count > 0 ? (salario.Count + 1) : 1);

                for (int i = 0; i < SalariosQuantidade; i++)
                {
                    salarios += "<input id='" + String.Format("HiddenDadosNivel{0}", (i + 1)) + "' name='HiddenDadosNivel[]' type='hidden' value='" + (salario.Count >= (i + 1) ? salario[i].Id.ToString() : "0") + "' />";
                    salarios += "<div class='accordion panel panel-default'>";
                    salarios += "   <div class='accordion-heading panel-heading' id='" + String.Format("DadosSalario{0}heading", (i + 1)) + "' role='tab'>";
                    salarios += "      <h4 class='accordion-title panel-title'>";
                    salarios += "         <a href='" + String.Format("#DadosSalario{0}collapse", (i + 1)) + "' aria-expanded='false' data-toggle='collapse' aria-controls='" + String.Format("DadosSalario{0}collapse", (i + 1)) + "' data-parent='#DadosSalariosAccordion' role='button' class='collapsed'>";
                    salarios += "            " + String.Format("Salario #{0}", (i + 1));
                    salarios += (salario.Count >= (i + 1) ? String.Format(" - {0} - {1} - {2}", (salario[i].Salario != null ? string.Format("R$ {0:##,##0.00}", salario[i].Salario) : "0,00").PadLeft(10, ' '), (salario[i].DataInicio != null ? salario[i].DataInicio.Value.ToString("dd/MM/yyyy") : string.Empty), (salario[i].DataFim != null ? salario[i].DataFim.Value.ToString("dd/MM/yyyy") : string.Empty)) : String.Empty);

                    salarios += "         </a>";
                    salarios += "      </h4>";
                    salarios += "   </div>";
                    salarios += "   <div class='panel-collapse collapse' id='" + String.Format("DadosSalario{0}collapse", (i + 1)) + "' role='tabpanel' aria-labelledby='" + String.Format("DadosSalario{0}heading", (i + 1)) + "' aria-expanded='false' style='height: 0px;'>";
                    salarios += "      <div class='accordion-body panel-body'>";
                    salarios += "         <div class='row'>";
                    salarios += "            <div class='col-md-4'>";
                    salarios += "               <div class='form-group'>";
                    salarios += "                  <label class='titlefield'>Salário <span class='required'>*</span></label>";
                    salarios += "                  <input type='text' id='" + String.Format("DadosSalario{0}Salario", (i + 1)) + "' name='DadosSalarioSalario[]' class='form-control' placeholder='Informe um Valor' data-msg-required='Por favor, insira o valor.' data-rule-required='true' maxlength='14' value='" + (salario.Count >= (i + 1) ? salario[i].Salario.Value.ToString("#,0.00", new CultureInfo("pt-BR")) : string.Empty) + "' />";
                    salarios += "               </div>";
                    salarios += "            </div>";
                    salarios += "            <div class='col-md-2'>";
                    salarios += "                <div class='form-group'>";
                    salarios += "                   <label class='titlefield'>Data Início <span class='required'>*</span></label>";
                    salarios += "                   <input id='" + String.Format("DadosSalario{0}DataInicio", (i + 1)) + "' name='DadosSalarioDataInicio[]' class='form-control' placeholder='Selecione uma Data' type='text' style='height: 40px !important; cursor: pointer;' value='" + (salario.Count >= (i + 1) ? salario[i].DataInicio.Value.ToString("dd/MM/yyyy") : string.Empty) + "' readonly />";
                    salarios += "                   <label id='" + String.Format("DadosSalario{0}DataInicio", (i + 1)) + "' name='DadosSalarioDataInicioValidate[]' for='" + String.Format("salario{0}DataInicio", (i + 1)) + "' class='error hidden'>Por favor, selecionar uma Data.</label>";
                    salarios += "                </div>";
                    salarios += "             </div>";
                    salarios += "            <div class='col-md-2'>";
                    salarios += "                <div class='form-group'>";
                    salarios += "                   <label class='titlefield'>Data Fim <span class='required'>*</span></label>";
                    salarios += "                   <input id='" + String.Format("DadosSalario{0}DataFim", (i + 1)) + "' name='DadosSalarioDataFim[]' class='form-control' placeholder='Selecione uma Data' type='text' style='height: 40px !important; cursor: pointer;' value='" + (salario.Count >= (i + 1) ? salario[i].DataFim.Value.ToString("dd/MM/yyyy") : string.Empty) + "' readonly />";
                    salarios += "                   <label id='" + String.Format("DadosSalario{0}DataFim", (i + 1)) + "' name='DadosSalarioDataFimValidate[]' for='" + String.Format("salario{0}DataFim", (i + 1)) + "' class='error hidden'>Por favor, selecionar uma Data.</label>";
                    salarios += "                </div>";
                    salarios += "             </div>";
                    salarios += "         </div>";
                    salarios += "         <div class='row'>";
                    salarios += "            <div class='col-md-12'>";
                    salarios += "               <div class='form-group'>";
                    salarios += "                  <label class='titlefield'>Motivo Mudança</label>";
                    salarios += "                   <textarea id='" + String.Format("DadosSalario{0}MotivoMudanca", (i + 1)) + "' name='DadosSalarioMotivoMudanca[]' class='form-control' rows='1' placeholder='Descreva-se aqui ...' maxlength='50'> " + (salario.Count >= (i + 1) ? salario[i].MotivoMudanca : String.Empty) + "</textarea>";
                    salarios += "               </div>";
                    salarios += "            </div>";
                    salarios += "         </div>";
                    salarios += "         <div class='row'>";
                    salarios += "            <div class='col-md-4'>";
                    salarios += "               <button type='button' id='" + String.Format("ButtonDadosSalario{0}Incluir", (i + 1)) + "' name='" + String.Format("ButtonDadosSalario{0}Incluir", (i + 1)) + "' class='btn btn-primary' data-loading-text='Salvando...' onclick='ButtonDadosSalarioSalvar_Click(" + (i + 1) + ");'>";
                    salarios += "                  Salvar";
                    salarios += "               </button>";
                    salarios += "            </div>";
                    salarios += "            <div class='col-md-3' style='text-align: center;'>";
                    salarios += "            </div>";
                    salarios += "            <div class='col-md-5' style='text-align: right;'>";
                    salarios += "               <button type='button' id='" + String.Format("ButtonDadosSalario{0}Excluir", (i + 1)) + "' name='" + String.Format("ButtonDadosSalario{0}Excluir", (i + 1)) + "' class='btn btn-danger' data-loading-text='Apagando...' onclick='ButtonDadosSalarioExcluir_Click(" + (i + 1) + ");' " + (salario.Count >= (i + 1) ? String.Empty : "disabled='disabled'") + ">";
                    salarios += "                  Apagar";
                    salarios += "               </button>";
                    salarios += "               <div id='" + String.Format("ButtonDadosSalario{0}ExcluirConfirmacao", (i + 1)) + "' style='display:none'>";
                    salarios += "                  <label id='" + String.Format("LabelDadosSalario{0}ExcluirConfirmacao", (i + 1)) + "' style='font-weight: normal !important; font-size: 12px !important;'>Você deseja realmente excluir esse Salário?</label><br />";
                    salarios += "                  <button id='" + String.Format("accept{0}", (i + 1)) + "' class='btn btn-default btn-xs' value='true'>Sim</button>";
                    salarios += "                  <button id='" + String.Format("go_back{0}", (i + 1)) + "' class='btn btn-primary btn-xs' value='false'>Não</button>";
                    salarios += "               </div>";
                    salarios += "            </div>";
                    salarios += "         </div>";
                    salarios += "      </div>";
                    salarios += "   </div>";
                    salarios += "</div>";
                    salarios += "<script>";
                    salarios += "   $('#DadosSalario" + (i + 1) + "DataInicio').datepicker({";
                    salarios += "      language: 'pt-BR',";
                    salarios += "      autoclose: true,";
                    salarios += "      todayHighlight: true,";
                    salarios += "      orientation: 'bottom',";
                    salarios += "      clearBtn: true";
                    salarios += "   });";
                    salarios += "   $('#DadosSalario" + (i + 1) + "DataFim').datepicker({";
                    salarios += "      language: 'pt-BR',";
                    salarios += "      autoclose: true,";
                    salarios += "      todayHighlight: true,";
                    salarios += "      orientation: 'bottom',";
                    salarios += "      clearBtn: true";
                    salarios += "   });";
                    salarios += "   $('#Salario" + (i + 1) + "Salario').maskMoney({ thousands: '.', decimal: ',', symbolStay: true });";
                    salarios += "</script> ";
                }

                var result = new { response = "success", salarios = salarios, count = SalariosQuantidade };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        [HttpPost]
        public JsonResult SalvarDadosSalarios(Int32 Id, Int32 IdCargo, String Salario, String DataInicio, String DataFim, String MotivoMudanca)
        {
            try
            {
                RHCargosSalarios salario = new RHCargosSalarios();
                salario.Id = Id;
                salario.IdCargoNivel = IdCargo;

                if (Id == 0)
                    Id = RHCargosSalariosBo.Inserir(salario);

                if (Salario != null && Salario.Length > 0)
                {
                    salario.Salario = Decimal.Parse(Salario);
                }
                else
                {
                    salario.Salario = 0;
                }

                if (DataInicio != null && DataInicio.Length > 0)
                {
                    salario.DataInicio = DateTime.Parse(DataInicio);
                }

                if (DataFim != null && DataFim.Length > 0)
                {
                    salario.DataFim = DateTime.Parse(DataFim);
                }

                salario.MotivoMudanca = MotivoMudanca;

                salario.DataCadastro = DateTime.Now;
                salario.UsuarioCadastro = Sessao.Usuario.Nome;
                salario.DataAlteracao = DateTime.Now;
                salario.UsuarioAlteracao = Sessao.Usuario.Nome;

                if (Id == 0)
                    Id = RHCargosSalariosBo.Inserir(salario);

                salario.Id = Id;
                RHCargosSalariosBo.Salvar(salario);

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
        public JsonResult ExcluirDadosSalarios(Int32 Id)
        {
            try
            {
                RHCargosSalariosBo.Excluir(Id);

                var result = new { response = "removed" };
                return Json(result);
            }
            catch (Exception ex)
            {
                var result = new { response = "error" };
                return Json(result);
            }
        }

        public ActionResult Reajustar(String TipoContrato = "", Int32 Cargo = 0, String PercentualReajuste = "", String Salario = "")
        {
            if (Sessao.Usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                if (Cargo == 0 && PercentualReajuste == "" && Salario == "")
                {
                    return View();

                }
                else
                {
                    using (var context = new UranusEntities())
                    {
                        decimal percentualReajuste = (PercentualReajuste != "" ? decimal.Parse(PercentualReajuste) : 0);
                        decimal salario = (Salario != "" ? decimal.Parse(Salario) : 0);


                        SqlParameter param1 = new SqlParameter("@TipoContrato", TipoContrato);
                        SqlParameter param2 = new SqlParameter("@Cargo", Cargo);
                        SqlParameter param3 = new SqlParameter("@PercentualReajuste", percentualReajuste);
                        SqlParameter param4 = new SqlParameter("@Salario", salario);
                        SqlParameter param5 = new SqlParameter("@Funcionario", Sessao.Usuario.Nome);

                        context.Database.ExecuteSqlCommand("stpRHCargosSalariosReajuste @TipoContrato, @IdCargo, @PercentualReajuste, @Salario, @Funcionario", param1, param2, param3, param4, param5);

                    }

                    return View();
                }
            }
        }

        //[HttpPost]
        //public JsonResult ReajustarSalarios(String TipoContrato, Int32 Cargo, String PercentualReajuste, String Salario)
        //{
        //    var codigo = "00";
        //    try
        //    {

        //        using (var context = new UranusEntities())
        //        {
        //            SqlParameter param1 = new SqlParameter("@TipoContrato", Cargo);
        //            SqlParameter param2 = new SqlParameter("@Cargo", Cargo);
        //            SqlParameter param3 = new SqlParameter("@PercentualReajuste", "PercentualReajuste");
        //            SqlParameter param4 = new SqlParameter("@Salario", Salario);
        //            SqlParameter param5 = new SqlParameter("@Funcionario", Sessao.Usuario.Nome);

        //            context.Database.ExecuteSqlCommand("stpRHCargosSalariosReajuste @TipoContrato, @Cargo, @PercentualReajuste, @Salario, @Funcionario", param1, param2, param3, param4, param5);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        codigo = "99";
        //        throw;
        //    }

        //    var result = new { codigo = codigo };
        //    return Json(result);



        //}
    }
}