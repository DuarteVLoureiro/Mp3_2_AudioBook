using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mp3_2_AudioBook
{
    internal class DotAnimation
    {
        public static System.Windows.Forms.Timer _dotTimer;
        public static int _dotCount = 0;

        public static void StartDotAnimation(Func<string> text, Label lblStatus)
        {
            _dotCount = 1;
            _dotTimer = new System.Windows.Forms.Timer();
            _dotTimer.Interval = 400;
            _dotTimer.Tick += (s, e) =>
            {
                _dotCount = (_dotCount % 3) + 1;
                lblStatus.Text = text();
            };
            lblStatus.Text = text();
            _dotTimer.Start();
        }
        public static void StopDotAnimation()
        {
            _dotTimer?.Stop();
            _dotTimer?.Dispose();
            _dotTimer = null;
        }
    }
}
