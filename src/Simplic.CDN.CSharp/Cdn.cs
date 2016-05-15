using System;
using System.Collections.Generic;
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
            state = CdnConnectionState.NotConnected;
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
        /// Complete url with ending slash
        /// </summary>
        /// <param name="controller">Name of the controller</param>
        /// <param name="action">Name of the action</param>
        /// <returns>Complete and valid url</returns>
        private string BuildUrl(string controller, string action)
        {
            return $"{(url.EndsWith("/") ? url : url + "/")}{controller}/{action}";
        }

        /// <summary>
        /// Get new http client instance
        /// </summary>
        /// <returns>Instance of a http client</returns>
        private HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(url.EndsWith("/") ? url : url + "/");

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

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
                if (state == CdnConnectionState.Connected)
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
                    throw new Exception(await response.Content.ReadAsStringAsync() + " Status code: " + response.StatusCode);
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
                if (state == CdnConnectionState.Connected)
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
                    throw new Exception(await response.Content.ReadAsStringAsync() + " Status code: " + response.StatusCode);
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
        private async Task<byte[]> GetAsByteArrayAsync(string controller, string action, string parameter)
        {
            using (HttpClient client = new HttpClient())
            {
                if (state == CdnConnectionState.Connected)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("jwt", token);
                }

                client.BaseAddress = new Uri(url.EndsWith("/") ? url : url + "/");

                // Get response
                var response = await client.GetAsync($"{controller}/{action}{parameter ?? ""}");

                if (response.IsSuccessStatusCode)
                {
                    // Get json and parse
                    return await response.Content.ReadAsByteArrayAsync();
                }
                else
                {
                    throw new Exception(await response.Content.ReadAsStringAsync() + " Status code: " + response.StatusCode);
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
            return PingAsync().Result;
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

        #region [Connect]
        /// <summary>
        /// Establish a new connection to a simplic cdn service
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Returns true if connecting was successfull</returns>
        public bool Connect(string userName, string password)
        {
            return ConnectAsync(userName, password).Result;
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
            state = CdnConnectionState.NotConnected;

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
                state = CdnConnectionState.Connected;
                return true;
            }

            return false;
        }
        #endregion

        #region [WriteData]
        public Model.SaveBlobResultModel WriteData(string path, byte[] data)
        {
            return WriteDataAsync(path, data).Result;
        }

        public async Task<Model.SaveBlobResultModel> WriteDataAsync(string path, byte[] data)
        {
            return await PostAsync<Model.SaveBlobResultModel, Model.SaveBlobModel>("cdn", "set", new Model.SaveBlobModel()
            {
                Path = path,
                Data = data
            });
        }
        #endregion

        #region [ReadData]
        public byte[] ReadData(string path)
        {
            return ReadDataAsync(path).Result;
        }

        public async Task<byte[]> ReadDataAsync(string path)
        {
            return await GetAsByteArrayAsync("cdn", "getraw", $"?path={path}");
        }
        #endregion

        #region [Dispose]
        /// <summary>
        /// Dispose the current instance/session
        /// </summary>
        public void Dispose()
        {
            state = CdnConnectionState.NotConnected;
            token = null;
        }
        #endregion

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
