using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Izhguzin.GoogleIdentity.Standalone
{
    internal class HttpCodeListener
    {
        #region Fileds and Properties

        private readonly string _responseHtml;

        private readonly HttpListener _httpListener;

        #endregion

        public HttpCodeListener(string uri, string responseHtml)
        {
            _responseHtml = responseHtml;
            _httpListener = new HttpListener();
            try
            {
                _httpListener.Prefixes.Add(uri);
            }
            catch (HttpListenerException exception)
            {
                throw new GoogleSignInException(CommonStatus.NetworkError,
                    $"Error occurred in HttpListener: Failed to add prefix {uri}: {exception.Message}");
            }
        }

        public async Task<string> WaitForCodeAsync(string state)
        {
            string code = null;
            await StartAsync(state, s => code = s);
            return code;
        }

        public async Task StartAsync(string state, Action<string> callback)
        {
            try
            {
                _httpListener.Start();
            }
            catch (HttpListenerException ex)
            {
                throw new GoogleSignInException(CommonStatus.NetworkError,
                    $"Error occurred in HttpListener: {ex.Message}");
            }

            Task                      timeoutTask   = Task.Delay(TimeSpan.FromMinutes(1));
            Task<HttpListenerContext> contextTask   = _httpListener.GetContextAsync();
            Task                      completedTask = await Task.WhenAny(contextTask, timeoutTask);
            if (completedTask == timeoutTask)
                throw new GoogleSignInException(CommonStatus.Timeout, "Timeout waiting for incoming requests.");

            HttpListenerContext context = contextTask.Result;
            await ProcessResponseAsync(context, state, callback);
        }

        private void CheckForErrors(NameValueCollection query)
        {
            if (query.Get("error") != null)
                throw new GoogleSignInException(CommonStatus.ResponseError,
                    $"OAuth authorization error: {query.Get("error")}.");

            query.Get("code").ThrowIfNull(new GoogleSignInException(CommonStatus.ResponseError,
                "Malformed authorization response. Code value is null."));

            query.Get("state").ThrowIfNull(new GoogleSignInException(CommonStatus.ResponseError,
                "Malformed authorization response. State value is null"));
        }

        private async Task ProcessResponseAsync(HttpListenerContext context, string state, Action<string> callback)
        {
            try
            {
                NameValueCollection query = context.Request.QueryString;
                CheckForErrors(query);
                string code          = query.Get("code");
                string incomingState = query.Get("state");

                if (incomingState != state)
                    throw new GoogleSignInException(CommonStatus.ResponseError,
                        $"Received request with invalid state ({incomingState})");

                await SendResponseAsync(context);
                _httpListener.Stop();
                callback.Invoke(code);
            }
            catch (GoogleSignInException)
            {
                _httpListener.Stop();
                throw;
            }
        }

        private async Task SendResponseAsync(HttpListenerContext context)
        {
            HttpListenerResponse response = context.Response;
            byte[]               buffer   = Encoding.UTF8.GetBytes(_responseHtml);
            response.ContentLength64 = buffer.Length;
            try
            {
                await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
            catch (InvalidOperationException exception)
            {
                throw new GoogleSignInException(CommonStatus.ResponseError,
                    $"Error occurred in HttpListenerResponse: {exception.Message}");
            }
            finally
            {
                response.Close();
            }
        }
    }
}