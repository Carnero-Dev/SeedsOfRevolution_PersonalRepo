using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugSaveUI : MonoBehaviour
{
    [Header("SERVICES")]
    IGameManager _gameManager;
    [Header("Debug_Seeds")]
    int seedRandomNum;
    int randomNum;

    [Header("SaveSystem")]
    public TMP_Text levelNameText;
    public TMP_Text currentLevelText;

    public TMP_InputField playerNameInput;
    public Button saveButton;
    public Button loadButton;
    public Button changeNameButton;
    public Button resetPlayerData;

    [Header("SeedSystem")]
    public TMP_Text seedText;
    
    public TMP_Text runNumText;
    public TMP_Text seedRandomNumText;
    public TMP_Text randomNumText;

    public Button rerollSeeds;
    public Button rerollRngNums;


    void Start() {
        //Get Services
        _gameManager = ServiceLocator.Get<IGameManager>();

        loadButton.onClick.AddListener(LoadGame);
        saveButton.onClick.AddListener(SaveGame);
        changeNameButton.onClick.AddListener(ChangeName);
        resetPlayerData.onClick.AddListener(ClearSaveData);
        rerollSeeds.onClick.AddListener(Reset);
        rerollRngNums.onClick.AddListener(RerollRngNums);

        UpdateUI();
    }

    void LoadGame() {
        _gameManager.LoadGame();
        UpdateUI();
    }

    void SaveGame() {
        SaveSystem.Save<GameData>(GameDataService.Current);
        UpdateUI();
    }

    void ChangeName() {
        if(playerNameInput == null) return;        
        UpdateUI();
    }

    void ClearSaveData() {
        SaveSystem.Clear<GameData>();
    }

    void Reset() {
        //_gameManager.Reset();
        SeedRandom.Init();
        GameDataService.Current.run.seed = SeedRandom.GetSeed();
        UpdateUI();
    }

    void RerollRngNums() {
        GameDataService.Current.run.randomNum = SeedRandom.RangeInt(SeedCategory.TEST, 1, 101);
        seedRandomNum = SeedRandom.RangeInt(SeedCategory.GLOBAL, 1, 101);
        randomNum = Random.Range(1, 101);

        UpdateUI();
    }

    void UpdateUI() {

        seedText.text = $"Seed: {GameDataService.Current.run.seed}";
        runNumText.text = $" Run Random Num: {GameDataService.Current.run.randomNum}";
        seedRandomNumText.text = $"Seed Random Num: {seedRandomNum}";
        randomNumText.text = $"Full Random Num: {randomNum}";
    }
}
