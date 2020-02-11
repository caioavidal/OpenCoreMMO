using System.Collections.Generic;

public class Attribute
{
    public string Key { get; set; }
    public string Value { get; set; }
}

public class Item
{
    public string Id { get; set; }
    public string Article { get; set; }
    public string Toid { get; set; }
    public string Fromid { get; set; }
    public string Name { get; set; }
    public IList<Attribute> Attribute { get; set; }
}



public class ItemConfig
{
    public static IReadOnlyCollection<Item> Items { get; private set; }
    public static async void Load()
    {
        if (Items == null)
        {
            Items = await JsonLoader.GetLoadedItems();
        }
    }
}



