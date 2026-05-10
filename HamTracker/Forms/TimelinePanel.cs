using System;
using System.Drawing;
using System.Windows.Forms;
using HamTracker.Database;

namespace HamTracker.Forms
{
    public class TimelinePanel : UserControl
    {
        private AuditLogRepository _audit;
        private DataGridView _dgv;

        public TimelinePanel(AuditLogRepository audit)
        {
            _audit = audit;
            BuildUI();
        }

        private void BuildUI()
        {
            var lblTitle = new Label
            {
                Text = "Activity Timeline (Tamper-Proof Log)",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            _dgv = new DataGridView
            {
                Location = new Point(10, 42),
                Size = new Size(770, 420),
                ReadOnly = true,
                AllowUserToAddRows = false,
                BorderStyle = BorderStyle.None,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9f),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            _dgv.Columns.Add("Timestamp", "Timestamp");
            _dgv.Columns.Add("Action", "Action");
            _dgv.Columns.Add("EntityType", "Type");
            _dgv.Columns.Add("Details", "Details");
            _dgv.Columns["Timestamp"].Width = 140;
            _dgv.Columns["Action"].Width = 140;
            _dgv.Columns["EntityType"].Width = 80;

            this.Controls.AddRange(new Control[] { lblTitle, _dgv });
            LoadData();
        }

        private void LoadData()
        {
            _dgv.Rows.Clear();
            foreach (var log in _audit.GetAll())
            {
                int i = _dgv.Rows.Add(
                    log.Timestamp.ToString("dd MMM yyyy HH:mm:ss"),
                    log.Action,
                    log.EntityType,
                    log.Details);

                if (log.Action.Contains("Verified"))
                    _dgv.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(225, 245, 238);
                else if (log.Action.Contains("Created"))
                    _dgv.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(230, 241, 251);
                else if (log.Action.Contains("Deleted"))
                    _dgv.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(252, 235, 235);
            }
        }
    }
}