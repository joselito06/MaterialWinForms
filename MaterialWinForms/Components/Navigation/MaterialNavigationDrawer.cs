using MaterialWinForms.Core;
using Microsoft.VisualBasic.Devices;
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
    /// Navigation Drawer Material Design - Menú lateral deslizante
    /// </summary>
    [ToolboxItem(true)]
    [DesignTimeVisible(true)]
    public partial class MaterialNavigationDrawer : MaterialControl
    {
        private NavigationDrawerStyle _style = NavigationDrawerStyle.Standard;
        private DrawerHeaderStyle _headerStyle = DrawerHeaderStyle.Standard;
        private Form? _overlayForm;
        private Panel? _headerPanel;
        private CornerRadius _cornerRadius = new CornerRadius(0);
        private ShadowSettings _shadowSettings = new ShadowSettings();

        private List<DrawerItem> _menuItems = new List<DrawerItem>();
        private List<DrawerItem> _bottomItems = new List<DrawerItem>();
        private int _selectedIndex = -1;
        private DrawerType _drawerType = DrawerType.Standard;
        private bool _isExpanded = true;
        private bool _isAnimating = false;
        private string _headerTitle = "";
        private string _headerSubtitle = "";
        private Image? _headerImage = null;
        private Color _headerBackColor = Color.Transparent;
        private GradientSettings _headerGradient = new GradientSettings();
        private int _expandedWidth = 280;
        private int _collapsedWidth = 72;
        private System.Windows.Forms.Timer? _animationTimer;
        private int _targetWidth;
        private int _currentWidth;

        public enum DrawerType
        {
            Standard,   // Siempre visible, puede colapsar
            Rail,       // Barra lateral estrecha con iconos
            Modal       // Se oculta/muestra (para pantallas pequeñas)
        }

        public class DrawerItem
        {
            public string Text { get; set; } = "";
            public string Description { get; set; } = "";
            public Image? Icon { get; set; }
            public bool IsHeader { get; set; } = false;
            public bool IsDivider { get; set; } = false;
            public bool IsEnabled { get; set; } = true;
            public bool IsSelected { get; set; } = false;
            public int BadgeCount { get; set; } = 0;
            public string BadgeText { get; set; } = "";
            public Color BadgeColor { get; set; } = Color.Red;
            public EventHandler? Click { get; set; }
            public object? Tag { get; set; }

            public DrawerItem(string text) { Text = text; }
            public DrawerItem(string text, Image? icon) { Text = text; Icon = icon; }
            public DrawerItem(string text, string description, Image? icon)
            {
                Text = text;
                Description = description;
                Icon = icon;
            }

            public static DrawerItem CreateHeader(string text) => new(text) { IsHeader = true };
            public static DrawerItem CreateDivider() => new("") { IsDivider = true };
        }

        #region Eventos

        public event EventHandler<DrawerItemSelectedEventArgs>? ItemSelected;
        public event EventHandler<bool>? ExpandedStateChanged;

        public class DrawerItemSelectedEventArgs : EventArgs
        {
            public int Index { get; }
            public DrawerItem Item { get; }

            public DrawerItemSelectedEventArgs(int index, DrawerItem item)
            {
                Index = index;
                Item = item;
            }
        }

        #endregion

        #region Propiedades

        [Category("Material")]
        [Description("Items del menú principal")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<DrawerItem> MenuItems
        {
            get => _menuItems;
            set { _menuItems = value ?? new List<DrawerItem>(); Invalidate(); }
        }

        [Category("Material")]
        [Description("Items del menú inferior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<DrawerItem> BottomItems
        {
            get => _bottomItems;
            set { _bottomItems = value ?? new List<DrawerItem>(); Invalidate(); }
        }

        [Category("Material")]
        [Description("Tipo de navigation drawer")]
        [DefaultValue(DrawerType.Standard)]
        public DrawerType Type
        {
            get => _drawerType;
            set
            {
                _drawerType = value;
                UpdateDrawerForType();
            }
        }

        [Category("Material")]
        [Description("Estado expandido/colapsado")]
        [DefaultValue(true)]
        public bool IsExpanded
        {
            get => _isExpanded;
            set { SetExpandedState(value, true); }
        }

        [Category("Material")]
        [Description("Ancho cuando está expandido")]
        [DefaultValue(280)]
        public int ExpandedWidth
        {
            get => _expandedWidth;
            set
            {
                _expandedWidth = Math.Max(200, value);
                if (_isExpanded) Width = _expandedWidth;
            }
        }

        [Category("Material")]
        [Description("Ancho cuando está colapsado")]
        [DefaultValue(72)]
        public int CollapsedWidth
        {
            get => _collapsedWidth;
            set
            {
                _collapsedWidth = Math.Max(48, value);
                if (!_isExpanded) Width = _collapsedWidth;
            }
        }

        [Category("Material - Header")]
        [Description("Título del header")]
        [DefaultValue("")]
        public string HeaderTitle
        {
            get => _headerTitle;
            set { _headerTitle = value ?? ""; Invalidate(); }
        }

        [Category("Material - Header")]
        [Description("Subtítulo del header")]
        [DefaultValue("")]
        public string HeaderSubtitle
        {
            get => _headerSubtitle;
            set { _headerSubtitle = value ?? ""; Invalidate(); }
        }

        [Category("Material - Header")]
        [Description("Imagen del header")]
        public Image? HeaderImage
        {
            get => _headerImage;
            set { _headerImage = value; Invalidate(); }
        }

        [Category("Material - Header")]
        [Description("Color de fondo del header")]
        [DefaultValue(typeof(Color), "Transparent")]
        public Color HeaderBackColor
        {
            get => _headerBackColor;
            set { _headerBackColor = value; Invalidate(); }
        }

        [Category("Material - Header")]
        [Description("Gradiente del header")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GradientSettings HeaderGradient
        {
            get => _headerGradient;
            set { _headerGradient = value ?? new GradientSettings(); Invalidate(); }
        }

        [Category("Material - Style")]
        [Description("Estilo del Navigation Drawer")]
        [DefaultValue(NavigationDrawerStyle.Standard)]
        public NavigationDrawerStyle Style
        {
            get => _style;
            set
            {
                if (_style != value)
                {
                    _style = value;
                    ApplyDrawerStyle();
                    Invalidate();
                }
            }
        }

        [Category("Material - Style")]
        [Description("Estilo del header del drawer")]
        [DefaultValue(DrawerHeaderStyle.Standard)]
        public DrawerHeaderStyle HeaderStyle
        {
            get => _headerStyle;
            set
            {
                if (_headerStyle != value)
                {
                    _headerStyle = value;
                    UpdateHeaderStyle();
                    Invalidate();
                }
            }
        }

        [Category("Material - Style")]
        [Description("Z-Index para drawer flotante")]
        [DefaultValue(1000)]
        public int ZIndex { get; set; } = 1000;

        [Category("Material - Appearance")]
        [Description("Configuración de esquinas redondeadas")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CornerRadius CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = value ?? new CornerRadius(0); Invalidate(); }
        }

        [Category("Material - Appearance")]
        [Description("Configuración de sombra")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ShadowSettings ShadowSettings
        {
            get => _shadowSettings;
            set { _shadowSettings = value ?? new ShadowSettings(); Invalidate(); }
        }

        #endregion

        private void ApplyDrawerStyle()
        {
            SuspendLayout();

            switch (_style)
            {
                case NavigationDrawerStyle.Standard:
                    ApplyStandardStyle();
                    break;
                case NavigationDrawerStyle.Modal:
                    ApplyModalStyle();
                    break;
                case NavigationDrawerStyle.OverAppBar:
                    ApplyOverAppBarStyle();
                    break;
                case NavigationDrawerStyle.Floating:
                    ApplyFloatingStyle();
                    break;
                case NavigationDrawerStyle.Mini:
                    ApplyMiniStyle();
                    break;
                case NavigationDrawerStyle.Push:
                    ApplyPushStyle();
                    break;
            }
            // Asegurar que el fondo se mantenga consistente
            BackColor = ColorScheme.Surface;

            ResumeLayout();
            Invalidate();
        }

        private void ApplyStandardStyle()
        {
            // Estilo normal - drawer junto al contenido
            Dock = DockStyle.Left;
            BringToFront();
            _shadowSettings.Type = MaterialShadowType.None;
            _cornerRadius = new CornerRadius(0);
            Invalidate();
        }

        private void ApplyModalStyle()
        {
            // Drawer modal - se superpone con overlay
            Dock = DockStyle.None;
            BringToFront();
            _shadowSettings.Type = MaterialShadowType.Medium;
            _shadowSettings.Blur = 16;
            _shadowSettings.OffsetX = 8;
            _shadowSettings.OffsetY = 2;
            _shadowSettings.Opacity = 50;
            Invalidate();
        }

        private void ApplyOverAppBarStyle()
        {
            // Drawer sobre el AppBar - se superpone a todo
            Dock = DockStyle.None;
            BringToFront();
            _shadowSettings.Type = MaterialShadowType.Medium;
            _shadowSettings.Blur = 12;
            _shadowSettings.OffsetX = 4;
            _shadowSettings.OffsetY = 0;
            _shadowSettings.Opacity = 40;

            // Configurar posición para cubrir desde arriba
            if (Parent != null)
            {
                Location = new Point(0, 0);
                Size = new Size(280, Parent.Height);
            }
            Invalidate();
        }

        private void ApplyFloatingStyle()
        {
            // Drawer flotante con esquinas redondeadas
            Dock = DockStyle.None;
            BringToFront();
            _cornerRadius = new CornerRadius(0, 16, 16, 0);
            _shadowSettings.Type = MaterialShadowType.Soft;
            _shadowSettings.Blur = 24;
            _shadowSettings.OffsetX = 12;
            _shadowSettings.OffsetY = 4;
            _shadowSettings.Opacity = 30;

            // Posicionar con margen
            if (Parent != null)
            {
                Location = new Point(8, 8);
                Size = new Size(280, Parent.Height - 16);
            }
            Invalidate();
        }

        private void ApplyMiniStyle()
        {
            // Drawer mini colapsado
            Dock = DockStyle.Left;
            Width = 64; // Ancho mini
            BringToFront();
            _shadowSettings.Type = MaterialShadowType.Soft;
            _shadowSettings.Blur = 8;
            _shadowSettings.OffsetX = 2;
            _shadowSettings.OffsetY = 1;
            _shadowSettings.Opacity = 25;
            Invalidate();
        }

        private void ApplyPushStyle()
        {
            // Drawer que empuja el contenido (similar a standard pero con animación)
            Dock = DockStyle.Left;
            BringToFront();
            _shadowSettings.Type = MaterialShadowType.Medium;
            _shadowSettings.Blur = 8;
            _shadowSettings.OffsetX = 4;
            _shadowSettings.OffsetY = 2;
            _shadowSettings.Opacity = 35;
            Invalidate();
        }

        private void UpdateHeaderStyle()
        {
            if (_headerPanel != null)
            {
                Controls.Remove(_headerPanel);
                _headerPanel.Dispose();
                _headerPanel = null;
            }

            switch (_headerStyle)
            {
                case DrawerHeaderStyle.Standard:
                    CreateStandardHeader();
                    break;
                case DrawerHeaderStyle.Compact:
                    CreateCompactHeader();
                    break;
                case DrawerHeaderStyle.Image:
                    CreateImageHeader();
                    break;
                case DrawerHeaderStyle.Gradient:
                    CreateGradientHeader();
                    break;
                case DrawerHeaderStyle.Card:
                    CreateCardHeader();
                    break;
                case DrawerHeaderStyle.None:
                    // Sin header
                    break;
            }
        }

        private void CreateStandardHeader()
        {
            _headerPanel = new Panel
            {
                Height = 160,
                Dock = DockStyle.Top,
                BackColor = ColorScheme.Primary
            };

            var titleLabel = new Label
            {
                Text = HeaderTitle,
                Font = new Font("Segoe UI", 18f, FontStyle.Bold),
                ForeColor = ColorScheme.OnPrimary,
                Location = new Point(16, 100),
                AutoSize = true
            };

            var subtitleLabel = new Label
            {
                Text = HeaderSubtitle,
                Font = new Font("Segoe UI", 12f),
                ForeColor = Color.FromArgb(200, ColorScheme.OnPrimary),
                Location = new Point(16, 130),
                AutoSize = true
            };

            _headerPanel.Controls.Add(titleLabel);
            _headerPanel.Controls.Add(subtitleLabel);
            Controls.Add(_headerPanel);
            _headerPanel.SendToBack();
        }

        private void CreateCompactHeader()
        {
            _headerPanel = new Panel
            {
                Height = 80,
                Dock = DockStyle.Top,
                BackColor = ColorScheme.Surface
            };

            var titleLabel = new Label
            {
                Text = HeaderTitle,
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = ColorScheme.OnSurface,
                Location = new Point(16, 30),
                AutoSize = true
            };

            _headerPanel.Controls.Add(titleLabel);
            Controls.Add(_headerPanel);
            _headerPanel.SendToBack();
        }

        private void CreateImageHeader()
        {
            _headerPanel = new Panel
            {
                Height = 200,
                Dock = DockStyle.Top,
                BackColor = ColorScheme.Primary
            };

            _headerPanel.Paint += (s, e) =>
            {
                if (HeaderImage != null)
                {
                    // Dibujar imagen de fondo con overlay
                    e.Graphics.DrawImage(HeaderImage, _headerPanel.ClientRectangle);

                    using var overlayBrush = new SolidBrush(Color.FromArgb(120, Color.Black));
                    e.Graphics.FillRectangle(overlayBrush, _headerPanel.ClientRectangle);
                }
            };

            var titleLabel = new Label
            {
                Text = HeaderTitle,
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(16, 140),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            var subtitleLabel = new Label
            {
                Text = HeaderSubtitle,
                Font = new Font("Segoe UI", 12f),
                ForeColor = Color.FromArgb(220, Color.White),
                Location = new Point(16, 170),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            _headerPanel.Controls.Add(titleLabel);
            _headerPanel.Controls.Add(subtitleLabel);
            Controls.Add(_headerPanel);
            _headerPanel.SendToBack();
        }

        private void CreateGradientHeader()
        {
            _headerPanel = new Panel
            {
                Height = 160,
                Dock = DockStyle.Top
            };

            _headerPanel.Paint += (s, e) =>
            {
                var bounds = _headerPanel.ClientRectangle;
                using var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    bounds,
                    ColorScheme.Primary,
                    ColorHelper.Darken(ColorScheme.Primary, 0.3f),
                    45f);
                e.Graphics.FillRectangle(brush, bounds);
            };

            var titleLabel = new Label
            {
                Text = HeaderTitle,
                Font = new Font("Segoe UI", 18f, FontStyle.Bold),
                ForeColor = ColorScheme.OnPrimary,
                Location = new Point(16, 100),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            var subtitleLabel = new Label
            {
                Text = HeaderSubtitle,
                Font = new Font("Segoe UI", 12f),
                ForeColor = Color.FromArgb(200, ColorScheme.OnPrimary),
                Location = new Point(16, 130),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            _headerPanel.Controls.Add(titleLabel);
            _headerPanel.Controls.Add(subtitleLabel);
            Controls.Add(_headerPanel);
            _headerPanel.SendToBack();
        }

        private void CreateCardHeader()
        {
            _headerPanel = new Panel
            {
                Height = 140,
                Dock = DockStyle.Top,
                BackColor = Color.Transparent,
                Padding = new Padding(12)
            };

            var cardPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ColorScheme.Surface
            };

            cardPanel.Paint += (s, e) =>
            {
                var bounds = cardPanel.ClientRectangle;
                // Dibujar sombra de tarjeta
                using var shadowBrush = new SolidBrush(Color.FromArgb(20, Color.Black));
                e.Graphics.FillRoundedRectangle(shadowBrush, new Rectangle(2, 2, bounds.Width, bounds.Height), 12);

                // Dibujar tarjeta
                using var cardBrush = new SolidBrush(ColorScheme.Surface);
                e.Graphics.FillRoundedRectangle(cardBrush, bounds, 12);
            };

            var titleLabel = new Label
            {
                Text = HeaderTitle,
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = ColorScheme.OnSurface,
                Location = new Point(16, 40),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            var subtitleLabel = new Label
            {
                Text = HeaderSubtitle,
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(150, ColorScheme.OnSurface),
                Location = new Point(16, 70),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            cardPanel.Controls.Add(titleLabel);
            cardPanel.Controls.Add(subtitleLabel);
            _headerPanel.Controls.Add(cardPanel);
            Controls.Add(_headerPanel);
            _headerPanel.SendToBack();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ApplyDrawerStyle();
            UpdateHeaderStyle();
        }


        public MaterialNavigationDrawer()
        {
            InitializeDrawer();
        }

        private void InitializeDrawer()
        {
            // Configuración del control
            Dock = DockStyle.Left;
            Width = _expandedWidth;
            _currentWidth = Width;

            // Configurar animación
            _animationTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _animationTimer.Tick += AnimationTimer_Tick;

            // Configurar sombra por defecto
            //Shadow.Type = MaterialShadowType.Medium;
            //Shadow.Opacity = 30;
            //Shadow.Blur = 8;
            //Shadow.OffsetX = 4;

            Font = new Font("Segoe UI", 9f);
        }

        #region Métodos públicos

        public void AddMenuItem(string text, Image? icon = null, EventHandler? clickHandler = null)
        {
            _menuItems.Add(new DrawerItem(text, icon) { Click = clickHandler });
            Invalidate();
        }

        public void AddMenuHeader(string text)
        {
            _menuItems.Add(DrawerItem.CreateHeader(text));
            Invalidate();
        }

        public void AddMenuDivider()
        {
            _menuItems.Add(DrawerItem.CreateDivider());
            Invalidate();
        }

        public void AddBottomItem(string text, Image? icon = null, EventHandler? clickHandler = null)
        {
            _bottomItems.Add(new DrawerItem(text, icon) { Click = clickHandler });
            Invalidate();
        }

        public void Toggle()
        {
            IsExpanded = !_isExpanded;
        }

        public void Expand()
        {
            IsExpanded = true;
        }

        public void Collapse()
        {
            IsExpanded = false;
        }

        #endregion

        #region State Management

        private void SetExpandedState(bool expanded, bool animate)
        {
            if (_drawerType == DrawerType.Modal)
            {
                // Para modal, cambiar visibilidad
                Visible = expanded;
                _isExpanded = expanded;
                ExpandedStateChanged?.Invoke(this, expanded);
                return;
            }

            _isExpanded = expanded;
            _targetWidth = expanded ? _expandedWidth : _collapsedWidth;

            if (animate && !_isAnimating)
            {
                StartAnimation();
            }
            else
            {
                Width = _targetWidth;
                _currentWidth = _targetWidth;
                Invalidate();
            }

            ExpandedStateChanged?.Invoke(this, expanded);
        }

        private void StartAnimation()
        {
            if (_isAnimating) return;

            _isAnimating = true;
            _animationTimer?.Start();
        }

        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            var step = Math.Sign(_targetWidth - _currentWidth) * Math.Max(8, Math.Abs(_targetWidth - _currentWidth) / 10);
            _currentWidth += step;

            if (Math.Sign(step) > 0 ? _currentWidth >= _targetWidth : _currentWidth <= _targetWidth)
            {
                _currentWidth = _targetWidth;
                _isAnimating = false;
                _animationTimer?.Stop();
            }

            Width = _currentWidth;
            Invalidate();
        }

        private void UpdateDrawerForType()
        {
            switch (_drawerType)
            {
                case DrawerType.Rail:
                    _isExpanded = false;
                    _collapsedWidth = 72;
                    Width = _collapsedWidth;
                    break;
                case DrawerType.Modal:
                    Dock = DockStyle.None;
                    break;
                case DrawerType.Standard:
                    Dock = DockStyle.Left;
                    break;
            }
            Invalidate();
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
            /*if (Shadow.Type != MaterialShadowType.None)
            {
                g.DrawMaterialShadow(bounds, new CornerRadius(0), Shadow);
            }*/

            // Dibujar fondo
            using var backgroundBrush = new SolidBrush(ColorScheme.Surface);
            g.FillRectangle(backgroundBrush, bounds);

            // Dibujar header
            if (_isExpanded || _drawerType == DrawerType.Rail)
            {
                DrawHeader(g);
            }

            // Dibujar items del menú
            DrawMenuItems(g);

            // Dibujar items inferiores
            DrawBottomItems(g);
        }

        private void DrawHeader(Graphics g)
        {
            if (string.IsNullOrEmpty(_headerTitle) && _headerImage == null) return;

            var headerHeight = _isExpanded ? 160 : 64;
            var headerBounds = new Rectangle(0, 0, Width, headerHeight);

            // Dibujar fondo del header
            if (_headerGradient.Type != GradientType.None)
            {
                g.FillRoundedRectangleWithGradient(headerBounds, new CornerRadius(0), _headerGradient);
            }
            else
            {
                var headerColor = _headerBackColor != Color.Transparent ? _headerBackColor : ColorScheme.Primary;
                using var headerBrush = new SolidBrush(headerColor);
                g.FillRectangle(headerBrush, headerBounds);
            }

            // Dibujar imagen de fondo del header
            if (_headerImage != null && _isExpanded)
            {
                var imageAttribs = new System.Drawing.Imaging.ImageAttributes();
                var colorMatrix = new System.Drawing.Imaging.ColorMatrix { Matrix33 = 0.7f };
                imageAttribs.SetColorMatrix(colorMatrix);
                g.DrawImage(_headerImage, headerBounds, 0, 0, _headerImage.Width, _headerImage.Height, GraphicsUnit.Pixel, imageAttribs);
            }

            if (_isExpanded)
            {
                // Dibujar texto completo del header
                var textY = headerHeight - 60;

                if (!string.IsNullOrEmpty(_headerTitle))
                {
                    using var titleFont = new Font("Segoe UI", 18f, FontStyle.Bold);
                    using var titleBrush = new SolidBrush(ColorScheme.OnPrimary);
                    var titleRect = new Rectangle(16, textY, Width - 32, 28);
                    g.DrawString(_headerTitle, titleFont, titleBrush, titleRect);
                    textY += 28;
                }

                if (!string.IsNullOrEmpty(_headerSubtitle))
                {
                    using var subtitleFont = new Font("Segoe UI", 12f);
                    using var subtitleBrush = new SolidBrush(Color.FromArgb(200, ColorScheme.OnPrimary));
                    var subtitleRect = new Rectangle(16, textY, Width - 32, 20);
                    g.DrawString(_headerSubtitle, subtitleFont, subtitleBrush, subtitleRect);
                }
            }
            else
            {
                // Dibujar solo iniciales en modo colapsado
                if (!string.IsNullOrEmpty(_headerTitle))
                {
                    var initials = GetInitials(_headerTitle);
                    using var font = new Font("Segoe UI", 16f, FontStyle.Bold);
                    using var brush = new SolidBrush(ColorScheme.OnPrimary);
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(initials, font, brush, headerBounds, sf);
                }
            }
        }

        private void DrawMenuItems(Graphics g)
        {
            var headerHeight = HasHeader() ? (_isExpanded ? 160 : 64) : 0;
            var currentY = headerHeight + 8;
            var itemHeight = 56;

            foreach (var item in _menuItems)
            {
                if (item.IsDivider)
                {
                    if (_isExpanded) DrawDivider(g, currentY);
                    currentY += _isExpanded ? 16 : 8;
                }
                else if (item.IsHeader)
                {
                    if (_isExpanded) DrawSectionHeader(g, item, currentY);
                    currentY += _isExpanded ? 32 : 16;
                }
                else
                {
                    var isSelected = _menuItems.IndexOf(item) == _selectedIndex;
                    DrawMenuItem(g, item, new Rectangle(0, currentY, Width, itemHeight), isSelected);
                    currentY += itemHeight;
                }
            }
        }

        private void DrawBottomItems(Graphics g)
        {
            if (_bottomItems.Count == 0) return;

            var itemHeight = 56;
            var startY = Height - (_bottomItems.Count * itemHeight) - 16;

            // Dibujar divisor antes de los items inferiores
            if (_isExpanded) DrawDivider(g, startY - 8);

            var currentY = startY;
            foreach (var item in _bottomItems)
            {
                DrawMenuItem(g, item, new Rectangle(0, currentY, Width, itemHeight), false);
                currentY += itemHeight;
            }
        }

        private void DrawMenuItem(Graphics g, DrawerItem item, Rectangle bounds, bool isSelected)
        {
            // Fondo para item seleccionado
            if (isSelected)
            {
                var selectedColor = Color.FromArgb(20, ColorScheme.Primary);
                using var selectedBrush = new SolidBrush(selectedColor);
                var selectedBounds = new Rectangle(bounds.X + 8, bounds.Y + 4, bounds.Width - 16, bounds.Height - 8);
                g.FillRoundedRectangle(selectedBrush, selectedBounds, 12);
            }

            var textColor = isSelected ? ColorScheme.Primary : ColorScheme.OnSurface;

            if (_isExpanded)
            {
                // Modo expandido: icono + texto
                var contentX = 16;

                // Dibujar icono
                if (item.Icon != null)
                {
                    var iconRect = new Rectangle(contentX, bounds.Y + (bounds.Height - 24) / 2, 24, 24);
                    using var tintedIcon = g.ApplyTint(item.Icon, textColor);
                    g.DrawImage(tintedIcon, iconRect);
                    contentX += 40;
                }

                // Dibujar texto
                using var font = new Font("Segoe UI", 10f, isSelected ? FontStyle.Bold : FontStyle.Regular);
                using var textBrush = new SolidBrush(textColor);

                var textRect = new Rectangle(contentX, bounds.Y, bounds.Width - contentX - 40, bounds.Height);
                var sf = new StringFormat { LineAlignment = StringAlignment.Center };

                g.DrawString(item.Text, font, textBrush, textRect, sf);

                // Dibujar badge si existe
                if ((item.BadgeCount > 0 || !string.IsNullOrEmpty(item.BadgeText)))
                {
                    DrawItemBadge(g, bounds, item);
                }
            }
            else
            {
                // Modo colapsado: solo icono centrado
                if (item.Icon != null)
                {
                    var iconSize = 24;
                    var iconRect = new Rectangle(
                        bounds.X + (bounds.Width - iconSize) / 2,
                        bounds.Y + (bounds.Height - iconSize) / 2,
                        iconSize,
                        iconSize
                    );
                    using var tintedIcon = g.ApplyTint(item.Icon, textColor);
                    g.DrawImage(tintedIcon, iconRect);

                    // Badge pequeño para modo colapsado
                    if (item.BadgeCount > 0 || !string.IsNullOrEmpty(item.BadgeText))
                    {
                        var badgeRect = new Rectangle(bounds.Right - 20, bounds.Y + 8, 12, 12);
                        using var badgeBrush = new SolidBrush(item.BadgeColor);
                        g.FillEllipse(badgeBrush, badgeRect);
                    }
                }
                else
                {
                    // Sin icono, mostrar inicial del texto
                    var initial = string.IsNullOrEmpty(item.Text) ? "?" : item.Text[0].ToString().ToUpper();
                    using var font = new Font("Segoe UI", 12f, FontStyle.Bold);
                    using var brush = new SolidBrush(textColor);
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(initial, font, brush, bounds, sf);
                }
            }
        }

        private void DrawSectionHeader(Graphics g, DrawerItem header, int y)
        {
            using var font = new Font("Segoe UI", 9f, FontStyle.Bold);
            using var brush = new SolidBrush(Color.FromArgb(150, ColorScheme.OnSurface));
            var headerRect = new Rectangle(16, y, Width - 32, 24);
            var sf = new StringFormat { LineAlignment = StringAlignment.Center };

            g.DrawString(header.Text.ToUpper(), font, brush, headerRect, sf);
        }

        private void DrawDivider(Graphics g, int y)
        {
            using var pen = new Pen(Color.FromArgb(30, ColorScheme.OnSurface), 1);
            g.DrawLine(pen, 16, y, Width - 16, y);
        }

        private void DrawItemBadge(Graphics g, Rectangle itemBounds, DrawerItem item)
        {
            var badgeText = !string.IsNullOrEmpty(item.BadgeText)
                ? item.BadgeText
                : item.BadgeCount.ToString();

            using var font = new Font("Segoe UI", 8f, FontStyle.Bold);
            var textSize = g.MeasureString(badgeText, font);

            var badgeSize = Math.Max(18, (int)Math.Max(textSize.Width + 8, textSize.Height + 4));
            var badgeRect = new Rectangle(
                itemBounds.Right - badgeSize - 16,
                itemBounds.Y + (itemBounds.Height - badgeSize) / 2,
                badgeSize,
                badgeSize
            );

            using var badgeBrush = new SolidBrush(item.BadgeColor);
            g.FillEllipse(badgeBrush, badgeRect);

            using var textBrush = new SolidBrush(Color.White);
            using var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString(badgeText, font, textBrush, badgeRect, sf);
        }

        #endregion

        #region Event Handlers

        protected override void OnMouseClick(MouseEventArgs e)
        {
            var clickedItem = GetItemAtPoint(e.Location);
            if (clickedItem != null && clickedItem.IsEnabled && !clickedItem.IsHeader && !clickedItem.IsDivider)
            {
                var index = _menuItems.IndexOf(clickedItem);
                if (index >= 0)
                {
                    _selectedIndex = index;
                    clickedItem.Click?.Invoke(this, e);
                    ItemSelected?.Invoke(this, new DrawerItemSelectedEventArgs(index, clickedItem));
                    Invalidate();
                }
            }

            base.OnMouseClick(e);
        }

        #endregion

        #region Helper Methods

        private bool HasHeader()
        {
            return !string.IsNullOrEmpty(_headerTitle) || _headerImage != null;
        }

        private string GetInitials(string text)
        {
            if (string.IsNullOrEmpty(text)) return "?";

            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 1)
                return words[0][0].ToString().ToUpper();
            else
                return (words[0][0].ToString() + words[1][0].ToString()).ToUpper();
        }

        private DrawerItem? GetItemAtPoint(Point point)
        {
            var headerHeight = HasHeader() ? (_isExpanded ? 160 : 64) : 0;
            var currentY = headerHeight + 8;
            var itemHeight = 56;

            foreach (var item in _menuItems)
            {
                if (item.IsDivider)
                {
                    currentY += _isExpanded ? 16 : 8;
                }
                else if (item.IsHeader)
                {
                    currentY += _isExpanded ? 32 : 16;
                }
                else
                {
                    if (point.Y >= currentY && point.Y <= currentY + itemHeight)
                    {
                        return item;
                    }
                    currentY += itemHeight;
                }
            }

            return null;
        }

        #endregion

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
