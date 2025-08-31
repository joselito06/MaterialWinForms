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
    /// Configuración de gradiente
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class GradientSettings
    {
        [DefaultValue(GradientType.None)]
        public GradientType Type { get; set; } = GradientType.None;

        [DefaultValue(typeof(Color), "Blue")]
        public Color StartColor { get; set; } = Color.Blue;

        [DefaultValue(typeof(Color), "LightBlue")]
        public Color EndColor { get; set; } = Color.LightBlue;

        [DefaultValue(GradientDirection.Horizontal)]
        public GradientDirection Direction { get; set; } = GradientDirection.Horizontal;

        [DefaultValue(0f)]
        public float CustomAngle { get; set; } = 0f;

        [DefaultValue(0.5f)]
        public float CenterX { get; set; } = 0.5f;

        [DefaultValue(0.5f)]
        public float CenterY { get; set; } = 0.5f;

        public override string ToString()
        {
            return $"{Type}, {StartColor.Name} -> {EndColor.Name}";
        }
    }
}
