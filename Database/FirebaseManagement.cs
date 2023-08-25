using Database.Core;
using Database.Models;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Database
{
    public class FirebaseManagement
    {
        public async Task<string> GetDatabaseFriendRequestsEndpoint(string userEmail)
        {
            string userId = await GetUserIdWithUserEmail(userEmail);
            return $"{FirebaseConfig.BaseUrl}Users/{userId}/FriendRequests.json";
        }

        public async Task<string> GetDatabaseKeyFriendRequestEndpoint(string userEmail, FriendRequest friendRequest)
        {
            string userId = await GetUserIdWithUserEmail(userEmail);
            return $"{FirebaseConfig.BaseUrl}Users/{userId}/FriendRequests/{friendRequest.Key}.json";
        }

        public async Task<string> GetDatabaseFriendsEndpoint(string userEmail)
        {
            string userId = await GetUserIdWithUserEmail(userEmail);
            return $"{FirebaseConfig.BaseUrl}Users/{userId}/Friends.json";
        }

        public async Task<string> GetDatabaseUserStatusEndpoint(string userEmail)
        {
            string userId = await GetUserIdWithUserEmail(userEmail);
            return $"{FirebaseConfig.BaseUrl}Users/{userId}.json";
        }

        public async Task<string> GetDatabaseChatsEndpoint(string userEmail)
        {
            string userId = await GetUserIdWithUserEmail(userEmail);
            return $"{FirebaseConfig.BaseUrl}Users/{userId}/Chats.json";
        }

        public async Task<string> GetDatabaseChatsMessagesEndpoint(string userEmail, string chatId)
        {
            string userId = await GetUserIdWithUserEmail(userEmail);
            return $"{FirebaseConfig.BaseUrl}Users/{userId}/Chats/{chatId}/Messages.json";
        }

        private readonly string DatabaseUsersEndpoint = $"{FirebaseConfig.BaseUrl}Users.json";

        private readonly string SignUpEndpoint = $"{FirebaseConfig.AuthBaseUrl}accounts:signUp?key={FirebaseConfig.WebApiKey}";
        private readonly string UserInfoEndpoint = $"{FirebaseConfig.AuthBaseUrl}accounts:lookup?key={FirebaseConfig.WebApiKey}";
        private readonly string SignInEndpoint = $"{FirebaseConfig.AuthBaseUrl}accounts:signInWithPassword?key={FirebaseConfig.WebApiKey}";
        private readonly string SendOobCodeEndpoint = $"{FirebaseConfig.AuthBaseUrl}accounts:sendOobCode?key={FirebaseConfig.WebApiKey}";
        private readonly string UpdateUserEndpoint = $"{FirebaseConfig.AuthBaseUrl}accounts:update?key={FirebaseConfig.WebApiKey}";
        private readonly string UsersEndpoint = $"{FirebaseConfig.AuthBaseUrl}projects/{FirebaseConfig.ProjectId}/accounts:batchGet";

        public UserMainModel CurrentUserMainModel { get; set; }
        public UserInfoModel CurrentUserInfoModel { get; set; }

        public readonly HttpClient _httpClient = new HttpClient();
        public List<string> MessageKeys = new List<string>();

        public async Task<string> HttpRequest(string endpoint, string method, string json = "")
        {
            try
            {
                if (endpoint != "")
                {
                    HttpResponseMessage httpResponseMessage = null;
                    StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                    switch (method)
                    {
                        case "GET":
                            httpResponseMessage = await _httpClient.GetAsync(endpoint);
                            break;
                        case "POST":
                            httpResponseMessage = await _httpClient.PostAsync(endpoint, stringContent);
                            break;
                        case "PUT":
                            httpResponseMessage = await _httpClient.PutAsync(endpoint, stringContent);
                            break;
                        case "PATCH":
                            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), endpoint);
                            httpRequestMessage.Content = stringContent;
                            httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage);
                            break;
                        case "DELETE":
                            httpResponseMessage = await _httpClient.DeleteAsync(endpoint);
                            break;
                    }

                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        return await httpResponseMessage.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        await HttpRequest(endpoint, method, json);
                    }
                }

                return null;
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public async Task<bool> CheckToFriendRequestsList(string userEmail)
        {
            string endpoint = await GetDatabaseFriendRequestsEndpoint(userEmail);
            string response = await HttpRequest(endpoint, HttpMethod.Get.ToString());

            if (response != null || response != "")
            {
                Dictionary<string, FriendRequest> model = JsonHelper.Deserialize<Dictionary<string, FriendRequest>>(response);

                if (model != null)
                {
                    var result = model.Values.Where(x => x.SenderEmail == CurrentUserInfoModel.Email).ToList();
                    if (result.Count >= 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public async Task<bool> AcceptFriendRequest(FriendRequest friendRequest)
        {
            try
            {
                string receiverUserId = await GetUserIdWithUserEmail(friendRequest.ReceiverEmail);
                string senderUserId = await GetUserIdWithUserEmail(friendRequest.SenderEmail);
                await AddToFriendList(receiverUserId, senderUserId);

                friendRequest.Accepted = true;

                string endpoint = await GetDatabaseKeyFriendRequestEndpoint(CurrentUserInfoModel.Email, friendRequest);

                friendRequest.Key = null;

                string json = JsonHelper.Serialize(friendRequest);
                string response = await HttpRequest(endpoint, HttpMethod.Put.ToString(), json);

                if (response != null && response != "")
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetUserEmailWithUserId(string userId)
        {
            try
            {
                string response = await HttpRequest(DatabaseUsersEndpoint, HttpMethod.Get.ToString());

                if (response != null && response != "")
                {
                    Dictionary<string, User> model = JsonHelper.Deserialize<Dictionary<string, User>>(response);

                    if (model != null)
                    {
                        return model.Where(x => x.Key == userId).Select(x => x.Value.Email).FirstOrDefault();
                    }
                }
                else
                {
                    await GetUserEmailWithUserId(userId);
                }

                return "";
            }
            catch
            {
                return "";
            }
        }

        public async Task<bool> CheckToChatsList(string userEmail, string userId)
        {
            await Task.Delay(2000);
            string endpoint = await GetDatabaseChatsEndpoint(userEmail);
            string response = await HttpRequest(endpoint, HttpMethod.Get.ToString());

            if (response != null && response != "")
            {
                Dictionary<string, Chat> model = JsonHelper.Deserialize<Dictionary<string, Chat>>(response);

                if (model != null)
                {
                    var result = model.Where(x => x.Value.ChatReceiverUserId == userId).ToList();
                    if (result.Count >= 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            if (response == "null")
            {
                return false;
            }

            return true;
        }

        public async Task<List<Chat>> GetChats()
        {
            try
            {
                if (CurrentUserInfoModel != null && CurrentUserMainModel != null)
                {
                    string endpoint = await GetDatabaseChatsEndpoint(CurrentUserInfoModel.Email);
                    string response = await HttpRequest(endpoint, HttpMethod.Get.ToString());

                    if (response != null && response != "")
                    {
                        Dictionary<string, Chat> model = JsonHelper.Deserialize<Dictionary<string, Chat>>(response);

                        if (model != null)
                        {
                            List<Chat> chats = new List<Chat>();

                            foreach (KeyValuePair<string, Chat> item in model)
                            {
                                string chatId = model.Where(x => x.Value.ChatReceiverUserId == item.Value.ChatReceiverUserId).Select(x => x.Key).FirstOrDefault();

                                Chat chat = new Chat
                                {
                                    Key = chatId,
                                    ChatId = item.Value.ChatId,
                                    ChatReceiverUserId = item.Value.ChatReceiverUserId,
                                    Messages = item.Value.Messages
                                };

                                chats.Add(chat);
                            }

                            return chats;
                        }
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CreateChat(string chatReceiverUserId)
        {
            try
            {
                string chatId = Guid.NewGuid().ToString();

                if (!await CheckToChatsList(CurrentUserInfoModel.Email, chatReceiverUserId))
                {
                    var requestData = new
                    {
                        ChatId = chatId,
                        ChatReceiverUserId = chatReceiverUserId
                    };

                    string endpoint = await GetDatabaseChatsEndpoint(CurrentUserInfoModel.Email);
                    string json = JsonHelper.Serialize(requestData);
                    string response = await HttpRequest(endpoint, HttpMethod.Post.ToString(), json);
                }

                string senderUserId = await GetUserIdWithUserEmail(CurrentUserInfoModel.Email);
                string senderUserEmail = await GetUserEmailWithUserId(chatReceiverUserId);

                if (!await CheckToChatsList(senderUserEmail, senderUserId))
                {
                    var requestData = new
                    {
                        ChatId = chatId,
                        ChatReceiverUserId = senderUserId
                    };

                    string endpoint = await GetDatabaseChatsEndpoint(senderUserEmail);
                    string json = JsonHelper.Serialize(requestData);
                    string response = await HttpRequest(endpoint, HttpMethod.Post.ToString(), json);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AddToFriendList(string receiverUserId, string senderUserId)
        {
            try
            {
                string receiverEmail = await GetUserEmailWithUserId(receiverUserId);
                string senderEmail = await GetUserEmailWithUserId(senderUserId);

                if (!await CheckToFriendList(receiverEmail))
                {
                    Friend friend = new Friend
                    {
                        UserId = senderUserId
                    };

                    string json = JsonHelper.Serialize(friend);
                    string endpoint = await GetDatabaseFriendsEndpoint(receiverEmail);
                    string response = await HttpRequest(endpoint, HttpMethod.Post.ToString(), json);
                }
                else
                {
                    return false;
                }

                if (!await CheckToFriendList(senderEmail))
                {
                    Friend friend = new Friend
                    {
                        UserId = receiverUserId
                    };

                    string json = JsonHelper.Serialize(friend);
                    string endpoint = await GetDatabaseFriendsEndpoint(senderEmail);
                    string response = await HttpRequest(endpoint, HttpMethod.Post.ToString(), json);
                }
                else
                {
                    return false;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RejectFriendRequest(FriendRequest friendRequest)
        {
            try
            {
                string endpoint = await GetDatabaseKeyFriendRequestEndpoint(CurrentUserInfoModel.Email, friendRequest);
                string response = await HttpRequest(endpoint, HttpMethod.Delete.ToString());

                if (response != null && response != "")
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CheckToFriendList(string userEmail)
        {
            string endpoint = await GetDatabaseFriendsEndpoint(userEmail);
            string response = await HttpRequest(endpoint, HttpMethod.Get.ToString());

            if (response != null && response != "")
            {
                Dictionary<string, Friend> model = JsonHelper.Deserialize<Dictionary<string, Friend>>(response);

                if (model != null)
                {
                    string userId = await GetUserIdWithUserEmail(CurrentUserInfoModel.Email);
                    var result = model.Values.Where(x => x.UserId == userId).ToList();
                    if (result.Count >= 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public async Task<bool> SendFriendshipRequest(string receiverEmail)
        {
            try
            {
                if (!await CheckToFriendRequestsList(receiverEmail))
                {
                    if (!await CheckToFriendList(receiverEmail))
                    {
                        FriendRequest friendRequest = new FriendRequest
                        {
                            SenderEmail = CurrentUserInfoModel.Email,
                            ReceiverEmail = receiverEmail,
                            Accepted = false
                        };

                        string json = JsonHelper.Serialize(friendRequest);
                        string endpoint = await GetDatabaseFriendRequestsEndpoint(receiverEmail);
                        string response = await HttpRequest(endpoint, HttpMethod.Post.ToString(), json);

                        if (response != null && response != "")
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetUserIdWithUserEmail(string userEmail)
        {
            try
            {
                string response = await HttpRequest(DatabaseUsersEndpoint, HttpMethod.Get.ToString());

                if (response != null && response != "")
                {
                    Dictionary<string, User> model = JsonHelper.Deserialize<Dictionary<string, User>>(response);

                    if (model != null)
                    {
                        return model.Where(x => x.Value.Email == userEmail).Select(x => x.Key).FirstOrDefault();
                    }
                }
                else
                {
                    await GetUserIdWithUserEmail(userEmail);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<User> GetUserWithUserId(string userId)
        {
            try
            {
                string response = await HttpRequest(DatabaseUsersEndpoint, HttpMethod.Get.ToString());

                if (response != null && response != "")
                {
                    Dictionary<string, User> model = JsonHelper.Deserialize<Dictionary<string, User>>(response);

                    if (model != null)
                    {
                        var result = model.Where(x => x.Key == userId).FirstOrDefault();

                        User user = result.Value;
                        user.Key = result.Key;

                        return user;
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateUserStatus(bool status)
        {
            try
            {
                if (CurrentUserInfoModel != null && CurrentUserMainModel != null)
                {
                    var requestData = new
                    {
                        Status = status
                    };

                    string endpoint = await GetDatabaseUserStatusEndpoint(CurrentUserInfoModel.Email);
                    string json = JsonHelper.Serialize(requestData);
                    string response = await HttpRequest(endpoint, "PATCH", json);

                    if (response != null && response != "")
                    {
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetAccessToken()
        {
            try
            {
                string[] scopes = { "https://www.googleapis.com/auth/cloud-platform" };
                var credential = GoogleCredential.FromFile(FirebaseConfig.ServiceAccountFilePath).CreateScoped(scopes);

                return await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            }
            catch
            {
                return "";
            }
        }

        public async Task<UserMainModel> GetUsers()
        {
            try
            {
                string accessToken = await GetAccessToken();

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                string response = await HttpRequest(UsersEndpoint, HttpMethod.Get.ToString());

                if (response != null && response != "")
                {
                    UserMainModel userMainModel = JsonHelper.Deserialize<UserMainModel>(response);

                    _httpClient.DefaultRequestHeaders.Authorization = null;

                    return userMainModel;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<UserMainInfoModel> SearchUserByEmail(string email)
        {
            try
            {
                UserMainModel userMainModel = await GetUsers();

                UserMainInfoModel result = userMainModel.Users.Where(x => x.Email.Contains(email) && x.Email != CurrentUserInfoModel.Email).FirstOrDefault();

                return result;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<User>> GetFriends()
        {
            try
            {
                string endpoint = await GetDatabaseFriendsEndpoint(CurrentUserInfoModel.Email);
                string response = await HttpRequest(endpoint, HttpMethod.Get.ToString());

                if (response != null && response != "")
                {
                    Dictionary<string, Friend> model = JsonHelper.Deserialize<Dictionary<string, Friend>>(response);

                    if (model != null)
                    {
                        List<User> friends = new List<User>();
                        foreach (KeyValuePair<string, Friend> item in model)
                        {
                            User friend = await GetUserWithUserId(item.Value.UserId);

                            friends.Add(friend);
                        }

                        return friends;
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<FriendRequest>> GetFriendRequests()
        {
            try
            {
                if (CurrentUserInfoModel != null && CurrentUserMainModel != null)
                {
                    string endpoint = await GetDatabaseFriendRequestsEndpoint(CurrentUserInfoModel.Email);
                    string response = await HttpRequest(endpoint, HttpMethod.Get.ToString());

                    if (response != null && response != "")
                    {
                        Dictionary<string, FriendRequest> model = JsonHelper.Deserialize<Dictionary<string, FriendRequest>>(response);

                        if (model != null)
                        {
                            List<FriendRequest> friendRequests = new List<FriendRequest>();
                            foreach (KeyValuePair<string, FriendRequest> item in model)
                            {
                                FriendRequest friendRequest = new FriendRequest
                                {
                                    Key = item.Key,
                                    Accepted = item.Value.Accepted,
                                    ReceiverEmail = item.Value.ReceiverEmail,
                                    SenderEmail = item.Value.SenderEmail
                                };

                                friendRequests.Add(friendRequest);
                            }

                            return friendRequests.Where(x => x.Accepted == false).ToList();
                        }
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<AuthOobCodeModel> SendOobCode(string requestType, string email, string idToken = "")
        {
            try
            {
                var requestData = new
                {
                    requestType,
                    email,
                    idToken
                };

                string json = JsonHelper.Serialize(requestData);

                if (string.IsNullOrEmpty(idToken))
                {
                    var requestData1 = new
                    {
                        requestType,
                        email
                    };

                    json = JsonHelper.Serialize(requestData1);
                }

                string response = await HttpRequest(SendOobCodeEndpoint, HttpMethod.Post.ToString(), json);

                if (response != null && response != "")
                {
                    AuthOobCodeModel authOobCodeModel = JsonHelper.Deserialize<AuthOobCodeModel>(response);

                    return authOobCodeModel;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateUser(string idToken, string displayName, string photoUrl, string password)
        {
            try
            {
                var requestData = new
                {
                    idToken,
                    displayName,
                    photoUrl,
                    password,
                    returnSecureToken = true
                };

                string json = JsonHelper.Serialize(requestData);
                string response = await HttpRequest(UpdateUserEndpoint, HttpMethod.Post.ToString(), json);

                if (response != null && response != "")
                {
                    CurrentUserMainModel.Users[0] = JsonHelper.Deserialize<UserMainInfoModel>(response);
                    CurrentUserMainModel.Users[0].Password = password;
                    CurrentUserInfoModel.IdToken = CurrentUserMainModel.Users[0].IdToken;

                    if (Properties.Settings.Default.CurrentUserInfoModel != "" && Properties.Settings.Default.CurrentUserMainModel != "")
                    {
                        SaveUser();
                    }

                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<UserMainModel> SignUp(string email, string password)
        {
            try
            {
                var requestData = new
                {
                    email,
                    password,
                    returnSecureToken = true
                };

                var requestData1 = new
                {
                    Email = email,
                    Status = false
                };

                string json = JsonHelper.Serialize(requestData);
                string response = await HttpRequest(SignUpEndpoint, HttpMethod.Post.ToString(), json);

                if (response != null && response != "")
                {
                    UserInfoModel userInfoModel = JsonHelper.Deserialize<UserInfoModel>(response);
                    UserMainModel userMainModel = await GetUserInfo(userInfoModel.IdToken);

                    string json1 = JsonHelper.Serialize(requestData1);
                    string response1 = await HttpRequest(DatabaseUsersEndpoint, HttpMethod.Post.ToString(), json1);

                    return userMainModel;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<UserMainModel> GetUserInfo(string idToken)
        {
            try
            {
                var requestData = new
                {
                    idToken
                };

                string json = JsonHelper.Serialize(requestData);
                string response = await HttpRequest(UserInfoEndpoint, HttpMethod.Post.ToString(), json);

                if (response != null && response != "")
                {
                    UserMainModel userMainModel = JsonHelper.Deserialize<UserMainModel>(response);

                    return userMainModel;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<Models.Message>> GetMessages(string chatId)
        {
            try
            {
                string endpoint = await GetDatabaseChatsMessagesEndpoint(CurrentUserInfoModel.Email, chatId);
                string response = await HttpRequest(endpoint, HttpMethod.Get.ToString());

                if (response != null && response != "")
                {
                    Dictionary<string, Models.Message> model = JsonHelper.Deserialize<Dictionary<string, Models.Message>>(response);

                    if (model != null)
                    {
                        List<Models.Message> messages = new List<Models.Message>();
                        foreach (KeyValuePair<string, Models.Message> item in model)
                        {
                            Models.Message message = new Models.Message
                            {
                                Key = item.Value.Key,
                                MessageSenderUserId = item.Value.MessageSenderUserId,
                                MessageSenderDisplayName = item.Value.MessageSenderDisplayName,
                                MessageReceiverUserId = item.Value.MessageReceiverUserId,
                                MessageReceiverDisplayName = item.Value.MessageReceiverDisplayName,
                                Text = item.Value.Text
                            };

                            messages.Add(message);
                        }

                        return messages;
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Models.Message> GetLastMessages(string chatId)
        {
            try
            {
                string endpoint = await GetDatabaseChatsMessagesEndpoint(CurrentUserInfoModel.Email, chatId);
                string response = await HttpRequest(endpoint, HttpMethod.Get.ToString());

                if (response != null && response != "")
                {
                    Dictionary<string, Models.Message> model = JsonHelper.Deserialize<Dictionary<string, Models.Message>>(response);

                    if (model != null)
                    {
                        List<Models.Message> messages = new List<Models.Message>();
                        foreach (KeyValuePair<string, Models.Message> item in model)
                        {
                            Models.Message message = new Models.Message
                            {
                                Key = item.Value.Key,
                                MessageSenderUserId = item.Value.MessageSenderUserId,
                                MessageSenderDisplayName = item.Value.MessageSenderDisplayName,
                                MessageReceiverUserId = item.Value.MessageReceiverUserId,
                                MessageReceiverDisplayName = item.Value.MessageReceiverDisplayName,
                                Text = item.Value.Text
                            };

                            messages.Add(message);
                        }

                        return messages.LastOrDefault();
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task SendMessage(string receiverDisplayName, string chatId, string receiverUserId, string text)
        {
            string senderUserId = await GetUserIdWithUserEmail(CurrentUserInfoModel.Email);
            string receiverName = "No username";
            string senderName = "No username";

            if (CurrentUserInfoModel.DisplayName != "" && CurrentUserInfoModel.DisplayName != null)
            {
                senderName = CurrentUserInfoModel.DisplayName;
            }

            if (receiverDisplayName != "" && receiverDisplayName != null)
            {
                receiverName = receiverDisplayName;
            }

            Models.Message requestData = new Models.Message
            {
                Key = Guid.NewGuid().ToString(),
                MessageSenderUserId = senderUserId,
                MessageSenderDisplayName = senderName,
                MessageReceiverUserId = receiverUserId,
                MessageReceiverDisplayName = receiverName,
                Text = text
            };

            string chatId1 = await GetChatKeyWithChatId(chatId, CurrentUserInfoModel.Email);

            if (await CheckToChatsMessageList(CurrentUserInfoModel.Email, chatId1))
            {
                string endpoint = await GetDatabaseChatsMessagesEndpoint(CurrentUserInfoModel.Email, chatId1);
                string json = JsonHelper.Serialize(requestData);
                string response = await HttpRequest(endpoint, HttpMethod.Post.ToString(), json);
            }

            string senderUserEmail = await GetUserEmailWithUserId(receiverUserId);
            string chatId2 = await GetChatKeyWithChatId(chatId, senderUserEmail);

            if (await CheckToChatsMessageList(senderUserEmail, chatId2))
            {
                string endpoint = await GetDatabaseChatsMessagesEndpoint(senderUserEmail, chatId2);
                string json = JsonHelper.Serialize(requestData);
                string response = await HttpRequest(endpoint, HttpMethod.Post.ToString(), json);
            }
        }

        public async Task<bool> CheckToChatsMessageList(string userEmail, string chatId)
        {
            string endpoint = await GetDatabaseChatsEndpoint(userEmail);
            string response = await HttpRequest(endpoint, HttpMethod.Get.ToString());

            if (response != null && response != "")
            {
                Dictionary<string, Chat> model = JsonHelper.Deserialize<Dictionary<string, Chat>>(response);

                if (model != null)
                {
                    var result = model.Where(x => x.Key == chatId).ToList();
                    if (result.Count >= 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public async Task<string> GetChatKeyWithChatId(string chatId, string email)
        {
            try
            {
                string endpoint = await GetDatabaseChatsEndpoint(email);
                string response = await HttpRequest(endpoint, HttpMethod.Get.ToString());

                if (response != null && response != "")
                {
                    Dictionary<string, Chat> model = JsonHelper.Deserialize<Dictionary<string, Chat>>(response);

                    if (model != null)
                    {
                        return model.Where(x => x.Value.ChatId == chatId).Select(x => x.Key).FirstOrDefault();
                    }
                }

                return "";
            }
            catch
            {
                return "";
            }
        }

        public async Task<UserMainModel> SignIn(string email, string password, bool rememberMe)
        {
            try
            {
                var requestData = new
                {
                    email,
                    password,
                    returnSecureToken = true
                };

                string json = JsonHelper.Serialize(requestData);
                string response = await HttpRequest(SignInEndpoint, HttpMethod.Post.ToString(), json);

                if (response != null && response != "")
                {
                    CurrentUserInfoModel = JsonHelper.Deserialize<UserInfoModel>(response);

                    CurrentUserMainModel = await GetUserInfo(CurrentUserInfoModel.IdToken);
                    CurrentUserMainModel.Users[0].Password = password;

                    if (rememberMe)
                    {
                        SaveUser();
                    }

                    await UpdateUserStatus(true);

                    return CurrentUserMainModel;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public void SaveUser()
        {
            Properties.Settings.Default.CurrentUserInfoModel = JsonHelper.Serialize(CurrentUserInfoModel);
            Properties.Settings.Default.CurrentUserMainModel = JsonHelper.Serialize(CurrentUserMainModel);
            Properties.Settings.Default.Save();
        }

        public async Task<bool> AutomaticSignIn()
        {
            if (Properties.Settings.Default.CurrentUserInfoModel != "" && Properties.Settings.Default.CurrentUserMainModel != "")
            {
                CurrentUserInfoModel = JsonHelper.Deserialize<UserInfoModel>(Properties.Settings.Default.CurrentUserInfoModel);
                CurrentUserMainModel = JsonHelper.Deserialize<UserMainModel>(Properties.Settings.Default.CurrentUserMainModel);

                await UpdateUserStatus(true);

                return true;
            }

            return false;
        }

        public async void SignOut()
        {
            await UpdateUserStatus(false);

            CurrentUserInfoModel = null;
            CurrentUserMainModel = null;
            Properties.Settings.Default.CurrentUserInfoModel = "";
            Properties.Settings.Default.CurrentUserMainModel = "";
            Properties.Settings.Default.Save();
        }
    }
}