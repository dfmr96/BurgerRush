using UnityEngine;
using UnityEngine.UI;

public class VibrationOnClick : MonoBehaviour
{
    [SerializeField] private VibrationPreset vibrationPreset;
    private void Start()
    {
        Button btn = GetComponent<Button>();
      
        btn.onClick.AddListener(() => VibrationManager.Vibrate(vibrationPreset));
    }
}