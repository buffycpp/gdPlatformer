using System;
using System.Text.Json;
using Godot;

public partial class SavegameManager : SingletonNode<SavegameManager>
{
    public event Action<SavegameData> OnSaveEvent;
    public event Action<SavegameData> OnLoadEvent;
    public SavegameData CurrentSave { get; private set; }
    private string _saveId = "1"; //This should probably be set to accountId later
    private bool _autosaveEnabled = true;

    private string GetSavePath(string saveId)
    {
        return $"user://save_{saveId}.json";
    }

    public override void _Ready()
    {        
        Initialise();
    }

    public void Initialise()
    {
        try
        {
            Load();
            GD.Print("Savegame loaded successfully.");
        }
        catch (Exception e)
        {
            GD.PrintErr($"Failed to load savegame: {e.Message}");
            GD.Print("Starting new savegame.");
            CurrentSave = new SavegameData();
            BroadcastLoadEvent();
        }
    }

    public void Save()
    {
        OnSaveEvent?.Invoke(CurrentSave);

        string path = GetSavePath(_saveId);
        string json = JsonSerializer.Serialize(CurrentSave);

        using var file = FileAccess.Open(
            path,
            FileAccess.ModeFlags.Write
        );

        file.StoreString(json);

    }

    public void Load()
    {
        string path = GetSavePath(_saveId);

        if (!FileAccess.FileExists(path))
        {
            throw new Exception($"No save found for id={_saveId}");
        }            

        using var file = FileAccess.Open(
            path,
            FileAccess.ModeFlags.Read
        );

        string json = file.GetAsText();

        CurrentSave = JsonSerializer.Deserialize<SavegameData>(json);

        BroadcastLoadEvent();
    }

    public void BroadcastLoadEvent()
    {
        OnLoadEvent?.Invoke(CurrentSave);
        GD.Print("Game-load broadcasted");
    }

    public void TriggerAutosave()
    {
        if (_autosaveEnabled)
        {
            Save();
            GD.Print("Game autosaved.");
        }
    }
}