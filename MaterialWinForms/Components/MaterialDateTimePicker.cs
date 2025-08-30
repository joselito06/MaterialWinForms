using MaterialWinForms.Components.Containers;
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

namespace MaterialWinForms.Components
{
    public class MaterialDateTimePicker : MaterialControl
    {
        private DateTime _selectedDate = DateTime.Today;
        private string _hintText = "Seleccionar fecha";
        private bool _isCalendarOpen = false;
        private bool _isHovered = false;
        private bool _isFocused = false;
        private MaterialCard? _calendarCard;
        private string _dateFormat = "dd/MM/yyyy";
        private Form? _calendarForm;

        public event EventHandler<DateTime>? DateChanged;

        [Category("Material")]
        [Description("Fecha seleccionada")]
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    DateChanged?.Invoke(this, _selectedDate);
                    Invalidate();
                }
            }
        }

        [Category("Material")]
        [Description("Texto de ayuda")]
        public string HintText
        {
            get => _hintText;
            set { _hintText = value ?? ""; Invalidate(); }
        }

        [Category("Material")]
        [Description("Formato de fecha a mostrar")]
        public string DateFormat
        {
            get => _dateFormat;
            set { _dateFormat = value ?? "dd/MM/yyyy"; Invalidate(); }
        }

        public MaterialDateTimePicker()
        {
            Size = new Size(200, 60);
            Cursor = Cursors.Hand;
            TabStop = true;
        }

        protected override void OnColorSchemeChanged()
        {
            base.OnColorSchemeChanged();
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            _isFocused = true;
            Invalidate();
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            _isFocused = false;
            Invalidate();
            base.OnLostFocus(e);
        }

        protected override void OnClick(EventArgs e)
        {
            if (!_isCalendarOpen)
            {
                ShowCalendar();
            }
            Focus();
            base.OnClick(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Space || keyData == Keys.Enter)
            {
                if (!_isCalendarOpen)
                {
                    ShowCalendar();
                }
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ShowCalendar()
        {
            if (_isCalendarOpen) return;

            _isCalendarOpen = true;

            var calendar = new MonthCalendar
            {
                SelectionStart = _selectedDate,
                SelectionEnd = _selectedDate,
                Font = new Font("Segoe UI", 9F),
                BackColor = ColorScheme.Surface,
                ForeColor = ColorScheme.OnSurface,
                TitleBackColor = ColorScheme.Primary,
                TitleForeColor = ColorScheme.OnPrimary,
                TrailingForeColor = Color.FromArgb(128, ColorScheme.OnSurface)
            };

            _calendarForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                Size = new Size(calendar.Size.Width + 20, calendar.Size.Height + 20),
                TopMost = true,
                ShowInTaskbar = false,
                BackColor = ColorScheme.Surface,
                Padding = new Padding(10)
            };

            // Posicionar el calendario
            var location = PointToScreen(new Point(0, Height + 5));
            var screen = Screen.FromControl(this);

            // Ajustar si se sale de la pantalla
            if (location.X + _calendarForm.Width > screen.WorkingArea.Right)
            {
                location.X = screen.WorkingArea.Right - _calendarForm.Width;
            }
            if (location.Y + _calendarForm.Height > screen.WorkingArea.Bottom)
            {
                location.Y = PointToScreen(Point.Empty).Y - _calendarForm.Height - 5;
            }

            _calendarForm.Location = location;
            _calendarForm.Controls.Add(calendar);
            calendar.Dock = DockStyle.Fill;

            // Eventos
            calendar.DateSelected += (s, e) =>
            {
                SelectedDate = calendar.SelectionStart;
                CloseCalendar();
            };

            _calendarForm.Deactivate += (s, e) => CloseCalendar();

            // Manejar clicks fuera del calendario
            _calendarForm.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Dibujar sombra
                DrawShadow(g, new Rectangle(5, 5, _calendarForm.Width - 10, _calendarForm.Height - 10), Elevation);

                // Dibujar borde redondeado
                using var borderPen = new Pen(Color.FromArgb(50, ColorScheme.OnSurface), 1);
                g.DrawRoundedRectangle(borderPen, new Rectangle(0, 0, _calendarForm.Width - 1, _calendarForm.Height - 1), 8);
            };

            _calendarForm.Show();
        }

        private void CloseCalendar()
        {
            if (_calendarForm != null && !_calendarForm.IsDisposed)
            {
                _calendarForm.Close();
                _calendarForm.Dispose();
                _calendarForm = null;
            }
            _isCalendarOpen = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var bounds = new Rectangle(0, 0, Width, Height);
            var hasDate = _selectedDate != DateTime.MinValue;

            // Dibujar sombra si tiene elevación
            if (Elevation > 0)
            {
                DrawShadow(g, bounds, Elevation);
            }

            // Dibujar fondo
            var backgroundColor = _isHovered
                ? ColorHelper.Lighten(ColorScheme.Surface, 0.05f)
                : ColorScheme.Surface;

            using (var backgroundBrush = new SolidBrush(backgroundColor))
            {
                g.FillRoundedRectangle(backgroundBrush, bounds, 8);
            }

            // Dibujar borde
            var borderColor = _isFocused ? ColorScheme.Primary :
                             _isHovered ? ColorScheme.Primary :
                             Color.FromArgb(128, ColorScheme.OnSurface);
            var borderWidth = _isFocused ? 2 : 1;

            using (var borderPen = new Pen(borderColor, borderWidth))
            {
                var borderBounds = new Rectangle(
                    borderWidth / 2,
                    borderWidth / 2,
                    Width - borderWidth,
                    Height - borderWidth);
                g.DrawRoundedRectangle(borderPen, borderBounds, 8);
            }

            // Dibujar etiqueta flotante
            if (hasDate || _isFocused)
            {
                using var labelFont = new Font("Segoe UI", 8F);
                using var labelBrush = new SolidBrush(_isFocused ? ColorScheme.Primary : ColorScheme.OnSurface);

                // Fondo para la etiqueta
                var labelText = _hintText;
                var labelSize = g.MeasureString(labelText, labelFont);
                var labelRect = new Rectangle(12, 2, (int)labelSize.Width + 4, (int)labelSize.Height);

                using var labelBackBrush = new SolidBrush(backgroundColor);
                g.FillRectangle(labelBackBrush, labelRect);

                g.DrawString(labelText, labelFont, labelBrush, new Point(14, 2));
            }

            // Dibujar fecha o hint
            var textY = (hasDate || _isFocused) ? 22 : (Height - 16) / 2;
            var displayText = hasDate ? _selectedDate.ToString(_dateFormat) :
                            (!_isFocused ? _hintText : "");

            if (!string.IsNullOrEmpty(displayText))
            {
                var textColor = hasDate ? ColorScheme.OnSurface :
                              Color.FromArgb(128, ColorScheme.OnSurface);

                using (var font = new Font("Segoe UI", 10F))
                using (var brush = new SolidBrush(textColor))
                {
                    var textRect = new Rectangle(12, textY, Width - 50, 20);
                    g.DrawString(displayText, font, brush, textRect,
                        new StringFormat { LineAlignment = StringAlignment.Center });
                }
            }

            // Dibujar icono de calendario
            var iconRect = new Rectangle(Width - 35, (Height - 20) / 2, 20, 20);
            using (var iconPen = new Pen(ColorScheme.OnSurface, 1.5f))
            {
                // Cuerpo del calendario
                var calendarBody = new Rectangle(iconRect.X + 2, iconRect.Y + 4, 16, 12);
                g.DrawRoundedRectangle(iconPen, calendarBody, 2);

                // Anillas superiores
                g.DrawLine(iconPen, iconRect.X + 6, iconRect.Y + 4, iconRect.X + 6, iconRect.Y);
                g.DrawLine(iconPen, iconRect.X + 14, iconRect.Y + 4, iconRect.X + 14, iconRect.Y);

                // Línea separadora
                g.DrawLine(iconPen, iconRect.X + 4, iconRect.Y + 8, iconRect.X + 16, iconRect.Y + 8);

                // Puntos para representar días
                using var dotBrush = new SolidBrush(ColorScheme.OnSurface);
                for (int i = 0; i < 6; i++)
                {
                    var dotX = iconRect.X + 6 + (i % 3) * 3;
                    var dotY = iconRect.Y + 10 + (i / 3) * 3;
                    g.FillEllipse(dotBrush, dotX, dotY, 1, 1);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CloseCalendar();
            }
            base.Dispose(disposing);
        }
    }
}
