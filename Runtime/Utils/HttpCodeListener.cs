using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Izhguzin.GoogleIdentity.Utils
{
    internal class HttpCodeListener
    {
        #region Fileds and Properties

        private readonly string _responseHtml;

        private readonly HttpListener _httpListener;

        #endregion

        /// <exception cref="HttpListenerException"></exception>
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
                throw AuthorizationFailedException.Create(CommonErrorCodes.NetworkError, new HttpListenerException(
                    exception.ErrorCode,
                    $"Failed to add prefix {uri} ({exception.Message})"));
            }
        }

        public async Task<string> WaitForCodeAsync(string state)
        {
            try
            {
                _httpListener.Start();
            }
            catch (HttpListenerException ex)
            {
                throw AuthorizationFailedException.Create(CommonErrorCodes.NetworkError,
                    new HttpListenerException(ex.ErrorCode, $"Failed to start listening ({ex.Message})"));
            }

            Task                      timeoutTask   = Task.Delay(TimeSpan.FromMinutes(1));
            Task<HttpListenerContext> contextTask   = _httpListener.GetContextAsync();
            Task                      completedTask = await Task.WhenAny(contextTask, timeoutTask);
            if (completedTask == timeoutTask)
            {
                _httpListener.Stop();
                throw AuthorizationFailedException.Create(CommonErrorCodes.Timeout,
                    new TimeoutException("Timeout waiting for incoming requests."));
            }

            HttpListenerContext context = contextTask.Result;
            return await ProcessResponseAsync(context, state);
        }

        private void CheckForErrors(NameValueCollection query)
        {
            if (query.Get("error") != null)
                throw new AuthorizationFailedException(CommonErrorCodes.ResponseError,
                    $"OAuth authorization error: {query.Get("error")}.");

            if (query.Get("code").IsNullOrEmpty())
                throw new AuthorizationFailedException(CommonErrorCodes.ResponseError,
                    "Malformed authorization response. Code value is null.");

            if (query.Get("state").IsNullOrEmpty())
                throw new AuthorizationFailedException(CommonErrorCodes.ResponseError,
                    "Malformed authorization response. State value is null");
        }

        /// <exception cref="RequestFailedException"></exception>
        private async Task<string> ProcessResponseAsync(HttpListenerContext context, string state)
        {
            try
            {
                NameValueCollection query = context.Request.QueryString;
                CheckForErrors(query);
                string code          = query.Get("code");
                string incomingState = query.Get("state");

                if (incomingState != state)
                    throw new AuthorizationFailedException(CommonErrorCodes.ResponseError,
                        $"Received request with invalid state ({incomingState})");

                await SendResponseAsync(context);
                return code;
            }
            finally
            {
                _httpListener.Stop();
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
                throw new AuthorizationFailedException(CommonErrorCodes.DeveloperError,
                    $"Error occurred in HttpListenerResponse: {exception.Message}");
            }
            finally
            {
                response.Close();
            }
        }
    }
}