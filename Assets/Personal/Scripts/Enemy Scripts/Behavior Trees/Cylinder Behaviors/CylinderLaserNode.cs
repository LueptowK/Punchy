using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderLaserNode : BehaviorNode
{
    public CylinderLaserNode(BehaviorNode[] nodeChildren, Dictionary<string, object> nodeContext) : base(nodeChildren, nodeContext)
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
