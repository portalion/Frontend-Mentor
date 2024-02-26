namespace FrontendMentor_Readme_Updater;

public static class FrontendMentorPrettier
{
    public static Dictionary<string, List<FrontendMentorChallengeData>> SortAndAssignData(List<FrontendMentorChallengeData> data)
    {
        Dictionary<string, List<FrontendMentorChallengeData>> sortedData = new();
        foreach (var value in data)
        {
            if (!sortedData.ContainsKey(value.Difficulty))
            {
                sortedData.Add(value.Difficulty, new List<FrontendMentorChallengeData>());
            }
            sortedData[value.Difficulty].Add(value);
        }

        foreach (var setToSort in sortedData)
        {
            setToSort.Value.Sort((x, y) => x.Title.CompareTo(y.Title));
        }
        return sortedData;
    }
}
