using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 12f;

    [Header("Footstep Settings")]
    [SerializeField] private float runAnimLength = 0.6f;
    [SerializeField] private int stepsPerLoop = 2;
    [SerializeField] private float slowFactor = 1.05f;

    private float footstepRate;
    private float footstepTimer;

    private Rigidbody2D rb;
    private PlayerInput inputActions;

    public Vector2 movementInput;
    public Vector2 currentVelocity;

    public static PlayerMovement Instance;

    private void Awake()
    {
        Instance = this;

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        footstepRate = (runAnimLength / stepsPerLoop) * slowFactor;

        inputActions = new PlayerInput();
        inputActions.Enable();

        // Subscribe to input events
        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        // Read movement input from joystick or keyboard
        movementInput = context.ReadValue<Vector2>().normalized;
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        // Stop movement when input is canceled
        movementInput = Vector2.zero;
    }

    private void FixedUpdate()
    {
        // Smoothly interpolate velocity for smooth movement
        Vector2 targetVelocity = movementInput * moveSpeed;
        currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);

        rb.linearVelocity = currentVelocity;

        // Handle footstep sounds
        if (movementInput.magnitude > 0.1f && currentVelocity.magnitude > 0.1f)
        {
            footstepTimer -= Time.fixedDeltaTime;
            if (footstepTimer <= 0f)
            {
                PlayFootstep();
                footstepTimer = footstepRate;
            }
        }
        else
        {
            footstepTimer = 0f;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from input events and disable input actions
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;
        inputActions.Disable();
    }

    private void PlayFootstep()
    {
        // Play footstep sound if SoundManager is available
        if (SoundManager.Instance != null && SoundManager.Instance.playerWalkSound.Length > 0)
        {
            int index = Random.Range(0, SoundManager.Instance.playerWalkSound.Length);
            SoundManager.Instance.PlaySound(SoundManager.Instance.playerWalkSound[index]);
        }
    }
}