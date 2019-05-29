using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviorNode
{
    protected EnemyController controller;
    protected BehaviorNode[] children;
    public enum statusValues { success, failure, running };

    public BehaviorNode(EnemyController enemyController)
    {
        controller = enemyController;
    }

    public virtual void Update()
    {
        
    }

    public virtual statusValues FixedUpdate()
    {
        statusValues status = statusValues.failure;
        foreach (BehaviorNode child in children)
        {
            status = child.FixedUpdate();
            if (status == statusValues.success)
            {
                return status;
            }
            if (status == statusValues.running)
            {
                return status;
            }
        }
        return status;
    }

    public virtual void Enter()
    {

    }

    public virtual void Exit()
    {

    }
}
