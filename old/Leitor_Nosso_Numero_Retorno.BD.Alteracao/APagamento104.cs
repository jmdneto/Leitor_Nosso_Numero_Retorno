using ConnPostNpgsql;
using Npgsql;
using Retorno_Debito_Automatico.BD.Conexao;
using Retorno_Debito_Automatico.Layouts;
using System;
namespace Retorno_Debito_Automatico.BD.Alteracao
{
	internal class APagamento104 : openPost
	{
		public Pagamento pagamento = new Pagamento();
        public bool statusPagamento(ref ConnPostNpgsql.Conexao conexao, string status, string desc_retorno, string motivo_devolucao)
		{
			this.erros.classe = "APagamento104";
			this.erros.funcao = "statusPagamento";
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
				string commandText = "UPDATE tb_pagamentos  SET  id_status = :id_status, motivo_devolucao = :motivo_devolucao, data_alteracao = :data_alteracao WHERE id_imobiliaria  = :id_imobiliaria AND id_venda  = :id_venda AND id_pagamento  = :id_pagamento";
				npgsqlCommand.CommandText = commandText;
				openPost.setParameters(npgsqlCommand, ":id_imobiliaria", this.pagamento.codImobiliaria);
				openPost.setParameters(npgsqlCommand, ":id_venda", this.pagamento.id_venda);
				openPost.setParameters(npgsqlCommand, ":id_pagamento", this.pagamento.id_pagamento);
				openPost.setParameters(npgsqlCommand, ":id_status", status);
				if (motivo_devolucao == "")
				{
					openPost.setParameters(npgsqlCommand, ":motivo_devolucao", null);
				}
				else
				{
					openPost.setParameters(npgsqlCommand, ":motivo_devolucao", motivo_devolucao);
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
