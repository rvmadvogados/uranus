using R9.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uranus.Data.R9
{
    [Table("TemplatesIniciaisAposentadoria")]
    public class TemplateInicialAposentadoria : BaseModel
    {
        [ID] public int IDTemplate { get; set; }
        [Column] public DateTime DataCadastro { get; set; }
        [Column] public int IdProfissional { get; set; }
        [Column] public int IdCliente { get; set; }
        [Column] public string VaraNumero { get; set; }
        [Column] public string VaraCidade { get; set; }
        [Column] public string ProcessoDataProtocolo { get; set; }
        [Column] public string RequerimentoNúmero { get; set; }
        [Column] public string ProcessoAtividade { get; set; }
        [Column] public bool ProcessoReafirmacaoDER { get; set; }
        [Column] public bool ProcessoGratuidade { get; set; }
        [Column] public string ProcessoValorCausa { get; set; }
        [Column] public string ProcessoRodapeData { get; set; }
        [Column] public string ProcessoAdvogado { get; set; }
        [Column] public string ProcessoImagem { get; set; }
        [Column] public string ProcessoOAB { get; set; }
        [Column] public string ProcessoResumoPeríodo { get; set; }
        [Column] public string ProcessoResumoPeríodosDER { get; set; }
        [Column] public string ProcessoExcel { get; set; }

        [Column] public string ClienteNome { get; set; }
        [Column] public string ClienteNacionalidade { get; set; }
        [Column] public string ClienteEstadoCivil { get; set; }
        [Column] public string ClienteProfissao { get; set; }
        [Column] public string ClienteCPF { get; set; }
        [Column] public string ClienteEndereco { get; set; }
        [Column] public string ClienteBairro { get; set; }
        [Column] public string ClienteCidade { get; set; }
        [Column] public string ClienteEstado { get; set; }
        [Column] public string ClienteCEP { get; set; }
        [Column] public string ClienteEmail { get; set; }
    }

    [Table("TemplatesIniciaisAposentadoriaPeriodos")]
    public class TemplateInicialPeríodo : BaseModel
    {
        [ID] public int IDPeriodo { get; set; }
        [Column] public int IDTemplate { get; set; }
        [Column] public string Periodo { get; set; }
        [Column] public string Empresa { get; set; }
        [Column] public string Atividades { get; set; }
        [Column] public string EnquadramentoLegal { get; set; }
        [Column] public string Provas { get; set; }
        [Column] public string ProvasAProduzir { get; set; }
        [Column] public string OBS { get; set; }
    }
}
