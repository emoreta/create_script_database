using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateScriptDatabase.Template
{
    public class GetSetModelEntity
    {
        public static string pathScripts = System.Configuration.ConfigurationManager.AppSettings["rutaScripts"] ?? "Not Found";
        private static string ConvertirPrimeraLetraEnMayuscula(string texto)
        {
            string str = "";
            str = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(texto);

            return str;
        }
        private static string limpiaGuion(string texto)
        {
            string str = "";
            str = texto.Replace("_","");

            return str;
        }
        public String entityClass(DataSet ds, String table)
        {
            StringBuilder sb = new StringBuilder();
            DataTable dt = ds.Tables[0];
            int flagFive = 1;

            /*cls_sql sql = new cls_sql();

            List<String> primary = new List<string>();
            primary = sql.obteniendoPrimaryKey(table, conexion);
            primaryString = primary[0];*/


            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Web;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
            sb.AppendLine("namespace " + "arnuv_api.Models");
            sb.AppendLine("{");
            sb.AppendLine("[Table(\""+ table.ToLower() + "\")]");
            sb.AppendLine("public class " + limpiaGuion(table));
            sb.AppendLine("{");


            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {

                    string column = Convert.ToString(row["COLUMN_NAME"]);
                    string datatype = Convert.ToString(row["DATA_TYPE"]);
                    string maxlenght = Convert.ToString(row["CHARACTER_MAXIMUM_LENGTH"]);
                    string numericPrecision = Convert.ToString(row["NUMERIC_PRECISION"]);
                    string numericScale = Convert.ToString(row["NUMERIC_SCALE"]);
                    


                    if (datatype == "int" || datatype == "smallint")
                    {
                        sb.AppendLine("[Column(\""+ column + "\")]");
                        sb.AppendLine("public int " + limpiaGuion(column) + "  {get ; set; }");
                    }
                    else if (datatype == "char")
                    {
                        sb.AppendLine("[Column(\"" + column + "\")]");
                        sb.AppendLine("public char " + limpiaGuion(column) + "  {get ; set; }");
                    }
                    else if (datatype == "bigint")
                    {
                        sb.AppendLine("[Column(\"" + column + "\")]");
                        sb.AppendLine("public Int64 " + limpiaGuion(column) + "  {get ; set; }");
                    }
                    else if (datatype == "varchar")
                    {
                        sb.AppendLine("[Column(\"" + column + "\")]");
                        sb.AppendLine("public string " + limpiaGuion(column) + "  {get ; set; }");
                    }
                    else if (datatype == "money" || datatype == "numeric" || datatype == "decimal")
                    {
                        sb.AppendLine("[Column(\"" + column + "\")]");
                        sb.AppendLine("public decimal " + limpiaGuion(column) + "  {get ; set; }");
                    }
                    else if (datatype == "datetime" || datatype == "smalldatetime")
                    {
                        sb.AppendLine("[Column(\"" + column + "\")]");
                        sb.AppendLine("public DateTime " + limpiaGuion(column) + "  {get ; set; }");
                    }
                    else if (datatype == "date")
                    {
                        sb.AppendLine("[Column(\"" + column + "\")]");
                        sb.AppendLine("public Date" + limpiaGuion(column) + "  {get ; set; }");
                    }
                    else if (datatype == "bit")
                    {

                        sb.AppendLine("[Column(\"" + column + "\")]");
                        sb.AppendLine("public Boolean " + limpiaGuion(column) + "  {get ; set; }");

                    }
                    flagFive++;

                }
            }
            sb.AppendLine("}");
            sb.AppendLine("}");
            createEntity(sb.ToString(), table);

            return null;
        }
        public void createEntity(String structura, String nombre)
        {

            System.IO.File.WriteAllText(pathScripts + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(nombre)) + ".cs", structura);
        }
    }
}
