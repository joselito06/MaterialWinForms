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

namespace MaterialWinForms.Components.Inputs
{
    /// <summary>
    /// CheckBox Material Design
    /// </summary>
    public class MaterialCheckBox : MaterialControl
    {
        private bool _isChecked = false;
        private bool _isHovered = false;
        private bool _isPressed = false;
        private string _text = "CheckBox";
        private float _checkOpacity = 0f;
        private System.Windows.Forms.Timer? _animationTimer;

        public event EventHandler<bool>? CheckedChanged;

        [Category("Material")]
        [Description("Estado del checkbox")]
        public bool Checked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    AnimateCheck();
                    CheckedChanged?.Invoke(this, _isChecked);
                }
            }
        }

        [Category("Material")]
        [Description("Texto del checkbox")]
        public new string Text
        {
            get => _text;
            set { _text = value ?? ""; Invalidate(); }
        }

        public MaterialCheckBox()
        {
            Size = new Size(150, 24);
            Cursor = Cursors.Hand;

            _animationTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _animationTimer.Tick += AnimationTimer_Tick;
        }

        private void AnimateCheck()
        {
            _animationTimer?.Start();
        }

        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            var target = _isChecked ? 1f : 0f;
            var difference = target - _checkOpacity;

            if (Math.Abs(difference) < 0.05f)
            {
                _checkOpacity = target;
                _animationTimer?.Stop();
            }
            else
            {
                _checkOpacity += difference * 0.3f;
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
            Checked = !Checked;
            base.OnClick(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var checkboxSize = 18;
            var checkboxRect = new Rectangle(0, (Height - checkboxSize) / 2, checkboxSize, checkboxSize);
            var textRect = new Rectangle(checkboxSize + 8, 0, Width - checkboxSize - 8, Height);

            // Dibujar halo si está hover
            if (_isHovered)
            {
                var haloSize = checkboxSize + 8;
                var haloRect = new Rectangle(
                    checkboxRect.X - 4,
                    checkboxRect.Y - 4,
                    haloSize,
                    haloSize
                );
                var haloColor = Color.FromArgb(20, _isChecked ? ColorScheme.Primary : ColorScheme.OnSurface);
                using (var haloBrush = new SolidBrush(haloColor))
                {
                    g.FillEllipse(haloBrush, haloRect);
                }
            }

            // Dibujar checkbox
            var boxColor = _isChecked
                ? ColorScheme.Primary
                : (_isHovered ? Color.FromArgb(60, ColorScheme.OnSurface) : Color.Transparent);
            var borderColor = _isChecked
                ? ColorScheme.Primary
                : ColorScheme.OnSurface;

            using (var boxBrush = new SolidBrush(boxColor))
            {
                g.FillRoundedRectangle(boxBrush, checkboxRect, 2);
            }

            using (var borderPen = new Pen(borderColor, 2))
            {
                g.DrawRoundedRectangle(borderPen, checkboxRect, 2);
            }

            // Dibujar check mark
            if (_checkOpacity > 0)
            {
                var checkColor = Color.FromArgb(
                    (int)(255 * _checkOpacity),
                    ColorScheme.OnPrimary
                );
                using (var checkPen = new Pen(checkColor, 2))
                {
                    checkPen.StartCap = LineCap.Round;
                    checkPen.EndCap = LineCap.Round;

                    var checkX = checkboxRect.X + 3;
                    var checkY = checkboxRect.Y + checkboxRect.Height / 2;

                    g.DrawLine(checkPen,
                        checkX, checkY,
                        checkX + 4, checkY + 4);
                    g.DrawLine(checkPen,
                        checkX + 4, checkY + 4,
                        checkX + 10, checkY - 4);
                }
            }

            // Dibujar texto
            if (!string.IsNullOrEmpty(_text))
            {
                using var font = new Font("Segoe UI", 9F);
                using var textBrush = new SolidBrush(ColorScheme.OnSurface);
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(_text, font, textBrush, textRect, sf);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animationTimer?.Stop();
                _animationTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
