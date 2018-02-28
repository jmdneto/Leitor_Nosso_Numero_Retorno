using ASL_CodigosErros;
using ConnPostNpgsql;
using Retorno_Debito_Automatico.BD.Alteracao;
using Retorno_Debito_Automatico.BD.Conexao;
using Retorno_Debito_Automatico.BD.Consulta;
using Retorno_Debito_Automatico.BD.Inclusao;
using Retorno_Debito_Automatico.Extras;
using Retorno_Debito_Automatico.IO.Log;
using Retorno_Debito_Automatico.Layouts;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace Retorno_Debito_Automatico.Processos
{
    internal class Retorno_Debito_Automatico : openPost
    {
        private CodigosErros codErros = new CodigosErros();
        private Cod_Retorno_Banco codRetorno = new Cod_Retorno_Banco();
        public Pagamento pagamento = new Pagamento();
        public PrmRetorno prmRetorno = new PrmRetorno();
        private LogErroTxt logErrosTxt = new LogErroTxt();
        private LogProcessoTxt logProc = new LogProcessoTxt();
        private Complementa_Dados complDados = new Complementa_Dados();
        private APagamento aPagamento = new APagamento();
        private AComissionamento aComissao = new AComissionamento();
        private IHistPagamento iHistPagamento = new IHistPagamento();
        private IHistComissao iHistComissao = new IHistComissao();
        private CImobiliaria cImobiliaria = new CImobiliaria();
        private ASolicitacao_Debito_Automatico aSol_Deb_Aut = new ASolicitacao_Debito_Automatico();
        private CCodDevDebito cCodDevDeb = new CCodDevDebito();
        private IHistComissaoStatus iHistComStatus = new IHistComissaoStatus();
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
            List<string> status_antecipacao = new List<string> { "C21" }; //Lista de status de Antecipação.
			this.erros.classe = "Retorno_Debito_Automatico";
			this.erros.funcao = "Processar";
			this.erros.descErro = "";
			bool result;
			try
			{
				string text = "";
				this.complDados.prmRetorno = this.prmRetorno;
				if (!this.complDados.dados_venda_pagamento(ref conexao))
				{
					this.erros = this.complDados.erros;
					this.logErrosTxt.gravaLogProcesso(this.erros);
					result = false;
					return result;
				}
				this.dtHr_Alteracao = this.dtHr_Alteracao.AddMilliseconds(1.0);
				this.iHistPagamento.pagamento = this.complDados.pagamento;
				this.iHistPagamento.pagamento.data_alteracaoHist = this.dtHr_Alteracao;
				this.aPagamento.pagamento = this.iHistPagamento.pagamento;
				this.aPagamento.pagamento.cod_movimento = this.prmRetorno.detalhe.F12;
				if (this.aPagamento.pagamento.status_pagamento == "P07")
				{
					this.erros.codErro = "10";
					this.erros.descErro = "PAGAMENTO JA EFETUADO ANTERIORMENTE, REGISTRO NAO PROCESSADO. ID_REGISTRO: " + this.prmRetorno.detalhe.F08;
					this.logErrosTxt.gravaLogProcesso(this.erros);
					this.pagamento = this.aPagamento.pagamento;
					result = true;
					return result;
				}
                if (this.prmRetorno.detalhe.F07 == "55" && this.prmRetorno.header.A05 != "399")
                {
                    result = true;
                    return result;
                }
				if (this.aPagamento.pagamento.cod_movimento == "0")
				{
					if (this.prmRetorno.detalhe.cad_optante_itau)
					{
						if (!(this.aPagamento.pagamento.status_pagamento == "P02"))
						{
							this.erros.codErro = "100";
							this.erros.descErro = "STATUS PAGAMENTO INVALIDO PARA CONFIRMACAO DE AGENDAMENTO: ID_PAGAMENTO:" + this.aPagamento.pagamento.id_pagamento;
							this.logErrosTxt.gravaLogProcesso(this.erros);
							result = false;
							return result;
						}
						text = "P03";
					}
					else
					{
						if (this.prmRetorno.detalhe.F07 == "00" || this.prmRetorno.detalhe.F07 == "31")
						{
							text = "P07";
							this.aPagamento.pagamento.data_pagamento = this.func.montaDataddMMyyyy(this.prmRetorno.detalhe.F05);                            
							foreach (Comissionamento current in this.aPagamento.pagamento.comissionamento)
							{
                                //TODO: Peter checar com o Robson. A condicional do IF é código novo. Antes, estava sempre alterando o status para C05.
                                if (current.id_status_ant == "C01")
                                {
                                    current.id_status = "C05";
                                }
                                else if (status_antecipacao.Contains(current.id_status_ant)) //AntecipaçãoC21
                                {
                                    current.id_status = "C19";
                                }
                                else
                                {
                                    current.id_status = current.id_status_ant;
                                }
							}
						}
						if (this.prmRetorno.detalhe.F07 == "01")
						{
                            //TODO: Verificar se neste ponto do código teremos que validar se o número de reapresentação foi excedido
							text = "P08";
							this.descricaoRetorno(this.prmRetorno.detalhe.F07, ref conexao);
							foreach (Comissionamento current in this.aPagamento.pagamento.comissionamento)
							{
                                if (status_antecipacao.Contains(current.id_status_ant))
                                {
                                    current.id_status = current.id_status_ant; //Mantem antecipacao
                                }
                                else
                                {
                                    current.id_status = "C04";
                                }
								current.desc_hist = this.descRetorno;
							}
						}
						if (this.prmRetorno.detalhe.F07 == "02" || this.prmRetorno.detalhe.F07 == "04" || this.prmRetorno.detalhe.F07 == "05" || this.prmRetorno.detalhe.F07 == "10" || this.prmRetorno.detalhe.F07 == "12" || this.prmRetorno.detalhe.F07 == "14" || this.prmRetorno.detalhe.F07 == "15" || this.prmRetorno.detalhe.F07 == "18" || this.prmRetorno.detalhe.F07 == "19" || this.prmRetorno.detalhe.F07 == "20" || this.prmRetorno.detalhe.F07 == "30" || this.prmRetorno.detalhe.F07 == "31" || this.prmRetorno.detalhe.F07 == "96")
						{
                            //TODO: Verificar se neste ponto do código teremos que validar se o número de reapresentação foi excedido
							if (this.aPagamento.pagamento.cod_banco == "33" && this.prmRetorno.detalhe.F07 == "04")
							{
								text = "P08";
								this.descricaoRetorno(this.prmRetorno.detalhe.F07, ref conexao);
								foreach (Comissionamento current in this.aPagamento.pagamento.comissionamento)
								{
                                    if (status_antecipacao.Contains(current.id_status_ant))
                                    {
                                        current.id_status = current.id_status_ant; //Mantem antecipacao
                                    }
                                    else if (current.id_status_ant == "C02")
                                    {
                                        current.id_status = "C02";
                                    }
                                    else
                                    {
                                        current.id_status = "C04";
                                    }
									current.desc_hist = this.descRetorno;
								}
							}
							else
							{
								text = "P04";
								this.descricaoRetorno(this.prmRetorno.detalhe.F07, ref conexao);
								foreach (Comissionamento current in this.aPagamento.pagamento.comissionamento)
								{
                                    if (status_antecipacao.Contains(current.id_status_ant))
                                    {
                                        current.id_status = "C20"; //Antecipacao Problema Cobrança
                                    }
                                    else if (current.id_status_ant == "C02")
                                    {
                                        current.id_status = "C02";
                                    }
                                    else
                                    {
                                        current.id_status = "C13";
                                    }
									current.desc_hist = this.descRetorno;
								}
							}
						}
						if (this.prmRetorno.detalhe.F07 == "13")
						{
							text = "P05";
						}
						if (this.prmRetorno.detalhe.F07 == "99")
						{
							if (this.prmRetorno.header.A05 != "104")
							{
								text = "P04";
								this.descricaoRetorno("60", ref conexao);
								foreach (Comissionamento current in this.aPagamento.pagamento.comissionamento)
								{
                                    if (status_antecipacao.Contains(current.id_status_ant))
                                    {
                                        current.id_status = "C20"; //Antecipação Problema Cobrança.
                                    }
                                    else if (current.id_status_ant == "C02")
                                    {
                                        current.id_status = "C02";
                                    }
                                    else
                                    {
                                        current.id_status = "C15";
                                    }
									current.desc_hist = this.descRetorno;
								}
							}
							else
							{
                                //Verificar se pagamento é relativo a antecipacao
                                if (this.aPagamento.pagamento.antecipacao == false)
                                {
                                    text = "P13";
                                    this.descricaoRetorno(this.prmRetorno.detalhe.F07, ref conexao);
                                    foreach (Comissionamento current in this.aPagamento.pagamento.comissionamento)
                                    {
                                        if (status_antecipacao.Contains(current.id_status_ant))
                                        {
                                            current.id_status = "C20"; //Antecipação Problema Cobrança
                                        }
                                        else if (current.id_status_ant == "C02")
                                        {
                                            current.id_status = "C02";
                                        }
                                        else
                                        {
                                            current.id_status = "C12";
                                        }
                                        current.desc_hist = this.descRetorno;
                                    }
                                }
                                else
                                {
                                    this.erros.codErro = "11";
                                    this.erros.descErro = "CANCELAMENTO NEGADO. PAGAMENTO COM UMA COMISSÃO ANTECIPADA: " + this.prmRetorno.detalhe.F07;
                                    result = false;
                                    return result;
                                
                                }
							}
						}
						if (text == "")
						{
							this.erros.codErro = "11";
							this.erros.descErro = "CODIGO DO RETORNO INVALIDO PARA UM DEBITO: " + this.prmRetorno.detalhe.F07;
							result = false;
							return result;
						}
					}
				}
				if (this.aPagamento.pagamento.cod_movimento == "1")
				{
					if (this.prmRetorno.detalhe.F07 == "99")
					{
						text = "P13";
						this.descricaoRetorno(this.prmRetorno.detalhe.F07, ref conexao);
						foreach (Comissionamento current in this.aPagamento.pagamento.comissionamento)
						{
                            if (status_antecipacao.Contains(current.id_status_ant))
                            {
                                current.id_status = "C20"; //Antecipação Problema Cobrança
                            }
                            else if (current.id_status_ant == "C02")
                            {
                                current.id_status = "C02";
                            }
                            else
                            {
                                current.id_status = "C12";
                            }
							current.desc_hist = this.descRetorno;
						}
					}
					if (this.prmRetorno.detalhe.F07 == "96" || this.prmRetorno.detalhe.F07 == "97" || this.prmRetorno.detalhe.F07 == "98")
					{
						text = "P14";
						this.descricaoRetorno(this.prmRetorno.detalhe.F07, ref conexao);
						foreach (Comissionamento current in this.aPagamento.pagamento.comissionamento)
						{
                            if (status_antecipacao.Contains(current.id_status_ant))
                            {
                                current.id_status = "C20"; //Antecipação Problema Cobrança
                            }
                            else if (current.id_status_ant == "C02")
                            {
                                current.id_status = "C02";
                            }
                            else
                            {
                                current.id_status = "C12";
                            }
							current.desc_hist = this.descRetorno;
						}
					}
					if (text == "")
					{
						this.erros.codErro = "11";
						this.erros.descErro = "CODIGO DO RETORNO INVALIDO PARA UM CANCELAMENTO: " + this.prmRetorno.detalhe.F07 + " SOLDEBAUT " + this.prmRetorno.detalhe.F08;
						result = false;
						return result;
					}
				}
				if (this.aPagamento.pagamento.cod_movimento != "0" && this.aPagamento.pagamento.cod_movimento != "1")
				{
					this.erros.codErro = "11";
					this.erros.descErro = "CODIGO DO MOVIMENTO INVALIDO: " + this.aPagamento.pagamento.cod_movimento;
					result = false;
					return result;
				}
				if (!this.iHistPagamento.gravaHistPagamento(ref conexao))
				{
					this.erros = this.iHistPagamento.erros;
					this.logErrosTxt.gravaLogProcesso(this.erros);
					result = false;
					return result;
				}
				if (!this.aPagamento.statusPagamento(ref conexao, text, this.prmRetorno.detalhe.F07))
				{
					this.erros = this.aPagamento.erros;
					this.logErrosTxt.gravaLogProcesso(this.erros);
					result = false;
					return result;
				}
				this.pagamento = this.aPagamento.pagamento;
				if (this.pagamento.cod_movimento == "0" && this.pagamento.seq_solicitacao_deb_aut != "0")
				{
					this.aSol_Deb_Aut.pagamento = this.pagamento;
					if (!this.aSol_Deb_Aut.atualiza_sol_deb_aut(ref conexao, this.prmRetorno.detalhe.F07, this.descRetorno))
					{
						this.erros = this.aSol_Deb_Aut.erros;
						this.logErrosTxt.gravaLogProcesso(this.erros);
						result = false;
						return result;
					}
				}
				if (text != "P03" && text != "P05")
				{
					this.iHistComissao.pagamento = this.pagamento;
					this.aComissao.pagamento = this.pagamento;
					this.iHistComStatus.pagamento = this.pagamento;

                    bool enviaEmail = false; //Tratamento Bradesco Cancelamentos

					for (int i = 0; i < this.pagamento.comissionamento.Count; i++)
					{
                        //Adicionados C04, C11, C12, C13, C15 - Tratamento falha ao cancelar Bradesco
                        if (text == "P07" && this.aComissao.pagamento.comissionamento[i].id_status_ant != "C00" && this.aComissao.pagamento.comissionamento[i].id_status_ant != "C01" && this.aComissao.pagamento.comissionamento[i].id_status_ant != "C02" && this.aComissao.pagamento.comissionamento[i].id_status_ant != "C21" && this.aComissao.pagamento.comissionamento[i].id_status_ant != "C04" && this.aComissao.pagamento.comissionamento[i].id_status_ant != "C11" & this.aComissao.pagamento.comissionamento[i].id_status_ant != "C12" && this.aComissao.pagamento.comissionamento[i].id_status_ant != "C13" && this.aComissao.pagamento.comissionamento[i].id_status_ant != "C15")
                        {
                                this.erros.codErro = "100";
                                this.erros.descErro = "PAGAMENTO EFETUADO--->STATUS COMISSAO INVALIDO PARA LIBERACAO DE RESGATE-->ID_COMISSAO:" + this.aComissao.pagamento.comissionamento[i].id_comissionamento;
                                this.logErrosTxt.gravaLogProcesso(this.erros);
                                result = false;
                                return result;                            
                        }
                        else
                        {

                            //C04, C11, C12, C13, C15 - Tratamento falha ao cancelar Bradesco
                            if (text == "P07" && (this.aComissao.pagamento.comissionamento[i].id_status_ant == "C04" || this.aComissao.pagamento.comissionamento[i].id_status_ant == "C11" || this.aComissao.pagamento.comissionamento[i].id_status_ant == "C12" || this.aComissao.pagamento.comissionamento[i].id_status_ant == "C13" || this.aComissao.pagamento.comissionamento[i].id_status_ant == "C15"))
                            {
                                this.aComissao.pagamento.comissionamento[i].id_status = "C02"; //bloqueia comissão
                                enviaEmail = true;
                            }

                            // busca dados da imobiliaria para calcular a taxa de comissão
                            Imobiliaria imob = new Imobiliaria();
                            imob.id_imobiliaria = this.pagamento.codImobiliaria;
                            this.cImobiliaria.imobiliaria = imob;
                            if (!this.cImobiliaria.buscaImobiliaria(ref conexao)) {
                                this.erros = this.cImobiliaria.erros;
                                this.logErrosTxt.gravaLogProcesso(this.erros);
                                result = false;
                                return result;
                            }
                            
                            if (!this.iHistComissao.gravaHistComissionamento(ref conexao, i))
                            {
                                this.erros = this.iHistComissao.erros;
                                this.logErrosTxt.gravaLogProcesso(this.erros);
                                result = false;
                                return result;
                            }
                            if (this.aComissao.pagamento.comissionamento[i].valor_percentual_banco <= 0)
                            {
                                //inicio do calculo da taxa de comissão
                                double txComissao = 0;
                                if (this.aComissao.pagamento.comissionamento[i].cpf_autonomo.Length <= 11)
                                {
                                    txComissao = imob.percentual_banco;
                                }
                                else
                                {
                                    txComissao = imob.taxa_de_servico_cnpj;
                                }
                                this.aComissao.pagamento.comissionamento[i].valor_percentual_banco = this.aComissao.pagamento.comissionamento[i].valor * (txComissao / 100.0);
                            }
                            if (!this.aComissao.updateComissao(ref conexao, i, this.aComissao.pagamento.comissionamento[i].id_status))
                            {
                                this.erros = this.aComissao.erros;
                                this.logErrosTxt.gravaLogProcesso(this.erros);
                                result = false;
                                return result;
                            }
                        }

                        /*this.vlr_ted = this.prmLibCom.Tarifa_ted;
                        if (this.prmLibCom.pagamento.Tp_pagamento == "1")
                        {
                            this.vlr_deb_boleto_comprador = this.prmLibCom.pagamento.Valor_tot_deb_aut / (double)this.prmLibCom.pagamento.Tot_corretores;
                        }*/

                        //if (this.aComissao.pagamento.comissionamento[i].id_status_ant != "C00")
                        //{
                        //    if (this.aComissao.pagamento.comissionamento[i].id_status_ant == "C02")
                        //    {
                        //        this.iHistComStatus.pagamento.data_alteracaoHist = this.iHistComStatus.pagamento.data_alteracaoHist.AddMilliseconds(1.0);
                        //        if (!this.iHistComStatus.gravaHistComissionamento(ref conexao, i))
                        //        {
                        //            this.erros = this.iHistComissao.erros;
                        //            this.logErrosTxt.gravaLogProcesso(this.erros);
                        //            result = false;
                        //            return result;
                        //        }
                        //    }
                        //    else
                        //    {
                        //        if (!this.iHistComissao.gravaHistComissionamento(ref conexao, i))
                        //        {
                        //            this.erros = this.iHistComissao.erros;
                        //            this.logErrosTxt.gravaLogProcesso(this.erros);
                        //            result = false;
                        //            return result;
                        //        }
                        //        if (!this.aComissao.updateComissao(ref conexao, i, this.aComissao.pagamento.comissionamento[i].id_status))
                        //        {
                        //            this.erros = this.aComissao.erros;
                        //            this.logErrosTxt.gravaLogProcesso(this.erros);
                        //            result = false;
                        //            return result;
                        //        }
                        //    }
                        //}
					}


                    if (enviaEmail)
                    {
                        try
                        {
                            string textoEmail = ("Verificar necessidade de estorno (debito falhou ao cancelar e debitou), imob: " + this.pagamento.codImobiliaria + " pv: " + this.pagamento.id_venda_imobiliaria + " id_pag_imob: " + this.pagamento.id_pagamento_imobiliaria);
                            this.EnviaEmail("Falha Debito Não Cancelado e Debitado", textoEmail);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Erro ao enviar Email: " + ex.Message);
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

        public void EnviaEmail(string subject, string body)
        {
            using (var message = new MailMessage("robobm@paulistaservicos.com.br", "henrique.soncin@paulistaservicos.com.br, backoffice@paulistaservicos.com.br, wassa-team@bitsforest.com"))
            {
                message.Subject = subject;
                message.Body = body;
                using (SmtpClient client = new SmtpClient
                {
                    EnableSsl = true,
                    Host = "smtp.gmail.com",
                    Port = 587,
                    Credentials = new NetworkCredential("robobm@paulistaservicos.com.br", "pservicos2016"),
                })
                {
                    client.Send(message);
                    Console.WriteLine("Email enviado as: " + DateTime.Now.ToString());
                }
            }
        }

    }
}
