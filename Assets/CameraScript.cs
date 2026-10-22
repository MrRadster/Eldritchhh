using UnityEngine;
using UnityEngine.InputSystem;

public class EightDirectionLook : MonoBehaviour
{
    [Header("Horizontal Look")]
    public float rotationSpeed = 12f;
    private readonly float[] directions = { 0f, 45f, 90f, 135f, 180f, 225f, 270f, 315f };
    private int currentIndex = 0;
    private float targetY;

    [Header("Look Down")]
    public float lookDownAngle = 55f;          // how far the camera tilts down
    public float lookDownSpeed = 8f;
    public Key lookDownKey = Key.E;            // change to Key.Space if you prefer

    private bool isLookingDown = false;
    private float currentXRotation = 0f;

    void Start()
    {
        targetY = directions[currentIndex];
        transform.rotation = Quaternion.Euler(0f, targetY, 0f);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleHorizontalTurn();
        HandleLookDown();
        ApplyRotation();
    }

    void HandleHorizontalTurn()
    {
        if (isLookingDown) return; // can't turn while looking down

        if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            currentIndex = (currentIndex - 1 + 8) % 8;
            targetY = directions[currentIndex];
        }
        if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            currentIndex = (currentIndex + 1) % 8;
            targetY = directions[currentIndex];
        }
    }

    void HandleLookDown()
    {
        if (Keyboard.current[lookDownKey].wasPressedThisFrame)
        {
            isLookingDown = !isLookingDown;
        }

        float targetX = isLookingDown ? lookDownAngle : 0f;
        currentXRotation = Mathf.Lerp(currentXRotation, targetX, Time.deltaTime * lookDownSpeed);
    }

    void ApplyRotation()
    {
        Quaternion targetRot = Quaternion.Euler(currentXRotation, targetY, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
    }

    // Public so other scripts can check the state
    public bool IsLookingDown => isLookingDown;
}