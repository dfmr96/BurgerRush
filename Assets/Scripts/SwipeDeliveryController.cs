using UnityEngine;
using UnityEngine.InputSystem;

public class SwipeDeliveryController : MonoBehaviour
{
    [SerializeField] private float swipeThreshold = 50f;
    [SerializeField] private OrderManager orderManager;

    private Vector2 swipeStartPos;
    private bool isSwiping;

    private void Update()
    {
        // Solo continúa si hay touchscreen
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame)
            {
                swipeStartPos = touch.position.ReadValue();
                isSwiping = true;
            }

            if (touch.press.wasReleasedThisFrame && isSwiping)
            {
                Vector2 swipeEndPos = touch.position.ReadValue();
                Vector2 swipeDelta = swipeEndPos - swipeStartPos;

                Debug.Log($"Swipe delta: {swipeDelta}");

                // Verificamos si es un swipe mayor a threshold y mayor en Y que en X
                if (swipeDelta.magnitude > swipeThreshold && swipeDelta.y > Mathf.Abs(swipeDelta.x))
                {
                    TryAutoDeliverMostUrgentOrder();
                }

                isSwiping = false;
            }
        }

/*#if UNITY_EDITOR
        // Simulación en editor con mouse
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            swipeStartPos = Mouse.current.position.ReadValue();
            isSwiping = true;
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && isSwiping)
        {
            Vector2 swipeDelta = Mouse.current.position.ReadValue() - swipeStartPos;

            if (swipeDelta.y > swipeThreshold && Mathf.Abs(swipeDelta.x) < swipeThreshold * 0.5f)
            {
                TryAutoDeliverMostUrgentOrder();
            }

            isSwiping = false;
        }
#endif*/
    }

    public void TryAutoDeliverMostUrgentOrder()
    {
        Order target = GetMostUrgentDeliverableOrder();
        if (target != null)
        {
            Debug.Log("📦 Auto-delivering most urgent order!");
            target.SendMessage("AttemptDelivery");
            VibrationManager.Vibrate(VibrationPreset.Medium);
        }
        else
        {
            Debug.Log("⚠ No deliverable order found.");
        }
    }

    private Order GetMostUrgentDeliverableOrder()
    {
        Order[] orders = orderManager.Orders;
        Order selected = null;
        float minTime = float.MaxValue;

        foreach (var order in orders)
        {
            if (!order.gameObject.activeSelf) continue;

            var type = typeof(Order);
            var deliverableField = type.GetField("_isDeliverable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            bool isDeliverable = (bool)deliverableField.GetValue(order);

            if (!isDeliverable) continue;

            float timeLeft = (float)type.GetField("timer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(order);

            if (timeLeft < minTime)
            {
                selected = order;
                minTime = timeLeft;
            }
        }

        return selected;
    }
}