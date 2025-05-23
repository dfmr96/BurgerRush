﻿using UnityEngine;

namespace ScriptableObjects.BurgerComplexity
{
    [CreateAssetMenu(fileName = "BurgerComplexity", menuName = "BurgerRush/Burger Complexity")]
    public class BurgerComplexityData : ScriptableObject
    {
        
        public enum DifficultyType { Easy, Medium, Hard }
        [SerializeField] private DifficultyType difficulty;
        public DifficultyType Difficulty => difficulty;
        
        [Tooltip("Tiempo total (en segundos) antes de que esta orden expire.")]
        [SerializeField] private float expirationTime;
        public float ExpirationTime => expirationTime;
        
        [Header("Clutch Settings")]
        [Tooltip("Tiempo bonus si se entrega en los últimos 3 segundos")]
        
        [SerializeField] private float clutchBonusTime = 0f;
        [Tooltip("Visual identifier only")] 
        [SerializeField] private string label;
        
        [Header("Ingredient Settings")]
        [Tooltip("Min toppings (exclusive of bun and protein)")]
        [SerializeField] private int minToppings;
        [Tooltip("Max toppings (exclusive of bun and protein)")]
        [SerializeField] private int maxToppings;
        
        [Header("Scoring")]
        [SerializeField] private int baseScore;
        [SerializeField] private int pointsPerTopping;
        [Header("Score Pop-up Style")]
        [SerializeField] private ScorePopUpStyle scorePopupStyle;
        
        [Header("Bonus Time Settings")]
        [Tooltip("Time (in seconds) added when this order is successfully delivered. Set to 0 for no bonus.")]
        [SerializeField] private float bonusTimeOnDelivery = 0f;
        public float BonusTimeOnDelivery => bonusTimeOnDelivery;
        
        [Tooltip("Chance (0-1) for this order to be flagged as bonus time order.")]
        [SerializeField] private float chanceToBeBonus = 0f;
        
        [Range(0.5f, 2f)]
        [SerializeField] private float orderCreatedPitch = 1f;

        [SerializeField] private Color orderBackgroundColor;
        public float ChanceToBeBonus => chanceToBeBonus;
        
        public ScorePopUpStyle ScorePopupStyle => scorePopupStyle;
        public string Label => label;
        public int MaxToppings => maxToppings;
        public int MinToppings => minToppings;

        public int BaseScore => baseScore;

        public int PointsPerTopping => pointsPerTopping;

        public float OrderCreatedPitch => orderCreatedPitch;

        public Color OrderBackgroundColor => orderBackgroundColor;

        public float ClutchBonusTime => clutchBonusTime;
    }
}