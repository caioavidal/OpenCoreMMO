using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

public class JsonLoader
{

    public static async Task<T> ConvertToObject<T>(string path)
    {
        var myJsonString = await File.ReadAllTextAsync("Data/vocation.json"); //todo: add on appsettings.json
        return JsonConvert.DeserializeObject<T>(myJsonString);
    }

    public static async void LoadVocation(IServiceCollection service)
    {
        Console.WriteLine("Loading vocation!"); //todo: change to event

        var vocation = await ConvertToObject<Vocation>("Data/vocation.json");

        service.AddSingleton<Vocation>(vocation); //todo: change to event
        Console.WriteLine("Vocation loaded!"); //todo: change to event
    }

}