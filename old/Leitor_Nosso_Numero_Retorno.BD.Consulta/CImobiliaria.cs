using Npgsql;
using Retorno_Debito_Automatico.BD.Conexao;
using Retorno_Debito_Automatico.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Retorno_Debito_Automatico.BD.Consulta
{
    internal class CImobiliaria : openPost
    {
        public Imobiliaria imobiliaria = new Imobiliaria();
        public bool buscaImobiliaria(ref ConnPostNpgsql.Conexao conexao)
        {
            this.erros.classe = "CImobiliaria";
            this.erros.funcao = "buscaImobiliaria()";
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
                string commandText = "SELECT percentual_banco, taxa_de_servico_cnpj from tb_imobiliarias WHERE id_imobiliaria = :id_imobiliaria";
                npgsqlCommand.CommandText = commandText;
                openPost.setParameters(npgsqlCommand, ":id_imobiliaria", imobiliaria.id_imobiliaria);
                npgsqlDataReader = npgsqlCommand.ExecuteReader();
                while (npgsqlDataReader.Read())
                {
                    int ordinal = npgsqlDataReader.GetOrdinal("percentual_banco");
                    this.imobiliaria.percentual_banco = Convert.ToDouble(npgsqlDataReader.GetValue(ordinal).ToString());

                    ordinal = npgsqlDataReader.GetOrdinal("taxa_de_servico_cnpj");
                    this.imobiliaria.taxa_de_servico_cnpj = Convert.ToDouble(npgsqlDataReader.GetValue(ordinal).ToString());
                    flag = true;
                }
                if (!flag)
                {
                    this.erros.descErro = "NAO HA IMOBILIARIAS COM ID " + imobiliaria.id_imobiliaria;
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
