using Retorno_Debito_Automatico.BD.Conexao;
using Retorno_Debito_Automatico.Layouts;
using System;
namespace Retorno_Debito_Automatico.Validacoes
{
	internal class Valida : openPost
	{
		public PrmRetorno prmRetorno = new PrmRetorno();
		public bool header()
		{
			this.erros.classe = "Valida";
			this.erros.funcao = "header";
			this.erros.descErro = "";
			bool result;
			if (this.prmRetorno.header.A01 != "A")
			{
				this.erros.rc = 10;
				this.erros.descErro = "LAYOUT HEADER INVALIDO. A01:" + this.prmRetorno.header.A01;
				result = false;
			}
			else
			{
				if (this.prmRetorno.header.A02 != "2")
				{
					this.erros.rc = 11;
					this.erros.descErro = "LAYOUT HEADER(A) INVALIDO. A02:" + this.prmRetorno.header.A02;
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}
		public bool detalhe()
		{
			this.erros.classe = "Valida";
			this.erros.funcao = "detalhe";
			this.erros.descErro = "";
			bool result;
			if (this.prmRetorno.detalhe.F01 != "F")
			{
				this.erros.rc = 12;
				this.erros.descErro = "LAYOUT DETALHE(F) INVALIDO. F01:" + this.prmRetorno.detalhe.F01;
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}
		public bool trailler()
		{
			this.erros.classe = "Valida";
			this.erros.funcao = "trailler";
			this.erros.descErro = "";
			bool result;
			if (Convert.ToInt32(this.prmRetorno.trailler.Z02) != this.prmRetorno.totais.totRegistros)
			{
				this.erros.rc = 13;
				this.erros.descErro = "LAYOUT TRILLER(Z) TOTAL REGISTROS INVALIDO - Encontrado: " + this.prmRetorno.trailler.Z02 + "   Calculado: " + this.prmRetorno.totais.totRegistros.ToString();
				result = false;
			}
			else
			{
				if (Convert.ToDouble(this.prmRetorno.trailler.Z03) != this.prmRetorno.totais.totValores)
				{
					this.erros.rc = 14;
					this.erros.descErro = "LAYOUT TRILLER(Z) VALOR TOTAL REGISTROS INVALIDO - Encontrado: "+ this.prmRetorno.trailler.Z03 + "   Calculado: "+ this.prmRetorno.totais.totValores.ToString();
					result = false;
				}
				else
				{
					result = true;
				}
			}
			return result;
		}
	}
}
