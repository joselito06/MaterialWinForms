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

namespace MaterialWinForms.Components.Buttons
{
    /// <summary>
    /// Split Button Material Design - Botón principal con dropdown de opciones
    /// </summary>
    [ToolboxItem(true)]
    [DesignTimeVisible(true)]
    public class MaterialSplitButton : MaterialControl
    {
        private string _text = "Split Button";
        private ButtonType _buttonType = ButtonType.Contained;
        private bool _isMainHovered = false;
        private bool _isDropdownHovered = false;
        private bool _isMainPressed = false;
        private bool _isDropdownPressed = false;
        private bool _isDropdownOpen = false;
        private Point _rippleLocation;
        private int _rippleSize = 0;
        private System.Windows.Forms.Timer? _rippleTimer;
        private CornerRadius _cornerRadius = new CornerRadius(20);
        private GradientSettings _gradient = new GradientSettings();
        private ShadowSettings _shadow = new ShadowSettings();
        private IconSettings _iconSettings = new IconSettings();
        private TextSettings _textSettings = new TextSettings();
        private Form? _dropdownForm;
        private List<SplitButtonItem> _dropdownItems = new List<SplitButtonItem>();

        public enum ButtonType
        {
            Contained,
            Outlined,
            Text
        }

        public class SplitButtonItem
        {
            public string TextContent { get; set; } = "";
            public Image? Icon { get; set; }
            public bool Enabled { get; set; } = true;
            public object? Tag { get; set; }
            public EventHandler? Click { get; set; }

            public SplitButtonItem(string text)
            {
                TextContent = text;
            }

            public SplitButtonItem(string text, Image? icon) : this(text)
            {
                Icon = icon;
            }

            public SplitButtonItem(string text, EventHandler clickHandler) : this(text)
            {
                Click = clickHandler;
            }

            public SplitButtonItem(string text, Image? icon, EventHandler clickHandler) : this(text, icon)
            {
                Click = clickHandler;
            }
        }

        #region Eventos

        public event EventHandler? MainButtonClick;
        public event EventHandler<SplitButtonItem>? DropdownItemClick;

        #endregion

        #region Propiedades

        [Category("Material")]
        [Description("Texto del botón principal")]
        public new string Text
        {
            get => _text;
            set
            {
                _text = value ?? "";
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

        [Category("Material")]
        [Description("Elementos del dropdown")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<SplitButtonItem> DropdownItems
        {
            get => _dropdownItems;
            set { _dropdownItems = value ?? new List<SplitButtonItem>(); }
        }

        [Category("Material - Appearance")]
        [Description("Configuración de esquinas redondeadas")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CornerRadius CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = value ?? new CornerRadius(20); Invalidate(); }
        }

        [Category("Material - Appearance")]
        [Description("Configuración de gradiente")]
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

        #endregion

        public MaterialSplitButton()
        {
            Size = new Size(140, 40);
            Cursor = Cursors.Hand;
            Padding = new Padding(12, 8, 32, 8); // Espacio extra para la flecha

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
                default:
                    _shadow.Type = MaterialShadowType.None;
                    break;
            }
        }

        #region Métodos públicos

        public void AddDropdownItem(string text)
        {
            _dropdownItems.Add(new SplitButtonItem(text));
        }

        public void AddDropdownItem(string text, EventHandler clickHandler)
        {
            _dropdownItems.Add(new SplitButtonItem(text, clickHandler));
        }

        public void AddDropdownItem(string text, Image icon, EventHandler clickHandler)
        {
            _dropdownItems.Add(new SplitButtonItem(text, icon, clickHandler));
        }

        public void ClearDropdownItems()
        {
            _dropdownItems.Clear();
        }

        #endregion

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

        protected override void OnMouseMove(MouseEventArgs e)
        {
            var dropdownRect = GetDropdownRect();
            var wasMainHovered = _isMainHovered;
            var wasDropdownHovered = _isDropdownHovered;

            _isMainHovered = !dropdownRect.Contains(e.Location);
            _isDropdownHovered = dropdownRect.Contains(e.Location);

            if (wasMainHovered != _isMainHovered || wasDropdownHovered != _isDropdownHovered)
            {
                Invalidate();
            }

            Cursor = _isDropdownHovered ? Cursors.Hand : Cursors.Hand;
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isMainHovered = false;
            _isDropdownHovered = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            var dropdownRect = GetDropdownRect();

            if (dropdownRect.Contains(e.Location))
            {
                _isDropdownPressed = true;
                if (UseRippleEffect && e.Button == MouseButtons.Left)
                {
                    _rippleLocation = e.Location;
                    _rippleSize = 0;
                    _rippleTimer?.Start();
                }
            }
            else
            {
                _isMainPressed = true;
                if (UseRippleEffect && e.Button == MouseButtons.Left)
                {
                    _rippleLocation = e.Location;
                    _rippleSize = 0;
                    _rippleTimer?.Start();
                }
            }

            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isMainPressed = false;
            _isDropdownPressed = false;
            Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnClick(EventArgs e)
        {
            var mouseArgs = e as MouseEventArgs ?? new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0);
            var dropdownRect = GetDropdownRect();

            if (dropdownRect.Contains(mouseArgs.Location))
            {
                ToggleDropdown();
            }
            else
            {
                MainButtonClick?.Invoke(this, e);
            }

            base.OnClick(e);
        }

        protected override void OnColorSchemeChanged()
        {
            base.OnColorSchemeChanged();
            Invalidate();
        }

        #endregion

        #region Dropdown

        private Rectangle GetDropdownRect()
        {
            return new Rectangle(Width - 32, 0, 32, Height);
        }

        private void ToggleDropdown()
        {
            if (_isDropdownOpen)
            {
                CloseDropdown();
            }
            else
            {
                ShowDropdown();
            }
        }

        private void ShowDropdown()
        {
            if (_dropdownItems.Count == 0) return;

            _isDropdownOpen = true;

            var dropdownWidth = Math.Max(Width, 200);
            var itemHeight = 36;
            var dropdownHeight = _dropdownItems.Count * itemHeight + 8; // Padding

            _dropdownForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                Size = new Size(dropdownWidth, dropdownHeight),
                TopMost = true,
                ShowInTaskbar = false,
                BackColor = ColorScheme.Surface
            };

            // Posicionar dropdown
            var location = PointToScreen(new Point(0, Height + 4));
            var screen = Screen.FromControl(this);

            if (location.X + dropdownWidth > screen.WorkingArea.Right)
                location.X = screen.WorkingArea.Right - dropdownWidth;
            if (location.Y + dropdownHeight > screen.WorkingArea.Bottom)
                location.Y = PointToScreen(Point.Empty).Y - dropdownHeight - 4;

            _dropdownForm.Location = location;

            // Crear items del dropdown
            CreateDropdownItems();

            _dropdownForm.Deactivate += (s, e) => CloseDropdown();
            _dropdownForm.Show();
        }

        private void CreateDropdownItems()
        {
            if (_dropdownForm == null) return;

            var itemHeight = 36;
            var y = 4;

            foreach (var item in _dropdownItems)
            {
                var itemPanel = new Panel
                {
                    Size = new Size(_dropdownForm.Width - 8, itemHeight),
                    Location = new Point(4, y),
                    Cursor = item.Enabled ? Cursors.Hand : Cursors.Default,
                    Tag = item
                };

                itemPanel.Paint += (s, e) => DrawDropdownItem(e.Graphics, itemPanel, item);

                if (item.Enabled)
                {
                    itemPanel.MouseEnter += (s, e) => { itemPanel.BackColor = Color.FromArgb(20, ColorScheme.OnSurface); };
                    itemPanel.MouseLeave += (s, e) => { itemPanel.BackColor = Color.Transparent; };
                    itemPanel.Click += (s, e) =>
                    {
                        item.Click?.Invoke(this, e);
                        DropdownItemClick?.Invoke(this, item);
                        CloseDropdown();
                    };
                }

                _dropdownForm.Controls.Add(itemPanel);
                y += itemHeight;
            }
        }

        private void DrawDropdownItem(Graphics g, Panel panel, SplitButtonItem item)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var textColor = item.Enabled ? ColorScheme.OnSurface : Color.FromArgb(128, ColorScheme.OnSurface);
            var iconX = 12;
            var textX = iconX;

            // Dibujar icono si existe
            if (item.Icon != null)
            {
                var iconSize = 20;
                var iconRect = new Rectangle(iconX, (panel.Height - iconSize) / 2, iconSize, iconSize);

                if (item.Enabled)
                    g.DrawImage(item.Icon, iconRect);
                else
                {
                    using var disabledIcon = g.ApplyTint(item.Icon, Color.FromArgb(128, Color.Gray));
                    g.DrawImage(disabledIcon, iconRect);
                }

                textX += iconSize + 8;
            }

            // Dibujar texto
            using var font = new Font("Segoe UI", 9f);
            using var brush = new SolidBrush(textColor);
            var textRect = new Rectangle(textX, 0, panel.Width - textX - 12, panel.Height);
            var sf = new StringFormat { LineAlignment = StringAlignment.Center };

            g.DrawString(item.TextContent, font, brush, textRect, sf);
        }

        private void CloseDropdown()
        {
            if (_dropdownForm != null && !_dropdownForm.IsDisposed)
            {
                _dropdownForm.Close();
                _dropdownForm.Dispose();
                _dropdownForm = null;
            }
            _isDropdownOpen = false;
        }

        #endregion

        #region Painting

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

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

            // Dibujar línea divisoria
            DrawDivider(g, bounds);

            // Dibujar contenido
            DrawContent(g, bounds);

            // Dibujar efectos
            DrawEffects(g, bounds);
        }

        private Padding CalculateShadowPadding()
        {
            if (_shadow.Type == MaterialShadowType.None) return Padding.Empty;

            var blur = _shadow.Blur;
            var offsetX = Math.Abs(_shadow.OffsetX);
            var offsetY = Math.Abs(_shadow.OffsetY);

            return new Padding(blur + offsetX + 2, blur + offsetY + 2, blur + offsetX + 2, blur + offsetY + 2);
        }

        private void DrawBackground(Graphics g, Rectangle bounds)
        {
            if (_buttonType == ButtonType.Text) return;

            using var path = GraphicsExtensions.CreateRoundedRectanglePath(bounds, _cornerRadius);
            var oldClip = g.Clip;
            g.SetClip(path);

            if (_gradient.Type != GradientType.None)
            {
                g.FillRoundedRectangleWithGradient(bounds, _cornerRadius, _gradient);
            }
            else
            {
                var backgroundColor = GetBackgroundColor();
                using var brush = new SolidBrush(backgroundColor);
                g.FillRoundedRectangle(brush, bounds, _cornerRadius);
            }

            g.Clip = oldClip;

            // Dibujar borde si es outlined
            if (_buttonType == ButtonType.Outlined)
            {
                var borderColor = ColorScheme.Primary;
                using var pen = new Pen(borderColor, 1);
                g.DrawRoundedRectangle(pen, bounds, _cornerRadius);
            }
        }

        private void DrawDivider(Graphics g, Rectangle bounds)
        {
            if (_buttonType == ButtonType.Text) return;

            var dividerX = bounds.Right - 32;
            var dividerColor = GetDividerColor();

            using var pen = new Pen(dividerColor, 1);
            g.DrawLine(pen, dividerX, bounds.Y + 8, dividerX, bounds.Bottom - 8);
        }

        private void DrawContent(Graphics g, Rectangle bounds)
        {
            // Área del botón principal
            var mainRect = new Rectangle(bounds.X, bounds.Y, bounds.Width - 32, bounds.Height);
            var dropdownRect = new Rectangle(bounds.Right - 32, bounds.Y, 32, bounds.Height);

            // Dibujar contenido del botón principal
            DrawMainContent(g, mainRect);

            // Dibujar flecha del dropdown
            DrawDropdownArrow(g, dropdownRect);
        }

        private void DrawMainContent(Graphics g, Rectangle bounds)
        {
            var contentBounds = new Rectangle(
                bounds.X + Padding.Left,
                bounds.Y + Padding.Top,
                bounds.Width - Padding.Left - 4,
                bounds.Height - Padding.Vertical
            );

            var hasIcon = _iconSettings.Icon != null;
            var hasText = !string.IsNullOrEmpty(_text);

            if (hasIcon && hasText)
            {
                // Icono y texto
                var iconRect = new Rectangle(
                    contentBounds.X,
                    contentBounds.Y + (contentBounds.Height - _iconSettings.Size) / 2,
                    _iconSettings.Size,
                    _iconSettings.Size
                );

                var textRect = new Rectangle(
                    iconRect.Right + _iconSettings.Spacing,
                    contentBounds.Y,
                    contentBounds.Width - iconRect.Width - _iconSettings.Spacing,
                    contentBounds.Height
                );

                var textColor = GetTextColor();
                g.DrawIcon(_iconSettings.Icon!, contentBounds, _iconSettings, textColor);

                using var font = new Font(_textSettings.FontFamily, _textSettings.FontSize, _textSettings.FontStyle);
                using var brush = new SolidBrush(textColor);
                using var sf = new StringFormat { LineAlignment = StringAlignment.Center };

                g.DrawString(_text, font, brush, textRect, sf);
            }
            else if (hasText)
            {
                // Solo texto
                var textColor = GetTextColor();
                using var font = new Font(_textSettings.FontFamily, _textSettings.FontSize, _textSettings.FontStyle);
                using var brush = new SolidBrush(textColor);
                using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

                g.DrawString(_text, font, brush, contentBounds, sf);
            }
            else if (hasIcon)
            {
                // Solo icono
                var textColor = GetTextColor();
                g.DrawIcon(_iconSettings.Icon!, contentBounds, _iconSettings, textColor);
            }
        }

        private void DrawDropdownArrow(Graphics g, Rectangle bounds)
        {
            var arrowColor = GetTextColor();
            using var pen = new Pen(arrowColor, 2f)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };

            var centerX = bounds.X + bounds.Width / 2;
            var centerY = bounds.Y + bounds.Height / 2;
            var size = 4;

            // Flecha hacia abajo
            g.DrawLine(pen, centerX - size, centerY - 2, centerX, centerY + 2);
            g.DrawLine(pen, centerX, centerY + 2, centerX + size, centerY - 2);
        }

        private void DrawEffects(Graphics g, Rectangle bounds)
        {
            var mainRect = new Rectangle(bounds.X, bounds.Y, bounds.Width - 32, bounds.Height);
            var dropdownRect = new Rectangle(bounds.Right - 32, bounds.Y, 32, bounds.Height);

            using var mainPath = GraphicsExtensions.CreateRoundedRectanglePath(mainRect,
                new CornerRadius(_cornerRadius.TopLeft, 0, _cornerRadius.BottomLeft, 0));
            using var dropdownPath = GraphicsExtensions.CreateRoundedRectanglePath(dropdownRect,
                new CornerRadius(0, _cornerRadius.TopRight, 0, _cornerRadius.BottomRight));

            var oldClip = g.Clip;

            // Efectos en el botón principal
            if (_isMainHovered || _isMainPressed)
            {
                g.SetClip(mainPath);
                var effectColor = _isMainPressed ? GetPressedColor() : GetHoverColor();
                using var brush = new SolidBrush(effectColor);
                g.FillPath(brush, mainPath);
            }

            // Efectos en el dropdown
            if (_isDropdownHovered || _isDropdownPressed)
            {
                g.SetClip(dropdownPath);
                var effectColor = _isDropdownPressed ? GetPressedColor() : GetHoverColor();
                using var brush = new SolidBrush(effectColor);
                g.FillPath(brush, dropdownPath);
            }

            // Efecto ripple
            if (UseRippleEffect && _rippleSize > 0)
            {
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

            g.Clip = oldClip;
        }

        #endregion

        #region Color Helpers

        private Color GetBackgroundColor()
        {
            return _buttonType switch
            {
                ButtonType.Contained => ColorScheme.Primary,
                _ => Color.Transparent
            };
        }

        private Color GetTextColor()
        {
            return _buttonType switch
            {
                ButtonType.Contained => ColorScheme.OnPrimary,
                _ => ColorScheme.Primary
            };
        }

        private Color GetDividerColor()
        {
            return _buttonType switch
            {
                ButtonType.Contained => Color.FromArgb(60, ColorScheme.OnPrimary),
                _ => Color.FromArgb(60, ColorScheme.Primary)
            };
        }

        private Color GetHoverColor()
        {
            return _buttonType switch
            {
                ButtonType.Contained => Color.FromArgb(20, ColorScheme.OnPrimary),
                _ => Color.FromArgb(20, ColorScheme.Primary)
            };
        }

        private Color GetPressedColor()
        {
            return _buttonType switch
            {
                ButtonType.Contained => Color.FromArgb(30, ColorScheme.OnPrimary),
                _ => Color.FromArgb(30, ColorScheme.Primary)
            };
        }

        private Color GetRippleColor()
        {
            return _buttonType switch
            {
                ButtonType.Contained => Color.FromArgb(40, ColorScheme.OnPrimary),
                _ => Color.FromArgb(40, ColorScheme.Primary)
            };
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _rippleTimer?.Stop();
                _rippleTimer?.Dispose();
                CloseDropdown();
            }
            base.Dispose(disposing);
        }
    }
}
