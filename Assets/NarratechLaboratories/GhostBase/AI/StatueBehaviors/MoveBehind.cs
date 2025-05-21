using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

public class MoveBehind : Action
{
    public SharedVector3 behind;
    private NavMeshAgent navMeshAgent;

    public override void OnAwake()
    {
        navMeshAgent = transform.gameObject.GetComponent<NavMeshAgent>();
    }

    public override void OnStart()
    {
        navMeshAgent.SetDestination(behind.Value);
    }

    public override TaskStatus OnUpdate()
    {
        if (Vector3.SqrMagnitude(transform.position - behind.Value) < 0.1f)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }
}
