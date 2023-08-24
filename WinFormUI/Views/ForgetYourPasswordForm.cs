using Database;
using Database.Models;
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
    public partial class ForgetYourPasswordForm : Form
    {
        private readonly FirebaseManagement _firebaseManagement;

        public ForgetYourPasswordForm(FirebaseManagement firebaseManagement)
        {
            InitializeComponent();

            _firebaseManagement = firebaseManagement;
        }

        private async void buttonSend_Click(object sender, EventArgs e)
        {
            if (textBoxEmail.Text != "")
            {
                if (CheckEmail.Check(textBoxEmail.Text))
                {
                    await _firebaseManagement.SendOobCode(OobReqType.PASSWORD_RESET.ToString(), textBoxEmail.Text);

                    textBoxEmail.Clear();

                    MessageBox.Show("Password reset email send successfully. Check your email to reset.", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("E-mail not correct!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}
