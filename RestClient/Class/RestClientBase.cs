using Newtonsoft.Json;
using RestClient.Exception;
using RestClient.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Class
{
    public class Client
    {

        #region Properties
        private HttpClient _httpClient;
        private Encoding _encoding;
        private string _mediaType;
        private TokenResponse _tokenResponse;

        bool _refreshToken;
        string _urlLogin;
        string _grantType;
        string _username;
        string _password;
        Encoding _encodingToken;
        string _applicationEncode;
        private int? _MINUTES_TO_REFRESH_TOKEN = 5;

        #endregion

        #region Constructors
        public Client(Uri baseAddress, long maxResponseContentBufferSize, TimeSpan timeout, Encoding encoding)
        {
            _encoding = encoding;
            _httpClient = new HttpClient
            {
                BaseAddress = baseAddress,
                MaxResponseContentBufferSize = maxResponseContentBufferSize,
                Timeout = timeout,
            };
        }
        public Client(Uri baseAddress, TimeSpan timeout, Encoding encoding, string mediaType)
        {
            _encoding = encoding;
            _mediaType = mediaType;
            _httpClient = new HttpClient
            {
                BaseAddress = baseAddress,
                Timeout = timeout,
            };
        }
        public Client(Uri baseAddress, long maxResponseContentBufferSize, TimeSpan timeout, Encoding encoding, string mediaType)
        {
            _encoding = encoding;
            _mediaType = mediaType;
            _httpClient = new HttpClient
            {
                BaseAddress = baseAddress,
                MaxResponseContentBufferSize = maxResponseContentBufferSize,
                Timeout = timeout,
            };
        }
        public Client(Uri baseAddress, long maxResponseContentBufferSize, TimeSpan timeout)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = baseAddress,
                MaxResponseContentBufferSize = maxResponseContentBufferSize,
                Timeout = timeout,
            };
        }
        public Client(Uri baseAddress, long maxResponseContentBufferSize)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = baseAddress,
                MaxResponseContentBufferSize = maxResponseContentBufferSize,
            };
        }
        public Client(Uri baseAddress, long maxResponseContentBufferSize, Encoding encoding)
        {
            _encoding = encoding;
            _httpClient = new HttpClient
            {
                BaseAddress = baseAddress,
                MaxResponseContentBufferSize = maxResponseContentBufferSize,
            };
        }
        public Client(Uri baseAddress, long maxResponseContentBufferSize, Encoding encoding, string mediaType)
        {
            _encoding = encoding;
            _mediaType = mediaType;
            _httpClient = new HttpClient
            {
                BaseAddress = baseAddress,
                MaxResponseContentBufferSize = maxResponseContentBufferSize,
            };
        }
        public Client(Uri baseAddress)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = baseAddress,
            };
        }
        public Client(Uri baseAddress, Encoding encoding)
        {
            _encoding = encoding;
            _httpClient = new HttpClient
            {
                BaseAddress = baseAddress,
            };
        }
        public Client(Uri baseAddress, Encoding encoding, string mediaType)
        {
            _encoding = encoding;
            _mediaType = mediaType;
            _httpClient = new HttpClient
            {
                BaseAddress = baseAddress,
            };
        }
        public Client(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public Client(HttpClient httpClient, 
           bool refreshToken = false, 
           string urlLogin = null,
           string grantType=null,
           string username=null,
           string password=null,
           Encoding encoding=null,
           string applicationEncode=null, 
           int? MINUTES_TO_REFRESH_TOKEN=null)
        {
            
            //Comentario
            if (refreshToken)
            {
                if (urlLogin == null)
                {
                    throw new ArgumentNullException(nameof(urlLogin));
                }
                if (grantType == null)
                {
                    throw new ArgumentNullException(nameof(grantType));
                }
                if (username == null)
                {
                    throw new ArgumentNullException(nameof(username));
                }
                if (password == null)
                {
                    throw new ArgumentNullException(nameof(password));
                }
                if (encoding == null)
                {
                    throw new ArgumentNullException(nameof(encoding));
                }
                if (applicationEncode == null)
                {
                    throw new ArgumentNullException(nameof(applicationEncode));
                }
                if (MINUTES_TO_REFRESH_TOKEN != null)
                {
                    _MINUTES_TO_REFRESH_TOKEN = MINUTES_TO_REFRESH_TOKEN.Value;
                }

                _refreshToken =refreshToken;
                 _urlLogin=urlLogin;
                 _grantType=grantType;
                 _username=username;
                 _password=password;
                 _encodingToken=encoding;
                 _applicationEncode=applicationEncode;
            }
            _httpClient = httpClient;
        }
        #endregion

        #region Token
        public async Task<TokenResponse> GetTokenAsync(
           string urlLogin,
           string grantType,
           string username,
           string password,
           Encoding encoding,
           string applicationEncode)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = _httpClient.BaseAddress;
                var response = await client.PostAsync(urlLogin, new StringContent($"grant_type={grantType}&username={username}&password={password}", encoding, applicationEncode));
                var resultJSON = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TokenResponse>(
                    resultJSON);
                _tokenResponse = result;
                return result;
            }
            catch
            {
                return null;
            }
        }



        public TokenResponse GetToken(
           string urlLogin,
           string grantType,
           string username,
           string password,
           Encoding encoding,
           string applicationEncode)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = _httpClient.BaseAddress;
                var response =  client.PostAsync(urlLogin, new StringContent($"grant_type={grantType}&username={username}&password={password}", encoding, applicationEncode)).Result;
                var resultJSON =  response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<TokenResponse>(
                    resultJSON);
                _tokenResponse = result;
                return result;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region Response
        private T Interpreter<T>(HttpResponseMessage httpResponseMessage)
        {
            try
            {
                if (HttpStatusCode.OK==httpResponseMessage.StatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(httpResponseMessage.Content.ReadAsStringAsync().Result);
                }
                throw new ExceptionJson();
            }
            catch (ExceptionJson ex)
            {
                throw new ExceptionJson(httpResponseMessage.StatusCode, httpResponseMessage,ex.Message,ex);
            }
            catch (System.Exception ex)
            {
                throw new ExceptionJson(httpResponseMessage.StatusCode, httpResponseMessage, ex.Message, ex);
            }


        }
        private async Task<T> InterpreterAsync<T>(HttpResponseMessage httpResponseMessage)
        {
            try
            {
                if (HttpStatusCode.OK == httpResponseMessage.StatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(await httpResponseMessage.Content.ReadAsStringAsync());
                }
                throw new ExceptionJson();
            }
            catch (ExceptionJson ex)
            {
                throw new ExceptionJson(httpResponseMessage.StatusCode, httpResponseMessage, ex.Message, ex);
            }
            catch (System.Exception ex)
            {
                throw new ExceptionJson(httpResponseMessage.StatusCode, httpResponseMessage, ex.Message, ex);
            }

        }
        #endregion

        #region Gets
        public  T Get<T>(string requestUrl)
        {
            try
            {
                RefreshToken();
                return Interpreter<T>(_httpClient.GetAsync($"{_httpClient.BaseAddress}{requestUrl}").Result);
            }
            catch (ExceptionJson)
            {
                throw;
            }
            catch (System.Exception ex)
            {
                throw new ExceptionJson(ex.Message, ex);
            }

        }

        public async Task<object> GetAsync<T>(string requestUrl)
        {
            try
            {
                await RefreshTokenAsync();
                return await InterpreterAsync<T>(await _httpClient.GetAsync($"{_httpClient.BaseAddress}{requestUrl}"));
            }
            catch (ExceptionJson)
            {
                throw;
            }
            catch (System.Exception ex)
            {
                throw new ExceptionJson(ex.Message, ex);
            }
        }
        public async Task<object> GetAsync<T>(string requestUrl, string parameters)
        {
            try
            {
                await RefreshTokenAsync();
                return await InterpreterAsync<T>(await _httpClient.GetAsync($"{_httpClient.BaseAddress}{requestUrl}{parameters}"));
            }
            catch (ExceptionJson)
            {
                throw;
            }
            catch (System.Exception ex)
            {
                throw new ExceptionJson(ex.Message, ex);
            }
        }
        public object Get<T>(string requestUrl, string parameters)
        {
            try
            {
                RefreshToken();
                return Interpreter<T>(_httpClient.GetAsync($"{_httpClient.BaseAddress}{requestUrl}{parameters}").Result);
            }
            catch (ExceptionJson)
            {
                throw;
            }
            catch (System.Exception ex)
            {
                throw new ExceptionJson(ex.Message, ex);
            }
        }

        public HttpResponseMessage Get(string requestUrl)
        {
            try
            {
                RefreshToken();
                return _httpClient.GetAsync($"{_httpClient.BaseAddress}{requestUrl}").Result;
            }
            catch (System.Exception ex)
            {
                throw new ExceptionJson(ex.Message, ex);
            }
        }
        public async Task<HttpResponseMessage> GetAsync(string requestUrl)
        {
            try
            {
                await RefreshTokenAsync();
                return await _httpClient.GetAsync($"{_httpClient.BaseAddress}{requestUrl}");
            }
            catch (System.Exception ex)
            {
                throw new ExceptionJson(ex.Message, ex);
            }
        }
   
        public HttpResponseMessage Get(string requestUrl, string parameters)
        {
            try
            {
                RefreshToken();
                return _httpClient.GetAsync($"{_httpClient.BaseAddress}{requestUrl}{parameters}").Result;
            }
            catch (System.Exception ex)
            {
                throw new ExceptionJson(ex.Message, ex);
            }
        }
       
        public async Task<HttpResponseMessage> GetAsync(string requestUrl, string parameters)
        {
            try
            {
                await RefreshTokenAsync();
                return await _httpClient.GetAsync($"{_httpClient.BaseAddress}{requestUrl}{parameters}");
            }
            catch (System.Exception ex)
            {
                throw new ExceptionJson(ex.Message, ex);
            }
        }
        
        #endregion

        #region Post
        public HttpResponseMessage Post(string requestUrl, object body)
        {
            RefreshToken();
            var d = _tokenResponse;
            var request = JsonConvert.SerializeObject(body);
            var content = new StringContent(request, _encoding, _mediaType);
            return _httpClient.PostAsync($"{_httpClient.BaseAddress}{requestUrl}", content).Result;
        }
        public async Task<HttpResponseMessage> PostAsync(string requestUrl, object body)
        {
            await RefreshTokenAsync();
            var request = JsonConvert.SerializeObject(body);
            var content = new StringContent(request, _encoding, _mediaType);
            return await _httpClient.PostAsync($"{_httpClient.BaseAddress}{requestUrl}", content);
        }
        public HttpResponseMessage Post(string requestUrl, object body, Encoding encoding)
        {
            RefreshToken();
            var request = JsonConvert.SerializeObject(body);
            var content = new StringContent(request, encoding, _mediaType);
            return _httpClient.PostAsync($"{_httpClient.BaseAddress}{requestUrl}", content).Result;
        }
        public HttpResponseMessage Post(string requestUrl, object body, Encoding encoding, string mediaType)
        {
            RefreshToken();
            var request = JsonConvert.SerializeObject(body);
            var content = new StringContent(request, encoding, mediaType);
            return _httpClient.PostAsync($"{_httpClient.BaseAddress}{requestUrl}", content).Result;
        }
        #endregion

        #region Accept
        private void ClearAccept()
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
        }
        private HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> GetAccepts()
        {
            return _httpClient.DefaultRequestHeaders.Accept;
        }
        private void AddAccept(string mediaType)
        {
            GetAccepts().Add(new MediaTypeWithQualityHeaderValue(mediaType));
        }
        private void AddAccept(string mediaType, double quality)
        {
            GetAccepts().Add(new MediaTypeWithQualityHeaderValue(mediaType, quality));
        }
        public void AddAccept(string mediaType, double? quality, bool rewrite = false)
        {
            if (rewrite)
                ClearAccept();

            if (quality == null)
                AddAccept(mediaType);
            else
                AddAccept(mediaType, quality);
        }
        public void AddAccept(Dictionary<string, double?> mediaTypeWithQualityHeaderValue, bool rewrite = false)
        {
            if (rewrite)
                ClearAccept();

            foreach (var item in mediaTypeWithQualityHeaderValue)
            {
                if (item.Value == null)
                    AddAccept(item.Key);
                else
                    AddAccept(item.Key, item.Value.Value);
            }
        }
        #endregion

        #region Authentication
        private AuthenticationHeaderValue GetAuthenticationHeaderValue(string tokenType, string accessToken)
        {
            return new AuthenticationHeaderValue(tokenType, accessToken);
        }
        public void AddAuthentication(string tokenType, string accessToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = GetAuthenticationHeaderValue(tokenType, accessToken);
        }
        public void AddAuthentication(AuthenticationHeaderValue authenticationHeaderValue)
        {
            _httpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
        }
        #endregion

        #region DefaultRequestHeader
        private void ClearDefaultRequestHeaders()
        {
            _httpClient.DefaultRequestHeaders.Clear();
        }
        private void AddDefaultRequestHeaders(string key, string value)
        {
            _httpClient.DefaultRequestHeaders.Add(key, value);
        }
        public void AddRequestHeaders(Dictionary<string, string> requestHeaders, bool rewrite = false)
        {
            if (rewrite)
                ClearDefaultRequestHeaders();
            foreach (var item in requestHeaders)
                AddDefaultRequestHeaders(item.Key, item.Value);
        }
        #endregion


        #region ValidateToken

        private bool EsTokenValido()
        {
            try
            {
                var validadorJwt = new JwtSecurityTokenHandler();
                var tokenJwt = validadorJwt.ReadToken(_httpClient.DefaultRequestHeaders.Authorization.Parameter);
                return tokenJwt.ValidTo > DateTime.UtcNow.AddMinutes(_MINUTES_TO_REFRESH_TOKEN.Value);
            }
            catch
            {
                return false;
            }
        }


        protected async Task ValidarTokenAsync()
        {
            if (!EsTokenValido())
               await GetTokenAsync(_urlLogin,_grantType,_username,_password,_encoding,_applicationEncode);
        }

        protected void ValidarToken()
        {
            if (!EsTokenValido())
                 GetToken(_urlLogin, _grantType, _username, _password, _encoding, _applicationEncode);
        }


        private async Task RefreshTokenAsync()
        {
            if (_refreshToken)
            {
              await  ValidarTokenAsync();
            }
        }

        private void RefreshToken()
        {
            if (_refreshToken)
            {
                ValidarToken();
            }
        }

        #endregion
    }
}
