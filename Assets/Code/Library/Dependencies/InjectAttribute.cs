using System;

namespace Chuh007Lib.Dependencies
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public class InjectAttribute : Attribute
    { }
}