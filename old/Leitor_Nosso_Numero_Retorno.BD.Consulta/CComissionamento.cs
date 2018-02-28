using ConnPostNpgsql;
using Npgsql;
using Retorno_Debito_Automatico.BD.Conexao;
using Retorno_Debito_Automatico.Layouts;
using System;
namespace Retorno_Debito_Automatico.BD.Consulta
{
	internal class CComissionamento : openPost
	{
		public Pagamento pagamento = new Pagamento();
		private Comissionamento comissao = new Comissionamento();
        public bool buscaComissionamento(ref ConnPostNpgsql.Conexao conexao)
		{
			this.erros.classe = "CComissao";
			this.erros.funcao = "buscaComissionamento()";
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
				string commandText = "SELECT * from tb_comissionamentos  WHERE id_imobiliaria = :id_imobiliaria AND id_venda = :id_venda AND id_pagamento = :id_pagamento";
				npgsqlCommand.CommandText = commandText;
				openPost.setParameters(npgsqlCommand, ":id_imobiliaria", this.pagamento.codImobiliaria);
				openPost.setParameters(npgsqlCommand, ":id_venda", this.pagamento.id_venda);
				openPost.setParameters(npgsqlCommand, ":id_pagamento", this.pagamento.id_pagamento);
				npgsqlDataReader = npgsqlCommand.ExecuteReader();
				this.pagamento.comissionamento.Clear();
				while (npgsqlDataReader.Read())
				{
					int ordinal = npgsqlDataReader.GetOrdinal("cpf_autonomo");
					this.comissao.cpf_autonomo = npgsqlDataReader.GetValue(ordinal).ToString();
					ordinal = npgsqlDataReader.GetOrdinal("id_comissionamento");
					this.comissao.id_comissionamento = npgsqlDataReader.GetValue(ordinal).ToString();
					ordinal = npgsqlDataReader.GetOrdinal("id_status");
					this.comissao.id_status_ant = npgsqlDataReader.GetValue(ordinal).ToString();
                    ordinal = npgsqlDataReader.GetOrdinal("valor");
                    this.comissao.valor = Convert.ToDouble(npgsqlDataReader.GetValue(ordinal).ToString());
                    ordinal = npgsqlDataReader.GetOrdinal("valor_percentual_banco");
                    this.comissao.valor_percentual_banco = Convert.ToDouble(npgsqlDataReader.GetValue(ordinal).ToString());
                    //ordinal = npgsqlDataReader.GetOrdinal("valor_ted");
                    //this.comissao.valor_ted = Convert.ToDouble(npgsqlDataReader.GetValue(ordinal).ToString());
                    //ordinal = npgsqlDataReader.GetOrdinal("valor_deb_boleto_comprador");
                    //this.comissao.valor_deb_boleto_comprador = Convert.ToDouble(npgsqlDataReader.GetValue(ordinal).ToString());
					this.pagamento.comissionamento.Add(this.comissao);
					this.comissao = new Comissionamento();
					flag = true;
				}
				if (!flag)
				{
					this.erros.descErro = "NAO HA COMISSIONAMENTO PARA O AUTONOMO...";
					this.erros.rc = 9;
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
