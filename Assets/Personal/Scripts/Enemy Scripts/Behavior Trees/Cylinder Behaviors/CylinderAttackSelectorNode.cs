using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderAttackSelectorNode : BehaviorNode
{
    public CylinderAttackSelectorNode(BehaviorNode[] nodeChildren, Dictionary<string, object> nodeContext) : base(nodeChildren, nodeContext)
    {
        children = nodeChildren;
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
