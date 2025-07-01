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
    public partial class FormSecurity : Form
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

        public FormSecurity()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
        }

        private void FormSecurity_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();
            btnIngresar_Security.Click += btnIngresar_Security_Click;

            this.Activate();
            this.BringToFront();
        }

        private void btnIngresar_Security_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text))
            {
                MessageBox.Show("Por favor, introduce el código de seguridad.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (txtCode.Text.Trim() == ForgotPass.securityCode)
            {
                try
                {
                    FormNewPass newPass = new FormNewPass();
                    newPass.Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al abrir el formulario de nueva contraseña: {ex.Message}",
                                   "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("El código introducido no es válido. Por favor, verifica e intenta nuevamente.",
                                "Código inválido",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Login loginForm = new Login();
            loginForm.Show();
            this.Close();
        }
    }
}
