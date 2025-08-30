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

namespace MaterialWinForms.Components.Navigation
{
    /// <summary>
    /// Tabs Material Design con indicador animado
    /// </summary>
    public class MaterialTabs : MaterialControl
    {
        private readonly List<TabPage> _tabPages = new();
        private int _selectedIndex = 0;
        private int _hoveredIndex = -1;
        private float _indicatorPosition = 0f;
        private float _indicatorWidth = 0f;
        private System.Windows.Forms.Timer? _animationTimer;
        private float _targetIndicatorPosition = 0f;
        private float _targetIndicatorWidth = 0f;

        public event EventHandler<int>? SelectedIndexChanged;

        [Category("Material")]
        [Description("Índice de la pestaña seleccionada")]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value >= 0 && value < _tabPages.Count && _selectedIndex != value)
                {
                    _selectedIndex = value;
                    AnimateIndicator();
                    SelectedIndexChanged?.Invoke(this, _selectedIndex);
                    Invalidate();
                }
            }
        }

        [Category("Material")]
        [Description("Colección de páginas de pestañas")]
        public List<TabPage> TabPages => _tabPages;

        public class TabPage
        {
            public string Text { get; set; } = "";
            public Image? Icon { get; set; }
            public Control? Content { get; set; }
            public bool Enabled { get; set; } = true;
        }

        public MaterialTabs()
        {
            Size = new Size(400, 48);
            Cursor = Cursors.Hand;

            _animationTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _animationTimer.Tick += AnimationTimer_Tick;
        }

        public void AddTab(string text, Control? content = null, Image? icon = null)
        {
            _tabPages.Add(new TabPage
            {
                Text = text,
                Content = content,
                Icon = icon
            });

            if (_tabPages.Count == 1)
            {
                _selectedIndex = 0;
                AnimateIndicator();
            }

            Invalidate();
        }

        public void RemoveTab(int index)
        {
            if (index >= 0 && index < _tabPages.Count)
            {
                _tabPages.RemoveAt(index);

                if (_selectedIndex >= _tabPages.Count)
                    _selectedIndex = Math.Max(0, _tabPages.Count - 1);

                AnimateIndicator();
                Invalidate();
            }
        }

        private void AnimateIndicator()
        {
            if (_tabPages.Count == 0) return;

            var tabWidth = _tabPages.Count > 0 ? (float)Width / _tabPages.Count : 0;
            _targetIndicatorPosition = _selectedIndex * tabWidth;
            _targetIndicatorWidth = tabWidth;

            _animationTimer?.Start();
        }

        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            var positionDiff = _targetIndicatorPosition - _indicatorPosition;
            var widthDiff = _targetIndicatorWidth - _indicatorWidth;

            if (Math.Abs(positionDiff) < 1f && Math.Abs(widthDiff) < 1f)
            {
                _indicatorPosition = _targetIndicatorPosition;
                _indicatorWidth = _targetIndicatorWidth;
                _animationTimer?.Stop();
            }
            else
            {
                _indicatorPosition += positionDiff * 0.2f;
                _indicatorWidth += widthDiff * 0.2f;
            }

            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            var newHoveredIndex = GetTabIndexFromPoint(e.Location);
            if (newHoveredIndex != _hoveredIndex)
            {
                _hoveredIndex = newHoveredIndex;
                Invalidate();
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _hoveredIndex = -1;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnClick(EventArgs e)
        {
            if (e is MouseEventArgs mouseArgs)
            {
                var clickedIndex = GetTabIndexFromPoint(mouseArgs.Location);
                if (clickedIndex >= 0 && clickedIndex < _tabPages.Count && _tabPages[clickedIndex].Enabled)
                {
                    SelectedIndex = clickedIndex;
                }
            }
            base.OnClick(e);
        }

        private int GetTabIndexFromPoint(Point point)
        {
            if (_tabPages.Count == 0) return -1;

            var tabWidth = (float)Width / _tabPages.Count;
            var index = (int)(point.X / tabWidth);
            return index >= 0 && index < _tabPages.Count ? index : -1;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (_tabPages.Count == 0) return;

            var tabWidth = (float)Width / _tabPages.Count;

            // Dibujar fondo
            using (var backgroundBrush = new SolidBrush(ColorScheme.Surface))
            {
                g.FillRectangle(backgroundBrush, ClientRectangle);
            }

            // Dibujar tabs
            for (int i = 0; i < _tabPages.Count; i++)
            {
                var tabRect = new RectangleF(i * tabWidth, 0, tabWidth, Height);
                var tab = _tabPages[i];
                var isSelected = i == _selectedIndex;
                var isHovered = i == _hoveredIndex;

                // Color del texto
                var textColor = isSelected
                    ? ColorScheme.Primary
                    : (tab.Enabled ? ColorScheme.OnSurface : Color.FromArgb(100, ColorScheme.OnSurface));

                // Efecto hover
                if (isHovered && !isSelected && tab.Enabled)
                {
                    var hoverColor = Color.FromArgb(10, ColorScheme.OnSurface);
                    using (var hoverBrush = new SolidBrush(hoverColor))
                    {
                        g.FillRectangle(hoverBrush, tabRect);
                    }
                }

                // Dibujar texto del tab
                using (var font = new Font("Segoe UI", 9F, isSelected ? FontStyle.Bold : FontStyle.Regular))
                using (var textBrush = new SolidBrush(textColor))
                {
                    var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(tab.Text, font, textBrush, tabRect, sf);
                }
            }

            // Dibujar indicador animado
            if (_indicatorWidth > 0)
            {
                var indicatorRect = new RectangleF(
                    _indicatorPosition + tabWidth * 0.2f,
                    Height - 3,
                    _indicatorWidth * 0.6f,
                    3
                );

                using (var indicatorBrush = new SolidBrush(ColorScheme.Primary))
                {
                    g.FillRoundedRectangle(indicatorBrush, Rectangle.Round(indicatorRect), 2);
                }
            }

            // Línea divisoria inferior
            using (var dividerPen = new Pen(Color.FromArgb(20, ColorScheme.OnSurface), 1))
            {
                g.DrawLine(dividerPen, 0, Height - 1, Width, Height - 1);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AnimateIndicator();
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
