using Leitor_Nosso_Numero_Retorno.BD.Conexao;
using Leitor_Nosso_Numero_Retorno.Layouts;
using ConnPostNpgsql;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leitor_Nosso_Numero_Retorno.BD.Inclusao
{

    internal class ILinhasRemessa : openPost
    {
        //DadosArquivoRemessa dados_linha_remessa = new DadosArquivoRemessa();
        DadosArquivoRemessa dados_arquivo_remessa = new DadosArquivoRemessa();

        public bool insereRegistroBD(ref ConnPostNpgsql.Conexao conexao, List<DadosArquivoRemessa> dadosarquivoremessa)
        {
            this.erros.classe = "ILinhasRemessa";
            this.erros.funcao = "insereRegistroBD";
            this.erros.descErro = "";
            NpgsqlCommand npgsqlCommand = new NpgsqlCommand();
            bool result;
            List<DadosArquivoRemessa> dados_arquivo_remessa = new List<DadosArquivoRemessa>();
         
            dados_arquivo_remessa = dadosarquivoremessa;
            try
            {
                int tipoAcessConn = 2;

                if (!conexao.open(tipoAcessConn, 1, true))
                {
                    erros.descErro = "Erro na conexao:" + conexao.MsgErro;
                    erros.rc = 5;
                    // logErroTxt.gravaLogProcesso(erros);
                    Console.WriteLine(erros.descErro);
                    result = false;
                    return result;
                }

                npgsqlCommand.Connection = conexao.ConexaoBd;
                if (conexao.transactionIsOpen())
                {
                    npgsqlCommand.Transaction = conexao.TransacaoBd;
                }
                foreach (DadosArquivoRemessa a in dados_arquivo_remessa)
                {
                    string nome = a.nome_arquivo;
                    string nosso = a.nosso_numero;
                    string lin = a.linha;
                    //var a = dados_arquivo_remessa.G
                    //this.dados_arquivo_remessa.nome_arquivo = dados_arquivo_remessa.

                    //string commandText = "INSERT INTO tb_dados_remessas_boletos () / SELECT :data_gravacao_historico,cpf_comprador,cod_banco,numero_agencia,numero_conta,dig_verificador_conta,id_status,data_alteracao,desc_hist  FROM tb_cadastro_cc_debito_automatico WHERE cpf_comprador = :cpf_comprador AND cod_banco = :cod_banco AND numero_agencia = :numero_agencia AND numero_conta = :numero_conta AND dig_verificador_conta = :dig_verificador_conta";
                    string commandText = "INSERT INTO tb_dados_remessas_boletos (nome_arquivo,nosso_numero,linha) values (:nome_arquivo,:nosso_numero,:linha)";
                    npgsqlCommand.CommandText = commandText;
                    
                    openPost.setParameters(npgsqlCommand, ":nome_arquivo", nome);
                    openPost.setParameters(npgsqlCommand, ":nosso_numero", nosso);
                    openPost.setParameters(npgsqlCommand, ":linha", lin);
                    //openPost.setParameters(npgsqlCommand, ":dig_verificador_conta", this.pagamento.dig_conta);
                    //openPost.setParameters(npgsqlCommand, ":data_gravacao_historico", this.pagamento.data_alteracaoHist);
                    npgsqlCommand.ExecuteNonQuery();
                    
                }
                result = true;
                npgsqlCommand.Dispose();
                conexao.commitTransacao();
                conexao.Dispose();

            }
            catch (Exception ex)
            {
                this.erros.rc = 4;
                this.erros.descErro = "EXCEPTION. MSG.:" + ex.Message;
                npgsqlCommand.Dispose();
                conexao.rollbackTransacao();
                conexao.Dispose();
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
