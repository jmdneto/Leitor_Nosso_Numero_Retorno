using ASL_CodigosErros;
using ConnPostNpgsql;
using Npgsql;
using Retorno_Debito_Automatico.BD.Conexao;
using Retorno_Debito_Automatico.Layouts;
using System;
namespace Retorno_Debito_Automatico.BD.Consulta
{
	internal class CSolDebAutomatico : openPost
	{
		private CodigosErros codErros = new CodigosErros();
		public Pagamento pagamento = new Pagamento();
		public bool flgProcessar = false;
        public bool busca_sol_deb_automatico(ref ConnPostNpgsql.Conexao conexao)
		{
			this.erros.classe = "CSolDebAutomatico";
			this.erros.funcao = "busca_sol_deb_automatico()";
			this.erros.codErro = "";
			NpgsqlCommand npgsqlCommand = new NpgsqlCommand();
			NpgsqlDataReader npgsqlDataReader = null;
			bool result;
			try
			{
				npgsqlCommand.Connection = conexao.ConexaoBd;
				if (conexao.transactionIsOpen())
				{
					npgsqlCommand.Transaction = conexao.TransacaoBd;
				}
				string commandText = "SELECT *   from tb_solicitacao_deb_aut  WHERE seq_solicitacao_deb_aut = :seq_solicitacao_deb_aut";
				npgsqlCommand.CommandText = commandText;
				openPost.setParameters(npgsqlCommand, ":seq_solicitacao_deb_aut", this.pagamento.seq_solicitacao_deb_aut);
				npgsqlDataReader = npgsqlCommand.ExecuteReader();
				if (npgsqlDataReader.Read())
				{
					int ordinal = npgsqlDataReader.GetOrdinal("id_imobiliaria");
					this.pagamento.codImobiliaria = npgsqlDataReader.GetValue(ordinal).ToString();
					ordinal = npgsqlDataReader.GetOrdinal("id_venda");
					this.pagamento.id_venda = npgsqlDataReader.GetValue(ordinal).ToString();
					ordinal = npgsqlDataReader.GetOrdinal("id_pagamento");
					this.pagamento.id_pagamento = npgsqlDataReader.GetValue(ordinal).ToString();
					npgsqlDataReader.Dispose();
					result = true;
				}
				else
				{
					this.erros.codErro = "9999";
					this.erros.descErro = "NAO HA PAGAMENTOS PARA SOLICITACAO DEBITO AUTOMATICO...";
					this.erros.rc = 59;
					npgsqlCommand.Dispose();
					npgsqlDataReader.Dispose();
					result = false;
				}
			}
			catch (Exception ex)
			{
				this.erros.codErro = "9999";
				this.erros.descErro = "EXCEPTION:" + ex.Message;
				this.erros.rc = 4;
				npgsqlCommand.Dispose();
				npgsqlDataReader.Dispose();
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
