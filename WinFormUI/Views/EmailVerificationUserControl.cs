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
    public partial class EmailVerificationUserControl : UserControl
    {
        private readonly FirebaseManagement _firebaseManagement;

        public EmailVerificationUserControl(FirebaseManagement firebaseManagement)
        {
            InitializeComponent();

            _firebaseManagement = firebaseManagement;
        }

        private async void buttonVerify_Click(object sender, EventArgs e)
        {
            if (!_firebaseManagement.CurrentUserMainModel.Users[0].EmailVerified)
            {
                if (CheckEmail.Check(_firebaseManagement.CurrentUserMainModel.Users[0].Email))
                {
                    await _firebaseManagement.SendOobCode(OobReqType.VERIFY_EMAIL.ToString(), _firebaseManagement.CurrentUserMainModel.Users[0].Email, _firebaseManagement.CurrentUserInfoModel.IdToken);

                    MessageBox.Show("Successfully send email verification email. Check your email to verify.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("E-mail not correct!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}
