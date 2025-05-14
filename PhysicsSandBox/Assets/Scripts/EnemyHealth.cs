using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI References")]
    public Canvas healthCanvas;      // A World-Space canvas childed to the enemy’s head
    public Slider healthSlider;      // The Slider component on that canvas
    public float displayDuration = 2f;  // How long to show after each hit

    private Coroutine hideRoutine;
    LockOnSystem lockOnSystem;

    void Start()
    {
        // initialize
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        healthCanvas.gameObject.SetActive(false);
        lockOnSystem = FindFirstObjectByType<LockOnSystem>();
    }

    /// <summary>
    /// Call this from your slash‐hit script when you hit the enemy
    /// </summary>
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // update UI and make sure it’s visible
        healthSlider.value = currentHealth;
        ShowHealthBar();

        if (currentHealth <= 0f)
            Die();
    }

    void ShowHealthBar()
    {
        // stop any pending hide
        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        healthCanvas.gameObject.SetActive(true);
        hideRoutine = StartCoroutine(HideAfterDelay());
    }

    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        healthCanvas.gameObject.SetActive(false);
    }

    void Die()
    {
        // notify the lock‐on system that *this* enemy is gone
        if (lockOnSystem != null)
            lockOnSystem.OnEnemyDeath(transform);

        // then handle your death visuals / destruction
        Destroy(gameObject);
    }
}
