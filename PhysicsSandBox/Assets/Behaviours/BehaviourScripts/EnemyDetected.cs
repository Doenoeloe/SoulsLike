using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/EnemyDetected")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "EnemyDetected", message: "Agen has spotted Enemy", category: "Events", id: "8f4d81ae3a185195939f1ccd9b0c4613")]
public partial class EnemyDetected : EventChannelBase
{
    public delegate void EnemyDetectedEventHandler();
    public event EnemyDetectedEventHandler Event; 

    public void SendEventMessage()
    {
        Event?.Invoke();
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        Event?.Invoke();
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        EnemyDetectedEventHandler del = () =>
        {
            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as EnemyDetectedEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as EnemyDetectedEventHandler;
    }
}

