using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JorStock
{
    /// <summary>
    /// Clase principal que contiene el punto de entrada de la aplicación y métodos de gestión de formularios
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Lista estática que mantiene un registro de todos los formularios abiertos en la aplicación
        /// Permite gestionar múltiples ventanas simultáneamente
        /// </summary>
        public static List<Form> OpenForms = new List<Form>();

        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// Configura el estilo visual y inicia la aplicación mostrando el formulario de login
        /// </summary>
        [STAThread] // Atributo requerido para aplicaciones Windows Forms
        static void Main()
        {
            // Habilita los estilos visuales de Windows para una apariencia moderna
            Application.EnableVisualStyles();
            // Configura el renderizado de texto para compatibilidad
            Application.SetCompatibleTextRenderingDefault(false);

            // Muestra el formulario de login como primera pantalla
            ShowForm(new Login());
            // Ejecuta la aplicación con un formulario principal invisible
            Application.Run(new InvisibleMainForm());
        }

        /// <summary>
        /// Método para mostrar un formulario y agregarlo a la lista de formularios abiertos
        /// También configura el evento para remover el formulario de la lista cuando se cierre
        /// </summary>
        /// <param name="form">El formulario que se va a mostrar</param>
        public static void ShowForm(Form form)
        {
            // Agrega el formulario a la lista de formularios abiertos
            OpenForms.Add(form);
            // Configura un evento que se ejecuta cuando el formulario se cierra
            // para removerlo automáticamente de la lista
            form.FormClosed += (s, args) => OpenForms.Remove(form);
            // Muestra el formulario
            form.Show();
        }

        /// <summary>
        /// Cierra todos los formularios abiertos excepto el especificado
        /// Útil para limpiar la interfaz cuando se navega entre pantallas
        /// </summary>
        /// <param name="exceptForm">El formulario que NO se debe cerrar</param>
        public static void CloseAllExcept(Form exceptForm)
        {
            // Itera sobre una copia de la lista para evitar errores de modificación durante la iteración
            foreach (var form in OpenForms.ToList())
            {
                // Verifica que el formulario no sea el que queremos mantener y que no esté ya cerrado
                if (form != exceptForm && !form.IsDisposed)
                {
                    form.Close();
                }
            }
        }
    }

    /// <summary>
    /// Formulario invisible que actúa como formulario principal de la aplicación
    /// Permite que la aplicación siga ejecutándose mientras se muestran otros formularios
    /// </summary>
    public class InvisibleMainForm : Form
    {
        /// <summary>
        /// Constructor que configura el formulario para ser invisible y no aparecer en la barra de tareas
        /// </summary>
        public InvisibleMainForm()
        {
            // Oculta el formulario de la barra de tareas
            this.ShowInTaskbar = false;
            // Hace el formulario completamente transparente
            this.Opacity = 0;
            // Elimina el borde del formulario
            this.FormBorderStyle = FormBorderStyle.None;
            // Establece un tamaño mínimo
            this.Size = new System.Drawing.Size(1, 1);
            // Posiciona el formulario manualmente
            this.StartPosition = FormStartPosition.Manual;
            // Mueve el formulario fuera de la pantalla visible
            this.Location = new System.Drawing.Point(-2000, -2000);
        }

        /// <summary>
        /// Evento que se ejecuta cuando el formulario se carga
        /// Configura el comportamiento de cierre de la aplicación
        /// </summary>
        /// <param name="e">Argumentos del evento de carga</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Configura un evento que se ejecuta cuando se intenta cerrar este formulario
            this.FormClosing += (s, args) => {
                // Si no hay formularios abiertos, cierra la aplicación
                if (Program.OpenForms.Count == 0)
                {
                    Application.Exit();
                }
            };
        }
    }
}
