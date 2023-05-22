using System;

namespace Izhguzin.GoogleIdentity
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class RequestParameterAttribute : Attribute
    {
        #region Fileds and Properties

        public string Name       { get; }
        public bool   IsRequired { get; }

        #endregion

        public RequestParameterAttribute(string name, bool isRequired)
        {
            Name       = name;
            IsRequired = isRequired;
        }
    }
}