using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using System.Web.Security;
namespace Sistema
{
    
    public partial class Default : System.Web.UI.Page
    {

        private SqlConnection con = null;
        private SqlCommand OrdenSql;
        private SqlDataReader Leer;
        protected void Page_Load(object sender, EventArgs e)
        {
            String strConexion = "server=DESKTOP-O8UKCDP\\SQLEXPRESS; database=LogisticaDB; uid=sa; pwd=1234;";
            con = new SqlConnection(strConexion);
            AlertDanger.Visible = false;
            if (!IsPostBack)
            {
                if (Response.Cookies["email"] != null && Response.Cookies["pass"] != null)
                {
                    txtUsuario.Text = Request.Cookies["email"].Value;
                    txtPassword.Attributes["value"] = Request.Cookies["pass"].Value;
                }
            }
            if(Request.Params["id"] != null)
            {
                String identificador = Request.Params["id"];
                if (identificador == "1")
                {
                    AlertDanger.Visible = true;
                    lblDanger.Text = "* Debe iniciar sesión como <strong>Invitado</strong> o <strong>Administrador</strong>";
                }
                
            }
            lblmsj.Visible = false;
            txtUsuario.Focus();
        }

        protected void BtnAdmin_Click(object sender, EventArgs e)
        {
            PanelBotones.Visible = false;
            PanelLogin.Visible = true;
            AlertDanger.Visible = false;
            lblmsj.Visible = false;
        }

        protected void BtnCancelar_Click(object sender, EventArgs e)
        {
            PanelBotones.Visible = true;
            PanelLogin.Visible = false;
            AlertDanger.Visible = false;
            lblmsj.Visible = false;
        }
       

        protected void BtnIniciar_Click(object sender, EventArgs e)
        {
            String nombre,correo,password;
            try
            {
                using (con)
                {
                    con.Open();
                    String query1 = "Select Nombre,Correo,Password From Usuarios";
                    OrdenSql = new SqlCommand(query1, con);
                        Leer = OrdenSql.ExecuteReader();
                    if (txtUsuario.Text != "" || txtPassword.Text != "") {
                        while (Leer.Read())
                        {

                            nombre = Leer[0].ToString();
                            correo = Leer[1].ToString();
                            password = Leer[2].ToString();

                            if (correo == txtUsuario.Text || password == txtPassword.Text)
                            {

                                if (correo == txtUsuario.Text)
                                {
                                    if (password == txtPassword.Text)
                                    {
                                        if (CheckRecordar.Checked)
                                        {
                                            Response.Cookies["email"].Value = txtUsuario.Text;
                                            Response.Cookies["pass"].Value = txtPassword.Text;
                                            Response.Cookies["email"].Expires = DateTime.Now.AddMinutes(1);
                                            Response.Cookies["pass"].Expires = DateTime.Now.AddMinutes(1);
                                        }
                                        else
                                        {
                                            Response.Cookies["email"].Expires = DateTime.Now.AddMinutes(-1);
                                            Response.Cookies["pass"].Expires = DateTime.Now.AddMinutes(-1);
                                        }
                                        AlertDanger.Visible = false;
                                        Session["Usuario"] = nombre;
                                        Response.Redirect("Inicio.aspx");
                                    }
                                    else
                                    {
                                        AlertDanger.Visible = true;
                                        lblmsj.Visible = false;
                                        lblDanger.Text = "<strong>¡Cuidado!</strong>La contraseña es incorrecta...";
                                        txtPassword.Text = "";
                                        txtPassword.Focus();
                                    }
                                }
                                else
                                {
                                    AlertDanger.Visible = true;
                                    lblmsj.Visible = false;
                                    lblDanger.Text = "<strong>¡Cuidado!</strong>El usuario es incorrecto...";
                                    txtUsuario.Text = "";
                                    txtUsuario.Focus();
                                }
                            }

                        }
                    }
                    else
                    {
                        lblmsj.Visible = true;
                        lblmsj.Text = "*Ingresa tus datos";
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                AlertDanger.Visible = true;
                lblDanger.Text = "<strong>¡Error!</strong> Al conectarse al servidor:" + ex.Message;
            }
        }
        protected void LbtnInvitado_Click1(object sender, EventArgs e)
        {
            String nombre = "Invitado";
            Session["Usuario"] = nombre;
            Response.Redirect("Inicio.aspx");
        }
    }
}