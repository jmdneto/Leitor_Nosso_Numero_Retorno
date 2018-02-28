using System;
namespace Retorno_Debito_Automatico.Layouts
{
	public class Comissionamento
	{
		public string cpf_autonomo;
		public string id_comissionamento;
		public Double valor;
        public Double valor_percentual_banco;
        public Double valor_ted;
        public Double valor_deb_boleto_comprador;
		public string id_status;
		public string id_status_ant;
		public DateTime data_alteracao;
		public DateTime data_alteracaoHist;
		public string id_usuario;
		public string desc_hist;

	}
}
