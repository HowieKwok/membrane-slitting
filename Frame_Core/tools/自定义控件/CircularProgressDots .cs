using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;

namespace Frame_Core.tools
{

    public class CircularProgressDots : Control
    {
        private readonly Timer _timer;
        private int _angle;

        public int DotCount { get; set; } = 8;
        public int DotRadius { get; set; } = 6;
        public int RingRadius { get; set; } = 18;
        public Color DotColor { get; set; } = Color.FromArgb(0, 120, 215);

        public string LoadingText { get; set; } = "正在加载中...";
        public Font TextFont { get; set; } = new Font("微软雅黑", 10F);

        public CircularProgressDots()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            ForeColor = Color.FromArgb(64, 64, 64);
            Size = new Size(120, 80);   // 给文字留空间
            _timer = new Timer { Interval = 60 };
            _timer.Tick += (_, __) => { _angle = (_angle + 360 / DotCount) % 360; Invalidate(); };
        }

        public void Start()
        {
            _timer.Start();
            Visible = true;
            BringToFront();
            System.Diagnostics.Debug.WriteLine("[Progress] Start");
        }
        public void Stop()
        {
            _timer.Stop();
            Visible = false;
            System.Diagnostics.Debug.WriteLine("[Progress] Stop");
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int cx = Width / 2;
            int cy = (Height - 20) / 2;   // 上移给文字留位

            for (int i = 0; i < DotCount; i++)
            {
                int a = _angle + i * (360 / DotCount);
                int x = cx + (int)(RingRadius * Math.Cos(a * Math.PI / 180));
                int y = cy + (int)(RingRadius * Math.Sin(a * Math.PI / 180));
                int alpha = 255 - (i * 255 / DotCount);
                using (var brush = new SolidBrush(Color.FromArgb(alpha, DotColor)))
                    g.FillEllipse(brush, x - DotRadius, y - DotRadius, DotRadius * 2, DotRadius * 2);
            }

            // 文字居中（底部）
            using (var brush = new SolidBrush(ForeColor))
            {
                var sz = g.MeasureString(LoadingText, TextFont);
                g.DrawString(LoadingText, TextFont, brush,
                    (Width - sz.Width) / 2,
                    Height - sz.Height - 2);
            }
        }
    }
}
