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
    public partial class ForgotPass : Form
    {
        public static string securityCode = string.Empty;
        public static string userEmail = string.Empty;
        private bool isFormSwitching = false;

        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<BsonDocument> _usersCollection;

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

        public ForgotPass()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));

            _client = new MongoClient("mongodb://localhost:27017");
            _database = _client.GetDatabase("JorStock");
            _usersCollection = _database.GetCollection<BsonDocument>("users");
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
        }

        private void ForgotPass_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();
            btnSiguiente_Forgot.Click += btnSiguiente_Forgot_Click;
        }

        private async Task<bool> CheckEmailExistsAsync(string email)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("email", email);
                var user = await _usersCollection.Find(filter).FirstOrDefaultAsync();
                return user != null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al verificar el correo en la base de datos: {ex.Message}",
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private async void btnSiguiente_Forgot_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMail.Text))
            {
                MessageBox.Show("Por favor, introduce un correo electrónico válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                btnSiguiente_Forgot.Enabled = false;

                Cursor = Cursors.WaitCursor;

                string email = txtMail.Text.Trim();
                bool emailExists = await CheckEmailExistsAsync(email);

                if (!emailExists)
                {
                    Cursor = Cursors.Default;
                    MessageBox.Show("El correo electrónico no existe en nuestra base de datos.",
                                   "Correo no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnSiguiente_Forgot.Enabled = true;
                    return;
                }

                Random random = new Random();
                securityCode = random.Next(100000, 999999).ToString();
                userEmail = email;

                bool enviado = await SendEmailAsync(userEmail, securityCode);

                Cursor = Cursors.Default;

                if (enviado)
                {
                    try
                    {
                        isFormSwitching = true;

                        FormSecurity securityForm = new FormSecurity();
                        securityForm.TopMost = true;
                        securityForm.Show();
                        this.Hide();
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

        private async Task<bool> SendEmailAsync(string emailTo, string code)
        {
            try
            {
                string senderEmail = "davidmg2512@gmail.com";
                string password = "fkto qmrp gacd yrmh";

                MailMessage message = new MailMessage();
                message.From = new MailAddress(senderEmail);
                message.Subject = "Código de recuperación de contraseña - JorStock";
                message.To.Add(new MailAddress(emailTo));
                message.Body = $"<html><body>Tu código de recuperación de contraseña es: <strong>{code}</strong><br>Por favor, no compartas este código con nadie.</body></html>";
                message.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(senderEmail, password);
                smtp.EnableSsl = true;
                smtp.Timeout = 30000;

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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (isFormSwitching && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                return;
            }

            base.OnFormClosing(e);
        }
    }
}
