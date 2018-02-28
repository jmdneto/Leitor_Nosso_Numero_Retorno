using ConnPostNpgsql;
using Npgsql;
using Retorno_Debito_Automatico.BD.Conexao;
using Retorno_Debito_Automatico.Layouts;
using System;
namespace Retorno_Debito_Automatico.BD.Consulta
{
	internal class CVenda : openPost
	{
		public Pagamento pagamento = new Pagamento();
        public bool buscaVenda(ref ConnPostNpgsql.Conexao conexao)
		{
			this.erros.classe = "CVenda";
			this.erros.funcao = "buscaVenda()";
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
				string commandText = "SELECT * from tb_vendas  WHERE id_imobiliaria = :id_imobiliaria AND id_venda = :id_venda";
				npgsqlCommand.CommandText = commandText;
				openPost.setParameters(npgsqlCommand, ":id_imobiliaria", this.pagamento.codImobiliaria);
				openPost.setParameters(npgsqlCommand, ":id_venda", this.pagamento.id_venda);
				npgsqlDataReader = npgsqlCommand.ExecuteReader();
				if (npgsqlDataReader.Read())
				{
					int ordinal = npgsqlDataReader.GetOrdinal("id_venda_imobiliaria");
					this.pagamento.id_venda_imobiliaria = npgsqlDataReader.GetValue(ordinal).ToString();
					npgsqlDataReader.Dispose();
					result = true;
				}
				else
				{
					this.erros.codErro = "9999";
					this.erros.descErro = "VENDA NAO CADASTRADA...";
					this.erros.rc = 8;
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
