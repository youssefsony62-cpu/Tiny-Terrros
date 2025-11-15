using UnityEngine;

public class CornerLookCamera : MonoBehaviour
{
    public Transform cameraTransform; // Assign your Camera here
    public float lookSpeed = 2f;       // How fast to rotate
    public float maxHorizontal = 30f;  // Left-right limit in degrees
    public float maxVertical = 15f;    // Up-down limit in degrees
    public float returnSpeed = 3f;     // Speed to return to center

    private Vector2 currentRotation;
    private Vector2 targetRotation;

    private void Start()
    {
        currentRotation = Vector2.zero;
        targetRotation = Vector2.zero;
    }

    private void Update()
    {
        if (Input.GetMouseButton(1)) // Holding right click
        {
            float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

            targetRotation.x += mouseX;
            targetRotation.y -= mouseY;

            targetRotation.x = Mathf.Clamp(targetRotation.x, -maxHorizontal, maxHorizontal);
            targetRotation.y = Mathf.Clamp(targetRotation.y, -maxVertical, maxVertical);
        }
        else
        {
            // Return to center smoothly
            targetRotation = Vector2.Lerp(targetRotation, Vector2.zero, Time.deltaTime * returnSpeed);
        }

        currentRotation = Vector2.Lerp(currentRotation, targetRotation, Time.deltaTime * lookSpeed);

        cameraTransform.localRotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
    }
}
