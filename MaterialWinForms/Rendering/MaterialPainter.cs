using MaterialWinForms.Core;
using MaterialWinForms.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaterialWinForms.Rendering
{
    public static class MaterialPainter
    {
        public static Padding CalculateShadowPadding(ShadowSettings shadow)
        {
            if (shadow.Type == MaterialShadowType.None) return Padding.Empty;
            var blur = shadow.Blur;
            var offsetX = Math.Abs(shadow.OffsetX);
            var offsetY = Math.Abs(shadow.OffsetY);
            var spread = Math.Abs(shadow.Spread);

            return new Padding(
                blur + offsetX + spread + 2,
                blur + offsetY + spread + 2,
                blur + offsetX + spread + 2,
                blur + offsetY + spread + 2
            );
        }

        public static void DrawShadow(Graphics g, Rectangle bounds, CornerRadius radius, ShadowSettings shadow)
        {
            if (shadow.Type == MaterialShadowType.None) return;
            g.DrawMaterialShadow(bounds, radius, shadow);
        }
    }
}
