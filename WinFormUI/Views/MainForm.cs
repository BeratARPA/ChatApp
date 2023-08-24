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
    public partial class MainForm : Form
    {
        private readonly FirebaseManagement _firebaseManagement;

        public MainForm(FirebaseManagement firebaseManagement)
        {
            InitializeComponent();

            _firebaseManagement = firebaseManagement;

            NavigationManager.OpenForm(new SignInForm(_firebaseManagement), DockStyle.Top, panelMain);
        }

        public int pageNumber = 0;

        private void MainForm_Load(object sender, EventArgs e)
        {
            AutomaticSignIn();
        }

        public async void AutomaticSignIn()
        {
            if (await _firebaseManagement.AutomaticSignIn())
            {
                NavigationManager.OpenForm(new DashboardForm(_firebaseManagement), DockStyle.Fill, panelMain);

                linkLabelNavigate.Text = "Sign Out";
                pageNumber = 1;
            }
        }

        private Form GetForm(int number)
        {
            switch (number)
            {
                case 0:
                    pageNumber = 1;
                    linkLabelNavigate.Text = "Sign In";
                    return new SignUpForm(_firebaseManagement);
                case 1:
                    pageNumber = 0;
                    linkLabelNavigate.Text = "Sign Up";

                    _firebaseManagement.SignOut();

                    ChatForm chatForm = (ChatForm)Application.OpenForms["ChatForm"];
                    if (chatForm != null)
                        chatForm.Close();

                    DashboardForm dashboardForm = (DashboardForm)Application.OpenForms["DashboardForm"];
                    if (dashboardForm != null)
                        dashboardForm.Close();

                    return new SignInForm(_firebaseManagement);
                default:
                    return new SignInForm(_firebaseManagement);
            }
        }

        private void linkLabelNavigate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            NavigationManager.OpenForm(GetForm(pageNumber), DockStyle.Top, panelMain);
        }

        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            await _firebaseManagement.UpdateUserStatus(false);

            e.Cancel = false;

            Environment.Exit(0);
        }
    }
}
