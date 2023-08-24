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
    public partial class UserUserControl : UserControl
    {
        private readonly UserMainInfoModel _userMainInfoModel;
        private readonly FirebaseManagement _firebaseManagement;
        private readonly User _user;

        public UserUserControl(UserMainInfoModel userMainInfoModel, FirebaseManagement firebaseManagement, User user)
        {
            InitializeComponent();

            _userMainInfoModel = userMainInfoModel;
            _firebaseManagement = firebaseManagement;
            _user = user;
        }

        private void UserUserControl_Load(object sender, EventArgs e)
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

            if (_user.Status)
            {
                labelStatus.Text = "Online";
                panelStatusColor.BackColor = Color.Green;
            }
            else
            {
                labelStatus.Text = "Offline";
                panelStatusColor.BackColor = Color.Silver;
            }
        }

        private async void UserUserControl_Click(object sender, EventArgs e)
        {
            if(await _firebaseManagement.CreateChat(_user.Key))
            {
                ChatForm chatForm = (ChatForm)Application.OpenForms["ChatForm"];
                if (chatForm != null)
                {
                    chatForm.Chats();
                }
            }
        }
    }
}
