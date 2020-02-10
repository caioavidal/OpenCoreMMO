using System.IO;
using System.Threading.Tasks;

public class OtbFile
{
    public static byte[] Items {get;private set;}
    public static async Task LoadItems()
    {
        Items = await File.ReadAllBytesAsync("Data/Items/items.otb");
    }
}