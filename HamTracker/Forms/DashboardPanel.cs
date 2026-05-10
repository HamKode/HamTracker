using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using HamTracker.Database;
using HamTracker.Models;

namespace HamTracker.Forms
{
    public class DashboardPanel : UserControl
    {
        private ProjectRepository _pr;
        private TaskRepository _tr;
        private EvidenceRepository _er;
        private bool _built = false;

        public DashboardPanel(ProjectRepository pr,
                              TaskRepository tr,
                              EvidenceRepository er)
        {
            _pr = pr; _tr = tr; _er = er;
            this.BackColor = Color.FromArgb(246, 246, 243);
            this.AutoScroll = false;
            this.Dock = DockStyle.Fill;
            this.AutoSize = false;
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
            this.AutoScroll = false;

            int W = this.ClientSize.Width;
            int H = this.ClientSize.Height;
            if (W < 200) W = 900;
            if (H < 200) H = 580;

            int pad = 18;
            int gap = 14;
            int CW = W - pad * 2;

            var projects = _pr.GetAll();
            var tasks = _tr.GetAll();
            var evidences = _er.GetAll();

            int totalProj = projects.Count;
            int doneTasks = tasks.FindAll(t => t.Status == "Done").Count;
            int inProg = tasks.FindAll(t => t.Status == "InProgress").Count;
            int totalEv = evidences.Count;
            int verified = evidences.FindAll(ev => ev.IsVerified).Count;

            int metricH = 120;
            int remaining = H - pad * 3 - gap * 2 - metricH;
            int taskH = (int)(remaining * 0.52);
            int projH = remaining - taskH;
            if (taskH < 160) taskH = 160;
            if (projH < 120) projH = 120;

            int y = pad;

            // ══════════════════════════════════════════════════════
            // ROW 1 — 4 Metric Cards
            // ══════════════════════════════════════════════════════
            int cardW = (CW - gap * 3) / 4;

            var mData = new[]
            {
                new { Label="Active Projects",   Val=totalProj.ToString(),
                      Sub="Total projects",
                      Ac=Color.FromArgb(29,158,117),
                      VFg=Color.FromArgb(15,110,86) },
                new { Label="Tasks Completed",   Val=doneTasks.ToString(),
                      Sub=inProg+" in progress",
                      Ac=Color.FromArgb(55,138,221),
                      VFg=Color.FromArgb(24,95,165) },
                new { Label="Evidence Files",    Val=totalEv.ToString(),
                      Sub=verified+" verified",
                      Ac=Color.FromArgb(239,159,39),
                      VFg=Color.FromArgb(133,79,11) },
                new { Label="Disputes Resolved", Val="0",
                      Sub="Clean record",
                      Ac=Color.FromArgb(127,119,221),
                      VFg=Color.FromArgb(83,74,183) }
            };

            for (int i = 0; i < mData.Length; i++)
            {
                var m = mData[i];
                int cx = pad + i * (cardW + gap);

                var card = new Panel
                {
                    Location = new Point(cx, y),
                    Size = new Size(cardW, metricH),
                    BackColor = Color.White
                };

                Color ac = m.Ac;
                Color vf = m.VFg;
                int cw2 = cardW;
                int ch2 = metricH;

                card.Paint += (s, e) =>
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    // Rounded border
                    using (var pen = new Pen(Color.FromArgb(232, 232, 230), 1f))
                    {
                        var path = new GraphicsPath();
                        int rad = 10;
                        var rect = new Rectangle(0, 0, cw2 - 1, ch2 - 1);
                        path.AddArc(rect.X, rect.Y,
                            rad * 2, rad * 2, 180, 90);
                        path.AddArc(rect.Right - rad * 2, rect.Y,
                            rad * 2, rad * 2, 270, 90);
                        path.AddArc(rect.Right - rad * 2, rect.Bottom - rad * 2,
                            rad * 2, rad * 2, 0, 90);
                        path.AddArc(rect.X, rect.Bottom - rad * 2,
                            rad * 2, rad * 2, 90, 90);
                        path.CloseFigure();
                        e.Graphics.DrawPath(pen, path);
                    }

                    // Top accent bar
                    using (var br = new SolidBrush(ac))
                        e.Graphics.FillRectangle(br,
                            new Rectangle(0, 0, cw2, 5));
                };

                var lblVal = new Label
                {
                    Text = m.Val,
                    Font = new Font("Segoe UI", 28f, FontStyle.Bold),
                    ForeColor = vf,
                    Location = new Point(16, 18),
                    AutoSize = true
                };
                var lblLabel = new Label
                {
                    Text = m.Label,
                    Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(100, 100, 100),
                    Location = new Point(16, 66),
                    Size = new Size(cardW - 24, 18),
                    AutoEllipsis = true
                };
                var lblSub = new Label
                {
                    Text = m.Sub,
                    Font = new Font("Segoe UI", 8f),
                    ForeColor = Color.FromArgb(185, 185, 185),
                    Location = new Point(16, 86),
                    Size = new Size(cardW - 24, 16),
                    AutoEllipsis = true
                };
                var strip = new Panel
                {
                    Location = new Point(16, metricH - 12),
                    Size = new Size(40, 4),
                    BackColor = ac
                };

                card.Controls.AddRange(new Control[]
                    { lblVal, lblLabel, lblSub, strip });
                this.Controls.Add(card);
            }

            y += metricH + gap;

            // ══════════════════════════════════════════════════════
            // ROW 2 — Recent Tasks (full width)
            // ══════════════════════════════════════════════════════
            var taskCard = MakeCard(pad, y, CW, taskH);

            var tHdr = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(CW, 46),
                BackColor = Color.White
            };
            tHdr.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(240, 240, 238), 1))
                    e.Graphics.DrawLine(pen, 0, 45, CW, 45);
            };
            tHdr.Controls.Add(new Label
            {
                Text = "Recent Tasks",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 20),
                Location = new Point(18, 13),
                AutoSize = true
            });

            var b1 = MakeBadge(doneTasks + " Done",
                Color.FromArgb(225, 245, 238), Color.FromArgb(15, 110, 86));
            var b2 = MakeBadge(inProg + " In Progress",
                Color.FromArgb(250, 238, 218), Color.FromArgb(133, 79, 11));
            var b3 = MakeBadge(
                (tasks.Count - doneTasks - inProg) + " To Do",
                Color.FromArgb(241, 239, 232), Color.FromArgb(95, 94, 90));

            b1.Location = new Point(CW - 300, 13);
            b2.Location = new Point(CW - 200, 13);
            b3.Location = new Point(CW - 96, 13);
            tHdr.Controls.Add(b1);
            tHdr.Controls.Add(b2);
            tHdr.Controls.Add(b3);
            taskCard.Controls.Add(tHdr);

            int tc1 = (int)(CW * 0.38);
            int tc2 = (int)(CW * 0.14);
            int tc3 = (int)(CW * 0.16);

            var tColHdr = MakeColHeader(CW, 46);
            tColHdr.Controls.Add(ColHdr("#", 18, 28));
            tColHdr.Controls.Add(ColHdr("Task Title", 46, tc1 - 10));
            tColHdr.Controls.Add(ColHdr("Status", tc1 + 46, tc2));
            tColHdr.Controls.Add(ColHdr("Project", tc1 + tc2 + 46, tc3));
            tColHdr.Controls.Add(ColHdr("Created", tc1 + tc2 + tc3 + 46, 100));
            taskCard.Controls.Add(tColHdr);

            int rowH = 34;
            int maxRows = (taskH - 46 - 28) / rowH;
            var disp = tasks.Count > maxRows
                ? tasks.GetRange(0, maxRows) : tasks;

            int tRowY = 74;
            int rNum = 1;
            foreach (var t in disp)
            {
                string pName = "—";
                var pj = projects.Find(p => p.ProjectId == t.ProjectId);
                if (pj != null) pName = pj.Name;
                taskCard.Controls.Add(
                    MakeTaskRow(t, pName, rNum, CW, tRowY, tc1, tc2, tc3));
                tRowY += rowH;
                rNum++;
            }

            if (tasks.Count == 0)
                taskCard.Controls.Add(EmptyLbl(
                    "No tasks yet — go to Tasks to add one.", 18, 90));

            this.Controls.Add(taskCard);
            y += taskH + gap;

            // ══════════════════════════════════════════════════════
            // ROW 3 — Projects Overview (full width)
            // ══════════════════════════════════════════════════════
            var projCard = MakeCard(pad, y, CW, projH);

            var pHdr = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(CW, 46),
                BackColor = Color.White
            };
            pHdr.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(240, 240, 238), 1))
                    e.Graphics.DrawLine(pen, 0, 45, CW, 45);
            };
            pHdr.Controls.Add(new Label
            {
                Text = "Projects Overview",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 20),
                Location = new Point(18, 13),
                AutoSize = true
            });
            pHdr.Controls.Add(new Label
            {
                Text = totalProj + " total",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(170, 170, 170),
                Location = new Point(CW - 70, 16),
                AutoSize = true
            });
            projCard.Controls.Add(pHdr);

            int pc1 = (int)(CW * 0.26);
            int pc2 = (int)(CW * 0.14);
            int pc3 = (int)(CW * 0.11);
            int pc4 = (int)(CW * 0.30);

            var pColHdr = MakeColHeader(CW, 46);
            pColHdr.Controls.Add(ColHdr("Project Name", 18, pc1));
            pColHdr.Controls.Add(ColHdr("Client", pc1 + 18, pc2));
            pColHdr.Controls.Add(ColHdr("Status", pc1 + pc2 + 18, pc3));
            pColHdr.Controls.Add(ColHdr("Progress", pc1 + pc2 + pc3 + 18, pc4));
            pColHdr.Controls.Add(ColHdr("Tasks", pc1 + pc2 + pc3 + pc4 + 18, 60));
            projCard.Controls.Add(pColHdr);

            Color[] pAccents =
            {
                Color.FromArgb(29,158,117),  Color.FromArgb(55,138,221),
                Color.FromArgb(239,159,39),  Color.FromArgb(127,119,221),
                Color.FromArgb(208,80,80)
            };

            int projRowH = 56;
            int maxPRows = (projH - 46 - 28) / projRowH;
            int pRowY = 74;
            int pIdx = 0;

            foreach (var p in projects)
            {
                if (pIdx >= maxPRows) break;
                var tList = _tr.GetByProject(p.ProjectId);
                int done2 = tList.FindAll(t => t.Status == "Done").Count;
                int pct = tList.Count == 0 ? 0
                    : (int)((done2 * 100.0) / tList.Count);
                AddProjRow(projCard, p, pct, tList.Count,
                    pAccents[pIdx % pAccents.Length],
                    CW, pRowY, pc1, pc2, pc3, pc4, pIdx);
                pRowY += projRowH;
                pIdx++;
            }

            if (projects.Count == 0)
                projCard.Controls.Add(EmptyLbl(
                    "No projects yet — go to Projects to create one.",
                    18, 80));

            this.Controls.Add(projCard);
        }

        // ── Task Row ──────────────────────────────────────────────
        private Panel MakeTaskRow(TaskItem t, string projName,
                                   int num, int W, int top,
                                   int c1, int c2, int c3)
        {
            bool even = num % 2 == 0;
            var row = new Panel
            {
                Location = new Point(0, top),
                Size = new Size(W, 33),
                BackColor = even
                    ? Color.FromArgb(251, 251, 249)
                    : Color.White,
                Cursor = Cursors.Hand
            };
            row.Paint += (s, e) =>
            {
                using (var p = new Pen(Color.FromArgb(244, 244, 242), 1))
                    e.Graphics.DrawLine(p, 0, 32, W, 32);
            };
            row.MouseEnter += (s, e) =>
                row.BackColor = Color.FromArgb(240, 249, 245);
            row.MouseLeave += (s, e) =>
                row.BackColor = even
                    ? Color.FromArgb(251, 251, 249)
                    : Color.White;

            row.Controls.Add(new Label
            {
                Text = num.ToString(),
                Font = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(200, 200, 200),
                Location = new Point(18, 8),
                Size = new Size(24, 16),
                TextAlign = ContentAlignment.MiddleRight
            });

            Color dotC =
                t.Status == "Done" ? Color.FromArgb(29, 158, 117) :
                t.Status == "InProgress" ? Color.FromArgb(239, 159, 39) :
                                           Color.FromArgb(200, 200, 200);
            var dot = new Panel
            {
                Location = new Point(46, 13),
                Size = new Size(8, 8),
                BackColor = dotC
            };
            MakeRounded(dot, 4);
            row.Controls.Add(dot);

            row.Controls.Add(new Label
            {
                Text = t.Title,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(25, 25, 25),
                Location = new Point(60, 8),
                Size = new Size(c1 - 18, 17),
                AutoEllipsis = true
            });

            Color bBg = t.Status == "Done"
                ? Color.FromArgb(225, 245, 238)
                : t.Status == "InProgress"
                    ? Color.FromArgb(250, 238, 218)
                    : Color.FromArgb(241, 239, 232);
            Color bFg = t.Status == "Done"
                ? Color.FromArgb(15, 110, 86)
                : t.Status == "InProgress"
                    ? Color.FromArgb(133, 79, 11)
                    : Color.FromArgb(95, 94, 90);
            string stTxt = t.Status == "InProgress"
                ? "In Progress" : t.Status;

            row.Controls.Add(new Label
            {
                Text = stTxt,
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = bFg,
                BackColor = bBg,
                Location = new Point(c1 + 46, 8),
                AutoSize = true,
                Padding = new Padding(5, 2, 5, 2)
            });

            row.Controls.Add(new Label
            {
                Text = projName,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(120, 120, 120),
                Location = new Point(c1 + c2 + 46, 9),
                Size = new Size(c3 - 8, 16),
                AutoEllipsis = true
            });

            row.Controls.Add(new Label
            {
                Text = t.CreatedAt.ToString("dd MMM yyyy"),
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(160, 160, 160),
                Location = new Point(c1 + c2 + c3 + 46, 9),
                AutoSize = true
            });

            return row;
        }

        // ── Project Row ───────────────────────────────────────────
        private void AddProjRow(Panel parent, Project p,
                                 int pct, int taskCount,
                                 Color accent, int W, int y,
                                 int pc1, int pc2, int pc3,
                                 int pc4, int idx)
        {
            bool even = idx % 2 == 0;
            var row = new Panel
            {
                Location = new Point(0, y),
                Size = new Size(W, 55),
                BackColor = even
                    ? Color.FromArgb(251, 251, 249)
                    : Color.White
            };
            row.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(244, 244, 242), 1))
                    e.Graphics.DrawLine(pen, 0, 54, W, 54);
            };
            row.MouseEnter += (s, e) =>
                row.BackColor = Color.FromArgb(240, 249, 245);
            row.MouseLeave += (s, e) =>
                row.BackColor = even
                    ? Color.FromArgb(251, 251, 249)
                    : Color.White;

            var dot = new Panel
            {
                Location = new Point(18, 22),
                Size = new Size(10, 10),
                BackColor = accent
            };
            MakeRounded(dot, 5);
            row.Controls.Add(dot);

            row.Controls.Add(new Label
            {
                Text = p.Name,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 20),
                Location = new Point(34, 9),
                Size = new Size(pc1 - 16, 18),
                AutoEllipsis = true
            });
            row.Controls.Add(new Label
            {
                Text = p.StartDate.ToString("dd MMM yyyy"),
                Font = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(185, 185, 185),
                Location = new Point(34, 28),
                Size = new Size(pc1 - 16, 14),
                AutoEllipsis = true
            });

            row.Controls.Add(new Label
            {
                Text = string.IsNullOrEmpty(p.ClientName)
                               ? "—" : p.ClientName,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(70, 70, 70),
                Location = new Point(pc1 + 18, 18),
                Size = new Size(pc2 - 8, 18),
                AutoEllipsis = true
            });

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
                Location = new Point(pc1 + pc2 + 18, 18),
                AutoSize = true,
                Padding = new Padding(6, 2, 6, 2)
            });

            int barW = pc4 - 54;
            int fillW = (int)(barW * pct / 100.0);
            int bx = pc1 + pc2 + pc3 + 18;

            row.Controls.Add(new Label
            {
                Text = pct + "%",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = accent,
                Location = new Point(bx, 8),
                AutoSize = true
            });

            var prgBg = new Panel
            {
                Location = new Point(bx, 30),
                Size = new Size(barW, 6),
                BackColor = Color.FromArgb(236, 236, 234)
            };
            prgBg.Controls.Add(new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(Math.Max(fillW, 2), 6),
                BackColor = accent
            });
            row.Controls.Add(prgBg);

            row.Controls.Add(new Label
            {
                Text = taskCount + " tasks",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(150, 150, 150),
                Location = new Point(bx + barW + 10, 18),
                AutoSize = true
            });

            parent.Controls.Add(row);
        }

        // ── Helpers ───────────────────────────────────────────────
        private Panel MakeCard(int x, int y, int w, int h)
        {
            var card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(w, h),
                BackColor = Color.White
            };
            int cw = w, ch = h;
            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var pen = new Pen(Color.FromArgb(232, 232, 230), 1f))
                    DrawRoundedRect(e.Graphics, pen,
                        new Rectangle(0, 0, cw - 1, ch - 1), 10);
            };
            return card;
        }

        private Panel MakeColHeader(int W, int top)
        {
            var hdr = new Panel
            {
                Location = new Point(0, top),
                Size = new Size(W, 28),
                BackColor = Color.FromArgb(250, 250, 248)
            };
            hdr.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(240, 240, 238), 1))
                {
                    e.Graphics.DrawLine(pen, 0, 0, W, 0);
                    e.Graphics.DrawLine(pen, 0, 27, W, 27);
                }
            };
            return hdr;
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

        private Label MakeBadge(string text, Color bg, Color fg)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                ForeColor = fg,
                BackColor = bg,
                AutoSize = true,
                Padding = new Padding(6, 2, 6, 2)
            };
        }

        private Label EmptyLbl(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.FromArgb(180, 180, 180),
                Location = new Point(x, y),
                AutoSize = true
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

        private void DrawRoundedRect(Graphics g, Pen pen,
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