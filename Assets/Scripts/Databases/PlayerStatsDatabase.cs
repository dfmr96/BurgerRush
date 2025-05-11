using AYellowpaper.SerializedCollections;
using ScriptableObjects.PlayerData;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(fileName = "PlayerStatsDatabase", menuName = "Stats/Player Stats Database")]
    public class PlayerStatsDatabase : ScriptableObject
    {
        [System.Serializable]
        public class StatDict : SerializedDictionary<string, PlayerStatDefinition> { }

        public StatDict stats;
    }
}