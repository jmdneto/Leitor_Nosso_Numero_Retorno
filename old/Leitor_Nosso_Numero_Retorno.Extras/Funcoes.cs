using ASL_TipoCPF;
using Retorno_Debito_Automatico.Layouts;
using System;
namespace Retorno_Debito_Automatico.Extras
{
	internal class Funcoes
	{
		public Erros erros = new Erros();
		public string editaValorInteiro(string valor)
		{
			this.erros.classe = "Funcoes";
			this.erros.funcao = "verificaNumerico()";
			string result;
			try
			{
				double num = Convert.ToDouble(valor);
				result = (num / 100.0).ToString().Replace(',', '.');
			}
			catch (Exception ex)
			{
				this.erros.codErro = "9999";
				this.erros.descErro = "EXCEPTION. Msg.: " + ex.Message;
				result = "0";
			}
			return result;
		}
		public bool verificaNumerico(string numero)
		{
			this.erros.classe = "Funcoes";
			this.erros.funcao = "verificaNumerico()";
			long num = 0L;
			bool result;
			try
			{
				if (!long.TryParse(numero, out num))
				{
					result = false;
					return result;
				}
			}
			catch (Exception ex)
			{
				this.erros.codErro = "9999";
				this.erros.descErro = "EXCEPTION. Msg.: " + ex.Message;
				result = false;
				return result;
			}
			result = true;
			return result;
		}
		public DateTime montaDataddMMyyyy(string strData)
		{
			DateTime result = default(DateTime);
			this.erros.classe = "Funcoes";
			this.erros.funcao = "montaDataddMMyyyy";
			this.erros.descErro = "";
			try
			{
				result = DateTime.Parse(string.Concat(new string[]
				{
					strData.Substring(6, 2),
					"/",
					strData.Substring(4, 2),
					"/",
					strData.Substring(0, 4)
				}));
			}
			catch (Exception ex)
			{
				this.erros.descErro = "EXCEPTION:" + ex.Message;
			}
			return result;
		}
		public bool validaCPF(string cpf)
		{
			this.erros.classe = "Funcoes";
			this.erros.funcao = "validaCPF()";
			TipoCPF tipoCPF = new TipoCPF();
			bool result;
			try
			{
				tipoCPF.setString = cpf;
				if (!tipoCPF.isOk())
				{
					this.erros.descErro = "CPF INVALIDADO...";
					result = false;
				}
				else
				{
					result = true;
				}
			}
			catch (Exception ex)
			{
				this.erros.codErro = "9999";
				this.erros.descErro = "EXCEPTION (VALIDA CPF). Msg.: " + ex.Message;
				result = false;
			}
			return result;
		}
	}
}
