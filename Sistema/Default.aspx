<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Sistema.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Login" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server"></asp:ScriptManagerProxy>  
     <asp:UpdatePanel ID="UpdatePanel1" runat="server"> 
        <ContentTemplate>
       <div class="container moverlogin">
           <div class="row"></div>
           <div class="row">
              <div class="col-md-3" ></div>
               <div class="col-md-6 col-xs-12">
                   <div class="caja">
                       <div class="row">
                           <div class="col-md-12 col-xs-12 text-center text-muted">
                           <h2><asp:Label runat="server" Text="Coordinación de Logistica y Finanzas"></asp:Label></h2>
                           </div>
                       </div><!----1ra fila de la caja-->
                       <div class="row">
                           
                           <div class="col-md-1"></div>
                           <div class="col-md-5 text-center">
                           <asp:Image ID="Image1" runat="server"  CssClass="img-fluid" ImageUrl="~/Imagenes/Scaleitor.PNG" />
                           </div>
                           <div class="col-md-6 alinear">
                               <asp:Panel ID="PanelBotones" runat="server" Visible="true"><br />
                                 <div class="col-md-10">
                               <div class="form-group"><asp:LinkButton ID="LbtnInvitado" runat="server" Cssclass="btn btn-outline-primary btn-block" OnClick="LbtnInvitado_Click1">Invitado <span class="icon icon-user"></span></asp:LinkButton></div>
                               <div class="form-group"> <asp:LinkButton ID="LbtnAdmin" runat="server" Cssclass="btn btn-outline-primary btn-block" OnClick="BtnAdmin_Click">Administrador  <span class="icon icon-cogs"></span></asp:LinkButton></div>
                                  </div>
                               </asp:Panel>
                          
                           
                           
                                <asp:Panel ID="PanelLogin" runat="server" Visible="false">
                                    <asp:Label  Cssclass="text-danger" ID="lblmsj" runat="server" visible="false"></asp:Label>
                                    <form class="form-horizontal">
                                            <div class="form-group"> 
                                                <div class="col-md-12">
                                                <asp:TextBox ID="txtUsuario" runat="server" Cssclass="form-control" AutoCompleteType="Cellular" TextMode="Email" placeholder="Usuario:"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <div class="col-md-12">
                                                <asp:TextBox ID="txtPassword" runat="server" Cssclass="form-control" TextMode="Password"  placeholder="Contraseña:"></asp:TextBox>                                               
                                                </div>
                                            </div>
                                        <div class="form-inline">
                                            <div class="col-md-5 text-left">
                                           <asp:CheckBox ID="CheckRecordar" runat="server" Cssclass="checkbox text-muted" Text="Recordar" Checked="True" />
                                            </div>
                                           
                                            <div  class="col-md-7 text-right">
                                           <asp:LinkButton ID="LbtnIniciar" runat="server" Cssclass="btn btn-info btn-block" OnClick="BtnIniciar_Click"><span class="icon icon-enter"></span> Entrar</asp:LinkButton>
                                            </div>   
                                          </div>
                                        
                                       <div class="form-group">
                                           <div class="col-md-12 text-right">
                                               <asp:LinkButton ID="LbtnCancelar" runat="server" Cssclass="btn btn-outline-secondary btn-block" OnClick="BtnCancelar_Click"><span class="icon icon-undo2"></span></asp:LinkButton>
                                           </div>
                                       </div>
                                    </form>
                                </asp:Panel>
                                </div>
                           </div> <!----2da de la caja-->
                          <div class="row"> 
                              <div class="col-md-12 col-xs-12 text-center text-muted">&copy; <%: DateTime.Now.Year %>- <asp:Label runat="server" Text="www.scalapc.com"></asp:Label> </div>
                          </div>
                          </div> <!----caja-->
                       
                       </div>
                    <div class="col-md-3"></div>
                   </div>
              
           <div class="row"></div>
           </div>
    <div class="alert alert-danger alert-dismissible fixed-top" style=" width: 90%;margin-left: 4em;" role="alert" visible="false" runat="server" ID="AlertDanger">
        <button class="close" type="button" data-dismiss="alert"><span class="icon icon-cancel-circle"></span></button>
        <span class="icon icon-sad" aria-hidden="true"></span>
        <asp:Label ID="lblDanger" runat="server" ></asp:Label>
    </div>
      </ContentTemplate> 
       </asp:UpdatePanel>
</asp:Content>
