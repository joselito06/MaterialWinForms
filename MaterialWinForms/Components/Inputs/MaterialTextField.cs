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
    /// Campo de texto Material Design para .NET 8.0
    /// </summary>
    public class MaterialTextField : MaterialControl
    {
        private TextBox? _textBox;
        private string _hintText = "";
        private bool _isFloatingLabel = true;
        private bool _isFocused = false;

        [Category("Material")]
        [Description("Texto de ayuda que se muestra cuando el campo está vacío")]
        public string HintText
        {
            get => _hintText;
            set { _hintText = value ?? ""; Invalidate(); }
        }

        [Category("Material")]
        [Description("Si la etiqueta debe flotar cuando hay contenido")]
        public bool IsFloatingLabel
        {
            get => _isFloatingLabel;
            set { _isFloatingLabel = value; Invalidate(); }
        }

        [Category("Material")]
        [Description("Texto del campo")]
        public override string Text
        {
            get => _textBox?.Text ?? "";
            set
            {
                if (_textBox != null)
                {
                    _textBox.Text = value ?? "";
                    Invalidate();
                }
            }
        }

        public MaterialTextField()
        {
            Height = 60;

            _textBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point),
                Location = new Point(0, 25),
                Width = Width,
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };

            _textBox.GotFocus += TextBox_GotFocus;
            _textBox.LostFocus += TextBox_LostFocus;
            _textBox.TextChanged += TextBox_TextChanged;

            Controls.Add(_textBox);
        }

        private void TextBox_GotFocus(object? sender, EventArgs e)
        {
            _isFocused = true;
            Invalidate();
        }

        private void TextBox_LostFocus(object? sender, EventArgs e)
        {
            _isFocused = false;
            Invalidate();
        }

        private void TextBox_TextChanged(object? sender, EventArgs e)
        {
            Invalidate();
            OnTextChanged(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_textBox != null)
                _textBox.Width = Width - 4;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Dibujar línea inferior
            var lineY = Height - 8;
            var lineColor = _isFocused ? ColorScheme.Primary : ColorScheme.OnSurface;
            var lineWidth = _isFocused ? 2 : 1;

            using (var pen = new Pen(lineColor, lineWidth))
            {
                g.DrawLine(pen, 0, lineY, Width, lineY);
            }

            // Dibujar etiqueta flotante o hint
            if (_isFloatingLabel && (!string.IsNullOrEmpty(Text) || _isFocused))
            {
                // Etiqueta flotante
                using var font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point);
                using var brush = new SolidBrush(_isFocused ? ColorScheme.Primary : ColorScheme.OnSurface);
                g.DrawString(_hintText, font, brush, new Point(0, 2));
            }
            else if (!_isFocused && string.IsNullOrEmpty(Text) && !string.IsNullOrEmpty(_hintText))
            {
                // Hint text
                using var font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
                using var brush = new SolidBrush(Color.FromArgb(128, ColorScheme.OnSurface));
                g.DrawString(_hintText, font, brush, new Point(0, 25));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_textBox != null)
                {
                    _textBox.GotFocus -= TextBox_GotFocus;
                    _textBox.LostFocus -= TextBox_LostFocus;
                    _textBox.TextChanged -= TextBox_TextChanged;
                    _textBox.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
