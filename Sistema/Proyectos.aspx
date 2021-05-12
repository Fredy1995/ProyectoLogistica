<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Proyectos.aspx.cs" Inherits="Sistema.Proyectos" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
   <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1"/>
    <title>Supervisión</title>
	<link rel="stylesheet" type="text/css" href="boostrap/bootstrap.min.css"/>
    <script type="text/javascript" src="js/jquery-3.5.1.min.js"></script>
	<script type="text/javascript" src="js/Script.js"></script>
	<script type="text/javascript" src="js/StyleTableArea.js"></script>
	<script type="text/javascript" src="js/StyleTableAnual.js"></script>
	<script type="text/javascript" src="js/StyleTableReportes.js"></script>
	<script type="text/javascript" src="js/StyleFooterTable.js"></script>
	<link  rel="stylesheet" type="text/css" href="css/Estilos.css"/>
	<link rel="stylesheet" type="text/css" href="css/Responsive.css"/>
	<link rel="stylesheet" type="text/css" href="css/Iconos/demo.css"/>
	<script type="text/javascript" src="js/bootstrap.min.js"></script>
	<script type="text/javascript" src="https://www.gstatic.com/charts/loader.js"></script>
	<script src="https://unpkg.com/sweetalert/dist/sweetalert.min.js"></script>
</head>
<body>
	<style type="text/css">
       #GvDatosReportes thead,tbody tr{
            display:table;
            width:100%;
            table-layout:fixed;
          } 
		 #GvDatosReportes tbody th{
            display:table;
            position:sticky;
          }
	    
         #GvDatosReportes thead{
              width:98.90%;
			  font-size:12px;
			  text-align:center;
			  overflow-wrap:break-word;
          }
		 
		 #GvDatosReportes table{
			  width:100%;
		  }
          #GvDatosReportes tbody{
              display:block;
              height:450px;
              overflow-y:auto;
              overflow-x:hidden;
          }
		 #GvDatosReportes.table tr td{
			  width:150px;
			  text-align:right;
		  }
         #GvDatosReportes.table thead tr th{
              width:150px;
          }
		 #GvDatosReportes.table thead tr th:nth-child(2){
              width:350px;
          }
		 #GvDatosReportes.table tr td:nth-child(2){
			  width:350px;
		  }

         #GvDatosReportes thead{
              background-color:silver;
          } 
		
		 /***********************/
		 #GVContratos {
			 font-size:14px;
			 width:123.99px;
			 text-align:center;
		 }
		 #GVContratos.table tr td{
			  width:123.99px;
			  font-size:12px;
			  text-align:right;
			  overflow-wrap:break-word;
		  }	
		 /************************/
		  #GVTableAnual {
			 font-size:14px;
			 width:123.99px;
			 text-align:center;
		 }
		 #GVTableAnual.table tr td{
			  width:123.99px;
			  font-size:12px;
			  text-align:right;
			  overflow-wrap:break-word;
		  }
    </style>
    <form id="form1" runat="server">
		<asp:ScriptManager ID="ScriptManager1" runat="server">
		</asp:ScriptManager>
       <header>
			<section id="logo">
           <h3>LOGISTICA Y FINANZAS</h3>
           <span id="abrir_Menu" class="icon icon-menu"></span>
		   </section>
			<section id="menu">
				<div><span id="cerrar_Menu" class="icon icon-cross"></span></div><hr />
					<ul>
						<asp:LinkButton ID="LbtnConsolidado" runat="server" OnClick="LbtnConsolidado_Click" ><span class="icon icon-stack"></span> Consolidado</asp:LinkButton>
                        <asp:LinkButton ID="LbtnSupervision" runat="server" OnClick="LbtnSupervision_Click"><span class="icon icon-quill"></span> Supervisión</asp:LinkButton>
                        <asp:LinkButton ID="LbtnProyectos" runat="server"><span class="icon icon-books"> Proyectos</span></asp:LinkButton>
                        <asp:LinkButton ID="LbtnAmbiental" runat="server" OnClick="LbtnAmbiental_Click"><span class="icon icon-leaf"></span> Ambiental</asp:LinkButton>
                        <asp:LinkButton ID="LbtnContruccion" runat="server" OnClick="LbtnContruccion_Click"><span class="icon icon-office"></span> Construcción</asp:LinkButton>
                        <asp:LinkButton ID="LbtnSalir" runat="server" OnClick="LbtnSalir_Click" ><span class="icon icon-exit"></span> Salir</asp:LinkButton>
                        <p><span class="icon icon-user"></span> <asp:Label ID="lblmensaje" runat="server" Text=""></asp:Label></p>
					</ul>
		    </section>
		</header>

        <div class="container">
            <div class="row">
                <div class="col-md-12 col-xs-12" >
						<h4 runat="server" visible="true" id="TitleS" class="card-header">HISTORIAL DE RENTABILIDAD-<strong>PROYECTOS</strong></h4>
						<h4 runat="server" visible="false" id="TitleSC" class="card-header">PROYECTOS - <strong>CONTRATOS</strong></h4>
					    <h4 runat="server" visible="false" id="TitleCAnual" class="card-header">CONSOLIDADO ANUAL - <strong>PROYECTOS</strong></h4>
                </div>
            </div>
		</div><br />

        <div class="container"> 
            <div class="row">
					<div runat="server" id="MenuGeneral" visible="true">
					  <div class="container">
						<div class="form-inline">
							   <div class="input-group">
		    					<asp:LinkButton ID="LbtnBuscar" runat="server" CssClass="btn btn-outline-dark mb-2 mr-sm-2" OnClick="LbtnBuscar_Click"><span class="icon icon-search"></span> Buscar:</asp:LinkButton>    
							    <asp:LinkButton ID="LbtnBuscarAnual" runat="server" CssClass="btn btn-outline-dark mb-2 mr-sm-2" Visible="false" OnClick="LbtnBuscarAnual_Click"><span class="icon icon-search"></span> Buscar:</asp:LinkButton>
								<asp:DropDownList ID="DdlContratoMG" runat="server" CssClass="form-control mb-2 mr-sm-2"  AutoPostBack="True" OnSelectedIndexChanged="DdlContratoMG_SelectedIndexChanged" ToolTip="Seleccione un contrato"></asp:DropDownList>	
								</div>
							<asp:Label ID="LblDeAnual" runat="server" Text="De:" CssClass="mb-1 mr-sm-2" Visible =" false"></asp:Label>
                            <div class="input-group">
		     					<asp:DropDownList ID="DdlMesIAnual" runat="server" CssClass="form-control mb-2 mr-sm-2" ToolTip="Seleccione un mes inicial" Visible =" false"></asp:DropDownList>
						    </div>
							<asp:Label ID="LblaAnual" runat="server" Text="a" CssClass="mb-1 mr-sm-2" Visible =" false"></asp:Label>
							<div class="input-group">
		     					<asp:DropDownList ID="DdlMesFAnual" runat="server" CssClass="form-control mb-2 mr-sm-2" ToolTip="Seleccione un mes final" Visible =" false"></asp:DropDownList> 
							</div>
							<asp:Label ID="lblDe" runat="server" Text="De:" CssClass="mb-1 mr-sm-2"></asp:Label>
							<asp:Label ID="LblDdlAnual" runat="server" Text="Año:" Visible="false" CssClass="mb-1 mr-sm-2"></asp:Label>
								<div class="input-group">
		     					<asp:DropDownList ID="DdlMesInicial" runat="server" CssClass="form-control mb-2 mr-sm-2" ToolTip="Seleccione un mes inicial"></asp:DropDownList>
								<asp:DropDownList ID="DdlAnual" runat="server" CssClass="form-control mb-2 mr-sm-2"  ToolTip="Seleccione el año" Visible="false"></asp:DropDownList>
			
								</div>
							 <asp:Label ID="lblA" runat="server" Text="a" CssClass="mb-1 mr-sm-2"></asp:Label>
								<div class="input-group">
		     					<asp:DropDownList ID="DdlMesFinal" runat="server" CssClass="form-control mb-2 mr-sm-2" ToolTip="Seleccione un mes final"></asp:DropDownList> 
								</div>
					           
								<div class="input-group">
								<asp:LinkButton ID="LbtnGraficaArea" runat="server" CssClass="btn btn-outline-success mb-2 mr-sm-2" Visible ="true" OnClick="LbtnGraficaArea_Click" ><span class="icon icon-stats-dots"></span> Grafica</asp:LinkButton>
								<asp:LinkButton ID="LbtnAnual" runat="server" CssClass="btn btn-outline-info mb-2 mr-sm-2" OnClick="LbtnAnual_Click"><span class="icon icon-table"></span> Anual</asp:LinkButton>
                                <asp:LinkButton ID="LbtnContratos" runat="server" CssClass="btn btn-outline-info mb-2 mr-sm-2" OnClick="LbtnContratos_Click"><span class="icon icon-table"></span> Contratos</asp:LinkButton>  
								</div>
						</div>
					   </div>
					 </div> 
		       
		
					<div runat="server" id="MenuContratos" visible="false">
						<div class="container">
						<div class="form-inline">
						<div class="input-group">
							
							<asp:LinkButton ID="LbtnBuscarC" runat="server" CssClass="btn btn-outline-dark mb-2 mr-sm-2" OnClick="LbtnBuscarC_Click"><span class="icon icon-search"></span> Buscar:</asp:LinkButton>
							<asp:DropDownList ID="DdlContratos" runat="server" CssClass="form-control mb-2 mr-sm-2" ToolTip="Seleccione un contrato"></asp:DropDownList>
						</div>
						<div class="input-group">
                            <asp:DropDownList ID="DdlMesC" runat="server" CssClass="form-control mb-2 mr-sm-2"  Visible="false">
									 <asp:ListItem Value="-1">Seleccione el Mes:</asp:ListItem>
		     					</asp:DropDownList>
							<asp:DropDownList ID="DdlAnioC" runat="server" CssClass="form-control mb-2 mr-sm-2"  Visible="false" >
									 <asp:ListItem Value="-1">Seleccione el Año:</asp:ListItem>
                                     <asp:ListItem Value="1">2019</asp:ListItem>
		     					</asp:DropDownList> 
						</div>
						<div class="input-group">
							<asp:LinkButton ID="LbtnRegresarGeneral" runat="server" CssClass="btn btn-outline-secondary mb-2 mr-sm-2" OnClick="LbtnRegresarGeneral_Click"><span class="icon icon-table"></span> General</asp:LinkButton>
							<asp:LinkButton ID="LbtnGraficaC" runat="server" CssClass="btn btn-outline-success mb-2 mr-sm-2" OnClick="LbtnGraficaC_Click"><span class="icon icon-stats-dots"></span> Grafica</asp:LinkButton>
						</div>
						<div class="input-group">
								<asp:LinkButton ID="LbtnEditarC" runat="server" CssClass="btn btn-outline-info mb-2 mr-sm-2" OnClick="LbtnEditarC_Click"><span class="icon icon-pencil2"></span> Editar Tabla</asp:LinkButton>
								<asp:LinkButton ID="LbtnCambiarPorcentaje" runat="server" CssClass="btn btn-outline-success mb-2" OnClick="LbtnCambiarPorcentaje_Click"><span class="icon icon-pencil2"></span> Editar %</asp:LinkButton>
						</div>
					 </div>
					</div>
					</div><!--Fin Menu Contratos-->
            </div>
			<div class="container">
			<div class="row">
				<div class="col-md-12 text-right">
					<asp:LinkButton ID="LbtnCancelEditionC" runat="server" CssClass="btn btn-danger" Visible="false" OnClick="LbtnCancelEditionC_Click"><span class="icon icon-cross"></span> Cerrar</asp:LinkButton>
				</div>
			</div>
			</div>
        </div><!--Fin de Container--><br />

		<div class="container">
           
			<asp:Panel ID="PnlTable" runat="server" Visible="false">
					<div class="text-left"> <h5><strong><asp:Label ID="LblTitleRentabilidad" runat="server" Text="Rentabilidad de los contratos" Visible="false"></asp:Label></strong></h5></div>
					 <div class="text-left"><asp:Label ID="LblTitleResumenC" runat="server"></asp:Label></div>
				 <div class="row">
				   <div class="col-md-12 col-xs-12">
					<asp:Panel ID="Panel1" runat="server">
						<div class="table-responsive">
								<asp:GridView ID="GVContratos" runat="server" AutoGenerateColumns="false"  CellPadding="3" BackColor="White" BorderColor="#00003e" BorderStyle="None" BorderWidth="1px" CssClass="table table-hover">
										<Columns>
                                            <asp:BoundField DataField="Contrato" HeaderText="Contrato" />
                                            <asp:BoundField DataField="MontoIngresoA" HeaderText="Monto ingreso acumulado" />
                                            <asp:BoundField DataField="CostoDirectoA" HeaderText="Costo Directo acumulado" />
											<asp:BoundField DataField="Indirecto" HeaderText="Indirecto" />
											<asp:BoundField DataField="IndirectoOficina" HeaderText="Indirecto de Oficina Central" />
											<asp:BoundField DataField="TotalCDI" HeaderText="Total (Costo Directo + Indirecto)" />
											<asp:BoundField DataField="CFinanciamiento" HeaderText="Costo del Financiamiento" />
											<asp:BoundField DataField="UtilidadContrato" HeaderText="Utilidad del contrato" />
											<asp:BoundField DataField="Utilidad" HeaderText="% Utilidad" />
                                        </Columns>
										<FooterStyle BackColor="White" ForeColor="#000066" />
										<HeaderStyle BackColor="#666666" Font-Bold="False" ForeColor="#00003e" Wrap="true"  />
										<PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
										<RowStyle ForeColor="#000066" />
										<SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White"/>
										<SortedAscendingCellStyle BackColor="#F1F1F1" />
										<SortedAscendingHeaderStyle BackColor="#007DBB" />
										<SortedDescendingCellStyle BackColor="#CAC9C9" />
										<SortedDescendingHeaderStyle BackColor="#00547E" />
							</asp:GridView>
						</div>
					</asp:Panel>
				  </div>
			    </div>
		        </asp:Panel>
			 <asp:Panel ID="PnlVacio" runat="server" visible="false">
				<br /><br />
				<div class="row">
					<div class="col-md-12 text-center">
					<h3 runat="server" class="card-body"><strong>:( No hay contenido en esta vista.</strong></h3>
					</div>
				</div>
            </asp:Panel>
           
            <asp:Panel ID="PnlGraficaC" runat="server" Visible="false">
			<div class="row">
                <div class="col-md-12 col-xs-12" >
					<div class="row">
					<div class="col-md-3 col-1"></div>
					<div class="col-md-6 col-9 text-center">
                        <asp:Label ID="LblTitleG" runat="server" Text="" CssClass="SizeFontTitleG" Visible="false"></asp:Label>
					</div>
					<div class="col-md-3 col-1 text-right">
						<asp:LinkButton ID="LbtnCerrarGraficaC" runat="server" CssClass="btn btn-danger" OnClick="LbtnCerrarGraficaC_Click"><span class="icon icon-cross"></span></asp:LinkButton>
						<asp:LinkButton ID="LbtnCerrarGraficaG" runat="server" CssClass="btn btn-danger" Visible ="false" OnClick="LbtnCerrarGraficaG_Click"><span class="icon icon-cross"></span></asp:LinkButton>
					</div>
					</div>
					<div class="row">
						<div class="table-responsive"> 
						<div id="chart_div" style=" width:1500px; height: 500px;"></div>
				        </div>
					</div>
                </div>
            </div>
            </asp:Panel>
            			 
			<asp:Panel ID="PnlTableAnual" runat="server" Visible="false">
				<div class="text-right"><asp:LinkButton ID="LbtnCerrarAnual" runat="server" CssClass="btn btn-danger" OnClick="LbtnCerrarAnual_Click"><span class="icon icon-cross"></span></asp:LinkButton></div>
				<div class="text-left"> <h5><strong><asp:Label ID="LblTitleAnual" runat="server" Text="Consolidado general por área"></asp:Label></strong></h5></div>
				 <div class="text-left"><asp:Label ID="LblTitleResumenAnual" runat="server"></asp:Label></div>
				 <div class="row">
				   <div class="col-md-12 col-xs-12">
					<asp:Panel ID="Panel2" runat="server">
						<div class="table-responsive">
								<asp:GridView ID="GVTableAnual" runat="server" AutoGenerateColumns="false"  CellPadding="3" BackColor="White" BorderColor="#00003e" BorderStyle="None" BorderWidth="1px" CssClass="table table-hover">
										<Columns>
                                            <asp:BoundField DataField="Area" HeaderText="Área" />
                                            <asp:BoundField DataField="MontoIngresoA" HeaderText="Monto ingreso acumulado" />
                                            <asp:BoundField DataField="CostoDirectoA" HeaderText="Costo Directo acumulado" />
											<asp:BoundField DataField="Indirecto" HeaderText="Indirecto" />
											<asp:BoundField DataField="IndirectoOficina" HeaderText="Indirecto de Oficina Central" />
											<asp:BoundField DataField="TotalCDI" HeaderText="Total (Costo Directo + Indirecto)" />
											<asp:BoundField DataField="CFinanciamiento" HeaderText="Costo del Financiamiento" />
											<asp:BoundField DataField="UtilidadContrato" HeaderText="Utilidad del contrato" />
											<asp:BoundField DataField="Utilidad" HeaderText="% Utilidad" />
                                        </Columns>
										<FooterStyle BackColor="White" ForeColor="#000066" />
										<HeaderStyle BackColor="#666666" Font-Bold="False" ForeColor="#00003e" Wrap="true"  />
										<PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
										<RowStyle ForeColor="#000066" />
										<SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White"/>
										<SortedAscendingCellStyle BackColor="#F1F1F1" />
										<SortedAscendingHeaderStyle BackColor="#007DBB" />
										<SortedDescendingCellStyle BackColor="#CAC9C9" />
										<SortedDescendingHeaderStyle BackColor="#00547E" />
							</asp:GridView>
						</div>
					</asp:Panel>
				  </div>
			    </div>
            </asp:Panel>
		
			<asp:Panel ID="PnlCPorcentaje" runat="server" Visible="false" CssClass="BordeTitleTable mb-3">
				<p class="text-muted" style="text-align: left; font-size:16px;">Ingrese el <strong>porcentaje (%)</strong> en los siguientes campos:</p>
                <div class="form-inline was-validated">
                <asp:Label ID="LblPorcentajeIC" runat="server" Text="Indirecto de Campo:" CssClass="mb-2 mr-sm-2 text-dark"></asp:Label>
                <asp:TextBox ID="TxtPorcentajeIC" runat="server" placeholder="00.00 %" CssClass="form-control mb-2 mr-sm-2" required=""></asp:TextBox>
                <asp:Label ID="LblPorcentajeIO" runat="server" Text="Indirecto de Oficina Central:" CssClass="mb-2 mr-sm-2 text-dark"></asp:Label>
                <asp:TextBox ID="TxtPorcentajeIO" runat="server" placeholder="00.00 %" CssClass="form-control mb-2 mr-sm-2" required=""></asp:TextBox>
                <asp:LinkButton ID="LbtnActualizarP" runat="server" CssClass="btn btn-outline-info mb-2 mr-sm-2" OnClick="LbtnActualizarP_Click"><span class="icon icon-checkmark"></span> Aplicar</asp:LinkButton>
				<asp:LinkButton ID="LbtnCancelarPnlP" runat="server" CssClass="btn btn-danger mb-2" OnClick="LbtnCancelarPnlP_Click"><span class="icon icon-cross"></span> Cancelar</asp:LinkButton>
				</div>
			</asp:Panel>
			
			 <asp:Panel ID="PnlTableC" runat="server" Visible="false">
                 <input class="form-control mb-3" runat="server" id="myInput" visible="false" onkeyup="myFunctionFiltrar()" type="text" name="search"  placeholder="Buscar por concepto..." autofocus="autofocus" />
				<div class="BordeTitleTable" id="HeaderTable" runat="server" visible="false">
							<div class="row"><div class="col-md-6 col-xs-6"></div><div class="col-md-6 col-xs-6 text-right fuenteDatos"><asp:Label ID="Label1" runat="server" Text="Fecha: " CssClass="text-muted"></asp:Label><asp:Label ID="LblHeaderFecha" runat="server" Text=""></asp:Label></div></div>
							<div class="row"><div class="col-md-6 col-xs-6"></div><div class="col-md-6 col-xs-6 text-right fuenteDatos"><asp:Label ID="Label2" runat="server" Text="Monto Contratado: " CssClass="text-muted"></asp:Label><asp:Label ID="LblHeaderMonto" runat="server" Text=""></asp:Label></div></div>
							<div class="row"><div class="col-md-6 col-6  fuenteTitle text-muted text-info"><asp:Label ID="LblHeaderResidencia" runat="server" Text=""></asp:Label></div><div class="col-md-6 col-6 text-right fuenteDatos"><asp:Label ID="Label5" runat="server" Text="Fecha de Inicio: " CssClass="text-muted"></asp:Label><asp:Label ID="LblHeaderFechaInicio" runat="server" Text=""></asp:Label></div></div>
							<div class="row"><div class="col-md-6 col-xs-6"></div><div class="col-md-6 col-xs-6 text-right fuenteDatos"><asp:Label ID="Label4" runat="server" Text="Fecha de Terminación: " CssClass="text-muted"></asp:Label><asp:Label ID="LblHeaderFechaTermino" runat="server" Text=""></asp:Label></div></div>    
			    </div>
			 <div class="row">
				<div class="col-md-12 col-xs-12">
                    <asp:Panel ID="PnlDatosReportes" runat="server">
						<div class="table-responsive">
						<asp:GridView ID="GvDatosReportes" runat="server"  CellPadding="3" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CssClass="table table-hover">
						<FooterStyle BackColor="White" ForeColor="#000066" />
                        <HeaderStyle BackColor="#000028" Font-Bold="True" ForeColor="White" Wrap="true" />
                        <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                        <RowStyle ForeColor="#000066" />
                        <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White"/>
                        <SortedAscendingCellStyle BackColor="#F1F1F1" />
                        <SortedAscendingHeaderStyle BackColor="#007DBB" />
                        <SortedDescendingCellStyle BackColor="#CAC9C9" />
                        <SortedDescendingHeaderStyle BackColor="#00547E" />
                        </asp:GridView>
					   </div>
					</asp:Panel>
				</div>
			</div>
				 <div class="BordeTitleTable" id="FooterTable" runat="server" visible="false">
				  <div class="row"><div class="col-md-4 col-2"></div><div class="col-md-4 col-2"></div><div class="col-md-2 col-3 text-muted text-info text-center fuenteDatos"><asp:Label ID="Label10" runat="server" Text="PRESUPUESTO"></asp:Label></div><div class="col-md-2 col-2 text-muted text-info text-center fuenteDatos"><asp:Label ID="Label11" runat="server" Text="REAL"></asp:Label></div></div>
				  <div class="row"><div class="col-md-4 "></div><div class="col-md-4 col-4 text-muted text-right fuenteDatos"><asp:Label ID="Label12" runat="server" Text="Costo Directo:"></asp:Label></div><div class="col-md-2 col-3 text-right colorResultadosP"><asp:Label ID="LblFooterCDP" runat="server" Text=""></asp:Label></div><div class="col-md-2 col-2 text-right colorResultados"><asp:Label ID="LblFooterCDR" runat="server" Text=""></asp:Label></div></div>		
				  <div class="row"><div class="col-md-4 "></div><div class="col-md-4 col-4 text-muted text-right fuenteDatos"><asp:Label ID="Label15" runat="server" Text="Indirecto de Campo:"></asp:Label></div><div class="col-md-2 col-3 text-right colorResultadosP"><asp:Label ID="LblFooterIP" runat="server" Text=""></asp:Label></div><div class="col-md-2 col-2 text-right colorResultados"><asp:Label ID="LblFooterIR" runat="server" Text=""></asp:Label></div></div>
				  <div class="row"><div class="col-md-4 "></div><div class="col-md-4 col-4 text-muted text-right fuenteDatos"><asp:Label ID="Label3" runat="server" Text="Indirecto de Oficina Central:"></asp:Label></div><div class="col-md-2 col-3 text-right colorResultadosP"><asp:Label ID="LblFooterIOCP" runat="server" Text=""></asp:Label></div><div class="col-md-2 col-2 text-right colorResultados"><asp:Label ID="LblFooterIOCR" runat="server" Text=""></asp:Label></div></div>
				  <div class="row"><div class="col-md-4 "></div><div class="col-md-4 col-4 text-muted text-right fuenteDatos"><asp:Label ID="Label18" runat="server" Text="Total (Costo Directo + Indirecto):"></asp:Label></div><div class="col-md-2 col-3 text-right colorResultadosP"><asp:Label ID="LblFooterTCDIP" runat="server" Text=""></asp:Label></div><div class="col-md-2 col-2 text-right colorResultados"><asp:Label ID="LblFooterTCDIR" runat="server" Text=""></asp:Label></div></div>
				  <div class="row"><div class="col-md-4 "></div><div class="col-md-4 col-4 text-muted text-right fuenteDatos"><asp:Label ID="Label21" runat="server" Text="Costo del Financiamento:"></asp:Label></div><div class="col-md-2 col-3 text-right colorResultadosP"><asp:Label ID="LblFooterFP" runat="server" Text=""></asp:Label></div><div class="col-md-2 col-2 text-right colorResultados"><asp:Label ID="LblFooterFR" runat="server" Text=""></asp:Label></div></div>
				  <div class="row"><div class="col-md-4 "></div><div class="col-md-4 col-4 text-muted text-right fuenteDatos"><asp:Label ID="Label24" runat="server" Text="Monto del Contrato:"></asp:Label></div><div class="col-md-2 col-3 text-right colorResultadosP"><asp:Label ID="LblFooterMCP" runat="server" Text=""></asp:Label></div><div class="col-md-2 col-2 text-right colorResultados"><asp:Label ID="LblFooterMCR" runat="server" Text=""></asp:Label></div></div>
				  <div class="row"><div class="col-md-4 "></div><div class="col-md-4 col-4 text-muted text-right fuenteDatos"><asp:Label ID="Label27" runat="server" Text="UTILIDAD DEL CONTRATO:"></asp:Label></div><div class="col-md-2 col-3 text-right"><asp:Label ID="LblFooterUP" runat="server" Text=""></asp:Label></div><div class="col-md-2 col-2 text-right"><asp:Label ID="LblFooterUR" runat="server" Text=""></asp:Label></div></div>				 
				  <div class="row"><div class="col-md-4 "></div><div class="col-md-4 col-4 text-muted text-right fuenteDatos"><asp:Label ID="Label30" runat="server" Text="Porcentaje:"></asp:Label></div><div class="col-md-2 col-3 text-right"><asp:Label ID="LblFooterPP" runat="server" Text=""></asp:Label></div><div class="col-md-2 col-2 text-right"><asp:Label ID="LblFooterPR" runat="server" Text=""></asp:Label></div></div>
				 </div>
			</asp:Panel>

                   
			
            <asp:Panel ID="PnlEditar" runat="server" Visible="false" >
				<div class="container">
					<p class="text-muted" style="text-align: center; font-size:18px;">En esta vista puedes <strong>crear</strong> o <strong>modificar</strong> los contratos y reportes.</p>
				<div class="row">
					<div class="col-sm-4"></div>
					<div class="form-inline">
						<div class="form-group">
						<div class="col-md-12 col-12">
                            <asp:LinkButton ID="LbtnNuevo" runat="server" CssClass="btn btn-outline-info" OnClick="LbtnNuevo_Click"><span class="icon icon-plus"></span> Nuevo</asp:LinkButton>
						</div>
						</div>
						<div class="form-group">
						<div class="col-md-12 col-12">
                            <asp:LinkButton ID="LbtnEliminar" runat="server" CssClass="btn btn-outline-info" OnClick="LbtnEliminar_Click"><span class="icon icon-minus"></span> Eliminar</asp:LinkButton>
						</div>
						</div>
						<div class="form-group">
						<div class="col-md-12 col-12">
                            <asp:LinkButton ID="LbtnActualizar" runat="server" CssClass="btn btn-outline-info" OnClick="LbtnActualizar_Click" ><span class="icon icon-loop2"></span> Actualizar</asp:LinkButton>
						</div>
						</div>
					</div>
				</div><br />
				<div class="row">
					<div class="col-md-3"></div>
					<div class="col-md-7">
						<div class="card" id="PnlNuevo" runat ="server" visible="false">  <!--................Panel de Agregar Nuevo-->
							<div class="card-header">
								<div class="row">
								<div class="col-md-3 col-1"></div>
								<div class="col-md-6 col-8 text-muted text-center">
								<h5><asp:Label ID="LblTitlePnlAgregarN" runat="server" Text="" Visible="false"></asp:Label></h5>
								</div>
								<div class="col-md-3 col-1 text-right">
					 		    <asp:LinkButton ID="LbtnCancelar" runat="server" CssClass="btn btn-danger" OnClick="LbtnCancelar_Click"><span class="icon icon-cross"></span></asp:LinkButton> 
								</div>
							   
								</div>
							</div>
							<div class="card-body">
								
									<div class="form-inline">
				                      <div class="input-group mb-3 mr-sm-2">
                                        <asp:LinkButton ID="LbtnAgregarContrato" runat="server" CssClass="btn btn-outline-secondary" OnClick="LbtnAgregarContrato_Click"><span class="icon icon-plus"></span> Agregar Contrato</asp:LinkButton>
									  </div>
		      						  <div class="input-group mb-3 mr-sm-2">
										<asp:LinkButton ID="LbtnReporteMensualN" runat="server"  CssClass="btn btn-outline-secondary" OnClick="LbtnReporteMensualN_Click"><span class="icon icon-plus"></span> Agregar Reporte</asp:LinkButton>	
									  </div>
									   <div class="input-group mb-3">
										 <asp:LinkButton ID="LbtnAgregarProyeccion" runat="server" CssClass="btn btn-outline-secondary" OnClick="LbtnAgregarProyeccion_Click"><span class="icon icon-stats-dots"></span> Agregar Proyección</asp:LinkButton>
				                       </div>
									</div>
								 
								<div class="row">
									<div class="col-md-3"></div>
										<div class="col-md-6">
											<div class="form-group">
                                                <asp:Label ID="LblFechaInicialNC" runat="server" Text="Fecha inicial:" Visible="false"></asp:Label>
											    <asp:TextBox id="TxtFechaInicialNC" runat="server" CssClass="form-control"  MaxLength="10" TextMode="Date"   Visible="false" ToolTip="Ingrese la fecha inicial del contrato"></asp:TextBox>
                                                <asp:Label ID="LblFechaTerminoNC" runat="server" Text="Fecha de termino:" Visible="false"></asp:Label>
											    <asp:TextBox id="TxtFechaTerminoNC" runat="server" CssClass="form-control"  MaxLength="10"  TextMode="Date" Visible="false" ToolTip="Ingrese la fecha de termino del contrato"></asp:TextBox>
												<asp:Label ID="LblContratosN" runat="server" Text="Selecciona un contrato:" Visible="false"></asp:Label>
									        <asp:DropDownList ID="DdlContratosN" runat="server" CssClass="form-control" Visible="false"  ToolTip="Selecciona un contrato"></asp:DropDownList>
					 						<asp:Label ID="LblFechaReporte" runat="server" Text="Fecha de reporte:" Visible="false"></asp:Label>
											<asp:TextBox ID="TxtFechaReporte" runat="server" CssClass="form-control"  MaxLength="10" TextMode="Date"   Visible="false" ToolTip="Ingrese la fecha del reporte mensual"></asp:TextBox>
											<asp:Label ID="LblContratosProyeccion" runat="server" Text="Selecciona un contrato:" Visible="false"></asp:Label>
											<asp:DropDownList ID="DdlContratosProyeccion" runat="server" CssClass="form-control" Visible="false"  AutoPostBack="True" OnSelectedIndexChanged="DdlContratosProyeccion_SelectedIndexChanged" ToolTip="Selecciona un contrato"></asp:DropDownList>
											</div>
											<div class="form-group">
												<div class="input-group mb-3">
													<div class="input-group-append">
													<span class="input-group-text" id="SignoMoneda" runat="server" Visible="false">$</span>
													</div>
													<asp:TextBox id="TxtMontoC" runat="server" CssClass="form-control"  placeholder="Monto Inicial $" Visible="false" ToolTip="Ingrese el monto contratado inicial" TextMode="Number"></asp:TextBox>
												</div>
												<asp:FileUpload ID="FUpExaminar" runat="server" Visible="false"/>
											    <asp:LinkButton ID="LbtnImportar" runat="server" Cssclass="btn btn-outline-info mt-1" OnClick="LbtnImportar_Click" Visible="false"><span class="icon icon-cloud-upload"></span> Subir</asp:LinkButton>
											<asp:Label ID="LblMesProyeccion" runat="server" Text="Selecciona mes incial de proyección:" Visible="false"></asp:Label>
											<asp:DropDownList ID="DdlMesProyecccion" runat="server" CssClass="form-control" Visible="false"  ToolTip="Selecciona el mes inicial de la proyección"></asp:DropDownList>
											</div>
											<div class="form-group">
											<asp:TextBox ID="TxtNombreResidencia" runat="server" CssClass="form-control"  placeholder="Nombre del Contrato:" Visible="false" ToolTip="Ingrese el nombre del contrato"></asp:TextBox>
											</div>
											<div class="form-group text-right">
											<asp:Label ID="LblbtnFormatoExcel" runat="server" Text="Formato de Excel:" Visible="false" CssClass="text-muted"></asp:Label>
											<asp:LinkButton ID="LbtnHelp" runat="server" Visible="false" CssClass="btn btn-info" data-toggle="modal" data-target="#myModal"><span class="icon icon-question"></span></asp:LinkButton>

										</div>
									 </div>
								</div>
							
							<!-- The Modal -->
  <div class="modal" id="myModal">
    <div class="modal-dialog modal-dialog-scrollable">
      <div class="modal-content">
      
        <!-- Modal Header -->
        <div class="modal-header">
          <h1 class="modal-title"><strong>Ejemplo:</strong></h1>
          <button type="button" class="close" data-dismiss="modal">×</button>
        </div>
        
        <!-- Modal body -->
        <div class="modal-body">
          <h4 class="text-info text-left">Formato para subir la información al sistema.</h4>
          <p><asp:Image ID="Imgloanding" runat="server"  ImageUrl="~/Imagenes/Formato.JPG" AlternateText="FormatoEjemplo" CssClass="img-fluid"/></p><hr />
		  <p class="text-primary">Deberá llenar el formato de la siguiente forma:</p>
		  <p class="text-justify"> <b>1.- Campo Concepto.-</b> El concepto de cada contrato a 300 caracteres máximo, puede ir con espacios.<br />
			<b>2.- Campo Observación.-</b> La observación descrita para cada concepto a 500 caracteres máximo, puede ir con espacios.<br />
			<b>3.- Campo PRESUPUESTO.-</b> El presupuesto para cada concepto puede ser mayor a 8 dígitos, sin espacios,sin formato (tipo de dato), puede llevar puntos fraccionarios, solo dos decimales.<br />
			<b>4.- Campo REAL.-</b> El real para cada concepto puede ser mayor a 8 dígitos, sin espacios, sin formato (tipo de dato), puede llevar puntos fraccionarios, solo dos decimales.  <br />
			<b>5.-</b> En el ejemplo del formato, hay N conceptos (filas), estos pueden variar para cada contrato pero <b class="text-danger">NO para cada reporte</b>. <br />
			<b>6.- No puede haber conceptos repetidos</b>, por lo que sera necesario distinguir cada concepto como esta en el formato de ejemplo.<br />
			<b>7.-</b> El <b>Financiamiento</b> en la columna PRESUPUESTO, deberá ingresarse manualmente, pero <b class="text-danger">NO dejarlo en blanco</b>. <br />
		    <b class="text-danger">*</b> Se recomienda guardar el archivo para cada reporte mensual con el nombre de la residencia, mes y año <b>(Eje. Sub Amozoc-PeroteAbril2019)</b> para identificarlo fácilmente.</p>
			<p class="text-warning"><strong>¡Importante!</strong></p>
			<p class="text-justify"><b class="text-danger">*</b> Para los campos <b>PRESUPUESTO</b> Y <b>REAL</b><b class="text-danger"> NO dejar celdas en blanco, </b>el mínimo valor es 0.<br />
			<b class="text-danger">*</b> Por ningún motivo eliminar alguna columna o modificar los encabezados.<br />
			<b class="text-danger">*</b> <b>No modificar</b> las celdas con fondo gris, Solo si es necesario se puede alterar la columna observación.<br />
			<b class="text-danger">*</b> No cambiar el nombre de la hoja.<br />
			<b class="text-danger">*</b> <b>Guardar</b> el archivo como tipo de formato<b class="text-danger"> “Libro de Excel 97-2003” </b>por cuestiones de compatibilidad</p>.
		</div>
        
        <!-- Modal footer -->
        <div class="modal-footer">
          <button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
        </div>
        
      </div>
    </div>
  </div>
  
								
							<div class="row">
								<div class="col-md-12 col-xs-6" >
					 				<asp:LinkButton ID="LbtnVisualizar" runat="server" CssClass="btn btn-outline-success" OnClick="LbtnVisualizar_Click" Visible="true"><span class="icon icon-eye"></span> Visualizar datos</asp:LinkButton>
									<asp:LinkButton ID="LbtnOcultarN" runat="server" CssClass="btn btn-outline-success" Visible="false" OnClick="LbtnOcultarN_Click"><span class="icon icon-eye-blocked"></span> Ocultar datos</asp:LinkButton>
								</div>
							</div>
							<div class="row">
								<div class="col-md-12 col-xs-12">
								<div id="PnlVisualizar" runat="server" visible="false" class="scrolling">
                                    <asp:GridView ID="GvDatosN" runat="server" AutoGenerateColumns ="False" CellPadding="4" ForeColor="#333333" GridLines="None" CssClass="table-bordered" >
                                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                        <Columns>
                                            <asp:BoundField DataField="Concepto" HeaderText="Concepto" />
                                            <asp:BoundField DataField="Observación" HeaderText="Observación" />
                                            <asp:BoundField DataField="PRESUPUESTO" HeaderText="Presupuesto" />
											<asp:BoundField DataField="REAL" HeaderText="Real" />
                                        </Columns>
                                        <EditRowStyle BackColor="#999999" />
                                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" Height="50px" />
                                        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                        <SortedAscendingCellStyle BackColor="#E9E7E2" />
                                        <SortedAscendingHeaderStyle BackColor="#506C8C" />
                                        <SortedDescendingCellStyle BackColor="#FFFDF8" />
                                        <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                                    </asp:GridView>	
							</div> <!-- fin div gridview-->
						</div>
					</div>
					</div><!--Fin Card-body-->
					<div class="card-footer">
						<div class="form-inline">
							<div class="form-group">
								<div class="col-md-12">
								<asp:LinkButton ID="LbtnGuardarC" runat="server" CssClass="btn btn-outline-info" Visible="false" OnClick="LbtnGuardarC_Click"><span class="icon icon-floppy-disk"></span> Guardar</asp:LinkButton>
								<asp:LinkButton ID="LbtnGuardarR" runat="server" CssClass="btn btn-outline-info" Visible="false" OnClick="LbtnGuardarR_Click"><span class="icon icon-floppy-disk"></span> Guardar</asp:LinkButton>
								<asp:LinkButton ID="LbtnGuardarP" runat="server" CssClass="btn btn-outline-info" Visible="false" OnClick="LbtnGuardarP_Click"><span class="icon icon-floppy-disk"></span> Guardar</asp:LinkButton>
								</div>
							</div>
						</div>	
					</div>
				</div> <!-- Fin Panel de agregar Nuevo-->
						
						<div class="card" id="PnlEliminar" runat ="server" visible="false">  <!--................Inicio Panel de Eliminar-->
							<div class="card-header">
								<div class="row">
									<div class="col-md-3 col-1"></div>
									<div class="col-md-6 col-8 text-muted text-center">
										<h5><asp:Label ID="LblTitlePnlEliminar" runat="server" Text="" Visible="false"></asp:Label></h5>
									</div>
									<div class="col-md-3 col-1 text-right">
										<asp:LinkButton ID="LbtnCancelarE" runat="server" CssClass="btn btn-danger" OnClick="LbtnCancelarE_Click" ><span class="icon icon-cross"></span></asp:LinkButton>
									</div>
								</div>
							</div>
							<div class="card-body">
									<div class="form-inline">
										<div class="input-group mb-3 mr-sm-4" >
											<asp:LinkButton ID="LbtnContratoE" runat="server" CssClass="btn btn-outline-secondary" OnClick="LbtnContratoE_Click" ><span class="icon icon-minus"></span> Eliminar Contrato</asp:LinkButton>	
										</div>
		      							<div class="input-group mb-3 mr-sm-4" >
											<asp:LinkButton ID="LbtnReporteE" runat="server"  CssClass="btn btn-outline-secondary" OnClick="LbtnReporteE_Click"><span class="icon icon-minus"></span> Eliminar Reporte</asp:LinkButton>
		     							</div>
										<div class="input-group mb-3">
											<asp:LinkButton ID="LbtnEliminarProyeccion" runat="server" CssClass="btn btn-outline-secondary" OnClick="LbtnEliminarProyeccion_Click"><span class="icon icon-stats-dots"></span> Eliminar Proyección</asp:LinkButton>
										</div>
									</div>
								    <div class="row">
										<div class="col-md-3"></div>
										<div class="col-md-6">
											<div class="form-group">
												<asp:Label ID="LblEliminarContrato" runat="server" Text="Selecciona un contrato:" Visible="false"></asp:Label>
												<asp:DropDownList ID="DDlEleminarResidencia" runat="server" CssClass="form-control" Visible="false"  AutoPostBack="True" OnSelectedIndexChanged="DDlEleminarResidencia_SelectedIndexChanged" ToolTip="Selecciona un contrato"></asp:DropDownList>
											</div>
											<div class="form-group">
												<asp:Label ID="LblEliminarReporte" runat="server" Text="Selecciona fecha del reporte:" Visible="false"></asp:Label>
												<asp:DropDownList ID="DDlEliminarReporte" runat="server" CssClass="form-control" Visible="false" ToolTip="Selecciona la fecha del reporte a borrar"></asp:DropDownList>
											</div>
										</div>
								    </div>
								
							</div><!--Fin Card-body-->
							<div class="card-footer">
								<div class="form-inline">
									<div class="form-group">
									<div class="col-md-12">
										<asp:LinkButton ID="LbtnConfirmarC" runat="server" CssClass="btn btn-outline-info" Visible="false" OnClick="LbtnConfirmarC_Click" OnClientClick="return confirm(&quot;¿Está seguro que desea borrar el contrato?&quot;);"><span class="icon icon-bin"></span> Borrar</asp:LinkButton>  
										<asp:LinkButton ID="LbtnConfirmarR" runat="server" CssClass="btn btn-outline-info" Visible="false" OnClick="LbtnConfirmarR_Click" OnClientClick="return confirm(&quot;¿Está seguro que desea borrar el reporte?&quot;);"><span class="icon icon-bin"></span> Borrar</asp:LinkButton>
										<asp:LinkButton ID="LbtnConfirmarP" runat="server" CssClass="btn btn-outline-info" Visible="false" OnClick="LbtnConfirmarP_Click" OnClientClick="return confirm(&quot;¿Está seguro que desea borrar la proyección?&quot;);" ><span class="icon icon-bin"></span> Borrar</asp:LinkButton>
									</div>
									</div>
								</div>	
							</div>
					</div> <!-- Fin Panel de Eliminar-->
				    <div class="card" id="PnlActualizar" runat ="server" visible="false">  <!--................Panel de Actualizar-->
						<div class="card-header">
								<div class="row">
									<div class="col-md-3 col-1"></div>
									<div class="col-md-6 col-8 text-muted text-center"">
										<h5><asp:Label ID="LblTitlePnlActualizar" runat="server" Text="" Visible="false"></asp:Label></h5>
									</div>
									<div class="col-md-3 col-1 text-right">
										 <asp:LinkButton ID="LbtnCancelarA" runat="server" CssClass="btn btn-danger" OnClick="LbtnCancelarA_Click"><span class="icon icon-cross"></span></asp:LinkButton> 
									</div>
								</div>
						</div>
						<div class="card-body">	
							  <div class="form-inline">
								    <div class="input-group mb-3 mr-sm-4">
										<asp:LinkButton ID="LbtnAConceptos" CssClass="btn btn-outline-secondary" runat="server" OnClick="LbtnAConceptos_Click" Visible="false"><span class="icon icon-loop2"></span> Actualizar Conceptos</asp:LinkButton>			
								    </div>
									<div  class="input-group mb-3">
										<asp:LinkButton ID="LbtnAProyeccion" runat="server" CssClass="btn btn-outline-secondary" Visible="false" OnClick="LbtnAProyeccion_Click"><span class="icon icon-stats-dots"></span> Actualizar Proyección</asp:LinkButton>
											
								    </div>		
							   </div>
                           <div class="row">
							   <div class="col-md-3"></div>
							   <div class="col-md-6">
								 <div class="form-group">
									  <asp:Label ID="LblAContrato" runat="server" Text="* Selecciona un contrato: " Visible="false" CssClass="text-primary"></asp:Label>
									  <asp:DropDownList ID="DdlContratoA" CssClass="form-control" runat="server" Visible="false" AutoPostBack="True" OnSelectedIndexChanged="DdlContratoA_SelectedIndexChanged" ToolTip="Selecciona un contrato"></asp:DropDownList>
									 <asp:Label ID="LblContratoAP" runat="server" Text="Selecciona un contrato: " Visible="false" CssClass="text-primary"></asp:Label>
									 <asp:DropDownList ID="DDlContratoAP" runat="server" CssClass="form-control" Visible="false" AutoPostBack="True" OnSelectedIndexChanged="DDlContratoAP_SelectedIndexChanged" ToolTip="Selecciona un contrato"></asp:DropDownList>
								 </div>
								 <div class="form-group">
									  <asp:Label ID="LblAConcepto" runat="server" Text="* Seleccionar concepto: " Visible="false" CssClass="text-primary"></asp:Label>
									  <asp:DropDownList ID="DdlConceptosA" CssClass="form-control" runat="server"  Visible="false" AutoPostBack="True" OnSelectedIndexChanged="DdlConceptosA_SelectedIndexChanged" ToolTip="Selecciona un concepto"></asp:DropDownList>
								 	<asp:Label ID="LblMesAP" runat="server" Text="Selecciona mes inicial de proyección: " Visible="false" CssClass="text-primary"></asp:Label>
									<asp:DropDownList ID="DDlMesAP" runat="server" CssClass="form-control" Visible="false" ToolTip="Selecciona el mes inicial de la proyección"></asp:DropDownList>
								 </div>
								 <div class="form-group">
									 <asp:Label ID="LblAContrato2" runat="server" Text="* Editar concepto: " Visible="false" CssClass="text-primary"></asp:Label>
									 <asp:TextBox ID="TxtConceptoA" CssClass="form-control" runat="server"  placeholder="Concepto..." Visible="false" ToolTip="Ingrese o modifique el nuevo concepto"></asp:TextBox>
								 </div> 
								 <div class="form-group">
									 <asp:Label ID="LblAObservacion" runat="server" Text="* Editar observación: " Visible="false" CssClass="text-primary text-left"></asp:Label>
									 <asp:TextBox ID="TxtObservacionA" CssClass="form-control" runat="server"   placeholder="Observacíón..." Visible="false" TextMode="MultiLine" Wrap="False" ToolTip="Ingrese o modifique la observación"></asp:TextBox>
								 </div>
							   </div>
                           </div>
						</div> <!--Fin Card-Body-->
						<div  class="card-footer">
							<div class="form-inline">
								<div class="form-group">
									<div class="col-md-12">
										<!--<asp:LinkButton ID="LbtnACambiosC" runat="server" CssClass="btn btn-outline-info" OnClick="LbtnACambiosC_Click" Visible="false"><span class="icon icon-floppy-disk"></span> Aplicar cambios</asp:LinkButton>-->
										<asp:LinkButton ID="LbtnACambiosCptos" runat="server" CssClass="btn btn-outline-info" OnClick="LbtnACambiosCptos_Click" Visible="false"><span class="icon icon-floppy-disk"></span> Aplicar</asp:LinkButton>
										<asp:LinkButton ID="LbtnACambiosP" runat="server" CssClass="btn btn-outline-info" Visible="false" OnClick="LbtnACambiosP_Click"><span class="icon icon-floppy-disk"></span> Aplicar</asp:LinkButton>
									</div>
								</div>	
							</div>
						</div>
					</div> <!-- Fin Panel actualizar-->

				   </div><!--Fin de Div contiene los panels de los botones Nuevo,Eliminar,Actualizar-->
				</div>
			  </div> <!--container dentro del panel Editar--->
            </asp:Panel>
			
			<div  style="font-size:14px;"><div class="col-md-12 col-xs-12 text-center text-muted">&copy; <%: DateTime.Now.Year %>- <asp:Label runat="server" Text="www.scalapc.com"></asp:Label> </div></div>
		</div><!--container fuera del panel Editar--->

    </form>
	<div class=" alert alert-success   alert-dismissible fixed-top" style=" width: 90%;margin-left: 4em;" role="alert" visible="false" runat="server" id="AlertSuccess">
        <button class="close" type="button" data-dismiss="alert"><span class="icon icon-cancel-circle"></span></button>
        <span class="icon icon-checkmark" aria-hidden="true"></span>
        <asp:Label ID="lblSucces" runat="server" Font-Size="Larger" ></asp:Label>
    </div>
	 <div class=" alert alert-warning  alert-dismissible fixed-top" style=" width: 90%;margin-left: 4em;" role="alert" visible="false" runat="server" id="AlertWarning">
        <button class="close" type="button" data-dismiss="alert"><span class="icon icon-cancel-circle"></span></button>
        <span class="icon icon-warning" aria-hidden="true"></span>
        <asp:Label ID="lblWarning" runat="server" ></asp:Label>
    </div>
     <div class=" alert alert-danger  alert-dismissible fixed-top" style=" width: 90%;margin-left: 4em;" role="alert" visible="false" runat="server" id="AlertDanger">
        <button class="close" type="button" data-dismiss="alert"><span class="icon icon-cancel-circle"></span></button>
        <span class="icon icon-sad2" aria-hidden="true"></span>
        <asp:Label ID="lblDanger" runat="server" ></asp:Label>
     </div>
	 <div class=" alert alert-secondary  alert-dismissible fixed-top" style=" width: 90%;margin-left: 4em;" role="alert" visible="false" runat="server" id="AlertSecundary">
        <button class="close" type="button" data-dismiss="alert"><span class="icon icon-cancel-circle"></span></button>
        <span class="icon icon-sad2" aria-hidden="true"></span>
        <asp:Label ID="lblSecundary" runat="server" ></asp:Label>
     </div>

	<script  type="text/javascript">
       
		$(document).ready(function () {
				$('#GvDatosReportes tbody tr:first').wrap('<thead/>').parent().prependTo('#GvDatosReportes');
		});
		function myFunctionFiltrar() {
            var input, filter, table, tr, td, i, txtValue;
            input = document.getElementById("myInput");
            filter = input.value.toUpperCase();
            table = document.getElementById("GvDatosReportes");
            tr = table.getElementsByTagName("tr");
            for (i = 0; i < tr.length; i++) {
                td = tr[i].getElementsByTagName("td")[0];
                if (td) {
                    txtValue = td.textContent || td.innerText;
                    if (txtValue.toUpperCase().indexOf(filter) > -1) {
                        tr[i].style.display = "";
                    } else {
                        tr[i].style.display = "none";
                    }
                }
            }
		}
		//Evitar un  postback al dar enter en el textbox para filtrar tabla
        function stopRKey(evt) {
            var evt = (evt) ? evt : ((event) ? event : null);
            var node = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null);
            if ((evt.keyCode == 13) && (node.type == "text")) { return false; }
		}
		document.onkeypress = stopRKey; 
		
        google.charts.load('current', { 'packages': ['corechart'] });
        google.charts.setOnLoadCallback(drawVisualization);
        function drawVisualization() {
            // Some raw data (not necessarily accurate)
            var data = google.visualization.arrayToDataTable(<%=cadDatos%>);

                var options = {
                    title: <%=titleGrafica%>,
                    height: '100%',
                    width: '100%',
                    hAxis: {
                        title: 'MES', titleTextStyle: { bold: 'true', color: 'darkslategrey' }, textStyle: { color: 'Grey', fontSize: 14 }
                    },
                    seriesType: 'bars',
                    series: {
                        0: { targetAxisIndex: 0, color: '#e5e4e2' },
                        1: { targetAxisIndex: 1, type: 'line', color: '#1c91c0', pointSize: 10 },
                        2: { targetAxisIndex: 1, type: 'line', color: 'black', lineDashStyle: [4, 4], lineWidth: 3 }
                    },
                    vAxes: {
                        // Adds titles to each axis.
                        0: { title: 'U t i l i d a d ($)', titleTextStyle: { bold: 'true', color: 'darkslategrey' }, textStyle: { color: 'Red' }, format: 'currency' },
                        1: { title: 'P o r c e n t a j e (%)', titleTextStyle: { bold: 'true', color: 'darkslategrey' }, textStyle: { bold: 'true' }, format: '#\'%\'' }
                    }
                };

                var chart = new google.visualization.ComboChart(document.getElementById('chart_div'));

                var formatter2 = new google.visualization.NumberFormat(
                    { prefix: '$', negativeColor: 'red', negativeParens: true });
                formatter2.format(data, 1); // Apply formatter to Utilidad ($)

                var formatter3 = new google.visualization.NumberFormat(
                    { negativeColor: 'red', negativeParens: true, pattern: '#\'%\'' });
                formatter3.format(data, 2); // Apply formatter to Real (%)
                /*
                        var formatter4 = new google.visualization.NumberFormat(
                            { negativeColor: 'red', negativeParens: true, pattern: '#\'%\'' });
                        formatter4.format(data, 4); // Apply formatter to Proyectado (%)
                  */


                chart.draw(data, options);

            }
        
        
    </script>
</body>
</html>
