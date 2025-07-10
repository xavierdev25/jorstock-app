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

namespace JorStock
{
    /// <summary>
    /// Formulario para establecer nueva contraseña
    /// Permite al usuario cambiar su contraseña después de verificar el código de seguridad
    /// </summary>
    public partial class FormNewPass : Form
    {
        /// <summary>
        /// Referencia al formulario anterior (opcional)
        /// Se usa para manejar la navegación entre formularios
        /// </summary>
        private Form _previousForm;

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
        /// Constructor del formulario de nueva contraseña
        /// Inicializa la conexión a MongoDB y configura la interfaz de usuario
        /// </summary>
        /// <param name="previousForm">Formulario anterior (opcional)</param>
        public FormNewPass(Form previousForm = null)
        {
            InitializeComponent();
            // Elimina el borde estándar del formulario para un diseño personalizado
            this.FormBorderStyle = FormBorderStyle.None;
            // Aplica bordes redondeados al formulario
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
            _previousForm = previousForm;

            // Establece la conexión a MongoDB
            _client = new MongoClient("mongodb://localhost:27017");
            _database = _client.GetDatabase("JorStock");
            _usersCollection = _database.GetCollection<BsonDocument>("users");

            // Asigna el evento al botón de ingresar nueva contraseña
            btnIngresar_New.Click += btnIngresar_New_Click;
        }

        /// <summary>
        /// Evento que se ejecuta cuando el formulario se carga
        /// Centra el formulario en la pantalla
        /// </summary>
        /// <param name="sender">Objeto que disparó el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void FormNewPass_Load(object sender, EventArgs e)
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
        /// Método que maneja el evento de clic del botón de ingresar nueva contraseña
        /// Valida la nueva contraseña y la actualiza en la base de datos
        /// </summary>
        /// <param name="sender">Objeto que disparó el evento (botón ingresar)</param>
        /// <param name="e">Argumentos del evento</param>
        private async void btnIngresar_New_Click(object sender, EventArgs e)
        {
            // Valida que el campo de nueva contraseña no esté vacío
            if (string.IsNullOrWhiteSpace(txtNewPass.Text))
            {
                MessageBox.Show("Por favor, introduce una nueva contraseña válida.",
                               "Contraseña inválida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Deshabilita el botón para evitar múltiples clics
                btnIngresar_New.Enabled = false;
                // Cambia el cursor para indicar que se está procesando
                Cursor = Cursors.WaitCursor;

                // Actualiza la contraseña en la base de datos
                bool actualizado = await UpdatePasswordAsync(ForgotPass.userEmail, txtNewPass.Text);

                Cursor = Cursors.Default;

                if (actualizado)
                {
                    // Si la actualización fue exitosa, muestra mensaje de confirmación
                    MessageBox.Show("Tu contraseña ha sido actualizada correctamente.",
                                   "Contraseña actualizada", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Abre el formulario de login y cierra el actual
                    Login loginForm = new Login();
                    loginForm.Show();
                    this.Close();
                }
                else
                {
                    // Si la actualización falló, habilita el botón nuevamente
                    btnIngresar_New.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                btnIngresar_New.Enabled = true;
                MessageBox.Show($"Error al actualizar la contraseña: {ex.Message}",
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Actualiza la contraseña del usuario en la base de datos MongoDB
        /// </summary>
        /// <param name="email">Correo electrónico del usuario</param>
        /// <param name="newPassword">Nueva contraseña a establecer</param>
        /// <returns>True si la actualización fue exitosa, False en caso contrario</returns>
        private async Task<bool> UpdatePasswordAsync(string email, string newPassword)
        {
            try
            {
                // Crea un filtro para buscar el usuario por correo electrónico
                var filter = Builders<BsonDocument>.Filter.Eq("email", email);
                // Crea la actualización para cambiar la contraseña
                var update = Builders<BsonDocument>.Update.Set("password", newPassword);

                // Ejecuta la actualización en la base de datos
                var result = await _usersCollection.UpdateOneAsync(filter, update);

                // Verifica si se actualizó algún documento
                if (result.ModifiedCount == 0)
                {
                    MessageBox.Show("No se pudo actualizar la contraseña. Por favor, inténtalo de nuevo.",
                                   "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar la contraseña en la base de datos: {ex.Message}",
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Evento que se ejecuta cuando se hace clic en el botón de minimizar
        /// Minimiza el formulario
        /// </summary>
        /// <param name="sender">Objeto que disparó el evento (botón minimizar)</param>
        /// <param name="e">Argumentos del evento</param>
        private void button3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// Evento que se ejecuta cuando se hace clic en el botón de volver
        /// Regresa al formulario de login
        /// </summary>
        /// <param name="sender">Objeto que disparó el evento (botón volver)</param>
        /// <param name="e">Argumentos del evento</param>
        private void button2_Click(object sender, EventArgs e)
        {
            Form loginForm = new Login();
            loginForm.Show();
            this.Close();
        }

        /// <summary>
        /// Evento que se ejecuta cuando se intenta cerrar el formulario
        /// Maneja la navegación cuando se cierra el formulario
        /// </summary>
        /// <param name="e">Argumentos del evento de cierre</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // Si hay un formulario anterior y no está visible, abre el login
                if (_previousForm != null && !_previousForm.Visible)
                {
                    Form loginForm = new Login();
                    loginForm.Show();
                }
            }
            base.OnFormClosing(e);
        }
    }
}
