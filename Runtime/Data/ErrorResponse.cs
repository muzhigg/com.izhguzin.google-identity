using System;
using UnityEngine;

namespace Izhguzin.GoogleIdentity
{
    [Serializable]
    internal class ErrorResponse
    {
        [SerializeField] private string error;

        [SerializeField] private string error_description;

        public string Error
        {
            get => error;
            set => error = value;
        }

        public string ErrorDescription
        {
            get => error_description;
            set => error_description = value;
        }
    }
}