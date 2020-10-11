using System;
using System.IO;
using ConsoleForTestAPI.Models;
using System.Drawing;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleForTestAPI
{
    class Program
    {
        private const string UriBaseUsers = "https://photoapimsp.azurewebsites.net/api/Users/";
        private const string UriBasePhotos = "https://photoapimsp.azurewebsites.net/api/Photos/";

        static void Main()
        {
            TestUsersConroller();
            TestPhotoContoller();

            Console.Read();
        }

        private static string GetToken()
        {
            AuthUserRequest aurequest = new AuthUserRequest()
            { 
                Login = "admin",
                Password = "123"
            };

            string jsonString = JsonConvert.SerializeObject(aurequest);

            using (var client = new HttpClient())
            {
                var response = client.PostAsync(
                     "https://photoapimsp.azurewebsites.net/api/Security/",
                     new StringContent(jsonString, Encoding.UTF8, "application/json"));

                string str = response.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                AuthUserResponse auresponse = JsonConvert.DeserializeObject<AuthUserResponse>(str);
                return auresponse.AccessToken;
            }
        }

        #region TestUsersContoller

        private static void TestUsersConroller()
        {
            var tokenKey = GetToken();

            DeleteAllUsers(tokenKey);

            int userId;
            // AddUser
            AddUser(tokenKey, "sos_2010", "qwerty");
            userId = AddUser(tokenKey, "ogurechik", "aa7aa");

            // GetUser{id}
            GetUserById(tokenKey, userId);

            // UpdateUser
            using (var client = new HttpClient())
            {
                User aurequest = new User()
                {
                    Id = userId,
                    UserLogin = "ogurechik",
                    UserPassword = "bb7bb"
                };

                string jsonString = JsonConvert.SerializeObject(aurequest);
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = new Uri(UriBaseUsers),
                    Content = new StringContent(jsonString, Encoding.UTF8, "application/json")
                };

                request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + tokenKey);

                var response = client.SendAsync(request);
                Console.WriteLine(response.Result.StatusCode);
                string str = response.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            };

            // Get updated info
            GetUserById(tokenKey, userId);

            // Delete user by id
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(UriBaseUsers + userId)
                };

                request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + tokenKey);

                var response = client.SendAsync(request);
                Console.WriteLine(response.Result.StatusCode);
                string str = response.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            };

            // GetAllUsers
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(UriBaseUsers)
                };

                request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + tokenKey);

                var response = client.SendAsync(request);
                Console.WriteLine(response.Result.StatusCode);
                string str = response.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                IEnumerable<User> users = JsonConvert.DeserializeObject<IEnumerable<User>>(str);

                users.ToList().ForEach(u => Console.WriteLine(u));
            };

            DeleteAllUsers(tokenKey);
        }

        private static int AddUser(string tokenKey, string login, string password)
        {
            int userId;
            using (var client = new HttpClient())
            {
                // tokenKey = GetToken();

                UserRequest aurequest = new UserRequest()
                {
                    UserLogin = login,
                    UserPassword = password
                };

                string jsonString = JsonConvert.SerializeObject(aurequest);
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(UriBaseUsers + "AddUser"),
                    Content = new StringContent(jsonString, Encoding.UTF8, "application/json")
                };

                request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + tokenKey);

                var response = client.SendAsync(request);
                Console.WriteLine(response.Result.StatusCode);
                string str = response.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                userId = int.Parse(str);
            };
            return userId;
        }

        private static void GetUserById(string tokenKey, int userId)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(UriBaseUsers + userId)
                };

                request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + tokenKey);

                var response = client.SendAsync(request);
                Console.WriteLine(response.Result.StatusCode);
                string str = response.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                User user = JsonConvert.DeserializeObject<User>(str);
                Console.WriteLine(user);
            };
        }

        private static void DeleteAllUsers(string tokenKey)
        {
            // DeleteAllUsers (instead of admin)
            using (var client = new HttpClient())
            {

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(UriBaseUsers)
                };

                request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + tokenKey);

                var response = client.SendAsync(request);
                Console.WriteLine(response.Result.StatusCode);
                string str = response.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
        }

        #endregion

        #region TestPhotosController
        private static void TestPhotoContoller()
        {
            int userId = AddPhoto("dog.jpg", 3);
            int userId2 = AddPhoto("kitty.jpg", 4);

            // GetAllPhotos
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(UriBasePhotos);

                Console.WriteLine(response.Result.StatusCode);
                string str = response.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                IEnumerable<Photo> photos = JsonConvert.DeserializeObject<IEnumerable<Photo>>(str);
                photos.ToList().ForEach(p => Console.WriteLine(p));
            }

            // DeletePhoto{id}
            using (var client = new HttpClient())
            {
                var response = client.DeleteAsync(UriBasePhotos + userId);

                Console.WriteLine(response.Result.StatusCode);
            }

            // GetPhotoByDate
            using (var client = new HttpClient())
            {
                GetPhotoByDateRequest request = new GetPhotoByDateRequest()
                {
                    Date = DateTime.ParseExact("11/10/2020", "dd/MM/yyyy", null),
                    UserId = 2
                };

                string jsonString = JsonConvert.SerializeObject(request);
                var response = client.PostAsync(
                     UriBasePhotos + "GetPhotoByDate",
                    new StringContent(jsonString, Encoding.UTF8, "application/json"));

                Console.WriteLine(response.Result.StatusCode);
                string str = response.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                var photos = JsonConvert.DeserializeObject<IEnumerable<Photo>>(str);
                photos.ToList().ForEach(p => Console.WriteLine(p));


            }

            // GetPhoto{id}
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(UriBasePhotos + userId2);
                Console.WriteLine(response.Result.StatusCode);
                string str = response.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                var photo = JsonConvert.DeserializeObject<Photo>(str);
                Console.WriteLine(photo);

            }
        }

        private static byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }

        private static byte[] ReadImageFromFile(string path)
        {
            FileInfo fileInfo = new FileInfo(path);

            byte[] data = new byte[fileInfo.Length];

            using (FileStream fs = fileInfo.OpenRead())
            {
                fs.Read(data, 0, data.Length);
            }

            return data;
        }

        private static int AddPhoto(string path, int userId)
        {
            using (var client = new HttpClient())
            {
                PhotoRequest photo = new PhotoRequest()
                {
                    UserId = userId,
                    Date = DateTime.Today,
                    Image = ReadImageFromFile(path)
                };

                string jsonString = JsonConvert.SerializeObject(photo);

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(UriBasePhotos + "AddPhoto"),
                    Content = new StringContent(jsonString, Encoding.UTF8, "application/json")
                };

                var response = client.SendAsync(request);

                Console.WriteLine(response.Result.StatusCode);
                string str = response.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return int.Parse(str);

            }
        }
        #endregion
    }
}