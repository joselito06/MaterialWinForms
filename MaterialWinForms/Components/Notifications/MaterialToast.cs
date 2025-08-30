using MaterialWinForms.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaterialWinForms.Components.Notifications
{
    /// <summary>
    /// Sistema de notificaciones Toast
    /// </summary>
    public static class MaterialToast
    {
        public enum ToastType
        {
            Info,
            Success,
            Warning,
            Error
        }

        public static void Show(string message, ToastType type = ToastType.Info, int duration = 3000)
        {
            var (backgroundColor, textColor, icon) = GetToastStyle(type);

            var toast = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                TopMost = true,
                ShowInTaskbar = false,
                BackColor = backgroundColor,
                Size = new Size(350, 80)
            };

            var iconLabel = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = textColor,
                Location = new Point(20, 20),
                Size = new Size(30, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var messageLabel = new Label
            {
                Text = message,
                Font = new Font("Segoe UI", 10F),
                ForeColor = textColor,
                Location = new Point(60, 20),
                Size = new Size(270, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            toast.Controls.Add(iconLabel);
            toast.Controls.Add(messageLabel);

            // Posicionar en la esquina superior derecha
            var workingArea = Screen.PrimaryScreen.WorkingArea;
            toast.Location = new Point(
                workingArea.Right - toast.Width - 20,
                workingArea.Top + 20
            );

            toast.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var bounds = new Rectangle(0, 0, toast.Width, toast.Height);

                // Sombra
                for (int i = 0; i < 3; i++)
                {
                    using var shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0));
                    var shadowRect = new Rectangle(i, i, toast.Width, toast.Height);
                    g.FillRoundedRectangle(shadowBrush, shadowRect, 12);
                }

                // Fondo
                using (var backgroundBrush = new SolidBrush(backgroundColor))
                {
                    g.FillRoundedRectangle(backgroundBrush, bounds, 12);
                }
            };

            toast.Show();

            // Auto-hide timer
            var timer = new System.Windows.Forms.Timer { Interval = duration };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                timer.Dispose();
                toast.Close();
                toast.Dispose();
            };
            timer.Start();

            // Cerrar al hacer clic
            toast.Click += (s, e) =>
            {
                timer.Stop();
                timer.Dispose();
                toast.Close();
                toast.Dispose();
            };
        }

        private static (Color backgroundColor, Color textColor, string icon) GetToastStyle(ToastType type)
        {
            return type switch
            {
                ToastType.Success => (Color.FromArgb(76, 175, 80), Color.White, "✓"),
                ToastType.Warning => (Color.FromArgb(255, 152, 0), Color.White, "⚠"),
                ToastType.Error => (Color.FromArgb(244, 67, 54), Color.White, "✕"),
                _ => (Color.FromArgb(33, 150, 243), Color.White, "ℹ")
            };
        }
    }
}
