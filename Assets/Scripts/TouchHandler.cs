using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class TouchHandler : MonoBehaviour
{
    [SerializeField] private Camera cam;
    

    private void Awake()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    private void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    private void Update()
    {
        if ((Touchscreen.current != null) && Touchscreen.current.primaryTouch.press.isPressed)
        {
            //Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
            Touch activeTouch = Touch.activeTouches[0];

            Vector2 touchPos = activeTouch.screenPosition;
            Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, cam.nearClipPlane));
            Debug.Log("Touch Position:" + worldPos);

            transform.position = worldPos;
        }

        foreach (Touch touch in Touch.activeTouches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                
            }
            else
            {
                
            }
        }
    }
}
