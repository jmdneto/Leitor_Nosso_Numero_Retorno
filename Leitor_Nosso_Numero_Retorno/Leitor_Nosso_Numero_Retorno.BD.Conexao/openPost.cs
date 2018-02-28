using Npgsql;
using Leitor_Nosso_Numero_Retorno.Layouts;
using System;
namespace Leitor_Nosso_Numero_Retorno.BD.Conexao
{
    public class openPost
    {
        public Erros erros = new Erros();
        public static NpgsqlCommand getCommand(string sql, NpgsqlConnection connection)
        {
            return new NpgsqlCommand(sql, connection);
        }
        public static void setParameters(NpgsqlCommand command, string parameter, string value)
        {
            NpgsqlParameter npgsqlParameter = new NpgsqlParameter();
            npgsqlParameter.ParameterName = parameter;
            if (value == null)
            {
                npgsqlParameter.Value = DBNull.Value;
            }
            else
            {
                npgsqlParameter.Value = value;
            }
            command.Parameters.Add(npgsqlParameter);
        }
        public static void setParameters(NpgsqlCommand command, string parameter, int value)
        {
            NpgsqlParameter npgsqlParameter = new NpgsqlParameter();
            npgsqlParameter.ParameterName = parameter;
            bool flag = 0 == 0;
            npgsqlParameter.Value = value;
            command.Parameters.Add(npgsqlParameter);
        }
        public static void setParameters(NpgsqlCommand command, string parameter, long value)
        {
            NpgsqlParameter npgsqlParameter = new NpgsqlParameter();
            npgsqlParameter.ParameterName = parameter;
            bool flag = 0 == 0;
            npgsqlParameter.Value = value;
            command.Parameters.Add(npgsqlParameter);
        }
        public static void setParameters(NpgsqlCommand command, string parameter, object value)
        {
            NpgsqlParameter npgsqlParameter = new NpgsqlParameter();
            npgsqlParameter.ParameterName = parameter;
            if (value == null)
            {
                npgsqlParameter.Value = DBNull.Value;
            }
            else
            {
                npgsqlParameter.Value = value;
            }
            command.Parameters.Add(npgsqlParameter);
        }
    }
}
