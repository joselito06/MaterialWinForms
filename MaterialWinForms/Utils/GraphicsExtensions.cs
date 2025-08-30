using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialWinForms.Utils
{
    /// <summary>
    /// Extensiones para Graphics con formas redondeadas
    /// </summary>
    public static class GraphicsExtensions
    {
        public static void FillRoundedRectangle(this Graphics g, Brush brush, Rectangle rect, int cornerRadius)
        {
            using var path = CreateRoundedRectanglePath(rect, cornerRadius);
            g.FillPath(brush, path);
        }

        public static void DrawRoundedRectangle(this Graphics g, Pen pen, Rectangle rect, int cornerRadius)
        {
            using var path = CreateRoundedRectanglePath(rect, cornerRadius);
            g.DrawPath(pen, path);
        }

        public static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int cornerRadius)
        {
            var path = new GraphicsPath();
            var diameter = Math.Min(cornerRadius * 2, Math.Min(rect.Width, rect.Height));

            if (diameter > 0)
            {
                path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
                path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();
            }
            else
            {
                path.AddRectangle(rect);
            }

            return path;
        }

        public static void DrawMaterialShadow(this Graphics g, Rectangle bounds, int elevation)
        {
            if (elevation <= 0) return;

            // Sombra más realista basada en Material Design
            var shadowColor = Color.FromArgb(40, 0, 0, 0);
            var shadowOffset = elevation / 2;
            var shadowBlur = elevation;

            for (int i = 0; i < shadowBlur; i++)
            {
                var alpha = Math.Max(5, 40 - (i * 35 / shadowBlur));
                using var shadowBrush = new SolidBrush(Color.FromArgb(alpha, 0, 0, 0));

                var shadowBounds = new Rectangle(
                    bounds.X - i + shadowOffset,
                    bounds.Y - i + shadowOffset + 1,
                    bounds.Width + i * 2,
                    bounds.Height + i * 2
                );

                using var shadowPath = CreateRoundedRectanglePath(shadowBounds, 20);
                g.FillPath(shadowBrush, shadowPath);
            }
        }
    }
}
