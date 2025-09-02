using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialWinForms.Core.CustomControls
{
    /// <summary>
    /// Clase base para FABs personalizados
    /// </summary>
    public abstract class MaterialFABBase : MaterialControl
    {
        private string _text = "";
        private bool _isExtended = false;

        #region Propiedades esenciales

        [Category("Material - Content")]
        [Description("Texto del FAB (para FAB extendido)")]
        public virtual string Text
        {
            get => _text;
            set
            {
                _text = value ?? "";
                OnTextChanged();
                Invalidate();
            }
        }

        [Category("Material - Behavior")]
        [Description("FAB extendido con texto")]
        [DefaultValue(false)]
        public virtual bool IsExtended
        {
            get => _isExtended;
            set
            {
                _isExtended = value;
                OnIsExtendedChanged();
                Invalidate();
            }
        }

        #endregion

        #region Eventos esenciales

        public new event EventHandler? Click;

        #endregion

        #region Métodos virtuales

        protected virtual void OnTextChanged() { }
        protected virtual void OnIsExtendedChanged() { }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            Click?.Invoke(this, e);
        }

        #endregion

        public MaterialFABBase()
        {
            Size = new Size(56, 56);
        }
    }
}
