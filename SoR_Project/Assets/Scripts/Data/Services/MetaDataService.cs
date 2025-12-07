public static class MetaDataService {
    public static MetaData Current { get; private set; }

    public static void Init(MetaData newData) {
        Current = newData;
    }

    public static void Clear() => Current = null;

    public static bool IsInitialized => Current != null;
}
