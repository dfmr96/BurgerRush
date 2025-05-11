using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services.Cloud;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class NicknamePanelController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button confirmButton;
    [SerializeField] private GameObject nicknamePanel;
    
    private void OnEnable()
    {
        UGSInitializer.OnUGSReady += InitPanel;
    }

    private void OnDisable()
    {
        UGSInitializer.OnUGSReady -= InitPanel;
    }
    
    private void InitPanel()
    {
        // Primero chequeo localmente
        string localName = PlayerPrefs.GetString("PlayerName", "");

        if (!string.IsNullOrWhiteSpace(localName))
        {
            nicknamePanel.SetActive(false);
            Debug.Log($"✅ Nickname already stored locally: {localName}. Skipping panel.");
            return;
        }

        // Si no hay, buscar en la nube
        LoadNameFromCloud();
    }

    private async void LoadNameFromCloud()
    {
        var name = await CloudNicknameHandler.LoadNickname();

        if (!string.IsNullOrWhiteSpace(name))
        {
            Debug.Log($"👤 Nickname already set: {name}. Hiding panel.");
            nicknamePanel.SetActive(false);
            return;
        }

        // Mostrar el panel y permitir ingresar el nombre
        nicknamePanel.SetActive(true);
        nameInputField.text = "";
        nameInputField.onValueChanged.AddListener(OnInputChanged);
        UpdateButtonInteractable();
    }

    private void OnInputChanged(string newText)
    {
        UpdateButtonInteractable();
    }

    private void UpdateButtonInteractable()
    {
        bool hasValidText = !string.IsNullOrWhiteSpace(nameInputField.text);
        bool isSignedIn = AuthenticationService.Instance.IsSignedIn;

        confirmButton.interactable = hasValidText && isSignedIn;
    }

    public async void OnConfirmClicked()
    {
        string nickname = nameInputField.text.Trim();
        if (!string.IsNullOrEmpty(nickname))
        {
            await CloudNicknameHandler.SaveNickname(nickname);
            PlayerPrefs.SetString("PlayerName", nickname);
            PlayerPrefs.Save();
            Debug.Log($"✅ Nickname '{nickname}' saved locally and to cloud.");
            nicknamePanel.SetActive(false);
        }
    }
}
