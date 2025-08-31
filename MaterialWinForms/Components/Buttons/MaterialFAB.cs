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

namespace MaterialWinForms.Components.Buttons
{
    /// <summary>
    /// Floating Action Button Material Design
    /// </summary>
    [ToolboxItem(true)]
    [DesignTimeVisible(true)]
    public class MaterialFAB : MaterialControl
    {
        private string _text = "";
        private Image? _icon = null;
        private bool _isExtended = false;
        private bool _isHovered = false;
        private bool _isPressed = false;
        private FABSize _size = FABSize.Normal;
        private Point _rippleLocation;
        private int _rippleSize = 0;
        private System.Windows.Forms.Timer? _rippleTimer;
        private CornerRadius _cornerRadius = new CornerRadius(28);
        private GradientSettings _gradient = new GradientSettings();
        private ShadowSettings _shadow = new ShadowSettings();
        private IconSettings _iconSettings = new IconSettings();
        private TextSettings _textSettings = new TextSettings();

        public enum FABSize
        {
            Small,   // 56x56
            Normal,  // 80x80
            Large    // 112x112
        }

        #region Propiedades Básicas

        [Category("Material")]
        [Description("Texto del FAB (solo visible si IsExtended = true)")]
        public new string TextContent
        {
            get => _text;
            set
            {
                _text = value ?? "";
                UpdateSize();
                Invalidate();
                OnTextChanged(EventArgs.Empty);
            }
        }

        [Category("Material")]
        [Description("Icono del FAB")]
        public Image? Icon
        {
            get => _icon;
            set { _icon = value; Invalidate(); }
        }

        [Category("Material")]
        [Description("FAB extendido con texto")]
        [DefaultValue(false)]
        public bool IsExtended
        {
            get => _isExtended;
            set { _isExtended = value; UpdateSize(); Invalidate(); }
        }

        [Category("Material")]
        [Description("Tamaño del FAB")]
        [DefaultValue(FABSize.Normal)]
        public FABSize Size
        {
            get => _size;
            set { _size = value; UpdateSize(); Invalidate(); }
        }

        #endregion

        #region Propiedades de Apariencia

        [Category("Material - Appearance")]
        [Description("Configuración de esquinas redondeadas")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CornerRadius CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = value ?? new CornerRadius(28); Invalidate(); }
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

        [Category("Material - Appearance")]
        [Description("Configuración de icono")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IconSettings IconSettings
        {
            get => _iconSettings;
            set { _iconSettings = value ?? new IconSettings(); Invalidate(); }
        }

        [Category("Material - Appearance")]
        [Description("Configuración de texto")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TextSettings TextSettings
        {
            get => _textSettings;
            set { _textSettings = value ?? new TextSettings(); Invalidate(); }
        }

        [Category("Material")]
        [Description("Elevación del FAB (sombra automática)")]
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

        public MaterialFAB()
        {
            Cursor = Cursors.Hand;
            Elevation = 6;
            Padding = new Padding(8);

            // Configurar valores por defecto del FAB
            _shadow.Type = MaterialShadowType.Medium;
            _shadow.Opacity = 50;
            _shadow.Blur = 12;
            _shadow.OffsetY = 4;

            _iconSettings.Size = 32; // Tamaño mejorado del icono
            _iconSettings.Position = IconPosition.Left;
            _iconSettings.Spacing = 8;

            _textSettings.FontStyle = FontStyle.Bold;
            _textSettings.FontSize = 10f;

            UpdateSize();

            _rippleTimer = new System.Windows.Forms.Timer { Interval = 10 };
            _rippleTimer.Tick += RippleTimer_Tick;
        }

        private void UpdateSize()
        {
            if (_isExtended && !string.IsNullOrEmpty(_text))
            {
                // FAB extendido
                if (!IsInDesignMode)
                {
                    using var g = CreateGraphics();
                    using var font = new Font(_textSettings.FontFamily, _textSettings.FontSize, _textSettings.FontStyle);
                    var textSize = g.MeasureString(_text, font);
                    var fabHeight = GetFABHeight();
                    var iconWidth = _icon != null ? _iconSettings.Size + _iconSettings.Spacing : 0;
                    base.Size = new System.Drawing.Size((int)textSize.Width + iconWidth + Padding.Horizontal + 16, fabHeight);
                }
                else
                {
                    // Tamaño por defecto para el designer
                    var fabHeight = GetFABHeight();
                    base.Size = new System.Drawing.Size(120, fabHeight);
                }
            }
            else
            {
                // FAB circular
                var fabSize = GetFABHeight();
                base.Size = new System.Drawing.Size(fabSize, fabSize);
                _cornerRadius.All = fabSize / 2; // Mantener circular
            }
        }

        private void UpdateElevationShadow()
        {
            _shadow.Blur = Math.Max(6, Elevation * 2);
            _shadow.OffsetY = Math.Max(2, Elevation / 2);
            _shadow.Opacity = Math.Min(60, 30 + Elevation * 2);
        }

        private int GetFABHeight() => _size switch
        {
            FABSize.Small => 56,   // Tamaño corregido
            FABSize.Large => 112,  // Tamaño corregido
            _ => 80                // Tamaño corregido para Normal
        };

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
                _gradient.StartColor = ColorScheme.Primary;
                _gradient.EndColor = ColorHelper.Lighten(ColorScheme.Primary, 0.2f);
            }
        }

        #region Event Handlers

        private void RippleTimer_Tick(object? sender, EventArgs e)
        {
            _rippleSize += 12;
            if (_rippleSize > Math.Max(Width, Height) * 2)
            {
                _rippleTimer?.Stop();
                _rippleSize = 0;
            }
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            // Elevar ligeramente en hover
            var originalBlur = _shadow.Blur;
            var originalOffset = _shadow.OffsetY;
            _shadow.Blur = Math.Min(24, originalBlur + 3);
            _shadow.OffsetY = Math.Min(12, originalOffset + 2);
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            UpdateElevationShadow(); // Restaurar sombra original
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _isPressed = true;
            if (UseRippleEffect && e.Button == MouseButtons.Left)
            {
                _rippleLocation = e.Location;
                _rippleSize = 0;
                _rippleTimer?.Start();
            }

            // Reducir elevación al presionar
            _shadow.Blur = Math.Max(4, _shadow.Blur - 4);
            _shadow.OffsetY = Math.Max(1, _shadow.OffsetY - 2);

            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isPressed = false;
            UpdateElevationShadow(); // Restaurar sombra
            Invalidate();
            base.OnMouseUp(e);
        }

        #endregion

        #region Painting

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Calcular áreas considerando sombra
            var shadowPadding = CalculateShadowPadding();
            var fabBounds = new Rectangle(
                shadowPadding.Left,
                shadowPadding.Top,
                Width - shadowPadding.Horizontal,
                Height - shadowPadding.Vertical
            );

            // Dibujar sombra
            if (_shadow.Type != MaterialShadowType.None)
            {
                g.DrawMaterialShadow(fabBounds, _cornerRadius, _shadow);
            }

            // Dibujar fondo del FAB
            DrawFABBackground(g, fabBounds);

            // Configurar clipping
            using var fabPath = _isExtended
                ? GraphicsExtensions.CreateRoundedRectanglePath(fabBounds, _cornerRadius)
                : CreateCirclePath(fabBounds);

            var oldClip = g.Clip;
            g.SetClip(fabPath);

            // Dibujar efectos de interacción
            DrawInteractionEffects(g, fabBounds);

            // Dibujar efecto ripple
            DrawRippleEffect(g);

            // Restaurar clipping
            g.Clip = oldClip;

            // Dibujar contenido
            DrawFABContent(g, fabBounds);
        }

        private Padding CalculateShadowPadding()
        {
            if (_shadow.Type == MaterialShadowType.None) return Padding.Empty;

            var blur = _shadow.Blur;
            var offsetX = Math.Abs(_shadow.OffsetX);
            var offsetY = Math.Abs(_shadow.OffsetY);
            var spread = Math.Abs(_shadow.Spread);

            return new Padding(
                blur + offsetX + spread + 2,
                blur + offsetY + spread + 2,
                blur + offsetX + spread + 2,
                blur + offsetY + spread + 2
            );
        }

        private GraphicsPath CreateCirclePath(Rectangle bounds)
        {
            var path = new GraphicsPath();
            path.AddEllipse(bounds);
            return path;
        }

        private void DrawFABBackground(Graphics g, Rectangle bounds)
        {
            // Dibujar gradiente si está configurado
            if (_gradient.Type != GradientType.None)
            {
                if (_isExtended)
                {
                    g.FillRoundedRectangleWithGradient(bounds, _cornerRadius, _gradient);
                }
                else
                {
                    using var brush = GraphicsExtensions.CreateGradientBrush(bounds, _gradient);
                    if (brush != null)
                    {
                        g.FillEllipse(brush, bounds);
                    }
                }
            }
            else
            {
                // Color sólido
                var backgroundColor = GetBackgroundColor();
                using var brush = new SolidBrush(backgroundColor);

                if (_isExtended)
                {
                    g.FillRoundedRectangle(brush, bounds, _cornerRadius);
                }
                else
                {
                    g.FillEllipse(brush, bounds);
                }
            }
        }

        private void DrawInteractionEffects(Graphics g, Rectangle bounds)
        {
            // Efecto hover
            if (_isHovered && !_isPressed)
            {
                var hoverColor = Color.FromArgb(20, ColorScheme.OnPrimary);
                using var brush = new SolidBrush(hoverColor);

                if (_isExtended)
                {
                    g.FillRoundedRectangle(brush, bounds, _cornerRadius);
                }
                else
                {
                    g.FillEllipse(brush, bounds);
                }
            }

            // Efecto pressed
            if (_isPressed)
            {
                var pressedColor = Color.FromArgb(40, ColorScheme.OnPrimary);
                using var brush = new SolidBrush(pressedColor);

                if (_isExtended)
                {
                    g.FillRoundedRectangle(brush, bounds, _cornerRadius);
                }
                else
                {
                    g.FillEllipse(brush, bounds);
                }
            }
        }

        private void DrawRippleEffect(Graphics g)
        {
            if (!UseRippleEffect || _rippleSize <= 0) return;

            var rippleColor = Color.FromArgb(60, ColorScheme.OnPrimary);
            using var brush = new SolidBrush(rippleColor);
            var rippleRect = new Rectangle(
                _rippleLocation.X - _rippleSize / 2,
                _rippleLocation.Y - _rippleSize / 2,
                _rippleSize,
                _rippleSize
            );
            g.FillEllipse(brush, rippleRect);
        }

        private void DrawFABContent(Graphics g, Rectangle bounds)
        {
            var contentBounds = new Rectangle(
                bounds.X + Padding.Left,
                bounds.Y + Padding.Top,
                bounds.Width - Padding.Horizontal,
                bounds.Height - Padding.Vertical
            );

            if (_isExtended && !string.IsNullOrEmpty(_text))
            {
                DrawExtendedFABContent(g, contentBounds);
            }
            else
            {
                DrawCircularFABContent(g, contentBounds);
            }
        }

        private void DrawExtendedFABContent(Graphics g, Rectangle bounds)
        {
            var hasIcon = _icon != null;
            var iconRect = Rectangle.Empty;

            if (hasIcon)
            {
                // Calcular posición del icono
                iconRect = new Rectangle(
                    bounds.X,
                    bounds.Y + (bounds.Height - _iconSettings.Size) / 2,
                    _iconSettings.Size,
                    _iconSettings.Size
                );

                // Dibujar icono
                var textColor = ColorScheme.OnPrimary;
                g.DrawIcon(_icon!, bounds, _iconSettings, textColor);
            }

            // Calcular área de texto
            var textRect = hasIcon
                ? new Rectangle(
                    iconRect.Right + _iconSettings.Spacing,
                    bounds.Y,
                    bounds.Width - iconRect.Width - _iconSettings.Spacing,
                    bounds.Height)
                : bounds;

            // Dibujar texto
            if (!string.IsNullOrEmpty(_text))
            {
                var textColor = ColorScheme.OnPrimary;
                using var font = new Font(_textSettings.FontFamily, _textSettings.FontSize, _textSettings.FontStyle);
                using var brush = new SolidBrush(textColor);
                using var sf = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center
                };

                if (_textSettings.UseEllipsis)
                {
                    sf.Trimming = StringTrimming.EllipsisCharacter;
                    sf.FormatFlags = StringFormatFlags.LineLimit;
                }

                g.DrawString(_text, font, brush, textRect, sf);
            }
        }

        private void DrawCircularFABContent(Graphics g, Rectangle bounds)
        {
            if (_icon != null)
            {
                // Ajustar el tamaño del icono según el tamaño del FAB
                var maxIconSize = _size switch
                {
                    FABSize.Small => 28,   // 50% del diámetro
                    FABSize.Large => 56,   // 50% del diámetro
                    _ => 40                // 50% del diámetro para Normal
                };

                var iconSize = Math.Min(_iconSettings.Size, maxIconSize);
                var iconRect = new Rectangle(
                    bounds.X + (bounds.Width - iconSize) / 2,
                    bounds.Y + (bounds.Height - iconSize) / 2,
                    iconSize,
                    iconSize
                );

                var textColor = ColorScheme.OnPrimary;
                if (_iconSettings.TintColor != Color.Transparent)
                {
                    using var tintedIcon = g.ApplyTint(_icon, _iconSettings.TintColor);
                    g.DrawImage(tintedIcon, iconRect);
                }
                else
                {
                    g.DrawImage(_icon, iconRect);
                }
            }
            else
            {
                // Dibujar icono + por defecto
                DrawDefaultPlusIcon(g, bounds);
            }
        }

        private void DrawDefaultPlusIcon(Graphics g, Rectangle bounds)
        {
            var strokeWidth = _size switch
            {
                FABSize.Small => 2.5f,
                FABSize.Large => 4f,
                _ => 3f // Normal
            };

            using var pen = new Pen(ColorScheme.OnPrimary, strokeWidth)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };

            var center = new Point(
                bounds.X + bounds.Width / 2,
                bounds.Y + bounds.Height / 2
            );

            var iconSize = _size switch
            {
                FABSize.Small => GetFABHeight() / 4,  // 14px para Small
                FABSize.Large => GetFABHeight() / 4,  // 28px para Large  
                _ => GetFABHeight() / 4               // 20px para Normal
            };

            g.DrawLine(pen, center.X - iconSize, center.Y, center.X + iconSize, center.Y);
            g.DrawLine(pen, center.X, center.Y - iconSize, center.X, center.Y + iconSize);
        }

        #endregion

        #region Color Helpers

        private Color GetBackgroundColor()
        {
            return _isPressed
                ? ColorHelper.Darken(ColorScheme.Primary, 0.1f)
                : ColorScheme.Primary;
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _rippleTimer?.Stop();
                _rippleTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

