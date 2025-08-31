using MaterialWinForms.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaterialWinForms.Core
{
    /// <summary>
    /// Clase base para todos los controles Material Design
    /// </summary>
    [ToolboxItem(false)]
    public abstract class MaterialControl : UserControl
    {
        private MaterialColorScheme? _colorScheme = MaterialColorScheme.Light;

        [Category("Material")]
        [Description("Esquema de colores Material Design")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MaterialColorScheme ColorScheme { 
            get => _colorScheme ?? MaterialColorScheme.Light;
            set
            {
                _colorScheme = value ?? MaterialColorScheme.Light;
                OnColorSchemeChanged();
                Invalidate();
            } 
        }

        [Category("Material")]
        [Description("Elevación del control (altura de sombra)")]
        [DefaultValue(2)]
        public int Elevation { get; set; } = 2;

        [Category("Material")]
        [Description("Habilitar efecto ripple en interacciones")]
        [DefaultValue(true)]
        public bool UseRippleEffect { get; set; } = true;

        protected MaterialControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            _colorScheme = MaterialColorScheme.Light;
            BackColor = Color.Transparent;
        }

        protected void DrawShadow(Graphics g, Rectangle bounds, int elevation)
        {
            if (elevation <= 0) return;

            g.SmoothingMode = SmoothingMode.AntiAlias;

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

        /// <summary>
        /// Método virtual para que los controles derivados respondan a cambios de tema
        /// </summary>
        protected virtual void OnColorSchemeChanged()
        {
            // Los controles derivados pueden override este método
            // para actualizar sus colores específicos
        }

        /// <summary>
        /// Determina si estamos en modo de diseño
        /// </summary>
        protected bool IsInDesignMode
        {
            get
            {
                return LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
                       DesignMode ||
                       (Site != null && Site.DesignMode);
            }
        }

    }
}
