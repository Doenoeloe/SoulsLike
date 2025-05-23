using UnityEngine;

public class IdleState : EnemyState
{
    float idleTimer;

    public IdleState(EnemyAI ai) : base(ai) { }

    public override void Enter()
    {
        // ai.SetAnimation("Idle");
        idleTimer = Random.Range(2f, 5f);
    }

    public override void Tick()
    {
        idleTimer -= Time.deltaTime;
        if (ai.CanSeePlayer())
            ai.TransitionToState(ai.alertState);
        else if (idleTimer <= 0)
            ai.TransitionToState(ai.patrolState);
    }
}
