using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using StockifyWeb.EmpresaWS;

namespace StockifyWeb
{
    public partial class Proveedores : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // ASP.NET Web Forms no soporta async en Page_Load directamente
                // Usamos RegisterAsyncTask para operaciones asíncronas
                RegisterAsyncTask(new PageAsyncTask(CargarProveedoresAsync));
            }
        }

        private async Task CargarProveedoresAsync()
        {
            try
            {
                // Crear el cliente del Web Service
                EmpresaWSClient cliente = new EmpresaWSClient();
                
                // Llamar al método asíncrono
                var response = await cliente.listarEmpresasAsync();
                var empresas = response.@return; // Nota el @return porque es una palabra reservada
                
                if (empresas != null && empresas.Length > 0)
                {
                    // Transformar los datos para el GridView
                    var proveedores = empresas.Select(e => new
                    {
                        IdEmpresa = e.idEmpresa,
                        Nombre = e.razonSocial ?? "",
                        Producto = "---",
                        Telefono = e.telefono ?? "",
                        Email = e.email ?? "",
                        TipoEmpresa = e.tipoEmpresaSpecified ? e.tipoEmpresa.ToString() : "---",
                        Activo = e.activo ? "Si" : "No"
                    }).ToList();

                    gvProveedores.DataSource = proveedores;
                    gvProveedores.DataBind();
                    
                    System.Diagnostics.Debug.WriteLine($"✅ Cargados {proveedores.Count} proveedores desde el Web Service");
                }
                else
                {
                    gvProveedores.DataSource = new List<object>();
                    gvProveedores.DataBind();
                    System.Diagnostics.Debug.WriteLine("⚠️ No hay proveedores en la base de datos");
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error de conexión: {ex.Message}");
                CargarProveedoresEjemplo();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");
                CargarProveedoresEjemplo();
            }
        }

        protected void btnAddSupplier_Click(object sender, EventArgs e)
        {
            // Para el evento Click también necesitamos async
            RegisterAsyncTask(new PageAsyncTask(async () => await AgregarProveedorAsync()));
        }

        private async Task AgregarProveedorAsync()
        {
            try
            {
                string razonSocial = txtSupplierName.Text.Trim();
                string telefono = txtTelefono.Text.Trim();
                string email = txtEmail.Text.Trim();
                bool activo = ddlActivo.SelectedValue == "Si";

                if (string.IsNullOrEmpty(razonSocial))
                {
                    MostrarMensaje("Por favor ingrese el nombre de la empresa");
                    return;
                }

                if (string.IsNullOrEmpty(telefono))
                {
                    MostrarMensaje("Por favor ingrese el teléfono");
                    return;
                }

                if (!string.IsNullOrEmpty(email) && !EsEmailValido(email))
                {
                    MostrarMensaje("Por favor ingrese un email válido");
                    return;
                }

                await GuardarProveedorAsync(razonSocial, telefono, email, activo);

                LimpiarFormulario();
                
                ScriptManager.RegisterStartupScript(this, GetType(), "cerrarModal", 
                    "if(typeof cerrarModal === 'function') cerrarModal();", true);

                await CargarProveedoresAsync();

                MostrarMensaje("✅ Proveedor agregado correctamente", true);
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al agregar proveedor: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ Error al agregar: {ex.Message}");
            }
        }

        private async Task GuardarProveedorAsync(string razonSocial, string telefono, string email, bool activo)
        {
            try
            {
                EmpresaWSClient cliente = new EmpresaWSClient();
                
                var nuevaEmpresa = new empresa
                {
                    razonSocial = razonSocial,
                    telefono = telefono,
                    email = email,
                    activo = activo,
                    tipoEmpresa = tipoEmpresa.PROVEEDOR, // Especificar tipo
                    tipoEmpresaSpecified = true
                };

                // El método guardarEmpresaAsync requiere empresa y estado
                await cliente.guardarEmpresaAsync(nuevaEmpresa, estado.NUEVO);
                
                System.Diagnostics.Debug.WriteLine($"✅ Proveedor guardado: {razonSocial}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error al guardar: {ex.Message}");
                throw new Exception($"No se pudo guardar el proveedor: {ex.Message}", ex);
            }
        }

        private void CargarProveedoresEjemplo()
        {
            var proveedores = new List<dynamic>
            {
                new { IdEmpresa = 0, Nombre = "Changa SAC", Producto = "Monitor", 
                      Telefono = "7687784556", Email = "ChangaSAC@gmail.com", 
                      TipoEmpresa = "PROVEEDOR", Activo = "Si" },
                new { IdEmpresa = 0, Nombre = "Knitos", Producto = "Teclados", 
                      Telefono = "9867545368", Email = "KnitosCorp@gmail.com", 
                      TipoEmpresa = "PROVEEDOR", Activo = "Si" }
            };

            gvProveedores.DataSource = proveedores;
            gvProveedores.DataBind();
        }

        private void LimpiarFormulario()
        {
            txtSupplierName.Text = "";
            txtProduct.Text = "";
            txtTelefono.Text = "";
            txtEmail.Text = "";
            if (ddlTipoEmpresa != null) ddlTipoEmpresa.SelectedIndex = 0;
            if (ddlActivo != null) ddlActivo.SelectedIndex = 0;
        }

        private void MostrarMensaje(string mensaje, bool esExitoso = false)
        {
            mensaje = mensaje.Replace("'", "\\'");
            string script = esExitoso 
                ? $"alert('✅ {mensaje}');" 
                : $"alert('{mensaje}');";
                
            ScriptManager.RegisterStartupScript(this, GetType(), "mostrarMensaje", script, true);
        }

        private bool EsEmailValido(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Método para eliminar proveedores
        protected void EliminarProveedor_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(async () =>
            {
                try
                {
                    // Obtener el ID del botón que disparó el evento
                    Button btn = (Button)sender;
                    int idEmpresa = Convert.ToInt32(btn.CommandArgument);
                    
                    EmpresaWSClient cliente = new EmpresaWSClient();
                    await cliente.eliminarEmpresaAsync(idEmpresa);
                    
                    await CargarProveedoresAsync();
                    MostrarMensaje("Proveedor eliminado correctamente", true);
                }
                catch (Exception ex)
                {
                    MostrarMensaje($"Error al eliminar proveedor: {ex.Message}");
                }
            }));
        }
    }
}