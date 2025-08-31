using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialWinForms.Core
{
    /// <summary>
    /// Configuración de icono
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class IconSettings
    {
        [DefaultValue(null)]
        public Image? Icon { get; set; }

        [DefaultValue(IconPosition.Left)]
        public IconPosition Position { get; set; } = IconPosition.Left;

        [DefaultValue(16)]
        public int Size { get; set; } = 16;

        [DefaultValue(8)]
        public int Spacing { get; set; } = 8;

        [DefaultValue(typeof(Color), "Transparent")]
        public Color TintColor { get; set; } = Color.Transparent;

        public override string ToString()
        {
            return Icon != null ? $"{Position}, Size={Size}" : "No Icon";
        }
    }
}
