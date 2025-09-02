using MaterialWinForms.Components.Containers;
using MaterialWinForms.Components.Layout;
using MaterialWinForms.Components.Navigation;
using MaterialWinForms.Core.NavigationCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialWinForms.Utils
{
    /// <summary>
    /// Configurador rápido para MaterialScaffold con navegación
    /// </summary>
    public static class MaterialScaffoldExtensions
    {
        private static readonly Dictionary<MaterialScaffold, MaterialFormNavigator> _navigators = new();

        /// <summary>
        /// Obtener o crear el navegador para este scaffold
        /// </summary>
        public static MaterialFormNavigator GetNavigator(this MaterialScaffold scaffold)
        {
            if (!_navigators.ContainsKey(scaffold))
            {
                _navigators[scaffold] = new MaterialFormNavigator(scaffold);
            }
            return _navigators[scaffold];
        }

        /// <summary>
        /// Configuración rápida de scaffold con navegación
        /// </summary>
        public static MaterialFormNavigator SetupWithNavigation(
            this MaterialScaffold scaffold,
            string appTitle = "Mi Aplicación",
            string headerTitle = "",
            string headerSubtitle = "")
        {
            // Habilitar componentes básicos
            scaffold.EnableAppBar = true;
            scaffold.EnableDrawer = true;

            // Configurar AppBar
            if (scaffold.AppBar == null)
            {
                /*scaffold.AppBar = new MaterialAppBar
                {
                    Title = appTitle,
                    ShowNavigationIcon = true
                };*/
                scaffold.AppBar.NavigationIconClick += (s, e) => scaffold.ToggleDrawer();
            }

            // Configurar NavigationDrawer
            if (scaffold.NavigationDrawer == null)
            {
                /*scaffold.NavigationDrawer = new MaterialNavigationDrawer
                {
                    HeaderTitle = string.IsNullOrEmpty(headerTitle) ? appTitle : headerTitle,
                    HeaderSubtitle = headerSubtitle
                };*/
            }

            return scaffold.GetNavigator();
        }

        /// <summary>
        /// Configuración completa con páginas en una línea
        /// </summary>
        public static void SetupCompleteNavigation(
            this MaterialScaffold scaffold,
            string appTitle,
            string startPageKey,
            params (string key, string title, Type formType, string category)[] pages)
        {
            var navigator = scaffold.SetupWithNavigation(appTitle);
            navigator.RegisterPages(pages);
            navigator.BuildNavigationDrawer();
            navigator.NavigateTo(startPageKey);
        }
    }
}
