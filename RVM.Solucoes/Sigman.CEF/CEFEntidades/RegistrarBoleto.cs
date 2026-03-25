using System.Collections.Generic;

namespace Sigman.CEF.CEFEntidades
{
    using System;
    using System.IO;
    using System.Xml.Serialization;

    //XmlSerializer serializer = new XmlSerializer(typeof(Envelope));
    //using (StringReader reader = new StringReader(xml))
    //{
    //   var test = (Envelope)serializer.Deserialize(reader);


    #region Request
    [XmlRoot(ElementName = "HEADER")]
    public class HEADER
    {

        [XmlElement(ElementName = "VERSAO")]
        public object VERSAO { get; set; }

        [XmlElement(ElementName = "AUTENTICACAO")]
        public object AUTENTICACAO { get; set; }

        [XmlElement(ElementName = "USUARIO_SERVICO")]
        public object USUARIOSERVICO { get; set; }

        [XmlElement(ElementName = "USUARIO")]
        public object USUARIO { get; set; }

        [XmlElement(ElementName = "OPERACAO")]
        public object OPERACAO { get; set; }

        [XmlElement(ElementName = "INDICE")]
        public object INDICE { get; set; }

        [XmlElement(ElementName = "SISTEMA_ORIGEM")]
        public object SISTEMAORIGEM { get; set; }

        [XmlElement(ElementName = "UNIDADE")]
        public object UNIDADE { get; set; }

        [XmlElement(ElementName = "IDENTIFICADOR_ORIGEM")]
        public object IDENTIFICADORORIGEM { get; set; }

        [XmlElement(ElementName = "DATA_HORA")]
        public object DATAHORA { get; set; }

        [XmlElement(ElementName = "ID_PROCESSO")]
        public object IDPROCESSO { get; set; }
    }

    [XmlRoot(ElementName = "JUROS_MORA")]
    public class JUROSMORA
    {

        [XmlElement(ElementName = "TIPO")]
        public object TIPO { get; set; }

        [XmlElement(ElementName = "DATA")]
        public object DATA { get; set; }

        [XmlElement(ElementName = "VALOR")]
        public object VALOR { get; set; }

        [XmlElement(ElementName = "PERCENTUAL")]
        public object PERCENTUAL { get; set; }
    }

    [XmlRoot(ElementName = "POS_VENCIMENTO")]
    public class POSVENCIMENTO
    {

        [XmlElement(ElementName = "ACAO")]
        public object ACAO { get; set; }

        [XmlElement(ElementName = "NUMERO_DIAS")]
        public object NUMERODIAS { get; set; }
    }

    [XmlRoot(ElementName = "ENDERECO")]
    public class ENDERECO
    {

        [XmlElement(ElementName = "LOGRADOURO")]
        public object LOGRADOURO { get; set; }

        [XmlElement(ElementName = "BAIRRO")]
        public object BAIRRO { get; set; }

        [XmlElement(ElementName = "CIDADE")]
        public object CIDADE { get; set; }

        [XmlElement(ElementName = "UF")]
        public object UF { get; set; }

        [XmlElement(ElementName = "CEP")]
        public object CEP { get; set; }
    }

    [XmlRoot(ElementName = "PAGADOR")]
    public class PAGADOR
    {

        [XmlElement(ElementName = "CPF")]
        public object CPF { get; set; }

        [XmlElement(ElementName = "NOME")]
        public object NOME { get; set; }

        [XmlElement(ElementName = "CNPJ")]
        public object CNPJ { get; set; }

        [XmlElement(ElementName = "RAZAO_SOCIAL")]
        public object RAZAOSOCIAL { get; set; }

        [XmlElement(ElementName = "ENDERECO")]
        public ENDERECO ENDERECO { get; set; }
    }

    [XmlRoot(ElementName = "SACADOR_AVALISTA")]
    public class SACADORAVALISTA
    {

        [XmlElement(ElementName = "CPF")]
        public object CPF { get; set; }

        [XmlElement(ElementName = "NOME")]
        public object NOME { get; set; }

        [XmlElement(ElementName = "CNPJ")]
        public object CNPJ { get; set; }

        [XmlElement(ElementName = "RAZAO_SOCIAL")]
        public object RAZAOSOCIAL { get; set; }
    }

    [XmlRoot(ElementName = "MULTA")]
    public class MULTA
    {

        [XmlElement(ElementName = "DATA")]
        public object DATA { get; set; }

        [XmlElement(ElementName = "VALOR")]
        public object VALOR { get; set; }

        [XmlElement(ElementName = "PERCENTUAL")]
        public object PERCENTUAL { get; set; }
    }

    [XmlRoot(ElementName = "DESCONTO")]
    public class DESCONTO
    {

        [XmlElement(ElementName = "DATA")]
        public object DATA { get; set; }

        [XmlElement(ElementName = "VALOR")]
        public object VALOR { get; set; }

        [XmlElement(ElementName = "PERCENTUAL")]
        public object PERCENTUAL { get; set; }

        [XmlElement(ElementName = "TIPO")]
        public object TIPO { get; set; }
    }

    [XmlRoot(ElementName = "DESCONTOS")]
    public class DESCONTOS
    {

        [XmlElement(ElementName = "DESCONTO")]
        public DESCONTO DESCONTO { get; set; }
    }

    [XmlRoot(ElementName = "MENSAGENS")]
    public class MENSAGENS
    {

        [XmlElement(ElementName = "MENSAGEM")]
        public object MENSAGEM { get; set; }
    }

    [XmlRoot(ElementName = "FICHA_COMPENSACAO")]
    public class FICHACOMPENSACAO
    {

        [XmlElement(ElementName = "MENSAGENS")]
        public MENSAGENS MENSAGENS { get; set; }
    }

    [XmlRoot(ElementName = "RECIBO_PAGADOR")]
    public class RECIBOPAGADOR
    {

        [XmlElement(ElementName = "MENSAGENS")]
        public MENSAGENS MENSAGENS { get; set; }
    }

    [XmlRoot(ElementName = "PAGAMENTO")]
    public class PAGAMENTO
    {

        [XmlElement(ElementName = "QUANTIDADE_PERMITIDA")]
        public object QUANTIDADEPERMITIDA { get; set; }

        [XmlElement(ElementName = "TIPO")]
        public object TIPO { get; set; }

        [XmlElement(ElementName = "VALOR_MINIMO")]
        public object VALORMINIMO { get; set; }

        [XmlElement(ElementName = "VALOR_MAXIMO")]
        public object VALORMAXIMO { get; set; }

        [XmlElement(ElementName = "PERCENTUAL_MINIMO")]
        public object PERCENTUALMINIMO { get; set; }

        [XmlElement(ElementName = "PERCENTUAL_MAXIMO")]
        public object PERCENTUALMAXIMO { get; set; }
    }

    [XmlRoot(ElementName = "TITULO")]
    public class TITULO
    {

        [XmlElement(ElementName = "NOSSO_NUMERO")]
        public object NOSSONUMERO { get; set; }

        [XmlElement(ElementName = "TIPO")]
        public object TIPO { get; set; }

        [XmlElement(ElementName = "NUMERO_DOCUMENTO")]
        public object NUMERODOCUMENTO { get; set; }

        [XmlElement(ElementName = "DATA_VENCIMENTO")]
        public object DATAVENCIMENTO { get; set; }

        [XmlElement(ElementName = "VALOR")]
        public object VALOR { get; set; }

        [XmlElement(ElementName = "TIPO_ESPECIE")]
        public object TIPOESPECIE { get; set; }

        [XmlElement(ElementName = "FLAG_ACEITE")]
        public object FLAGACEITE { get; set; }

        [XmlElement(ElementName = "DATA_EMISSAO")]
        public object DATAEMISSAO { get; set; }

        [XmlElement(ElementName = "JUROS_MORA")]
        public JUROSMORA JUROSMORA { get; set; }

        [XmlElement(ElementName = "VALOR_ABATIMENTO")]
        public object VALORABATIMENTO { get; set; }

        [XmlElement(ElementName = "POS_VENCIMENTO")]
        public POSVENCIMENTO POSVENCIMENTO { get; set; }

        [XmlElement(ElementName = "CODIGO_MOEDA")]
        public object CODIGOMOEDA { get; set; }

        [XmlElement(ElementName = "PAGADOR")]
        public PAGADOR PAGADOR { get; set; }

        [XmlElement(ElementName = "SACADOR_AVALISTA")]
        public SACADORAVALISTA SACADORAVALISTA { get; set; }

        [XmlElement(ElementName = "MULTA")]
        public MULTA MULTA { get; set; }

        [XmlElement(ElementName = "DESCONTOS")]
        public DESCONTOS DESCONTOS { get; set; }

        [XmlElement(ElementName = "VALOR_IOF")]
        public object VALORIOF { get; set; }

        [XmlElement(ElementName = "IDENTIFICACAO_EMPRESA")]
        public object IDENTIFICACAOEMPRESA { get; set; }

        [XmlElement(ElementName = "FICHA_COMPENSACAO")]
        public FICHACOMPENSACAO FICHACOMPENSACAO { get; set; }

        [XmlElement(ElementName = "RECIBO_PAGADOR")]
        public RECIBOPAGADOR RECIBOPAGADOR { get; set; }

        [XmlElement(ElementName = "PAGAMENTO")]
        public PAGAMENTO PAGAMENTO { get; set; }

        [XmlElement(ElementName = "CARTEIRA")]
        public object CARTEIRA { get; set; }
    }

    [XmlRoot(ElementName = "INCLUI_BOLETO")]
    public class INCLUIBOLETO
    {

        [XmlElement(ElementName = "CODIGO_BENEFICIARIO")]
        public object CODIGOBENEFICIARIO { get; set; }

        [XmlElement(ElementName = "TITULO")]
        public TITULO TITULO { get; set; }
    }

    [XmlRoot(ElementName = "DADOS")]
    public class DADOS
    {

        [XmlElement(ElementName = "INCLUI_BOLETO")]
        public INCLUIBOLETO INCLUIBOLETO { get; set; }
    }

    [XmlRoot(ElementName = "Envelope")]
    public class Envelope
    {

        [XmlElement(ElementName = "HEADER")]
        public HEADER HEADER { get; set; }

        [XmlElement(ElementName = "DADOS")]
        public DADOS DADOS { get; set; }

        [XmlAttribute(AttributeName = "soapenv")]
        public string Soapenv { get; set; }

        [XmlAttribute(AttributeName = "ext")]
        public string Ext { get; set; }

        [XmlAttribute(AttributeName = "sib")]
        public string Sib { get; set; }
    }

    [XmlRoot(ElementName = "Root")]
    public class Root
    {

        [XmlElement(ElementName = "Envelope")]
        public Envelope Envelope { get; set; }

        [XmlAttribute(AttributeName = "sibar_base")]
        public string SibarBase { get; set; }
    }


    #endregion

    #region Response

    #endregion
}