namespace HamTracker.Forms
{
    partial class RegisterForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblFNLbl = new System.Windows.Forms.Label();
            this.txtFullName = new System.Windows.Forms.TextBox();
            this.lblUNLbl = new System.Windows.Forms.Label();
            this.txtRegUser = new System.Windows.Forms.TextBox();
            this.lblEMLbl = new System.Windows.Forms.Label();
            this.txtRegEmail = new System.Windows.Forms.TextBox();
            this.lblPWLbl = new System.Windows.Forms.Label();
            this.txtRegPass = new System.Windows.Forms.TextBox();
            this.lblCFLbl = new System.Windows.Forms.Label();
            this.txtRegConfirm = new System.Windows.Forms.TextBox();
            this.lblRegError = new System.Windows.Forms.Label();
            this.lblRegSuccess = new System.Windows.Forms.Label();
            this.btnRegister = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(20, 20, 20);
            this.lblTitle.Location = new System.Drawing.Point(30, 20);
            this.lblTitle.AutoSize = true;
            this.lblTitle.Text = "Create account";

            // lblFNLbl
            this.lblFNLbl.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblFNLbl.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
            this.lblFNLbl.Location = new System.Drawing.Point(30, 70);
            this.lblFNLbl.AutoSize = true;
            this.lblFNLbl.Text = "Full Name";

            // txtFullName
            this.txtFullName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFullName.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtFullName.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            this.txtFullName.Location = new System.Drawing.Point(30, 90);
            this.txtFullName.Size = new System.Drawing.Size(380, 30);

            // lblUNLbl
            this.lblUNLbl.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblUNLbl.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
            this.lblUNLbl.Location = new System.Drawing.Point(30, 132);
            this.lblUNLbl.AutoSize = true;
            this.lblUNLbl.Text = "Username";

            // txtRegUser
            this.txtRegUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRegUser.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtRegUser.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            this.txtRegUser.Location = new System.Drawing.Point(30, 152);
            this.txtRegUser.Size = new System.Drawing.Size(380, 30);

            // lblEMLbl
            this.lblEMLbl.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblEMLbl.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
            this.lblEMLbl.Location = new System.Drawing.Point(30, 194);
            this.lblEMLbl.AutoSize = true;
            this.lblEMLbl.Text = "Email (optional)";

            // txtRegEmail
            this.txtRegEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRegEmail.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtRegEmail.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            this.txtRegEmail.Location = new System.Drawing.Point(30, 214);
            this.txtRegEmail.Size = new System.Drawing.Size(380, 30);

            // lblPWLbl
            this.lblPWLbl.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblPWLbl.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
            this.lblPWLbl.Location = new System.Drawing.Point(30, 256);
            this.lblPWLbl.AutoSize = true;
            this.lblPWLbl.Text = "Password";

            // txtRegPass
            this.txtRegPass.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRegPass.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtRegPass.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            this.txtRegPass.UseSystemPasswordChar = true;
            this.txtRegPass.Location = new System.Drawing.Point(30, 276);
            this.txtRegPass.Size = new System.Drawing.Size(380, 30);

            // lblCFLbl
            this.lblCFLbl.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblCFLbl.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);
            this.lblCFLbl.Location = new System.Drawing.Point(30, 318);
            this.lblCFLbl.AutoSize = true;
            this.lblCFLbl.Text = "Confirm Password";

            // txtRegConfirm
            this.txtRegConfirm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRegConfirm.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtRegConfirm.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            this.txtRegConfirm.UseSystemPasswordChar = true;
            this.txtRegConfirm.Location = new System.Drawing.Point(30, 338);
            this.txtRegConfirm.Size = new System.Drawing.Size(380, 30);

            // lblRegError
            this.lblRegError.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblRegError.ForeColor = System.Drawing.Color.FromArgb(200, 60, 60);
            this.lblRegError.Location = new System.Drawing.Point(30, 378);
            this.lblRegError.Size = new System.Drawing.Size(380, 18);
            this.lblRegError.Text = "";

            // lblRegSuccess
            this.lblRegSuccess.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblRegSuccess.ForeColor = System.Drawing.Color.FromArgb(15, 110, 86);
            this.lblRegSuccess.Location = new System.Drawing.Point(30, 378);
            this.lblRegSuccess.Size = new System.Drawing.Size(380, 18);
            this.lblRegSuccess.Text = "";

            // btnRegister
            this.btnRegister.BackColor = System.Drawing.Color.FromArgb(29, 158, 117);
            this.btnRegister.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegister.FlatAppearance.BorderSize = 0;
            this.btnRegister.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnRegister.ForeColor = System.Drawing.Color.White;
            this.btnRegister.Location = new System.Drawing.Point(30, 404);
            this.btnRegister.Size = new System.Drawing.Size(380, 40);
            this.btnRegister.Text = "Create Account";
            this.btnRegister.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);

            // btnBack
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.FlatAppearance.BorderSize = 0;
            this.btnBack.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnBack.ForeColor = System.Drawing.Color.FromArgb(100, 100, 100);
            this.btnBack.Location = new System.Drawing.Point(30, 454);
            this.btnBack.Size = new System.Drawing.Size(180, 30);
            this.btnBack.Text = "← Back to Sign In";
            this.btnBack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);

            // RegisterForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(440, 500);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnRegister);
            this.Controls.Add(this.lblRegSuccess);
            this.Controls.Add(this.lblRegError);
            this.Controls.Add(this.txtRegConfirm);
            this.Controls.Add(this.lblCFLbl);
            this.Controls.Add(this.txtRegPass);
            this.Controls.Add(this.lblPWLbl);
            this.Controls.Add(this.txtRegEmail);
            this.Controls.Add(this.lblEMLbl);
            this.Controls.Add(this.txtRegUser);
            this.Controls.Add(this.lblUNLbl);
            this.Controls.Add(this.txtFullName);
            this.Controls.Add(this.lblFNLbl);
            this.Controls.Add(this.lblTitle);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RegisterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create Account — HamTracker";
            this.Shown += new System.EventHandler(this.OnFormShown);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblFNLbl;
        private System.Windows.Forms.TextBox txtFullName;
        private System.Windows.Forms.Label lblUNLbl;
        private System.Windows.Forms.TextBox txtRegUser;
        private System.Windows.Forms.Label lblEMLbl;
        private System.Windows.Forms.TextBox txtRegEmail;
        private System.Windows.Forms.Label lblPWLbl;
        private System.Windows.Forms.TextBox txtRegPass;
        private System.Windows.Forms.Label lblCFLbl;
        private System.Windows.Forms.TextBox txtRegConfirm;
        private System.Windows.Forms.Label lblRegError;
        private System.Windows.Forms.Label lblRegSuccess;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.Button btnBack;
    }
}