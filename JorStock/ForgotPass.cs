using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Driver;
using MongoDB.Bson;

namespace JorStock
{
    /// <summary>
    /// Formulario para recuperación de contraseña
    /// Permite a los usuarios solicitar un código de verificación por correo electrónico
    /// </summary>
    public partial class ForgotPass : Form
    {
        /// <summary>
        /// Código de seguridad generado para la verificación
        /// Se almacena estáticamente para ser accesible desde otros formularios
        /// </summary>
        public static string securityCode = string.Empty;
        
        /// <summary>
        /// Correo electrónico del usuario que solicita la recuperación
        /// Se almacena estáticamente para ser accesible desde otros formularios
        /// </summary>
        public static string userEmail = string.Empty;
        
        /// <summary>
        /// Bandera que indica si se está realizando un cambio de formulario
        /// Previene el cierre accidental del formulario durante la transición
        /// </summary>
        private bool isFormSwitching = false;

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
        /// Constructor del formulario de recuperación de contraseña
        /// Inicializa la conexión a MongoDB y configura la interfaz de usuario
        /// </summary>
        public ForgotPass()
        {
            InitializeComponent();
            // Elimina el borde estándar del formulario para un diseño personalizado
            this.FormBorderStyle = FormBorderStyle.None;
            // Aplica bordes redondeados al formulario
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));

            // Establece la conexión a MongoDB
            _client = new MongoClient("mongodb://localhost:27017");
            _database = _client.GetDatabase("JorStock");
            _usersCollection = _database.GetCollection<BsonDocument>("users");
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
        /// Evento que se ejecuta cuando el formulario se carga
        /// Centra el formulario en la pantalla y configura los eventos
        /// </summary>
        /// <param name="sender">Objeto que disparó el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void ForgotPass_Load(object sender, EventArgs e)
        {
            this.CenterToScreen(); // Centra el formulario en la pantalla
            btnSiguiente_Forgot.Click += btnSiguiente_Forgot_Click; // Asigna el evento al botón siguiente
        }

        /// <summary>
        /// Verifica si el correo electrónico existe en la base de datos
        /// </summary>
        /// <param name="email">El correo electrónico a verificar</param>
        /// <returns>True si el correo existe, False en caso contrario</returns>
        private async Task<bool> CheckEmailExistsAsync(string email)
        {
            try
            {
                // Crea un filtro para buscar el usuario por correo electrónico
                var filter = Builders<BsonDocument>.Filter.Eq("email", email);
                var user = await _usersCollection.Find(filter).FirstOrDefaultAsync();
                return user != null; // Retorna true si se encuentra el usuario
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al verificar el correo en la base de datos: {ex.Message}",
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Método que maneja el evento de clic del botón siguiente
        /// Valida el correo electrónico, genera un código de seguridad y envía el correo
        /// </summary>
        /// <param name="sender">Objeto que disparó el evento (botón siguiente)</param>
        /// <param name="e">Argumentos del evento</param>
        private async void btnSiguiente_Forgot_Click(object sender, EventArgs e)
        {
            // Valida que el campo de correo no esté vacío
            if (string.IsNullOrWhiteSpace(txtMail.Text))
            {
                MessageBox.Show("Por favor, introduce un correo electrónico válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Deshabilita el botón para evitar múltiples clics
                btnSiguiente_Forgot.Enabled = false;
                // Cambia el cursor para indicar que se está procesando
                Cursor = Cursors.WaitCursor;

                string email = txtMail.Text.Trim();
                // Verifica si el correo existe en la base de datos
                bool emailExists = await CheckEmailExistsAsync(email);

                if (!emailExists)
                {
                    Cursor = Cursors.Default;
                    MessageBox.Show("El correo electrónico no existe en nuestra base de datos.",
                                   "Correo no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnSiguiente_Forgot.Enabled = true;
                    return;
                }

                // Genera un código de seguridad aleatorio de 6 dígitos
                Random random = new Random();
                securityCode = random.Next(100000, 999999).ToString();
                userEmail = email;

                // Envía el correo electrónico con el código de verificación
                bool enviado = await SendEmailAsync(userEmail, securityCode);

                Cursor = Cursors.Default;

                if (enviado)
                {
                    try
                    {
                        // Marca que se está realizando un cambio de formulario
                        isFormSwitching = true;

                        // Abre el formulario de verificación de seguridad
                        FormSecurity securityForm = new FormSecurity();
                        securityForm.TopMost = true; // Mantiene el formulario en primer plano
                        securityForm.Show();
                        this.Hide(); // Oculta el formulario actual
                    }
                    catch (Exception formEx)
                    {
                        isFormSwitching = false;
                        MessageBox.Show($"Error al abrir el formulario de seguridad: {formEx.Message}",
                                       "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        btnSiguiente_Forgot.Enabled = true;
                    }
                }
                else
                {
                    btnSiguiente_Forgot.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                btnSiguiente_Forgot.Enabled = true;
                MessageBox.Show($"Error al procesar la solicitud: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Envía un correo electrónico con el código de verificación
        /// Utiliza SMTP de Gmail para el envío
        /// </summary>
        /// <param name="emailTo">Correo electrónico del destinatario</param>
        /// <param name="code">Código de verificación a enviar</param>
        /// <returns>True si el correo se envió exitosamente, False en caso contrario</returns>
        private async Task<bool> SendEmailAsync(string emailTo, string code)
        {
            try
            {
                // Configuración del correo remitente (Gmail)
                string senderEmail = "davidmg2512@gmail.com";
                string password = "fkto qmrp gacd yrmh"; // Contraseña de aplicación de Gmail

                // Crea el mensaje de correo
                MailMessage message = new MailMessage();
                message.From = new MailAddress(senderEmail);
                message.Subject = "Código de recuperación de contraseña - JorStock";
                message.To.Add(new MailAddress(emailTo));
                message.Body = $"<html><body>Tu código de recuperación de contraseña es: <strong>{code}</strong><br>Por favor, no compartas este código con nadie.</body></html>";
                message.IsBodyHtml = true; // Permite formato HTML en el cuerpo del correo

                // Configura el cliente SMTP de Gmail
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                smtp.Port = 587; // Puerto para TLS
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(senderEmail, password);
                smtp.EnableSsl = true; // Habilita SSL/TLS
                smtp.Timeout = 30000; // Timeout de 30 segundos

                // Envía el correo de forma asíncrona
                await smtp.SendMailAsync(message);

                MessageBox.Show("Se ha enviado un código de verificación a tu correo electrónico.",
                               "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al enviar el correo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            Login loginForm = new Login();
            loginForm.Show();
            this.Close();
        }

        /// <summary>
        /// Evento que se ejecuta cuando se intenta cerrar el formulario
        /// Previene el cierre accidental durante la transición a otro formulario
        /// </summary>
        /// <param name="e">Argumentos del evento de cierre</param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Si se está realizando un cambio de formulario y el usuario intenta cerrar
            if (isFormSwitching && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true; // Cancela el cierre
                this.Hide(); // Solo oculta el formulario
                return;
            }

            base.OnFormClosing(e);
        }
    }
}
