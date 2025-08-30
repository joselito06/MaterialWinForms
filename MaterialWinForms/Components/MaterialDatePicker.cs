using MaterialWinForms.Components.Containers;
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

namespace MaterialWinForms.Components
{
    /// <summary>
    /// DatePicker Material Design
    /// </summary>
    public class MaterialDatePicker : MaterialControl
    {
        private DateTime _selectedDate = DateTime.Today;
        private string _hintText = "Seleccionar fecha";
        private bool _isCalendarOpen = false;
        private bool _isHovered = false;
        private bool _isFocused = false;
        private MaterialCard? _calendarCard;
        private string _dateFormat = "dd/MM/yyyy";

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

        public MaterialDatePicker()
        {
            Size = new Size(200, 60);
            Cursor = Cursors.Hand;
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

        protected override void OnClick(EventArgs e)
        {
            ShowCalendar();
            base.OnClick(e);
        }

        private void ShowCalendar()
        {
            using var calendar = new MonthCalendar
            {
                SelectionStart = _selectedDate,
                SelectionEnd = _selectedDate,
                Font = new Font("Segoe UI", 9F)
            };

            var form = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                Size = calendar.Size,
                TopMost = true,
                ShowInTaskbar = false
            };

            var location = PointToScreen(new Point(0, Height));
            form.Location = location;
            form.Controls.Add(calendar);

            calendar.DateSelected += (s, e) =>
            {
                SelectedDate = calendar.SelectionStart;
                form.Close();
            };

            form.Deactivate += (s, e) => form.Close();
            form.ShowDialog();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var bounds = new Rectangle(0, 0, Width, Height);
            var hasDate = _selectedDate != DateTime.MinValue;

            // Dibujar fondo
            var backgroundColor = _isHovered
                ? Color.FromArgb(10, ColorScheme.OnSurface)
                : ColorScheme.Surface;

            using (var backgroundBrush = new SolidBrush(backgroundColor))
            {
                g.FillRoundedRectangle(backgroundBrush, bounds, 8);
            }

            // Dibujar borde
            var borderColor = _isFocused ? ColorScheme.Primary : ColorScheme.OnSurface;
            using (var borderPen = new Pen(borderColor, _isFocused ? 2 : 1))
            {
                g.DrawRoundedRectangle(borderPen, bounds, 8);
            }

            // Dibujar etiqueta flotante
            if (hasDate)
            {
                using var labelFont = new Font("Segoe UI", 8F);
                using var labelBrush = new SolidBrush(_isFocused ? ColorScheme.Primary : ColorScheme.OnSurface);
                g.DrawString(_hintText, labelFont, labelBrush, new Point(8, 4));
            }

            // Dibujar fecha o hint
            var textY = hasDate ? 22 : (Height - 16) / 2;
            var displayText = hasDate ? _selectedDate.ToString(_dateFormat) : _hintText;
            var textColor = hasDate ? ColorScheme.OnSurface : Color.FromArgb(128, ColorScheme.OnSurface);

            using (var font = new Font("Segoe UI", 10F))
            using (var brush = new SolidBrush(textColor))
            {
                var textRect = new Rectangle(8, textY, Width - 40, 20);
                g.DrawString(displayText, font, brush, textRect);
            }

            // Dibujar icono de calendario
            var iconRect = new Rectangle(Width - 30, (Height - 16) / 2, 16, 16);
            using (var iconPen = new Pen(ColorScheme.OnSurface, 1.5f))
            {
                // Dibujar icono de calendario simple
                g.DrawRectangle(iconPen, iconRect);
                g.DrawLine(iconPen, iconRect.X + 4, iconRect.Y, iconRect.X + 4, iconRect.Y - 3);
                g.DrawLine(iconPen, iconRect.X + 12, iconRect.Y, iconRect.X + 12, iconRect.Y - 3);
                g.DrawLine(iconPen, iconRect.X, iconRect.Y + 4, iconRect.Right, iconRect.Y + 4);
            }
        }
    }
}
