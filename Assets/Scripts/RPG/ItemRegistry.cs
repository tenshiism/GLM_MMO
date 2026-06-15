using UnityEngine;
using System.Collections.Generic;

public static class ItemRegistry
{
    private static Dictionary<string, ItemDefinition> items;
    private static bool initialized;

    public static void Initialize()
    {
        if (initialized) return;
        items = new Dictionary<string, ItemDefinition>();
        var allItems = Resources.LoadAll<ItemDefinition>("Items");
        foreach (var item in allItems)
        {
            if (item != null && !items.ContainsKey(item.name))
                items[item.name] = item;
        }
        initialized = true;
    }

    public static ItemDefinition Get(string id)
    {
        Initialize();
        items.TryGetValue(id, out var item);
        return item;
    }

    public static T Get<T>(string id) where T : ItemDefinition
    {
        return Get(id) as T;
    }
}
