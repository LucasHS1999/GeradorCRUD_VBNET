using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace geradorDeClasses.Conexao
{
    public class Mysql
    {
        public MySqlConnection conexao(string bancoDados)
        {

            return new MySqlConnection("server=ddsinf0.ddns.com.br;port=3307;user id=ddsinfo; password=dds21231; database=db_murici; SslMode=none");
        }

        public DataRow[] retornaSelect(string bancoDados, string tabela)
        {
            var con = conexao(bancoDados);
            con.Open();           

            MySqlCommand cmd = new MySqlCommand("desc " + tabela, con);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            DataRow[] rows = dt.Select();

            con.Close();

            return rows;
        }
    }
}