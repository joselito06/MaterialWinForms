using MaterialWinForms.Components.Layout;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaterialWinForms.Core.NavigationCore
{
    /// <summary>
    /// Gestor simple de navegación entre Forms para MaterialScaffold
    /// </summary>
    public class MaterialFormNavigator
    {
        private readonly MaterialScaffold _scaffold;
        private readonly Dictionary<string, FormPageInfo> _registeredPages;
        private readonly Dictionary<string, Form> _formInstances;
        private string? _currentPageKey;

        public event EventHandler<string>? PageChanged;
        public string? CurrentPageKey => _currentPageKey;

        public MaterialFormNavigator(MaterialScaffold scaffold)
        {
            _scaffold = scaffold ?? throw new ArgumentNullException(nameof(scaffold));
            _registeredPages = new Dictionary<string, FormPageInfo>();
            _formInstances = new Dictionary<string, Form>();
        }

        /// <summary>
        /// Registrar un Form como página navegable
        /// </summary>
        public void RegisterPage(string key, string title, Type formType, string category = "")
        {
            var pageInfo = new FormPageInfo
            {
                Key = key,
                Title = title,
                FormType = formType,
                Category = category
            };

            _registeredPages[key] = pageInfo;
        }

        /// <summary>
        /// Registrar múltiples páginas de una vez
        /// </summary>
        public void RegisterPages(params (string key, string title, Type formType, string category)[] pages)
        {
            foreach (var (key, title, formType, category) in pages)
            {
                RegisterPage(key, title, formType, category);
            }
        }

        /// <summary>
        /// Construir el NavigationDrawer automáticamente
        /// </summary>
        public void BuildNavigationDrawer()
        {
            if (_scaffold.NavigationDrawer == null) return;

            //_scaffold.NavigationDrawer.MenuItems.Clear();

            // Agrupar por categoría
            var groupedPages = _registeredPages.Values
                .GroupBy(p => string.IsNullOrEmpty(p.Category) ? "GENERAL" : p.Category)
                .OrderBy(g => g.Key);

            foreach (var group in groupedPages)
            {
                // Agregar header de categoría
                if (group.Key != "GENERAL")
                {
                    //_scaffold.NavigationDrawer.AddMenuHeader(group.Key);
                }

                // Agregar items
                foreach (var page in group.OrderBy(p => p.Title))
                {
                    //_scaffold.NavigationDrawer.AddMenuItem(page.Title, null, (s, e) => NavigateTo(page.Key));
                }

                // Divisor entre categorías (excepto para la última)
                if (group != groupedPages.Last())
                {
                    //_scaffold.NavigationDrawer.AddMenuDivider();
                }
            }
        }

        /// <summary>
        /// Navegar a una página específica
        /// </summary>
        public bool NavigateTo(string pageKey)
        {
            if (!_registeredPages.ContainsKey(pageKey))
            {
                throw new ArgumentException($"Page '{pageKey}' is not registered");
            }

            var pageInfo = _registeredPages[pageKey];

            // Crear Form si no existe
            if (!_formInstances.ContainsKey(pageKey))
            {
                var form = (Form)Activator.CreateInstance(pageInfo.FormType);
                PrepareFormForEmbedding(form);
                _formInstances[pageKey] = form;
            }

            var targetForm = _formInstances[pageKey];

            // Crear panel host
            var hostPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            hostPanel.Controls.Add(targetForm);
            targetForm.Show();

            // Establecer como body
            _scaffold.Body = hostPanel;

            // Actualizar AppBar
            if (_scaffold.AppBar != null)
            {
                _scaffold.AppBar.Title = pageInfo.Title;
            }

            _currentPageKey = pageKey;
            PageChanged?.Invoke(this, pageKey);

            return true;
        }

        /// <summary>
        /// Obtener información de una página registrada
        /// </summary>
        public FormPageInfo? GetPageInfo(string pageKey)
        {
            return _registeredPages.ContainsKey(pageKey) ? _registeredPages[pageKey] : null;
        }

        /// <summary>
        /// Obtener todas las páginas registradas
        /// </summary>
        public IReadOnlyDictionary<string, FormPageInfo> GetAllPages()
        {
            return _registeredPages.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        private void PrepareFormForEmbedding(Form form)
        {
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            form.WindowState = FormWindowState.Normal;
        }

    }
}
