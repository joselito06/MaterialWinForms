using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaterialWinForms.Core
{
    /// <summary>
    /// Información de página para uso público
    /// </summary>
    public class FormPageInfo
    {
        public string Key { get; set; } = "";
        public string Title { get; set; } = "";
        public Type FormType { get; set; } = typeof(Form);
        public string Category { get; set; } = "";
    }
}
