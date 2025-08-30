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
            OnError = Color.White
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
            OnError = Color.FromArgb(96, 20, 16)
        };

        public required Color Primary { get; init; }
        public required Color PrimaryVariant { get; init; }
        public required Color Secondary { get; init; }
        public required Color SecondaryVariant { get; init; }
        public required Color Background { get; init; }
        public required Color Surface { get; init; }
        public required Color Error { get; init; }
        public required Color OnPrimary { get; init; }
        public required Color OnSecondary { get; init; }
        public required Color OnBackground { get; init; }
        public required Color OnSurface { get; init; }
        public required Color OnError { get; init; }
    }
}
