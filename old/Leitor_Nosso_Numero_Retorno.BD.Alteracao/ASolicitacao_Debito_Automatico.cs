using ConnPostNpgsql;
using Npgsql;
using Retorno_Debito_Automatico.BD.Conexao;
using Retorno_Debito_Automatico.Layouts;
using System;
namespace Retorno_Debito_Automatico.BD.Alteracao
{
	internal class ASolicitacao_Debito_Automatico : openPost
	{
		public Pagamento pagamento = new Pagamento();
        public bool atualiza_sol_deb_aut(ref ConnPostNpgsql.Conexao conexao, string cod_retorno, string desc_retorno)
		{
			this.erros.classe = "ASolicitacao_Debito_Automatico";
			this.erros.funcao = "atualiza_sol_deb_aut";
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
				string commandText = "Update tb_solicitacao_deb_aut SET  cod_retorno = :cod_retorno,  descricao_retorno = :descricao_retorno, dt_retorno = :dt_retorno WHERE id_pagamento = :id_pagamento AND cpf_comprador = :cpf_comprador AND id_venda = :id_venda AND id_imobiliaria = :id_imobiliaria AND seq_solicitacao_deb_aut = :seq_solicitacao_deb_aut";
				npgsqlCommand.CommandText = commandText;
				openPost.setParameters(npgsqlCommand, ":cod_retorno", cod_retorno);
				openPost.setParameters(npgsqlCommand, ":cpf_comprador", this.pagamento.cpf_comprador);
				openPost.setParameters(npgsqlCommand, ":id_pagamento", this.pagamento.id_pagamento);
				openPost.setParameters(npgsqlCommand, ":id_venda", this.pagamento.id_venda);
				openPost.setParameters(npgsqlCommand, ":id_imobiliaria", this.pagamento.codImobiliaria);
				openPost.setParameters(npgsqlCommand, ":seq_solicitacao_deb_aut", this.pagamento.seq_solicitacao_deb_aut);
				openPost.setParameters(npgsqlCommand, ":dt_retorno", this.pagamento.data_alteracaoHist);
				openPost.setParameters(npgsqlCommand, ":descricao_retorno", desc_retorno);
				int num = npgsqlCommand.ExecuteNonQuery();
				if (num == 0)
				{
					this.erros.descErro = string.Concat(new string[]
					{
						"TB SOLICITACAO DEBITO NAO ATUALIZADO. IMOBI:",
						this.pagamento.codImobiliaria,
						" - COMPRADOR:",
						this.pagamento.cpf_comprador,
						" - PV:",
						this.pagamento.id_venda,
						" - PGTO:",
						this.pagamento.id_pagamento,
						" - ID_SOLI:",
						this.pagamento.seq_solicitacao_deb_aut
					});
					this.erros.rc = 2;
					npgsqlCommand.Dispose();
					result = false;
				}
				else
				{
					result = true;
				}
			}
			catch (Exception ex)
			{
				this.erros.codErro = "9999";
				this.erros.descErro = "EXCEPTION:" + ex.Message;
				this.erros.rc = 4;
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
