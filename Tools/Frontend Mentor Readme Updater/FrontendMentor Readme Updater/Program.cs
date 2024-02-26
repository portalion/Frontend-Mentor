using FrontendMentor_Readme_Updater;
using Microsoft.Extensions.Configuration;

HttpClient httpClient = new HttpClient();
const string url = "https://www.frontendmentor.io/challenges";

var configBuilder = new ConfigurationBuilder().AddUserSecrets<Program>();
var config = configBuilder.Build();

var pathOfReadme = config["FrontendMentorReadmePath"];
while (string.IsNullOrEmpty(pathOfReadme))
{
    Console.Write("Path of readme you want to update:");
    pathOfReadme = Console.ReadLine();
}


try
{
    FrontendMentorReader reader = new(httpClient);
    FrontendMentorParser parser = new FrontendMentorParser();
    ReadmeManager readmeManager = new(pathOfReadme);

    var website = await reader.ReadUrl(url);
    var listValues = await parser.ParseHtml(website);
    var cleanedData = FrontendMentorPrettier.SortAndAssignData(listValues);

    readmeManager.UpdateReadme(cleanedData);
}
catch (HttpRequestException e)
{
    Console.WriteLine("Exception thrown when trying to get http response");
    Console.WriteLine($"Message: {e.Message}");
}
catch (Exception e)
{
    Console.WriteLine("Exception thrown");
    Console.WriteLine($"Message: {e.Message}");
}