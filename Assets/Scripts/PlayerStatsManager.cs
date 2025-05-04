using UnityEngine;

namespace DefaultNamespace
{
    public class PlayerStatsManager
    {
        private const string Key_TotalPlays = "TotalPlays";
        private const string Key_TotalBurgers = "TotalBurgersDelivered";
        private const string Key_TotalOrdersFailed = "TotalOrdersFailed";
        private const string Key_TotalDiscarded = "TotalBurgersDiscarded";
        private const string Key_TotalIngredients = "TotalIngredientsPlaced";
        private const string Key_TotalSecondsPlayed = "TotalSecondsPlayed";
        private const string Key_OrdersOnTime = "OrdersOnTime";
        private const string Key_ClutchDeliveries = "ClutchDeliveries";
        private const string Key_PerfectStreak = "PerfectBurgerStreak";
        private const string Key_HighScore = "HighScore";
        private const string Key_MaxStreak = "MaxStreak";
        private const string Key_PlayerName = "PlayerName";
        private const string Key_DifficultyUnlocked = "DifficultyUnlocked";
        private const string Key_EasyDeliveries = "EasyDeliveries";
        private const string Key_MediumDeliveries = "MediumDeliveries";
        private const string Key_HardDeliveries = "HardDeliveries";


        // --- Métodos de modificación ---
        public static void AddPlay() => IncrementInt(Key_TotalPlays);
        public static void AddBurger() => IncrementInt(Key_TotalBurgers);
        public static void AddOrderFail() => IncrementInt(Key_TotalOrdersFailed);
        public static void AddDiscarded() => IncrementInt(Key_TotalDiscarded);
        public static void AddIngredient() => IncrementInt(Key_TotalIngredients);
        public static void AddSecondPlayed() => IncrementInt(Key_TotalSecondsPlayed);
        public static void AddOrderOnTime() => IncrementInt(Key_OrdersOnTime);
        public static void AddClutchDelivery() => IncrementInt(Key_ClutchDeliveries);
        public static void AddEasyDelivery() => IncrementInt(Key_EasyDeliveries);
        public static void AddMediumDelivery() => IncrementInt(Key_MediumDeliveries);
        public static void AddHardDelivery() => IncrementInt(Key_HardDeliveries);
        
        public static int GetEasyDeliveries() => PlayerPrefs.GetInt(Key_EasyDeliveries, 0);
        public static int GetMediumDeliveries() => PlayerPrefs.GetInt(Key_MediumDeliveries, 0);
        public static int GetHardDeliveries() => PlayerPrefs.GetInt(Key_HardDeliveries, 0);


        public static void UpdatePerfectStreak(int value)
        {
            if (value > PlayerPrefs.GetInt(Key_PerfectStreak, 0))
                PlayerPrefs.SetInt(Key_PerfectStreak, value);
        }

        public static void UpdateHighScore(int score)
        {
            if (score > PlayerPrefs.GetInt(Key_HighScore, 0))
                PlayerPrefs.SetInt(Key_HighScore, score);
        }

        public static void UpdateMaxStreak(int streak)
        {
            if (streak > PlayerPrefs.GetInt(Key_MaxStreak, 0))
                PlayerPrefs.SetInt(Key_MaxStreak, streak);
        }

        public static void SetPlayerName(string name) => PlayerPrefs.SetString(Key_PlayerName, name);
        public static string GetPlayerName() => PlayerPrefs.GetString(Key_PlayerName, "Player");

        public static void SetDifficultyUnlocked(int level)
        {
            int current = PlayerPrefs.GetInt(Key_DifficultyUnlocked, 0);
            if (level > current)
                PlayerPrefs.SetInt(Key_DifficultyUnlocked, level);
        }

        public static int GetDifficultyUnlocked() => PlayerPrefs.GetInt(Key_DifficultyUnlocked, 0);

        // --- Reseteo completo ---
        public static void ResetAllStats()
        {
            PlayerPrefs.DeleteKey(Key_TotalPlays);
            PlayerPrefs.DeleteKey(Key_TotalBurgers);
            PlayerPrefs.DeleteKey(Key_TotalOrdersFailed);
            PlayerPrefs.DeleteKey(Key_TotalDiscarded);
            PlayerPrefs.DeleteKey(Key_TotalIngredients);
            PlayerPrefs.DeleteKey(Key_TotalSecondsPlayed);
            PlayerPrefs.DeleteKey(Key_OrdersOnTime);
            PlayerPrefs.DeleteKey(Key_ClutchDeliveries);
            PlayerPrefs.DeleteKey(Key_PerfectStreak);
            PlayerPrefs.DeleteKey(Key_HighScore);
            PlayerPrefs.DeleteKey(Key_MaxStreak);
            PlayerPrefs.DeleteKey(Key_PlayerName);
            PlayerPrefs.DeleteKey(Key_DifficultyUnlocked);
        }

        // --- Métodos utilitarios ---
        private static void IncrementInt(string key)
        {
            int current = PlayerPrefs.GetInt(key, 0);
            PlayerPrefs.SetInt(key, current + 1);
        }

        public static void Save() => PlayerPrefs.Save();
    }
}