public static class GameDataService {
    public static GameData Current { get; private set; }

    public static void Init(GameData newData) {
        newData.run.seedState.Deserialize();
        Current = newData;
    }

    public static void Clear() => Current = null;

    public static bool IsInitialized => Current != null;
}
