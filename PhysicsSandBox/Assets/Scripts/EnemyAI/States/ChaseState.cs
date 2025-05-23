using UnityEngine;

public class ChaseState : EnemyState
{
    public ChaseState(EnemyAI ai) : base(ai) { }

    public override void Enter()
    {
        // ai.SetAnimation("Run");
    }

    public override void Tick()
    {
        // If we lose sight, go back to Alert (to check last‐known spot)
        if (!ai.CanSeePlayer())
        {
            ai.TransitionToState(ai.alertState);
            return;
        }

        // If we get into attack range, go attack
        if (ai.InAttackRange())
        {
            ai.TransitionToState(ai.strafeState);
            return;
        }

        // Otherwise, keep chasing
        ai.MoveTo(ai.player);
    }
}
