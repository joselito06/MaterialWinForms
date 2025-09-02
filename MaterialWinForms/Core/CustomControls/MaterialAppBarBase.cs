using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaterialWinForms.Core.CustomControls
{
    /// <summary>
    /// Clase base para AppBars personalizados
    /// </summary>
    public abstract class MaterialAppBarBase : MaterialControl
    {
        private string _title = "";
        private string _subtitle = "";
        private bool _showNavigationIcon = false;
        private bool _centerTitle = false;

        #region Propiedades esenciales para el Scaffold

        [Category("Material - Content")]
        [Description("Título del AppBar")]
        public virtual string Title
        {
            get => _title;
            set
            {
                _title = value ?? "";
                OnTitleChanged();
                Invalidate();
            }
        }

        [Category("Material - Content")]
        [Description("Subtítulo del AppBar")]
        public virtual string Subtitle
        {
            get => _subtitle;
            set
            {
                _subtitle = value ?? "";
                OnSubtitleChanged();
                Invalidate();
            }
        }

        [Category("Material - Behavior")]
        [Description("Mostrar icono de navegación")]
        [DefaultValue(false)]
        public virtual bool ShowNavigationIcon
        {
            get => _showNavigationIcon;
            set
            {
                _showNavigationIcon = value;
                OnShowNavigationIconChanged();
                Invalidate();
            }
        }

        [Category("Material - Behavior")]
        [Description("Centrar título")]
        [DefaultValue(false)]
        public virtual bool CenterTitle
        {
            get => _centerTitle;
            set
            {
                _centerTitle = value;
                OnCenterTitleChanged();
                Invalidate();
            }
        }

        #endregion

        #region Eventos esenciales

        /// <summary>
        /// Evento cuando se hace clic en el icono de navegación
        /// </summary>
        public event EventHandler? NavigationIconClick;

        /// <summary>
        /// Evento cuando se agrega una acción al AppBar
        /// </summary>
        public event EventHandler<string>? ActionClick;

        #endregion

        #region Métodos virtuales para personalización

        /// <summary>
        /// Llamado cuando cambia el título - Override para personalizar
        /// </summary>
        protected virtual void OnTitleChanged() { }

        /// <summary>
        /// Llamado cuando cambia el subtítulo - Override para personalizar
        /// </summary>
        protected virtual void OnSubtitleChanged() { }

        /// <summary>
        /// Llamado cuando cambia ShowNavigationIcon - Override para personalizar
        /// </summary>
        protected virtual void OnShowNavigationIconChanged() { }

        /// <summary>
        /// Llamado cuando cambia CenterTitle - Override para personalizar
        /// </summary>
        protected virtual void OnCenterTitleChanged() { }

        /// <summary>
        /// Disparar evento de clic en navegación - llamar desde tu UserControl
        /// </summary>
        protected virtual void OnNavigationIconClick()
        {
            NavigationIconClick?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Disparar evento de acción - llamar desde tu UserControl
        /// </summary>
        protected virtual void OnActionClick(string actionName)
        {
            ActionClick?.Invoke(this, actionName);
        }

        #endregion

        public MaterialAppBarBase()
        {
            Dock = DockStyle.Top;
            Height = 64;
        }
    }
}
