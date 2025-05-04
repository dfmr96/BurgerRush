using UnityEngine;

public static class PlayerStatsManager
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
    private const string Key_EasyFails = "EasyFails";
    private const string Key_MediumFails = "MediumFails";
    private const string Key_HardFails = "HardFails";
    private const string Key_LongestSession = "LongestSessionDuration";


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
    public static void AddEasyFail() => IncrementInt(Key_EasyFails);
    public static void AddMediumFail() => IncrementInt(Key_MediumFails);
    public static void AddHardFail() => IncrementInt(Key_HardFails);

    public static int GetEasyFails() => PlayerPrefs.GetInt(Key_EasyFails, 0);
    public static int GetMediumFails() => PlayerPrefs.GetInt(Key_MediumFails, 0);
    public static int GetHardFails() => PlayerPrefs.GetInt(Key_HardFails, 0);
        
    public static int GetEasyDeliveries() => PlayerPrefs.GetInt(Key_EasyDeliveries, 0);
    public static int GetMediumDeliveries() => PlayerPrefs.GetInt(Key_MediumDeliveries, 0);
    public static int GetHardDeliveries() => PlayerPrefs.GetInt(Key_HardDeliveries, 0);
    public static int GetLongestSession() => PlayerPrefs.GetInt(Key_LongestSession, 0);
    


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
    
    public static void UpdateLongestSession(int seconds)
    {
        int previous = PlayerPrefs.GetInt(Key_LongestSession, 0);
        if (seconds > previous)
            PlayerPrefs.SetInt(Key_LongestSession, seconds);
    }

    public static void SetPlayerName(string name) => PlayerPrefs.SetString(Key_PlayerName, name);
    public static string GetPlayerName() => PlayerPrefs.GetString(Key_PlayerName, "Player");

    public static void SetDifficultyUnlocked(int level)
    {
        int current = PlayerPrefs.GetInt(Key_DifficultyUnlocked, 0);
        if (level > current)
            PlayerPrefs.SetInt(Key_DifficultyUnlocked, level);
    }
    
    public static void AddSecondsPlayed(int seconds)
    {
        int current = PlayerPrefs.GetInt(Key_TotalSecondsPlayed, 0);
        PlayerPrefs.SetInt(Key_TotalSecondsPlayed, current + seconds);
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
        PlayerPrefs.DeleteKey(Key_EasyFails);
        PlayerPrefs.DeleteKey(Key_MediumFails);
        PlayerPrefs.DeleteKey(Key_HardFails);
        PlayerPrefs.DeleteKey(Key_LongestSession);

    }

    // --- Métodos utilitarios ---
    private static void IncrementInt(string key)
    {
        int current = PlayerPrefs.GetInt(key, 0);
        PlayerPrefs.SetInt(key, current + 1);
    }

    public static void Save() => PlayerPrefs.Save();
}