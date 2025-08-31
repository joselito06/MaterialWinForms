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
    /// Panel Material con elevación y bordes redondeados
    /// </summary>
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    [ToolboxItem(true)]
    [DesignTimeVisible(false)]
    public class MaterialSimplePanel : MaterialControl
    {
        private bool _showBorder = false;

        [Category("Material")]
        [Description("Mostrar borde del panel")]
        public bool ShowBorder
        {
            get => _showBorder;
            set { _showBorder = value; Invalidate(); }
        }

        public MaterialSimplePanel()
        {
            Size = new Size(200, 150);
            Padding = new Padding(16);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var bounds = new Rectangle(0, 0, Width, Height);

            // Dibujar sombra si tiene elevación
            if (Elevation > 0)
            {
                DrawShadow(g, bounds, Elevation);
            }

            // Dibujar fondo
            using (var backgroundBrush = new SolidBrush(ColorScheme.Surface))
            {
                g.FillRoundedRectangle(backgroundBrush, bounds, 12);
            }

            // Dibujar borde opcional
            if (_showBorder)
            {
                using (var borderPen = new Pen(Color.FromArgb(40, ColorScheme.OnSurface), 1))
                {
                    g.DrawRoundedRectangle(borderPen, bounds, 12);
                }
            }
        }
    }
}
