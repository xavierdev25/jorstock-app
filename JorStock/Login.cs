using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Runtime.InteropServices;

namespace JorStock
{
    /// <summary>
    /// Formulario de inicio de sesión que permite autenticar usuarios contra la base de datos MongoDB
    /// </summary>
    public partial class Login : Form
    {
        /// <summary>
        /// Cliente de conexión a MongoDB
        /// </summary>
        private readonly MongoClient _client;
        
        /// <summary>
        /// Referencia a la base de datos JorStock
        /// </summary>
        private readonly IMongoDatabase _database;
        
        /// <summary>
        /// Colección de usuarios en la base de datos
        /// </summary>
        private readonly IMongoCollection<BsonDocument> _usersCollection;

        /// <summary>
        /// Importación de la función de Windows API para crear regiones con bordes redondeados
        /// Permite dar un aspecto moderno al formulario
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
        /// Constructor del formulario de login
        /// Inicializa la conexión a MongoDB y configura la interfaz de usuario
        /// </summary>
        public Login()
        {
            InitializeComponent();

            // Establece la conexión a MongoDB en localhost
            _client = new MongoClient("mongodb://localhost:27017");
            // Obtiene la referencia a la base de datos JorStock
            _database = _client.GetDatabase("JorStock");
            // Obtiene la colección de usuarios
            _usersCollection = _database.GetCollection<BsonDocument>("users");

            // Configura el campo de contraseña para mostrar puntos en lugar del texto
            txtPassword.PasswordChar = '•';

            // Asigna el evento de clic al botón de ingresar
            btnIngresar_Login.Click += BtnIngresar_Login_Click;

            // Elimina el borde estándar del formulario para un diseño personalizado
            this.FormBorderStyle = FormBorderStyle.None;
            // Aplica bordes redondeados al formulario
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
        }

        /// <summary>
        /// Evento que se ejecuta cuando el formulario se carga
        /// Centra el formulario en la pantalla
        /// </summary>
        /// <param name="sender">Objeto que disparó el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void Login_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();
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
        /// Método que maneja el evento de clic del botón de ingresar
        /// Valida las credenciales del usuario contra la base de datos MongoDB
        /// </summary>
        /// <param name="sender">Objeto que disparó el evento (botón de ingresar)</param>
        /// <param name="e">Argumentos del evento</param>
        private async void BtnIngresar_Login_Click(object sender, EventArgs e)
        {
            // Obtiene y limpia los valores de los campos de texto
            string username = txtUser.Text.Trim();
            string password = txtPassword.Text;

            // Valida que ambos campos no estén vacíos
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Por favor, completa todos los campos", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Crea un filtro para buscar el usuario con las credenciales proporcionadas
                var filter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("username", username),
                    Builders<BsonDocument>.Filter.Eq("password", password)
                );

                // Busca el usuario en la base de datos de forma asíncrona
                var user = await _usersCollection.Find(filter).FirstOrDefaultAsync();

                // Si se encuentra el usuario, abre el formulario principal
                if (user != null)
                {
                    Home homeForm = new Home();
                    this.Hide(); // Oculta el formulario de login
                    homeForm.ShowDialog(); // Muestra el formulario principal
                    this.Close(); // Cierra el formulario de login
                }
                else
                {
                    // Si no se encuentra el usuario, muestra un mensaje de error
                    MessageBox.Show("Usuario o contraseña incorrectos", "Error de autenticación",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // Si ocurre un error de conexión, muestra el mensaje de error
                MessageBox.Show($"Error al conectar con la base de datos: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Evento que se ejecuta cuando se hace clic en el enlace "¿Olvidaste tu contraseña?"
        /// Abre el formulario de recuperación de contraseña
        /// </summary>
        /// <param name="sender">Objeto que disparó el evento (linkLabel)</param>
        /// <param name="e">Argumentos del evento</param>
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ForgotPass forgotPassForm = new ForgotPass();

            this.Hide(); // Oculta el formulario de login
            forgotPassForm.ShowDialog(); // Muestra el formulario de recuperación
            this.Close(); // Cierra el formulario de login
        }

        /// <summary>
        /// Evento que se ejecuta cuando se hace clic en el botón de cerrar (X)
        /// Cierra completamente la aplicación
        /// </summary>
        /// <param name="sender">Objeto que disparó el evento (botón cerrar)</param>
        /// <param name="e">Argumentos del evento</param>
        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Evento que se ejecuta cuando se hace clic en el botón de minimizar
        /// Minimiza el formulario de login
        /// </summary>
        /// <param name="sender">Objeto que disparó el evento (botón minimizar)</param>
        /// <param name="e">Argumentos del evento</param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
