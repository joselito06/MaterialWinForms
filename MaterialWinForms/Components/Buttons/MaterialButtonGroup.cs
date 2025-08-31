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
    /// Button Group Material Design - Agrupador de botones con estilos consistentes
    /// </summary>
    public class MaterialButtonGroup : MaterialControl
    {
        private List<ButtonGroupItem> _buttons = new List<ButtonGroupItem>();
        private GroupOrientation _orientation = GroupOrientation.Horizontal;
        private ButtonGroupStyle _groupStyle = ButtonGroupStyle.Contained;
        private int _spacing = 1;
        private bool _equalSizes = true;
        private CornerRadius _cornerRadius = new CornerRadius(20);
        private ShadowSettings _shadow = new ShadowSettings();
        private GradientSettings _gradient = new GradientSettings();

        public enum GroupOrientation
        {
            Horizontal,
            Vertical
        }

        public enum ButtonGroupStyle
        {
            Contained,   // Botones unidos sin separación
            Outlined,    // Botones con bordes unidos
            Separated,   // Botones separados con spacing
            Floating     // Botones flotantes con sombra individual
        }

        public class ButtonGroupItem : MaterialControl
        {
            private MaterialButtonGroup? _parent;
            private string _text = "";
            private Image? _icon;
            private bool _isSelected = false;
            private bool _isHovered = false;
            private bool _isPressed = false;
            private Point _rippleLocation;
            private int _rippleSize = 0;
            private System.Windows.Forms.Timer? _rippleTimer;

            public event EventHandler? Click;
            public event EventHandler? SelectionChanged;

            #region Propiedades

            [Category("Material")]
            [Description("Texto del botón")]
            public new string Text
            {
                get => _text;
                set { _text = value ?? ""; Invalidate(); OnTextChanged(EventArgs.Empty); }
            }

            [Category("Material")]
            [Description("Icono del botón")]
            public Image? Icon
            {
                get => _icon;
                set { _icon = value; Invalidate(); }
            }

            [Category("Material")]
            [Description("Estado de selección del botón")]
            public bool IsSelected
            {
                get => _isSelected;
                set
                {
                    if (_isSelected != value)
                    {
                        _isSelected = value;
                        SelectionChanged?.Invoke(this, EventArgs.Empty);
                        Invalidate();
                    }
                }
            }

            [Browsable(false)]
            public MaterialButtonGroup? ParentGroup
            {
                get => _parent;
                set => _parent = value;
            }

            #endregion

            public ButtonGroupItem()
            {
                Cursor = Cursors.Hand;
                Size = new Size(80, 40);

                _rippleTimer = new System.Windows.Forms.Timer { Interval = 10 };
                _rippleTimer.Tick += (s, e) =>
                {
                    _rippleSize += 6;
                    if (_rippleSize > Math.Max(Width, Height) * 1.5)
                    {
                        _rippleTimer?.Stop();
                        _rippleSize = 0;
                    }
                    Invalidate();
                };
            }

            public ButtonGroupItem(string text) : this()
            {
                Text = text;
            }

            public ButtonGroupItem(string text, Image? icon) : this(text)
            {
                Icon = icon;
            }

            #region Event Handlers

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
                Click?.Invoke(this, e);
                base.OnClick(e);
            }

            #endregion

            #region Painting

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                if (_parent == null) return;

                var bounds = new Rectangle(0, 0, Width, Height);
                var corners = _parent.GetItemCorners(this);

                // Dibujar según el estilo del grupo
                DrawItemBackground(g, bounds, corners);
                DrawItemEffects(g, bounds, corners);
                DrawItemContent(g, bounds);
            }

            private void DrawItemBackground(Graphics g, Rectangle bounds, CornerRadius corners)
            {
                if (_parent?.GroupStyle == ButtonGroupStyle.Separated ||
                    _parent?.GroupStyle == ButtonGroupStyle.Floating)
                {
                    // Sombra individual para botones separados
                    if (_parent.Shadow.Type != MaterialShadowType.None)
                    {
                        g.DrawMaterialShadow(bounds, corners, _parent.Shadow);
                    }
                }

                // Fondo del botón
                var backgroundColor = GetBackgroundColor();
                if (backgroundColor != Color.Transparent)
                {
                    if (_parent?.Gradient.Type != GradientType.None && IsSelected)
                    {
                        g.FillRoundedRectangleWithGradient(bounds, corners, _parent.Gradient);
                    }
                    else
                    {
                        using var brush = new SolidBrush(backgroundColor);
                        g.FillRoundedRectangle(brush, bounds, corners);
                    }
                }

                // Borde para outlined
                if (_parent?.GroupStyle == ButtonGroupStyle.Outlined)
                {
                    var borderColor = IsSelected ? ColorScheme.Primary : ColorScheme.OnSurface;
                    using var pen = new Pen(borderColor, 1);
                    g.DrawRoundedRectangle(pen, bounds, corners);
                }
            }

            private void DrawItemEffects(Graphics g, Rectangle bounds, CornerRadius corners)
            {
                using var path = GraphicsExtensions.CreateRoundedRectanglePath(bounds, corners);
                var oldClip = g.Clip;
                g.SetClip(path);

                // Efecto hover
                if (_isHovered && !_isPressed)
                {
                    var hoverColor = GetHoverColor();
                    using var brush = new SolidBrush(hoverColor);
                    g.FillRoundedRectangle(brush, bounds, corners);
                }

                // Efecto pressed
                if (_isPressed)
                {
                    var pressedColor = GetPressedColor();
                    using var brush = new SolidBrush(pressedColor);
                    g.FillRoundedRectangle(brush, bounds, corners);
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

            private void DrawItemContent(Graphics g, Rectangle bounds)
            {
                var contentBounds = new Rectangle(bounds.X + 8, bounds.Y + 4, bounds.Width - 16, bounds.Height - 8);
                var textColor = GetTextColor();

                if (_icon != null && !string.IsNullOrEmpty(_text))
                {
                    // Icono y texto
                    var iconSize = 20;
                    var spacing = 4;
                    var totalWidth = iconSize + spacing + (int)g.MeasureString(_text, Font).Width;
                    var startX = contentBounds.X + Math.Max(0, (contentBounds.Width - totalWidth) / 2);

                    var iconRect = new Rectangle(startX, contentBounds.Y + (contentBounds.Height - iconSize) / 2, iconSize, iconSize);
                    var textRect = new Rectangle(iconRect.Right + spacing, contentBounds.Y,
                        contentBounds.Right - iconRect.Right - spacing, contentBounds.Height);

                    // Dibujar icono
                    using var tintedIcon = g.ApplyTint(_icon, textColor);
                    g.DrawImage(tintedIcon, iconRect);

                    // Dibujar texto
                    using var font = new Font("Segoe UI", 9f, FontStyle.Regular);
                    using var brush = new SolidBrush(textColor);
                    using var sf = new StringFormat { LineAlignment = StringAlignment.Center };
                    g.DrawString(_text, font, brush, textRect, sf);
                }
                else if (!string.IsNullOrEmpty(_text))
                {
                    // Solo texto
                    using var font = new Font("Segoe UI", 9f, FontStyle.Regular);
                    using var brush = new SolidBrush(textColor);
                    using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(_text, font, brush, contentBounds, sf);
                }
                else if (_icon != null)
                {
                    // Solo icono
                    var iconSize = Math.Min(24, Math.Min(contentBounds.Width, contentBounds.Height));
                    var iconRect = new Rectangle(
                        contentBounds.X + (contentBounds.Width - iconSize) / 2,
                        contentBounds.Y + (contentBounds.Height - iconSize) / 2,
                        iconSize, iconSize
                    );

                    using var tintedIcon = g.ApplyTint(_icon, textColor);
                    g.DrawImage(tintedIcon, iconRect);
                }
            }

            #endregion

            #region Color Helpers

            private Color GetBackgroundColor()
            {
                if (_parent?.GroupStyle == ButtonGroupStyle.Outlined)
                    return IsSelected ? Color.FromArgb(20, ColorScheme.Primary) : Color.Transparent;

                return IsSelected ? ColorScheme.Primary : ColorScheme.Surface;
            }

            private Color GetTextColor()
            {
                if (_parent?.GroupStyle == ButtonGroupStyle.Outlined)
                    return IsSelected ? ColorScheme.Primary : ColorScheme.OnSurface;

                return IsSelected ? ColorScheme.OnPrimary : ColorScheme.OnSurface;
            }

            private Color GetHoverColor()
            {
                if (_parent?.GroupStyle == ButtonGroupStyle.Outlined)
                    return Color.FromArgb(20, ColorScheme.OnSurface);

                return IsSelected
                    ? Color.FromArgb(20, ColorScheme.OnPrimary)
                    : Color.FromArgb(20, ColorScheme.OnSurface);
            }

            private Color GetPressedColor()
            {
                if (_parent?.GroupStyle == ButtonGroupStyle.Outlined)
                    return Color.FromArgb(30, ColorScheme.OnSurface);

                return IsSelected
                    ? Color.FromArgb(30, ColorScheme.OnPrimary)
                    : Color.FromArgb(30, ColorScheme.OnSurface);
            }

            private Color GetRippleColor()
            {
                if (_parent?.GroupStyle == ButtonGroupStyle.Outlined)
                    return Color.FromArgb(40, ColorScheme.OnSurface);

                return IsSelected
                    ? Color.FromArgb(40, ColorScheme.OnPrimary)
                    : Color.FromArgb(40, ColorScheme.OnSurface);
            }

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

        #endregion

        #region Propiedades del ButtonGroup

        [Category("Material")]
        [Description("Orientación del grupo de botones")]
        [DefaultValue(GroupOrientation.Horizontal)]
        public GroupOrientation Orientation
        {
            get => _orientation;
            set { _orientation = value; ArrangeButtons(); Invalidate(); }
        }

        [Category("Material")]
        [Description("Estilo visual del grupo")]
        [DefaultValue(ButtonGroupStyle.Contained)]
        public ButtonGroupStyle GroupStyle
        {
            get => _groupStyle;
            set { _groupStyle = value; UpdateButtonStyles(); Invalidate(); }
        }

        [Category("Material")]
        [Description("Espaciado entre botones")]
        [DefaultValue(1)]
        public int Spacing
        {
            get => _spacing;
            set { _spacing = Math.Max(0, value); ArrangeButtons(); }
        }

        [Category("Material")]
        [Description("Los botones tienen el mismo tamaño")]
        [DefaultValue(true)]
        public bool EqualSizes
        {
            get => _equalSizes;
            set { _equalSizes = value; ArrangeButtons(); }
        }

        [Category("Material")]
        [Description("Botones en el grupo")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<ButtonGroupItem> Buttons
        {
            get => _buttons;
            set
            {
                _buttons = value ?? new List<ButtonGroupItem>();
                ArrangeButtons();
                Invalidate();
            }
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
        [Description("Configuración de sombra")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ShadowSettings Shadow
        {
            get => _shadow;
            set { _shadow = value ?? new ShadowSettings(); UpdateButtonStyles(); }
        }

        [Category("Material - Appearance")]
        [Description("Configuración de gradiente")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GradientSettings Gradient
        {
            get => _gradient;
            set { _gradient = value ?? new GradientSettings(); Invalidate(); }
        }

        #endregion

        public MaterialButtonGroup()
        {
            Size = new Size(300, 40);
            _shadow.Type = MaterialShadowType.Soft;
            _shadow.Opacity = 30;
            _shadow.Blur = 6;
            _shadow.OffsetY = 2;
        }

        #region Métodos públicos

        public void AddButton(string text)
        {
            var button = new ButtonGroupItem(text) { ParentGroup = this };
            _buttons.Add(button);
            Controls.Add(button);
            ArrangeButtons();
        }

        public void AddButton(string text, Image? icon)
        {
            var button = new ButtonGroupItem(text, icon) { ParentGroup = this };
            _buttons.Add(button);
            Controls.Add(button);
            ArrangeButtons();
        }

        public void RemoveButton(ButtonGroupItem button)
        {
            if (_buttons.Contains(button))
            {
                _buttons.Remove(button);
                Controls.Remove(button);
                ArrangeButtons();
            }
        }

        public void RemoveButtonAt(int index)
        {
            if (index >= 0 && index < _buttons.Count)
            {
                var button = _buttons[index];
                _buttons.RemoveAt(index);
                Controls.Remove(button);
                ArrangeButtons();
            }
        }

        public void ClearButtons()
        {
            foreach (var button in _buttons)
            {
                Controls.Remove(button);
            }
            _buttons.Clear();
            ArrangeButtons();
        }

        #endregion

        #region Layout y Styling

        private void ArrangeButtons()
        {
            if (_buttons.Count == 0) return;

            SuspendLayout();

            try
            {
                var availableSize = ClientSize;
                var totalSpacing = (_buttons.Count - 1) * _spacing;

                if (_orientation == GroupOrientation.Horizontal)
                {
                    ArrangeHorizontally(availableSize, totalSpacing);
                }
                else
                {
                    ArrangeVertically(availableSize, totalSpacing);
                }

                UpdateButtonCorners();
            }
            finally
            {
                ResumeLayout(true);
            }
        }

        private void ArrangeHorizontally(Size availableSize, int totalSpacing)
        {
            var buttonWidth = _equalSizes
                ? (availableSize.Width - totalSpacing) / _buttons.Count
                : availableSize.Width / _buttons.Count;

            var x = 0;
            foreach (var button in _buttons)
            {
                var width = _equalSizes ? buttonWidth : button.Width;
                button.SetBounds(x, 0, width, availableSize.Height);
                x += width + _spacing;
            }

            // Actualizar el tamaño del contenedor si es necesario
            if (!_equalSizes)
            {
                var totalWidth = _buttons.Sum(b => b.Width) + totalSpacing;
                Size = new Size(totalWidth, Height);
            }
        }

        private void ArrangeVertically(Size availableSize, int totalSpacing)
        {
            var buttonHeight = _equalSizes
                ? (availableSize.Height - totalSpacing) / _buttons.Count
                : availableSize.Height / _buttons.Count;

            var y = 0;
            foreach (var button in _buttons)
            {
                var height = _equalSizes ? buttonHeight : button.Height;
                button.SetBounds(0, y, availableSize.Width, height);
                y += height + _spacing;
            }

            // Actualizar el tamaño del contenedor si es necesario
            if (!_equalSizes)
            {
                var totalHeight = _buttons.Sum(b => b.Height) + totalSpacing;
                Size = new Size(Width, totalHeight);
            }
        }

        private void UpdateButtonCorners()
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                var corners = GetItemCorners(_buttons[i]);
                // Los botones individuales manejarán sus propias esquinas en OnPaint
            }
        }

        public CornerRadius GetItemCorners(ButtonGroupItem item)
        {
            var index = _buttons.IndexOf(item);
            if (index == -1 || _buttons.Count == 1)
                return _cornerRadius;

            if (_groupStyle == ButtonGroupStyle.Separated || _groupStyle == ButtonGroupStyle.Floating)
                return _cornerRadius; // Esquinas completas para botones separados

            // Para botones unidos (Contained, Outlined)
            if (_orientation == GroupOrientation.Horizontal)
            {
                if (index == 0) // Primer botón
                    return new CornerRadius(_cornerRadius.TopLeft, 0, _cornerRadius.BottomLeft, 0);
                else if (index == _buttons.Count - 1) // Último botón
                    return new CornerRadius(0, _cornerRadius.TopRight, 0, _cornerRadius.BottomRight);
                else // Botones del medio
                    return new CornerRadius(0);
            }
            else // Vertical
            {
                if (index == 0) // Primer botón
                    return new CornerRadius(_cornerRadius.TopLeft, _cornerRadius.TopRight, 0, 0);
                else if (index == _buttons.Count - 1) // Último botón
                    return new CornerRadius(0, 0, _cornerRadius.BottomLeft, _cornerRadius.BottomRight);
                else // Botones del medio
                    return new CornerRadius(0);
            }
        }

        private void UpdateButtonStyles()
        {
            foreach (var button in _buttons)
            {
                button.Invalidate();
            }
        }

        #endregion

        #region Event Handling

        protected override void OnResize(EventArgs e)
        {
            ArrangeButtons();
            base.OnResize(e);
        }

        protected override void OnColorSchemeChanged()
        {
            base.OnColorSchemeChanged();
            UpdateButtonStyles();
        }

        #endregion

        #region Painting

        protected override void OnPaint(PaintEventArgs e)
        {
            // El MaterialButtonGroup generalmente no dibuja nada propio,
            // ya que los botones individuales manejan su propia apariencia
            // Sin embargo, podríamos dibujar un fondo o contenedor si fuera necesario

            if (_groupStyle == ButtonGroupStyle.Contained && _buttons.Count > 1)
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var bounds = ClientRectangle;

                // Dibujar sombra del contenedor para botones unidos
                if (_shadow.Type != MaterialShadowType.None)
                {
                    g.DrawMaterialShadow(bounds, _cornerRadius, _shadow);
                }
            }

            base.OnPaint(e);
        }

        #endregion
    }
}
