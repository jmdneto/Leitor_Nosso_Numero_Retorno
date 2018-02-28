using ConnPostNpgsql;
using Npgsql;
using Retorno_Debito_Automatico.BD.Conexao;
using Retorno_Debito_Automatico.Layouts;
using System;
namespace Retorno_Debito_Automatico.BD.Alteracao
{
	internal class ASequencia : openPost
	{
		public Pagamento pagamento = new Pagamento();
        public bool updateSequencia(ref ConnPostNpgsql.Conexao conexao)
		{
			this.erros.classe = "ASequencia";
			this.erros.funcao = "updateSequencia";
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
				string commandText = "UPDATE tb_conv_bco_empresa_deb_aut  SET  sequencial_arquivo = :sequencial_arquivo, data_alteracao = :data_alteracao WHERE  codigo_convenio = :codigo_convenio AND cod_banco  = :cod_banco AND id_empresa  = :id_empresa";
				npgsqlCommand.CommandText = commandText;
				openPost.setParameters(npgsqlCommand, ":codigo_convenio", this.pagamento.cod_convenio);
				openPost.setParameters(npgsqlCommand, ":cod_banco", this.pagamento.cod_banco);
				openPost.setParameters(npgsqlCommand, ":id_empresa", this.pagamento.codImobiliaria);
				openPost.setParameters(npgsqlCommand, ":sequencial_arquivo", this.pagamento.seq_banco);
				openPost.setParameters(npgsqlCommand, ":data_alteracao", DateTime.Now);
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
