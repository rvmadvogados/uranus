using System;
using System.Collections.Generic;

namespace Sigman.Domain.Entities
{
    public class DadosNFE
    {
        public virtual NotasFiscais notafiscal { get; set; }
        public virtual NaturezaOperacoes naturezaoperacao { get; set; }
        public virtual Empresas empresa { get; set; }
        public virtual Clientes cliente { get; set; }
        public virtual ClientesEnderecos clienteEndereco { get; set; }
        public string clienteTelefone { get; set; }
        public virtual string clientesemails { get; set; }
        public virtual Transportadoras transportadora { get; set; }
        public virtual List<NotasFiscaisParcelas> duplicatasnfe { get; set; }
        public virtual List<NotasFiscaisProdutos> produtos { get; set; }
        public Int32 TipoDocumento { get; set; }
        public Int32 FinalidadeEmissao { get; set; }
        public virtual Fornecedores fornecedor { get; set; }
        public string fornecedorTelefone { get; set; }
        public virtual FornecedoresEmail fornecedosemail { get; set; }
        public virtual List<NotasFiscaisReferenciadas> referenciadas { get; set; }
        public string CaminhoXml { get; set; }
        public string CaminhoDanfe { get; set; }
        public string URL { get; set; }
        public string TipoIntegracao { get; set; }
        public string DescricaoPagamento { get; set; }
        public string ObservacaoConsumidor { get; set; }
        public string ObservacaoIcms { get; set; }
        public string ObservacaoFisco { get; set; }
    }

    public class DadosNFS
    {
        public virtual NotasServicos notaservico { get; set; }
        public virtual Empresas empresa { get; set; }
        public virtual Clientes cliente { get; set; }
        public virtual ClientesEnderecos clienteEndereco { get; set; }
        public string clienteTelefone { get; set; }
        public virtual string clientesemails { get; set; }
        public virtual List<NotasServicosParcelas> duplicatasnfs { get; set; }
        public string CaminhoXml { get; set; }
        public string CaminhoDanfe { get; set; }
        public string URL { get; set; }
    }
}
