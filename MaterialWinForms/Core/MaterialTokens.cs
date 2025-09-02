using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialWinForms.Core
{
    public static class MaterialTokens
    {
        public static class Button
        {
            public const int DefaultCornerRadius = 20;
            public const int RippleIncrement = 8;
            public const int ElevationMin = 0;
            public const int ElevationMax = 24;
            public static readonly Size DefaultSize = new(140, 40);
        }
    }
}
