using MaterialWinForms.Core;
using MaterialWinForms.Core.CustomControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaterialWinForms_Test
{
    public partial class AppbarModerno : MaterialAppBarBase
    {
        ShadowSettings shadowSettings;
        CornerRadius cornerRadius;

        public AppbarModerno()
        {
            shadowSettings = new ShadowSettings();
            cornerRadius = new CornerRadius();
            InitializeComponent();
            InitializeCustomComponent();
            SetupCustomAppBar();
        }

        private void InitializeCustomComponent()
        {
            // Este código lo genera el diseñador de Visual Studio
            this.SuspendLayout();

            // Tu diseño personalizado aquí - botones, imágenes, etc.
            var logoButton = new Button
            {
                Size = new Size(40, 40),
                Location = new Point(12, 12),
                //BackgroundImage = Properties.Resources.MyLogo, // Tu logo
                BackgroundImageLayout = ImageLayout.Zoom,
                FlatStyle = FlatStyle.Flat
            };
            logoButton.FlatAppearance.BorderSize = 0;

            var titleLabel = new Label
            {
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                Location = new Point(70, 20),
                Size = new Size(200, 24),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            var searchButton = new Button
            {
                Text = "🔍",
                Size = new Size(40, 40),
                Location = new Point(this.Width - 100, 12),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat
            };

            var menuButton = new Button
            {
                Text = "☰",
                Size = new Size(40, 40),
                Location = new Point(this.Width - 50, 12),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat
            };

            // Conectar eventos
            logoButton.Click += (s, e) => OnNavigationIconClick();
            searchButton.Click += (s, e) => OnActionClick("search");
            menuButton.Click += (s, e) => OnActionClick("menu");

            this.Controls.Add(logoButton);
            this.Controls.Add(titleLabel);
            this.Controls.Add(searchButton);
            this.Controls.Add(menuButton);

            // Configurar Material Design
            this.BackColor = Color.FromArgb(103, 80, 164);
            this.Height = 64;

            this.ResumeLayout();
        }

        private void SetupCustomAppBar()
        {
            // Configurar sombra personalizada
            shadowSettings.Type = MaterialShadowType.Medium;
            shadowSettings.Blur = 8;
            shadowSettings.OffsetY = 4;
            shadowSettings.Opacity = 40;

            // Esquinas redondeadas opcionales
            cornerRadius = new CornerRadius(0, 0, 16, 16);
        }

        // Sobrescribir métodos para actualizar tu UI personalizada
        protected override void OnTitleChanged()
        {
            var titleLabel = Controls.OfType<Label>().FirstOrDefault();
            if (titleLabel != null)
            {
                titleLabel.Text = Title;
            }
        }

        protected override void OnColorSchemeChanged()
        {
            base.OnColorSchemeChanged();
            BackColor = ColorScheme.Primary;
        }
    }
}
