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
using WinFormUI.Core;

namespace WinFormUI.Views
{
    public partial class ChatUserControl : UserControl
    {
        private readonly UserMainInfoModel _userMainInfoModel;
        private readonly FirebaseManagement _firebaseManagement;
        private readonly Chat _chat;

        public ChatUserControl(UserMainInfoModel userMainInfoModel, FirebaseManagement firebaseManagement, Chat chat)
        {
            InitializeComponent();

            _userMainInfoModel = userMainInfoModel;
            _firebaseManagement = firebaseManagement;
            _chat = chat;
        }

        private void ChatUserControl_Load(object sender, EventArgs e)
        {
            if (_userMainInfoModel.PhotoUrl != "" && _userMainInfoModel.PhotoUrl != null)
            {
                //pictureBoxAvatar.Load(_userMainInfoModel.PhotoUrl);
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

        private void ChatUserControl_Click(object sender, EventArgs e)
        {
            ChatForm chatform = (ChatForm)Application.OpenForms["ChatForm"];
            if (chatform != null)
            {
                NavigationManager.OpenForm(new MessagesForm(_userMainInfoModel, _firebaseManagement, _chat), DockStyle.Fill, chatform.panelMain);
            }
        }
    }
}
