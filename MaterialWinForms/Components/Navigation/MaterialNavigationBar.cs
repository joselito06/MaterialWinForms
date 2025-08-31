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
    /// Navigation Bar Material Design - Barra de navegación inferior
    /// </summary>
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(System.ComponentModel.Design.IDesigner))]
    [ToolboxItem(true)]
    [DesignTimeVisible(true)]
    public class MaterialNavigationBar : MaterialControl
    {
        private List<NavigationItem> _items = new List<NavigationItem>();
        private int _selectedIndex = 0;
        private NavigationBarType _barType = NavigationBarType.Fixed;
        private bool _showLabels = true;
        private bool _showBadges = true;
        private CornerRadius _cornerRadius = new CornerRadius(0);
        private ShadowSettings _shadow = new ShadowSettings();
        private GradientSettings _gradient = new GradientSettings();
        private Color _customBackColor = Color.Transparent;
        private Color _selectedColor = Color.Transparent;
        private Color _unselectedColor = Color.Transparent;

        public enum NavigationBarType
        {
            Fixed,     // Items con ancho fijo
            Shifting,  // Items que cambian de tamaño según selección
            Scrollable // Items scrolleables horizontalmente
        }

        public class NavigationItem
        {
            public string Text { get; set; } = "";
            public Image? Icon { get; set; }
            public Image? SelectedIcon { get; set; }
            public bool IsEnabled { get; set; } = true;
            public int BadgeCount { get; set; } = 0;
            public string BadgeText { get; set; } = "";
            public Color BadgeColor { get; set; } = Color.Red;
            public EventHandler? Click { get; set; }
            public object? Tag { get; set; }

            public NavigationItem(string text) { Text = text; }
            public NavigationItem(string text, Image icon) { Text = text; Icon = icon; }
            public NavigationItem(string text, Image icon, Image selectedIcon)
            {
                Text = text;
                Icon = icon;
                SelectedIcon = selectedIcon;
            }
        }

        #region Eventos

        public event EventHandler<NavigationItemSelectedEventArgs>? ItemSelected;

        public class NavigationItemSelectedEventArgs : EventArgs
        {
            public int Index { get; }
            public NavigationItem Item { get; }

            public NavigationItemSelectedEventArgs(int index, NavigationItem item)
            {
                Index = index;
                Item = item;
            }
        }

        #endregion

        #region Propiedades

        [Category("Material")]
        [Description("Items de navegación")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<NavigationItem> Items
        {
            get => _items;
            set
            {
                _items = value ?? new List<NavigationItem>();
                if (_selectedIndex >= _items.Count)
                    _selectedIndex = Math.Max(0, _items.Count - 1);
                Invalidate();
            }
        }

        [Category("Material")]
        [Description("Índice del item seleccionado")]
        [DefaultValue(0)]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value >= 0 && value < _items.Count && value != _selectedIndex)
                {
                    _selectedIndex = value;
                    ItemSelected?.Invoke(this, new NavigationItemSelectedEventArgs(value, _items[value]));
                    Invalidate();
                }
            }
        }

        [Category("Material")]
        [Description("Tipo de barra de navegación")]
        [DefaultValue(NavigationBarType.Fixed)]
        public NavigationBarType Type
        {
            get => _barType;
            set { _barType = value; Invalidate(); }
        }

        [Category("Material")]
        [Description("Mostrar etiquetas de texto")]
        [DefaultValue(true)]
        public bool ShowLabels
        {
            get => _showLabels;
            set { _showLabels = value; Invalidate(); }
        }

        [Category("Material")]
        [Description("Mostrar badges en los items")]
        [DefaultValue(true)]
        public bool ShowBadges
        {
            get => _showBadges;
            set { _showBadges = value; Invalidate(); }
        }

        [Category("Material")]
        [Description("Color para items seleccionados")]
        [DefaultValue(typeof(Color), "Transparent")]
        public Color SelectedColor
        {
            get => _selectedColor;
            set { _selectedColor = value; Invalidate(); }
        }

        [Category("Material")]
        [Description("Color para items no seleccionados")]
        [DefaultValue(typeof(Color), "Transparent")]
        public Color UnselectedColor
        {
            get => _unselectedColor;
            set { _unselectedColor = value; Invalidate(); }
        }

        [Category("Material")]
        [Description("Color de fondo personalizado")]
        [DefaultValue(typeof(Color), "Transparent")]
        public Color CustomBackColor
        {
            get => _customBackColor;
            set { _customBackColor = value; Invalidate(); }
        }

        [Category("Material - Appearance")]
        [Description("Configuración de esquinas redondeadas")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CornerRadius CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = value ?? new CornerRadius(0); Invalidate(); }
        }

        [Category("Material - Appearance")]
        [Description("Configuración de gradiente")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GradientSettings Gradient
        {
            get => _gradient;
            set { _gradient = value ?? new GradientSettings(); Invalidate(); }
        }

        [Category("Material - Appearance")]
        [Description("Configuración de sombra")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ShadowSettings Shadow
        {
            get => _shadow;
            set { _shadow = value ?? new ShadowSettings(); Invalidate(); }
        }

        #endregion

        public MaterialNavigationBar()
        {
            InitializeNavigationBar();
        }

        private void InitializeNavigationBar()
        {
            Dock = DockStyle.Bottom;
            Height = 72;

            // Configurar sombra por defecto (hacia arriba)
            _shadow.Type = MaterialShadowType.Soft;
            _shadow.Opacity = 30;
            _shadow.Blur = 8;
            _shadow.OffsetY = -2;

            Cursor = Cursors.Hand;
        }

        #region Métodos públicos

        public void AddItem(string text, Image? icon = null)
        {
            _items.Add(new NavigationItem(text, icon));
            Invalidate();
        }

        public void AddItem(string text, Image? icon, Image? selectedIcon)
        {
            _items.Add(new NavigationItem(text, icon, selectedIcon));
            Invalidate();
        }

        public void AddItem(NavigationItem item)
        {
            _items.Add(item);
            Invalidate();
        }

        public void RemoveItem(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                _items.RemoveAt(index);
                if (_selectedIndex >= _items.Count)
                    _selectedIndex = Math.Max(0, _items.Count - 1);
                Invalidate();
            }
        }

        public void SetBadge(int itemIndex, int count)
        {
            if (itemIndex >= 0 && itemIndex < _items.Count)
            {
                _items[itemIndex].BadgeCount = count;
                Invalidate();
            }
        }

        public void SetBadge(int itemIndex, string text)
        {
            if (itemIndex >= 0 && itemIndex < _items.Count)
            {
                _items[itemIndex].BadgeText = text;
                Invalidate();
            }
        }

        #endregion

        #region Event Handling

        protected override void OnMouseClick(MouseEventArgs e)
        {
            var clickedIndex = GetItemAtPoint(e.Location);
            if (clickedIndex >= 0 && _items[clickedIndex].IsEnabled)
            {
                SelectedIndex = clickedIndex;
                _items[clickedIndex].Click?.Invoke(this, e);
            }
            base.OnMouseClick(e);
        }

        private int GetItemAtPoint(Point point)
        {
            if (_items.Count == 0) return -1;

            var itemWidth = (float)Width / _items.Count;
            var index = (int)(point.X / itemWidth);

            return (index >= 0 && index < _items.Count) ? index : -1;
        }

        #endregion

        #region Painting

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var bounds = new Rectangle(0, 0, Width, Height);

            // Dibujar sombra
            if (_shadow.Type != MaterialShadowType.None)
            {
                GraphicsExtensions.DrawMaterialShadow(g, bounds, _cornerRadius, _shadow);
            }

            // Dibujar fondo
            DrawBackground(g, bounds);

            // Dibujar items
            if (_items.Count > 0)
            {
                DrawNavigationItems(g);
            }
        }

        private void DrawBackground(Graphics g, Rectangle bounds)
        {
            if (_gradient.Type != GradientType.None)
            {
                g.FillRoundedRectangleWithGradient(bounds, _cornerRadius, _gradient);
            }
            else
            {
                var backgroundColor = GetBackgroundColor();
                using var brush = new SolidBrush(backgroundColor);
                g.FillRoundedRectangle(brush, bounds, _cornerRadius);
            }
        }

        private void DrawNavigationItems(Graphics g)
        {
            var itemWidth = (float)Width / _items.Count;

            for (int i = 0; i < _items.Count; i++)
            {
                var item = _items[i];
                var itemBounds = new Rectangle((int)(i * itemWidth), 0, (int)itemWidth, Height);
                var isSelected = i == _selectedIndex;

                DrawNavigationItem(g, item, itemBounds, isSelected, i);
            }
        }

        private void DrawNavigationItem(Graphics g, NavigationItem item, Rectangle bounds, bool isSelected, int index)
        {
            if (!item.IsEnabled) return;

            // Colores
            var iconColor = isSelected ? GetSelectedItemColor() : GetUnselectedItemColor();
            var textColor = iconColor;

            // Indicador de selección para tipo shifting
            if (isSelected && _barType == NavigationBarType.Shifting)
            {
                var indicatorColor = Color.FromArgb(30, iconColor);
                using var indicatorBrush = new SolidBrush(indicatorColor);
                var indicatorBounds = new Rectangle(bounds.X + 8, bounds.Y + 8, bounds.Width - 16, bounds.Height - 16);
                g.FillRoundedRectangle(indicatorBrush, indicatorBounds, 16);
            }

            // Calcular posiciones
            var iconSize = 24;
            var totalHeight = iconSize + (_showLabels ? 20 : 0);
            var startY = bounds.Y + (bounds.Height - totalHeight) / 2;

            // Dibujar icono
            if (item.Icon != null)
            {
                var iconToUse = isSelected && item.SelectedIcon != null ? item.SelectedIcon : item.Icon;
                var iconRect = new Rectangle(
                    bounds.X + (bounds.Width - iconSize) / 2,
                    startY,
                    iconSize,
                    iconSize
                );

                using var tintedIcon = g.ApplyTint(iconToUse, iconColor);
                g.DrawImage(tintedIcon, iconRect);

                // Dibujar badge si existe
                if (_showBadges && (item.BadgeCount > 0 || !string.IsNullOrEmpty(item.BadgeText)))
                {
                    DrawBadge(g, iconRect, item);
                }
            }

            // Dibujar texto
            if (_showLabels && !string.IsNullOrEmpty(item.Text))
            {
                var textY = startY + iconSize + 4;
                var textRect = new Rectangle(bounds.X + 4, textY, bounds.Width - 8, 16);

                using var font = new Font("Segoe UI", 10f, isSelected ? FontStyle.Bold : FontStyle.Regular);
                using var brush = new SolidBrush(textColor);
                using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

                g.DrawString(item.Text, font, brush, textRect, sf);
            }
        }

        private void DrawBadge(Graphics g, Rectangle iconRect, NavigationItem item)
        {
            var badgeText = !string.IsNullOrEmpty(item.BadgeText) ? item.BadgeText : item.BadgeCount.ToString();
            if (string.IsNullOrEmpty(badgeText)) return;

            using var font = new Font("Segoe UI", 8f, FontStyle.Bold);
            var textSize = g.MeasureString(badgeText, font);

            var badgeSize = Math.Max(16, (int)Math.Max(textSize.Width + 6, textSize.Height + 2));
            var badgeRect = new Rectangle(
                iconRect.Right - badgeSize / 2,
                iconRect.Y - badgeSize / 2,
                badgeSize,
                badgeSize
            );

            // Fondo del badge
            using var badgeBrush = new SolidBrush(item.BadgeColor);
            g.FillEllipse(badgeBrush, badgeRect);

            // Texto del badge
            using var textBrush = new SolidBrush(Color.White);
            using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(badgeText, font, textBrush, badgeRect, sf);
        }

        #endregion

        #region Color Helpers

        private Color GetBackgroundColor()
        {
            return _customBackColor != Color.Transparent ? _customBackColor : ColorScheme.Surface;
        }

        private Color GetSelectedItemColor()
        {
            return _selectedColor != Color.Transparent ? _selectedColor : ColorScheme.Primary;
        }

        private Color GetUnselectedItemColor()
        {
            return _unselectedColor != Color.Transparent ? _unselectedColor : Color.FromArgb(128, ColorScheme.OnSurface);
        }

        #endregion
    }
}
