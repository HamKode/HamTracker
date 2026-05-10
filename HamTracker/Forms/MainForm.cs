using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using HamTracker.Database;

namespace HamTracker.Forms
{
    public class MainForm : Form
    {
        private Panel pnlSidebar, pnlContent, pnlTop;
        private Label lblCurrentPage, lblBreadcrumb;
        private Button btnDashboard, btnProjects, btnTasks,
                       btnEvidence, btnTimeline, btnReports;

        private ProjectRepository _projectRepo = new ProjectRepository();
        private TaskRepository _taskRepo = new TaskRepository();
        private EvidenceRepository _evidenceRepo = new EvidenceRepository();
        private AuditLogRepository _auditRepo = new AuditLogRepository();

        public MainForm()
        {
            InitializeLayout();
            ShowDashboard();
        }

        private void InitializeLayout()
        {
            // Icon
            if (Program.AppIcon != null)
                this.Icon = Program.AppIcon;

            this.Text = "HamTracker — Proof Management System";
            this.Size = new Size(1200, 720);
            this.MinimumSize = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(246, 246, 243);
            this.Font = new Font("Segoe UI", 9.5f);

            // ── Sidebar ──────────────────────────────────────────
            pnlSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 230,
                BackColor = Color.White
            };
            pnlSidebar.Paint += PaintSidebarBorder;

            // ── Logo area ─────────────────────────────────────────
            var pnlLogo = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(230, 70),
                BackColor = Color.White
            };

            var picIcon = new Panel
            {
                Location = new Point(18, 18),
                Size = new Size(34, 34),
                BackColor = Color.FromArgb(29, 158, 117)
            };
            picIcon.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var pen = new Pen(Color.White, 2.5f))
                {
                    e.Graphics.DrawEllipse(pen, 6, 6, 20, 20);
                    e.Graphics.DrawLine(pen, 13, 17, 16, 20);
                    e.Graphics.DrawLine(pen, 16, 20, 22, 13);
                }
            };
            MakeRounded(picIcon, 8);

            var lblLogo = new Label
            {
                Text = "HamTracker",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 110, 86),
                Location = new Point(60, 14),
                AutoSize = true
            };
            var lblSub = new Label
            {
                Text = "Proof Management System",
                Font = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(160, 160, 160),
                Location = new Point(61, 37),
                AutoSize = true
            };
            pnlLogo.Controls.AddRange(new Control[] { picIcon, lblLogo, lblSub });

            var divLogo = new Panel
            {
                Location = new Point(0, 69),
                Size = new Size(230, 1),
                BackColor = Color.FromArgb(240, 240, 240)
            };

            // ── Nav buttons ───────────────────────────────────────
            var lblMain = MakeNavSection("MAIN", 86);

            // Icon chars from Segoe MDL2 Assets font
            btnDashboard = MakeSidebarButton("Dashboard", "\uE80F", 108);
            btnProjects = MakeSidebarButton("Projects", "\uE7C3", 148);
            btnTasks = MakeSidebarButton("Tasks", "\uE9D5", 188);
            btnEvidence = MakeSidebarButton("Evidence", "\uEB9F", 228);

            var lblReportsSection = MakeNavSection("REPORTS", 274);
            btnTimeline = MakeSidebarButton("Timeline", "\uE81C", 294);
            btnReports = MakeSidebarButton("Reports", "\uE9F9", 334);

            btnDashboard.Click += (s, e) => ShowDashboard();
            btnProjects.Click += (s, e) => ShowProjects();
            btnTasks.Click += (s, e) => ShowTasks();
            btnEvidence.Click += (s, e) => ShowEvidence();
            btnTimeline.Click += (s, e) => ShowTimeline();
            btnReports.Click += (s, e) => ShowReports();

            // ── User bar ──────────────────────────────────────────
            var pnlUser = new Panel
            {
                Location = new Point(0, 598),
                Size = new Size(230, 82),
                BackColor = Color.White
            };
            var divUser = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(230, 1),
                BackColor = Color.FromArgb(240, 240, 240)
            };

            string initials = "?";
            string fullName = "Guest";
            string userRole = "Unknown";

            if (LoginForm.CurrentUser != null)
            {
                fullName = LoginForm.CurrentUser.FullName;
                userRole = LoginForm.CurrentUser.Role;
                var parts = fullName.Trim().Split(' ');
                if (parts.Length >= 2)
                    initials = ("" + parts[0][0] + parts[1][0]).ToUpper();
                else if (parts.Length == 1 && parts[0].Length > 0)
                    initials = ("" + parts[0][0]).ToUpper();
            }

            var avatarPanel = new Panel
            {
                Location = new Point(14, 16),
                Size = new Size(36, 36),
                BackColor = Color.FromArgb(159, 225, 203)
            };
            string capturedInitials = initials;
            avatarPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                e.Graphics.DrawString(capturedInitials,
                    new Font("Segoe UI", 10f, FontStyle.Bold),
                    new SolidBrush(Color.FromArgb(15, 110, 86)),
                    new RectangleF(0, 0, 36, 36), sf);
            };
            MakeRounded(avatarPanel, 18);

            var lblUserName = new Label
            {
                Text = fullName,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Location = new Point(58, 12),
                Size = new Size(160, 18),
                AutoEllipsis = true
            };
            var lblUserRole = new Label
            {
                Text = userRole,
                Font = new Font("Segoe UI", 8f),
                ForeColor = Color.Gray,
                Location = new Point(58, 30),
                AutoSize = true
            };

            var btnLogout = new Button
            {
                Text = "  \uE7E8  Logout",
                Location = new Point(14, 54),
                Size = new Size(200, 24),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(180, 60, 60),
                Font = new Font("Segoe UI", 8.5f),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                FlatAppearance = { BorderSize = 0 }
            };
            btnLogout.Click += (s, e) =>
            {
                var result = MessageBox.Show(
                    "Are you sure you want to logout?",
                    "Logout — HamTracker",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    Application.Restart();
            };
            btnLogout.MouseEnter += (s, e) =>
            {
                btnLogout.ForeColor = Color.FromArgb(160, 30, 30);
                btnLogout.BackColor = Color.FromArgb(252, 235, 235);
            };
            btnLogout.MouseLeave += (s, e) =>
            {
                btnLogout.ForeColor = Color.FromArgb(180, 60, 60);
                btnLogout.BackColor = Color.White;
            };

            pnlUser.Controls.AddRange(new Control[]
            {
                divUser, avatarPanel,
                lblUserName, lblUserRole,
                btnLogout
            });

            pnlSidebar.Controls.AddRange(new Control[]
            {
                pnlLogo, divLogo, lblMain,
                btnDashboard, btnProjects, btnTasks, btnEvidence,
                lblReportsSection, btnTimeline, btnReports,
                pnlUser
            });

            // ── Top bar ───────────────────────────────────────────
            pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White
            };
            pnlTop.Paint += PaintTopBorder;

            lblCurrentPage = new Label
            {
                Text = "Dashboard",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 20),
                Location = new Point(22, 12),
                AutoSize = true
            };
            lblBreadcrumb = new Label
            {
                Text = "HamTracker / Overview",
                Font = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(170, 170, 170),
                Location = new Point(24, 36),
                AutoSize = true
            };

            var btnUpload = new Button
            {
                Text = "+ Upload Evidence",
                Size = new Size(155, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(29, 158, 117),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 },
                Anchor = AnchorStyles.Right | AnchorStyles.Top
            };
            btnUpload.Location = new Point(pnlTop.Width - 175, 12);
            btnUpload.Click += (s, e) => ShowEvidence();
            MakeRounded(btnUpload, 6);

            pnlTop.Controls.AddRange(new Control[]
                { lblCurrentPage, lblBreadcrumb, btnUpload });

            pnlTop.Resize += (s, e) =>
            {
                btnUpload.Location = new Point(pnlTop.Width - 175, 12);
            };

            // ── Content area ──────────────────────────────────────
            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(246, 246, 243),
                Padding = new Padding(20)
            };

            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlTop);
            this.Controls.Add(pnlSidebar);
        }

        // ── Sidebar button with ICON + TEXT ───────────────────────
        private Button MakeSidebarButton(string text, string iconChar, int top)
        {
            var btn = new Button
            {
                Location = new Point(8, top),
                Size = new Size(214, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(90, 90, 90),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 },
                Tag = iconChar
            };

            btn.Paint += (s, e) =>
            {
                var b = (Button)s;
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint =
                    System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                // Background
                using (var br = new SolidBrush(b.BackColor))
                    g.FillRectangle(br, b.ClientRectangle);

                // Active left accent bar
                if (b.BackColor == Color.FromArgb(225, 245, 238))
                {
                    using (var br = new SolidBrush(Color.FromArgb(29, 158, 117)))
                        g.FillRectangle(br, 0, 6, 3, 26);
                }

                // Icon (Segoe MDL2 Assets)
                string icon = b.Tag?.ToString() ?? "";
                using (var iconFont = new Font("Segoe MDL2 Assets", 13f))
                using (var iconBrush = new SolidBrush(b.ForeColor))
                {
                    var iconSf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(icon, iconFont, iconBrush,
                        new RectangleF(12, 0, 30, 38), iconSf);
                }

                // Label text
                using (var txtFont = new Font("Segoe UI",
                    b.BackColor == Color.FromArgb(225, 245, 238) ? 9.5f : 9.5f,
                    b.BackColor == Color.FromArgb(225, 245, 238)
                        ? FontStyle.Bold : FontStyle.Regular))
                using (var txtBrush = new SolidBrush(b.ForeColor))
                {
                    var txtSf = new StringFormat
                    {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(text, txtFont, txtBrush,
                        new RectangleF(48, 0, 160, 38), txtSf);
                }
            };

            // Hover effects
            btn.MouseEnter += (s, e) =>
            {
                var b = (Button)s;
                if (b.BackColor != Color.FromArgb(225, 245, 238))
                {
                    b.BackColor = Color.FromArgb(248, 248, 246);
                    b.ForeColor = Color.FromArgb(30, 30, 30);
                    b.Invalidate();
                }
            };
            btn.MouseLeave += (s, e) =>
            {
                var b = (Button)s;
                if (b.BackColor != Color.FromArgb(225, 245, 238))
                {
                    b.BackColor = Color.White;
                    b.ForeColor = Color.FromArgb(90, 90, 90);
                    b.Invalidate();
                }
            };

            return btn;
        }

        // ── Helpers ───────────────────────────────────────────────
        private void PaintSidebarBorder(object sender, PaintEventArgs e)
        {
            using (var pen = new Pen(Color.FromArgb(235, 235, 235), 1))
                e.Graphics.DrawLine(pen, 229, 0, 229,
                    ((Panel)sender).Height);
        }

        private void PaintTopBorder(object sender, PaintEventArgs e)
        {
            using (var pen = new Pen(Color.FromArgb(235, 235, 235), 1))
                e.Graphics.DrawLine(pen, 0, 59,
                    ((Panel)sender).Width, 59);
        }

        private Label MakeNavSection(string text, int top)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 180, 180),
                Location = new Point(18, top),
                AutoSize = true
            };
        }

        private void MakeRounded(Control ctrl, int radius)
        {
            ctrl.Paint += (s, e) =>
            {
                var c = (Control)s;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = RoundedRect(c.ClientRectangle, radius))
                    c.Region = new Region(path);
            };
        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y,
                radius * 2, radius * 2, 180, 90);
            path.AddArc(bounds.Right - radius * 2, bounds.Y,
                radius * 2, radius * 2, 270, 90);
            path.AddArc(bounds.Right - radius * 2,
                bounds.Bottom - radius * 2,
                radius * 2, radius * 2, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - radius * 2,
                radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(278, 244);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void SetActive(Button active)
        {
            Button[] all = { btnDashboard, btnProjects, btnTasks,
                             btnEvidence, btnTimeline, btnReports };
            foreach (var b in all)
            {
                b.BackColor = Color.White;
                b.ForeColor = Color.FromArgb(90, 90, 90);
                b.Invalidate();
            }
            active.BackColor = Color.FromArgb(225, 245, 238);
            active.ForeColor = Color.FromArgb(15, 110, 86);
            active.Invalidate();
        }

        private void LoadPanel(UserControl panel)
        {
            pnlContent.Controls.Clear();
            panel.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(panel);
        }

        // ── Screen loaders ────────────────────────────────────────
        private void ShowDashboard()
        {
            SetActive(btnDashboard);
            lblCurrentPage.Text = "Dashboard";
            lblBreadcrumb.Text = "HamTracker / Overview";
            LoadPanel(new DashboardPanel(_projectRepo, _taskRepo, _evidenceRepo));
        }
        private void ShowProjects()
        {
            SetActive(btnProjects);
            lblCurrentPage.Text = "Projects";
            lblBreadcrumb.Text = "HamTracker / Projects";
            LoadPanel(new ProjectsPanel(_projectRepo, _auditRepo));
        }
        private void ShowTasks()
        {
            SetActive(btnTasks);
            lblCurrentPage.Text = "Tasks";
            lblBreadcrumb.Text = "HamTracker / Tasks";
            LoadPanel(new TasksPanel(_taskRepo, _projectRepo, _auditRepo));
        }
        private void ShowEvidence()
        {
            SetActive(btnEvidence);
            lblCurrentPage.Text = "Evidence";
            lblBreadcrumb.Text = "HamTracker / Evidence";
            LoadPanel(new EvidencePanel(_evidenceRepo, _taskRepo, _auditRepo));
        }
        private void ShowTimeline()
        {
            SetActive(btnTimeline);
            lblCurrentPage.Text = "Timeline";
            lblBreadcrumb.Text = "HamTracker / Timeline";
            LoadPanel(new TimelinePanel(_auditRepo));
        }
        private void ShowReports()
        {
            SetActive(btnReports);
            lblCurrentPage.Text = "Reports";
            lblBreadcrumb.Text = "HamTracker / Reports";
            LoadPanel(new ReportsPanel(_projectRepo, _taskRepo, _evidenceRepo));
        }
    }
}