using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TMP_Text loadingText;

    public void SetProgress(float progress) => loadingBar.value = progress;
    public void SetMessage(string message) => loadingText.text = message;
}