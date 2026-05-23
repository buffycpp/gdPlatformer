using System.Collections.Generic;
using Godot;

public class CurrentRunSavegameData
{
    public int CurrentLevel {get; set;} = 0;
    public int LevelDeaths {get; set;} = 0;
    public float TotalTime {get; set;} = 0;
    public int TotalDeaths {get; set;} = 0;
    public List<string> CollectedStars {get; set;} = [];
}

public class SavegameData
{
    public CurrentRunSavegameData CurrentRun { get; set; } = null;
    public float TotalTime { get; set; } = 0;
    public int TotalDeaths { get; set; } = 0;
    public List<string> CollectedStars {get; set;} = [];

    public SavegameData()
    {

    }

    public void StartNewGame()
    {
        CurrentRun = new CurrentRunSavegameData();
    }
}