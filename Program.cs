using System.Text.Json;

string jsonFilePath = "v1.json";
string downloadDirectory = "downloaded_images";

await JsonDownload(jsonFilePath);

List<ImageInfo> images = ImageUrlObtainer(jsonFilePath, downloadDirectory);

await Downloader(downloadDirectory, images);

static List<ImageInfo> ImageUrlObtainer(string jsonFilePath, string downloadDirectory)
{
    Directory.CreateDirectory(downloadDirectory);

    string jsonContent = File.ReadAllText(jsonFilePath);
    var jsonDocument = JsonDocument.Parse(jsonContent);
    List<ImageInfo> images = [];

    foreach (var document in jsonDocument.RootElement.GetProperty("data").GetProperty("documents").EnumerateArray())
    {
        bool isMarket = document.GetProperty("fields").EnumerateArray()
            .Any(field => field.GetProperty("key").GetString() == "title" &&
                          field.GetProperty("value").GetString().Contains("Market"));
        bool containsTucuman = document.GetProperty("fields").EnumerateArray()
            .Any(field => field.GetProperty("key").GetString() == "cities" &&
                          field.GetProperty("value").GetString().Contains("Tucumán"));
        if (!(isMarket && containsTucuman))
            continue;
        foreach (var field in document.GetProperty("fields").EnumerateArray())
        {
            if (field.GetProperty("key").GetString() == "images")
            {
                images = JsonSerializer.Deserialize<List<ImageInfo>>(field.GetProperty("value").GetString());
                break;
            }
        }
    }
    return images;
}
static async Task Downloader(string downloadDirectory, List<ImageInfo> images)
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
static async Task JsonDownload(string jsonFilePath)
{
    using (HttpClient client = new HttpClient())
    {
        string Url = @"
    https://www.carrefour.com.ar/_v/public/graphql/v1?workspace=master&maxAge=short&appsEtag=remove&domain=store&locale=es-AR&operationName=GetBrochures&variables=%7B%7D&extensions=%7B%22persistedQuery%22%3A%7B%22version%22%3A1%2C%22sha256Hash%22%3A%221c12219b0c54037ae949ec91600b8c1d770c13ef2cb761e46e87a4ea10fc5194%22%2C%22sender%22%3A%22valtech.carrefourar-brochures%401.x%22%2C%22provider%22%3A%22vtex.store-graphql%402.x%22%7D%2C%22variables%22%3A%22eyJ3aGVyZSI6IihkYXRlVGltZUVuZD4yMDI0LTExLTA1VDIwOjM0OjIyLjA2MFopIiwiYWNjb3VudCI6ImNhcnJlZm91cmFyIn0%3D%22%7D
    ";
        byte[] Bytes = await client.GetByteArrayAsync(Url);
        await File.WriteAllBytesAsync(jsonFilePath, Bytes);
        Console.WriteLine($"Downloaded {jsonFilePath}");
    }
}