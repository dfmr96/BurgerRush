using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class MainMenuUIManager: MonoBehaviour
    {
        [SerializeField] private GameObject startGamePanel;

        public void OnPlayPressed()
        {
            startGamePanel.SetActive(true);
        }

        public void OnStartGamePressed()
        {
            SceneManager.LoadScene("Gameplay");
        }
        
        public void OnCancelStartGame()
        {
            startGamePanel.SetActive(false);
        }
    }
}