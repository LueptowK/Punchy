using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderEscapeNode : BehaviorNode
{
    public CylinderEscapeNode(BehaviorNode[] nodeChildren, CylinderContext nodeContext) : base(nodeChildren, nodeContext)
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
        return base.FixedUpdate();
    }
}
