using Databases;
using Services;
using UnityEngine;

public static class PlayerStatsManager
{
    private static PlayerStatsDatabase db => Resources.Load<PlayerStatsDatabase>("PlayerStatsDatabase");
    
    public static int GetHighScore() => (int)PlayerStatsService.Get(db.stats["HighScore"]);
    public static int GetTotalSecondsPlayed() => (int)PlayerStatsService.Get(db.stats["TotalSecondsPlayed"]);

    public static void AddPlay()
    {
        PlayerStatsService.Increment(db.stats["TotalPlays"]);
        //EVENTO A ANALYTICS
    }
        
    public static void AddBurger() => PlayerStatsService.Increment(db.stats["TotalBurgersDelivered"]);
    public static void AddOrderFail() => PlayerStatsService.Increment(db.stats["TotalOrdersFailed"]);
    public static void AddDiscarded() => PlayerStatsService.Increment(db.stats["TotalBurgersDiscarded"]);
    public static void AddIngredient() => PlayerStatsService.Increment(db.stats["TotalIngredientsPlaced"]);
    public static void AddClutchDelivery() => PlayerStatsService.Increment(db.stats["ClutchDeliveries"]);
    public static void AddEasyDelivery() => PlayerStatsService.Increment(db.stats["EasyDeliveries"]);
    public static void AddMediumDelivery() => PlayerStatsService.Increment(db.stats["MediumDeliveries"]);
    public static void AddHardDelivery() => PlayerStatsService.Increment(db.stats["HardDeliveries"]);
    public static void AddEasyFail() => PlayerStatsService.Increment(db.stats["EasyFails"]);
    public static void AddMediumFail() => PlayerStatsService.Increment(db.stats["MediumFails"]);
    public static void AddHardFail() => PlayerStatsService.Increment(db.stats["HardFails"]);

    public static void UpdateHighScore(int score)
    {
        int current = (int)PlayerStatsService.Get(db.stats["HighScore"]);
        if (score > current)
            PlayerStatsService.Set(db.stats["HighScore"], score);
    }

    public static void UpdateMaxStreak(int streak)
    {
        int current = (int)PlayerStatsService.Get(db.stats["MaxStreak"]);
        if (streak > current)
            PlayerStatsService.Set(db.stats["MaxStreak"], streak);
    }

    public static void UpdateLongestSession(int seconds)
    {
        int current = (int)PlayerStatsService.Get(db.stats["LongestSessionDuration"]);
        if (seconds > current)
            PlayerStatsService.Set(db.stats["LongestSessionDuration"], seconds);
    }

    public static void AddSecondsPlayed(int seconds)
    {
        int current = (int)PlayerStatsService.Get(db.stats["TotalSecondsPlayed"]);
        PlayerStatsService.Set(db.stats["TotalSecondsPlayed"], current + seconds);
    }
}