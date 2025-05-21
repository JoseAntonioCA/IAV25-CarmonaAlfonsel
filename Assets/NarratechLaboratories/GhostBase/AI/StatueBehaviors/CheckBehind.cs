using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class CheckBehind : Conditional
{
    public SharedVector3 destiny;
    public SharedVector3 behind;
    public SharedGameObject totem;

    public override void OnAwake()
    {
        
    }

    public override TaskStatus OnUpdate()
    {
        Vector3 totPos = totem.Value.transform.position;
        if (Physics.Raycast(totPos, 2 * totPos - destiny.Value, 2, ~6))
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
}
