using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialWinForms.Core;
using MaterialWinForms.Utils;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;

namespace MaterialWinForms.Components.Containers
{
    /// <summary>
    /// Card Material Design para contenedores con características completas
    /// </summary>
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(IDesigner))]
    [ToolboxItem(true)]
    [DesignTimeVisible(true)]
    //[DesignerSerializationVisibility(true)]
    public class MaterialCard : MaterialControl
    {
        private string _title = "";
        private string _subtitle = "";
        private Image? _cardImage = null;
        private bool _showDivider = true;
        private bool _isClickable = false;
        private bool _isHovered = false;
        private bool _isPressed = false;
        private Point _rippleLocation;
        private int _rippleSize = 0;
        private System.Windows.Forms.Timer? _rippleTimer;
        private CornerRadius _cornerRadius = new CornerRadius(12);
        private GradientSettings _gradient = new GradientSettings();
        private ShadowSettings _shadow = new ShadowSettings();
        private TextSettings _titleSettings = new TextSettings();
        private TextSettings _subtitleSettings = new TextSettings();
        private CardImageSettings _imageSettings = new CardImageSettings();
        private Color _customBackColor = Color.Transparent;

        public class CardImageSettings
        {
            [DefaultValue(120)]
            [Description("Altura de la imagen en píxeles")]
            public int Height { get; set; } = 120;

            [DefaultValue(ImagePosition.Top)]
            [Description("Posición de la imagen en la card")]
            public ImagePosition Position { get; set; } = ImagePosition.Top;

            [DefaultValue(ContentAlignment.MiddleCenter)]
            [Description("Alineación de la imagen")]
            public ContentAlignment Alignment { get; set; } = ContentAlignment.MiddleCenter;

            [DefaultValue(true)]
            [Description("Esquinas redondeadas en la imagen")]
            public bool RoundedCorners { get; set; } = true;

            [DefaultValue(ImageScaleMode.StretchToFill)]
            [Description("Modo de escala de la imagen")]
            public ImageScaleMode ScaleMode { get; set; } = ImageScaleMode.StretchToFill;

            [DefaultValue(0)]
            [Description("Margen alrededor de la imagen")]
            public int Margin { get; set; } = 0;

            public enum ImagePosition
            {
                Top,
                Left,
                Right,
                Background
            }

            public enum ImageScaleMode
            {
                None,           // Tamaño original
                StretchToFill,  // Estirar para llenar
                ScaleToFit,     // Escalar manteniendo proporción
                Crop            // Recortar para llenar
            }

            public override string ToString()
            {
                return $"{Position}, {ScaleMode}, H={Height}";
            }
        }

        #region Eventos

        public event EventHandler? CardClick;
        public event EventHandler? CardDoubleClick;

        #endregion

        #region Propiedades principales para el Designer

        [Category("Material - Content")]
        [Description("Título principal de la card")]
        [DefaultValue("")]
        public string Title
        {
            get => _title;
            set { _title = value ?? ""; CalculateLayout(); Invalidate(); }
        }

        [Category("Material - Content")]
        [Description("Subtítulo de la card")]
        [DefaultValue("")]
        public string Subtitle
        {
            get => _subtitle;
            set { _subtitle = value ?? ""; CalculateLayout(); Invalidate(); }
        }

        [Category("Material - Content")]
        [Description("Imagen opcional de la card")]
        [DefaultValue(null)]
        public Image? CardImage
        {
            get => _cardImage;
            set { _cardImage = value; CalculateLayout(); Invalidate(); }
        }

        [Category("Material - Content")]
        [Description("Altura de la imagen en píxeles")]
        [DefaultValue(120)]
        public int ImageHeight
        {
            get => _imageSettings.Height;
            set { _imageSettings.Height = Math.Max(50, value); CalculateLayout(); Invalidate(); }
        }

        [Category("Material - Content")]
        [Description("Modo de escala de la imagen")]
        [DefaultValue(CardImageSettings.ImageScaleMode.StretchToFill)]
        public CardImageSettings.ImageScaleMode ImageScaleMode
        {
            get => _imageSettings.ScaleMode;
            set { _imageSettings.ScaleMode = value; Invalidate(); }
        }

        [Category("Material - Content")]
        [Description("Margen alrededor de la imagen")]
        [DefaultValue(0)]
        public int ImageMargin
        {
            get => _imageSettings.Margin;
            set { _imageSettings.Margin = Math.Max(0, value); CalculateLayout(); Invalidate(); }
        }

        [Category("Material - Behavior")]
        [Description("Mostrar línea divisoria después del encabezado")]
        [DefaultValue(true)]
        public bool ShowDivider
        {
            get => _showDivider;
            set { _showDivider = value; Invalidate(); }
        }

        [Category("Material - Behavior")]
        [Description("La card responde a clicks")]
        [DefaultValue(false)]
        public bool IsClickable
        {
            get => _isClickable;
            set
            {
                _isClickable = value;
                Cursor = value ? Cursors.Hand : Cursors.Default;
                if (value)
                {
                    _rippleTimer = new System.Windows.Forms.Timer { Interval = 10 };
                    _rippleTimer.Tick += RippleTimer_Tick;
                }
                else
                {
                    _rippleTimer?.Stop();
                    _rippleTimer?.Dispose();
                    _rippleTimer = null;
                }
            }
        }

        [Category("Material - Behavior")]
        [Description("Color de fondo personalizado")]
        [DefaultValue(typeof(Color), "Transparent")]
        public Color CustomBackColor
        {
            get => _customBackColor;
            set { _customBackColor = value; Invalidate(); }
        }

        #endregion

        #region Propiedades de configuración avanzada

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

        [Category("Material - Appearance")]
        [Description("Configuración del título")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TextSettings TitleSettings
        {
            get => _titleSettings;
            set { _titleSettings = value ?? new TextSettings(); Invalidate(); }
        }

        [Category("Material - Appearance")]
        [Description("Configuración del subtítulo")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TextSettings SubtitleSettings
        {
            get => _subtitleSettings;
            set { _subtitleSettings = value ?? new TextSettings(); Invalidate(); }
        }

        [Category("Material - Appearance")]
        [Description("Configuración de la imagen")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CardImageSettings ImageSettings
        {
            get => _imageSettings;
            set { _imageSettings = value ?? new CardImageSettings(); CalculateLayout(); Invalidate(); }
        }

        [Category("Material")]
        [Description("Elevación de la card (sombra automática)")]
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

        public MaterialCard()
        {
            // Configurar como contenedor
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.ContainerControl, true);

            Size = new Size(300, 200);
            Elevation = 2;
            Padding = new Padding(16);

            // Configurar sombra por defecto
            _shadow.Type = MaterialShadowType.Soft;
            _shadow.Opacity = 30;
            _shadow.Blur = 4;
            _shadow.OffsetY = 2;

            // Configurar texto por defecto
            _titleSettings.FontFamily = "Segoe UI";
            _titleSettings.FontSize = 16f;
            _titleSettings.FontStyle = FontStyle.Bold;

            _subtitleSettings.FontFamily = "Segoe UI";
            _subtitleSettings.FontSize = 12f;
            _subtitleSettings.FontStyle = FontStyle.Regular;

            // Habilitar scroll si es necesario
            AutoScroll = true;
            AllowDrop = true;

            CalculateLayout();
        }

        private void UpdateElevationShadow()
        {
            _shadow.Blur = Math.Max(2, Elevation * 2);
            _shadow.OffsetY = Math.Max(1, Elevation / 2);
            _shadow.Opacity = Math.Min(50, 20 + Elevation * 2);
        }

        private void CalculateLayout()
        {
            // Calcular el espacio necesario para el header
            var headerHeight = 0;

            if (!string.IsNullOrEmpty(_title))
                headerHeight += (int)_titleSettings.FontSize + 8;

            if (!string.IsNullOrEmpty(_subtitle))
                headerHeight += (int)_subtitleSettings.FontSize + 4;

            if (_showDivider && headerHeight > 0)
                headerHeight += 8;

            // Ajustar padding superior si hay imagen en top
            if (_cardImage != null && _imageSettings.Position == CardImageSettings.ImagePosition.Top)
            {
                // Dejar espacio para imagen + header + margen
                var totalHeaderSpace = _imageSettings.Height + headerHeight + 16;
                Padding = new Padding(16, totalHeaderSpace, 16, 16);
            }
            else if (headerHeight > 0)
            {
                // Solo header sin imagen
                Padding = new Padding(16, headerHeight + 24, 16, 16);
            }
            else
            {
                // Sin header ni imagen
                Padding = new Padding(16);
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
                _gradient.EndColor = ColorHelper.Lighten(ColorScheme.Surface, 0.05f);
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

        protected override void OnMouseEnter(EventArgs e)
        {
            if (_isClickable)
            {
                _isHovered = true;
                // Elevar ligeramente la card
                var originalElevation = Elevation;
                if (originalElevation < 12)
                {
                    _shadow.Blur += 2;
                    _shadow.OffsetY += 1;
                    _shadow.Opacity = Math.Min(60, _shadow.Opacity + 10);
                }
                Invalidate();
            }
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (_isClickable)
            {
                _isHovered = false;
                UpdateElevationShadow(); // Restaurar sombra original
                Invalidate();
            }
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (_isClickable)
            {
                _isPressed = true;
                if (UseRippleEffect && e.Button == MouseButtons.Left)
                {
                    _rippleLocation = e.Location;
                    _rippleSize = 0;
                    _rippleTimer?.Start();
                }

                // Reducir elevación al presionar
                _shadow.Blur = Math.Max(2, _shadow.Blur - 2);
                _shadow.OffsetY = Math.Max(1, _shadow.OffsetY - 1);
                Invalidate();
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_isClickable)
            {
                _isPressed = false;
                UpdateElevationShadow(); // Restaurar sombra
                Invalidate();
            }
            base.OnMouseUp(e);
        }

        protected override void OnClick(EventArgs e)
        {
            if (_isClickable)
            {
                CardClick?.Invoke(this, e);
            }
            base.OnClick(e);
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            if (_isClickable)
            {
                CardDoubleClick?.Invoke(this, e);
            }
            base.OnDoubleClick(e);
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

            // Configurar clipping para contenido
            using var cardPath = GraphicsExtensions.CreateRoundedRectanglePath(bounds, _cornerRadius);
            var oldClip = g.Clip;
            g.SetClip(cardPath);

            // Dibujar imagen de fondo si corresponde
            if (_cardImage != null && _imageSettings.Position == CardImageSettings.ImagePosition.Background)
            {
                DrawBackgroundImage(g, bounds);
            }

            // Dibujar efectos de interacción
            DrawInteractionEffects(g, bounds);

            // Dibujar efecto ripple
            DrawRippleEffect(g, bounds);

            g.Clip = oldClip;

            // Dibujar contenido
            DrawCardContent(g, bounds);
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
                using var brush = new SolidBrush(backgroundColor);
                g.FillRoundedRectangle(brush, bounds, _cornerRadius);
            }
        }

        private void DrawBackgroundImage(Graphics g, Rectangle bounds)
        {
            if (_cardImage == null) return;

            // Dibujar imagen como fondo con transparencia
            var imageAttribs = new System.Drawing.Imaging.ImageAttributes();
            var colorMatrix = new System.Drawing.Imaging.ColorMatrix
            {
                Matrix33 = 0.3f // 30% de opacidad
            };
            imageAttribs.SetColorMatrix(colorMatrix);

            g.DrawImage(_cardImage, bounds, 0, 0, _cardImage.Width, _cardImage.Height, GraphicsUnit.Pixel, imageAttribs);
        }

        private void DrawInteractionEffects(Graphics g, Rectangle bounds)
        {
            if (!_isClickable) return;

            // Efecto hover
            if (_isHovered && !_isPressed)
            {
                var hoverColor = Color.FromArgb(10, ColorScheme.OnSurface);
                using var brush = new SolidBrush(hoverColor);
                g.FillRoundedRectangle(brush, bounds, _cornerRadius);
            }

            // Efecto pressed
            if (_isPressed)
            {
                var pressedColor = Color.FromArgb(20, ColorScheme.OnSurface);
                using var brush = new SolidBrush(pressedColor);
                g.FillRoundedRectangle(brush, bounds, _cornerRadius);
            }
        }

        private void DrawRippleEffect(Graphics g, Rectangle bounds)
        {
            if (!_isClickable || !UseRippleEffect || _rippleSize <= 0) return;

            var rippleColor = Color.FromArgb(30, ColorScheme.OnSurface);
            using var brush = new SolidBrush(rippleColor);
            var rippleRect = new Rectangle(
                _rippleLocation.X - _rippleSize / 2,
                _rippleLocation.Y - _rippleSize / 2,
                _rippleSize,
                _rippleSize
            );
            g.FillEllipse(brush, rippleRect);
        }

        private void DrawCardContent(Graphics g, Rectangle bounds)
        {
            var currentY = bounds.Y + 16;

            // Dibujar imagen si está en top
            if (_cardImage != null && _imageSettings.Position == CardImageSettings.ImagePosition.Top)
            {
                currentY += DrawTopImage(g, bounds, currentY);
            }

            // Dibujar título
            if (!string.IsNullOrEmpty(_title))
            {
                currentY += DrawTitle(g, bounds, currentY);
            }

            // Dibujar subtítulo
            if (!string.IsNullOrEmpty(_subtitle))
            {
                currentY += DrawSubtitle(g, bounds, currentY);
            }

            // Dibujar divisor
            if (_showDivider && (!string.IsNullOrEmpty(_title) || !string.IsNullOrEmpty(_subtitle)))
            {
                DrawDivider(g, bounds, currentY);
            }
        }

        private int DrawTopImage(Graphics g, Rectangle bounds, int startY)
        {
            if (_cardImage == null) return 0;

            var imageHeight = _imageSettings.Height;
            // La imagen debe ocupar todo el ancho del card, sin márgenes laterales
            var imageRect = new Rectangle(
                bounds.X + _imageSettings.Margin,
                bounds.Y + _imageSettings.Margin,
                bounds.Width - (_imageSettings.Margin * 2),
                imageHeight - (_imageSettings.Margin * 2)
            );

            // Crear clipping si se necesitan esquinas redondeadas
            GraphicsPath? imagePath = null;
            Region? oldClip = null;

            if (_imageSettings.RoundedCorners)
            {
                // Solo redondear las esquinas superiores para imagen top
                var imageCorners = _imageSettings.Margin > 0
                    ? new CornerRadius(Math.Max(0, _cornerRadius.TopLeft - _imageSettings.Margin),
                                     Math.Max(0, _cornerRadius.TopRight - _imageSettings.Margin), 0, 0)
                    : new CornerRadius(_cornerRadius.TopLeft, _cornerRadius.TopRight, 0, 0);

                imagePath = GraphicsExtensions.CreateRoundedRectanglePath(imageRect, imageCorners);
                oldClip = g.Clip;
                g.SetClip(imagePath);
            }

            // Dibujar la imagen según el modo de escala
            DrawScaledImage(g, _cardImage, imageRect, _imageSettings.ScaleMode);

            // Restaurar clipping
            if (_imageSettings.RoundedCorners)
            {
                g.Clip = oldClip;
                imagePath?.Dispose();
            }

            return imageHeight + 8; // Retornar altura total + margen
        }

        private void DrawScaledImage(Graphics g, Image image, Rectangle targetRect, CardImageSettings.ImageScaleMode scaleMode)
        {
            switch (scaleMode)
            {
                case CardImageSettings.ImageScaleMode.None:
                    // Tamaño original, centrado
                    var originalRect = new Rectangle(
                        targetRect.X + (targetRect.Width - image.Width) / 2,
                        targetRect.Y + (targetRect.Height - image.Height) / 2,
                        image.Width,
                        image.Height
                    );
                    g.DrawImage(image, originalRect);
                    break;

                case CardImageSettings.ImageScaleMode.StretchToFill:
                    // Estirar para llenar completamente
                    g.DrawImage(image, targetRect);
                    break;

                case CardImageSettings.ImageScaleMode.ScaleToFit:
                    // Escalar manteniendo proporción
                    var scaleX = (float)targetRect.Width / image.Width;
                    var scaleY = (float)targetRect.Height / image.Height;
                    var scale = Math.Min(scaleX, scaleY);

                    var scaledWidth = (int)(image.Width * scale);
                    var scaledHeight = (int)(image.Height * scale);

                    var scaledRect = new Rectangle(
                        targetRect.X + (targetRect.Width - scaledWidth) / 2,
                        targetRect.Y + (targetRect.Height - scaledHeight) / 2,
                        scaledWidth,
                        scaledHeight
                    );
                    g.DrawImage(image, scaledRect);
                    break;

                case CardImageSettings.ImageScaleMode.Crop:
                    // Recortar para llenar manteniendo proporción
                    var cropScaleX = (float)targetRect.Width / image.Width;
                    var cropScaleY = (float)targetRect.Height / image.Height;
                    var cropScale = Math.Max(cropScaleX, cropScaleY);

                    var cropWidth = (int)(image.Width * cropScale);
                    var cropHeight = (int)(image.Height * cropScale);

                    // Calcular el área de recorte del imagen original
                    var sourceWidth = (int)(targetRect.Width / cropScale);
                    var sourceHeight = (int)(targetRect.Height / cropScale);
                    var sourceX = (image.Width - sourceWidth) / 2;
                    var sourceY = (image.Height - sourceHeight) / 2;

                    g.DrawImage(image, targetRect, sourceX, sourceY, sourceWidth, sourceHeight, GraphicsUnit.Pixel);
                    break;
            }
        }

        private int DrawTitle(Graphics g, Rectangle bounds, int startY)
        {
            using var font = new Font(_titleSettings.FontFamily, _titleSettings.FontSize, _titleSettings.FontStyle);
            using var brush = new SolidBrush(ColorScheme.OnSurface);

            var titleRect = new Rectangle(bounds.X + 16, startY, bounds.Width - 32, (int)_titleSettings.FontSize + 8);
            var sf = GetStringFormat(_titleSettings.Alignment);

            g.DrawString(_title, font, brush, titleRect, sf);
            return titleRect.Height;
        }

        private int DrawSubtitle(Graphics g, Rectangle bounds, int startY)
        {
            using var font = new Font(_subtitleSettings.FontFamily, _subtitleSettings.FontSize, _subtitleSettings.FontStyle);
            using var brush = new SolidBrush(Color.FromArgb(180, ColorScheme.OnSurface));

            var subtitleRect = new Rectangle(bounds.X + 16, startY, bounds.Width - 32, (int)_subtitleSettings.FontSize + 4);
            var sf = GetStringFormat(_subtitleSettings.Alignment);

            g.DrawString(_subtitle, font, brush, subtitleRect, sf);
            return subtitleRect.Height;
        }

        private void DrawDivider(Graphics g, Rectangle bounds, int y)
        {
            using var pen = new Pen(Color.FromArgb(30, ColorScheme.OnSurface), 1);
            g.DrawLine(pen, bounds.X + 16, y + 4, bounds.Right - 16, y + 4);
        }

        private StringFormat GetStringFormat(ContentAlignment alignment)
        {
            var sf = new StringFormat();

            switch (alignment)
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

        private Color GetBackgroundColor()
        {
            return _customBackColor != Color.Transparent ? _customBackColor : ColorScheme.Surface;
        }

        #endregion

        #region Propiedades de contenedor para el Designer

        [Browsable(true)]
        [Category("Layout")]
        [Description("Controles contenidos en la card")]
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
