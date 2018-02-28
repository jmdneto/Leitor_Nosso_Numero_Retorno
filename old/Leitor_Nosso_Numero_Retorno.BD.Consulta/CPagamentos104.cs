using ASL_CodigosErros;
using ConnPostNpgsql;
using Npgsql;
using Retorno_Debito_Automatico.BD.Conexao;
using Retorno_Debito_Automatico.IO.Log;
using Retorno_Debito_Automatico.Layouts;
using System;
namespace Retorno_Debito_Automatico.BD.Consulta
{
    internal class CPagamentos104 : openPost
    {
        private CodigosErros codErros = new CodigosErros();
        private LogErroTxt logErrosTxt = new LogErroTxt();
        public List_Pagamento list_pagamento = new List_Pagamento();
        public Pagamento pagamento = new Pagamento();
        public bool flgProcessar = false;
        public bool busca_pagamentos(ref ConnPostNpgsql.Conexao conexao)
        {
            this.erros.classe = "CPagamentos104";
            this.erros.funcao = "busca_pagamentos()";
            this.erros.codErro = "";
            NpgsqlCommand npgsqlCommand = new NpgsqlCommand();
            NpgsqlDataReader npgsqlDataReader = null;
            bool flag = false;
            //string text = "";
            //string value = "";
            bool result;
            try
            {
                npgsqlCommand.Connection = conexao.ConexaoBd;
                if (conexao.transactionIsOpen())
                {
                    npgsqlCommand.Transaction = conexao.TransacaoBd;
                }

                string txtquery = string.Empty;

                //string txtquery = "SELECT cpf_comprador, id_venda, id_pagamento, id_pagamento_imobiliaria, id_imobiliaria, deb_cod_banco ,deb_numero_agencia, deb_numero_conta, deb_dig_verificador_conta";
                //txtquery += " from tb_pagamentos WHERE cpf_comprador = :cpf_comprador AND deb_cod_banco = :deb_cod_banco AND deb_dig_verificador_conta = :deb_dig_verificador_conta AND id_status = 'P16'";
                //txtquery += "AND deb_numero_agencia = :deb_numero_agencia and ( (substr(lpad(deb_numero_conta,8,'0'),4,8) = :deb_numero_conta) or (substr(lpad(deb_numero_conta,8,'0'),4,8) = :deb_numero_conta) )";

                //ANTIGO SUBSTITUIDO POR LIKE
                //txtquery = "SELECT * FROM";
                //txtquery += "      (SELECT id_status, ";
                //txtquery += "              CASE WHEN length(deb_numero_conta)>8 THEN substr(deb_numero_conta,(length(deb_numero_conta)-4),5)";
                //txtquery += "                   ELSE CASE WHEN substr(deb_numero_conta,1,3) = '001' THEN  substr(lpad(substr(deb_numero_conta,4,length(deb_numero_conta)),8,'0'),4,5)";
                //txtquery += "                             ELSE substr(lpad(deb_numero_conta,8,'0'),(length(lpad(deb_numero_conta,8,'0'))-4),5)";
                //txtquery += "                        END";
                //txtquery += "              END contaformatada,";
                //txtquery += "       cpf_comprador, id_venda, id_pagamento, id_pagamento_imobiliaria, id_imobiliaria, deb_cod_banco ,deb_numero_agencia, deb_numero_conta, deb_dig_verificador_conta ";
                //txtquery += "       FROM tb_pagamentos";
                //txtquery += "       WHERE cpf_comprador = :cpf_comprador AND deb_cod_banco = :deb_cod_banco AND deb_dig_verificador_conta = :deb_dig_verificador_conta AND (id_status = 'P16' OR id_status = 'P10' OR id_status = 'P13') AND deb_numero_agencia = :deb_numero_agencia";
                //txtquery += "      ) AS Tabela ";
                //txtquery += " WHERE contaformatada = :deb_numero_conta"; 

                txtquery =  "      SELECT id_status, ";
                txtquery += "       cpf_comprador, id_venda, id_pagamento, id_pagamento_imobiliaria, id_imobiliaria, deb_cod_banco ,deb_numero_agencia, deb_numero_conta, deb_dig_verificador_conta ";
                txtquery += "       FROM tb_pagamentos";
                txtquery += "       WHERE cpf_comprador = "+ this.pagamento.cpf_comprador + " AND deb_cod_banco = "+ this.pagamento.cod_banco + " AND (id_status = 'P16' OR id_status = 'P10' OR id_status = 'P13') AND deb_numero_agencia = '"+ this.pagamento.agencia+"'";
                txtquery += " AND deb_dig_verificador_conta = '"+ this.pagamento.dig_conta + "' AND deb_numero_conta like '%" + this.pagamento.conta.Substring(this.pagamento.conta.Length >= 2 ? this.pagamento.conta.Length - 2 : 0, this.pagamento.conta.Length >= 2 ? 2 : this.pagamento.conta.Length) + "%';";

                npgsqlCommand.CommandText = txtquery;
                //openPost.setParameters(npgsqlCommand, ":cpf_comprador", this.pagamento.cpf_comprador);
                //openPost.setParameters(npgsqlCommand, ":deb_cod_banco", this.pagamento.cod_banco);
                //openPost.setParameters(npgsqlCommand, ":deb_numero_agencia", this.pagamento.agencia);
                ////openPost.setParameters(npgsqlCommand, ":deb_numero_conta", this.pagamento.conta.Substring(6)); // trocado por LIKE
                //openPost.setParameters(npgsqlCommand, ":deb_dig_verificador_conta", this.pagamento.dig_conta);
                npgsqlDataReader = npgsqlCommand.ExecuteReader();
                this.list_pagamento.pagamentos.Clear();
                while (npgsqlDataReader.Read())
                {
                    int ordinal = npgsqlDataReader.GetOrdinal("id_status");
                    if (npgsqlDataReader.GetValue(ordinal).ToString() == "P16")
                    {
                    ordinal = npgsqlDataReader.GetOrdinal("id_imobiliaria");
                    this.pagamento.codImobiliaria = npgsqlDataReader.GetValue(ordinal).ToString();
                    ordinal = npgsqlDataReader.GetOrdinal("id_pagamento_imobiliaria");
                    this.pagamento.id_pagamento_imobiliaria = npgsqlDataReader.GetValue(ordinal).ToString();
                    ordinal = npgsqlDataReader.GetOrdinal("id_venda");
                    this.pagamento.id_venda = npgsqlDataReader.GetValue(ordinal).ToString();
                    ordinal = npgsqlDataReader.GetOrdinal("id_pagamento");
                    this.pagamento.id_pagamento = npgsqlDataReader.GetValue(ordinal).ToString();
                    ordinal = npgsqlDataReader.GetOrdinal("cpf_comprador");
                    this.pagamento.cpf_comprador = npgsqlDataReader.GetValue(ordinal).ToString();
                    //ordinal = npgsqlDataReader.GetOrdinal("cpf_comprador");
                    //this.pagamento.cpf_comprador = npgsqlDataReader.GetValue(ordinal).ToString();
                    ordinal = npgsqlDataReader.GetOrdinal("deb_cod_banco");
                    this.pagamento.cod_banco = npgsqlDataReader.GetValue(ordinal).ToString();
                    ordinal = npgsqlDataReader.GetOrdinal("deb_numero_agencia");
                    this.pagamento.agencia = npgsqlDataReader.GetValue(ordinal).ToString();
                    ordinal = npgsqlDataReader.GetOrdinal("deb_numero_conta");
                    this.pagamento.conta = npgsqlDataReader.GetValue(ordinal).ToString();
                    ordinal = npgsqlDataReader.GetOrdinal("deb_dig_verificador_conta");
                    this.pagamento.dig_conta = npgsqlDataReader.GetValue(ordinal).ToString();
                    this.list_pagamento.pagamentos.Add(this.pagamento);
                    this.pagamento = new Pagamento();
                    }
                    else
                    {                        
                        ordinal = npgsqlDataReader.GetOrdinal("id_pagamento");
                        string id_pagamento = npgsqlDataReader.GetValue(ordinal).ToString();
                        Erros erroCancelado = new Erros();
                        erroCancelado.classe = "CPagamentos104";
                        erroCancelado.funcao = "busca_pagamentos()";
                        erroCancelado.codErro = "66";
                        erroCancelado.rc = 66;
                        erroCancelado.descErro = "Pagamento " + id_pagamento + " foi Cancelado.";
                        this.logErrosTxt.gravaLogProcesso(erroCancelado);
                    }
                    flag = true;
                }
                if (!flag)
                {
                    this.erros.codErro = "9999";
                    this.erros.descErro = "NAO HA PAGAMENTOS REFERENTE AO REGISTRO RETORNO...";
                    this.erros.rc = 25;
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
