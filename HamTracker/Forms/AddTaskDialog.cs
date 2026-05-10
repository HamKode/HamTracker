using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using HamTracker.Models;

namespace HamTracker.Forms
{
    public class AddTaskDialog : Form
    {
        public TaskItem NewTask { get; private set; }

        private TextBox txtTitle, txtDesc;
        private ComboBox cboProject, cboStatus;

        public AddTaskDialog(List<Project> projects)
        {
            this.Text = "New Task";
            this.Size = new Size(400, 290);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Font = new Font("Segoe UI", 9.5f);

            AddLabel("Project:", 20);
            cboProject = new ComboBox
            {
                Location = new Point(20, 42),
                Size = new Size(340, 26),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboProject.DataSource = projects;
            cboProject.DisplayMember = "Name";
            cboProject.ValueMember = "ProjectId";
            this.Controls.Add(cboProject);

            AddLabel("Task Title:", 78);
            txtTitle = new TextBox { Location = new Point(20, 100), Size = new Size(340, 24), BorderStyle = BorderStyle.FixedSingle };
            this.Controls.Add(txtTitle);

            AddLabel("Description:", 130);
            txtDesc = new TextBox { Location = new Point(20, 152), Size = new Size(340, 24), BorderStyle = BorderStyle.FixedSingle };
            this.Controls.Add(txtDesc);

            AddLabel("Status:", 182);
            cboStatus = new ComboBox
            {
                Location = new Point(20, 204),
                Size = new Size(160, 26),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboStatus.Items.AddRange(new object[] { "ToDo", "InProgress", "Done" });
            cboStatus.SelectedIndex = 0;
            this.Controls.Add(cboStatus);

            var btnSave = new Button
            {
                Text = "Create Task",
                DialogResult = DialogResult.OK,
                Location = new Point(200, 200),
                Size = new Size(160, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(29, 158, 117),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                FlatAppearance = { BorderSize = 0 }
            };
            btnSave.Click += OnSave;
            this.Controls.Add(btnSave);
        }

        private void AddLabel(string text, int top)
        {
            this.Controls.Add(new Label
            {
                Text = text,
                Location = new Point(20, top),
                AutoSize = true,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 8.5f)
            });
        }

        private void OnSave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Task title is required.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }
            NewTask = new TaskItem
            {
                ProjectId = (int)cboProject.SelectedValue,
                Title = txtTitle.Text.Trim(),
                Description = txtDesc.Text.Trim(),
                Status = cboStatus.SelectedItem.ToString(),
                CreatedAt = DateTime.Now
            };
        }
    }
}