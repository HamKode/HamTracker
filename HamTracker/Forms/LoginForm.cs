using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using HamTracker.Database;
using HamTracker.Models;

namespace HamTracker.Forms
{
    public partial class LoginForm : Form
    {
        public static User CurrentUser { get; private set; }
        private UserRepository _userRepo = new UserRepository();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd,
            int msg, int wParam, string lParam);
        private const int EM_SETCUEBANNER = 0x1501;

        public LoginForm()
        {
            _userRepo.EnsureTable();
            InitializeComponent();
        }

        private void OnFormShown(object sender, EventArgs e)
        {
            SendMessage(txtUsername.Handle,
                EM_SETCUEBANNER, 1, "Enter your username");
            SendMessage(txtPassword.Handle,
                EM_SETCUEBANNER, 1, "Enter your password");

            if (Program.AppIcon != null)
                this.Icon = Program.AppIcon;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            lblLoginError.Text = "";
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblLoginError.Text = "Please enter username and password.";
                return;
            }
            var user = _userRepo.Login(
                txtUsername.Text, txtPassword.Text);
            if (user != null)
            {
                CurrentUser = user;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                lblLoginError.Text = "Invalid username or password.";
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnLogin_Click(null, null);
        }

        private void btnEye_Click(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar =
                !txtPassword.UseSystemPasswordChar;
        }

        private void btnGoRegister_Click(object sender, EventArgs e)
        {
            using (var reg = new RegisterForm())
            {
                if (reg.ShowDialog() == DialogResult.OK)
                {
                    txtUsername.Text = reg.RegisteredUsername;
                    txtPassword.Focus();
                }
            }
        }

        private void btnLogin_MouseEnter(object sender, EventArgs e)
        {
            btnLogin.BackColor =
                Color.FromArgb(15, 110, 86);
        }
        private void btnLogin_MouseLeave(object sender, EventArgs e)
        {
            btnLogin.BackColor =
                Color.FromArgb(29, 158, 117);
        }
    }
}