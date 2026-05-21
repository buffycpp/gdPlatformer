using System.Collections.Generic;
using Godot;

public class CurrentRunSavegameData
{
    public int CurrentLevel = 0;
    public int LevelDeaths = 0;
    public float TotalTime = 0;
    public int TotalDeaths = 0;
    public List<string> CollectedStars = [];
}

public partial class SavegameData : Resource
{
    public CurrentRunSavegameData CurrentRun { get; set; } = null;
    public float TotalTime { get; set; } = 0;
    public int TotalDeaths { get; set; } = 0;
    public List<string> CollectedStars = [];

    public SavegameData()
    {

    }
}