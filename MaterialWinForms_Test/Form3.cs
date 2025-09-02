using MaterialWinForms.Components.Layout;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaterialWinForms_Test
{
    public partial class Form3 : Form
    {
        private MaterialScaffold scaffold;

        public Form3()
        {
            InitializeComponent();
        }

        private void SetupCustomMaterialApp()
        {
            // Crear scaffold
            scaffold = new MaterialScaffold();
            scaffold.Dock = DockStyle.Fill;

            // Usar tus controles personalizados
            var customAppBar = new AppbarModerno();
            customAppBar.Title = "Mi Aplicación Personalizada";
            customAppBar.NavigationIconClick += (s, e) => scaffold.ToggleDrawer();
            customAppBar.ActionClick += (s, e) => MessageBox.Show($"Acción: {e}");

            var customDrawer = new DrawerModerno();
            customDrawer.HeaderTitle = "Usuario Demo";
            customDrawer.ItemSelected += (s, pageKey) => NavigateToPage(pageKey);

            // Establecer controles personalizados
            scaffold.SetCustomAppBar(customAppBar);
            scaffold.SetCustomDrawer(customDrawer);

            // Configurar body
            var welcomePanel = new Panel
            {
                BackColor = Color.LightGray,
                Dock = DockStyle.Fill
            };
            var welcomeLabel = new Label
            {
                Text = "¡Bienvenido a tu app personalizada!",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 16F)
            };
            welcomePanel.Controls.Add(welcomeLabel);
            scaffold.Body = welcomePanel;

            this.Controls.Add(scaffold);
        }

        private void NavigateToPage(string pageKey)
        {
            MessageBox.Show($"Navegando a: {pageKey}");
            // Aquí implementarías tu lógica de navegación
        }
    }
}
