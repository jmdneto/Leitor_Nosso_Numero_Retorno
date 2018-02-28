using ASL_CodigosErros;
using ConnPostNpgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leitor_Nosso_Numero_Retorno;
using Leitor_Nosso_Numero_Retorno.Layouts;
using Leitor_Nosso_Numero_Retorno.Processos;
using Leitor_Nosso_Numero_Retorno.IO.Log;
using Leitor_Nosso_Numero_Retorno.BD.Inclusao;

namespace Leitor_Nosso_Numero_Retorno
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            CodigosErros codigosErros = new CodigosErros();
            LogErroTxt logErroTxt = new LogErroTxt();
            Conexao conexao = new Conexao();
            Erros erros = new Erros();
            Ler_Arquivo_Remessa ler_Arquivo_Remessa = new Ler_Arquivo_Remessa();
           

            int tipoAcessConn = 2;
            int result;

            try
            {
                if (args.Length == 0)
                {
                    erros.descErro = "CAMINHO DO ARQUIVO RETORNO BANCO NAO INFORMADO...";
                    Console.WriteLine("CAMINHO DO ARQUIVO RETORNO BANCO NAO INFORMADO...");
                    logErroTxt.gravaLogProcesso(erros);
                    result = 1;
                    return result;
                }
                string text = args[0];
                
                string[] array = text.Split(new char[]
                {
                    '\\'
                });
                ler_Arquivo_Remessa.PathArqRemessa = text;
                string nome_arquivo = array[2];
                Console.WriteLine("ARQUIVO LIDO: " + text);

                

                if (!ler_Arquivo_Remessa.lerArquivo(nome_arquivo))
                {
                    erros = ler_Arquivo_Remessa.erros;
                    logErroTxt.gravaLogProcesso(erros);
                    Console.WriteLine(erros.descErro);
                    result = 2;
                    return result;
                }
                if (!ler_Arquivo_Remessa.verifica_arq_processado(ref array[array.Length - 1]))
                {
                    erros = ler_Arquivo_Remessa.erros;
                    logErroTxt.gravaLogProcesso(erros);
                    Console.WriteLine(erros.descErro);
                    result = 2;
                    return result;
                }

                //Console.WriteLine("V003 - connPost.open(" + tipoAcessConn.ToString() + ", 1, true)");
                //if (!conexao.open(tipoAcessConn, 1, true))
                //{
                //    erros.descErro = "Erro na conexao:" + conexao.MsgErro;
                //    erros.rc = 5;
                //    // logErroTxt.gravaLogProcesso(erros);
                //    Console.WriteLine(erros.descErro);
                //    result = 3;
                //    return result;
                //}


                conexao.commitTransacao();
                conexao.close();
                Console.WriteLine("PROCESSAMENTO OK...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(erros.descErro);
                conexao.rollbackTransacao();
                conexao.close();
                result = 99;
                return result;
            }
            result = 0;
            return result;
        }
    }
}













//            try
//            {
//                Console.WriteLine("V003 - connPost.open(" + tipoAcessConn.ToString() + ", 1, true)");
//                if (!conexao.open(tipoAcessConn, 1, true))
//                {
//                    erros.descErro = "Erro na conexao:" + conexao.MsgErro;
//                    erros.rc = 5;
//                    // logErroTxt.gravaLogProcesso(erros);
//                    Console.WriteLine(erros.descErro);
//                    result = 3;
//                    return result;
//                }
                
//            }

//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//                Console.WriteLine(erros.descErro);
//                conexao.rollbackTransacao();
//                conexao.close();
//                result = 99;
//                return result;
//            }
//            result = 0;
//            return result;
//        }
//    }
//}
