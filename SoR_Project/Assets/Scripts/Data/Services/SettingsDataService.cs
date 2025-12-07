public static class SettingsDataService {
    public static SettingsData Current { get; private set; }

    public static void Init(SettingsData newData) {
        Current = newData;
    }

    public static void Clear() => Current = null;

    public static bool IsInitialized => Current != null;
}
