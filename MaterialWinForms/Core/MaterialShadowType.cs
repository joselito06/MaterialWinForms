using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialWinForms.Core
{
    /// <summary>
    /// Tipos de sombra Material Design
    /// </summary>
    public enum MaterialShadowType
    {
        None,
        Soft,
        Medium,
        Hard,
        Inset,
        Glow
    }

    /// <summary>
    /// Tipos de gradiente
    /// </summary>
    public enum GradientType
    {
        None,
        Linear,
        Radial,
        Path,
        Diagonal
    }

    /// <summary>
    /// Direcciones para gradiente lineal
    /// </summary>
    public enum GradientDirection
    {
        Horizontal,
        Vertical,
        DiagonalUp,
        DiagonalDown,
        Custom
    }

    /// <summary>
    /// Posiciones de iconos en botones
    /// </summary>
    public enum IconPosition
    {
        Left,
        Right,
        Top,
        Bottom,
        Center
    }
}
