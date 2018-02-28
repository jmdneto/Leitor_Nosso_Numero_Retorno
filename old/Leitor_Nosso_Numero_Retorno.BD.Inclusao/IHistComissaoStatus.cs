using ConnPostNpgsql;
using Npgsql;
using Retorno_Debito_Automatico.BD.Conexao;
using Retorno_Debito_Automatico.Layouts;
using System;
namespace Retorno_Debito_Automatico.BD.Inclusao
{
	internal class IHistComissaoStatus : openPost
	{
		public Pagamento pagamento = new Pagamento();
        public bool gravaHistComissionamento(ref ConnPostNpgsql.Conexao conexao, int i)
		{
			this.erros.classe = "IHistComissao";
			this.erros.funcao = "gravaHistComissionamento";
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
				string commandText = "INSERT INTO tb_hist_comissionamentos SELECT :data_gravacao_historico,cpf_autonomo,id_imobiliaria,cpf_comprador,id_venda,id_pagamento,id_comissionamento,valor,resgate_cod_banco,resgate_agencia,resgate_agencia_digito,resgate_numero_conta,resgate_numero_conta_digito,resgate_nome_titular_conta,resgate_cpf_cnpj_conta,resgate_tp_conta_corrente,resgate_tp_conta_pessoa,:id_status,id_usuario,:data_alteracao,:desc_hist,codigo_finalidade_ted,valor_percentual_banco,valor_ted,valor_deb_boleto_comprador,valor_liquido FROM tb_comissionamentos WHERE cpf_autonomo = :cpf_autonomo AND id_imobiliaria = :id_imobiliaria AND id_venda = :id_venda AND id_pagamento = :id_pagamento AND id_comissionamento = :id_comissionamento";
				npgsqlCommand.CommandText = commandText;
				openPost.setParameters(npgsqlCommand, ":cpf_autonomo", this.pagamento.comissionamento[i].cpf_autonomo);
				openPost.setParameters(npgsqlCommand, ":id_imobiliaria", this.pagamento.codImobiliaria);
				openPost.setParameters(npgsqlCommand, ":id_venda", this.pagamento.id_venda);
				openPost.setParameters(npgsqlCommand, ":id_pagamento", this.pagamento.id_pagamento);
				openPost.setParameters(npgsqlCommand, ":id_comissionamento", this.pagamento.comissionamento[i].id_comissionamento);
				openPost.setParameters(npgsqlCommand, ":data_gravacao_historico", this.pagamento.data_alteracaoHist);
				openPost.setParameters(npgsqlCommand, ":desc_hist", "RETORNO");
				openPost.setParameters(npgsqlCommand, ":id_status", this.pagamento.comissionamento[i].id_status);
				openPost.setParameters(npgsqlCommand, ":data_alteracao", this.pagamento.data_alteracaoHist);
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
