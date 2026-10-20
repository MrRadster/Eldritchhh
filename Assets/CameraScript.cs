using UnityEngine;
using UnityEngine.InputSystem;

public class EightDirectionLook : MonoBehaviour
{
    [Header("Settings")]
    public float rotationSpeed = 12f;   // Higher = faster snap

    private readonly float[] directions = { 0f, 45f, 90f, 135f, 180f, 225f, 270f, 315f };
    private int currentIndex = 0;
    private float targetY;

    void Start()
    {
        targetY = directions[currentIndex];
        transform.rotation = Quaternion.Euler(0f, targetY, 0f);

        // Optional: lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Turn left (A or Left Arrow)
        if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            currentIndex = (currentIndex - 1 + 8) % 8;
            targetY = directions[currentIndex];
        }

        // Turn right (D or Right Arrow)
        if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            currentIndex = (currentIndex + 1) % 8;
            targetY = directions[currentIndex];
        }

        // Smoothly rotate the camera
        Quaternion targetRot = Quaternion.Euler(0f, targetY, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
    }
}