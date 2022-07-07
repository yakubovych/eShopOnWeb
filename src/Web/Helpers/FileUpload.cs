namespace Microsoft.eShopWeb.Web.Helpers;

public static class FileUpload
{
    public static async Task UploadAsync(string jsonData)
    {
        System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "orderReserver.json", jsonData);

        HttpClient http = new HttpClient();
        using var content = new MultipartFormDataContent();
        using var fileStream = File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + "orderReserver.json");
        content.Add(new StreamContent(fileStream), "file", "ReservedOrder.json");

        await http.PostAsync("https://fileuploadordersreserver.azurewebsites.net/api/OrederItemsReserver?", content);
    }
}
