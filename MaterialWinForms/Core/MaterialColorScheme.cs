using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialWinForms.Core
{
    /// <summary>
    /// Esquema de colores Material Design 3.0
    /// </summary>
    public class MaterialColorScheme
    {
        public static readonly MaterialColorScheme Light = new()
        {
            Primary = Color.FromArgb(103, 80, 164),
            PrimaryVariant = Color.FromArgb(63, 81, 181),
            Secondary = Color.FromArgb(255, 87, 34),
            SecondaryVariant = Color.FromArgb(255, 152, 0),
            Background = Color.FromArgb(250, 250, 250),
            Surface = Color.White,
            Error = Color.FromArgb(244, 67, 54),
            OnPrimary = Color.White,
            OnSecondary = Color.White,
            OnBackground = Color.FromArgb(33, 33, 33),
            OnSurface = Color.FromArgb(33, 33, 33),
            OnError = Color.White,
            SecondaryContainer = Color.FromArgb(255, 235, 238),
            OnSecondaryContainer = Color.FromArgb(156, 39, 176)
        };

        public static readonly MaterialColorScheme Dark = new()
        {
            Primary = Color.FromArgb(187, 134, 252),
            PrimaryVariant = Color.FromArgb(124, 77, 255),
            Secondary = Color.FromArgb(3, 218, 198),
            SecondaryVariant = Color.FromArgb(29, 233, 182),
            Background = Color.FromArgb(18, 18, 18),
            Surface = Color.FromArgb(24, 24, 24),
            Error = Color.FromArgb(207, 102, 121),
            OnPrimary = Color.FromArgb(55, 0, 179),
            OnSecondary = Color.FromArgb(0, 54, 61),
            OnBackground = Color.FromArgb(230, 225, 229),
            OnSurface = Color.FromArgb(230, 225, 229),
            OnError = Color.FromArgb(96, 20, 16),
            SecondaryContainer = Color.FromArgb(74, 74, 74),
            OnSecondaryContainer = Color.FromArgb(220, 184, 255)
        };

        // Constructor sin parámetros para el Designer
        public MaterialColorScheme()
        {
            // Inicializar con valores por defecto (Light theme)
            Primary = Color.FromArgb(103, 80, 164);
            PrimaryVariant = Color.FromArgb(63, 81, 181);
            Secondary = Color.FromArgb(255, 87, 34);
            SecondaryVariant = Color.FromArgb(255, 152, 0);
            Background = Color.FromArgb(250, 250, 250);
            Surface = Color.White;
            Error = Color.FromArgb(244, 67, 54);
            OnPrimary = Color.White;
            OnSecondary = Color.White;
            OnBackground = Color.FromArgb(33, 33, 33);
            OnSurface = Color.FromArgb(33, 33, 33);
            OnError = Color.White;
            SecondaryContainer = Color.FromArgb(255, 235, 238);
            OnSecondaryContainer = Color.FromArgb(156, 39, 176);
        }

        public Color Primary { get; init; }
        public Color PrimaryVariant { get; init; }
        public Color Secondary { get; init; }
        public Color SecondaryVariant { get; init; }
        public Color Background { get; init; }
        public Color Surface { get; init; }
        public Color Error { get; init; }
        public Color OnPrimary { get; init; }
        public Color OnSecondary { get; init; }
        public Color OnBackground { get; init; }
        public Color OnSurface { get; init; }
        public Color OnError { get; init; }
        public Color SecondaryContainer { get; set; }
        public Color OnSecondaryContainer { get; set; }
    }
}
