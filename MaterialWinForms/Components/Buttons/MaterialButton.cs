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
    /// Botón Material Design con soporte completo para .NET 8.0
    /// </summary>
    public class MaterialButton : MaterialControl
    {
        private string _text = "Button";
        private ButtonType _buttonType = ButtonType.Contained;
        private bool _isHovered = false;
        private bool _isPressed = false;
        private Point _rippleLocation;
        private int _rippleSize = 0;
        private System.Windows.Forms.Timer? _rippleTimer;

        public enum ButtonType
        {
            Contained,
            Outlined,
            Text
        }

        [Category("Material")]
        [Description("Texto que se muestra en el botón")]
        public new string Text
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
        public ButtonType Type
        {
            get => _buttonType;
            set { _buttonType = value; Invalidate(); }
        }

        [Category("Material")]
        [Description("Elevación del botón (sombra)")]
        public new int Elevation
        {
            get => base.Elevation;
            set { base.Elevation = Math.Max(0, Math.Min(24, value)); Invalidate(); }
        }

        public MaterialButton()
        {
            Size = new Size(120, 40);
            Cursor = Cursors.Hand;

            _rippleTimer = new System.Windows.Forms.Timer { Interval = 10 };
            _rippleTimer.Tick += RippleTimer_Tick;
        }

        private void RippleTimer_Tick(object? sender, EventArgs e)
        {
            _rippleSize += 5;
            if (_rippleSize > Math.Max(Width, Height))
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

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var bounds = new Rectangle(0, 0, Width - 1, Height - 1);

            // Dibujar sombra para botones contenidos
            if (_buttonType == ButtonType.Contained)
            {
                DrawShadow(g, bounds, Elevation);
            }

            // Determinar colores según el estado
            var (backgroundColor, textColor, borderColor) = GetButtonColors();

            // Dibujar fondo
            if (_buttonType != ButtonType.Text)
            {
                using var brush = new SolidBrush(backgroundColor);
                g.FillRoundedRectangle(brush, bounds, 20);
            }

            // Dibujar borde para botones outlined
            if (_buttonType == ButtonType.Outlined)
            {
                using var pen = new Pen(borderColor, 1);
                g.DrawRoundedRectangle(pen, bounds, 20);
            }

            // Dibujar efecto hover
            if (_isHovered)
            {
                var hoverColor = Color.FromArgb(20, ColorScheme.Primary);
                using var brush = new SolidBrush(hoverColor);
                g.FillRoundedRectangle(brush, bounds, 20);
            }

            // Dibujar efecto ripple
            if (UseRippleEffect && _rippleSize > 0)
            {
                var rippleColor = Color.FromArgb(40, ColorScheme.Primary);
                using var brush = new SolidBrush(rippleColor);
                var rippleRect = new Rectangle(
                    _rippleLocation.X - _rippleSize / 2,
                    _rippleLocation.Y - _rippleSize / 2,
                    _rippleSize,
                    _rippleSize
                );
                g.FillEllipse(brush, rippleRect);
            }

            // Dibujar texto
            if (!string.IsNullOrEmpty(_text))
            {
                using var font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
                using var brush = new SolidBrush(textColor);
                var textRect = bounds;
                using var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(_text, font, brush, textRect, sf);
            }
        }

        private (Color backgroundColor, Color textColor, Color borderColor) GetButtonColors()
        {
            return _buttonType switch
            {
                ButtonType.Contained => (
                    backgroundColor: _isPressed
                        ? ColorHelper.Darken(ColorScheme.Primary, 0.1f)
                        : ColorScheme.Primary,
                    textColor: ColorScheme.OnPrimary,
                    borderColor: Color.Transparent
                ),
                ButtonType.Outlined => (
                    backgroundColor: Color.Transparent,
                    textColor: ColorScheme.Primary,
                    borderColor: _isPressed
                        ? ColorHelper.Darken(ColorScheme.Primary, 0.1f)
                        : ColorScheme.Primary
                ),
                _ => ( // Text
                    backgroundColor: Color.Transparent,
                    textColor: ColorScheme.Primary,
                    borderColor: Color.Transparent
                )
            };
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
}
