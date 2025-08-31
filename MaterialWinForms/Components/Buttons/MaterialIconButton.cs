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
    /// Icon Button Material Design optimizado para iconos únicamente
    /// </summary>
    [ToolboxItem(true)]
    [DesignTimeVisible(true)]
    public class MaterialIconButton : MaterialControl
    {
        private bool _isHovered = false;
        private bool _isPressed = false;
        private IconButtonStyle _style = IconButtonStyle.Standard;
        private IconButtonSize _size = IconButtonSize.Medium;
        private Point _rippleLocation;
        private int _rippleSize = 0;
        private System.Windows.Forms.Timer? _rippleTimer;
        private CornerRadius _cornerRadius = new CornerRadius(20);
        private GradientSettings _gradient = new GradientSettings();
        private ShadowSettings _shadow = new ShadowSettings();
        private IconSettings _iconSettings = new IconSettings();
        private Color _backgroundColorCustom = Color.Transparent;
        private bool _isToggleable = false;
        private bool _isToggled = false;

        public enum IconButtonStyle
        {
            Standard,    // Transparente con hover
            Filled,      // Fondo sólido
            FilledTonal, // Fondo tonal
            Outlined     // Con borde
        }

        public enum IconButtonSize
        {
            Small,   // 32x32
            Medium,  // 40x40  
            Large    // 48x48
        }

        #region Eventos

        public event EventHandler? ToggleChanged;

        #endregion

        #region Propiedades Básicas

        [Category("Material")]
        [Description("Estilo del icon button")]
        [DefaultValue(IconButtonStyle.Standard)]
        public IconButtonStyle Style
        {
            get => _style;
            set { _style = value; UpdateDefaultColors(); Invalidate(); }
        }

        [Category("Material")]
        [Description("Tamaño del icon button")]
        [DefaultValue(IconButtonSize.Medium)]
        public IconButtonSize Size
        {
            get => _size;
            set { _size = value; UpdateSize(); Invalidate(); }
        }

        [Category("Material")]
        [Description("Color de fondo personalizado")]
        [DefaultValue(typeof(Color), "Transparent")]
        public Color BackgroundColorCustom
        {
            get => _backgroundColorCustom;
            set { _backgroundColorCustom = value; Invalidate(); }
        }

        [Category("Material")]
        [Description("El botón puede alternar entre estados")]
        [DefaultValue(false)]
        public bool IsToggleable
        {
            get => _isToggleable;
            set { _isToggleable = value; Invalidate(); }
        }

        [Category("Material")]
        [Description("Estado actual del toggle")]
        [DefaultValue(false)]
        public bool IsToggled
        {
            get => _isToggled;
            set
            {
                if (_isToggled != value)
                {
                    _isToggled = value;
                    ToggleChanged?.Invoke(this, EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        #endregion

        #region Propiedades de Apariencia

        [Category("Material - Appearance")]
        [Description("Configuración de esquinas redondeadas")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CornerRadius CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = value ?? new CornerRadius(20); Invalidate(); }
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
        [Description("Configuración avanzada de icono")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IconSettings IconSettings
        {
            get => _iconSettings;
            set { _iconSettings = value ?? new IconSettings(); Invalidate(); }
        }

        [Category("Material")]
        [Description("Elevación del botón (sombra automática)")]
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

        public MaterialIconButton()
        {
            Cursor = Cursors.Hand;
            TabStop = true;

            // Configurar valores por defecto
            _shadow.Type = MaterialShadowType.None; // Sin sombra por defecto para icon buttons
            _iconSettings.Size = 24;
            _iconSettings.Position = IconPosition.Center;

            UpdateSize();
            UpdateDefaultColors();

            _rippleTimer = new System.Windows.Forms.Timer { Interval = 10 };
            _rippleTimer.Tick += RippleTimer_Tick;
        }

        private void UpdateSize()
        {
            var buttonSize = _size switch
            {
                IconButtonSize.Small => 32,
                IconButtonSize.Large => 48,
                _ => 40 // Medium
            };

            base.Size = new System.Drawing.Size(buttonSize, buttonSize);

            // Actualizar corner radius para mantener proporción
            if (_cornerRadius.All == 20 && !_cornerRadius.UseIndividualCorners)
            {
                _cornerRadius.All = buttonSize / 2; // Circular por defecto
            }

            // Actualizar tamaño del icono según el tamaño del botón
            if (_iconSettings.Size == 24) // Solo si es el valor por defecto
            {
                _iconSettings.Size = _size switch
                {
                    IconButtonSize.Small => 18,
                    IconButtonSize.Large => 30,
                    _ => 24
                };
            }
        }

        private void UpdateDefaultColors()
        {
            // Los colores se actualizarán dinámicamente en GetIconColor() y GetBackgroundColor()
            // pero podemos configurar sombras según el estilo
            switch (_style)
            {
                case IconButtonStyle.Filled:
                    _shadow.Type = MaterialShadowType.Soft;
                    _shadow.Blur = 4;
                    _shadow.Opacity = 30;
                    break;
                case IconButtonStyle.FilledTonal:
                    _shadow.Type = MaterialShadowType.Soft;
                    _shadow.Blur = 2;
                    _shadow.Opacity = 20;
                    break;
                default:
                    _shadow.Type = MaterialShadowType.None;
                    break;
            }
        }

        private void UpdateElevationShadow()
        {
            if (_style == IconButtonStyle.Filled || _style == IconButtonStyle.FilledTonal)
            {
                _shadow.Blur = Math.Max(2, Elevation * 2);
                _shadow.OffsetY = Math.Max(1, Elevation / 2);
                _shadow.Opacity = Math.Min(50, 20 + Elevation * 2);
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
                _gradient.StartColor = ColorScheme.Primary;
                _gradient.EndColor = ColorHelper.Lighten(ColorScheme.Primary, 0.2f);
            }
        }

        #region Event Handlers

        private void RippleTimer_Tick(object? sender, EventArgs e)
        {
            _rippleSize += 6;
            if (_rippleSize > Math.Max(Width, Height) * 1.5)
            {
                _rippleTimer?.Stop();
                _rippleSize = 0;
            }
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
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
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isPressed = false;
            Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnClick(EventArgs e)
        {
            if (_isToggleable)
            {
                IsToggled = !IsToggled;
            }
            base.OnClick(e);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            Invalidate();
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            Invalidate();
            base.OnLostFocus(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Space || keyData == Keys.Enter)
            {
                if (_isToggleable)
                {
                    IsToggled = !IsToggled;
                }
                OnClick(EventArgs.Empty);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
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
            var buttonBounds = new Rectangle(
                shadowPadding.Left,
                shadowPadding.Top,
                Width - shadowPadding.Horizontal,
                Height - shadowPadding.Vertical
            );

            // Dibujar sombra
            if (_shadow.Type != MaterialShadowType.None)
            {
                g.DrawMaterialShadow(buttonBounds, _cornerRadius, _shadow);
            }

            // Dibujar fondo del botón
            DrawButtonBackground(g, buttonBounds);

            // Configurar clipping
            using var buttonPath = GraphicsExtensions.CreateRoundedRectanglePath(buttonBounds, _cornerRadius);
            var oldClip = g.Clip;
            g.SetClip(buttonPath);

            // Dibujar efectos de interacción
            DrawInteractionEffects(g, buttonBounds);

            // Dibujar indicador de focus
            if (Focused)
            {
                DrawFocusIndicator(g, buttonBounds);
            }

            // Dibujar efecto ripple
            DrawRippleEffect(g);

            // Restaurar clipping
            g.Clip = oldClip;

            // Dibujar borde para botones outlined
            if (_style == IconButtonStyle.Outlined)
            {
                DrawButtonBorder(g, buttonBounds);
            }

            // Dibujar icono
            DrawIcon(g, buttonBounds);
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

        private void DrawButtonBackground(Graphics g, Rectangle bounds)
        {
            var backgroundColor = GetBackgroundColor();
            if (backgroundColor == Color.Transparent && _gradient.Type == GradientType.None) return;

            // Dibujar gradiente si está configurado
            if (_gradient.Type != GradientType.None)
            {
                g.FillRoundedRectangleWithGradient(bounds, _cornerRadius, _gradient);
            }
            else if (backgroundColor != Color.Transparent)
            {
                // Color sólido
                using var brush = new SolidBrush(backgroundColor);
                g.FillRoundedRectangle(brush, bounds, _cornerRadius);
            }
        }

        private void DrawInteractionEffects(Graphics g, Rectangle bounds)
        {
            Color? effectColor = null;

            if (_isPressed)
            {
                effectColor = GetPressedOverlayColor();
            }
            else if (_isHovered)
            {
                effectColor = GetHoverOverlayColor();
            }

            if (effectColor.HasValue && effectColor.Value != Color.Transparent)
            {
                using var brush = new SolidBrush(effectColor.Value);
                g.FillRoundedRectangle(brush, bounds, _cornerRadius);
            }
        }

        private void DrawFocusIndicator(Graphics g, Rectangle bounds)
        {
            var focusColor = Color.FromArgb(50, ColorScheme.Primary);
            using var brush = new SolidBrush(focusColor);

            // Crear un rectángulo ligeramente más grande para el indicador de focus
            var focusBounds = new Rectangle(
                bounds.X - 2, bounds.Y - 2,
                bounds.Width + 4, bounds.Height + 4
            );

            var focusCorners = new CornerRadius(_cornerRadius.All + 2);
            g.FillRoundedRectangle(brush, focusBounds, focusCorners);
        }

        private void DrawRippleEffect(Graphics g)
        {
            if (!UseRippleEffect || _rippleSize <= 0) return;

            var rippleColor = GetRippleColor();
            using var brush = new SolidBrush(rippleColor);
            var rippleRect = new Rectangle(
                _rippleLocation.X - _rippleSize / 2,
                _rippleLocation.Y - _rippleSize / 2,
                _rippleSize,
                _rippleSize
            );
            g.FillEllipse(brush, rippleRect);
        }

        private void DrawButtonBorder(Graphics g, Rectangle bounds)
        {
            var borderColor = GetBorderColor();
            using var pen = new Pen(borderColor, 1);
            g.DrawRoundedRectangle(pen, bounds, _cornerRadius);
        }

        private void DrawIcon(Graphics g, Rectangle bounds)
        {
            if (_iconSettings.Icon == null) return;

            var iconSize = _iconSettings.Size;
            var iconRect = new Rectangle(
                bounds.X + (bounds.Width - iconSize) / 2,
                bounds.Y + (bounds.Height - iconSize) / 2,
                iconSize,
                iconSize
            );

            var iconColor = GetIconColor();

            if (iconColor != Color.Transparent && iconColor != ColorScheme.OnSurface)
            {
                using var tintedIcon = g.ApplyTint(_iconSettings.Icon, iconColor);
                g.DrawImage(tintedIcon, iconRect);
            }
            else
            {
                g.DrawImage(_iconSettings.Icon, iconRect);
            }
        }

        #endregion

        #region Color Helpers

        private Color GetBackgroundColor()
        {
            if (_backgroundColorCustom != Color.Transparent)
                return _backgroundColorCustom;

            var isActive = _isToggleable && _isToggled;

            return _style switch
            {
                IconButtonStyle.Filled => isActive
                    ? ColorHelper.Darken(ColorScheme.Primary, 0.1f)
                    : ColorScheme.Primary,

                IconButtonStyle.FilledTonal => isActive
                    ? ColorHelper.Lighten(ColorScheme.Primary, 0.8f)
                    : ColorHelper.Lighten(ColorScheme.Primary, 0.9f),

                IconButtonStyle.Outlined => isActive
                    ? Color.FromArgb(20, ColorScheme.Primary)
                    : Color.Transparent,

                _ => isActive // Standard
                    ? Color.FromArgb(20, ColorScheme.Primary)
                    : Color.Transparent
            };
        }

        private Color GetIconColor()
        {
            // Prioridad: IconSettings.TintColor > colores automáticos por estilo
            if (_iconSettings.TintColor != Color.Transparent)
                return _iconSettings.TintColor;

            var isActive = _isToggleable && _isToggled;

            return _style switch
            {
                IconButtonStyle.Filled => ColorScheme.OnPrimary,

                IconButtonStyle.FilledTonal => isActive
                    ? ColorScheme.Primary
                    : ColorScheme.OnSurface,

                IconButtonStyle.Outlined => isActive
                    ? ColorScheme.Primary
                    : ColorScheme.OnSurface,

                _ => isActive // Standard
                    ? ColorScheme.Primary
                    : ColorScheme.OnSurface
            };
        }

        private Color GetHoverOverlayColor()
        {
            return _style switch
            {
                IconButtonStyle.Filled => Color.FromArgb(20, ColorScheme.OnPrimary),
                _ => Color.FromArgb(20, ColorScheme.OnSurface)
            };
        }

        private Color GetPressedOverlayColor()
        {
            return _style switch
            {
                IconButtonStyle.Filled => Color.FromArgb(30, ColorScheme.OnPrimary),
                _ => Color.FromArgb(30, ColorScheme.OnSurface)
            };
        }

        private Color GetRippleColor()
        {
            return _style switch
            {
                IconButtonStyle.Filled => Color.FromArgb(40, ColorScheme.OnPrimary),
                _ => Color.FromArgb(40, ColorScheme.OnSurface)
            };
        }

        private Color GetBorderColor()
        {
            var isActive = _isToggleable && _isToggled;
            return isActive ? ColorScheme.Primary : ColorScheme.OnSurface;
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
