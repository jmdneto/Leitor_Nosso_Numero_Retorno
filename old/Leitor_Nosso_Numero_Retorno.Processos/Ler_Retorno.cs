using Retorno_Debito_Automatico.BD.Conexao;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Retorno_Debito_Automatico.Processos
{
	internal class Ler_Retorno : openPost
	{
		public string PathArqRetorno = "";
		public string[] arquivoRetorno;
		public string nomeArq = "";
		public string PathLog = "";
		public string[] arquivos;
		public string nomeArqProcessados;
		public string[] registros;


        public bool retornoBanco()
		{
			this.erros.funcao = "Ler_Retorno";
			this.erros.funcao = "retornoBanco";
			this.erros.descErro = "";
			bool result;
			try
			{
				this.arquivoRetorno = File.ReadAllLines(this.PathArqRetorno);
                
                if (this.arquivoRetorno.GetLength(0) == 0)
				{
					this.erros.descErro = "NAO EXISTE ARQUIVO PARA PROCESSAMENTO EM: " + this.PathArqRetorno;
					this.erros.rc = 5;
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
		private bool getPath()
		{
			bool result;
			try
			{
				StreamReader streamReader = new StreamReader("PathLog.txt");
				this.PathLog = streamReader.ReadLine();
			}
			catch (Exception ex)
			{
				this.erros.descErro = "EXCEPTION:" + ex.Message;
				this.erros.rc = 5;
				result = false;
				return result;
			}
			result = true;
			return result;
		}
		public bool verifica_arq_processado(ref string retorno_a_processar)
		{
			this.erros.funcao = "Ler_Retorno";
			this.erros.funcao = "verifica_arq_processado";
			this.erros.descErro = "";
			bool result;
			try
			{
				if (!this.getPath())
				{
					result = false;
				}
				else
				{
					this.arquivos = Directory.GetFiles(this.PathLog, "*");
					if (this.arquivos.GetLength(0) == 0)
					{
						this.erros.descErro = "NAO EXISTE ARQUIVO PARA PROCESSAMENTO EM: " + this.PathLog;
						result = false;
					}
					else
					{
						for (int i = 0; i < this.arquivos.Length; i++)
						{
							string[] array = this.arquivos[i].ToString().Split(new char[]
							{
								'\\'
							});
							this.nomeArqProcessados = array[array.Length - 1];
							if (string.Compare(retorno_a_processar, this.nomeArqProcessados.Substring(9, this.nomeArqProcessados.Length - 9)) == 0)
							{
								this.erros.descErro = "ARQUIVO JA PROCESSADO.....:" + this.nomeArqProcessados;
								result = false;
								return result;
							}
						}
						result = true;
					}
				}
			}
			catch (Exception ex)
			{
				this.erros.descErro = "VERIFIQUE SE O DIRETORIO ESTA ACESSIVEL.";
				result = false;
			}
			return result;
		}
	}
}
