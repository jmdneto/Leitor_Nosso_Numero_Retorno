using ConnPostNpgsql;
using Npgsql;
using Retorno_Debito_Automatico.BD.Conexao;
using Retorno_Debito_Automatico.Layouts;
using System;
namespace Retorno_Debito_Automatico.BD.Alteracao
{
	internal class APagamento : openPost
	{
		public Pagamento pagamento = new Pagamento();
        public bool statusPagamento(ref ConnPostNpgsql.Conexao conexao, string status, string motivo_devolucao)
		{
			this.erros.classe = "APagamento";
			this.erros.funcao = "cancelaPagamento";
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
				string commandText = "UPDATE tb_pagamentos  SET  id_status = :id_status, motivo_devolucao = :motivo_devolucao, status_envio_efetivacao = :status_envio_efetivacao, data_pagamento = :data_pagamento, data_alteracao = :data_alteracao WHERE id_imobiliaria  = :id_imobiliaria AND id_venda  = :id_venda AND id_pagamento  = :id_pagamento";
				npgsqlCommand.CommandText = commandText;
				openPost.setParameters(npgsqlCommand, ":id_imobiliaria", this.pagamento.codImobiliaria);
				openPost.setParameters(npgsqlCommand, ":id_venda", this.pagamento.id_venda);
				openPost.setParameters(npgsqlCommand, ":id_pagamento", this.pagamento.id_pagamento);
				openPost.setParameters(npgsqlCommand, ":motivo_devolucao", motivo_devolucao);
				if (this.pagamento.cod_movimento == "0" && status == "P07")
				{
					openPost.setParameters(npgsqlCommand, ":id_status", status);
					openPost.setParameters(npgsqlCommand, ":data_pagamento", this.pagamento.data_pagamento);
					openPost.setParameters(npgsqlCommand, ":status_envio_efetivacao", 1);
				}
				else
				{
					openPost.setParameters(npgsqlCommand, ":id_status", status);
					openPost.setParameters(npgsqlCommand, ":data_pagamento", null);
					openPost.setParameters(npgsqlCommand, ":status_envio_efetivacao", 0);
				}
				openPost.setParameters(npgsqlCommand, ":data_alteracao", this.pagamento.data_alteracaoHist);
				npgsqlCommand.ExecuteNonQuery();
				result = true;
			}
			catch (Exception ex)
			{
				this.erros.codErro = "9999";
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
