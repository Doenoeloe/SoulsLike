using UnityEngine;

public class StrafeState : EnemyState
{
    readonly float strafeDuration = 1.2f;
    float timer;
    Vector3 strafeDir;

    public StrafeState(EnemyAI ai) : base(ai) { }

    public override void Enter()
    {
        timer = strafeDuration;
        // ai.SetAnimation("Strafe");

        // 1) Stop any existing path so the agent won't chase you while strafing
        ai.agent.ResetPath();
        ai.agent.isStopped      = true;
        ai.agent.updateRotation = false;

        // 2) Compute strafe direction relative to the player→enemy line
        Vector3 toEnemy = (ai.transform.position - ai.player.position).normalized;
        Vector3 right   = Vector3.Cross(Vector3.up, toEnemy);
        strafeDir       = (Random.value > 0.5f) ? right : -right;
    }

    public override void Tick()
    {
        timer -= Time.deltaTime;

        // 3) Slide sideways every frame
        ai.agent.Move(strafeDir * ai.agent.speed * Time.deltaTime);
        
        ai.transform.rotation = Quaternion.LookRotation(
            (ai.player.position - ai.transform.position).normalized
        );
        // 4) If we can attack now, go attack
        if (ai.InAttackRange())
        {
            ai.TransitionToState(ai.attackState);
            return;
        }

        // 5) Once our time is up, go back to chasing
        if (timer <= 0f)
        {
            ai.TransitionToState(ai.chaseState);
        }
    }

    public override void Exit()
    {
        // 6) Restore normal NavMeshAgent behavior
        ai.agent.isStopped      = false;
        ai.agent.updateRotation = true;
    }
}
