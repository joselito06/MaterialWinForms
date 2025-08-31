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
    /// Botón Material Design completamente personalizable
    /// </summary>
    [ToolboxItem(true)]
    [DesignTimeVisible(true)]
    public class MaterialButton : MaterialSimpleButton
    {
        private string _text = "Button";
        private ButtonType _buttonType = ButtonType.Contained;
        private bool _isHovered = false;
        private bool _isPressed = false;
        private Point _rippleLocation;
        private int _rippleSize = 0;
        private System.Windows.Forms.Timer? _rippleTimer;
        private CornerRadius _cornerRadius = new CornerRadius(20);
        private GradientSettings _gradient = new GradientSettings();
        private ShadowSettings _shadow = new ShadowSettings();
        private IconSettings _iconSettings = new IconSettings();
        private TextSettings _textSettings = new TextSettings();

        public enum ButtonType
        {
            Contained,
            Outlined,
            Text,
            Floating
        }

        #region Propiedades Básicas

        [Category("Material")]
        [Description("Texto que se muestra en el botón")]
        [DefaultValue("Button")]
        public new string TextContent
        {
            get => _text;
            set
            {
                _text = value;
                Invalidate();
                OnTextChanged(EventArgs.Empty);
            }
        }

        [Category("Material")]
        [Description("Tipo de botón Material Design")]
        [DefaultValue(ButtonType.Contained)]
        public ButtonType Type
        {
            get => _buttonType;
            set { _buttonType = value; UpdateDefaultShadow(); Invalidate(); }
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
        [Description("Configuración de icono")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IconSettings Icon
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

        public MaterialButton()
        {
            Size = new Size(140, 40);
            Cursor = Cursors.Hand;
            Padding = new Padding(8);

            UpdateDefaultShadow();

            _rippleTimer = new System.Windows.Forms.Timer { Interval = 10 };
            _rippleTimer.Tick += RippleTimer_Tick;
        }

        private void UpdateDefaultShadow()
        {
            switch (_buttonType)
            {
                case ButtonType.Contained:
                    _shadow.Type = MaterialShadowType.Soft;
                    _shadow.Opacity = 40;
                    _shadow.Blur = 8;
                    _shadow.OffsetY = 2;
                    break;
                case ButtonType.Floating:
                    _shadow.Type = MaterialShadowType.Medium;
                    _shadow.Opacity = 50;
                    _shadow.Blur = 12;
                    _shadow.OffsetY = 4;
                    break;
                default:
                    _shadow.Type = MaterialShadowType.None;
                    break;
            }
        }

        private void UpdateElevationShadow()
        {
            if (_buttonType == ButtonType.Contained || _buttonType == ButtonType.Floating)
            {
                _shadow.Blur = Math.Max(4, Elevation * 2);
                _shadow.OffsetY = Math.Max(1, Elevation / 2);
                _shadow.Opacity = Math.Min(60, 30 + Elevation * 2);
            }
        }

        #region Event Handlers

        private void RippleTimer_Tick(object? sender, EventArgs e)
        {
            _rippleSize += 8;
            if (_rippleSize > Math.Max(Width, Height) * 1.5)
            {
                _rippleTimer?.Stop();
                _rippleSize = 0;
            }
            Invalidate();
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

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            if (_buttonType == ButtonType.Contained || _buttonType == ButtonType.Floating)
            {
                // Elevar ligeramente en hover
                var originalBlur = _shadow.Blur;
                var originalOffset = _shadow.OffsetY;
                _shadow.Blur = Math.Min(24, originalBlur + 2);
                _shadow.OffsetY = Math.Min(12, originalOffset + 1);
            }
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
            if (_buttonType == ButtonType.Contained || _buttonType == ButtonType.Floating)
            {
                _shadow.Blur = Math.Max(2, _shadow.Blur - 3);
                _shadow.OffsetY = Math.Max(1, _shadow.OffsetY - 1);
            }

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

            // Configurar clipping para efectos
            using var buttonPath = GraphicsExtensions.CreateRoundedRectanglePath(buttonBounds, _cornerRadius);
            var oldClip = g.Clip;
            g.SetClip(buttonPath);

            // Dibujar efectos de interacción
            DrawInteractionEffects(g, buttonBounds);

            // Dibujar efecto ripple
            DrawRippleEffect(g);

            // Restaurar clipping
            g.Clip = oldClip;

            // Dibujar borde para botones outlined
            if (_buttonType == ButtonType.Outlined)
            {
                DrawButtonBorder(g, buttonBounds);
            }

            // Dibujar contenido (icono y texto)
            DrawButtonContent(g, buttonBounds);
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

        private void DrawButtonBackground(Graphics g, Rectangle bounds)
        {
            if (_buttonType == ButtonType.Text) return;

            // Dibujar gradiente si está configurado
            if (_gradient.Type != GradientType.None)
            {
                g.FillRoundedRectangleWithGradient(bounds, _cornerRadius, _gradient);
            }
            else
            {
                // Color sólido
                var backgroundColor = GetBackgroundColor();
                using var brush = new SolidBrush(backgroundColor);
                g.FillRoundedRectangle(brush, bounds, _cornerRadius);
            }
        }

        private void DrawInteractionEffects(Graphics g, Rectangle bounds)
        {
            // Efecto hover
            if (_isHovered && !_isPressed)
            {
                var hoverColor = GetHoverColor();
                using var brush = new SolidBrush(hoverColor);
                g.FillRoundedRectangle(brush, bounds, _cornerRadius);
            }

            // Efecto pressed
            if (_isPressed)
            {
                var pressedColor = GetPressedColor();
                using var brush = new SolidBrush(pressedColor);
                g.FillRoundedRectangle(brush, bounds, _cornerRadius);
            }
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
            var borderColor = _isPressed ? ColorHelper.Darken(ColorScheme.Primary, 0.2f) :
                            _isHovered ? ColorHelper.Lighten(ColorScheme.Primary, 0.1f) :
                            ColorScheme.Primary;

            using var pen = new Pen(borderColor, _isPressed ? 2 : 1);
            g.DrawRoundedRectangle(pen, bounds, _cornerRadius);
        }

        private void DrawButtonContent(Graphics g, Rectangle bounds)
        {
            var hasIcon = _iconSettings.Icon != null;
            var hasText = !string.IsNullOrEmpty(_text);

            if (!hasIcon && !hasText) return;

            var contentBounds = new Rectangle(
                bounds.X + Padding.Left,
                bounds.Y + Padding.Top,
                bounds.Width - Padding.Horizontal,
                bounds.Height - Padding.Vertical
            );

            if (hasIcon && hasText)
            {
                DrawIconAndText(g, contentBounds);
            }
            else if (hasIcon)
            {
                DrawIconOnly(g, contentBounds);
            }
            else
            {
                DrawTextOnly(g, contentBounds);
            }
        }

        private void DrawIconAndText(Graphics g, Rectangle bounds)
        {
            var textColor = GetTextColor();
            var iconRect = CalculateIconRect(bounds);
            var textRect = CalculateTextRect(bounds, iconRect);

            // Dibujar icono
            g.DrawIcon(_iconSettings.Icon!, bounds, _iconSettings, textColor);

            // Dibujar texto
            //using var font = new Font("Segoe UI", 9F, FontStyle.Regular);
            //using var brush = new SolidBrush(textColor);
            //using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            //g.DrawString(_text, font, brush, textRect, sf);

            // Dibujar texto con configuración personalizada
            DrawTextWithSettings(g, textRect, textColor);
        }

        private void DrawIconOnly(Graphics g, Rectangle bounds)
        {
            var textColor = GetTextColor();
            g.DrawIcon(_iconSettings.Icon!, bounds, _iconSettings, textColor);
        }

        private void DrawTextOnly(Graphics g, Rectangle bounds)
        {
            var textColor = GetTextColor();
            //using var font = new Font("Segoe UI", 9F, FontStyle.Regular);
            //using var brush = new SolidBrush(textColor);
            //using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            //g.DrawString(_text, font, brush, bounds, sf);

            // Aplicar offset si está configurado
            var textBounds = bounds;
            if (_textSettings.OffsetX != 0 || _textSettings.OffsetY != 0)
            {
                textBounds = new Rectangle(
                    bounds.X + _textSettings.OffsetX,
                    bounds.Y + _textSettings.OffsetY,
                    bounds.Width,
                    bounds.Height
                );
            }

            DrawTextWithSettings(g, textBounds, textColor);
        }

        private void DrawTextWithSettings(Graphics g, Rectangle textRect, Color textColor)
        {
            if (string.IsNullOrEmpty(_text)) return;

            using var font = new Font(_textSettings.FontFamily, _textSettings.FontSize, _textSettings.FontStyle);
            using var brush = new SolidBrush(textColor);

            var stringFormat = CreateStringFormat();

            // Si UseEllipsis está habilitado, configurar para mostrar puntos suspensivos
            if (_textSettings.UseEllipsis)
            {
                stringFormat.Trimming = StringTrimming.EllipsisCharacter;
                stringFormat.FormatFlags = StringFormatFlags.LineLimit;
            }

            g.DrawString(_text, font, brush, textRect, stringFormat);
        }

        private StringFormat CreateStringFormat()
        {
            var sf = new StringFormat();

            // Convertir ContentAlignment a StringFormat
            switch (_textSettings.Alignment)
            {
                case ContentAlignment.TopLeft:
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopCenter:
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopRight:
                    sf.Alignment = StringAlignment.Far;
                    sf.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.MiddleLeft:
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleCenter:
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleRight:
                    sf.Alignment = StringAlignment.Far;
                    sf.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.BottomLeft:
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomCenter:
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomRight:
                    sf.Alignment = StringAlignment.Far;
                    sf.LineAlignment = StringAlignment.Far;
                    break;
            }

            return sf;
        }

        private Rectangle CalculateIconRect(Rectangle bounds)
        {
            var iconSize = new Size(_iconSettings.Size, _iconSettings.Size);
            var hasText = !string.IsNullOrEmpty(_text);

            return _iconSettings.Position switch
            {
                IconPosition.Left when hasText => new Rectangle(
                    bounds.X,
                    bounds.Y + (bounds.Height - iconSize.Height) / 2,
                    iconSize.Width,
                    iconSize.Height),

                IconPosition.Right when hasText => new Rectangle(
                    bounds.Right - iconSize.Width,
                    bounds.Y + (bounds.Height - iconSize.Height) / 2,
                    iconSize.Width,
                    iconSize.Height),

                IconPosition.Top when hasText => new Rectangle(
                    bounds.X + (bounds.Width - iconSize.Width) / 2,
                    bounds.Y,
                    iconSize.Width,
                    iconSize.Height),

                IconPosition.Bottom when hasText => new Rectangle(
                    bounds.X + (bounds.Width - iconSize.Width) / 2,
                    bounds.Bottom - iconSize.Height,
                    iconSize.Width,
                    iconSize.Height),

                _ => new Rectangle(
                    bounds.X + (bounds.Width - iconSize.Width) / 2,
                    bounds.Y + (bounds.Height - iconSize.Height) / 2,
                    iconSize.Width,
                    iconSize.Height)
            };
        }

        private Rectangle CalculateTextRect(Rectangle bounds, Rectangle iconRect)
        {
            return _iconSettings.Position switch
            {
                IconPosition.Left => new Rectangle(
                    iconRect.Right + _iconSettings.Spacing,
                    bounds.Y,
                    bounds.Width - iconRect.Width - _iconSettings.Spacing,
                    bounds.Height),

                IconPosition.Right => new Rectangle(
                    bounds.X,
                    bounds.Y,
                    bounds.Width - iconRect.Width - _iconSettings.Spacing,
                    bounds.Height),

                IconPosition.Top => new Rectangle(
                    bounds.X,
                    iconRect.Bottom + _iconSettings.Spacing,
                    bounds.Width,
                    bounds.Height - iconRect.Height - _iconSettings.Spacing),

                IconPosition.Bottom => new Rectangle(
                    bounds.X,
                    bounds.Y,
                    bounds.Width,
                    bounds.Height - iconRect.Height - _iconSettings.Spacing),

                _ => bounds
            };
        }

        #endregion

        #region Color Helpers

        private Color GetBackgroundColor()
        {
            return _buttonType switch
            {
                ButtonType.Contained or ButtonType.Floating =>
                    _isPressed ? ColorHelper.Darken(ColorScheme.Primary, 0.1f) : ColorScheme.Primary,
                _ => Color.Transparent
            };
        }

        private Color GetTextColor()
        {
            return _buttonType switch
            {
                ButtonType.Contained or ButtonType.Floating => ColorScheme.OnPrimary,
                _ => ColorScheme.Primary
            };
        }

        private Color GetHoverColor()
        {
            return _buttonType switch
            {
                ButtonType.Contained or ButtonType.Floating => Color.FromArgb(20, ColorScheme.OnPrimary),
                _ => Color.FromArgb(20, ColorScheme.Primary)
            };
        }

        private Color GetPressedColor()
        {
            return _buttonType switch
            {
                ButtonType.Contained or ButtonType.Floating => Color.FromArgb(40, ColorScheme.OnPrimary),
                _ => Color.FromArgb(40, ColorScheme.Primary)
            };
        }

        private Color GetRippleColor()
        {
            return _buttonType switch
            {
                ButtonType.Contained or ButtonType.Floating => Color.FromArgb(60, ColorScheme.OnPrimary),
                _ => Color.FromArgb(60, ColorScheme.Primary)
            };
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
