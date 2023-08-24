using System.IO;
using System.Reflection;

namespace Database
{
    public class FirebaseConfig
    {
        public static string ProjectId = "chatapp-6923d";
        public static string WebApiKey = "AIzaSyD9oqJ_WGO_UD5JwLUodnUUWuQwAeidxqg";
        public static string BaseUrl= "https://chatapp-6923d-default-rtdb.europe-west1.firebasedatabase.app/";
        public static string AuthBaseUrl = "https://identitytoolkit.googleapis.com/v1/";
        public static string ServiceAccountFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "chatapp-6923d-firebase-adminsdk-qzpgl-820bf4644e.json");
    }
}
