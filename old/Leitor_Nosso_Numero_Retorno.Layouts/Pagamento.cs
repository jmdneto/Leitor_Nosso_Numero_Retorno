using System;
using System.Collections.Generic;
namespace Retorno_Debito_Automatico.Layouts
{
	public class Pagamento
	{
		public string cod_convenio;
		public int seq_banco;
		public string codImobiliaria;
		public string nome_imobiliaria;
		public string status_imobiliaria;
		public string id_venda;
		public string id_venda_imobiliaria;
		public string status_venda;
		public string id_pagamento;
		public string id_pagamento_imobiliaria;
		public string status_pagamento;
		public string cod_banco;
		public string nome_banco;
		public string agencia;
		public string conta;
		public string dig_conta;
		public string data_agendamento;
		public string valor_debito;
		public string cod_moeda;
		public string cpf_comprador;
		public string cod_movimento;
		public string nome_arquivo;
		public string dt_hr_processamento;
		public DateTime data_alteracaoHist;
		public DateTime data_pagamento;
		public string status_retorno_banco;
		public string seq_solicitacao_deb_aut;
		public bool cad_optante_itau;
        public bool antecipacao;
		public List<Comissionamento> comissionamento = new List<Comissionamento>();
	}
}
