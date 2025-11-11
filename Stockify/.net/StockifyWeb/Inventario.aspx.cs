using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using StockifyWeb.StockifyWS;

namespace StockifyWeb
{
    public partial class Inventario : System.Web.UI.Page
    {
        #region Page Load

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarCategorias();
                CargarProductos();
                ConfigurarBotonGuardar(false); // Modo agregar por defecto
            }
            else
            {
                // En postback, verificar si necesitamos recargar categorías
                if (ddlCategoria.Items.Count == 0 ||
                    (ddlCategoria.Items.Count == 1 && ddlCategoria.Items[0].Value == "0"))
                {
                    CargarCategorias();
                }
            }
        }

        #endregion

        #region Cargar Datos desde Web Services

        private void CargarCategorias()
        {
            try
            {
                ddlCategoria.Items.Clear();

                CategoriaWSClient categoriaCliente = new CategoriaWSClient();
                var categorias = categoriaCliente.listarCategorias();

                System.Diagnostics.Debug.WriteLine($"Categorías recibidas: {categorias?.Length ?? 0}");

                if (categorias != null && categorias.Length > 0)
                {
                    // Agregar opción por defecto primero
                    ddlCategoria.Items.Add(new ListItem("Seleccione categoría", "0"));

                    // Agregar cada categoría manualmente para mejor control
                    foreach (var cat in categorias)
                    {
                        string nombreCategoria = cat.nombre ?? "Sin nombre";
                        string idCategoria = cat.idCategoria.ToString();

                        System.Diagnostics.Debug.WriteLine($"Agregando: {nombreCategoria} (ID: {idCategoria})");
                        ddlCategoria.Items.Add(new ListItem(nombreCategoria, idCategoria));
                    }
                }
                else
                {
                    ddlCategoria.Items.Add(new ListItem("No hay categorías disponibles", "0"));
                    MostrarMensaje("No se encontraron categorías");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar categorías: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                ddlCategoria.Items.Add(new ListItem("Error al cargar", "0"));
                MostrarMensaje($"Error al cargar categorías: {ex.Message}");
            }
        }

        private void CargarProductos()
        {
            try
            {
                ProductoWSClient cliente = new ProductoWSClient();
                var productos = cliente.listarProductos();

                if (productos != null && productos.Length > 0)
                {
                    var listaProductos = productos.Select(p => new
                    {
                        IdProducto = p.idProducto,
                        Producto = p.nombre ?? "Sin nombre",
                        Precio = p.precioUnitario,
                        Descripcion = p.descripcion ?? "---",
                        Marca = p.marca ?? "---",
                        Categoria = p.categoria != null ? p.categoria.nombre : "---"
                    }).ToList();

                    gvProductos.DataSource = listaProductos;
                    gvProductos.DataBind();
                }
                else
                {
                    gvProductos.DataSource = null;
                    gvProductos.DataBind();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar productos: {ex.Message}");
                MostrarMensaje("Error al cargar productos desde el servidor");
            }
        }

        #endregion

        #region Guardar Producto (Agregar o Actualizar)

        protected void btnOpenModal_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            CargarCategorias();
            ConfigurarBotonGuardar(false);

            ScriptManager.RegisterStartupScript(this, GetType(), "openModal",
                "abrirModal();", true);
        }

        protected void btnSaveProduct_Click(object sender, EventArgs e)
        {
            if (!ValidarFormulario()) return;

            bool esEdicion = ViewState["EditMode"] != null && (bool)ViewState["EditMode"];

            if (esEdicion)
            {
                ActualizarProducto();
            }
            else
            {
                AgregarProducto();
            }
        }

        private void AgregarProducto()
        {
            try
            {
                string nombre = txtProductName.Text.Trim();
                double precio = double.Parse(txtPrecioUnitario.Text);
                string descripcion = txtDescripcion.Text.Trim();
                string marca = txtMarca.Text.Trim();
                int categoriaId = int.Parse(ddlCategoria.SelectedValue);

                // Obtener la categoría completa
                CategoriaWSClient categoriaCliente = new CategoriaWSClient();
                var categoria = categoriaCliente.obtenerCategoria(categoriaId);

                if (categoria == null)
                {
                    MostrarMensaje("Categoría no encontrada");
                    return;
                }

                // Crear producto
                var nuevoProducto = new producto
                {
                    nombre = nombre,
                    precioUnitario = precio,
                    descripcion = descripcion,
                    marca = marca,
                    categoria = categoria,
                    stockMaximo = 100,
                    stockMinimo = 10
                };

                // Guardar en el Web Service
                ProductoWSClient productoCliente = new ProductoWSClient();
                productoCliente.guardarProducto(nuevoProducto, estado.NUEVO);

                LimpiarFormulario();
                CargarProductos();

                ScriptManager.RegisterStartupScript(this, GetType(), "success",
                    "cerrarModal(); alert('✅ Producto agregado correctamente');", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al agregar producto: {ex.Message}");
                MostrarMensaje("Error al agregar el producto");
            }
        }

        private void ActualizarProducto()
        {
            try
            {
                int idProducto = int.Parse(hdnProductoId.Value);

                // Obtener el producto existente
                ProductoWSClient cliente = new ProductoWSClient();
                var producto = cliente.obtenerProducto(idProducto);

                if (producto == null)
                {
                    MostrarMensaje("Producto no encontrado");
                    return;
                }

                // Actualizar datos
                producto.nombre = txtProductName.Text.Trim();
                producto.precioUnitario = double.Parse(txtPrecioUnitario.Text);
                producto.descripcion = txtDescripcion.Text.Trim();
                producto.marca = txtMarca.Text.Trim();

                int categoriaId = int.Parse(ddlCategoria.SelectedValue);
                CategoriaWSClient categoriaCliente = new CategoriaWSClient();
                producto.categoria = categoriaCliente.obtenerCategoria(categoriaId);

                // Guardar cambios
                cliente.guardarProducto(producto, estado.MODIFICADO);

                LimpiarFormulario();
                CargarProductos();

                ScriptManager.RegisterStartupScript(this, GetType(), "updated",
                    "cerrarModal(); alert('✅ Producto actualizado correctamente');", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al actualizar producto: {ex.Message}");
                MostrarMensaje("Error al actualizar el producto");
            }
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MostrarMensaje("Por favor ingrese el nombre del producto");
                return false;
            }

            if (!double.TryParse(txtPrecioUnitario.Text, out double precio) || precio <= 0)
            {
                MostrarMensaje("Por favor ingrese un precio válido mayor a 0");
                return false;
            }

            string categoriaSeleccionada = ddlCategoria.SelectedValue;
            System.Diagnostics.Debug.WriteLine($"Categoría seleccionada: {categoriaSeleccionada}");
            System.Diagnostics.Debug.WriteLine($"Items en dropdown: {ddlCategoria.Items.Count}");

            if (string.IsNullOrEmpty(categoriaSeleccionada) || categoriaSeleccionada == "0")
            {
                MostrarMensaje($"Por favor seleccione una categoría válida. Categorías disponibles: {ddlCategoria.Items.Count}");
                return false;
            }

            if (!int.TryParse(categoriaSeleccionada, out int catId) || catId <= 0)
            {
                MostrarMensaje("Por favor seleccione una categoría válida");
                return false;
            }

            return true;
        }

        #endregion

        #region Ver Detalle Producto

        protected void gvProductos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "VerDetalle")
            {
                int idProducto = Convert.ToInt32(e.CommandArgument);
                MostrarDetalleProducto(idProducto);
            }
        }

        private void MostrarDetalleProducto(int idProducto)
        {
            try
            {
                ProductoWSClient cliente = new ProductoWSClient();
                var producto = cliente.obtenerProducto(idProducto);

                if (producto != null)
                {
                    hdnProductoId.Value = idProducto.ToString();

                    // Título del modal
                    litDetalleNombre.Text = producto.nombre ?? "Producto Sin Nombre";

                    // Información principal
                    litNombre.Text = producto.nombre ?? "---";
                    litIdProducto.Text = producto.idProducto > 0 ? producto.idProducto.ToString("D4") : "---";
                    litCategoria.Text = producto.categoria?.nombre ?? "Sin categoría";
                    litMarca.Text = !string.IsNullOrEmpty(producto.marca) ? producto.marca : "---";
                    litDescripcion.Text = !string.IsNullOrEmpty(producto.descripcion) ? producto.descripcion : "Sin descripción";
                    litPrecio.Text = $"₹{producto.precioUnitario:N2}";

                    // Información de stock
                    litStockMax.Text = producto.stockMaximo.ToString();
                    litStockMin.Text = producto.stockMinimo.ToString();

                    // Obtener stock actual desde ExistenciasWS
                    int stockActual = ObtenerStockActual(idProducto);
                    litStockActual.Text = stockActual.ToString();

                    System.Diagnostics.Debug.WriteLine($"Mostrando detalle de: {producto.nombre}");
                    System.Diagnostics.Debug.WriteLine($"  ID: {producto.idProducto}");
                    System.Diagnostics.Debug.WriteLine($"  Precio: ₹{producto.precioUnitario:N2}");
                    System.Diagnostics.Debug.WriteLine($"  Stock: {stockActual}/{producto.stockMaximo}");

                    ScriptManager.RegisterStartupScript(this, GetType(), "abrirDetalle",
                        "document.getElementById('detalleProductoModal').style.display='flex';", true);
                }
                else
                {
                    MostrarMensaje("No se pudo cargar el detalle del producto");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al mostrar detalle: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                MostrarMensaje("Error al cargar el detalle del producto");
            }
        }

        private int ObtenerStockActual(int idProducto)
        {
            try
            {
                ExistenciasWSClient existenciasCliente = new ExistenciasWSClient();
                var existencias = existenciasCliente.listarExistencias();

                if (existencias != null)
                {
                    var existencia = existencias.FirstOrDefault(e =>
                        e.producto != null && e.producto.idProducto == idProducto);

                    if (existencia != null)
                    {
                        // Intenta acceder a la propiedad usando reflection
                        var tipo = existencia.GetType();
                        var propiedades = tipo.GetProperties();

                        foreach (var prop in propiedades)
                        {
                            var nombreProp = prop.Name.ToLower();
                            if (nombreProp.Contains("cantidad") || nombreProp.Contains("stock") || nombreProp.Contains("actual"))
                            {
                                var valor = prop.GetValue(existencia);
                                if (valor != null && int.TryParse(valor.ToString(), out int cantidad))
                                {
                                    return cantidad;
                                }
                            }
                        }
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener stock: {ex.Message}");
                return 0;
            }
        }

        #endregion

        #region Editar Producto

        protected void btnEditDetalle_Click(object sender, EventArgs e)
        {
            try
            {
                int idProducto = int.Parse(hdnProductoId.Value);

                ProductoWSClient cliente = new ProductoWSClient();
                var producto = cliente.obtenerProducto(idProducto);

                if (producto != null)
                {
                    // Recargar categorías antes de editar
                    CargarCategorias();

                    // Cargar datos en el formulario
                    txtProductName.Text = producto.nombre;
                    txtPrecioUnitario.Text = producto.precioUnitario.ToString("F2");
                    txtDescripcion.Text = producto.descripcion;
                    txtMarca.Text = producto.marca;

                    if (producto.categoria != null)
                    {
                        string idCat = producto.categoria.idCategoria.ToString();
                        System.Diagnostics.Debug.WriteLine($"Intentando seleccionar categoría ID: {idCat}");

                        ListItem item = ddlCategoria.Items.FindByValue(idCat);
                        if (item != null)
                        {
                            ddlCategoria.SelectedValue = idCat;
                            System.Diagnostics.Debug.WriteLine($"Categoría seleccionada: {item.Text}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"No se encontró la categoría con ID: {idCat}");
                        }
                    }

                    // Configurar modo edición
                    ViewState["EditMode"] = true;
                    ConfigurarBotonGuardar(true);
                    litModalTitle.Text = "Editar Producto";

                    ScriptManager.RegisterStartupScript(this, GetType(), "editarModal",
                        "cerrarDetalleModal(); abrirModal();", true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al editar: {ex.Message}");
                MostrarMensaje("Error al cargar el producto para editar");
            }
        }

        #endregion

        #region Eliminar Producto

        protected void btnEliminarDetalle_Click(object sender, EventArgs e)
        {
            try
            {
                int idProducto = int.Parse(hdnProductoId.Value);

                ProductoWSClient cliente = new ProductoWSClient();
                cliente.eliminarProducto(idProducto);

                CargarProductos();

                ScriptManager.RegisterStartupScript(this, GetType(), "deleted",
                    "cerrarDetalleModal(); alert('✅ Producto eliminado correctamente');", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al eliminar: {ex.Message}");
                MostrarMensaje("Error al eliminar el producto");
            }
        }

        #endregion

        #region Utilidades

        private void ConfigurarBotonGuardar(bool esEdicion)
        {
            if (esEdicion)
            {
                btnSaveProduct.Text = "Actualizar";
                litModalTitle.Text = "Editar Producto";
            }
            else
            {
                btnSaveProduct.Text = "Agregar";
                litModalTitle.Text = "Agregar Producto";
            }
        }

        private void LimpiarFormulario()
        {
            txtProductName.Text = string.Empty;
            txtPrecioUnitario.Text = string.Empty;
            txtDescripcion.Text = string.Empty;
            txtMarca.Text = string.Empty;
            ddlCategoria.SelectedIndex = 0;
            hdnProductoId.Value = string.Empty;
            ViewState["EditMode"] = null;
            ConfigurarBotonGuardar(false);
        }

        private void MostrarMensaje(string mensaje)
        {
            string script = $"alert('{mensaje.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "mensaje", script, true);
        }

        #endregion
    }
}