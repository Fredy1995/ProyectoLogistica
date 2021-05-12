using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;
using System.Data.Sql;
using System.Data.SqlClient;
using Microsoft.SqlServer.Server;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Windows.Documents;
using Page = System.Web.UI.Page;
using System.Runtime.ConstrainedExecution;
using System.Windows;

namespace Sistema
{
    public partial class Supervision : System.Web.UI.Page 
    {
        public String nombreUsuario,UltimaFechaReporte, FechaIReporte;
        public String strConexion = "server=DESKTOP-O8UKCDP\\SQLEXPRESS; database=LogisticaDB; uid=sa; pwd=1234;";
        private SqlConnection con = null;
        private SqlCommand OrdenSql, OrdenSqlER, OrdenSqlEC, OrdenSqlConcepto, sqlFechaR, OrdenSqlDatosR, OrdenSqlUtilidadA,OrdenSqlProy, OrdenSqlIdC, OrdenSqlExRep, OrdenSqlObtR, OrdenSqlIdRe, OrdenSqlMesAgregado;
        public int IDarea, Idresidencia, Idreporte, numConceptos, Idconcepto;
        public int contadorConceptos = 0,fila,colum;
        public Int64  numreportes,IdConceptoA, IdReporteAnterior;
        public double TotalconFtoR, IndirectoCP, IndirectoCR, IndirectoOCP, IndirectoOCR, IngresoMensualP, IngresoMensualR, UtilidadAA, FinanciamientoA, UtilidadMP, UtilidadMR, FinanciamientoP, FinanciamientoR, presupuesto = 0, real = 0, variacion = 0, MontoP, MontoTP, MontoR, TotalCDIP = 0, TotalCDIR = 0, Financiamientop, Financiamientor;
        public double CDP, ICP, IOCP, CDR, ICR, IOCR,AIngresoR,ACDR,AICR,AIOCR,ACFR;
        const double  porcentajeIC = 0.045, porcentajeIO=0.1239; //Variables como CONSTANTES
        private SqlDataReader Leer, LeerFechaR, LeerDatosPRV, LeerFechaRA,LeerReporte,LeerConcepto, LeerDatosDeReporte,LeerIdReporte, LeerIdC, LeerFR;
        Boolean ExisteReporte, HayAlgo, ExistenContratos, ExistenReportes,ExistenConceptos, CambiarPorcentaje,MostrarGrafica, MostrarGraficaA, ExProInicial, ExisteFechaAnterior;
        public Boolean ExProy;
        public string Columna,concepto, mesyanio,cadDatos,titleGrafica,Mesp;
        protected void Page_Load(object sender, EventArgs e)
        {
            AlertDanger.Visible = false;
            AlertSuccess.Visible = false;
            AlertWarning.Visible = false;
            if (!IsPostBack)
            {
               
               CargarResidencias(); //Metodo que se encarga de Cargar los contratos en las listas despegables correspondientes
               CargarTablaGlobalArea(); //este metodo es para cargar la Tabla Global por Área
               LbtnSupervision.Enabled = false;
            }
            
            try
            {
                nombreUsuario = Session["Usuario"].ToString();
                lblmensaje.Text = nombreUsuario;
                if (nombreUsuario == "Invitado")
                {
                    LbtnEditarC.Visible = false;
                    LbtnCambiarPorcentaje.Visible = false;

                }

            }
            catch (Exception)
            {
                Response.Redirect("Default.aspx?id=1");
            }
        }
      
        protected void CargarResidencias() //Metodo para cargar en el nombre de los contratos en las listas despegables
        {
           
            Boolean ExisteContrato = VerificarSiExistenContratos();
            IDarea = ObtenerIdArea();
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    if (ExisteContrato)
                    {
                        String ObtenerResidencias = "Select Residencia From Residencias Where IdArea = @idarea";
                        OrdenSql = new SqlCommand(ObtenerResidencias, con);
                        OrdenSql.Parameters.AddWithValue("@idarea", IDarea);
                        con.Open();
                        DdlContratoMG.DataSource = OrdenSql.ExecuteReader();
                        DdlContratoMG.DataTextField = "Residencia";
                        DdlContratoMG.DataBind();
                        con.Close();
                        con.Open();
                        DdlContratos.DataSource = OrdenSql.ExecuteReader();
                        DdlContratos.DataTextField = "Residencia";
                        DdlContratos.DataBind();
                        con.Close();
                        con.Open();
                        DDlEleminarResidencia.DataSource = OrdenSql.ExecuteReader();
                        DDlEleminarResidencia.DataTextField = "Residencia";
                        DDlEleminarResidencia.DataBind();
                        con.Close();
                        con.Open();
                        DdlContratoA.DataSource = OrdenSql.ExecuteReader();
                        DdlContratoA.DataTextField = "Residencia";
                        DdlContratoA.DataBind();
                        con.Close();
                        con.Open();
                        DdlContratosProyeccion.DataSource = OrdenSql.ExecuteReader();
                        DdlContratosProyeccion.DataTextField = "Residencia";
                        DdlContratosProyeccion.DataBind();
                        con.Close();
                        con.Open();
                        DDlContratoAP.DataSource = OrdenSql.ExecuteReader();
                        DDlContratoAP.DataTextField = "Residencia";
                        DDlContratoAP.DataBind();
                        con.Close();
                        
                    }
                    else if (DdlContratos.SelectedValue.ToString().CompareTo("Ninguno") != 0)
                    {
                        DdlContratos.Items.Clear();
                        DdlContratoMG.Items.Clear();
                        DdlContratos.Items.Add("Ninguno");
                        DdlContratoMG.Items.Add("Ninguno");
                        PnlVacio.Visible = true;
                        HeaderTable.Visible = false;
                        PnlTableC.Visible = false;
                        FooterTable.Visible = false;
                    }
                    
                   

                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }

        protected Boolean VerificarSiExistenContratos() //Metodo que verifica si exiten contratos agregados, si no existen entonces muestro un panel para indicar que no hay nada que mostrar
        {

            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string ConsultarReporte = "Select 'true' From Residencias where IdArea = @idarea";
                    OrdenSqlExRep = new SqlCommand(ConsultarReporte, con);
                    con.Open();
                    OrdenSqlExRep.Parameters.AddWithValue("@idarea", ObtenerIdArea());
                    ExistenContratos = Convert.ToBoolean(OrdenSqlExRep.ExecuteScalar());
                    con.Close();
                }

            }
            catch (Exception ex)
            {
                AlertDanger.Visible = true;
                lblDanger.Text = "<strong><strong>Detalles de Error:</strong> " + ex.Message;
            }
            return ExistenContratos;
        }
        //A Q U I   S E   C O N S T R U Y E     L A     T A B L A  C O N S O L I D A D O    A N U A L   D E L    Á R E A
        protected void ConstruirTablaAnual(string MesI, string MesF, string anio)
        {
            int numMesI = DevuelveNumeroMes(MesI), numMesF = DevuelveNumeroMes(MesF);
            string IdC;
            try 
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string ObtenerIdContratos = "Select IdResidencia From Residencias where IdArea = @idarea";
                    OrdenSqlIdC = new SqlCommand(ObtenerIdContratos, con);
                    con.Open();
                    OrdenSqlIdC.Parameters.AddWithValue("@idarea", ObtenerIdArea());
                    LeerIdC = OrdenSqlIdC.ExecuteReader();
                    DataTable dtAnual = new DataTable(); //Aqui creo un obejto de tipo DataTable para mostrar los datos en un GridView (Tabla de Resumen para el Consolidado anual de los contratos por AREA)
                    //Creo las columnas del DataTable TableAnual
                    dtAnual.Columns.Add("Area");
                    dtAnual.Columns.Add("MontoIngresoA");
                    dtAnual.Columns.Add("CostoDirectoA");
                    dtAnual.Columns.Add("Indirecto");
                    dtAnual.Columns.Add("IndirectoOficina");
                    dtAnual.Columns.Add("TotalCDI");
                    dtAnual.Columns.Add("CFinanciamiento");
                    dtAnual.Columns.Add("UtilidadContrato");
                    dtAnual.Columns.Add("Utilidad");
                    DataRow Fila = dtAnual.NewRow();
                    while (LeerIdC.Read()) //Aqui leo los Id's de cada Contrato que hay en el Area seleccionada
                    {
                        IdC = LeerIdC[0].ToString();
                        ObtenerFechasDeReportes(Convert.ToInt32(IdC),numMesI, numMesF, Convert.ToInt64(anio), Fila);
                    }
                    con.Close();
                    dtAnual.Rows.Add(Fila);
                    GVTableAnual.DataSource = dtAnual;  //Aquí cargo los datos al GridView que mostrará el resumén anual
                    GVTableAnual.DataBind();
                    //Mostrar el Titulo de la Tabla Anual
                    if (MesI != MesF)
                    {
                        LblTitleResumenAnual.Text = "Resumen " + MesI + " - " + MesF + " " + anio + " ";
                    }
                    else
                    {
                        LblTitleResumenAnual.Text = "Resumen " + MesI + " - " + anio;
                    }
                }
            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }
        protected int DevuelveNumeroMes(string mes) //Metodo que se le pasa como parametro (Mes) y devuelve el numero del Mes correspondiente
        {
            int NumMes;
            if (mes == "Enero")
            {
                NumMes = 1;
            }
            else if (mes == "Febrero")
            {
                NumMes = 2;
            }
            else if (mes == "Marzo")
            {
                NumMes = 3;
            }
            else if (mes == "Abril")
            {
                NumMes = 4;
            }
            else if (mes == "Mayo")
            {
                NumMes = 5;
            }
            else if (mes == "Junio")
            {
                NumMes = 6;
            }
            else if (mes == "Julio")
            {
                NumMes = 7;
            }
            else if (mes == "Agosto")
            {
                NumMes = 8;
            }
            else if (mes == "Septiembre")
            {
                NumMes = 9;
            }
            else if (mes == "Octubre")
            {
                NumMes = 10;
            }
            else if (mes == "Noviembre")
            {
                NumMes = 11;
            }
            else
            {
                NumMes = 12;
            }
            return NumMes;
        }
        protected void ObtenerFechasDeReportes(int IdC, int numMesI, int numMesF, Int64 Anios, DataRow Fila) //Metodo para obtener las fechas de cada REPORTE, según el AÑO seleccionado en la lista despegable
        {
            string fecha,FechaR,nmes,anio;
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string FechaReportes = "Select FechaDeReporte From Reportes Where IdResidencia = @idcontrato AND FechaDeReporte LIKE @anio ORDER BY FechaDeReporte";
                    OrdenSql = new SqlCommand(FechaReportes, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idcontrato", IdC);
                    OrdenSql.Parameters.AddWithValue("@anio", Anios+"%");
                    LeerFR = OrdenSql.ExecuteReader();
                   
                    while (LeerFR.Read()) //Aquí leo todas las fechas de los reportes agregados en la Tabla Reportes del contrato Recibido como parametro
                    {
                        fecha = LeerFR[0].ToString();
                        DateTime Fecha = DateTime.Parse(fecha);
                        DateTime fechames = DateTime.Parse(fecha);          //Se crea un objeto de tipo DateTime para Mes
                        DateTime fechaanio = DateTime.Parse(fecha);         //Se crea un objeto de tipo DateTime para Año
                        nmes = Convert.ToString(fechames.Month);             //Obtengo solo el Mes de la fecha completa
                        anio = Convert.ToString(fechaanio.Year);            //Obtengo solo el Año de la fecha completa
                        if (numMesI <= numMesF && Convert.ToInt64(anio) == Anios)
                        {
                            //Solo entra cuando se seleccionan fechas del Mismo Año
                            if (Convert.ToInt16(nmes) >= numMesI)
                            {
                                if (Convert.ToInt16(nmes) <= numMesF)
                                {
                                    FechaR = Convert.ToString(Fecha.ToShortDateString()); //Guardo la Fecha del Reporte Mensual a procesar
                                    LlenarFilasTablaAnual(IdC, FechaR, Fila);
                                }
                            }
                        }
                        else
                        {
                            AlertDanger.Visible = true;
                            lblDanger.Text = "<strong>¡Error!</strong> La búsqueda no se puede procesar. <strong>Detalles:</strong> El orden de las fechas es incorrecto...";
                        }
                      
                    }
                   // con.Close();
                }
            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }
        protected void LlenarFilasTablaAnual(int IdContrato, string FechaR,DataRow Fila)
        {
            int fila = 0, nconceptos = ObtenerNumConceptos(IdContrato);
            Int64 idreporte;
            double presupuesto, real, variacion, UPC, UtilidadMensual, aux;
            string  MontoIngresoA, CostoDirectoA, Indirecto, IndirectoOficina, TotalCDI, CFinanciamiento, UtilidadContrato, Utilidad;
            Fila[0] = "Supervisión de la Construcción de Obras de Infraestructuras de Vías Terrestres";
         
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string DatosPRV = "Select IdReporte, Presupuesto,Real,Variacion From DatosDeReporte where IdReporte =  (Select IdReporte From Reportes where FechaDeReporte = @fechareporte AND IdResidencia = @idresidencia) ORDER BY IdConcepto";
                    OrdenSqlDatosR = new SqlCommand(DatosPRV, con);
                    con.Open();
                    OrdenSqlDatosR.Parameters.Clear();
                    OrdenSqlDatosR.Parameters.AddWithValue("@fechareporte", FechaR);
                    OrdenSqlDatosR.Parameters.AddWithValue("@idresidencia", IdContrato);
                    LeerDatosPRV = OrdenSqlDatosR.ExecuteReader();
                    while (LeerDatosPRV.Read())
                    {
                        if (fila < nconceptos)
                        {
                            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat; //Creo Objeto para personalizar el símbolo de moneda
                            nfi.CurrencyPositivePattern = 2; //Establesco la posicion del simbolo de moneda
                            idreporte = Convert.ToInt64(LeerDatosPRV[0].ToString());       //Todo el array es de un solo tipo de dato
                            presupuesto = Convert.ToDouble(LeerDatosPRV[1].ToString());
                            real = Convert.ToDouble(LeerDatosPRV[2].ToString());
                            variacion = Convert.ToDouble(LeerDatosPRV[3].ToString());
                           // ExisteReporte = VerificarSiExisteReporte(idreporte);
                            if (fila == 0)  //Obtengo los costos del concepto Ingresos (Ingreso Mensual)
                            {
                                //IngresoMensualP = presupuesto;
                                //IngresoMensualR = real;
                                AIngresoR += real;  //Ingreso Mensual REAL
                                MontoIngresoA = string.Format(nfi, "{0:C}", AIngresoR);
                                Fila[1] = MontoIngresoA;
                            }
                            if (fila >= nconceptos - 7) //Si existe el reporte y a de más la fila es mayor o igual al concepto Total Costo directo, entra en la condición{
                            {
                                if (fila == nconceptos - 7) //Total Costo Directo REAL
                                {
                                    ACDR += real;
                                    CostoDirectoA = string.Format(nfi, "{0:C}", ACDR);
                                    Fila[2] = CostoDirectoA;
                                }
                                else if (fila == nconceptos - 6) //Indirecto de campo REAL
                                {
                                    AICR += real;
                                    Indirecto = string.Format(nfi, "{0:C}", AICR);
                                    Fila[3] = Indirecto;
                                }
                                else if (fila == nconceptos - 5) //Indirecto de Oficina Central REAL
                                {
                                    AIOCR += real;
                                    IndirectoOficina = string.Format(nfi, "{0:C}", AIOCR);
                                    Fila[4] = IndirectoOficina;
                                    TotalCDIR = ACDR + AICR + AIOCR;
                                    TotalCDI = string.Format(nfi, "{0:C}", TotalCDIR);
                                    Fila[5] = TotalCDI;
                                }
                                else if (fila == nconceptos - 1) // Costo del Financiamiento REAL
                                {
                                    ACFR += real;
                                    CFinanciamiento = string.Format(nfi, "{0:C}", ACFR);
                                    Fila[6] = CFinanciamiento;
                                    UPC = AIngresoR - TotalCDIR - ACFR;
                                    if (UPC < 0)
                                    {
                                        aux = UPC * -1;
                                        aux = Convert.ToDouble(string.Format("{0:F2}", aux));
                                        UtilidadContrato = "-" + string.Format(nfi, "{0:C}", aux);
                                        Fila[7] = UtilidadContrato;
                                    }
                                    else
                                    {
                                        aux = Convert.ToDouble(string.Format("{0:F2}", UPC));
                                        UtilidadContrato = string.Format(nfi, "{0:C}", aux);
                                        Fila[7] = UtilidadContrato;
                                    }

                                    if (AIngresoR != 0)
                                    {
                                        UtilidadMensual = Math.Round((UPC / AIngresoR) * 100); //Aquí se obtiene el porcentaje de utilidad mensual (Real %)
                                        Utilidad = Convert.ToString(UtilidadMensual) + "%";
                                        Fila[8] = Utilidad;
                                    }
                                    else
                                    {
                                        UtilidadMensual = 0;
                                        Utilidad = Convert.ToString(UtilidadMensual) + "%";
                                        Fila[8] = Utilidad;
                                    }
                                   

                                }
                            }
                            ++fila;    //Incremento la posición de la fila

                        }
                        else
                        {
                            fila = 0; //Asigno valor 0 la variable fila que lleva el control de las filas de los conceptos de cada reporte
                        }

                    }//Fin de while
                    //con.Close();


                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }
        //A Q U I   S E   C O N S T R U Y E     L A     T A B L A   G L O B A L     P O R   Á R E A
        protected void ConstruirTablaGlobalArea(Int64 IdContrato,string MesInicial,string MesFinal) //Metodo para Cargar en el GridView GvContratos los Totales de todos los contratos agregados
        {
            string fecha,nmes,anio,FechaR,mesI,mesF,mes;
            int nmesInicial = DevuelveNumMes(MesInicial), nmesFinal = DevuelveNumMes(MesFinal);
            Int64 anioInicial = DevuelveAnio(MesInicial), anioFinal = DevuelveAnio(MesFinal);
           
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string FechaReportes = "Select FechaDeReporte From Reportes Where IdResidencia = @idcontrato ORDER BY FechaDeReporte";
                    OrdenSql = new SqlCommand(FechaReportes, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idcontrato", IdContrato);
                    Leer = OrdenSql.ExecuteReader();
                    DataTable dtContrato = new DataTable(); //Aqui creo un obejto de tipo DataTable para mostrar los datos en un GridView (Tabla de Resumen para la rentabilidad de los contratos por AREA)
                    //Creo las columnas del DataTable Contratos
                    dtContrato.Columns.Add("Contrato");
                    dtContrato.Columns.Add("MontoIngresoA");
                    dtContrato.Columns.Add("CostoDirectoA");
                    dtContrato.Columns.Add("Indirecto");
                    dtContrato.Columns.Add("IndirectoOficina");
                    dtContrato.Columns.Add("TotalCDI");
                    dtContrato.Columns.Add("CFinanciamiento");
                    dtContrato.Columns.Add("UtilidadContrato");
                    dtContrato.Columns.Add("Utilidad");
                    DataRow Fila = dtContrato.NewRow();
                    //************************DATA TABLE PARA LA GRAFICA****************************************
                    DataTable Datos = new DataTable(); //Aquí creo un objeto de tipo DataTable para guardar los datos para poder crear la grafica
                    //Construyo las columnas de los datos de la grafica
                    Datos.Columns.Add(new DataColumn("MES", typeof(string)));
                    Datos.Columns.Add(new DataColumn("UTILIDAD DEL CONTRATO", typeof(string)));
                    Datos.Columns.Add(new DataColumn("REAL", typeof(string)));
                    Datos.Columns.Add(new DataColumn("{ 'type': 'string', 'role': 'style'}", typeof(string)));
                    //******************************************************************************************
                    while (Leer.Read())    //Aquí leo todas las fechas de los reportes agregados en la Tabla Reportes del contrato seleccionado
                    {
                        fecha = Leer[0].ToString();
                        DateTime Fecha = DateTime.Parse(fecha);
                        DateTime fechames = DateTime.Parse(fecha);          //Se crea un objeto de tipo DateTime para Mes
                        DateTime fechaanio = DateTime.Parse(fecha);         //Se crea un objeto de tipo DateTime para Año
                        nmes = Convert.ToString(fechames.Month);             //Obtengo solo el Mes de la fecha completa
                        anio = Convert.ToString(fechaanio.Year);            //Obtengo solo el Año de la fecha completa
                       if(anioInicial <= anioFinal)
                        {
                            
                            if ((Convert.ToInt64(anio) == anioInicial) && (Convert.ToInt64(anio) == anioFinal))
                            {   //Solo entra cuando se seleccionan fechas del Mismo Año
                                if(nmesInicial <= nmesFinal)
                                {
                                    if (Convert.ToInt16(nmes) >= nmesInicial)
                                    {
                                        if (Convert.ToInt16(nmes) <= nmesFinal)
                                        {
                                            FechaR = Convert.ToString(Fecha.ToShortDateString());
                                            LlenarFilasTablaResumen(IdContrato,FechaR,Datos, Fila);
                                        }
                                    }
                                }
                                else
                                {
                                    AlertDanger.Visible = true;
                                    lblDanger.Text = "<strong>¡Error!</strong> La búsqueda no se puede procesar. <strong>Detalles:</strong> El orden de las fechas es incorrecto...";
                                }
                               
                            }
                            else
                            {
                                if (Convert.ToInt64(anio) == anioInicial && Convert.ToInt64(anio) < anioFinal)
                                { //Solo entra cuando se seleccionan fechas de Años Diferentes
                                    if (Convert.ToInt16(nmes) >= nmesInicial && Convert.ToInt64(anio) == anioInicial)
                                    { 
                                        FechaR = Convert.ToString(Fecha.ToShortDateString());
                                        LlenarFilasTablaResumen(IdContrato, FechaR, Datos, Fila);
                                    }

                                }
                                else
                                {    
                                    if (Convert.ToInt16(nmes) <= nmesFinal && Convert.ToInt64(anio) == anioFinal)
                                    {
                                        FechaR = Convert.ToString(Fecha.ToShortDateString());
                                        LlenarFilasTablaResumen(IdContrato, FechaR, Datos, Fila);
                                    }
                                }
                            }
                        }
                        else
                        {
                            AlertDanger.Visible = true;
                            lblDanger.Text = "<strong>¡Error!</strong> La búsqueda no se puede procesar. <strong>Detalles:</strong> El orden de las fechas es incorrecto...";
                        }
                        mes = DevuelveMes(Convert.ToString(nmesFinal));
                        titleGrafica = TituloG(mes, Convert.ToString(anioFinal));
                    }
                    Leer.Close();
                    con.Close();
                    dtContrato.Rows.Add(Fila);
                    GVContratos.DataSource = dtContrato;  //Aquí cargo los datos al GridView que mostrará el resumén de los contratos
                    GVContratos.DataBind();
                    cadDatos = ObtenerDatos(Datos); //Metodo para crear el arreglo de datos para la construcciona de la grafica, recibe como parametro un objeto Datos de tipo DataTable
                    mesI = DevuelveMes(Convert.ToString(nmesInicial));
                    mesF = DevuelveMes(Convert.ToString(nmesFinal));
                    if (anioInicial != anioFinal)
                    {
                        
                        LblTitleResumenC.Text = "Resumen " + mesI+" "+anioInicial+" "+ " - " + mesF+" "+anioFinal+" ";
                    }
                    else
                    {  
                        if(mesI != mesF)
                        {
                            LblTitleResumenC.Text = "Resumen " + mesI + " - " + mesF + " " + anioFinal + " ";
                        }
                        else
                        {
                            LblTitleResumenC.Text = "Resumen " + mesI + " - " + anioFinal;
                        }
                        
                    }
                    if (MostrarGraficaA)
                    {
                        LblTitleRentabilidad.Visible = false;
                        LblTitleResumenC.Visible = false;
                        PnlTable.Visible = false;
                    }
                    else
                    {
                        PnlTable.Visible = true;
                        LblTitleRentabilidad.Visible = true;
                        LblTitleResumenC.Visible = true;
                    }
                   
                }
            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }

        }
        // A Q U I  S E    L L E N A N    L A S   F I L A S   D E   L A   T A B L A    G L O B A L  P O R   A R E A 
        protected void LlenarFilasTablaResumen(Int64 IdContrato,string FechaR,DataTable Datos,DataRow Fila) //Metodo para llenar la unica fila en la Tabla Global por Contrato
        {
            int fila = 0, nconceptos = ObtenerNumConceptos(Convert.ToInt32(IdContrato));
            Int64 idreporte;
            double presupuesto, real, variacion, UPC, UtilidadMensual,aux;
            string Contrato = DdlContratoMG.SelectedItem.ToString(), MontoIngresoA,CostoDirectoA,Indirecto,IndirectoOficina,TotalCDI,CFinanciamiento,UtilidadContrato,Utilidad;
            Fila[0] = Contrato;
            string nmes, anio, m, MesAnio;
           // DateTime Fecha = DateTime.Parse(FechaR);              //Se crea un objeto de tipo DataTime para obtener solo la fecha sin la hora
            DateTime fechames = DateTime.Parse(FechaR);          //Se crea un objeto de tipo DateTime para Mes
            DateTime fechaanio = DateTime.Parse(FechaR);         //Se crea un objeto de tipo DateTime para Año
            nmes = Convert.ToString(fechames.Month);             //Obtengo solo el Mes de la fecha completa
            anio = Convert.ToString(fechaanio.Year);            //Obtengo solo el Año de la fecha completa
            m = DevuelveMes(nmes);
            MesAnio = m + "(" + anio + ")";
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string DatosPRV = "Select IdReporte, Presupuesto,Real,Variacion From DatosDeReporte where IdReporte =  (Select IdReporte From Reportes where FechaDeReporte = @fechareporte AND IdResidencia = @idresidencia) ORDER BY IdConcepto";
                    OrdenSqlDatosR = new SqlCommand(DatosPRV, con);
                    con.Open();
                    OrdenSqlDatosR.Parameters.Clear();
                    OrdenSqlDatosR.Parameters.AddWithValue("@fechareporte",FechaR);
                    OrdenSqlDatosR.Parameters.AddWithValue("@idresidencia", IdContrato);
                    LeerDatosPRV = OrdenSqlDatosR.ExecuteReader();
                    while (LeerDatosPRV.Read())
                    {
                        if (fila < nconceptos)
                        {
                            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat; //Creo Objeto para personalizar el símbolo de moneda
                            nfi.CurrencyPositivePattern = 2; //Establesco la posicion del simbolo de moneda
                            idreporte = Convert.ToInt64(LeerDatosPRV[0].ToString());       //Todo el array es de un solo tipo de dato
                            presupuesto = Convert.ToDouble(LeerDatosPRV[1].ToString());
                            real = Convert.ToDouble(LeerDatosPRV[2].ToString());
                            variacion = Convert.ToDouble(LeerDatosPRV[3].ToString());
                            ExisteReporte = VerificarSiExisteReporte(idreporte);
                            if (fila == 0)  //Obtengo los costos del concepto Ingresos (Ingreso Mensual)
                            {
                                //IngresoMensualP = presupuesto;
                                //IngresoMensualR = real;
                                AIngresoR += real;  //Ingreso Mensual REAL
                                MontoIngresoA = string.Format(nfi, "{0:C}", AIngresoR);
                                Fila[1] = MontoIngresoA;
                            }
                            if (ExisteReporte == true && fila >= nconceptos - 7) //Si existe el reporte y a de más la fila es mayor o igual al concepto Total Costo directo, entra en la condición{
                            {
                                if (fila == nconceptos - 7) //Total Costo Directo REAL
                                {
                                    ACDR += real;
                                    CostoDirectoA = string.Format(nfi, "{0:C}", ACDR);
                                    Fila[2] = CostoDirectoA;
                                }
                                else if (fila == nconceptos - 6) //Indirecto de campo REAL
                                {
                                    AICR += real;
                                    Indirecto = string.Format(nfi, "{0:C}", AICR);
                                    Fila[3] = Indirecto;
                                }
                                else if (fila == nconceptos - 5) //Indirecto de Oficina Central REAL
                                {
                                    AIOCR += real;
                                    IndirectoOficina = string.Format(nfi, "{0:C}",AIOCR);
                                    Fila[4] = IndirectoOficina;
                                    TotalCDIR = ACDR + AICR + AIOCR;
                                    TotalCDI = string.Format(nfi, "{0:C}", TotalCDIR);
                                    Fila[5] = TotalCDI;
                                }
                                else if (fila == nconceptos - 1) // Costo del Financiamiento REAL
                                {
                                    ACFR += real;
                                    CFinanciamiento = string.Format(nfi, "{0:C}", ACFR);
                                    Fila[6] = CFinanciamiento;
                                    UPC = AIngresoR - TotalCDIR - ACFR;
                                    if (UPC < 0)
                                    {
                                        aux = UPC * -1;
                                        aux = Convert.ToDouble(string.Format("{0:F2}", aux));
                                        UtilidadContrato = "-"  + string.Format(nfi, "{0:C}",aux);
                                        Fila[7] = UtilidadContrato;
                                    }
                                    else
                                    {
                                        aux = Convert.ToDouble(string.Format("{0:F2}", UPC));
                                        UtilidadContrato = string.Format(nfi, "{0:C}", aux);
                                        Fila[7] = UtilidadContrato;
                                    }
                                    
                                    if (AIngresoR != 0)
                                    {
                                        UtilidadMensual = Math.Round((UPC / AIngresoR) * 100); //Aquí se obtiene el porcentaje de utilidad mensual (Real %)
                                        Utilidad = Convert.ToString(UtilidadMensual) + "%";
                                        Fila[8] = Utilidad;
                                    }
                                    else
                                    {
                                        UtilidadMensual = 0;
                                        Utilidad = Convert.ToString(UtilidadMensual)+"%";
                                        Fila[8] = Utilidad;
                                    }
                                    //************************** DATOS CAPTURADOS PARA LA GRAFICA ******************************//
                                    string point = "'point { size: 5; shape-type: circle; fill-color: orange; }'";
                                    Datos.Rows.Add(new Object[] { MesAnio, string.Format("{0:F2}", UPC), Convert.ToString(UtilidadMensual), point });

                                }
                            }
                            ++fila;    //Incremento la posición de la fila

                        }
                        else
                        {
                            fila = 0; //Asigno valor 0 la variable fila que lleva el control de las filas de los conceptos de cada reporte
                        }

                    }//Fin de while
                    con.Close();
                   

                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }
        // A Q U I   S E  C O N S T R U Y E   L A  T A B L A     D E  F L U J O  D E   E F E C T I V O   P A R A  L O S C O N T R A T O S
        protected void ConstruirTabla(int IdR) //...........Metodo para construir y llenar la tabla de datos solo para los Contratos o Residencias, recibe el Id de residencia
        {
            string concepto, observacion, pre, real, va,totalpre,totalreal,totalvar;
            string mes, nmes, anio, fecha, dateR;// col1 = "MES", col2 = "UTILIDAD DEL CONTRATO", col3 = "% REAL", col4 = "% PROYECTADO";
            int IdsReportes = ObtenerIdsReportes(IdR),contador=1; //Devuelve el Id de un reporte que pertenecen a la misma residencia que hay en la tabla reportes de la BD
            Int64 nreportes = ObtenerNreportes(IdR);   //Obtiene la cantidad de reportes que hay en la residencia actual
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {

                    string queryconcepto = "Select Concepto.Concepto,DatosDeReporte.Observacion From Concepto INNER JOIN DatosDeReporte ON Concepto.IdConcepto = DatosDeReporte.IdConcepto AND DatosDeReporte.IdReporte = @idreporte ORDER BY Concepto.IdConcepto";
                    string fechaR = "Select FechaDeReporte From Reportes Where IdResidencia = @idresidencia ORDER BY FechaDeReporte";
                    OrdenSql = new SqlCommand(queryconcepto, con);
                    sqlFechaR = new SqlCommand(fechaR, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idreporte", IdsReportes);
                    Leer = OrdenSql.ExecuteReader();
                    DataTable dt = new DataTable(); //Aquí creo un objeto de Tipo DataTable para guardar los datos y mostrarlos en un GridView (Tabla Contratos) 
                    dt.Columns.Add("Concepto");                 //Columna Concepto
                    dt.Columns.Add("Observación");  //Columna Observacion
                    //************************DATA TABLE PARA LA GRAFICA****************************************
                    DataTable Datos = new DataTable(); //Aquí creo un objeto de tipo DataTable para guardar los datos para poder crear la grafica
                    //Construyo las columnas de los datos de la grafica
                    Datos.Columns.Add(new DataColumn("MES", typeof(string)));
                    Datos.Columns.Add(new DataColumn("UTILIDAD DEL CONTRATO", typeof(string)));
                    Datos.Columns.Add(new DataColumn("REAL", typeof(string)));
                    Datos.Columns.Add(new DataColumn("{ 'type': 'string', 'role': 'style'}", typeof(string)));
                    Datos.Columns.Add(new DataColumn("PROYECTADO", typeof(string)));
                    //******************************************************************************************
                    while (Leer.Read())
                    {
                        HayAlgo = true;
                       //Aqui Lleno las filas de las columnas Concepto y Observacion
                       DataRow fila = dt.NewRow();
                        concepto = Leer[0].ToString();
                        observacion = Leer[1].ToString();
                        fila[0] = concepto;
                        fila[1] = observacion;
                        dt.Rows.Add(fila);

                    }
                    Leer.Close();
                    con.Close();
                   
                    con.Open();
                    sqlFechaR.Parameters.AddWithValue("@idresidencia", IdR);
                    LeerFechaR = sqlFechaR.ExecuteReader();
                    while (LeerFechaR.Read())
                    {
                        //Aqui Creo las columnas Presupuesto,Real y Variacion de cada Reporte

                        fecha = LeerFechaR[0].ToString();
                        DateTime fechames = DateTime.Parse(fecha);          //Se crea un objeto de tipo DateTime para Mes
                        DateTime fechaanio = DateTime.Parse(fecha);         //Se crea un objeto de tipo DateTime para Año
                        nmes = Convert.ToString(fechames.Month);             //Obtengo solo el Mes de la fecha completa
                        anio = Convert.ToString(fechaanio.Year);            //Obtengo solo el Año de la fecha completa
                        mes = DevuelveMes(nmes);
                        dateR = mes + anio;                         //Concateno el mes y año para identificar cada columna segun el mes y el año que fueron agregados los reportes, Eje: Presupuesto_Abril2019
                        pre = "Presupuesto" +" ("+ dateR +")";               //Concateno Nombre de la columna con Mes y Año
                        real = "Real" + " (" + dateR + ")";                     //"
                        va = "Variación" + " (" + dateR + ")";                  //"    
                        dt.Columns.Add(pre);
                        dt.Columns.Add(real);
                        dt.Columns.Add(va);
                        LlenarFilas(fecha, IdR, dt, Datos, pre, real, va, nmes);     //Mando a llamar el metodo para llenar las filas de las columnas correspondientes a los nombres de cada columna que llevan los parametros: pre,re,va
                        if (contador == nreportes)
                        {
                            totalpre = "TOTAL PRESUPUESTO";
                            totalreal = "TOTAL REAL";
                            totalvar = "TOTAL VARIACIÓN";
                            dt.Columns.Add(totalpre);                 //Columna Total Presupuesto
                            dt.Columns.Add(totalreal);                       //Columna Total Real
                            dt.Columns.Add(totalvar);                  //Columna Total Variación
                            LlenarFilasTotales(IdR,dt,totalpre,totalreal,totalvar);  //Aqui Mando a llamar el metodo para llenar las columnas de los totales
                            
                        }
                       
                        ++contador;
                       titleGrafica = TituloG(mes,anio);
                    }
                    con.Close();
                    GvDatosReportes.DataSource = dt;
                    GvDatosReportes.DataBind();
                    cadDatos = ObtenerDatos(Datos); //Metodo para crear el arreglo de datos para la construcciona de la grafica, recibe como parametro un objeto Datos de tipo DataTable
                    ExProInicial = false;
                    CambiarPorcentaje = false;
                    ExProy = false;
                    if (HayAlgo)   //Pregunto si hay datos que mostrar el mensaje de "no hay contenido en esta vista" no se mostrará
                    {
                        if (MostrarGrafica)
                        {
                            PnlVacio.Visible = false;  //No se muestra el msj
                            PnlTableC.Visible = false;
                            myInput.Visible = false;
                            HeaderTable.Visible = false;
                            PnlDatosReportes.Visible = false;
                            FooterTable.Visible = false;
                           
                        }
                        else
                        {
                            PnlVacio.Visible = false;  //No se muestra el msj
                            PnlTableC.Visible = true;
                            myInput.Visible = true;
                            HeaderTable.Visible = true;
                            PnlDatosReportes.Visible = true;
                            FooterTable.Visible = true;
                            CargarDatosResidencia(IdR);  //Aqui mando a llamar el Metodo que mostrará los datos de la Residencia, Encabezado de tabla general y Pie de tabla genera
                        }
                        LbtnGraficaC.Enabled = true;
                        LbtnCambiarPorcentaje.Enabled = true;
                    }
                    else
                    {
                        PnlTableC.Visible = false;
                        myInput.Visible = false;
                        HeaderTable.Visible = false;
                        PnlDatosReportes.Visible = false;
                        FooterTable.Visible = false;
                        PnlVacio.Visible = true;  //Se muestra el msj
                        LbtnGraficaC.Enabled = false;
                        LbtnCambiarPorcentaje.Enabled = false;
                    }
                   
                   
                    
                }
            }
            catch (Exception ex)
            {
                AlertDanger.Visible = true;
                lblDanger.Text = "<strong><strong>Detalles de Error:</strong> " + ex.Message;
            }
        }
        protected Int64 DevuelveAnio(string MesIniOFin)
        {
            int ncaracteres = MesIniOFin.Length;
            string anio = MesIniOFin.Remove(0, (ncaracteres - 4));
            return Convert.ToInt64(anio);
        }

        protected int DevuelveNumMes(string MesIniOFin) //Metodo que se le pasa como parametro (Mes-Año) y devuelve el numero del Mes correspondiente
        {
            int NumMes,ncaracteres = MesIniOFin.Length;
            string mes = MesIniOFin.Substring(0, (ncaracteres - 5));
           // string anio = MesIniOFin.Remove(0, (ncaracteres - 4));
            if(mes == "Enero")
            {
                NumMes = 1;
            }else if(mes == "Febrero")
            {
                NumMes = 2;
            }
            else if (mes == "Marzo")
            {
                NumMes = 3;
            }
            else if (mes == "Abril")
            {
                NumMes = 4;
            }
            else if (mes == "Mayo")
            {
                NumMes = 5;
            }
            else if (mes == "Junio")
            {
                NumMes = 6;
            }
            else if (mes == "Julio")
            {
                NumMes = 7;
            }
            else if (mes == "Agosto")
            {
                NumMes = 8;
            }
            else if (mes == "Septiembre")
            {
                NumMes = 9;
            }
            else if (mes == "Octubre")
            {
                NumMes = 10;
            }
            else if (mes == "Noviembre")
            {
                NumMes = 11;
            }
            else
            {
                NumMes = 12;
            }
            return NumMes;
        }
        protected string DevuelveMes(string nmes) //Metodo que recibe como parameto el numero del mes de tipo string para devolver el nombre del mes correspondiente
        { string mes;
            if (nmes == "1")
            {
                mes = "Enero";
            }
            else if (nmes == "2")
            {
                mes = "Febrero";
            }
            else if (nmes == "3")
            {
                mes = "Marzo";
            }
            else if (nmes == "4")
            {
                mes = "Abril";
            }
            else if (nmes == "5")
            {
                mes = "Mayo";
            }
            else if (nmes == "6")
            {
                mes = "Junio";
            }
            else if (nmes == "7")
            {
                mes = "Julio";
            }
            else if (nmes == "8")
            {
                mes = "Agosto";
            }
            else if (nmes == "9")
            {
                mes = "Septiembre";
            }
            else if (nmes == "10")
            {
                mes = "Octubre";
            }
            else if (nmes == "11")
            {
                mes = "Noviembre";
            }
            else
            {
                mes = "Diciembre";
            }
            return mes;
        }
        protected string ObtenerDatos(DataTable Datos) //Metodo que construye la tabla que necesita googlechart para representar los datos en la grafica lineal, recibe como un objeto de tipo DataTable con los datos requeridos
        {
            

            string strDatos;
            
            if (ExProy) //Si hay proyección, entonces se agrega la linea punteada
            {
                strDatos = "[['MES','UTILIDAD DEL CONTRATO','REAL',{'type': 'string', 'role': 'style'},'PROYECTADO'],";
                foreach (DataRow dr in Datos.Rows)
                {
                    strDatos += "[";
                    strDatos += "'" + dr[0] + "'" + "," + dr[1] + "," + dr[2] + "," + dr[3]+"," + dr[4];
                    strDatos += "],";
                }
            }
            else
            {
                strDatos = "[['MES','UTILIDAD DEL CONTRATO','REAL',{'type': 'string', 'role': 'style'}],";
                foreach (DataRow dr in Datos.Rows)
                {
                    strDatos += "[";
                    strDatos += "'" + dr[0] + "'" + "," + dr[1] + "," + dr[2] + "," + dr[3];
                    strDatos += "],";
                }
            }
            
            strDatos += "]";
            return strDatos;
        }
        protected string TituloG(string mes, string anio) //Metodo para mostrar el titulo en la grafica
        {
            string titulo = "'Rentabilidad acumulada al mes de "+mes.ToLower()+" "+anio+"'";
            //string titulo = "'Rentabilidad acumulada al mes de diciembre 2019'";
            return titulo;
        }
        protected Int64 ObtenerNreportes(int IdR)
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string nreportes = "Select COUNT(*) From Reportes Where IdResidencia = @idresidencia";
                    OrdenSql = new SqlCommand(nreportes, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idresidencia", IdR);
                    numreportes = Convert.ToInt64(OrdenSql.ExecuteScalar());
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                AlertDanger.Visible = true;
                lblDanger.Text = "<strong><strong>Detalles de Error:</strong> " + ex.Message;
            }
            return numreportes;
        }
        // A Q U I  S E  L L E N A N   L A S  F I L A S  D E  L A  T A B L A  D E  C O N T R A T O S
        protected void LlenarFilas(string fecha, int IdR, DataTable dt,DataTable Datos, string pre, string re, string va, string mes) //Metodo para agregar filas de la tabla de datos para contratos
        {                                                                                               // Parametros: fecha de un reporte, id de la residencia, objeto Datatable, cadenas con los nombres de columnas: pre,re,va
            double presupuesto, real, variacion, TotalCDP = 0.0, TotalCDR = 0.0, TotalCDV, FinanciamientoV;
            double IndirectoCV, TotalCDIV, UtilidadMV, UtilidadAP, UtilidadAR, UtilidadAV, idreporte, IndirectoOCV, Aux,UPC,UtilidadMensual;
            int fila = 0, nconceptos = ObtenerNumConceptos(IdR); //Obtengo la cantidad de conceptos que hay en cada contrato de la Tabla Concepto de la BD
            char Columna;
            string nmes, MesAnio,m,anio;
            Boolean ExisteReporte;           
            DateTime Fecha = DateTime.Parse(fecha);              //Se crea un objeto de tipo DataTime para obtener solo la fecha sin la hora
            DateTime fechames = DateTime.Parse(fecha);          //Se crea un objeto de tipo DateTime para Mes
            DateTime fechaanio = DateTime.Parse(fecha);         //Se crea un objeto de tipo DateTime para Año
            nmes = Convert.ToString(fechames.Month);             //Obtengo solo el Mes de la fecha completa
            anio = Convert.ToString(fechaanio.Year);            //Obtengo solo el Año de la fecha completa
            m = DevuelveMes(nmes);
            MesAnio = m +"("+ anio+")";
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string DatosPRV = "Select IdReporte, Presupuesto,Real,Variacion From DatosDeReporte where IdReporte =  (Select IdReporte From Reportes where FechaDeReporte = @fechareporte AND IdResidencia = @idresidencia) ORDER BY IdConcepto";
                    OrdenSqlDatosR = new SqlCommand(DatosPRV, con);
                    con.Open();
                    OrdenSqlDatosR.Parameters.Clear();
                    OrdenSqlDatosR.Parameters.AddWithValue("@fechareporte", Convert.ToString(Fecha.ToShortDateString()));
                    OrdenSqlDatosR.Parameters.AddWithValue("@idresidencia", IdR);
                    LeerDatosPRV = OrdenSqlDatosR.ExecuteReader();
                    while (LeerDatosPRV.Read())
                    {
                        if (fila < nconceptos)              
                        {
                            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat; //Creo Objeto para personalizar el símbolo de moneda
                            nfi.CurrencyPositivePattern = 2; //Establesco la posicion del simbolo de moneda
                            idreporte = Convert.ToDouble(LeerDatosPRV[0].ToString());       //Todo el array es de un solo tipo de dato
                            presupuesto = Convert.ToDouble(LeerDatosPRV[1].ToString());
                            real = Convert.ToDouble(LeerDatosPRV[2].ToString());
                            variacion = Convert.ToDouble(LeerDatosPRV[3].ToString());
                            ExisteReporte = VerificarSiExisteReporte(idreporte);
                            if (fila == 0)  //Obtengo los costos del concepto Ingresos (Ingreso Mensual)
                            {
                                IngresoMensualP = presupuesto;
                                IngresoMensualR = real;
                                AIngresoR += real;  //Ingreso Mensual REAL
                            }
                            if (fila > 0 && fila < nconceptos - 7)        //Aqui Obtengo la suma de todos los costos de los conceptos hasta 10% impuesto facturar
                            {
                                if (!ExisteReporte || CambiarPorcentaje == true) //Si no existe el reporte, entonces se hace la suma del nuevo Reporte
                                {
                                    TotalCDP +=  presupuesto;  //Va acumulando la suma de cada concepto en la columna Presupuesto
                                    TotalCDR +=  real;         //Va acumulando la suma de cada concepto en la columna Real
                                   //TotalCDV = TotalCDV + variacion;    //Va acumulando la suma de cada concepto en la columna Variacion
                                }
                                
                            }
                            if (ExisteReporte == true && fila >= nconceptos - 7) //Si existe el reporte y a de más la fila es mayor o igual al concepto Total Costo directo, entra en la condición
                            {
                                
                                 
                                if(fila == nconceptos - 7) //Total Costo Directo REAL
                                {
                                    ACDR += real;
                                }else if (fila == nconceptos - 6) //Indirecto de campo REAL
                                {
                                    AICR += real;
                                }else if (fila == nconceptos - 5) //Indirecto de Oficina Central REAL
                                {
                                    AIOCR += real;
                                    TotalCDIR = ACDR + AICR + AIOCR;
                                }
                                else if (fila == nconceptos - 1) // Costo del Financiamiento REAL
                                {
                                    ACFR += real;
                                    UPC = Math.Round(AIngresoR - TotalCDIR - ACFR); //Aquí se obtiene la Utilidad Proyectada del Contrato (Real $)
                                    if(AIngresoR != 0)
                                    {
                                        UtilidadMensual = Math.Round((UPC / AIngresoR) * 100); //Aquí se obtiene el porcentaje de utilidad mensual (Real %)
                                    }
                                    else
                                    {
                                        UtilidadMensual = 0;
                                    }
                                    //************************** DATOS CAPTURADOS PARA LA GRAFICA ******************************//
                                    string point = "'point { size: 5; shape-type: circle; fill-color: orange; }'";
                                    ExProy = ExisteProyeccion(IdR);
                                    if (ExProy) //Pregunto si hay proyección, es decir, si se agrego el mes a donde iniciaria la proyección
                                    {
                                        
                                        string Mesp = ObtenerMesProy(Convert.ToInt64(IdR), Convert.ToInt64(idreporte)); //Metodo que devuelve el mes de proyeccion inicial
                                        if (MesAnio.CompareTo(Mesp) == 0)
                                        {
                                            Datos.Rows.Add(new Object[] { MesAnio, string.Format("{0:F2}", UPC), Convert.ToString(UtilidadMensual), point, Convert.ToString(UtilidadMensual) });
                                            ExProInicial = true;
                                        }
                                        else if (ExProInicial)
                                        {
                                            Datos.Rows.Add(new Object[] { MesAnio, string.Format("{0:F2}", UPC), Convert.ToString(UtilidadMensual), point, Convert.ToString(UtilidadMensual) });
                                        }
                                        else
                                        {
                                            Datos.Rows.Add(new Object[] { MesAnio, string.Format("{0:F2}", UPC), Convert.ToString(UtilidadMensual), point, "null" });
                                        }
                                      
                                    }
                                    else
                                    {
                                        Datos.Rows.Add(new Object[] { MesAnio, string.Format("{0:F2}", UPC), Convert.ToString(UtilidadMensual), point });
                                    }
                                    
                                }

                            }
                            
                            if ((ExisteReporte == false || CambiarPorcentaje == true) && fila >= nconceptos - 7) //Aqui agrego los datos a la fila: Total Costo Directo, Su posición es 58 de la columna Concepto
                            {

                                if (fila == nconceptos - 7) //Total Costo Directo
                                {
                                    concepto = "Total Costo Directo";
                                    //EnviarTotalCDaSQL(TotalCDP, TotalCDR, TotalCDV, idreporte);    // Aquí mando el resultado de los TOTALES de cada columna a la Base de Datos
                                    TotalCDV = TotalCDR - TotalCDP;
                                    ActualizarDatosDeReporte(concepto,TotalCDP, TotalCDR, TotalCDV, idreporte, IdR); //Aquí actualizo la tabla DatosDeReporte en la fila(concepto) correspondiente para: Presupuesto,Real,Variación
                                    dt.Rows[fila][pre] = string.Format(nfi, "{0:C}", TotalCDP);         //Columna de presupuesto con formato de moneda
                                    dt.Rows[fila][re] = string.Format(nfi, "{0:C}", TotalCDR);           //Columna de Real con formato de moneda
                                    if (TotalCDV < 0)
                                    {
                                        Aux = TotalCDV * -1;
                                        dt.Rows[fila][va] = "-" + string.Format(nfi, "{0:C}", Aux);          //Columna de variación con formato de moneda
                                    }
                                    else
                                    {
                                        dt.Rows[fila][va] = string.Format(nfi, "{0:C}", TotalCDV);          //Columna de variación con formato de moneda
                                    }
                                }else

                                if (fila == nconceptos - 6) // Indirecto de Campo
                                {
                                    concepto = "Indirecto de campo";
                                    if (CambiarPorcentaje)
                                    {
                                        IndirectoCP = TotalCDP * (Convert.ToDouble(TxtPorcentajeIC.Text)/100); //Calcular Indirecto de campo para Presupuesto: Total Costo Directo de presupuesto * 12.39% -> porcentaje por default
                                        IndirectoCR = TotalCDR * (Convert.ToDouble(TxtPorcentajeIC.Text)/100); //Calcular Indirecto de campo para Real: Total Costo Directo de real* 4.50% -> porcentaje por default
                                    }
                                    else
                                    {
                                      
                                        IndirectoCP = TotalCDP * porcentajeIC; //Calcular Indirecto de campo para Presupuesto: Total Costo Directo de presupuesto * 12.39% -> porcentaje por default
                                        IndirectoCR = TotalCDR * porcentajeIC; //Calcular Indirecto de campo para Real: Total Costo Directo de real* 4.50% -> porcentaje por default
                                    }
                                    IndirectoCV = IndirectoCR - IndirectoCP;   //Aqui Obtengo la Variación (Puede salir negativo)
                                    ActualizarDatosDeReporte(concepto, IndirectoCP, IndirectoCR, IndirectoCV, idreporte, IdR); //Aquí actualizo la tabla DatosDeReporte en la fila(concepto) correspondiente para: Presupuesto,Real,Variación
                                    //EnviarIndirectoCSql(IndirectoCP, IndirectoCR, IndirectoCV, idreporte); //Enviar los datos a la Base de Datos
                                    dt.Rows[fila][pre] = string.Format(nfi, "{0:C}", IndirectoCP);         //Columna de presupuesto con formato de moneda
                                    dt.Rows[fila][re] = string.Format(nfi, "{0:C}", IndirectoCR);           //Columna de Real con formato de moneda
                                    if (IndirectoCV < 0)
                                    {
                                        Aux = IndirectoCV * -1;
                                        dt.Rows[fila][va] = "-" + string.Format(nfi, "{0:C}", Aux);          //Columna de variación con formato de moneda
                                    }
                                    else
                                    {
                                        dt.Rows[fila][va] = string.Format(nfi, "{0:C}", IndirectoCV);          //Columna de variación con formato de moneda
                                    }
                                }else
                                if (fila == nconceptos - 5) //Indirecto Oficina Central
                                {
                                    concepto = "Indirecto de Oficina Central";
                                    if (CambiarPorcentaje)
                                    {
                                        IndirectoOCP = TotalCDP * (Convert.ToDouble(TxtPorcentajeIO.Text) / 100); //Calcular Indirecto de Oficina Central para Presupuesto: Total Costo Directo de presupuesto * 12.39% -> porcentaje por default
                                        IndirectoOCR = TotalCDR * (Convert.ToDouble(TxtPorcentajeIO.Text) / 100);//Calcular Indirecto de Oficina Central para Real: Total Costo Directo  de real* 12.39% -> porcentaje por default
                                    }
                                    else
                                    {
                                        IndirectoOCP = TotalCDP * porcentajeIO; //Calcular Indirecto de Oficina Central para Presupuesto: Total Costo Directo de presupuesto * 12.39% -> porcentaje por default
                                        IndirectoOCR = TotalCDR * porcentajeIO;//Calcular Indirecto de Oficina Central para Real: Total Costo Directo  de real* 12.39% -> porcentaje por default
                                    }
                                    IndirectoOCV = IndirectoOCR - IndirectoOCP; //Aqui Obtengo la Variación (Puede salir negativo)
                                    ActualizarDatosDeReporte(concepto, IndirectoOCP, IndirectoOCR, IndirectoOCV, idreporte, IdR); //Aquí actualizo la tabla DatosDeReporte en la fila(concepto) correspondiente para: Presupuesto,Real,Variación
                                    //EnviarIndirectoOCSql(IndirectoOCP, IndirectoOCR, IndirectoOCV, idreporte); //Enviar los datos a la Base de Datos
                                    dt.Rows[fila][pre] = string.Format(nfi, "{0:C}", IndirectoOCP);         //Columna de presupuesto con formato de moneda
                                    dt.Rows[fila][re] = string.Format(nfi, "{0:C}", IndirectoOCR);           //Columna de Real con formato de moneda
                                    if (IndirectoOCV < 0)
                                    {
                                        Aux = IndirectoOCV * -1;
                                        dt.Rows[fila][va] = "-" + string.Format(nfi, "{0:C}", Aux);          //Columna de variación con formato de moneda
                                    }
                                    else
                                    {
                                        dt.Rows[fila][va] = string.Format(nfi, "{0:C}", IndirectoOCV);          //Columna de variación con formato de moneda
                                    }
                                }
                                else
                                 if(fila == nconceptos - 4)  // SubTotal Costo Directo + Indirecto
                                {
                                    concepto = "Subtotal Costo Directo + Indirecto";
                                    TotalCDIP = TotalCDP + IndirectoCP + IndirectoOCP;  //Aqui Obtengo la SUMA de Total Costo Directo + Indirecto de Presupuesto
                                    TotalCDIR = TotalCDR + IndirectoCR + IndirectoOCR; //Aqui Obtengo la SUMA de Total Costo Directo + Indirecto de Real
                                    TotalCDIV = TotalCDIR - TotalCDIP; //Aqui obtengo la Variacion de Total Costo Directo + Indirecto
                                    ActualizarDatosDeReporte(concepto, TotalCDIP, TotalCDIR, TotalCDIV, idreporte, IdR); //Aquí actualizo la tabla DatosDeReporte en la fila(concepto) correspondiente para: Presupuesto,Real,Variación
                                    //EnviarTotalCDIaSql(TotalCDIP, TotalCDIR, TotalCDIV, idreporte); //Enviar los datos a la Base de Datos
                                    dt.Rows[fila][pre] = string.Format(nfi, "{0:C}", TotalCDIP);         //Columna de presupuesto con formato de moneda
                                    dt.Rows[fila][re] = string.Format(nfi, "{0:C}", TotalCDIR);           //Columna de Real con formato de moneda
                                    if (TotalCDIV < 0)
                                    {
                                        Aux = TotalCDIV * -1;
                                        dt.Rows[fila][va] = "-" + string.Format(nfi, "{0:C}", Aux);          //Columna de variación con formato de moneda
                                    }
                                    else
                                    {
                                        dt.Rows[fila][va] = string.Format(nfi, "{0:C}", TotalCDIV);          //Columna de variación con formato de moneda
                                    }
                                }
                                else
                                if(fila == nconceptos - 3) // Utilidad Mensual
                                {
                                    concepto = "Utilidad Mensual";
                                    UtilidadMP = IngresoMensualP - TotalCDIP; //Aqui Obtengo la utilida Mensual, Ingreso Mensual Presupuesto - Total con Financiamiento de Presupuesto
                                    UtilidadMR = IngresoMensualR - TotalCDIR; //""
                                    UtilidadMV = UtilidadMR - UtilidadMP; // Aqui Obtengo la Variación Mensual
                                    ActualizarDatosDeReporte(concepto, UtilidadMP, UtilidadMR, UtilidadMV, idreporte, IdR); //Aquí actualizo la tabla DatosDeReporte en la fila(concepto) correspondiente para: Presupuesto,Real,Variación
                                    if (CambiarPorcentaje)
                                    {
                                        ActualizarUtilidadMSql(UtilidadMP, UtilidadMR, UtilidadMV, idreporte);  //Actualizar los datos a la Base de Datos
                                    }
                                    else
                                    {
                                        EnviarUtilidadaMSql(UtilidadMP, UtilidadMR, UtilidadMV, idreporte);  //Enviar los datos a la Base de Datos
                                    }
                                    
                                    if(UtilidadMP < 0)
                                    {
                                        Aux = UtilidadMP * -1;
                                        dt.Rows[fila][pre] = "-" + string.Format(nfi, "{0:C}", Aux);         //Columna de presupuesto negativo con formato de moneda
                                    }
                                    else
                                    {
                                        dt.Rows[fila][pre] = string.Format(nfi, "{0:C}", UtilidadMP);         //Columna de presupuesto con formato de moneda
                                    }
                                    
                                    if(UtilidadMR < 0)
                                    {
                                        Aux = UtilidadMR * -1;
                                        dt.Rows[fila][re] = "-" + string.Format(nfi, "{0:C}", Aux);           //Columna de Real negativo con formato de moneda
                                    }
                                    else
                                    {
                                        dt.Rows[fila][re] = string.Format(nfi, "{0:C}", UtilidadMR);           //Columna de Real con formato de moneda
                                    }
                                   
                                    if (UtilidadMV < 0)
                                    {
                                        Aux = UtilidadMV * -1;
                                        dt.Rows[fila][va] = "-" + string.Format(nfi, "{0:C}", Aux);          //Columna de variación negativo con formato de moneda
                                    }
                                    else
                                    {
                                        dt.Rows[fila][va] = string.Format(nfi, "{0:C}", UtilidadMV);          //Columna de variación con formato de moneda
                                    }
                                }
                                else
                                if(fila == nconceptos - 2) //Utilidad Acumulada
                                {
                                    concepto = "Utilidad Acumulada";
                                    Columna = 'P';
                                    UtilidadAP = ObtenerUtilidadAcumulada(UtilidadMP, Columna, IdR, anio, mes);       //Aqui Obtengo la Utilidad Acumulada para Presupuesto: Utilidad Mensual Presupuesto mes actual + Utilidad Acumulada mes anterior  
                                    Columna = 'R';
                                    UtilidadAR = ObtenerUtilidadAcumulada(UtilidadMR, Columna, IdR, anio, mes);   //Aqui Obtengo la Utilidad Acumulada para Real: Utilidad Mensual Presupuesto mes actual + Utilidad Acumulada mes anterior
                                    UtilidadAV = UtilidadAR - UtilidadAP; //Aquí obtengo la variación de la utilidad Acumulada Actual
                                    ActualizarDatosDeReporte(concepto, UtilidadAP, UtilidadAR, UtilidadAV, idreporte, IdR); //Aquí actualizo la tabla DatosDeReporte en la fila(concepto) correspondiente para: Presupuesto,Real,Variación
                                    if (CambiarPorcentaje)
                                    {
                                        ActualizarUtilidadAaSql(UtilidadAP, UtilidadAR, UtilidadAV, idreporte);
                                    }
                                    else
                                    {
                                        EnviarUtilidadAaSql(UtilidadAP, UtilidadAR, UtilidadAV, idreporte);
                                    }
                                    if (UtilidadAP < 0)
                                    {
                                        Aux = UtilidadAP * -1;
                                        dt.Rows[fila][pre] ="-" + string.Format(nfi, "{0:C}", Aux);         //Columna de presupuesto negativo con formato de moneda
                                    }
                                    else
                                    {
                                        dt.Rows[fila][pre] = string.Format(nfi, "{0:C}", UtilidadAP);         //Columna de presupuesto con formato de moneda
                                    }
                                   
                                    if(UtilidadAR < 0)
                                    {
                                        Aux = UtilidadAR * -1;
                                        dt.Rows[fila][re] = "-" + string.Format(nfi, "{0:C}", Aux);           //Columna de Real negativo con formato de moneda
                                    }
                                    else
                                    {
                                        dt.Rows[fila][re] = string.Format(nfi, "{0:C}", UtilidadAR);           //Columna de Real con formato de moneda
                                    }
                                    
                                    if (UtilidadAV < 0)
                                    {
                                        Aux = UtilidadAV * -1;
                                        dt.Rows[fila][va] = "-" + string.Format(nfi, "{0:C}", Aux);          //Columna de variación negativo con formato de moneda
                                    }
                                    else
                                    {
                                        dt.Rows[fila][va] = string.Format(nfi, "{0:C}", UtilidadAV);          //Columna de variación con formato de moneda
                                    }
                          
                                    TotalCDP = 0.0;     //limpiar las variables acomuladoras
                                    TotalCDR = 0.0;
                                    //TotalCDV = 0.0;

                                }
                                else
                                if(fila == nconceptos - 1)  // Financiamiento
                                {
                                    concepto = "Financiamiento";
                                    FinanciamientoP = presupuesto; // ObtenerFinanciamientoP((fila + 1),idreporte); //Aqui Obtengo el Financiamiento para Presupuesto: Se ingresa de forma Manual desde el Archvio de Excel
                                    Columna = 'R';
                                    FinanciamientoR = ObtenerFinanciamientoR(TotalCDIR, IngresoMensualR, UtilidadMR, Columna, IdR, anio, mes);//Aqui Obtengo el Financiamiento para Real:
                                    if (CambiarPorcentaje)
                                    {
                                        ActualizarFinanciamientoaSql(FinanciamientoP, FinanciamientoR, idreporte);
                                    }
                                    else
                                    {
                                        EnviarFinanciamientoaSql(FinanciamientoP, FinanciamientoR, idreporte);
                                    }
                                    FinanciamientoV = FinanciamientoR - FinanciamientoP;
                                    ActualizarDatosDeReporte(concepto, FinanciamientoP, FinanciamientoR, FinanciamientoV, idreporte, IdR); //Aquí actualizo la tabla DatosDeReporte en la fila(concepto) correspondiente para: Presupuesto,Real,Variación
                                    dt.Rows[fila][pre] = string.Format(nfi, "{0:C}", FinanciamientoP);         //Columna de presupuesto con formato de moneda
                                    dt.Rows[fila][re] = string.Format(nfi, "{0:C}", FinanciamientoR);           //Columna de Real con formato de moneda
                                    if (FinanciamientoV < 0)
                                    {
                                        Aux = FinanciamientoV * -1;
                                        dt.Rows[fila][va] = "-" + string.Format(nfi, "{0:C}", Aux);          //Columna de variación con formato de moneda
                                    }
                                    else
                                    {
                                        dt.Rows[fila][va] = string.Format(nfi, "{0:C}", FinanciamientoV);          //Columna de variación con formato de moneda
                                    }
                                }
                                

                                ++fila;    //Incremento la posición de la fila
                            }
                            else
                            {
                                //Aqui Agrego los costos en cada fila de cada Columna
                                if (presupuesto < 0)
                                {
                                    Aux = presupuesto * -1;
                                    dt.Rows[fila][pre] = "-" + string.Format(nfi, "{0:C}", Aux); //Columna de presupuesto negativo con formato de moneda
                                }
                                else
                                {
                                    dt.Rows[fila][pre] = string.Format(nfi, "{0:C}", presupuesto); //Columna de presupuesto con formato de moneda
                                }
                               
                                if (real < 0)
                                {
                                    Aux = real * -1;
                                    dt.Rows[fila][re] ="-" + string.Format(nfi, "{0:C}", Aux);           //Columna de Real negativo con formato de moneda
                                }
                                else
                                {
                                    dt.Rows[fila][re] = string.Format(nfi, "{0:C}", real);           //Columna de Real con formato de moneda
                                }
                               
                                if(variacion < 0)
                                {
                                    Aux = variacion * -1;
                                    dt.Rows[fila][va] = "-" + string.Format(nfi, "{0:C}", Aux);       //Columna de variación con formato de moneda
                                }
                                else
                                {
                                    dt.Rows[fila][va] = string.Format(nfi, "{0:C}", (variacion));       //Columna de variación con formato de moneda
                                }
                               
                                ++fila;
                            }

                        }
                        else
                        {
                            fila = 0; //Asigno valor 0 la variable fila que lleva el control de los conceptos de cada reporte

                        }
                       

                    } //Fin de While
                   
                }
            }
            catch (Exception ex)
            {
                AlertDanger.Visible = true;
                lblDanger.Text = "<strong><strong>Detalles de Error:</strong> " + ex.Message;
            }

        }
        protected string ObtenerMesProy(Int64 IdContrato,Int64 IdReporte) //Metodo que devuelve el mes y anio como un string como Proyección
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string ObtenerMesProy = "Select MesProyeccion From ProyeccionPorContrato Where IdReportes = @idreporte AND IdContrato = @idcontrato";
                    OrdenSql= new SqlCommand(ObtenerMesProy, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idreporte", IdReporte);
                    OrdenSql.Parameters.AddWithValue("@idcontrato", IdContrato);
                    Mesp = Convert.ToString(OrdenSql.ExecuteScalar());
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                AlertDanger.Visible = true;
                lblDanger.Text = "<strong><strong>Detalles de Error:</strong> " + ex.Message;
            }
            return Mesp;
        }
        protected void LlenarFilasTotales(int IdR, DataTable dt, string totalpre, string totalreal,string totalvar) //Metodo para obtener el Id de cada Concepto y posteriormente obtener el Id de cada reporte para poder llenar las columnas
        {
            Int64 idconcepto;
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string Concepto = "Select IdConcepto From Concepto Where IdResidencia = @idresidencia Order by IdConcepto";
                    OrdenSqlConcepto = new SqlCommand(Concepto, con);
                    con.Open();
                    OrdenSqlConcepto.Parameters.AddWithValue("@idresidencia", IdR);
                    LeerConcepto = OrdenSqlConcepto.ExecuteReader();
                    while (LeerConcepto.Read())
                    {
                        idconcepto = Convert.ToInt64(LeerConcepto[0].ToString());
                        CalcularTotalesIdReporte(idconcepto,IdR,dt,totalpre,totalreal,totalvar, contadorConceptos); //Metodo para obtener el Id de cada Reporte
                        presupuesto = 0;
                        real = 0;
                        variacion = 0;
                        ++contadorConceptos;
                    }
                    contadorConceptos = 0;
                }
            }
            catch (Exception ex)
            {
                AlertDanger.Visible = true;
                lblDanger.Text = "<strong><strong>Detalles de Error:</strong> " + ex.Message;
            }
        }
        protected void CalcularTotalesIdReporte(Int64 idconcepto,int IdR, DataTable dt,string totalpre,string totalreal,string totalvar,int contadorConceptos) //Aqui Obtengo el Id de cada reporte
        {
            int nconceptos = ObtenerNumConceptos(IdR);
            Int64 idreporte;
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string Reporte = "Select IdReporte From Reportes Where IdResidencia = @idresidencia  Order by FechaDeReporte";
                    OrdenSql = new SqlCommand(Reporte, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idresidencia", IdR);
                    LeerReporte = OrdenSql.ExecuteReader();
                    while (LeerReporte.Read())
                    {
                        idreporte = Convert.ToInt64(LeerReporte[0].ToString());
                        CalcularTotales(idconcepto, idreporte, dt, totalpre, totalreal, totalvar, contadorConceptos,nconceptos); //Aqui mando a llamar el metodo que se encargará del llenado de las columnas de los Totales
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                AlertDanger.Visible = true;
                lblDanger.Text = "<strong><strong>Detalles de Error:</strong> " + ex.Message;
            }
        }
        protected void CalcularTotales(Int64 idconcepto,Int64 idreporte,DataTable dt,string totalpre,string totalreal,string totalvar,int contadorConceptos,int nconceptos)  //Aqui realizo la sumade cada columna Presupuesto,Real de cada Reporte
        {  
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat; //Creo Objeto para personalizar el símbolo de moneda
            nfi.CurrencyPositivePattern = 2; //Establesco la posicion del simbolo de moneda
            double Aux;
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string DatosDeReporte = "Select Presupuesto,Real,Variacion From DatosDeReporte Where IdReporte = @idreporte AND IdConcepto = @idconcepto";
                    OrdenSql = new SqlCommand(DatosDeReporte, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idreporte", idreporte);
                    OrdenSql.Parameters.AddWithValue("@idconcepto", idconcepto);
                    LeerDatosDeReporte = OrdenSql.ExecuteReader();
                    while (LeerDatosDeReporte.Read())
                    {
                        presupuesto +=  Convert.ToDouble(LeerDatosDeReporte[0].ToString());
                        real += Convert.ToDouble(LeerDatosDeReporte[1].ToString());
                        variacion = real - presupuesto;
                        if(presupuesto < 0)
                        {
                            Aux = presupuesto * -1;
                            dt.Rows[contadorConceptos][totalpre] = "-" + string.Format(nfi, "{0:C}", Aux);         //Columna Total presupuesto negativo con formato de moneda 
                        }
                        else
                        {
                            dt.Rows[contadorConceptos][totalpre] = string.Format(nfi, "{0:C}", presupuesto);         //Columna Total presupuesto con formato de moneda
                        }
                       
                        if(real < 0)
                        {
                            Aux = real * -1;
                            dt.Rows[contadorConceptos][totalreal] = "-" + string.Format(nfi, "{0:C}", Aux);           //Columna Total Real negativo con formato de moneda
                        }
                        else
                        {
                            dt.Rows[contadorConceptos][totalreal] = string.Format(nfi, "{0:C}", real);           //Columna Total Real con formato de moneda
                        }
                       
                        if (variacion < 0)
                        {
                            Aux = variacion * -1;
                            dt.Rows[contadorConceptos][totalvar] = "-" + string.Format(nfi, "{0:C}", Aux);          //Columna de variación negativo con formato de moneda
                        }
                        else
                        {
                            dt.Rows[contadorConceptos][totalvar] = string.Format(nfi, "{0:C}", variacion);          //Columna de variación con formato de moneda
                        }
                       
                        if (contadorConceptos == nconceptos - 7)
                        {
                            LblFooterCDP.Text = string.Format(nfi, "{0:C}", presupuesto);               //Aquí muestro el Costo Directo para Presupuesto
                            LblFooterCDR.Text = string.Format(nfi, "{0:C}", real);                       //Aquí muestro el Costo Directo para Real
                            CDP = presupuesto;
                            CDR = real;
                        }
                        else if (contadorConceptos == nconceptos - 6)
                        {
                            LblFooterIP.Text = string.Format(nfi, "{0:C}", presupuesto);         //Aquí muestro el Indirecto de Campo para Presupuesto
                            LblFooterIR.Text = string.Format(nfi, "{0:C}", real);                //Aquí muestro el Indirecto de Campo para Real
                            ICP = presupuesto;
                            ICR = real;
                        }
                        else if (contadorConceptos == nconceptos - 5) 
                        {
                            LblFooterIOCP.Text = string.Format(nfi, "{0:C}", presupuesto);  //Aquí muestro el Indirecto de Oficina Central para presupuesto
                            LblFooterIOCR.Text = string.Format(nfi, "{0:C}", real);         //Aquí muestro el Infirecto de Oficina Central para Real
                            IOCP = presupuesto;
                            IOCR = real;
                        }
                        else if(contadorConceptos == nconceptos - 4)
                        {
                            TotalCDIP = CDP + ICP + IOCP;
                            TotalCDIR = CDR + ICR + IOCR;
                            LblFooterTCDIP.Text = string.Format(nfi, "{0:C}", TotalCDIP);       //Aquí muestro el Total (Costo Directo + Indirecto) para presupuesto (Costo Directo + Indirecto de Campo + Indirecto de Oficina Central)
                            LblFooterTCDIR.Text = string.Format(nfi, "{0:C}", TotalCDIR);       //Aquí muestro el Total (Costo Directo + Indirecto) para real   (Costo Directo + Indirecto de Campo + Indirecto de Oficina Central)
                        }
                        else
                        if (contadorConceptos == nconceptos - 1)
                        {
                            LblFooterFP.Text = string.Format(nfi, "{0:C}", presupuesto);        //Aquí muestro el Financiamiento para presupuesto
                            LblFooterFR.Text = string.Format(nfi, "{0:C}", real);               //Aquí muestro el Financiamiento para real
                            
                            Financiamientop = presupuesto;
                            Financiamientor = real;
                        }else
                        if (contadorConceptos == 0)
                        {
                            MontoTP = presupuesto;
                            LblFooterMCP.Text = string.Format(nfi, "{0:C}", presupuesto);
                            MontoR = real;
                            LblFooterMCR.Text = string.Format(nfi, "{0:C}", real);          //Aqui muestro el Monto del Contrato para Real, es el Total Real de Ingresos
                        }
                    }
                    
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                AlertDanger.Visible = true;
                lblDanger.Text = "<strong><strong>Detalles de Error:</strong> " + ex.Message;
            }
        }
        protected Boolean VerificarSiExisteReporte(double idreporte) //Metodo que devuelve verdadero si existe el reporte en las Tablas de los calculos 
        {
            
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string ConsultarReporte = "Select 'true' From UtilidadMensual As um ,UtilidadAcumulada As ua, Financiamiento As f" +
                        " Where  um.IdReporte = @idreporte AND ua.IdReporte = @idreporte AND f.IdReporte = @idreporte";
                    OrdenSql = new SqlCommand(ConsultarReporte, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idreporte", idreporte);
                    ExisteReporte = Convert.ToBoolean(OrdenSql.ExecuteScalar());
                    con.Close();
                }

            }
            catch (Exception ex)
            {
                AlertDanger.Visible = true;
                lblDanger.Text = "<strong><strong>Detalles de Error:</strong> " + ex.Message;
            }
            return ExisteReporte;
        }

      
        protected void ActualizarDatosDeReporte(string concepto,double Presupuesto,double Real,double Variacion,double idreporte,int IdR) //Metodo para actualizar la Tabla DatosDeReporte
        {
            
            double nConcepto = ObtenerIdConceptoActualizar(concepto, IdR);
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                   
                        String queryArea = "UPDATE DatosDeReporte SET Presupuesto =@presupuesto,Real =@real,Variacion=@variacion WHERE IdReporte =@idreporte AND IdConcepto = @nConcepto"; // Actualizo Presupuesto,Real y Variación de la Tabla DatosDeReporte
                        OrdenSql = new SqlCommand(queryArea, con);
                        con.Open();
                        OrdenSql.Parameters.AddWithValue("@presupuesto", Presupuesto);
                        OrdenSql.Parameters.AddWithValue("@real", Real);
                        OrdenSql.Parameters.AddWithValue("@variacion", Variacion);
                        OrdenSql.Parameters.AddWithValue("@idreporte", idreporte);
                        OrdenSql.Parameters.AddWithValue("@nConcepto", nConcepto);
                        OrdenSql.ExecuteNonQuery();
                        con.Close();
 
                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }
       
        protected void EnviarUtilidadaMSql(double UtilidadMP, double UtilidadMR, double UtilidadMV, double idreporte) //Aqui Hago un Insert INTO EN la tabla UtilidadMensual Para guardar los costos por reporte mensual
        {
            Boolean ExisteReporte = VerificarSiExisteReporte(idreporte);
            if (!ExisteReporte)
            {
                try
                {
                    con = new SqlConnection(strConexion);
                    using (con)
                    {
                        String queryArea = "INSERT INTO UtilidadMensual (IdReporte,Presupuesto,Real,Variacion) VALUES (@idreporte, @presupuesto, @real, @variacion)"; //Inserto la suma de todos los costos a la tabla UtilidadMensual
                        OrdenSql = new SqlCommand(queryArea, con);
                        con.Open();
                        OrdenSql.Parameters.AddWithValue("@idreporte", idreporte);
                        OrdenSql.Parameters.AddWithValue("@presupuesto", UtilidadMP);
                        OrdenSql.Parameters.AddWithValue("@real", UtilidadMR);
                        OrdenSql.Parameters.AddWithValue("@variacion", UtilidadMV);
                        OrdenSql.ExecuteNonQuery();
                        con.Close();
                    }

                }
                catch (Exception ex)
                {

                    AlertDanger.Visible = true;
                    lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
                }
            }
                
        }
        protected void ActualizarUtilidadMSql(double UtilidadMP, double UtilidadMR, double UtilidadMV, double idreporte) //Metodo para actualizar la Utilidad Mensual solo si se cambian los porcentajes
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {

                    String queryArea = "UPDATE UtilidadMensual SET Presupuesto = @presupuesto,Real =@real,Variacion=@variacion WHERE IdReporte =@idreporte"; // Actualizo Presupuesto,Real y Variación de la Tabla DatosDeReporte
                    OrdenSql = new SqlCommand(queryArea, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@presupuesto", UtilidadMP);
                    OrdenSql.Parameters.AddWithValue("@real", UtilidadMR);
                    OrdenSql.Parameters.AddWithValue("@variacion", UtilidadMV);
                    OrdenSql.Parameters.AddWithValue("@idreporte", idreporte);
                    OrdenSql.ExecuteNonQuery();
                    con.Close();

                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }
        protected void EnviarUtilidadAaSql(double UtilidadAP, double UtilidadAR, double UtilidadAV, double idreporte) //Envio la Utilidad Acumulada a la Base de Datos
        {
            Boolean ExisteReporte = VerificarSiExisteReporte(idreporte);
            if (!ExisteReporte)
            {
                try
                {
                    con = new SqlConnection(strConexion);
                    using (con)
                    {
                        String queryArea = "INSERT INTO UtilidadAcumulada (IdReporte,Presupuesto,Real,Variacion) VALUES (@idreporte, @presupuesto, @real, @variacion)"; //Inserto la suma de todos los costos a la tabla UtilidadAcumulada
                        OrdenSql = new SqlCommand(queryArea, con);
                        con.Open();
                        OrdenSql.Parameters.AddWithValue("@idreporte", idreporte);
                        OrdenSql.Parameters.AddWithValue("@presupuesto", UtilidadAP);
                        OrdenSql.Parameters.AddWithValue("@real", UtilidadAR);
                        OrdenSql.Parameters.AddWithValue("@variacion", UtilidadAV);
                        OrdenSql.ExecuteNonQuery();
                        con.Close();
                    }

                }
                catch (Exception ex)
                {

                    AlertDanger.Visible = true;
                    lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
                }
            }
               
        }
        protected void ActualizarUtilidadAaSql(double UtilidadAP, double UtilidadAR, double UtilidadAV, double idreporte) //Metodo para actualizar la utilidad acumulada del reporte,solo si se cambiaron porcentajes
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {

                    String queryArea = "UPDATE UtilidadAcumulada SET Presupuesto = @presupuesto,Real =@real,Variacion=@variacion WHERE IdReporte =@idreporte"; // Actualizo Presupuesto,Real y Variación de la Tabla DatosDeReporte
                    OrdenSql = new SqlCommand(queryArea, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@presupuesto", UtilidadAP);
                    OrdenSql.Parameters.AddWithValue("@real", UtilidadAR);
                    OrdenSql.Parameters.AddWithValue("@variacion", UtilidadAV);
                    OrdenSql.Parameters.AddWithValue("@idreporte", idreporte);
                    OrdenSql.ExecuteNonQuery();
                    con.Close();

                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }
        protected void EnviarFinanciamientoaSql(double FinanciamientoP, double FinanciamientoR, double idreporte) //Enviar el financiamiento a la Base de Datos
        {
            Boolean ExisteReporte = VerificarSiExisteReporte(idreporte);
            if (!ExisteReporte)
            {
                try
                {
                    con = new SqlConnection(strConexion);
                    using (con)
                    {
                        String queryArea = "INSERT INTO Financiamiento (IdReporte,Presupuesto,Real) VALUES (@idreporte, @presupuesto, @real)"; //Inserto el financiamento obtenido en la Tabla Financiamiento
                        OrdenSql = new SqlCommand(queryArea, con);
                        con.Open();
                        OrdenSql.Parameters.AddWithValue("@idreporte", idreporte);
                        OrdenSql.Parameters.AddWithValue("@presupuesto", FinanciamientoP);
                        OrdenSql.Parameters.AddWithValue("@real", FinanciamientoR);
                        OrdenSql.ExecuteNonQuery();
                        con.Close();
                    }

                }
                catch (Exception ex)
                {

                    AlertDanger.Visible = true;
                    lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
                }
            }
                
        }
        protected void ActualizarFinanciamientoaSql(double FinanciamientoP, double FinanciamientoR, double idreporte) //Metodo para actualizar el financiamiento solo si se cambia el porcentaje
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {

                    String queryArea = "UPDATE Financiamiento SET Presupuesto = @presupuesto,Real =@real WHERE IdReporte =@idreporte"; // Actualizo Presupuesto,Real y Variación de la Tabla DatosDeReporte
                    OrdenSql = new SqlCommand(queryArea, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@presupuesto", FinanciamientoP);
                    OrdenSql.Parameters.AddWithValue("@real", FinanciamientoR);
                    OrdenSql.Parameters.AddWithValue("@idreporte", idreporte);
                    OrdenSql.ExecuteNonQuery();
                    con.Close();

                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        } 
      
        protected double ObtenerUtilidadAcumulada(double UtilidadMActual, Char Columna, int IdR, string anio, string mes) //Metodo para obtener la Utilidad Acumulada del mes anterior, parametros recibidos: 
        {                                                                                                                  //Tipo de Columna P:Presupuesto y R:Real, Id de la residencia, año y mes
            Boolean ExisteFechaAnterior = false;
            Int64 IdReporteAnterior;
            int m;
            string fechaAnterior;
            if (mes.Length < 2)
            {
                m = (Convert.ToInt16(mes) - 1);
                if (m == 0)
                {
                    
                    fechaAnterior = (Convert.ToInt16(anio) -1) + "-" +"12%"; //Agrego un cero para formar la fecha y poder hacer la consulta con LIKE
                }
                else
                {
                    fechaAnterior = anio + "-" +"0"+ m +"%";
                }
                
            }
            else
            {
                m= (Convert.ToInt16(mes) - 1);
                if(m < 10)
                {
                    fechaAnterior = anio + "-" + "0" + m +"%";
                }
                else
                {
                    fechaAnterior = anio + "-" + m +"%";
                }
            }

            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string IdreporteAnterior = "Select 'true',IdReporte From Reportes Where EXISTS (Select IdReporte Where IdResidencia = @idresidencia AND FechaDeReporte Like @fechaanterior) ";
                    OrdenSql = new SqlCommand(IdreporteAnterior, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idresidencia", IdR);
                    OrdenSql.Parameters.AddWithValue("@fechaanterior", fechaAnterior);
                    LeerFechaRA = OrdenSql.ExecuteReader();
                    while (LeerFechaRA.Read())
                    {
                        ExisteFechaAnterior = Convert.ToBoolean(LeerFechaRA[0].ToString());
                        IdReporteAnterior = Convert.ToInt64(LeerFechaRA[1].ToString());
                        if (!ExisteFechaAnterior)       //Pregunto si no hay un mes anterior entonces la Utilidad Mensual Actual se guarda en Utilidad Acumulada Actual 
                        {
                            UtilidadAA = UtilidadMActual;               
                        }
                        else
                        {                              // Si Existe un mes anterior entonces la Utilidad del Mes Actual se suma con la Utilidad Acumulada del Mes Anterior
                            UtilidadAA = UtilidadMActual + CalcularUAA(Columna, IdReporteAnterior);
                        }
                    }
                    if (!ExisteFechaAnterior)       
                    {
                        UtilidadAA = UtilidadMActual;
                    }
                    con.Close();
                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }

            return UtilidadAA;          //Devuelvo la Utilidad Acumuluda del Mes Anterior
        }
        //  F O R M U L A  P A R A   O B T E N E R   E L   F I N A N C I A M I E N T O
        /*SI la Utilidad Mensual Actual es < 0 entonces
           *Nuevamente revisa, SI la Utilidad Acumulada del Mes Anterior es < al valor Absoluto de la Utilidad Mensual Actual, entonces
           * Vuelvo a revisar SI la Utilidad Acumulada del Mes Anterior es < 0, entonces FINANCIAMIENTO = Sumar(Financiamiento Anterior + Valor Absoluto de Utilidad Mensual Actual) * 0.01, SINO entonces FINANCIAMIENTO = Valor absoluto(Utilidad Acumulada del Mes Anterior + Utilidad Mensual Actual) * 0.01
           * SINO FINANCIAMIENTO es = '0'
           * SINO entonces REVISA SI la Utilidad Mensual Actual es > Financiamiento Anterior entonces el FINANCIAMIENTO = '0' SINO FINANCIAMIENTO = Financiamiento Anterio * 1.01
           */
     
        protected double ObtenerFinanciamientoR(double TotalCDIPoR, double IngresoMensualPoR, double UtilidadMActual, Char Columna, int IdR, string anio, string mes)
        {
            Boolean ExisteFechaAnterior;
            Int64 IdReporteAnterior;
            int m;
            string fechaAnterior;
            if (mes.Length < 2)
            {
                m = (Convert.ToInt16(mes) - 1);
                if (m == 0)
                {

                    fechaAnterior = (Convert.ToInt16(anio) - 1) + "-" + "12%";
                }
                else
                {
                    fechaAnterior = anio + "-" + "0" + m + "%";//Agrego un cero para formar la fecha y poder hacer la consulta con LIKE
                }
            }
            else
            {
                m = (Convert.ToInt16(mes) - 1);
                if (m < 10)
                {
                    fechaAnterior = anio + "-" + "0" + m + "%"; //Agrego un cero para formar la fecha y poder hacer la consulta con LIKE
                }
                else
                {
                    fechaAnterior = anio + "-" + m + "%";
                }
            }

            try
            {
                ExisteFechaAnterior = ExisteFechaAnt(IdR,fechaAnterior);
                IdReporteAnterior = ObtenerIdReporteAnterior(IdR, fechaAnterior);
                
                if (ExisteFechaAnterior)       //Pregunto si hay un mes anterior 
                        {
                    // Existe un mes anterior 
                    if (UtilidadMActual < 0)    //SI la Utilidad Mensual Actual es < 0 entonces
                    {
                        if (CalcularUAA(Columna, IdReporteAnterior) < Math.Abs(UtilidadMActual)) //Nuevamente revisa, SI la Utilidad Acumulada del Mes Anterior es < al valor Absoluto de la Utilidad Mensual Actual
                        {
                            if (CalcularUAA(Columna, IdReporteAnterior) < 0) //Vuelvo a revisar SI la Utilidad Acumulada del Mes Anterior es < 0
                            {
                                FinanciamientoA = (CalcularFntoA(Columna, IdReporteAnterior) + Math.Abs(UtilidadMActual)) * 0.01; //entonces FINANCIAMIENTO = Sumar(Financiamiento Anterior + Valor Absoluto de Utilidad Mensual Actual) * 0.01
                            }
                            else
                            {
                                FinanciamientoA = Math.Abs(CalcularUAA(Columna, IdReporteAnterior) + UtilidadMActual) * 0.01; //SINO entonces FINANCIAMIENTO = Valor absoluto(Utilidad Acumulada del Mes Anterior + Utilidad Mensual Actual) * 0.01
                            }
                        }
                        else
                        {
                            FinanciamientoA = 0.00; //SINO FINANCIAMIENTO es = '0'
                        }
                    }
                    else
                    {
                        if (UtilidadMActual > CalcularFntoA(Columna, IdReporteAnterior)) // SINO entonces REVISA SI la Utilidad Mensual Actual es > Financiamiento Anterior
                        {
                            FinanciamientoA = 0.00;   //SINO FINANCIAMIENTO es = '0'
                        }
                        else
                        {
                            FinanciamientoA = CalcularFntoA(Columna, IdReporteAnterior) * 1.01;  //SINO FINANCIAMIENTO = Financiamiento Anterio * 1.01
                        }
                    }

                }
                else
               {
                    if (IngresoMensualPoR < TotalCDIPoR)
                    {
                        FinanciamientoA = TotalCDIPoR * 0.01;
                    }
                    else
                    {
                        FinanciamientoA = 0.00;
                    }
                }
            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }

            return FinanciamientoA;
        }
        protected Boolean ExisteFechaAnt(int IdR, string fechaAnterior)
        {
            
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string IdreporteAnterior = "Select 'true' From Reportes Where  IdResidencia = @idresidencia AND FechaDeReporte Like @fechaanterior ";
                    OrdenSql = new SqlCommand(IdreporteAnterior, con);
                    con.Open();
                    OrdenSql.Parameters.Clear();
                    OrdenSql.Parameters.AddWithValue("@idresidencia", IdR);
                    OrdenSql.Parameters.AddWithValue("@fechaanterior", fechaAnterior);
                    ExisteFechaAnterior = Convert.ToBoolean(OrdenSql.ExecuteScalar());
                    con.Close();
               
                }
            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
            return ExisteFechaAnterior;
        }
        protected Int64 ObtenerIdReporteAnterior(int IdR,string fechaAnterior)
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string IdreporteAnterior = "Select IdReporte From Reportes Where  IdResidencia = @idresidencia AND FechaDeReporte Like @fechaanterior ";
                    OrdenSql = new SqlCommand(IdreporteAnterior, con);
                    con.Open();
                    OrdenSql.Parameters.Clear();
                    OrdenSql.Parameters.AddWithValue("@idresidencia", IdR);
                    OrdenSql.Parameters.AddWithValue("@fechaanterior", fechaAnterior);
                    IdReporteAnterior = Convert.ToInt64(OrdenSql.ExecuteScalar());
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
            return IdReporteAnterior;
        }
        protected double CalcularFntoA(char Columna, Int64 idreporteA)      //Metodo para Obtener el financiamiento del Mes Anterior
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    if (Columna == 'P') //Aquí obtiene la Utilidad Acumulada del mes anterior para PRESUPUESTO
                    {
                        string ObtenerUtilidadAP = "Select Presupuesto From Financiamiento Where IdReporte = @idreporteAnterior";
                        OrdenSqlUtilidadA = new SqlCommand(ObtenerUtilidadAP, con);
                        con.Open();
                        OrdenSqlUtilidadA.Parameters.AddWithValue("@idreporteAnterior", idreporteA);
                        FinanciamientoA = Convert.ToDouble(OrdenSqlUtilidadA.ExecuteScalar());
                        con.Close();
                    }
                    else if (Columna == 'R') //Aquí obtiene la Utilidad Acumulada del mes anterior para REAL
                    {
                        string ObtenerUtilidadAR = "Select Real From Financiamiento Where IdReporte = @idreporteAnterior";
                        OrdenSqlUtilidadA = new SqlCommand(ObtenerUtilidadAR, con);
                        con.Open();
                        OrdenSqlUtilidadA.Parameters.AddWithValue("@idreporteAnterior", idreporteA);
                        FinanciamientoA = Convert.ToDouble(OrdenSqlUtilidadA.ExecuteScalar());
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
            return FinanciamientoA;
        }

        protected double CalcularUAA(char Columna, Int64 idreporteA)      //Metodo para obtener la Utilidad Acumulada del Mes Anterior
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    if (Columna == 'P') //Aquí obtiene la Utilidad Acumulada del mes anterior para PRESUPUESTO
                    {
                        string ObtenerUtilidadAP = "Select Presupuesto From UtilidadAcumulada Where IdReporte = @idreporteAnterior";
                        OrdenSqlUtilidadA = new SqlCommand(ObtenerUtilidadAP, con);
                        con.Open();
                        OrdenSqlUtilidadA.Parameters.AddWithValue("@idreporteAnterior", idreporteA);
                        UtilidadAA = Convert.ToDouble(OrdenSqlUtilidadA.ExecuteScalar());
                        con.Close();
                    }else if (Columna == 'R') //Aquí obtiene la Utilidad Acumulada del mes anterior para REAL
                    {
                        string ObtenerUtilidadAR = "Select Real From UtilidadAcumulada Where IdReporte = @idreporteAnterior";
                        OrdenSqlUtilidadA = new SqlCommand(ObtenerUtilidadAR, con);
                        con.Open();
                        OrdenSqlUtilidadA.Parameters.AddWithValue("@idreporteAnterior", idreporteA);
                        UtilidadAA = Convert.ToDouble(OrdenSqlUtilidadA.ExecuteScalar());
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
            return UtilidadAA;
        }
      
        protected int ObtenerIdsReportes(int IdR)        //Metodo para obtener el Id de un reporte, recibe como parametro el Id de la residencia
        {
           
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String queryArea = "Select IdReporte From Reportes where IdResidencia = @idresidencia"; //Consulta para obtener el Id de un reporte
                    OrdenSql = new SqlCommand(queryArea, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idresidencia",IdR);
                    Idresidencia = Convert.ToUInt16(OrdenSql.ExecuteScalar());
                    con.Close();
                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
            return Idresidencia;
        }
        protected int ObtenerNumConceptos(int IdR) //Metodo para obtener el número de conceptos en total que hay en cada contrato, recibe Id de residencia (contrato)
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string querynConceptos = "Select COUNT(*) FROM Concepto Where IdResidencia = @idresidencia";
                    OrdenSql = new SqlCommand(querynConceptos,con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idresidencia", IdR);
                    numConceptos = Convert.ToInt16(OrdenSql.ExecuteScalar());
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
            return numConceptos;
        }
        protected void LbtnSalir_Click(object sender, EventArgs e)
        {
            Session.Remove("Usuario");
            Response.Redirect("Default.aspx");
        }
        protected void LimpiarGridView() //Limpiar el GridView del Reporte cargado desde Excel
        {
            GvDatosN.DataSource = "";
            GvDatosN.DataBind();
        }
      
        protected void LbtnEditar_Click(object sender, EventArgs e)
        {
            PnlTable.Visible = false;
            PnlEditar.Visible = true;
            LbtnContratos.Visible = false;
        }

        protected void LbtnNuevo_Click(object sender, EventArgs e)
        {
            
            PnlNuevo.Visible = true;
            LbtnAgregarProyeccion.Visible = true;
            LbtnOcultarN.Visible = false;
            LbtnVisualizar.Visible = false;
            PnlVisualizar.Visible = false;
            //Inhabilitar botones y cajas de texto
            LbtnCancelEditionC.Enabled = false;
            LbtnNuevo.Enabled = false;
            LbtnEliminar.Enabled = false;
            LbtnActualizar.Enabled = false;
            //Mostrar titulo del panel
            LblTitlePnlAgregarN.Visible = true;
            LblTitlePnlAgregarN.Text = "Agregar Nuevo Historial";
        }
        protected void LbtnAContratos_Click(object sender, EventArgs e)
        {
            LblTitlePnlActualizar.Text = "Actualizar Contrato";
            LbtnAProyeccion.Visible = false;
            LbtnAConceptos.Visible = false;
            LbtnACambiosC.Visible = true;

        }

       

        protected void LbtnAConceptos_Click(object sender, EventArgs e)
        {
            LblTitlePnlActualizar.Text = "Actualizar Conceptos";
            LbtnAConceptos.Visible = false;
            LbtnAProyeccion.Visible = false;
            LbtnACambiosCptos.Visible = true;
            LblAConcepto.Visible = true;
            LblAContrato.Visible = true;
            LblAContrato2.Visible = true;
            LblAObservacion.Visible = true;
            DdlContratoA.Visible = true;
            DdlConceptosA.Visible = true;
            TxtConceptoA.Visible = true;
            TxtObservacionA.Visible = true;

            Boolean ExisteContrato = VerificarSiExistenContratos();
            if (ExisteContrato)
            {
                string contrato = DdlContratoA.SelectedItem.ToString();
                Int64 IdR = Convert.ToInt64(ObtenerIdResidencia(contrato));
                Boolean ExistenConceptosEnContrato = VerificarSiExistenConceptos(IdR);
                if (ExistenConceptosEnContrato)
                {
                    CargarConceptosA(IdR);
                }
                else if (DdlConceptosA.SelectedValue.ToString().CompareTo("Ninguno") != 0)
                {
                    DdlConceptosA.Items.Clear();
                    DdlConceptosA.Items.Add("Ninguno");
                    LbtnACambiosCptos.Enabled = false;
                }


            }
            else if (DdlContratoA.SelectedValue.ToString().CompareTo("Ninguno") != 0 && DdlConceptosA.SelectedValue.ToString().CompareTo("Ninguno") != 0)
            {
                DdlContratoA.Items.Clear();
                DdlContratoA.Items.Add("Ninguno");
                DdlConceptosA.Items.Add("Ninguno");
                LbtnACambiosCptos.Enabled = false;
               
            }
        }
        protected void CargarConceptosA(Int64 IdR)
        {
            string concepto;
            Int64 IdC;
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String ObtenerResidencias = "Select Concepto From Concepto Where IdResidencia = @idresidencia ORDER BY IdConcepto";
                    OrdenSql = new SqlCommand(ObtenerResidencias, con);
                    OrdenSql.Parameters.AddWithValue("@idresidencia", IdR);
                    con.Open();

                    DdlConceptosA.DataSource = OrdenSql.ExecuteReader();
                    DdlConceptosA.DataTextField = "Concepto";
                    DdlConceptosA.DataBind();
                    con.Close();
                    concepto = DdlConceptosA.SelectedItem.ToString();
                    IdC = ObtenerIdConcepto(concepto, IdR);
                    MostrarConcepto(IdR, IdC);  //Muestro en el TxtBox el concepto a editar
                    MostrarObservacion(IdC);   //Muestro en el Txtbox la observación a editar
                    LbtnACambiosCptos.Enabled = true;
                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }

    protected void LbtnACambiosC_Click(object sender, EventArgs e)
        {
            PnlActualizar.Visible = false;
            LblTitlePnlActualizar.Visible = false;
            LbtnACambiosC.Visible = false;
            //habilitar botones y cajas de texto
            LbtnCancelEditionC.Enabled = true;
            LbtnNuevo.Enabled = true;
            LbtnEliminar.Enabled = true;
            LbtnActualizar.Enabled = true;
        }

        protected void LbtnAgregarProyeccion_Click(object sender, EventArgs e)
        {
            LbtnReporteMensualN.Visible = false;
            LbtnAgregarContrato.Visible = false;
            LbtnAgregarProyeccion.Visible = false;
            LbtnGuardarP.Visible = true;
            LblContratosProyeccion.Visible = true;
            DdlContratosProyeccion.Visible = true;
            LblMesProyeccion.Visible = true;
            DdlMesProyecccion.Visible = true;
            //Cambiar Texto a titulo del panel
            LblTitlePnlAgregarN.Visible = true;
            LblTitlePnlAgregarN.Text = "Agregar Proyección";
            if (DdlContratosProyeccion.SelectedIndex != -1)
            {
                string contrato = DdlContratosProyeccion.SelectedItem.ToString();
                Int64 IdR = Convert.ToInt64(ObtenerIdResidencia(contrato));
                Boolean ExistenReportes = VerificarSiExistenReportes(IdR);
                if (ExistenReportes)
                {
                    CargarFechasDeReportes(IdR); //Aqui Mando a llamar al metodo para que muestre los reportes por fechas en la lista despegable de eliminar reporte
                    LbtnGuardarP.Enabled = true;
                }
                else if (DdlMesProyecccion.SelectedValue.ToString().CompareTo("Ninguno") != 0)
                {
                    DdlMesProyecccion.Items.Clear();
                    DdlMesProyecccion.Items.Add("Ninguno");
                    LbtnGuardarP.Enabled = false;

                }
            }
            else if (DdlContratosProyeccion.SelectedValue.ToString().CompareTo("Ninguno") != 0 && DdlMesProyecccion.SelectedValue.ToString().CompareTo("Ninguno") != 0)
            {
                DdlContratosProyeccion.Items.Clear();
                DdlContratosProyeccion.Items.Add("Ninguno");
                DdlMesProyecccion.Items.Add("Ninguno");
                LbtnGuardarP.Enabled = false;

            }
        }

        protected void LbtnGuardarP_Click(object sender, EventArgs e)
        {
            PnlNuevo.Visible = false;
            LbtnReporteMensualN.Visible = true;
            LbtnAgregarContrato.Visible = true;
            LblContratosProyeccion.Visible = false;
            DdlContratosProyeccion.Visible = false;
            LblMesProyeccion.Visible = false;
            DdlMesProyecccion.Visible = false;
            LbtnGuardarP.Visible = false;
            //habilitar botones y cajas de texto
            LbtnCancelEditionC.Enabled = true;
            LbtnNuevo.Enabled = true;
            LbtnEliminar.Enabled = true;
            LbtnActualizar.Enabled = true;
            //No mostrar el Texto a titulo del panel
            LblTitlePnlAgregarN.Visible = false;
            string fechadereporte = DdlMesProyecccion.SelectedItem.ToString(); //Seleccionó una fecha de la liste despegable
            Int64 idreporte;
            int ncaracteres = fechadereporte.Length;
            string mes = fechadereporte.Substring(0, (ncaracteres - 5));
            string anio = fechadereporte.Remove(0, (ncaracteres - 4));
            string namecontrato = DdlContratosProyeccion.SelectedItem.ToString();  //Selecciona un contrato de la lista despegable de Contratos
            string mesProyeccion = mes + "(" + anio + ")";
            Int64 IdContrato = ObtenerIdResidencia(namecontrato); //Obtenemos el Id de la Residencia o Contrato
            if (mes.CompareTo("Enero") == 0)
            {
                mesyanio = anio + "-" + "01%";
            }
            else if (mes.CompareTo("Febrero") == 0)
            {
                mesyanio = anio + "-" + "02%";
            }
            else if (mes.CompareTo("Marzo") == 0)
            {
                mesyanio = anio + "-" + "03%";
            }
            else if (mes.CompareTo("Abril") == 0)
            {
                mesyanio = anio + "-" + "04%";
            }
            else if (mes.CompareTo("Mayo") == 0)
            {
                mesyanio = anio + "-" + "05%";
            }
            else if (mes.CompareTo("Junio") == 0)
            {
                mesyanio = anio + "-" + "06%";
            }
            else if (mes.CompareTo("Julio") == 0)
            {
                mesyanio = anio + "-" + "07%";
            }
            else if (mes.CompareTo("Agosto") == 0)
            {
                mesyanio = anio + "-" + "08%";
            }
            else if (mes.CompareTo("Septiembre") == 0)
            {
                mesyanio = anio + "-" + "09%";
            }
            else if (mes.CompareTo("Octubre") == 0)
            {
                mesyanio = anio + "-" + "10%";
            }
            else if (mes.CompareTo("Noviembre") == 0)
            {
                mesyanio = anio + "-" + "11%";
            }
            else if (mes.CompareTo("Diciembre") == 0)
            {
                mesyanio = anio + "-" + "12%";
            }
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String ObtenerIdReportePorfecha = "SELECT IdReporte FROM Reportes WHERE IdResidencia =@idcontrato AND FechaDeReporte Like @mesyanio"; //Consulta para obtener el Id de un reporte por fecha de reporte
                    OrdenSql = new SqlCommand(ObtenerIdReportePorfecha, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@mesyanio", mesyanio);
                    OrdenSql.Parameters.AddWithValue("@idcontrato", IdContrato);
                    idreporte = Convert.ToInt64(OrdenSql.ExecuteScalar());              //Me devuelve el Id del reporte
                    EnviarDatosParaProyeccion(idreporte,IdContrato, mesProyeccion); //Le paso los datos a este metodo para enviarlos a la BD en la Tabla ProyeccionPorContrato
                    con.Close();
                  
                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }
        protected void EnviarDatosParaProyeccion(Int64 IdReporte,Int64 IdContrato,string MesP)  //Metodo que se encarga del envio del Id de un reporte, contrato y el mes(año) de la proyección inicial que se mostrará en la grafica
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String InsertarProyeccion = "INSERT INTO ProyeccionPorContrato (IdReportes,IdContrato, MesProyeccion) VALUES (@idreporte, @idcontrato,@mesp)";
                    OrdenSql = new SqlCommand(InsertarProyeccion, con);
                    con.Open();
                    Boolean ExisteProy = ExisteProyeccion(IdContrato);
                    if (!ExisteProy) //Pregunto si no existe algún mes como proyección en la Tabla ProyeccionPorContrato, entonces se puede agregar una nueva proyección
                    {
                        OrdenSql.Parameters.AddWithValue("@idreporte", IdReporte);
                        OrdenSql.Parameters.AddWithValue("@idcontrato", IdContrato);
                        OrdenSql.Parameters.AddWithValue("@mesp",MesP);
                        OrdenSql.ExecuteNonQuery();
                        //AlertSuccess.Visible = true;
                        //lblSucces.Text = " ¡<strong>Proyección agregada</strong> con éxito!";
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "click", " swal('¡Bien hecho!', '¡Proyección agregada con éxito!', 'success');", true);
                    }
                    else
                    {
                        //AlertWarning.Visible = true;
                        //lblWarning.Text = " <strong>¡Cuidado!</strong> La proyección ya existe! </br> <strong>Detalles:</strong> No esta permitido agregar más de una proyeccion en el mismo contrato...";
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "click", " swal('¡Cuidado!', '¡La proyección ya existe!, solo se puede agregar una proyección por contrato...', 'warning');", true);
                    }

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }

        protected void LbtnEliminarProyeccion_Click(object sender, EventArgs e)
        {
            LbtnContratoE.Visible = false;
            LbtnReporteE.Visible = false;
            LbtnEliminarProyeccion.Visible = false;
            LblEliminarContrato.Visible = true;
            DDlEleminarResidencia.Visible = true;
            LblEliminarReporte.Visible = false;
            DDlEliminarReporte.Visible = false;
            LbtnConfirmarP.Visible = true;
            LblTitlePnlEliminar.Visible = true;
            LblTitlePnlEliminar.Text = "Eliminar Proyección";
            CargarDDLEliminar(); //Metodo que cargará los datos en las listas despegables Eliminar Reporte y Eliminar Proyección
        }

        protected void LbtnConfirmarP_Click(object sender, EventArgs e)
        {
            LblTitlePnlEliminar.Visible = false;
            PnlEliminar.Visible = false;
            LblEliminarContrato.Visible = false;
            DDlEleminarResidencia.Visible = false;
            LbtnConfirmarP.Visible = false;
            //habilitar botones y cajas de texto
            LbtnCancelEditionC.Enabled = true;
            LbtnNuevo.Enabled = true;
            LbtnEliminar.Enabled = true;
            LbtnActualizar.Enabled = true;
            string namecontrato = DDlEleminarResidencia.SelectedItem.ToString();
            Int64 IdContrato = ObtenerIdResidencia(namecontrato);
            if (ExisteProyeccion(IdContrato)) //Pregunto si existe Proyección con el Id del contrato, para poder eliminarlo, si no existe entonces no tiene caso hacer nada
            {
                BorrarProyeccion(IdContrato);
                //AlertSuccess.Visible = true;
                //lblSucces.Text = " ¡<strong>Proyección eliminada</strong> con éxito!";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "click", " swal('¡Bien hecho!', 'Proyección borrada con éxito!', 'success');", true);
            }
            else
            {
                //AlertWarning.Visible = true;
                //lblWarning.Text = "¡<strong>No existe proyección</strong> en el contrato seleccionado!";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "click", " swal('¡No existe proyección!', 'Para ejecutar esta acción, primero agregue una proyección...', 'warning');", true);
                LbtnConfirmarP.Enabled = false; //Deshabilito el botón Borrar Proyección
            }
            
        }
        protected void BorrarProyeccion(Int64 IdContrato) //Metodo que se encarga de Borrar la proyección de la BD
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String BorrarProyeccion = "Delete From ProyeccionPorContrato Where  IdContrato = @idcontrato";
                    OrdenSqlProy = new SqlCommand(BorrarProyeccion, con);
                    con.Open();
                    OrdenSqlProy.Parameters.AddWithValue("@idcontrato", IdContrato);
                    OrdenSqlProy.ExecuteNonQuery();
                    con.Close();
                   
                }
            }
            catch (Exception ex)
            {
                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }

        protected Boolean ExisteProyeccion(Int64 IdContrato) //Metodo que devuelve True si existe ya una proyección en un Contrato especifico
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String ExisteProyeccion = "Select 'true' From ProyeccionPorContrato Where  IdContrato = @idcontrato";
                    OrdenSqlProy = new SqlCommand(ExisteProyeccion, con);
                    con.Open();
                    OrdenSqlProy.Parameters.AddWithValue("@idcontrato", IdContrato);
                    ExisteReporte = Convert.ToBoolean(OrdenSqlProy.ExecuteScalar());
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
            return ExisteReporte;
        }

        protected void LbtnAProyeccion_Click(object sender, EventArgs e)
        {
            LblTitlePnlActualizar.Text = "Actualizar Proyección";
            LbtnAProyeccion.Visible = false;
            LbtnACambiosP.Visible = true;
            LbtnAConceptos.Visible = false;
            LblContratoAP.Visible = true;
            DDlContratoAP.Visible = true;
            LblMesAP.Visible = true;
            DDlMesAP.Visible = true;
            if (DDlContratoAP.SelectedIndex != -1)
            {
                string contrato = DDlContratoAP.SelectedItem.ToString();
                Int64 IdR = Convert.ToInt64(ObtenerIdResidencia(contrato));
                Boolean ExistenReportes = VerificarSiExistenReportes(IdR);
                if (ExistenReportes)
                {
                    CargarFechasDeReportes(IdR); //Aqui Mando a llamar al metodo para que muestre los reportes por fechas en la lista despegable de eliminar reporte
                    LbtnACambiosP.Enabled = true;
                }
                else if (DDlMesAP.SelectedValue.ToString().CompareTo("Ninguno") != 0)
                {
                    DDlMesAP.Items.Clear();
                    DDlMesAP.Items.Add("Ninguno");
                    LbtnACambiosP.Enabled = false;

                }
            }
            else if (DDlContratoAP.ToString().CompareTo("Ninguno") != 0 && DDlMesAP.SelectedValue.ToString().CompareTo("Ninguno") != 0)
            {
                DDlContratoAP.Items.Clear();
                DDlContratoAP.Items.Add("Ninguno");
                DDlMesAP.Items.Add("Ninguno");
                LbtnACambiosP.Enabled = false;

            }
        }

        protected void LbtnACambiosP_Click(object sender, EventArgs e)
        {
            PnlActualizar.Visible = false;
            LblTitlePnlActualizar.Visible = false;
            LbtnACambiosP.Visible = false;
            LblContratoAP.Visible = false;
            DDlContratoAP.Visible = false;
            LblMesAP.Visible = false;
            DDlMesAP.Visible = false;
            //habilitar botones y cajas de texto
            LbtnCancelEditionC.Enabled = true;
            LbtnNuevo.Enabled = true;
            LbtnEliminar.Enabled = true;
            LbtnActualizar.Enabled = true;
            string fechadereporte =DDlMesAP.SelectedItem.ToString(); //Seleccionó una fecha de la liste despegable
            Int64 idreporte;
            int ncaracteres = fechadereporte.Length;
            string mes = fechadereporte.Substring(0, (ncaracteres - 5));
            string anio = fechadereporte.Remove(0, (ncaracteres - 4));
            string namecontrato = DDlContratoAP.SelectedItem.ToString();  //Selecciona un contrato de la lista despegable de Contratos
            string mesProyeccion = mes + "(" + anio + ")";
            Int64 IdContrato = ObtenerIdResidencia(namecontrato); //Obtenemos el Id de la Residencia o Contrato
            if (mes.CompareTo("Enero") == 0)
            {
                mesyanio = anio + "-" + "01%";
            }
            else if (mes.CompareTo("Febrero") == 0)
            {
                mesyanio = anio + "-" + "02%";
            }
            else if (mes.CompareTo("Marzo") == 0)
            {
                mesyanio = anio + "-" + "03%";
            }
            else if (mes.CompareTo("Abril") == 0)
            {
                mesyanio = anio + "-" + "04%";
            }
            else if (mes.CompareTo("Mayo") == 0)
            {
                mesyanio = anio + "-" + "05%";
            }
            else if (mes.CompareTo("Junio") == 0)
            {
                mesyanio = anio + "-" + "06%";
            }
            else if (mes.CompareTo("Julio") == 0)
            {
                mesyanio = anio + "-" + "07%";
            }
            else if (mes.CompareTo("Agosto") == 0)
            {
                mesyanio = anio + "-" + "08%";
            }
            else if (mes.CompareTo("Septiembre") == 0)
            {
                mesyanio = anio + "-" + "09%";
            }
            else if (mes.CompareTo("Octubre") == 0)
            {
                mesyanio = anio + "-" + "10%";
            }
            else if (mes.CompareTo("Noviembre") == 0)
            {
                mesyanio = anio + "-" + "11%";
            }
            else if (mes.CompareTo("Diciembre") == 0)
            {
                mesyanio = anio + "-" + "12%";
            }
            if (ExisteProyeccion(IdContrato))
            {
                try
                {
                    con = new SqlConnection(strConexion);
                    using (con)
                    {
                        String ObtenerIdReportePorfecha = "SELECT IdReporte FROM Reportes WHERE IdResidencia =@idcontrato AND FechaDeReporte Like @mesyanio"; //Consulta para obtener el Id de un reporte por fecha de reporte
                        OrdenSql = new SqlCommand(ObtenerIdReportePorfecha, con);
                        con.Open();
                        OrdenSql.Parameters.AddWithValue("@mesyanio", mesyanio);
                        OrdenSql.Parameters.AddWithValue("@idcontrato", IdContrato);
                        idreporte = Convert.ToInt64(OrdenSql.ExecuteScalar());              //Me devuelve el Id del reporte
                        ActualizarProyeccionEnSql(idreporte, IdContrato, mesProyeccion); //Le paso los datos a este metodo para actualizar la proyeccion en la BD de la Tabla ProyeccionPorContrato
                        con.Close();

                    }

                }
                catch (Exception ex)
                {

                    AlertDanger.Visible = true;
                    lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
                }
            }
            else
            {
                //AlertWarning.Visible = true;
                //lblWarning.Text = "¡<strong>No existe proyección</strong> en el contrato seleccionado!";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "click", " swal('¡No existe proyección!', 'Para ejecutar esta acción, primero agregue una proyección...', 'warning');", true);
                LbtnACambiosP.Enabled = false; //Deshabilito el botón Aplicar Actualizar Proyección
            }
           
            
        }
        protected void ActualizarProyeccionEnSql(Int64 IdReporte,Int64 IdContrato,string MesP)
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String InsertarProyeccion = "UPDATE ProyeccionPorContrato SET IdReportes = @idreporte, MesProyeccion = @mesp WHERE IdContrato =  @idcontrato";
                    OrdenSql = new SqlCommand(InsertarProyeccion, con);
                    con.Open();
                        OrdenSql.Parameters.AddWithValue("@idreporte", IdReporte);
                        OrdenSql.Parameters.AddWithValue("@idcontrato", IdContrato);
                        OrdenSql.Parameters.AddWithValue("@mesp", MesP);
                        OrdenSql.ExecuteNonQuery();
                    //AlertSuccess.Visible = true;
                    //lblSucces.Text = " ¡<strong>Proyección actualizada</strong> con éxito!";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "click", " swal('¡Bien hecho!', '¡Proyección actualizada correctamente!', 'success');", true);
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }

        protected void DDlContratoAP_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DDlContratoAP.Visible == true)
            {
                string contrato = DDlContratoAP.SelectedItem.ToString();
                Int64 IdContrato = Convert.ToInt64(ObtenerIdResidencia(contrato));
                Boolean ExisteReportEnContrato = ExisteRepEnContrato(IdContrato);
                if (ExisteReportEnContrato)
                {
                    CargarFechasDeReportes(IdContrato); //Aqui Mando a llamar al metodo para que muestre los reportes por fechas en la lista despegable de MesProyección
                    LbtnACambiosP.Enabled = true;
                }
                else if (DDlMesAP.SelectedValue.ToString().CompareTo("Ninguno") != 0)
                {
                    DDlMesAP.Items.Clear();
                    DDlMesAP.Items.Add("Ninguno");
                    LbtnACambiosP.Enabled = false;
                }

            }
        }

        protected void DdlContratosProyeccion_SelectedIndexChanged(object sender, EventArgs e) //Lista despegable de los contratos
        {
            if (DdlContratosProyeccion.Visible == true)
            {
                string contrato = DdlContratosProyeccion.SelectedItem.ToString();
                Int64 IdContrato = Convert.ToInt64(ObtenerIdResidencia(contrato));
                Boolean ExisteReportEnContrato = ExisteRepEnContrato(IdContrato);
                if (ExisteReportEnContrato)
                {
                    CargarFechasDeReportes(IdContrato); //Aqui Mando a llamar al metodo para que muestre los reportes por fechas en la lista despegable de MesProyección
                    LbtnGuardarP.Enabled = true;
                }
                else if (DdlMesProyecccion.SelectedValue.ToString().CompareTo("Ninguno") != 0)
                {
                    DdlMesProyecccion.Items.Clear();
                    DdlMesProyecccion.Items.Add("Ninguno");
                    LbtnGuardarP.Enabled = false;
                }

            }
        }

        protected void LbtnACambiosR_Click(object sender, EventArgs e)
        {
            PnlActualizar.Visible = false;
            LblTitlePnlActualizar.Visible = false;
            //habilitar botones y cajas de texto
            LbtnCancelEditionC.Enabled = true;
            LbtnNuevo.Enabled = true;
            LbtnEliminar.Enabled = true;
            LbtnActualizar.Enabled = true;
        }

        protected void LbtnACambiosCptos_Click(object sender, EventArgs e)
        {
            PnlActualizar.Visible = false;
            LblTitlePnlActualizar.Visible = false;
            LbtnACambiosCptos.Visible = false;
            DdlContratoA.Visible = false;
            DdlConceptosA.Visible = false;
            TxtConceptoA.Visible = false;
            TxtObservacionA.Visible = false;
            LblAConcepto.Visible = false;
            LblAContrato.Visible = false;
            LblAContrato2.Visible = false;
            LblAObservacion.Visible = false;
            //habilitar botones y cajas de texto
            LbtnCancelEditionC.Enabled = true;
            LbtnNuevo.Enabled = true;
            LbtnEliminar.Enabled = true;
            LbtnActualizar.Enabled = true;
            string contrato = DdlContratoA.SelectedItem.ToString();
            string concepto = DdlConceptosA.SelectedItem.ToString();
            Int64 IdR = Convert.ToInt64(ObtenerIdResidencia(contrato));
            Int64 IdC = ObtenerIdConcepto(concepto,IdR);
            ActualizarConceptos(IdC);
            ActualizarObservacion(IdC);
            //AlertSuccess.Visible = true;
            //lblSucces.Text = "¡<strong>Actualización</strong> aplicada correctamente!";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "click", " swal('¡Bien hecho!', '¡Concepto actualizado correctamente!', 'success');", true);
        }
        protected void ActualizarConceptos(Int64 IdC) //Metodo para actualizar el concepto seleccionado, recibe como parametro el Id del concepto
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {

                    String updateconcepto = "UPDATE  Concepto SET Concepto = @concepto where IdConcepto = @idconcepto"; // Actualizo el Concepto  de la Tabla Concepto solo del Id del contrato seleccionado
                    OrdenSql = new SqlCommand(updateconcepto, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@concepto", TxtConceptoA.Text);
                    OrdenSql.Parameters.AddWithValue("@idconcepto", IdC);
                    OrdenSql.ExecuteNonQuery();
                    con.Close();

                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }
        protected void ActualizarObservacion(Int64 IdC)  //Metodo para actualizar la observacion del concepto seleccionado
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {

                    String updateObservacion = "UPDATE  DatosDeReporte SET Observacion = @observacion where IdConcepto = @idconcepto"; // Actualizo el Concepto  de la Tabla Concepto solo del Id del contrato seleccionado
                    OrdenSql = new SqlCommand(updateObservacion, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@observacion", TxtObservacionA.Text);
                    OrdenSql.Parameters.AddWithValue("@idconcepto", IdC);
                    OrdenSql.ExecuteNonQuery();
                    con.Close();

                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }

        protected void LbtnBuscar_Click(object sender, EventArgs e) //Metodo que acciona el evento de buscar un resumen de los totales del contrato seleccionado
        {
            CargarContratosArea();
        }
        protected void CargarContratosArea() //Metodo para cargar la Tabla de rentabilidad de los contratos
        {
            string Contrato, MesInicial, MesFinal;
            Int64 IdContrato;
            Boolean ExisteContrato = VerificarSiExistenContratos();
            if (ExisteContrato)
            {
                Contrato = DdlContratoMG.SelectedItem.ToString(); //Obtenemos el nombre del Contrato seleccionado en la lista despegable del Menu General
                IdContrato = ObtenerIdResidencia(Contrato); //Obtenemos el Id del Contrato
               if (ExisteRepEnContrato(IdContrato))
                {
                    MesInicial = DdlMesInicial.SelectedItem.ToString(); //Obtenemos el Mes Inicial (Mes-Año) seleccionado
                    MesFinal = DdlMesFinal.SelectedItem.ToString(); //Obtenemos el Mes Final (Mes-Año) Seleccionado
                    ConstruirTablaGlobalArea(IdContrato, MesInicial, MesFinal);
                    
                   
                }
                else
                {
                    PnlVacio.Visible = true;
                    LblTitleRentabilidad.Visible = false;
                    LblTitleResumenC.Visible = false;
                    GVContratos.DataSource = ""; //Limpio el GridView de la Tabla Contratos, porque no hay al menos un Reporte Agregado
                    GVContratos.DataBind();
                }

            }
            else
            {
                PnlVacio.Visible = true;
                LblTitleRentabilidad.Visible = false;
                LblTitleResumenC.Visible = false;
            }
        }

        protected void DdlContratoA_SelectedIndexChanged(object sender, EventArgs e)
        {
           
            
                string contrato = DdlContratoA.SelectedItem.ToString();
                Int64 IdContrato = Convert.ToInt64(ObtenerIdResidencia(contrato));
                Boolean ExistenConceptosEnContrato = VerificarSiExistenConceptos(IdContrato);
                if (ExistenConceptosEnContrato)
                {
                     CargarConceptosA(IdContrato);
                }
                else if (DdlConceptosA.SelectedValue.ToString().CompareTo("Ninguno") != 0)
                {
                    DdlConceptosA.Items.Clear();
                    DdlConceptosA.Items.Add("Ninguno");
                    TxtConceptoA.Text = "";
                    TxtObservacionA.Text = "";
                    LbtnACambiosCptos.Enabled = false;
                }

           
        }

        protected void LbtnGraficaArea_Click(object sender, EventArgs e)
        {
            string contrato;
            MenuGeneral.Visible = false;
            PnlGraficaC.Visible = true;
            LblTitleG.Visible = true;
            LbtnCerrarGraficaG.Visible = true;
            LbtnCerrarGraficaC.Visible = false;
            if (DdlContratoMG.SelectedIndex != -1)
            {
                contrato = DdlContratos.SelectedItem.ToString();
                LblTitleG.Text = "Avance Presupuestal " + "<strong>" + contrato + "</strong>";
                MostrarGraficaA = true;
                CargarContratosArea();
            }
        }

        protected void LbtnCerrarGraficaG_Click(object sender, EventArgs e)
        {
            MenuGeneral.Visible = true;
            PnlTable.Visible = true;
            LblTitleRentabilidad.Visible = true;
            LblTitleResumenC.Visible = true;
            PnlGraficaC.Visible = false;
            LblTitleG.Visible = false;
            LbtnCerrarGraficaG.Visible = false;
        }

        protected void LbtnAnual_Click(object sender, EventArgs e) //Evento del botón Tabla Anual
        {
            controlesAnual(); //Metodo para ocultar y mostrar los controles
            if (DdlAnual.Visible == true)
            {
              
                if (ConsultarExistenReportes(ObtenerIdArea())) //Pregunto si existen Reportes agregados en el Area
                {
                    CargarFechasIncialFinalAnual(ObtenerIdArea());  //Cargar Listas depegables Mes Inicial y Mes Final de todo el Área
                    DdlMesFAnual.SelectedValue = UltimoMesAgregado(ObtenerIdArea());
                    CargarAnios(ObtenerIdArea()); //Con este metodo Cargo a la lista despegable los años sin repetirse de todos los reportes de un AREA
                    ConstruirTablaAnual(DdlMesIAnual.SelectedItem.ToString(), DdlMesFAnual.SelectedItem.ToString(), DdlAnual.SelectedItem.ToString());
                    LbtnBuscarAnual.Enabled = true;
                    PnlTableAnual.Visible = true;
                    PnlVacio.Visible = false;
                }
                else if (DdlAnual.SelectedValue.ToString().CompareTo("Ninguno") != 0 && DdlMesIAnual.SelectedValue.ToString().CompareTo("Ninguno") != 0 && DdlMesFAnual.SelectedValue.ToString().CompareTo("Ninguno") != 0)
                {
                   
                    DdlAnual.Items.Clear();
                    DdlMesIAnual.Items.Clear();
                    DdlMesFAnual.Items.Clear();
                    DdlAnual.Items.Add("Ninguno");
                    DdlMesIAnual.Items.Add("Ninguno");
                    DdlMesFAnual.Items.Add("Ninguno");
                    LbtnBuscarAnual.Enabled = false;
                    PnlTableAnual.Visible = false;
                    PnlVacio.Visible = true;
                }

            }
        }
        protected void controlesAnual() //Metodo para ocultar y mostrar los controles
        {
            //Oculto los controles al seleccionar el botón Anual
            LbtnBuscar.Visible = false;
            DdlContratoMG.Visible = false;
            lblDe.Visible = false;
            DdlMesInicial.Visible = false;
            lblA.Visible = false;
            DdlMesFinal.Visible = false;
            LbtnGraficaArea.Visible = false;
            LbtnAnual.Visible = false;
            LbtnContratos.Visible = false;
            LblTitleRentabilidad.Visible = false;
            PnlTable.Visible = false;
            TitleS.Visible = false;
            //Mostrar los controles deseables
            LbtnBuscarAnual.Visible = true;
            LblDeAnual.Visible = true;
            DdlMesIAnual.Visible = true;
            LblaAnual.Visible = true;
            DdlMesFAnual.Visible = true;
            DdlAnual.Visible = true;
            LblDdlAnual.Visible = true;
            TitleCAnual.Visible = true;
          
        }
        protected Boolean ConsultarExistenReportes(int IdArea) //Metodo que se encarga de verificar si exiten Reportes agregados en el Area, recibe como parametro el IdArea
        {

            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string ConsultarReportes = "Select DISTINCT 'true' From Reportes R INNER JOIN Residencias C ON R.IdResidencia =  C.IdResidencia AND C.IdArea = @idarea";
                    OrdenSql = new SqlCommand(ConsultarReportes, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idarea", IdArea);
                    ExistenReportes = Convert.ToBoolean(OrdenSql.ExecuteScalar());
                    con.Close();
                }

            }
            catch (Exception ex)
            {
                AlertDanger.Visible = true;
                lblDanger.Text = "<strong><strong>Detalles de Error:</strong> " + ex.Message;
            }
            return ExistenReportes;
        }
        protected string UltimoMesAgregado(int IdArea) //Metodo que obtiene el Ultimo mes agregado en todos los reportes que exiten en todas las areas
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String ObtenerMes = ";WITH meses AS (SELECT datename (month,R.FechaDeReporte) AS NameMes, MIN(R.FechaDeReporte) AS Mes FROM Reportes  R INNER JOIN Residencias C ON R.IdResidencia =  C.IdResidencia AND C.IdArea = @idarea GROUP BY datename (month,R.FechaDeReporte)) SELECT NameMes FROM meses ORDER BY MONTH(Mes) DESC";
                    OrdenSqlMesAgregado = new SqlCommand(ObtenerMes, con);
                    con.Open();
                    OrdenSqlMesAgregado.Parameters.AddWithValue("@idarea", IdArea);
                    UltimaFechaReporte = Convert.ToString(OrdenSqlMesAgregado.ExecuteScalar());
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
            return UltimaFechaReporte;
        }
        protected void CargarFechasIncialFinalAnual(int IdArea)
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String ObtenerMes = ";WITH meses AS (SELECT datename (month,R.FechaDeReporte) AS NameMes, MIN(R.FechaDeReporte) AS Mes FROM Reportes  R INNER JOIN Residencias C ON R.IdResidencia =  C.IdResidencia AND C.IdArea = @idarea GROUP BY datename (month,R.FechaDeReporte)) SELECT NameMes FROM meses ORDER BY MONTH(Mes)";
                    OrdenSql = new SqlCommand(ObtenerMes, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idarea", IdArea);
                    DdlMesIAnual.DataSource = OrdenSql.ExecuteReader();
                    DdlMesIAnual.DataTextField = "NameMes";
                    DdlMesIAnual.DataBind();
                    con.Close();
                    con.Open();
                    DdlMesFAnual.DataSource = OrdenSql.ExecuteReader();
                    DdlMesFAnual.DataTextField = "NameMes";
                    DdlMesFAnual.DataBind();
                    con.Close();
                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }
        protected void CargarAnios(int IdArea) //Metodo que se encarga de cargar los años en la lista despegable de la Tabla Consolidado Anual, Estos años se cargan de acuerdo a los reportes agregados por Area
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String ObtenerAnios = "Select DISTINCT (DATEPART(yy, FechaDeReporte)) as Año From Reportes as R INNER JOIN Residencias as C ON R.IdResidencia = C.IdResidencia AND C.IdArea = @idarea";
                    OrdenSql = new SqlCommand(ObtenerAnios, con);
                    OrdenSql.Parameters.AddWithValue("@idarea", IdArea);
                    con.Open();
                    DdlAnual.DataSource = OrdenSql.ExecuteReader();
                    DdlAnual.DataTextField = "Año";
                    DdlAnual.DataBind();
                    con.Close();
                   
                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }

        protected void LbtnCerrarAnual_Click(object sender, EventArgs e) //Evento del Botón Cerrar el panel de la Tabla Anual
        {
            //Mostrar los controles al seleccionar el botón Anual
            LbtnBuscar.Visible = true;
            DdlContratoMG.Visible = true;
            lblDe.Visible = true;
            DdlMesInicial.Visible = true;
            lblA.Visible = true;
            DdlMesFinal.Visible = true;
            LbtnGraficaArea.Visible = true;
            LbtnAnual.Visible = true;
            LbtnContratos.Visible = true;
            LblTitleRentabilidad.Visible = true;
            PnlTable.Visible = true;
            TitleS.Visible = true;
            //Ocultar los controles desables
            TitleCAnual.Visible = false;
            LbtnBuscarAnual.Visible = false;
            LblDeAnual.Visible = false;
            DdlMesIAnual.Visible = false;
            LblaAnual.Visible = false;
            DdlMesFAnual.Visible = false;
            DdlAnual.Visible = false;
            LblDdlAnual.Visible = false;
            PnlTableAnual.Visible = false;
            
        }

        protected void LbtnBuscarAnual_Click(object sender, EventArgs e)  //Evento del botón Buscar, y obtener los datos en la Tabla Anual del Área
        {
          
            ConstruirTablaAnual(DdlMesIAnual.SelectedItem.ToString(), DdlMesFAnual.SelectedItem.ToString(), DdlAnual.SelectedItem.ToString());
        }

        protected void DdlConceptosA_SelectedIndexChanged(object sender, EventArgs e)
        {
            string contrato = DdlContratoA.SelectedItem.ToString();
            string concepto = DdlConceptosA.SelectedItem.ToString();
            Int64 IdR = Convert.ToInt64(ObtenerIdResidencia(contrato));
            Int64 IdC = ObtenerIdConcepto(concepto, IdR);
            MostrarConcepto(IdR,IdC);
            MostrarObservacion(IdC);
        }

        protected void DdlContratoMG_SelectedIndexChanged(object sender, EventArgs e) //Evento de la Lista Despegable Contratos del Menu General, Esta acción llena las listas despegables de FechaInicial y FehaFinal
        {
            CargarDDlFechasInicialFinal(); //Metodo para cargar las listas despegables
        }
        protected void CargarDDlFechasInicialFinal() //Metodo para cargar las listas despegables
        {
            
            if (DdlContratoMG.Visible == true )
            {
                string contrato = DdlContratoMG.SelectedItem.ToString();
                Int64 IdContrato = Convert.ToInt64(ObtenerIdResidencia(contrato));
                Boolean ExisteReportEnContrato = ExisteRepEnContrato(IdContrato);
                if (ExisteReportEnContrato)
                {
                    CargarFechasIncialFinal(IdContrato); //Aqui Mando a llamar al metodo para que muestre las fechas de los reportes mensuales en la listas despegables de Fecha Inicial y Fecha Final
                    LbtnBuscar.Enabled = true;
                    LbtnGraficaArea.Enabled = true;
                    LbtnAnual.Enabled = true;
                    DdlMesFinal.SelectedValue = UltimaFechaDeReporte(IdContrato);
                    LblTitleRentabilidad.Visible = true;
                    LblTitleResumenC.Visible = true;
                }
                else if (DdlMesInicial.SelectedValue.ToString().CompareTo("Ninguno") != 0 && DdlMesFinal.SelectedValue.ToString().CompareTo("Ninguno") != 0)
                {
                    DdlMesInicial.Items.Clear();
                    DdlMesFinal.Items.Clear();
                    DdlMesFinal.Items.Add("Ninguno");
                    DdlMesInicial.Items.Add("Ninguno");
                    LbtnBuscar.Enabled = false;
                    LbtnGraficaArea.Enabled = false;
                    LbtnAnual.Enabled = false;
                }

            }
           
        }
        protected void CargarFechasIncialFinal(Int64 IdContrato) //Metodo para cargar las fechas de los reportes en las listas depegables Fecha Inicial y Fecha Final
        {
            string fecha, nmes, anio, Mes_Anio;
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String ObtenerResidencias = "Select FechaDeReporte From Reportes Where IdResidencia = @idcontrato Order BY FechaDeReporte";
                    OrdenSql = new SqlCommand(ObtenerResidencias, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idcontrato", IdContrato);
                    Leer = OrdenSql.ExecuteReader();
                    DdlMesInicial.Items.Clear(); //Limpio las listar despegables, para que no se vaya agregando los datos de la consulta
                    DdlMesFinal.Items.Clear();
                    while (Leer.Read())    //Aquí leo todas las fechas de los reportes agregados en la Tabla Reportes del contrato seleccionado
                    {
                        fecha = Leer[0].ToString();
                        DateTime Fecha = DateTime.Parse(fecha);
                        DateTime fechames = DateTime.Parse(fecha);          //Se crea un objeto de tipo DateTime para Mes
                        DateTime fechaanio = DateTime.Parse(fecha);         //Se crea un objeto de tipo DateTime para Año
                        nmes = Convert.ToString(fechames.Month);             //Obtengo solo el Mes de la fecha completa
                        anio = Convert.ToString(fechaanio.Year);            //Obtengo solo el Año de la fecha completa
                        Mes_Anio = DevuelveMes_Anio(nmes, anio);             //Metodo que devuelve El Mes-Anio, la Fecha del Reporte Leido
                        DdlMesInicial.Items.Add(Mes_Anio);
                        DdlMesFinal.Items.Add(Mes_Anio);

                    }
                    con.Close();
                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }
        protected string DevuelveMes_Anio(string nmes, string anio)          //Metodo que devuelve el Mes-Anio, la fecha del reporte leido
        {
            if (nmes == "1")
            {
                FechaIReporte = "Enero-" + anio;
            }
            else if (nmes == "2")
            {
                FechaIReporte = "Febrero-" + anio;
            }
            else if (nmes == "3")
            {
                FechaIReporte = "Marzo-" + anio;
            }
            else if (nmes == "4")
            {
                FechaIReporte = "Abril-" + anio;
            }
            else if (nmes == "5")
            {
                FechaIReporte = "Mayo-" + anio;
            }
            else if (nmes == "6")
            {
                FechaIReporte = "Junio-" + anio;
            }
            else if (nmes == "7")
            {
                FechaIReporte = "Julio-" + anio;
            }
            else if (nmes == "8")
            {
                FechaIReporte = "Agosto-" + anio;
            }
            else if (nmes == "9")
            {
                FechaIReporte = "Septiembre-" + anio;
            }
            else if (nmes == "10")
            {
                FechaIReporte = "Octubre-" + anio;
            }
            else if (nmes == "11")
            {
                FechaIReporte = "Noviembre-" + anio;
            }
            else
            {
                FechaIReporte = "Diciembre-" + anio;
            }
            return FechaIReporte;
        }
        protected void MostrarConcepto(Int64 IdR,Int64 IdC)
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string ConsultarConcepto = "Select Concepto From Concepto Where IdResidencia = @idresidencia AND IdConcepto = @idconcepto";
                    OrdenSql = new SqlCommand(ConsultarConcepto, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idresidencia", IdR);
                    OrdenSql.Parameters.AddWithValue("@idconcepto", IdC);
                    TxtConceptoA.Text = Convert.ToString(OrdenSql.ExecuteScalar());
                    con.Close();
                }

            }
            catch (Exception ex)
            {
                AlertDanger.Visible = true;
                lblDanger.Text = "<strong><strong>Detalles de Error:</strong> " + ex.Message;
            }
        }
        protected void MostrarObservacion(Int64 IdC)
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string ConsultarObservacion = "Select Observacion From DatosDeReporte WHERE IdConcepto = @idconcepto";
                    OrdenSql = new SqlCommand(ConsultarObservacion, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idconcepto", IdC);
                    TxtObservacionA.Text = Convert.ToString(OrdenSql.ExecuteScalar());
                    con.Close();
                }

            }
            catch (Exception ex)
            {
                AlertDanger.Visible = true;
                lblDanger.Text = "<strong><strong>Detalles de Error:</strong> " + ex.Message;
            }
        }
        protected Boolean VerificarSiExistenConceptos(Int64 IdR)  //Metodo que verifica si existen conceptos en el contrato seleccionado, recibe como paramtro el Id del contrato
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string ConsultarConceptos = "Select 'true' From Concepto Where IdResidencia = @idresidencia";
                    OrdenSql = new SqlCommand(ConsultarConceptos, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idresidencia",IdR);
                    ExistenConceptos = Convert.ToBoolean(OrdenSql.ExecuteScalar());
                    con.Close();
                }

            }
            catch (Exception ex)
            {
                AlertDanger.Visible = true;
                lblDanger.Text = "<strong><strong>Detalles de Error:</strong> " + ex.Message;
            }
            return ExistenConceptos;
        }

        protected void LbtnCambiarPorcentaje_Click(object sender, EventArgs e)
        {
            PnlCPorcentaje.Visible = true;
            TxtPorcentajeIC.Text = "";
            TxtPorcentajeIO.Text = "";
            //Inhabilitar botones y cajas de texto
            LbtnBuscarC.Enabled = false;
            DdlContratos.Enabled = false;
            DdlMesC.Enabled = false;
            DdlAnioC.Enabled = false;
            LbtnRegresarGeneral.Enabled = false;
            LbtnGraficaC.Enabled = false;
            LbtnEditarC.Visible = false;
            LbtnCambiarPorcentaje.Visible = false;
            CargarContratos();
        }

        protected void LbtnActualizarP_Click(object sender, EventArgs e) //Evento del Control Aplicar para Cambiar el porcentaje de la Tabla de los Reportes 
        {
            if(TxtPorcentajeIC.Text != "" && TxtPorcentajeIO.Text != "")
            {
                PnlCPorcentaje.Visible = false;
                CambiarPorcentaje = true;
                CargarContratos();
                //habilitar botones y cajas de texto
                LbtnBuscarC.Enabled = true;
                DdlContratos.Enabled = true;
                DdlMesC.Enabled = true;
                DdlAnioC.Enabled = true;
                LbtnRegresarGeneral.Enabled = true;
                LbtnGraficaC.Enabled = true;
                LbtnEditarC.Visible = true;
                LbtnCambiarPorcentaje.Visible = true;
                AlertSuccess.Visible = true;
                lblSucces.Text = " ¡Los <strong>porcentajes</strong> se han actualizado correctamente!";
            }
            else
            {
                if(TxtPorcentajeIC.Text == "")
                {
                    TxtPorcentajeIC.Focus();
                }
                else
                {
                    TxtPorcentajeIO.Focus();
                }
                AlertWarning.Visible = true;
                lblWarning.Text = "<strong>¡Importante!</strong> ¡Debe llenar todos los campos!";
            }
                
        }

        protected void LbtnCancelarPnlP_Click(object sender, EventArgs e)
        {
            PnlCPorcentaje.Visible = false;
            //habilitar botones y cajas de texto
            LbtnBuscarC.Enabled = true;
            DdlContratos.Enabled = true;
            DdlMesC.Enabled = true;
            DdlAnioC.Enabled = true;
            LbtnRegresarGeneral.Enabled = true;
            LbtnGraficaC.Enabled = true;
            LbtnEditarC.Visible = true;
            LbtnCambiarPorcentaje.Visible = true;
            CargarContratos(); 
        }

        protected Int64 ObtenerIdConcepto(String concepto,Int64 IdR) //METODO que recibe como parametro el nombre del concepto y  el Id de contrato, devuelve el Id de un concepto
        {

           
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String ObtenerResidencias = "Select IdConcepto From Concepto Where Concepto = @concepto AND IdResidencia = @idresidencia";
                    OrdenSql = new SqlCommand(ObtenerResidencias, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@concepto", concepto);
                    OrdenSql.Parameters.AddWithValue("@idresidencia", IdR);
                    IdConceptoA = Convert.ToInt64(OrdenSql.ExecuteScalar());
                    con.Close();

                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
            return IdConceptoA;
        }
        protected void LbtnCancelar_Click(object sender, EventArgs e)
        {
            PnlVisualizar.Visible = false;
            PnlNuevo.Visible = false;
            LbtnVisualizar.Visible = false;
            //Ocultar botones y cajas de texto para el boton Agregar contrato
            LbtnReporteMensualN.Visible = true;
            LbtnAgregarContrato.Visible = true;
            LblFechaInicialNC.Visible = false;
            TxtFechaInicialNC.Visible = false;
            LblFechaTerminoNC.Visible = false;
            TxtFechaTerminoNC.Visible = false;
            SignoMoneda.Visible = false;
            TxtMontoC.Visible = false;
            TxtNombreResidencia.Visible = false;
            LbtnGuardarC.Visible = false;
            //Ocultar botones y cajas de texto para el boton Agregar reporte
            LblContratosN.Visible = false;
            DdlContratosN.Visible = false;
            LblFechaReporte.Visible = false;
            TxtFechaReporte.Visible = false;
            FUpExaminar.Visible = false;
            LbtnImportar.Visible = false;
            LbtnHelp.Visible = false;
            LblbtnFormatoExcel.Visible = false;
            LbtnGuardarR.Visible = false;
            //Ocultar botones y cajas de texto para el boton Agregar Proyección
            LbtnGuardarP.Visible = false;
            LblContratosProyeccion.Visible = false;
            DdlContratosProyeccion.Visible = false;
            LblMesProyeccion.Visible = false;
            DdlMesProyecccion.Visible = false;
            //habilitar botones y cajas de texto
            LbtnCancelEditionC.Enabled = true;
            LbtnNuevo.Enabled = true;
            LbtnEliminar.Enabled = true;
            LbtnActualizar.Enabled = true;
            LimpiarGridView();
            TxtFechaReporte.Text = "";
            LblTitlePnlAgregarN.Visible = false;
        }

        protected void LbtnVisualizar_Click(object sender, EventArgs e)
        {
            PnlVisualizar.Visible = true;
            LbtnOcultarN.Visible = true;
            LbtnVisualizar.Visible = false;
        }

        protected void LbtnEliminar_Click(object sender, EventArgs e)
        {
            PnlEliminar.Visible = true;
            LbtnContratoE.Visible = true;
            LbtnReporteE.Visible = true;
            LbtnEliminarProyeccion.Visible = true;
            LblTitlePnlEliminar.Visible = true;
            LblTitlePnlEliminar.Text = "Eliminar Historial";
            //Inhabilitar botones y cajas de texto
            LbtnCancelEditionC.Enabled = false;
            LbtnNuevo.Enabled = false;
            LbtnEliminar.Enabled = false;
            LbtnActualizar.Enabled = false;
        }

       
        protected void LbtnCancelarE_Click(object sender, EventArgs e)
        {
            PnlEliminar.Visible = false;
            LblTitlePnlEliminar.Visible = false;
            //habilitar botones y cajas de texto
            LbtnCancelEditionC.Enabled = true;
            LbtnNuevo.Enabled = true;
            LbtnEliminar.Enabled = true;
            LbtnActualizar.Enabled = true;
            //Ocultar botones y dropdownlist del evento Eliminar contrato
            DDlEleminarResidencia.Visible = false;
            LbtnConfirmarC.Visible = false;
            //Ocultar botones y dropdownlist del evento Eliminar reporte
            LblEliminarContrato.Visible = false;
            DDlEleminarResidencia.Visible = false;
            LblEliminarReporte.Visible = false;
            DDlEliminarReporte.Visible = false;
            LbtnConfirmarR.Visible = false;
            LbtnContratoE.Visible = true;
            LbtnReporteE.Visible = true;
            //Eliminar Proyección
            LbtnEliminarProyeccion.Visible = true;
            LbtnConfirmarP.Visible = false;
            
        }

        protected void LbtnOcultarN_Click(object sender, EventArgs e)
        {
            PnlVisualizar.Visible = false;
            LbtnVisualizar.Visible = true;
            LbtnOcultarN.Visible = false;

        }
        protected void LbtnActualizar_Click(object sender, EventArgs e)
        {
     
            PnlActualizar.Visible = true;
            LblTitlePnlActualizar.Visible = true;
            LblTitlePnlActualizar.Text = "Actualizar Historial";
            //LbtnAContratos.Visible = true;
            //LbtnAReportes.Visible = true;
            LbtnAConceptos.Visible = true;
            LbtnAProyeccion.Visible = true;
            //Inhabilitar botones y cajas de texto
            LbtnCancelEditionC.Enabled = false;
            LbtnNuevo.Enabled = false;
            LbtnEliminar.Enabled = false;
            LbtnActualizar.Enabled = false;
        }

      
        protected void LbtnCancelarA_Click(object sender, EventArgs e)
        {
           
            PnlActualizar.Visible = false;
            LblTitlePnlActualizar.Visible = false;
            LbtnAProyeccion.Visible = false;
            LbtnAConceptos.Visible = false;
            LbtnACambiosC.Visible = false;
            LbtnACambiosP.Visible = false;
            LbtnACambiosCptos.Visible = false;
            DdlContratoA.Visible = false;
            DdlConceptosA.Visible = false;
            LblContratoAP.Visible = false;
            DDlContratoAP.Visible = false;
            LblMesAP.Visible = false;
            DDlMesAP.Visible = false;
            TxtConceptoA.Visible = false;
            TxtObservacionA.Visible = false;
            LblAConcepto.Visible = false;
            LblAContrato.Visible = false;
            LblAContrato2.Visible = false;
            LblAObservacion.Visible = false;
            //habilitar botones y cajas de texto
            LbtnCancelEditionC.Enabled = true;
            LbtnNuevo.Enabled = true;
            LbtnEliminar.Enabled = true;
            LbtnActualizar.Enabled = true;
        }

        protected void DDlEleminarResidencia_SelectedIndexChanged(object sender, EventArgs e) //Evento de la lista despegable a donde se muestran los contratos,Si selecciono un contrato, se despliegan las fechas de los reportes de ese contrato
        {
            if(DDlEliminarReporte.Visible == true)
            {
                string contrato = DDlEleminarResidencia.SelectedItem.ToString();
                Int64 IdContrato = Convert.ToInt64(ObtenerIdResidencia(contrato));
                Boolean ExisteReportEnContrato = ExisteRepEnContrato(IdContrato);
                if (ExisteReportEnContrato)
                {
                  CargarFechasDeReportes(IdContrato); //Aqui Mando a llamar al metodo para que muestre los reportes por fechas en la lista despegable de eliminar reporte
                }
                else if (DDlEliminarReporte.SelectedValue.ToString().CompareTo("Ninguno") != 0)
                {
                    DDlEliminarReporte.Items.Clear();
                    DDlEliminarReporte.Items.Add("Ninguno");
                    LbtnConfirmarR.Enabled = false;
                }
               
            }
    
        }

        protected void LbtnGraficaC_Click(object sender, EventArgs e)
        {
            string residencia;
            LbtnRegresarGeneral.Visible = false;
            PnlTableC.Visible = false;
            myInput.Visible = false;
            PnlGraficaC.Visible = true;
            LblTitleG.Visible = true;
            LbtnCerrarGraficaC.Visible = true;
            LbtnGraficaC.Visible = false;
            PnlVacio.Visible = false;
            //LbtnEditarC.Enabled = false;
            //LbtnCambiarPorcentaje.Enabled = false;
            //Inhabilitar botones y cajas de texto
            LbtnBuscarC.Visible = false;
            DdlContratos.Visible = false;
            LbtnEditarC.Visible = false;
            LbtnCambiarPorcentaje.Visible = false;
            if (DdlContratos.SelectedIndex != -1) 
            {
                residencia = DdlContratos.SelectedItem.ToString();
                LblTitleG.Text = "Avance Presupuestal " + "<strong>"+residencia+"</strong>";
                MostrarGrafica = true;
                CargarContratos();
            }
            
        }
        protected void BorrarTodosReportes(Int64 IdR) //Metodo para borrar todos los reportes de la BD 
        {
            Int64 IdReporte;
            int Accion = 1;
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string ObtenerReporte = "Select IdReporte From Reportes Where IdResidencia = @idresidencia";
                    OrdenSql = new SqlCommand(ObtenerReporte, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idresidencia", IdR);
                    LeerIdReporte = OrdenSql.ExecuteReader();
                    while (LeerIdReporte.Read())
                    {
                        IdReporte = Convert.ToInt64(LeerIdReporte[0].ToString());
                        while (Accion <= 5)
                        {
                            BorrarReporteEnSQL(Accion, IdReporte); //Metodo que borra todo el reporte en la base de datos
                            ++Accion;
                        }
                        Accion = 1;
                    }
                    LeerIdReporte.Close();
                    con.Close();

                }
            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }
        protected void BorrarContratoEnSQL(int Accion,string namecontrato)  //Metodo que se encarga de Borrar el contrato en la base de datos
        {

            
            int IdResidencia = ObtenerIdResidencia(namecontrato);
           
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string NameStoredProcedure = "Elimina_Residencia";
                    OrdenSql = new SqlCommand(NameStoredProcedure, con);
                    con.Open();
                    OrdenSql.CommandType = CommandType.StoredProcedure;
                    OrdenSql.Parameters.AddWithValue("@idresidencia", IdResidencia);
                    OrdenSql.Parameters.AddWithValue("@Accion", Accion);
                    OrdenSql.ExecuteNonQuery();
                    con.Close();
                    
                }
            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }
          protected void LbtnConfirmarC_Click(object sender, EventArgs e)
        {

            IDarea = ObtenerIdArea();
            int Accion = 1;
            string accion = "Borrar";
            LblTitlePnlEliminar.Visible = false;
            PnlEliminar.Visible = false;
            LblEliminarContrato.Visible = false;
            DDlEleminarResidencia.Visible = false;
            LbtnConfirmarC.Visible = false;
            //habilitar botones y cajas de texto
            LbtnCancelEditionC.Enabled = true;
            LbtnNuevo.Enabled = true;
            LbtnEliminar.Enabled = true;
            LbtnActualizar.Enabled = true;
            string namecontrato = DDlEleminarResidencia.SelectedItem.ToString();  //Seleccionó el contrato de la lista despegable de la seccion Eliminar Contrato
            Int64 IdR = ObtenerIdResidencia(namecontrato);
            Boolean ExistenReportes = VerificarSiExistenReportes(IdR);
                if (ExisteProyeccion(IdR)) //Si existe proyección en el contrato especificado, entonces se borra primero la proyección
                {
                    BorrarProyeccion(IdR); //Metodo que se encarga de borrar la Proyección de la BD
                }
                if (ExistenReportes) //solo si hay reportes se ejecuta esta accion, de lo contrario SOLO se elimina el contrato
                {
                    BorrarTodosReportes(IdR);  //Mando a llamar el metodo que va a obtener todos los reportes de la BD de la Tabla REPORTES de acuerdo al IdR (Id de Residencia o contrato) y poder borrarlos
                }
                while (Accion <= 2)
                {
                    BorrarContratoEnSQL(Accion, namecontrato); //Mando a llamar al metodo BorrarContratoEnSQL para borrar el contrato de la BD
                    ++Accion;
                }
                TotalContratos(IDarea, accion); //Metodo que se encarga de contar el numero de contratos que hay por Area
                DDlEleminarResidencia.Items.Clear();
                DdlContratos.Items.Clear();
                DdlContratosN.Items.Clear();
                //AlertSuccess.Visible = true;
                //lblSucces.Text = "¡Contrato <strong>borrado</strong> con éxito! ";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "click", " swal('¡Bien hecho!', 'Contrato borrado con éxito!', 'success');", true);
  
        }

        protected void LbtnCerrarGraficaC_Click(object sender, EventArgs e)
        {
            LbtnRegresarGeneral.Visible = true;
            PnlTableC.Visible = true;
            PnlGraficaC.Visible = false;
            MostrarGrafica = false;    //Aqui cambia a false, para poder mostrar la tabla de contratos
            LblTitleG.Visible = false;
            LbtnGraficaC.Visible = true;
            //LbtnEditarC.Enabled = true;
            //LbtnCambiarPorcentaje.Enabled = true;
            //habilitar botones y cajas de texto
            //LbtnBuscarC.Enabled = true;
            //DdlContratos.Enabled = true;
            LbtnBuscarC.Visible = true;
            DdlContratos.Visible = true;
            if (nombreUsuario != "Invitado") //Pregunto si es es diferente de Invitado para poder mostrar los controles Editar Tabla y Editar Porcentaje
            {
                LbtnEditarC.Visible = true;
                LbtnCambiarPorcentaje.Visible = true;
            }
           
            // VerificarSiExistenContratos();
            CargarContratos();

        }

        protected void LbtnConfirmarR_Click(object sender, EventArgs e) //Metodo de Evento para Borrar un Reporte Mensual
        {
            LblTitlePnlEliminar.Visible = false;
            PnlEliminar.Visible = false;
            LblEliminarContrato.Visible = false;
            DDlEleminarResidencia.Visible = false;
            LblEliminarReporte.Visible = false;
            DDlEliminarReporte.Visible = false;
            LbtnConfirmarR.Visible = false;
            //habilitar botones y cajas de texto
            LbtnCancelEditionC.Enabled = true;
            LbtnNuevo.Enabled = true;
            LbtnEliminar.Enabled = true;
            LbtnActualizar.Enabled = true;
            string fechadereporte = DDlEliminarReporte.SelectedItem.ToString(); //Seleccionó una fecha de la liste despegable
            Int64 idreporte;
            int Accion = 1, ncaracteres = fechadereporte.Length;
            string mes = fechadereporte.Substring(0,(ncaracteres  -5));
            string anio = fechadereporte.Remove(0, (ncaracteres - 4));
            string namecontrato = DDlEleminarResidencia.SelectedItem.ToString();
            Int64 IdContrato = ObtenerIdResidencia(namecontrato); //Obtenemos el Id de la Residencia
            if (mes.CompareTo("Enero") == 0)
            {
                mesyanio = anio+"-"+"01%";
            }
            else if (mes.CompareTo("Febrero") == 0)
            {
                mesyanio = anio + "-" + "02%";
            }
            else if (mes.CompareTo("Marzo") == 0)
            {
                mesyanio = anio + "-" + "03%";
            }
            else if (mes.CompareTo("Abril") == 0)
            {
                mesyanio = anio + "-" + "04%";
            }
            else if (mes.CompareTo("Mayo") == 0)
            {
                mesyanio = anio + "-" + "05%";
            }
            else if (mes.CompareTo("Junio") == 0)
            {
                mesyanio = anio + "-" + "06%";
            }
            else if (mes.CompareTo("Julio") == 0)
            {
                mesyanio = anio + "-" + "07%";
            }
            else if (mes.CompareTo("Agosto") == 0)
            {
                mesyanio = anio + "-" + "08%";
            }
            else if (mes.CompareTo("Septiembre") == 0)
            {
                mesyanio = anio + "-" + "09%";
            }
            else if(mes.CompareTo("Octubre") == 0)
            {
                mesyanio = anio + "-" + "10%";
            }
            else if(mes.CompareTo("Noviembre") == 0)
            {
                mesyanio = anio + "-" + "11%";
            }
            else if(mes.CompareTo("Diciembre") == 0)
            {
                mesyanio = anio + "-" + "12%";
            }
            
                try
                {
                    con = new SqlConnection(strConexion);
                    using (con)
                    {
                        String ObtenerIdReportePorfecha = "SELECT IdReporte FROM Reportes WHERE IdResidencia =@idcontrato AND FechaDeReporte Like @mesyanio"; //Consulta para obtener el Id de un reporte por fecha de reporte
                        OrdenSql = new SqlCommand(ObtenerIdReportePorfecha, con);
                        con.Open();
                        OrdenSql.Parameters.AddWithValue("@mesyanio", mesyanio);
                        OrdenSql.Parameters.AddWithValue("@idcontrato", IdContrato);
                        idreporte = Convert.ToInt64(OrdenSql.ExecuteScalar());              //Me devuelve el Id del reporte
                    if (ExisteProyeccionReporte(idreporte)) //Primero pregunto si existe proyección en el contrato, utiizando como condición el IdReporte
                    {
                        BorrarProyeccion(IdContrato);
                    }
                    while (Accion <= 5)
                        {
                            BorrarReporteEnSQL(Accion, idreporte); //Metodo que borra todo el reporte en la base de datos
                            ++Accion;
                        }
                        con.Close();
                        DDlEliminarReporte.Items.Clear();
                    //AlertSuccess.Visible = true;
                    //lblSucces.Text = "¡Reporte mensual <strong>borrado</strong> con éxito! ";
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "click", " swal('¡Bien hecho!', 'Reporte mensual borrado con éxito!', 'success');", true);
                }

                }
                catch (Exception ex)
                {

                    AlertDanger.Visible = true;
                    lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
                }
        }
        protected Boolean ExisteProyeccionReporte(Int64 IdReporte) //Metodo que devuelve True si existe Una Proyeccion en el contrato, utilizando el IdReporte como condición
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String ExisteProyeccion = "Select 'true' From ProyeccionPorContrato Where  IdReportes = @idreporte";
                    OrdenSqlProy = new SqlCommand(ExisteProyeccion, con);
                    con.Open();
                    OrdenSqlProy.Parameters.AddWithValue("@idreporte", IdReporte);
                    ExisteReporte = Convert.ToBoolean(OrdenSqlProy.ExecuteScalar());
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
            return ExisteReporte;
        }
        protected void BorrarReporteEnSQL(int Accion, Int64 idreporte) //Metodo que ejecuta un procedimiento almacenado, pasandole como parametro la accion a realizar
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string NameStoredProcedure = "EliminarReporte";
                    OrdenSql = new SqlCommand(NameStoredProcedure, con);
                    con.Open();
                    OrdenSql.CommandType = CommandType.StoredProcedure;
                    OrdenSql.Parameters.AddWithValue("@Accion", Accion);
                    OrdenSql.Parameters.AddWithValue("@IdReporte", idreporte);
                    OrdenSql.ExecuteNonQuery();
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }
        protected void LbtnConsolidado_Click(object sender, EventArgs e)
        {
            Session["Usuario"] = nombreUsuario;
            Response.Redirect("Inicio.aspx");
        }
        protected void LbtnProyectos_Click(object sender, EventArgs e)
        {
            Session["Usuario"] = nombreUsuario;
            Response.Redirect("Proyectos.aspx");
        }

        protected void LbtnAmbiental_Click(object sender, EventArgs e)
        {
            Session["Usuario"] = nombreUsuario;
            Response.Redirect("Ambiental.aspx");
        }

        protected void LbtnContruccion_Click(object sender, EventArgs e)
        {
            Session["Usuario"] = nombreUsuario;
            Response.Redirect("Construccion.aspx");
        }
       protected Boolean BuscarCptoRepetido()   ///Metodo que busca un concepto repetido en la columna Concepto
       {
            for (int f = 0; f < GvDatosN.Rows.Count; f++)
            {
                for (int i = f + 1; i < GvDatosN.Rows.Count; i++)
                {
                    if (GvDatosN.Rows[f].Cells[0].Text .CompareTo(GvDatosN.Rows[i].Cells[0].Text) == 0)
                    {
                        fila = f;
                        return true;
                    }
                }
            }
            return false;
        }
        protected Boolean BuscarCeldasVacias()  //Metodo que busca si una celda se encuentra vacia en cualquiera de las columnas Presupuesto y Real
        {
            string columnConcepto, columnPresupuesto, columnReal;
            for (int f = 0; f < GvDatosN.Rows.Count; f++)
            {
                columnConcepto = HttpUtility.HtmlDecode(GvDatosN.Rows[f].Cells[0].Text);
                columnPresupuesto =  HttpUtility.HtmlDecode(GvDatosN.Rows[f].Cells[2].Text);
                columnReal = HttpUtility.HtmlDecode(GvDatosN.Rows[f].Cells[3].Text);
                if (string.IsNullOrWhiteSpace(columnPresupuesto) && string.IsNullOrWhiteSpace(columnReal))
                {
                    fila = f;
                    Columna = "Presupuesto, Real";
                    return true;
                }
                else if (string.IsNullOrWhiteSpace(columnConcepto))
                {
                    fila = f;
                    Columna = "Concepto";
                    return true;
                }
                else
                if (string.IsNullOrWhiteSpace(columnPresupuesto))
                {
                    fila = f;
                    Columna = "Presupuesto";
                    return true;
                        
                }else if (string.IsNullOrWhiteSpace(columnReal))
                {
                    fila = f;
                    Columna = "Real";
                    return true;
                }
            }
          
            return false;
        }

        DataView ImportarDatos(String nombreArchivo) //Metodo que recibe la ruta junto con el nombre del archivo a subir al servidor y devuelve una tabla llenada con los datos leidos
        {
            String conexion = string.Format("Provider=Microsoft.Jet.OleDb.4.0; Data Source = {0};Extended Properties = \"Excel 8.0;HDR=YES\"", nombreArchivo);
            DataTable tabla = new DataTable();
            OleDbConnection conector = new OleDbConnection(conexion);
            using (conector)
            {
                OleDbCommand consulta = new OleDbCommand("Select * from [Hoja1$]", conector);
                conector.Open();
                OleDbDataAdapter adaptador = new OleDbDataAdapter(consulta.CommandText, conector);
                adaptador.Fill(tabla);
            }

            return tabla.DefaultView;
    
        }
        protected void LbtnImportar_Click(object sender, EventArgs e)  //Evento click para subir el archivo de Excel al servidor
        { 
            string RutaArchivo;
            Boolean ExisteContrato = VerificarSiExistenContratos();
            if (FUpExaminar.HasFile == true)
            {

                try
                {
                    FUpExaminar.SaveAs(MapPath("~/Archivos/" + FUpExaminar.FileName.ToString())); //Guardamos el archivo en la carpeta Archivos del servidor
                    RutaArchivo = Server.MapPath("Archivos/" + FUpExaminar.FileName); //Accedemos a la carpeta a donde estan los archivos subidos
                   
                    GvDatosN.DataSource = ImportarDatos(RutaArchivo);
                    GvDatosN.DataBind();
                    
                    if ( BuscarCptoRepetido()) //Este metodo devuelve true si existe un concepto REPETIDO solo en la columna CONCEPTO
                    {
                        AlertWarning.Visible = true;
                        lblWarning.Text = "Hay un concepto <strong>REPETIDO: </strong>\"" + GvDatosN.Rows[fila].Cells[0].Text +"\"";
                        LbtnGuardarR.Enabled = false;
                    }
                    else

                    if (BuscarCeldasVacias())  //Este metodo devuelve true si existe una celda vacía, solo en las Columnas PRESUPUESTO y REAL
                    {
                        AlertWarning.Visible = true;
                        lblWarning.Text = "Hay una celda <strong>vacía -></strong>(<strong>Concepto:</strong>\"" + GvDatosN.Rows[fila].Cells[0].Text+ "\", <strong>Columna:</strong> " + Columna + ")";
                        LbtnGuardarR.Enabled = false;
                    }
                    else
                    {
                        AlertSuccess.Visible = true;
                        lblSucces.Text = " ¡Archivo <strong>cargado</strong> con éxito!";
                        if (ExisteContrato)     //Pregunto si existe al menos un contrato, entonces activo el botón Guardar Reporte
                        {
                            LbtnGuardarR.Enabled = true;
                        }
                        
                    }
                   
                    if (LbtnOcultarN.Visible == true)
                    {
                        LbtnVisualizar.Visible = false;
                    }
                    else
                    {
                        LbtnVisualizar.Visible = true;
                    }
                   
                }
                catch (Exception ex)
                {
                    AlertDanger.Visible = true;
                    lblDanger.Text = " Error en el formato de archivo a cargar... <br /> <strong><strong>Detalles de Error:</strong> " + ex.Message;
                }
            }
            else
            {
                AlertWarning.Visible = true;
                lblWarning.Text = " ¡<strong>Olvidó seleccionar un archivo...<strong>!";
            
            }
        }

      
        
        protected void LbtnContratos_Click(object sender, EventArgs e)
        {
            MenuGeneral.Visible = false;
            TitleS.Visible = false;
            TitleSC.Visible = true;
            MenuContratos.Visible = true;
            PnlTable.Visible = false;
            CargaTablaContratos();
           // VerificarSiExistenContratos();
            LbtnCancelEditionC.Visible = false;
          
        }

        protected void LbtnRegresarGeneral_Click(object sender, EventArgs e)
        {
            TitleS.Visible = true;
            TitleSC.Visible = false;
            MenuContratos.Visible = false;
            PnlEditar.Visible = false;
            MenuGeneral.Visible = true;
            LbtnContratos.Visible = true;
            PnlTable.Visible = true;
            PnlTableC.Visible = false;
            HeaderTable.Visible = false;
            FooterTable.Visible = false;
            PnlVacio.Visible = false;
            LbtnCancelEditionC.Visible = false;
            PnlNuevo.Visible = false;
            PnlEliminar.Visible = false;
            PnlActualizar.Visible = false;
            LbtnGraficaArea.Visible = true;
            LimpiarGridView(); //Limpiar GridView Tabla De Reportes
            GVContratos.DataSource = "";
            GVContratos.DataBind();
            CargarDDlFechasInicialFinal(); //Mando a llamar este metodo para llenar las listas despegables de la Fecha Inicial y Fecha Final
            CargarContratosArea(); // Metodo para cargar el resumen  de Rentabilidad de los contrato
           
        }

        protected void LbtnEditarC_Click(object sender, EventArgs e)
        {
            LbtnEditarC.Visible = false;
            LbtnCancelEditionC.Visible = true;
            PnlTable.Visible = false;
            PnlTableC.Visible = false;
            PnlEditar.Visible = true;
            LbtnRegresarGeneral.Visible = false;
            LbtnContratos.Visible = false;
            LbtnVisualizar.Visible = false;
            LbtnGraficaC.Visible = false;
            PnlVacio.Visible = false;
            LbtnCambiarPorcentaje.Visible = false;
            //Ocultar botones y cajas de texto
            LbtnBuscarC.Visible = false;
            DdlContratos.Visible  = false;
            DdlMesC.Enabled = false;  //Este control nunca se utilizan, se puede quitar
            DdlAnioC.Enabled = false; //""
           
        }

        protected void LbtnCancelEditionC_Click(object sender, EventArgs e)
        {
            LbtnEditarC.Visible = true;
            LbtnCancelEditionC.Visible = false;
            MenuGeneral.Visible = false;
            TitleS.Visible = false;
            TitleSC.Visible = true;
            MenuContratos.Visible = true;
            LbtnGraficaC.Visible = true;
            PnlTable.Visible = false;
            PnlTableC.Visible = true;
            LbtnRegresarGeneral.Visible = true;
            PnlEditar.Visible = false;
            PnlNuevo.Visible = false;
            PnlEliminar.Visible = false;
            PnlActualizar.Visible = false;
            LbtnVisualizar.Visible = false;
            //Ocultar botones y cajas de texto
            LbtnBuscarC.Visible = true;
            DdlContratos.Visible = true;
            LbtnCambiarPorcentaje.Visible = true;
            LimpiarGridView();  //Limpiar GridView Mostrada al usuario
            //VerificarSiExistenContratos();
            //Recargar La Tabla de contratos y la lista despegable, para que se actualicen los datos
            CargarResidencias();
            CargaTablaContratos();
        }

        protected void LbtnAgregarContrato_Click(object sender, EventArgs e)
        {
            LbtnReporteMensualN.Visible = false;
            LbtnAgregarContrato.Visible = false;
            LbtnAgregarProyeccion.Visible = false;
            LblFechaInicialNC.Visible = true;
            TxtFechaInicialNC.Visible = true;
            LblFechaTerminoNC.Visible = true;
            TxtFechaTerminoNC.Visible = true;
            TxtMontoC.Visible = true;
            SignoMoneda.Visible = true;
            TxtNombreResidencia.Visible = true;
            LbtnGuardarC.Visible = true;
            //Cambiar Texto a titulo del panel
            LblTitlePnlAgregarN.Text = "Agregar Contrato";
        }
        protected int ObtenerIdArea() //METODO que devuelve el Id del area
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String queryArea = "Select IdArea From Areas Where Area = 'Supervision'"; //Consulta para obtener el IdArea de Supervision
                    OrdenSql = new SqlCommand(queryArea, con);
                    con.Open();
                    IDarea = Convert.ToUInt16(OrdenSql.ExecuteScalar());
                    con.Close();
                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
            return IDarea;
        }
       
        protected void LbtnGuardarC_Click(object sender, EventArgs e)
        {
            IDarea = ObtenerIdArea();
            Boolean ExisteContrato;
            String queryInsertR = "Insertar_Residencia",accion = "Agregar"; //Nombre del procedimiento almacenado
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String queryExisteResidencia = "Select 'true' From Residencias Where EXISTS(Select* From Residencias where Residencia =@residencia AND IdArea = @idarea)";
                    OrdenSql = new SqlCommand(queryInsertR, con);
                    OrdenSqlER = new SqlCommand(queryExisteResidencia, con);
                    OrdenSql.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    //Verificar si el contrato ya existe en la base de datos y si no, se crea una nueva
                    OrdenSqlER.Parameters.AddWithValue("@residencia", TxtNombreResidencia.Text);
                    OrdenSqlER.Parameters.AddWithValue("@idarea", IDarea);
                    ExisteContrato = Convert.ToBoolean(OrdenSqlER.ExecuteScalar());
                    if (!ExisteContrato)
                    {
                        if (TxtFechaInicialNC.Text != "" && TxtFechaTerminoNC.Text !="" && TxtMontoC.Text != "" && TxtNombreResidencia.Text != "")
                        {
                            //Agrego los datos ingresados en los TextBox
                            OrdenSql.Parameters.AddWithValue("@idarea", IDarea);
                            OrdenSql.Parameters.AddWithValue("@residencia", TxtNombreResidencia.Text);
                            OrdenSql.Parameters.AddWithValue("@fechainicio", TxtFechaInicialNC.Text);
                            OrdenSql.Parameters.AddWithValue("@fechatermino", TxtFechaTerminoNC.Text);
                            OrdenSql.Parameters.AddWithValue("@montocontratado", TxtMontoC.Text);
                            OrdenSql.ExecuteNonQuery();
                            //AlertSuccess.Visible = true;
                            //lblSucces.Text = " ¡Contrato <strong>creado</strong> con exito!";
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "click", " swal('¡Bien hecho!', '¡Contrato agregado con éxito!', 'success');", true);
                            TotalContratos(IDarea, accion);
                            ClearTextboxN();
                            Controles_LbtnGuardarC();
                        }
                        else
                        {
                            //AlertWarning.Visible = true;
                            //lblWarning.Text = " <strong>¡Importante!</strong> Es necesario llenar todos los campos...";
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "click", " swal('¡Importante!', '¡Debe llenar todos los campos!', 'warning');", true);
                            TxtFechaInicialNC.Focus();
                        }
                       
                    }
                    else
                    {
                        //AlertWarning.Visible = true;
                        ClearTextboxN();
                        //lblWarning.Text = " <strong>¡Cuidado!</strong> El contrato ya existe! </br> <strong>Solución:</strong>Ingrese un nombre de residencia distinto...";
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "click", " swal('¡Cuidado!', '¡El contrato ya existe! Vuelva a intentarlo....', 'warning');", true);
                    }

                    con.Close();


                }
            }
            catch (Exception ex)
            {
                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }


        }
        protected void TotalContratos(Int64 IDarea,string accion) //Metodo para llevar  acabo el conteo de contratos en cada Area
        {
            Int64 CuentaContratos, ncontrato;
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String ObtenerTotalContratos = "Select TotalContratos From Areas Where IdArea = @idarea";
                    OrdenSql = new SqlCommand(ObtenerTotalContratos, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idarea", IDarea);
                    ncontrato = Convert.ToInt64(OrdenSql.ExecuteScalar());
                    if(ncontrato > 0 && accion.CompareTo("Agregar")==0)  //Si la accion a realizar es aagregar un contrato la variable CuentaContratos incrementa 1
                    {
                        CuentaContratos = ncontrato + 1;
                        ActualizarTotalContratos(IDarea, CuentaContratos); //Metodo para actualizar el Total de Contratos que hay en el Area de Supervisión
                    }
                    else if (ncontrato > 0 && accion.CompareTo("Borrar")==0) //Si la accion a realizar es agregar un contrato la variable CuentaContratos decrementa 1
                    {
                        CuentaContratos = ncontrato - 1;
                        ActualizarTotalContratos(IDarea, CuentaContratos);
                    }
                    else if(accion.CompareTo("Agregar") == 0)
                    {     //Aqui entra SOLO si es la PRIMERA VEZ que se agreaga un contrato
                        CuentaContratos = 1;
                        ActualizarTotalContratos(IDarea,CuentaContratos); //Metodo para actualizar el Total de Contratos que hay en el Area de Supervisión
                    }
                    con.Close();

                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }
        protected void ActualizarTotalContratos(Int64 IDarea,Int64 CuentaContratos)
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String queryArea = "UPDATE Areas SET TotalContratos =@cuentacontratos WHERE IdArea =@idarea"; // Actualizo TotalContratos de la Tabla Areas, para llevar un control de cuantos contratos hay por área
                    OrdenSql = new SqlCommand(queryArea, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@cuentacontratos", CuentaContratos);
                    OrdenSql.Parameters.AddWithValue("@idarea", IDarea);
                    OrdenSql.ExecuteNonQuery();
                    con.Close();
                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }
        protected void ClearTextboxN()
        {
            TxtNombreResidencia.Text = "";
            TxtFechaInicialNC.Text = "";
            TxtFechaTerminoNC.Text = "";
            TxtMontoC.Text = "";
        }
        protected void Controles_LbtnGuardarC()
        {
            PnlNuevo.Visible = false;
            LbtnReporteMensualN.Visible = true;
            LbtnAgregarContrato.Visible = true;
            LblFechaInicialNC.Visible = false;
            TxtFechaInicialNC.Visible = false;
            LblFechaTerminoNC.Visible = false;
            TxtFechaTerminoNC.Visible = false;
            SignoMoneda.Visible = false;
            TxtMontoC.Visible = false;
            TxtNombreResidencia.Visible = false;
            LbtnGuardarC.Visible = false;
            //habilitar botones y cajas de texto
            LbtnCancelEditionC.Enabled = true;
            LbtnNuevo.Enabled = true;
            LbtnEliminar.Enabled = true;
            LbtnActualizar.Enabled = true;
            //Ocultar titulo del panel
            LblTitlePnlAgregarN.Visible = false;
        }

        protected void LbtnReporteMensualN_Click(object sender, EventArgs e) //Evento del botón ReporteMensual de Agregar Nuevo Historial
        {
            LbtnReporteMensualN.Visible = false;
            LbtnAgregarContrato.Visible = false;
            LbtnAgregarProyeccion.Visible = false;
            LblContratosN.Visible = true;
            DdlContratosN.Visible = true;
            LblFechaReporte.Visible = true;
            TxtFechaReporte.Visible = true;
            FUpExaminar.Visible = true;
            LbtnImportar.Visible = true;
            LbtnGuardarR.Visible = true;
            LbtnHelp.Visible = true;
            LblbtnFormatoExcel.Visible = true;
            //Mostrar titulo del panel
            LblTitlePnlAgregarN.Visible = true;
            LblTitlePnlAgregarN.Text = "Agregar Reporte";
            Boolean ExisteContrato = VerificarSiExistenContratos();
            if (ExisteContrato)
            {
                try
                {
                    con = new SqlConnection(strConexion);
                    using (con)
                    {
                        String ObtenerResidencias = "Select Residencia From Residencias where IdArea = @idarea";
                        OrdenSqlObtR = new SqlCommand(ObtenerResidencias, con);
                        con.Open();
                        OrdenSqlObtR.Parameters.AddWithValue("@idarea", ObtenerIdArea());
                        DdlContratosN.DataSource = OrdenSqlObtR.ExecuteReader();
                        DdlContratosN.DataTextField = "Residencia";
                        DdlContratosN.DataBind();
                        con.Close();
                        LbtnGuardarR.Enabled = true;

                    }

                }
                catch (Exception ex)
                {

                    AlertDanger.Visible = true;
                    lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
                }
            }
            else if(DdlContratosN.SelectedValue.ToString().CompareTo("Ninguno") != 0)
            {
                DdlContratosN.Items.Add("Ninguno");
                LbtnGuardarR.Enabled = false;
            }
            
        }
        protected int ObtenerIdReporte(int IdR, string fechareporte)  //..........................Metodo para obtener el Id de un reporte por fecha
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String queryreporte = "Select IdReporte From Reportes Where IdResidencia = @idresidencia AND FechaDeReporte = @datereporte"; //Consulta para obtener el IdReporte de Supervision
                    OrdenSql = new SqlCommand(queryreporte, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idresidencia", IdR);
                    OrdenSql.Parameters.AddWithValue("@datereporte", fechareporte);
                    Idreporte = Convert.ToUInt16(OrdenSql.ExecuteScalar());
                    con.Close();
                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
            return Idreporte;
        }

        protected void LbtnBuscarC_Click(object sender, EventArgs e) //Botón para buscar los datos de la residencia seleccionada en la lista despegable/// CONTRATOS
        {
            CargarContratos(); //Mando a llamar este metodo para cargar en la tabla un contrato seleccionado en la lista despegable
        }
        protected void CargarContratos()  //Metodo que Carga el contrato seleccionado en la lista despegable a donde se muestran los contratos
        {
            string Residencia;
            int IdResidencia;
            Boolean ExisteContrato = VerificarSiExistenContratos();
            if (ExisteContrato)
            {
                Residencia = DdlContratos.SelectedItem.ToString(); //Obtenemos el nombre de la residencia seleccionada
                IdResidencia = ObtenerIdResidencia(Residencia); //Obtenemos el Id de la Residencia
                ConstruirTabla(IdResidencia);

            }
            else
            {
               
                PnlVacio.Visible = true;
            }
        }
        protected Boolean VerificarSiExistenReportes(Int64 IdR) //Metodo que verifica si exiten contratos agregados, si no existen entonces muestro un panel para indicar que no hay nada que mostrar
        {

            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string ConsultarReporte = "Select 'true' From Reportes Where IdResidencia = @idresidencia";
                    OrdenSql = new SqlCommand(ConsultarReporte, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idresidencia",IdR);
                    ExistenReportes = Convert.ToBoolean(OrdenSql.ExecuteScalar());
                    con.Close();
                }

            }
            catch (Exception ex)
            {
                AlertDanger.Visible = true;
                lblDanger.Text = "<strong><strong>Detalles de Error:</strong> " + ex.Message;
            }
            return ExistenReportes;
        }
        protected void CargarDatosResidencia(int IdR) //Aqui Cargo los datos de una residencia (Contrato), recibe como parametro el Id de Residencia (Contrato)
        {
            string anio, mes, dia,fechaInicio,fechaTermino,NombreResidencia;
            double UcP, UcR;
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat; //Creo Objeto para personalizar el símbolo de moneda
            nfi.CurrencyPositivePattern = 2; //Establesco la posicion del simbolo de moneda
            DateTime fechaActual = DateTime.Now;
            anio = Convert.ToString(fechaActual.Year);
            mes = Convert.ToString(fechaActual.Month);
            dia = Convert.ToString(fechaActual.Day);
            if (dia.Length < 2)
            {
                dia = "0" + dia;
            }
            if (mes.Length < 2)
            {
                mes = "0" + mes;
            }
            LblHeaderFecha.Text = dia+"/"+mes+"/"+anio;
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string datos = "Select Residencia,FechaDeInicio,FechaDeTermino,MontoContratado From Residencias Where IdResidencia = @idresidencia";
                    OrdenSql = new SqlCommand(datos,con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idresidencia", IdR);
                    Leer = OrdenSql.ExecuteReader();
                    while (Leer.Read())
                    {
                        NombreResidencia = Leer[0].ToString();
                        LblHeaderResidencia.Text = "FLUJO DE EFECTIVO " + NombreResidencia.ToUpper();
                        fechaInicio = Leer[1].ToString();
                        DateTime fechadia = DateTime.Parse(fechaInicio);
                        DateTime fechames = DateTime.Parse(fechaInicio);     
                        DateTime fechaanio = DateTime.Parse(fechaInicio); 
                        dia = Convert.ToString(fechadia.Day);
                        mes = Convert.ToString(fechames.Month);           
                        anio = Convert.ToString(fechaanio.Year);
                        if (dia.Length < 2)
                        {
                            dia = "0" + dia;
                        }
                        if (mes.Length < 2)
                        {
                            mes = "0" + mes;
                        }
                        LblHeaderFechaInicio.Text = dia+"/"+mes+"/"+anio;
                        fechaTermino = Leer[2].ToString();
                        DateTime fechaDia = DateTime.Parse(fechaTermino);
                        DateTime fechaMes = DateTime.Parse(fechaTermino);
                        DateTime fechaAnio = DateTime.Parse(fechaTermino);
                        dia = Convert.ToString(fechaDia.Day);
                        mes = Convert.ToString(fechaMes.Month);
                        anio = Convert.ToString(fechaAnio.Year);
                        if (dia.Length < 2)
                        {
                            dia = "0" + dia;  
                        }
                        if (mes.Length < 2)
                        {
                            mes = "0" + mes;
                        }
                        LblHeaderFechaTermino.Text = dia + "/" + mes + "/" + anio;
                        MontoP = Convert.ToDouble(Leer[3].ToString());
                        LblHeaderMonto.Text = string.Format(nfi, "{0:C}",MontoP);  //Aqui muestro el monto del contrato
                                                                                   //LblFooterMCP.Text = string.Format(nfi, "{0:C}", MontoP);     //Aquí  muestro el monto del contrato para presupuesto
                        UcP = MontoTP - TotalCDIP - Financiamientop;     //Utilidad del contrato para Presupuesto 
                        UcR = MontoR - TotalCDIR - Financiamientor;     //Utilidad del contrato para Real
                        if (UcP < 0)
                        {
                            UcP *= -1;
                            LblFooterUP.Text ="-" + string.Format(nfi, "{0:C}",UcP );      // Aquí muestro  la utilidad del Contrato para presupuesto
                        }
                        else
                        {
                            LblFooterUP.Text = string.Format(nfi, "{0:C}", UcP);      // Aquí muestro  la utilidad del Contrato para presupuesto
                        }
                        if(UcR < 0)
                        {
                            UcR *= -1;
                            LblFooterUR.Text = "-" + string.Format(nfi, "{0:C}", UcR);      // Aquí muestro la utilidad del Contrato para real con formato de moneda
   
                        }
                        else
                        {
                                 // Aquí muestro la utilidad del Contrato para real con formato de moneda
                            LblFooterUR.Text = string.Format(nfi, "{0:C}", UcR);
                        }
                        if(MontoP != 0)
                        {
                            LblFooterPP.Text = Convert.ToString(Math.Round(((MontoTP - TotalCDIP - Financiamientop) / MontoP) * 100)) + "%";  //Aquí muestro el porcentaje para presupuesto
                        }
                        else
                        {
                            LblFooterPP.Text = "0%";
                        }
                        if (MontoR != 0)
                        {
                            LblFooterPR.Text = Convert.ToString(Math.Round(((MontoR - TotalCDIR - Financiamientor) / MontoR) * 100)) + "%";    //Aquí muestro el porcentaje para real
                        }
                        else
                        {
                            LblFooterPR.Text = "0%";
                        }

                        

                    }
                        con.Close();
                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }
        protected string UltimaFechaDeReporte(Int64 IdContrato) //Metodo que devuelve la fecha del ultimo reporte agregado
        {
            string fecha,nmes,anio;
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string UltimaFechaAgregada = "Select FechaDeReporte From Reportes Where IdResidencia = @idcontrato Order BY FechaDeReporte DESC";
                    OrdenSql = new SqlCommand(UltimaFechaAgregada, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idcontrato", IdContrato);
                    fecha = Convert.ToString(OrdenSql.ExecuteScalar());
                    con.Close();
                    DateTime Fecha = DateTime.Parse(fecha);
                    DateTime fechames = DateTime.Parse(fecha);          //Se crea un objeto de tipo DateTime para Mes
                    DateTime fechaanio = DateTime.Parse(fecha);         //Se crea un objeto de tipo DateTime para Año
                    nmes = Convert.ToString(fechames.Month);             //Obtengo solo el Mes de la fecha completa
                    anio = Convert.ToString(fechaanio.Year);            //Obtengo solo el Año de la fecha completa
                    if (nmes == "1")
                    {
                        UltimaFechaReporte = "Enero-" + anio;
                    }
                    else if (nmes == "2")
                    {
                        UltimaFechaReporte = "Febrero-" + anio;
                    }
                    else if (nmes == "3")
                    {
                        UltimaFechaReporte = "Marzo-" + anio;
                    }
                    else if (nmes == "4")
                    {
                        UltimaFechaReporte = "Abril-" + anio;
                    }
                    else if (nmes == "5")
                    {
                        UltimaFechaReporte = "Mayo-" + anio;
                    }
                    else if (nmes == "6")
                    {
                        UltimaFechaReporte = "Junio-" + anio;
                    }
                    else if (nmes == "7")
                    {
                        UltimaFechaReporte = "Julio-" + anio;
                    }
                    else if (nmes == "8")
                    {
                        UltimaFechaReporte = "Agosto-" + anio;
                    }
                    else if (nmes == "9")
                    {
                        UltimaFechaReporte = "Septiembre-" + anio;
                    }
                    else if (nmes == "10")
                    {
                        UltimaFechaReporte = "Octubre-" + anio;
                    }
                    else if (nmes == "11")
                    {
                        UltimaFechaReporte = "Noviembre-" + anio;
                    }
                    else
                    {
                        UltimaFechaReporte = "Diciembre-" + anio;
                    }
                }
            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
            return UltimaFechaReporte;
        }
        protected void CargarTablaGlobalArea() //Metodo utilidado para cargar y actualizar los datos de la Tabla Global de los Contratos de Supervisión
        {
          
                CargarDDlFechasInicialFinal(); //Mando a llamar este metodo para llenar las listas despegables de la Fecha Inicial y Fecha Final
                CargarContratosArea(); // Metodo para cargar el resumen  de Rentabilidad de los contrato
        }
        protected void CargaTablaContratos()        //Metodo utilizado para cargar y actualizar los datos de la Tabla contratos
        {
            int IdContrato = ConsultarIdResidencia();
            ConstruirTabla(IdContrato);
        }
        protected int ConsultarIdResidencia()
        {
            string Residencia;
          
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String queryreporte = "Select Residencia From Residencias  Where IdArea = @idarea ORDER By IdResidencia"; //Consulta que devuelve la primera residencia o contrato agregada(o) en Supervision
                    OrdenSqlIdRe = new SqlCommand(queryreporte, con);
                    con.Open();
                    OrdenSqlIdRe.Parameters.AddWithValue("@idarea", ObtenerIdArea());
                    Residencia = Convert.ToString(OrdenSqlIdRe.ExecuteScalar());
                    Idresidencia = ObtenerIdResidencia(Residencia); //Obtenemos el Id de la Residencia
                    con.Close();
                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
            return Idresidencia;
        }
       
        protected void LbtnGuardarR_Click(object sender, EventArgs e)    //botón para aguardar el REPORTE NUEVO DE CADA MES, PARA ESO HAY QUE OBTENER LOS DATOS DEL EXCEL
        {
           
            String Residencia;
            int IdResidencia;

            if ((LbtnVisualizar.Visible == true || LbtnOcultarN.Visible == true) && TxtFechaReporte.Text != "")
            {
                
                    Residencia = DdlContratosN.SelectedItem.ToString(); //Obtenemos el nombre del contrato seleccionado
                    IdResidencia = ObtenerIdResidencia(Residencia); //Obtenemos el Id de la Residencia
                    AgregarReporte(IdResidencia);               //Mando a llamar el metodo para agregar un nuevo reporte 
               
                    Controles_LbtnGuardarR();
                    TxtFechaReporte.Text = "";
               
            }
            else if(LbtnVisualizar.Visible == false && TxtFechaReporte.Text != "")
            {
                //AlertWarning.Visible = true;
                //lblWarning.Text = " ¡Olvido cargar el archivo Excel!";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "click", " swal('¡Cuidado!', '¡Olvidó cargar el archivo de Excel!', 'warning');", true);
            }
            else if((LbtnVisualizar.Visible == true || LbtnOcultarN.Visible == true) && TxtFechaReporte.Text == "")
            {
                //AlertWarning.Visible = true;
                //lblWarning.Text = " ¡Olvido llenar el campo fecha del reporte!";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "click", " swal('¡Cuidado!', '¡Olvidó llenar el campo fecha del reporte!', 'warning');", true);
            }
            else
            {
                //AlertWarning.Visible = true;
                //lblWarning.Text = "<strong>¡Importante!</strong> Es obligatorio llenar todos los campos...";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "click", " swal('¡Importante!', '¡Debe llenar todos los campos!', 'warning');", true);
            }
            
        }
        protected int ObtenerIdConceptoActualizar(string concepto,int IdR)      
        {
            
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String ObtenerIdConceptoA = "Select IdConcepto From Concepto where IdResidencia =@idresidencia AND Concepto=@concepto";
                    OrdenSql = new SqlCommand(ObtenerIdConceptoA, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idresidencia", IdR);
                    OrdenSql.Parameters.AddWithValue("@concepto", concepto);
                    Idconcepto = Convert.ToUInt16(OrdenSql.ExecuteScalar());
                    con.Close();

                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
            return Idconcepto;
        }
     
        protected void EnviarGridaSql(int IdR) //Metodo utilizado para enviar los datos obtenidos del gridview mostrado al usuario a la BD
        {
            string columnaConcepto, columnaObs;
            string  columnaPres, columnaReal;
            decimal columnaVar;
            int idReport = ObtenerIdReporte(IdR,TxtFechaReporte.Text), Idconcepto,i=0, nconceptos = ObtenerNumConceptos(IdR); ;
            string[] Obs = new string[nconceptos];
            int[] IdC = new int[nconceptos];
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string InsertarDatos = "INSERT INTO DatosDeReporte (IdReporte, IdConcepto,Observacion,Presupuesto,Real,Variacion) VALUES (@idreporte,@idconcepto,@observacion,@presupuesto,@real,@variacion)";
                    string ObtenerIdConcepto = "Select IdConcepto From Concepto where Concepto = @concepto AND IdResidencia = @idresidencia"; 
                    OrdenSql = new SqlCommand(InsertarDatos, con);
                    OrdenSqlConcepto = new SqlCommand(ObtenerIdConcepto,con);
                    con.Open();
                    foreach (GridViewRow row in GvDatosN.Rows)
                    {
                        OrdenSql.Parameters.Clear();
                        OrdenSqlConcepto.Parameters.Clear();
                        columnaConcepto = HttpUtility.HtmlDecode(row.Cells[0].Text);//CONVERTIR a una CADENA que se ha codificado en HTML
                        OrdenSqlConcepto.Parameters.AddWithValue("@concepto", columnaConcepto); //Paso como parametro el contenido de la columna Concepto del GridView al Query ObtenerIdConcepto
                        OrdenSqlConcepto.Parameters.AddWithValue("@idresidencia", IdR);
                        Idconcepto = Convert.ToUInt16(OrdenSqlConcepto.ExecuteScalar()); //Obtengo el Id del concepto de la tabla Concepto de la BD
                        IdC[i] = Idconcepto;
                        columnaObs = HttpUtility.HtmlDecode(row.Cells[1].Text);
                        Obs[i] = columnaObs;
                        columnaPres = HttpUtility.HtmlDecode(row.Cells[2].Text);
                        columnaReal = HttpUtility.HtmlDecode(row.Cells[3].Text);
                        columnaVar = (Convert.ToDecimal(columnaReal) - Convert.ToDecimal(columnaPres));     //Aqui calculo la variacion de cada fila
                       
                        OrdenSql.Parameters.AddWithValue("@IdReporte", idReport);
                        OrdenSql.Parameters.AddWithValue("@IdConcepto", Idconcepto);
                        OrdenSql.Parameters.AddWithValue("@observacion", columnaObs);
                        OrdenSql.Parameters.AddWithValue("@presupuesto", Convert.ToDecimal(columnaPres));
                        OrdenSql.Parameters.AddWithValue("@real", Convert.ToDecimal(columnaReal));
                        OrdenSql.Parameters.AddWithValue("@variacion", columnaVar);
                        OrdenSql.ExecuteNonQuery();
                        ++i;
                    }
                    con.Close();
                   // A Q U I    S E   R E A L I Z A   L A   A C T U A L I Z A C I O N   D E   L A     O B S E R V A C I O N   P A R A  C A D A   C O N C E P T O  
                   //Si el usuario agrega la observación desde el EXCEL, entoces esa observación se va actualizar en la BD
                    for (i = 0; i < nconceptos; i++)
                    {
                        Idconcepto = IdC[i];
                        columnaObs = Obs[i];
                        ActualizarObsAlEnviarBD(Convert.ToInt64(Idconcepto),columnaObs); //Este Metodo se encarga de actualizar la OBSERVACIÓN del concepto en la BD, si se agrega una Observación desde EXCEL
                    }
                }
            }catch(Exception)
            {
                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Algo salio mal!</strong> Informe: Se detectó un problema en un concepto, revise en el archivo de excel si los conceptos coinciden con los ya cargados en el sistema."
                    + "\n <strong>Sugerencia:</strong > Borrar el reporte creado e intente de nuevo, si el problema persiste borrar todo el contrato.";
            }

        }
      
        protected void ActualizarObsAlEnviarBD(Int64 IdC,string Obs) //Metodo que se encarga de actualizar la Observación del concepto en la BD al Ingresar un Nuevo REPORTE MENSUAL,
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {

                    String updateObservacion = "UPDATE  DatosDeReporte SET Observacion = @observacion where IdConcepto = @idconcepto"; // Actualizo el Concepto  de la Tabla Concepto solo del Id del contrato seleccionado
                    OrdenSql = new SqlCommand(updateObservacion, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@observacion", Obs);
                    OrdenSql.Parameters.AddWithValue("@idconcepto", IdC);
                    OrdenSql.ExecuteNonQuery();
                    con.Close();

                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }
        protected void CargarConcepto(int IdR)          //Metodo para Cargar el concepto a la Base de Datos
        {
            Boolean ExisteConcepto;
            String columnConcepto;
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String queryExisteConcepto = "Select 'true' From Concepto Where EXISTS(Select IdResidencia From Concepto Where IdResidencia = @idresidencia)";
                    String InsertarConcepto = "INSERT INTO Concepto (IdResidencia,Concepto) VALUES (@idresidencia, @concepto)";
                    OrdenSql = new SqlCommand(InsertarConcepto, con);
                    OrdenSqlEC = new SqlCommand(queryExisteConcepto, con);
                    con.Open();
                    //Verificar si existe la residencia con los conceptos cargados, y si no, entonces cargo los conceptos con el nuevo id de residencia (contrato)
                    OrdenSqlEC.Parameters.AddWithValue("@idresidencia", IdR);
                    ExisteConcepto = Convert.ToBoolean(OrdenSqlEC.ExecuteScalar());
                    if (!ExisteConcepto)
                    {
                        foreach (GridViewRow row in GvDatosN.Rows)          //Leer cada elemento del GridView (Tabla) Mostrada al Usuario despues de subir el archivo
                        {
                            OrdenSql.Parameters.Clear();
                            columnConcepto = HttpUtility.HtmlDecode(row.Cells[0].Text); //CONVERTIR a una CADENA que se ha codificado en HTML
                            if (!string.IsNullOrWhiteSpace(columnConcepto))
                            {
                                OrdenSql.Parameters.AddWithValue("@idresidencia", IdR);
                                OrdenSql.Parameters.AddWithValue("@concepto", columnConcepto);
                                OrdenSql.ExecuteNonQuery();
                            }

                        }
                    }
                   
                    
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }
        protected void AgregarReporte(int IdR)                      // Método para agregar un reporte, Id reporte como parametro 
        {
            Boolean ExisteReporte;
            int ncaracteres = TxtFechaReporte.Text.Length;
            string mesyanio = TxtFechaReporte.Text.Substring(0, (ncaracteres - 3));
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String queryExisteReporte = "Select 'true' From Reportes Where  IdResidencia = @idresidencia AND FechaDeReporte LIKE @fechareporte";
                    String InsertarReporte = "INSERT INTO Reportes (IdResidencia, FechaDeReporte) VALUES (@idresidencia, @fechareporte)";
                    OrdenSql = new SqlCommand(InsertarReporte, con);
                    OrdenSqlER = new SqlCommand(queryExisteReporte, con);
                    con.Open();
                    //Verificar si el Reporte ya existe en la base de datos y si no, se crea una nueva
                    OrdenSqlER.Parameters.AddWithValue("@idresidencia", IdR);
                    OrdenSqlER.Parameters.AddWithValue("@fechareporte", mesyanio + "%");
                    ExisteReporte = Convert.ToBoolean(OrdenSqlER.ExecuteScalar());
                    if (!ExisteReporte)
                    {
                        OrdenSql.Parameters.AddWithValue("@idresidencia", IdR);
                        OrdenSql.Parameters.AddWithValue("@fechareporte", TxtFechaReporte.Text);
                        OrdenSql.ExecuteNonQuery();
                        CargarConcepto(IdR);               //Mando llamar el metodo para cargar todos los conceptos que hay en un reporte(archivo de excel a cargar)
                        EnviarGridaSql(IdR);               //De los datos del GridView mostrados al usuario, los envio a la BD
                                                           //AlertSuccess.Visible = true;
                                                           //lblSucces.Text = " ¡Reporte <strong>guardado</strong> con éxito!";
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "click", " swal('¡Bien hecho!', '¡Reporte agregado con éxito!', 'success');", true);
                    }
                    else
                    {
                        //AlertWarning.Visible = true;
                        //lblWarning.Text = " <strong>¡Cuidado!</strong> El reporte ya existe! </br> <strong>Detalles:</strong> Esta intentando agregar un <strong>reporte con la misma residencia y fecha de reporte...</strong> ";
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "click", " swal('¡Cuidado!', '¡El reporte ya existe!, Vuelva a intentarlo...', 'warning');", true);
                        TxtFechaReporte.Text = "";
                    }
                   
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }

        }
        protected void LbtnContratoE_Click(object sender, EventArgs e)  //Este es el boton para el evento Eliminar contrato del area de SUPERVISION
        {
            LbtnContratoE.Visible = false;
            LbtnReporteE.Visible = false;
            LbtnEliminarProyeccion.Visible = false;
            LbtnConfirmarC.Visible = true;
            LblTitlePnlEliminar.Visible = true;
            LblTitlePnlEliminar.Text = "Eliminar Contrato";
            LblEliminarContrato.Visible = true;
            DDlEleminarResidencia.Visible = true;
            Boolean ExisteContrato = VerificarSiExistenContratos();
          
                if (ExisteContrato)
                {
                    CargarContratosE();
                    LbtnConfirmarC.Enabled = true;
                }
                else if (DDlEleminarResidencia.SelectedValue.ToString().CompareTo("Ninguno") != 0)
                {
                    DDlEleminarResidencia.Items.Clear();
                    DDlEleminarResidencia.Items.Add("Ninguno");
                    LbtnConfirmarC.Enabled = false;
                }
        }
        protected void CargarContratosE() //Metodo para mostrar los Contratos o Residencias en la lista despegable de eliminar contrato
        {
            IDarea = ObtenerIdArea();
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                   
                        String ObtenerContratos = "Select Residencia From Residencias Where IdArea = @idarea";
                        OrdenSql = new SqlCommand(ObtenerContratos, con);
                        OrdenSql.Parameters.AddWithValue("@idarea", IDarea);
                        con.Open();
                        DDlEleminarResidencia.DataSource = OrdenSql.ExecuteReader();
                        DDlEleminarResidencia.DataTextField = "Residencia";
                        DDlEleminarResidencia.DataBind();
                        con.Close();
                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }
        protected void LbtnReporteE_Click(object sender, EventArgs e) //Este es el boton para el evento Eliminar reporte del contrato en el area de  SUPERVISION
        {
            LbtnContratoE.Visible = false;
            LbtnReporteE.Visible = false;
            LbtnEliminarProyeccion.Visible = false;
            LblEliminarContrato.Visible = true;
            DDlEleminarResidencia.Visible = true;
            LblEliminarReporte.Visible = true;
            DDlEliminarReporte.Visible = true;
            LbtnConfirmarR.Visible = true;
            LblTitlePnlEliminar.Visible = true;
            LblTitlePnlEliminar.Text = "Eliminar Reporte";
            CargarDDLEliminar(); //Metodo que cargará los datos en las listas despegables Eliminar Reporte y Eliminar Proyección
        }
        protected void CargarDDLEliminar() //Metodo para cargar las listas despegables de Eliminar Reporte y Eliminar Proyección
        {
            if (DDlEleminarResidencia.SelectedIndex != -1)
            {
                string contrato = DDlEleminarResidencia.SelectedItem.ToString();
                Int64 IdR = Convert.ToInt64(ObtenerIdResidencia(contrato));
                Boolean ExistenReportes = VerificarSiExistenReportes(IdR);
                if (ExistenReportes)
                {
                    CargarFechasDeReportes(IdR); //Aqui Mando a llamar al metodo para que muestre los reportes por fechas en la lista despegable de eliminar reporte
                    LbtnConfirmarR.Enabled = true; //Habilito el Botón Borrar Reporte
                    LbtnConfirmarP.Enabled = true; //Habilito el Botón Borrar Proyección
                }
                else if (DDlEliminarReporte.SelectedValue.ToString().CompareTo("Ninguno") != 0)
                {
                    DDlEliminarReporte.Items.Clear();
                    DDlEliminarReporte.Items.Add("Ninguno");
                    LbtnConfirmarR.Enabled = false;

                }
            }
            else if (DDlEleminarResidencia.SelectedValue.ToString().CompareTo("Ninguno") != 0 && DDlEliminarReporte.SelectedValue.ToString().CompareTo("Ninguno") != 0)
            {
                DDlEleminarResidencia.Items.Clear();
                DDlEleminarResidencia.Items.Add("Ninguno");
                DDlEliminarReporte.Items.Add("Ninguno");
                LbtnConfirmarR.Enabled = false;
                LbtnConfirmarP.Enabled = false;

            }
        }
        protected void CargarFechasDeReportes(Int64 idcontrato)  //Metodo para cargar las fechas de los reportes, segun sea el contrato seleccionado
        {
            string fecha, nmes, anio, Mes_Anio;
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String ObtenerResidencias = "Select FechaDeReporte From Reportes Where IdResidencia = @idcontrato Order BY FechaDeReporte";
                    OrdenSql = new SqlCommand(ObtenerResidencias, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idcontrato", idcontrato);
                    Leer = OrdenSql.ExecuteReader();
                    DDlEliminarReporte.Items.Clear(); //Limpio las listar despegables, para que no se vaya agregando los datos de la consulta
                    DdlMesProyecccion.Items.Clear();
                    DDlMesAP.Items.Clear();
                    while (Leer.Read())    //Aquí leo todas las fechas de los reportes agregados en la Tabla Reportes del contrato seleccionado
                    {
                        fecha = Leer[0].ToString();
                        DateTime Fecha = DateTime.Parse(fecha);
                        DateTime fechames = DateTime.Parse(fecha);          //Se crea un objeto de tipo DateTime para Mes
                        DateTime fechaanio = DateTime.Parse(fecha);         //Se crea un objeto de tipo DateTime para Año
                        nmes = Convert.ToString(fechames.Month);             //Obtengo solo el Mes de la fecha completa
                        anio = Convert.ToString(fechaanio.Year);            //Obtengo solo el Año de la fecha completa
                        Mes_Anio = DevuelveMes_Anio(nmes, anio);             //Metodo que devuelve El Mes-Anio, la Fecha del Reporte Leido
                        DDlEliminarReporte.Items.Add(Mes_Anio);
                        DdlMesProyecccion.Items.Add(Mes_Anio);
                        DDlMesAP.Items.Add(Mes_Anio);

                    }
                    con.Close();
                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }
        protected Boolean ExisteRepEnContrato(Int64 idcontrato)
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string ConsultarReporte = "Select 'true' From Reportes Where IdResidencia = @idcontrato";
                    OrdenSql = new SqlCommand(ConsultarReporte, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idcontrato", idcontrato);
                    ExistenReportes = Convert.ToBoolean(OrdenSql.ExecuteScalar());
                    con.Close();
                }
            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
            return ExistenReportes;
        }
       
        protected int ObtenerIdResidencia(String R) //METODO que recibe como parametro el nombre de la residencia y devuelve el Id de la residencia
        {
            
            int idarea = ObtenerIdArea();
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String ObtenerResidencias = "Select IdResidencia From Residencias Where Residencia =@nameresidencia AND IdArea = @IDarea";
                    OrdenSql = new SqlCommand(ObtenerResidencias, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@nameresidencia",R);
                    OrdenSql.Parameters.AddWithValue("@IDarea", idarea);
                    Idresidencia = Convert.ToUInt16(OrdenSql.ExecuteScalar());
                    con.Close();
                   
                }
                
            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
            return Idresidencia;
        }
        protected void Controles_LbtnGuardarR()    //Estos son los controles para el evento Guardar Reporte
        {
            PnlNuevo.Visible = false;
            LbtnReporteMensualN.Visible = true;
            LbtnAgregarContrato.Visible = true;
            LblContratosN.Visible = false;
            DdlContratosN.Visible = false;
            LblFechaReporte.Visible = false;
            TxtFechaReporte.Visible = false;
            FUpExaminar.Visible = false;
            LbtnImportar.Visible = false;
            LbtnHelp.Visible = false;
            LbtnGuardarR.Visible = false;
            LblbtnFormatoExcel.Visible = false;
            //habilitar botones y cajas de texto
            LbtnCancelEditionC.Enabled = true;
            LbtnNuevo.Enabled = true;
            LbtnEliminar.Enabled = true;
            LbtnActualizar.Enabled = true;
            LimpiarGridView();          //Limpio el GridView mostrada al usuario
            //Ocultar titulo del panel
            LblTitlePnlAgregarN.Visible = false;
        }
     
    }
}