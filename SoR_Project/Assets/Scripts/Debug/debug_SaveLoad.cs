using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class debug_SaveLoad : MonoBehaviour {
	[SerializeField] Button saveButton;
	[SerializeField] Button loadButton;
	
	void Start() {
		loadButton.onClick.AddListener(LoadGame);
		saveButton.onClick.AddListener(SaveGame);
	}

	void SaveGame() {
		SaveSystem.Save<GameData>(GameDataService.Current);
	}

	void LoadGame() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}