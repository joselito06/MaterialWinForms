using MaterialWinForms.Components.Buttons;
using MaterialWinForms.Components.Containers;
using MaterialWinForms.Components.Layout;
using MaterialWinForms.Components.Navigation;
using MaterialWinForms.Components.Selection;
using MaterialWinForms.Core;
using MaterialWinForms.Utils;

namespace MaterialWinForms_Test
{
    public partial class Form1 : Form
    {
        private MaterialScaffold scaffold;
        private MaterialFAB mainFab;
        private MaterialFAB fabMenu;
        //private Timer notificationTimer;
        private int notificationCount = 0;
        private Dictionary<string, Panel> pages;
        private string currentPageKey = "dashboard";

        public Form1()
        {
            InitializeComponent();
            MaterialThemeManager.RegisterForm(this);
            SetupAdvancedMaterialApp();
        }

        private void SetupAdvancedMaterialApp()
        {
            // Configurar formulario
            this.Text = "Material Design Advanced App";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 600);

            // Inicializar scaffold
            scaffold = new MaterialScaffold();

            // Configurar scaffold con todos los contenedores
            scaffold.EnableAppBar = true;
            scaffold.EnableDrawer = true;
            scaffold.EnableBottomBar = true;

            // Configurar AppBar avanzado
            SetupAdvancedAppBar();

            // Configurar NavigationDrawer avanzado
            SetupAdvancedNavigationDrawer();

            // Configurar cuerpo de la aplicación
            SetupApplicationBody();

            // Configurar FAB avanzado con menú
            SetupAdvancedFAB();

            // Configurar bottom bar
            SetupBottomBar();

            // Agregar scaffold al formulario
            this.Controls.Add(scaffold);

            // Configurar timer para simular notificaciones
            SetupNotificationSystem();
        }

        private void SetupAdvancedAppBar()
        {
            var appBar = new MaterialAppBar
            {
                Title = "Material Dashboard",
                ShowNavigationIcon = true,
                ShowSubtitle = true,
                Subtitle = "Advanced Material Design App",
                Type = MaterialAppBar.AppBarType.Large
            };

            // Configurar gradiente personalizado
            appBar.Gradient.Type = GradientType.Linear;
            appBar.Gradient.StartColor = Color.FromArgb(63, 81, 181);
            appBar.Gradient.EndColor = Color.FromArgb(103, 58, 183);
            appBar.Gradient.Direction = GradientDirection.Horizontal;

            // Agregar acciones al AppBar
            appBar.AddAction("Buscar", CreateSearchIcon(), (s, e) => ShowSearchDialog());
            appBar.AddAction("Notificaciones", CreateNotificationIcon(), (s, e) => ShowNotifications());
            appBar.AddAction("Configuración", CreateSettingsIcon(), (s, e) => NavigateToPage("settings"));
            appBar.AddAction("Perfil", CreateProfileIcon(), (s, e) => ShowProfile());

            // Configurar navegación
            appBar.NavigationIconClick += (s, e) => scaffold.ToggleDrawer();

            //scaffold.AppBar = appBar;
        }

        private void SetupAdvancedNavigationDrawer()
        {
            var drawer = new MaterialNavigationDrawer();

            // Configurar header con gradiente
            drawer.HeaderTitle = "Material App";
            drawer.HeaderSubtitle = "usuario@ejemplo.com";
            drawer.HeaderBackColor = Color.FromArgb(103, 58, 183);
            drawer.HeaderGradient.Type = GradientType.Linear;
            drawer.HeaderGradient.StartColor = Color.FromArgb(103, 58, 183);
            drawer.HeaderGradient.EndColor = Color.FromArgb(63, 81, 181);

            // Sección principal
            drawer.AddMenuHeader("NAVEGACIÓN");
            drawer.AddMenuItem("Dashboard", CreateDashboardIcon(), (s, e) => NavigateToPage("dashboard"));
            drawer.AddMenuItem("Proyectos", CreateProjectsIcon(), (s, e) => NavigateToPage("projects"));
            drawer.AddMenuItem("Tareas", CreateTasksIcon(), (s, e) => NavigateToPage("tasks"));
            drawer.AddMenuItem("Calendario", CreateCalendarIcon(), (s, e) => NavigateToPage("calendar"));
            drawer.AddMenuItem("Reportes", CreateReportsIcon(), (s, e) => NavigateToPage("reports"));

            drawer.AddMenuDivider();

            // Sección de herramientas
            drawer.AddMenuHeader("HERRAMIENTAS");
            drawer.AddMenuItem("Análisis", CreateAnalyticsIcon(), (s, e) => NavigateToPage("analytics"));
            drawer.AddMenuItem("Configuración", CreateSettingsIcon(), (s, e) => NavigateToPage("settings"));
            drawer.AddMenuItem("Ayuda", CreateHelpIcon(), (s, e) => ShowHelp());

            // Items inferiores
            drawer.AddBottomItem("Cerrar Sesión", CreateLogoutIcon(), (s, e) => Logout());
            drawer.AddBottomItem("Acerca de", CreateInfoIcon(), (s, e) => ShowAbout());

            // Configurar badges dinámicos
            SetupDrawerBadges(drawer);

            // Eventos
            drawer.ItemSelected += OnDrawerItemSelected;

            //scaffold.NavigationDrawer = drawer;
        }

        private void SetupApplicationBody()
        {
            pages = new Dictionary<string, Panel>();

            // Crear páginas
            pages["dashboard"] = CreateDashboardPage();
            pages["projects"] = CreateProjectsPage();
            pages["tasks"] = CreateTasksPage();
            pages["calendar"] = CreateCalendarPage();
            pages["reports"] = CreateReportsPage();
            pages["analytics"] = CreateAnalyticsPage();
            pages["settings"] = CreateSettingsPage();

            // Mostrar página inicial
            NavigateToPage("dashboard");
        }

        private void SetupAdvancedFAB()
        {
            // FAB principal
            mainFab = new MaterialFAB
            {
                Size = new Size(56, 56),
                Icon = CreateAddIcon(),
                BackColor = Color.FromArgb(255, 87, 34),
                UseRippleEffect = true,
                Extended = false
            };

            // Configurar sombra
            mainFab.Shadow.Type = MaterialShadowType.Medium;
            mainFab.Shadow.Blur = 12;
            mainFab.Shadow.OffsetY = 4;

            // FAB Menu con acciones secundarias
            //fabMenu = new MaterialFAB(mainFab);

            //fabMenu.AddAction("Nuevo Proyecto", CreateProjectIcon(), Color.FromArgb(76, 175, 80), (s, e) => CreateNewProject());
            //fabMenu.AddAction("Nueva Tarea", CreateTaskIcon(), Color.FromArgb(33, 150, 243), (s, e) => CreateNewTask());
            //fabMenu.AddAction("Nuevo Evento", CreateEventIcon(), Color.FromArgb(156, 39, 176), (s, e) => CreateNewEvent());
            //fabMenu.AddAction("Importar", CreateImportIcon(), Color.FromArgb(255, 193, 7), (s, e) => ImportData());

            // Configurar ubicación
            scaffold.FabLocation = MaterialScaffold.FloatingActionButtonLocation.EndFloat;
            scaffold.FloatingActionButton = mainFab;
        }

        private void SetupBottomBar()
        {
            var bottomBar = new MaterialBottomBar();

            // Agregar items de navegación rápida
            //bottomBar.AddItem("Inicio", CreateHomeIcon(), true, (s, e) => NavigateToPage("dashboard"));
            //bottomBar.AddItem("Buscar", CreateSearchIcon(), false, (s, e) => ShowSearchDialog());
            //bottomBar.AddItem("Favoritos", CreateFavoriteIcon(), false, (s, e) => ShowFavorites());
            //bottomBar.AddItem("Perfil", CreateProfileIcon(), false, (s, e) => ShowProfile());

            //scaffold.BottomBar = bottomBar;
        }

        private void SetupNotificationSystem()
        {
            //notificationTimer = new Timer { Interval = 30000 }; // 30 segundos
            //notificationTimer.Tick += (s, e) => SimulateNotification();
            //notificationTimer.Start();
        }

        #region Páginas de la aplicación

        private Panel CreateDashboardPage()
        {
            var panel = new Panel { BackColor = Color.FromArgb(250, 250, 250) };

            var titleLabel = new Label
            {
                Text = "Dashboard",
                Font = new Font("Segoe UI", 24f, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(32, 32),
                AutoSize = true
            };

            var subtitleLabel = new Label
            {
                Text = "Bienvenido a tu panel de control",
                Font = new Font("Segoe UI", 12f),
                ForeColor = Color.FromArgb(117, 117, 117),
                Location = new Point(32, 70),
                AutoSize = true
            };

            // Tarjetas de estadísticas
            var statsPanel = CreateStatsPanel();
            statsPanel.Location = new Point(32, 120);

            // Gráfico simulado
            var chartPanel = CreateChartPanel();
            chartPanel.Location = new Point(32, 300);

            panel.Controls.Add(titleLabel);
            panel.Controls.Add(subtitleLabel);
            panel.Controls.Add(statsPanel);
            panel.Controls.Add(chartPanel);

            return panel;
        }

        private Panel CreateProjectsPage()
        {
            var panel = new Panel { BackColor = Color.FromArgb(250, 250, 250) };

            var titleLabel = new Label
            {
                Text = "Proyectos",
                Font = new Font("Segoe UI", 24f, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(32, 32),
                AutoSize = true
            };

            // Lista de proyectos simulada
            var projectsList = CreateProjectsList();
            projectsList.Location = new Point(32, 80);

            panel.Controls.Add(titleLabel);
            panel.Controls.Add(projectsList);

            return panel;
        }

        private Panel CreateTasksPage()
        {
            var panel = new Panel { BackColor = Color.FromArgb(250, 250, 250) };

            var titleLabel = new Label
            {
                Text = "Tareas",
                Font = new Font("Segoe UI", 24f, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(32, 32),
                AutoSize = true
            };

            // Lista de tareas simulada
            var tasksList = CreateTasksList();
            tasksList.Location = new Point(32, 80);

            panel.Controls.Add(titleLabel);
            panel.Controls.Add(tasksList);

            return panel;
        }

        private Panel CreateCalendarPage()
        {
            var panel = new Panel { BackColor = Color.FromArgb(250, 250, 250) };

            var titleLabel = new Label
            {
                Text = "Calendario",
                Font = new Font("Segoe UI", 24f, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(32, 32),
                AutoSize = true
            };

            panel.Controls.Add(titleLabel);
            return panel;
        }

        private Panel CreateReportsPage()
        {
            var panel = new Panel { BackColor = Color.FromArgb(250, 250, 250) };

            var titleLabel = new Label
            {
                Text = "Reportes",
                Font = new Font("Segoe UI", 24f, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(32, 32),
                AutoSize = true
            };

            panel.Controls.Add(titleLabel);
            return panel;
        }

        private Panel CreateAnalyticsPage()
        {
            var panel = new Panel { BackColor = Color.FromArgb(250, 250, 250) };

            var titleLabel = new Label
            {
                Text = "Análisis",
                Font = new Font("Segoe UI", 24f, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(32, 32),
                AutoSize = true
            };

            panel.Controls.Add(titleLabel);
            return panel;
        }

        private Panel CreateSettingsPage()
        {
            var panel = new Panel { BackColor = Color.FromArgb(250, 250, 250) };

            var titleLabel = new Label
            {
                Text = "Configuración",
                Font = new Font("Segoe UI", 24f, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                Location = new Point(32, 32),
                AutoSize = true
            };

            // Switches para configuraciones
            var themeSwitch = new MaterialSwitch
            {
                Text = "Tema Oscuro",
                Location = new Point(32, 100),
                Size = new Size(200, 40)
            };
            themeSwitch.CheckedChanged += (s, e) => ToggleTheme(themeSwitch.Checked);

            var notificationSwitch = new MaterialSwitch
            {
                Text = "Notificaciones",
                Location = new Point(32, 150),
                Size = new Size(200, 40),
                Checked = true
            };

            panel.Controls.Add(titleLabel);
            panel.Controls.Add(themeSwitch);
            panel.Controls.Add(notificationSwitch);

            return panel;
        }

        #endregion

        #region Componentes auxiliares

        private Panel CreateStatsPanel()
        {
            var panel = new Panel
            {
                Size = new Size(800, 150),
                BackColor = Color.Transparent
            };

            var cards = new[]
            {
                CreateStatCard("Proyectos Activos", "24", Color.FromArgb(76, 175, 80)),
                CreateStatCard("Tareas Completadas", "156", Color.FromArgb(33, 150, 243)),
                CreateStatCard("Horas Trabajadas", "1,240", Color.FromArgb(255, 87, 34)),
                CreateStatCard("Eficiencia", "94%", Color.FromArgb(156, 39, 176))
            };

            for (int i = 0; i < cards.Length; i++)
            {
                cards[i].Location = new Point(i * 190, 0);
                panel.Controls.Add(cards[i]);
            }

            return panel;
        }

        private Panel CreateStatCard(string title, string value, Color accentColor)
        {
            var card = new Panel
            {
                Size = new Size(180, 120),
                BackColor = Color.White
            };

            // Agregar sombra simulada con borde
            card.Paint += (s, e) =>
            {
                var bounds = new Rectangle(0, 0, card.Width, card.Height);
                using var pen = new Pen(Color.FromArgb(30, Color.Black), 1);
                e.Graphics.DrawRectangle(pen, bounds);
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 28f, FontStyle.Bold),
                ForeColor = accentColor,
                Location = new Point(16, 16),
                AutoSize = true
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(117, 117, 117),
                Location = new Point(16, 80),
                AutoSize = true
            };

            card.Controls.Add(valueLabel);
            card.Controls.Add(titleLabel);

            return card;
        }

        private Panel CreateChartPanel()
        {
            var panel = new Panel
            {
                Size = new Size(800, 200),
                BackColor = Color.White
            };

            var titleLabel = new Label
            {
                Text = "Actividad Semanal",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                Location = new Point(16, 16),
                AutoSize = true
            };

            // Simulación de gráfico con barras
            panel.Paint += (s, e) =>
            {
                var g = e.Graphics;
                var barWidth = 40;
                var maxHeight = 120;
                var values = new[] { 0.8f, 0.6f, 0.9f, 0.4f, 0.7f, 0.3f, 0.5f };
                var colors = new[] { Color.FromArgb(76, 175, 80), Color.FromArgb(33, 150, 243), Color.FromArgb(255, 87, 34) };

                for (int i = 0; i < values.Length; i++)
                {
                    var height = (int)(values[i] * maxHeight);
                    var x = 60 + i * (barWidth + 10);
                    var y = 170 - height;
                    var color = colors[i % colors.Length];

                    using var brush = new SolidBrush(color);
                    g.FillRectangle(brush, x, y, barWidth, height);
                }
            };

            panel.Controls.Add(titleLabel);
            return panel;
        }

        private ListView CreateProjectsList()
        {
            var listView = new ListView
            {
                Size = new Size(700, 300),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };

            listView.Columns.Add("Proyecto", 200);
            listView.Columns.Add("Estado", 100);
            listView.Columns.Add("Progreso", 100);
            listView.Columns.Add("Fecha", 120);

            var projects = new[]
            {
                new ListViewItem(new[] { "App Móvil", "En Progreso", "75%", "2024-01-15" }),
                new ListViewItem(new[] { "Sitio Web", "Completado", "100%", "2024-01-10" }),
                new ListViewItem(new[] { "Dashboard", "En Progreso", "45%", "2024-01-20" }),
                new ListViewItem(new[] { "API Rest", "Planificado", "0%", "2024-02-01" })
            };

            listView.Items.AddRange(projects);
            return listView;
        }

        private ListView CreateTasksList()
        {
            var listView = new ListView
            {
                Size = new Size(700, 300),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };

            listView.Columns.Add("Tarea", 300);
            listView.Columns.Add("Prioridad", 100);
            listView.Columns.Add("Estado", 100);
            listView.Columns.Add("Asignado", 120);

            var tasks = new[]
            {
                new ListViewItem(new[] { "Diseñar interfaz de usuario", "Alta", "En Progreso", "Juan Pérez" }),
                new ListViewItem(new[] { "Implementar autenticación", "Media", "Completado", "María García" }),
                new ListViewItem(new[] { "Pruebas unitarias", "Alta", "Pendiente", "Carlos López" }),
                new ListViewItem(new[] { "Documentación API", "Baja", "En Progreso", "Ana Rodríguez" })
            };

            listView.Items.AddRange(tasks);
            return listView;
        }

        #endregion

        #region Event Handlers

        private void NavigateToPage(string pageKey)
        {
            if (!pages.ContainsKey(pageKey)) return;

            currentPageKey = pageKey;
            scaffold.Body = pages[pageKey];

            // Actualizar título del AppBar
            if (scaffold.AppBar != null)
            {
                scaffold.AppBar.Title = GetPageTitle(pageKey);
            }

            // Actualizar selección en drawer
            UpdateDrawerSelection(pageKey);
        }

        private string GetPageTitle(string pageKey)
        {
            return pageKey switch
            {
                "dashboard" => "Material Dashboard",
                "projects" => "Proyectos",
                "tasks" => "Tareas",
                "calendar" => "Calendario",
                "reports" => "Reportes",
                "analytics" => "Análisis",
                "settings" => "Configuración",
                _ => "Material App"
            };
        }

        private void OnDrawerItemSelected(object sender, MaterialNavigationDrawer.DrawerItemSelectedEventArgs e)
        {
            // Manejar selección del drawer
            var selectedText = e.Item.Text.ToLower();

            if (pages.ContainsKey(selectedText))
            {
                NavigateToPage(selectedText);
            }
        }

        private void ToggleTheme(bool isDark)
        {
            var newScheme = isDark ? MaterialColorScheme.Dark : MaterialColorScheme.Light;
            MaterialThemeManager.SetGlobalTheme(newScheme);
        }

        #endregion

        #region FAB Actions

        private void CreateNewProject()
        {
            MessageBox.Show("Crear Nuevo Proyecto", "Acción FAB", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CreateNewTask()
        {
            MessageBox.Show("Crear Nueva Tarea", "Acción FAB", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CreateNewEvent()
        {
            MessageBox.Show("Crear Nuevo Evento", "Acción FAB", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ImportData()
        {
            MessageBox.Show("Importar Datos", "Acción FAB", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region AppBar Actions

        private void ShowSearchDialog()
        {
            MessageBox.Show("Función de Búsqueda", "Buscar", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowNotifications()
        {
            MessageBox.Show($"Tienes {notificationCount} notificaciones", "Notificaciones", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowProfile()
        {
            MessageBox.Show("Perfil de Usuario", "Perfil", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region Utility Methods

        private void SimulateNotification()
        {
            notificationCount++;
            UpdateNotificationBadge();
        }

        private void UpdateNotificationBadge()
        {
            if (scaffold.NavigationDrawer != null)
            {
                // Actualizar badge en el drawer si existe un item de notificaciones
                var notificationItem = FindDrawerItem("notificaciones");
                if (notificationItem != null)
                {
                    notificationItem.BadgeCount = notificationCount;
                    scaffold.NavigationDrawer.Invalidate();
                }
            }
        }

        private MaterialNavigationDrawer.DrawerItem FindDrawerItem(string text)
        {
            if (scaffold.NavigationDrawer == null) return null;

            /*foreach (var item in scaffold.NavigationDrawer.MenuItems)
            {
                if (item.Text.ToLower().Contains(text.ToLower()))
                    return item;
            }*/
            return null;
        }

        private void SetupDrawerBadges(MaterialNavigationDrawer drawer)
        {
            // Configurar badges iniciales
            var tasksItem = FindDrawerItem("tareas");
            if (tasksItem != null)
            {
                tasksItem.BadgeCount = 5;
                tasksItem.BadgeColor = Color.FromArgb(255, 87, 34);
            }

            var projectsItem = FindDrawerItem("proyectos");
            if (projectsItem != null)
            {
                projectsItem.BadgeText = "NEW";
                projectsItem.BadgeColor = Color.FromArgb(76, 175, 80);
            }
        }

        private void UpdateDrawerSelection(string pageKey)
        {
            // Lógica para actualizar la selección visual en el drawer
            // Esto dependería de la implementación específica del NavigationDrawer
        }

        private void ShowFavorites()
        {
            MessageBox.Show("Favoritos", "Bottom Navigation", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowHelp()
        {
            MessageBox.Show("Ayuda y Soporte", "Ayuda", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowAbout()
        {
            MessageBox.Show("Material Design WinForms\nVersión 1.0", "Acerca de", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Logout()
        {
            if (MessageBox.Show("¿Seguro que deseas cerrar sesión?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        #endregion

        #region Icon Creation Methods (Placeholders)

        private Image CreateSearchIcon() => CreatePlaceholderIcon("🔍");
        private Image CreateNotificationIcon() => CreatePlaceholderIcon("🔔");
        private Image CreateSettingsIcon() => CreatePlaceholderIcon("⚙️");
        private Image CreateProfileIcon() => CreatePlaceholderIcon("👤");
        private Image CreateDashboardIcon() => CreatePlaceholderIcon("📊");
        private Image CreateProjectsIcon() => CreatePlaceholderIcon("📁");
        private Image CreateTasksIcon() => CreatePlaceholderIcon("✓");
        private Image CreateCalendarIcon() => CreatePlaceholderIcon("📅");
        private Image CreateReportsIcon() => CreatePlaceholderIcon("📈");
        private Image CreateAnalyticsIcon() => CreatePlaceholderIcon("📊");
        private Image CreateHelpIcon() => CreatePlaceholderIcon("❓");
        private Image CreateLogoutIcon() => CreatePlaceholderIcon("🚪");
        private Image CreateInfoIcon() => CreatePlaceholderIcon("ℹ️");
        private Image CreateAddIcon() => CreatePlaceholderIcon("➕");
        private Image CreateProjectIcon() => CreatePlaceholderIcon("📋");
        private Image CreateTaskIcon() => CreatePlaceholderIcon("📝");
        private Image CreateEventIcon() => CreatePlaceholderIcon("📅");
        private Image CreateImportIcon() => CreatePlaceholderIcon("📥");
        private Image CreateHomeIcon() => CreatePlaceholderIcon("🏠");
        private Image CreateFavoriteIcon() => CreatePlaceholderIcon("⭐");

        private Image CreatePlaceholderIcon(string emoji)
        {
            var bitmap = new Bitmap(24, 24);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                using (var brush = new SolidBrush(Color.White))
                using (var font = new Font("Segoe UI Emoji", 12f))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(emoji, font, brush, new RectangleF(0, 0, 24, 24), sf);
                }
            }
            return bitmap;
        }

        #endregion

        /*protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //notificationTimer?.Stop();
                //notificationTimer?.Dispose();
            }
            base.Dispose(disposing);
        }*/
    }

    // Clases adicionales necesarias para el ejemplo completo
    public class MaterialFAB : Button
    {
        public Image Icon { get; set; }
        public bool Extended { get; set; }
        public bool UseRippleEffect { get; set; }
        public ShadowSettings Shadow { get; set; } = new ShadowSettings();

        public MaterialFAB()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Size = new Size(56, 56);
            Font = new Font("Segoe UI", 14f);
            UseVisualStyleBackColor = false;
        }
    }

    public class MaterialFABMenu
    {
        private MaterialFAB mainFab;
        private List<MaterialFAB> actionFabs = new List<MaterialFAB>();
        private bool isExpanded = false;

        public MaterialFABMenu(MaterialFAB mainFab)
        {
            this.mainFab = mainFab;
            this.mainFab.Click += ToggleMenu;
        }

        public void AddAction(string text, Image icon, Color color, EventHandler clickHandler)
        {
            var actionFab = new MaterialFAB
            {
                Size = new Size(40, 40),
                Icon = icon,
                BackColor = color,
                Visible = false
            };
            actionFab.Click += clickHandler;
            actionFabs.Add(actionFab);

            if (mainFab.Parent != null)
            {
                mainFab.Parent.Controls.Add(actionFab);
            }
        }

        private void ToggleMenu(object sender, EventArgs e)
        {
            isExpanded = !isExpanded;
            ShowActions(isExpanded);
        }

        private void ShowActions(bool show)
        {
            for (int i = 0; i < actionFabs.Count; i++)
            {
                var fab = actionFabs[i];
                fab.Visible = show;
                if (show)
                {
                    fab.Location = new Point(
                        mainFab.Location.X + 8,
                        mainFab.Location.Y - (i + 1) * 60
                    );
                }
            }
        }
    }

    

}
