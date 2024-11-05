using System.Text.Json;

string jsonFilePath = @"C:\Users\Juan Cruz Avila\Documents\FolletosCrf\v1.json";
string downloadDirectory = "downloaded_images";

Directory.CreateDirectory(downloadDirectory);

string jsonContent = await File.ReadAllTextAsync(jsonFilePath);
var jsonDocument = JsonDocument.Parse(jsonContent);

foreach (var document in jsonDocument.RootElement.GetProperty("data").GetProperty("documents").EnumerateArray())
{
    bool isBlackCarrefourMarket = document.GetProperty("fields").EnumerateArray()
        .Any(field => field.GetProperty("key").GetString() == "title" &&
                      field.GetProperty("value").GetString() == "Black Carrefour Market");

    bool containsTucuman = document.GetProperty("fields").EnumerateArray()
        .Any(field => field.GetProperty("key").GetString() == "cities" &&
                      field.GetProperty("value").GetString().Contains("Tucumán"));

    if (!(isBlackCarrefourMarket && containsTucuman))
        continue;

    foreach (var field in document.GetProperty("fields").EnumerateArray())
    {
        if (field.GetProperty("key").GetString() == "images")
        {
            var images = JsonSerializer.Deserialize<List<ImageInfo>>(field.GetProperty("value").GetString());
            await Dowloader(downloadDirectory, images);
            break;
        }
    }
}

static async Task Dowloader(string downloadDirectory, List<ImageInfo> images)
{
    using (HttpClient client = new HttpClient())
    {
        int pageNum = 1;
        foreach (var imageInfo in images)
        {
            string imageUrl = imageInfo.image;
            byte[] imageBytes = await client.GetByteArrayAsync(imageUrl);
            string filePath;
            if (pageNum < 10)
            {
                filePath = Path.Combine(downloadDirectory, $"0{pageNum}.jpg");

            }
            else
            {
                filePath = Path.Combine(downloadDirectory, $"{pageNum}.jpg");
            }
            await File.WriteAllBytesAsync(filePath, imageBytes);
            Console.WriteLine($"Downloaded {filePath}");

            pageNum++;
        }
    }
}

public class ImageInfo
{
    public string name { get; set; }
    public string image { get; set; }
}