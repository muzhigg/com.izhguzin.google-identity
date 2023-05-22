using System;
using System.Text;
using Izhguzin.GoogleIdentity.Utils;
using UnityEngine.Networking;

namespace Izhguzin.GoogleIdentity
{
    internal abstract class RequestUrl : IRequestUrl
    {
        public abstract string EndPointUrl { get; }

        /// <exception cref="InvalidOperationException"></exception>
        public string BuildUrl()
        {
            string result = $"{EndPointUrl}?{BuildBody()}";

            return result;
        }

        /// <exception cref="InvalidOperationException"></exception>
        public string BuildBody()
        {
            string queryParams = string.Empty;

            RequestParamUtils.IterateParameters(this, (name, value) =>
            {
                queryParams += queryParams.Length > 0 ? "&" : "";
                queryParams += $"{name}={value}";
            });

            return queryParams;
        }

        public UnityWebRequest CreatePostRequest()
        {
            UnityWebRequest request = new(EndPointUrl, UnityWebRequest.kHttpVerbPOST)
            {
                uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(BuildBody()))
                {
                    contentType = "application/x-www-form-urlencoded"
                },
                downloadHandler = new DownloadHandlerBuffer()
            };

            return request;
        }
    }
}