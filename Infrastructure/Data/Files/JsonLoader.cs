using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public static async Task<IReadOnlyDictionary<string, Vocation>> GetLoadedVocations()
    {
        Console.WriteLine("Loading vocation!"); //todo: change to event

        var vocations = await ConvertToObject<IList<Vocation>>("Data/vocations.json");

        Console.WriteLine("Vocation loaded!"); //todo: change to event

        return vocations.ToDictionary(x=>x.Name,x=>x);


        
    }

}