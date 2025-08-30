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

namespace MaterialWinForms.Components
{
    /// <summary>
    /// Chip Material Design para etiquetas y filtros
    /// </summary>
    public class MaterialChip : MaterialControl
    {
        private string _text = "Chip";
        private Image? _avatar = null;
        private bool _isSelected = false;
        private bool _isDeletable = false;
        private bool _isHovered = false;
        private bool _isDeleteHovered = false;
        private ChipType _chipType = ChipType.Action;
        private Rectangle _deleteButtonRect;

        public event EventHandler? DeleteClicked;
        public event EventHandler<bool>? SelectedChanged;

        public enum ChipType
        {
            Action,
            Filter,
            Choice,
            Input
        }

        [Category("Material")]
        [Description("Texto del chip")]
        public new string Text
        {
            get => _text;
            set { _text = value ?? ""; Invalidate(); }
        }

        [Category("Material")]
        [Description("Avatar o icono del chip")]
        public Image? Avatar
        {
            get => _avatar;
            set { _avatar = value; Invalidate(); }
        }

        [Category("Material")]
        [Description("Estado seleccionado (para chips de filtro)")]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    SelectedChanged?.Invoke(this, _isSelected);
                    Invalidate();
                }
            }
        }

        [Category("Material")]
        [Description("Permite eliminar el chip")]
        public bool IsDeletable
        {
            get => _isDeletable;
            set { _isDeletable = value; CalculateSize(); Invalidate(); }
        }

        [Category("Material")]
        [Description("Tipo de chip")]
        public ChipType Type
        {
            get => _chipType;
            set { _chipType = value; Invalidate(); }
        }

        public MaterialChip()
        {
            Cursor = Cursors.Hand;
            CalculateSize();
        }

        private void CalculateSize()
        {
            using (var g = CreateGraphics())
            using (var font = new Font("Segoe UI", 9F))
            {
                var textSize = g.MeasureString(_text, font);
                var width = (int)textSize.Width + 24; // Padding

                if (_avatar != null) width += 24; // Avatar
                if (_isDeletable) width += 20; // Delete button

                Size = new Size(width, 32);

                // Calcular posición del botón de eliminar
                _deleteButtonRect = new Rectangle(Width - 24, 4, 20, 24);
            }
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
            _isDeleteHovered = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            var wasDeleteHovered = _isDeleteHovered;
            _isDeleteHovered = _isDeletable && _deleteButtonRect.Contains(e.Location);

            if (wasDeleteHovered != _isDeleteHovered)
            {
                Cursor = _isDeleteHovered ? Cursors.Hand : Cursors.Default;
                Invalidate();
            }

            base.OnMouseMove(e);
        }

        protected override void OnClick(EventArgs e)
        {
            if (e is MouseEventArgs mouseArgs)
            {
                if (_isDeletable && _deleteButtonRect.Contains(mouseArgs.Location))
                {
                    DeleteClicked?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }

            if (_chipType == ChipType.Filter || _chipType == ChipType.Choice)
            {
                IsSelected = !IsSelected;
            }

            base.OnClick(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var bounds = new Rectangle(0, 0, Width, Height);

            // Determinar colores según estado y tipo
            var (backgroundColor, textColor, borderColor) = GetChipColors();

            // Dibujar fondo
            using (var backgroundBrush = new SolidBrush(backgroundColor))
            {
                g.FillRoundedRectangle(backgroundBrush, bounds, Height / 2);
            }

            // Dibujar borde si es necesario
            if (_chipType == ChipType.Choice && !_isSelected)
            {
                using (var borderPen = new Pen(borderColor, 1))
                {
                    g.DrawRoundedRectangle(borderPen, bounds, Height / 2);
                }
            }

            // Efecto hover
            if (_isHovered && !_isSelected)
            {
                var hoverColor = Color.FromArgb(20, ColorScheme.OnSurface);
                using (var hoverBrush = new SolidBrush(hoverColor))
                {
                    g.FillRoundedRectangle(hoverBrush, bounds, Height / 2);
                }
            }

            // Calcular posiciones del contenido
            var contentX = 12;

            // Dibujar avatar
            if (_avatar != null)
            {
                var avatarSize = Height - 8;
                var avatarRect = new Rectangle(4, 4, avatarSize, avatarSize);
                using (var path = new GraphicsPath())
                {
                    path.AddEllipse(avatarRect);
                    g.SetClip(path);
                    g.DrawImage(_avatar, avatarRect);
                    g.ResetClip();
                }
                contentX += avatarSize + 4;
            }

            // Dibujar texto
            using (var font = new Font("Segoe UI", 9F))
            using (var textBrush = new SolidBrush(textColor))
            {
                var textWidth = _isDeletable ? Width - contentX - 24 : Width - contentX - 12;
                var textRect = new Rectangle(contentX, 0, textWidth, Height);
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(_text, font, textBrush, textRect, sf);
            }

            // Dibujar botón de eliminar
            if (_isDeletable)
            {
                var deleteColor = _isDeleteHovered ? ColorScheme.Error : textColor;
                using (var deletePen = new Pen(deleteColor, 2))
                {
                    var center = new Point(
                        _deleteButtonRect.X + _deleteButtonRect.Width / 2,
                        _deleteButtonRect.Y + _deleteButtonRect.Height / 2
                    );
                    var size = 6;

                    g.DrawLine(deletePen, center.X - size, center.Y - size, center.X + size, center.Y + size);
                    g.DrawLine(deletePen, center.X - size, center.Y + size, center.X + size, center.Y - size);
                }
            }
        }

        private (Color backgroundColor, Color textColor, Color borderColor) GetChipColors()
        {
            return _chipType switch
            {
                ChipType.Filter when _isSelected => (
                    ColorScheme.Primary,
                    ColorScheme.OnPrimary,
                    Color.Transparent
                ),
                ChipType.Choice when _isSelected => (
                    ColorScheme.Primary,
                    ColorScheme.OnPrimary,
                    Color.Transparent
                ),
                ChipType.Input => (
                    ColorScheme.Surface,
                    ColorScheme.OnSurface,
                    ColorScheme.OnSurface
                ),
                _ => (
                    Color.FromArgb(40, ColorScheme.OnSurface),
                    ColorScheme.OnSurface,
                    ColorScheme.OnSurface
                )
            };
        }

        protected override void OnTextChanged(EventArgs e)
        {
            CalculateSize();
            base.OnTextChanged(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            //_isPressed = false;
            Invalidate();
            base.OnMouseUp(e);
        }

        /*protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var bounds = new Rectangle(0, 0, Width, Height);
            var elevation = _isPressed ? Elevation + 2 : (_isHovered ? Elevation + 1 : Elevation);

            // Dibujar sombra
            DrawShadow(g, bounds, elevation);

            // Color de fondo
            var backgroundColor = _isPressed
                ? ColorHelper.Darken(ColorScheme.Primary, 0.1f)
                : ColorScheme.Primary;

            // Dibujar fondo
            using (var backgroundBrush = new SolidBrush(backgroundColor))
            {
                if (_isExtended)
                {
                    g.FillRoundedRectangle(backgroundBrush, bounds, Height / 2);
                }
                else
                {
                    g.FillEllipse(backgroundBrush, bounds);
                }
            }

            // Dibujar efecto ripple
            if (UseRippleEffect && _rippleSize > 0)
            {
                var rippleColor = Color.FromArgb(60, ColorScheme.OnPrimary);
                using (var rippleBrush = new SolidBrush(rippleColor))
                {
                    var rippleRect = new Rectangle(
                        _rippleLocation.X - _rippleSize / 2,
                        _rippleLocation.Y - _rippleSize / 2,
                        _rippleSize,
                        _rippleSize
                    );
                    g.FillEllipse(rippleBrush, rippleRect);
                }
            }

            // Dibujar contenido
            if (_isExtended && !string.IsNullOrEmpty(_text))
            {
                // FAB extendido con icono y texto
                var iconSize = GetFABHeight() - 16;
                var iconX = 8;
                var textX = iconX + iconSize + 8;

                // Dibujar icono
                if (_icon != null)
                {
                    var iconRect = new Rectangle(iconX, (Height - iconSize) / 2, iconSize, iconSize);
                    g.DrawImage(_icon, iconRect);
                }

                // Dibujar texto
                using var font = new Font("Segoe UI", 9F, FontStyle.Bold);
                using var textBrush = new SolidBrush(ColorScheme.OnPrimary);
                var textRect = new Rectangle(textX, 0, Width - textX - 8, Height);
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(_text, font, textBrush, textRect, sf);
            }
            else
            {
                // FAB circular solo con icono
                if (_icon != null)
                {
                    var iconSize = GetFABHeight() / 2;
                    var iconRect = new Rectangle(
                    (Width - iconSize) / 2,
                        (Height - iconSize) / 2,
                        iconSize,
                        iconSize
                    );
                    g.DrawImage(_icon, iconRect);
                }
                else
                {
                    // Icono + por defecto
                    using (var pen = new Pen(ColorScheme.OnPrimary, 3))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;

                        var center = new Point(Width / 2, Height / 2);
                        var size = GetFABHeight() / 4;

                        g.DrawLine(pen, center.X - size, center.Y, center.X + size, center.Y);
                        g.DrawLine(pen, center.X, center.Y - size, center.X, center.Y + size);
                    }
                }
            }
        }*/

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //_rippleTimer?.Stop();
                //_rippleTimer?.Dispose();
                //_dropdownForm?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

