using FrontendMentor_Readme_Updater;

HttpClient httpClient = new HttpClient();
const string url = "https://www.frontendmentor.io/challenges";

try
{
    FrontendMentorReader reader = new(httpClient);
    FrontendMentorParser parser = new FrontendMentorParser();

    var website = await reader.ReadUrl(url);
    var listValues = await parser.ParseHtml(website);
    var cleanedData = FrontendMentorPrettier.SortAndAssignData(listValues);
    var result = cleanedData.OrderBy(x => x.Key);
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