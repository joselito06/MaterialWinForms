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

namespace MaterialWinForms.Components.Inputs
{
    /// <summary>
    /// RadioButton Material Design con group management
    /// </summary>
    public class MaterialRadioButton : MaterialControl
    {
        private bool _isChecked = false;
        private bool _isHovered = false;
        private bool _isPressed = false;
        private string _text = "RadioButton";
        private string _groupName = "default";
        private float _checkOpacity = 0f;
        private System.Windows.Forms.Timer? _animationTimer;

        public event EventHandler<bool>? CheckedChanged;

        [Category("Material")]
        [Description("Estado del radio button")]
        public bool Checked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;

                    if (_isChecked)
                    {
                        // Desmarcar otros radio buttons del mismo grupo
                        UncheckOthersInGroup();
                    }

                    AnimateCheck();
                    CheckedChanged?.Invoke(this, _isChecked);
                }
            }
        }

        [Category("Material")]
        [Description("Texto del radio button")]
        public new string Text
        {
            get => _text;
            set { _text = value ?? ""; Invalidate(); }
        }

        [Category("Material")]
        [Description("Nombre del grupo para agrupar radio buttons")]
        public string GroupName
        {
            get => _groupName;
            set { _groupName = value ?? "default"; }
        }

        public MaterialRadioButton()
        {
            Size = new Size(150, 24);
            Cursor = Cursors.Hand;

            _animationTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _animationTimer.Tick += AnimationTimer_Tick;
        }

        private void UncheckOthersInGroup()
        {
            if (Parent == null) return;

            foreach (Control sibling in Parent.Controls)
            {
                if (sibling is MaterialRadioButton radioButton &&
                    radioButton != this &&
                    radioButton.GroupName == _groupName)
                {
                    radioButton._isChecked = false;
                    radioButton.AnimateCheck();
                    radioButton.CheckedChanged?.Invoke(radioButton, false);
                }
            }
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

        protected override void OnClick(EventArgs e)
        {
            if (!_isChecked)
                Checked = true;
            base.OnClick(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var radioSize = 18;
            var radioRect = new Rectangle(0, (Height - radioSize) / 2, radioSize, radioSize);
            var textRect = new Rectangle(radioSize + 8, 0, Width - radioSize - 8, Height);

            // Dibujar halo si está hover
            if (_isHovered)
            {
                var haloSize = radioSize + 8;
                var haloRect = new Rectangle(
                    radioRect.X - 4,
                    radioRect.Y - 4,
                    haloSize,
                    haloSize
                );
                var haloColor = Color.FromArgb(20, _isChecked ? ColorScheme.Primary : ColorScheme.OnSurface);
                using (var haloBrush = new SolidBrush(haloColor))
                {
                    g.FillEllipse(haloBrush, haloRect);
                }
            }

            // Dibujar círculo exterior
            var borderColor = _isChecked ? ColorScheme.Primary : ColorScheme.OnSurface;
            using (var borderPen = new Pen(borderColor, 2))
            {
                g.DrawEllipse(borderPen, radioRect);
            }

            // Dibujar círculo interior (checked)
            if (_checkOpacity > 0)
            {
                var innerSize = (int)(radioSize * 0.5f * _checkOpacity);
                var innerRect = new Rectangle(
                    radioRect.X + (radioSize - innerSize) / 2,
                    radioRect.Y + (radioSize - innerSize) / 2,
                    innerSize,
                    innerSize
                );

                var innerColor = Color.FromArgb(
                    (int)(255 * _checkOpacity),
                    ColorScheme.Primary
                );

                using (var innerBrush = new SolidBrush(innerColor))
                {
                    g.FillEllipse(innerBrush, innerRect);
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
