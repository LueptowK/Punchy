using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderCheckDangerNode : BehaviorNode
{
    new CylinderContext context;

    public CylinderCheckDangerNode(BehaviorNode[] nodeChildren, CylinderContext nodeContext) : base(nodeChildren, nodeContext)
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
        if (Vector3.Distance(context.Player.transform.position, context.Transform.position) < context.RunAwayDistance)
        {
            context.Tether = null;
        }
        return statusValues.failure;
    }
}
