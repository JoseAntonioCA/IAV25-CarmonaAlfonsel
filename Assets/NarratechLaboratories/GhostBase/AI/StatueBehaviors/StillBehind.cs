using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class StillBehind : Conditional
{
    public SharedVector3 destiny;
    public SharedVector3 behind;
    public SharedGameObject totem;

    public override void OnAwake()
    {

    }

    public override TaskStatus OnUpdate()
    {
        Vector3 highPos = transform.position;
        highPos.y = 0.44f;

        if (Physics.Raycast(highPos, destiny.Value, 
            Vector3.SqrMagnitude(highPos - destiny.Value), 6))
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Failure;
    }
}
