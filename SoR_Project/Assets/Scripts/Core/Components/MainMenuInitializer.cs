using UnityEngine;

public class MainMenuInitializer : MonoBehaviour
{
    void Awake() {
        InitPersistenData();
        InitGameDataPreview();
        
    }
    void InitPersistenData() {
        if (!SaveSystem.IsFileExist<MetaData>())
            MetaDataService.Init(DataFactory.Create<MetaData>());
        else MetaDataService.Init(SaveSystem.Load<MetaData>());

        if (!SaveSystem.IsFileExist<SettingsData>())
            SettingsDataService.Init(DataFactory.Create<SettingsData>());
        else SettingsDataService.Init(SaveSystem.Load<SettingsData>());
    }

    void InitGameDataPreview() {
        if (!SaveSystem.IsFileExist<GameData>()) {
            GameDataService.Clear();
            //TODO: Ocultar continuar
        }
        else {
            GameDataService.Init(SaveSystem.Load<GameData>());
            //TODO: Mostrar info de la partida
        }        
    }
}
