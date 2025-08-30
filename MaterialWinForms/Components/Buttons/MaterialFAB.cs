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

        public enum FABSize
        {
            Small,   // 40x40
            Normal,  // 56x56
            Large    // 96x96
        }

        [Category("Material")]
        [Description("Texto del FAB (solo visible si IsExtended = true)")]
        public new string Text
        {
            get => _text;
            set { _text = value ?? ""; UpdateSize(); Invalidate(); }
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
        public bool IsExtended
        {
            get => _isExtended;
            set { _isExtended = value; UpdateSize(); Invalidate(); }
        }

        [Category("Material")]
        [Description("Tamaño del FAB")]
        public FABSize Size
        {
            get => _size;
            set { _size = value; UpdateSize(); Invalidate(); }
        }

        public MaterialFAB()
        {
            Cursor = Cursors.Hand;
            Elevation = 6;
            UpdateSize();

            _rippleTimer = new System.Windows.Forms.Timer { Interval = 10 };
            _rippleTimer.Tick += RippleTimer_Tick;
        }

        private void UpdateSize()
        {
            if (_isExtended && !string.IsNullOrEmpty(_text))
            {
                // FAB extendido
                using (var g = CreateGraphics())
                using (var font = new Font("Segoe UI", 9F, FontStyle.Bold))
                {
                    var textSize = g.MeasureString(_text, font);
                    var fabHeight = GetFABHeight();
                    base.Size = new System.Drawing.Size((int)textSize.Width + fabHeight + 16, fabHeight);
                }
            }
            else
            {
                // FAB circular
                var fabSize = GetFABHeight();
                base.Size = new System.Drawing.Size(fabSize, fabSize);
            }
        }

        private int GetFABHeight() => _size switch
        {
            FABSize.Small => 40,
            FABSize.Large => 96,
            _ => 56
        };

        private void RippleTimer_Tick(object? sender, EventArgs e)
        {
            _rippleSize += 8;
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
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _rippleTimer?.Stop();
                _rippleTimer?.Dispose();
                //_dropdownForm?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
