using Database;
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
    public partial class DashboardForm : Form
    {
        private readonly FirebaseManagement _firebaseManagement;

        public DashboardForm(FirebaseManagement firebaseManagement)
        {
            InitializeComponent();

            _firebaseManagement = firebaseManagement;
        }

        private void DashboardForm_Load(object sender, EventArgs e)
        {
            CreateProfileUserControl();
            CreateEmailVerificationUserControl();
            CreateChatForm();
        }

        private void CreateProfileUserControl()
        {
            panelTopTwo.Controls.Add(new ProfileUserControl(_firebaseManagement) { Dock = DockStyle.Fill });
        }

        private void CreateEmailVerificationUserControl()
        {
            if (!_firebaseManagement.CurrentUserMainModel.Users[0].EmailVerified)
            {
                panelTopOne.Controls.Add(new EmailVerificationUserControl(_firebaseManagement) { Dock = DockStyle.Fill });
                tableLayoutPanel1.Enabled = false;     
            }
            else
            {
                tableLayoutPanelMain.RowStyles[0].Height = 0;
            }
        }

        public void CreateChatForm()
        {
            NavigationManager.OpenForm(new ChatForm(_firebaseManagement), DockStyle.Fill, panelMain);
        }
    }
}
