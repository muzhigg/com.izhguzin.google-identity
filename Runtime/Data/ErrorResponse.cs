using System;
using Unity.VisualScripting.FullSerializer;

namespace Izhguzin.GoogleIdentity
{
    [Serializable, fsObject]
    internal class ErrorResponse
    {
        [fsProperty("error")] public string Error { get; set; }

        [fsProperty("error_description")] public string ErrorDescription { get; set; }
    }
}