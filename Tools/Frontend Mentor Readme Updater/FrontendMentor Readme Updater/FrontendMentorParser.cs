using AngleSharp;

namespace FrontendMentor_Readme_Updater;

public struct FrontendMentorChallengeData
{
    public string Title;
    public string Difficulty;
    public bool Premium;
}

public class FrontendMentorParser
{
    readonly IConfiguration config;
    readonly IBrowsingContext context;

    public FrontendMentorParser()
    {
        config = Configuration.Default;
        context = BrowsingContext.New(config);
    }

    public async Task<List<FrontendMentorChallengeData>> ParseHtml(string source)
    {
        var result = new List<FrontendMentorChallengeData>();

        var document = await context.OpenAsync(req => req.Content(source));
        var cards = document.QuerySelectorAll("div").Where(a => a?.ClassName?.Contains("Card__Wrapper") ?? false);
        foreach(var card in cards)
        {
            var toAppend = new FrontendMentorChallengeData();

            try
            {
                var divWithNeededData = card.ChildNodes[2];
                var dificultyDiv = divWithNeededData?.ChildNodes[1].ChildNodes[1].FirstChild;

                var difficultyNumber = dificultyDiv?.ChildNodes[0].TextContent ?? string.Empty;
                var difficultyText = dificultyDiv?.ChildNodes[1].TextContent ?? string.Empty;

                toAppend.Title = divWithNeededData?.FirstChild?.FirstChild?.TextContent ?? string.Empty;
                
                if(string.IsNullOrEmpty(toAppend.Title) || 
                    string.IsNullOrEmpty(difficultyText) ||
                    string.IsNullOrEmpty(difficultyNumber))
                {
                    continue;
                }
                toAppend.Difficulty = $"{difficultyNumber} - {difficultyText}";

                result.Add(toAppend);
            }
            catch { }
        }
        return result;
    }
}
