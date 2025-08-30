using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaterialWinForms.Core
{
    /// <summary>
    /// Manager para administrar temas globalmente
    /// </summary>
    public static class MaterialThremeManager
    {
        public static MaterialColorScheme CurrentScheme { get; private set; } = MaterialColorScheme.Light;

        public static event EventHandler<MaterialColorScheme>? ThemeChanged;

        public static void SetGlobalTheme(MaterialColorScheme scheme)
        {
            CurrentScheme = scheme;
            ThemeChanged?.Invoke(null, scheme);
        }

        public static void ApplyThemeToControl(MaterialControl control)
        {
            //control.ColorScheme = CurrentScheme;
            control.Invalidate();

            // Aplicar recursivamente a controles hijos
            foreach (Control child in control.Controls)
            {
                if (child is MaterialControl materialChild)
                {
                    ApplyThemeToControl(materialChild);
                }
            }
        }
    }
}
