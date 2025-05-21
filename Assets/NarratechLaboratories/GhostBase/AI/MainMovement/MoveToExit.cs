using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

public class MoveToExit : Action
{
    public SharedVector3 exit;
    private NavMeshAgent navMeshAgent;

    public override void OnAwake()
    {
        navMeshAgent = transform.gameObject.GetComponent<NavMeshAgent>();
    }

    public override void OnStart()
    {
        navMeshAgent.SetDestination(exit.Value);
    }

    public override TaskStatus OnUpdate()
    {
        if (Vector3.SqrMagnitude(transform.position - exit.Value) < 0.01f)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }
}
