using System;
using System.IO;
using UnityEngine;

public static class SaveSystem {
    private static string BasePath => Application.persistentDataPath;

    private static string gameSavePath => "player_save";
    private static string metaSavePath => "meta_save";
    private static string settingsSavePath => "settings_save";

    public static event Action OnGameSaved;
    public static event Action OnGameLoaded;
    public static event Action OnGameDataCleared;

    public static bool IsFileExist<T>() where T : class, IData, new() {
        string path = GetPath(GetSavePathName(new T()));
        if (File.Exists(path))
            return true;
        else
            return false;
    }
    

    private static string GetSavePathName(IData dataType) {
        switch (dataType) {
            case GameData:
                return gameSavePath;
            case MetaData:
                return metaSavePath;
            case SettingsData:
                return settingsSavePath;
            default:
                return "test_save";
        }
    }
 public static void Save<T>(T data) where T : IData
    {
        string path = GetPath(GetSavePathName(data));

        if (data is GameData gd) {
            gd.run.seed = SeedRandom.GetSeed();
            gd.run.seedState = SeedRandom.GetState();
            gd.run.seedState.Serialize();
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        if (data is GameData) OnGameSaved?.Invoke();
    }

    public static T Load<T>() where T : class, IData, new() {
        string path = GetPath(GetSavePathName(new T()));

        if (!File.Exists(path)) {
            Debug.Log($"No file found for {GetSavePathName(new T())}, creating new.");
            var newData = DataFactory.Create<T>();

            if (newData is GameData gd) {
                gd.run.seedState.Deserialize();
                OnGameLoaded?.Invoke();
            }
            return newData;
        }
        string json = File.ReadAllText(path);
        T data = JsonUtility.FromJson<T>(json);

        if (data is GameData gd2) {
            gd2.run.seedState.Deserialize();
            OnGameLoaded?.Invoke(); 
        }

        return data;
    }

    public static void Clear<T>() where T : IData, new() {
        string path = GetPath(GetSavePathName(new T()));
        if (File.Exists(path)) File.Delete(path);
        if (typeof(T) == typeof(GameData)) OnGameDataCleared?.Invoke();
    }

    private static string GetPath(string fileName) => Path.Combine(BasePath, $"{fileName}.json");

    // Antiguo
    // public static void SaveGameData(GameData data)
    // {
    //     Debug.Log("Saving data on Path => " + BasePath);
    //     SeedState state = SeedRandom.GetState();
    //     state.Serialize();

    //     data.run.seed = SeedRandom.GetSeed();
    //     data.run.seedState = state;

    //     string json = JsonUtility.ToJson(data, true);
    //     File.WriteAllText(BasePath, json);
    //     OnGameSaved?.Invoke();
    // }

    // public static GameData LoadGameData() {
    //     if (!IsFileExist()) {
    //         Debug.Log("Save file not found. Creating new data");
    //         GameData newData = DataFactory.Create<GameData>();
    //         newData.run.seedState.Deserialize();

    //         OnGameLoaded?.Invoke();
    //         return newData;
    //     }
    //     string json = File.ReadAllText(BasePath);
    //     Debug.Log("Game Data loaded succesfuly");
    //     GameData data = JsonUtility.FromJson<GameData>(json);
    //     data.run.seedState.Deserialize();
    //     OnGameLoaded?.Invoke();
    //     return data;
    // }

    // public static void ClearSaveData() {
    //     if (IsFileExist())
    //         Debug.Log("Deleting data from path =>" + BasePath);
    //         File.Delete(BasePath);
    //         SetNewGame();
    //         OnDataCleared?.Invoke();
    // }




}
