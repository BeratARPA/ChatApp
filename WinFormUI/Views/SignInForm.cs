using Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormUI.Core;

namespace WinFormUI.Views
{
    public partial class SignInForm : Form
    {
        private readonly FirebaseManagement _firebaseManagement;

        public SignInForm(FirebaseManagement firebaseManagement)
        {
            InitializeComponent();

            _firebaseManagement = firebaseManagement;
        }

        private async void buttonSignIn_Click(object sender, EventArgs e)
        {
            if (textBoxEmail.Text != "" && textBoxPassword.Text != "")
            {
                if (CheckEmail.Check(textBoxEmail.Text))
                {
                    if (await _firebaseManagement.SignIn(textBoxEmail.Text, textBoxPassword.Text, checkBoxRememberMe.Checked) != null)
                    {
                        textBoxEmail.Clear();
                        textBoxPassword.Clear();

                        MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];
                        if (mainForm != null)
                        {
                            NavigationManager.OpenForm(new DashboardForm(_firebaseManagement), DockStyle.Fill, mainForm.panelMain);

                            mainForm.linkLabelNavigate.Text = "Sign Out";
                            mainForm.pageNumber = 1;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("E-Mail not correct!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void checkBoxShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxShowPassword.Checked)
            {
                textBoxPassword.UseSystemPasswordChar = false;
            }
            else
            {
                textBoxPassword.UseSystemPasswordChar = true;
            }
        }

        private void linkLabelForgetYourPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MainForm mainForm = (MainForm)Application.OpenForms["MainForm"];
            if (mainForm != null)
            {
                NavigationManager.OpenForm(new ForgetYourPasswordForm(_firebaseManagement), DockStyle.Top, mainForm.panelMain);

                mainForm.linkLabelNavigate.Text = "Back";
                mainForm.pageNumber = 1;
            }
        }
    }
}
