using ConnPostNpgsql;
using Npgsql;
using Retorno_Debito_Automatico.BD.Conexao;
using Retorno_Debito_Automatico.Layouts;
using System;
namespace Retorno_Debito_Automatico.BD.Alteracao
{
	internal class AComissionamento : openPost
	{
		public Pagamento pagamento = new Pagamento();
        public bool updateComissao(ref ConnPostNpgsql.Conexao conexao, int i, string status)
		{
			this.erros.classe = "AComissionamento";
			this.erros.funcao = "liberaComissao";
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
                string commandText = "UPDATE tb_comissionamentos  SET  id_status = :id_status, desc_hist = :desc_hist, data_alteracao = :data_alteracao, valor_percentual_banco = :valor_percentual_banco WHERE cpf_autonomo = :cpf_autonomo AND id_imobiliaria = :id_imobiliaria AND id_venda  = :id_venda AND id_pagamento  = :id_pagamento AND id_comissionamento  = :id_comissionamento";
				npgsqlCommand.CommandText = commandText;
                openPost.setParameters(npgsqlCommand, ":cpf_autonomo", this.pagamento.comissionamento[i].cpf_autonomo);
				openPost.setParameters(npgsqlCommand, ":id_imobiliaria", this.pagamento.codImobiliaria);
				openPost.setParameters(npgsqlCommand, ":id_venda", this.pagamento.id_venda);
				openPost.setParameters(npgsqlCommand, ":id_pagamento", this.pagamento.id_pagamento);
				openPost.setParameters(npgsqlCommand, ":id_comissionamento", this.pagamento.comissionamento[i].id_comissionamento);
				openPost.setParameters(npgsqlCommand, ":desc_hist", string.IsNullOrEmpty(this.pagamento.comissionamento[i].desc_hist) ? "" : this.pagamento.comissionamento[i].desc_hist);
				openPost.setParameters(npgsqlCommand, ":id_status", status);
				openPost.setParameters(npgsqlCommand, ":data_alteracao", this.pagamento.data_alteracaoHist);
                openPost.setParameters(npgsqlCommand, ":valor_percentual_banco", this.pagamento.comissionamento[i].valor_percentual_banco);
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
