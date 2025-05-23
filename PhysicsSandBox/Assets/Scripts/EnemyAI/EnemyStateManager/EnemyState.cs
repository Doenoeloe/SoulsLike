using UnityEngine;

public abstract class EnemyState
{
    protected EnemyAI ai; // reference to the “owner”

    protected EnemyState(EnemyAI ai)
    {
        this.ai = ai;
    }

    public virtual void Enter()
    {
    }

    public virtual void Tick()
    {
    } // called every frame

    public virtual void Exit()
    {
    }
}