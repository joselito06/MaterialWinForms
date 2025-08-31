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
    /// Configuración de sombra
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ShadowSettings
    {
        [DefaultValue(MaterialShadowType.Soft)]
        public MaterialShadowType Type { get; set; } = MaterialShadowType.Soft;

        [DefaultValue(typeof(Color), "Black")]
        public Color Color { get; set; } = Color.Black;

        [DefaultValue(50)]
        public int Opacity { get; set; } = 50;

        [DefaultValue(2)]
        public int OffsetX { get; set; } = 2;

        [DefaultValue(2)]
        public int OffsetY { get; set; } = 2;

        [DefaultValue(4)]
        public int Blur { get; set; } = 4;

        [DefaultValue(0)]
        public int Spread { get; set; } = 0;

        public override string ToString()
        {
            return $"{Type}, Offset({OffsetX},{OffsetY}), Blur={Blur}";
        }
    }
}
