using MaterialWinForms.Components.Navigation;
using MaterialWinForms.Core;

namespace MaterialWinForms_Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            MaterialThremeManager.RegisterForm(this);
        }

    }
}
