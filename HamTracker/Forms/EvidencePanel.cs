using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using HamTracker.Database;
using HamTracker.Models;
using HamTracker.Services;

namespace HamTracker.Forms
{
    public class EvidencePanel : UserControl
    {
        private EvidenceRepository _evidenceRepo;
        private TaskRepository _taskRepo;
        private AuditLogRepository _audit;
        private DataGridView _dgv;
        private Label _lblHash;
        private ComboBox _cboTask;
        private TextBox _txtDesc, _txtFile;

        public EvidencePanel(EvidenceRepository er, TaskRepository tr, AuditLogRepository audit)
        {
            _evidenceRepo = er;
            _taskRepo = tr;
            _audit = audit;
            BuildUI();
        }

        private void BuildUI()
        {
            // Upload section
            var pnlUpload = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(360, 420),
                BackColor = Color.White
            };

            var lblTitle = new Label
            {
                Text = "Upload Evidence",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                Location = new Point(14, 14),
                AutoSize = true
            };

            var lblTask = new Label { Text = "Select Task:", Location = new Point(14, 50), AutoSize = true, ForeColor = Color.Gray };
            _cboTask = new ComboBox
            {
                Location = new Point(14, 70),
                Size = new Size(320, 26),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            foreach (var t in _taskRepo.GetAll())
                _cboTask.Items.Add(t);
            _cboTask.DisplayMember = "Title";
            if (_cboTask.Items.Count > 0) _cboTask.SelectedIndex = 0;

            var lblDesc = new Label { Text = "Description:", Location = new Point(14, 106), AutoSize = true, ForeColor = Color.Gray };
            _txtDesc = new TextBox { Location = new Point(14, 126), Size = new Size(320, 24), BorderStyle = BorderStyle.FixedSingle };

            var lblFile = new Label { Text = "File:", Location = new Point(14, 162), AutoSize = true, ForeColor = Color.Gray };
            _txtFile = new TextBox { Location = new Point(14, 182), Size = new Size(240, 24), BorderStyle = BorderStyle.FixedSingle, ReadOnly = true };

            var btnBrowse = new Button
            {
                Text = "Browse",
                Location = new Point(262, 180),
                Size = new Size(72, 26),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 1 }
            };
            btnBrowse.Click += OnBrowse;

            _lblHash = new Label
            {
                Text = "SHA-256: —",
                Font = new Font("Courier New", 7.5f),
                ForeColor = Color.Gray,
                Location = new Point(14, 218),
                Size = new Size(320, 36),
                AutoEllipsis = true
            };

            var btnUpload = new Button
            {
                Text = "Submit Evidence",
                Location = new Point(14, 268),
                Size = new Size(320, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(29, 158, 117),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
            btnUpload.Click += OnUpload;

            pnlUpload.Controls.AddRange(new Control[]
                { lblTitle, lblTask, _cboTask, lblDesc, _txtDesc,
                  lblFile, _txtFile, btnBrowse, _lblHash, btnUpload });

            // Evidence list
            _dgv = new DataGridView
            {
                Location = new Point(385, 10),
                Size = new Size(390, 420),
                ReadOnly = true,
                AllowUserToAddRows = false,
                BorderStyle = BorderStyle.None,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 8.5f),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            _dgv.Columns.Add("FileName", "File");
            _dgv.Columns.Add("UploadedAt", "Uploaded");
            _dgv.Columns.Add("IsVerified", "Verified");
            _dgv.Columns.Add("FileHash", "SHA-256");

            this.Controls.AddRange(new Control[] { pnlUpload, _dgv });
            LoadEvidenceList();
        }

        private void OnBrowse(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Images|*.png;*.jpg;*.jpeg|PDF|*.pdf|All Files|*.*";
                ofd.Title = "Select Evidence File";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _txtFile.Text = ofd.FileName;
                    string hash = HashService.ComputeFileHash(ofd.FileName);
                    _lblHash.Text = "SHA-256: " + hash;
                    _lblHash.ForeColor = Color.FromArgb(15, 110, 86);
                }
            }
        }

        private void OnUpload(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_txtFile.Text))
            {
                MessageBox.Show("Please select a file.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_cboTask.SelectedItem == null)
            {
                MessageBox.Show("Please select a task.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var task = (TaskItem)_cboTask.SelectedItem;
            var svc = new EvidenceService();
            var ev = svc.PrepareEvidence(_txtFile.Text, task.TaskId, _txtDesc.Text.Trim());

            _evidenceRepo.Insert(ev);
            _audit.Log("EvidenceUploaded", "Evidence", task.TaskId,
                       "File: " + ev.FileName + " | Hash: " + ev.FileHash);

            MessageBox.Show("Evidence uploaded!\nSHA-256: " + ev.FileHash,
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            _txtFile.Text = string.Empty;
            _txtDesc.Text = string.Empty;
            _lblHash.Text = "SHA-256: —";
            _lblHash.ForeColor = Color.Gray;
            LoadEvidenceList();
        }

        private void LoadEvidenceList()
        {
            _dgv.Rows.Clear();
            foreach (var ev in _evidenceRepo.GetAll())
            {
                string shortHash = ev.FileHash.Length > 16 ? ev.FileHash.Substring(0, 16) + "..." : ev.FileHash;
                int i = _dgv.Rows.Add(ev.FileName,
                            ev.UploadedAt.ToString("dd MMM yyyy HH:mm"),
                            ev.IsVerified ? "Verified" : "Pending",
                            shortHash);
                _dgv.Rows[i].DefaultCellStyle.ForeColor = ev.IsVerified
                    ? Color.FromArgb(15, 110, 86)
                    : Color.FromArgb(133, 79, 11);
            }
        }
    }
}