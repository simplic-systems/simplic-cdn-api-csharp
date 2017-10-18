using Simplic.CDN.Common.Models;
using Simplic.CDN.CSharp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Simplic.CDN.CSharp
{
    /// <summary>
    /// Contains all methods and properties to connect and work with a simplic cdn service
    /// </summary>
    public class Cdn : IDisposable
    {
        #region Events & Delegates

        #endregion

        #region Fields
        private string url;
        private string token;
        private CdnConnectionState state;
        #endregion

        #region Constructor
        /// <summary>
        /// Initilize a new CDN instance
        /// </summary>
        public Cdn()
        {
            state = CdnConnectionState.NotAuthenticated;
        }

        /// <summary>
        /// Initilize a new CDN instance
        /// </summary>
        /// <param name="url">Url to the cdn enpoints</param>
        public Cdn(string url) : this()
        {
            this.url = url;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Generate an exception from an unseccessfull response
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private async Task<Exception> GenerateInvalidResponseException(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NonAuthoritativeInformation ||
                    response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    token = null;
                    state = CdnConnectionState.NotAuthenticated;
                }

                try
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    var info = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.InterfaceExceptionInformation>(jsonResult);

                    return new InterfaceException(info.Message + "\r\n" + info.ExceptionMessage, info.ErrorCode, (int)response.StatusCode);
                }
                catch { /* We just tried some thing, but when this failed to nothing... */}

                return new Exception($"Exception: {await response.Content.ReadAsStringAsync()} @ HttpStatus {response.StatusCode}");
            }

            return null;
        }

        /// <summary>
        /// Get new http client instance
        /// </summary>
        /// <param name="setJsonContentType">Set default json content-type</param>
        /// <returns>Instance of a http client</returns>
        private HttpClient GetHttpClient(bool setJsonContentType = true)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(url.EndsWith("/") ? url : url + "/");

            if (setJsonContentType)
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

            return client;
        }

        /// <summary>
        /// Execute an async post message and request data from the CDN service
        /// </summary>
        /// <typeparam name="R">Type of the expected result</typeparam>
        /// <typeparam name="I">Type of the input model</typeparam>
        /// <param name="controller">Name of the controller, e.g. auth</param>
        /// <param name="action">Name of the action in the controller, e.g. login</param>
        /// <param name="model">Instance of the model which sould be passed</param>
        /// <returns>Result of the request or an exception will be thrown</returns>
        private async Task<R> PostAsync<R, I>(string controller, string action, I model)
        {
            using (var client = GetHttpClient())
            {
                if (state == CdnConnectionState.Authenticated)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("jwt", token);
                }

                // Get response
                HttpResponseMessage response = await client.PostAsJsonAsync<I>($"{controller}/{action}", model);

                if (response.IsSuccessStatusCode)
                {
                    // Get json and parse
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<R>(jsonResult);
                }
                else
                {
                    throw await GenerateInvalidResponseException(response);
                }
            }
        }

        /// <summary>
        /// Post multipart content
        /// </summary>
        /// <typeparam name="R">Expected result type</typeparam>
        /// <param name="controller">Action name</param>
        /// <param name="action">Controller anme</param>
        /// <param name="path">Binary/stream path</param>
        /// <param name="stream">Stream to send</param>
        /// <returns>Expected returnvalue or exception will be thrown</returns>
        private async Task<R> PostMultipartAsync<R>(string controller, string action, string path, Stream stream)
        {
            using (var client = GetHttpClient())
            {
                if (state == CdnConnectionState.Authenticated)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("jwt", token);
                }

                // Create multipart content
                using (var content = new MultipartFormDataContent())
                {
                    var fileContent = new StreamContent(stream);
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = path
                    };
                    content.Add(fileContent);

                    var response = client.PostAsync($"{controller}/{action}", content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        // Get json and parse
                        var jsonResult = await response.Content.ReadAsStringAsync();
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<R>(jsonResult);
                    }
                    else
                    {
                        throw await GenerateInvalidResponseException(response);
                    }
                }
            }
        }

        /// <summary>
        /// Send an asnyc get request to the cdn service
        /// </summary>
        /// <typeparam name="R">Type of the expected result</typeparam>
        /// <param name="controller">Name of the controller, e.g. auth</param>
        /// <param name="action">Name of the action in the controller, e.g. login</param>
        /// <param name="parameter">Additional url parameter. e.g. /1</param>
        /// <returns>Result of the get request as poco</returns>
        private async Task<R> GetAsync<R>(string controller, string action, string parameter)
        {
            using (HttpClient client = GetHttpClient())
            {
                if (state == CdnConnectionState.Authenticated)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("jwt", token);
                }

                // Get response
                var response = await client.GetAsync($"{controller}/{action}{parameter ?? ""}");

                if (response.IsSuccessStatusCode)
                {
                    // Get json and parse
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<R>(jsonResult);
                }
                else
                {
                    throw await GenerateInvalidResponseException(response);
                }
            }
        }

        /// <summary>
        /// Send an asnyc get request to the cdn service and return raw stream/binary (must convert internally as base64)
        /// </summary>
        /// <param name="controller">Name of the controller, e.g. auth</param>
        /// <param name="action">Name of the action in the controller, e.g. login</param>
        /// <param name="parameter">Additional url parameter. e.g. /1</param>
        /// <returns>Result of the get request as poco</returns>
        private async Task<Stream> GetAsStreamAsync(string controller, string action, string parameter)
        {
            using (HttpClient client = new HttpClient())
            {
                if (state == CdnConnectionState.Authenticated)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("jwt", token);
                }

                client.BaseAddress = new Uri(url.EndsWith("/") ? url : url + "/");

                // Get response
                var response = await client.GetAsync($"{controller}/{action}{parameter ?? ""}");

                if (response.IsSuccessStatusCode)
                {
                    // Get json and parse
                    return await response.Content.ReadAsStreamAsync();
                }
                else
                {
                    throw await GenerateInvalidResponseException(response);
                }
            }
        }

        #endregion

        #region Public Methods

        #region [Ping]
        /// <summary>
        /// Send a ping request to the service
        /// </summary>
        /// <returns>Ping result as string</returns>
        public string Ping()
        {
            return Task.Run(() => PingAsync()).Result;

        }

        /// <summary>
        /// Send an async ping request to the service
        /// </summary>
        /// <returns>Ping result as awaitable Task/String</returns>
        public async Task<string> PingAsync()
        {
            return (await GetAsync<Model.PingResult>("cdn", "ping", "")).Pong;
        }
        #endregion

        #region [GetLog]
        /// <summary>
        /// Get logging information
        /// </summary>
        /// <returns>Logging information</returns>
        public Model.LogContent GetLog()
        {
            return Task.Run(() => GetLogAsync()).Result;

        }

        /// <summary>
        /// Get logging information async
        /// </summary>
        /// <returns>PLogging information</returns>
        public async Task<Model.LogContent> GetLogAsync()
        {
            return (await GetAsync<Model.LogContent>("log", "get", ""));
        }
        #endregion

        #region [Connect]
        /// <summary>
        /// Establish a new connection to a simplic cdn service
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Returns true if connecting was successfull</returns>
        public bool Connect(string userName, string password)
        {
            return Task.Run(() => ConnectAsync(userName, password)).Result;
        }

        /// <summary>
        /// Establish a new connection to a simplic cdn service async
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Returns true if connecting was successfull</returns>
        public async Task<bool> ConnectAsync(string userName, string password)
        {
            token = null;
            state = CdnConnectionState.NotAuthenticated;

            // Call login method
            var result = await PostAsync<Model.LoginResult, Model.LoginModel>("auth", "login", new Model.LoginModel()
            {
                UserName = userName,
                Password = password
            });

            // Check login result
            if (result != null && !string.IsNullOrWhiteSpace(result.Token))
            {
                token = result.Token;
                state = CdnConnectionState.Authenticated;
                return true;
            }

            return false;
        }
        #endregion

        #region [WriteData]
        /// <summary>
        /// Write data to the simplic cdn
        /// </summary>
        /// <param name="path">Path of the data, must not contains slash, just chars that are allowed in a file name</param>
        /// <param name="data">Data to write as stream</param>
        /// <returns>Result of the write data process</returns>
        public Model.SaveBlobResultModel WriteData(string path, Stream stream)
        {
            return Task.Run(() => WriteDataAsync(path, stream)).Result;
        }

        /// <summary>
        /// Write data to the simplic cdn async
        /// </summary>
        /// <param name="path">Path of the data, must not contains slashs, just chars that are allowed in a file name</param>
        /// <param name="data">Data to write as binary</param>
        /// <returns>Result of the write data process</returns>
        public async Task<Model.SaveBlobResultModel> WriteDataAsync(string path, Stream stream)
        {
            return await PostMultipartAsync<Model.SaveBlobResultModel>("cdn", "set", path, stream);
        }
        #endregion

        #region [ReadData]
        /// <summary>
        /// Read data from a simplic cdn service.
        /// </summary>
        /// <param name="path">Path of the data, must not contains slashs, just chars that are allowed in a file name</param>
        /// <returns>Data which are located under the specific path</returns>
        public Stream ReadData(string path)
        {
            return Task.Run(() => ReadDataAsync(path)).Result;
        }

        /// <summary>
        /// Read data from a simplic cdn service async.
        /// </summary>
        /// <param name="path">Path of the data, must not contains slashs, just chars that are allowed in a file name</param>
        /// <returns>Data which are located under the specific path</returns>
        public async Task<Stream> ReadDataAsync(string path)
        {
            return await GetAsStreamAsync("cdn", "getraw", $"?path={path}");
        }
        #endregion

        #region [Dispose]
        /// <summary>
        /// Dispose the current instance/session
        /// </summary>
        public void Dispose()
        {
            state = CdnConnectionState.NotAuthenticated;
            token = null;
        }
        #endregion

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns>A list of <see cref="UserModel"/> objects</returns>
        public async Task<List<AdminUserModel>> GetAllUsers()
        {
            return (await GetAsync<List<AdminUserModel>>("UserAdmin", "GetAllUsers", ""));
        }

        /// <summary>
        /// Adds a user
        /// </summary>        
        public async Task AddUser(AdminUserModel userModel)
        {            
            // TODO: add exception handling, return proper error messages
            await PostAsync<string, AdminUserModel>("UserAdmin", "AddUser", userModel);            
        }

        /// <summary>
        /// Update a user
        /// </summary>        
        public async Task UpdateUser(AdminUserModel userModel)
        {
            // TODO: add exception handling, return proper error messages
            await PostAsync<string, AdminUserModel>("UserAdmin", $"UpdateUser?userName={userModel.UserName}", userModel);
        }

        /// <summary>
        /// Delete a user
        /// </summary>        
        public async Task DeleteUser(AdminUserModel userModel)
        {
            // TODO: add exception handling, return proper error messages
            await PostAsync<string, AdminUserModel>("UserAdmin", $"RemoveUser", userModel);
        }

        /// <summary>
        /// Get the index configuration
        /// </summary>
        /// <returns>Configuration modek</returns>     
        public async Task<AdminIndexModel> GetIndexingConfig()
        {
            // TODO: add exception handling, return proper error messages
            return await GetAsync<AdminIndexModel>("IndexAdmin", "GetConfig", "");
        }

        /// <summary>
        /// Update the indexing config
        /// </summary>        
        public async Task SetIndexingConfig(AdminIndexModel indexModel)
        {
            // TODO: add exception handling, return proper error messages
            await PostAsync<string, AdminIndexModel>("IndexAdmin", "SaveConfig", indexModel);
        }


        /// <summary>
        /// Get the index configuration
        /// </summary>
        /// <returns>Configuration modek</returns>     
        public async Task<AdminCommunicationModel> GetCommunicationConfig()
        {
            // TODO: add exception handling, return proper error messages
            return await GetAsync<AdminCommunicationModel>("CommunicationAdmin", "GetConfig", "");
        }

        /// <summary>
        /// Update the indexing config
        /// </summary>        
        public async Task SetCommunicationConfig(AdminCommunicationModel adminCommunicationModel)
        {
            // TODO: add exception handling, return proper error messages
            await PostAsync<string, AdminCommunicationModel>("CommunicationAdmin", "SetHttpConfig", adminCommunicationModel);
        }

        #endregion

        #region Public Member
        /// <summary>
        /// Get the current connection state
        /// </summary>
        public CdnConnectionState State
        {
            get
            {
                return state;
            }
        }

        /// <summary>
        /// Get the currently used url
        /// </summary>
        public string Url
        {
            get
            {
                return url;
            }
        }
        #endregion
    }
}
