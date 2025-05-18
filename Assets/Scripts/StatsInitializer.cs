using Databases;
using Services;
using UnityEngine;

public class StatsInitializer : MonoBehaviour
{
    [SerializeField] private PlayerStatsDatabase statsDB;

    private void Awake()
    {
        PlayerStatsService.InitializeAllStats(statsDB);
    }
}