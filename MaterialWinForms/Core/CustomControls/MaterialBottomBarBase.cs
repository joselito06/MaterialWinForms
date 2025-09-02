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
    /// Clase base para BottomBars personalizados
    /// </summary>
    public abstract class MaterialBottomBarBase : MaterialControl
    {
        private int _selectedIndex = 0;

        #region Propiedades esenciales

        [Category("Material - Behavior")]
        [Description("Índice del item seleccionado")]
        [DefaultValue(0)]
        public virtual int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (_selectedIndex != value)
                {
                    _selectedIndex = value;
                    OnSelectedIndexChanged();
                    SelectedIndexChanged?.Invoke(this, _selectedIndex);
                }
            }
        }

        #endregion

        #region Eventos esenciales

        public event EventHandler<int>? SelectedIndexChanged;
        public event EventHandler<string>? ItemClick;

        #endregion

        #region Métodos virtuales

        protected virtual void OnSelectedIndexChanged() { }

        protected virtual void OnItemClick(string itemKey)
        {
            ItemClick?.Invoke(this, itemKey);
        }

        #endregion

        public MaterialBottomBarBase()
        {
            Dock = DockStyle.Bottom;
            Height = 56;
        }
    }
}
