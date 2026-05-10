using System;
using System.Drawing;
using System.Windows.Forms;
using HamTracker.Database;
using HamTracker.Models;
using HamTracker.Services;

namespace HamTracker.Forms
{
    public class ReportsPanel : UserControl
    {
        private ProjectRepository _projectRepo;
        private TaskRepository _taskRepo;
        private EvidenceRepository _evidenceRepo;
        private ComboBox _cboProject;

        public ReportsPanel(ProjectRepository pr, TaskRepository tr, EvidenceRepository er)
        {
            _projectRepo = pr;
            _taskRepo = tr;
            _evidenceRepo = er;
            BuildUI();
        }

        private void BuildUI()
        {
            var lblTitle = new Label
            {
                Text = "Generate Project Report",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            var lblSel = new Label
            {
                Text = "Select Project:",
                Location = new Point(10, 50),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            _cboProject = new ComboBox
            {
                Location = new Point(10, 72),
                Size = new Size(300, 26),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            foreach (var p in _projectRepo.GetAll())
                _cboProject.Items.Add(p);
            _cboProject.DisplayMember = "Name";
            if (_cboProject.Items.Count > 0)
                _cboProject.SelectedIndex = 0;

            var btnGenerate = new Button
            {
                Text = "Export Report (.txt)",
                Location = new Point(10, 116),
                Size = new Size(200, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(29, 158, 117),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
            btnGenerate.Click += OnGenerate;

            var lblInfo = new Label
            {
                Text = "Report includes all tasks, evidence filenames,\nuploaded dates, verification status and SHA-256 hashes.",
                Location = new Point(10, 170),
                AutoSize = true,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9f)
            };

            this.Controls.AddRange(new Control[]
                { lblTitle, lblSel, _cboProject, btnGenerate, lblInfo });
        }

        private void OnGenerate(object sender, EventArgs e)
        {
            if (_cboProject.SelectedItem == null)
            {
                MessageBox.Show("Select a project first.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var project = (Project)_cboProject.SelectedItem;
            var tasks = _taskRepo.GetByProject(project.ProjectId);
            var evidences = _evidenceRepo.GetAll();

            using (var sfd = new SaveFileDialog())
            {
                sfd.FileName = "HamTracker_Report_" + project.Name + ".txt";
                sfd.Filter = "Text Files|*.txt";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var svc = new ReportService();
                    svc.GenerateTextReport(project, tasks, evidences, sfd.FileName);
                    MessageBox.Show("Report saved to:\n" + sfd.FileName,
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}