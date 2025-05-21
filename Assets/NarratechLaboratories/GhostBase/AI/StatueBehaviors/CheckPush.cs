using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class CheckPush : Conditional
{
    public SharedVector3 destiny;
    public SharedInt color;

    public SharedGameObject totem;
    private Vector3[] destinies;

    public override void OnAwake()
    {
        if (color.Value%10 == 0)
        {
            totem = GameObject.Find("RedMagicStatue");
        }
        else if (color.Value%10 == 1)
        {
            totem = GameObject.Find("GreenMagicStatue");
        }
        else if (color.Value%10 == 2)
        {
            totem = GameObject.Find("BlueMagicStatue");
        }

        destinies = totem.Value.GetComponent<StatueRoute>().points;
        destiny = destinies[color.Value / 10];
    }

    public override TaskStatus OnUpdate()
    {
        if (Physics.Raycast(destiny.Value, totem.Value.transform.position, 
            Vector3.SqrMagnitude(transform.position - destiny.Value), ~6))
        {
            color.Value += 10;
            return TaskStatus.Failure;
        }

        return TaskStatus.Success;
    }
}
