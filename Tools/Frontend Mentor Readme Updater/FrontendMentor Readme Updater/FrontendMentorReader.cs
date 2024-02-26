namespace FrontendMentor_Readme_Updater;

public class FrontendMentorReader
{
    readonly HttpClient _httpClient;
    const int CacheExpiringInDays = 7;

    public FrontendMentorReader(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private string GetCache(string cachePath)
    {
        var path = $"{cachePath}.txt";
        if(!File.Exists(path) || 
            (DateTime.Now - File.GetLastWriteTime(path)).Days > CacheExpiringInDays)
        {
            return string.Empty;
        }

        return File.ReadAllText(path);
    }

    private void SetCache(string cachePath, string text)
    {
        File.WriteAllText($"{cachePath}.txt", text);
    }

    public async Task<string> ReadUrl(string url)
    {
        //Get data from cache if possible
        var cachePath = url.Split("//").Skip(1).First().Split(".").Skip(1).First();
        if (cachePath is null) throw new Exception("Invalid Url");
        var result = GetCache(cachePath);

        if (!string.IsNullOrEmpty(result)) return result;
        
        Console.WriteLine("Cache expired, getting data from url");

        //Otherwise fetch from url and save to cache
        using HttpResponseMessage response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        result = await response.Content.ReadAsStringAsync();

        SetCache(cachePath, result);

        return result;
    }
}
