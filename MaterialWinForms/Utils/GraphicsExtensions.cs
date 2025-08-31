using MaterialWinForms.Core;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialWinForms.Utils
{
    /// <summary>
    /// Extensiones mejoradas para Graphics con soporte completo Material Design
    /// </summary>
    public static class GraphicsExtensions
    {
        #region Formas Redondeadas

        public static void FillRoundedRectangle(this Graphics g, Brush brush, Rectangle rect, CornerRadius corners)
        {
            using var path = CreateRoundedRectanglePath(rect, corners);
            g.FillPath(brush, path);
        }

        public static void FillRoundedRectangle(this Graphics g, Brush brush, Rectangle rect, int cornerRadius)
        {
            using var path = CreateRoundedRectanglePath(rect, new CornerRadius(cornerRadius));
            g.FillPath(brush, path);
        }

        public static void DrawRoundedRectangle(this Graphics g, Pen pen, Rectangle rect, CornerRadius corners)
        {
            using var path = CreateRoundedRectanglePath(rect, corners);
            g.DrawPath(pen, path);
        }

        public static void DrawRoundedRectangle(this Graphics g, Pen pen, Rectangle rect, int cornerRadius)
        {
            using var path = CreateRoundedRectanglePath(rect, new CornerRadius(cornerRadius));
            g.DrawPath(pen, path);
        }

        public static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, CornerRadius corners)
        {
            var path = new GraphicsPath();

            if (corners.All <= 0 && !corners.UseIndividualCorners)
            {
                path.AddRectangle(rect);
                return path;
            }

            var topLeft = corners.UseIndividualCorners ? corners.TopLeft : corners.All;
            var topRight = corners.UseIndividualCorners ? corners.TopRight : corners.All;
            var bottomLeft = corners.UseIndividualCorners ? corners.BottomLeft : corners.All;
            var bottomRight = corners.UseIndividualCorners ? corners.BottomRight : corners.All;

            // Ajustar para que no excedan las dimensiones del rectángulo
            var maxCorner = Math.Min(rect.Width, rect.Height) / 2;
            topLeft = Math.Min(topLeft, maxCorner);
            topRight = Math.Min(topRight, maxCorner);
            bottomLeft = Math.Min(bottomLeft, maxCorner);
            bottomRight = Math.Min(bottomRight, maxCorner);

            // Crear el path
            if (topLeft > 0)
                path.AddArc(rect.X, rect.Y, topLeft * 2, topLeft * 2, 180, 90);
            else
                path.AddLine(rect.X, rect.Y, rect.X, rect.Y);

            if (topRight > 0)
                path.AddArc(rect.Right - topRight * 2, rect.Y, topRight * 2, topRight * 2, 270, 90);
            else
                path.AddLine(rect.Right, rect.Y, rect.Right, rect.Y);

            if (bottomRight > 0)
                path.AddArc(rect.Right - bottomRight * 2, rect.Bottom - bottomRight * 2, bottomRight * 2, bottomRight * 2, 0, 90);
            else
                path.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Bottom);

            if (bottomLeft > 0)
                path.AddArc(rect.X, rect.Bottom - bottomLeft * 2, bottomLeft * 2, bottomLeft * 2, 90, 90);
            else
                path.AddLine(rect.X, rect.Bottom, rect.X, rect.Bottom);

            path.CloseFigure();
            return path;
        }

        #endregion

        #region Gradientes

        public static void FillRoundedRectangleWithGradient(this Graphics g, Rectangle rect, CornerRadius corners, GradientSettings gradient)
        {
            if (gradient.Type == GradientType.None) return;

            using var brush = CreateGradientBrush(rect, gradient);
            if (brush != null)
            {
                g.FillRoundedRectangle(brush, rect, corners);
            }
        }

        public static Brush? CreateGradientBrush(Rectangle rect, GradientSettings gradient)
        {
            return gradient.Type switch
            {
                GradientType.Linear => CreateLinearGradientBrush(rect, gradient),
                GradientType.Radial => CreateRadialGradientBrush(rect, gradient),
                GradientType.Path => CreatePathGradientBrush(rect, gradient),
                GradientType.Diagonal => CreateDiagonalGradientBrush(rect, gradient),
                _ => null
            };
        }

        private static LinearGradientBrush CreateLinearGradientBrush(Rectangle rect, GradientSettings gradient)
        {
            var (startPoint, endPoint) = GetGradientPoints(rect, gradient.Direction, gradient.CustomAngle);
            return new LinearGradientBrush(startPoint, endPoint, gradient.StartColor, gradient.EndColor);
        }

        private static PathGradientBrush CreateRadialGradientBrush(Rectangle rect, GradientSettings gradient)
        {
            var centerX = rect.X + rect.Width * gradient.CenterX;
            var centerY = rect.Y + rect.Height * gradient.CenterY;
            var center = new PointF(centerX, centerY);

            var path = new GraphicsPath();
            path.AddEllipse(rect);

            var brush = new PathGradientBrush(path)
            {
                CenterPoint = center,
                CenterColor = gradient.StartColor,
                SurroundColors = new[] { gradient.EndColor }
            };

            return brush;
        }

        private static PathGradientBrush CreatePathGradientBrush(Rectangle rect, GradientSettings gradient)
        {
            var path = new GraphicsPath();
            path.AddRectangle(rect);

            var brush = new PathGradientBrush(path)
            {
                CenterColor = gradient.StartColor,
                SurroundColors = new[] { gradient.EndColor }
            };

            return brush;
        }

        private static LinearGradientBrush CreateDiagonalGradientBrush(Rectangle rect, GradientSettings gradient)
        {
            var startPoint = new Point(rect.X, rect.Y);
            var endPoint = new Point(rect.Right, rect.Bottom);
            return new LinearGradientBrush(startPoint, endPoint, gradient.StartColor, gradient.EndColor);
        }

        private static (PointF startPoint, PointF endPoint) GetGradientPoints(Rectangle rect, GradientDirection direction, float customAngle)
        {
            return direction switch
            {
                GradientDirection.Horizontal => (new PointF(rect.X, rect.Y), new PointF(rect.Right, rect.Y)),
                GradientDirection.Vertical => (new PointF(rect.X, rect.Y), new PointF(rect.X, rect.Bottom)),
                GradientDirection.DiagonalUp => (new PointF(rect.X, rect.Bottom), new PointF(rect.Right, rect.Y)),
                GradientDirection.DiagonalDown => (new PointF(rect.X, rect.Y), new PointF(rect.Right, rect.Bottom)),
                GradientDirection.Custom => GetCustomAnglePoints(rect, customAngle),
                _ => (new PointF(rect.X, rect.Y), new PointF(rect.Right, rect.Y))
            };
        }

        private static (PointF startPoint, PointF endPoint) GetCustomAnglePoints(Rectangle rect, float angle)
        {
            var radians = angle * Math.PI / 180;
            var centerX = rect.X + rect.Width / 2f;
            var centerY = rect.Y + rect.Height / 2f;
            var distance = Math.Max(rect.Width, rect.Height) / 2f;

            var startX = centerX - (float)(Math.Cos(radians) * distance);
            var startY = centerY - (float)(Math.Sin(radians) * distance);
            var endX = centerX + (float)(Math.Cos(radians) * distance);
            var endY = centerY + (float)(Math.Sin(radians) * distance);

            return (new PointF(startX, startY), new PointF(endX, endY));
        }

        #endregion

        #region Sombras

        public static void DrawMaterialShadow(this Graphics g, Rectangle rect, CornerRadius corners, ShadowSettings shadow)
        {
            if (shadow.Type == MaterialShadowType.None) return;

            var originalSmoothingMode = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            switch (shadow.Type)
            {
                case MaterialShadowType.Soft:
                    DrawSoftShadow(g, rect, corners, shadow);
                    break;
                case MaterialShadowType.Medium:
                    DrawMediumShadow(g, rect, corners, shadow);
                    break;
                case MaterialShadowType.Hard:
                    DrawHardShadow(g, rect, corners, shadow);
                    break;
                case MaterialShadowType.Inset:
                    DrawInsetShadow(g, rect, corners, shadow);
                    break;
                case MaterialShadowType.Glow:
                    DrawGlowShadow(g, rect, corners, shadow);
                    break;
            }

            g.SmoothingMode = originalSmoothingMode;
        }

        private static void DrawSoftShadow(Graphics g, Rectangle rect, CornerRadius corners, ShadowSettings shadow)
        {
            var blur = Math.Max(1, shadow.Blur);
            for (int i = 0; i < blur; i++)
            {
                var alpha = Math.Max(5, shadow.Opacity - (i * shadow.Opacity / blur));
                var shadowColor = Color.FromArgb(alpha, shadow.Color);

                var shadowRect = new Rectangle(
                    rect.X + shadow.OffsetX - i + shadow.Spread,
                    rect.Y + shadow.OffsetY - i + shadow.Spread,
                    rect.Width + i * 2 - shadow.Spread * 2,
                    rect.Height + i * 2 - shadow.Spread * 2
                );

                using var brush = new SolidBrush(shadowColor);
                g.FillRoundedRectangle(brush, shadowRect, corners);
            }
        }

        private static void DrawMediumShadow(Graphics g, Rectangle rect, CornerRadius corners, ShadowSettings shadow)
        {
            var shadowColor = Color.FromArgb(shadow.Opacity, shadow.Color);
            var shadowRect = new Rectangle(
                rect.X + shadow.OffsetX,
                rect.Y + shadow.OffsetY,
                rect.Width,
                rect.Height
            );

            // Sombra principal
            using var brush = new SolidBrush(shadowColor);
            g.FillRoundedRectangle(brush, shadowRect, corners);

            // Difuminado
            var blur = shadow.Blur / 2;
            for (int i = 1; i <= blur; i++)
            {
                var alpha = shadow.Opacity * (blur - i) / blur;
                var blurColor = Color.FromArgb(Math.Max(10, alpha), shadow.Color);
                var blurRect = new Rectangle(
                    shadowRect.X - i,
                    shadowRect.Y - i,
                    shadowRect.Width + i * 2,
                    shadowRect.Height + i * 2
                );

                using var blurBrush = new SolidBrush(blurColor);
                g.FillRoundedRectangle(blurBrush, blurRect, corners);
            }
        }

        private static void DrawHardShadow(Graphics g, Rectangle rect, CornerRadius corners, ShadowSettings shadow)
        {
            var shadowColor = Color.FromArgb(shadow.Opacity, shadow.Color);
            var shadowRect = new Rectangle(
                rect.X + shadow.OffsetX,
                rect.Y + shadow.OffsetY,
                rect.Width,
                rect.Height
            );

            using var brush = new SolidBrush(shadowColor);
            g.FillRoundedRectangle(brush, shadowRect, corners);
        }

        private static void DrawInsetShadow(Graphics g, Rectangle rect, CornerRadius corners, ShadowSettings shadow)
        {
            var shadowColor = Color.FromArgb(shadow.Opacity, shadow.Color);
            var insetRect = new Rectangle(
                rect.X + shadow.Blur,
                rect.Y + shadow.Blur,
                rect.Width - shadow.Blur * 2,
                rect.Height - shadow.Blur * 2
            );

            using var brush = new SolidBrush(shadowColor);
            using var outerPath = CreateRoundedRectanglePath(rect, corners);
            using var innerPath = CreateRoundedRectanglePath(insetRect, corners);

            using var region = new Region(outerPath);
            region.Exclude(innerPath);

            var oldClip = g.Clip;
            g.Clip = region;
            g.FillRoundedRectangle(brush, rect, corners);
            g.Clip = oldClip;
        }

        private static void DrawGlowShadow(Graphics g, Rectangle rect, CornerRadius corners, ShadowSettings shadow)
        {
            var glowSize = shadow.Blur;
            for (int i = 0; i < glowSize; i++)
            {
                var alpha = shadow.Opacity * (glowSize - i) / glowSize;
                var glowColor = Color.FromArgb(Math.Max(5, alpha), shadow.Color);

                var glowRect = new Rectangle(
                    rect.X - i,
                    rect.Y - i,
                    rect.Width + i * 2,
                    rect.Height + i * 2
                );

                using var brush = new SolidBrush(glowColor);
                g.FillRoundedRectangle(brush, glowRect, corners);
            }
        }

        #endregion

        #region Iconos

        public static void DrawIcon(this Graphics g, Image icon, Rectangle bounds, IconSettings settings, Color? tintColor = null)
        {
            if (icon == null) return;

            var iconSize = new Size(settings.Size, settings.Size);
            var iconRect = CalculateIconRect(bounds, iconSize, settings.Position, settings.Spacing);

            // Aplicar tinte si es necesario
            if (settings.TintColor != Color.Transparent || tintColor.HasValue)
            {
                var tint = tintColor ?? settings.TintColor;
                using var tintedIcon = ApplyTint(icon, tint);
                g.DrawImage(tintedIcon, iconRect);
            }
            else
            {
                g.DrawImage(icon, iconRect);
            }
        }

        private static Rectangle CalculateIconRect(Rectangle bounds, Size iconSize, IconPosition position, int spacing)
        {
            return position switch
            {
                IconPosition.Left => new Rectangle(
                    bounds.X + spacing,
                    bounds.Y + (bounds.Height - iconSize.Height) / 2,
                    iconSize.Width,
                    iconSize.Height),

                IconPosition.Right => new Rectangle(
                    bounds.Right - iconSize.Width - spacing,
                    bounds.Y + (bounds.Height - iconSize.Height) / 2,
                    iconSize.Width,
                    iconSize.Height),

                IconPosition.Top => new Rectangle(
                    bounds.X + (bounds.Width - iconSize.Width) / 2,
                    bounds.Y + spacing,
                    iconSize.Width,
                    iconSize.Height),

                IconPosition.Bottom => new Rectangle(
                    bounds.X + (bounds.Width - iconSize.Width) / 2,
                    bounds.Bottom - iconSize.Height - spacing,
                    iconSize.Width,
                    iconSize.Height),

                IconPosition.Center => new Rectangle(
                    bounds.X + (bounds.Width - iconSize.Width) / 2,
                    bounds.Y + (bounds.Height - iconSize.Height) / 2,
                    iconSize.Width,
                    iconSize.Height),

                _ => new Rectangle(bounds.Location, iconSize)
            };
        }

        private static Bitmap ApplyTint(Image originalImage, Color tintColor)
        {
            var bitmap = new Bitmap(originalImage.Width, originalImage.Height);

            using var g = Graphics.FromImage(bitmap);

            // Crear ColorMatrix para aplicar tinte
            var colorMatrix = new ColorMatrix(new float[][]
            {
                new float[] {tintColor.R/255f, 0, 0, 0, 0},
                new float[] {0, tintColor.G/255f, 0, 0, 0},
                new float[] {0, 0, tintColor.B/255f, 0, 0},
                //new float[] {0, 0, 0, tintColor.A/255f, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {0, 0, 0, 0, 1}
            });

            var imageAttributes = new System.Drawing.Imaging.ImageAttributes();
            imageAttributes.SetColorMatrix(colorMatrix);

            g.DrawImage(originalImage,
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                0, 0, originalImage.Width, originalImage.Height,
                GraphicsUnit.Pixel, imageAttributes);

            return bitmap;
        }

        public static Bitmap ApplyTint(this Graphics g, Image originalImage, Color tintColor)
        {
            return ApplyTint(originalImage, tintColor);
        }

        #endregion
    }
}
