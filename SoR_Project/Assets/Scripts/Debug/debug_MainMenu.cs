using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class debug_MainMenu : MonoBehaviour
{
    [SerializeField] Button newGame_Button;
    [SerializeField] Button newDebugTesting_Button;
    [SerializeField] Button continue_Button;
    [SerializeField] Button exit_Button;
    
    [SerializeField] Button applyChanges_button;
    [SerializeField] Button resetChanges_button;
    [SerializeField] TMP_InputField profileName_Input;
    [SerializeField] TMP_Text volume_text;
    [SerializeField] Slider volume_slider;


    void Start() {
        newGame_Button.onClick.AddListener(delegate{NewGame(false);});
        newDebugTesting_Button.onClick.AddListener(delegate{NewGame(true);});
        continue_Button.onClick.AddListener(Continue);
        exit_Button.onClick.AddListener(Exit);
        
        applyChanges_button.onClick.AddListener(ApplyChanges);
        resetChanges_button.onClick.AddListener(ResetChanges);
        volume_slider.onValueChanged.AddListener(delegate{ChangeVolume();});

        UpdateUI();
        
    }
    void UpdateUI () {
        SettingsData settingsData = SettingsDataService.Current;
        MetaData metaData = MetaDataService.Current;

        volume_slider.value = settingsData.volumeData.musicVolume;
        profileName_Input.text = metaData.profileName;
    }

    void NewGame(bool debugEnabled) {
        SaveSystem.Clear<GameData>();
        SeedRandom.SetDebugMode(debugEnabled);
        SceneManager.LoadScene("Game_debug");
    }
    void Continue() {
        SceneManager.LoadScene("Game_debug");
    }
    void Exit() {
        Application.Quit();
        Debug.Log("Exit Game");
    }
    void ApplyChanges() {
        MetaDataService.Current.profileName = profileName_Input.text;
        SaveSystem.Save<MetaData>(MetaDataService.Current);
        SaveSystem.Save<SettingsData>(SettingsDataService.Current);
        UpdateUI();
    }

    void ChangeVolume() {
        SettingsDataService.Current.volumeData.musicVolume = volume_slider.value;
        SettingsDataService.Current.hasChanged = true;
        volume_text.text = volume_slider.value.ToString();
    }

	void ResetChanges() {
		MetaDataService.Init(SaveSystem.Load<MetaData>());
		SettingsDataService.Init(SaveSystem.Load<SettingsData>());
        UpdateUI();
	}



}
