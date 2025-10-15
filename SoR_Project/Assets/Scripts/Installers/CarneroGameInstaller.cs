using UnityEngine;

public class CarneroGameInstaller : MonoBehaviour
{
    public TimeManager timeManager;
    private void Awake() {
        ServiceLocator.Instance.RegisterService<TimeManager>(timeManager);
    }
}
