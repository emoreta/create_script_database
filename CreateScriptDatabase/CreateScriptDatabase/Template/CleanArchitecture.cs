using CreateScriptDatabase.Class;
using CreateScriptDatabase.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateScriptDatabase.Template
{
    public class CleanArchitecture
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

        public void EntitiesTables(DataSet ds, String table, String conexion, List<Field> fields,string nameProject)
        {

            StringBuilder sb = new StringBuilder();
            cls_sql sql = new cls_sql();
            String primaryString = "";
            DataTable dt = ds.Tables[0];
            int flagFive = 1;

            List<String> primary = new List<string>();
            primary = sql.obteniendoPrimaryKey(table, conexion);
            primaryString = primary[0];


            sb.AppendLine("using "+ nameProject + ".Domain.Common;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations;");
            sb.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Text;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("");
            sb.AppendLine("namespace " + nameProject +".Domain.Entities");
            sb.AppendLine("{");
            sb.AppendLine("[Table(\"" + table + "\")]");
            sb.AppendLine("public sealed class " + limpiaGuion(table)+ " : BaseEntity");
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


                    if(column== primaryString)
                    {
                        sb.AppendLine("[Key]");
                    }
                    if (datatype == "int" || datatype == "smallint")
                    {
                        sb.AppendLine("[Column(\"" + column + "\")]");
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
            createEntity(sb,table);
        }

        public void InterfaceRepository(DataSet ds, String table, String conexion, List<Field> fields, string nameProject)
        {

            StringBuilder sb = new StringBuilder();
            cls_sql sql = new cls_sql();
            String primaryString = "";
            DataTable dt = ds.Tables[0];
            int flagFive = 1;

            List<String> primary = new List<string>();
            primary = sql.obteniendoPrimaryKey(table, conexion);
            primaryString = primary[0];

            String tipoDatoPrimary = "";
            String columnPrimary = "";
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {

                    string column = Convert.ToString(row["COLUMN_NAME"]);
                    string datatype = Convert.ToString(row["DATA_TYPE"]);
                    string maxlenght = Convert.ToString(row["CHARACTER_MAXIMUM_LENGTH"]);
                    string numericPrecision = Convert.ToString(row["NUMERIC_PRECISION"]);
                    string numericScale = Convert.ToString(row["NUMERIC_SCALE"]);


                    if (column == primaryString)
                    {

                        columnPrimary = column;
                        if (datatype == "int" || datatype == "smallint")
                        {

                            tipoDatoPrimary = "int ";
                        }
                        else if (datatype == "char")
                        {

                            tipoDatoPrimary = "char ";
                        }
                        else if (datatype == "bigint")
                        {
                            tipoDatoPrimary = "Int64 ";
                        }
                        else if (datatype == "varchar")
                        {
                            tipoDatoPrimary = "string";
                        }
                        else if (datatype == "money" || datatype == "numeric" || datatype == "decimal")
                        {
                            tipoDatoPrimary = "decimal";
                        }
                        else if (datatype == "datetime" || datatype == "smalldatetime")
                        {
                            tipoDatoPrimary = "DateTime";
                        }
                        else if (datatype == "date")
                        {
                            tipoDatoPrimary = "Date";
                        }
                        else if (datatype == "bit")
                        {

                            tipoDatoPrimary = "Boolean";
                        }
                    }
                }
            }


            sb.AppendLine("using " + nameProject + ".Domain.Entities;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Text;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("");
            sb.AppendLine("namespace " + nameProject + ".Application.Repositories");
            sb.AppendLine("{");
            sb.AppendLine("    public interface I" + table + "Repository : IBaseRepository<" + table + ">");
            sb.AppendLine("    {");
            sb.AppendLine("      Task<" + table + "> GetById("+tipoDatoPrimary+ " "+ primaryString+", CancellationToken cancellationToken);");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            createInterfaceRepository(sb, table);
        }
        public void Features(DataSet ds, String table, String conexion, List<Field> fields, string nameProject)
        {

            StringBuilder sb5 = new StringBuilder();
            StringBuilder sb6 = new StringBuilder();
            cls_sql sql = new cls_sql();
            String primaryString = "";
            DataTable dt = ds.Tables[0];
            int flagFive = 1;

            List<String> primary = new List<string>();
            primary = sql.obteniendoPrimaryKey(table, conexion);
            primaryString = primary[0];

            //crear carpetas
            string ruta = pathScripts + "Features";
            if (!Directory.Exists(ruta))
            {
                DirectoryInfo di = Directory.CreateDirectory(ruta);
                
            }
            if (!Directory.Exists(ruta + @"\Create" + table))
            {
                StringBuilder sb = new StringBuilder();
                StringBuilder sb1 = new StringBuilder();
                StringBuilder sb2 = new StringBuilder();
                StringBuilder sb3 = new StringBuilder();
                StringBuilder sb4 = new StringBuilder();


                DirectoryInfo diC = Directory.CreateDirectory(ruta + @"\Create" + table);

                sb.AppendLine("using AutoMapper;");
                sb.AppendLine("using " + nameProject + ".Application.Repositories;");
                sb.AppendLine("using " + nameProject + ".Domain.Entities;");
                sb.AppendLine("using MediatR;");
                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Linq;");
                sb.AppendLine("using System.Text;");
                sb.AppendLine("using System.Threading.Tasks;");
                sb.AppendLine("");
                sb.AppendLine("namespace " + nameProject + ".Application.Features." + table + "Features.Create" + table + "");
                sb.AppendLine("{");
                sb.AppendLine("    public sealed class Create" + table + "Handler : IRequestHandler<Create" + table + "Request, Create" + table + "Response>");
                sb.AppendLine("    {");
                sb.AppendLine("        private readonly IUnitOfWork _unitOfWork;");
                sb.AppendLine("        private readonly I" + table + "Repository _" + table.ToLower() + "Repository;");
                sb.AppendLine("        private readonly IMapper _mapper;");
                sb.AppendLine("        public Create" + table + "Handler(IUnitOfWork unitOfWork, I" + table + "Repository " + table.ToLower() + "Repository, IMapper mapper)");
                sb.AppendLine("        {");
                sb.AppendLine("            _unitOfWork = unitOfWork;");
                sb.AppendLine("            _" + table.ToLower() + "Repository = " + table.ToLower() + "Repository;");
                sb.AppendLine("            _mapper = mapper;");
                sb.AppendLine("        }");
                sb.AppendLine("");
                sb.AppendLine("        public async Task<Create" + table + "Response> Handle(Create" + table + "Request request, CancellationToken cancellationToken)");
                sb.AppendLine("        {");
                sb.AppendLine("            var " + table.ToLower() + " = _mapper.Map<" + table + ">(request);");
                sb.AppendLine("            _" + table.ToLower() + "Repository.Create(" + table.ToLower() + ");");
                sb.AppendLine("            await _unitOfWork.Save(cancellationToken);");
                sb.AppendLine("");
                sb.AppendLine("            return _mapper.Map<Create" + table + "Response>(" + table.ToLower() + ");");
                sb.AppendLine("        }");
                sb.AppendLine("    }");
                sb.AppendLine("}");

                createHandler(sb, table,ruta,table);


                sb1.AppendLine("using AutoMapper;");
                sb1.AppendLine("using " + nameProject + ".Domain.Entities;");
                sb1.AppendLine("using System;");
                sb1.AppendLine("using System.Collections.Generic;");
                sb1.AppendLine("using System.Linq;");
                sb1.AppendLine("using System.Text;");
                sb1.AppendLine("using System.Threading.Tasks;");
                sb1.AppendLine("");
                sb1.AppendLine("namespace " + nameProject + ".Application.Features." + table + "Features.Create" + table + "");
                sb1.AppendLine("{");
                sb1.AppendLine("    public class Create" + table + "Mapper : Profile");
                sb1.AppendLine("    {");
                sb1.AppendLine("        public Create" + table + "Mapper()");
                sb1.AppendLine("        {");
                sb1.AppendLine("            CreateMap<Create" + table + "Request, " + table + ">();");
                sb1.AppendLine("            CreateMap<" + table + ", Create" + table + "Response>();");
                sb1.AppendLine("        }");
                sb1.AppendLine("    }");
                sb1.AppendLine("}");

                createMapper(sb1, table, ruta, table);

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

                            sb3.Append("int " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "char")
                        {

                            sb3.Append("char " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "bigint")
                        {
                            sb3.Append(" Int64 " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "varchar")
                        {
                            sb3.Append(" string " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "money" || datatype == "numeric" || datatype == "decimal")
                        {
                            sb3.Append(" decimal " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "datetime" || datatype == "smalldatetime")
                        {
                            sb3.Append(" DateTime " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "date")
                        {
                            sb3.Append(" Date" + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "bit")
                        {

                            sb3.Append("Boolean " + limpiaGuion(column) + ",");
                        }
                        
                        flagFive++;
                        

                    }
                }
                string cadena=sb3.ToString().Substring(0, sb3.ToString().Length - 1);

                sb2.AppendLine("using MediatR;");
                sb2.AppendLine("using System;");
                sb2.AppendLine("using System.Collections.Generic;");
                sb2.AppendLine("using System.Linq;");
                sb2.AppendLine("using System.Text;");
                sb2.AppendLine("using System.Threading.Tasks;");
                sb2.AppendLine("");
                sb2.AppendLine("namespace " + nameProject + ".Application.Features." + table + "Features.Create" + table + "");
                sb2.AppendLine("{");
                sb2.AppendLine("    public sealed record Create" + table + "Request(");
                sb2.AppendLine(cadena);
                sb2.AppendLine(") : IRequest<Create" + table + "Response>;");
                sb2.AppendLine("");
                sb2.AppendLine("}");

                createRequest(sb2, table, ruta, table);

                sb4.AppendLine("using System;");
                sb4.AppendLine("using System.Collections.Generic;");
                sb4.AppendLine("using System.Linq;");
                sb4.AppendLine("using System.Text;");
                sb4.AppendLine("using System.Threading.Tasks;");
                sb4.AppendLine("");
                sb4.AppendLine("namespace " + nameProject + ".Application.Features." + table + "Features.Create" + table + "");
                sb4.AppendLine("{");
                sb4.AppendLine("    public sealed record Create" + table + "Response");
                sb4.AppendLine("    {");

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {

                        string column = Convert.ToString(row["COLUMN_NAME"]);
                        string datatype = Convert.ToString(row["DATA_TYPE"]);
                        string maxlenght = Convert.ToString(row["CHARACTER_MAXIMUM_LENGTH"]);
                        string numericPrecision = Convert.ToString(row["NUMERIC_PRECISION"]);
                        string numericScale = Convert.ToString(row["NUMERIC_SCALE"]);

                        /*if (column == primaryString)
                        {
                            sb.AppendLine("[Key]");
                        }*/
                        if (datatype == "int" || datatype == "smallint")
                        {
                            sb4.AppendLine("public int " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "char")
                        {
                            sb4.AppendLine("public char " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "bigint")
                        {
                            sb4.AppendLine("public Int64 " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "varchar")
                        {
                            sb4.AppendLine("public string " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "money" || datatype == "numeric" || datatype == "decimal")
                        {
                            sb4.AppendLine("public decimal " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "datetime" || datatype == "smalldatetime")
                        {
                            sb4.AppendLine("public DateTime " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "date")
                        {
                            sb4.AppendLine("public Date" + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "bit")
                        {
                            sb4.AppendLine("public Boolean " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        flagFive++;
                    }
                }

                sb4.AppendLine("    }");
                sb4.AppendLine("}");

                createResponse(sb4, table, ruta, table);

            }

            String tipoDatoPrimary = "";
            String columnPrimary = "";
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {

                    string column = Convert.ToString(row["COLUMN_NAME"]);
                    string datatype = Convert.ToString(row["DATA_TYPE"]);
                    string maxlenght = Convert.ToString(row["CHARACTER_MAXIMUM_LENGTH"]);
                    string numericPrecision = Convert.ToString(row["NUMERIC_PRECISION"]);
                    string numericScale = Convert.ToString(row["NUMERIC_SCALE"]);


                    if (column == primaryString)
                    {
                       
                        columnPrimary = column;
                        if (datatype == "int" || datatype == "smallint")
                        {

                            tipoDatoPrimary = "int ";
                        }
                        else if (datatype == "char")
                        {

                            tipoDatoPrimary = "char ";
                        }
                        else if (datatype == "bigint")
                        {
                            tipoDatoPrimary = "Int64 ";
                        }
                        else if (datatype == "varchar")
                        {
                            tipoDatoPrimary = "string";
                        }
                        else if (datatype == "money" || datatype == "numeric" || datatype == "decimal")
                        {
                            tipoDatoPrimary = "decimal";
                        }
                        else if (datatype == "datetime" || datatype == "smalldatetime")
                        {
                            tipoDatoPrimary = "DateTime";
                        }
                        else if (datatype == "date")
                        {
                            tipoDatoPrimary = "Date";
                        }
                        else if (datatype == "bit")
                        {

                            tipoDatoPrimary = "Boolean";
                        }
                    }
                }
            }
            sb5.AppendLine("using " + nameProject + ".Application.Repositories;");
            sb5.AppendLine("using " + nameProject + ".Domain.Entities;");
            sb5.AppendLine("using " + nameProject + ".Persistence.Context;");
            sb5.AppendLine("using Microsoft.EntityFrameworkCore;");
            sb5.AppendLine("using System;");
            sb5.AppendLine("using System.Collections.Generic;");
            sb5.AppendLine("using System.Linq;");
            sb5.AppendLine("using System.Text;");
            sb5.AppendLine("using System.Threading.Tasks;");
            sb5.AppendLine("");
            sb5.AppendLine("namespace " + nameProject + ".Persistence.Repositories");
            sb5.AppendLine("{");
            sb5.AppendLine("    public class " + table + "Repository : BaseRepository<" + table + ">, I" + table + "Repository");
            sb5.AppendLine("    {");
            sb5.AppendLine("        public " + table + "Repository(DataContext context) : base(context)");
            sb5.AppendLine("        {");
            sb5.AppendLine("        }");

            sb5.AppendLine("public async Task<"+ table +"> GetById("+tipoDatoPrimary+" id, CancellationToken cancellationToken)");
            sb5.AppendLine("{");
            sb5.AppendLine("    return await Context." + table + ".FirstOrDefaultAsync(x => x."+ columnPrimary + " == id, cancellationToken);");
            sb5.AppendLine("}");

            sb5.AppendLine("");
            sb5.AppendLine("    }");
            sb5.AppendLine("}");

            repository(sb5, table, ruta, table);


            sb6.AppendLine("using " + nameProject + ".Application.Features." + table + "Features.Create" + table + ";");
            sb6.AppendLine("using " + nameProject + ".Application.Features." + table + "Features.GetAll" + table + ";");
            sb6.AppendLine("using " + nameProject + ".Application.Features." + table + "Features.Update" + table + ";");
            sb6.AppendLine("using MediatR;");
            sb6.AppendLine("using Microsoft.AspNetCore.Mvc;");
            sb6.AppendLine("");
            sb6.AppendLine("namespace " + nameProject + ".Controllers");
            sb6.AppendLine("{");
            sb6.AppendLine("");
            sb6.AppendLine("    [ApiController]");
            sb6.AppendLine("    [Route(\"" + table + "\")]");
            sb6.AppendLine("    public class " + table + "Controller : ControllerBase");
            sb6.AppendLine("    {");
            sb6.AppendLine("        private readonly IMediator _mediator;");
            sb6.AppendLine("");
            sb6.AppendLine("        public " + table + "Controller(IMediator mediator)");
            sb6.AppendLine("        {");
            sb6.AppendLine("            _mediator = mediator;");
            sb6.AppendLine("        }");
            sb6.AppendLine("");
            sb6.AppendLine("        [HttpGet]");
            sb6.AppendLine("        //[AllowAnonymous]");
            sb6.AppendLine("        [Route(\"Get/\")]");
            sb6.AppendLine("        public async Task<ActionResult<List<Get" + table + "Response>>> Get([FromQuery] Get" + table + "Request id,CancellationToken cancellationToken)");
            sb6.AppendLine("        {");
            sb6.AppendLine("            var response = await _mediator.Send(id, cancellationToken);");
            sb6.AppendLine("            return Ok(response);");
            sb6.AppendLine("        }");
            sb6.AppendLine("");
            sb6.AppendLine("        [HttpPost]");
            sb6.AppendLine("        //[AllowAnonymous]");
            sb6.AppendLine("        [Route(\"Create/\")]");
            sb6.AppendLine("        public async Task<ActionResult<Create" + table + "Response>> Create(Create" + table + "Request request,");
            sb6.AppendLine("            CancellationToken cancellationToken)");
            sb6.AppendLine("        {");
            sb6.AppendLine("            var response = await _mediator.Send(request, cancellationToken);");
            sb6.AppendLine("            return Ok(response);");
            sb6.AppendLine("        }");
            sb6.AppendLine("        [HttpPost]");
            sb6.AppendLine("        //[AllowAnonymous]");
            sb6.AppendLine("        [Route(\"Update/\")]");
            sb6.AppendLine("        public async Task<ActionResult<Update" + table + "Response>> UpdateCatalog(Update" + table + "Request request,");
            sb6.AppendLine("            CancellationToken cancellationToken)");
            sb6.AppendLine("        {");
            sb6.AppendLine("            var response = await _mediator.Send(request, cancellationToken);");
            sb6.AppendLine("            return Ok(response);");
            sb6.AppendLine("        }");
            sb6.AppendLine("");
            sb6.AppendLine("        [HttpGet]");
            sb6.AppendLine("        //[AllowAnonymous]");
            sb6.AppendLine("        [Route(\"GetAll/\")]");
            sb6.AppendLine("        public async Task<ActionResult<List<GetAll" + table + "Response>>> GetAll(CancellationToken cancellationToken)");
            sb6.AppendLine("        {");
            sb6.AppendLine("            var response = await _mediator.Send(new GetAll" + table + "Request(), cancellationToken);");
            sb6.AppendLine("            return Ok(response);");
            sb6.AppendLine("        }");
            sb6.AppendLine("    }");
            sb6.AppendLine("}");
            sb6.AppendLine("");
            sb6.AppendLine("");
            sb6.AppendLine("");
            sb6.AppendLine("");
            sb6.AppendLine("");
            sb6.AppendLine("");
            sb6.AppendLine("");
            sb6.AppendLine("");

            controller(sb6, table, ruta, table);


            if (!Directory.Exists(ruta + @"\Update" + table))
            {
                StringBuilder sb = new StringBuilder();
                StringBuilder sb1 = new StringBuilder();
                StringBuilder sb2 = new StringBuilder();
                StringBuilder sb3 = new StringBuilder();
                StringBuilder sb4 = new StringBuilder();


                DirectoryInfo diC = Directory.CreateDirectory(ruta + @"\Update" + table);

                sb.AppendLine("using AutoMapper;");
                sb.AppendLine("using " + nameProject + ".Application.Repositories;");
                sb.AppendLine("using " + nameProject + ".Domain.Entities;");
                sb.AppendLine("using MediatR;");
                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Linq;");
                sb.AppendLine("using System.Text;");
                sb.AppendLine("using System.Threading.Tasks;");
                sb.AppendLine("");
                sb.AppendLine("namespace " + nameProject + ".Application.Features." + table + "Features.Update" + table + "");
                sb.AppendLine("{");
                sb.AppendLine("    public sealed class Update" + table + "Handler : IRequestHandler<Update" + table + "Request, Update" + table + "Response>");
                sb.AppendLine("    {");
                sb.AppendLine("        private readonly IUnitOfWork _unitOfWork;");
                sb.AppendLine("        private readonly I" + table + "Repository _" + table.ToLower() + "Repository;");
                sb.AppendLine("        private readonly IMapper _mapper;");
                sb.AppendLine("        public Update" + table + "Handler(IUnitOfWork unitOfWork, I" + table + "Repository " + table.ToLower() + "Repository, IMapper mapper)");
                sb.AppendLine("        {");
                sb.AppendLine("            _unitOfWork = unitOfWork;");
                sb.AppendLine("            _" + table.ToLower() + "Repository = " + table.ToLower() + "Repository;");
                sb.AppendLine("            _mapper = mapper;");
                sb.AppendLine("        }");
                sb.AppendLine("");
                sb.AppendLine("        public async Task<Update" + table + "Response> Handle(Update" + table + "Request request, CancellationToken cancellationToken)");
                sb.AppendLine("        {");
                sb.AppendLine("            var " + table.ToLower() + " = _mapper.Map<" + table + ">(request);");
                sb.AppendLine("            _" + table.ToLower() + "Repository.Update(" + table.ToLower() + ");");
                sb.AppendLine("            await _unitOfWork.Save(cancellationToken);");
                sb.AppendLine("");
                sb.AppendLine("            return _mapper.Map<Update" + table + "Response>(" + table.ToLower() + ");");
                sb.AppendLine("        }");
                sb.AppendLine("    }");
                sb.AppendLine("}");

                updateHandler(sb, table, ruta, table);


                sb1.AppendLine("using AutoMapper;");
                sb1.AppendLine("using " + nameProject + ".Domain.Entities;");
                sb1.AppendLine("using System;");
                sb1.AppendLine("using System.Collections.Generic;");
                sb1.AppendLine("using System.Linq;");
                sb1.AppendLine("using System.Text;");
                sb1.AppendLine("using System.Threading.Tasks;");
                sb1.AppendLine("");
                sb1.AppendLine("namespace " + nameProject + ".Application.Features." + table + "Features.Update" + table + "");
                sb1.AppendLine("{");
                sb1.AppendLine("    public class Update" + table + "Mapper : Profile");
                sb1.AppendLine("    {");
                sb1.AppendLine("        public Update" + table + "Mapper()");
                sb1.AppendLine("        {");
                sb1.AppendLine("            CreateMap<Update" + table + "Request, " + table + ">();");
                sb1.AppendLine("            CreateMap<" + table + ", Update" + table + "Response>();");
                sb1.AppendLine("        }");
                sb1.AppendLine("    }");
                sb1.AppendLine("}");

                updateMapper(sb1, table, ruta, table);

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

                            sb3.Append("int " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "char")
                        {

                            sb3.Append("char " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "bigint")
                        {
                            sb3.Append(" Int64 " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "varchar")
                        {
                            sb3.Append(" string " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "money" || datatype == "numeric" || datatype == "decimal")
                        {
                            sb3.Append(" decimal " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "datetime" || datatype == "smalldatetime")
                        {
                            sb3.Append(" DateTime " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "date")
                        {
                            sb3.Append(" Date" + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "bit")
                        {

                            sb3.Append("Boolean " + limpiaGuion(column) + ",");
                        }

                        flagFive++;


                    }
                }
                string cadena = sb3.ToString().Substring(0, sb3.ToString().Length - 1);

                sb2.AppendLine("using MediatR;");
                sb2.AppendLine("using System;");
                sb2.AppendLine("using System.Collections.Generic;");
                sb2.AppendLine("using System.Linq;");
                sb2.AppendLine("using System.Text;");
                sb2.AppendLine("using System.Threading.Tasks;");
                sb2.AppendLine("");
                sb2.AppendLine("namespace " + nameProject + ".Application.Features." + table + "Features.Update" + table + "");
                sb2.AppendLine("{");
                sb2.AppendLine("    public sealed record Update" + table + "Request(");
                sb2.AppendLine(cadena);
                sb2.AppendLine(") : IRequest<Update" + table + "Response>;");
                sb2.AppendLine("");
                sb2.AppendLine("}");

                updateRequest(sb2, table, ruta, table);

                sb4.AppendLine("using System;");
                sb4.AppendLine("using System.Collections.Generic;");
                sb4.AppendLine("using System.Linq;");
                sb4.AppendLine("using System.Text;");
                sb4.AppendLine("using System.Threading.Tasks;");
                sb4.AppendLine("");
                sb4.AppendLine("namespace " + nameProject + ".Application.Features." + table + "Features.Update" + table + "");
                sb4.AppendLine("{");
                sb4.AppendLine("    public sealed record Update" + table + "Response");
                sb4.AppendLine("    {");

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {

                        string column = Convert.ToString(row["COLUMN_NAME"]);
                        string datatype = Convert.ToString(row["DATA_TYPE"]);
                        string maxlenght = Convert.ToString(row["CHARACTER_MAXIMUM_LENGTH"]);
                        string numericPrecision = Convert.ToString(row["NUMERIC_PRECISION"]);
                        string numericScale = Convert.ToString(row["NUMERIC_SCALE"]);

                        /*if (column == primaryString)
                        {
                            sb.AppendLine("[Key]");
                        }*/
                        if (datatype == "int" || datatype == "smallint")
                        {
                            sb4.AppendLine("public int " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "char")
                        {
                            sb4.AppendLine("public char " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "bigint")
                        {
                            sb4.AppendLine("public Int64 " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "varchar")
                        {
                            sb4.AppendLine("public string " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "money" || datatype == "numeric" || datatype == "decimal")
                        {
                            sb4.AppendLine("public decimal " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "datetime" || datatype == "smalldatetime")
                        {
                            sb4.AppendLine("public DateTime " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "date")
                        {
                            sb4.AppendLine("public Date" + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "bit")
                        {
                            sb4.AppendLine("public Boolean " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        flagFive++;
                    }
                }

                sb4.AppendLine("    }");
                sb4.AppendLine("}");

                updateResponse(sb4, table, ruta, table);

            }
            if (!Directory.Exists(ruta + @"\Delete" + table))
            {
                StringBuilder sb = new StringBuilder();
                StringBuilder sb1 = new StringBuilder();
                StringBuilder sb2 = new StringBuilder();
                StringBuilder sb3 = new StringBuilder();
                StringBuilder sb4 = new StringBuilder();

                DirectoryInfo diC = Directory.CreateDirectory(ruta + @"\Delete" + table);

                sb.AppendLine("using AutoMapper;");
                sb.AppendLine("using " + nameProject + ".Application.Repositories;");
                sb.AppendLine("using " + nameProject + ".Domain.Entities;");
                sb.AppendLine("using MediatR;");
                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Linq;");
                sb.AppendLine("using System.Text;");
                sb.AppendLine("using System.Threading.Tasks;");
                sb.AppendLine("");
                sb.AppendLine("namespace " + nameProject + ".Application.Features." + table + "Features.Delete" + table + "");
                sb.AppendLine("{");
                sb.AppendLine("    public sealed class Delete" + table + "Handler : IRequestHandler<Delete" + table + "Request, Delete" + table + "Response>");
                sb.AppendLine("    {");
                sb.AppendLine("        private readonly IUnitOfWork _unitOfWork;");
                sb.AppendLine("        private readonly I" + table + "Repository _" + table.ToLower() + "Repository;");
                sb.AppendLine("        private readonly IMapper _mapper;");
                sb.AppendLine("        public Delete" + table + "Handler(IUnitOfWork unitOfWork, I" + table + "Repository " + table.ToLower() + "Repository, IMapper mapper)");
                sb.AppendLine("        {");
                sb.AppendLine("            _unitOfWork = unitOfWork;");
                sb.AppendLine("            _" + table.ToLower() + "Repository = " + table.ToLower() + "Repository;");
                sb.AppendLine("            _mapper = mapper;");
                sb.AppendLine("        }");
                sb.AppendLine("");
                sb.AppendLine("        public async Task<Delete" + table + "Response> Handle(Delete" + table + "Request request, CancellationToken cancellationToken)");
                sb.AppendLine("        {");
                sb.AppendLine("            var " + table.ToLower() + " = _mapper.Map<" + table + ">(request);");
                sb.AppendLine("            _" + table.ToLower() + "Repository.Delete(" + table.ToLower() + ");");
                sb.AppendLine("            await _unitOfWork.Save(cancellationToken);");
                sb.AppendLine("");
                sb.AppendLine("            return _mapper.Map<Delete" + table + "Response>(" + table.ToLower() + ");");
                sb.AppendLine("        }");
                sb.AppendLine("    }");
                sb.AppendLine("}");

                deleteHandler(sb, table, ruta, table);


                sb1.AppendLine("using AutoMapper;");
                sb1.AppendLine("using " + nameProject + ".Domain.Entities;");
                sb1.AppendLine("using System;");
                sb1.AppendLine("using System.Collections.Generic;");
                sb1.AppendLine("using System.Linq;");
                sb1.AppendLine("using System.Text;");
                sb1.AppendLine("using System.Threading.Tasks;");
                sb1.AppendLine("");
                sb1.AppendLine("namespace " + nameProject + ".Application.Features." + table + "Features.Delete" + table + "");
                sb1.AppendLine("{");
                sb1.AppendLine("    public class Delete" + table + "Mapper : Profile");
                sb1.AppendLine("    {");
                sb1.AppendLine("        public Delete" + table + "Mapper()");
                sb1.AppendLine("        {");
                sb1.AppendLine("            CreateMap<Delete" + table + "Request, " + table + ">();");
                sb1.AppendLine("            CreateMap<" + table + ", Delete" + table + "Response>();");
                sb1.AppendLine("        }");
                sb1.AppendLine("    }");
                sb1.AppendLine("}");

                deleteMapper(sb1, table, ruta, table);

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

                            sb3.Append("int " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "char")
                        {

                            sb3.Append("char " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "bigint")
                        {
                            sb3.Append(" Int64 " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "varchar")
                        {
                            sb3.Append(" string " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "money" || datatype == "numeric" || datatype == "decimal")
                        {
                            sb3.Append(" decimal " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "datetime" || datatype == "smalldatetime")
                        {
                            sb3.Append(" DateTime " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "date")
                        {
                            sb3.Append(" Date" + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "bit")
                        {

                            sb3.Append("Boolean " + limpiaGuion(column) + ",");
                        }

                        flagFive++;


                    }
                }
                string cadena = sb3.ToString().Substring(0, sb3.ToString().Length - 1);

                sb2.AppendLine("using MediatR;");
                sb2.AppendLine("using System;");
                sb2.AppendLine("using System.Collections.Generic;");
                sb2.AppendLine("using System.Linq;");
                sb2.AppendLine("using System.Text;");
                sb2.AppendLine("using System.Threading.Tasks;");
                sb2.AppendLine("");
                sb2.AppendLine("namespace " + nameProject + ".Application.Features." + table + "Features.Delete" + table + "");
                sb2.AppendLine("{");
                sb2.AppendLine("    public sealed record Delete" + table + "Request(");
                sb2.AppendLine(cadena);
                sb2.AppendLine(") : IRequest<Delete" + table + "Response>;");
                sb2.AppendLine("");
                sb2.AppendLine("}");

                deleteRequest(sb2, table, ruta, table);

                sb4.AppendLine("using System;");
                sb4.AppendLine("using System.Collections.Generic;");
                sb4.AppendLine("using System.Linq;");
                sb4.AppendLine("using System.Text;");
                sb4.AppendLine("using System.Threading.Tasks;");
                sb4.AppendLine("");
                sb4.AppendLine("namespace " + nameProject + ".Application.Features." + table + "Features.Delete" + table + "");
                sb4.AppendLine("{");
                sb4.AppendLine("    public sealed record Delete" + table + "Response");
                sb4.AppendLine("    {");

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {

                        string column = Convert.ToString(row["COLUMN_NAME"]);
                        string datatype = Convert.ToString(row["DATA_TYPE"]);
                        string maxlenght = Convert.ToString(row["CHARACTER_MAXIMUM_LENGTH"]);
                        string numericPrecision = Convert.ToString(row["NUMERIC_PRECISION"]);
                        string numericScale = Convert.ToString(row["NUMERIC_SCALE"]);

                        /*if (column == primaryString)
                        {
                            sb.AppendLine("[Key]");
                        }*/
                        if (datatype == "int" || datatype == "smallint")
                        {
                            sb4.AppendLine("public int " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "char")
                        {
                            sb4.AppendLine("public char " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "bigint")
                        {
                            sb4.AppendLine("public Int64 " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "varchar")
                        {
                            sb4.AppendLine("public string " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "money" || datatype == "numeric" || datatype == "decimal")
                        {
                            sb4.AppendLine("public decimal " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "datetime" || datatype == "smalldatetime")
                        {
                            sb4.AppendLine("public DateTime " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "date")
                        {
                            sb4.AppendLine("public Date" + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "bit")
                        {
                            sb4.AppendLine("public Boolean " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        flagFive++;
                    }
                }

                sb4.AppendLine("    }");
                sb4.AppendLine("}");

                deleteResponse(sb4, table, ruta, table);

            }
            if (!Directory.Exists(ruta + @"\Get" + table))
            {

                DirectoryInfo diC = Directory.CreateDirectory(ruta + @"\Get" + table);

                StringBuilder sb = new StringBuilder();
                StringBuilder sb1 = new StringBuilder();
                StringBuilder sb2 = new StringBuilder();
                StringBuilder sb3 = new StringBuilder();
                StringBuilder sb4 = new StringBuilder();

                


                sb.AppendLine("using AutoMapper;");
                sb.AppendLine("using " + nameProject + ".Application.Repositories;");
                sb.AppendLine("using " + nameProject + ".Domain.Entities;");
                sb.AppendLine("using MediatR;");
                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Linq;");
                sb.AppendLine("using System.Text;");
                sb.AppendLine("using System.Threading.Tasks;");
                sb.AppendLine("");
                sb.AppendLine("namespace " + nameProject + ".Application.Features." + table + "Features.Get" + table + "");
                sb.AppendLine("{");
                sb.AppendLine("    public sealed class Get" + table + "Handler : IRequestHandler<Get" + table + "Request, Get" + table + "Response>");
                sb.AppendLine("    {");
                sb.AppendLine("        private readonly IUnitOfWork _unitOfWork;");
                sb.AppendLine("        private readonly I" + table + "Repository _" + table.ToLower() + "Repository;");
                sb.AppendLine("        private readonly IMapper _mapper;");
                sb.AppendLine("        public Get" + table + "Handler(IUnitOfWork unitOfWork, I" + table + "Repository " + table.ToLower() + "Repository, IMapper mapper)");
                sb.AppendLine("        {");
                sb.AppendLine("            _unitOfWork = unitOfWork;");
                sb.AppendLine("            _" + table.ToLower() + "Repository = " + table.ToLower() + "Repository;");
                sb.AppendLine("            _mapper = mapper;");
                sb.AppendLine("        }");
                sb.AppendLine("");
                sb.AppendLine("        public async Task<Get" + table + "Response> Handle(Get" + table + "Request request, CancellationToken cancellationToken)");
                sb.AppendLine("        {");
                sb.AppendLine("            var "+ table.ToLower() + " = await _" + table.ToLower() + "Repository.GetById(request."+ columnPrimary + ",cancellationToken);");
                sb.AppendLine("            return _mapper.Map<Get" + table + "Response>(" + table.ToLower() + ");");
                sb.AppendLine("        }");
                sb.AppendLine("    }");
                sb.AppendLine("}");

                getHandler(sb, table, ruta, table);


                sb1.AppendLine("using AutoMapper;");
                sb1.AppendLine("using " + nameProject + ".Domain.Entities;");
                sb1.AppendLine("using System;");
                sb1.AppendLine("using System.Collections.Generic;");
                sb1.AppendLine("using System.Linq;");
                sb1.AppendLine("using System.Text;");
                sb1.AppendLine("using System.Threading.Tasks;");
                sb1.AppendLine("");
                sb1.AppendLine("namespace " + nameProject + ".Application.Features." + table + "Features.Get" + table + "");
                sb1.AppendLine("{");
                sb1.AppendLine("    public class Get" + table + "Mapper : Profile");
                sb1.AppendLine("    {");
                sb1.AppendLine("        public Get" + table + "Mapper()");
                sb1.AppendLine("        {");
                sb1.AppendLine("            CreateMap<Get" + table + "Request, " + table + ">();");
                sb1.AppendLine("            CreateMap<" + table + ", Get" + table + "Response>();");
                sb1.AppendLine("        }");
                sb1.AppendLine("    }");
                sb1.AppendLine("}");

                getMapper(sb1, table, ruta, table);

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

                            sb3.Append("int " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "char")
                        {

                            sb3.Append("char " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "bigint")
                        {
                            sb3.Append(" Int64 " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "varchar")
                        {
                            sb3.Append(" string " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "money" || datatype == "numeric" || datatype == "decimal")
                        {
                            sb3.Append(" decimal " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "datetime" || datatype == "smalldatetime")
                        {
                            sb3.Append(" DateTime " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "date")
                        {
                            sb3.Append(" Date" + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "bit")
                        {

                            sb3.Append("Boolean " + limpiaGuion(column) + ",");
                        }

                        flagFive++;


                    }
                }
                string cadena = sb3.ToString().Substring(0, sb3.ToString().Length - 1);

                sb2.AppendLine("using MediatR;");
                sb2.AppendLine("using System;");
                sb2.AppendLine("using System.Collections.Generic;");
                sb2.AppendLine("using System.Linq;");
                sb2.AppendLine("using System.Text;");
                sb2.AppendLine("using System.Threading.Tasks;");
                sb2.AppendLine("");
                sb2.AppendLine("namespace " + nameProject + ".Application.Features." + table + "Features.Get" + table + "");
                sb2.AppendLine("{");
                sb2.AppendLine("    public sealed record Get" + table + "Request(");
                sb2.AppendLine(cadena);
                sb2.AppendLine(") : IRequest<Get" + table + "Response>;");
                sb2.AppendLine("");
                sb2.AppendLine("}");

                getRequest(sb2, table, ruta, table);

                sb4.AppendLine("using System;");
                sb4.AppendLine("using System.Collections.Generic;");
                sb4.AppendLine("using System.Linq;");
                sb4.AppendLine("using System.Text;");
                sb4.AppendLine("using System.Threading.Tasks;");
                sb4.AppendLine("");
                sb4.AppendLine("namespace " + nameProject + ".Application.Features." + table + "Features.Get" + table + "");
                sb4.AppendLine("{");
                sb4.AppendLine("    public sealed record Get" + table + "Response");
                sb4.AppendLine("    {");

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {

                        string column = Convert.ToString(row["COLUMN_NAME"]);
                        string datatype = Convert.ToString(row["DATA_TYPE"]);
                        string maxlenght = Convert.ToString(row["CHARACTER_MAXIMUM_LENGTH"]);
                        string numericPrecision = Convert.ToString(row["NUMERIC_PRECISION"]);
                        string numericScale = Convert.ToString(row["NUMERIC_SCALE"]);

                        /*if (column == primaryString)
                        {
                            sb.AppendLine("[Key]");
                        }*/
                        if (datatype == "int" || datatype == "smallint")
                        {
                            sb4.AppendLine("public int " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "char")
                        {
                            sb4.AppendLine("public char " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "bigint")
                        {
                            sb4.AppendLine("public Int64 " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "varchar")
                        {
                            sb4.AppendLine("public string " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "money" || datatype == "numeric" || datatype == "decimal")
                        {
                            sb4.AppendLine("public decimal " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "datetime" || datatype == "smalldatetime")
                        {
                            sb4.AppendLine("public DateTime " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "date")
                        {
                            sb4.AppendLine("public Date" + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "bit")
                        {
                            sb4.AppendLine("public Boolean " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        flagFive++;
                    }
                }

                sb4.AppendLine("    }");
                sb4.AppendLine("}");
                getResponse(sb4, table, ruta, table);
            }
            if (!Directory.Exists(ruta + @"\GetAll" + table))
            {

                DirectoryInfo diC = Directory.CreateDirectory(ruta + @"\GetAll" + table);
                StringBuilder sb = new StringBuilder();
                StringBuilder sb1 = new StringBuilder();
                StringBuilder sb2 = new StringBuilder();
                StringBuilder sb3 = new StringBuilder();
                StringBuilder sb4 = new StringBuilder();


                sb.AppendLine("using AutoMapper;");
                sb.AppendLine("using " + nameProject + ".Application.Repositories;");
                sb.AppendLine("using " + nameProject + ".Domain.Entities;");
                sb.AppendLine("using MediatR;");
                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Linq;");
                sb.AppendLine("using System.Text;");
                sb.AppendLine("using System.Threading.Tasks;");
                sb.AppendLine("");
                sb.AppendLine("namespace " + nameProject + ".Application.Features." + table + "Features.GetAll" + table + "");
                sb.AppendLine("{");
                sb.AppendLine("    public sealed class GetAll" + table + "Handler : IRequestHandler<GetAll" + table + "Request, List<GetAll" + table + "Response>>");
                sb.AppendLine("    {");
                sb.AppendLine("        private readonly IUnitOfWork _unitOfWork;");
                sb.AppendLine("        private readonly I" + table + "Repository _" + table.ToLower() + "Repository;");
                sb.AppendLine("        private readonly IMapper _mapper;");
                sb.AppendLine("        public GetAll" + table + "Handler(IUnitOfWork unitOfWork, I" + table + "Repository " + table.ToLower() + "Repository, IMapper mapper)");
                sb.AppendLine("        {");
                sb.AppendLine("            _unitOfWork = unitOfWork;");
                sb.AppendLine("            _" + table.ToLower() + "Repository = " + table.ToLower() + "Repository;");
                sb.AppendLine("            _mapper = mapper;");
                sb.AppendLine("        }");
                sb.AppendLine("");
                sb.AppendLine("        public async Task<List<GetAll" + table + "Response>> Handle(GetAll" + table + "Request request, CancellationToken cancellationToken)");
                sb.AppendLine("        {");
                sb.AppendLine("            var " + table.ToLower() + "s = await _" + table.ToLower() + "Repository.GetAll(cancellationToken);");
                sb.AppendLine("            return _mapper.Map<List<GetAll" + table + "Response>>(" + table.ToLower() + "s);");
                sb.AppendLine("        }");
                sb.AppendLine("    }");
                sb.AppendLine("}");

                getAllHandler(sb, table, ruta, table);


                sb1.AppendLine("using AutoMapper;");
                sb1.AppendLine("using " + nameProject + ".Domain.Entities;");
                sb1.AppendLine("using System;");
                sb1.AppendLine("using System.Collections.Generic;");
                sb1.AppendLine("using System.Linq;");
                sb1.AppendLine("using System.Text;");
                sb1.AppendLine("using System.Threading.Tasks;");
                sb1.AppendLine("");
                sb1.AppendLine("namespace " + nameProject + ".Application.Features." + table + "Features.GetAll" + table + "");
                sb1.AppendLine("{");
                sb1.AppendLine("    public class GetAll" + table + "Mapper : Profile");
                sb1.AppendLine("    {");
                sb1.AppendLine("        public GetAll" + table + "Mapper()");
                sb1.AppendLine("        {");
                sb1.AppendLine("            //CreateMap<GetAll" + table + "Request, " + table + ">();");
                sb1.AppendLine("            CreateMap<" + table + ", GetAll" + table + "Response>();");
                sb1.AppendLine("        }");
                sb1.AppendLine("    }");
                sb1.AppendLine("}");

                getAllMapper(sb1, table, ruta, table);

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

                            sb3.Append("int " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "char")
                        {

                            sb3.Append("char " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "bigint")
                        {
                            sb3.Append(" Int64 " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "varchar")
                        {
                            sb3.Append(" string " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "money" || datatype == "numeric" || datatype == "decimal")
                        {
                            sb3.Append(" decimal " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "datetime" || datatype == "smalldatetime")
                        {
                            sb3.Append(" DateTime " + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "date")
                        {
                            sb3.Append(" Date" + limpiaGuion(column) + ",");
                        }
                        else if (datatype == "bit")
                        {

                            sb3.Append("Boolean " + limpiaGuion(column) + ",");
                        }

                        flagFive++;


                    }
                }
                string cadena = sb3.ToString().Substring(0, sb3.ToString().Length - 1);

                sb2.AppendLine("using MediatR;");
                sb2.AppendLine("using System;");
                sb2.AppendLine("using System.Collections.Generic;");
                sb2.AppendLine("using System.Linq;");
                sb2.AppendLine("using System.Text;");
                sb2.AppendLine("using System.Threading.Tasks;");
                sb2.AppendLine("");
                sb2.AppendLine("namespace " + nameProject + ".Application.Features." + table + "Features.GetAll" + table + "");
                sb2.AppendLine("{");
                sb2.AppendLine("    public sealed record GetAll" + table + "Request(");
                //sb2.AppendLine(cadena);
                sb2.AppendLine(") : IRequest<List<GetAll" + table + "Response>>;");
                sb2.AppendLine("");
                sb2.AppendLine("}");

                getAllRequest(sb2, table, ruta, table);

                sb4.AppendLine("using System;");
                sb4.AppendLine("using System.Collections.Generic;");
                sb4.AppendLine("using System.Linq;");
                sb4.AppendLine("using System.Text;");
                sb4.AppendLine("using System.Threading.Tasks;");
                sb4.AppendLine("");
                sb4.AppendLine("namespace " + nameProject + ".Application.Features." + table + "Features.GetAll" + table + "");
                sb4.AppendLine("{");
                sb4.AppendLine("    public sealed record GetAll" + table + "Response");
                sb4.AppendLine("    {");

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {

                        string column = Convert.ToString(row["COLUMN_NAME"]);
                        string datatype = Convert.ToString(row["DATA_TYPE"]);
                        string maxlenght = Convert.ToString(row["CHARACTER_MAXIMUM_LENGTH"]);
                        string numericPrecision = Convert.ToString(row["NUMERIC_PRECISION"]);
                        string numericScale = Convert.ToString(row["NUMERIC_SCALE"]);

                        /*if (column == primaryString)
                        {
                            sb.AppendLine("[Key]");
                        }*/
                        if (datatype == "int" || datatype == "smallint")
                        {
                            sb4.AppendLine("public int " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "char")
                        {
                            sb4.AppendLine("public char " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "bigint")
                        {
                            sb4.AppendLine("public Int64 " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "varchar")
                        {
                            sb4.AppendLine("public string " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "money" || datatype == "numeric" || datatype == "decimal")
                        {
                            sb4.AppendLine("public decimal " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "datetime" || datatype == "smalldatetime")
                        {
                            sb4.AppendLine("public DateTime " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "date")
                        {
                            sb4.AppendLine("public Date" + limpiaGuion(column) + "  {get ; set; }");
                        }
                        else if (datatype == "bit")
                        {
                            sb4.AppendLine("public Boolean " + limpiaGuion(column) + "  {get ; set; }");
                        }
                        flagFive++;
                    }
                }
                sb4.AppendLine("    }");
                sb4.AppendLine("}");
                getAllResponse(sb4, table, ruta, table);

            }

            //createInterfaceRepository(sb, table);
        }

        public void createEntity(StringBuilder structura, String nombre)
        {

            System.IO.File.WriteAllText(pathScripts + nombre + ".cs", structura.ToString());
        }
        public void createInterfaceRepository(StringBuilder structura, String nombre)
        {

            System.IO.File.WriteAllText(pathScripts + "I"+nombre+ "Repository.cs", structura.ToString());
        }
        public void createHandler(StringBuilder structura, String nombre,string ruta,string table)
        {

            System.IO.File.WriteAllText(ruta + @"\Create" + table +@"\Create"+ table + "Handler.cs", structura.ToString());
        }
        public void createMapper(StringBuilder structura, String nombre, string ruta, string table)
        {

            System.IO.File.WriteAllText(ruta + @"\Create" + table + @"\Create" + table + "Mapper.cs", structura.ToString());
        }
        public void createRequest(StringBuilder structura, String nombre, string ruta, string table)
        {

            System.IO.File.WriteAllText(ruta + @"\Create" + table + @"\Create" + table + "Request.cs", structura.ToString());
        }
        public void createResponse(StringBuilder structura, String nombre, string ruta, string table)
        {

            System.IO.File.WriteAllText(ruta + @"\Create" + table + @"\Create" + table + "Response.cs", structura.ToString());
        }
        public void repository(StringBuilder structura, String nombre, string ruta, string table)
        {

            System.IO.File.WriteAllText(pathScripts + table + "Repository.cs", structura.ToString());
        }
        public void controller(StringBuilder structura, String nombre, string ruta, string table)
        {

            System.IO.File.WriteAllText(pathScripts + table + "Controller.cs", structura.ToString());
        }

        public void updateHandler(StringBuilder structura, String nombre, string ruta, string table)
        {

            System.IO.File.WriteAllText(ruta + @"\Update" + table + @"\Update" + table + "Handler.cs", structura.ToString());
        }
        public void updateMapper(StringBuilder structura, String nombre, string ruta, string table)
        {

            System.IO.File.WriteAllText(ruta + @"\Update" + table + @"\Update" + table + "Mapper.cs", structura.ToString());
        }
        public void updateRequest(StringBuilder structura, String nombre, string ruta, string table)
        {

            System.IO.File.WriteAllText(ruta + @"\Update" + table + @"\Update" + table + "Request.cs", structura.ToString());
        }
        public void updateResponse(StringBuilder structura, String nombre, string ruta, string table)
        {

            System.IO.File.WriteAllText(ruta + @"\Update" + table + @"\Update" + table + "Response.cs", structura.ToString());
        }


        public void deleteHandler(StringBuilder structura, String nombre, string ruta, string table)
        {

            System.IO.File.WriteAllText(ruta + @"\Delete" + table + @"\Delete" + table + "Handler.cs", structura.ToString());
        }
        public void deleteMapper(StringBuilder structura, String nombre, string ruta, string table)
        {

            System.IO.File.WriteAllText(ruta + @"\Delete" + table + @"\Delete" + table + "Mapper.cs", structura.ToString());
        }
        public void deleteRequest(StringBuilder structura, String nombre, string ruta, string table)
        {

            System.IO.File.WriteAllText(ruta + @"\Delete" + table + @"\Delete" + table + "Request.cs", structura.ToString());
        }
        public void deleteResponse(StringBuilder structura, String nombre, string ruta, string table)
        {

            System.IO.File.WriteAllText(ruta + @"\Delete" + table + @"\Delete" + table + "Response.cs", structura.ToString());
        }

        public void getHandler(StringBuilder structura, String nombre, string ruta, string table)
        {

            System.IO.File.WriteAllText(ruta + @"\Get" + table + @"\Get" + table + "Handler.cs", structura.ToString());
        }
        public void getMapper(StringBuilder structura, String nombre, string ruta, string table)
        {

            System.IO.File.WriteAllText(ruta + @"\Get" + table + @"\Get" + table + "Mapper.cs", structura.ToString());
        }
        public void getRequest(StringBuilder structura, String nombre, string ruta, string table)
        {

            System.IO.File.WriteAllText(ruta + @"\Get" + table + @"\Get" + table + "Request.cs", structura.ToString());
        }
        public void getResponse(StringBuilder structura, String nombre, string ruta, string table)
        {

            System.IO.File.WriteAllText(ruta + @"\Get" + table + @"\Get" + table + "Response.cs", structura.ToString());
        }

        public void getAllHandler(StringBuilder structura, String nombre, string ruta, string table)
        {

            System.IO.File.WriteAllText(ruta + @"\GetAll" + table + @"\GetAll" + table + "Handler.cs", structura.ToString());
        }
        public void getAllMapper(StringBuilder structura, String nombre, string ruta, string table)
        {

            System.IO.File.WriteAllText(ruta + @"\GetAll" + table + @"\GetAll" + table + "Mapper.cs", structura.ToString());
        }
        public void getAllRequest(StringBuilder structura, String nombre, string ruta, string table)
        {

            System.IO.File.WriteAllText(ruta + @"\GetAll" + table + @"\GetAll" + table + "Request.cs", structura.ToString());
        }
        public void getAllResponse(StringBuilder structura, String nombre, string ruta, string table)
        {

            System.IO.File.WriteAllText(ruta + @"\GetAll" + table + @"\GetAll" + table + "Response.cs", structura.ToString());
        }
    }
}

    
