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
    public partial class DrawerModerno : MaterialNavigationDrawerBase
    {
        private Panel headerPanel;
        private ListView menuList;
        private ShadowSettings shadowSettings;

        public DrawerModerno()
        {
            shadowSettings = new ShadowSettings();
            InitializeComponent();
        }

        private void InitializeCustomComponent()
        {
            this.SuspendLayout();

            // Header personalizado
            headerPanel = new Panel
            {
                Height = 160,
                Dock = DockStyle.Top,
                //BackgroundImage = Properties.Resources.DrawerBackground // Tu imagen de fondo
            };

            var avatarPicture = new PictureBox
            {
                Size = new Size(64, 64),
                Location = new Point(16, 80),
                //Image = Properties.Resources.UserAvatar,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            var nameLabel = new Label
            {
                Location = new Point(16, 120),
                Size = new Size(200, 20),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold)
            };

            headerPanel.Controls.Add(avatarPicture);
            headerPanel.Controls.Add(nameLabel);

            // Lista de menú personalizada
            menuList = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                HeaderStyle = ColumnHeaderStyle.None,
                FullRowSelect = true,
                MultiSelect = false,
                BorderStyle = BorderStyle.None
            };
            menuList.Columns.Add("Items", 280);
            menuList.ItemSelectionChanged += MenuList_ItemSelectionChanged;

            this.Controls.Add(headerPanel);
            this.Controls.Add(menuList);

            // Configurar Material Design
            this.BackColor = ColorScheme.Surface;

            this.ResumeLayout();
        }

        private void SetupCustomDrawer()
        {
            // Agregar items del menú
            AddMenuItem("🏠 Inicio", "home");
            AddMenuItem("👥 Clientes", "customers");
            AddMenuItem("📦 Productos", "products");
            AddMenuItem("⚙️ Configuración", "settings");

            // Configurar sombra
            shadowSettings.Type = MaterialShadowType.Soft;
            shadowSettings.Blur = 12;
            shadowSettings.OffsetX = 4;
        }

        private void AddMenuItem(string text, string key)
        {
            var item = new ListViewItem(text) { Tag = key };
            menuList.Items.Add(item);
        }

        private void MenuList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected && e.Item.Tag != null)
            {
                OnItemSelected(e.Item.Tag.ToString());
            }
        }

        protected override void OnHeaderTitleChanged()
        {
            var nameLabel = headerPanel.Controls.OfType<Label>().FirstOrDefault();
            if (nameLabel != null)
            {
                nameLabel.Text = HeaderTitle;
            }
        }

        protected override void OnExpandedStateChanged()
        {
            // Animar cambio de tamaño, ocultar/mostrar texto, etc.
            if (IsExpanded)
            {
                // Mostrar elementos completos
            }
            else
            {
                // Mostrar solo iconos
            }
        }
    }
}
