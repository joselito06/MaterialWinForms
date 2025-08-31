using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialWinForms.Utils
{
    /// <summary>
    /// Utilidades para manipulación de colores
    /// </summary>
    public static class ColorHelper
    {
        public static Color Darken(Color color, float amount)
        {
            amount = Math.Max(0, Math.Min(1, amount));
            return Color.FromArgb(
                color.A,
                (int)(color.R * (1 - amount)),
                (int)(color.G * (1 - amount)),
                (int)(color.B * (1 - amount))
            );
        }

        public static Color Lighten(Color color, float amount)
        {
            amount = Math.Max(0, Math.Min(1, amount));
            return Color.FromArgb(
                color.A,
                (int)(color.R + (255 - color.R) * amount),
                (int)(color.G + (255 - color.G) * amount),
                (int)(color.B + (255 - color.B) * amount)
            );
        }

    }
}
