using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateScriptDatabase.Class
{
    public class cls_sql
    {
       // public  string cadenaConexion = System.Configuration.ConfigurationManager.AppSettings["Connection"] ?? "Not Found";
        public DataSet Tablas(String cadenaConexion,String table)
        {
            Boolean val = false; ;
            String retorno = "";
            //DataSet ds = new DataSet();
            DataSet ds = new DataSet();

            using (SqlConnection openCon = new SqlConnection(cadenaConexion))
            {
                string saveStaff = "SELECT COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,NUMERIC_PRECISION,NUMERIC_SCALE " +
                            "FROM Information_Schema.Columns "+
                            "WHERE TABLE_NAME = '"+ table + "' "+
                            "ORDER BY COLUMN_NAME";
                using (SqlCommand querySaveStaff = new SqlCommand(saveStaff))
                {
                    SqlDataAdapter da = new SqlDataAdapter();
                    querySaveStaff.Connection = openCon;
                    querySaveStaff.CommandText = saveStaff;
                    da.SelectCommand = querySaveStaff;
                    
                    openCon.Open();
                    da.Fill(ds);
                    openCon.Close();
                    int i = 0;
                    int recordsAffected;
                    try
                    {
                        for (i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                        {
                            retorno = ds.Tables[0].Rows[i].ItemArray[0].ToString();
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            if (retorno != "")
            {
                val = true;
            }
            return ds;
        }
        public List<String> obteniendoPrimaryKey(String table, String cadenaConexion)
        {
            Boolean val = false; 
            String retorno = "";
            //DataSet ds = new DataSet();
            DataSet ds = new DataSet();

            List<String> primary = new List<string>();

            using (SqlConnection openCon = new SqlConnection(cadenaConexion))
            {
                string saveStaff = "SELECT KU.table_name as TABLENAME,column_name as PRIMARYKEYCOLUMN "+
                                   " FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC "+
                                   " INNER JOIN "+
                                    "    INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KU "+
                                    "          ON TC.CONSTRAINT_TYPE = 'PRIMARY KEY' AND "+
                                    "             TC.CONSTRAINT_NAME = KU.CONSTRAINT_NAME AND "+
                                     "            KU.table_name = '"+ table + "'  "+
                                   " ORDER BY KU.TABLE_NAME, KU.ORDINAL_POSITION; ";

                using (SqlCommand querySaveStaff = new SqlCommand(saveStaff))
                {
                    SqlDataAdapter da = new SqlDataAdapter();
                    querySaveStaff.Connection = openCon;
                    querySaveStaff.CommandText = saveStaff;
                    da.SelectCommand = querySaveStaff;

                    openCon.Open();
                    da.Fill(ds);
                    openCon.Close();
                    int i = 0;
                    int recordsAffected;
                    try
                    {
                        for (i = 0; i <= ds.Tables[0].Rows.Count - 1; i++)
                        {
                            retorno = ds.Tables[0].Rows[i].ItemArray[1].ToString();
                            primary.Add(retorno);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            if (retorno != "")
            {
                val = true;
            }
            return primary;
        }
    }
}
