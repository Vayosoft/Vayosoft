﻿using System.Reflection;

namespace Vayosoft.Persistence.MongoDB
{
    public class AutoRegistration
    {
        public static void RegisterClassMap(Assembly domainAssembly)
        {
            domainAssembly ??= Assembly.GetCallingAssembly();
            var classMaps = domainAssembly
                .GetTypes()
                .Where(t => t.BaseType is { IsGenericType: true } && t.BaseType.GetGenericTypeDefinition() == typeof(MongoClassMap<>));

            foreach (var classMap in classMaps)
                Activator.CreateInstance(classMap);
        }
    }
}
