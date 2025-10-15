using TMPro;
using UnityEngine;

public class debug_InteractablesUI : MonoBehaviour
{
    public TextMeshProUGUI actionText;

    public void SetAction(string action){
        actionText.text = action;
    }
}
