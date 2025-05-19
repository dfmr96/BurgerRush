using System;
using Unity.Services.Analytics;
using UnityEngine;

namespace Services
{
    public static class AnalyticsManager
    {
        private static float Round1Decimal(float value) =>
            (float)Math.Round(value, 1, MidpointRounding.AwayFromZero);

        public static void TrackBurgerDelivered(string difficulty, int ingredientsCount, float timeSinceStart, bool isBonus)
        {
            var e = new CustomEvent("order_delivered")
            {
                { "difficulty", difficulty },
                { "ingredients_count", ingredientsCount },
                { "time_since_start", Round1Decimal(timeSinceStart) },
                { "is_bonus_order", isBonus }
            };

            RecordEventSafe(e, "order_delivered");
            Debug.Log($"📊 Recorded custom event: order_delivered ({difficulty}, {ingredientsCount} ingredients, {Round1Decimal(timeSinceStart)}s, bonus: {isBonus})");
        }

        public static void TrackBurgerBuilt(float timeToComplete, int ingredientsCount, string difficulty)
        {
            var e = new CustomEvent("burger_completed")
            {
                { "time_to_complete", Round1Decimal(timeToComplete) },
                { "ingredients_count", ingredientsCount },
                { "difficulty", difficulty }
            };

            RecordEventSafe(e, "burger_completed");
            Debug.Log($"📊 Recorded custom event: burger_completed ({Round1Decimal(timeToComplete)}s, {ingredientsCount} ingredients, {difficulty})");
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
            var e = new CustomEvent("game_session_end")
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

            RecordEventSafe(e, "game_session_end");
            Debug.Log($"📊 Recorded custom event: game_session_end ({secondsPlayed}s, {score} points, {ordersDelivered} orders delivered)");
        }

        private static void RecordEventSafe(CustomEvent e, string name)
        {
            try
            {
                AnalyticsService.Instance.RecordEvent(e);
                Debug.Log($"📊 Sent event: {name}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"⚠️ Analytics error on {name}: {ex.Message}");
            }
        }
    }
}
