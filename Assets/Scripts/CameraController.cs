using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    public float normalSpeed = 10f;
    public float fastSpeed = 50f;

    [Header("Look")]
    public float mouseSensitivity = 2f;

    private float pitch = 0f;
    private float yaw = 0f;

    private void Start()
    {
        Vector3 euler = transform.eulerAngles;
        pitch = euler.x;
        yaw = euler.y;
    }

    private void Update()
    {
        // 1. Mouse Look
        if (Input.GetMouseButton(1)) // Hold right-click to look around (optional, or remove condition to always look)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

            pitch = Mathf.Clamp(pitch, -89f, 89f);

            transform.eulerAngles = new Vector3(pitch, yaw, 0f);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // 2. Movement
        float currentSpeed = Input.GetKey(KeyCode.LeftControl) ? fastSpeed : normalSpeed;

        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) moveDirection += transform.forward;
        if (Input.GetKey(KeyCode.S)) moveDirection -= transform.forward;
        if (Input.GetKey(KeyCode.A)) moveDirection -= transform.right;
        if (Input.GetKey(KeyCode.D)) moveDirection += transform.right;

        // Up / Down
        if (Input.GetKey(KeyCode.Space)) moveDirection += Vector3.up;
        if (Input.GetKey(KeyCode.LeftShift)) moveDirection -= Vector3.up;

        transform.position += moveDirection * currentSpeed * Time.deltaTime;
    }
}
