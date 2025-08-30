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

namespace MaterialWinForms.Components.Selection
{
    /// <summary>
    /// Switch Material Design
    /// </summary>
    public class MaterialSwitch : MaterialControl
    {
        private bool _isChecked = false;
        private bool _isHovered = false;
        private bool _isPressed = false;
        private float _thumbPosition = 0f;
        private System.Windows.Forms.Timer? _animationTimer;
        private float _targetPosition = 0f;

        public event EventHandler<bool>? CheckedChanged;

        [Category("Material")]
        [Description("Estado del switch")]
        public bool Checked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    AnimateToPosition(_isChecked ? 1f : 0f);
                    CheckedChanged?.Invoke(this, _isChecked);
                }
            }
        }

        public MaterialSwitch()
        {
            Size = new Size(52, 32);
            Cursor = Cursors.Hand;

            _animationTimer = new System.Windows.Forms.Timer { Interval = 16 }; // 60 FPS
            _animationTimer.Tick += AnimationTimer_Tick;
        }

        private void AnimateToPosition(float target)
        {
            _targetPosition = target;
            _animationTimer?.Start();
        }

        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            var difference = _targetPosition - _thumbPosition;
            if (Math.Abs(difference) < 0.05f)
            {
                _thumbPosition = _targetPosition;
                _animationTimer?.Stop();
            }
            else
            {
                _thumbPosition += difference * 0.2f;
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

            var trackRect = new Rectangle(0, Height / 2 - 7, Width, 14);
            var thumbSize = _isPressed ? 24 : 20;
            var thumbY = Height / 2 - thumbSize / 2;
            var thumbX = (int)(_thumbPosition * (Width - thumbSize));
            var thumbRect = new Rectangle(thumbX, thumbY, thumbSize, thumbSize);

            // Colores según el estado
            var trackColor = _isChecked
                ? Color.FromArgb(128, ColorScheme.Primary)
                : Color.FromArgb(60, ColorScheme.OnSurface);
            var thumbColor = _isChecked
                ? ColorScheme.Primary
                : ColorScheme.Surface;

            // Dibujar track
            using (var trackBrush = new SolidBrush(trackColor))
            {
                g.FillRoundedRectangle(trackBrush, trackRect, 7);
            }

            // Dibujar halo si está hover
            if (_isHovered)
            {
                var haloSize = thumbSize + 8;
                var haloRect = new Rectangle(
                    thumbX - 4,
                    thumbY - 4,
                    haloSize,
                    haloSize
                );
                var haloColor = Color.FromArgb(20, _isChecked ? ColorScheme.Primary : ColorScheme.OnSurface);
                using (var haloBrush = new SolidBrush(haloColor))
                {
                    g.FillEllipse(haloBrush, haloRect);
                }
            }

            // Dibujar sombra del thumb
            var shadowRect = new Rectangle(thumbX + 1, thumbY + 1, thumbSize, thumbSize);
            using (var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
            {
                g.FillEllipse(shadowBrush, shadowRect);
            }

            // Dibujar thumb
            using (var thumbBrush = new SolidBrush(thumbColor))
            {
                g.FillEllipse(thumbBrush, thumbRect);
            }

            // Borde del thumb
            using (var thumbBorder = new Pen(Color.FromArgb(40, ColorScheme.OnSurface), 1))
            {
                g.DrawEllipse(thumbBorder, thumbRect);
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
