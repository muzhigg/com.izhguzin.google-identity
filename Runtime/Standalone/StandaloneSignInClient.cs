//using System;
//using System.Text;
//using System.Threading.Tasks;
//using Izhguzin.GoogleIdentity.Standalone;
//using UnityEngine;
//using UnityEngine.Networking;
//using Decoder = Izhguzin.GoogleIdentity.JWTDecoder.Decoder;

//#pragma warning disable CS4014

//namespace Izhguzin.GoogleIdentity
//{
//    internal sealed class StandaloneSignInClient : GoogleIdentityService
//    {
//        /// <exception cref="JsonDeserializationException"></exception>
//        private static UserCredential GetCredentialFromResponse(string tokenResponseJson)
//        {
//            TokenResponse  response   = TokenResponse.FromJson(tokenResponseJson);
//            UserCredential credential = Decoder.DecodePayload<UserCredential>(response.IdToken);
//            credential.Token = response;
//            return credential;
//        }

//        #region Fileds and Properties

//        private readonly StandaloneSignInOptions _standaloneOptions;

//        #endregion

//        internal StandaloneSignInClient(GoogleAuthOptions options) : base(options)
//        {
//            _standaloneOptions = options.Standalone;
//        }

//        protected override void BeginSignOut(GoogleRequestAsyncOperation operation)
//        {
//            InvokeOnSuccess(null, operation);
//        }

//        protected override void BeginSignIn(GoogleRequestAsyncOperation operation)
//        {
//            PerformSignInAsync(operation);
//        }

//        private async Task PerformSignInAsync(GoogleRequestAsyncOperation operation)
//        {
//            try
//            {
//                AuthorizationRequestUrl requestUrl = new(_standaloneOptions)
//                {
//                    ResponseType = "code",
//                    Scope        = "email profile"
//                };
//                HttpCodeListener listener = new(requestUrl.RedirectUri,
//                    _standaloneOptions.ResponseHtml);
//                Application.OpenURL(requestUrl.BuildUrl());

//                string code = await listener.WaitForCodeAsync(requestUrl.State);

//                UserCredential credential = await SendCodeExchangeRequestAsync(code,
//                    requestUrl.ProofCodeKey.codeVerifier, requestUrl.RedirectUri);

//                InvokeOnSuccess(credential, operation);
//            }
//            catch (GoogleSignInException exception)
//            {
//                OnExceptionCatch(operation, exception.CommonStatus, exception);
//            }
//            catch (NullReferenceException exception)
//            {
//                OnExceptionCatch(operation, CommonStatus.DeveloperError, exception);
//            }
//        }

//        /// <exception cref="GoogleSignInException"></exception>
//        /// <exception cref="NullReferenceException"></exception>
//        private async Task<UserCredential> SendCodeExchangeRequestAsync(string code, string codeVerifier,
//            string                                                             redirectUri)
//        {
//            TokenRequestUrl requestUrl = new(_standaloneOptions)
//            {
//                Code         = code,
//                RedirectUri  = redirectUri,
//                CodeVerifier = codeVerifier
//            };
//            using UnityWebRequest tokenRequest = CreatePostRequest(requestUrl);
//            await tokenRequest.SendWebRequest();
//            CheckResponseForErrors(tokenRequest, "Token exchange");

//            try
//            {
//                return GetCredentialFromResponse(tokenRequest.downloadHandler.text);
//            }
//            catch (Exception exception)
//            {
//                throw new GoogleSignInException(CommonStatus.ResponseError,
//                    $"Failed to exchange authorization code for access token: {exception.Message}");
//            }
//        }

//        private UnityWebRequest CreatePostRequest(RequestUrl url)
//        {
//            UnityWebRequest request = new(url.EndPointUrl, UnityWebRequest.kHttpVerbPOST)
//            {
//                uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(url.BuildBody()))
//                {
//                    contentType = "application/x-www-form-urlencoded"
//                },
//                downloadHandler = new DownloadHandlerBuffer()
//            };

//            return request;
//        }

//        /// <exception cref="GoogleSignInException"></exception>
//        private void CheckResponseForErrors(UnityWebRequest tokenRequest, string method)
//        {
//            if (tokenRequest.result != UnityWebRequest.Result.Success)
//                throw new GoogleSignInException(CommonStatus.ResponseError,
//                    $"{method} request failed with error: {tokenRequest.error}");

//            if (tokenRequest.responseCode != 200)
//                throw new GoogleSignInException(CommonStatus.ResponseError,
//                    $"{method} request failed with status code {tokenRequest.responseCode} and message: {tokenRequest.downloadHandler.text}");
//        }
//    }
//}

