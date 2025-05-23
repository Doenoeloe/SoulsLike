using UnityEngine;

public class PatrolState : EnemyState
{
    int currentWaypoint = 0;

    public PatrolState(EnemyAI ai) : base(ai) { }

    public override void Enter()
    {
        // ai.SetAnimation("Walk");
        if (ai.waypoints.Count > 0)
            ai.MoveTo(ai.waypoints[currentWaypoint]);
    }

    public override void Tick()
    {
        Vector3 dir    = (ai.player.position - ai.transform.position).normalized;
        float   dist   = Vector3.Distance(ai.transform.position, ai.player.position);
        float   angle  = Vector3.Angle(ai.transform.forward, dir);
        Debug.Log($"[Patrol] dist={dist:F1}, angle={angle:F1}");
        if (ai.CanSeePlayer())
        {
            Debug.Log("[Patrol] Saw player! Transitioning to Alert.");
            ai.TransitionToState(ai.alertState);
            return;
        }

        // If we’ve reached our current waypoint, pick the next
        if (ai.HasReachedDestination() && ai.waypoints.Count > 0)
        {
            currentWaypoint = (currentWaypoint + 1) % ai.waypoints.Count;
            ai.MoveTo(ai.waypoints[currentWaypoint]);
        }
    }
}
