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
                { "time_since_start", timeSinceStart },
                { "is_bonus_order", isBonus }
            };

            AnalyticsService.Instance.RecordEvent(burgerEvent);
            Debug.Log("📊 Recorded custom event: burger_delivered");
        }
    }
}