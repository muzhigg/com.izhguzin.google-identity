using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Izhguzin.GoogleIdentity
{
    internal class StandaloneHttpCodeListener
    {
        #region Fileds and Properties

        private readonly string _responseHtml;

        private readonly HttpListener _httpListener;

        #endregion

        public StandaloneHttpCodeListener(string uri, string responseHtml)
        {
            _responseHtml = responseHtml;
            _httpListener = new HttpListener();
            try
            {
                _httpListener.Prefixes.Add(uri);
            }
            catch (HttpListenerException exception)
            {
                throw new GoogleSignInException(
                    $"Error occurred in HttpListener: Failed to add prefix {uri}: {exception.Message}");
            }
        }

        public async Task StartAsync(string state, Action<string> callback)
        {
            try
            {
                _httpListener.Start();
            }
            catch (HttpListenerException ex)
            {
                throw new GoogleSignInException($"Error occurred in HttpListener: {ex.Message}",
                    new HttpListenerException(ex.ErrorCode,
                        $"Failed to start listening for incoming requests: {ex.Message}"));
            }

            Task                      timeoutTask   = Task.Delay(TimeSpan.FromMinutes(1));
            Task<HttpListenerContext> contextTask   = _httpListener.GetContextAsync();
            Task                      completedTask = await Task.WhenAny(contextTask, timeoutTask);
            if (completedTask == timeoutTask) throw new GoogleSignInException("Timeout waiting for incoming requests.");

            HttpListenerContext context = contextTask.Result;
            await ProcessResponseAsync(context, state, callback);
        }

        private void CheckForErrors(NameValueCollection query)
        {
            if (query.Get("error") != null)
                throw new GoogleSignInException($"OAuth authorization error: {query.Get("error")}.");

            if (query.Get("code") == null || query.Get("state") == null)
                throw new GoogleSignInException("Malformed authorization response. " + query);
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
                    throw new GoogleSignInException($"Received request with invalid state ({incomingState})");

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
                throw new GoogleSignInException($"Error occurred in HttpListenerResponse: {exception.Message}",
                    exception);
            }
            finally
            {
                response.Close();
            }
        }
    }
}