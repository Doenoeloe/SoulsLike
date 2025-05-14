using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(SphereCollider))]
public class LockOnSystem : MonoBehaviour
{
    [Header("References")] public GameObject lockOnPrefab;
    public LayerMask enemyLayer;
    public LayerMask slashLayer;
    [Header("Input")] public KeyCode lockKey = KeyCode.Tab;
    public KeyCode nextKey = KeyCode.E;
    public KeyCode prevKey = KeyCode.Q;

    [Header("Lock Settings")] [Range(0, 100)]
    public float maxLockDist = 15f;
    public List<Transform> targets = new List<Transform>();
    public Transform currentTarget;
    int currentIndex = -1;

    GameObject lockOnInstance;
    Camera cam;

    private void Reset()
    {
        // auto-add/configure the trigger collider
        var col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = maxLockDist;
    }

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // Toggle lock
        if (Input.GetKeyDown(lockKey))
        {
            if (currentTarget == null) AcquireFirstTarget();
            else ClearLock();
        }

        // Cycle through the **already accumulated** targets
        if (currentTarget != null)
        {
            if (Input.GetKeyDown(nextKey)) CycleTarget(+1);
            else if (Input.GetKeyDown(prevKey)) CycleTarget(-1);

            // If the current target was destroyed/deactivated, drop lock
            if (!IsTargetStillAlive(currentTarget))
            {
                ClearLock();
            }
        }
    }

    void LateUpdate()
    {
        if (currentTarget != null && lockOnInstance != null)
            UpdateMarkerTransform();
    }

    // ——— Trigger callbacks build the live list ———

    void OnTriggerEnter(Collider other)
    {
        // only add if on the enemyLayer
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            var t = other.transform;
            if (!targets.Contains(t))
                targets.Add(t);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == slashLayer)
            return;
        
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            var t = other.transform;
            targets.Remove(t);
            // if we just lost our locked-on target, clear it
            if (t == currentTarget)
                ClearLock();
        }
    }

    // ——— Lock / Cycle ———
    public void AcquireFirstTarget()
    {
        // 0) Purge any dead or destroyed
        targets = targets
            .Where(IsTargetStillAlive)
            .ToList();

        if (targets.Count == 0)
            return;

        // 1) sort by angle then distance (now all t are valid)
        var sorted = targets
            .Select(t =>
            {
                Vector3 dir = (t.position - transform.position).normalized;
                float ang = Vector3.SignedAngle(transform.forward, dir, Vector3.up);
                return (t, key1: Mathf.Abs(ang), key2: Vector3.Distance(transform.position, t.position));
            })
            .OrderBy(x => x.key1)
            .ThenBy(x => x.key2)
            .Select(x => x.t)
            .ToList();

        targets = sorted;

        // 2) pick the first
        currentIndex = 0;
        currentTarget = targets[0];
        SpawnMarker();
    }

    void ClearLock()
    {
        currentTarget = null;
        currentIndex = -1;
        if (lockOnInstance != null)
            Destroy(lockOnInstance);
    }

    void CycleTarget(int dir)
    {
        if (targets.Count == 0) return;
        currentIndex = (currentIndex + dir + targets.Count) % targets.Count;
        currentTarget = targets[currentIndex];
    }
    bool IsTargetStillAlive(Transform t) => t != null && t.gameObject.activeInHierarchy;

    // ——— Marker Logic (unchanged) ———
    void SpawnMarker()
    {
        if (lockOnInstance != null) Destroy(lockOnInstance);
        lockOnInstance = Instantiate(lockOnPrefab);
    }

    Bounds GetCombinedBounds(Transform root)
    {
        var rens = root.GetComponentsInChildren<Renderer>();
        if (rens.Length == 0) return new Bounds(root.position, Vector3.zero);
        var b = rens[0].bounds;
        foreach (var r in rens.Skip(1)) b.Encapsulate(r.bounds);
        return b;
    }

    void UpdateMarkerTransform()
    {
        Bounds body = GetCombinedBounds(currentTarget);
        Vector3 center = body.center + Vector3.up * (body.extents.y * .5f);
        Vector3 toCam = (cam.transform.position - center).normalized;
        float push = 1f + body.extents.magnitude * .05f;
        Vector3 pos = center + toCam * push;

        lockOnInstance.transform.position = pos;
        lockOnInstance.transform.rotation = Quaternion.LookRotation(pos - cam.transform.position);
        float dist = Vector3.Distance(cam.transform.position, pos);
        lockOnInstance.transform.localScale = Vector3.one * (dist * .001f);
    }

    public void OnEnemyDeath(Transform deadTarget)
    {
        // 1) Remove it from the list
        targets.Remove(deadTarget);

        // 2) If it was the one we were locked on:
        if (deadTarget == currentTarget)
        {
            if (targets.Count > 0)
            {
                // wrap currentIndex to be within the new list size
                currentIndex = currentIndex % targets.Count;
                currentTarget = targets[currentIndex];
                // ensure we have a lock‐on marker
                if (lockOnInstance == null) SpawnMarker();
            }
            else
            {
                // no more targets, clear the lock
                ClearLock();
            }
        }
    }
}