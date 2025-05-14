using System;
using UnityEngine;

public class SlashDamage : MonoBehaviour
{
    public int damage = 10;           // tunable in the inspector
    public LayerMask enemyLayer;      // set to your “Enemy” layer
    // automatically called when something enters the trigger
    void OnTriggerEnter(Collider other)
    {
        // only hit once per slash (optional—you can add logic to prevent repeated hits)
        if (((1 << other.gameObject.layer) & enemyLayer) == 0) 
            return;

        var enemy = other.GetComponent<EnemyHealth>();
        Debug.Log("Hit");
        if (enemy != null)
            enemy.TakeDamage(damage);
        
        // if you only want one hit per slash, you could:
        // enabled = false;
    }
}
