using MaterialWinForms.Core;
using MaterialWinForms.Templates;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaterialWinForms_Test
{
    public partial class Form2 : MaterialMainFormTemplate
    {
        public Form2()
        {
            InitializeComponent();
            this.Text = "Mi Aplicación con Estilos";
            FinishSetup("home");
        }

        protected override void RegisterPages()
        {
            Navigator.RegisterPages(
                ("home", "Inicio", typeof(Nav1), "PRINCIPAL"),
                ("customers", "Clientes", typeof(Nav2), "GESTIÓN")
            );
        }

        protected override void ConfigureScaffold()
        {
            // Configuraciones adicionales opcionales
            Scaffold.NavigationDrawer.HeaderSubtitle = "usuario@ejemplo.com";

            // NUEVAS OPCIONES DE ESTILOS:

            // Opción 1: Drawer que se superpone sobre el AppBar
            Scaffold.SetDrawerStyle(NavigationDrawerStyle.Standard, DrawerHeaderStyle.Standard);

            // Opción 2: AppBar flotante
            Scaffold.SetAppBarStyle(AppBarStyle.Dense);

            // Opción 3: Configurar ambos a la vez
            //Scaffold.SetMaterialStyle(NavigationDrawerStyle.Floating, AppBarStyle.Transparent, DrawerHeaderStyle.Card);

            // Opción 4: Solo cambiar el header del drawer
            //Scaffold.NavigationDrawer.HeaderStyle = DrawerHeaderStyle.Gradient;

            // Opción 5: Solo cambiar el estilo del AppBar
            //Scaffold.AppBar.Style = AppBarStyle.Elevated;
        }
    }
}
