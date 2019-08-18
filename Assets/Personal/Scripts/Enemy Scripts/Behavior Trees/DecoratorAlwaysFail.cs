using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoratorAlwaysFail : BehaviorNode
{
    public DecoratorAlwaysFail(BehaviorNode[] nodeChildren, TreeContextObject nodeContext) : base(nodeChildren, nodeContext)
    {
        children = nodeChildren;
        context = nodeContext;
    }

    public override void Update()
    {
        base.Update();
    }

    public override statusValues FixedUpdate()
    {
        base.Update();
        return statusValues.failure;
    }
}
