<%@ Page Title="" Language="C#" MasterPageFile="~/Stockify.Master" AutoEventWireup="true" 
    CodeBehind="Inventario.aspx.cs" Inherits="StockifyWeb.Inventario" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        :root {
            --bg: #0b0c0f;
            --card: #21252d;
            --card2: #2a2f39;
            --stroke: #323844;
            --text: #e7eaf0;
            --muted: #a9b3c7;
            --accent: #8aa2ff;
            --danger: #ff6b6b;
            --radius: 16px;
            --shadow: 0 10px 24px rgba(0,0,0,.35);
        }

        .products-container {
            background: var(--bg);
            padding: 22px 28px;
            border-radius: var(--radius);
            color: var(--text);
        }

        .products-container h1 {
            color: var(--text);
            margin: 0 0 20px 0;
            font-size: 24px;
            font-weight: 600;
        }

        .header-actions {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
            flex-wrap: wrap;
            gap: 16px;
        }

        .search-box {
            flex: 1;
            min-width: 300px;
            position: relative;
        }

        .search-box input {
            width: 100%;
            padding: 10px 40px 10px 15px;
            background: #121419;
            border: 1px solid var(--stroke);
            border-radius: 40px;
            font-size: 14px;
            color: var(--text);
        }

        .search-box input:focus {
            outline: none;
            border-color: var(--accent);
        }

        .search-box i {
            position: absolute;
            right: 15px;
            top: 50%;
            transform: translateY(-50%);
            color: var(--muted);
        }

        .action-buttons {
            display: flex;
            gap: 10px;
        }

        .btn-filter, .btn-add {
            padding: 10px 20px;
            border-radius: var(--radius);
            cursor: pointer;
            display: flex;
            align-items: center;
            gap: 5px;
            transition: all 0.3s;
            border: none;
        }

        .btn-filter {
            background: var(--card2);
            color: var(--text);
            border: 1px solid var(--stroke);
        }

        .btn-add {
            background: var(--card);
            color: var(--accent);
            border: 1px solid var(--accent);
        }

        .btn-filter:hover {
            background: var(--stroke);
        }

        .btn-add:hover {
            background: var(--accent);
            color: var(--bg);
        }

        .products-table {
            width: 100%;
            border-collapse: collapse;
            margin: 20px 0;
            background: var(--card);
            border-radius: var(--radius);
            overflow: hidden;
            box-shadow: var(--shadow);
        }

        .products-table th,
        .products-table td {
            padding: 12px;
            text-align: left;
            border-bottom: 1px solid var(--stroke);
            color: var(--text);
        }

        .products-table th {
            background: var(--card2);
            font-weight: 600;
        }

        .products-table tbody tr:hover {
            background: var(--card2);
        }

        .btn-detalle {
            background: var(--accent);
            color: var(--bg);
            border: none;
            padding: 8px 16px;
            border-radius: 8px;
            cursor: pointer;
            font-size: 13px;
            font-weight: 600;
            transition: all 0.3s;
        }

        .btn-detalle:hover {
            background: #9ab1ff;
            transform: translateY(-1px);
        }

        .modal-overlay {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.8);
            z-index: 1000;
            justify-content: center;
            align-items: center;
        }

        .modal-content {
            background: #1a1d24;
            border: 1px solid #323844;
            border-radius: var(--radius);
            padding: 28px;
            width: 90%;
            max-width: 500px;
            max-height: 90vh;
            overflow-y: auto;
            box-shadow: 0 20px 60px rgba(0,0,0,.6);
        }

        .modal-detalle {
            max-width: 750px;
        }

        .modal-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
            border-bottom: 1px solid var(--stroke);
            padding-bottom: 15px;
        }

        .modal-title {
            color: #ffffff;
            font-size: 22px;
            font-weight: 700;
            margin: 0;
        }

        .detalle-actions {
            display: flex;
            gap: 10px;
            align-items: center;
        }

        .close-modal {
            background: none;
            border: none;
            color: var(--muted);
            font-size: 24px;
            cursor: pointer;
            padding: 0;
            width: 30px;
            height: 30px;
        }

        .close-modal:hover {
            color: var(--text);
        }

        .form-group {
            margin-bottom: 15px;
        }

        .form-group label {
            display: block;
            color: var(--muted);
            margin-bottom: 5px;
            font-size: 14px;
            font-weight: 500;
        }

        .form-control {
            width: 100%;
            padding: 10px 12px;
            background: var(--bg);
            border: 1px solid var(--stroke);
            border-radius: 8px;
            color: var(--text);
            font-size: 14px;
            box-sizing: border-box;
        }

        .form-control:focus {
            outline: none;
            border-color: var(--accent);
        }

        .modal-actions {
            display: flex;
            justify-content: flex-end;
            gap: 10px;
            margin-top: 20px;
            border-top: 1px solid var(--stroke);
            padding-top: 20px;
        }

        .btn-submit, .btn-discard, .btn-edit, .btn-delete {
            padding: 10px 20px;
            border-radius: 8px;
            cursor: pointer;
            transition: all 0.3s;
            font-weight: 600;
        }

        .btn-submit {
            background: var(--accent);
            color: var(--bg);
            border: none;
        }

        .btn-discard {
            background: var(--card2);
            color: var(--text);
            border: 1px solid var(--stroke);
        }

        .btn-edit {
            background: #2a2f39;
            color: #ffffff;
            border: 1px solid #3d4454;
            padding: 9px 18px;
            font-size: 14px;
        }

        .btn-delete {
            background: transparent;
            color: #ff6b6b;
            border: 1px solid #ff6b6b;
            padding: 9px 18px;
            font-size: 14px;
        }

        .btn-submit:hover {
            background: #9ab1ff;
        }

        .btn-discard:hover, .btn-edit:hover {
            background: var(--stroke);
        }

        .btn-delete:hover {
            background: var(--danger);
            color: var(--bg);
        }

        .section-title {
            color: #ffffff;
            font-size: 17px;
            font-weight: 700;
            margin-bottom: 18px;
            margin-top: 10px;
            padding-bottom: 10px;
            border-bottom: 2px solid #323844;
        }

        .detalle-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 15px;
            margin-bottom: 20px;
        }

        .detalle-field {
            display: flex;
            flex-direction: column;
            gap: 5px;
        }

        .detalle-field.full-width {
            grid-column: 1 / -1;
        }

        .detalle-label {
            color: #8a95aa;
            font-size: 13px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .detalle-value {
            color: #ffffff;
            font-size: 15px;
            padding: 12px 15px;
            background: #16181d;
            border-radius: 8px;
            border: 1px solid #2a2f39;
            min-height: 20px;
        }

        .stock-section {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 15px;
            margin-top: 20px;
        }

        .stock-card {
            background: #16181d;
            padding: 20px 15px;
            border-radius: 12px;
            text-align: center;
            border: 1px solid #2a2f39;
        }

        .stock-label {
            color: #8a95aa;
            font-size: 12px;
            margin-bottom: 10px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .stock-value {
            color: #ffffff;
            font-size: 28px;
            font-weight: 700;
        }

        .product-image-section {
            margin-bottom: 20px;
            text-align: center;
        }

        .product-image {
            max-width: 200px;
            max-height: 200px;
            background: white;
            padding: 15px;
            border-radius: 12px;
            margin: 0 auto;
        }

        @media (max-width: 768px) {
            .header-actions {
                flex-direction: column;
            }
            .detalle-grid, .stock-section {
                grid-template-columns: 1fr;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cph_Contenido" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />
    
    <div class="products-container">
        <div class="header-actions">
            <div class="search-box">
                <input type="text" placeholder="Buscar productos..." id="txtBuscar" />
                <i class="fas fa-search"></i>
            </div>
            <div class="action-buttons">
                <button class="btn-filter" type="button">
                    <i class="fas fa-filter"></i> Filtros
                </button>
                <asp:Button ID="btnOpenModal" runat="server" Text="Agregar Producto" 
                    CssClass="btn-add" OnClick="btnOpenModal_Click" />
            </div>
        </div>

        <h1>Productos</h1>
        
        <asp:GridView ID="gvProductos" runat="server" AutoGenerateColumns="false" 
            CssClass="products-table" OnRowCommand="gvProductos_RowCommand">
            <Columns>
                <asp:BoundField DataField="Producto" HeaderText="Producto" />
                <asp:BoundField DataField="Precio" HeaderText="Precio" DataFormatString="₹{0:N2}" />
                <asp:BoundField DataField="Descripcion" HeaderText="Descripción" />
                <asp:BoundField DataField="Marca" HeaderText="Marca" />
                <asp:BoundField DataField="Categoria" HeaderText="Categoría" />
                <asp:TemplateField HeaderText="Acciones">
                    <ItemTemplate>
                        <asp:Button ID="btnVerDetalle" runat="server" Text="Ver Detalle" 
                            CssClass="btn-detalle" CommandName="VerDetalle" 
                            CommandArgument='<%# Eval("IdProducto") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <!-- Modal Agregar/Editar Producto -->
    <div class="modal-overlay" id="addProductModal">
        <div class="modal-content">
            <div class="modal-header">
                <h2 class="modal-title">
                    <asp:Literal ID="litModalTitle" runat="server" Text="Agregar Producto" />
                </h2>
                <button class="close-modal" type="button" onclick="cerrarModal()">&times;</button>
            </div>
            
            <div class="form-group">
                <label>Nombre del producto</label>
                <asp:TextBox ID="txtProductName" runat="server" CssClass="form-control" 
                    placeholder="Ingrese nombre"></asp:TextBox>
            </div>
            
            <div class="form-group">
                <label>Categoría</label>
                <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-control" />
            </div>
            
            <div class="form-group">
                <label>Precio unitario</label>
                <asp:TextBox ID="txtPrecioUnitario" runat="server" CssClass="form-control" 
                    placeholder="0.00"></asp:TextBox>
            </div>
            
            <div class="form-group">
                <label>Descripción</label>
                <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" 
                    TextMode="MultiLine" Rows="3"></asp:TextBox>
            </div>
            
            <div class="form-group">
                <label>Marca</label>
                <asp:TextBox ID="txtMarca" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            
            <div class="modal-actions">
                <button type="button" class="btn-discard" onclick="cerrarModal()">Cancelar</button>
                <asp:Button ID="btnSaveProduct" runat="server" 
                    CssClass="btn-submit" OnClick="btnSaveProduct_Click" />
            </div>
        </div>
    </div>

    <!-- Modal Ver Detalle -->
    <div class="modal-overlay" id="detalleProductoModal">
        <div class="modal-content modal-detalle">
            <div class="modal-header">
                <h2 class="modal-title">
                    <asp:Literal ID="litDetalleNombre" runat="server" Text="Detalle del Producto" />
                </h2>
                <div class="detalle-actions">
                    <asp:Button ID="btnEditDetalle" runat="server" Text="✏️ Editar" 
                        CssClass="btn-edit" OnClick="btnEditDetalle_Click" />
                    <asp:Button ID="btnEliminarDetalle" runat="server" Text="🗑️ Eliminar" 
                        CssClass="btn-delete" OnClick="btnEliminarDetalle_Click"
                        OnClientClick="return confirm('¿Está seguro de eliminar este producto?');" />
                    <button class="close-modal" type="button" onclick="cerrarDetalleModal()" title="Cerrar">&times;</button>
                </div>
            </div>
            
            <h3 class="section-title">📋 Información del Producto</h3>
            
            <div class="detalle-grid">
                <div class="detalle-field">
                    <span class="detalle-label">Nombre del producto</span>
                    <div class="detalle-value">
                        <asp:Literal ID="litNombre" runat="server" Text="---" />
                    </div>
                </div>
                
                <div class="detalle-field">
                    <span class="detalle-label">ID Producto</span>
                    <div class="detalle-value">
                        <asp:Literal ID="litIdProducto" runat="server" Text="---" />
                    </div>
                </div>
                
                <div class="detalle-field">
                    <span class="detalle-label">Categoría</span>
                    <div class="detalle-value">
                        <asp:Literal ID="litCategoria" runat="server" Text="---" />
                    </div>
                </div>
                
                <div class="detalle-field">
                    <span class="detalle-label">Marca</span>
                    <div class="detalle-value">
                        <asp:Literal ID="litMarca" runat="server" Text="---" />
                    </div>
                </div>
                
                <div class="detalle-field">
                    <span class="detalle-label">Precio Unitario</span>
                    <div class="detalle-value">
                        <asp:Literal ID="litPrecio" runat="server" Text="---" />
                    </div>
                </div>
                
                <div class="detalle-field full-width">
                    <span class="detalle-label">Descripción</span>
                    <div class="detalle-value">
                        <asp:Literal ID="litDescripcion" runat="server" Text="---" />
                    </div>
                </div>
            </div>
            
            <h3 class="section-title">📦 Información de Stock</h3>
            
            <div class="stock-section">
                <div class="stock-card">
                    <div class="stock-label">Stock Máximo</div>
                    <div class="stock-value">
                        <asp:Literal ID="litStockMax" runat="server" Text="0" />
                    </div>
                </div>
                <div class="stock-card">
                    <div class="stock-label">Stock Actual</div>
                    <div class="stock-value">
                        <asp:Literal ID="litStockActual" runat="server" Text="0" />
                    </div>
                </div>
                <div class="stock-card">
                    <div class="stock-label">Stock Mínimo</div>
                    <div class="stock-value">
                        <asp:Literal ID="litStockMin" runat="server" Text="0" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="hdnProductoId" runat="server" />

    <script>
        // Búsqueda en tiempo real
        document.getElementById('txtBuscar').addEventListener('keyup', function () {
            var filtro = this.value.toLowerCase();
            var filas = document.querySelectorAll('.products-table tbody tr');

            filas.forEach(function (fila) {
                if (fila.cells[0]) {
                    var texto = Array.from(fila.cells).map(c => c.textContent.toLowerCase()).join(' ');
                    fila.style.display = texto.includes(filtro) ? '' : 'none';
                }
            });
        });

        // Modales
        function abrirModal() {
            document.getElementById('addProductModal').style.display = 'flex';
        }

        function cerrarModal() {
            document.getElementById('addProductModal').style.display = 'none';
        }

        function cerrarDetalleModal() {
            document.getElementById('detalleProductoModal').style.display = 'none';
        }

        // Cerrar al hacer clic fuera
        document.getElementById('addProductModal').addEventListener('click', function (e) {
            if (e.target === this) cerrarModal();
        });

        document.getElementById('detalleProductoModal').addEventListener('click', function (e) {
            if (e.target === this) cerrarDetalleModal();
        });
    </script>
</asp:Content>