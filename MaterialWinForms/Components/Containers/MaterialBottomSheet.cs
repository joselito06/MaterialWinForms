using MaterialWinForms.Core;
using Microsoft.VisualBasic.Devices;
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
    /// Bottom Sheet Material Design - Panel deslizante desde la parte inferior
    /// </summary>
    public partial class MaterialBottomSheet : Form
    {
        private string _titleText = "";
        private bool _isDraggable = true;
        private bool _isModal = true;
        private bool _showHandle = true;
        private int _peekHeight = 100;
        private int _maxHeight = 400;
        private BottomSheetState _currentState = BottomSheetState.Collapsed;
        private bool _isDragging = false;
        private Point _dragStartPoint;
        private int _dragStartY;
        private CornerRadius _cornerRadius = new CornerRadius(16, 16, 0, 0);
        private ShadowSettings _shadow = new ShadowSettings();
        private Color _overlayColor = Color.FromArgb(120, 0, 0, 0);
        private Color _handleColor = Color.Gray;
        private Form? _overlayForm;
        private System.Windows.Forms.Timer? _animationTimer;
        private int _targetY;
        private int _animationStep;

        public enum BottomSheetState
        {
            Hidden,
            Collapsed,  // Parcialmente visible
            Expanded    // Completamente visible
        }

        #region Eventos

        public event EventHandler<BottomSheetState>? StateChanged;
        public event EventHandler? DragStarted;
        public event EventHandler? DragEnded;

        #endregion

        #region Propiedades

        [Category("Material")]
        [Description("Título del bottom sheet")]
        public string TitleText
        {
            get => _titleText;
            set { _titleText = value ?? ""; UpdateContent(); }
        }

        [Category("Material")]
        [Description("Permitir arrastrar el bottom sheet")]
        [DefaultValue(true)]
        public bool IsDraggable
        {
            get => _isDraggable;
            set { _isDraggable = value; }
        }

        [Category("Material")]
        [Description("Mostrar como modal (con overlay)")]
        [DefaultValue(true)]
        public bool IsModal
        {
            get => _isModal;
            set { _isModal = value; }
        }

        [Category("Material")]
        [Description("Mostrar handle de arrastre")]
        [DefaultValue(true)]
        public bool ShowHandle
        {
            get => _showHandle;
            set { _showHandle = value; Invalidate(); }
        }

        [Category("Material")]
        [Description("Altura cuando está colapsado")]
        [DefaultValue(100)]
        public int PeekHeight
        {
            get => _peekHeight;
            set { _peekHeight = Math.Max(50, value); }
        }

        [Category("Material")]
        [Description("Altura máxima cuando está expandido")]
        [DefaultValue(400)]
        public int MaxHeight
        {
            get => _maxHeight;
            set { _maxHeight = Math.Max(_peekHeight + 50, value); }
        }

        [Category("Material")]
        [Description("Estado actual del bottom sheet")]
        public BottomSheetState CurrentState
        {
            get => _currentState;
            set { SetState(value, true); }
        }

        [Category("Material")]
        [Description("Color del overlay de fondo")]
        public Color OverlayColor
        {
            get => _overlayColor;
            set { _overlayColor = value; }
        }

        [Category("Material")]
        [Description("Color del handle de arrastre")]
        public Color HandleColor
        {
            get => _handleColor;
            set { _handleColor = value; Invalidate(); }
        }

        [Category("Material - Appearance")]
        [Description("Configuración de esquinas redondeadas")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CornerRadius CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = value ?? new CornerRadius(16, 16, 0, 0); Invalidate(); }
        }

        [Category("Material - Appearance")]
        [Description("Configuración de sombra")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ShadowSettings Shadow
        {
            get => _shadow;
            set { _shadow = value ?? new ShadowSettings(); Invalidate(); }
        }

        #endregion

        public MaterialBottomSheet()
        {
            InitializeBottomSheet();
        }

        public MaterialBottomSheet(string title) : this()
        {
            _titleText = title;
        }

        private void InitializeBottomSheet()
        {
            // Configuración básica del formulario
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            MinimizeBox = false;
            MaximizeBox = false;
            ControlBox = false;

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
            _shadow.Opacity = 40;
            _shadow.Blur = 16;
            _shadow.OffsetY = -8;

            // Configurar animación
            _animationTimer = new System.Windows.Forms.Timer { Interval = 16 }; // ~60fps
            _animationTimer.Tick += AnimationTimer_Tick;

            // Eventos
            MouseDown += MaterialBottomSheet_MouseDown;
            MouseMove += MaterialBottomSheet_MouseMove;
            MouseUp += MaterialBottomSheet_MouseUp;
            Paint += MaterialBottomSheet_Paint;
            Load += MaterialBottomSheet_Load;

            // Tamaño inicial
            Size = new Size(Screen.PrimaryScreen.WorkingArea.Width, _maxHeight);
        }

        private void MaterialBottomSheet_Load(object? sender, EventArgs e)
        {
            if (Owner != null)
            {
                // Posicionar en la parte inferior del formulario padre
                var parentBounds = Owner.Bounds;
                Location = new Point(parentBounds.X, parentBounds.Bottom);
                Width = parentBounds.Width;
            }
            else
            {
                // Posicionar en la parte inferior de la pantalla
                var screen = Screen.PrimaryScreen.WorkingArea;
                Location = new Point(screen.X, screen.Bottom);
                Width = screen.Width;
            }

            if (_isModal)
            {
                CreateOverlay();
            }

            SetState(BottomSheetState.Hidden, false);
        }

        private void CreateOverlay()
        {
            if (_overlayForm != null) return;

            _overlayForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                ShowInTaskbar = false,
                TopMost = false,
                BackColor = Color.Black,
                Opacity = 0,
                WindowState = FormWindowState.Maximized
            };

            _overlayForm.Click += (s, e) => Hide();
            _overlayForm.Show();

            // Animar opacity del overlay
            var overlayTimer = new System.Windows.Forms.Timer { Interval = 16 };
            var targetOpacity = _overlayColor.A / 255.0;
            var currentOpacity = 0.0;
            var step = targetOpacity / 15; // 15 frames para la animación

            overlayTimer.Tick += (s, e) =>
            {
                currentOpacity += step;
                if (currentOpacity >= targetOpacity)
                {
                    currentOpacity = targetOpacity;
                    overlayTimer.Stop();
                    overlayTimer.Dispose();
                }
                _overlayForm.Opacity = currentOpacity;
            };
            overlayTimer.Start();

            BringToFront();
        }

        private void RemoveOverlay()
        {
            if (_overlayForm == null) return;

            _overlayForm.Close();
            _overlayForm.Dispose();
            _overlayForm = null;
        }

        #region State Management

        public void Show()
        {
            Show(Owner);
        }

        public new void Show(IWin32Window? owner)
        {
            if (owner is Form form)
                Owner = form;

            base.Show(owner);
            SetState(BottomSheetState.Collapsed, true);
        }

        public new void Hide()
        {
            SetState(BottomSheetState.Hidden, true);
        }

        public void Expand()
        {
            SetState(BottomSheetState.Expanded, true);
        }

        public void Collapse()
        {
            SetState(BottomSheetState.Collapsed, true);
        }

        private void SetState(BottomSheetState newState, bool animate)
        {
            if (_currentState == newState) return;

            var oldState = _currentState;
            _currentState = newState;

            _targetY = CalculateTargetY(newState);

            if (animate)
            {
                StartAnimation();
            }
            else
            {
                Top = _targetY;
                if (newState == BottomSheetState.Hidden)
                {
                    base.Hide();
                    RemoveOverlay();
                }
            }

            StateChanged?.Invoke(this, newState);
        }

        private int CalculateTargetY(BottomSheetState state)
        {
            var parentBottom = Owner?.Bottom ?? Screen.PrimaryScreen.WorkingArea.Bottom;

            return state switch
            {
                BottomSheetState.Hidden => parentBottom,
                BottomSheetState.Collapsed => parentBottom - _peekHeight,
                BottomSheetState.Expanded => parentBottom - _maxHeight,
                _ => parentBottom
            };
        }

        private void StartAnimation()
        {
            _animationStep = Math.Sign(_targetY - Top) * Math.Max(8, Math.Abs(_targetY - Top) / 20);
            _animationTimer?.Start();
        }

        private void AnimationTimer_Tick(object? sender, EventArgs e)
        {
            var currentY = Top;
            var newY = currentY + _animationStep;

            // Verificar si hemos llegado al objetivo
            if (Math.Sign(_animationStep) > 0 ? newY >= _targetY : newY <= _targetY)
            {
                newY = _targetY;
                _animationTimer?.Stop();

                if (_currentState == BottomSheetState.Hidden)
                {
                    base.Hide();
                    RemoveOverlay();
                }
            }

            Top = newY;
        }

        #endregion

        #region Drag Handling

        private void MaterialBottomSheet_MouseDown(object? sender, MouseEventArgs e)
        {
            if (!_isDraggable || e.Button != MouseButtons.Left) return;

            // Solo permitir drag desde el área del handle
            if (_showHandle && e.Y > 40) return;

            _isDragging = true;
            _dragStartPoint = e.Location;
            _dragStartY = Top;
            Cursor = Cursors.SizeNS;

            DragStarted?.Invoke(this, EventArgs.Empty);
        }

        private void MaterialBottomSheet_MouseMove(object? sender, MouseEventArgs e)
        {
            if (!_isDragging) return;

            var deltaY = e.Y - _dragStartPoint.Y;
            var newY = _dragStartY + deltaY;

            // Limitar el movimiento
            var minY = CalculateTargetY(BottomSheetState.Expanded);
            var maxY = CalculateTargetY(BottomSheetState.Collapsed);

            newY = Math.Max(minY, Math.Min(maxY, newY));
            Top = newY;
        }

        private void MaterialBottomSheet_MouseUp(object? sender, MouseEventArgs e)
        {
            if (!_isDragging) return;

            _isDragging = false;
            Cursor = Cursors.Default;

            // Determinar el estado final basado en la posición y velocidad
            var currentY = Top;
            var collapsedY = CalculateTargetY(BottomSheetState.Collapsed);
            var expandedY = CalculateTargetY(BottomSheetState.Expanded);
            var midPoint = expandedY + (collapsedY - expandedY) / 2;

            var targetState = currentY < midPoint ? BottomSheetState.Expanded : BottomSheetState.Collapsed;

            SetState(targetState, true);
            DragEnded?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Painting

        private void MaterialBottomSheet_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var bounds = new Rectangle(0, 0, Width, Height);

            // Dibujar sombra
            if (_shadow.Type != MaterialShadowType.None)
            {
                GraphicsExtensions.DrawMaterialShadow(g, bounds, _cornerRadius, _shadow);
            }

            // Dibujar fondo
            using var brush = new SolidBrush(MaterialThemeManager.CurrentScheme.Surface);
            g.FillRoundedRectangle(brush, bounds, _cornerRadius);

            // Dibujar handle si está habilitado
            if (_showHandle)
            {
                DrawHandle(g);
            }

            // Dibujar título si existe
            if (!string.IsNullOrEmpty(_titleText))
            {
                DrawTitle(g);
            }
        }

        private void DrawHandle(Graphics g)
        {
            var handleWidth = 40;
            var handleHeight = 4;
            var handleX = (Width - handleWidth) / 2;
            var handleY = 8;

            using var handleBrush = new SolidBrush(_handleColor);
            var handleRect = new Rectangle(handleX, handleY, handleWidth, handleHeight);
            g.FillRoundedRectangle(handleBrush, handleRect, 2);
        }

        private void DrawTitle(Graphics g)
        {
            var titleY = _showHandle ? 24 : 16;
            var titleRect = new Rectangle(16, titleY, Width - 32, 24);

            using var font = new Font("Segoe UI", 12f, FontStyle.Bold);
            using var brush = new SolidBrush(MaterialThemeManager.CurrentScheme.OnSurface);
            using var sf = new StringFormat { LineAlignment = StringAlignment.Center };

            g.DrawString(_titleText, font, brush, titleRect, sf);
        }

        #endregion

        #region Content Management

        private void UpdateContent()
        {
            Invalidate();
        }

        /// <summary>
        /// Agregar un control al bottom sheet
        /// </summary>
        public void AddContent(Control control)
        {
            var contentY = 16;
            if (_showHandle) contentY += 16;
            if (!string.IsNullOrEmpty(_titleText)) contentY += 24;

            control.Location = new Point(control.Location.X, contentY + control.Location.Y);
            Controls.Add(control);
        }

        /// <summary>
        /// Obtener el área disponible para contenido
        /// </summary>
        public Rectangle GetContentArea()
        {
            var contentY = 16;
            if (_showHandle) contentY += 16;
            if (!string.IsNullOrEmpty(_titleText)) contentY += 24;

            return new Rectangle(16, contentY, Width - 32, Height - contentY - 16);
        }

        #endregion

        #region Métodos estáticos

        public static MaterialBottomSheet Show(Form owner, string title = "")
        {
            var bottomSheet = new MaterialBottomSheet(title);
            bottomSheet.Show(owner);
            return bottomSheet;
        }

        public static MaterialBottomSheet Show(Form owner, Control content, string title = "")
        {
            var bottomSheet = new MaterialBottomSheet(title);
            bottomSheet.AddContent(content);
            bottomSheet.Show(owner);
            return bottomSheet;
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _animationTimer?.Stop();
                _animationTimer?.Dispose();
                RemoveOverlay();
            }
            base.Dispose(disposing);
        }
    }
}
