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
    public partial class SignUpForm : Form
    {
        private readonly FirebaseManagement _firebaseManagement;

        public SignUpForm(FirebaseManagement firebaseManagement)
        {
            InitializeComponent();

            _firebaseManagement = firebaseManagement;
        }

        private async void buttonSignUp_Click(object sender, EventArgs e)
        {
            if (textBoxEmail.Text != "" && textBoxPassword.Text != "")
            {
                if (CheckEmail.Check(textBoxEmail.Text))
                {
                    if (await _firebaseManagement.SignUp(textBoxEmail.Text, textBoxPassword.Text) != null)
                    {
                        textBoxEmail.Clear();
                        textBoxPassword.Clear();
                        MessageBox.Show("Sign up successful!");
                    }
                }
                else
                {
                    MessageBox.Show("E-mail not correct!");
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
    }
}
