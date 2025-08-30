using MaterialWinForms.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaterialWinForms.Core
{
    /// <summary>
    /// Clase base para todos los controles Material Design
    /// </summary>
    public abstract class MaterialControl : UserControl
    {
        protected MaterialColorScheme ColorScheme { get; set; }
        protected int Elevation { get; set; } = 2;
        protected bool UseRippleEffect { get; set; } = true;

        protected MaterialControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            ColorScheme = MaterialColorScheme.Light;
            BackColor = Color.Transparent;
        }

        protected void DrawShadow(Graphics g, Rectangle bounds, int elevation)
        {
            if (elevation <= 0) return;

            for (int i = 0; i < elevation; i++)
            {
                using var shadowBrush = new SolidBrush(Color.FromArgb(Math.Max(0, 30 - i * 3), 0, 0, 0));
                var shadowRect = new Rectangle(
                    bounds.X + i,
                    bounds.Y + i,
                    bounds.Width,
                    bounds.Height
                );
                g.FillRoundedRectangle(shadowBrush, shadowRect, 8);
            }
        }
    }
}
