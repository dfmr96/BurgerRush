using UnityEngine;

namespace ScriptableObjects.PlayerData
{
    [CreateAssetMenu(fileName = "New PlayerStat", menuName = "Stats/Player Stat", order = 0)]
    public class PlayerStatDefinition : ScriptableObject
    {
        public string statKey;
        public string displayName;
        public StatType statType;
        public string defaultValue;
    }

    public enum StatType
    {
        Int,
        Float,
        String,
        Bool
    }
}