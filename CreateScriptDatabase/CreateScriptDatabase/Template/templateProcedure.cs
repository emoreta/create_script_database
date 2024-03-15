using CreateScriptDatabase.Class;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateScriptDatabase.Template
{
    class templateProcedure
    {
        public static string pathScripts = System.Configuration.ConfigurationManager.AppSettings["rutaScripts"] ?? "Not Found";
        public static string conectionDestino = System.Configuration.ConfigurationManager.AppSettings["conectionDestino"] ?? "Not Found";

        public static string nombreProyectoEntity = System.Configuration.ConfigurationManager.AppSettings["nombreProyectoEntity"] ?? "Not Found";
        public static string nombreProyectoProcedureClass = System.Configuration.ConfigurationManager.AppSettings["nombreProyectoProcedureClass"] ?? "Not Found";

        private static string ConvertirPrimeraLetraEnMayuscula(string texto)
        {
            string str = "";
            str = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(texto);

            return str;
        }
        public String entityClass(DataSet ds, String table)
        {
            StringBuilder sb = new StringBuilder();
            DataTable dt = ds.Tables[0];
            int flagFive = 1;


            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Web;");
            sb.AppendLine("namespace "+ nombreProyectoEntity);
            sb.AppendLine("{");
            sb.AppendLine("public class "+ table );
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


                    if (datatype == "int")
                    {
                        sb.AppendLine("int " + column + ";");

                        sb.AppendLine("public int " + ConvertirPrimeraLetraEnMayuscula(column) + "  {get => " + column + "; set => " + column + " = value;}");
                    }
                    else if (datatype == "varchar")
                    {
                        sb.AppendLine("String " + column + ";");
                        sb.AppendLine("public string " + ConvertirPrimeraLetraEnMayuscula(column) + "  {get => " + column + "; set => " + column + " = value;}");
                    }
                    else if (datatype == "money" || datatype == "numeric")
                    {
                        sb.AppendLine("double " + column + ";");
                        sb.AppendLine("public double " + ConvertirPrimeraLetraEnMayuscula(column) + "  {get => " + column + "; set => " + column + " = value;}");
                    }
                    else if (datatype == "datetime" || datatype == "smalldatetime")
                    {
                        sb.AppendLine("DateTime " + column + ";");
                        sb.AppendLine("public DateTime " + ConvertirPrimeraLetraEnMayuscula(column) + "  {get => " + column + "; set => " + column + " = value;}");
                    }
                    else if (datatype == "bit")
                    {
                        
                        sb.AppendLine("Boolean " + column + ";");
                        sb.AppendLine("public Boolean " + ConvertirPrimeraLetraEnMayuscula(column) + "  {get => " + column + "; set => " + column + " = value;}");

                    }
                    flagFive++;

                }
            }
            sb.AppendLine("}");
            sb.AppendLine("}");
            createFile(sb.ToString(), table);

            return null;
        }
        public String methodSelectAppCred(DataSet ds, String table, String prefijoClass, String conexion)
        {
            DataTable dt = ds.Tables[0];
            int flag = 1;
            String columnCadena="";
            cls_sql sql = new cls_sql();
            String primaryString = "";

            List<String> primary = new List<string>();
            primary = sql.obteniendoPrimaryKey(table, conexion);
            primaryString = primary[0];

            StringBuilder sb = new StringBuilder();
         
            sb.AppendLine("public DataSet " + prefijoClass + "_" + table + "(int Id)");
            sb.AppendLine("  {");
            sb.AppendLine("   var ds = new DataSet();");
            sb.AppendLine("   try");
            sb.AppendLine("   {");
            sb.AppendLine("   Con().Open();");
            sb.Append("   SqlCommand cmd = new SqlCommand(\"");
            sb.Append("select ");

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {

                    string column = Convert.ToString(row["COLUMN_NAME"]);
                    //columnCadena = column;
                    if (flag== ds.Tables[0].Rows.Count)
                    {
                        columnCadena += column;
                    }
                    else { 
                    columnCadena += column + ",";
                    }

                    flag++;

                }
                sb.Append(columnCadena);
                sb.AppendLine(" from "+ table+ " where "+ primaryString+ "= \" +Id +\"\" , con);");

            }
            sb.AppendLine("   var dataAdapter = new SqlDataAdapter(cmd);");
            sb.AppendLine("   var commandBuilder = new SqlCommandBuilder(dataAdapter);");

            sb.AppendLine("   Con().Close();");
            sb.AppendLine("   dataAdapter.Fill(ds);");
            sb.AppendLine("    }");
            sb.AppendLine("    catch (Exception ex)");
            sb.AppendLine("   {");
            sb.AppendLine("   Con().Close();");
            sb.AppendLine("   }");
            sb.AppendLine("   finally");
            sb.AppendLine("   {");
            sb.AppendLine("   Con().Close();");
            sb.AppendLine("   }");
            sb.AppendLine("   return ds;");
            sb.AppendLine("}");


            createQueryAppCred(sb.ToString(), table);

            return null;
        }
        public String methodCallServiceAppCred(DataSet ds,String table, String prefijoClass)
        {
            DataTable dt = ds.Tables[0];
            int flag = 1;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("public void " + prefijoClass + "_" + table + "(int Id)");
            sb.AppendLine("  {");
            sb.AppendLine("  SRInfoCliente.PublicRequestClient sp = new SRInfoCliente.PublicRequestClient();");
            sb.AppendLine("   DataSet ds = new DataSet();");
            sb.AppendLine("   cls_sql sqlQuery = new cls_sql();");
            sb.AppendLine("   ds = sqlQuery." + prefijoClass + "_" + table + "(Id);");
            sb.AppendLine("   if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)");
            sb.AppendLine("   {");
            sb.AppendLine("   foreach (DataRow renglon in ds.Tables[0].Rows)");
            sb.AppendLine("   {");
            sb.AppendLine("   SRInfoCliente." + table + " ent=new SRInfoCliente." + table + "();");
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {

                    string column = Convert.ToString(row["COLUMN_NAME"]);
                    

                    string datatype = Convert.ToString(row["DATA_TYPE"]);
                    string maxlenght = Convert.ToString(row["CHARACTER_MAXIMUM_LENGTH"]);
                    string numericPrecision = Convert.ToString(row["NUMERIC_PRECISION"]);
                    string numericScale = Convert.ToString(row["NUMERIC_SCALE"]);

                    //sb.AppendLine("   ent.Id1 = Convert.ToInt32(renglon["Id"].ToString());");
                    if (datatype == "int")
                    {
                      
                        sb.AppendLine("   ent."+ ConvertirPrimeraLetraEnMayuscula(column) +" = Convert.ToInt32(renglon[\""+ column + "\"].ToString());");
                    }
                    else if (datatype == "varchar")
                    {
                        sb.AppendLine("   ent." + ConvertirPrimeraLetraEnMayuscula(column) + " = renglon[\"" + column + "\"].ToString();");
                    }
                    else if (datatype == "money" || datatype == "numeric")
                    {
                        sb.AppendLine("   ent." + ConvertirPrimeraLetraEnMayuscula(column) + " = Convert.ToDouble(renglon[\"" + column + "\"].ToString());");
                    }
                    else if (datatype == "datetime" || datatype == "smalldatetime")
                    {
                        sb.AppendLine("   ent." + ConvertirPrimeraLetraEnMayuscula(column) + " = Convert.ToDateTime(renglon[\"" + column + "\"].ToString());");
                    }
                    else if (datatype == "bit")
                    {
                        sb.AppendLine("   ent." + ConvertirPrimeraLetraEnMayuscula(column) + " = Convert.ToBoolean(renglon[\"" + column + "\"].ToString());");

                    }

                    

                }
            }
            sb.AppendLine("   sp." + prefijoClass + "_" + table + "(ent);");
            sb.AppendLine("   }");

            sb.AppendLine(" }");

            createCallServiceAppCred(sb.ToString(), table);

            return null;
        }
        public String methodAppCred(String table, String prefijoClass)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("public String "+ prefijoClass + "_"+ table+"("+ table + " entityClass)");
            sb.AppendLine("  {");
            sb.AppendLine("   return ssr." + prefijoClass + "_" + table + "(entityClass);");
            sb.AppendLine(" }");
            createFileSvc(sb.ToString(), table);

            return null;
        }
        public String methodSvc(String table, String prefijoClass)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("public async Task<String> " + prefijoClass + "_" + table + "(" + table+" entityClass)");
            sb.AppendLine("  {");
            sb.AppendLine("   return await Task.Run(() =>");
            sb.AppendLine("   {");
            sb.AppendLine("     IntegratorSolicitud i = new IntegratorSolicitud();");
            sb.AppendLine("     return i."+ prefijoClass + "_" + table + "(entityClass);");
            sb.AppendLine("   }");
            sb.AppendLine("   );");
            sb.AppendLine(" }");
            createFileSvc(sb.ToString(), table);

            return null;
        }
        public String methodInterface(String table, String prefijoClass)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[OperationContract]");
            sb.AppendLine("[WebInvoke(UriTemplate = \"/private/"+ prefijoClass + "_" + table +"\",");
            sb.AppendLine("RequestFormat = WebMessageFormat.Json,");
            sb.AppendLine("ResponseFormat = WebMessageFormat.Json, Method = \"POST\")]");
            sb.AppendLine("Task<String> " + prefijoClass + "_" + table + "(" + table + " entityClass);");

            createFileInterface(sb.ToString(), table);

            return null;
        }
        public String procedureClase(DataSet ds,String table,String prefijoClass, String prefijoProcedure)
        {
            StringBuilder sb = new StringBuilder();
            DataTable dt = ds.Tables[0];
            int flag = 1;

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Web;");
            sb.AppendLine("using DllSQLHelper;");
            sb.AppendLine("using System.Data;");
            sb.AppendLine("using System.Data.SqlClient;");
            sb.AppendLine("namespace "+ nombreProyectoEntity);
            sb.AppendLine("{ ");
            sb.AppendLine("public class "+ nombreProyectoProcedureClass);
            sb.AppendLine("{");

            sb.AppendLine("public String "+prefijoClass+"_"+ table+ "("+table +" entityClass)");
            sb.AppendLine("  {");
            sb.AppendLine("    String mensaje = \"\";");
            sb.AppendLine("    DataSet ds;");
            sb.AppendLine("    log.Info(\""+ table + "\");");
            sb.AppendLine("    try");
            sb.AppendLine("     { ");
            sb.AppendLine("       SqlHelper SQLH = new SqlHelper("+ conectionDestino + ");");
            sb.AppendLine("       SqlParameter[] parametros = new SqlParameter[] {");

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {

                    string column = Convert.ToString(row["COLUMN_NAME"]);

                    if(flag== ds.Tables[0].Rows.Count)
                    { 
                        sb.AppendLine("         new SqlParameter(\"@" + column + "\", entityClass." + ConvertirPrimeraLetraEnMayuscula(column) + ")");
                    }
                    else
                    {
                        sb.AppendLine("         new SqlParameter(\"@" + column + "\", entityClass." + ConvertirPrimeraLetraEnMayuscula(column) + "),");
                    }

                    flag++;
                }
            }
            sb.AppendLine("      };");
            sb.AppendLine("      ds = SQLH.Procredit_ExecuteDataset(CommandType.StoredProcedure, \"[dbo].["+prefijoProcedure+"_"+ table+"]\", parametros);");
            sb.AppendLine("      if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)");
            sb.AppendLine("         {");
            sb.AppendLine("           foreach (DataRow row in ds.Tables[0].Rows)");
            sb.AppendLine("           {");
            sb.AppendLine("             mensaje = row[\"mensaje\"].ToString();");
            sb.AppendLine("           }" );
            sb.AppendLine("         }");
            sb.AppendLine("     else");
            sb.AppendLine("         {");
            sb.AppendLine("          mensaje = \"sin accion\";");
            sb.AppendLine("         }");
            sb.AppendLine("         }");
            sb.AppendLine("         catch (Exception ex)");
            sb.AppendLine("         {");
            sb.AppendLine("         log.Error(\""+ table+":\",ex);");
            sb.AppendLine("         }");
            sb.AppendLine("     return mensaje;");
            sb.AppendLine("   }");
            sb.AppendLine(" }");
            createFile(sb.ToString(), prefijoClass + "_" + table);

            return null;
        }
        public void createEntity(String structura, String nombre)
        {

            System.IO.File.WriteAllText(pathScripts + nombre + ".cs", structura);
        }
        public void createFileSvc(String structura, String nombre)
        {

            System.IO.File.WriteAllText(pathScripts + nombre +"Svc.cs", structura);
        }
        public void createCallServiceAppCred(String structura, String nombre)
        {

            System.IO.File.WriteAllText(pathScripts + nombre + "CallSAppCred.cs", structura);
        }
        public void createQueryAppCred(String structura, String nombre)
        {

            System.IO.File.WriteAllText(pathScripts + nombre + "queryAppCred.cs", structura);
        }
        public void createFileInterface(String structura, String nombre)
        {

            System.IO.File.WriteAllText(pathScripts + nombre + "Int.cs", structura);
        }
        public void createFile(String structura,String nombre)
        {
       
            System.IO.File.WriteAllText(pathScripts + nombre+".cs", structura);
        }
        public void createFileSql(String structura, String nombre)
        {

            System.IO.File.WriteAllText(pathScripts + nombre + ".sql", structura);
        }
        public List<String> obteniendoPrimaryKey()
        {

            return null;
        }
        public String procedureScriptSql(DataSet ds,String name,String typepoOperation,String table,String conexion)
        {
            StringBuilder sbs = new StringBuilder();
            cls_sql sql = new cls_sql();
            DataTable dt = ds.Tables[0];
            String primaryString = "";
            int flagOne = 1;
            int flagTwo = 1;
            int flagTree = 1;
            int flagFour = 1;
            List<String> primary = new List<string>();
            primary = sql.obteniendoPrimaryKey(table, conexion);
            primaryString = primary[0];
            //String id = 0;
            sbs.AppendLine("CREATE PROCEDURE [dbo].[" + name+ "_"+ table+"]");
            

            //campos

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {

                    string column = Convert.ToString(row["COLUMN_NAME"]);
                    string datatype = Convert.ToString(row["DATA_TYPE"]);
                    string maxlenght = Convert.ToString(row["CHARACTER_MAXIMUM_LENGTH"]);
                    string numericPrecision = Convert.ToString(row["NUMERIC_PRECISION"]);
                    string numericScale = Convert.ToString(row["NUMERIC_SCALE"]);

                    if (datatype=="int" || datatype == "bit" || datatype == "money" || datatype == "datetime" || datatype == "smalldatetime")
                    { 

                        if (flagFour == ds.Tables[0].Rows.Count)
                        {
                            sbs.AppendLine(" @"+ column +" "+ datatype+" ");
                        }
                        else
                        {
                            sbs.AppendLine(" @" + column + " " + datatype + " , ");
                        }
                    }
                    else if(datatype == "numeric")
                    {

                        if (flagFour == ds.Tables[0].Rows.Count)
                        {
                            sbs.AppendLine(" @" + column + " " + datatype + "(" + numericPrecision + ","+ numericScale+") ");
                        }
                        else
                        {
                            sbs.AppendLine(" @" + column + " " + datatype + "(" + numericPrecision + "," + numericScale + ") ,");
                        }
                    }
                    else
                    {
                        if (flagFour == ds.Tables[0].Rows.Count)
                        {
                            sbs.AppendLine(" @" + column + " " + datatype + "(" + maxlenght + ") ");
                        }
                        else
                        {
                            sbs.AppendLine(" @" + column + " " + datatype + "(" + maxlenght + ") ,");
                        }
                    }


                    flagFour++;


                }
                sbs.AppendLine("AS");
                sbs.AppendLine("BEGIN");
                
            }
            //campos de entrada


            sbs.AppendLine("IF EXISTS(SELECT * FROM [dbo].["+ table + "] WHERE  "+ primaryString+" = @"+ primaryString+" ) ");
            sbs.AppendLine("BEGIN");

            //update
            sbs.AppendLine("UPDATE [dbo].["+ table + "]");
            sbs.AppendLine("SET");

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {

                    string column = Convert.ToString(row["COLUMN_NAME"]);

              
                    if(column!= primaryString)
                    { 
                    
                    if (flagOne == ds.Tables[0].Rows.Count)
                    {
                        sbs.AppendLine("[" + column + "]=@" + column + " ");
                    }
                    else
                    {
                        sbs.AppendLine("[" + column + "]=@" + column + " ,");
                    }
                    }


                    flagOne++;


                }
                sbs.AppendLine("WHERE "+ primaryString+" = @"+ primaryString);
            }
            sbs.AppendLine("select 'UPDATE' as mensaje");

            sbs.AppendLine("END");

            sbs.AppendLine("ELSE");

            sbs.AppendLine("BEGIN");

            //INSERT 

            sbs.AppendLine("INSERT INTO [dbo].[" + table + "]  (" );

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {

                    string column = Convert.ToString(row["COLUMN_NAME"]);

                    if (column != primaryString)
                    {


                        if (flagTwo == ds.Tables[0].Rows.Count)
                        {
                            sbs.AppendLine("[" + column + "] ");
                        }
                        else
                        {
                            //sbs.AppendLine("@" + column + ",");
                            sbs.AppendLine("[" + column + "], ");
                        }
                    }

                    flagTwo++;

                }
                sbs.AppendLine(")");

                sbs.AppendLine("VALUES");
                sbs.AppendLine("(");
                foreach (DataRow row in dt.Rows)
                {

                    string column = Convert.ToString(row["COLUMN_NAME"]);


                    if (column != primaryString)
                    {

                        if (flagTree == ds.Tables[0].Rows.Count)
                        {
                            sbs.AppendLine("@" + column + " ");
                        }
                        else
                        {
                            sbs.AppendLine("@" + column + ",");
                        }
                    }

                    flagTree++;

                }
                sbs.AppendLine(")");

                sbs.AppendLine("select 'INSERT' as mensaje");

                sbs.AppendLine("END");

            }
            sbs.AppendLine("END");


            createFileSql(sbs.ToString(), name+"_"+table);

            return null;
        }

    }
}
