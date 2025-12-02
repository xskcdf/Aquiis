using System.Reflection;
using Aquiis.SimpleStart.Core.Entities;

namespace Aquiis.SimpleStart.Utilities;

public static class SchedulableEntityRegistry
{
    private static List<Type>? _entityTypes;
    private static Dictionary<string, Type>? _entityTypeMap;

    public static List<Type> GetSchedulableEntityTypes()
    {
        if (_entityTypes == null)
        {
            _entityTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(ISchedulableEntity).IsAssignableFrom(t) 
                         && t.IsClass && !t.IsAbstract)
                .ToList();
        }
        return _entityTypes;
    }

    public static List<string> GetEntityTypeNames()
    {
        var types = GetSchedulableEntityTypes();
        var names = new List<string>();

        foreach (var type in types)
        {
            try
            {
                // Create a temporary instance to get the event type name
                var instance = Activator.CreateInstance(type) as ISchedulableEntity;
                if (instance != null)
                {
                    var eventType = instance.GetEventType();
                    if (!string.IsNullOrEmpty(eventType) && !names.Contains(eventType))
                    {
                        names.Add(eventType);
                    }
                }
            }
            catch
            {
                // If instantiation fails, use the class name as fallback
                if (!names.Contains(type.Name))
                {
                    names.Add(type.Name);
                }
            }
        }

        return names;
    }

    public static Dictionary<string, Type> GetEntityTypeMap()
    {
        if (_entityTypeMap == null)
        {
            _entityTypeMap = new Dictionary<string, Type>();
            var types = GetSchedulableEntityTypes();

            foreach (var type in types)
            {
                try
                {
                    var instance = Activator.CreateInstance(type) as ISchedulableEntity;
                    if (instance != null)
                    {
                        var eventType = instance.GetEventType();
                        if (!string.IsNullOrEmpty(eventType) && !_entityTypeMap.ContainsKey(eventType))
                        {
                            _entityTypeMap[eventType] = type;
                        }
                    }
                }
                catch
                {
                    // Skip types that can't be instantiated
                }
            }
        }

        return _entityTypeMap;
    }
}
