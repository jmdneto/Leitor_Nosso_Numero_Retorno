using ASL_CodigosErros;
using Retorno_Debito_Automatico.BD.Conexao;
using Retorno_Debito_Automatico.Layouts;
using Retorno_Debito_Automatico.Validacoes;
using System;
namespace Retorno_Debito_Automatico.Processos
{
	internal class Montar_Retorno_Banco : openPost
	{
		private CodigosErros codErros = new CodigosErros();
		public PrmRetorno prmRetorno = new PrmRetorno();
		private Valida valida = new Valida();
		public string arquivoRetorno;

        private bool montaHeader()
		{
			this.erros.classe = "Montar_Retorno_Banco";
			this.erros.classe = "montaHeader";
			this.erros.classe = "";
			this.erros.rc = 0;
			bool result;
			try
			{
				this.prmRetorno.header.A01 = this.arquivoRetorno.Substring(0, 1);
				this.prmRetorno.header.A02 = this.arquivoRetorno.Substring(1, 1);
				this.prmRetorno.header.A03 = this.arquivoRetorno.Substring(2, 20);
				this.prmRetorno.header.A04 = this.arquivoRetorno.Substring(22, 20);
				this.prmRetorno.header.A05 = this.arquivoRetorno.Substring(42, 3);
				this.prmRetorno.header.A06 = this.arquivoRetorno.Substring(45, 20);
				this.prmRetorno.header.A07 = this.arquivoRetorno.Substring(65, 8);
				this.prmRetorno.header.A08 = this.arquivoRetorno.Substring(73, 6);
				this.prmRetorno.header.A09 = this.arquivoRetorno.Substring(79, 2);
				this.prmRetorno.header.A10 = this.arquivoRetorno.Substring(81, 17);
				this.prmRetorno.header.A11 = this.arquivoRetorno.Substring(98, 52);

                


            }
			catch (Exception ex)
			{
				this.erros.descErro = ex.Message;
				this.erros.rc = 9999;
				result = false;
				return result;
			}
			this.valida.prmRetorno = this.prmRetorno;
			if (!this.valida.header())
			{
				this.erros = this.valida.erros;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}
		private bool montaDetalhe()
		{
			this.erros.classe = "Montar_Retorno_Banco";
			this.erros.classe = "montaDetalhe";
			this.erros.classe = "";
			this.erros.rc = 0;
			bool result;
			try
			{
                
                // VER DEBITO EM CONTA-Fluxo de arquivos Remessa e Retorno-TROCA DE ARQUIVO COM BANCOS.pdf pagina 7 e 8

                this.prmRetorno.detalhe.F01 = this.arquivoRetorno.Substring(0, 1); // F01-Codigo do Registro - Conteúdo Fixo "F"
                this.prmRetorno.detalhe.F02 = this.arquivoRetorno.Substring(1, 25); // F02-Identificação do Cliente na Empresa - O conteúdo deverá ser idêntico ao anteriormente enviado pelo Banco, no registro tipo “E”
                this.prmRetorno.detalhe.F03 = this.arquivoRetorno.Substring(26, 4); // F03-Agência para Débito - O conteúdo deverá ser idêntico ao anteriormente enviado pelo Banco, no registro tipo “E”
                this.prmRetorno.detalhe.F04 = this.arquivoRetorno.Substring(30, 14); // F04-Identificação do Cliente no Banco - O conteúdo deverá ser idêntico ao anteriormente enviado pelo Banco, no registro tipo “E”
                this.prmRetorno.detalhe.F05 = this.arquivoRetorno.Substring(44, 8); // F05-Data do Vencimento ou Débito - Data do Vencimento, se o Código de Retorno, for diferente de “00”, Data real do Débito, se o Código de retorno for igual a “00” (debitado) - Formato AAAAMMDD
                this.prmRetorno.detalhe.F06 = this.arquivoRetorno.Substring(52, 15); // F06-Valor Original ou Debitado - Valor Original enviado, se o Código de Retorno, for diferente de 00 e Valor efetivamente Debitado, se o Código de Retorno for igual a 00
                this.prmRetorno.detalhe.F07 = this.arquivoRetorno.Substring(67, 2); // F07-Código de Retorno - Ver tb_debito_codigo_devolucao
                this.prmRetorno.detalhe.F08 = this.arquivoRetorno.Substring(69, 60); // F08-Uso da Empresa - O conteúdo deverá ser idêntico ao anteriormente enviado pelo Banco, no registro tipo “E”
                this.prmRetorno.detalhe.F09 = this.arquivoRetorno.Substring(129, 1); // F09-Tipo de Identificação - 1 CNPJ, 2 CPF
                this.prmRetorno.detalhe.F10 = this.arquivoRetorno.Substring(130, 15); // F10-Identificação - CNPJ: 999999999 = Número, 9999 = Filial, e 99 = DV CPF: 0000999999999
                this.prmRetorno.detalhe.F11 = this.arquivoRetorno.Substring(145, 4); // F11-Reservado para o futuro - Brancos
                this.prmRetorno.detalhe.F12 = this.arquivoRetorno.Substring(149, 1); // F12-Código do Movimento - O conteúdo deverá ser idêntico ao anteriormente enviado pela Empresa, no registro tipo “E”

            



                if (this.prmRetorno.header.A05 == "341")
				{
					if (Convert.ToDouble(this.arquivoRetorno.Substring(44, 25)) == 0.0)
					{
						this.prmRetorno.detalhe.cad_optante_itau = true;
					}
					else
					{
						this.prmRetorno.detalhe.cad_optante_itau = false;
					}
				}
				if (!this.valida.detalhe())
				{
					this.erros = this.valida.erros;
					result = false;
					return result;
				}
			}
			catch (Exception ex)
			{
				this.erros.descErro = ex.Message;
				this.erros.rc = 9999;
				result = false;
				return result;
			}
			result = true;
			return result;
		}
		private bool montaTrailler()
		{
			this.erros.classe = "Montar_Retorno_Banco";
			this.erros.classe = "montaTrailler";
			this.erros.classe = "";
			this.erros.rc = 0;
			bool result;
			try
			{
				this.prmRetorno.trailler.Z01 = this.arquivoRetorno.Substring(0, 1);
				this.prmRetorno.trailler.Z02 = this.arquivoRetorno.Substring(1, 6);
				this.prmRetorno.trailler.Z03 = this.arquivoRetorno.Substring(7, 17);
				this.prmRetorno.trailler.Z04 = this.arquivoRetorno.Substring(24, 126);
			}
			catch (Exception ex)
			{
				this.erros.descErro = ex.Message;
				this.erros.rc = 9999;
				result = false;
				return result;
			}
			if (!this.valida.trailler())
			{
				this.erros = this.valida.erros;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}
		public bool montaRetornoBanco(string tpRetorno)
		{
			bool result;
			if (tpRetorno == "A" && !this.montaHeader())
			{
				result = false;
			}
			else
			{
				if (tpRetorno == "F")
				{
					if (!this.montaDetalhe())
					{
						result = false;
						return result;
					}
				}
				result = (!(tpRetorno == "Z") || this.montaTrailler());
			}
			return result;
		}
	}
}
