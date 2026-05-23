public interface ISaveable
{
    void OnSave(SavegameData save);
    void OnLoad(SavegameData save);
}

public static class SaveableExtensions
{
    public static void SubscribeAsSaveable(this ISaveable saveable)
    {
        SavegameManager.Instance.OnSaveEvent += saveable.OnSave;
        SavegameManager.Instance.OnLoadEvent += saveable.OnLoad;
    }
}