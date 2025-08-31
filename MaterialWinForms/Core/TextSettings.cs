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
    /// Configuración de texto en botones
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TextSettings
    {
        [DefaultValue(ContentAlignment.MiddleCenter)]
        public ContentAlignment Alignment { get; set; } = ContentAlignment.MiddleCenter;

        [DefaultValue(true)]
        public bool AutoCenter { get; set; } = true;

        [DefaultValue(0)]
        public int OffsetX { get; set; } = 0;

        [DefaultValue(0)]
        public int OffsetY { get; set; } = 0;

        [DefaultValue("Segoe UI")]
        public string FontFamily { get; set; } = "Segoe UI";

        [DefaultValue(9f)]
        public float FontSize { get; set; } = 9f;

        [DefaultValue(FontStyle.Regular)]
        public FontStyle FontStyle { get; set; } = FontStyle.Regular;

        [DefaultValue(true)]
        public bool UseEllipsis { get; set; } = true;

        public override string ToString()
        {
            return $"{Alignment}, {FontFamily} {FontSize}pt";
        }
    }
}
