using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Data;

namespace JorStock
{
    public partial class Home : Form
    {

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );
        public Home()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
            ConfigurarPlaceholders();
            ConfigurarEventosTextChanged();
            ConfigurarMenuOrdenamiento();
            btnLimpiarBuscar.Click += btnLimpiarBuscar_Click;
            btnEditar.Click += btnEditar_Click;
            tblProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void ConfigurarMenuOrdenamiento()
        {
            menuOrdenar = new ContextMenuStrip();

            menuOrdenar.Items.Add("Nombre de Productos (A - Z)", null, OrdenarProductos_Click);
            menuOrdenar.Items.Add("Nombre de Productos (Z - A)", null, OrdenarProductos_Click);
            menuOrdenar.Items.Add("Más stock", null, OrdenarProductos_Click);
            menuOrdenar.Items.Add("Menos stock", null, OrdenarProductos_Click);
            menuOrdenar.Items.Add("Mayor precio", null, OrdenarProductos_Click);
            menuOrdenar.Items.Add("Menor precio", null, OrdenarProductos_Click);
            menuOrdenar.Items.Add("Fecha reciente", null, OrdenarProductos_Click);
            menuOrdenar.Items.Add("Fecha antigua", null, OrdenarProductos_Click);

            button5.Click += button5_Click;
        }

        private void ConfigurarPlaceholders()
        {
            txtAutoparte.Text = "Nombre de Autoparte";
            txtPrecio.Text = "Precio";
            txtProveedor.Text = "Proveedor";
            txtStock.Text = "Stock";
            txtCodeAut.Text = "Código";
            txtNomAuto.Text = "Nombre de Autoparte";
            txtProv.Text = "Proveedor";

            ConfigurarEventosPlaceholder(txtAutoparte, "Nombre de Autoparte");
            ConfigurarEventosPlaceholder(txtPrecio, "Precio");
            ConfigurarEventosPlaceholder(txtProveedor, "Proveedor");
            ConfigurarEventosPlaceholder(txtStock, "Stock");
            ConfigurarEventosPlaceholder(txtCodeAut, "Código");
            ConfigurarEventosPlaceholder(txtNomAuto, "Nombre de Autoparte");
            ConfigurarEventosPlaceholder(txtProv, "Proveedor");
        }

        private void ConfigurarEventosPlaceholder(TextBox textBox, string placeholderText)
        {
            textBox.ForeColor = Color.Gray;

            textBox.Enter += (sender, e) =>
            {
                if (textBox.Text == placeholderText)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                }
            };

            textBox.Leave += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholderText;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }

        private void Home_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();
            CargarProductos();
        }

        private async void CargarProductos()
        {
            try
            {
                var client = new MongoClient("mongodb://localhost:27017");
                var database = client.GetDatabase("JorStock");
                var productosCollection = database.GetCollection<BsonDocument>("productos");
                var proveedoresCollection = database.GetCollection<BsonDocument>("proveedores");

                var productos = await productosCollection.Find(new BsonDocument()).ToListAsync();

                DataTable dt = new DataTable();
                dt.Columns.Add("Nombre", typeof(string));
                dt.Columns.Add("Stock", typeof(int));
                dt.Columns.Add("Precio Unitario", typeof(decimal));
                dt.Columns.Add("Fecha de Ingreso", typeof(DateTime));
                dt.Columns.Add("Serial", typeof(string));
                dt.Columns.Add("Código de Proveedor", typeof(string));
                dt.Columns.Add("Nombre Proveedor", typeof(string));

                dt.Columns.Add("Grupo Proveedor", typeof(int));

                Dictionary<string, int> gruposProveedores = new Dictionary<string, int>();
                int contadorGrupos = 1;

                List<Tuple<BsonDocument, DateTime>> productosConFecha = new List<Tuple<BsonDocument, DateTime>>();

                foreach (var producto in productos)
                {
                    if (producto.Contains("codigo_proveedor"))
                    {
                        string codigoProveedor = producto["codigo_proveedor"].ToString();
                        if (!gruposProveedores.ContainsKey(codigoProveedor))
                        {
                            gruposProveedores.Add(codigoProveedor, contadorGrupos++);
                        }
                    }
                }

                foreach (var producto in productos)
                {
                    DateTime fechaProducto = DateTime.Now;
                    if (producto.Contains("fecha"))
                    {
                        if (DateTime.TryParse(producto["fecha"].ToString(), out DateTime fechaParseada))
                        {
                            fechaProducto = fechaParseada;
                        }
                    }
                    productosConFecha.Add(new Tuple<BsonDocument, DateTime>(producto, fechaProducto));
                }

                var productosOrdenados = productosConFecha
                    .OrderBy(p =>
                    {
                        if (p.Item1.Contains("codigo_proveedor"))
                        {
                            string codigo = p.Item1["codigo_proveedor"].ToString();
                            return gruposProveedores.ContainsKey(codigo) ? gruposProveedores[codigo] : 999;
                        }
                        return 999; 
                    })
                    .ThenByDescending(p => p.Item2)
                    .Select(p => p.Item1);

                foreach (var producto in productosOrdenados)
                {
                    DataRow row = dt.NewRow();

                    row["Nombre"] = producto.Contains("nombre") ? producto["nombre"].ToString() : string.Empty;
                    row["Stock"] = producto.Contains("stock") ? producto["stock"].ToInt32() : 0;
                    row["Precio Unitario"] = producto.Contains("precio_unitario") ? producto["precio_unitario"].ToDecimal() : 0M;

                    if (producto.Contains("fecha"))
                    {
                        if (DateTime.TryParse(producto["fecha"].ToString(), out DateTime fechaParseada))
                        {
                            row["Fecha de Ingreso"] = fechaParseada;
                        }
                        else
                        {
                            row["Fecha de Ingreso"] = DateTime.Now;
                        }
                    }
                    else
                    {
                        row["Fecha de Ingreso"] = DateTime.Now;
                    }

                    row["Serial"] = producto.Contains("serial") ? producto["serial"].ToString() : string.Empty;

                    string codigoProveedor = producto.Contains("codigo_proveedor") ? producto["codigo_proveedor"].ToString() : string.Empty;
                    row["Código de Proveedor"] = codigoProveedor;

                    if (!string.IsNullOrEmpty(codigoProveedor) && gruposProveedores.ContainsKey(codigoProveedor))
                    {
                        row["Grupo Proveedor"] = gruposProveedores[codigoProveedor];
                    }
                    else
                    {
                        row["Grupo Proveedor"] = 0;
                    }

                    string nombreProveedor = string.Empty;
                    if (!string.IsNullOrEmpty(codigoProveedor))
                    {
                        try
                        {
                            if (ObjectId.TryParse(codigoProveedor, out ObjectId proveedorId))
                            {
                                var filtroProveedor = Builders<BsonDocument>.Filter.Eq("_id", proveedorId);
                                var proveedor = await proveedoresCollection.Find(filtroProveedor).FirstOrDefaultAsync();
                                if (proveedor != null && proveedor.Contains("nombre"))
                                {
                                    nombreProveedor = proveedor["nombre"].ToString();
                                }
                            }
                            else
                            {
                                var filtroProveedor = Builders<BsonDocument>.Filter.Eq("_id", codigoProveedor);
                                var proveedor = await proveedoresCollection.Find(filtroProveedor).FirstOrDefaultAsync();
                                if (proveedor != null && proveedor.Contains("nombre"))
                                {
                                    nombreProveedor = proveedor["nombre"].ToString();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error al buscar proveedor: {ex.Message}");
                        }
                    }

                    row["Nombre Proveedor"] = nombreProveedor;

                    dt.Rows.Add(row);
                }

                tblProductos.DataSource = dt;

                tblProductos.Columns["Grupo Proveedor"].Visible = false;

                ConfigurarAparienciaGruposProveedores();

                tblProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                tblProductos.AllowUserToAddRows = false;
                tblProductos.AllowUserToDeleteRows = false;
                tblProductos.ReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los productos: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigurarAparienciaGruposProveedores()
        {
            tblProductos.CellFormatting -= TblProductos_CellFormatting;

            tblProductos.CellFormatting += TblProductos_CellFormatting;
        }

        private void TblProductos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && tblProductos.Columns.Contains("Grupo Proveedor"))
            {
                DataGridViewRow row = tblProductos.Rows[e.RowIndex];

                int grupoProveedor = 0;
                if (row.Cells["Grupo Proveedor"].Value != null &&
                    int.TryParse(row.Cells["Grupo Proveedor"].Value.ToString(), out int grupo))
                {
                    grupoProveedor = grupo;
                }

                if (grupoProveedor > 0)
                {
                    if (grupoProveedor % 2 == 0)
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
                    }
                    else
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
                    }
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private string productoEnEdicionId = null;
        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtAutoparte.Text == "Nombre de Autoparte" ||
                    txtPrecio.Text == "Precio" ||
                    txtStock.Text == "Stock" ||
                    txtProveedor.Text == "Proveedor" ||
                    txtCodeAut.Text == "Código")
                {
                    MessageBox.Show("Por favor, complete todos los campos antes de guardar.",
                        "Campos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtPrecio.Text, out decimal precio))
                {
                    MessageBox.Show("El precio debe ser un valor numérico.",
                        "Error de formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!int.TryParse(txtStock.Text, out int stock))
                {
                    MessageBox.Show("El stock debe ser un valor numérico entero.",
                        "Error de formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var client = new MongoClient("mongodb://localhost:27017");
                var database = client.GetDatabase("JorStock");

                var proveedoresCollection = database.GetCollection<BsonDocument>("proveedores");

                var filtroProveedor = Builders<BsonDocument>.Filter.Eq("nombre", txtProveedor.Text);
                var proveedorExistente = await proveedoresCollection.Find(filtroProveedor).FirstOrDefaultAsync();

                string codigoProveedor;

                if (proveedorExistente == null)
                {
                    var nuevoProveedor = new BsonDocument
    {
        { "nombre", txtProveedor.Text },
        { "fecha_registro", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
    };

                    await proveedoresCollection.InsertOneAsync(nuevoProveedor);

                    proveedorExistente = await proveedoresCollection.Find(filtroProveedor).FirstOrDefaultAsync();
                    codigoProveedor = proveedorExistente["_id"].ToString();
                }
                else
                {
                    codigoProveedor = proveedorExistente["_id"].ToString();
                }

                var productosCollection = database.GetCollection<BsonDocument>("productos");

                if (productoEnEdicionId != null)
                {
                    try
                    {
                        var filtroProducto = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(productoEnEdicionId));

                        var actualizacion = Builders<BsonDocument>.Update
                            .Set("nombre", txtAutoparte.Text)
                            .Set("precio_unitario", precio)
                            .Set("stock", stock)
                            .Set("serial", txtCodeAut.Text)
                            .Set("codigo_proveedor", codigoProveedor);

                        var resultado = await productosCollection.UpdateOneAsync(filtroProducto, actualizacion);

                        if (resultado.ModifiedCount > 0)
                        {
                            MessageBox.Show("Producto actualizado exitosamente.",
                                "Actualización exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("No se pudo actualizar el producto. Por favor, intente nuevamente.",
                                "Error de actualización", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        productoEnEdicionId = null;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al actualizar el producto: {ex.Message}",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    var nuevoProducto = new BsonDocument
            {
                { "nombre", txtAutoparte.Text },
                { "precio_unitario", precio },
                { "stock", stock },
                { "fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                { "serial", txtCodeAut.Text },
                { "codigo_proveedor", codigoProveedor }
            };

                    await productosCollection.InsertOneAsync(nuevoProducto);

                    MessageBox.Show("Producto guardado exitosamente.",
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                LimpiarCamposRegistro();

                CargarProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al procesar el producto: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiarRegistrar_Click(object sender, EventArgs e)
        {
            LimpiarCamposRegistro();
        }

        private void LimpiarCamposRegistro()
        {
            txtAutoparte.Text = "Nombre de Autoparte";
            txtAutoparte.ForeColor = Color.Gray;

            txtPrecio.Text = "Precio";
            txtPrecio.ForeColor = Color.Gray;

            txtStock.Text = "Stock";
            txtStock.ForeColor = Color.Gray;

            txtProveedor.Text = "Proveedor";
            txtProveedor.ForeColor = Color.Gray;

            txtCodeAut.Text = "Código";
            txtCodeAut.ForeColor = Color.Gray;

            productoEnEdicionId = null;
        }

        private void tblProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                tblProductos.Rows[e.RowIndex].Selected = true;
            }
        }

        private async void btnBuscar_Click(object sender, EventArgs e)
        {
            string nombreBusqueda = txtNomAuto.Text;
            string proveedorBusqueda = txtProv.Text;

            bool nombreVacio = nombreBusqueda == "Nombre de Autoparte" || string.IsNullOrWhiteSpace(nombreBusqueda);
            bool proveedorVacio = proveedorBusqueda == "Proveedor" || string.IsNullOrWhiteSpace(proveedorBusqueda);

            if (nombreVacio && proveedorVacio)
            {
                CargarProductos();
                return;
            }

            try
            {
                var client = new MongoClient("mongodb://localhost:27017");
                var database = client.GetDatabase("JorStock");
                var productosCollection = database.GetCollection<BsonDocument>("productos");
                var proveedoresCollection = database.GetCollection<BsonDocument>("proveedores");

                var filtros = new List<FilterDefinition<BsonDocument>>();

                if (!nombreVacio)
                {
                    var filtroNombre = Builders<BsonDocument>.Filter.Regex("nombre",
                        new BsonRegularExpression(nombreBusqueda, "i"));
                    filtros.Add(filtroNombre);
                }

                if (!proveedorVacio)
                {
                    var filtroProveedorNombre = Builders<BsonDocument>.Filter.Regex("nombre",
                        new BsonRegularExpression(proveedorBusqueda, "i"));
                    var proveedores = await proveedoresCollection.Find(filtroProveedorNombre).ToListAsync();

                    if (proveedores.Any())
                    {
                        var filtrosProveedor = new List<FilterDefinition<BsonDocument>>();
                        foreach (var proveedor in proveedores)
                        {
                            var proveedorId = proveedor["_id"].ToString();
                            filtrosProveedor.Add(Builders<BsonDocument>.Filter.Eq("codigo_proveedor", proveedorId));
                        }
                        filtros.Add(Builders<BsonDocument>.Filter.Or(filtrosProveedor));
                    }
                    else
                    {
                        MessageBox.Show($"No se encontraron proveedores con el nombre '{proveedorBusqueda}'.",
                            "Proveedor no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }

                if (filtros.Count == 0)
                {
                    CargarProductos();
                    return;
                }

                FilterDefinition<BsonDocument> filtroFinal;
                if (filtros.Count > 1)
                {
                    filtroFinal = Builders<BsonDocument>.Filter.And(filtros);
                }
                else
                {
                    filtroFinal = filtros[0];
                }

                var productos = await productosCollection.Find(filtroFinal).ToListAsync();

                if (productos.Count == 0)
                {
                    MessageBox.Show("No se encontraron productos que coincidan con los criterios de búsqueda.",
                        "Sin resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DataTable dt = new DataTable();
                dt.Columns.Add("Nombre", typeof(string));
                dt.Columns.Add("Stock", typeof(int));
                dt.Columns.Add("Precio Unitario", typeof(decimal));
                dt.Columns.Add("Fecha de Ingreso", typeof(DateTime));
                dt.Columns.Add("Serial", typeof(string));
                dt.Columns.Add("Código de Proveedor", typeof(string));
                dt.Columns.Add("Nombre Proveedor", typeof(string));

                List<Tuple<BsonDocument, DateTime>> productosConFecha = new List<Tuple<BsonDocument, DateTime>>();

                foreach (var producto in productos)
                {
                    DateTime fechaProducto = DateTime.Now;
                    if (producto.Contains("fecha"))
                    {
                        if (DateTime.TryParse(producto["fecha"].ToString(), out DateTime fechaParseada))
                        {
                            fechaProducto = fechaParseada;
                        }
                    }
                    productosConFecha.Add(new Tuple<BsonDocument, DateTime>(producto, fechaProducto));
                }

                var productosOrdenados = productosConFecha.OrderByDescending(p => p.Item2).Select(p => p.Item1);

                foreach (var producto in productosOrdenados)
                {
                    DataRow row = dt.NewRow();

                    row["Nombre"] = producto.Contains("nombre") ? producto["nombre"].ToString() : string.Empty;
                    row["Stock"] = producto.Contains("stock") ? producto["stock"].ToInt32() : 0;
                    row["Precio Unitario"] = producto.Contains("precio_unitario") ? producto["precio_unitario"].ToDecimal() : 0M;

                    if (producto.Contains("fecha"))
                    {
                        if (DateTime.TryParse(producto["fecha"].ToString(), out DateTime fechaParseada))
                        {
                            row["Fecha de Ingreso"] = fechaParseada;
                        }
                        else
                        {
                            row["Fecha de Ingreso"] = DateTime.Now;
                        }
                    }
                    else
                    {
                        row["Fecha de Ingreso"] = DateTime.Now;
                    }

                    row["Serial"] = producto.Contains("serial") ? producto["serial"].ToString() : string.Empty;

                    string codigoProveedor = producto.Contains("codigo_proveedor") ? producto["codigo_proveedor"].ToString() : string.Empty;
                    row["Código de Proveedor"] = codigoProveedor;

                    string nombreProveedor = string.Empty;
                    if (!string.IsNullOrEmpty(codigoProveedor))
                    {
                        try
                        {
                            if (ObjectId.TryParse(codigoProveedor, out ObjectId proveedorId))
                            {
                                var filtroProveedor = Builders<BsonDocument>.Filter.Eq("_id", proveedorId);
                                var proveedor = await proveedoresCollection.Find(filtroProveedor).FirstOrDefaultAsync();
                                if (proveedor != null && proveedor.Contains("nombre"))
                                {
                                    nombreProveedor = proveedor["nombre"].ToString();
                                }
                            }
                            else
                            {
                                var filtroProveedor = Builders<BsonDocument>.Filter.Eq("_id", codigoProveedor);
                                var proveedor = await proveedoresCollection.Find(filtroProveedor).FirstOrDefaultAsync();
                                if (proveedor != null && proveedor.Contains("nombre"))
                                {
                                    nombreProveedor = proveedor["nombre"].ToString();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error al buscar proveedor: {ex.Message}");
                        }
                    }

                    row["Nombre Proveedor"] = nombreProveedor;

                    dt.Rows.Add(row);
                }

                tblProductos.DataSource = dt;

                tblProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                tblProductos.AllowUserToAddRows = false;
                tblProductos.AllowUserToDeleteRows = false;
                tblProductos.ReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar productos: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void VerificarCamposBusquedaVacios(object sender, EventArgs e)
        {
            bool nombreVacio = txtNomAuto.Text == "Nombre de Autoparte" || string.IsNullOrWhiteSpace(txtNomAuto.Text);
            bool proveedorVacio = txtProv.Text == "Proveedor" || string.IsNullOrWhiteSpace(txtProv.Text);

            if (nombreVacio && proveedorVacio)
            {
                CargarProductos();
            }
        }
        private void ConfigurarEventosTextChanged()
        {
            txtNomAuto.TextChanged += VerificarCamposBusquedaVacios;
            txtProv.TextChanged += VerificarCamposBusquedaVacios;
        }

        private void btnLimpiarBuscar_Click(object sender, EventArgs e)
        {
            LimpiarCamposBusqueda();
            CargarProductos();
        }

        private void LimpiarCamposBusqueda()
        {
            txtNomAuto.Text = "Nombre de Autoparte";
            txtNomAuto.ForeColor = Color.Gray;

            txtProv.Text = "Proveedor";
            txtProv.ForeColor = Color.Gray;
        }

        private async void btnEditar_Click(object sender, EventArgs e)
        {
            if (tblProductos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione un producto para editar.",
                    "Ningún producto seleccionado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataGridViewRow filaSeleccionada = tblProductos.SelectedRows[0];

                string serial = filaSeleccionada.Cells["Serial"].Value.ToString();

                var client = new MongoClient("mongodb://localhost:27017");
                var database = client.GetDatabase("JorStock");
                var productosCollection = database.GetCollection<BsonDocument>("productos");

                var filtro = Builders<BsonDocument>.Filter.Eq("serial", serial);
                var producto = await productosCollection.Find(filtro).FirstOrDefaultAsync();

                if (producto == null)
                {
                    MessageBox.Show("No se pudo encontrar el producto en la base de datos.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                productoEnEdicionId = producto["_id"].ToString();

                txtAutoparte.Text = producto.Contains("nombre") ? producto["nombre"].ToString() : string.Empty;
                txtAutoparte.ForeColor = Color.Black;

                txtPrecio.Text = producto.Contains("precio_unitario") ?
                    producto["precio_unitario"].ToDecimal().ToString() : string.Empty;
                txtPrecio.ForeColor = Color.Black;

                txtStock.Text = producto.Contains("stock") ?
                    producto["stock"].ToInt32().ToString() : string.Empty;
                txtStock.ForeColor = Color.Black;

                txtCodeAut.Text = producto.Contains("serial") ?
                    producto["serial"].ToString() : string.Empty;
                txtCodeAut.ForeColor = Color.Black;

                if (producto.Contains("codigo_proveedor"))
                {
                    string codigoProveedor = producto["codigo_proveedor"].ToString();
                    var proveedoresCollection = database.GetCollection<BsonDocument>("proveedores");

                    if (ObjectId.TryParse(codigoProveedor, out ObjectId proveedorId))
                    {
                        var filtroProveedor = Builders<BsonDocument>.Filter.Eq("_id", proveedorId);
                        var proveedor = await proveedoresCollection.Find(filtroProveedor).FirstOrDefaultAsync();

                        if (proveedor != null && proveedor.Contains("nombre"))
                        {
                            txtProveedor.Text = proveedor["nombre"].ToString();
                            txtProveedor.ForeColor = Color.Black;
                        }
                    }
                }

                MessageBox.Show("Los datos del producto se han cargado para edición.\n" +
                    "Realice los cambios necesarios y presione 'Guardar' para actualizar.",
                    "Modo Edición", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar los datos para edición: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                productoEnEdicionId = null;
            }
        }

        private async void btnBorrar_Click(object sender, EventArgs e)
        {
            if (tblProductos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione un producto para eliminar.",
                    "Ningún producto seleccionado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataGridViewRow filaSeleccionada = tblProductos.SelectedRows[0];

                string nombreProducto = filaSeleccionada.Cells["Nombre"].Value.ToString();
                string serial = filaSeleccionada.Cells["Serial"].Value.ToString();

                DialogResult resultado = MessageBox.Show(
                    $"¿Está seguro que desea eliminar el producto '{nombreProducto}' con serial '{serial}'?\n\n" +
                    "Esta acción no se puede deshacer.",
                    "Confirmar eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (resultado == DialogResult.No)
                {
                    return;
                }


                var client = new MongoClient("mongodb://localhost:27017");
                var database = client.GetDatabase("JorStock");
                var productosCollection = database.GetCollection<BsonDocument>("productos");

                var filtro = Builders<BsonDocument>.Filter.Eq("serial", serial);

                var resultado_eliminacion = await productosCollection.DeleteOneAsync(filtro);

                if (resultado_eliminacion.DeletedCount > 0)
                {
                    MessageBox.Show($"El producto '{nombreProducto}' ha sido eliminado correctamente.",
                        "Eliminación exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CargarProductos();
                }
                else
                {
                    MessageBox.Show("No se pudo eliminar el producto. Es posible que ya haya sido eliminado.",
                        "Error al eliminar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar el producto: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private ContextMenuStrip menuOrdenar;


        private void button5_Click(object sender, EventArgs e)
        {
            menuOrdenar.Show(button5, new Point(0, button5.Height));
        }

        private void OrdenarProductos_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem opcionSeleccionada = (ToolStripMenuItem)sender;
            string criterioOrdenamiento = opcionSeleccionada.Text;

            if (tblProductos.DataSource is DataTable dt)
            {
                DataView vista = dt.DefaultView;

                switch (criterioOrdenamiento)
                {
                    case "Nombre de Productos (A - Z)":
                        vista.Sort = "Nombre ASC";
                        break;
                    case "Nombre de Productos (Z - A)":
                        vista.Sort = "Nombre DESC";
                        break;
                    case "Más stock":
                        vista.Sort = "Stock DESC";
                        break;
                    case "Menos stock":
                        vista.Sort = "Stock ASC";
                        break;
                    case "Mayor precio":
                        vista.Sort = "Precio Unitario DESC";
                        break;
                    case "Menor precio":
                        vista.Sort = "Precio Unitario ASC";
                        break;
                    case "Fecha reciente":
                        vista.Sort = "Fecha de Ingreso DESC";
                        break;
                    case "Fecha antigua":
                        vista.Sort = "Fecha de Ingreso ASC";
                        break;
                }

                tblProductos.DataSource = vista.ToTable();
            }
            else
            {
                MessageBox.Show("No hay datos para ordenar.", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
