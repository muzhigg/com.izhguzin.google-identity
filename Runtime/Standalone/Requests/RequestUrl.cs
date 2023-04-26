﻿namespace Izhguzin.GoogleIdentity.Standalone
{
    internal abstract class RequestUrl : IRequestUrl
    {
        public abstract string EndPointUrl { get; }

        public string BuildUrl()
        {
            string result = $"{EndPointUrl}?{BuildBody()}";

            return result;
        }

        public string BuildBody()
        {
            string queryParams = string.Empty;

            ParamUtils.IterateParameters(this, (name, value) =>
            {
                queryParams += queryParams.Length > 0 ? "&" : "";
                queryParams += $"{name}={value}";
            });

            return queryParams;
        }
    }
}