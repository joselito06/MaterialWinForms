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
    public static class MaterialThemeManager
    {
        public static MaterialColorScheme CurrentScheme { get; private set; } = MaterialColorScheme.Light;

        public static event EventHandler<MaterialColorScheme>? ThemeChanged;

        public static void SetGlobalTheme(MaterialColorScheme scheme)
        {
            CurrentScheme = scheme ?? throw new ArgumentNullException(nameof(scheme));
            ThemeChanged?.Invoke(null, scheme);
        }

        public static void ApplyThemeToControl(MaterialControl control)
        {
            if (control == null) return;

            control.ColorScheme = CurrentScheme;
            control.Invalidate();

            // Aplicar recursivamente a controles hijos
            ApplyThemeToChildren(control);
            /*foreach (Control child in control.Controls)
            {
                if (child is MaterialControl materialChild)
                {
                    ApplyThemeToControl(materialChild);
                }
            }*/
        }

        public static void ApplyThemeToForm(Form form)
        {
            if (form == null) return;

            form.BackColor = CurrentScheme.Background;
            form.ForeColor = CurrentScheme.OnBackground;

            // Aplicar a todos los controles Material en el formulario
            ApplyThemeToChildren(form);
        }

        private static void ApplyThemeToChildren(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                switch (child)
                {
                    case MaterialControl materialChild:
                        materialChild.ColorScheme = CurrentScheme;
                        materialChild.Invalidate();
                        ApplyThemeToChildren(materialChild);
                        break;

                    case Label label:
                        label.ForeColor = CurrentScheme.OnBackground;
                        break;

                    case Panel panel:
                        panel.BackColor = CurrentScheme.Background;
                        ApplyThemeToChildren(panel);
                        break;

                    default:
                        // Aplicar recursivamente si tiene hijos
                        if (child.HasChildren)
                            ApplyThemeToChildren(child);
                        break;
                }
            }
        }

        /// <summary>
        /// Registra un formulario para recibir actualizaciones automáticas de tema
        /// </summary>
        public static void RegisterForm(Form form)
        {
            ThemeChanged += (sender, scheme) =>
            {
                if (!form.IsDisposed)
                {
                    form.Invoke(() => ApplyThemeToForm(form));
                }
            };

            // Aplicar tema actual inmediatamente
            ApplyThemeToForm(form);
        }
    }
}
