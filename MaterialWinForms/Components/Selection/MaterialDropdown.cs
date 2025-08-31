using MaterialWinForms.Components.Buttons;
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
    /// Dropdown/Select Material Design
    /// </summary>
    public class MaterialDropdown : MaterialControl
    {
        private readonly List<DropdownItem> _items = new();
        private int _selectedIndex = -1;
        private string _hintText = "Seleccionar opción";
        private bool _isOpen = false;
        private bool _isHovered = false;
        private bool _isFocused = false;
        private Form? _dropdownForm;

        public event EventHandler<int>? SelectedIndexChanged;
        public event EventHandler<DropdownItem?>? SelectedItemChanged;

        public class DropdownItem
        {
            public string Text { get; set; } = "";
            public object? Value { get; set; }
            public Image? Icon { get; set; }
            public bool Enabled { get; set; } = true;

            public override string ToString() => Text;
        }

        [Category("Material")]
        [Description("Índice del elemento seleccionado")]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value >= -1 && value < _items.Count && _selectedIndex != value)
                {
                    _selectedIndex = value;
                    SelectedIndexChanged?.Invoke(this, _selectedIndex);
                    SelectedItemChanged?.Invoke(this, SelectedItem);
                    Invalidate();
                }
            }
        }

        [Category("Material")]
        [Description("Elemento seleccionado")]
        public DropdownItem? SelectedItem =>
            _selectedIndex >= 0 && _selectedIndex < _items.Count ? _items[_selectedIndex] : null;

        [Category("Material")]
        [Description("Texto mostrado cuando no hay selección")]
        public string HintText
        {
            get => _hintText;
            set { _hintText = value ?? ""; Invalidate(); }
        }

        [Category("Material")]
        [Description("Colección de elementos del dropdown")]
        public List<DropdownItem> Items => _items;

        public MaterialDropdown()
        {
            Size = new Size(200, 56);
            Cursor = Cursors.Hand;
        }

        public void AddItem(string text, object? value = null, Image? icon = null)
        {
            _items.Add(new DropdownItem { Text = text, Value = value, Icon = icon });
            Invalidate();
        }

        public void AddItems(params string[] texts)
        {
            foreach (var text in texts)
            {
                AddItem(text, text);
            }
        }

        public void RemoveItem(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                _items.RemoveAt(index);
                if (_selectedIndex == index)
                    _selectedIndex = -1;
                else if (_selectedIndex > index)
                    _selectedIndex--;
                Invalidate();
            }
        }

        public void ClearItems()
        {
            _items.Clear();
            _selectedIndex = -1;
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
            ShowDropdownList();
            base.OnClick(e);
        }

        private void ShowDropdownList()
        {
            if (_items.Count == 0) return;

            _dropdownForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                TopMost = true,
                ShowInTaskbar = false,
                Size = new Size(Width, Math.Min(200, _items.Count * 40 + 8))
            };

            var listPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ColorScheme.Surface,
                Padding = new Padding(4)
            };

            // Agregar sombra al dropdown
            _dropdownForm.Paint += (s, e) =>
            {
                var bounds = new Rectangle(0, 0, _dropdownForm.Width, _dropdownForm.Height);
                using (var shadowBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
                {
                    e.Graphics.FillRoundedRectangle(shadowBrush, bounds, 8);
                }
                using (var backgroundBrush = new SolidBrush(ColorScheme.Surface))
                {
                    e.Graphics.FillRoundedRectangle(backgroundBrush, bounds, 8);
                }
            };

            // Crear elementos de la lista
            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                var itemIndex = i;

                var itemButton = new MaterialSimpleButton
                {
                    Text = item.Text,
                    Type = MaterialSimpleButton.ButtonType.Text,
                    Size = new Size(Width - 8, 36),
                    Location = new Point(4, 4 + i * 36),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    Enabled = item.Enabled
                };

                itemButton.Click += (s, e) =>
                {
                    SelectedIndex = itemIndex;
                    _dropdownForm?.Close();
                };

                listPanel.Controls.Add(itemButton);
            }

            _dropdownForm.Controls.Add(listPanel);

            // Posicionar dropdown
            var location = PointToScreen(new Point(0, Height));
            _dropdownForm.Location = location;

            // Cerrar cuando pierde foco
            _dropdownForm.Deactivate += (s, e) => _dropdownForm?.Close();

            _dropdownForm.ShowDialog();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var bounds = new Rectangle(0, 0, Width - 1, Height - 1);
            var hasSelection = _selectedIndex >= 0;

            // Dibujar fondo
            var backgroundColor = _isHovered
                ? Color.FromArgb(10, ColorScheme.OnSurface)
                : ColorScheme.Surface;

            using (var backgroundBrush = new SolidBrush(backgroundColor))
            {
                g.FillRoundedRectangle(backgroundBrush, bounds, 8);
            }

            // Dibujar borde
            var borderColor = _isFocused ? ColorScheme.Primary : ColorScheme.OnSurface;
            using (var borderPen = new Pen(borderColor, _isFocused ? 2 : 1))
            {
                g.DrawRoundedRectangle(borderPen, bounds, 8);
            }

            // Dibujar etiqueta flotante
            if (hasSelection || _isFocused)
            {
                using var labelFont = new Font("Segoe UI", 8F);
                using var labelBrush = new SolidBrush(_isFocused ? ColorScheme.Primary : ColorScheme.OnSurface);
                g.DrawString(_hintText, labelFont, labelBrush, new Point(12, 4));
            }

            // Dibujar texto seleccionado o hint
            var textY = hasSelection || _isFocused ? 22 : (Height - 16) / 2;
            var displayText = hasSelection ? SelectedItem?.Text ?? "" : _hintText;
            var textColor = hasSelection ? ColorScheme.OnSurface : Color.FromArgb(128, ColorScheme.OnSurface);

            using (var font = new Font("Segoe UI", 10F))
            using (var brush = new SolidBrush(textColor))
            {
                var textRect = new Rectangle(12, textY, Width - 40, 20);
                g.DrawString(displayText, font, brush, textRect);
            }

            // Dibujar flecha dropdown
            var arrowRect = new Rectangle(Width - 30, (Height - 12) / 2, 12, 12);
            using (var arrowPen = new Pen(ColorScheme.OnSurface, 2))
            {
                var centerX = arrowRect.X + arrowRect.Width / 2;
                var centerY = arrowRect.Y + arrowRect.Height / 2;

                // Flecha hacia abajo
                g.DrawLine(arrowPen,
                    centerX - 4, centerY - 2,
                    centerX, centerY + 2);
                g.DrawLine(arrowPen,
                    centerX, centerY + 2,
                    centerX + 4, centerY - 2);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dropdownForm?.Dispose();
                /*if (_textBox != null)
                {
                    _textBox.GotFocus -= TextBox_GotFocus;
                    _textBox.LostFocus -= TextBox_LostFocus;
                    _textBox.TextChanged -= TextBox_TextChanged;
                    _textBox.Dispose();
                }*/
            }
            base.Dispose(disposing);
        }
    }
}
