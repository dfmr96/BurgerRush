using UnityEngine;

namespace Services.Utils
{
    public class WorldPositionHelper
    {
        public static Vector3 GetWorldPositionFromUI(RectTransform uiElement)
        {
            if (Camera.main == null) return Vector3.zero;

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, uiElement.position);
            float zDepth = 0;
            return Camera.main.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, zDepth));
        }
    }
}