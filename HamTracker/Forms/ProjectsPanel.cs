using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using HamTracker.Database;
using HamTracker.Models;

namespace HamTracker.Forms
{
    public class ProjectsPanel : UserControl
    {
        private ProjectRepository _repo;
        private AuditLogRepository _audit;
        private bool _built = false;

        public ProjectsPanel(ProjectRepository repo,
                             AuditLogRepository audit)
        {
            _repo = repo;
            _audit = audit;
            this.BackColor = Color.FromArgb(246, 246, 243);
            this.AutoScroll = false;
            this.Dock = DockStyle.Fill;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!_built)
            {
                _built = true;
                var t = new Timer { Interval = 30 };
                t.Tick += (s, ev) => { t.Stop(); BuildUI(); };
                t.Start();
            }
        }

        private void BuildUI()
        {
            this.Controls.Clear();

            int W = this.ClientSize.Width;
            int H = this.ClientSize.Height;
            if (W < 200) W = 900;
            if (H < 200) H = 580;

            int pad = 18;
            int gap = 12;
            int CW = W - pad * 2;

            var projects = _repo.GetAll();

            // ══════════════════════════════════════════════════════
            // TOP BAR — Title + Buttons
            // ══════════════════════════════════════════════════════
            var topBar = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(W, 64),
                BackColor = Color.White
            };
            topBar.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(236, 236, 234), 1))
                    e.Graphics.DrawLine(pen, 0, 63, W, 63);
            };

            topBar.Controls.Add(new Label
            {
                Text = "Projects",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 20),
                Location = new Point(pad, 17),
                AutoSize = true
            });
            topBar.Controls.Add(new Label
            {
                Text = projects.Count + " total projects",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(170, 170, 170),
                Location = new Point(pad, 40),
                AutoSize = true
            });

            // Buttons — right side
            var btnDel = MakeBtn("Delete Selected",
                Color.FromArgb(245, 245, 243),
                Color.FromArgb(180, 60, 60),
                Color.FromArgb(220, 220, 218));
            btnDel.Size = new Size(148, 36);
            btnDel.Location = new Point(W - 310, 14);
            btnDel.Click += OnDelete;

            var btnAdd = MakeBtn("+ New Project",
                Color.FromArgb(29, 158, 117),
                Color.White,
                Color.Transparent);
            btnAdd.Size = new Size(140, 36);
            btnAdd.Location = new Point(W - 154, 14);
            btnAdd.Click += OnAdd;
            btnAdd.MouseEnter += (s, e) =>
                btnAdd.BackColor = Color.FromArgb(15, 110, 86);
            btnAdd.MouseLeave += (s, e) =>
                btnAdd.BackColor = Color.FromArgb(29, 158, 117);

            topBar.Controls.Add(btnDel);
            topBar.Controls.Add(btnAdd);
            this.Controls.Add(topBar);

            // ══════════════════════════════════════════════════════
            // STATS ROW — 3 mini stat cards
            // ══════════════════════════════════════════════════════
            int statY = 76;
            int statH = 72;
            int statW = (CW - gap * 2) / 3;

            int active = projects.FindAll(p => p.Status == "Active").Count;
            int completed = projects.FindAll(p => p.Status == "Completed").Count;
            int paused = projects.Count - active - completed;

            var stats = new[]
            {
                new { Label="Active",    Val=active.ToString(),
                      Ac=Color.FromArgb(29,158,117),
                      Fg=Color.FromArgb(15,110,86),
                      Bg=Color.FromArgb(225,245,238) },
                new { Label="Completed", Val=completed.ToString(),
                      Ac=Color.FromArgb(55,138,221),
                      Fg=Color.FromArgb(24,95,165),
                      Bg=Color.FromArgb(230,241,251) },
                new { Label="Paused",    Val=paused.ToString(),
                      Ac=Color.FromArgb(239,159,39),
                      Fg=Color.FromArgb(133,79,11),
                      Bg=Color.FromArgb(250,238,218) }
            };

            for (int i = 0; i < stats.Length; i++)
            {
                var st = stats[i];
                int sx = pad + i * (statW + gap);

                var sc = new Panel
                {
                    Location = new Point(sx, statY),
                    Size = new Size(statW, statH),
                    BackColor = Color.White
                };

                Color ac2 = st.Ac;
                Color bg2 = st.Bg;
                Color fg2 = st.Fg;
                int sw2 = statW;
                int sh2 = statH;

                sc.Paint += (s, e) =>
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using (var pen = new Pen(Color.FromArgb(232, 232, 230), 1))
                        DrawRounded(e.Graphics, pen,
                            new Rectangle(0, 0, sw2 - 1, sh2 - 1), 8);
                    using (var br = new SolidBrush(ac2))
                        e.Graphics.FillRectangle(br,
                            new Rectangle(0, 0, sw2, 4));
                };

                // Icon pill
                var pill = new Panel
                {
                    Location = new Point(statW - 52, 14),
                    Size = new Size(36, 22),
                    BackColor = bg2
                };
                MakeRounded(pill, 11);

                sc.Controls.Add(pill);
                sc.Controls.Add(new Label
                {
                    Text = st.Val,
                    Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                    ForeColor = fg2,
                    Location = new Point(14, 10),
                    AutoSize = true
                });
                sc.Controls.Add(new Label
                {
                    Text = st.Label,
                    Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(130, 130, 130),
                    Location = new Point(14, 46),
                    AutoSize = true
                });

                this.Controls.Add(sc);
            }

            // ══════════════════════════════════════════════════════
            // MAIN TABLE CARD
            // ══════════════════════════════════════════════════════
            int tableY = statY + statH + gap;
            int tableH = H - tableY - pad;
            if (tableH < 200) tableH = 200;

            var tableCard = new Panel
            {
                Location = new Point(pad, tableY),
                Size = new Size(CW, tableH),
                BackColor = Color.White
            };
            int cw3 = CW, ch3 = tableH;
            tableCard.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var pen = new Pen(Color.FromArgb(232, 232, 230), 1))
                    DrawRounded(e.Graphics, pen,
                        new Rectangle(0, 0, cw3 - 1, ch3 - 1), 10);
            };

            // ── Table header ──────────────────────────────────────
            var tblHdr = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(CW, 28),
                BackColor = Color.FromArgb(250, 250, 248)
            };
            tblHdr.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(240, 240, 238), 1))
                    e.Graphics.DrawLine(pen, 0, 27, CW, 27);
            };

            int pc1 = (int)(CW * 0.27);
            int pc2 = (int)(CW * 0.15);
            int pc3 = (int)(CW * 0.11);
            int pc4 = (int)(CW * 0.14);
            int pc5 = (int)(CW * 0.18);

            tblHdr.Controls.Add(ColHdr("Project Name", 16, pc1));
            tblHdr.Controls.Add(ColHdr("Client", pc1 + 16, pc2));
            tblHdr.Controls.Add(ColHdr("Status", pc1 + pc2 + 16, pc3));
            tblHdr.Controls.Add(ColHdr("Start Date", pc1 + pc2 + pc3 + 16, pc4));
            tblHdr.Controls.Add(ColHdr("Progress", pc1 + pc2 + pc3 + pc4 + 16, pc5));
            tblHdr.Controls.Add(ColHdr("Actions", CW - 100, 80));
            tableCard.Controls.Add(tblHdr);

            // ── Project rows ──────────────────────────────────────
            if (projects.Count == 0)
            {
                var pnlEmpty = new Panel
                {
                    Location = new Point(0, 28),
                    Size = new Size(CW, tableH - 28),
                    BackColor = Color.White
                };

                pnlEmpty.Controls.Add(new Label
                {
                    Text = "No projects yet",
                    Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(200, 200, 200),
                    Location = new Point(CW / 2 - 80, tableH / 2 - 40),
                    AutoSize = true
                });
                pnlEmpty.Controls.Add(new Label
                {
                    Text = "Click '+ New Project' to get started",
                    Font = new Font("Segoe UI", 9.5f),
                    ForeColor = Color.FromArgb(200, 200, 200),
                    Location = new Point(CW / 2 - 110, tableH / 2 - 14),
                    AutoSize = true
                });
                tableCard.Controls.Add(pnlEmpty);
            }

            Color[] accents =
            {
                Color.FromArgb(29,158,117), Color.FromArgb(55,138,221),
                Color.FromArgb(239,159,39), Color.FromArgb(127,119,221),
                Color.FromArgb(208,80,80)
            };

            int rowH = 54;
            int rowY = 28;
            int pIdx = 0;

            foreach (var p in projects)
            {
                if (rowY + rowH > tableH) break;
                Color ac = accents[pIdx % accents.Length];
                AddProjectRow(tableCard, p, ac, CW, rowY,
                    pc1, pc2, pc3, pc4, pc5, pIdx, rowH);
                rowY += rowH;
                pIdx++;
            }

            this.Controls.Add(tableCard);
        }

        // ── Project Table Row ─────────────────────────────────────
        private void AddProjectRow(Panel parent, Project p,
                                    Color accent, int W,
                                    int y, int pc1, int pc2,
                                    int pc3, int pc4, int pc5,
                                    int idx, int rowH)
        {
            bool even = idx % 2 == 0;
            var row = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(W, rowH),
                BackColor = even
                    ? Color.FromArgb(252, 252, 250)
                    : Color.White,
                Tag = p.ProjectId
            };
            row.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(244, 244, 242), 1))
                    e.Graphics.DrawLine(pen, 0, rowH - 1, W, rowH - 1);
            };
            row.MouseEnter += (s, e) =>
                row.BackColor = Color.FromArgb(240, 249, 245);
            row.MouseLeave += (s, e) =>
                row.BackColor = even
                    ? Color.FromArgb(252, 252, 250)
                    : Color.White;

            // Color dot
            var dot = new Panel
            {
                Location = new Point(16, rowH / 2 - 5),
                Size = new Size(10, 10),
                BackColor = accent
            };
            MakeRounded(dot, 5);
            row.Controls.Add(dot);

            // Project name + description
            row.Controls.Add(new Label
            {
                Text = p.Name,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 20),
                Location = new Point(32, 10),
                Size = new Size(pc1 - 36, 18),
                AutoEllipsis = true
            });
            row.Controls.Add(new Label
            {
                Text = string.IsNullOrEmpty(p.Description)
                               ? "No description"
                               : p.Description,
                Font = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(180, 180, 180),
                Location = new Point(32, 30),
                Size = new Size(pc1 - 36, 14),
                AutoEllipsis = true
            });

            // Client
            row.Controls.Add(new Label
            {
                Text = string.IsNullOrEmpty(p.ClientName)
                               ? "—" : p.ClientName,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(pc1 + 16, rowH / 2 - 9),
                Size = new Size(pc2 - 8, 18),
                AutoEllipsis = true
            });

            // Status badge
            Color sBg = p.Status == "Active"
                ? Color.FromArgb(225, 245, 238)
                : p.Status == "Completed"
                    ? Color.FromArgb(230, 241, 251)
                    : Color.FromArgb(241, 239, 232);
            Color sFg = p.Status == "Active"
                ? Color.FromArgb(15, 110, 86)
                : p.Status == "Completed"
                    ? Color.FromArgb(24, 95, 165)
                    : Color.FromArgb(95, 94, 90);

            row.Controls.Add(new Label
            {
                Text = p.Status,
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = sFg,
                BackColor = sBg,
                Location = new Point(pc1 + pc2 + 16, rowH / 2 - 10),
                AutoSize = true,
                Padding = new Padding(6, 3, 6, 3)
            });

            // Start date
            row.Controls.Add(new Label
            {
                Text = p.StartDate.ToString("dd MMM yyyy"),
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(pc1 + pc2 + pc3 + 16, rowH / 2 - 8),
                AutoSize = true
            });

            // Progress bar
            var _tr = new TaskRepository();
            var tList = _tr.GetByProject(p.ProjectId);
            int done = tList.FindAll(t => t.Status == "Done").Count;
            int pct = tList.Count == 0 ? 0
                : (int)((done * 100.0) / tList.Count);
            int barX = pc1 + pc2 + pc3 + pc4 + 16;
            int barW = pc5 - 50;

            row.Controls.Add(new Label
            {
                Text = pct + "%",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = accent,
                Location = new Point(barX, 10),
                AutoSize = true
            });

            var prgBg = new Panel
            {
                Location = new Point(barX, 30),
                Size = new Size(barW, 6),
                BackColor = Color.FromArgb(236, 236, 234)
            };
            int fillW = (int)(barW * pct / 100.0);
            prgBg.Controls.Add(new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(Math.Max(fillW, 2), 6),
                BackColor = accent
            });
            row.Controls.Add(prgBg);

            // Task count pill
            row.Controls.Add(new Label
            {
                Text = tList.Count + " tasks",
                Font = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(150, 150, 150),
                Location = new Point(barX + barW + 6, 30),
                AutoSize = true
            });

            // Delete button
            var btnDel = new Button
            {
                Text = "✕",
                Location = new Point(W - 48, rowH / 2 - 14),
                Size = new Size(28, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(200, 200, 200),
                Font = new Font("Segoe UI", 9f),
                Cursor = Cursors.Hand,
                FlatAppearance =
                {
                    BorderSize  = 1,
                    BorderColor = Color.FromArgb(230,230,228)
                },
                Tag = p.ProjectId
            };
            btnDel.MouseEnter += (s, e) =>
            {
                btnDel.BackColor = Color.FromArgb(254, 235, 235);
                btnDel.ForeColor = Color.FromArgb(180, 60, 60);
                btnDel.FlatAppearance.BorderColor =
                    Color.FromArgb(240, 180, 180);
            };
            btnDel.MouseLeave += (s, e) =>
            {
                btnDel.BackColor = Color.White;
                btnDel.ForeColor = Color.FromArgb(200, 200, 200);
                btnDel.FlatAppearance.BorderColor =
                    Color.FromArgb(230, 230, 228);
            };
            btnDel.Click += (s, e) =>
            {
                int pid = (int)((Button)s).Tag;
                var res = MessageBox.Show(
                    "Delete project \"" + p.Name + "\"?\nThis cannot be undone.",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                if (res == DialogResult.Yes)
                {
                    _repo.Delete(pid);
                    _audit.Log("ProjectDeleted", "Project",
                               pid, "Deleted: " + p.Name);
                    BuildUI();
                }
            };
            row.Controls.Add(btnDel);

            parent.Controls.Add(row);
        }

        // ── Event Handlers ────────────────────────────────────────
        private void OnAdd(object sender, EventArgs e)
        {
            using (var dlg = new AddProjectDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _repo.Insert(dlg.NewProject);
                    _audit.Log("ProjectCreated", "Project",
                               0, "Created: " + dlg.NewProject.Name);
                    BuildUI();
                }
            }
        }

        private void OnDelete(object sender, EventArgs e)
        {
            // Find selected row (any hovered or first)
            MessageBox.Show(
                "Use the ✕ button on each row to delete a project.",
                "Delete Project",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // ── UI Helpers ────────────────────────────────────────────
        private Button MakeBtn(string text, Color bg,
                                Color fg, Color border)
        {
            var btn = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                BackColor = bg,
                ForeColor = fg,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance =
                {
                    BorderSize  = border == Color.Transparent ? 0 : 1,
                    BorderColor = border == Color.Transparent
                                  ? Color.FromArgb(29,158,117)
                                  : border
                }
            };
            return btn;
        }

        private Label ColHdr(string text, int x, int width)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(145, 145, 145),
                Location = new Point(x, 6),
                Size = new Size(width, 16)
            };
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

        private void DrawRounded(Graphics g, Pen pen,
                                  Rectangle r, int radius)
        {
            using (var path = BuildPath(r, radius))
                g.DrawPath(pen, path);
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
    }
}