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

namespace JorStock
{
    /// <summary>
    /// Formulario de verificación de código de seguridad
    /// Permite al usuario ingresar el código de verificación enviado por correo electrónico
    /// </summary>
    public partial class FormSecurity : Form
    {
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
        /// Constructor del formulario de verificación de seguridad
        /// Configura la interfaz de usuario con bordes redondeados
        /// </summary>
        public FormSecurity()
        {
            InitializeComponent();
            // Elimina el borde estándar del formulario para un diseño personalizado
            this.FormBorderStyle = FormBorderStyle.None;
            // Aplica bordes redondeados al formulario
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
        }

        /// <summary>
        /// Evento que se ejecuta cuando el formulario se carga
        /// Centra el formulario, configura los eventos y lo trae al frente
        /// </summary>
        /// <param name="sender">Objeto que disparó el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void FormSecurity_Load(object sender, EventArgs e)
        {
            this.CenterToScreen(); // Centra el formulario en la pantalla
            btnIngresar_Security.Click += btnIngresar_Security_Click; // Asigna el evento al botón de ingresar

            this.Activate(); // Activa el formulario
            this.BringToFront(); // Lo trae al frente de todas las ventanas
        }

        /// <summary>
        /// Método que maneja el evento de clic del botón de ingresar código
        /// Valida el código de seguridad ingresado por el usuario
        /// </summary>
        /// <param name="sender">Objeto que disparó el evento (botón ingresar)</param>
        /// <param name="e">Argumentos del evento</param>
        private void btnIngresar_Security_Click(object sender, EventArgs e)
        {
            // Valida que el campo de código no esté vacío
            if (string.IsNullOrWhiteSpace(txtCode.Text))
            {
                MessageBox.Show("Por favor, introduce el código de seguridad.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Compara el código ingresado con el código de seguridad generado
            if (txtCode.Text.Trim() == ForgotPass.securityCode)
            {
                try
                {
                    // Si el código es correcto, abre el formulario para establecer nueva contraseña
                    FormNewPass newPass = new FormNewPass();
                    newPass.Show();
                    this.Close(); // Cierra el formulario actual
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al abrir el formulario de nueva contraseña: {ex.Message}",
                                   "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Si el código es incorrecto, muestra un mensaje de error
                MessageBox.Show("El código introducido no es válido. Por favor, verifica e intenta nuevamente.",
                                "Código inválido",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
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
    }
}
