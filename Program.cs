using System.Data;
using geradorDeClasses;
using geradorDeClasses.Conexao;
using MySql.Data.MySqlClient;

Mysql con = new Mysql();
Util util = new Util();

Console.WriteLine("Qual o banco de dados que quer utilizar?");
var bancoDados = Console.ReadLine()!;

Console.WriteLine("Qual a tabela?");
var tabela = Console.ReadLine()!;

var linhas = con.retornaSelect(bancoDados!, tabela!)!;

var sw = util.devolverArquivo(bancoDados!, tabela!);

tabela = tabela.Substring(tabela.IndexOf("_") + 1, tabela.Length - (tabela.IndexOf("_") + 1));

var pk = util.devolvePK(linhas);

util.gerandoClasseBase(ref sw, linhas, tabela);
util.gerandoDetalhe(ref sw, linhas, tabela, pk);
util.gerandoDelete(ref sw, tabela, pk);
util.gerandoGerarCodigo(ref sw, tabela, pk);
util.gerandoInsert(ref sw, linhas, tabela, pk);

sw.Close();
Console.WriteLine("Fim");
Console.Read();