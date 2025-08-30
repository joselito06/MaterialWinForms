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
    /// Slider Material Design
    /// </summary>
    public class MaterialSlider : MaterialControl
    {
        private float _value = 50f;
        private float _minimum = 0f;
        private float _maximum = 100f;
        private bool _isDragging = false;
        private bool _isHovered = false;

        public event EventHandler<float>? ValueChanged;

        [Category("Material")]
        [Description("Valor actual del slider")]
        public float Value
        {
            get => _value;
            set
            {
                var newValue = Math.Max(_minimum, Math.Min(_maximum, value));
                if (Math.Abs(_value - newValue) > 0.01f)
                {
                    _value = newValue;
                    ValueChanged?.Invoke(this, _value);
                    Invalidate();
                }
            }
        }

        [Category("Material")]
        [Description("Valor mínimo")]
        public float Minimum
        {
            get => _minimum;
            set
            {
                _minimum = value;
                if (_value < _minimum) Value = _minimum;
                Invalidate();
            }
        }

        [Category("Material")]
        [Description("Valor máximo")]
        public float Maximum
        {
            get => _maximum;
            set
            {
                _maximum = value;
                if (_value > _maximum) Value = _maximum;
                Invalidate();
            }
        }

        public MaterialSlider()
        {
            Size = new Size(200, 32);
            Cursor = Cursors.Hand;
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
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                UpdateValueFromMouse(e.X);
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_isDragging)
            {
                UpdateValueFromMouse(e.X);
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isDragging = false;
            base.OnMouseUp(e);
        }

        private void UpdateValueFromMouse(int mouseX)
        {
            var trackWidth = Width - 20; // 10px margen a cada lado
            var position = Math.Max(0, Math.Min(trackWidth, mouseX - 10));
            var percentage = position / trackWidth;
            Value = _minimum + percentage * (_maximum - _minimum);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var trackY = Height / 2 - 2;
            var trackHeight = 4;
            var trackRect = new Rectangle(10, trackY, Width - 20, trackHeight);

            // Calcular posición del thumb
            var percentage = (_value - _minimum) / (_maximum - _minimum);
            var thumbX = (int)(10 + percentage * (Width - 20));
            var thumbSize = _isDragging || _isHovered ? 16 : 12;
            var thumbRect = new Rectangle(thumbX - thumbSize / 2, Height / 2 - thumbSize / 2, thumbSize, thumbSize);

            // Dibujar track inactivo
            using (var inactiveTrackBrush = new SolidBrush(Color.FromArgb(60, ColorScheme.OnSurface)))
            {
                g.FillRoundedRectangle(inactiveTrackBrush, trackRect, 2);
            }

            // Dibujar track activo
            var activeTrackRect = new Rectangle(trackRect.X, trackRect.Y, thumbX - trackRect.X, trackRect.Height);
            using (var activeTrackBrush = new SolidBrush(ColorScheme.Primary))
            {
                g.FillRoundedRectangle(activeTrackBrush, activeTrackRect, 2);
            }

            // Dibujar halo del thumb si está hover o dragging
            if (_isHovered || _isDragging)
            {
                var haloSize = thumbSize + 16;
                var haloRect = new Rectangle(
                    thumbX - haloSize / 2,
                    Height / 2 - haloSize / 2,
                    haloSize,
                    haloSize
                );
                var haloColor = Color.FromArgb(20, ColorScheme.Primary);
                using (var haloBrush = new SolidBrush(haloColor))
                {
                    g.FillEllipse(haloBrush, haloRect);
                }
            }

            // Dibujar sombra del thumb
            var shadowRect = new Rectangle(thumbRect.X + 1, thumbRect.Y + 1, thumbRect.Width, thumbRect.Height);
            using (var shadowBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
            {
                g.FillEllipse(shadowBrush, shadowRect);
            }

            // Dibujar thumb
            using (var thumbBrush = new SolidBrush(ColorScheme.Primary))
            {
                g.FillEllipse(thumbBrush, thumbRect);
            }
        }
    }
}
