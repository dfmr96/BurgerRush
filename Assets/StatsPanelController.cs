using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsPanelController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform statsContentParent; // Content del Scroll View
    [SerializeField] private StatItem statItemPrefab;

    private void OnEnable()
    {
        PopulateStats();
    }

    private void PopulateStats()
    {
        // Primero, limpiar stats previas si las hay
        foreach (Transform child in statsContentParent)
        {
            Destroy(child.gameObject);
        }

        // Diccionario o lista de stats a mostrar
        var stats = new Dictionary<string, string>
        {
            { "High Score", PlayerPrefs.GetInt("HighScore", 0).ToString() },
            { "Max Streak", PlayerPrefs.GetInt("MaxStreak", 0).ToString() },
            { "Total Plays", PlayerPrefs.GetInt("TotalPlays", 0).ToString() },
            { "Total Burgers", PlayerPrefs.GetInt("TotalBurgersDelivered", 0).ToString() },
            { "Discarded Burgers", PlayerPrefs.GetInt("TotalBurgersDiscarded", 0).ToString() },
            { "Failed Orders", PlayerPrefs.GetInt("TotalOrdersFailed", 0).ToString() },
            { "Ingredients Placed", PlayerPrefs.GetInt("TotalIngredientsPlaced", 0).ToString() },
            { "Clutch Deliveries", PlayerPrefs.GetInt("ClutchDeliveries", 0).ToString() },
            { "Time Played (s)", PlayerPrefs.GetInt("TotalSecondsPlayed", 0).ToString() },
            { "Longest Session (s)", PlayerPrefs.GetInt("LongestSessionDuration", 0).ToString() },
            { "Easy Deliveries", PlayerPrefs.GetInt("EasyDeliveries", 0).ToString() },
            { "Medium Deliveries", PlayerPrefs.GetInt("MediumDeliveries", 0).ToString() },
            { "Hard Deliveries", PlayerPrefs.GetInt("HardDeliveries", 0).ToString() },
            { "Easy Fails", PlayerPrefs.GetInt("EasyFails", 0).ToString() },
            { "Medium Fails", PlayerPrefs.GetInt("MediumFails", 0).ToString() },
            { "Hard Fails", PlayerPrefs.GetInt("HardFails", 0).ToString() },
        };

        // Instanciar StatItem por cada par
        foreach (var stat in stats)
        {
            var item = Instantiate(statItemPrefab, statsContentParent);
            item.SetData(stat.Key, stat.Value);
        }
    }
}
