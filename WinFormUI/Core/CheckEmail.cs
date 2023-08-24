using System.Net.Mail;

namespace WinFormUI.Core
{
    public class CheckEmail
    {
        public static bool Check(string email)
        {
			try
			{				
				MailAddress mailAddress = new MailAddress(email);
				return true;
			}
			catch
			{
				return false;
			}
        }
    }
}
