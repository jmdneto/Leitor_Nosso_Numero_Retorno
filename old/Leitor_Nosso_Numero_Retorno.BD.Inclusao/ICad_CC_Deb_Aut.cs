using ConnPostNpgsql;
using Npgsql;
using Retorno_Debito_Automatico.BD.Conexao;
using Retorno_Debito_Automatico.Layouts;
using System;
namespace Retorno_Debito_Automatico.BD.Inclusao
{
	internal class ICad_CC_Deb_Aut : openPost
	{
		public Pagamento pagamento = new Pagamento();
        public bool grava_hist_cad_cc_deb_aut(ref ConnPostNpgsql.Conexao conexao)
		{
			this.erros.classe = "ICad_CC_Deb_Aut";
			this.erros.funcao = "grava_hist_cad_cc_deb_aut";
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
				string commandText = "INSERT INTO tb_hist_cadastro_cc_debito_automatico SELECT :data_gravacao_historico,cpf_comprador,cod_banco,numero_agencia,numero_conta,dig_verificador_conta,id_status,data_alteracao,desc_hist  FROM tb_cadastro_cc_debito_automatico WHERE cpf_comprador = :cpf_comprador AND cod_banco = :cod_banco AND numero_agencia = :numero_agencia AND numero_conta = :numero_conta AND dig_verificador_conta = :dig_verificador_conta";
				npgsqlCommand.CommandText = commandText;
				openPost.setParameters(npgsqlCommand, ":cpf_comprador", this.pagamento.cpf_comprador);
				openPost.setParameters(npgsqlCommand, ":cod_banco", this.pagamento.cod_banco);
				openPost.setParameters(npgsqlCommand, ":numero_agencia", this.pagamento.agencia);
				openPost.setParameters(npgsqlCommand, ":numero_conta", this.pagamento.conta);
				openPost.setParameters(npgsqlCommand, ":dig_verificador_conta", this.pagamento.dig_conta);
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
