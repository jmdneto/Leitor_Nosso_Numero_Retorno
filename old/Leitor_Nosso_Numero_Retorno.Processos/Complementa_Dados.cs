using ASL_CodigosErros;
using ConnPostNpgsql;
using Retorno_Debito_Automatico.BD.Conexao;
using Retorno_Debito_Automatico.BD.Consulta;
using Retorno_Debito_Automatico.Layouts;
using System;
using System.Linq;
using System.Text.RegularExpressions;
namespace Retorno_Debito_Automatico.Processos
{
	internal class Complementa_Dados : openPost
	{
		private CodigosErros codErros = new CodigosErros();
		public Pagamento pagamento = new Pagamento();
		public PrmRetorno prmRetorno = new PrmRetorno();
		private CVenda cVenda = new CVenda();
		private CPagamentos cPagamento = new CPagamentos();
		private CComissionamento cComissao = new CComissionamento();
		private CSolDebAutomatico cSolDebAut = new CSolDebAutomatico();
		private string[] dadosF08;
		public bool dados_venda_pagamento(ref Conexao conexao)
		{
			this.erros.classe = "ProcessaDebitos";
			this.erros.funcao = "dados_venda_pagamento()";
			this.erros.descErro = "";
			bool result;
			try
			{
				this.dadosF08 = this.prmRetorno.detalhe.F08.Split(new char[]
				{
					';'
				});
				if (this.dadosF08.Count<string>() > 3)
				{
					this.pagamento.codImobiliaria = this.dadosF08[0].Trim();
					this.pagamento.id_venda = this.dadosF08[1].Trim();
					this.pagamento.id_pagamento = this.dadosF08[2].Trim();
					this.pagamento.seq_solicitacao_deb_aut = this.dadosF08[3].Trim();
				}
				else
				{
					if (this.dadosF08.Count<string>() > 1)
					{
						this.pagamento.codImobiliaria = this.dadosF08[0].Trim();
						this.pagamento.id_venda = this.dadosF08[1].Trim();
						this.pagamento.id_pagamento = this.dadosF08[2].Trim();
						this.pagamento.seq_solicitacao_deb_aut = "0";
					}
					else
					{
                        this.pagamento.seq_solicitacao_deb_aut = Regex.Replace(this.dadosF08[0], @"[^\d]", String.Empty).Trim();
					}
				}
				if (this.dadosF08.Count<string>() == 1)
				{
					this.cSolDebAut.pagamento = this.pagamento;
					if (!this.cSolDebAut.busca_sol_deb_automatico(ref conexao))
					{
						this.erros = this.cSolDebAut.erros;
						result = false;
						return result;
					}
				}
				this.cVenda.pagamento = this.pagamento;
				if (!this.cVenda.buscaVenda(ref conexao))
				{
					this.erros = this.cVenda.erros;
					result = false;
					return result;
				}
				this.cPagamento.pagamento = this.pagamento;
				if (!this.cPagamento.busca_pagamentos_deb_automatico(ref conexao))
				{
					this.erros = this.cPagamento.erros;
					result = false;
					return result;
				}
				this.cComissao.pagamento = this.pagamento;
				if (!this.cComissao.buscaComissionamento(ref conexao))
				{
					this.erros = this.cComissao.erros;
					result = false;
					return result;
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
