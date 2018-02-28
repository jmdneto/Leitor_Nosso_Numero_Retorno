using ASL_CodigosErros;
using ConnPostNpgsql;
using Npgsql;
using Retorno_Debito_Automatico.BD.Conexao;
using Retorno_Debito_Automatico.Layouts;
using System;
namespace Retorno_Debito_Automatico.BD.Consulta
{
	internal class CPagamentos : openPost
	{
		private CodigosErros codErros = new CodigosErros();
		public Pagamento pagamento = new Pagamento();
		public bool flgProcessar = false;
        public bool busca_pagamentos_deb_automatico(ref ConnPostNpgsql.Conexao conexao)
		{
			this.erros.classe = "CPagamento";
			this.erros.funcao = "busca_pagamentos_deb_automatico()";
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
				string commandText = "SELECT cpf_comprador, id_venda, id_pagamento, id_pagamento_imobiliaria, id_imobiliaria, deb_cod_banco ,deb_numero_agencia,deb_numero_conta, deb_dig_verificador_conta, valor, data_agendamento, id_status, antecipacao FROM tb_pagamentos  WHERE id_imobiliaria = :id_imobiliaria AND id_venda = :id_venda AND id_pagamento = :id_pagamento";
				npgsqlCommand.CommandText = commandText;
				openPost.setParameters(npgsqlCommand, ":id_imobiliaria", this.pagamento.codImobiliaria);
				openPost.setParameters(npgsqlCommand, ":id_venda", this.pagamento.id_venda);
				openPost.setParameters(npgsqlCommand, ":id_pagamento", this.pagamento.id_pagamento);
				npgsqlDataReader = npgsqlCommand.ExecuteReader();
				if (npgsqlDataReader.Read())
				{
					int ordinal = npgsqlDataReader.GetOrdinal("id_pagamento_imobiliaria");
					this.pagamento.id_pagamento_imobiliaria = npgsqlDataReader.GetValue(ordinal).ToString();
					ordinal = npgsqlDataReader.GetOrdinal("id_status");
					this.pagamento.status_pagamento = npgsqlDataReader.GetValue(ordinal).ToString();
					ordinal = npgsqlDataReader.GetOrdinal("cpf_comprador");
					this.pagamento.cpf_comprador = npgsqlDataReader.GetValue(ordinal).ToString();
					ordinal = npgsqlDataReader.GetOrdinal("deb_cod_banco");
					this.pagamento.cod_banco = npgsqlDataReader.GetValue(ordinal).ToString();
                    ordinal = npgsqlDataReader.GetOrdinal("antecipacao");
                    bool _antecipacao;
                    bool.TryParse(npgsqlDataReader.GetValue(ordinal).ToString(), out _antecipacao);
                    this.pagamento.antecipacao = _antecipacao;
					npgsqlDataReader.Dispose();
					result = true;
				}
				else
				{
					this.erros.codErro = "9999";
					this.erros.descErro = "NAO HA PAGAMENTOS PARA DEBITO AUTOMATICO...";
					this.erros.rc = 25;
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
