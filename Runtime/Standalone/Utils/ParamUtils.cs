using System;
using System.Reflection;

namespace Izhguzin.GoogleIdentity.Standalone
{
    internal class ParamUtils
    {
        public static void IterateParameters(object request, Action<string, object> action)
        {
            foreach (PropertyInfo propertyInfo in request.GetType().GetProperties())
            {
                RequestParameterAttribute attribute = propertyInfo.GetCustomAttribute<RequestParameterAttribute>();

                if (attribute == null) continue;

                string name = attribute.Name ?? propertyInfo.Name.ToLower();

                object value = propertyInfo.GetValue(request, null);

                if (value == null)
                {
                    if (attribute.IsRequired)
                        throw new GoogleSignInException(CommonStatus.DeveloperError,
                            $"The required parameter ({propertyInfo.Name}) for the query has a value of zero.");

                    continue;
                }

                action(name, Uri.EscapeDataString(value.ToString()));
            }
        }
    }
}