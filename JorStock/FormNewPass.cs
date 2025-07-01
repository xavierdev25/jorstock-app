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
    public partial class FormNewPass : Form
    {
        private Form _previousForm;

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

        public FormNewPass(Form previousForm = null)
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
            _previousForm = previousForm;

            _client = new MongoClient("mongodb://localhost:27017");
            _database = _client.GetDatabase("JorStock");
            _usersCollection = _database.GetCollection<BsonDocument>("users");

            btnIngresar_New.Click += btnIngresar_New_Click;
        }

        private void FormNewPass_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 15, 15));
        }

        private async void btnIngresar_New_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNewPass.Text))
            {
                MessageBox.Show("Por favor, introduce una nueva contraseña válida.",
                               "Contraseña inválida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                btnIngresar_New.Enabled = false;

                Cursor = Cursors.WaitCursor;

                bool actualizado = await UpdatePasswordAsync(ForgotPass.userEmail, txtNewPass.Text);

                Cursor = Cursors.Default;

                if (actualizado)
                {
                    MessageBox.Show("Tu contraseña ha sido actualizada correctamente.",
                                   "Contraseña actualizada", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Login loginForm = new Login();
                    loginForm.Show();
                    this.Close();
                }
                else
                {
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

        private async Task<bool> UpdatePasswordAsync(string email, string newPassword)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("email", email);
                var update = Builders<BsonDocument>.Update.Set("password", newPassword);

                var result = await _usersCollection.UpdateOneAsync(filter, update);

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

        private void button3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form loginForm = new Login();
            loginForm.Show();
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
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
