using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services;
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
        UGSInitializer.OnUGSReady += CheckIfNicknameIsNeeded;
    }

    private void OnDisable()
    {
        UGSInitializer.OnUGSReady -= CheckIfNicknameIsNeeded;
    }
    
    private async void CheckIfNicknameIsNeeded()
    {
        // 1. Chequear si ya hay nickname en local
        string localName = PlayerPrefs.GetString("PlayerName", "");
        if (!string.IsNullOrWhiteSpace(localName))
        {
            Debug.Log($"✅ Nickname found in local: {localName}. Panel not shown.");
            nicknamePanel.SetActive(false);
            return;
        }

        // 2. Chequear en la nube
        var cloudName = await CloudNicknameHandler.LoadNickname();
        if (!string.IsNullOrWhiteSpace(cloudName))
        {
            Debug.Log($"✅ Nickname found in cloud: {cloudName}. Caching locally.");
            PlayerPrefs.SetString("PlayerName", cloudName);
            PlayerPrefs.Save();
            nicknamePanel.SetActive(false);
            return;
        }

        // 3. No hay nickname → mostrar panel
        Debug.Log("👤 No nickname found. Showing panel.");
        nicknamePanel.SetActive(true);
        SetupUI();
    }
    
    private void SetupUI()
    {
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
