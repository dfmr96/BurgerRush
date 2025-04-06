using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

public class StackedIngredientView : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    public void SetData(IngredientData data)
    {
        if (data != null && iconImage != null)
        {
            iconImage.sprite = data.IngredientIcon;
        }
    }
}