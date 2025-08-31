using MaterialWinForms.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialWinForms.Utils;
using System.ComponentModel.Design;

namespace MaterialWinForms.Components.Containers
{
    /// <summary>
    /// Card Material Design para contenedores
    /// </summary>
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    [ToolboxItem(false)]
    [DesignTimeVisible(true)]
    public class MaterialSimpleCard : MaterialControl
    {
        private string _title = "";
        private string _subtitle = "";
        private Image? _cardImage = null;
        private bool _showDivider = true;

        [Category("Material")]
        [Description("Título principal de la card")]
        public string Title
        {
            get => _title;
            set { _title = value ?? ""; Invalidate(); }
        }

        [Category("Material")]
        [Description("Subtítulo de la card")]
        public string Subtitle
        {
            get => _subtitle;
            set { _subtitle = value ?? ""; Invalidate(); }
        }

        [Category("Material")]
        [Description("Imagen opcional de la card")]
        public Image? CardImage
        {
            get => _cardImage;
            set { _cardImage = value; Invalidate(); }
        }

        [Category("Material")]
        [Description("Mostrar línea divisoria después del encabezado")]
        public bool ShowDivider
        {
            get => _showDivider;
            set { _showDivider = value; Invalidate(); }
        }

        public MaterialSimpleCard()
        {
            Size = new Size(300, 200);
            BackColor = ColorScheme.Surface;
            Elevation = 2;
            Padding = new Padding(16);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var bounds = new Rectangle(0, 0, Width, Height);

            // Dibujar sombra
            DrawShadow(g, bounds, Elevation);

            // Dibujar fondo de la card
            using (var brush = new SolidBrush(ColorScheme.Surface))
            {
                g.FillRoundedRectangle(brush, bounds, 12);
            }

            var currentY = Padding.Top;

            // Dibujar imagen si existe
            if (_cardImage != null)
            {
                var imageHeight = Math.Min(120, Height / 3);
                var imageRect = new Rectangle(0, 0, Width, imageHeight);
                g.SetClip(CreateRoundedRectanglePath(imageRect, 12, true, false));
                g.DrawImage(_cardImage, imageRect);
                g.ResetClip();
                currentY = imageHeight + 8;
            }

            // Dibujar título
            if (!string.IsNullOrEmpty(_title))
            {
                using var titleFont = new Font("Segoe UI", 16F, FontStyle.Bold);
                using var titleBrush = new SolidBrush(ColorScheme.OnSurface);
                var titleRect = new Rectangle(Padding.Left, currentY, Width - Padding.Horizontal, 30);
                g.DrawString(_title, titleFont, titleBrush, titleRect);
                currentY += 30;
            }

            // Dibujar subtítulo
            if (!string.IsNullOrEmpty(_subtitle))
            {
                using var subtitleFont = new Font("Segoe UI", 12F);
                using var subtitleBrush = new SolidBrush(Color.FromArgb(180, ColorScheme.OnSurface));
                var subtitleRect = new Rectangle(Padding.Left, currentY, Width - Padding.Horizontal, 20);
                g.DrawString(_subtitle, subtitleFont, subtitleBrush, subtitleRect);
                currentY += 25;
            }

            // Dibujar divisor
            if (_showDivider && (!string.IsNullOrEmpty(_title) || !string.IsNullOrEmpty(_subtitle)))
            {
                using var dividerPen = new Pen(Color.FromArgb(30, ColorScheme.OnSurface), 1);
                g.DrawLine(dividerPen, Padding.Left, currentY, Width - Padding.Right, currentY);
            }
        }

        private GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius, bool topLeft, bool bottomLeft)
        {
            var path = new GraphicsPath();
            var diameter = radius * 2;

            if (topLeft)
            {
                path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
                path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            }
            else
            {
                path.AddLine(rect.X, rect.Y, rect.Right, rect.Y);
            }

            if (bottomLeft)
            {
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            }
            else
            {
                path.AddLine(rect.Right, rect.Bottom, rect.X, rect.Bottom);
            }

            path.CloseFigure();
            return path;
        }
    }
}
