using MaterialWinForms.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialWinForms.Animations
{
    public class MaterialRipple
    {
        private int _size;
        private readonly System.Windows.Forms.Timer _timer;
        private Point _location;

        public int Size => _size;
        public Point Location => _location;
        public bool Active => _timer.Enabled;

        public event EventHandler? Invalidated;

        public MaterialRipple()
        {
            _timer = new System.Windows.Forms.Timer { Interval = 10 };
            _timer.Tick += (s, e) => Animate();
        }

        public void Start(Point location)
        {
            _location = location;
            _size = 0;
            _timer.Start();
        }

        private void Animate()
        {
            _size += MaterialTokens.Button.RippleIncrement;
            if (_size > 400) // tokenizable
                _timer.Stop();

            Invalidated?.Invoke(this, EventArgs.Empty);
        }

        public void Draw(Graphics g, Color color)
        {
            if (!Active && _size == 0) return;

            using var brush = new SolidBrush(color);
            var rippleRect = new Rectangle(
                _location.X - _size / 2,
                _location.Y - _size / 2,
                _size, _size
            );
            g.FillEllipse(brush, rippleRect);
        }
    }
}
