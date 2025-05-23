using UnityEngine;

public class AttackState : EnemyState
{
    string[] attackNames = { "Slash", "HeavySlash", "Lunge" };
    float[]  attackWeights = { 0.5f, 0.3f, 0.2f };

    string chosenAttack;
    bool   hasTriggered = false;

    public AttackState(EnemyAI ai) : base(ai) { }

    public override void Enter()
    {
        // Weighted pick
        float r = Random.value;
        float cum = 0;
        for (int i = 0; i < attackNames.Length; i++)
        {
            cum += attackWeights[i];
            if (r <= cum)
            {
                chosenAttack = attackNames[i];
                break;
            }
        }
        // ai.SetAnimation(chosenAttack);
        hasTriggered = false;
    }

    public override void Tick()
    {
        // Wait for animation event to trigger the actual damage
        if (!hasTriggered && ai.IsAnimationAtFrame(chosenAttack, 0.3f))
        {
            // ai.DealDamage();       // your hit‐detection logic
            hasTriggered = true;
        }

        // When animation ends, go to Retreat or Strafe randomly
        if (ai.IsAnimationComplete(chosenAttack))
        {
            if (Random.value < 0.4f)
                ai.TransitionToState(ai.retreatState);
            else
                ai.TransitionToState(ai.strafeState);
        }
    }
}
