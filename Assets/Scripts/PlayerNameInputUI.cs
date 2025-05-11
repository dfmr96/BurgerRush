using Services.Cloud;
using TMPro;
using UnityEngine;

public class PlayerNameInputUI: MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;

    private async void Start()
    {
        // Intenta cargar el nombre al iniciar
        var name = await CloudNicknameHandler.LoadNickname();
        nameInputField.text = string.IsNullOrEmpty(name) ? "Player" : name;
    }

    public async void OnNameChanged(string newName)
    {
        await CloudNicknameHandler.SaveNickname(newName);
        Debug.Log($"✅ Nickname saved: {newName}");
    }
}