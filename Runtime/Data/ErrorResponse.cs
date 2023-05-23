using System;
using UnityEngine;

//using Unity.VisualScripting.FullSerializer;

namespace Izhguzin.GoogleIdentity
{
    [Serializable /*, fsObject*/]
    internal class ErrorResponse
    {
        [SerializeField] private string error;

        [SerializeField] private string error_description;

        /*[fsProperty("error")]*/
        public string Error
        {
            get => error;
            set => error = value;
        }

        /*[fsProperty("error_description")] */
        public string ErrorDescription
        {
            get => error_description;
            set => error_description = value;
        }
    }
}