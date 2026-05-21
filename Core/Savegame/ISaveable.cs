public interface ISaveable
{
    void SubscribeAsSaveable()
    {
        SavegameManager.Instance.OnSaveEvent += OnSave;
        SavegameManager.Instance.OnLoadEvent += OnLoad;
    }

    void OnSave(SavegameData save);
    void OnLoad(SavegameData save);
}