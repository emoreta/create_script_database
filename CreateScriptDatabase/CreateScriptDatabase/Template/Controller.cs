using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateScriptDatabase.Template
{
    public class Controller
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
            str = texto.Replace("_", "");

            return str;
        }
        public void createEntity(String structura, String nombre)
        {

            System.IO.File.WriteAllText(pathScripts + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(nombre)) + "Controller.cs", structura);
        }
        
        public String ControllerClass(DataSet ds, String table)
        {
            StringBuilder sb = new StringBuilder();
            DataTable dt = ds.Tables[0];
            int flagFive = 1;

            sb.AppendLine("using arnuv_api.Contexts;");
            sb.AppendLine("using arnuv_api.Models;");
            sb.AppendLine("using Microsoft.AspNetCore.Mvc;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("namespace arnuv_api.Controllers");
            sb.AppendLine("{");
            sb.AppendLine("[ApiController]");
            sb.AppendLine("[Route(api/\"[controller]\")]");
            sb.AppendLine("public class "+ limpiaGuion(table)+"Controller : ControllerBase");
            sb.AppendLine("{");
            sb.AppendLine ("private DbContext"+ limpiaGuion(table) +" _dbContext"+ limpiaGuion(table) +";");
            sb.AppendLine("public "+limpiaGuion(table) + "Controller(DbContext" + limpiaGuion(table) + " context)");
            sb.AppendLine("{");
            sb.AppendLine(" _dbContext" + limpiaGuion(table) +" = context;");
            sb.AppendLine("}");
            sb.AppendLine("[HttpGet]");
            sb.AppendLine("[Route(\"get"+ limpiaGuion(table) +"/\")]");
            sb.AppendLine("public ActionResult<List<"+ limpiaGuion(table) +">> Get()");
            sb.AppendLine("{");
            sb.AppendLine("return _dbContext" + limpiaGuion(table) +"."+ limpiaGuion(table) +".ToList();");
            sb.AppendLine("}");
            //sb.AppendLine("}");
            //sb.AppendLine("}");
            //por id
            sb.AppendLine("[HttpGet]");
            sb.AppendLine("[Route(\"getbyid/{id}\")]");
            sb.AppendLine("public ActionResult<" + limpiaGuion(table) + "> Get(int id)");//pensado que todos son int toca ver cual es la primary key
            sb.AppendLine("{");
            sb.AppendLine("if (id == 0)");
            sb.AppendLine("{");
            sb.AppendLine(" return NotFound(\""+ limpiaGuion(table)+" id must be higher than zero\");");
            sb.AppendLine("}");
            sb.AppendLine(limpiaGuion(table) +" ob=_dbContext" + limpiaGuion(table)+ "."+ limpiaGuion(table) + ".FirstOrDefault(s => s.codeIva == id);");
            sb.AppendLine("if (ob == null)");
            sb.AppendLine("{");
            sb.AppendLine("return NotFound(\" "+ limpiaGuion(table)+ " not found\");");
            sb.AppendLine("}");
            sb.AppendLine("return Ok(ob);");
            sb.AppendLine("}");
            //insert
            sb.AppendLine("[HttpPost]");
            sb.AppendLine("[Route(\"insert/\")]");
            sb.AppendLine("public async Task<ActionResult> Post([FromBody] "+limpiaGuion(table)+ " "+limpiaGuion(table).ToLower()+")");
            sb.AppendLine("{");
            sb.AppendLine("if ("+ limpiaGuion(table).ToLower() + " == null)");
            sb.AppendLine("{");
            sb.AppendLine("return NotFound(\""+ limpiaGuion(table).ToLower() + " data is not supplied\");");
            sb.AppendLine("}");
            sb.AppendLine("if (!ModelState.IsValid)");
            sb.AppendLine("{");
            sb.AppendLine("return BadRequest(ModelState);");
            sb.AppendLine("}");
            sb.AppendLine("await _dbContext"+ limpiaGuion(table) + "."+ limpiaGuion(table) + ".AddAsync("+ limpiaGuion(table).ToLower() + ");");
            sb.AppendLine("await _dbContext" + limpiaGuion(table) + ".SaveChangesAsync();");
            sb.AppendLine("return Ok(" + limpiaGuion(table).ToLower() + ");");
            sb.AppendLine("}");
            //update
            sb.AppendLine("[HttpPut]");
            sb.AppendLine("[Route(\"update/\")]");
            sb.AppendLine("public async Task<ActionResult> Update([FromBody] " + limpiaGuion(table) + " " + limpiaGuion(table).ToLower() + ")");
            sb.AppendLine("{");
            sb.AppendLine("if (" + limpiaGuion(table).ToLower() + " == null)");
            sb.AppendLine("{");
            sb.AppendLine("return NotFound(\"" + limpiaGuion(table).ToLower() + " data is not supplied\");");
            sb.AppendLine("}");
            sb.AppendLine("if (!ModelState.IsValid)");
            sb.AppendLine("{");
            sb.AppendLine("return BadRequest(ModelState);");
            sb.AppendLine("}");
            sb.AppendLine(limpiaGuion(table)+ " ob = _dbContext" + limpiaGuion(table)+"."+ limpiaGuion(table)+ ".FirstOrDefault(s => s.codeIva == " + limpiaGuion(table).ToLower() + ".codeIva);");
            sb.AppendLine("if (ob == null)"); 
            sb.AppendLine("{");
            sb.AppendLine("return NotFound(\""+ limpiaGuion(table) + " does not exist in the database\");");
            sb.AppendLine("}");
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string column = Convert.ToString(row["COLUMN_NAME"]);
                    sb.AppendLine("ob."+ limpiaGuion(column) +"="+ limpiaGuion(table).ToLower()+"."+ limpiaGuion(column) +";");

                }
            }
            sb.AppendLine("_dbContext" + limpiaGuion(table)+".Attach(ob).State = Microsoft.EntityFrameworkCore.EntityState.Modified;");
            sb.AppendLine("await _dbContext" + limpiaGuion(table)+".SaveChangesAsync();");
            sb.AppendLine("return Ok(_dbContext" + limpiaGuion(table)+");");
            sb.AppendLine("}");
            sb.AppendLine("}");
            sb.AppendLine("}");

            createEntity(sb.ToString(), table);


            return null;

        }

        


    }
}
