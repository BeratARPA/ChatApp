using Database;
using Database.Models;
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
    public partial class AddFriendUserControl : UserControl
    {
        private readonly UserMainInfoModel _userMainInfoModel;
        private readonly FirebaseManagement _firebaseManagement;

        public AddFriendUserControl(UserMainInfoModel userMainInfoModel, FirebaseManagement firebaseManagement)
        {
            InitializeComponent();

            _userMainInfoModel = userMainInfoModel;
            _firebaseManagement = firebaseManagement;
        }

        private void AddFriendUserControl_Load(object sender, EventArgs e)
        {
            if (_userMainInfoModel.PhotoUrl != "" && _userMainInfoModel.PhotoUrl != null)
            {
                pictureBoxAvatar.Load(_userMainInfoModel.PhotoUrl);
            }     

            if (_userMainInfoModel.DisplayName != "" && _userMainInfoModel.DisplayName != null)
            {
                labelUsername.Text = _userMainInfoModel.DisplayName;
            }
            else
            {
                labelUsername.Text = _userMainInfoModel.Email;
            }
        }

        private async void buttonAddFriend_Click(object sender, EventArgs e)
        {
            if (await _firebaseManagement.SendFriendshipRequest(_userMainInfoModel.Email))
            {
                MessageBox.Show("Friend added!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
