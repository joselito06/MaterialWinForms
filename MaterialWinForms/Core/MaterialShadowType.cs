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

    /// <summary>
    /// Estilos de NavigationDrawer
    /// </summary>
    public enum NavigationDrawerStyle
    {
        Standard,       // Drawer normal junto al contenido
        Modal,          // Drawer que se superpone con overlay
        OverAppBar,     // Drawer que se superpone sobre el AppBar
        Floating,       // Drawer flotante con esquinas redondeadas
        Mini,           // Drawer mini que se expande al hover
        Push            // Drawer que empuja el contenido
    }

    /// <summary>
    /// Estilos de AppBar
    /// </summary>
    public enum AppBarStyle
    {
        Standard,       // AppBar normal fijo arriba
        Floating,       // AppBar flotante con esquinas redondeadas
        Transparent,    // AppBar transparente
        Elevated,       // AppBar con sombra prominente
        Collapsing,     // AppBar que colapsa al hacer scroll
        Dense           // AppBar compacto
    }

    /// <summary>
    /// Estilos de Header del NavigationDrawer
    /// </summary>
    public enum DrawerHeaderStyle
    {
        Standard,       // Header normal con texto
        Compact,        // Header compacto
        Image,          // Header con imagen de fondo
        Gradient,       // Header con gradiente
        Card,           // Header estilo tarjeta
        None            // Sin header
    }
}
