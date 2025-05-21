using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class CheckGhostsNear : Conditional
{
    public SharedGameObject ghost;

    public override void OnAwake()
    {

    }

    public override TaskStatus OnUpdate()
    {
        Vector3 totPos = ghost.Value.transform.position;
        if (Physics.Raycast(totPos, 2 * totPos - transform.position, 5, ~6))
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
}
