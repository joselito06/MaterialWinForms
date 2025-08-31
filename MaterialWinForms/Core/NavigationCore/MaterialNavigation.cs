using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialWinForms.Core.NavigationCore
{
    /// <summary>
    /// Clase estática para navegación global simple
    /// </summary>
    public class MaterialNavigation
    {
        private static MaterialFormNavigator? _globalNavigator;

        /// <summary>
        /// Configurar navegador global
        /// </summary>
        public static void Initialize(MaterialFormNavigator navigator)
        {
            _globalNavigator = navigator;
        }

        /// <summary>
        /// Navegar globalmente
        /// </summary>
        public static bool NavigateTo(string pageKey)
        {
            if (_globalNavigator == null)
            {
                throw new InvalidOperationException("MaterialNavigation not initialized. Call Initialize() first.");
            }
            return _globalNavigator.NavigateTo(pageKey);
        }

        /// <summary>
        /// Página actual
        /// </summary>
        public static string? CurrentPage => _globalNavigator?.CurrentPageKey;
    }
}

