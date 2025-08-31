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
        }
    }
}
