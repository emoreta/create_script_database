using CreateScriptDatabase.Class;
using CreateScriptDatabase.Model;
using CreateScriptDatabase.Template;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreateScriptDatabase
{
    public partial class Form1 :Form
    {
        public static string cadenaConexion = System.Configuration.ConfigurationManager.AppSettings["Connection"] ?? "Not Found";
        public static string cadenaUseProcedure = System.Configuration.ConfigurationManager.AppSettings["conectionDestino"] ?? "Not Found";
        public static string rutaScripts = System.Configuration.ConfigurationManager.AppSettings["rutaScripts"] ?? "Not Found";


        StringBuilder sb = new StringBuilder();
        StringBuilder sbTables = new StringBuilder();
        List<string> columns = new List<string>();
        List<string> commands = new List<string>();
        List<caracteristicaQuery> cadenaQuery = new List<caracteristicaQuery>();

        int orden = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            txtRuta.Text = rutaScripts;
            txtClaseConexion.Text = cadenaUseProcedure;

            /*DataGridViewComboBoxColumn colType = new DataGridViewComboBoxColumn();
            colType.HeaderText = "Type";
            colType.DropDownWidth = 90;
            colType.Width = 90;
            colType.MaxDropDownItems = 5;
            this.dgvColumn.Columns.Insert(1, colType);
            colType.Items.AddRange("A", "N", "P", "S", "Z");
            this.dgvColumn.Columns[1].DataPropertyName = "trans_type";*/

        }

        public DataSet GetTables()
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();

            try
            {
                if (lblDataBase.Text != "")
                { 
                    SqlConnection con = new SqlConnection(lblstrConexion.Text);
                con.Open();
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "Use " + lblDataBase.Text + " Select table_name From INFORMATION_SCHEMA.Tables where table_type='BASE TABLE'";
                da.SelectCommand = cmd;
                con.Close();
                da.Fill(ds);
                }
            }
            catch (SqlException x)
            {
                MessageBox.Show("Error de SQL :" + x.Message);
            }
            catch (Exception y)
            {
                MessageBox.Show("Error :" + y.Message);
            }
            return ds;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (txtServerName.Text.Length == 0)
            {
                MessageBox.Show("Ingrese Nombre del Servidor");
                txtServerName.Focus();
            }
            else
            {
                DataSet ds = new DataSet();
                if (checkBox1.Checked)
                {
                    //lblstrConexion.Text = "SERVER=.\\SQLEXPRESS;Initial Catalog=" + cbListaBD.Text + ";Integrated Security=True";
                    lblstrConexion.Text = "Data Source = " + txtServerName.Text.Trim() + ";Initial Catalog=" + cbListaBD.Text + ";Integrated Security=True";
                }
                else
                {
                   // lblstrConexion.Text = "SERVER=" + txtServerName.Text.Trim() + ";Initial Catalog=" + cbListaBD.Text + ";USER ID=" + txtUser.Text + ";PWD=" + txtPass.Text + ";";
                    lblstrConexion.Text = "Data Source = " + txtServerName.Text.Trim() + ";Initial Catalog=" + cbListaBD.Text + ";USER ID=" + txtUser.Text + ";PWD=" + txtPass.Text + ";";
                }
                try
                {
                    SqlConnection con = new SqlConnection("" + lblstrConexion.Text + "");
                    con.Open();
                    //SqlCommand cmd = new SqlCommand("sp_databases", con);
                    SqlCommand cmd = new SqlCommand("SELECT * FROM sys.databases", con);
                    //cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    ad.Fill(ds);
                    con.Close();
                    cbListaBD.DataSource = ds.Tables[0];
                    cbListaBD.DisplayMember = "name";
                }
                catch (SqlException x)
                {
                    MessageBox.Show("Error de SQL :" + x.Message);
                }
                catch (Exception y)
                {
                    MessageBox.Show("Error :" + y.Message);
                }
                if (cbListaBD.Items.Count > 0)
                    btnNext.Enabled = true;
            }
        }

        private void dgvTablas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            String command="INNER JOIN";
            String commandSelect= "SELECT * FROM";
            caracteristicaQuery  cq= new caracteristicaQuery();

            orden = orden + 1;

            string nameTable = "";
            if (e.RowIndex >= 0)
            {
                nameTable=dgvTablas.Rows[e.RowIndex].Cells[1].Value.ToString();
                //commands.Add("inner join "+nameTable + " " + nameTable[0] + nameTable.Substring(nameTable.Length - 1) + " ");
                cq.Key = orden.ToString();
                cq.Tipo = "table";
                cq.Available = "1";
                cq.Description = nameTable;
                cq.Alias = nameTable[0] + nameTable.Substring(nameTable.Length - 1);
                

                
                if(orden==1)
                {
                    cq.Command = commandSelect;
                    sbTables.AppendLine(cq.Command + " " + cq.Description + " " + cq.Alias);
                    cadenaQuery.Add(cq);
                }
                else
                {
                    cq.Command = command;
                    sbTables.AppendLine(cq.Command + " " + cq.Description + " " + cq.Alias);
                    cadenaQuery.Add(cq);
                }
             

                var resultAll = String.Join(" ", commands.ToArray());
                sb.Clear();
                //sbTables.AppendLine(resultAll);
                //sb.AppendLine(resultAll);
                //txtEjecucion.Text = "";
                txtEjecucion.Text = sbTables.ToString() ;

                if (dgvTablas.Rows[e.RowIndex].Cells[0].Value == null)
                    dgvTablas.Rows[e.RowIndex].Cells[0].Value = true;
                else if (Boolean.Parse(dgvTablas.Rows[e.RowIndex].Cells[0].Value.ToString()) == true)
                    dgvTablas.Rows[e.RowIndex].Cells[0].Value = false;
                else if (Boolean.Parse(dgvTablas.Rows[e.RowIndex].Cells[0].Value.ToString()) == false)
                    dgvTablas.Rows[e.RowIndex].Cells[0].Value = true;

            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                foreach (DataGridViewRow row in dgvTablas.Rows)
                {
                    row.Cells[0].Value = true;
                }
            }
            else
            {
                foreach (DataGridViewRow row in dgvTablas.Rows)
                {
                    row.Cells[0].Value = false;
                }
            }
        }

        private void cbListaBD_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                //lblstrConexion.Text = "SERVER=.\\SQLEXPRESS;Initial Catalog=" + cbListaBD.Text + ";Integrated Security=True";
                lblstrConexion.Text = "Data Source = " + txtServerName.Text.Trim() + ";Initial Catalog=" + cbListaBD.Text + ";Integrated Security=True";
            }
            else
            {
                // lblstrConexion.Text = "SERVER=" + txtServerName.Text.Trim() + ";Initial Catalog=" + cbListaBD.Text + ";USER ID=" + txtUser.Text + ";PWD=" + txtPass.Text + ";";
                lblstrConexion.Text = "Data Source = " + txtServerName.Text.Trim() + ";Initial Catalog=" + cbListaBD.Text + ";USER ID=" + txtUser.Text + ";PWD=" + txtPass.Text + ";";
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            lblstrConexion.Text = lblstrConexion.Text;
            lblServerName.Text = txtServerName.Text;
            lblDataBase.Text = cbListaBD.Text;

            dgvTablas.DataSource = GetTables().Tables[0];
        }
        public DataSet GetColumns(string TableName)
        {

            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            DataSet ds = new DataSet();

            try
            {
                SqlConnection con = new SqlConnection(lblstrConexion.Text);
                con.Open();
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "Use " + lblDataBase.Text + " Select COLUMN_NAME,DATA_TYPE From INFORMATION_SCHEMA.columns where table_name=@TableName";
                cmd.Parameters.Add("@TableName", SqlDbType.NVarChar, 50).Value = TableName;
                da.SelectCommand = cmd;
                da.Fill(ds);
            }
            catch (SqlException x)
            {
                MessageBox.Show("Error de SQL :" + x.Message);
            }
            catch (Exception y)
            {
                MessageBox.Show("Error :" + y.Message);
            }
            return ds;
        }
        private void dgvTablas_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            dgvColumn.DataSource = GetColumns(dgvTablas.Rows[e.RowIndex].Cells[1].Value.ToString()).Tables[0];
        }

        private void btnGenerar_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            DataSet dsPrimary = new DataSet();
            cls_sql sql = new cls_sql();
            templateProcedure tp = new templateProcedure();
            FormCrudReact fcr = new FormCrudReact();
            FormCrudReactTemplate fcrt = new FormCrudReactTemplate();
            List<Field> Fields = new List<Field>();

            //tp.procedureClase(ds, "tscred_accionista", "class", "pro");
            //tp.procedureScriptSql(ds, "pro", "", "tscred_accionista");

            foreach (DataGridViewRow row in dgvColumn.Rows)
            {
                if (row.Cells[1].Value != null)
                {
                    Field fi = new Field();
                    fi.Tipo = row.Cells[1].Value.ToString();
                    fi.Columna = row.Cells[2].Value.ToString();
                    fi.TipoDato = row.Cells[3].Value.ToString();
                    Fields.Add(fi);
                }
            }

                foreach (DataGridViewRow row in dgvTablas.Rows)
            {
                if (row.Cells[0].Value != null)
                {
                    if (Boolean.Parse(row.Cells[0].Value.ToString()) == true)
                    {
                        ds = sql.Tablas(lblstrConexion.Text, row.Cells[1].Value.ToString().Substring(0, 1).ToUpper() + row.Cells[1].Value.ToString().Substring(1).ToLower());
                        //dsPrimary = sql.Tablas(lblstrConexion.Text, row.Cells[1].Value.ToString().Substring(0, 1).ToUpper() + row.Cells[1].Value.ToString().Substring(1).ToLower());
                        //GenerarClase(row.Cells[1].Value.ToString().Substring(0, 1).ToUpper() + row.Cells[1].Value.ToString().Substring(1).ToLower());
                        if (chkListaMetodo.CheckedItems.Count > 0)
                        {
                            for (int i = 0; i < chkListaMetodo.CheckedItems.Count; i++)
                            {
                                if (chkListaMetodo.CheckedItems[i].ToString().Equals("ClaseProcedure"))
                                {

                                    tp.procedureClase(ds, row.Cells[1].Value.ToString().Substring(0, 1).ToUpper() + row.Cells[1].Value.ToString().Substring(1).ToLower(), "class", "pro");

                                }
                                else if (chkListaMetodo.CheckedItems[i].ToString().Equals("ProcedureInsertUpdate"))
                                {
                                    tp.procedureScriptSql(ds, "pro", "", row.Cells[1].Value.ToString().Substring(0, 1).ToUpper() + row.Cells[1].Value.ToString().Substring(1).ToLower(), lblstrConexion.Text);
                                }
                                else if (chkListaMetodo.CheckedItems[i].ToString().Equals("EntityGetSet"))
                                {
                                    tp.entityClass(ds, row.Cells[1].Value.ToString().Substring(0, 1).ToUpper() + row.Cells[1].Value.ToString().Substring(1).ToLower());
                                }
                                else if (chkListaMetodo.CheckedItems[i].ToString().Equals("MethodSvc"))
                                {
                                    tp.methodSvc(row.Cells[1].Value.ToString().Substring(0, 1).ToUpper() + row.Cells[1].Value.ToString().Substring(1).ToLower(), "class");
                                }

                                else if (chkListaMetodo.CheckedItems[i].ToString().Equals("MethodInterface"))
                                {
                                    tp.methodInterface(row.Cells[1].Value.ToString().Substring(0, 1).ToUpper() + row.Cells[1].Value.ToString().Substring(1).ToLower(), "class");
                                }
                                else if (chkListaMetodo.CheckedItems[i].ToString().Equals("CallServiceAppCred"))
                                {
                                    tp.methodCallServiceAppCred(ds, row.Cells[1].Value.ToString().Substring(0, 1).ToUpper() + row.Cells[1].Value.ToString().Substring(1).ToLower(), "class");
                                }
                                else if (chkListaMetodo.CheckedItems[i].ToString().Equals("QueryAppCred"))
                                {
                                    tp.methodSelectAppCred(ds, row.Cells[1].Value.ToString().Substring(0, 1).ToUpper() + row.Cells[1].Value.ToString().Substring(1).ToLower(), "class", lblstrConexion.Text);
                                }
                                else if (chkListaMetodo.CheckedItems[i].ToString().Equals("FormReact"))
                                {
                                    fcr.formReactCru(ds, row.Cells[1].Value.ToString(), lblstrConexion.Text, Fields);
                                }
                                else if (chkListaMetodo.CheckedItems[i].ToString().Equals("FormReactTemplate"))
                                {
                                    fcrt.formReactCru(ds, row.Cells[1].Value.ToString(), lblstrConexion.Text, Fields);
                                }
                                else if (chkListaMetodo.CheckedItems[i].ToString().Equals("GetSetEntityNetCore"))
                                {
                                    GetSetModelEntity me = new GetSetModelEntity();
                                    me.entityClass(ds, row.Cells[1].Value.ToString().Substring(0, 1).ToUpper()+ row.Cells[1].Value.ToString().Substring(1));
                                }
                                else if (chkListaMetodo.CheckedItems[i].ToString().Equals("ControllerNetCore"))
                                {
                                    Controller c = new Controller();
                                    c.ControllerClass(ds, row.Cells[1].Value.ToString().Substring(0, 1).ToUpper() + row.Cells[1].Value.ToString().Substring(1));
                                }
                                else if (chkListaMetodo.CheckedItems[i].ToString().Equals("ContextNetCore"))
                                {
                                    Context c = new Context();
                                    c.ContextClass(ds, row.Cells[1].Value.ToString().Substring(0, 1).ToUpper() + row.Cells[1].Value.ToString().Substring(1));
                                }
                                else if (chkListaMetodo.CheckedItems[i].ToString().Equals("CodeNodeApi"))
                                {
                                    Context c = new Context();
                                    c.NodeApiRest(ds, row.Cells[1].Value.ToString().Substring(0, 1).ToUpper() + row.Cells[1].Value.ToString().Substring(1));
                                }
                                else if (chkListaMetodo.CheckedItems[i].ToString().Equals("BoostrapRazorPadre"))
                                {
                                    Context c = new Context();
                                    c.BoostrapPadre(ds, row.Cells[1].Value.ToString().Substring(0, 1).ToUpper() + row.Cells[1].Value.ToString().Substring(1));
                                }
                                else if (chkListaMetodo.CheckedItems[i].ToString().Equals("BoostrapRazorDetalle"))
                                {
                                    Context c = new Context();
                                    c.BoostrapDetalle(ds, row.Cells[1].Value.ToString().Substring(0, 1).ToUpper() + row.Cells[1].Value.ToString().Substring(1));
                                }
                                else if (chkListaMetodo.CheckedItems[i].ToString().Equals("InterfaceCs"))
                                {
                                    Context c = new Context();
                                    c.InterfaceNetCoreFront(ds, row.Cells[1].Value.ToString().Substring(0, 1).ToUpper() + row.Cells[1].Value.ToString().Substring(1));
                                }
                                else if (chkListaMetodo.CheckedItems[i].ToString().Equals("ControllerCs"))
                                {
                                    Context c = new Context();
                                    c.ControllerNetCoreFronrt(ds, row.Cells[1].Value.ToString().Substring(0, 1).ToUpper() + row.Cells[1].Value.ToString().Substring(1));
                                }
                                else if (chkListaMetodo.CheckedItems[i].ToString().Equals("CleanArchitecture"))
                                {
                                    CleanArchitecture c = new CleanArchitecture();
                                    c.EntitiesTables(ds, row.Cells[1].Value.ToString(), lblstrConexion.Text, Fields, txtNombreProyecto.Text);
                                    c.InterfaceRepository(ds, row.Cells[1].Value.ToString(), lblstrConexion.Text, Fields, txtNombreProyecto.Text);
                                    c.Features(ds, row.Cells[1].Value.ToString(), lblstrConexion.Text, Fields, txtNombreProyecto.Text);

                                }
                            }

                        }
                    }
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                //Data Source = (LocalDB)\MSSQLLocalDB; Initial Catalog = AppCred; Integrated Security = True; Connect Timeout = 30; user id = admin; password = admin"
                lblstrConexion.Text = "Data Source = " + txtServerName.Text.Trim() + ";Initial Catalog=" + cbListaBD.Text + ";Integrated Security=True";
                txtUser.Enabled = false;
                txtPass.Enabled = false;
            }
            else
            {
                lblstrConexion.Text = "Data Source = " + txtServerName.Text.Trim() + ";Initial Catalog=" + cbListaBD.Text + ";USER ID=" + txtUser.Text + ";PWD=" + txtPass.Text + ";";
                txtUser.Enabled = true;
                txtPass.Enabled = true;
                txtUser.Text = "";
                txtPass.Text = "";
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(rdbSelect.Checked)
            {
                commands.Add("Select ");
                commands.Add("*");
                commands.Add(" from ");

                var result = String.Join(" ", commands.ToArray());

                sb.Append(result);
                txtEjecucion.Text = sb.ToString();
            }

        }

        private void dgvColumn_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            sb.Length = 0;
            if (e.RowIndex >= 0)
            {

                if (dgvColumn.Rows[e.RowIndex].Cells[0].Value == null)
                {
                    dgvColumn.Rows[e.RowIndex].Cells[0].Value = true;
                    columns.Add(dgvColumn.Rows[e.RowIndex].Cells[2].Value.ToString());
                }
                else if (Boolean.Parse(dgvColumn.Rows[e.RowIndex].Cells[0].Value.ToString()) == true)
                {
                    dgvColumn.Rows[e.RowIndex].Cells[0].Value = false;
                    columns.Remove(dgvColumn.Rows[e.RowIndex].Cells[2].Value.ToString());
                }
                    
                else if (Boolean.Parse(dgvColumn.Rows[e.RowIndex].Cells[0].Value.ToString()) == false)
                {
                    dgvColumn.Rows[e.RowIndex].Cells[0].Value = true;
                    columns.Add(dgvColumn.Rows[e.RowIndex].Cells[2].Value.ToString());
                }
                    

            }
            var result = String.Join(", ", columns.ToArray());

            for (int i = 0; i < commands.Count; i++)
            {
                // Part 3: access element with index.
                Console.WriteLine($"{i} = {commands[i]}");
                if(i==1)
                {

                }
            }

            //commands[commands.FindIndex(ind => ind.Equals("*"))] = result;
            /*commands.Remove("*");
            commands.Insert(1, result);
            var resultAll = String.Join(" ", commands.ToArray());
            sb.AppendLine(resultAll);*/

            txtEjecucion.Text = sb.ToString();
        }

        private void chkListaMetodo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }
    }
    class caracteristicaQuery
    {
        String key;

        String tipo;
        String description;
        String relation;
        String alias;
        String available;
        String command;

        public string Tipo { get => tipo; set => tipo = value; }
        public string Description { get => description; set => description = value; }
        public string Relation { get => relation; set => relation = value; }
        public string Available { get => available; set => available = value; }
        public string Key { get => key; set => key = value; }
        public string Alias { get => alias; set => alias = value; }
        public string Command { get => command; set => command = value; }
    }
}
