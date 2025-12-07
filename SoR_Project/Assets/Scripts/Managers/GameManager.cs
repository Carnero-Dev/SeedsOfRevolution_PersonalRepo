using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IGameManager
{
    void Start() {
        
    }
    public void LoadGame() {
        SceneManager.LoadScene("SaveSystem_Test");
    }
    public void Reset() {
        SceneManager.LoadScene("SaveSystem_Test");
    } 
}
