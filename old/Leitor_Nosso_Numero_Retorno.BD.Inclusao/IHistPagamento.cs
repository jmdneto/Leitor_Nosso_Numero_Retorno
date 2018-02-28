using ConnPostNpgsql;
using Npgsql;
using Retorno_Debito_Automatico.BD.Conexao;
using Retorno_Debito_Automatico.Layouts;
using System;
namespace Retorno_Debito_Automatico.BD.Inclusao
{
	internal class IHistPagamento : openPost
	{
		public Pagamento pagamento = new Pagamento();
        public bool gravaHistPagamento(ref ConnPostNpgsql.Conexao conexao)
		{
			this.erros.classe = "IHistPagamento";
			this.erros.funcao = "gravaHistPagamento";
			this.erros.descErro = "";
			NpgsqlCommand npgsqlCommand = new NpgsqlCommand();
			bool result;
			try
			{
				npgsqlCommand.Connection = conexao.ConexaoBd;
				if (conexao.transactionIsOpen())
				{
					npgsqlCommand.Transaction = conexao.TransacaoBd;
				}
				string commandText = "INSERT INTO tb_hist_pagamentos SELECT :data_gravacao_historico,cpf_comprador,id_venda,id_pagamento,id_imobiliaria,id_pagamento_imobiliaria,valor,data_agendamento,tp_pagamento,deb_cod_banco,deb_numero_agencia,deb_dig_verificador_agencia,deb_numero_conta,deb_dig_verificador_conta,chc_cod_banco,chc_numero_agencia,chc_dig_verifcador_agencia,chc_numero_conta,chc_dig_verificador_conta,chc_numero_cheque,chc_cmc7,bol_numero_boleto,id_status,data_alteracao,id_usuario,'',data_pagamento,motivo_devolucao,total_envios_banco,vlr_tot_tarifa_deb_aut,total_corretores,data_agendamento_remessa FROM tb_pagamentos WHERE id_imobiliaria  = :id_imobiliaria AND id_venda  = :id_venda AND id_pagamento  = :id_pagamento";
				npgsqlCommand.CommandText = commandText;
				openPost.setParameters(npgsqlCommand, ":id_imobiliaria", this.pagamento.codImobiliaria);
				openPost.setParameters(npgsqlCommand, ":id_venda", this.pagamento.id_venda);
				openPost.setParameters(npgsqlCommand, ":id_pagamento", this.pagamento.id_pagamento);
				openPost.setParameters(npgsqlCommand, ":data_gravacao_historico", this.pagamento.data_alteracaoHist);
				npgsqlCommand.ExecuteNonQuery();
				result = true;
			}
			catch (Exception ex)
			{
				this.erros.rc = 4;
				this.erros.descErro = "EXCEPTION. MSG.:" + ex.Message;
				npgsqlCommand.Dispose();
				result = false;
			}
			finally
			{
				if (npgsqlCommand != null)
				{
					npgsqlCommand.Dispose();
					npgsqlCommand = null;
				}
			}
			return result;
		}
	}
}
