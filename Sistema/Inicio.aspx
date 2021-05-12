<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Inicio.aspx.cs" Inherits="Sistema.Inicio" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
<meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1"/>
    <title>Consolidados</title>
   <link rel="stylesheet" type="text/css" href="boostrap/bootstrap.min.css"/>
    <script type="text/javascript" src="js/jquery-3.5.1.min.js"></script>
	<script type="text/javascript" src="js/Script.js"></script>
	<script type="text/javascript" src="js/StyleTableConsolidado.js"></script>
	<link  rel="stylesheet" type="text/css" href="css/Estilos.css"/>
	<link rel="stylesheet" type="text/css" href="css/Responsive.css"/>
	<link rel="stylesheet" href="css/Iconos/demo.css"/>
	<script type="text/javascript" src="js/bootstrap.min.js"></script>

</head>
   
<body onload="myFunction()" style="margin:0;">
    <style type="text/css">
		/***********************/
		 #GVAreas {
             width:1100px;
			 font-size:14px;
			 text-align:center;
		 }
		 #GVAreas.table tr td{
              width:1100px;
			  font-size:12px;
			  text-align:right;
			 
		  }
        /***********************/
		 #GVConsolidado {
             width:1100px;
			 font-size:14px;
			 text-align:center;
		 }
		 #GVConsolidado.table tr td{
              width:1100px;
			  font-size:12px;
			  text-align:right;
			 
		  }

    </style>
   <div id="loader"></div>
    <form id="form1" runat="server" style="display:none;" class="animate-bottom">
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
						<asp:LinkButton ID="LbtnConsolidado" runat="server" ><span class="icon icon-stack"></span> Consolidado</asp:LinkButton>
                        <asp:LinkButton ID="LbtnSupervision" runat="server" OnClick="LbtnSupervision_Click"><span class="icon icon-quill"></span> Supervisión</asp:LinkButton>
                        <asp:LinkButton ID="LbtnProyectos" runat="server" OnClick="LbtnProyectos_Click" ><span class="icon icon-books"> Proyectos</span></asp:LinkButton>
                        <asp:LinkButton ID="LbtnAmbiental" runat="server" OnClick="LbtnAmbiental_Click"><span class="icon icon-leaf"></span> Ambiental</asp:LinkButton>
                        <asp:LinkButton ID="LbtnContruccion" runat="server" OnClick="LbtnContruccion_Click"><span class="icon icon-office"></span> Construcción</asp:LinkButton>
                        <asp:LinkButton ID="LbtnSalir" runat="server"  OnClick="BtnCerrarSesion_Click"><span class="icon icon-exit"></span> Salir</asp:LinkButton>
                        <p><span class="icon icon-user"></span> <asp:Label ID="lblmensaje" runat="server" Text=""></asp:Label></p>
					</ul>
		    </section>
	      </header>
        <div class="container">
            <div class="row">
                <div class="col-md-12 col-xs-12" >
                    <h4 class="card-header" >HISTORIAL DE RENTABILIDAD-<strong>CONSOLIDADO</strong></h4>
                </div>
            </div>
         </div><br />
        <div class="container">
                <div runat="server" id="MenuGlobal" visible="true">
                <div class="form-inline">
                            <div class="input-group">
		    					<asp:LinkButton ID="LbtnBuscar" runat="server" Cssclass="btn btn-outline-dark mb-2 mr-sm-2" OnClick="LbtnBuscar_Click"><span class="icon icon-search"></span> Buscar:</asp:LinkButton>
                            </div>
                            <asp:Label ID="lblDe" runat="server" Text="De:" CssClass="mb-1 mr-sm-2"></asp:Label>
                            <div class="input-group">
		     					<asp:DropDownList ID="DdlMesInicial" runat="server" CssClass="form-control mb-2 mr-sm-2" ToolTip="Seleccione un mes inicial"></asp:DropDownList>
						    </div>
							<asp:Label ID="lblA" runat="server" Text="a" CssClass="mb-1 mr-sm-2"></asp:Label>
							<div class="input-group">
		     					<asp:DropDownList ID="DdlMesFinal" runat="server" CssClass="form-control mb-2 mr-sm-2" ToolTip="Seleccione un mes final"></asp:DropDownList> 
							</div>
                            <asp:Label ID="lblAnio" runat="server" Text="Año:" CssClass="mb-1 mr-sm-2"></asp:Label>
                            <div  class="input-group">
                                <asp:DropDownList ID="DdlAnio" runat="server" CssClass="form-control mb-2 mr-sm-2" ToolTip="Seleccione el año"></asp:DropDownList>
                            </div>
                            <div class="input-group">
                               <asp:LinkButton ID="LbtnGraficaGlobal" runat="server" CssClass="btn btn-outline-success mb-2 mr-sm-2" Visible ="false"><span class="icon icon-stats-dots"></span> Grafica</asp:LinkButton>
							</div>
                    </div>
                    </div>
               
        </div><br />
		<div class="container">
        <asp:Panel ID="PnlTableAreas" runat="server" Visible="false">
			 <div class="text-left"> <h5><strong><asp:Label ID="LblTitleRentabilidad" runat="server" Text="Rentabilidad de los contratos" Visible=" false"></asp:Label></strong></h5></div>
			 <div class="text-left"><asp:Label ID="LblTitleResumenC" runat="server"></asp:Label></div>
             <div class="row">
				   <div class="col-md-12 col-xs-12">
					<asp:Panel ID="Panel1" runat="server">
						<div class="table-responsive">
								<asp:GridView ID="GVAreas" runat="server" AutoGenerateColumns="false"  CellPadding="3" BackColor="White" BorderColor="#00003e" BorderStyle="None" BorderWidth="1px" CssClass="table table-hover">
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
         <asp:Panel ID="PnlTableGlobal" runat="server" Visible="false">
			 <div class="text-left"> <h5><strong><asp:Label ID="LblTitleConsolidadoG" runat="server" Text="Consolidado General" Visible="false"></asp:Label></strong></h5></div>
			 <div class="text-left"><asp:Label ID="LblTitleResumenG" runat="server"></asp:Label></div>
             <div class="row">
				   <div class="col-md-12 col-xs-12">
					<asp:Panel ID="Panel2" runat="server">
						<div class="table-responsive">
								<asp:GridView ID="GVConsolidado" runat="server" AutoGenerateColumns="false"  CellPadding="3" BackColor="White" BorderColor="#00003e" BorderStyle="None" BorderWidth="1px" CssClass="table table-hover">
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
       
         <asp:Panel ID="PnlVacio" runat="server" visible="false">
				<br /><br />
				<div class="row">
					<div class="col-md-12 text-center">
					<h3 runat="server" class="card-body"><strong>:( No hay contenido en esta vista.</strong></h3>
					</div>
				</div>
         </asp:Panel>
         <div class="row" style="font-size:14px;">
             <div class="col-md-12 col-xs-12 text-center text-muted">&copy; <%: DateTime.Now.Year %>- <asp:Label runat="server" Text="www.scalapc.com"></asp:Label> </div>
         </div>
		</div><!--fin container--->
    </form>

    <!-- Aqui se encuentran los mensajes de alerta-->
	 <div class=" alert alert-success  alert-dismissible fixed-top" style=" width: 90%;margin-left: 4em;" role="alert" visible="false" runat="server" id="AlertSuccess">
        <button class="close" type="button" data-dismiss="alert"><span class="icon icon-cancel-circle"></span></button>
        <span class="icon icon-checkmark" aria-hidden="true"></span>
        <asp:Label ID="lblSucces" runat="server" ></asp:Label>
    </div>
	 <div class=" alert alert-warning   alert-dismissible fixed-top" style=" width: 90%;margin-left: 4em;" role="alert" visible="false" runat="server" id="AlertWarning">
        <button class="close" type="button" data-dismiss="alert"><span class="icon icon-cancel-circle"></span></button>
        <span class="icon icon-warning" aria-hidden="true"></span>
        <asp:Label ID="lblWarning" runat="server" ></asp:Label>
    </div>
     <div class=" alert alert-danger  alert-dismissible fixed-top" style=" width: 90%;margin-left: 4em;" role="alert" visible="false" runat="server" id="AlertDanger">
        <button class="close" type="button" data-dismiss="alert"><span class="icon icon-cancel-circle"></span></button>
        <span class="icon icon-sad2" aria-hidden="true"></span>
        <asp:Label ID="lblDanger" runat="server" ></asp:Label>
    </div>
     <script  type="text/javascript">
        var myVar;

        function myFunction() {
            myVar = setTimeout(showPage);
        }

        function showPage() {
            document.getElementById("loader").style.display = "none";
            document.getElementById("form1").style.display = "block";
        }
    </script>
</body>
</html>
