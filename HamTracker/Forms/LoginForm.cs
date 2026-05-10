using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using HamTracker.Database;
using HamTracker.Models;

namespace HamTracker.Forms
{
    public class LoginForm : Form
    {
        public static User CurrentUser { get; private set; }

        private UserRepository _userRepo = new UserRepository();

        private Panel pnlLeft, pnlRight;
        private TextBox txtUsername, txtPassword,
                           txtRegUser, txtRegFullName,
                           txtRegEmail, txtRegPass, txtRegConfirm;
        private Label lblLoginError, lblRegError, lblRegSuccess;
        private TabControl tabMain;

        // ── Win32 API for real placeholder support ────────────────
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg,
                                                  int wParam, string lParam);
        private const int EM_SETCUEBANNER = 0x1501;

        private void SetPlaceholder(TextBox txt, string hint)
        {
            SendMessage(txt.Handle, EM_SETCUEBANNER, 1, hint);
        }

        public LoginForm()
        {
            _userRepo.EnsureTable();
            InitUI();
        }

        private void InitUI()
        {
            // Icon
            if (Program.AppIcon != null)
                this.Icon = Program.AppIcon;
            this.Text = "HamTracker — Login";
            this.Size = new Size(920, 580);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.White;
            this.Font = new Font("Segoe UI", 9.5f);

            BuildLeftPanel();
            BuildRightPanel();

            // Set placeholders AFTER handles created
            this.Shown += OnFormShown;
        }

        private void OnFormShown(object sender, EventArgs e)
        {
            SetPlaceholder(txtUsername, "Enter your username");
            SetPlaceholder(txtPassword, "Enter your password");
            SetPlaceholder(txtRegFullName, "Your full name");
            SetPlaceholder(txtRegUser, "Choose a username");
            SetPlaceholder(txtRegEmail, "your@email.com");
            SetPlaceholder(txtRegPass, "Min 6 characters");
            SetPlaceholder(txtRegConfirm, "Repeat password");
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.SuspendLayout();
            // 
            // LoginForm
            // 
            this.ClientSize = new System.Drawing.Size(278, 244);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LoginForm";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.ResumeLayout(false);

        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }

        // ── Left branding panel ───────────────────────────────────
        private void BuildLeftPanel()
        {
            pnlLeft = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(380, 580),
                BackColor = Color.FromArgb(15, 110, 86)
            };

            pnlLeft.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var br = new SolidBrush(Color.FromArgb(20, 255, 255, 255)))
                {
                    g.FillEllipse(br, -60, -60, 200, 200);
                    g.FillEllipse(br, 220, 400, 220, 220);
                    g.FillEllipse(br, 280, -40, 130, 130);
                }
            };

            // Shield icon
            var picShield = new Panel
            {
                Location = new Point(140, 130),
                Size = new Size(100, 100),
                BackColor = Color.Transparent
            };
            picShield.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var br = new SolidBrush(Color.FromArgb(40, 255, 255, 255)))
                    g.FillEllipse(br, 0, 0, 100, 100);
                using (var pen = new Pen(Color.White, 3f))
                {
                    PointF[] shield = {
                        new PointF(50, 15), new PointF(85, 30),
                        new PointF(85, 55), new PointF(50, 85),
                        new PointF(15, 55), new PointF(15, 30)
                    };
                    g.DrawPolygon(pen, shield);
                    g.DrawLine(pen, 33, 50, 45, 62);
                    g.DrawLine(pen, 45, 62, 68, 38);
                }
            };

            var lblTitle = new Label
            {
                Text = "HamTracker",
                Font = new Font("Segoe UI", 24f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(0, 248),
                Size = new Size(380, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };
            var lblSub = new Label
            {
                Text = "Digital Evidence &\nProof Management System",
                Font = new Font("Segoe UI", 10.5f),
                ForeColor = Color.FromArgb(200, 255, 255, 255),
                Location = new Point(0, 292),
                Size = new Size(380, 50),
                TextAlign = ContentAlignment.MiddleCenter
            };

            string[] features = {
                "✓  Tamper-proof evidence hashing",
                "✓  Timeline activity tracking",
                "✓  PDF report generation",
                "✓  Dispute resolution ready"
            };
            int fy = 366;
            foreach (var f in features)
            {
                pnlLeft.Controls.Add(new Label
                {
                    Text = f,
                    Font = new Font("Segoe UI", 9f),
                    ForeColor = Color.FromArgb(210, 255, 255, 255),
                    Location = new Point(46, fy),
                    AutoSize = true
                });
                fy += 26;
            }

            pnlLeft.Controls.AddRange(new Control[]
                { picShield, lblTitle, lblSub });

            this.Controls.Add(pnlLeft);
        }

        // ── Right panel ───────────────────────────────────────────
        private void BuildRightPanel()
        {
            pnlRight = new Panel
            {
                Location = new Point(380, 0),
                Size = new Size(540, 580),
                BackColor = Color.White
            };

            tabMain = new TabControl
            {
                Location = new Point(0, 0),
                Size = new Size(540, 580),
                Appearance = TabAppearance.FlatButtons,
                ItemSize = new Size(0, 1),
                SizeMode = TabSizeMode.Fixed
            };

            var tabLogin = new TabPage { BackColor = Color.White };
            var tabRegister = new TabPage { BackColor = Color.White };

            BuildLoginTab(tabLogin);
            BuildRegisterTab(tabRegister);

            tabMain.TabPages.Add(tabLogin);
            tabMain.TabPages.Add(tabRegister);

            pnlRight.Controls.Add(tabMain);
            this.Controls.Add(pnlRight);
        }

        // ── LOGIN tab ─────────────────────────────────────────────
        private void BuildLoginTab(TabPage tab)
        {
            var lblWelcome = new Label
            {
                Text = "Welcome back",
                Font = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 20),
                Location = new Point(60, 70),
                AutoSize = true
            };
            var lblSub = new Label
            {
                Text = "Sign in to your HamTracker account",
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(150, 150, 150),
                Location = new Point(60, 108),
                AutoSize = true
            };

            var lblU = MakeFieldLabel("Username", 60, 160);
            txtUsername = MakeTextBox(60, 182, 420);

            var lblP = MakeFieldLabel("Password", 60, 228);
            txtPassword = MakeTextBox(60, 250, 390);
            txtPassword.UseSystemPasswordChar = true;

            var btnEye = new Button
            {
                Location = new Point(456, 250),
                Size = new Size(24, 34),
                FlatStyle = FlatStyle.Flat,
                Text = "👁",
                Font = new Font("Segoe UI", 10f),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
            btnEye.Click += (s, e) =>
            {
                txtPassword.UseSystemPasswordChar =
                    !txtPassword.UseSystemPasswordChar;
            };

            lblLoginError = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(200, 60, 60),
                Location = new Point(60, 294),
                Size = new Size(420, 20)
            };

            var btnLogin = MakePrimaryButton("Sign In", 60, 320, 420);
            btnLogin.Click += OnLogin;

            // Enter key
            txtPassword.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) OnLogin(null, null);
            };
            txtUsername.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) OnLogin(null, null);
            };

            var pnlDivider = new Panel
            {
                Location = new Point(60, 376),
                Size = new Size(420, 1),
                BackColor = Color.FromArgb(230, 230, 230)
            };

            var lblOr = new Label
            {
                Text = "Don't have an account?",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(170, 170, 170),
                Location = new Point(155, 388),
                AutoSize = true
            };

            var btnGoReg = new Button
            {
                Text = "Create new account",
                Location = new Point(60, 410),
                Size = new Size(420, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(15, 110, 86),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = {
                    BorderSize  = 1,
                    BorderColor = Color.FromArgb(29, 158, 117)
                }
            };
            btnGoReg.Click += (s, e) => tabMain.SelectedIndex = 1;
            btnGoReg.MouseEnter += (s, e) =>
            {
                btnGoReg.BackColor = Color.FromArgb(225, 245, 238);
            };
            btnGoReg.MouseLeave += (s, e) =>
            {
                btnGoReg.BackColor = Color.White;
            };

            tab.Controls.AddRange(new Control[]
            {
                lblWelcome, lblSub,
                lblU, txtUsername,
                lblP, txtPassword, btnEye,
                lblLoginError,
                btnLogin,
                pnlDivider, lblOr, btnGoReg
            });
        }

        // ── REGISTER tab ──────────────────────────────────────────
        private void BuildRegisterTab(TabPage tab)
        {
            var lblTitle = new Label
            {
                Text = "Create account",
                Font = new Font("Segoe UI", 22f, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 20),
                Location = new Point(60, 40),
                AutoSize = true
            };
            var lblSub = new Label
            {
                Text = "Join HamTracker as a freelancer or client",
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(150, 150, 150),
                Location = new Point(60, 78),
                AutoSize = true
            };

            var lblFN = MakeFieldLabel("Full Name", 60, 118);
            txtRegFullName = MakeTextBox(60, 138, 420);

            var lblUN = MakeFieldLabel("Username", 60, 180);
            txtRegUser = MakeTextBox(60, 200, 420);

            var lblEM = MakeFieldLabel("Email (optional)", 60, 242);
            txtRegEmail = MakeTextBox(60, 262, 420);

            var lblPW = MakeFieldLabel("Password", 60, 304);
            txtRegPass = MakeTextBox(60, 324, 420);
            txtRegPass.UseSystemPasswordChar = true;

            var lblCF = MakeFieldLabel("Confirm Password", 60, 366);
            txtRegConfirm = MakeTextBox(60, 386, 420);
            txtRegConfirm.UseSystemPasswordChar = true;

            lblRegError = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(200, 60, 60),
                Location = new Point(60, 430),
                Size = new Size(420, 18)
            };
            lblRegSuccess = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(15, 110, 86),
                Location = new Point(60, 430),
                Size = new Size(420, 18)
            };

            var btnReg = MakePrimaryButton("Create Account", 60, 454, 420);
            btnReg.Click += OnRegister;

            var btnBack = new Button
            {
                Text = "← Back to Sign In",
                Location = new Point(60, 506),
                Size = new Size(180, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(100, 100, 100),
                Font = new Font("Segoe UI", 9f),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
            btnBack.Click += (s, e) => tabMain.SelectedIndex = 0;
            btnBack.MouseEnter += (s, e) =>
                btnBack.ForeColor = Color.FromArgb(15, 110, 86);
            btnBack.MouseLeave += (s, e) =>
                btnBack.ForeColor = Color.FromArgb(100, 100, 100);

            tab.Controls.AddRange(new Control[]
            {
                lblTitle, lblSub,
                lblFN, txtRegFullName,
                lblUN, txtRegUser,
                lblEM, txtRegEmail,
                lblPW, txtRegPass,
                lblCF, txtRegConfirm,
                lblRegError, lblRegSuccess,
                btnReg, btnBack
            });
        }

        // ── Event handlers ────────────────────────────────────────
        private void OnLogin(object sender, EventArgs e)
        {
            lblLoginError.Text = "";

            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblLoginError.Text = "Please enter username and password.";
                return;
            }

            var user = _userRepo.Login(txtUsername.Text, txtPassword.Text);
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

        private void OnRegister(object sender, EventArgs e)
        {
            lblRegError.Text = "";
            lblRegSuccess.Text = "";

            if (string.IsNullOrWhiteSpace(txtRegFullName.Text) ||
                string.IsNullOrWhiteSpace(txtRegUser.Text) ||
                string.IsNullOrWhiteSpace(txtRegPass.Text))
            {
                lblRegError.Text = "Full name, username and password are required.";
                return;
            }
            if (txtRegPass.Text.Length < 6)
            {
                lblRegError.Text = "Password must be at least 6 characters.";
                return;
            }
            if (txtRegPass.Text != txtRegConfirm.Text)
            {
                lblRegError.Text = "Passwords do not match.";
                return;
            }

            var user = new User
            {
                FullName = txtRegFullName.Text.Trim(),
                Username = txtRegUser.Text.Trim(),
                Email = txtRegEmail.Text.Trim(),
                Password = txtRegPass.Text,
                Role = "Freelancer"
            };

            bool ok = _userRepo.Register(user);
            if (ok)
            {
                lblRegSuccess.Text = "Account created! You can now sign in.";
                lblRegError.Text = "";

                var timer = new Timer { Interval = 1500 };
                timer.Tick += (ts, te) =>
                {
                    timer.Stop();
                    tabMain.SelectedIndex = 0;
                    txtUsername.Text = txtRegUser.Text;
                    txtPassword.Focus();
                    lblRegSuccess.Text = "";
                    txtRegFullName.Clear();
                    txtRegUser.Clear();
                    txtRegEmail.Clear();
                    txtRegPass.Clear();
                    txtRegConfirm.Clear();
                };
                timer.Start();
            }
            else
            {
                lblRegError.Text = "Username already taken. Choose another.";
            }
        }

        // ── UI helpers ────────────────────────────────────────────
        private Label MakeFieldLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 80, 80),
                Location = new Point(x, y),
                AutoSize = true
            };
        }

        private TextBox MakeTextBox(int x, int y, int w)
        {
            var txt = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(w, 34),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10f),
                BackColor = Color.FromArgb(250, 250, 250)
            };
            txt.Enter += (s, e) => txt.BackColor = Color.White;
            txt.Leave += (s, e) => txt.BackColor = Color.FromArgb(250, 250, 250);
            return txt;
        }

        private Button MakePrimaryButton(string text, int x, int y, int w)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, 44),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(29, 158, 117),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(15, 110, 86);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(29, 158, 117);
            return btn;
        }
    }
}