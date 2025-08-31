using MaterialWinForms.Core;
using MaterialWinForms.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel.Design;

namespace MaterialWinForms.Components.Containers
{
    /// <summary>
    /// Panel Material Design con características completas como contenedor
    /// </summary>
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    [ToolboxItem(true)]
    [DesignTimeVisible(true)]
    public class MaterialPanel : MaterialControl
    {
        private bool _showBorder = false;
        private CornerRadius _cornerRadius = new CornerRadius(12);
        private GradientSettings _gradient = new GradientSettings();
        private ShadowSettings _shadow = new ShadowSettings();
        private Color _customBackColor = Color.Transparent;
        private BorderStyle _borderStyle = BorderStyle.None;
        private Color _borderColor = Color.Transparent;
        private int _borderWidth = 1;

        public enum BorderStyle
        {
            None,
            Solid,
            Dashed,
            Dotted
        }

        #region Propiedades

        [Category("Material")]
        [Description("Mostrar borde del panel")]
        [DefaultValue(false)]
        public bool ShowBorder
        {
            get => _showBorder;
            set { _showBorder = value; Invalidate(); }
        }

        [Category("Material")]
        [Description("Color de fondo personalizado")]
        [DefaultValue(typeof(Color), "Transparent")]
        public Color CustomBackColor
        {
            get => _customBackColor;
            set { _customBackColor = value; Invalidate(); }
        }

        [Category("Material")]
        [Description("Estilo del borde")]
        [DefaultValue(BorderStyle.None)]
        public BorderStyle Border
        {
            get => _borderStyle;
            set { _borderStyle = value; Invalidate(); }
        }

        [Category("Material")]
        [Description("Color del borde")]
        [DefaultValue(typeof(Color), "Transparent")]
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        [Category("Material")]
        [Description("Ancho del borde")]
        [DefaultValue(1)]
        public int BorderWidth
        {
            get => _borderWidth;
            set { _borderWidth = Math.Max(1, value); Invalidate(); }
        }

        [Category("Material - Appearance")]
        [Description("Configuración de esquinas redondeadas")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CornerRadius CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = value ?? new CornerRadius(12); Invalidate(); }
        }

        [Category("Material - Appearance")]
        [Description("Configuración de gradiente de fondo")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GradientSettings Gradient
        {
            get => _gradient;
            set { _gradient = value ?? new GradientSettings(); Invalidate(); }
        }

        [Category("Material - Appearance")]
        [Description("Configuración de sombra")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ShadowSettings Shadow
        {
            get => _shadow;
            set { _shadow = value ?? new ShadowSettings(); Invalidate(); }
        }

        [Category("Material")]
        [Description("Elevación del panel (sombra automática)")]
        public new int Elevation
        {
            get => base.Elevation;
            set
            {
                base.Elevation = Math.Max(0, Math.Min(24, value));
                UpdateElevationShadow();
                Invalidate();
            }
        }

        #endregion

        public MaterialPanel()
        {
            // Configurar como contenedor
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.ContainerControl, true);

            Size = new Size(200, 150);
            Padding = new Padding(16);

            // Configurar sombra por defecto
            _shadow.Type = MaterialShadowType.None; // Sin sombra por defecto
            _shadow.Opacity = 20;
            _shadow.Blur = 4;
            _shadow.OffsetY = 2;

            // Configurar borde por defecto
            _borderColor = Color.FromArgb(40, ColorScheme.OnSurface);

            // Habilitar scroll automático
            AutoScroll = true;

            // Configuraciones para el Designer
            AllowDrop = true;

            // Asegurar que actúe como contenedor en tiempo de diseño
            if (IsInDesignMode)
            {
                SetStyle(ControlStyles.Selectable, true);
            }
        }

        private void UpdateElevationShadow()
        {
            if (Elevation > 0)
            {
                _shadow.Type = MaterialShadowType.Soft;
                _shadow.Blur = Math.Max(2, Elevation * 2);
                _shadow.OffsetY = Math.Max(1, Elevation / 2);
                _shadow.Opacity = Math.Min(50, 20 + Elevation * 2);
            }
            else
            {
                _shadow.Type = MaterialShadowType.None;
            }
        }

        protected override void OnColorSchemeChanged()
        {
            base.OnColorSchemeChanged();
            UpdateGradientDefaults();
            Invalidate();
        }

        private void UpdateGradientDefaults()
        {
            if (_gradient.Type != GradientType.None && _gradient.StartColor == Color.Blue && _gradient.EndColor == Color.LightBlue)
            {
                _gradient.StartColor = ColorScheme.Surface;
                _gradient.EndColor = ColorHelper.Lighten(ColorScheme.Surface, 0.1f);
            }

            if (_borderColor == Color.Transparent)
            {
                _borderColor = Color.FromArgb(40, ColorScheme.OnSurface);
            }
        }

        #region Painting

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Calcular áreas considerando sombra
            var shadowPadding = CalculateShadowPadding();
            var bounds = new Rectangle(
                shadowPadding.Left,
                shadowPadding.Top,
                Width - shadowPadding.Horizontal,
                Height - shadowPadding.Vertical
            );

            // Dibujar sombra
            if (_shadow.Type != MaterialShadowType.None)
            {
                g.DrawMaterialShadow(bounds, _cornerRadius, _shadow);
            }

            // Dibujar fondo
            DrawBackground(g, bounds);

            // Dibujar borde
            DrawBorder(g, bounds);
        }

        private Padding CalculateShadowPadding()
        {
            if (_shadow.Type == MaterialShadowType.None) return Padding.Empty;

            var blur = _shadow.Blur;
            var offsetX = Math.Abs(_shadow.OffsetX);
            var offsetY = Math.Abs(_shadow.OffsetY);
            var spread = Math.Abs(_shadow.Spread);

            return new Padding(
                blur + offsetX + spread + 1,
                blur + offsetY + spread + 1,
                blur + offsetX + spread + 1,
                blur + offsetY + spread + 1
            );
        }

        private void DrawBackground(Graphics g, Rectangle bounds)
        {
            // Dibujar gradiente si está configurado
            if (_gradient.Type != GradientType.None)
            {
                g.FillRoundedRectangleWithGradient(bounds, _cornerRadius, _gradient);
            }
            else
            {
                // Color sólido
                var backgroundColor = GetBackgroundColor();
                if (backgroundColor != Color.Transparent)
                {
                    using var brush = new SolidBrush(backgroundColor);
                    g.FillRoundedRectangle(brush, bounds, _cornerRadius);
                }
            }
        }

        private void DrawBorder(Graphics g, Rectangle bounds)
        {
            if (_borderStyle == BorderStyle.None && !_showBorder) return;

            var borderColor = _borderColor != Color.Transparent ? _borderColor : Color.FromArgb(40, ColorScheme.OnSurface);

            switch (_borderStyle)
            {
                case BorderStyle.Solid:
                    using (var pen = new Pen(borderColor, _borderWidth))
                    {
                        g.DrawRoundedRectangle(pen, bounds, _cornerRadius);
                    }
                    break;

                case BorderStyle.Dashed:
                    using (var pen = new Pen(borderColor, _borderWidth))
                    {
                        pen.DashStyle = DashStyle.Dash;
                        g.DrawRoundedRectangle(pen, bounds, _cornerRadius);
                    }
                    break;

                case BorderStyle.Dotted:
                    using (var pen = new Pen(borderColor, _borderWidth))
                    {
                        pen.DashStyle = DashStyle.Dot;
                        g.DrawRoundedRectangle(pen, bounds, _cornerRadius);
                    }
                    break;

                default: // None pero ShowBorder = true
                    if (_showBorder)
                    {
                        using var pen = new Pen(borderColor, 1);
                        g.DrawRoundedRectangle(pen, bounds, _cornerRadius);
                    }
                    break;
            }
        }

        private Color GetBackgroundColor()
        {
            return _customBackColor != Color.Transparent ? _customBackColor : ColorScheme.Surface;
        }

        #endregion

        #region Métodos de contenedor

        /// <summary>
        /// Añade un control al panel con margen automático
        /// </summary>
        public void AddControl(Control control, int margin = 8)
        {
            control.Margin = new Padding(margin);
            Controls.Add(control);
        }

        /// <summary>
        /// Añade un control con posición específica
        /// </summary>
        public void AddControl(Control control, Point location)
        {
            control.Location = location;
            Controls.Add(control);
        }

        /// <summary>
        /// Añade un control con posición y tamaño específicos
        /// </summary>
        public void AddControl(Control control, Rectangle bounds)
        {
            control.Bounds = bounds;
            Controls.Add(control);
        }

        /// <summary>
        /// Organiza los controles en una grilla simple
        /// </summary>
        public void ArrangeInGrid(int columns, int spacing = 8)
        {
            if (Controls.Count == 0) return;

            SuspendLayout();
            try
            {
                var contentArea = new Rectangle(
                    Padding.Left,
                    Padding.Top,
                    ClientSize.Width - Padding.Horizontal,
                    ClientSize.Height - Padding.Vertical
                );

                var itemWidth = (contentArea.Width - (columns - 1) * spacing) / columns;
                var currentRow = 0;
                var currentCol = 0;

                foreach (Control control in Controls)
                {
                    var x = contentArea.X + currentCol * (itemWidth + spacing);
                    var y = contentArea.Y + currentRow * (control.Height + spacing);

                    control.Location = new Point(x, y);
                    control.Width = itemWidth;

                    currentCol++;
                    if (currentCol >= columns)
                    {
                        currentCol = 0;
                        currentRow++;
                    }
                }
            }
            finally
            {
                ResumeLayout(true);
            }
        }

        /// <summary>
        /// Organiza los controles verticalmente
        /// </summary>
        public void ArrangeVertically(int spacing = 8)
        {
            if (Controls.Count == 0) return;

            SuspendLayout();
            try
            {
                var y = Padding.Top;
                var contentWidth = ClientSize.Width - Padding.Horizontal;

                foreach (Control control in Controls)
                {
                    control.Location = new Point(Padding.Left, y);
                    if (control.Anchor.HasFlag(AnchorStyles.Left | AnchorStyles.Right))
                    {
                        control.Width = contentWidth;
                    }
                    y += control.Height + spacing;
                }
            }
            finally
            {
                ResumeLayout(true);
            }
        }

        /// <summary>
        /// Organiza los controles horizontalmente
        /// </summary>
        public void ArrangeHorizontally(int spacing = 8)
        {
            if (Controls.Count == 0) return;

            SuspendLayout();
            try
            {
                var x = Padding.Left;
                var contentHeight = ClientSize.Height - Padding.Vertical;

                foreach (Control control in Controls)
                {
                    control.Location = new Point(x, Padding.Top);
                    if (control.Anchor.HasFlag(AnchorStyles.Top | AnchorStyles.Bottom))
                    {
                        control.Height = contentHeight;
                    }
                    x += control.Width + spacing;
                }
            }
            finally
            {
                ResumeLayout(true);
            }
        }

        #endregion

        #region Propiedades de accesibilidad para el Designer

        [Browsable(true)]
        [Category("Layout")]
        [Description("Controles contenidos en el panel")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public new ControlCollection Controls => base.Controls;

        [Browsable(true)]
        [Category("Layout")]
        [Description("Habilita el scroll automático")]
        public new bool AutoScroll
        {
            get => base.AutoScroll;
            set => base.AutoScroll = value;
        }

        [Browsable(true)]
        [Category("Layout")]
        [Description("Tamaño mínimo para el scroll automático")]
        public new Size AutoScrollMinSize
        {
            get => base.AutoScrollMinSize;
            set => base.AutoScrollMinSize = value;
        }

        #endregion
    }
}
