using Databases;
using UnityEngine;

namespace Services.Cloud
{
    public class CloudSyncOnUgsReady : MonoBehaviour
    {
        [SerializeField] private PlayerStatsDatabase statsDB;

        private void OnEnable()
        {
            UgsInitializer.OnUGSReady += HandleUGSReady;
        }

        private void OnDisable()
        {
            UgsInitializer.OnUGSReady -= HandleUGSReady;
        }

        private async void HandleUGSReady()
        {
            await CloudSyncService.ValidateCloudSync(statsDB);
        }
    }
}