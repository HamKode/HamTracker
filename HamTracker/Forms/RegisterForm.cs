using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using HamTracker.Database;
using HamTracker.Models;

namespace HamTracker.Forms
{
    public partial class RegisterForm : Form
    {
        public string RegisteredUsername { get; private set; }
        private UserRepository _userRepo = new UserRepository();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd,
            int msg, int wParam, string lParam);
        private const int EM_SETCUEBANNER = 0x1501;

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void OnFormShown(object sender, EventArgs e)
        {
            SendMessage(txtFullName.Handle,
                EM_SETCUEBANNER, 1, "Your full name");
            SendMessage(txtRegUser.Handle,
                EM_SETCUEBANNER, 1, "Choose a username");
            SendMessage(txtRegEmail.Handle,
                EM_SETCUEBANNER, 1, "your@email.com");
            SendMessage(txtRegPass.Handle,
                EM_SETCUEBANNER, 1, "Min 6 characters");
            SendMessage(txtRegConfirm.Handle,
                EM_SETCUEBANNER, 1, "Repeat password");
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            lblRegError.Text = "";
            lblRegSuccess.Text = "";

            if (string.IsNullOrWhiteSpace(txtFullName.Text) ||
                string.IsNullOrWhiteSpace(txtRegUser.Text) ||
                string.IsNullOrWhiteSpace(txtRegPass.Text))
            {
                lblRegError.Text =
                    "Full name, username and password are required.";
                return;
            }
            if (txtRegPass.Text.Length < 6)
            {
                lblRegError.Text =
                    "Password must be at least 6 characters.";
                return;
            }
            if (txtRegPass.Text != txtRegConfirm.Text)
            {
                lblRegError.Text = "Passwords do not match.";
                return;
            }

            var user = new User
            {
                FullName = txtFullName.Text.Trim(),
                Username = txtRegUser.Text.Trim(),
                Email = txtRegEmail.Text.Trim(),
                Password = txtRegPass.Text,
                Role = "Freelancer"
            };

            bool ok = _userRepo.Register(user);
            if (ok)
            {
                lblRegSuccess.Text = "Account created successfully!";
                RegisteredUsername = txtRegUser.Text.Trim();

                var t = new Timer { Interval = 1200 };
                t.Tick += (ts, te) =>
                {
                    t.Stop();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                };
                t.Start();
            }
            else
            {
                lblRegError.Text =
                    "Username already taken. Choose another.";
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}