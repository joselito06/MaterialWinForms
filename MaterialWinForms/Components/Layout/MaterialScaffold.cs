using MaterialWinForms.Components.Containers;
using MaterialWinForms.Components.Navigation;
using MaterialWinForms.Core;
using MaterialWinForms.Core.CustomControls;
using MaterialWinForms.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaterialWinForms.Components.Layout
{
    /// <summary>
    /// Material Scaffold - Contenedor principal que estructura toda la aplicación
    /// </summary>
    [ToolboxItem(true)]
    [DesignTimeVisible(true)]
    public class MaterialScaffold : MaterialControl
    {
        // Paneles contenedores
        private Panel _appBarContainer;
        private Panel _drawerContainer;
        private Panel _bodyContainer;
        private Panel _bottomBarContainer;
        private Panel _sideSheetContainer;

        // Componentes Material
        private MaterialAppBarBase? _appBar;
        private MaterialNavigationDrawerBase? _navigationDrawer;
        private Control? _body;
        private MaterialBottomBarBase? _bottomBar;
        private Control? _floatingActionButton;
        private MaterialSideSheet? _sideSheet;

        // Configuración
        private bool _enableAppBar = false;
        private bool _enableDrawer = false;
        private bool _enableBottomBar = false;
        private bool _enableSideSheet = false;
        private bool _extendBodyBehindAppBar = false;
        private ScaffoldLayoutBehavior _layoutBehavior = ScaffoldLayoutBehavior.Standard;
        private Color _backgroundColor = Color.Transparent;
        private GradientSettings _backgroundGradient = new GradientSettings();

        // Drawer animation
        private System.Windows.Forms.Timer? _animationTimer;
        private bool _isAnimating = false;
        private int _drawerTargetWidth;
        private int _drawerCurrentWidth;

        public enum ScaffoldLayoutBehavior
        {
            Standard,
            Collapsing,
            FloatingAppBar,
            CenterAligned,
            Large,
            Dense
        }

        public enum FloatingActionButtonLocation
        {
            EndFloat,
            CenterFloat,
            StartFloat,
            EndDocked,
            CenterDocked,
            StartDocked,
            EndTop,
            StartTop
        }

        #region Propiedades de habilitación

        [Category("Material - Layout")]
        [Description("Habilitar contenedor para AppBar")]
        [DefaultValue(false)]
        public bool EnableAppBar
        {
            get => _enableAppBar;
            set { _enableAppBar = value; UpdateContainers(); }
        }

        [Category("Material - Layout")]
        [Description("Habilitar contenedor para NavigationDrawer")]
        [DefaultValue(false)]
        public bool EnableDrawer
        {
            get => _enableDrawer;
            set { _enableDrawer = value; UpdateContainers(); }
        }

        [Category("Material - Layout")]
        [Description("Habilitar contenedor para BottomBar")]
        [DefaultValue(false)]
        public bool EnableBottomBar
        {
            get => _enableBottomBar;
            set { _enableBottomBar = value; UpdateContainers(); }
        }

        [Category("Material - Layout")]
        [Description("Habilitar contenedor para SideSheet")]
        [DefaultValue(false)]
        public bool EnableSideSheet
        {
            get => _enableSideSheet;
            set { _enableSideSheet = value; UpdateContainers(); }
        }

        #endregion

        #region Propiedades de componentes

        [Category("Material - Components")]
        [Description("AppBar de la aplicación")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public MaterialAppBarBase? AppBar
        {
            get => _appBar;
            set { SetAppBar(value); }
        }

        [Category("Material - Components")]
        [Description("NavigationDrawer de la aplicación")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public MaterialNavigationDrawerBase? NavigationDrawer
        {
            get => _navigationDrawer;
            set { SetNavigationDrawer(value); }
        }

        [Category("Material - Components")]
        [Description("Contenido principal")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Control? Body
        {
            get => _body;
            set { SetBody(value); }
        }

        [Category("Material - Components")]
        [Description("BottomBar de la aplicación")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public MaterialBottomBarBase? BottomBar
        {
            get => _bottomBar;
            set { SetBottomBar(value); }
        }

        [Category("Material - Components")]
        [Description("SideSheet de la aplicación")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public MaterialSideSheet? SideSheet
        {
            get => _sideSheet;
            set { SetSideSheet(value); }
        }

        [Category("Material - Components")]
        [Description("Botón de acción flotante")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Control? FloatingActionButton
        {
            get => _floatingActionButton;
            set { SetFloatingActionButton(value); }
        }

        #endregion

        #region Propiedades de configuración

        [Category("Material - Layout")]
        [Description("Ubicación del botón de acción flotante")]
        [DefaultValue(FloatingActionButtonLocation.EndFloat)]
        public FloatingActionButtonLocation FabLocation { get; set; } = FloatingActionButtonLocation.EndFloat;

        [Category("Material - Behavior")]
        [Description("El cuerpo se extiende detrás del AppBar")]
        [DefaultValue(false)]
        public bool ExtendBodyBehindAppBar
        {
            get => _extendBodyBehindAppBar;
            set { _extendBodyBehindAppBar = value; UpdateLayout(); }
        }

        [Category("Material - Layout")]
        [Description("Comportamiento del layout")]
        [DefaultValue(ScaffoldLayoutBehavior.Standard)]
        public ScaffoldLayoutBehavior LayoutBehavior
        {
            get => _layoutBehavior;
            set { _layoutBehavior = value; UpdateLayoutBehavior(); }
        }

        [Category("Material - Appearance")]
        [Description("Color de fondo")]
        [DefaultValue(typeof(Color), "Transparent")]
        public Color BackgroundColor
        {
            get => _backgroundColor;
            set { _backgroundColor = value; Invalidate(); }
        }

        [Category("Material - Appearance")]
        [Description("Gradiente de fondo")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GradientSettings BackgroundGradient
        {
            get => _backgroundGradient;
            set { _backgroundGradient = value ?? new GradientSettings(); Invalidate(); }
        }

        #endregion

        #region Propiedades de acceso a contenedores

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Panel AppBarContainer => _appBarContainer;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Panel DrawerContainer => _drawerContainer;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Panel BodyContainer => _bodyContainer;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Panel BottomBarContainer => _bottomBarContainer;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Panel SideSheetContainer => _sideSheetContainer;

        #endregion

        #region Eventos

        public event EventHandler? DrawerOpened;
        public event EventHandler? DrawerClosed;
        public event EventHandler<Control>? BodyChanged;
        public event EventHandler<string>? LayoutUpdated;

        #endregion

        public MaterialScaffold()
        {
            InitializeScaffold();
        }

        private void InitializeScaffold()
        {
            Dock = DockStyle.Fill;

            // Crear contenedores
            CreateContainers();

            // Configurar animación para drawer
            _animationTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _animationTimer.Tick += AnimationTimer_Tick;

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.ContainerControl, true);
        }

        private void CreateContainers()
        {
            SuspendLayout();

            // AppBar Container (Top)
            _appBarContainer = new Panel
            {
                Name = "AppBarContainer",
                BackColor = Color.Transparent,
                Visible = false,
                Height = 64,
                Dock = DockStyle.Top
            };

            // Drawer Container (Left) 
            _drawerContainer = new Panel
            {
                Name = "DrawerContainer",
                BackColor = Color.Transparent,
                Visible = false,
                Width = 280,
                Dock = DockStyle.Left
            };

            // SideSheet Container (Right)
            _sideSheetContainer = new Panel
            {
                Name = "SideSheetContainer",
                BackColor = Color.Transparent,
                Visible = false,
                Width = 320,
                Dock = DockStyle.Right
            };

            // BottomBar Container (Bottom)
            _bottomBarContainer = new Panel
            {
                Name = "BottomBarContainer",
                BackColor = Color.Transparent,
                Visible = false,
                Height = 56,
                Dock = DockStyle.Bottom
            };

            // Body Container (Fill)
            _bodyContainer = new Panel
            {
                Name = "BodyContainer",
                BackColor = Color.Transparent,
                Dock = DockStyle.Fill
            };

            // Agregar en orden correcto para docking
            Controls.Add(_bodyContainer);          // Fill - debe ser primero
            Controls.Add(_sideSheetContainer);     // Right
            Controls.Add(_drawerContainer);        // Left  
            Controls.Add(_bottomBarContainer);     // Bottom
            Controls.Add(_appBarContainer);        // Top - debe ser último

            ResumeLayout();
        }

        private void UpdateContainers()
        {
            SuspendLayout();

            _appBarContainer.Visible = _enableAppBar;
            _drawerContainer.Visible = _enableDrawer;
            _bottomBarContainer.Visible = _enableBottomBar;
            _sideSheetContainer.Visible = _enableSideSheet;

            // Si el drawer está deshabilitado pero hay un drawer asignado, ocultarlo
            if (!_enableDrawer && _navigationDrawer != null)
            {
                _navigationDrawer.Visible = false;
            }
            else if (_enableDrawer && _navigationDrawer != null)
            {
                _navigationDrawer.Visible = true;
            }

            ResumeLayout();
            UpdateLayout();
        }

        #region Métodos de configuración de componentes

        private void SetAppBar(MaterialAppBarBase? appBar)
        {
            if (_appBar != null)
            {
                _appBarContainer.Controls.Remove(_appBar);
            }

            _appBar = appBar;

            if (_appBar != null && _enableAppBar)
            {
                _appBar.Dock = DockStyle.Fill;
                _appBarContainer.Controls.Add(_appBar);
                _appBarContainer.Height = _appBar.Height;
            }
        }

        private void SetNavigationDrawer(MaterialNavigationDrawerBase? drawer)
        {
            if (_navigationDrawer != null)
            {
                _drawerContainer.Controls.Remove(_navigationDrawer);
                _navigationDrawer.ExpandedStateChanged -= OnDrawerStateChanged;
            }

            _navigationDrawer = drawer;

            if (_navigationDrawer != null && _enableDrawer)
            {
                _navigationDrawer.Dock = DockStyle.Fill;
                _drawerContainer.Controls.Add(_navigationDrawer);
                _navigationDrawer.ExpandedStateChanged += OnDrawerStateChanged;
                UpdateDrawerWidth();
            }
        }

        private void SetBody(Control? body)
        {
            if (_body != null)
            {
                _bodyContainer.Controls.Remove(_body);
            }

            _body = body;

            if (_body != null)
            {
                _body.Dock = DockStyle.Fill;
                _bodyContainer.Controls.Add(_body);
            }

            BodyChanged?.Invoke(this, _body);
        }

        private void SetBottomBar(MaterialBottomBarBase? bottomBar)
        {
            if (_bottomBar != null)
            {
                _bottomBarContainer.Controls.Remove(_bottomBar);
            }

            _bottomBar = bottomBar;

            if (_bottomBar != null && _enableBottomBar)
            {
                _bottomBar.Dock = DockStyle.Fill;
                _bottomBarContainer.Controls.Add(_bottomBar);
                _bottomBarContainer.Height = _bottomBar.Height;
            }
        }

        private void SetSideSheet(MaterialSideSheet? sideSheet)
        {
            if (_sideSheet != null)
            {
                _sideSheetContainer.Controls.Remove(_sideSheet);
            }

            _sideSheet = sideSheet;

            if (_sideSheet != null && _enableSideSheet)
            {
                _sideSheet.Dock = DockStyle.Fill;
                _sideSheetContainer.Controls.Add(_sideSheet);
                _sideSheetContainer.Width = _sideSheet.Width;
            }
        }

        private void SetFloatingActionButton(Control? fab)
        {
            if (_floatingActionButton != null)
            {
                Controls.Remove(_floatingActionButton);
            }

            _floatingActionButton = fab;

            if (_floatingActionButton != null)
            {
                _floatingActionButton.Anchor = AnchorStyles.None;
                Controls.Add(_floatingActionButton);
                _floatingActionButton.BringToFront();
                UpdateFabLocation();
            }
        }

        #endregion

        #region Gestión del Drawer

        private void OnDrawerStateChanged(object? sender, bool isExpanded)
        {
            if (isExpanded)
            {
                DrawerOpened?.Invoke(this, EventArgs.Empty);
                _drawerTargetWidth = 280;
            }
            else
            {
                DrawerClosed?.Invoke(this, EventArgs.Empty);
                _drawerTargetWidth = 72;
            }

            StartDrawerAnimation();
        }

        private void StartDrawerAnimation()
        {
            if (_isAnimating || !_enableDrawer) return;

            _drawerCurrentWidth = _drawerContainer.Width;
            _isAnimating = true;
            _animationTimer?.Start();
        }

        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            var step = Math.Sign(_drawerTargetWidth - _drawerCurrentWidth) * Math.Max(8, Math.Abs(_drawerTargetWidth - _drawerCurrentWidth) / 10);
            _drawerCurrentWidth += step;

            if (Math.Sign(step) > 0 ? _drawerCurrentWidth >= _drawerTargetWidth : _drawerCurrentWidth <= _drawerTargetWidth)
            {
                _drawerCurrentWidth = _drawerTargetWidth;
                _isAnimating = false;
                _animationTimer?.Stop();
            }

            _drawerContainer.Width = _drawerCurrentWidth;
        }

        private void UpdateDrawerWidth()
        {
            if (_navigationDrawer == null || !_enableDrawer) return;

            _drawerTargetWidth = _navigationDrawer.IsExpanded ? 280 : 72;
            _drawerCurrentWidth = _drawerTargetWidth;
            _drawerContainer.Width = _drawerCurrentWidth;
        }

        #endregion

        #region Layout Management

        private void UpdateLayout()
        {
            if (_floatingActionButton != null)
            {
                UpdateFabLocation();
            }

            LayoutUpdated?.Invoke(this, GetDebugInfo());
        }

        private void UpdateLayoutBehavior()
        {
            switch (_layoutBehavior)
            {
                case ScaffoldLayoutBehavior.FloatingAppBar:
                    if (_appBar != null)
                    {
                        //_appBar.CornerRadius = new CornerRadius(12);
                        //_appBar.Shadow.Type = MaterialShadowType.Medium;
                    }
                    break;
                case ScaffoldLayoutBehavior.Dense:
                    _appBarContainer.Height = 48;
                    _bottomBarContainer.Height = 48;
                    break;
                case ScaffoldLayoutBehavior.Large:
                    _appBarContainer.Height = 96;
                    break;
            }
        }

        private void UpdateFabLocation()
        {
            if (_floatingActionButton == null) return;

            var fabSize = _floatingActionButton.Size;
            var margin = 16;
            var dockMargin = 8;

            var drawerWidth = _enableDrawer && _drawerContainer.Visible ? _drawerContainer.Width : 0;
            var appBarHeight = _enableAppBar && _appBarContainer.Visible ? _appBarContainer.Height : 0;
            var bottomBarHeight = _enableBottomBar && _bottomBarContainer.Visible ? _bottomBarContainer.Height : 0;

            Point newLocation = FabLocation switch
            {
                FloatingActionButtonLocation.EndFloat => new Point(
                    Width - fabSize.Width - margin,
                    Height - fabSize.Height - margin - bottomBarHeight),

                FloatingActionButtonLocation.CenterFloat => new Point(
                    drawerWidth + (Width - drawerWidth - fabSize.Width) / 2,
                    Height - fabSize.Height - margin - bottomBarHeight),

                FloatingActionButtonLocation.StartFloat => new Point(
                    margin + drawerWidth,
                    Height - fabSize.Height - margin - bottomBarHeight),

                FloatingActionButtonLocation.EndTop => new Point(
                    Width - fabSize.Width - margin,
                    appBarHeight + margin),

                _ => new Point(Width - fabSize.Width - margin, Height - fabSize.Height - margin - bottomBarHeight)
            };

            _floatingActionButton.Location = newLocation;
        }

        #endregion

        #region Event Handlers

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateLayout();
        }

        protected override void OnColorSchemeChanged()
        {
            base.OnColorSchemeChanged();

            _appBar?.Invalidate();
            _navigationDrawer?.Invalidate();
            _bottomBar?.Invalidate();
            _sideSheet?.Invalidate();

            Invalidate();
        }

        #endregion

        #region Painting

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var bounds = new Rectangle(0, 0, Width, Height);

            if (_backgroundGradient.Type != GradientType.None)
            {
                g.FillRoundedRectangleWithGradient(bounds, new CornerRadius(0), _backgroundGradient);
            }
            else
            {
                var backgroundColor = _backgroundColor != Color.Transparent ? _backgroundColor : ColorScheme.Background;
                using var brush = new SolidBrush(backgroundColor);
                g.FillRectangle(brush, bounds);
            }

            base.OnPaint(e);
        }

        #endregion

        #region Métodos públicos

        public void OpenDrawer()
        {
            _navigationDrawer?.Expand();
        }

        public void CloseDrawer()
        {
            _navigationDrawer?.Collapse();
        }

        public void ToggleDrawer()
        {
            _navigationDrawer?.Toggle();
        }

        public void SetupStandardLayout(string appTitle = "")
        {
            // Habilitar contenedores
            EnableAppBar = true;
            EnableDrawer = true;

            // Crear componentes
            var appBar = new MaterialAppBar
            {
                Title = appTitle,
                ShowNavigationIcon = true
            };
            appBar.NavigationIconClick += (s, e) => ToggleDrawer();

            var drawer = new MaterialNavigationDrawer();

            // Asignar
            //AppBar = appBar;
            //NavigationDrawer = drawer;
        }

        public string GetDebugInfo()
        {
            var info = new System.Text.StringBuilder();
            info.AppendLine("=== Material Scaffold Debug Info ===");
            info.AppendLine($"Containers Enabled:");
            info.AppendLine($"  - AppBar: {_enableAppBar} (Visible: {_appBarContainer.Visible})");
            info.AppendLine($"  - Drawer: {_enableDrawer} (Visible: {_drawerContainer.Visible}, Width: {_drawerContainer.Width})");
            info.AppendLine($"  - BottomBar: {_enableBottomBar} (Visible: {_bottomBarContainer.Visible})");
            info.AppendLine($"  - SideSheet: {_enableSideSheet} (Visible: {_sideSheetContainer.Visible})");
            info.AppendLine($"Components:");
            info.AppendLine($"  - AppBar: {(_appBar != null ? "Present" : "Null")}");
            info.AppendLine($"  - NavigationDrawer: {(_navigationDrawer != null ? "Present" : "Null")}");
            info.AppendLine($"  - Body: {(_body != null ? "Present" : "Null")}");
            info.AppendLine($"  - BottomBar: {(_bottomBar != null ? "Present" : "Null")}");
            info.AppendLine($"Layout:");
            info.AppendLine($"  - Scaffold Size: {Size}");
            info.AppendLine($"  - Body Container Bounds: {_bodyContainer.Bounds}");
            return info.ToString();
        }

        /// <summary>
        /// Configurar estilo del drawer
        /// </summary>
        public void SetDrawerStyle(NavigationDrawerStyle style, DrawerHeaderStyle headerStyle = DrawerHeaderStyle.Standard)
        {
            if (NavigationDrawer != null)
            {
                //NavigationDrawer.Style = style;
                //NavigationDrawer.HeaderStyle = headerStyle;
            }
        }

        /// <summary>
        /// Configurar estilo del AppBar
        /// </summary>
        public void SetAppBarStyle(AppBarStyle style)
        {
            if (AppBar != null)
            {
                //AppBar.Style = style;
            }
        }

        /// <summary>
        /// Configuración rápida de estilos
        /// </summary>
        public void SetMaterialStyle(NavigationDrawerStyle drawerStyle, AppBarStyle appBarStyle, DrawerHeaderStyle headerStyle = DrawerHeaderStyle.Standard)
        {
            SetDrawerStyle(drawerStyle, headerStyle);
            SetAppBarStyle(appBarStyle);
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animationTimer?.Stop();
                _animationTimer?.Dispose();

                if (_navigationDrawer != null)
                    _navigationDrawer.ExpandedStateChanged -= OnDrawerStateChanged;
            }
            base.Dispose(disposing);
        }

        #region Métodos para UserControls personalizados

        /// <summary>
        /// Establecer un AppBar personalizado
        /// </summary>
        public void SetCustomAppBar(MaterialAppBarBase customAppBar)
        {
            EnableAppBar = true;

            // Remover AppBar anterior si existe
            if (_appBar != null)
            {
                _appBarContainer.Controls.Remove(_appBar);
            }

            _appBar = customAppBar;

            if (_appBar != null)
            {
                _appBar.Dock = DockStyle.Fill;
                _appBarContainer.Controls.Add(_appBar);
                _appBarContainer.Height = _appBar.Height;
            }
        }

        /// <summary>
        /// Establecer un NavigationDrawer personalizado
        /// </summary>
        public void SetCustomDrawer(MaterialNavigationDrawerBase customDrawer)
        {
            EnableDrawer = true;

            // Remover drawer anterior si existe
            if (_navigationDrawer != null)
            {
                _drawerContainer.Controls.Remove(_navigationDrawer);
                if (_navigationDrawer is MaterialNavigationDrawerBase drawerBase)
                {
                    drawerBase.ExpandedStateChanged -= OnCustomDrawerStateChanged;
                }
            }

            _navigationDrawer = customDrawer;

            if (_navigationDrawer != null)
            {
                _navigationDrawer.Dock = DockStyle.Fill;
                _drawerContainer.Controls.Add(_navigationDrawer);

                if (_navigationDrawer is MaterialNavigationDrawerBase drawerBase)
                {
                    drawerBase.ExpandedStateChanged += OnCustomDrawerStateChanged;
                    UpdateCustomDrawerWidth(drawerBase);
                }
            }
        }

        /// <summary>
        /// Establecer un BottomBar personalizado
        /// </summary>
        public void SetCustomBottomBar(MaterialBottomBarBase customBottomBar)
        {
            EnableBottomBar = true;

            if (_bottomBar != null)
            {
                _bottomBarContainer.Controls.Remove(_bottomBar);
            }

            _bottomBar = customBottomBar;

            if (_bottomBar != null)
            {
                _bottomBar.Dock = DockStyle.Fill;
                _bottomBarContainer.Controls.Add(_bottomBar);
                _bottomBarContainer.Height = _bottomBar.Height;
            }
        }

        /// <summary>
        /// Establecer un FAB personalizado
        /// </summary>
        public void SetCustomFAB(MaterialFABBase customFAB)
        {
            SetFloatingActionButton(customFAB);
        }

        private void OnCustomDrawerStateChanged(object? sender, bool isExpanded)
        {
            if (sender is MaterialNavigationDrawerBase drawer)
            {
                if (isExpanded)
                {
                    DrawerOpened?.Invoke(this, EventArgs.Empty);
                    _drawerTargetWidth = drawer.ExpandedWidth;
                }
                else
                {
                    DrawerClosed?.Invoke(this, EventArgs.Empty);
                    _drawerTargetWidth = drawer.CollapsedWidth;
                }

                StartDrawerAnimation();
            }
        }

        private void UpdateCustomDrawerWidth(MaterialNavigationDrawerBase drawer)
        {
            if (!_enableDrawer) return;

            _drawerTargetWidth = drawer.IsExpanded ? drawer.ExpandedWidth : drawer.CollapsedWidth;
            _drawerCurrentWidth = _drawerTargetWidth;
            _drawerContainer.Width = _drawerCurrentWidth;
        }

        #endregion
    }

    // Placeholder classes
    public class MaterialBottomBar : MaterialControl
    {
        public MaterialBottomBar()
        {
            Height = 56;
            BackColor = ColorScheme.Surface;
        }
    }

    public class MaterialSideSheet : MaterialControl
    {
        public MaterialSideSheet()
        {
            Width = 320;
            BackColor = ColorScheme.Surface;
        }
    }
}
