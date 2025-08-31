using MaterialWinForms.Components.Layout;
using MaterialWinForms.Core.NavigationCore;
using MaterialWinForms.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaterialWinForms.Templates
{
    /// <summary>
    /// Template base para formularios principales con scaffold
    /// </summary>
    public class MaterialMainFormTemplate : Form
    {
        protected MaterialScaffold Scaffold { get; private set; }
        protected MaterialFormNavigator Navigator { get; private set; }

        public MaterialMainFormTemplate()
        {
            InitializeTemplate();
        }

        private void InitializeTemplate()
        {
            // Configurar formulario base
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Material Design Application";

            // Crear scaffold
            Scaffold = new MaterialScaffold();
            Scaffold.Dock = DockStyle.Fill;

            // Configurar navegación
            Navigator = Scaffold.SetupWithNavigation(this.Text);

            // Configurar navegación global
            MaterialNavigation.Initialize(Navigator);

            this.Controls.Add(Scaffold);
        }

        /// <summary>
        /// Método virtual para que las clases derivadas registren sus páginas
        /// </summary>
        protected virtual void RegisterPages()
        {
            // Override en clases derivadas
        }

        /// <summary>
        /// Método virtual para configuración adicional
        /// </summary>
        protected virtual void ConfigureScaffold()
        {
            // Override en clases derivadas
        }

        /// <summary>
        /// Finalizar configuración (llamar después de RegisterPages)
        /// </summary>
        protected void FinishSetup(string startPageKey)
        {
            RegisterPages();
            ConfigureScaffold();
            Navigator.BuildNavigationDrawer();

            if (!string.IsNullOrEmpty(startPageKey))
            {
                Navigator.NavigateTo(startPageKey);
            }
        }
    }
}
