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
    public partial class FriendsForm : Form
    {
        private readonly FirebaseManagement _firebaseManagement;

        public FriendsForm(FirebaseManagement firebaseManagement)
        {
            InitializeComponent();

            _firebaseManagement = firebaseManagement;
        }

        private void FriendsForm_Load(object sender, EventArgs e)
        {
            FriendRequest();
            FriendList();
        }

        private async void FriendRequest()
        {
            var result = await _firebaseManagement.GetFriendRequests();
            if (result != null)
            {
                flowLayoutPanelFriends.Controls.Clear();
                foreach (var item in result)
                {
                    UserMainInfoModel userMainInfoModel = await _firebaseManagement.SearchUserByEmail(item.SenderEmail);
                    if (userMainInfoModel != null)
                    {
                        flowLayoutPanelFriends.Controls.Add(new FriendRequestUserControl(userMainInfoModel, _firebaseManagement, item));
                    }
                }
            }
        }

        private async void FriendList()
        {
            var result = await _firebaseManagement.GetFriends();
            if (result != null)
            {
                flowLayoutPanelFriends.Controls.Clear();
                foreach (var item in result)
                {
                    UserMainInfoModel userMainInfoModel = await _firebaseManagement.SearchUserByEmail(item.Email);
                    if (userMainInfoModel != null)
                    {
                        flowLayoutPanelFriends.Controls.Add(new UserUserControl(userMainInfoModel, _firebaseManagement, item));
                    }
                }
            }
        }

        private async void FindUser()
        {
            var result = await _firebaseManagement.SearchUserByEmail(textBoxFindUser.Text);
            if (result != null)
            {
                flowLayoutPanelFriends.Controls.Clear();
                flowLayoutPanelFriends.Controls.Add(new AddFriendUserControl(result, _firebaseManagement));
            }
            else
            {
                flowLayoutPanelFriends.Controls.Clear();
                flowLayoutPanelFriends.Controls.Add(new Label { ForeColor = Color.White, Font = new Font("Microsoft Sans Serif", 15), Text = "User not found", AutoSize = true });
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

        private void textBoxFindUser_TextChanged(object sender, EventArgs e)
        {
            if (textBoxFindUser.Text.Length == 0)
            {
                flowLayoutPanelFriends.Controls.Clear();
                FriendRequest();
                FriendList();
            }
            else
            {
                FindUser();
            }
        }
    }
}
