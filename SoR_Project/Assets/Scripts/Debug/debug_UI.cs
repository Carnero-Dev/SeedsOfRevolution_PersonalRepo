using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class debug_UI : MonoBehaviour
{
    [Header("SERVICES")]
    IGameManager _gameManager;
    [Header("Debug_Seeds")]
    int seedRandomNum;
    int randomNum;

    [Header("SaveSystem")]
    public TMP_Text playerName;
    public TMP_Text currentLevelText;

    public TMP_InputField playerNameInput;

    public Button changeNameButton;

    [Header("SeedSystem")]
    public TMP_Text seedText;
    
    public TMP_Text runNumText;
    public TMP_Text seedRandomNumText;
    public TMP_Text randomNumText;

    public Button rerollSeeds;
    public Button rerollRngNums;
	void Awake() {
		SaveSystem.OnGameLoaded += UpdateUI;
	}
	void OnDisable() {
		SaveSystem.OnGameLoaded -= UpdateUI;
	}
	void Start() {
        //Get Services
        _gameManager = ServiceLocator.Get<IGameManager>();
        changeNameButton.onClick.AddListener(ChangeName);
        rerollSeeds.onClick.AddListener(RerollSeeds);
        rerollRngNums.onClick.AddListener(RerollRngNums);

        UpdateUI();
    }

    void ChangeName() {
        if(playerNameInput == null) return;        
        UpdateUI();
    }

    void RerollSeeds() {
        //_gameManager.Reset();
        SeedRandom.Init();
        GameDataService.Current.run.seed = SeedRandom.GetSeed();
        UpdateUI();
    }

    void RerollRngNums() {
        GameDataService.Current.run.randomNum = SeedRandom.RangeInt(SeedCategory.TEST, 1, 101);
        seedRandomNum = SeedRandom.RangeInt(SeedCategory.GLOBAL, 1, 101);
        randomNum = UnityEngine.Random.Range(1, 101);

        UpdateUI();
    }

    void UpdateUI() {

        seedText.text = $"Seed: {GameDataService.Current.run.seed}";
        runNumText.text = $" Run Random Num: {GameDataService.Current.run.randomNum}";
        seedRandomNumText.text = $"Seed Random Num: {seedRandomNum}";
        randomNumText.text = $"Full Random Num: {randomNum}";
    }
}
