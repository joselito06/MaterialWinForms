using MaterialWinForms.Components.Buttons;
using MaterialWinForms.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialWinForms.Utils;

namespace MaterialWinForms.Components.Containers
{
    /// <summary>
    /// Diálogo Material Design con características completas
    /// </summary>
    public partial class MaterialDialog : Form
    {
        private string _titleText = "";
        private string _messageText = "";
        private string _positiveButtonText = "OK";
        private string _negativeButtonText = "Cancel";
        private string _neutralButtonText = "";
        private Image? _dialogIcon = null;
        private DialogType _dialogType = DialogType.Information;
        private bool _showIcon = true;
        private bool _allowResize = false;
        private CornerRadius _cornerRadius = new CornerRadius(12);
        private ShadowSettings _shadow = new ShadowSettings();
        private Color _overlayColor = Color.FromArgb(120, 0, 0, 0);

        public enum DialogType
        {
            Information,
            Warning,
            Error,
            Success,
            Question,
            Custom
        }

        public enum DialogResult
        {
            None = 0,
            Positive = 1,
            Negative = 2,
            Neutral = 3
        }

        #region Eventos

        public event EventHandler<DialogResult>? DialogResultChanged;

        #endregion

        #region Propiedades

        [Category("Material")]
        [Description("Título del diálogo")]
        public string TitleText
        {
            get => _titleText;
            set { _titleText = value ?? ""; UpdateDialog(); }
        }

        [Category("Material")]
        [Description("Mensaje del diálogo")]
        public string MessageText
        {
            get => _messageText;
            set { _messageText = value ?? ""; UpdateDialog(); }
        }

        [Category("Material")]
        [Description("Texto del botón positivo")]
        public string PositiveButtonText
        {
            get => _positiveButtonText;
            set { _positiveButtonText = value ?? "OK"; UpdateDialog(); }
        }

        [Category("Material")]
        [Description("Texto del botón negativo")]
        public string NegativeButtonText
        {
            get => _negativeButtonText;
            set { _negativeButtonText = value ?? "Cancel"; UpdateDialog(); }
        }

        [Category("Material")]
        [Description("Texto del botón neutral (opcional)")]
        public string NeutralButtonText
        {
            get => _neutralButtonText;
            set { _neutralButtonText = value ?? ""; UpdateDialog(); }
        }

        [Category("Material")]
        [Description("Icono del diálogo")]
        public Image? DialogIcon
        {
            get => _dialogIcon;
            set { _dialogIcon = value; UpdateDialog(); }
        }

        [Category("Material")]
        [Description("Tipo de diálogo")]
        public DialogType Type
        {
            get => _dialogType;
            set { _dialogType = value; UpdateDialogIcon(); UpdateDialog(); }
        }

        [Category("Material")]
        [Description("Mostrar icono del diálogo")]
        public bool ShowIcon
        {
            get => _showIcon;
            set { _showIcon = value; UpdateDialog(); }
        }

        [Category("Material")]
        [Description("Permitir redimensionar el diálogo")]
        public bool AllowResize
        {
            get => _allowResize;
            set
            {
                _allowResize = value;
                FormBorderStyle = value ? System.Windows.Forms.FormBorderStyle.Sizable : System.Windows.Forms.FormBorderStyle.None;
            }
        }

        [Category("Material")]
        [Description("Color de la superposición de fondo")]
        public Color OverlayColor
        {
            get => _overlayColor;
            set { _overlayColor = value; }
        }

        [Category("Material - Appearance")]
        [Description("Configuración de esquinas redondeadas")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CornerRadius CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = value ?? new CornerRadius(12); Invalidate(); }
        }

        [Category("Material - Appearance")]
        [Description("Configuración de sombra")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ShadowSettings Shadow
        {
            get => _shadow;
            set { _shadow = value ?? new ShadowSettings(); Invalidate(); }
        }

        public new DialogResult _DialogResult { get; private set; } = DialogResult.None;

        #endregion

        public MaterialDialog()
        {
            InitializeDialog();
        }

        public MaterialDialog(string title, string message, DialogType type = DialogType.Information)
        {
            _titleText = title;
            _messageText = message;
            _dialogType = type;
            InitializeDialog();
        }

        private void InitializeDialog()
        {
            // Configuración básica del formulario
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            TopMost = true;
            MinimizeBox = false;
            MaximizeBox = false;
            KeyPreview = true;

            // Configuración de Material Design
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw, true);

            BackColor = MaterialThemeManager.CurrentScheme.Surface;
            ForeColor = MaterialThemeManager.CurrentScheme.OnSurface;
            Font = new Font("Segoe UI", 9f);

            // Configurar sombra
            _shadow.Type = MaterialShadowType.Medium;
            _shadow.Opacity = 60;
            _shadow.Blur = 20;
            _shadow.OffsetY = 8;

            // Tamaño inicial
            Size = new Size(400, 200);

            UpdateDialogIcon();
            BuildDialog();

            // Eventos
            KeyDown += MaterialDialog_KeyDown;
            Paint += MaterialDialog_Paint;
        }

        private void UpdateDialogIcon()
        {
            if (_dialogType == DialogType.Custom) return;

            _dialogIcon = CreateDefaultIcon(_dialogType);
        }

        private Image CreateDefaultIcon(DialogType type)
        {
            var bitmap = new Bitmap(32, 32);
            using var g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var iconColor = type switch
            {
                DialogType.Information => Color.FromArgb(33, 150, 243),
                DialogType.Warning => Color.FromArgb(255, 152, 0),
                DialogType.Error => Color.FromArgb(244, 67, 54),
                DialogType.Success => Color.FromArgb(76, 175, 80),
                DialogType.Question => Color.FromArgb(103, 58, 183),
                _ => MaterialThemeManager.CurrentScheme.Primary
            };

            using var brush = new SolidBrush(iconColor);
            using var pen = new Pen(iconColor, 2.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round };

            g.FillEllipse(brush, 2, 2, 28, 28);

            using var whitePen = new Pen(Color.White, 2.5f) { StartCap = LineCap.Round, EndCap = LineCap.Round };

            switch (type)
            {
                case DialogType.Information:
                    g.DrawLine(whitePen, 16, 12, 16, 20);
                    g.FillEllipse(Brushes.White, 14, 8, 4, 4);
                    break;

                case DialogType.Warning:
                    g.DrawLine(whitePen, 16, 10, 16, 18);
                    g.FillEllipse(Brushes.White, 14, 20, 4, 4);
                    break;

                case DialogType.Error:
                    g.DrawLine(whitePen, 10, 10, 22, 22);
                    g.DrawLine(whitePen, 22, 10, 10, 22);
                    break;

                case DialogType.Success:
                    g.DrawLine(whitePen, 10, 16, 14, 20);
                    g.DrawLine(whitePen, 14, 20, 22, 12);
                    break;

                case DialogType.Question:
                    g.DrawArc(whitePen, 12, 8, 8, 8, 180, 180);
                    g.DrawLine(whitePen, 16, 16, 16, 18);
                    g.FillEllipse(Brushes.White, 14, 20, 4, 4);
                    break;
            }

            return bitmap;
        }

        private void BuildDialog()
        {
            SuspendLayout();
            Controls.Clear();

            var padding = 24;
            var currentY = padding;

            // Calcular área de contenido
            var shadowPadding = CalculateShadowPadding();
            var contentWidth = Width - shadowPadding.Horizontal - (padding * 2);
            var contentX = shadowPadding.Left + padding;

            // Título
            if (!string.IsNullOrEmpty(_titleText))
            {
                var titleLabel = new Label
                {
                    Text = _titleText,
                    Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                    ForeColor = MaterialThemeManager.CurrentScheme.OnSurface,
                    AutoSize = false,
                    Size = new Size(contentWidth, 30),
                    Location = new Point(contentX, currentY),
                    TextAlign = ContentAlignment.MiddleLeft
                };

                Controls.Add(titleLabel);
                currentY += 40;
            }

            // Contenedor para icono y mensaje
            var messageContainer = new Panel
            {
                Location = new Point(contentX, currentY),
                Size = new Size(contentWidth, 80),
                BackColor = Color.Transparent
            };

            var messageX = 0;
            var messageWidth = contentWidth;

            // Icono
            if (_showIcon && _dialogIcon != null)
            {
                var iconPicture = new PictureBox
                {
                    Image = _dialogIcon,
                    Size = new Size(32, 32),
                    Location = new Point(0, 0),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BackColor = Color.Transparent
                };

                messageContainer.Controls.Add(iconPicture);
                messageX = 44;
                messageWidth = contentWidth - 44;
            }

            // Mensaje
            if (!string.IsNullOrEmpty(_messageText))
            {
                var messageLabel = new Label
                {
                    Text = _messageText,
                    Font = new Font("Segoe UI", 10f),
                    ForeColor = MaterialThemeManager.CurrentScheme.OnSurface,
                    AutoSize = false,
                    Size = new Size(messageWidth, 60),
                    Location = new Point(messageX, 0),
                    TextAlign = ContentAlignment.TopLeft
                };

                messageContainer.Controls.Add(messageLabel);

                // Ajustar altura del contenedor según el texto
                using var g = CreateGraphics();
                var textSize = g.MeasureString(_messageText, messageLabel.Font, messageWidth);
                var requiredHeight = Math.Max(32, (int)Math.Ceiling(textSize.Height));
                messageContainer.Height = requiredHeight;
            }

            Controls.Add(messageContainer);
            currentY += messageContainer.Height + 24;

            // Botones
            CreateButtons(contentX, currentY, contentWidth);

            // Ajustar tamaño del formulario
            var totalHeight = currentY + 60 + shadowPadding.Vertical;
            Size = new Size(Width, totalHeight);

            ResumeLayout();
        }

        private void CreateButtons(int x, int y, int width)
        {
            var buttonPanel = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, 40),
                BackColor = Color.Transparent
            };

            var buttonWidth = 80;
            var buttonSpacing = 12;
            var buttonsCount = 0;

            // Contar botones activos
            if (!string.IsNullOrEmpty(_positiveButtonText)) buttonsCount++;
            if (!string.IsNullOrEmpty(_negativeButtonText)) buttonsCount++;
            if (!string.IsNullOrEmpty(_neutralButtonText)) buttonsCount++;

            var totalButtonsWidth = (buttonsCount * buttonWidth) + ((buttonsCount - 1) * buttonSpacing);
            var startX = width - totalButtonsWidth;

            // Botón neutral
            if (!string.IsNullOrEmpty(_neutralButtonText))
            {
                var neutralBtn = new MaterialButton
                {
                    Text = _neutralButtonText,
                    Size = new Size(buttonWidth, 32),
                    Location = new Point(startX, 4),
                    Type = MaterialButton.ButtonType.Text
                };
                neutralBtn.Click += (s, e) => CloseDialog(DialogResult.Neutral);
                buttonPanel.Controls.Add(neutralBtn);
                startX += buttonWidth + buttonSpacing;
            }

            // Botón negativo
            if (!string.IsNullOrEmpty(_negativeButtonText))
            {
                var negativeBtn = new MaterialButton
                {
                    Text = _negativeButtonText,
                    Size = new Size(buttonWidth, 32),
                    Location = new Point(startX, 4),
                    Type = MaterialButton.ButtonType.Text
                };
                negativeBtn.Click += (s, e) => CloseDialog(DialogResult.Negative);
                buttonPanel.Controls.Add(negativeBtn);
                startX += buttonWidth + buttonSpacing;
            }

            // Botón positivo
            if (!string.IsNullOrEmpty(_positiveButtonText))
            {
                var positiveBtn = new MaterialButton
                {
                    Text = _positiveButtonText,
                    Size = new Size(buttonWidth, 32),
                    Location = new Point(startX, 4),
                    Type = MaterialButton.ButtonType.Text
                };
                positiveBtn.TextSettings.FontStyle = FontStyle.Bold;
                positiveBtn.Click += (s, e) => CloseDialog(DialogResult.Positive);
                buttonPanel.Controls.Add(positiveBtn);
            }

            Controls.Add(buttonPanel);
        }

        private void UpdateDialog()
        {
            if (!IsHandleCreated) return;
            BuildDialog();
            Invalidate();
        }

        private void CloseDialog(DialogResult result)
        {
            _DialogResult = result;
            DialogResultChanged?.Invoke(this, result);
            Close();
        }

        private Padding CalculateShadowPadding()
        {
            if (_shadow.Type == MaterialShadowType.None) return Padding.Empty;

            var blur = _shadow.Blur;
            var offsetX = Math.Abs(_shadow.OffsetX);
            var offsetY = Math.Abs(_shadow.OffsetY);

            return new Padding(blur + offsetX + 4, blur + offsetY + 4, blur + offsetX + 4, blur + offsetY + 4);
        }

        #region Event Handlers

        private void MaterialDialog_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    CloseDialog(DialogResult.Positive);
                    break;
                case Keys.Escape:
                    CloseDialog(DialogResult.Negative);
                    break;
            }
        }

        private void MaterialDialog_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var shadowPadding = CalculateShadowPadding();
            var bounds = new Rectangle(
                shadowPadding.Left,
                shadowPadding.Top,
                Width - shadowPadding.Horizontal,
                Height - shadowPadding.Vertical
            );

            // Dibujar sombra
            if (_shadow.Type != MaterialShadowType.None)
            {
                g.DrawMaterialShadow(bounds, _cornerRadius, _shadow);
            }

            // Dibujar fondo
            using var brush = new SolidBrush(MaterialThemeManager.CurrentScheme.Surface);
            g.FillRoundedRectangle(brush, bounds, _cornerRadius);
        }

        #endregion

        #region Métodos estáticos para mostrar diálogos

        public static DialogResult Show(string message, string title = "", DialogType type = DialogType.Information)
        {
            using var dialog = new MaterialDialog(title, message, type);
            dialog.ShowDialog();
            return dialog._DialogResult;
        }

        public static DialogResult Show(IWin32Window owner, string message, string title = "", DialogType type = DialogType.Information)
        {
            using var dialog = new MaterialDialog(title, message, type);
            dialog.ShowDialog(owner);
            return dialog._DialogResult;
        }

        public static DialogResult ShowYesNo(string message, string title = "")
        {
            using var dialog = new MaterialDialog(title, message, DialogType.Question)
            {
                PositiveButtonText = "Yes",
                NegativeButtonText = "No"
            };
            dialog.ShowDialog();
            return dialog._DialogResult;
        }

        public static DialogResult ShowYesNoCancel(string message, string title = "")
        {
            using var dialog = new MaterialDialog(title, message, DialogType.Question)
            {
                PositiveButtonText = "Yes",
                NegativeButtonText = "No",
                NeutralButtonText = "Cancel"
            };
            dialog.ShowDialog();
            return dialog._DialogResult;
        }

        public static void ShowError(string message, string title = "Error")
        {
            Show(message, title, DialogType.Error);
        }

        public static void ShowWarning(string message, string title = "Warning")
        {
            Show(message, title, DialogType.Warning);
        }

        public static void ShowSuccess(string message, string title = "Success")
        {
            Show(message, title, DialogType.Success);
        }

        public static void ShowInformation(string message, string title = "Information")
        {
            Show(message, title, DialogType.Information);
        }

        #endregion
    }
}
