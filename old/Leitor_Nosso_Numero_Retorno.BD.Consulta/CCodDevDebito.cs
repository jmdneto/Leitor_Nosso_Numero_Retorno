using ConnPostNpgsql;
using Npgsql;
using Retorno_Debito_Automatico.BD.Conexao;
using Retorno_Debito_Automatico.Layouts;
using System;
using System.Collections.Generic;
namespace Retorno_Debito_Automatico.BD.Consulta
{
	internal class CCodDevDebito : openPost
	{
		public List<Cod_Devolucao_Debito> listCodDevDeb = new List<Cod_Devolucao_Debito>();
		private Cod_Devolucao_Debito codDevDeb = new Cod_Devolucao_Debito();
        public bool buscaCodDevDebito(ref ConnPostNpgsql.Conexao conexao)
		{
			this.erros.classe = "CCodDevDebito";
			this.erros.funcao = "buscaCodDevDebito()";
			this.erros.codErro = "";
			NpgsqlCommand npgsqlCommand = new NpgsqlCommand();
			NpgsqlDataReader npgsqlDataReader = null;
			bool flag = false;
			bool result;
			try
			{
				npgsqlCommand.Connection = conexao.ConexaoBd;
				if (conexao.transactionIsOpen())
				{
					npgsqlCommand.Transaction = conexao.TransacaoBd;
				}
				string commandText = "SELECT * from tb_debito_codigo_devolucao  Order By codigo";
				npgsqlCommand.CommandText = commandText;
				npgsqlDataReader = npgsqlCommand.ExecuteReader();
				while (npgsqlDataReader.Read())
				{
					int ordinal = npgsqlDataReader.GetOrdinal("codigo");
					this.codDevDeb.codDev = npgsqlDataReader.GetInt32(ordinal);
					ordinal = npgsqlDataReader.GetOrdinal("desc_devolucao");
					this.codDevDeb.descDev = npgsqlDataReader.GetValue(ordinal).ToString();
					this.listCodDevDeb.Add(this.codDevDeb);
					this.codDevDeb = new Cod_Devolucao_Debito();
					flag = true;
				}
				if (!flag)
				{
					this.erros.descErro = "NAO HA CODIGOS DEVOLUCAO DEBITO...";
					this.erros.rc = 5;
					npgsqlCommand.Dispose();
					npgsqlDataReader.Dispose();
					result = false;
				}
				else
				{
					npgsqlDataReader.Dispose();
					result = true;
				}
			}
			catch (Exception ex)
			{
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
