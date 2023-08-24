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

namespace WinFormUI.Views
{
    public partial class ChatForm : Form
    {
        private readonly FirebaseManagement _firebaseManagement;

        public ChatForm(FirebaseManagement firebaseManagement)
        {
            InitializeComponent();

            _firebaseManagement = firebaseManagement;
        }

        private void ChatForm_Load(object sender, EventArgs e)
        {
            Chats();
        }

        public async void Chats()
        {
            var result = await _firebaseManagement.GetChats();
            if (result != null)
            {
                flowLayoutPanelChats.Controls.Clear();
                foreach (var item in result)
                {
                    string receiverEmail = await _firebaseManagement.GetUserEmailWithUserId(item.ChatReceiverUserId);
                    UserMainInfoModel userMainInfoModel = await _firebaseManagement.SearchUserByEmail(receiverEmail);
                    if (userMainInfoModel != null)
                    {
                        flowLayoutPanelChats.Controls.Add(new ChatUserControl(userMainInfoModel, _firebaseManagement, item));
                    }
                }
            }
        }

        private void timerUpdateInterface_Tick(object sender, EventArgs e)
        {
            Chats();
        }
    }
}
