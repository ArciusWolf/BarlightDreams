// 8/29/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("Joystick Settings")]
    public RectTransform background;
    public RectTransform handle;
    public float handleLimit = 1f;

    private Vector2 inputVector;

    public Vector2 InputVector => inputVector;

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            eventData.pressEventCamera,
            out position
        );

        position.x = (position.x / background.sizeDelta.x) * 2;
        position.y = (position.y / background.sizeDelta.y) * 2;

        inputVector = position.magnitude > 1.0f ? position.normalized : position;

        handle.anchoredPosition = new Vector2(
            inputVector.x * (background.sizeDelta.x / 2) * handleLimit,
            inputVector.y * (background.sizeDelta.y / 2) * handleLimit
        );
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }
}
