using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;

namespace Services
{
    public static class AnalyticsManager
    {
        public static void TrackBurgerDelivered(string difficulty, int ingredientsCount, float timeSinceStart, bool isBonus)
        {
            var burgerEvent = new CustomEvent("burger_delivered")
            {
                { "difficulty", difficulty },
                { "ingredients_count", ingredientsCount },
                { "time_since_start", Mathf.Round(timeSinceStart * 10f) / 10f },
                { "is_bonus_order", isBonus }
            };

            AnalyticsService.Instance.RecordEvent(burgerEvent);
            Debug.Log("📊 Recorded custom event: burger_delivered");
        }
        
        public static void TrackBurgerBuilt(float timeToComplete, int ingredientsCount, string complexity)
        {
            var builtEvent = new CustomEvent("burger_built")
            {
                { "time_to_complete", Mathf.Round(timeToComplete * 10f) / 10f },
                { "ingredients_count", ingredientsCount },
                { "complexity", complexity }
            };

            AnalyticsService.Instance.RecordEvent(builtEvent);
            Debug.Log($"📊 Recorded custom event: burger_built ({Mathf.RoundToInt(timeToComplete)}s, {ingredientsCount} ingredients, {complexity})");
        }
        
        public static void TrackGameSessionEnd(
            int secondsPlayed,
            int score,
            int ordersDelivered,
            int ordersFailed,
            bool usedContinue,
            int totalPlays,
            int highScore,
            int totalTimePlayed,
            int easyDelivered,
            int mediumDelivered,
            int hardDelivered)
        {
            var sessionEvent = new CustomEvent("game_session_end")
            {
                { "session_duration", secondsPlayed },
                { "final_score", score },
                { "orders_delivered", ordersDelivered },
                { "orders_failed", ordersFailed },
                { "used_continue", usedContinue },
                { "total_plays", totalPlays },
                { "high_score", highScore },
                { "total_time_played", totalTimePlayed },
                { "easy_delivered", easyDelivered },
                { "medium_delivered", mediumDelivered },
                { "hard_delivered", hardDelivered },
                { "total_delivered", easyDelivered + mediumDelivered + hardDelivered }
            };

            AnalyticsService.Instance.RecordEvent(sessionEvent);
            Debug.Log("📊 Recorded custom event: game_session_end");
        }

    }
}