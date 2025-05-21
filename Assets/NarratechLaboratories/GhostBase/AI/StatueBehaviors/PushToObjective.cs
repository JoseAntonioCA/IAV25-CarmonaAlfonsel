using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

public class PushToObjective : Action
{
    public float speed = 0.1f;
    public SharedVector3 objective;

    private NavMeshAgent navMeshAgent;

    public override void OnAwake()
    {
        navMeshAgent = transform.gameObject.GetComponent<NavMeshAgent>();
    }

    public override void OnStart()
    {
        navMeshAgent.SetDestination(objective.Value);
    }

    public override TaskStatus OnUpdate()
    {
        if (Vector3.SqrMagnitude(transform.position - objective.Value) < 0.1f)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }
}
