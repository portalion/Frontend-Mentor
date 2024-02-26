﻿using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;

namespace FrontendMentor_Readme_Updater;

public struct ReadmeChallengeListData
{
    public string doneText;
    public string nameOfChallenge;
    public string difficulty;
    public string link;
}

public class ReadmeManager
{
    readonly string readmePath;

    public ReadmeManager(string readmePath)
    {
        this.readmePath = readmePath;
    }

    public string GetChallengeListSection()
    {
        Regex regex = new(@"^## ", RegexOptions.Multiline);
        var text = File.ReadAllText(readmePath);
        var paragraphs = regex.Split(text);
        var challengeList = paragraphs.Where(s => s.StartsWith("List of Challenges")).FirstOrDefault();
        
        if (string.IsNullOrEmpty(challengeList))
        {
            throw new Exception("Cannot find challenge list in readme");
        }

        return challengeList.Trim();
    }

    public Dictionary<string, List<ReadmeChallengeListData>> ParseChallengeListSection(string challengeList)
    {
        Dictionary<string, List<ReadmeChallengeListData>> result = new();
        Regex regex = new(@"^### ", RegexOptions.Multiline);
        var challengeTables = regex.Split(challengeList).Skip(1);

        foreach(var table in challengeTables) 
        {
            var tableElements = table.Split("\r\n").Select(r => r.Trim());
            var title = tableElements.First().ToLower();
            tableElements = tableElements.Skip(4);
            result.Add(title, new List<ReadmeChallengeListData>());
            foreach(var row in tableElements)
            {
                var challengeData = new ReadmeChallengeListData();

                var rowValues = row.Split("|").Select(row=> row.Trim()).Skip(1).SkipLast(1).ToArray();

                challengeData.difficulty = title;
                challengeData.doneText = rowValues[0];
                challengeData.nameOfChallenge = rowValues[1];
                challengeData.link = rowValues[2];

                result[title].Add(challengeData);
            }
        }

        return result;
    }

    public Dictionary<string, List<ReadmeChallengeListData>> MapFrontendMentorDataToReadmeData(Dictionary<string, List<FrontendMentorChallengeData>> dataToMap)
    {
        var result = new Dictionary<string, List<ReadmeChallengeListData>>();
        foreach(var key in dataToMap.Keys)
        {
            result.Add(key, new List<ReadmeChallengeListData>());
        }
        foreach(var data in dataToMap.Values)
        {
            foreach(var value in data)
            {
                ReadmeChallengeListData toAdd = new();
                toAdd.difficulty = value.Difficulty;
                toAdd.nameOfChallenge = value.Title;
                toAdd.doneText = ":white_large_square:";
                result[toAdd.difficulty].Add(toAdd);
            }
        }
        return result;
    }

    public Dictionary<string, List<ReadmeChallengeListData>> MergeData(Dictionary<string, List<ReadmeChallengeListData>> original, Dictionary<string, List<ReadmeChallengeListData>> toMerge)
    {
        foreach(var difficultyData in original)
        {
            foreach(var challengeData in difficultyData.Value)
            {
                if (toMerge[difficultyData.Key].Exists(a => a.nameOfChallenge == challengeData.nameOfChallenge)) 
                {
                    var found = toMerge[difficultyData.Key].Find(a => a.nameOfChallenge == challengeData.nameOfChallenge);
                    found.link = challengeData.link;
                    found.doneText = challengeData.doneText;
                }
                else toMerge[difficultyData.Key].Add(challengeData);
            }
        }
        return toMerge;
    }

    public void UpdateReadme(Dictionary<string, List<FrontendMentorChallengeData>> dataToSwap)
    {
        var readmeChallenges = ParseChallengeListSection(GetChallengeListSection());
        var mappedFrontendMentorChallenges = MapFrontendMentorDataToReadmeData(dataToSwap);
        var mergedChallenges = MergeData(readmeChallenges, mappedFrontendMentorChallenges).OrderBy(a => a.Key);

    }
}
