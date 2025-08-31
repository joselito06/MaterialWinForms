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

namespace MaterialWinForms.Components.Buttons
{
    /// <summary>
    /// Segmented Button Material Design - Control de selección múltiple o única
    /// </summary>
    public class MaterialSegmentedButton : MaterialControl
    {
        private List<SegmentItem> _segments = new List<SegmentItem>();
        private SelectionMode _selectionMode = SelectionMode.Single;
        private bool _allowDeselection = false;
        private int _hoveredIndex = -1;
        private int _pressedIndex = -1;
        private Point _rippleLocation;
        private int _rippleSize = 0;
        private int _rippleSegmentIndex = -1;
        private System.Windows.Forms.Timer? _rippleTimer;
        private CornerRadius _cornerRadius = new CornerRadius(20);
        private ShadowSettings _shadow = new ShadowSettings();

        public enum SelectionMode
        {
            Single,    // Solo una opción seleccionada
            Multiple   // Múltiples opciones seleccionadas
        }

        public class SegmentItem
        {
            public string Text { get; set; } = "";
            public Image? Icon { get; set; }
            public bool IsSelected { get; set; } = false;
            public bool Enabled { get; set; } = true;
            public object? Tag { get; set; }

            public SegmentItem(string text)
            {
                Text = text;
            }

            public SegmentItem(string text, Image? icon) : this(text)
            {
                Icon = icon;
            }

            public SegmentItem(string text, bool isSelected) : this(text)
            {
                IsSelected = isSelected;
            }

            public SegmentItem(string text, Image? icon, bool isSelected) : this(text, icon)
            {
                IsSelected = isSelected;
            }
        }

        #region Eventos

        public event EventHandler<SegmentSelectionChangedEventArgs>? SelectionChanged;

        public class SegmentSelectionChangedEventArgs : EventArgs
        {
            public int SegmentIndex { get; }
            public SegmentItem Segment { get; }
            public bool IsSelected { get; }

            public SegmentSelectionChangedEventArgs(int index, SegmentItem segment, bool isSelected)
            {
                SegmentIndex = index;
                Segment = segment;
                IsSelected = isSelected;
            }
        }

        #endregion

        #region Propiedades

        [Category("Material")]
        [Description("Modo de selección de segmentos")]
        [DefaultValue(SelectionMode.Single)]
        public SelectionMode Selection
        {
            get => _selectionMode;
            set
            {
                _selectionMode = value;
                if (value == SelectionMode.Single)
                {
                    EnsureSingleSelection();
                }
                Invalidate();
            }
        }

        [Category("Material")]
        [Description("Permite deseleccionar todos los elementos en modo Single")]
        [DefaultValue(false)]
        public bool AllowDeselection
        {
            get => _allowDeselection;
            set { _allowDeselection = value; }
        }

        [Category("Material")]
        [Description("Segmentos del control")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<SegmentItem> Segments
        {
            get => _segments;
            set
            {
                _segments = value ?? new List<SegmentItem>();
                if (_selectionMode == SelectionMode.Single)
                {
                    EnsureSingleSelection();
                }
                UpdateSize();
                Invalidate();
            }
        }

        [Category("Material - Appearance")]
        [Description("Configuración de esquinas redondeadas")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CornerRadius CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = value ?? new CornerRadius(20); Invalidate(); }
        }

        [Category("Material - Appearance")]
        [Description("Configuración de sombra")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ShadowSettings Shadow
        {
            get => _shadow;
            set { _shadow = value ?? new ShadowSettings(); Invalidate(); }
        }

        [Category("Material")]
        [Description("Índices de los segmentos seleccionados")]
        [Browsable(false)]
        public int[] SelectedIndices
        {
            get => _segments.Select((s, i) => new { Segment = s, Index = i })
                           .Where(x => x.Segment.IsSelected)
                           .Select(x => x.Index)
                           .ToArray();
        }

        [Category("Material")]
        [Description("Índice del primer segmento seleccionado")]
        [Browsable(false)]
        public int SelectedIndex
        {
            get => SelectedIndices.FirstOrDefault(-1);
            set
            {
                if (value >= 0 && value < _segments.Count)
                {
                    SetSelectedIndex(value, true);
                }
                else
                {
                    ClearSelection();
                }
            }
        }

        #endregion

        public MaterialSegmentedButton()
        {
            Size = new Size(300, 40);
            Cursor = Cursors.Hand;

            _shadow.Type = MaterialShadowType.Soft;
            _shadow.Opacity = 20;
            _shadow.Blur = 4;
            _shadow.OffsetY = 1;

            _rippleTimer = new System.Windows.Forms.Timer { Interval = 10 };
            _rippleTimer.Tick += RippleTimer_Tick;
        }

        #region Métodos públicos

        public void AddSegment(string text)
        {
            _segments.Add(new SegmentItem(text));
            UpdateSize();
            Invalidate();
        }

        public void AddSegment(string text, Image? icon)
        {
            _segments.Add(new SegmentItem(text, icon));
            UpdateSize();
            Invalidate();
        }

        public void AddSegment(string text, bool isSelected)
        {
            _segments.Add(new SegmentItem(text, isSelected));
            if (_selectionMode == SelectionMode.Single && isSelected)
            {
                EnsureSingleSelection();
            }
            UpdateSize();
            Invalidate();
        }

        public void RemoveSegment(int index)
        {
            if (index >= 0 && index < _segments.Count)
            {
                _segments.RemoveAt(index);
                UpdateSize();
                Invalidate();
            }
        }

        public void ClearSegments()
        {
            _segments.Clear();
            UpdateSize();
            Invalidate();
        }

        public void SetSelectedIndex(int index, bool selected)
        {
            if (index < 0 || index >= _segments.Count) return;

            var segment = _segments[index];
            if (segment.IsSelected == selected) return;

            if (_selectionMode == SelectionMode.Single)
            {
                if (selected)
                {
                    // Deseleccionar todos los demás
                    for (int i = 0; i < _segments.Count; i++)
                    {
                        _segments[i].IsSelected = (i == index);
                    }
                }
                else if (_allowDeselection)
                {
                    segment.IsSelected = false;
                }
            }
            else
            {
                segment.IsSelected = selected;
            }

            SelectionChanged?.Invoke(this, new SegmentSelectionChangedEventArgs(index, segment, segment.IsSelected));
            Invalidate();
        }

        public void ClearSelection()
        {
            bool changed = false;
            for (int i = 0; i < _segments.Count; i++)
            {
                if (_segments[i].IsSelected)
                {
                    _segments[i].IsSelected = false;
                    changed = true;
                }
            }
            if (changed) Invalidate();
        }

        #endregion

        private void UpdateSize()
        {
            if (_segments.Count == 0) return;

            var minSegmentWidth = 80;
            var totalWidth = Math.Max(minSegmentWidth * _segments.Count, Width);
            base.Size = new Size(totalWidth, Height);
        }

        private void EnsureSingleSelection()
        {
            var selectedIndices = _segments.Select((s, i) => new { Index = i, IsSelected = s.IsSelected })
                                          .Where(x => x.IsSelected)
                                          .ToList();

            if (selectedIndices.Count > 1)
            {
                // Mantener solo el primer seleccionado
                for (int i = 0; i < _segments.Count; i++)
                {
                    _segments[i].IsSelected = (i == selectedIndices[0].Index);
                }
            }
        }

        #region Event Handlers

        private void RippleTimer_Tick(object? sender, EventArgs e)
        {
            _rippleSize += 6;
            var segmentWidth = GetSegmentWidth();
            if (_rippleSize > segmentWidth * 1.5)
            {
                _rippleTimer?.Stop();
                _rippleSize = 0;
                _rippleSegmentIndex = -1;
            }
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            var newHoveredIndex = GetSegmentAtPoint(e.Location);
            if (newHoveredIndex != _hoveredIndex)
            {
                _hoveredIndex = newHoveredIndex;
                Cursor = (_hoveredIndex >= 0 && _segments[_hoveredIndex].Enabled) ? Cursors.Hand : Cursors.Default;
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

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _pressedIndex = GetSegmentAtPoint(e.Location);
            if (_pressedIndex >= 0 && _segments[_pressedIndex].Enabled && UseRippleEffect && e.Button == MouseButtons.Left)
            {
                _rippleLocation = e.Location;
                _rippleSize = 0;
                _rippleSegmentIndex = _pressedIndex;
                _rippleTimer?.Start();
            }
            Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _pressedIndex = -1;
            Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnClick(EventArgs e)
        {
            var mouseArgs = e as MouseEventArgs ?? new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0);
            var clickedIndex = GetSegmentAtPoint(mouseArgs.Location);

            if (clickedIndex >= 0 && _segments[clickedIndex].Enabled)
            {
                var segment = _segments[clickedIndex];
                bool newSelectedState = !segment.IsSelected;

                // En modo single, si ya está seleccionado y no se permite deselección, no hacer nada
                if (_selectionMode == SelectionMode.Single && segment.IsSelected && !_allowDeselection)
                {
                    return;
                }

                SetSelectedIndex(clickedIndex, newSelectedState);
            }

            base.OnClick(e);
        }

        protected override void OnColorSchemeChanged()
        {
            base.OnColorSchemeChanged();
            Invalidate();
        }

        #endregion

        #region Helper Methods

        private int GetSegmentAtPoint(Point point)
        {
            if (_segments.Count == 0) return -1;

            var segmentWidth = GetSegmentWidth();
            var shadowPadding = CalculateShadowPadding();
            var adjustedX = point.X - shadowPadding.Left;

            if (adjustedX < 0 || adjustedX >= Width - shadowPadding.Horizontal) return -1;

            var index = (int)(adjustedX / segmentWidth);
            return (index >= 0 && index < _segments.Count) ? index : -1;
        }

        private float GetSegmentWidth()
        {
            if (_segments.Count == 0) return 0;
            var shadowPadding = CalculateShadowPadding();
            return (float)(Width - shadowPadding.Horizontal) / _segments.Count;
        }

        private Rectangle GetSegmentBounds(int index)
        {
            var shadowPadding = CalculateShadowPadding();
            var segmentWidth = GetSegmentWidth();

            return new Rectangle(
                shadowPadding.Left + (int)(index * segmentWidth),
                shadowPadding.Top,
                (int)segmentWidth,
                Height - shadowPadding.Vertical
            );
        }

        private Padding CalculateShadowPadding()
        {
            if (_shadow.Type == MaterialShadowType.None) return Padding.Empty;

            var blur = _shadow.Blur;
            var offsetX = Math.Abs(_shadow.OffsetX);
            var offsetY = Math.Abs(_shadow.OffsetY);

            return new Padding(blur + offsetX + 1, blur + offsetY + 1, blur + offsetX + 1, blur + offsetY + 1);
        }

        #endregion

        #region Painting

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (_segments.Count == 0) return;

            var shadowPadding = CalculateShadowPadding();
            var containerBounds = new Rectangle(
                shadowPadding.Left,
                shadowPadding.Top,
                Width - shadowPadding.Horizontal,
                Height - shadowPadding.Vertical
            );

            // Dibujar sombra del contenedor
            if (_shadow.Type != MaterialShadowType.None)
            {
                g.DrawMaterialShadow(containerBounds, _cornerRadius, _shadow);
            }

            // Dibujar fondo del contenedor
            DrawContainer(g, containerBounds);

            // Dibujar segmentos
            for (int i = 0; i < _segments.Count; i++)
            {
                DrawSegment(g, i);
            }

            // Dibujar divisores
            DrawDividers(g, containerBounds);
        }

        private void DrawContainer(Graphics g, Rectangle bounds)
        {
            // Fondo del contenedor
            var containerColor = ColorScheme.Surface;
            using var containerBrush = new SolidBrush(containerColor);
            g.FillRoundedRectangle(containerBrush, bounds, _cornerRadius);

            // Borde del contenedor
            var borderColor = Color.FromArgb(40, ColorScheme.OnSurface);
            using var borderPen = new Pen(borderColor, 1);
            g.DrawRoundedRectangle(borderPen, bounds, _cornerRadius);
        }

        private void DrawSegment(Graphics g, int index)
        {
            var segment = _segments[index];
            var bounds = GetSegmentBounds(index);
            var corners = GetSegmentCorners(index);

            // Crear path para el segmento
            using var segmentPath = GraphicsExtensions.CreateRoundedRectanglePath(bounds, corners);
            var oldClip = g.Clip;
            g.SetClip(segmentPath);

            // Dibujar fondo del segmento
            if (segment.IsSelected)
            {
                var selectedColor = ColorScheme.SecondaryContainer;
                using var selectedBrush = new SolidBrush(selectedColor);
                g.FillPath(selectedBrush, segmentPath);
            }

            // Dibujar efectos de interacción
            if (index == _hoveredIndex && segment.Enabled)
            {
                var hoverColor = segment.IsSelected
                    ? Color.FromArgb(20, ColorScheme.OnSecondaryContainer)
                    : Color.FromArgb(20, ColorScheme.OnSurface);
                using var hoverBrush = new SolidBrush(hoverColor);
                g.FillPath(hoverBrush, segmentPath);
            }

            if (index == _pressedIndex && segment.Enabled)
            {
                var pressedColor = segment.IsSelected
                    ? Color.FromArgb(30, ColorScheme.OnSecondaryContainer)
                    : Color.FromArgb(30, ColorScheme.OnSurface);
                using var pressedBrush = new SolidBrush(pressedColor);
                g.FillPath(pressedBrush, segmentPath);
            }

            // Dibujar ripple
            if (UseRippleEffect && _rippleSize > 0 && _rippleSegmentIndex == index)
            {
                var rippleColor = segment.IsSelected
                    ? Color.FromArgb(40, ColorScheme.OnSecondaryContainer)
                    : Color.FromArgb(40, ColorScheme.OnSurface);
                using var rippleBrush = new SolidBrush(rippleColor);

                var rippleRect = new Rectangle(
                    _rippleLocation.X - _rippleSize / 2,
                    _rippleLocation.Y - _rippleSize / 2,
                    _rippleSize,
                    _rippleSize
                );
                g.FillEllipse(rippleBrush, rippleRect);
            }

            g.Clip = oldClip;

            // Dibujar contenido del segmento
            DrawSegmentContent(g, bounds, segment);
        }

        private void DrawSegmentContent(Graphics g, Rectangle bounds, SegmentItem segment)
        {
            var contentBounds = new Rectangle(bounds.X + 8, bounds.Y + 4, bounds.Width - 16, bounds.Height - 8);
            var textColor = GetSegmentTextColor(segment);

            if (segment.Icon != null && !string.IsNullOrEmpty(segment.Text))
            {
                // Icono y texto
                var iconSize = 20;
                var spacing = 4;
                var totalWidth = iconSize + spacing + (int)g.MeasureString(segment.Text, Font).Width;
                var startX = contentBounds.X + (contentBounds.Width - totalWidth) / 2;

                var iconRect = new Rectangle(startX, contentBounds.Y + (contentBounds.Height - iconSize) / 2, iconSize, iconSize);
                var textRect = new Rectangle(iconRect.Right + spacing, contentBounds.Y, contentBounds.Width - (iconRect.Right - contentBounds.X) - spacing, contentBounds.Height);

                // Dibujar icono
                if (segment.Enabled)
                {
                    using var tintedIcon = g.ApplyTint(segment.Icon, textColor);
                    g.DrawImage(tintedIcon, iconRect);
                }
                else
                {
                    using var disabledIcon = g.ApplyTint(segment.Icon, Color.FromArgb(128, textColor));
                    g.DrawImage(disabledIcon, iconRect);
                }

                // Dibujar texto
                using var font = new Font("Segoe UI", 9f, FontStyle.Regular);
                using var brush = new SolidBrush(textColor);
                using var sf = new StringFormat { LineAlignment = StringAlignment.Center };
                g.DrawString(segment.Text, font, brush, textRect, sf);
            }
            else if (!string.IsNullOrEmpty(segment.Text))
            {
                // Solo texto
                using var font = new Font("Segoe UI", 9f, FontStyle.Regular);
                using var brush = new SolidBrush(textColor);
                using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(segment.Text, font, brush, contentBounds, sf);
            }
            else if (segment.Icon != null)
            {
                // Solo icono
                var iconSize = Math.Min(24, Math.Min(contentBounds.Width, contentBounds.Height));
                var iconRect = new Rectangle(
                    contentBounds.X + (contentBounds.Width - iconSize) / 2,
                    contentBounds.Y + (contentBounds.Height - iconSize) / 2,
                    iconSize, iconSize
                );

                if (segment.Enabled)
                {
                    using var tintedIcon = g.ApplyTint(segment.Icon, textColor);
                    g.DrawImage(tintedIcon, iconRect);
                }
                else
                {
                    using var disabledIcon = g.ApplyTint(segment.Icon, Color.FromArgb(128, textColor));
                    g.DrawImage(disabledIcon, iconRect);
                }
            }
        }

        private void DrawDividers(Graphics g, Rectangle containerBounds)
        {
            if (_segments.Count <= 1) return;

            var dividerColor = Color.FromArgb(40, ColorScheme.OnSurface);
            using var pen = new Pen(dividerColor, 1);

            var segmentWidth = GetSegmentWidth();

            for (int i = 1; i < _segments.Count; i++)
            {
                var dividerX = containerBounds.X + (int)(i * segmentWidth);
                g.DrawLine(pen, dividerX, containerBounds.Y + 8, dividerX, containerBounds.Bottom - 8);
            }
        }

        private CornerRadius GetSegmentCorners(int index)
        {
            if (_segments.Count == 1)
                return _cornerRadius;

            if (index == 0)
                return new CornerRadius(_cornerRadius.TopLeft, 0, _cornerRadius.BottomLeft, 0);
            else if (index == _segments.Count - 1)
                return new CornerRadius(0, _cornerRadius.TopRight, 0, _cornerRadius.BottomRight);
            else
                return new CornerRadius(0);
        }

        private Color GetSegmentTextColor(SegmentItem segment)
        {
            if (!segment.Enabled)
                return Color.FromArgb(128, ColorScheme.OnSurface);

            return segment.IsSelected
                ? ColorScheme.OnSecondaryContainer
                : ColorScheme.OnSurface;
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _rippleTimer?.Stop();
                _rippleTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
