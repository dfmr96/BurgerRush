using Services.Cloud;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_InputField))]
public class PlayerNameInputUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;
    private const string DefaultName = "Player";

    private async void Start()
    {
        // Carga y aplica el nombre al iniciar
        var loadedName = await CloudNicknameHandler.LoadNickname();
        nameInputField.text = string.IsNullOrWhiteSpace(loadedName) ? DefaultName : loadedName;
        
        nameInputField.onEndEdit.AddListener(OnNameChanged);
    }

    private async void OnNameChanged(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            Debug.LogWarning("⚠️ Attempted to save empty or whitespace nickname.");
            return;
        }

        await CloudNicknameHandler.SaveNickname(newName);
        Debug.Log($"✅ Nickname saved: {newName}");
    }

    private void OnDestroy()
    {
        nameInputField.onEndEdit.RemoveListener(OnNameChanged);
    }
}