using System;

namespace Buddy.JsSerializer
{
    /// <summary>
    /// Attribute indicating properties that should be ignored by the Javascript serializer
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class JsIgnoreAttribute : Attribute
    {
        
    }
}