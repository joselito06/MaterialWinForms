using MaterialWinForms.Components.Buttons;
using MaterialWinForms.Core;
using MaterialWinForms.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaterialWinForms.Components.Containers
{
    /// <summary>
    /// App Bar Material Design - Barra de aplicación superior
    /// </summary>
    [Designer("System.Windows.Forms.Design.ParentControlDesigner, System.Design", typeof(System.ComponentModel.Design.IDesigner))]
    [ToolboxItem(true)]
    [DesignTimeVisible(true)]
    public class MaterialAppBar : MaterialControl
    {
        private AppBarStyle _style = AppBarStyle.Standard;
        private ShadowSettings _shadowSettings = new ShadowSettings();

        private string _title = "";
        private string _subtitle = "";
        private Image? _navigationIcon = null;
        private bool _showNavigationIcon = false;
        private bool _showTitle = true;
        private bool _showSubtitle = false;
        private bool _centerTitle = false;
        private AppBarType _appBarType = AppBarType.Top;
        private List<AppBarAction> _actions = new List<AppBarAction>();
        private CornerRadius _cornerRadius = new CornerRadius(0);
        private GradientSettings _gradient = new GradientSettings();
        private ShadowSettings _shadow = new ShadowSettings();
        private Color _customBackColor = Color.Transparent;
        private bool _isScrollable = false;
        private int _scrollOffset = 0;

        public enum AppBarType
        {
            Top,       // Barra superior fija
            Large,     // Barra superior grande
            Medium,    // Barra superior mediana
            Small,     // Barra superior pequeña (colapsada)
            Bottom     // Barra inferior
        }

        public class AppBarAction
        {
            public string Text { get; set; } = "";
            public Image? Icon { get; set; }
            public bool IsVisible { get; set; } = true;
            public bool IsEnabled { get; set; } = true;
            public EventHandler? Click { get; set; }
            public string? ToolTip { get; set; }
            public object? Tag { get; set; }

            public AppBarAction(string text) { Text = text; }
            public AppBarAction(Image icon) { Icon = icon; }
            public AppBarAction(string text, Image? icon) { Text = text; Icon = icon; }
            public AppBarAction(string text, EventHandler clickHandler) { Text = text; Click = clickHandler; }
            public AppBarAction(Image icon, EventHandler clickHandler) { Icon = icon; Click = clickHandler; }
        }

        #region Eventos

        public event EventHandler? NavigationIconClick;
        public event EventHandler<AppBarAction>? ActionClick;
        public event EventHandler<int>? ScrollChanged;

        #endregion

        #region Propiedades

        [Category("Material - Content")]
        [Description("Título principal del app bar")]
        [DefaultValue("")]
        public string Title
        {
            get => _title;
            set { _title = value ?? ""; Invalidate(); }
        }

        [Category("Material - Content")]
        [Description("Subtítulo del app bar")]
        [DefaultValue("")]
        public string Subtitle
        {
            get => _subtitle;
            set { _subtitle = value ?? ""; Invalidate(); }
        }

        [Category("Material - Content")]
        [Description("Icono de navegación (hamburger, back, etc.)")]
        public Image? NavigationIcon
        {
            get => _navigationIcon;
            set { _navigationIcon = value; Invalidate(); }
        }

        [Category("Material - Behavior")]
        [Description("Mostrar icono de navegación")]
        [DefaultValue(false)]
        public bool ShowNavigationIcon
        {
            get => _showNavigationIcon;
            set { _showNavigationIcon = value; Invalidate(); }
        }

        [Category("Material - Behavior")]
        [Description("Mostrar título")]
        [DefaultValue(true)]
        public bool ShowTitle
        {
            get => _showTitle;
            set { _showTitle = value; Invalidate(); }
        }

        [Category("Material - Behavior")]
        [Description("Mostrar subtítulo")]
        [DefaultValue(false)]
        public bool ShowSubtitle
        {
            get => _showSubtitle;
            set { _showSubtitle = value; Invalidate(); }
        }

        [Category("Material - Behavior")]
        [Description("Centrar el título")]
        [DefaultValue(false)]
        public bool CenterTitle
        {
            get => _centerTitle;
            set { _centerTitle = value; Invalidate(); }
        }

        [Category("Material")]
        [Description("Tipo de app bar")]
        [DefaultValue(AppBarType.Top)]
        public AppBarType Type
        {
            get => _appBarType;
            set { _appBarType = value; UpdateAppBarHeight(); Invalidate(); }
        }

        [Category("Material")]
        [Description("Acciones del app bar")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<AppBarAction> Actions
        {
            get => _actions;
            set { _actions = value ?? new List<AppBarAction>(); CreateActionButtons(); }
        }

        [Category("Material")]
        [Description("El app bar puede hacer scroll")]
        [DefaultValue(false)]
        public bool IsScrollable
        {
            get => _isScrollable;
            set { _isScrollable = value; }
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

        [Category("Material - Style")]
        [Description("Estilo del AppBar")]
        [DefaultValue(AppBarStyle.Standard)]
        public AppBarStyle Style
        {
            get => _style;
            set
            {
                if (_style != value)
                {
                    _style = value;
                    ApplyAppBarStyle();
                    Invalidate();
                }
            }
        }

        [Category("Material - Appearance")]
        [Description("Configuración de esquinas redondeadas")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CornerRadius MaterialCornerRadius
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

        private void ApplyAppBarStyle()
        {
            switch (_style)
            {
                case AppBarStyle.Standard:
                    ApplyStandardAppBarStyle();
                    break;
                case AppBarStyle.Floating:
                    ApplyFloatingAppBarStyle();
                    break;
                case AppBarStyle.Transparent:
                    ApplyTransparentAppBarStyle();
                    break;
                case AppBarStyle.Elevated:
                    ApplyElevatedAppBarStyle();
                    break;
                case AppBarStyle.Collapsing:
                    ApplyCollapsingAppBarStyle();
                    break;
                case AppBarStyle.Dense:
                    ApplyDenseAppBarStyle();
                    break;
            }
        }

        private void ApplyStandardAppBarStyle()
        {
            Dock = DockStyle.Top;
            Height = 64;
            _shadowSettings.Type = MaterialShadowType.Soft;
            _shadowSettings.Blur = 4;
            _shadowSettings.OffsetY = 2;
            _shadowSettings.OffsetX = 0;
            _shadowSettings.Opacity = 30;
            _cornerRadius = new CornerRadius(0);
            Invalidate();
        }

        private void ApplyFloatingAppBarStyle()
        {
            Dock = DockStyle.None;
            Height = 64;
            _shadowSettings.Type = MaterialShadowType.Medium;
            _shadowSettings.Blur = 12;
            _shadowSettings.OffsetY = 4;
            _shadowSettings.OffsetX = 0;
            _shadowSettings.Opacity = 40;
            _cornerRadius = new CornerRadius(16);

            // Posicionar flotante
            if (Parent != null)
            {
                Location = new Point(16, 16);
                Size = new Size(Parent.Width - 32, Height);
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            }
            Invalidate();
        }

        private void ApplyTransparentAppBarStyle()
        {
            Dock = DockStyle.Top;
            Height = 64;
            BackColor = Color.Transparent;
            _shadowSettings.Type = MaterialShadowType.None;
            _cornerRadius = new CornerRadius(0);
            Invalidate();
        }

        private void ApplyElevatedAppBarStyle()
        {
            Dock = DockStyle.Top;
            Height = 64;
            _shadowSettings.Type = MaterialShadowType.Hard;
            _shadowSettings.Blur = 16;
            _shadowSettings.OffsetY = 8;
            _shadowSettings.OffsetX = 0;
            _shadowSettings.Opacity = 60;
            _cornerRadius = new CornerRadius(0);
            Invalidate();
        }

        private void ApplyCollapsingAppBarStyle()
        {
            Dock = DockStyle.Top;
            Height = 64; // Altura mínima
            _shadowSettings.Type = MaterialShadowType.Soft;
            _shadowSettings.Blur = 4;
            _shadowSettings.OffsetY = 2;
            _shadowSettings.OffsetX = 0;
            _shadowSettings.Opacity = 30;
            _cornerRadius = new CornerRadius(0);
            Invalidate();
        }

        private void ApplyDenseAppBarStyle()
        {
            Dock = DockStyle.Top;
            Height = 48;
            _shadowSettings.Type = MaterialShadowType.Soft;
            _shadowSettings.Blur = 2;
            _shadowSettings.OffsetY = 1;
            _shadowSettings.OffsetX = 0;
            _shadowSettings.Opacity = 25;
            _cornerRadius = new CornerRadius(0);
            Invalidate();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ApplyAppBarStyle();
        }

        public MaterialAppBar()
        {
            InitializeAppBar();
        }

        private void InitializeAppBar()
        {
            // Configuración como contenedor
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.ContainerControl, true);

            Dock = DockStyle.Top;
            UpdateAppBarHeight();

            // Configurar sombra por defecto
            _shadow.Type = MaterialShadowType.Soft;
            _shadow.Opacity = 30;
            _shadow.Blur = 4;
            _shadow.OffsetY = 2;

            Font = new Font("Segoe UI", 16f, FontStyle.Bold);
        }

        private void UpdateAppBarHeight()
        {
            Height = _appBarType switch
            {
                AppBarType.Large => 152,
                AppBarType.Medium => 112,
                AppBarType.Small => 48,
                AppBarType.Bottom => 56,
                _ => 64 // Top
            };
        }

        #region Métodos públicos

        public void AddAction(string text, EventHandler? clickHandler = null)
        {
            var action = new AppBarAction(text, clickHandler);
            _actions.Add(action);
            CreateActionButtons();
        }

        public void AddAction(Image icon, EventHandler? clickHandler = null)
        {
            var action = new AppBarAction(icon, clickHandler);
            _actions.Add(action);
            CreateActionButtons();
        }

        public void AddAction(string text, Image? icon, EventHandler? clickHandler = null)
        {
            var action = new AppBarAction(text, icon) { Click = clickHandler };
            _actions.Add(action);
            CreateActionButtons();
        }

        public void RemoveAction(AppBarAction action)
        {
            _actions.Remove(action);
            CreateActionButtons();
        }

        public void ClearActions()
        {
            _actions.Clear();
            CreateActionButtons();
        }

        public void SetScrollOffset(int offset)
        {
            if (!_isScrollable) return;

            _scrollOffset = Math.Max(0, offset);

            // Cambiar tipo de app bar según scroll
            if (_scrollOffset > 100 && _appBarType == AppBarType.Large)
            {
                _appBarType = AppBarType.Medium;
                UpdateAppBarHeight();
            }
            else if (_scrollOffset > 200 && _appBarType == AppBarType.Medium)
            {
                _appBarType = AppBarType.Small;
                UpdateAppBarHeight();
            }
            else if (_scrollOffset == 0)
            {
                _appBarType = AppBarType.Large;
                UpdateAppBarHeight();
            }

            ScrollChanged?.Invoke(this, _scrollOffset);
            Invalidate();
        }

        #endregion

        private void CreateActionButtons()
        {
            // Remover botones existentes
            var buttonsToRemove = new List<Control>();
            foreach (Control control in Controls)
            {
                if (control.Tag?.ToString() == "AppBarAction")
                {
                    buttonsToRemove.Add(control);
                }
            }

            foreach (var button in buttonsToRemove)
            {
                Controls.Remove(button);
            }

            // Crear nuevos botones
            var rightX = Width - 16;

            for (int i = _actions.Count - 1; i >= 0; i--)
            {
                var action = _actions[i];
                if (!action.IsVisible) continue;

                MaterialIconButton actionButton;

                if (action.Icon != null)
                {
                    // Botón con icono
                    actionButton = new MaterialIconButton
                    {
                        Style = MaterialIconButton.IconButtonStyle.Standard,
                        Size = MaterialIconButton.IconButtonSize.Medium,
                        Tag = "AppBarAction"
                    };
                    actionButton.IconSettings.Icon = action.Icon;
                }
                else
                {
                    // Botón de texto (menos común en app bars)
                    actionButton = new MaterialIconButton
                    {
                        Style = MaterialIconButton.IconButtonStyle.Standard,
                        Size = MaterialIconButton.IconButtonSize.Medium,
                        Tag = "AppBarAction"
                    };
                    // Para texto, podrías crear un icono de texto o usar un MaterialButton
                }

                // Configurar colores para que funcionen sobre el app bar
                actionButton.IconSettings.TintColor = GetActionIconColor();
                actionButton.Location = new Point(rightX - 40, (Height - 40) / 2);
                actionButton.Click += (s, e) =>
                {
                    action.Click?.Invoke(this, e);
                    ActionClick?.Invoke(this, action);
                };

                Controls.Add(actionButton);
                rightX -= 48; // Espaciado entre acciones
            }
        }

        protected override void OnColorSchemeChanged()
        {
            base.OnColorSchemeChanged();
            UpdateGradientDefaults();
            CreateActionButtons(); // Recrear botones con nuevos colores
            Invalidate();
        }

        private void UpdateGradientDefaults()
        {
            if (_gradient.Type != GradientType.None && _gradient.StartColor == Color.Blue && _gradient.EndColor == Color.LightBlue)
            {
                _gradient.StartColor = ColorScheme.Primary;
                _gradient.EndColor = ColorHelper.Darken(ColorScheme.Primary, 0.1f);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            CreateActionButtons();
            base.OnResize(e);
        }

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
                g.DrawMaterialShadow(bounds, _cornerRadius, _shadow);
            }

            // Dibujar fondo
            DrawBackground(g, bounds);

            // Dibujar contenido
            DrawContent(g, bounds);
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

        private void DrawContent(Graphics g, Rectangle bounds)
        {
            var contentX = 16;
            var contentWidth = Width - 32;

            // Dibujar icono de navegación
            if (_showNavigationIcon)
            {
                DrawNavigationIcon(g, new Point(contentX, (Height - 24) / 2));
                contentX += 40;
                contentWidth -= 40;
            }

            // Ajustar área para acciones
            var actionsWidth = _actions.Count * 48;
            contentWidth -= actionsWidth;

            // Dibujar título y subtítulo
            if (_showTitle || _showSubtitle)
            {
                DrawTitleContent(g, new Rectangle(contentX, 0, contentWidth, Height));
            }
        }

        private void DrawNavigationIcon(Graphics g, Point location)
        {
            var iconRect = new Rectangle(location.X, location.Y, 24, 24);

            if (_navigationIcon != null)
            {
                var iconColor = GetNavigationIconColor();
                using var tintedIcon = g.ApplyTint(_navigationIcon, iconColor);
                g.DrawImage(tintedIcon, iconRect);
            }
            else
            {
                // Dibujar icono hamburger por defecto
                DrawHamburgerIcon(g, iconRect);
            }
        }

        private void DrawHamburgerIcon(Graphics g, Rectangle bounds)
        {
            var iconColor = GetNavigationIconColor();
            using var pen = new Pen(iconColor, 2f) { StartCap = LineCap.Round, EndCap = LineCap.Round };

            var centerY = bounds.Y + bounds.Height / 2;
            var lineWidth = bounds.Width - 4;
            var startX = bounds.X + 2;

            g.DrawLine(pen, startX, centerY - 6, startX + lineWidth, centerY - 6);
            g.DrawLine(pen, startX, centerY, startX + lineWidth, centerY);
            g.DrawLine(pen, startX, centerY + 6, startX + lineWidth, centerY + 6);
        }

        private void DrawTitleContent(Graphics g, Rectangle bounds)
        {
            var currentY = bounds.Y;
            var titleHeight = Height;

            // Calcular posiciones según el tipo de app bar
            if (_appBarType == AppBarType.Large || _appBarType == AppBarType.Medium)
            {
                // Títulos más grandes para app bars grandes
                currentY = Height - 80;
                titleHeight = 80;
            }
            else
            {
                currentY = bounds.Y + (bounds.Height - 40) / 2;
                titleHeight = 40;
            }

            // Dibujar título
            if (_showTitle && !string.IsNullOrEmpty(_title))
            {
                var titleFont = GetTitleFont();
                var titleColor = GetTitleColor();

                using var font = titleFont;
                using var brush = new SolidBrush(titleColor);

                var titleRect = new Rectangle(bounds.X, currentY, bounds.Width, titleHeight / 2);
                var sf = new StringFormat
                {
                    Alignment = _centerTitle ? StringAlignment.Center : StringAlignment.Near,
                    LineAlignment = StringAlignment.Center
                };

                g.DrawString(_title, font, brush, titleRect, sf);

                if (_showSubtitle)
                {
                    currentY += titleHeight / 2;
                }
            }

            // Dibujar subtítulo
            if (_showSubtitle && !string.IsNullOrEmpty(_subtitle))
            {
                var subtitleFont = GetSubtitleFont();
                var subtitleColor = GetSubtitleColor();

                using var font = subtitleFont;
                using var brush = new SolidBrush(subtitleColor);

                var subtitleRect = new Rectangle(bounds.X, currentY, bounds.Width, titleHeight / 2);
                var sf = new StringFormat
                {
                    Alignment = _centerTitle ? StringAlignment.Center : StringAlignment.Near,
                    LineAlignment = StringAlignment.Center
                };

                g.DrawString(_subtitle, font, brush, subtitleRect, sf);
            }
        }

        private void DrawTitle(Graphics g, Rectangle bounds)
        {
            if (string.IsNullOrEmpty(Title)) return;

            using var font = new Font("Segoe UI", 18f, FontStyle.Regular);
            using var brush = new SolidBrush(ColorScheme.OnPrimary);

            var titleFormat = new StringFormat();
            if (CenterTitle)
            {
                titleFormat.Alignment = StringAlignment.Center;
                titleFormat.LineAlignment = StringAlignment.Center;

                // Área centrada (excluyendo iconos de navegación y acciones)
                var centerArea = new Rectangle(
                    bounds.X + (ShowNavigationIcon ? 56 : 16),
                    bounds.Y,
                    bounds.Width - (ShowNavigationIcon ? 56 : 16) - 120, // 120 para acciones
                    bounds.Height
                );

                g.DrawString(Title, font, brush, centerArea, titleFormat);
            }
            else
            {
                // Título a la izquierda (comportamiento actual)
                titleFormat.LineAlignment = StringAlignment.Center;
                var titleArea = new Rectangle(
                    bounds.X + (ShowNavigationIcon ? 56 : 16),
                    bounds.Y,
                    bounds.Width - 200,
                    bounds.Height
                );
                g.DrawString(Title, font, brush, titleArea, titleFormat);
            }
        }

        #endregion

        #region Event Handlers

        protected override void OnMouseClick(MouseEventArgs e)
        {
            // Verificar click en icono de navegación
            if (_showNavigationIcon && e.X >= 16 && e.X <= 56 && e.Y >= (Height - 24) / 2 && e.Y <= (Height + 24) / 2)
            {
                NavigationIconClick?.Invoke(this, EventArgs.Empty);
                return;
            }

            base.OnMouseClick(e);
        }

        /*protected override void OnResize(EventArgs e)
        {
            CreateActionButtons();
            base.OnResize(e);
        }*/

        #endregion

        #region Helpers

        private Font GetTitleFont()
        {
            return _appBarType switch
            {
                AppBarType.Large => new Font("Segoe UI", 24f, FontStyle.Bold),
                AppBarType.Medium => new Font("Segoe UI", 20f, FontStyle.Bold),
                AppBarType.Small => new Font("Segoe UI", 14f, FontStyle.Bold),
                _ => new Font("Segoe UI", 16f, FontStyle.Bold)
            };
        }

        private Font GetSubtitleFont()
        {
            return _appBarType switch
            {
                AppBarType.Large => new Font("Segoe UI", 14f),
                AppBarType.Medium => new Font("Segoe UI", 12f),
                _ => new Font("Segoe UI", 10f)
            };
        }

        private Color GetBackgroundColor()
        {
            return _customBackColor != Color.Transparent ? _customBackColor : ColorScheme.Primary;
        }

        private Color GetTitleColor()
        {
            return ColorScheme.OnPrimary;
        }

        private Color GetSubtitleColor()
        {
            return Color.FromArgb(200, ColorScheme.OnPrimary);
        }

        private Color GetNavigationIconColor()
        {
            return ColorScheme.OnPrimary;
        }

        private Color GetActionIconColor()
        {
            return ColorScheme.OnPrimary;
        }

        #endregion

        #region Métodos estáticos

        public static MaterialAppBar CreateForForm(Form form, string title = "")
        {
            var appBar = new MaterialAppBar
            {
                Title = title,
                Dock = DockStyle.Top
            };

            form.Controls.Add(appBar);
            appBar.BringToFront();

            return appBar;
        }

        public static MaterialAppBar CreateWithNavigation(Form form, string title, EventHandler navigationClick)
        {
            var appBar = CreateForForm(form, title);
            appBar.ShowNavigationIcon = true;
            appBar.NavigationIconClick += navigationClick;

            return appBar;
        }

        #endregion
    }
}
