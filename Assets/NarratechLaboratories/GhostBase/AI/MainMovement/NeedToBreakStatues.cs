using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class NeedToBreakStatues : Conditional
{        
    public SharedGameObject totem;

    public override void OnAwake()
    {

    }

    public override TaskStatus OnUpdate()
    {
        Vector3 totPos = totem.Value.transform.position;
        if (Physics.Raycast(totPos, 2 * totPos - transform.position, 10, ~6))
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
}
