using MaterialWinForms.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaterialWinForms.Utils
{
    /// <summary>
    /// Utilidad para configurar estilos iniciales en formularios Material Design
    /// </summary>
    public static class MaterialStyleInitializer
    {
        /// <summary>
        /// Configurar un formulario con Material Design
        /// </summary>
        public static void InitializeForm(Form form, MaterialTheme theme = MaterialTheme.Light, bool registerForThemeChanges = true)
        {
            // Aplicar tema
            var colorScheme = theme == MaterialTheme.Light ? MaterialColorScheme.Light : MaterialColorScheme.Dark;
            MaterialThemeManager.SetGlobalTheme(colorScheme);

            // Configurar formulario
            form.BackColor = colorScheme.Background;
            form.ForeColor = colorScheme.OnBackground;
            form.Font = new Font("Segoe UI", 9f);

            // Registrar para cambios automáticos de tema
            if (registerForThemeChanges)
            {
                MaterialThemeManager.RegisterForm(form);
            }

            // Aplicar configuraciones básicas
            ApplyBasicFormSettings(form);
        }

        /// <summary>
        /// Aplicar configuraciones predeterminadas a controles existentes
        /// </summary>
        public static void ApplyMaterialStyles(Control parentControl)
        {
            ApplyStylesToChildren(parentControl);
        }

        /// <summary>
        /// Crear configuraciones predeterminadas para diferentes tipos de aplicación
        /// </summary>
        public static MaterialAppConfig CreateAppConfig(MaterialAppType appType)
        {
            return appType switch
            {
                MaterialAppType.Desktop => new MaterialAppConfig
                {
                    Theme = MaterialTheme.Light,
                    PrimaryColor = Color.FromArgb(103, 80, 164),
                    SecondaryColor = Color.FromArgb(255, 87, 34),
                    DefaultElevation = 2,
                    DefaultCornerRadius = 12,
                    UseAnimations = true,
                    FontFamily = "Segoe UI"
                },
                MaterialAppType.Dashboard => new MaterialAppConfig
                {
                    Theme = MaterialTheme.Light,
                    PrimaryColor = Color.FromArgb(33, 150, 243),
                    SecondaryColor = Color.FromArgb(76, 175, 80),
                    DefaultElevation = 1,
                    DefaultCornerRadius = 8,
                    UseAnimations = true,
                    FontFamily = "Segoe UI"
                },
                MaterialAppType.DataEntry => new MaterialAppConfig
                {
                    Theme = MaterialTheme.Light,
                    PrimaryColor = Color.FromArgb(63, 81, 181),
                    SecondaryColor = Color.FromArgb(255, 193, 7),
                    DefaultElevation = 0,
                    DefaultCornerRadius = 4,
                    UseAnimations = false,
                    FontFamily = "Segoe UI"
                },
                _ => new MaterialAppConfig()
            };
        }

        /// <summary>
        /// Aplicar configuración de aplicación
        /// </summary>
        public static void ApplyAppConfig(Form form, MaterialAppConfig config)
        {
            // Crear esquema personalizado
            var customScheme = new MaterialColorScheme
            {
                Primary = config.PrimaryColor,
                Secondary = config.SecondaryColor,
                PrimaryVariant = ColorHelper.Darken(config.PrimaryColor, 0.2f),
                SecondaryVariant = ColorHelper.Darken(config.SecondaryColor, 0.2f),
                Background = config.Theme == MaterialTheme.Light ? Color.FromArgb(250, 250, 250) : Color.FromArgb(18, 18, 18),
                Surface = config.Theme == MaterialTheme.Light ? Color.White : Color.FromArgb(24, 24, 24),
                Error = Color.FromArgb(244, 67, 54),
                OnPrimary = Color.White,
                OnSecondary = Color.White,
                OnBackground = config.Theme == MaterialTheme.Light ? Color.FromArgb(33, 33, 33) : Color.FromArgb(230, 225, 229),
                OnSurface = config.Theme == MaterialTheme.Light ? Color.FromArgb(33, 33, 33) : Color.FromArgb(230, 225, 229),
                OnError = Color.White,
                SecondaryContainer = config.Theme == MaterialTheme.Light ? ColorHelper.Lighten(config.SecondaryColor, 0.8f) : ColorHelper.Darken(config.SecondaryColor, 0.6f),
                OnSecondaryContainer = config.Theme == MaterialTheme.Light ? ColorHelper.Darken(config.SecondaryColor, 0.2f) : ColorHelper.Lighten(config.SecondaryColor, 0.4f)
            };

            MaterialThemeManager.SetGlobalTheme(customScheme);
            InitializeForm(form, config.Theme);
        }

        private static void ApplyBasicFormSettings(Form form)
        {
            // Suavizar texto
            if (form is Form)
            {
                typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(form, true);
            }
        }

        private static void ApplyStylesToChildren(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                // Aplicar configuraciones específicas por tipo
                switch (control)
                {
                    case Label label:
                        label.ForeColor = MaterialThemeManager.CurrentScheme.OnBackground;
                        label.Font = new Font("Segoe UI", label.Font.Size);
                        break;

                    case Button button when !(button is MaterialControl):
                        button.FlatStyle = FlatStyle.Flat;
                        button.BackColor = MaterialThemeManager.CurrentScheme.Primary;
                        button.ForeColor = MaterialThemeManager.CurrentScheme.OnPrimary;
                        button.FlatAppearance.BorderSize = 0;
                        break;

                    case TextBox textBox:
                        textBox.BorderStyle = BorderStyle.FixedSingle;
                        textBox.BackColor = MaterialThemeManager.CurrentScheme.Surface;
                        textBox.ForeColor = MaterialThemeManager.CurrentScheme.OnSurface;
                        break;

                    case Panel panel when !(panel is MaterialControl):
                        panel.BackColor = MaterialThemeManager.CurrentScheme.Surface;
                        break;
                }

                // Aplicar recursivamente
                if (control.HasChildren)
                {
                    ApplyStylesToChildren(control);
                }
            }
        }

        public enum MaterialTheme
        {
            Light,
            Dark
        }

        public enum MaterialAppType
        {
            Desktop,
            Dashboard,
            DataEntry,
            Creative
        }

        public class MaterialAppConfig
        {
            public MaterialTheme Theme { get; set; } = MaterialTheme.Light;
            public Color PrimaryColor { get; set; } = Color.FromArgb(103, 80, 164);
            public Color SecondaryColor { get; set; } = Color.FromArgb(255, 87, 34);
            public int DefaultElevation { get; set; } = 2;
            public int DefaultCornerRadius { get; set; } = 12;
            public bool UseAnimations { get; set; } = true;
            public string FontFamily { get; set; } = "Segoe UI";
        }
    }

    /// <summary>
    /// Extensiones para facilitar la configuración de Material Design
    /// </summary>
    public static class MaterialFormExtensions
    {
        /// <summary>
        /// Configurar formulario como Material Design con una línea
        /// </summary>
        public static void SetupMaterial(this Form form, MaterialStyleInitializer.MaterialTheme theme = MaterialStyleInitializer.MaterialTheme.Light)
        {
            MaterialStyleInitializer.InitializeForm(form, theme);
        }

        /// <summary>
        /// Aplicar configuración de aplicación específica
        /// </summary>
        public static void SetupMaterial(this Form form, MaterialStyleInitializer.MaterialAppType appType)
        {
            var config = MaterialStyleInitializer.CreateAppConfig(appType);
            MaterialStyleInitializer.ApplyAppConfig(form, config);
        }

        /// <summary>
        /// Configuración personalizada completa
        /// </summary>
        public static void SetupMaterial(this Form form, Color primaryColor, Color secondaryColor, MaterialStyleInitializer.MaterialTheme theme = MaterialStyleInitializer.MaterialTheme.Light)
        {
            var config = new MaterialStyleInitializer.MaterialAppConfig
            {
                Theme = theme,
                PrimaryColor = primaryColor,
                SecondaryColor = secondaryColor
            };
            MaterialStyleInitializer.ApplyAppConfig(form, config);
        }
    }
}
