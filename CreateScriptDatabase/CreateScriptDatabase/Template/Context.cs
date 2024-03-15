using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateScriptDatabase.Template
{

    public class Context
    {
        public static string pathScripts = System.Configuration.ConfigurationManager.AppSettings["rutaScripts"] ?? "Not Found";
        private static string ConvertirPrimeraLetraEnMayuscula(string texto)
        {
            string str = "";
            str = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(texto);

            return str;
        }
        public void createContext(String structura, String nombre)
        {

            System.IO.File.WriteAllText(pathScripts + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(nombre)) + ".js", structura);
        }
        public void createRazor(String structura, String nombre)
        {

            System.IO.File.WriteAllText(pathScripts + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(nombre)) + ".razor", structura);
        }
        public void createCs(String structura, String nombre)
        {

            System.IO.File.WriteAllText(pathScripts + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(nombre)) + "Service.cs", structura);
        }
        public void createICs(String structura, String nombre)
        {

            System.IO.File.WriteAllText(pathScripts + "I" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(nombre)) + "Service.cs", structura);
        }
        private static string limpiaGuion(string texto)
        {
            string str = "";
            str = texto.Replace("_", "");

            return str;
        }
        public String ContextClass(DataSet ds, String table)
        {
            StringBuilder sb = new StringBuilder();
            DataTable dt = ds.Tables[0];
            int flagFive = 1;

            sb.AppendLine("using arnuv_api.Models;");
            sb.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("namespace arnuv_api.Contexts");

            sb.AppendLine("{");
            sb.AppendLine("public class DbContext" + limpiaGuion(table) + " : DbContext");
            sb.AppendLine("{");

            sb.AppendLine("public DbContext" + limpiaGuion(table) + "(DbContextOptions<DbContext" + limpiaGuion(table) + "> options) : base(options)");
            sb.AppendLine("{");
            sb.AppendLine("}");
            sb.AppendLine("public DbSet<" + limpiaGuion(table) + "> " + limpiaGuion(table) + " { get; set; }");
            sb.AppendLine("}");
            sb.AppendLine("}");

            createContext(sb.ToString(), table);


            return null;

        }
        public String NodeApiRest(DataSet ds, String table)
        {
            String columnCadena = "";
            int flag = 1;
            StringBuilder sb = new StringBuilder();
            DataTable dt = ds.Tables[0];
            int flagFive = 1;

            //config database
            sb.AppendLine("const Pool = require('pg').Pool");
            sb.AppendLine("const pool = new Pool({");
            sb.AppendLine("user: '11111',");
            sb.AppendLine("host: '12345',");
            sb.AppendLine("database: 'bd',");
            sb.AppendLine("password: '12345',");
            sb.AppendLine("port: 5432,");
            sb.AppendLine("})");

            //GET
            sb.AppendLine("const get" + limpiaGuion(table) + " = (request, response) => {");
            sb.AppendLine("pool.query('SELECT '+");

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {

                    string column = Convert.ToString(row["COLUMN_NAME"]);
                    //columnCadena = column;
                    if (flag == ds.Tables[0].Rows.Count)
                    {
                        columnCadena += column;
                    }
                    else
                    {
                        columnCadena += column + ",";
                    }

                    flag++;

                }
                sb.Append("'" + columnCadena);
                sb.AppendLine(" from dbo." + table + " ORDER BY id ASC', (error, results) => {");
            }
            sb.AppendLine("if (error) {");
            sb.AppendLine("throw error");
            sb.AppendLine("}");
            sb.AppendLine("response.status(200).json(results.rows)");
            sb.AppendLine("})");
            sb.AppendLine("}");

            //GET ID
            sb.AppendLine("const get" + limpiaGuion(table) + "ById = (request, response) => {");
            sb.AppendLine("const id = parseInt(request.params.id)");
            sb.AppendLine("pool.query('SELECT '+");
            columnCadena = "";
            string columnNumber = "";
            flag = 1;
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {

                    string column = Convert.ToString(row["COLUMN_NAME"]);
                    //columnCadena = column;
                    if (flag == ds.Tables[0].Rows.Count)
                    {
                        columnCadena += column;
                        columnNumber += "$" + flag.ToString();
                    }
                    else
                    {
                        columnCadena += column + ",";
                        columnNumber += "$" + flag.ToString() + ",";
                    }

                    flag++;

                }
                sb.Append("'" + columnCadena);
                sb.AppendLine(" from dbo." + table + " WHERE id = $1', (error, results) => {");
            }

            sb.AppendLine("if (error) {");
            sb.AppendLine("throw error");
            sb.AppendLine("}");
            sb.AppendLine("response.status(200).json(results.rows)");
            sb.AppendLine("})");
            sb.AppendLine("}");
            //POST
            sb.AppendLine("const create" + limpiaGuion(table) + " = (request, response) => {");
            sb.AppendLine("const { " + columnCadena + " } = request.body");
            sb.AppendLine("pool.query('INSERT INTO dbo." + table + "'+");
            sb.AppendLine("'(" + columnCadena + ") '+");
            sb.AppendLine("'VALUES '+");
            sb.AppendLine("'(" + columnNumber + ")',");
            sb.AppendLine("[" + columnCadena + "],");
            sb.AppendLine(" (error, results) => {");
            sb.AppendLine("if (error) {");
            sb.AppendLine("throw error");
            sb.AppendLine("}");
            sb.AppendLine("response.status(201).send(`" + table + " added with ID: ${result.insertId}`)");
            sb.AppendLine("})");
            sb.AppendLine("}");
            //PUT
            sb.AppendLine("const update" + limpiaGuion(table) + " = (request, response) => {");
            sb.AppendLine("const id = parseInt(request.params.id)");
            sb.AppendLine("const { name, email } = request.body");
            sb.AppendLine("pool.query(");
            sb.AppendLine("'UPDATE dbo." + table + " SET '+");
            string columnCadena1 = "";
            //string columnNumber = "";
            flag = 1;
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {

                    string column = Convert.ToString(row["COLUMN_NAME"]);
                    //columnCadena = column;
                    if (flag == ds.Tables[0].Rows.Count)
                    {
                        columnCadena1 += column + "=" + "$" + flag.ToString();
                        //columnNumber += "$" + flag.ToString();
                    }
                    else
                    {
                        columnCadena1 += column + "=" + "$" + flag.ToString() + ",";
                        //columnNumber += "$" + flag.ToString() + ",";
                    }

                    flag++;

                }
                sb.AppendLine("'" + columnCadena1 + "'+");
                sb.AppendLine("' where id=$1',");
                //sb.AppendLine(" from dbo." + table + " WHERE id = $1', (error, results) => {");
            }


            //sb.AppendLine("name = $1, email = $2 WHERE id = $3',");
            //sb.AppendLine("'"+columnCadena+"',");
            sb.AppendLine("[" + columnCadena + "],");
            sb.AppendLine("(error, results) => {");
            sb.AppendLine("if (error) {");
            sb.AppendLine("throw error");
            sb.AppendLine("}");
            sb.AppendLine("response.status(200).send(`User modified with ID: ${id}`)");
            sb.AppendLine("})");
            sb.AppendLine("}");

            //DELETE
            sb.AppendLine("const delete" + limpiaGuion(table) + " = (request, response) => {");
            sb.AppendLine("const id = parseInt(request.params.id)");
            sb.AppendLine("pool.query('DELETE FROM dbo." + table + " WHERE id = $1', [id], (error, results) => {");
            sb.AppendLine("if (error) {");
            sb.AppendLine("throw error");
            sb.AppendLine("}");
            sb.AppendLine("response.status(200).send(`User deleted with ID: ${id}`)");
            sb.AppendLine("})");
            sb.AppendLine("}");

            sb.AppendLine("module.exports = {");
            sb.AppendLine("get" + limpiaGuion(table) + ",");
            sb.AppendLine("get" + limpiaGuion(table) + "ById" + ",");
            sb.AppendLine("create" + limpiaGuion(table) + ",");
            sb.AppendLine("update" + limpiaGuion(table) + ",");
            sb.AppendLine("delete" + limpiaGuion(table) + ",");
            sb.AppendLine("}");

            createContext(sb.ToString(), table);

            return null;

        }
        public String BoostrapPadre(DataSet ds, String table)
        {
            String columnCadena = "";
            int flag = 1;
            StringBuilder sb = new StringBuilder();
            DataTable dt = ds.Tables[0];
            int flagFive = 1;

            //config database
            sb.AppendLine("@page \"/" + limpiaGuion(table) + "\"");
            sb.AppendLine("@inject I"+ limpiaGuion(table) + "Service "+ limpiaGuion(table) + "Service");
            sb.AppendLine("@inject NavigationManager NavigationManager");
            sb.AppendLine("<h3>" + limpiaGuion(table) + "</h3>");
            sb.AppendLine("<table class=\"table\">");
            sb.AppendLine(" <thead>");
            sb.AppendLine("<tr>");
            //sb.AppendLine("<th>Id</th>");
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow row in dt.Rows)
                {
                    string column = Convert.ToString(row["COLUMN_NAME"]);
                    string tipodato = Convert.ToString(row["DATA_TYPE"]);


                    if (tipodato == "varchar")
                    {

                        sb.AppendLine("<th>" + column + "</th>");
                    }
                    else if (tipodato == "int" || tipodato == "money" || tipodato == "numeric" || tipodato == "decimal")
                    {
                        sb.AppendLine("<th>" + column + "</th>");
                    }

                    else if (tipodato == "bit")
                    {
                        sb.AppendLine("<th>" + column + "</th>");
                    }
                    else if (tipodato == "date" || tipodato == "datetime" || tipodato == "smalldatetime")
                    {
                        sb.AppendLine("<th>" + column + "</th>");
                    }
                }
            }
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");
            sb.AppendLine("<tbody>");
            sb.AppendLine("@foreach (var " + limpiaGuion(table) + " in " + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "Service.List" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + ")");
            sb.AppendLine("{");
            sb.AppendLine("<tr>");

            foreach (DataRow row in dt.Rows)
            {
                string column = Convert.ToString(row["COLUMN_NAME"]);
                string tipodato = Convert.ToString(row["DATA_TYPE"]);


                if (tipodato == "varchar")
                {

                    sb.AppendLine("<td>@"+limpiaGuion(table)+"." + limpiaGuion(column) +"</td>");

                }
                else if (tipodato == "int" || tipodato == "money" || tipodato == "numeric" || tipodato == "decimal")
                {
                    sb.AppendLine("<td>@" + limpiaGuion(table) + "." + limpiaGuion(column) + "</td>");

                }

                else if (tipodato == "bit")
                {
                    sb.AppendLine("<td>@" + limpiaGuion(table) + "." + limpiaGuion(column) + "</td>");

                }
                else if (tipodato == "date" || tipodato == "datetime" || tipodato == "smalldatetime")
                {
                    sb.AppendLine("<td>@" + limpiaGuion(table) + "." + limpiaGuion(column) + "</td>");

                }
            }

            sb.AppendLine(" <td>");
            sb.AppendLine("<button class=\"btn btn - primary\" @onclick=\"(() => Show"+ ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "("+ limpiaGuion(table) + ".id))\"><i class=\"oi oi - pencil\"></i></button>");
            sb.AppendLine(" </td>");
            sb.AppendLine("</tr>");
            sb.AppendLine("}");
            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");
            sb.AppendLine("<button class=\"btn btn - primary\" @onclick=\"CreateNew" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "\">Nuevo "+ ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "</button>");
            sb.AppendLine("@code { protected override async Task OnInitializedAsync()");
            sb.AppendLine("{");
            sb.AppendLine(" await "+ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table))+"Service.Get"+ ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "();");
            sb.AppendLine("}");
            sb.AppendLine("void Show" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "(int id)");
            sb.AppendLine("{");
            sb.AppendLine(" NavigationManager.NavigateTo($\""+ limpiaGuion(table) + "Detalle/{ id}\");");
            sb.AppendLine("}");
            sb.AppendLine("void CreateNew"+ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table))+"()");
            sb.AppendLine("{");
            sb.AppendLine("NavigationManager.NavigateTo(\"/" + limpiaGuion(table) + "Detalle\");");
            sb.AppendLine("}");
            sb.AppendLine("}");


            createRazor(sb.ToString(), table);

            return null;
        }
        public String BoostrapDetalle(DataSet ds, String table)
        {
            //String column = "";
            int flag = 1;
            StringBuilder sb = new StringBuilder();
            DataTable dt = ds.Tables[0];
            int flagFive = 1;
            sb.AppendLine("@page \"/" + limpiaGuion(table) + "DetallePage\"");
            sb.AppendLine("@using Microsoft.Extensions.Logging");
            sb.AppendLine("@inject ILogger<" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "detallePage> Logger");
            sb.AppendLine("@inject I" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "Service " + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "Service");
            sb.AppendLine("@if (Id == null)");
            sb.AppendLine("{");
            sb.AppendLine("<h3>Crear " + limpiaGuion(table).Substring(0, 1).ToLower() + limpiaGuion(table).Substring(1).ToLower() + " </h3>");
            sb.AppendLine("}");
            sb.AppendLine("else");
            sb.AppendLine("{");
            sb.AppendLine("<h3>Edit @" + limpiaGuion(table).Substring(0, 1).ToLower() + limpiaGuion(table).Substring(1).ToLower() +  ".nombres</h3>");
            sb.AppendLine("}");
            sb.AppendLine("<EditForm Model=\"" + limpiaGuion(table) + "\" OnValidSubmit =\"HandleSubmit\" > ");
            sb.AppendLine("<DataAnnotationsValidator />");
            sb.AppendLine("<ValidationSummary />");



            foreach (DataRow row in dt.Rows)
            {
                string column = Convert.ToString(row["COLUMN_NAME"]);
                string tipodato = Convert.ToString(row["DATA_TYPE"]);


                if (tipodato == "varchar")
                {

                    sb.AppendLine("<div>");
                    sb.AppendLine("<label for=\""+ limpiaGuion(column) + "\" > "  + limpiaGuion(column) + "</label>");
                    sb.AppendLine("<InputText id=\""  + limpiaGuion(column) + "\" @bind-Value=\"" + limpiaGuion(table).Substring(0, 1).ToLower() + limpiaGuion(table).Substring(1).ToLower() + "." + limpiaGuion(column) + "\" class=\"form-control\" ></InputText>");
                    sb.AppendLine("</div>");

                }
                else if (tipodato == "money" || tipodato == "numeric" || tipodato == "decimal")
                {
                    sb.AppendLine("<div>");
                    sb.AppendLine("<label for=\"" + limpiaGuion(column) + "\" > "  + limpiaGuion(column) + "</label>");
                    sb.AppendLine("<InputText id=\"" + limpiaGuion(column) + "\" @bind-Value=\"" + limpiaGuion(table).Substring(0, 1).ToLower() + limpiaGuion(table).Substring(1).ToLower() + "." + limpiaGuion(column) + "\" class=\"form-control\" ></InputText>");
                    sb.AppendLine("</div>");

                }
                else if (tipodato == "int")//cuando es clave foranea
                {
                    sb.AppendLine("<div>");
                    sb.AppendLine("<label for=\""  + limpiaGuion(column) + "\" > " + limpiaGuion(column) + "</label>");
                    //sb.AppendLine("<InputSelect id=\""  + limpiaGuion(column) + "\" @bind-Value=\"" + limpiaGuion(table) + "." + limpiaGuion(column) + "\" class=\"form-control\" > ");
                    sb.AppendLine("<InputText id=\"" + limpiaGuion(column) + "\" @bind-Value=\"" + limpiaGuion(table).Substring(0, 1).ToLower() + limpiaGuion(table).Substring(1).ToLower() + "." + limpiaGuion(column) + "\" class=\"form-control\" ></InputText>");
                    sb.AppendLine("</div>");

                }

                else if (tipodato == "bit")
                {
                    sb.AppendLine("<div>");
                    sb.AppendLine("<label for=\""  + limpiaGuion(column) + "\" > " + limpiaGuion(table) + "." + limpiaGuion(column) + "</label>");
                    sb.AppendLine("<InputCheckbox id=\"" + limpiaGuion(column) + "\" @bind-Value=\"" + limpiaGuion(table).Substring(0, 1).ToLower() + limpiaGuion(table).Substring(1).ToLower() + "." + limpiaGuion(column) + "\" class=\"form-control\" ></InputCheckbox>");
                    sb.AppendLine("</div>");

                }
                else if (tipodato == "date" || tipodato == "datetime" || tipodato == "smalldatetime")
                {
                    sb.AppendLine("<div>");
                    sb.AppendLine("<label for=\""  + limpiaGuion(column) + "\" > " + limpiaGuion(column) + "</label>");
                    sb.AppendLine("<InputDate id=\"" + limpiaGuion(column) + "\" @bind-Value=\"" + limpiaGuion(table).Substring(0, 1).ToLower() + limpiaGuion(table).Substring(1).ToLower() + "." + limpiaGuion(column) + "\" class=\"form-control\" />");
                    sb.AppendLine("</div>");

                }
            }

            sb.AppendLine("<br />");
            sb.AppendLine("<button type=\"submit\" class=\"btn btn - primary\" > @btnText</button>");
            sb.AppendLine("<button type=\"button\" class=\"btn btn - danger\" @onclick =\"Delete" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + " \" > Delete " + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + " </button>");
            sb.AppendLine("</EditForm>");
            sb.AppendLine("@code {");
            sb.AppendLine("[Parameter]");
            sb.AppendLine("public int? Id { get; set; }");
            sb.AppendLine("string btnText = string.Empty;");
            sb.AppendLine("Model." + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "  " + limpiaGuion(table).Substring(0, 1).ToLower() + limpiaGuion(table).Substring(1).ToLower() + " = new Model." + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + " ();");
            sb.AppendLine(" protected override async Task OnInitializedAsync()");
            sb.AppendLine("{");
            sb.AppendLine("btnText = Id == null ? \"Guardar " + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + " \" : \"Actualizar " + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + " \"; ");
            sb.AppendLine("await " + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + " Service.GetCiudad(); --clave foranea");
            sb.AppendLine("}");
            sb.AppendLine("protected override async Task OnParametersSetAsync()");
            sb.AppendLine("{");
            sb.AppendLine("if (Id == null)");
            sb.AppendLine("{");
            sb.AppendLine("");
            sb.AppendLine("}");
            sb.AppendLine("else");
            sb.AppendLine("{");
            sb.AppendLine(""+ limpiaGuion(table).Substring(0, 1).ToLower() + limpiaGuion(table).Substring(1).ToLower()+ " = await " + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "Service.GetSingle" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + " ((int)Id);");
            sb.AppendLine("}");
            sb.AppendLine("}");
            sb.AppendLine("async Task HandleSubmit()");
            sb.AppendLine("{");
            sb.AppendLine("if (Id == null)");
            sb.AppendLine("{");
            sb.AppendLine("await " + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "Service.Create" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + " ("  +limpiaGuion(table).Substring(0, 1).ToLower() + limpiaGuion(table).Substring(1).ToLower()  + ");");
            sb.AppendLine("}");
            sb.AppendLine("else");
            sb.AppendLine("{");
            sb.AppendLine("await " + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "Service.Update" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "(" + limpiaGuion(table).Substring(0, 1).ToLower() + limpiaGuion(table).Substring(1).ToLower() + ");");
            sb.AppendLine("}");
            sb.AppendLine("}");
            sb.AppendLine(" private void HandleValidSubmit()");
            sb.AppendLine("{");
            sb.AppendLine("Logger.LogInformation(\"HandleValidSubmit called\");");
            sb.AppendLine("}");
            sb.AppendLine("async Task Delete"+ ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table))+" ()");
            sb.AppendLine("{");
            sb.AppendLine("await "+ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "Service.Delete" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "(" + limpiaGuion(table).Substring(0, 1).ToLower() + limpiaGuion(table).Substring(1).ToLower() + ".id);");
            sb.AppendLine("}");
            sb.AppendLine("}");
            createRazor(sb.ToString(), table + "Detalle");
            return null;
        }
        public String InterfaceNetCoreFront(DataSet ds, String table)
        {
            String columnCadena = "";
            int flag = 1;
            StringBuilder sb = new StringBuilder();
            DataTable dt = ds.Tables[0];
            int flagFive = 1;
            sb.AppendLine("using LawyerApp.Model;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("namespace LawyerApp.Services");
            sb.AppendLine("{");
            sb.AppendLine("interface I" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "");
            sb.AppendLine("{");
            sb.AppendLine("List<" + limpiaGuion(table) + "> List" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + " { get; set; }");
            sb.AppendLine("Task Get" + limpiaGuion(table) + "();");
            sb.AppendLine("Task<" + limpiaGuion(table) + "> GetSingle" + limpiaGuion(table) + "(int id);");
            sb.AppendLine("Task Create" + limpiaGuion(table) + "(" + limpiaGuion(table) + " " + limpiaGuion(table) + ");");
            sb.AppendLine("Task Update" + limpiaGuion(table) + "(" + limpiaGuion(table) + " " + limpiaGuion(table) + ");");
            sb.AppendLine("Task Delete" + limpiaGuion(table) + "(int id);");
            sb.AppendLine("}");
            sb.AppendLine("}");

            createICs(sb.ToString(), table);

            return null;
        }
        public String ControllerNetCoreFronrt(DataSet ds, String table)
        {
            String columnCadena = "";
            int flag = 1;
            StringBuilder sb = new StringBuilder();
            DataTable dt = ds.Tables[0];
            int flagFive = 1;

            sb.AppendLine("using LawyerApp.Model;");
            sb.AppendLine("using Microsoft.AspNetCore.Components;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Net.Http;");
            sb.AppendLine("using System.Net.Http.Json;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("namespace LawyerApp.Services");
            sb.AppendLine("{");
            sb.AppendLine("public class " + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + " : I" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "");
            sb.AppendLine("{");
            sb.AppendLine("private readonly HttpClient _http;");
            sb.AppendLine(" private readonly NavigationManager _navigationManager;");
            sb.AppendLine("private static string host= \"https://localhost:44370/\";");
            sb.AppendLine("public List<" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "> List" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "{ get; set; } = new List<" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + ">();");
            sb.AppendLine("public " + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "(HttpClient http, NavigationManager navigationManager)");
            sb.AppendLine("{");
            sb.AppendLine("_http = http;");
            sb.AppendLine("_navigationManager = navigationManager;");
            sb.AppendLine("}");
            sb.AppendLine("public async Task Create" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "(" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + " " + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + ")");
            sb.AppendLine("{");
            sb.AppendLine("var result = await _http.PostAsJsonAsync(host + \"api /" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "/insert\", " + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + ");");
            sb.AppendLine(" await Set" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "(result);");
            sb.AppendLine("}");
            sb.AppendLine(" public Task Delete" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "(int id)");
            sb.AppendLine("{");
            sb.AppendLine("throw new NotImplementedException();");
            sb.AppendLine("}");
            sb.AppendLine("public async Task<Tabgusuario> GetSingle" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "(int id)");
            sb.AppendLine("{");
            sb.AppendLine("var result = await _http.GetFromJsonAsync<" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + ">(host+$\"api /" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "/getbyid/{ id}\");");
            sb.AppendLine("if (result != null)");
            sb.AppendLine("return result;");
            sb.AppendLine(" throw new Exception(\"" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + " not found!\");");
            sb.AppendLine("}");
            sb.AppendLine("public async Task Get" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "()");
            sb.AppendLine("        {");
            sb.AppendLine("            var result = await _http.GetFromJsonAsync<List<" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + ">>(host + \"api/" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "/get" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "\");");
            sb.AppendLine("            if (result != null)");
            sb.AppendLine("                List" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + " = result;");
            sb.AppendLine("        }");
            sb.AppendLine("public async Task Update" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "(" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + " " + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + ")");
            sb.AppendLine("        {");
            sb.AppendLine("            var result = await _http.PutAsJsonAsync(host +\"api/" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "/update\", " + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + ");");
            sb.AppendLine("            await Set" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "(result);");
            sb.AppendLine("        }");
            sb.AppendLine("private async Task Set" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "(HttpResponseMessage result)");
            sb.AppendLine("        {");
            sb.AppendLine("            var response = await result.Content.ReadFromJsonAsync<" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + ">();");
            sb.AppendLine("            //ListUsuarios = response;");
            sb.AppendLine("            _navigationManager.NavigateTo(\"" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + "\");");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            createCs(sb.ToString(), table );

            return null;
        }
    }

}
