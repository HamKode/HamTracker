using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using HamTracker.Models;

namespace HamTracker.Forms
{
    public class AddProjectDialog : Form
    {
        public Project NewProject { get; private set; }

        private TextBox txtName, txtDesc, txtClient;
        private DateTimePicker dtp;

        public AddProjectDialog()
        {
            this.Text = "New Project";
            this.Size = new Size(460, 380);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5f);

            int lx = 24;  // label x
            int tx = 24;  // textbox x
            int tw = 390; // textbox width

            // ── Project Name ──────────────────────────────────────
            AddLabel("Project Name:", lx, 20);
            txtName = MakeTextBox(tx, 42, tw);

            // ── Client Name ───────────────────────────────────────
            AddLabel("Client Name:", lx, 90);
            txtClient = MakeTextBox(tx, 112, tw);

            // ── Description ───────────────────────────────────────
            AddLabel("Description:", lx, 160);
            txtDesc = MakeTextBox(tx, 182, tw);

            // ── Start Date ────────────────────────────────────────
            AddLabel("Start Date:", lx, 230);
            dtp = new DateTimePicker
            {
                Location = new Point(tx, 252),
                Size = new Size(tw, 30),
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 9.5f)
            };
            this.Controls.Add(dtp);

            // ── Buttons ───────────────────────────────────────────
            var btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(tx, 304),
                Size = new Size(186, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(245, 245, 243),
                ForeColor = Color.FromArgb(80, 80, 80),
                Font = new Font("Segoe UI", 9.5f),
                Cursor = Cursors.Hand,
                FlatAppearance =
                {
                    BorderSize  = 1,
                    BorderColor = Color.FromArgb(220, 220, 218)
                }
            };
            btnCancel.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            var btnSave = new Button
            {
                Text = "Create Project",
                Location = new Point(tx + 194, 304),
                Size = new Size(196, 38),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(29, 158, 117),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
            btnSave.MouseEnter += (s, e) =>
                btnSave.BackColor = Color.FromArgb(15, 110, 86);
            btnSave.MouseLeave += (s, e) =>
                btnSave.BackColor = Color.FromArgb(29, 158, 117);
            btnSave.Click += OnSave;

            this.Controls.Add(btnCancel);
            this.Controls.Add(btnSave);
        }

        // ── Helpers ───────────────────────────────────────────────
        private void AddLabel(string text, int x, int y)
        {
            this.Controls.Add(new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                ForeColor = Color.FromArgb(80, 80, 80),
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold)
            });
        }

        private TextBox MakeTextBox(int x, int y, int w)
        {
            var txt = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(w, 32),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9.5f),
                BackColor = Color.FromArgb(250, 250, 250)
            };
            txt.Enter += (s, e) => txt.BackColor = Color.White;
            txt.Leave += (s, e) =>
                txt.BackColor = Color.FromArgb(250, 250, 250);
            this.Controls.Add(txt);
            return txt;
        }

        // ── Save handler ──────────────────────────────────────────
        private void OnSave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show(
                    "Project name is required.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            NewProject = new Project
            {
                Name = txtName.Text.Trim(),
                ClientName = txtClient.Text.Trim(),
                Description = txtDesc.Text.Trim(),
                StartDate = dtp.Value,
                Status = "Active"
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}