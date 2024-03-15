using CreateScriptDatabase.Class;
using CreateScriptDatabase.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateScriptDatabase.Template
{
    public class FormCrudReact
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

        public String formReactCru(DataSet ds, String table, String conexion, List<Field> fields)
        {

            StringBuilder sbs = new StringBuilder();
            cls_sql sql = new cls_sql();
            String primaryString = "";
            DataTable dt = ds.Tables[0];

            List<String> primary = new List<string>();
            primary = sql.obteniendoPrimaryKey(table, conexion);
            primaryString = primary[0];

            sbs.AppendLine("import axios from 'axios';");
            sbs.AppendLine("import React, { Component } from 'react';");
            sbs.AppendLine("import {");
            sbs.AppendLine("    Table, Switch, Space,");
            sbs.AppendLine("    Form,");
            sbs.AppendLine("    Input,");
            sbs.AppendLine("    Button,");
            sbs.AppendLine("    Radio,");
            sbs.AppendLine("    Select,");
            sbs.AppendLine("    Cascader,");
            sbs.AppendLine("    DatePicker,");
            sbs.AppendLine("    InputNumber,");
            sbs.AppendLine("    TreeSelect, Divider, Row, Col");
            sbs.AppendLine("} from 'antd';");

            //columnas
            sbs.AppendLine("const columns = [");
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string column = Convert.ToString(row["COLUMN_NAME"]);
                    sbs.AppendLine("    {");
                    sbs.AppendLine("        title: '" + limpiaGuion(column) + "',");
                    sbs.AppendLine("        dataIndex: '" + limpiaGuion(column) + "',");
                    sbs.AppendLine("        render: (text) => <a>{text}</a>,");
                    sbs.AppendLine("    },");

                }
            }
            sbs.AppendLine("];");
            sbs.AppendLine("class " + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + " extends Component {");
            sbs.AppendLine("    state = {");
            sbs.AppendLine("        selectedRowKeys: [],");
            sbs.AppendLine("        data: [],");
            sbs.AppendLine("        columns: [],");
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow row in dt.Rows)
                {
                    string column = Convert.ToString(row["COLUMN_NAME"]);
                    string tipodato = Convert.ToString(row["DATA_TYPE"]);


                    if (tipodato == "varchar")
                    {

                        sbs.AppendLine("        " + limpiaGuion(column) + ": \"\",");
                    }
                    if (tipodato == "int")
                    {

                        sbs.AppendLine("        " + limpiaGuion(column) + ": 0,");
                    }
                    else if (tipodato == "bit")
                    {
                        sbs.AppendLine("        " + limpiaGuion(column) + ": false,");
                    }
                }
            }
            /*sbs.AppendLine("        codigo: \"\",");
            sbs.AppendLine("        valor: \"\",");
            sbs.AppendLine("        activo: false,");*/

            sbs.AppendLine("        disableControl: false,");
            sbs.AppendLine("        disableNuevo: false,");
            sbs.AppendLine("        disableEditar: false,");
            sbs.AppendLine("        disableGuardar: false,");
            sbs.AppendLine("        disableEliminar: false,");
            sbs.AppendLine("        disableCancelar: false,");
            sbs.AppendLine("        flagNuevo:false");
            sbs.AppendLine("");
            sbs.AppendLine("    };");
            sbs.AppendLine("    componentDidMount() {");
            sbs.AppendLine("        this.getInformation();");
            sbs.AppendLine("        this.setState({ disableControl: true });");
            sbs.AppendLine("        this.setState({ disableNuevo: false });//puede crear registro nuevo");
            sbs.AppendLine("        this.setState({ disableEditar: true });//puede crear registro nuevo");
            sbs.AppendLine("        this.setState({ disableGuardar: true });//puede crear registro nuevo");
            sbs.AppendLine("        this.setState({ disableEliminar: true });//puede crear registro nuevo");
            sbs.AppendLine("        this.setState({ disableCancelar: true });//puede crear registro nuevo");
            sbs.AppendLine("    };");
            sbs.AppendLine("    getInformation = async () => {");
            sbs.AppendLine("        await axios.get('https://localhost:44315/api/Iva/getIva')");
            sbs.AppendLine("            .then(response => {");
            sbs.AppendLine("                var options = response.data.map(function (row) {");
            sbs.AppendLine("                    return { ");
            string cadena = "";
            string cadenaState = "";
            sbs.AppendLine("  \"key\":primaria,");
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow row in dt.Rows)
                {
                    string column = Convert.ToString(row["COLUMN_NAME"]);
                    
                    sbs.AppendLine("  \"" + limpiaGuion(column) + "\":row." + limpiaGuion(column) + ",");
                    cadena = cadena+limpiaGuion(column) + ",";
                    cadenaState= cadenaState + "this.state."+limpiaGuion(column) + ",";
                }
            }
            sbs.AppendLine(" }"); ;
            sbs.AppendLine("                })");
            sbs.AppendLine("                this.setState({ data: options });");
            sbs.AppendLine("                console.log('data', options);");
            sbs.AppendLine("            }");
            sbs.AppendLine("            )");
            sbs.AppendLine("    };");
            sbs.AppendLine("    editInformation = async (" + cadena.TrimEnd(',') + ") => {");
            sbs.AppendLine("        const param = {");
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow row in dt.Rows)
                {
                    string column = Convert.ToString(row["COLUMN_NAME"]);
                    sbs.AppendLine("            \"" + limpiaGuion(column) + "\": " + limpiaGuion(column) + ",");
                }
            }
            sbs.AppendLine("        }");
            sbs.AppendLine("        console.log('param', param);");
            sbs.AppendLine("        await axios.put('https://localhost:44315/api/Iva/update', param)");
            sbs.AppendLine("            .then(response => {");
            sbs.AppendLine("                console.log('data', response);");
            sbs.AppendLine("            }");
            sbs.AppendLine("            )");
            sbs.AppendLine("    };");
            sbs.AppendLine("    insertInformation = async ("+ cadena.TrimEnd(',') + ") => {");
            sbs.AppendLine("        const param = {");
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow row in dt.Rows)
                {
                    string column = Convert.ToString(row["COLUMN_NAME"]);
                    sbs.AppendLine("            \"" + limpiaGuion(column) + "\": " + limpiaGuion(column) + ",");
                }
            }
            sbs.AppendLine("        }");
            sbs.AppendLine("        await axios.post('https://localhost:44315/api/Iva/insert', param)");
            sbs.AppendLine("            .then(response => {");
            sbs.AppendLine("                this.setState({ infoContract: response.data });");
            sbs.AppendLine("                console.log('data', response);");
            sbs.AppendLine("            }");
            sbs.AppendLine("            )");
            sbs.AppendLine("    };");
            sbs.AppendLine("    onSelectChange = (selectedRowKeys, selectedRows) => {");
            sbs.AppendLine("        if (selectedRowKeys.length > 1) {");
            sbs.AppendLine("            const lastSelectedRowIndex = [...selectedRowKeys].pop();");
            sbs.AppendLine("            this.setState({ selectedRowKeys: lastSelectedRowIndex });");
            sbs.AppendLine("        }");
            sbs.AppendLine("        this.setState({ selectedRowKeys });");
            sbs.AppendLine("        console.log('selectedRowKeys changed: ', selectedRowKeys);");
            sbs.AppendLine("        console.log('value changed: ', selectedRows);");

            /*sbs.AppendLine("        this.setState({ codigo: selectedRows[0].codeIva });");
            sbs.AppendLine("        this.setState({ valor: selectedRows[0].percentageIva });");
            sbs.AppendLine("        this.setState({ activo: selectedRows[0].isavailableIva });");*/
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow row in dt.Rows)
                {
                    string column = Convert.ToString(row["COLUMN_NAME"]);
                    string tipodato = Convert.ToString(row["DATA_TYPE"]);
                    sbs.AppendLine("        this.setState({ "+ limpiaGuion(column) + ": selectedRows[0]."+limpiaGuion(column)+" });");

                }
            }


                    sbs.AppendLine("        this.setState({ disableNuevo: true });");
            sbs.AppendLine("        this.setState({ disableEditar: false });");
            sbs.AppendLine("        this.setState({ disableGuardar: true });");
            sbs.AppendLine("        this.setState({ disableEliminar: true });");
            sbs.AppendLine("        this.setState({ disableCancelar: false });");
            sbs.AppendLine("        this.setState({ flagNuevo: false });");
            sbs.AppendLine("        console.log('vcodigo ', this.state.codigo);");
            sbs.AppendLine("    };");

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {

                foreach (DataRow row in dt.Rows)
                {
                    string column = Convert.ToString(row["COLUMN_NAME"]);
                    string tipodato = Convert.ToString(row["DATA_TYPE"]);

                    if (tipodato == "varchar")
                    {
                        sbs.AppendLine("    onChange" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(column)) + "  = (e) => {");
                        sbs.AppendLine("        this.setState({ " + limpiaGuion(column) + ": e.target.value })");
                        sbs.AppendLine("    }");

                    }
                    if (tipodato == "int" || tipodato == "money" || tipodato == "numeric" || tipodato == "decimal")
                    {
                        sbs.AppendLine("    onChange" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(column)) + "  = (e) => {");
                        sbs.AppendLine("        this.setState({ " + limpiaGuion(column) + ": e.target.value })");
                        sbs.AppendLine("    }");

                    }
                    else if (tipodato == "bit")
                    {
                        sbs.AppendLine("    onChange" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(column)) + " = (checked) => {");
                        sbs.AppendLine("        console.log(`switch to ${checked}`);");
                        sbs.AppendLine("        this.setState({ " + limpiaGuion(column) + ": checked})");
                        sbs.AppendLine("    };");
                    }
                    else if (tipodato == "date" || tipodato == "datetime" || tipodato == "smalldatetime")
                    {
                        sbs.AppendLine("    onChange" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(column)) + " = (date, dateString) => {");
                        sbs.AppendLine("        console.log(date, dateString);");
                        sbs.AppendLine("        this.setState({ " + limpiaGuion(column) + ": date})");
                        sbs.AppendLine("    };");
                    }
                }

                /*sbs.AppendLine("    onChange = (checked) => {");
                sbs.AppendLine("        console.log(`switch to ${checked}`);");
                sbs.AppendLine("    };");
                sbs.AppendLine("    onChangeCodigo = (e) => {");
                sbs.AppendLine("        this.setState({ codigo: e.target.value })");
                sbs.AppendLine("    }");
                sbs.AppendLine("    onChangeValor = (e) => {");
                sbs.AppendLine("        this.setState({ valor: e.target.value })");
                sbs.AppendLine("    }");*/

                sbs.AppendLine("    clickNuevo = () => {");
                sbs.AppendLine("        this.setState({ flagNuevo:true});");
                sbs.AppendLine("        this.setState({ disableControl: false });");
                sbs.AppendLine("        this.setState({ disableNuevo: true });");
                sbs.AppendLine("        this.setState({ disableEditar: true });");
                sbs.AppendLine("        this.setState({ disableGuardar: false });");
                sbs.AppendLine("        this.setState({ disableCancelar: false });");
                sbs.AppendLine("    };");
                sbs.AppendLine("    clickEditar = () => {");
                sbs.AppendLine("        this.setState({ disableControl: false });");
                sbs.AppendLine("        this.setState({ disableEditar: true });");
                sbs.AppendLine("        this.setState({ disableGuardar: false });");
                sbs.AppendLine("        this.setState({ disableEliminar: true });");
                sbs.AppendLine("        this.setState({ disableCancelar: false });");
                sbs.AppendLine("    };");
                sbs.AppendLine("    clickGuardar = async () => {");
                sbs.AppendLine("        if (this.state.flagNuevo == false) {");
                sbs.AppendLine("            await this.editInformation("+ cadenaState.TrimEnd(',') + ");");
                sbs.AppendLine("            this.getInformation();");
                sbs.AppendLine("            this.setState({ disableControl: true });");
                sbs.AppendLine("            this.setState({ disableNuevo: false });");
                sbs.AppendLine("            this.setState({ disableEditar: true });");
                sbs.AppendLine("            this.setState({ disableGuardar: true });");
                sbs.AppendLine("            this.setState({ disableEliminar: true });");
                sbs.AppendLine("            this.setState({ disableCancelar: true });");
                sbs.AppendLine("        }");
                sbs.AppendLine("        else {");
                sbs.AppendLine("            await this.insertInformation("+ cadenaState.TrimEnd(',') + ");");
                sbs.AppendLine("            this.getInformation();");
                sbs.AppendLine("            this.setState({ disableControl: true });");
                sbs.AppendLine("            this.setState({ disableNuevo: false });");
                sbs.AppendLine("            this.setState({ disableGuardar: true });");
                sbs.AppendLine("            this.setState({ disableCancelar: true });");
                sbs.AppendLine("        }");
                sbs.AppendLine("    };");
                sbs.AppendLine("    clickEliminar = () => {");
                sbs.AppendLine("        this.setState({ disableControl: false });");
                sbs.AppendLine("    };");
                sbs.AppendLine("    clickCancelar = () => {");
                sbs.AppendLine("        this.setState({ disableControl: true });");
                sbs.AppendLine("        this.setState({ disableNuevo: false });");
                sbs.AppendLine("        this.setState({ disableEditar: true });");
                sbs.AppendLine("        this.setState({ disableGuardar: true });");
                sbs.AppendLine("        this.setState({ disableCancelar: true });");
                sbs.AppendLine("    };");
                sbs.AppendLine("    render() {");
                sbs.AppendLine("        const { selectedRowKeys } = this.state;");
                sbs.AppendLine("        const rowSelection = {");
                sbs.AppendLine("            selectedRowKeys,");
                sbs.AppendLine("            onChange: this.onSelectChange,");
                sbs.AppendLine("        };");
                sbs.AppendLine("        return (");
                sbs.AppendLine("            <div>");
                sbs.AppendLine("                <Form");
                sbs.AppendLine("                    labelCol={{ span: 4 }}");
                sbs.AppendLine("                    wrapperCol={{ span: 14 }}");
                sbs.AppendLine("                    layout=\"horizontal\"");
                sbs.AppendLine("                    size=\"default\"");
                sbs.AppendLine("                >");
                sbs.AppendLine("                    <Table");
                sbs.AppendLine("                        rowSelection={{");
                sbs.AppendLine("                            type: 'radio',");
                sbs.AppendLine("                            ...rowSelection");
                sbs.AppendLine("                        }}");
                sbs.AppendLine("                        type={Radio}");
                sbs.AppendLine("                        columns={columns}");
                sbs.AppendLine("                        dataSource={this.state.data}");
                sbs.AppendLine("                        pagination={3}");
                sbs.AppendLine("                        size={5}");
                sbs.AppendLine("                    />");
                sbs.AppendLine("                    <Divider />");

                /*sbs.AppendLine("                    <Form.Item label=\"Codigo\">");
                sbs.AppendLine("                        <Input");
                sbs.AppendLine("                            disabled={this.state.disableControl}");
                sbs.AppendLine("                            onChange={this.onChangeCodigo} />");
                sbs.AppendLine("                    </Form.Item>");
                sbs.AppendLine("                    <Form.Item label=\"Valor\">");
                sbs.AppendLine("                        <Input");
                sbs.AppendLine("                            disabled={this.state.disableControl}");
                sbs.AppendLine("                            value={this.state.valor || ''}");
                sbs.AppendLine("                            onChange={this.onChangeValor} />");
                sbs.AppendLine("                    </Form.Item>");
                sbs.AppendLine("                    <Form.Item label=\"Activo\" valuePropName=\"checked\">");
                sbs.AppendLine("                        <Switch");
                sbs.AppendLine("                            onChange={this.onChange}");
                sbs.AppendLine("                            checked={this.state.activo}");
                sbs.AppendLine("                            disabled={this.state.disableControl}");
                sbs.AppendLine("                        />");*/

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {

                    foreach (DataRow row in dt.Rows)
                    {
                        string column = Convert.ToString(row["COLUMN_NAME"]);
                        string tipodato = Convert.ToString(row["DATA_TYPE"]);

                        if (tipodato == "varchar")
                        {
                            sbs.AppendLine("                    <Form.Item label=\"" + limpiaGuion(column) + "\">");
                            sbs.AppendLine("                        <Input");
                            sbs.AppendLine("                            disabled={this.state.disableControl}");
                            sbs.AppendLine("                            value ={ this.state."+ limpiaGuion(column) +" || ''}");
                            sbs.AppendLine("                            onChange={this.onChange" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(column)) + "} />");
                            sbs.AppendLine("                    </Form.Item>");
                        }
                        else if (tipodato == "int")
                        {
                            sbs.AppendLine("                    <Form.Item label=\"" + limpiaGuion(column) + "\">");
                            sbs.AppendLine("                        <Input");
                            sbs.AppendLine("                            disabled={this.state.disableControl}");
                            sbs.AppendLine("                            onChange={this.onChange" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(column)) + "} />");
                            sbs.AppendLine("                    </Form.Item>");
                        }
                        else if (tipodato == "bigint" ||  tipodato == "money" || tipodato == "numeric" || tipodato == "decimal")
                        {
                            sbs.AppendLine("                    <Form.Item label=\"" + limpiaGuion(column) + "\">");
                            sbs.AppendLine("                        <Input");
                            sbs.AppendLine("                            disabled={this.state.disableControl}");
                            sbs.AppendLine("                            onChange={this.onChange" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(column)) + "} />");
                            sbs.AppendLine("                    </Form.Item>");
                        }
                        /*else if (tipodato == "varchar")
                        {
                            sbs.AppendLine("                    <Form.Item label=\"" + limpiaGuion(column) + "\">");
                            sbs.AppendLine("                        <Input");
                            sbs.AppendLine("                            disabled={this.state.disableControl}");
                            sbs.AppendLine("                            onChange={this.onChange" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(column)) + "} />");
                            sbs.AppendLine("                    </Form.Item>");
                        }*/
                        else if (tipodato == "bit")
                        {
                            sbs.AppendLine("                    <Form.Item label=\"Activo\" valuePropName=\"checked\">");
                            sbs.AppendLine("                        <Switch");
                            sbs.AppendLine("                            onChange={this.onChange" + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(column)) + "}");
                            sbs.AppendLine("                            checked={this.state." + limpiaGuion(column) + " }");
                            sbs.AppendLine("                            disabled={this.state.disableControl}");
                            sbs.AppendLine("                        />");
                            sbs.AppendLine("                    </Form.Item>");

                        }
                        else if (tipodato == "smalldatetime" || tipodato == "date" || tipodato == "datetime")
                        {
                            sbs.AppendLine("                    < Form.Item label =\"" + limpiaGuion(column) + "\">");
                            sbs.AppendLine("                    < DatePicker />");
                            sbs.AppendLine("                    </ Form.Item >");
                        }
                       
                        else if (tipodato == "OptionButton")
                        {

                        }
                    }
                }
                
                sbs.AppendLine("                    <Row gutter={24}>");
                sbs.AppendLine("                        <Col span={3}>");
                sbs.AppendLine("                            <Form.Item >");
                sbs.AppendLine("                                <Button");
                sbs.AppendLine("                                    onClick={() => this.clickNuevo()}");
                sbs.AppendLine("                                    disabled={this.state.disableNuevo}");
                sbs.AppendLine("                                >Nuevo</Button>");
                sbs.AppendLine("                            </Form.Item>");
                sbs.AppendLine("                        </Col>");
                sbs.AppendLine("                        <Col span={3}>");
                sbs.AppendLine("                            <Form.Item >");
                sbs.AppendLine("                                <Button onClick={() => this.clickEditar()}");
                sbs.AppendLine("                                    disabled={this.state.disableEditar}");
                sbs.AppendLine("                                >Editar</Button>");
                sbs.AppendLine("                            </Form.Item>");
                sbs.AppendLine("                        </Col>");
                sbs.AppendLine("                        <Col span={3}>");
                sbs.AppendLine("                            <Form.Item >");
                sbs.AppendLine("                                <Button onClick={() => this.clickGuardar()}");
                sbs.AppendLine("                                    disabled={this.state.disableGuardar}");
                sbs.AppendLine("                                >Guardar</Button>");
                sbs.AppendLine("                            </Form.Item>");
                sbs.AppendLine("                        </Col>");
                sbs.AppendLine("                        <Col span={3}>");
                sbs.AppendLine("                            <Form.Item >");
                sbs.AppendLine("                                <Button");
                sbs.AppendLine("                                    disabled={this.state.disableEliminar}");
                sbs.AppendLine("                                >Eliminar</Button>");
                sbs.AppendLine("                            </Form.Item>");
                sbs.AppendLine("                        </Col>");
                sbs.AppendLine("                        <Col span={2}>");
                sbs.AppendLine("                            <Form.Item >");
                sbs.AppendLine("                                <Button onClick={() => this.clickCancelar()}");
                sbs.AppendLine("                                    disabled={this.state.disableCancelar}");
                sbs.AppendLine("                                >Cancelar</Button>");
                sbs.AppendLine("                            </Form.Item>");
                sbs.AppendLine("                        </Col>");
                sbs.AppendLine("                    </Row>");
                sbs.AppendLine("                </Form>");
                sbs.AppendLine("            </div>");
                sbs.AppendLine("        );");
                sbs.AppendLine("    }");
                sbs.AppendLine("}");
                sbs.AppendLine("export default " + ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)) + ";");
                createFileSql(sbs.ToString(),ConvertirPrimeraLetraEnMayuscula(limpiaGuion(table)));

            }



            return "";
        }
        public String formReactCrud_(DataSet ds, String table, String conexion, List<Field> fields)
        {

            StringBuilder sbs = new StringBuilder();
            cls_sql sql = new cls_sql();
            String primaryString = "";
            DataTable dt = ds.Tables[0];

            List<String> primary = new List<string>();
            primary = sql.obteniendoPrimaryKey(table, conexion);
            primaryString = primary[0];
            //String id = 0;
            sbs.AppendLine("import axios from 'axios';");
            sbs.AppendLine("import React, { Component } from 'react';");
            sbs.AppendLine("import {");
            sbs.AppendLine("  DataGrid, GridToolbarContainer, GridToolbarExport, GridToolbar, GridToolbarDensitySelector,");
            sbs.AppendLine("  GridToolbarFilterButton");
            sbs.AppendLine("} from '@mui/x-data-grid';");
            sbs.AppendLine("import PropTypes from 'prop-types';");
            sbs.AppendLine("import IconButton from '@material-ui/core/IconButton';");
            sbs.AppendLine("import TextField from '@material-ui/core/TextField';");
            sbs.AppendLine("import ClearIcon from '@material-ui/icons/Clear';");
            sbs.AppendLine("import SearchIcon from '@material-ui/icons/Search';");
            sbs.AppendLine("import { createTheme } from '@material-ui/core/styles';");
            sbs.AppendLine("import { makeStyles } from '@material-ui/styles';");
            sbs.AppendLine("import Button from '@material-ui/core/Button';");
            sbs.AppendLine("import Dialog from '@material-ui/core/Dialog';");
            sbs.AppendLine("import DialogActions from '@material-ui/core/DialogActions';");
            sbs.AppendLine("import DialogContent from '@material-ui/core/DialogContent';");
            sbs.AppendLine("import DialogContentText from '@material-ui/core/DialogContentText';");
            sbs.AppendLine("import DialogTitle from '@material-ui/core/DialogTitle';");
            sbs.AppendLine("function escapeRegExp(value) {");
            sbs.AppendLine(@"  return value.replace(/[-[\]{}()*+?.,\\^$|#\s]/g, '\\$&');");
            sbs.AppendLine("}");
            sbs.AppendLine("const defaultTheme = createTheme();");
            sbs.AppendLine("const useStyles = makeStyles(");
            sbs.AppendLine("  (theme) => ({");
            sbs.AppendLine("    root: {");
            sbs.AppendLine("      padding: theme.spacing(0.5, 0.5, 0),");
            sbs.AppendLine("      justifyContent: 'space-between',");
            sbs.AppendLine("      display: 'flex',");
            sbs.AppendLine("      alignItems: 'flex-start',");
            sbs.AppendLine("      flexWrap: 'wrap',");
            sbs.AppendLine("    },");
            sbs.AppendLine("    textField: {");
            sbs.AppendLine("      [theme.breakpoints.down('xs')]: {");
            sbs.AppendLine("        width: '100%',");
            sbs.AppendLine("      },");
            sbs.AppendLine("      margin: theme.spacing(1, 0.5, 1.5),");
            sbs.AppendLine("      '& .MuiSvgIcon-root': {");
            sbs.AppendLine("        marginRight: theme.spacing(0.5),");
            sbs.AppendLine("      },");
            sbs.AppendLine("      '& .MuiInput-underline:before': {");
            sbs.AppendLine("        borderBottom: `1px solid ${theme.palette.divider}`,");
            sbs.AppendLine("      },");
            sbs.AppendLine("    },");
            sbs.AppendLine("  }),");
            sbs.AppendLine("  { defaultTheme },");
            sbs.AppendLine(");");
            sbs.AppendLine("function QuickSearchToolbar(props) {");
            sbs.AppendLine("  const classes = useStyles();");
            sbs.AppendLine("  return (");
            sbs.AppendLine("    <div className={classes.root}>");
            sbs.AppendLine("      <div>");
            sbs.AppendLine("      <GridToolbarExport />");
            sbs.AppendLine("        {/*<GridToolbarFilterButton />");
            sbs.AppendLine("        <GridToolbarDensitySelector />*/}");
            sbs.AppendLine("      </div>");
            sbs.AppendLine("      <TextField");
            sbs.AppendLine("        variant=\"standard\"");
            sbs.AppendLine("        value={props.value}");
            sbs.AppendLine("        onChange={props.onChange}");
            sbs.AppendLine("        placeholder=\"Search…\"");
            sbs.AppendLine("        className={classes.textField}");
            sbs.AppendLine("        InputProps={{");
            sbs.AppendLine("          startAdornment: <SearchIcon fontSize=\"small\" />,");
            sbs.AppendLine("          endAdornment: (");
            sbs.AppendLine("            <IconButton");
            sbs.AppendLine("              title=\"Clear\"");
            sbs.AppendLine("              aria-label= \"Clear\"");
            sbs.AppendLine("              size=\"small\"");
            sbs.AppendLine("              style={{ visibility: props.value ? 'visible' : 'hidden' }}");
            sbs.AppendLine("              onClick={props.clearSearch}");
            sbs.AppendLine("            >");
            sbs.AppendLine("              <ClearIcon fontSize=\"small\" />");
            sbs.AppendLine("            </IconButton>");
            sbs.AppendLine("          ),");
            sbs.AppendLine("        }}");
            sbs.AppendLine("      />");
            sbs.AppendLine("    </div>");
            sbs.AppendLine("  );");
            sbs.AppendLine("}");
            sbs.AppendLine("QuickSearchToolbar.propTypes = {");
            sbs.AppendLine("  clearSearch: PropTypes.func.isRequired,");
            sbs.AppendLine("  onChange: PropTypes.func.isRequired,");
            sbs.AppendLine("  value: PropTypes.string.isRequired,");
            sbs.AppendLine("};");
            sbs.AppendLine("function CustomToolbar() {");
            sbs.AppendLine("  return (");
            sbs.AppendLine("    <GridToolbarContainer>");
            sbs.AppendLine("      <GridToolbarExport />");
            sbs.AppendLine("    </GridToolbarContainer>");
            sbs.AppendLine("  );");
            sbs.AppendLine("}");

            sbs.AppendLine("");
            sbs.AppendLine("const columns = [");
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    sbs.AppendLine("{");
                    string column = Convert.ToString(row["COLUMN_NAME"]);
                    sbs.AppendLine("name: '" + column + "'");
                    sbs.AppendLine("label: '" + column + "'");
                    sbs.AppendLine("options: {");
                    sbs.AppendLine("filter: true,");
                    sbs.AppendLine("sort: true,");
                    sbs.AppendLine("},");
                }
            }
            sbs.AppendLine("class ProcesoSPI extends Component {");
            sbs.AppendLine("  state = {");
            sbs.AppendLine("    selectedFile: null,");
            sbs.AppendLine("    informationFile: null,");
            sbs.AppendLine("    cabeceraFecha: '',");
            sbs.AppendLine("    cabeceraRegistros: '',");
            sbs.AppendLine("    cabeceraValor: '',");
            sbs.AppendLine("    cabeceraEstado: '',");
            sbs.AppendLine("    arrayInformationBody: [],");
            sbs.AppendLine("    searchText: '',");
            sbs.AppendLine("    rows: null,");
            sbs.AppendLine("    open:false,");
            sbs.AppendLine("    infoContract:[]");
            sbs.AppendLine("  };");
            sbs.AppendLine("");
            sbs.AppendLine("async componentDidMount() {");
            sbs.AppendLine("    ");
            sbs.AppendLine("");
            sbs.AppendLine("  }");
            sbs.AppendLine("");
            sbs.AppendLine("validateContract = async (contractNumber,id) => {");
            sbs.AppendLine("    const param={");
            sbs.AppendLine("      \"contractNumber\": contractNumber,");
            sbs.AppendLine("      \"id\":id");
            sbs.AppendLine("    }");
            sbs.AppendLine("");
            sbs.AppendLine("    await axios.post('https://localhost:44357/Contract/contingencySPI3',param)");
            sbs.AppendLine("    .then(response=> {");
            sbs.AppendLine("      this.setState({ infoContract: response.data[0]});");
            sbs.AppendLine("      console.log('data',response.data[0].balanceAvailable);");
            sbs.AppendLine("    }");
            sbs.AppendLine("      )");
            sbs.AppendLine("");
            sbs.AppendLine("  };");
            sbs.AppendLine("render() {");
            sbs.AppendLine(" <div style={{ height: 700, width: '100%' }}>");

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string column = Convert.ToString(row["COLUMN_NAME"]);

                    var infoColumns = from s in fields
                                      where s.Columna == column
                                      select new { s.Tipo };

                    /*foreach (String item in infoColumns.ToList())
                    {
                        //item;
                    }*/
                    string tipoComponente = infoColumns.First().Tipo.ToString();



                    if (tipoComponente == "Texto")
                    {
                        sbs.AppendLine("< TextField id = \"outlined -basic\" label = \"Outlined\" variant = \"outlined\" />");
                    }
                    else if (tipoComponente == "Combobox")
                    {
                        sbs.AppendLine("< InputLabel id = \"demo -simple-select-label\" > Age </ InputLabel >");
                        sbs.AppendLine("<Select");
                        sbs.AppendLine("    labelId=\"demo - simple - select - label\"");
                        sbs.AppendLine("    id=\"demo - simple - select\"");
                        sbs.AppendLine("    value=\"holadddddddddddd\"");
                        sbs.AppendLine("    label=\"Age\"");
                        sbs.AppendLine("    //onChange={handleChange}");
                        sbs.AppendLine("  >");
                        sbs.AppendLine("    <MenuItem value={10}>Ten</MenuItem>");
                        sbs.AppendLine("    <MenuItem value={20}>Twenty</MenuItem>");
                        sbs.AppendLine("    <MenuItem value={30}>Thirty</MenuItem>");
                        sbs.AppendLine("  </Select>");

                    }
                    else if (tipoComponente == "Checkbox")
                    {
                        sbs.AppendLine("< Switch { ...label}defaultChecked />");
                    }
                    else if (tipoComponente == "OptionButton")
                    {

                    }

                }
            }


            sbs.AppendLine("<DataGrid");
            sbs.AppendLine("            components={{");
            sbs.AppendLine("              Toolbar: QuickSearchToolbar,");
            sbs.AppendLine("            }}");
            sbs.AppendLine("            componentsProps={{");
            sbs.AppendLine("              toolbar: {");
            sbs.AppendLine("                value: this.state.searchText,");
            sbs.AppendLine("                onChange: (event) => this.requestSearch(event.target.value),");
            sbs.AppendLine("                clearSearch: () => this.requestSearch(''),");
            sbs.AppendLine("              },");
            sbs.AppendLine("            }}");
            sbs.AppendLine("");
            sbs.AppendLine("            rows={this.state.arrayInformationBody}");
            sbs.AppendLine("            columns={columns}");
            sbs.AppendLine("            pageSize={80}");
            sbs.AppendLine("            checkboxSelection");
            sbs.AppendLine("            disableSelectionOnClick");
            sbs.AppendLine("          />");
            sbs.AppendLine("        </div>");
            sbs.AppendLine(");");
            sbs.AppendLine("  }");
            sbs.AppendLine("}");
            sbs.AppendLine("");
            sbs.AppendLine("export default ProcesoSPI;");

            createFileSql(sbs.ToString(), "FORM_" + table);

            return "";

        }
        public void createFileSql(String structura, String nombre)
        {

            System.IO.File.WriteAllText(pathScripts + nombre + ".js", structura);
        }
    }

}
