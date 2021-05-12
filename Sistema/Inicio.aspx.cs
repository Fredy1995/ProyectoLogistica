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
using System.ComponentModel.DataAnnotations.Schema;
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
    public partial class Inicio : System.Web.UI.Page
    {
        string nombreUsuario, UltimaFechaReporte;
        private string strConexion = "server=DESKTOP-O8UKCDP\\SQLEXPRESS; database=LogisticaDB; uid=sa; pwd=1234;", NameArea;
        private int numConceptos;
        private SqlConnection con = null;
        private SqlCommand OrdenSql, OrdenSqlMesAgregado,OrdenSqlIdArea, OrdenSqlIdC, OrdenSqlFechaR, OrdenSqlDatosR,OrdenSqlArea;
        private SqlDataReader LeerIdArea,LeerIdC,LeerFR, LeerDatosPRV;
        Boolean ExistenReportes,band;
        private double   TotalCDIR = 0, TotalCDIRa = 0;
        private double  IOCR, AIngresoR, ACDR, AICR, AIOCR, ACFR, IOCRa, AIngresoRa, ACDRa, AICRa, AIOCRa, ACFRa; //Acumulado Ingreso Real area = AIngresoRa

        protected void Page_Load(object sender, EventArgs e)
        {
            AlertDanger.Visible = false;
            AlertSuccess.Visible = false;
            AlertWarning.Visible = false;
            if (!IsPostBack)
            {
                CargarTodo(); //Con este Metodo cargado Todo al entrar por primera vez, Carga las listas despegables, Mes Inicial , Mes Final , Año y Construye toda la Tabla Global
                LbtnConsolidado.Enabled = false;
            }
            try
            {
                nombreUsuario = Session["Usuario"].ToString();
                lblmensaje.Text = nombreUsuario;
                if (nombreUsuario == "Invitado")
                {
                    
                }
            }
            catch (Exception)
            {
                Response.Redirect("Default.aspx?id=1");
            }
        }
        protected void BtnCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Remove("Usuario");
            Response.Redirect("Default.aspx");
        }
        protected void LbtnSupervision_Click(object sender, EventArgs e)
        {
            Session["Usuario"] = nombreUsuario;
            Response.Redirect("Supervision.aspx");
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
        protected void CargarAnio() //Metodo para cargar los años que existen en los contratos de todas las areas
        {

            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String ObtenerAnios = "Select DISTINCT (DATEPART(yy, FechaDeReporte)) as Año From Reportes";
                    OrdenSql = new SqlCommand(ObtenerAnios, con);
                    con.Open();
                    DdlAnio.DataSource = OrdenSql.ExecuteReader();
                    DdlAnio.DataTextField = "Año";
                    DdlAnio.DataBind();
                    con.Close();

                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }
        protected Boolean ConsultarExistenReportes() //Metodo que se encarga de verificar si exiten Reportes agregados para todas las Areas
        {

            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string ConsultarReportes= "Select DISTINCT 'true' From Reportes";
                    OrdenSql = new SqlCommand(ConsultarReportes, con);
                    con.Open();
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

       
        protected void CargarTodo() //Metodo para cargar las listas despegables Fecha Inicio, Fecha Fin , Año y la Carga toda la Tabla Global
        { 
            if (ConsultarExistenReportes())
            {
                CargarFechasIncialFinal();  //Cargar Listas depegables Mes Inicial y Mes Final
                DdlMesFinal.SelectedValue = UltimoMesAgregado();
                CargarAnio();       //Cargar lista despegable para los Años existentes en la Tabla Reportes
                CargarTablaGlobal();
                LbtnBuscar.Enabled = true;
                LbtnGraficaGlobal.Enabled = true;
                LblTitleConsolidadoG.Visible = true;
                LblTitleResumenG.Visible = true;
            }
            else if (DdlMesInicial.SelectedValue.ToString().CompareTo("Ninguno") != 0 && DdlMesFinal.SelectedValue.ToString().CompareTo("Ninguno") != 0 && DdlAnio.SelectedValue.ToString().CompareTo("Ninguno") != 0)
            {
                DdlMesInicial.Items.Clear();
                DdlMesFinal.Items.Clear();
                DdlAnio.Items.Clear();
                DdlMesFinal.Items.Add("Ninguno");
                DdlMesInicial.Items.Add("Ninguno");
                DdlAnio.Items.Add("Ninguno");
                LbtnBuscar.Enabled = false;
                LbtnGraficaGlobal.Enabled = false;
                PnlVacio.Visible = true;
            }
        }
        //A Q U I   I N I C I A     L A     C O N S T R U C C I Ó N  D E  L A     T A B L A   G L O B A L     C O N S O L I D A D O
        protected void ConstruirTablaConsolidado(string MesI,string MesF,string Anio)
        {
            int nmesInicial = DevuelveNumMes(MesI), nmesFinal = DevuelveNumMes(MesF);
            string IdA;
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string ObtenerIdAreas = "Select IdArea From Areas";
                    OrdenSqlIdArea = new SqlCommand(ObtenerIdAreas, con);
                    con.Open();
                    LeerIdArea = OrdenSqlIdArea.ExecuteReader();
                    DataTable dtGlobal = new DataTable(); //Aqui creo un obejto de tipo DataTable para mostrar los datos en un GridView (Tabla de Resumen para el Consolidado anual de los contratos por AREA)
                    //Creo las columnas del DataTable TableAnual
                    dtGlobal.Columns.Add("Area");
                    dtGlobal.Columns.Add("MontoIngresoA");
                    dtGlobal.Columns.Add("CostoDirectoA");
                    dtGlobal.Columns.Add("Indirecto");
                    dtGlobal.Columns.Add("IndirectoOficina");
                    dtGlobal.Columns.Add("TotalCDI");
                    dtGlobal.Columns.Add("CFinanciamiento");
                    dtGlobal.Columns.Add("UtilidadContrato");
                    dtGlobal.Columns.Add("Utilidad");
                    DataRow Fila = dtGlobal.NewRow();
                    DataTable dtAreas = new DataTable(); //Aqui creo un obejto de tipo DataTable para mostrar los datos en un GridView (Tabla de Resumen para el Consolidado anual de los contratos por AREA)
                    //Creo las columnas del DataTable TableAnual
                    dtAreas.Columns.Add("Area");
                    dtAreas.Columns.Add("MontoIngresoA");
                    dtAreas.Columns.Add("CostoDirectoA");
                    dtAreas.Columns.Add("Indirecto");
                    dtAreas.Columns.Add("IndirectoOficina");
                    dtAreas.Columns.Add("TotalCDI");
                    dtAreas.Columns.Add("CFinanciamiento");
                    dtAreas.Columns.Add("UtilidadContrato");
                    dtAreas.Columns.Add("Utilidad");
                    while (LeerIdArea.Read()) //Leer los Id's de cada Area
                    {
                        DataRow FilaA = dtAreas.NewRow();
                        IdA = LeerIdArea[0].ToString();
                        LeerContratos(Convert.ToInt32(IdA),nmesInicial,nmesFinal,Convert.ToInt64(Anio), Fila,FilaA);
                        dtAreas.Rows.Add(FilaA);
                        //Aqui asigno "0" a estas variables para que se muestre el total de cada Área en la Tabla Áreas
                        AIngresoRa = 0;
                        IOCRa = 0;
                        ACDRa = 0;
                        AICRa = 0;
                        AIOCRa = 0;
                        ACFRa = 0;
                        TotalCDIRa = 0;
                        //**************************
                    }
                    LeerIdArea.Close();
                    con.Close();
                    dtGlobal.Rows.Add(Fila);
                    GVConsolidado.DataSource = dtGlobal;  //Aquí cargo los datos al GridView que mostrará el resumén anual para la Tabla General
                    GVConsolidado.DataBind();
                    PnlTableGlobal.Visible = true;
                    
                    GVAreas.DataSource = dtAreas;  //Aquí cargo los datos al GridView que mostrará el resumén anual para la Tabla Áreas
                    GVAreas.DataBind();
                    //Mostrar el Titulo de la Tabla General y Tabla Consolidado que pertenece a la Tabla Áreas
                    if (MesI != MesF)
                        {
                            LblTitleResumenC.Text = "Resumen " + MesI + " - " + MesF + " " + Anio + " ";
                            LblTitleResumenG.Text = "Resumen " + MesI + " - " + MesF + " " + Anio + " ";
                        }
                        else
                        {
                            LblTitleResumenC.Text = "Resumen " + MesI + " - " + Anio;
                            LblTitleResumenG.Text = "Resumen " + MesI + " - " + Anio;
                        }

                    
                }
            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }
     
        protected void LeerContratos(int IdA,int numMesI, int numMesF,Int64 Anio,DataRow Fila,DataRow FilaA) //Metodo que se encarga de Obtener los Id's de cada contrato
        {
            string IdC;
           
            FilaA[0] = ObtenerArea(IdA);
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string ObtenerIdContratos = "Select IdResidencia From Residencias where IdArea = @idarea";
                    OrdenSqlIdC = new SqlCommand(ObtenerIdContratos, con);
                    con.Open();
                    OrdenSqlIdC.Parameters.AddWithValue("@idarea", IdA);
                    LeerIdC = OrdenSqlIdC.ExecuteReader();
                    while (LeerIdC.Read()) //Aqui leo los Id's de cada Contrato que hay en el Area seleccionada
                    {
                        IdC = LeerIdC[0].ToString();
                        ObtenerFechasDeReportes(Convert.ToInt32(IdC),numMesI,numMesF,Anio,Fila,FilaA);
                    }
                }
            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }

        }
        protected void ObtenerFechasDeReportes(int IdC,int numMesI,int numMesF,Int64 Anios,DataRow Fila,DataRow FilaA) //Metodo para obtener las fechas de cada REPORTE, por Año y De Mes Inicio a Mes Fin
        {
            string fecha,nmes,anio,FechaR;
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat; //Creo Objeto para personalizar el símbolo de moneda
            nfi.CurrencyPositivePattern = 2; //Establesco la posicion del simbolo de moneda
            
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string FechaReportes = "Select FechaDeReporte From Reportes Where IdResidencia = @idcontrato AND FechaDeReporte LIKE @anio ORDER BY FechaDeReporte";
                    OrdenSqlFechaR = new SqlCommand(FechaReportes, con);
                    con.Open();
                    OrdenSqlFechaR.Parameters.AddWithValue("@idcontrato", IdC);
                    OrdenSqlFechaR.Parameters.AddWithValue("@anio", Anios + "%");
                    LeerFR = OrdenSqlFechaR.ExecuteReader();

                    while (LeerFR.Read()) //Aquí leo todas las fechas de los reportes agregados en la Tabla Reportes del contrato Recibido como parametro
                    {
                        fecha = LeerFR[0].ToString();
                        DateTime Fecha = DateTime.Parse(fecha);
                        DateTime fechames = DateTime.Parse(fecha);          //Se crea un objeto de tipo DateTime para Mes
                        DateTime fechaanio = DateTime.Parse(fecha);         //Se crea un objeto de tipo DateTime para Año
                        nmes = Convert.ToString(fechames.Month);             //Obtengo solo el Mes de la fecha completa
                        anio = Convert.ToString(fechaanio.Year);            //Obtengo solo el Año de la fecha completa

                        if(numMesI <= numMesF && Convert.ToInt64(anio) == Anios) //Pregunto Si Mes Inicial es <= Mes final Y anio de los reportes es == a Anios seleccionado en la lista despegable
                        {
                            //Solo entra cuando se seleccionan fechas del Mismo Año
                            if (Convert.ToInt16(nmes) >= numMesI)  //Pregunto Si el mes existente en los reportes es >= a Mes Inicial seleccionado
                               {
                                    if (Convert.ToInt16(nmes) <= numMesF) //Pregunto si el mes existente en los reportes es <= al Mes Final seleccionado
                                    {
                                          FechaR = Convert.ToString(Fecha.ToShortDateString()); //Guardo la Fecha del Reporte Mensual a procesar
                                          LlenarFilasTablaConsolidado(IdC, FechaR,Fila,FilaA);
                                          band = true;
                                    }
                                     if(!band) //Si no existe reporte entre el mes inicial y mes final seleccionado, entonces pongo a Ceros todo la Tabla Consolidado
                                    {
                                    //Si el Mes recuperado de la Tabla Reportes no es <= al Mes Final Seleccionado entonces no hay reportes agregados que correspondan a la fecha seleccionada
                                    //Agrego al GridView Ceros
                                    Fila[0] = "$ 0.0";
                                    Fila[1] = "$ 0.0";
                                    Fila[2] = "$ 0.0";
                                    Fila[3] = "$ 0.0";
                                    Fila[4] = "$ 0.0";
                                    Fila[5] = "$ 0.0";
                                    Fila[6] = "$ 0.0";
                                    Fila[7] = "$ 0.0";
                                    Fila[8] = "0%";
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

        protected void LlenarFilasTablaConsolidado(int IdContrato, string FechaR, DataRow Fila,DataRow FilaA)
        {
            int fila = 0, nconceptos = ObtenerNumConceptos(IdContrato);
            Int64 idreporte;
            double presupuesto, real, variacion, UPC,UPCa, UtilidadMensual, aux;
            string MontoIngresoA, CostoDirectoA, Indirecto, IndirectoOficina, TotalCDI, CFinanciamiento, UtilidadContrato, Utilidad,MIA,CDA,Ind,IO,TCDI,CF,UC ;
            Fila[0] = "Consolidado General";
           
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
                                AIngresoRa += real;
                                MIA = string.Format(nfi, "{0:C}", AIngresoRa);
                                FilaA[1] = MIA;
                            }
                            if (fila >= nconceptos - 7) //Si existe el reporte y a de más la fila es mayor o igual al concepto Total Costo directo, entra en la condición{
                            {
                                if (fila == nconceptos - 7) //Total Costo Directo REAL
                                {
                                    ACDR += real;
                                    CostoDirectoA = string.Format(nfi, "{0:C}", ACDR);
                                    Fila[2] = CostoDirectoA;
                                    ACDRa += real;
                                    CDA = string.Format(nfi, "{0:C}", ACDRa);
                                    FilaA[2] = CDA;
                                }
                                else if (fila == nconceptos - 6) //Indirecto de campo REAL
                                {
                                    AICR += real;
                                    Indirecto = string.Format(nfi, "{0:C}", AICR);
                                    Fila[3] = Indirecto;
                                    AICRa += real;
                                    Ind = string.Format(nfi, "{0:C}", AICRa);
                                    FilaA[3] = Ind;
                                }
                                else if (fila == nconceptos - 5) //Indirecto de Oficina Central REAL
                                {
                                    AIOCR += real;
                                    IndirectoOficina = string.Format(nfi, "{0:C}", AIOCR);
                                    Fila[4] = IndirectoOficina;
                                    AIOCRa += real;
                                    IO = string.Format(nfi, "{0:C}", AIOCRa);
                                    FilaA[4] = IO;
                                    TotalCDIR = ACDR + AICR + AIOCR;
                                    TotalCDI = string.Format(nfi, "{0:C}", TotalCDIR);
                                    Fila[5] = TotalCDI;
                                    TotalCDIRa = ACDRa + AICRa + AIOCRa;
                                    TCDI = string.Format(nfi, "{0:C}", TotalCDIRa);
                                    FilaA[5] = TCDI;
                                }
                                else if (fila == nconceptos - 1) // Costo del Financiamiento REAL
                                {
                                    ACFR += real;
                                    CFinanciamiento = string.Format(nfi, "{0:C}", ACFR);
                                    Fila[6] = CFinanciamiento;
                                    ACFRa += real;
                                    CF = string.Format(nfi, "{0:C}", ACFRa);
                                    FilaA[6] = CF;
                                    UPC = AIngresoR - TotalCDIR - ACFR;
                                    UPCa = AIngresoRa - TotalCDIRa - ACFRa;
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
                                    if (UPCa < 0)
                                    {
                                        aux = UPCa * -1;
                                        aux = Convert.ToDouble(string.Format("{0:F2}", aux));
                                        UC = "-" + string.Format(nfi, "{0:C}", aux);
                                        FilaA[7] = UC;
                                    }
                                    else
                                    {
                                        aux = Convert.ToDouble(string.Format("{0:F2}", UPC));
                                        UC = string.Format(nfi, "{0:C}", aux);
                                        FilaA[7] = UC;

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
                                    if (AIngresoRa != 0)
                                    {
                                        UtilidadMensual = Math.Round((UPCa / AIngresoRa) * 100); //Aquí se obtiene el porcentaje de utilidad mensual (Real %)
                                        Utilidad = Convert.ToString(UtilidadMensual) + "%";
                                        FilaA[8] = Utilidad;
                                    }
                                    else
                                    {
                                        UtilidadMensual = 0;
                                        Utilidad = Convert.ToString(UtilidadMensual) + "%";
                                        FilaA[8] = Utilidad;
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
        protected int ObtenerNumConceptos(int IdR) //Metodo para obtener el número de conceptos en total que hay en cada contrato, recibe Id de residencia (contrato)
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string querynConceptos = "Select COUNT(*) FROM Concepto Where IdResidencia = @idresidencia";
                    OrdenSql = new SqlCommand(querynConceptos, con);
                    con.Open();
                    OrdenSql.Parameters.AddWithValue("@idresidencia", IdR);
                    numConceptos = Convert.ToInt32(OrdenSql.ExecuteScalar());
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
        //A Q U I   F I N A L I Z A     L A     C O N S T R U C C I Ó N  D E  L A     T A B L A   G L O B A L     C O N S O L I D A D O
        protected void LbtnBuscar_Click(object sender, EventArgs e) //Evento del Botón buscar, para construir la Tabla Global Consolidado
        {
            CargarTablaGlobal();
        }
        protected void CargarTablaGlobal() //Metodo para construir la Tabla Consolidado Global
        {
            ConstruirTablaConsolidado(DdlMesInicial.SelectedItem.ToString(), DdlMesFinal.SelectedItem.ToString(), DdlAnio.SelectedItem.ToString());
        }

        protected void CargarFechasIncialFinal() //Metodo para cargar las fechas de los reportes en las listas depegables Fecha Inicial y Fecha Final
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String ObtenerMes = ";WITH meses AS (SELECT datename (month,FechaDeReporte) AS NameMes, MIN(FechaDeReporte) AS Mes FROM Reportes GROUP BY datename (month,FechaDeReporte)) SELECT NameMes FROM meses ORDER BY MONTH(Mes)";
                    OrdenSql = new SqlCommand(ObtenerMes, con);
                    con.Open();
                    DdlMesInicial.DataSource = OrdenSql.ExecuteReader();
                    DdlMesInicial.DataTextField = "NameMes";
                    DdlMesInicial.DataBind();
                    con.Close();
                    con.Open();
                    DdlMesFinal.DataSource = OrdenSql.ExecuteReader();
                    DdlMesFinal.DataTextField = "NameMes";
                    DdlMesFinal.DataBind();
                    con.Close();
                }

            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
        }
        protected string UltimoMesAgregado() //Metodo que obtiene el Ultimo mes agregado en todos los reportes que exiten en todas las areas
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    String ObtenerMes = ";WITH meses AS (SELECT datename (month,FechaDeReporte) AS NameMes, MIN(FechaDeReporte) AS Mes FROM Reportes GROUP BY datename (month,FechaDeReporte)) SELECT NameMes FROM meses ORDER BY MONTH(Mes) DESC";
                    OrdenSqlMesAgregado = new SqlCommand(ObtenerMes, con);
                    con.Open();
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
        protected int DevuelveNumMes(string mes) //Metodo que se le pasa como parametro (Mes-Año) y devuelve el numero del Mes correspondiente
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

        protected string ObtenerArea(int IdA)
        {
            try
            {
                con = new SqlConnection(strConexion);
                using (con)
                {
                    string ObtenerNameArea = "Select Area From Areas Where IdArea = @idarea";
                    OrdenSqlArea = new SqlCommand(ObtenerNameArea, con);
                    con.Open();
                    OrdenSqlArea.Parameters.AddWithValue("@idarea", IdA);
                    NameArea = Convert.ToString(OrdenSqlArea.ExecuteScalar());
                    con.Close();
                    if (String.Equals(NameArea, "Supervision"))
                    {
                        NameArea = "Supervisión de la Construcción de Obras de Infraestructuras de Vías Terrestres";
                    }
                    else if(String.Equals(NameArea, "Proyectos"))
                    {
                        NameArea = "Proyectos de Infraestructura de Vías Terrestres";
                    }else if (String.Equals(NameArea, "Ambiental"))
                    {
                        NameArea = "Elaboración de Estudios de Impacto Ambiental y Ejecución de Programas Ambientales";
                    }
                    else 
                    {
                        NameArea = "Construcción";
                    }


                }
            }
            catch (Exception ex)
            {

                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Informe: " + ex.Message; ;
            }
            return NameArea;
        }
    }
}