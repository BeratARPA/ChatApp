using Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormUI.Views
{
    public partial class EditProfileForm : Form
    {
        private readonly FirebaseManagement _firebaseManagement;

        public EditProfileForm(FirebaseManagement firebaseManagement)
        {
            InitializeComponent();

            _firebaseManagement = firebaseManagement;
        }

        private void EditProfileForm_Load(object sender, EventArgs e)
        {
            InitilizeProfile();
        }

        private void InitilizeProfile()
        {
            textBoxDisplayName.Text = _firebaseManagement.CurrentUserInfoModel.DisplayName;
            textBoxEMail.Text = _firebaseManagement.CurrentUserMainModel.Users[0].Email;
            textBoxPassword.Text = _firebaseManagement.CurrentUserMainModel.Users[0].Password;

            if (_firebaseManagement.CurrentUserMainModel.Users[0].PhotoUrl != "" && _firebaseManagement.CurrentUserMainModel.Users[0].PhotoUrl != null)
            {
                pictureBoxAvatar.Load(_firebaseManagement.CurrentUserMainModel.Users[0].PhotoUrl);
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

        private void GoChatForm()
        {
            DashboardForm dashboardForm = (DashboardForm)Application.OpenForms["DashboardForm"];
            if (dashboardForm != null)
            {
                dashboardForm.CreateChatForm();
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
            GoChatForm();
        }

        private async void buttonSave_Click(object sender, EventArgs e)
        {
            string displayName = textBoxDisplayName.Text != "" ? textBoxDisplayName.Text : _firebaseManagement.CurrentUserInfoModel.DisplayName;
            string photoUrl = pictureBoxAvatar.ImageLocation != "" ? pictureBoxAvatar.ImageLocation : _firebaseManagement.CurrentUserMainModel.Users[0].PhotoUrl;
            string password = textBoxPassword.Text != "" ? textBoxPassword.Text : _firebaseManagement.CurrentUserMainModel.Users[0].Password;

            await _firebaseManagement.UpdateUser(_firebaseManagement.CurrentUserInfoModel.IdToken, displayName, photoUrl, password);

            this.Close();
            GoChatForm();
        }

        private void buttonLoadAvatar_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPG|*.jpg|PNG|*.png";
            openFileDialog.Title = "Select avatar";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pictureBoxAvatar.Load(openFileDialog.FileName);
            }
        }
    }
}
