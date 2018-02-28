using Leitor_Nosso_Numero_Retorno.BD.Inclusao;
using Leitor_Nosso_Numero_Retorno.BD.Conexao;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leitor_Nosso_Numero_Retorno.Layouts;
using ConnPostNpgsql;

namespace Leitor_Nosso_Numero_Retorno.Processos
{
    internal class Ler_Arquivo_Remessa:openPost
    {
        public string PathArqRemessa = "";
        public string[] arquivoRemessa;
        public string nomeArq = "";
        public string PathLog = "";
        public string[] arquivos;
        public string nomeArqProcessados;
        public string[] registros;

        public bool lerArquivo(string nome_arquivo)
        {
            this.erros.funcao = "Ler_Arquivo";
            this.erros.funcao = "lerarquivo";
            this.erros.descErro = "";
            bool result;
            string nomearquivo = "";
            string nossonumero = "";
            string linha = "";

            List<DadosArquivoRemessa> dadosarquivoremessa = new List<DadosArquivoRemessa>();
            Conexao conexao = new Conexao();
            ILinhasRemessa incluir_Arquivo_Remessa = new ILinhasRemessa();
            try
            {
                this.arquivoRemessa = File.ReadAllLines(this.PathArqRemessa);

                if (this.arquivoRemessa.GetLength(0) == 0)
                {
                    this.erros.descErro = "NAO EXISTE ARQUIVO PARA PROCESSAMENTO EM: " + this.PathArqRemessa;
                    this.erros.rc = 5;
                    result = false;
                    return result;
                }
                for (int i = 0;i < arquivoRemessa.Length; i ++)
                {
                    int tipolinha;
                    tipolinha = Convert.ToInt32(arquivoRemessa[i].Substring(0, 1));
                    if (tipolinha == 1)
                    {
                        nomearquivo = nome_arquivo;
                        nossonumero = arquivoRemessa[i].Substring(59, 11);
                        linha = arquivoRemessa[i].Substring(395, 5);

                        DadosArquivoRemessa dadoslinha = new DadosArquivoRemessa();
                        dadoslinha.nome_arquivo = nomearquivo;
                        dadoslinha.nosso_numero = nossonumero;
                        dadoslinha.linha = linha;

                        //List<DadosArquivoRemessa> dadosarquivoremessa = new List<DadosArquivoRemessa>();
                        dadosarquivoremessa.Add(dadoslinha);

                        //if (!ILinhasRemessa.insereRegistroBD(nomearquivo, nossonumero, linha))
                        //{

                        //}
                    }
                    else
                    {
                        if (tipolinha == 0)
                        {
                            continue;
                        }
                        if(tipolinha == 9)
                        {
                            if (dadosarquivoremessa == null)
                            {
                                Console.WriteLine("acabou");
                            }
                            else
                            {
                                Console.WriteLine("escrever no banco");

                                if (!incluir_Arquivo_Remessa.insereRegistroBD(ref conexao, dadosarquivoremessa))
                                {


                                }

                                return true;
                            }
                        }
                    }
                }

                
               string teste =  arquivoRemessa[1].Substring(0, 1);
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
            this.erros.funcao = "Ler_Arquivo_Remessa";
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
