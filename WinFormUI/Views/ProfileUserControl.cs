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
using WinFormUI.Core;

namespace WinFormUI.Views
{
    public partial class ProfileUserControl : UserControl
    {
        private readonly FirebaseManagement _firebaseManagement;

        public ProfileUserControl(FirebaseManagement firebaseManagement)
        {
            InitializeComponent();

            _firebaseManagement = firebaseManagement;
        }

        private void ProfileUserControl_Load(object sender, EventArgs e)
        {
            if (_firebaseManagement.CurrentUserMainModel.Users[0].PhotoUrl!=""&& _firebaseManagement.CurrentUserMainModel.Users[0].PhotoUrl != null)
            {
                pictureBoxAvatar.Load(_firebaseManagement.CurrentUserMainModel.Users[0].PhotoUrl);
            }

            if (_firebaseManagement.CurrentUserMainModel.Users[0].DisplayName != "" && _firebaseManagement.CurrentUserMainModel.Users[0].DisplayName != null)
            {
                labelUsername.Text = _firebaseManagement.CurrentUserMainModel.Users[0].DisplayName;
            }
            else
            {
                labelUsername.Text = _firebaseManagement.CurrentUserMainModel.Users[0].Email;
            }
        }

        private void linkLabelEditProfile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DashboardForm dashboardForm = (DashboardForm)Application.OpenForms["DashboardForm"];
            if (dashboardForm != null)
            {
                NavigationManager.OpenForm(new EditProfileForm(_firebaseManagement), DockStyle.Fill, dashboardForm.panelMain);
            }
        }

        private void buttonFriends_Click(object sender, EventArgs e)
        {
            DashboardForm dashboardForm = (DashboardForm)Application.OpenForms["DashboardForm"];
            if (dashboardForm != null)
            {
                NavigationManager.OpenForm(new FriendsForm(_firebaseManagement), DockStyle.Fill, dashboardForm.panelMain);
            }
        }
    }
}
