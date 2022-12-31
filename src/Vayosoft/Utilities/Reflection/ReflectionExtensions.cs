using System.Reflection;

namespace Vayosoft.Utilities.Reflection
{
    public static class ReflectionExtensions
    {
        public static bool HasAttribute<T>(this Type type, out T attribute) where T : Attribute
        {
            attribute = (T)type.GetCustomAttributes(typeof(T), true).SingleOrDefault();
            return attribute != null;
        }

        public static bool HasAttribute<T>(this MemberInfo member, out T attribute) where T : Attribute
        {
            attribute = (T)member.GetCustomAttribute(typeof(T));
            return attribute != null;
        }

        public static bool HasAttributes<T>(this MemberInfo member, out List<T> attributes) where T : Attribute
        {
            attributes = member.GetCustomAttributes(typeof(T)).Cast<T>().ToList();
            return attributes.Any();
        }
    }
}
