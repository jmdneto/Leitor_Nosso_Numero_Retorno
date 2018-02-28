using ConnPostNpgsql;
using Retorno_Debito_Automatico.BD.Alteracao;
using Retorno_Debito_Automatico.BD.Conexao;
using Retorno_Debito_Automatico.BD.Consulta;
using Retorno_Debito_Automatico.BD.Inclusao;
using Retorno_Debito_Automatico.Extras;
using Retorno_Debito_Automatico.IO.Log;
using Retorno_Debito_Automatico.Layouts;
using System;
namespace Retorno_Debito_Automatico.Processos
{
	internal class Retorno_Solicita_Cadastro_Caixa : openPost
	{
		public PrmRetorno prmRetorno = new PrmRetorno();
		public Pagamento pagamento = new Pagamento();
		private Cod_Retorno_Banco codRetorno = new Cod_Retorno_Banco();
		private CPagamentos104 cPagamentos104 = new CPagamentos104();
		private APagamento104 aPagamento104 = new APagamento104();
		private ACad_Deb_Automatico aCad_Deb_Aut = new ACad_Deb_Automatico();
		private IHistPagamento iHistPagamento = new IHistPagamento();
		private ICad_CC_Deb_Aut iCad_CC_Deb_Aut = new ICad_CC_Deb_Aut();
		private LogErroTxt logErrosTxt = new LogErroTxt();
		private LogProcessoTxt logProc = new LogProcessoTxt();
		private CCodDevDebito cCodDevDeb = new CCodDevDebito();
		private IHistComissao iHistComissao = new IHistComissao();
		private AComissionamento104 aComissao = new AComissionamento104();
		private Funcoes func = new Funcoes();
		public DateTime dtHr_Alteracao = default(DateTime);
		private string descRetorno = "";
		private bool descricaoRetorno(string codRetornoBanco, ref Conexao conexao)
		{
			bool result;
			try
			{
				if (this.cCodDevDeb.listCodDevDeb.Count == 0)
				{
					if (!this.cCodDevDeb.buscaCodDevDebito(ref conexao))
					{
						this.erros = this.cCodDevDeb.erros;
						this.logErrosTxt.gravaLogProcesso(this.erros);
						result = false;
						return result;
					}
				}
				foreach (Cod_Devolucao_Debito current in this.cCodDevDeb.listCodDevDeb)
				{
					if (current.codDev == Convert.ToInt32(codRetornoBanco))
					{
						this.descRetorno = current.descDev;
						break;
					}
				}
				if (this.descRetorno == "")
				{
					this.erros.descErro = "CODIGO DO RETORNO DO BANCO INVALIDO:" + codRetornoBanco;
					this.erros.rc = 5;
					result = false;
				}
				else
				{
					result = true;
				}
			}
			catch (ExecutionEngineException ex)
			{
				this.erros.descErro = "EXCEPTION:" + ex.Message;
				this.erros.rc = 5;
				Console.WriteLine(ex.Message);
				result = false;
			}
			return result;
		}
		public bool Processar(ref Conexao conexao)
		{
			this.erros.classe = "Retorno_Solicita_Cadastro_Caixa";
			this.erros.funcao = "Processar";
			this.erros.descErro = "";
			string b = "";
			string motivo_devolucao = "";
			bool result;
			try
			{
				this.cPagamentos104.pagamento.cod_banco = this.prmRetorno.header.A05;
				this.cPagamentos104.pagamento.cpf_comprador = this.prmRetorno.detalhe.F10;
				this.cPagamentos104.pagamento.agencia = this.prmRetorno.detalhe.F03;
				this.cPagamentos104.pagamento.conta = this.prmRetorno.detalhe.F04.Substring(0, 3);
				Pagamento expr_DC = this.cPagamentos104.pagamento;
				expr_DC.conta += this.prmRetorno.detalhe.F04.Substring(3, 8);
				this.cPagamentos104.pagamento.dig_conta = this.prmRetorno.detalhe.F04.Substring(11, 1);
				this.cPagamentos104.pagamento.cod_movimento = this.prmRetorno.detalhe.F12;
				if (!this.cPagamentos104.busca_pagamentos(ref conexao))
				{
					this.erros = this.cPagamentos104.erros;
					this.logErrosTxt.gravaLogProcesso(this.erros);
					result = false;
					return result;
				}
				for (int i = 0; i < this.cPagamentos104.list_pagamento.pagamentos.Count; i++)
				{
					this.iHistPagamento.pagamento = this.cPagamentos104.list_pagamento.pagamentos[i];
					this.dtHr_Alteracao = this.dtHr_Alteracao.AddMilliseconds(1.0);
					this.iHistPagamento.pagamento.data_alteracaoHist = this.dtHr_Alteracao;
					this.descricaoRetorno(this.prmRetorno.detalhe.F07, ref conexao);
					if (!this.iHistPagamento.gravaHistPagamento(ref conexao))
					{
						this.erros = this.iHistPagamento.erros;
						this.logErrosTxt.gravaLogProcesso(this.erros);
						result = false;
						return result;
					}
					this.aPagamento104.pagamento = this.cPagamentos104.list_pagamento.pagamentos[i];
					string text;
					if (this.prmRetorno.detalhe.F07 == "00")
					{
						text = "P17";
					}
					else
					{
						text = "P18";
					}
					if (this.prmRetorno.detalhe.F07 == "02")
					{
						motivo_devolucao = "19";
					}
					if (!this.aPagamento104.statusPagamento(ref conexao, text, this.descRetorno, motivo_devolucao))
					{
						this.erros = this.aPagamento104.erros;
						this.logErrosTxt.gravaLogProcesso(this.erros);
						result = false;
						return result;
					}
					this.pagamento = this.cPagamentos104.list_pagamento.pagamentos[i];
					string text2 = this.pagamento.cpf_comprador;
					text2 += this.pagamento.cod_banco;
					text2 += this.pagamento.agencia;
					text2 += this.pagamento.conta;
					text2 += this.pagamento.dig_conta;
					if (text2 != b)
					{
						b = text2;
						this.iCad_CC_Deb_Aut.pagamento = this.pagamento;
						if (!this.iCad_CC_Deb_Aut.grava_hist_cad_cc_deb_aut(ref conexao))
						{
							this.erros = this.iCad_CC_Deb_Aut.erros;
							this.logErrosTxt.gravaLogProcesso(this.erros);
							result = false;
							return result;
						}
						this.aCad_Deb_Aut.pagamento = this.pagamento;
						if (!this.aCad_Deb_Aut.status_cad_deb_auto(ref conexao, text, this.descRetorno))
						{
							this.erros = this.aCad_Deb_Aut.erros;
							this.logErrosTxt.gravaLogProcesso(this.erros);
							result = false;
							return result;
						}
					}
					if (text == "P18")
					{
						this.aComissao.pagamento = this.pagamento;
						if (!this.aComissao.updateComissao(ref conexao, "C13"))
						{
							this.erros = this.aComissao.erros;
							this.logErrosTxt.gravaLogProcesso(this.erros);
							result = false;
							return result;
						}
					}
				}
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
	}
}
