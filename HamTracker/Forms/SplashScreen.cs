using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using HamTracker.Forms;

namespace HamTracker.Forms
{
    public class SplashScreen : Form
    {
        private Timer _timer;
        private Timer _dotTimer;
        private int _progress = 0;
        private int _dotCount = 0;
        private Label _lblStatus;
        private Panel _prgFill;

        public SplashScreen()
        {
            // Icon
            if (Program.AppIcon != null)
                this.Icon = Program.AppIcon;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(520, 340);
            this.BackColor = Color.FromArgb(15, 110, 86);
            this.TopMost = true;

            BuildUI();
        }

        private void BuildUI()
        {
            // ── Decorative circles (painted on form) ─────────────
            this.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (var br = new SolidBrush(
                    Color.FromArgb(18, 255, 255, 255)))
                {
                    g.FillEllipse(br, -80, -80, 220, 220);
                    g.FillEllipse(br, 360, 220, 200, 200);
                    g.FillEllipse(br, 380, -50, 120, 120);
                }

                // Border
                using (var pen = new Pen(
                    Color.FromArgb(30, 255, 255, 255), 1))
                    g.DrawRectangle(pen,
                        new Rectangle(0, 0,
                            this.Width - 1, this.Height - 1));
            };

            // ── Shield icon panel ─────────────────────────────────
            var shield = new Panel
            {
                Location = new Point(210, 38),
                Size = new Size(100, 100),
                BackColor = Color.Transparent
            };
            shield.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                // Outer glow circle
                using (var br = new SolidBrush(
                    Color.FromArgb(30, 255, 255, 255)))
                    g.FillEllipse(br, 0, 0, 100, 100);

                // Inner circle
                using (var br = new SolidBrush(
                    Color.FromArgb(50, 255, 255, 255)))
                    g.FillEllipse(br, 12, 12, 76, 76);

                // Shield shape
                using (var pen = new Pen(Color.White, 3f))
                {
                    PointF[] pts =
                    {
                        new PointF(50, 22),
                        new PointF(78, 34),
                        new PointF(78, 56),
                        new PointF(50, 80),
                        new PointF(22, 56),
                        new PointF(22, 34)
                    };
                    g.DrawPolygon(pen, pts);

                    // Checkmark
                    g.DrawLine(pen, 36, 51, 47, 62);
                    g.DrawLine(pen, 47, 62, 66, 40);
                }
            };
            this.Controls.Add(shield);

            // ── App name ──────────────────────────────────────────
            var lblApp = new Label
            {
                Text = "HamTracker",
                Font = new Font("Segoe UI", 28f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(0, 148),
                Size = new Size(520, 44),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblApp);

            // ── Tagline ───────────────────────────────────────────
            var lblTag = new Label
            {
                Text = "Digital Evidence & Proof Management System",
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(200, 255, 255, 255),
                Location = new Point(0, 196),
                Size = new Size(520, 24),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblTag);

            // ── Welcome message ───────────────────────────────────
            string name = LoginForm.CurrentUser != null
                ? "Welcome back, " + LoginForm.CurrentUser.FullName + "!"
                : "Welcome to HamTracker!";

            var lblWelcome = new Label
            {
                Text = name,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 255, 255, 255),
                Location = new Point(0, 224),
                Size = new Size(520, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblWelcome);

            // ── Progress bar background ───────────────────────────
            var prgBg = new Panel
            {
                Location = new Point(80, 270),
                Size = new Size(360, 6),
                BackColor = Color.FromArgb(40, 255, 255, 255)
            };
            MakeRounded(prgBg, 3);

            _prgFill = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(0, 6),
                BackColor = Color.White
            };
            MakeRounded(_prgFill, 3);
            prgBg.Controls.Add(_prgFill);
            this.Controls.Add(prgBg);

            // ── Status label ──────────────────────────────────────
            _lblStatus = new Label
            {
                Text = "Initializing",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(160, 255, 255, 255),
                Location = new Point(0, 285),
                Size = new Size(520, 18),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(_lblStatus);

            // ── Version label ─────────────────────────────────────
            this.Controls.Add(new Label
            {
                Text = "v1.0.0",
                Font = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(100, 255, 255, 255),
                Location = new Point(460, 315),
                AutoSize = true
            });

            // ── Progress timer ────────────────────────────────────
            string[] messages =
            {
                "Initializing",
                "Loading database",
                "Verifying integrity",
                "Setting up workspace",
                "Almost ready"
            };
            int msgIdx = 0;

            _timer = new Timer { Interval = 40 };
            _timer.Tick += (s, e) =>
            {
                _progress += 2;
                if (_progress > 100) _progress = 100;

                // Update fill width
                _prgFill.Width = (int)(360 * _progress / 100.0);

                // Update status message
                int step = _progress / 20;
                if (step < messages.Length && step != msgIdx)
                {
                    msgIdx = step;
                }
                _lblStatus.Text = messages[
                    Math.Min(msgIdx, messages.Length - 1)];

                if (_progress >= 100)
                {
                    _timer.Stop();
                    // Short pause then close
                    var closeTimer = new Timer { Interval = 400 };
                    closeTimer.Tick += (cs, ce) =>
                    {
                        closeTimer.Stop();
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    };
                    closeTimer.Start();
                }
            };

            // ── Dot animation timer ───────────────────────────────
            _dotTimer = new Timer { Interval = 400 };
            _dotTimer.Tick += (s, e) =>
            {
                _dotCount = (_dotCount + 1) % 4;
                string dots = new string('.', _dotCount);
                int step = _progress / 20;
                string msg = messages[
                    Math.Min(step, messages.Length - 1)];
                _lblStatus.Text = msg + dots;
            };
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _timer.Start();
            _dotTimer.Start();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            _timer.Stop();
            _dotTimer.Stop();
        }

        // ── Click to skip ─────────────────────────────────────────
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            _progress = 100;
        }

        private void MakeRounded(Control ctrl, int radius)
        {
            ctrl.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = BuildPath(
                    ((Control)s).ClientRectangle, radius))
                    ((Control)s).Region = new Region(path);
            };
        }

        private GraphicsPath BuildPath(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(r.X, r.Y,
                radius * 2, radius * 2, 180, 90);
            path.AddArc(r.Right - radius * 2, r.Y,
                radius * 2, radius * 2, 270, 90);
            path.AddArc(r.Right - radius * 2, r.Bottom - radius * 2,
                radius * 2, radius * 2, 0, 90);
            path.AddArc(r.X, r.Bottom - radius * 2,
                radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SplashScreen
            // 
            this.ClientSize = new System.Drawing.Size(278, 244);
            this.Name = "SplashScreen";
            this.Load += new System.EventHandler(this.SplashScreen_Load);
            this.ResumeLayout(false);

        }

        private void SplashScreen_Load(object sender, EventArgs e)
        {

        }
    }
}