using System.Collections.Generic;

namespace Uranus.Domain.Entities
{
    public class Performance
    {
        public string NomeServidor { get; set; }

        public string UsuariosConectados { get; set; }
        public string UsuariosAtivos { get; set; }

        public string ProcessadorVelocidade { get; set; }
        public string ProcessadorUtilizacao { get; set; }

        public string MemoriaDisponivel { get; set; }
        public string MemoriaLivre { get; set; }
        public string MemoriaTotal { get; set; }
        public string MemoriaOcupada { get; set; }

        public string EspacoDisponivel { get; set; }
        public string EspacoLivre { get; set; }
        public string EspacoTotal { get; set; }
        public string EspacoOcupada { get; set; }

        public string AgendasDisponiveis { get; set; }
        public string AgendasConfirmadas { get; set; }

        public List<UsuariosLogados> UsuariosLogados { get; set; }

        public List<SistemasOperacionais> SistemasOperacionais { get; set; }

        public List<Navegadores> Navegadores { get; set; }

        public List<DashProfissionaisSolicitacoes> DashProfissionaisSolicitacoes { get; set; }

        public List<DashProfissionaisSolicitacoesEspeciais> DashProfissionaisSolicitacoesEspeciais { get; set; }

        public List<DashProfissionaisAusencias> DashProfissionaisAusencias { get; set; }

        public List<DashProfissionaisDocumentos> DashProfissionaisDocumentos { get; set; }
    }

    public class UsuariosLogados
    {
        public string Usuario { get; set; }
        public int Total { get; set; }
    }
    
    public class SistemasOperacionais
    {
        public string Nome { get; set; }
        public int Total { get; set; }
        public int Percentual { get; set; }
    }

    public class Navegadores
    {
        public string Nome { get; set; }
        public int Total { get; set; }
        public int Percentual { get; set; }
    }

    public class DashProfissionaisSolicitacoes
    {
        public string Nome { get; set; }
        public int? Saldo { get; set; }
        public int? Dias { get; set; }
        public string DataInicio { get; set; }
    }
    public class DashProfissionaisSolicitacoesEspeciais
    {
        public string Nome { get; set; }
        public string Data { get; set; }
        public string Solicitacao { get; set; }
    }

    public class DashProfissionaisAusencias
    {
        public string Nome { get; set; }
        public string DataInicio { get; set; }
        public string DataFim { get; set; }
    }

    public class DashProfissionaisDocumentos
    {
        public string Nome { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string DataCadastro { get; set; }
    }

}
