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
        private readonly Encoding _encoding;
        private readonly string _mediaType;
        private TokenResponse _tokenResponse;
        readonly bool _refreshToken;
        readonly string _urlLogin;
        readonly string _grantType;
        readonly string _username;
        readonly string _password;
        readonly Encoding _encodingToken;
        readonly string _applicationEncode;
        private int? _MINUTES_TO_REFRESH_TOKEN = 5;

        #endregion

        #region Private

        private StringContent CreateStringContent(object body,Encoding encoding,string mediaType)
        {
            return new StringContent(JsonConvert.SerializeObject(body), encoding, _mediaType);
        }

        private StringContent CreateStringContent()
        {
            return new StringContent(string.Empty);
        }

        private string CreateRequestUri(Uri baseAddress)
        {
            return $"{baseAddress}";
        }

        private string CreateRequestUri(Uri baseAddress,string parameters)
        {
            return $"{baseAddress}{parameters}";
        }

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
           string grantType = null,
           string username = null,
           string password = null,
           Encoding encoding = null,
           string applicationEncode = null,
           int? MINUTES_TO_REFRESH_TOKEN = null)
        {

            //Comentario para mejorar
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

                _refreshToken = refreshToken;
                _urlLogin = urlLogin;
                _grantType = grantType;
                _username = username;
                _password = password;
                _encodingToken = encoding;
                _applicationEncode = applicationEncode;
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
                var response = client.PostAsync(urlLogin, new StringContent($"grant_type={grantType}&username={username}&password={password}", encoding, applicationEncode)).Result;
                var resultJSON = response.Content.ReadAsStringAsync().Result;
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
                if (HttpStatusCode.OK == httpResponseMessage.StatusCode)
                {
                    return JsonConvert.DeserializeObject<T>(httpResponseMessage.Content.ReadAsStringAsync().Result);
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
        
        #region Async

        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            try
            {
                await RefreshTokenAsync();
                return await _httpClient.GetAsync($"{_httpClient.BaseAddress}{requestUri}");
            }
            catch (System.Exception ex)
            {
                throw new ExceptionJson(ex.Message, ex);
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri, string parameters)
        {
            try
            {
                await RefreshTokenAsync();
                return await _httpClient.GetAsync($"{_httpClient.BaseAddress}{requestUri}{parameters}");
            }
            catch (System.Exception ex)
            {
                throw new ExceptionJson(ex.Message, ex);
            }
        }

        public async Task<T> GetAsync<T>(string requestUri)
        {
            try
            {
                await RefreshTokenAsync();
                return await InterpreterAsync<T>(await _httpClient.GetAsync($"{_httpClient.BaseAddress}{requestUri}"));
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

        public async Task<T> GetAsync<T>(string requestUri, string parameters)
        {
            try
            {
                await RefreshTokenAsync();
                return await InterpreterAsync<T>(await _httpClient.GetAsync($"{_httpClient.BaseAddress}{requestUri}{parameters}"));
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

        #endregion

        #region Sync

        public T Get<T>(string requestUri)
        {
            try
            {
                RefreshToken();
                return Interpreter<T>(_httpClient.GetAsync($"{_httpClient.BaseAddress}{requestUri}").Result);
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

        public T Get<T>(string requestUri, string parameters)
        {
            try
            {
                RefreshToken();
                return Interpreter<T>(_httpClient.GetAsync($"{_httpClient.BaseAddress}{requestUri}{parameters}").Result);
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

        public HttpResponseMessage Get(string requestUri)
        {
            try
            {
                RefreshToken();
                return _httpClient.GetAsync($"{_httpClient.BaseAddress}{requestUri}").Result;
            }
            catch (System.Exception ex)
            {
                throw new ExceptionJson(ex.Message, ex);
            }
        }



        public HttpResponseMessage Get(string requestUri, string parameters)
        {
            try
            {
                RefreshToken();
                return _httpClient.GetAsync($"{_httpClient.BaseAddress}{requestUri}{parameters}").Result;
            }
            catch (System.Exception ex)
            {
                throw new ExceptionJson(ex.Message, ex);
            }
        }

        #endregion
        
        #endregion

        #region Post

        #region Async

        public async Task<HttpResponseMessage> PostAsync(string requestUri)
        {
            await RefreshTokenAsync();
            return await _httpClient.PostAsync(CreateRequestUri(_httpClient.BaseAddress,requestUri),
                                               CreateStringContent());
        }

        public async Task<T> PostAsync<T>(string requestUri)
        {
            await RefreshTokenAsync();
            return await InterpreterAsync<T>(await _httpClient.PostAsync(CreateRequestUri(_httpClient.BaseAddress, requestUri),
                                               CreateStringContent()));
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, object body)
        {
            await RefreshTokenAsync();
            return await _httpClient.PostAsync(CreateRequestUri(_httpClient.BaseAddress, requestUri),
                                              CreateStringContent(body, _encoding, _mediaType));
        }

        public async Task<T> PostAsync<T>(string requestUri, object body)
        {
            await RefreshTokenAsync();
            return await InterpreterAsync<T>( await _httpClient.PostAsync(CreateRequestUri(_httpClient.BaseAddress, requestUri),
                                              CreateStringContent(body, _encoding, _mediaType)));
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, object body, Encoding encoding)
        {
            await RefreshTokenAsync();
            return await _httpClient.PostAsync(CreateRequestUri(_httpClient.BaseAddress, requestUri),
                                              CreateStringContent(body, encoding, _mediaType));
        }

        public async Task<T> PostAsync<T>(string requestUri, object body, Encoding encoding)
        {
            await RefreshTokenAsync();
            return await  InterpreterAsync<T>(await _httpClient.PostAsync(CreateRequestUri(_httpClient.BaseAddress, requestUri),
                                              CreateStringContent(body, encoding, _mediaType)));
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, object body, string mediaType)
        {
            await RefreshTokenAsync();
            return await _httpClient.PostAsync(CreateRequestUri(_httpClient.BaseAddress, requestUri),
                                              CreateStringContent(body, _encoding, mediaType));
        }

        public async Task<T> PostAsync<T>(string requestUri, object body, string mediaType)
        {
            await RefreshTokenAsync();
            return await InterpreterAsync<T>(await _httpClient.PostAsync(CreateRequestUri(_httpClient.BaseAddress, requestUri),
                                              CreateStringContent(body, _encoding, mediaType)));
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, object body, Encoding encoding, string mediaType)
        {
            await RefreshTokenAsync();
            return await _httpClient.PostAsync(CreateRequestUri(_httpClient.BaseAddress, requestUri),
                                              CreateStringContent(body, encoding, mediaType));
        }

        public async Task<T> PostAsync<T>(string requestUri, object body, Encoding encoding, string mediaType)
        {
            await RefreshTokenAsync();
            return await InterpreterAsync<T>(await _httpClient.PostAsync(CreateRequestUri(_httpClient.BaseAddress, requestUri),
                                              CreateStringContent(body, encoding, mediaType)));
        }

        #endregion

        #region Sync

        public HttpResponseMessage Post(string requestUri)
        {
            RefreshToken();
            return _httpClient.PostAsync(CreateRequestUri(_httpClient.BaseAddress,requestUri),
                                        CreateStringContent()).Result;
        }

        public T Post<T>(string requestUri)
        {
            RefreshToken();
            return  Interpreter<T>(_httpClient.PostAsync(CreateRequestUri(_httpClient.BaseAddress, requestUri),
                                        CreateStringContent()).Result);
        }

        public HttpResponseMessage Post(string requestUri, object body)
        {
            RefreshToken();
            return _httpClient.PostAsync(CreateRequestUri(_httpClient.BaseAddress, requestUri),
                                        CreateStringContent(body,_encoding,_mediaType)).Result;
        }

        public T Post<T>(string requestUri, object body)
        {
            RefreshToken();
            return  Interpreter<T>(_httpClient.PostAsync(CreateRequestUri(_httpClient.BaseAddress, requestUri),
                                        CreateStringContent(body, _encoding, _mediaType)).Result);
        }

        public HttpResponseMessage Post(string requestUri, object body, Encoding encoding)
        {
            RefreshToken();
            return _httpClient.PostAsync(CreateRequestUri(_httpClient.BaseAddress, requestUri),
                                       CreateStringContent(body, encoding, _mediaType)).Result;
        }

        public T Post<T>(string requestUri, object body, Encoding encoding)
        {
            RefreshToken();
            return  Interpreter<T>(_httpClient.PostAsync(CreateRequestUri(_httpClient.BaseAddress, requestUri),
                                       CreateStringContent(body, encoding, _mediaType)).Result);
        }

        public HttpResponseMessage Post(string requestUri, object body, string mediaType)
        {
            RefreshToken();
            return _httpClient.PostAsync(CreateRequestUri(_httpClient.BaseAddress, requestUri),
                                       CreateStringContent(body, _encoding, mediaType)).Result;
        }

        public T Post<T>(string requestUri, object body, string mediaType)
        {
            RefreshToken();
            return  Interpreter<T>(_httpClient.PostAsync(CreateRequestUri(_httpClient.BaseAddress, requestUri),
                                       CreateStringContent(body, _encoding, mediaType)).Result);
        }

        public HttpResponseMessage Post(string requestUri, object body, Encoding encoding, string mediaType)
        {
            RefreshToken();
            return _httpClient.PostAsync(CreateRequestUri(_httpClient.BaseAddress, requestUri),
                                       CreateStringContent(body, encoding, mediaType)).Result;
        }

        public T Post<T>(string requestUri, object body, Encoding encoding, string mediaType)
        {
            RefreshToken();
            return Interpreter<T>(_httpClient.PostAsync(CreateRequestUri(_httpClient.BaseAddress, requestUri),
                                       CreateStringContent(body, encoding, mediaType)).Result);
        }
        #endregion


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
                await GetTokenAsync(_urlLogin, _grantType, _username, _password, _encoding, _applicationEncode);
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
                await ValidarTokenAsync();
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
