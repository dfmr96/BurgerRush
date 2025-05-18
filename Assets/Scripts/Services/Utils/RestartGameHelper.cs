using UnityEngine.SceneManagement;

namespace Services.Utils
{
    public class RestartGameHelper
    {
        public static void RestartSceneWithInterstitial()
        {
            AdOverlayHandler.ShowInterstitialWithBannerRestore(() =>
            {
                AdsManager.Instance.IncrementPlayCount();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });
        }
    }
}