using UnityEngine;

public class RetreatState : EnemyState
{
    float retreatTime = 1f;
    float timer;

    public RetreatState(EnemyAI ai) : base(ai) { }

    public override void Enter()
    {
        timer = retreatTime;
        // ai.SetAnimation("BackwardStep");

        // 1) Direction from player to this enemy
        Vector3 awayDir = (ai.transform.position - ai.player.position).normalized;

        // 2) Target retreat position
        Vector3 retreatPos = ai.transform.position + awayDir * ai.backoffDistance;

        ai.MoveTo(retreatPos);
    }


    public override void Tick()
    {
        timer -= Time.deltaTime;

        // If we've reached that safe spot, or the timer has elapsed, go chase again
        if (timer <= 0f || ai.HasReachedDestination())
        {
            ai.TransitionToState(ai.chaseState);
        }
    }
}
