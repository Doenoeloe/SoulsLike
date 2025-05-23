using UnityEngine;

public class AlertState : EnemyState
{
    float alertTimer = 0f;
    const float MaxAlertTime = 2f;

    public AlertState(EnemyAI ai) : base(ai) { }

    public override void Enter()
    {
        // ai.SetAnimation("Investigate");
        Debug.Log("Checking for alert");
        alertTimer = 0f;
    }

    public override void Tick()
    {
        alertTimer += Time.deltaTime;

        // If we spot them outright, go chase
        if (ai.CanSeePlayer())
        {
            ai.TransitionToState(ai.chaseState);
            return;
        }

        // Otherwise, move toward last‐known spot for a few seconds
        ai.MoveTo(ai.GetLastKnownPlayerPos());

        if (alertTimer >= MaxAlertTime)
        {
            // Give up and go back to patrolling
            ai.TransitionToState(ai.patrolState);
        }
    }
}
