using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JorStock
{
    internal static class Program
    {
        public static List<Form> OpenForms = new List<Form>();

        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ShowForm(new Login());
            Application.Run(new InvisibleMainForm());
        }

        public static void ShowForm(Form form)
        {
            OpenForms.Add(form);
            form.FormClosed += (s, args) => OpenForms.Remove(form);
            form.Show();
        }

        public static void CloseAllExcept(Form exceptForm)
        {
            foreach (var form in OpenForms.ToList())
            {
                if (form != exceptForm && !form.IsDisposed)
                {
                    form.Close();
                }
            }
        }
    }

    public class InvisibleMainForm : Form
    {
        public InvisibleMainForm()
        {
            this.ShowInTaskbar = false;
            this.Opacity = 0;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new System.Drawing.Size(1, 1);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new System.Drawing.Point(-2000, -2000);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.FormClosing += (s, args) => {
                if (Program.OpenForms.Count == 0)
                {
                    Application.Exit();
                }
            };
        }
    }
}
