using System.Collections;
using System.Collections.Generic;
using ScriptableObjects.BurgerComplexityData;
using UnityEngine;

[System.Serializable]
public class ComplexityProgressionStep
{
    public int minOrdersDelivered;
    public int maxOrdersDelivered;

    [Range(0, 100)] public int easyWeight = 100;
    [Range(0, 100)] public int mediumWeight;
    [Range(0, 100)] public int hardWeight;

    public bool IsInRange(int ordersDelivered)
    {
        return ordersDelivered >= minOrdersDelivered && ordersDelivered <= maxOrdersDelivered;
    }

    public BurgerComplexityData GetRandomComplexity(BurgerComplexityData easy, BurgerComplexityData medium, BurgerComplexityData hard)
    {
        int total = easyWeight + mediumWeight + hardWeight;
        int roll = Random.Range(0, total);

        if (roll < easyWeight) return easy;
        if (roll < easyWeight + mediumWeight) return medium;
        return hard;
    }
}
