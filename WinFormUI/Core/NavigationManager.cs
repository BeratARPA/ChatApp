using System.Windows.Forms;

namespace WinFormUI.Core
{
    public class NavigationManager
    {
        public static void OpenForm(Form form, DockStyle dockStyle, Panel parentPanel)
        {
            FormCollection formCollection = Application.OpenForms;
            for (int i = formCollection.Count - 1; i >= 0; i--)
            {
                if (formCollection[i].Name != "MainForm" && formCollection[i].Name != "DashboardForm" && formCollection[i].Name != "ChatForm")
                {
                    formCollection[i].Close();
                }
            }

            parentPanel.Controls.Clear();

            Form formSearch = Application.OpenForms[form.Name];
            if (formSearch != null)
            {
                formSearch.Dock = dockStyle;
                formSearch.TopLevel = false;
                formSearch.TopMost = true;
                formSearch.Show();

                parentPanel.Controls.Add(formSearch);
            }
            else
            {
                form.Dock = dockStyle;
                form.TopLevel = false;
                form.TopMost = true;
                form.Show();

                parentPanel.Controls.Add(form);
            }
        }

    }
}
