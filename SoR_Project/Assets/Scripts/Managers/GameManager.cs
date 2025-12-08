using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IGameManager
{
	void Start() {
        
    }
    
#region Handlers
	public void HandleDataCleared() {
		Debug.Log("Data Cleared");
	}

	public void HandleGameLoaded() {
		Debug.Log("Game Loaded");
	}

	public void HandleGameSaved() {
		Debug.Log("Game Saved");
	}
#endregion

#region GameFlow
	public void OnLoadGame() {
		throw new System.NotImplementedException();
	}

	public void OnNewGame() {
		throw new System.NotImplementedException();
	}

	public void StartGame() {
		throw new System.NotImplementedException();
	}

    // public void LoadGame() {
    //     SceneManager.LoadScene("SaveSystem_Test");
    // }
    // public void Reset() {
    //     SceneManager.LoadScene("SaveSystem_Test");
    // } 
#endregion
}
