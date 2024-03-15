using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateScriptDatabase.Template
{
    public class tipoDato
    {
        public String Convertir(string val)
        {
            string convertido = "";
            convertido = val.Substring(0, 1).ToUpper() + val.Substring(1);
            return convertido;
        }


        public String ConvertirTipoGet(string tipo)
        {
            string convertido = "";

            switch (tipo)
            {
                //case "char":
                //    convertido = "GetString";
                case "datetime":
                    convertido = "GetDateTime";
                    break;
                case "float":
                    convertido = "GetDouble";
                    break;
                case "int":
                    convertido = "GetInt32";
                    break;
                case "varchar":
                    convertido = "GetString";
                    break;
                default:
                    convertido = "GetString";
                    break;
            }
            return convertido;
        }

        public String ConvertirTipo(string tipo)
        {
            string convertido = "";

            switch (tipo)
            {
                case "char":
                    convertido = "string";
                    break;
                case "datetime":
                    convertido = "DateTime";
                    break;
                case "float":
                    convertido = "double";
                    break;
                case "int":
                    convertido = "int";
                    break;
                case "varchar":
                    convertido = "string";
                    break;
            }
            return convertido;
        }

        public String ConvertirTipoSQL(string tipo)
        {
            string convertido = "";

            switch (tipo)
            {
                case "char":
                    convertido = "Char";
                    break;
                case "datetime":
                    convertido = "DateTime";
                    break;
                case "float":
                    convertido = "Float";
                    break;
                case "int":
                    convertido = "Int";
                    break;
                case "varchar":
                    convertido = "VarChar";
                    break;
            }
            return convertido;
        }
    }
}
