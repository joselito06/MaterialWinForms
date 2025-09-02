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
    /// Clase base para NavigationDrawers personalizados
    /// </summary>
    public abstract class MaterialNavigationDrawerBase : MaterialControl
    {
        private string _headerTitle = "";
        private string _headerSubtitle = "";
        private bool _isExpanded = true;
        private int _expandedWidth = 280;
        private int _collapsedWidth = 72;

        #region Propiedades esenciales para el Scaffold

        [Category("Material - Header")]
        [Description("Título del header")]
        public virtual string HeaderTitle
        {
            get => _headerTitle;
            set
            {
                _headerTitle = value ?? "";
                OnHeaderTitleChanged();
                Invalidate();
            }
        }

        [Category("Material - Header")]
        [Description("Subtítulo del header")]
        public virtual string HeaderSubtitle
        {
            get => _headerSubtitle;
            set
            {
                _headerSubtitle = value ?? "";
                OnHeaderSubtitleChanged();
                Invalidate();
            }
        }

        [Category("Material - Behavior")]
        [Description("Estado expandido del drawer")]
        [DefaultValue(true)]
        public virtual bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnExpandedStateChanged();
                    Width = _isExpanded ? _expandedWidth : _collapsedWidth;
                    ExpandedStateChanged?.Invoke(this, _isExpanded);
                }
            }
        }

        [Category("Material - Layout")]
        [Description("Ancho cuando está expandido")]
        [DefaultValue(280)]
        public virtual int ExpandedWidth
        {
            get => _expandedWidth;
            set
            {
                _expandedWidth = Math.Max(200, value);
                if (_isExpanded) Width = _expandedWidth;
            }
        }

        [Category("Material - Layout")]
        [Description("Ancho cuando está colapsado")]
        [DefaultValue(72)]
        public virtual int CollapsedWidth
        {
            get => _collapsedWidth;
            set
            {
                _collapsedWidth = Math.Max(48, value);
                if (!_isExpanded) Width = _collapsedWidth;
            }
        }

        #endregion

        #region Eventos esenciales

        /// <summary>
        /// Evento cuando cambia el estado expandido/colapsado
        /// </summary>
        public event EventHandler<bool>? ExpandedStateChanged;

        /// <summary>
        /// Evento cuando se selecciona un item del menú
        /// </summary>
        public event EventHandler<string>? ItemSelected;

        #endregion

        #region Métodos virtuales para personalización

        protected virtual void OnHeaderTitleChanged() { }
        protected virtual void OnHeaderSubtitleChanged() { }
        protected virtual void OnExpandedStateChanged() { }

        /// <summary>
        /// Expandir el drawer - llamar desde tu UserControl
        /// </summary>
        public virtual void Expand()
        {
            IsExpanded = true;
        }

        /// <summary>
        /// Colapsar el drawer - llamar desde tu UserControl
        /// </summary>
        public virtual void Collapse()
        {
            IsExpanded = false;
        }

        /// <summary>
        /// Alternar estado - llamar desde tu UserControl
        /// </summary>
        public virtual void Toggle()
        {
            IsExpanded = !IsExpanded;
        }

        /// <summary>
        /// Disparar selección de item - llamar desde tu UserControl
        /// </summary>
        protected virtual void OnItemSelected(string itemKey)
        {
            ItemSelected?.Invoke(this, itemKey);
        }

        #endregion

        public MaterialNavigationDrawerBase()
        {
            Dock = DockStyle.Left;
            Width = _expandedWidth;
        }
    }
}
