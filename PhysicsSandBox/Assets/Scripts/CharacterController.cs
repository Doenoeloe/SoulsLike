using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class PlayerLocomotion : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private LockOnSystem lockOnSystem;
    [SerializeField] private Slider _slider;
    [SerializeField] private Transform cameraTransform;
    public Rigidbody rb;
    // public Animator animator;

    [Header("Stats")] public float walkSpeed = 3f;
    public float runSpeed = 10f;
    public float rotationSpeed = 10f;
    public float rollDistance = 3f;
    public float rollCooldown = 1f;
    public float maxStamina = 100f;
    public float rollStaminaCost = 30f;

    [Header("Stamina Regen")] public float staminaRegenRate = 0.1f; // stamina points per second
    public float staminaRegenDelay = 100000.0f; // seconds after last use
    private float lastStaminaUseTime;

    float currentStamina;
    bool canRoll = true;

    void Start()
    {
        currentStamina = maxStamina;
    }

    void Update()
    {
        Vector3 inputDir = GetCameraRelativeInput();
        // 1) Handle Roll Input
        if (Input.GetButtonDown("Roll") && canRoll && currentStamina >= rollStaminaCost)
        {
            StartCoroutine(DoRoll(inputDir));
        }
        
        // 2) Movement & Animator
        float inputMag = inputDir.magnitude;
        bool isRunning = Input.GetButton("Run");

        bool isMoving = inputDir.sqrMagnitude > 0.01f;

        // If locked, always face the target instead of input direction
        if (lockOnSystem != null && lockOnSystem.currentTarget != null)
        {
            Vector3 lookDir = lockOnSystem.currentTarget.position - transform.position;
            lookDir.y = 0;
            if (lookDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir);
                rb.rotation = Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }
        else if (isMoving)
        {
            // existing movement‐based rotation
            Quaternion targetRot = Quaternion.LookRotation(inputDir);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        float targetSpeed = isRunning ? runSpeed : walkSpeed;
        Vector3 velocity = inputDir * targetSpeed * inputMag;
        Vector3 flatVel = new Vector3(velocity.x, 0, velocity.z);

        // Smooth rotation only if moving
        if (flatVel.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(flatVel);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // Animator parameters
        // animator.SetFloat("Speed", flatVel.magnitude);
        // animator.SetBool("IsRunning", isRunning);

        // 3) Apply movement (in FixedUpdate for physics)
        targetVelocity = flatVel;
    }

    Vector3 targetVelocity;

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + targetVelocity * Time.fixedDeltaTime);
        HandleStaminaRegen();
    }

    IEnumerator DoRoll(Vector3 rollDir)
    {
        canRoll = false;
        currentStamina -= rollStaminaCost;
        lastStaminaUseTime = Time.time;
        _slider.value = currentStamina;
        // animator.SetTrigger("Roll");

        // ignore any tiny input
        if (rollDir.sqrMagnitude < 0.01f)
            rollDir = transform.forward;
        rollDir.y = 0;
        rollDir.Normalize();

        float rollDuration = 0.5f; // match your animation length
        float speed = rollDistance / rollDuration;
        float startTime = Time.time;

        while (Time.time < startTime + rollDuration)
        {
            rb.MovePosition(rb.position + rollDir * speed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(rollCooldown);
        canRoll = true;
    }

    Vector3 GetCameraRelativeInput()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 forward = cameraTransform.forward;
        forward.y = 0;
        forward.Normalize();
        Vector3 right = cameraTransform.right;
        right.y = 0;
        right.Normalize();
        Vector3 dir = forward * v + right * h;
        return Vector3.ClampMagnitude(dir, 1f);
    }

    void HandleStaminaRegen()
    {
        // Only regen if enough time has passed and we're not full
        if (currentStamina < maxStamina && Time.time > lastStaminaUseTime + staminaRegenDelay)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
            _slider.value = currentStamina;
            // animator.SetFloat("Stamina", currentStamina);  // if you drive a UI or animator
        }
    }
}