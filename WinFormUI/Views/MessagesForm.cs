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
    public partial class MessagesForm : Form
    {
        private readonly UserMainInfoModel _userMainInfoModel;
        private readonly FirebaseManagement _firebaseManagement;
        private readonly Chat _chat;

        public MessagesForm(UserMainInfoModel userMainInfoModel, FirebaseManagement firebaseManagement, Chat chat)
        {
            InitializeComponent();

            _userMainInfoModel = userMainInfoModel;
            _firebaseManagement = firebaseManagement;
            _chat = chat;
        }

        private void MessagesForm_Load(object sender, EventArgs e)
        {
            _firebaseManagement.MessageKeys.Clear();
            Messages();
        }

        private async void LastMessages()
        {
            var result = await _firebaseManagement.GetLastMessages(_chat.Key);
            if (result != null)
            {
                if (_firebaseManagement.MessageKeys.Where(x => x.Contains(result.Key)).Count() == 0)
                {
                    AddText(result);

                    _firebaseManagement.MessageKeys.Add(result.Key);
                }
            }
        }

        private async void Messages()
        {
            var result = await _firebaseManagement.GetMessages(_chat.Key);
            if (result != null)
            {
                richTextBoxMessages.Clear();
                foreach (var item in result)
                {
                    AddText(item);

                    _firebaseManagement.MessageKeys.Add(item.Key);
                }
            }
        }

        public void AddText(Database.Models.Message message)
        {
            try
            {
                if (message.MessageSenderDisplayName == _firebaseManagement.CurrentUserInfoModel.DisplayName)
                {
                    richTextBoxMessages.AppendText($"(You) : {message.Text}\n\n");
                }
                else
                {
                    richTextBoxMessages.AppendText($"({message.MessageSenderDisplayName}) : {message.Text}\n\n");
                }
            }
            catch { }
        }

        private async void buttonSend_Click(object sender, EventArgs e)
        {
            if (richTextBoxtText.Text != "")
            {
                string message = richTextBoxtText.Text;

                richTextBoxtText.Clear();
                richTextBoxtText.Focus();

                await _firebaseManagement.SendMessage(_userMainInfoModel.DisplayName, _chat.ChatId, _chat.ChatReceiverUserId, message);

                ScrollToBottom();
            }
        }

        private void ScrollToBottom()
        {
            try
            {
                richTextBoxMessages.SelectionStart = richTextBoxMessages.Text.Length;
                richTextBoxMessages.ScrollToCaret();
            }
            catch { }
        }

        private void timerUpdateInterface_Tick(object sender, EventArgs e)
        {
            LastMessages();
        }
    }
}
