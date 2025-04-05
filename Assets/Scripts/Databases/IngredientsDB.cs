using AYellowpaper.SerializedCollections;
using ScriptableObjects;
using UnityEngine;

namespace Databases
{
    [CreateAssetMenu(fileName = "IngredientsDB", menuName = "ScriptableObjects/IngredientsDB", order = 1)]
    public class IngredientsDB : ScriptableObject
    {
        public IngredientData[] ingredients;
    }
}
