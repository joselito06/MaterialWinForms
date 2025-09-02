using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MaterialWinForms.Utils
{
    public static class TimerFactory
    {
        public static System.Windows.Forms.Timer Create(int interval, EventHandler tickHandler)
        {
            var timer = new System.Windows.Forms.Timer
            {
                Interval = interval
            };

            if (tickHandler != null)
                timer.Tick += tickHandler;

            return timer;
        }
    }
}
