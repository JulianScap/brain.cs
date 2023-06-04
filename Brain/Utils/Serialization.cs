using Brain.Models;
using Newtonsoft.Json;

namespace Brain.Utils;

public static class Serialization
{
    public static T ReadFile<T>(string filePath)
    {
        if (filePath.StartsWith('~'))
        {
            string home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            filePath = Path.Combine(home, filePath[2..]);
        }

        var serializer = new JsonSerializer
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        using var sw = new StreamReader(filePath);
        using JsonReader writer = new JsonTextReader(sw);

        return serializer.Deserialize<T>(writer) ?? throw new BrainException("Null");
    }
}
