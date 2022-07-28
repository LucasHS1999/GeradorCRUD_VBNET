using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace geradorDeClasses
{
    public class Util
    {
        public string tipoPropriedade(string Tipo)
        {
            switch (Tipo.ToLower())
            {
                case "int":

                    return "Int32";

                case "varchar":

                    return "String";

                case "char":

                    return "Boolean";

                case "text":

                    return "String";

                case "decimal":

                    return "Decimal";

                case "bigint":

                    return "Int64";

                case "double":

                    return "Double";

                case "smallint":

                    return "Int16";

                case "tinyint":

                    return "Boolean";

                default:
                    return "";

            }
        }

        public StreamWriter devolverArquivo(string bancoDados, string tabela)
        {
            var diretorio = Directory.GetCurrentDirectory() + "\\ClassesProntas" + "\\" + bancoDados;

            if (!Directory.Exists(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }
            else
            {
                Directory.Delete(diretorio, true);
                Directory.CreateDirectory(diretorio);
            }

            var caminhoArquivo = diretorio + "\\" + tabela + ".vb";

            StreamWriter sw = new StreamWriter(caminhoArquivo, true);

            return sw;
        }

        public string tipoDefault(string Padrao)
        {
            switch (Padrao)
            {
                case "Int32":

                    return "0";

                case "String":

                    return "\"\"";

                case "Boolean":

                    return "False";

                case "Decimal":

                    return "0";

                case "Double":

                    return "0";

                case "Int16":

                    return "0";

                case "Int64":

                    return "0";

                default:

                    return "\"\"";
            }
        }

        public void gerandoClasseBase(ref StreamWriter sw, DataRow[] linhas, string tabela)
        {
            sw.WriteLine("Public Class Cls_" + tabela.ToLower());
            sw.WriteLine();
            foreach (DataRow linha in linhas)
            {
                var tipos = linha["Type"].ToString()!;
                var ate = tipos.IndexOf("(") > 0 ? tipos.IndexOf("(") : tipos.Length;
                tipos = tipoPropriedade(tipos.Substring(0, ate));
                var padrao = tipoDefault(tipos);

                sw.WriteLine("Public Property " + linha["Field"].ToString()!.ToUpper() + " As " + tipos + " = " + padrao);
            }
            sw.WriteLine();
            sw.WriteLine("End Class");
            sw.WriteLine();
            sw.WriteLine();
        }

        public string devolvePK(DataRow[] linhas)
        {
            var pk = "";

            foreach (DataRow linha in linhas)
            {
                if (linha["Key"].ToString()!.ToUpper() == "PRI")
                {
                    pk = linha["Field"].ToString()!.ToUpper();
                }
            }

            return pk;
        }

        public void gerandoDetalhe(ref StreamWriter sw, DataRow[] linhas, string tabela, string pk)
        {
            sw.WriteLine("Public Function Detalhe" + tabela.ToLower() + "(" + pk.ToLower() + " As Int32) As Cls_" + tabela);
            sw.WriteLine("Dim " + tabela + " As New Cls_" + tabela);
            sw.WriteLine();
            sw.WriteLine("Mdi_Wms.ConexaoGeral.Comando.Parameters.Clear()");
            sw.WriteLine("Mdi_Wms.ConexaoGeral.Comando.Parameters.AddWithValue(\"@" + pk.ToLower() + "\", " + pk.ToLower() + ")");
            sw.WriteLine();
            sw.WriteLine("Dim sql = \"SELECT\"");

            foreach (DataRow linha in linhas)
            {
                sw.WriteLine("sql += \", " + linha["Field"].ToString()! + "\"");
            }
            sw.WriteLine("sql += \" FROM TB_" + tabela.ToUpper() + "\"");
            sw.WriteLine("sql += \" WHERE \"");
            sw.WriteLine("sql += \" " + pk + " = @" + pk.ToLower() + "\"");
            sw.WriteLine();
            sw.WriteLine("Mdi_Wms.ConexaoGeral.Comando.CommandText = sql");
            sw.WriteLine("Dim dt = Mdi_Wms.ConexaoGeral.ExecutaComandoDataTable");
            sw.WriteLine();
            sw.WriteLine("If dt.Rows.Count > 0 Then");

            foreach (DataRow linha in linhas)
            {
                var tipos = linha["Type"].ToString()!;
                var ate = tipos.IndexOf("(") > 0 ? tipos.IndexOf("(") : tipos.Length;
                tipos = tipoPropriedade(tipos.Substring(0, ate));
                var padrao = tipoDefault(tipos);

                sw.WriteLine(tabela + "." + linha["Field"].ToString()!.ToUpper() + " = dt.Rows(0).Item(\"" + linha["Field"].ToString()!.ToUpper() + "\")");
            }

            sw.WriteLine("End If");
            sw.WriteLine();
            sw.WriteLine("End Function");
            sw.WriteLine();
            sw.WriteLine();
        }

        public void gerandoDelete(ref StreamWriter sw, string tabela, string pk)
        {
            sw.WriteLine("Public Function Deleta" + tabela.ToLower() + "(" + pk.ToLower() + " As Int32) As Boolean");
            sw.WriteLine();
            sw.WriteLine("Mdi_Wms.ConexaoGeral.Comando.Parameters.Clear()");
            sw.WriteLine("Mdi_Wms.ConexaoGeral.Comando.Parameters.AddWithValue(\"@" + pk.ToLower() + "\", " + pk.ToLower() + ")");
            sw.WriteLine();
            sw.WriteLine("Dim sql = \" DELETE FROM TB_" + tabela.ToUpper() + " WHERE " + pk + " = @" + pk.ToLower() + "\"");
            sw.WriteLine();
            sw.WriteLine("Mdi_Wms.ConexaoGeral.Comando.CommandText = sql");
            sw.WriteLine("Dim dt = Mdi_Wms.ConexaoGeral.ExecutaComandoDataTable");
            sw.WriteLine();
            sw.WriteLine("Return True");
            sw.WriteLine();
            sw.WriteLine("End Function");
            sw.WriteLine();
            sw.WriteLine();
        }

        public void gerandoGerarCodigo(ref StreamWriter sw, string tabela, string pk)
        {
            sw.WriteLine("Public Function GeraCodigo() As Int32");
            sw.WriteLine();
            sw.WriteLine("Dim sql = \" SELECT (IFNULL(MAX(" + pk + "), 0) + 1) AS NOVO_CODIGO FROM TB_" + tabela.ToUpper() + "\"");
            sw.WriteLine();
            sw.WriteLine("Mdi_Wms.ConexaoGeral.Comando.CommandText = sql");
            sw.WriteLine("Dim dt = Mdi_Wms.ConexaoGeral.ExecutaComandoDataTable");
            sw.WriteLine();
            sw.WriteLine("Return dt.Rows(0).Item(\"NOVO_CODIGO\")");
            sw.WriteLine();
            sw.WriteLine("End Function");
            sw.WriteLine();
            sw.WriteLine();
        }

        public void gerandoInsert(ref StreamWriter sw, DataRow[] linhas, string tabela, string pk)
        {
            sw.WriteLine("Public Function Insere" + tabela + "() As Boolean");
            sw.WriteLine();
            sw.WriteLine("Mdi_Wms.ConexaoGeral.Comando.Parameters.Clear()");
            foreach (DataRow linha in linhas)
            {
                sw.WriteLine("Mdi_Wms.ConexaoGeral.Comando.Parameters.AddWithValue(\"@" + linha["Field"].ToString()!.ToLower() + "\", " + tabela.ToLower() + "." + linha["Field"].ToString()!.ToUpper() + ")");
            }
            sw.WriteLine();
            sw.WriteLine("Dim sql = \"INSERT INTO" + tabela + "\"");
            sw.WriteLine("sql += \"(\"");
            foreach (DataRow linha in linhas)
            {
                sw.WriteLine("sql += \", " + linha["Field"].ToString()! + "\"");
            }
            sw.WriteLine("sql += \")\"");
            sw.WriteLine("sql += \"VALUES\"");
            sw.WriteLine("sql += \"(\"");
            foreach (DataRow linha in linhas)
            {
                sw.WriteLine("sql += \", @" + linha["Field"].ToString()!.ToLower() + "\"");
            }
            sw.WriteLine("sql += \")\"");

            sw.WriteLine("Return True");
            sw.WriteLine();
            sw.WriteLine("End Function");
            sw.WriteLine();
            sw.WriteLine();
        }

    }
}