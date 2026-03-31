// 8/29/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Joystick Settings")]
    [SerializeField] private RectTransform joystickBackground; // Background for the joystick
    [SerializeField] private RectTransform joystickHandle;     // Handle for the joystick
    [SerializeField] private float handleLimit = 1f;           // Limit for handle movement

    private Vector2 inputVector = Vector2.zero;                // Input vector for movement

    public static JoystickController Instance;                 // Singleton for easy access

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Called when the joystick is dragged
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground,
            eventData.position,
            eventData.pressEventCamera,
            out position
        );

        position /= joystickBackground.sizeDelta;

        // Normalize input to a range of -1 to 1
        inputVector = new Vector2(
            position.x * 2,
            position.y * 2
        );
        inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

        // Move joystick handle
        joystickHandle.anchoredPosition = new Vector2(
            inputVector.x * (joystickBackground.sizeDelta.x / 2) * handleLimit,
            inputVector.y * (joystickBackground.sizeDelta.y / 2) * handleLimit
        );

        UpdatePlayerMovement();
    }

    // Called when the joystick is pressed
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    // Called when the joystick is released
    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        joystickHandle.anchoredPosition = Vector2.zero;

        UpdatePlayerMovement();
    }

    // Get the current input vector
    public Vector2 GetInputVector()
    {
        return inputVector;
    }

    // Update player movement when joystick input changes
    private void UpdatePlayerMovement()
    {
        if (PlayerMovement.Instance != null)
        {
            PlayerMovement.Instance.movementInput = inputVector;
        }
    }
}
