using MaterialWinForms.Core;
using MaterialWinForms.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaterialWinForms.Components.Inputs
{
    /// <summary>
    /// TextArea Material Design (multilinea)
    /// </summary>
    public class MaterialTextArea : MaterialControl
    {
        private TextBox? _textBox;
        private string _hintText = "";
        private bool _isFocused = false;
        private int _maxLength = 0;
        private bool _showCharacterCount = false;

        [Category("Material")]
        [Description("Texto de ayuda")]
        public string HintText
        {
            get => _hintText;
            set { _hintText = value ?? ""; Invalidate(); }
        }

        [Category("Material")]
        [Description("Longitud máxima del texto")]
        public int MaxLength
        {
            get => _maxLength;
            set
            {
                _maxLength = Math.Max(0, value);
                if (_textBox != null)
                    _textBox.MaxLength = _maxLength;
                Invalidate();
            }
        }

        [Category("Material")]
        [Description("Mostrar contador de caracteres")]
        public bool ShowCharacterCount
        {
            get => _showCharacterCount;
            set { _showCharacterCount = value; Invalidate(); }
        }

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

        public MaterialTextArea()
        {
            Size = new Size(300, 120);

            _textBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(12, 25),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                WordWrap = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            _textBox.GotFocus += TextBox_GotFocus;
            _textBox.LostFocus += TextBox_LostFocus;
            _textBox.TextChanged += TextBox_TextChanged;

            Controls.Add(_textBox);
            UpdateTextBoxBounds();
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

        private void UpdateTextBoxBounds()
        {
            if (_textBox == null) return;

            var padding = 12;
            var topOffset = string.IsNullOrEmpty(Text) && !_isFocused ? padding : 25;
            var bottomOffset = _showCharacterCount ? 25 : padding;

            _textBox.Location = new Point(padding, topOffset);
            _textBox.Size = new Size(
                Width - padding * 2,
                Height - topOffset - bottomOffset
            );
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateTextBoxBounds();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var bounds = new Rectangle(0, 0, Width - 1, Height - 1);
            var hasText = !string.IsNullOrEmpty(Text);

            // Dibujar fondo
            using (var backgroundBrush = new SolidBrush(ColorScheme.Surface))
            {
                g.FillRoundedRectangle(backgroundBrush, bounds, 8);
            }

            // Dibujar borde
            var borderColor = _isFocused ? ColorScheme.Primary : ColorScheme.OnSurface;
            var borderWidth = _isFocused ? 2 : 1;
            using (var borderPen = new Pen(borderColor, borderWidth))
            {
                g.DrawRoundedRectangle(borderPen, bounds, 8);
            }

            // Dibujar etiqueta flotante
            if (hasText || _isFocused)
            {
                using var labelFont = new Font("Segoe UI", 8F);
                using var labelBrush = new SolidBrush(_isFocused ? ColorScheme.Primary : ColorScheme.OnSurface);
                g.DrawString(_hintText, labelFont, labelBrush, new Point(12, 4));
            }
            else if (!_isFocused && !hasText && !string.IsNullOrEmpty(_hintText))
            {
                // Hint text cuando no hay contenido
                using var hintFont = new Font("Segoe UI", 10F);
                using var hintBrush = new SolidBrush(Color.FromArgb(128, ColorScheme.OnSurface));
                g.DrawString(_hintText, hintFont, hintBrush, new Point(12, 25));
            }

            // Contador de caracteres
            if (_showCharacterCount && _maxLength > 0)
            {
                var currentLength = Text?.Length ?? 0;
                var counterText = $"{currentLength}/{_maxLength}";
                var counterColor = currentLength > _maxLength
                    ? ColorScheme.Error
                    : Color.FromArgb(128, ColorScheme.OnSurface);

                using var counterFont = new Font("Segoe UI", 8F);
                using var counterBrush = new SolidBrush(counterColor);
                var counterRect = new Rectangle(0, Height - 18, Width - 12, 15);
                var sf = new StringFormat { Alignment = StringAlignment.Far };
                g.DrawString(counterText, counterFont, counterBrush, counterRect, sf);
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
