using ConnPostNpgsql;
using Npgsql;
using Retorno_Debito_Automatico.BD.Conexao;
using Retorno_Debito_Automatico.Layouts;
using System;
namespace Retorno_Debito_Automatico.BD.Alteracao
{
	internal class ACad_Deb_Automatico : openPost
	{
		public Pagamento pagamento = new Pagamento();
        public bool status_cad_deb_auto(ref ConnPostNpgsql.Conexao conexao, string status, string desc_hist)
		{
			this.erros.classe = "ACad_Deb_Automatico";
			this.erros.funcao = "status_cad_deb_auto()";
			this.erros.codErro = "";
			NpgsqlCommand npgsqlCommand = new NpgsqlCommand();
			bool result;
			try
			{
				npgsqlCommand.Connection = conexao.ConexaoBd;
				if (conexao.transactionIsOpen())
				{
					npgsqlCommand.Transaction = conexao.TransacaoBd;
				}
				string commandText = "Update tb_cadastro_cc_debito_automatico SET  id_status = :id_status,  desc_hist = :desc_hist, data_alteracao = :data_alteracao WHERE cpf_comprador = :cpf_comprador AND cod_banco = :cod_banco AND numero_agencia = :numero_agencia AND numero_conta = :numero_conta AND dig_verificador_conta = :dig_conta";
				npgsqlCommand.CommandText = commandText;
				openPost.setParameters(npgsqlCommand, ":cpf_comprador", this.pagamento.cpf_comprador);
				openPost.setParameters(npgsqlCommand, ":cod_banco", this.pagamento.cod_banco);
				openPost.setParameters(npgsqlCommand, ":numero_agencia", this.pagamento.agencia);
				openPost.setParameters(npgsqlCommand, ":numero_conta", this.pagamento.conta);
				openPost.setParameters(npgsqlCommand, ":dig_conta", this.pagamento.dig_conta);
				openPost.setParameters(npgsqlCommand, ":desc_hist", desc_hist);
				openPost.setParameters(npgsqlCommand, ":data_alteracao", this.pagamento.data_alteracaoHist);
				if (status == "P17")
				{
					openPost.setParameters(npgsqlCommand, ":id_status", "CD2");
				}
				else
				{
					openPost.setParameters(npgsqlCommand, ":id_status", "CD3");
				}
				npgsqlCommand.ExecuteNonQuery();
				result = true;
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
