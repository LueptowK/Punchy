using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviorNode
{
    protected EnemyController controller;
    protected BehaviorNode[] children;
    protected Dictionary<string, object> context;
    public enum statusValues { success, failure, running };

    public BehaviorNode(BehaviorNode[] nodeChildren, Dictionary<string, object> nodeContext)
    {
        children = nodeChildren;
        context = nodeContext;
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
                context["currentlyRunning"] = this; 
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
