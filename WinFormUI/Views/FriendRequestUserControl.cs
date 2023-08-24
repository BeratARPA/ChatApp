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
    public partial class FriendRequestUserControl : UserControl
    {
        private readonly UserMainInfoModel _userMainInfoModel;
        private readonly FirebaseManagement _firebaseManagement;
        private readonly FriendRequest _friendRequest;

        public FriendRequestUserControl(UserMainInfoModel userMainInfoModel, FirebaseManagement firebaseManagement, FriendRequest friendRequest)
        {
            InitializeComponent();

            _userMainInfoModel = userMainInfoModel;
            _firebaseManagement = firebaseManagement;
            _friendRequest = friendRequest;
        }

        private void FriendRequestUserControl_Load(object sender, EventArgs e)
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

        private async void buttonReject_Click(object sender, EventArgs e)
        {
            if(await _firebaseManagement.RejectFriendRequest(_friendRequest))
            {
                this.Dispose();
            }
        }

        private async void buttonAccept_Click(object sender, EventArgs e)
        {
            if (await _firebaseManagement.AcceptFriendRequest(_friendRequest))
            {
                this.Dispose();
            }
        }
    }
}
