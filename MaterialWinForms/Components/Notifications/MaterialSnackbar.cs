using MaterialWinForms.Components.Buttons;
using MaterialWinForms.Core;
using MaterialWinForms.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MaterialWinForms.Components.Notifications
{
    /// <summary>
    /// Snackbar Material Design para notificaciones
    /// </summary>
    public class MaterialSnackbar : Form
    {
        private MaterialButton? _actionButton;
        private Label? _messageLabel;
        private System.Windows.Forms.Timer? _autoHideTimer;
        private string _message = "";
        private string _actionText = "";
        private Action? _actionCallback;
        private int _duration = 4000; // 4 segundos por defecto

        public enum SnackbarPosition
        {
            BottomLeft,
            BottomCenter,
            BottomRight,
            TopLeft,
            TopCenter,
            TopRight
        }

        [Category("Material")]
        [Description("Mensaje a mostrar")]
        public string Message
        {
            get => _message;
            set
            {
                _message = value ?? "";
                if (_messageLabel != null)
                    _messageLabel.Text = _message;
            }
        }

        [Category("Material")]
        [Description("Texto del botón de acción")]
        public string ActionText
        {
            get => _actionText;
            set
            {
                _actionText = value ?? "";
                SetupActionButton();
            }
        }

        [Category("Material")]
        [Description("Duración en milisegundos antes de ocultarse automáticamente")]
        public int Duration
        {
            get => _duration;
            set { _duration = Math.Max(1000, value); }
        }

        public MaterialSnackbar()
        {
            InitializeComponent();
            SetupComponents();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            TopMost = true;
            ShowInTaskbar = false;
            BackColor = Color.FromArgb(50, 50, 50);
            Size = new Size(400, 60);

            ResumeLayout(false);
        }

        private void SetupComponents()
        {
            _messageLabel = new Label
            {
                Text = _message,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F),
                AutoSize = false,
                Location = new Point(16, 16),
                Size = new Size(280, 28),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Controls.Add(_messageLabel);
            SetupActionButton();

            _autoHideTimer = new System.Windows.Forms.Timer();
            _autoHideTimer.Tick += (s, e) => Hide();
        }

        private void SetupActionButton()
        {
            if (_actionButton != null)
            {
                Controls.Remove(_actionButton);
                _actionButton.Dispose();
                _actionButton = null;
            }

            if (!string.IsNullOrEmpty(_actionText))
            {
                _actionButton = new MaterialButton
                {
                    Text = _actionText.ToUpper(),
                    Type = MaterialButton.ButtonType.Text,
                    Location = new Point(310, 10),
                    Size = new Size(80, 40),
                    ColorScheme = new MaterialColorScheme
                    {
                        Primary = Color.LightBlue,
                        OnPrimary = Color.White,
                        Surface = Color.FromArgb(50, 50, 50),
                        OnSurface = Color.White,
                        Background = Color.FromArgb(50, 50, 50),
                        OnBackground = Color.White,
                        Secondary = Color.LightBlue,
                        OnSecondary = Color.White,
                        Error = Color.Red,
                        OnError = Color.White,
                        PrimaryVariant = Color.Blue,
                        SecondaryVariant = Color.Cyan
                    }
                };

                _actionButton.Click += (s, e) =>
                {
                    _actionCallback?.Invoke();
                    Hide();
                };

                Controls.Add(_actionButton);
            }

            // Ajustar tamaño del mensaje según si hay botón o no
            if (_messageLabel != null)
            {
                _messageLabel.Size = new Size(
                    !string.IsNullOrEmpty(_actionText) ? 280 : 360,
                    28
                );
            }
        }

        public static void Show(string message, int duration = 4000, SnackbarPosition position = SnackbarPosition.BottomCenter)
        {
            Show(message, "", null, duration, position);
        }

        public static void Show(string message, string actionText, Action? actionCallback, int duration = 4000, SnackbarPosition position = SnackbarPosition.BottomCenter)
        {
            var snackbar = new MaterialSnackbar
            {
                Message = message,
                ActionText = actionText,
                _actionCallback = actionCallback,
                Duration = duration
            };

            snackbar.ShowAt(position);
        }

        public void ShowAt(SnackbarPosition position)
        {
            var workingArea = Screen.PrimaryScreen.WorkingArea;
            var location = position switch
            {
                SnackbarPosition.BottomLeft => new Point(20, workingArea.Bottom - Height - 20),
                SnackbarPosition.BottomRight => new Point(workingArea.Right - Width - 20, workingArea.Bottom - Height - 20),
                SnackbarPosition.TopLeft => new Point(20, workingArea.Top + 20),
                SnackbarPosition.TopCenter => new Point((workingArea.Width - Width) / 2, workingArea.Top + 20),
                SnackbarPosition.TopRight => new Point(workingArea.Right - Width - 20, workingArea.Top + 20),
                _ => new Point((workingArea.Width - Width) / 2, workingArea.Bottom - Height - 20) // BottomCenter
            };

            Location = location;
            Show();

            _autoHideTimer!.Interval = Duration;
            _autoHideTimer.Start();

            // Auto-focus para capturar clics fuera
            Focus();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var bounds = new Rectangle(0, 0, Width, Height);

            // Dibujar sombra
            for (int i = 0; i < 4; i++)
            {
                using var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0));
                var shadowRect = new Rectangle(i, i, Width, Height);
                g.FillRoundedRectangle(shadowBrush, shadowRect, 8);
            }

            // Dibujar fondo
            using (var backgroundBrush = new SolidBrush(BackColor))
            {
                g.FillRoundedRectangle(backgroundBrush, bounds, 8);
            }

            base.OnPaint(e);
        }

        protected override void OnDeactivate(EventArgs e)
        {
            // No cerrar automáticamente cuando pierde foco
            // Solo con el timer o acción manual
            base.OnDeactivate(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _autoHideTimer?.Stop();
                _autoHideTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
