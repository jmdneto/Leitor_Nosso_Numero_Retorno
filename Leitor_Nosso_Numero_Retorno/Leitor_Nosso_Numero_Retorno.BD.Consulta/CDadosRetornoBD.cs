using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leitor_Nosso_Numero_Retorno.BD.Conexao;
using Leitor_Nosso_Numero_Retorno.Leitor_Nosso_Numero_Retorno.Layouts;
using Npgsql;

namespace Leitor_Nosso_Numero_Retorno.Leitor_Nosso_Numero_Retorno.BD.Consulta
{
    internal class CDadosRetornoBD : openPost
    {
        public DadosRetornoBD dadosretornobd = new DadosRetornoBD();
        public bool buscaDadosRetornoBD(ref ConnPostNpgsql.Conexao conexao)
        {
            this.erros.classe = "CDadosRetornoBD";
            this.erros.funcao = "buscaDadosRetornoBD()";
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
                string commandText = "SELECT id_retorno, arquivo, numero_linha,registro_aceito from temp_retorno_boletos WHERE id_imobiliaria = :id_imobiliaria";
                npgsqlCommand.CommandText = commandText;
                openPost.setParameters(npgsqlCommand, ":id_retorno", dadosretornobd.id_retorno);
                npgsqlDataReader = npgsqlCommand.ExecuteReader();
                while (npgsqlDataReader.Read())
                {
                    int ordinal = npgsqlDataReader.GetOrdinal("id_retorno");
                    this.dadosretornobd.id_retorno = Convert.ToInt32(npgsqlDataReader.GetValue(ordinal).ToString());

                    ordinal = npgsqlDataReader.GetOrdinal("arquivo");
                    this.dadosretornobd.nome_arquivo = npgsqlDataReader.GetValue(ordinal).ToString();

                    ordinal = npgsqlDataReader.GetOrdinal("numero_linha");
                    this.dadosretornobd.numero_linha = npgsqlDataReader.GetValue(ordinal).ToString();

                    ordinal = npgsqlDataReader.GetOrdinal("registro_aceito");
                    this.dadosretornobd.registro_aceito = npgsqlDataReader.GetValue(ordinal).ToString();

                    //ordinal = npgsqlDataReader.GetOrdinal("taxa_de_servico_cnpj");
                    //this.imobiliaria.taxa_de_servico_cnpj = Convert.ToDouble(npgsqlDataReader.GetValue(ordinal).ToString());


                    flag = true;
                }
                if (!flag)
                {
                    this.erros.descErro = "Não há resultados a serem exibidos"; //"NAO HA IMOBILIARIAS COM ID " + imobiliaria.id_imobiliaria;
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
