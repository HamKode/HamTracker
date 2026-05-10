using System;
using System.Drawing;
using System.Windows.Forms;
using HamTracker.Database;
using HamTracker.Models;

namespace HamTracker.Forms
{
    public class TasksPanel : UserControl
    {
        private TaskRepository _taskRepo;
        private ProjectRepository _projectRepo;
        private AuditLogRepository _audit;
        private DataGridView _dgv;
        private ComboBox _cboProject;

        public TasksPanel(TaskRepository tr, ProjectRepository pr, AuditLogRepository audit)
        {
            _taskRepo = tr;
            _projectRepo = pr;
            _audit = audit;
            BuildUI();
        }

        private void BuildUI()
        {
            var lblFilter = new Label
            {
                Text = "Filter by Project:",
                Location = new Point(10, 16),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.Gray
            };

            _cboProject = new ComboBox
            {
                Location = new Point(130, 12),
                Size = new Size(200, 26),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cboProject.Items.Add("All Projects");
            foreach (var p in _projectRepo.GetAll())
                _cboProject.Items.Add(p);
            _cboProject.DisplayMember = "Name";
            _cboProject.SelectedIndex = 0;
            _cboProject.SelectedIndexChanged += (s, e) => LoadData();

            var btnAdd = new Button
            {
                Text = "+ New Task",
                Location = new Point(620, 10),
                Size = new Size(120, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(29, 158, 117),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
            btnAdd.Click += OnAddTask;

            var btnMarkDone = new Button
            {
                Text = "Mark Done",
                Location = new Point(340, 10),
                Size = new Size(110, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(55, 138, 221),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
            btnMarkDone.Click += OnMarkDone;

            _dgv = new DataGridView
            {
                Location = new Point(10, 56),
                Size = new Size(750, 380),
                ReadOnly = true,
                AllowUserToAddRows = false,
                BorderStyle = BorderStyle.None,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9f),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            _dgv.Columns.Add("TaskId", "ID");
            _dgv.Columns.Add("Title", "Task Title");
            _dgv.Columns.Add("Status", "Status");
            _dgv.Columns.Add("CreatedAt", "Created");
            _dgv.Columns["TaskId"].Width = 40;

            this.Controls.AddRange(new Control[]
                { lblFilter, _cboProject, btnAdd, btnMarkDone, _dgv });

            LoadData();
        }

        private void LoadData()
        {
            _dgv.Rows.Clear();
            var tasks = _cboProject.SelectedIndex == 0
                ? _taskRepo.GetAll()
                : _taskRepo.GetByProject(((Project)_cboProject.SelectedItem).ProjectId);

            foreach (var t in tasks)
            {
                int i = _dgv.Rows.Add(t.TaskId, t.Title, t.Status,
                            t.CreatedAt.ToString("dd MMM yyyy"));
                if (t.Status == "Done")
                    _dgv.Rows[i].DefaultCellStyle.ForeColor = Color.FromArgb(15, 110, 86);
                else if (t.Status == "InProgress")
                    _dgv.Rows[i].DefaultCellStyle.ForeColor = Color.FromArgb(133, 79, 11);
            }
        }

        private void OnAddTask(object sender, EventArgs e)
        {
            var projects = _projectRepo.GetAll();
            if (projects.Count == 0)
            {
                MessageBox.Show("Create a project first.", "No Projects",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            using (var dlg = new AddTaskDialog(projects))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _taskRepo.Insert(dlg.NewTask);
                    _audit.Log("TaskCreated", "Task", 0, "Created: " + dlg.NewTask.Title);
                    LoadData();
                }
            }
        }

        private void OnMarkDone(object sender, EventArgs e)
        {
            if (_dgv.SelectedRows.Count == 0) return;
            int id = Convert.ToInt32(_dgv.SelectedRows[0].Cells["TaskId"].Value);
            _taskRepo.UpdateStatus(id, "Done");
            _audit.Log("TaskCompleted", "Task", id, "Marked done: ID " + id);
            LoadData();
        }
    }
}