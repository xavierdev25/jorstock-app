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
    /// <summary>
    /// Formulario principal de la aplicación JorStock
    /// Gestiona el inventario de autopartes con operaciones CRUD completas
    /// Incluye funcionalidades de búsqueda, ordenamiento y gestión de proveedores
    /// </summary>
    public partial class Home : Form
    {

        /// <summary>
        /// Importación de la función de Windows API para crear regiones con bordes redondeados
        /// Permite dar un aspecto moderno al formulario principal
        /// </summary>
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,      // Coordenada X del rectángulo izquierdo
            int nTopRect,       // Coordenada Y del rectángulo superior
            int nRightRect,     // Coordenada X del rectángulo derecho
            int nBottomRect,    // Coordenada Y del rectángulo inferior
            int nWidthEllipse,  // Ancho de la elipse para las esquinas redondeadas
            int nHeightEllipse  // Alto de la elipse para las esquinas redondeadas
        );
        
        /// <summary>
        /// Constructor del formulario principal
        /// Inicializa la interfaz de usuario y configura todos los eventos necesarios
        /// </summary>
        public Home()
        {
            InitializeComponent();
            // Elimina el borde estándar del formulario para un diseño personalizado
            this.FormBorderStyle = FormBorderStyle.None;
            // Aplica bordes redondeados al formulario
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
            // Configura los placeholders (textos de ayuda) en los campos de entrada
            ConfigurarPlaceholders();
            // Configura los eventos de cambio de texto para validación en tiempo real
            ConfigurarEventosTextChanged();
            // Configura el menú contextual para ordenamiento de productos
            ConfigurarMenuOrdenamiento();
            // Asigna eventos a los botones principales
            btnLimpiarBuscar.Click += btnLimpiarBuscar_Click;
            btnEditar.Click += btnEditar_Click;
            // Configura el DataGridView para selección de filas completas
            tblProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        /// <summary>
        /// Configura el menú contextual para ordenamiento de productos
        /// Crea opciones de ordenamiento por diferentes criterios
        /// </summary>
        private void ConfigurarMenuOrdenamiento()
        {
            menuOrdenar = new ContextMenuStrip();

            // Agrega opciones de ordenamiento al menú contextual
            menuOrdenar.Items.Add("Nombre de Productos (A - Z)", null, OrdenarProductos_Click);
            menuOrdenar.Items.Add("Nombre de Productos (Z - A)", null, OrdenarProductos_Click);
            menuOrdenar.Items.Add("Más stock", null, OrdenarProductos_Click);
            menuOrdenar.Items.Add("Menos stock", null, OrdenarProductos_Click);
            menuOrdenar.Items.Add("Mayor precio", null, OrdenarProductos_Click);
            menuOrdenar.Items.Add("Menor precio", null, OrdenarProductos_Click);
            menuOrdenar.Items.Add("Fecha reciente", null, OrdenarProductos_Click);
            menuOrdenar.Items.Add("Fecha antigua", null, OrdenarProductos_Click);

            // Asigna el evento de clic al botón de ordenamiento
            button5.Click += button5_Click;
        }

        /// <summary>
        /// Configura los placeholders (textos de ayuda) en todos los campos de entrada
        /// Los placeholders proporcionan guías visuales al usuario sobre qué información ingresar
        /// </summary>
        private void ConfigurarPlaceholders()
        {
            // Establece los textos de placeholder para cada campo
            txtAutoparte.Text = "Nombre de Autoparte";
            txtPrecio.Text = "Precio";
            txtProveedor.Text = "Proveedor";
            txtStock.Text = "Stock";
            txtCodeAut.Text = "Código";
            txtNomAuto.Text = "Nombre de Autoparte";
            txtProv.Text = "Proveedor";

            // Configura los eventos de placeholder para cada campo
            ConfigurarEventosPlaceholder(txtAutoparte, "Nombre de Autoparte");
            ConfigurarEventosPlaceholder(txtPrecio, "Precio");
            ConfigurarEventosPlaceholder(txtProveedor, "Proveedor");
            ConfigurarEventosPlaceholder(txtStock, "Stock");
            ConfigurarEventosPlaceholder(txtCodeAut, "Código");
            ConfigurarEventosPlaceholder(txtNomAuto, "Nombre de Autoparte");
            ConfigurarEventosPlaceholder(txtProv, "Proveedor");
        }

        /// <summary>
        /// Configura los eventos de placeholder para un TextBox específico
        /// Maneja el comportamiento de mostrar/ocultar el texto de ayuda
        /// </summary>
        /// <param name="textBox">El TextBox al que se le configurarán los eventos</param>
        /// <param name="placeholderText">El texto de placeholder que se mostrará</param>
        private void ConfigurarEventosPlaceholder(TextBox textBox, string placeholderText)
        {
            // Establece el color gris para el texto de placeholder
            textBox.ForeColor = Color.Gray;

            // Evento que se ejecuta cuando el TextBox recibe el foco
            textBox.Enter += (sender, e) =>
            {
                // Si el texto actual es el placeholder, lo limpia y cambia el color
                if (textBox.Text == placeholderText)
                {
                    textBox.Text = "";
                    textBox.ForeColor = Color.Black;
                }
            };

            // Evento que se ejecuta cuando el TextBox pierde el foco
            textBox.Leave += (sender, e) =>
            {
                // Si el campo está vacío, restaura el placeholder
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = placeholderText;
                    textBox.ForeColor = Color.Gray;
                }
            };
        }

        /// <summary>
        /// Evento que se ejecuta cuando el formulario se carga
        /// Centra el formulario en la pantalla y carga los productos iniciales
        /// </summary>
        /// <param name="sender">Objeto que disparó el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void Home_Load(object sender, EventArgs e)
        {
            this.CenterToScreen(); // Centra el formulario en la pantalla
            CargarProductos(); // Carga la lista inicial de productos
        }

        /// <summary>
        /// Carga todos los productos desde la base de datos MongoDB
        /// Organiza los productos por proveedor y fecha, y los muestra en el DataGridView
        /// </summary>
        private async void CargarProductos()
        {
            try
            {
                // Establece la conexión a MongoDB
                var client = new MongoClient("mongodb://localhost:27017");
                var database = client.GetDatabase("JorStock");
                var productosCollection = database.GetCollection<BsonDocument>("productos");
                var proveedoresCollection = database.GetCollection<BsonDocument>("proveedores");

                // Obtiene todos los productos de la base de datos
                var productos = await productosCollection.Find(new BsonDocument()).ToListAsync();

                // Crea una tabla de datos para mostrar en el DataGridView
                DataTable dt = new DataTable();
                dt.Columns.Add("Nombre", typeof(string));
                dt.Columns.Add("Stock", typeof(int));
                dt.Columns.Add("Precio Unitario", typeof(decimal));
                dt.Columns.Add("Fecha de Ingreso", typeof(DateTime));
                dt.Columns.Add("Serial", typeof(string));
                dt.Columns.Add("Código de Proveedor", typeof(string));
                dt.Columns.Add("Nombre Proveedor", typeof(string));
                dt.Columns.Add("Grupo Proveedor", typeof(int));

                // Diccionario para agrupar productos por proveedor
                Dictionary<string, int> gruposProveedores = new Dictionary<string, int>();
                int contadorGrupos = 1;

                // Lista para almacenar productos con sus fechas para ordenamiento
                List<Tuple<BsonDocument, DateTime>> productosConFecha = new List<Tuple<BsonDocument, DateTime>>();

                // Primera pasada: identificar todos los códigos de proveedor únicos
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

                // Segunda pasada: procesar fechas de productos
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

                // Ordena los productos: primero por grupo de proveedor, luego por fecha (más reciente primero)
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

                // Procesa cada producto ordenado y lo agrega a la tabla de datos
                foreach (var producto in productosOrdenados)
                {
                    DataRow row = dt.NewRow();

                    // Extrae y asigna los datos del producto a la fila
                    row["Nombre"] = producto.Contains("nombre") ? producto["nombre"].ToString() : string.Empty;
                    row["Stock"] = producto.Contains("stock") ? producto["stock"].ToInt32() : 0;
                    row["Precio Unitario"] = producto.Contains("precio_unitario") ? producto["precio_unitario"].ToDecimal() : 0M;

                    // Procesa la fecha del producto
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

                // Asigna la tabla de datos como fuente del DataGridView
                tblProductos.DataSource = dt;

                // Oculta la columna de grupo de proveedor (solo se usa para ordenamiento interno)
                tblProductos.Columns["Grupo Proveedor"].Visible = false;

                // Configura la apariencia visual de los grupos de proveedores
                ConfigurarAparienciaGruposProveedores();

                // Configura las propiedades del DataGridView
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

        /// <summary>
        /// Configura el evento de formateo de celdas para aplicar colores alternados por grupo de proveedor
        /// </summary>
        private void ConfigurarAparienciaGruposProveedores()
        {
            // Remueve el evento anterior para evitar duplicados
            tblProductos.CellFormatting -= TblProductos_CellFormatting;
            // Agrega el evento de formateo de celdas
            tblProductos.CellFormatting += TblProductos_CellFormatting;
        }

        /// <summary>
        /// Evento que se ejecuta cuando se formatea cada celda del DataGridView
        /// Aplica colores alternados para distinguir visualmente los grupos de proveedores
        /// </summary>
        /// <param name="sender">Objeto que disparó el evento (DataGridView)</param>
        /// <param name="e">Argumentos del evento de formateo</param>
        private void TblProductos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && tblProductos.Columns.Contains("Grupo Proveedor"))
            {
                DataGridViewRow row = tblProductos.Rows[e.RowIndex];

                // Obtiene el número de grupo del proveedor
                int grupoProveedor = 0;
                if (row.Cells["Grupo Proveedor"].Value != null &&
                    int.TryParse(row.Cells["Grupo Proveedor"].Value.ToString(), out int grupo))
                {
                    grupoProveedor = grupo;
                }

                // Aplica colores alternados basados en el grupo del proveedor
                if (grupoProveedor > 0)
                {
                    if (grupoProveedor % 2 == 0)
                    {
                        // Color azul claro para grupos pares
                        row.DefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
                    }
                    else
                    {
                        // Color gris claro para grupos impares
                        row.DefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
                    }
                }
            }
        }

        /// <summary>
        /// Evento que se ejecuta cuando se redimensiona el formulario
        /// Reaplica los bordes redondeados para mantener la apariencia
        /// </summary>
        /// <param name="e">Argumentos del evento de redimensionamiento</param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // Recrea la región con bordes redondeados con las nuevas dimensiones
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
        }

        /// <summary>
        /// Evento que se ejecuta cuando se hace clic en el botón de minimizar
        /// Minimiza el formulario principal
        /// </summary>
        /// <param name="sender">Objeto que disparó el evento (botón minimizar)</param>
        /// <param name="e">Argumentos del evento</param>
        private void button10_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// Evento que se ejecuta cuando se hace clic en el botón de cerrar
        /// Cierra completamente la aplicación
        /// </summary>
        /// <param name="sender">Objeto que disparó el evento (botón cerrar)</param>
        /// <param name="e">Argumentos del evento</param>
        private void button9_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Evento que se ejecuta cuando se hace clic en el botón de salir
        /// Cierra completamente la aplicación
        /// </summary>
        /// <param name="sender">Objeto que disparó el evento (botón salir)</param>
        /// <param name="e">Argumentos del evento</param>
        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Evento que se ejecuta cuando se hace clic en el contenido de una celda del DataGridView
        /// Actualmente no implementado
        /// </summary>
        /// <param name="sender">Objeto que disparó el evento (DataGridView)</param>
        /// <param name="e">Argumentos del evento de clic en celda</param>
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        
        /// <summary>
        /// Variable que almacena el ID del producto que se está editando actualmente
        /// Se usa para distinguir entre crear un nuevo producto y editar uno existente
        /// </summary>
        private string productoEnEdicionId = null;
        
        /// <summary>
        /// Método que maneja el evento de clic del botón guardar
        /// Permite crear nuevos productos o actualizar productos existentes en la base de datos
        /// </summary>
        /// <param name="sender">Objeto que disparó el evento (botón guardar)</param>
        /// <param name="e">Argumentos del evento</param>
        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                // Valida que todos los campos requeridos estén completos (no sean placeholders)
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

                // Valida que el precio sea un número decimal válido
                if (!decimal.TryParse(txtPrecio.Text, out decimal precio))
                {
                    MessageBox.Show("El precio debe ser un valor numérico.",
                        "Error de formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Valida que el stock sea un número entero válido
                if (!int.TryParse(txtStock.Text, out int stock))
                {
                    MessageBox.Show("El stock debe ser un valor numérico entero.",
                        "Error de formato", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Establece la conexión a MongoDB
                var client = new MongoClient("mongodb://localhost:27017");
                var database = client.GetDatabase("JorStock");

                var proveedoresCollection = database.GetCollection<BsonDocument>("proveedores");

                // Busca si el proveedor ya existe en la base de datos
                var filtroProveedor = Builders<BsonDocument>.Filter.Eq("nombre", txtProveedor.Text);
                var proveedorExistente = await proveedoresCollection.Find(filtroProveedor).FirstOrDefaultAsync();

                string codigoProveedor;

                // Si el proveedor no existe, lo crea automáticamente
                if (proveedorExistente == null)
                {
                    var nuevoProveedor = new BsonDocument
                    {
                        { "nombre", txtProveedor.Text },
                        { "fecha_registro", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
                    };

                    // Inserta el nuevo proveedor en la base de datos
                    await proveedoresCollection.InsertOneAsync(nuevoProveedor);

                    // Obtiene el proveedor recién creado para obtener su ID
                    proveedorExistente = await proveedoresCollection.Find(filtroProveedor).FirstOrDefaultAsync();
                    codigoProveedor = proveedorExistente["_id"].ToString();
                }
                else
                {
                    // Si el proveedor ya existe, usa su ID existente
                    codigoProveedor = proveedorExistente["_id"].ToString();
                }

                var productosCollection = database.GetCollection<BsonDocument>("productos");

                // Determina si se está editando un producto existente o creando uno nuevo
                if (productoEnEdicionId != null)
                {
                    try
                    {
                        // Actualiza el producto existente
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

                        // Limpia el ID de edición para indicar que ya no se está editando
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
                    // Crea un nuevo producto
                    var nuevoProducto = new BsonDocument
                    {
                        { "nombre", txtAutoparte.Text },
                        { "precio_unitario", precio },
                        { "stock", stock },
                        { "fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
                        { "serial", txtCodeAut.Text },
                        { "codigo_proveedor", codigoProveedor }
                    };

                    // Inserta el nuevo producto en la base de datos
                    await productosCollection.InsertOneAsync(nuevoProducto);

                    MessageBox.Show("Producto guardado exitosamente.",
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Limpia los campos del formulario y recarga la lista de productos
                LimpiarCamposRegistro();
                CargarProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al procesar el producto: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Evento que se ejecuta cuando se hace clic en el botón de limpiar registro
        /// Limpia todos los campos del formulario de registro
        /// </summary>
        /// <param name="sender">Objeto que disparó el evento (botón limpiar)</param>
        /// <param name="e">Argumentos del evento</param>
        private void btnLimpiarRegistrar_Click(object sender, EventArgs e)
        {
            LimpiarCamposRegistro();
        }

        /// <summary>
        /// Limpia todos los campos del formulario de registro y restaura los placeholders
        /// También limpia la variable de edición para indicar que no se está editando ningún producto
        /// </summary>
        private void LimpiarCamposRegistro()
        {
            // Restaura el placeholder y color para el campo de nombre de autoparte
            txtAutoparte.Text = "Nombre de Autoparte";
            txtAutoparte.ForeColor = Color.Gray;

            // Restaura el placeholder y color para el campo de precio
            txtPrecio.Text = "Precio";
            txtPrecio.ForeColor = Color.Gray;

            // Restaura el placeholder y color para el campo de stock
            txtStock.Text = "Stock";
            txtStock.ForeColor = Color.Gray;

            // Restaura el placeholder y color para el campo de proveedor
            txtProveedor.Text = "Proveedor";
            txtProveedor.ForeColor = Color.Gray;

            // Restaura el placeholder y color para el campo de código
            txtCodeAut.Text = "Código";
            txtCodeAut.ForeColor = Color.Gray;

            // Limpia la variable de edición para indicar que no se está editando ningún producto
            productoEnEdicionId = null;
        }

        /// <summary>
        /// Evento que se ejecuta cuando se hace clic en una celda del DataGridView
        /// Selecciona la fila completa cuando se hace clic en cualquier celda
        /// </summary>
        /// <param name="sender">Objeto que disparó el evento (DataGridView)</param>
        /// <param name="e">Argumentos del evento de clic en celda</param>
        private void tblProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                tblProductos.Rows[e.RowIndex].Selected = true;
            }
        }

        /// <summary>
        /// Método que maneja el evento de clic del botón de búsqueda
        /// Permite buscar productos por nombre y/o proveedor en la base de datos
        /// </summary>
        /// <param name="sender">Objeto que disparó el evento (botón buscar)</param>
        /// <param name="e">Argumentos del evento</param>
        private async void btnBuscar_Click(object sender, EventArgs e)
        {
            // Obtiene los criterios de búsqueda de los campos de texto
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
