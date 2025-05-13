using UnityEngine;

public class Camera_script : MonoBehaviour
{
    public LockOnSystem lockOnSystem;
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2f, -4f);

    public float sensitivity = 3f;
    public float rotationSmoothTime = 0.12f;
    public float minPitch = -30f;
    public float maxPitch = 60f;

    private float yaw;
    private float pitch;

    private Vector3 currentRotation;
    private Vector3 rotationSmoothVelocity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        // 1) Read look input
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float rightX = Input.GetAxis("RightStickHorizontal");
        float rightY = Input.GetAxis("RightStickVertical");
        bool usingStick = Mathf.Abs(rightX) > 0.2f || Mathf.Abs(rightY) > 0.2f;
        float inputX = usingStick ? rightX : mouseX;
        float inputY = usingStick ? rightY : mouseY;

        // 2) Accumulate yaw/pitch
        yaw   += inputX * sensitivity;
        pitch -= inputY * sensitivity;
        pitch  = Mathf.Clamp(pitch, minPitch, maxPitch);

        // 3) Override yaw if locked-on
        if (lockOnSystem != null && lockOnSystem.currentTarget != null)
        {
            Vector3 toEnemy = lockOnSystem.currentTarget.position - target.position;
            toEnemy.y = 0;
            if (toEnemy.sqrMagnitude > 0.001f)
                yaw = Mathf.Atan2(toEnemy.x, toEnemy.z) * Mathf.Rad2Deg;
        }

        // 4) Smooth each angle separately (handles wrap at ±180°)
        float smoothYaw = Mathf.SmoothDampAngle(
            currentRotation.y,    // from
            yaw,                  // to
            ref rotationSmoothVelocity.y,
            rotationSmoothTime
        );
        float smoothPitch = Mathf.SmoothDampAngle(
            currentRotation.x,
            pitch,
            ref rotationSmoothVelocity.x,
            rotationSmoothTime
        );
        currentRotation = new Vector3(smoothPitch, smoothYaw, 0f);

        // 5) Apply
        transform.eulerAngles = currentRotation;
        transform.position    = target.position + transform.rotation * offset;
    }
    
}
