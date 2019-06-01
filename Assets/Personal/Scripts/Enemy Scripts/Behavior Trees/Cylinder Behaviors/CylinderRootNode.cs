using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderRootNode : BehaviorNode
{
    public CylinderRootNode(BehaviorNode[] nodeChildren, Dictionary<string, object> nodeContext) : base(nodeChildren, nodeContext)
    {
        context = nodeContext;
        context.Add("root", new ContextItem<BehaviorNode>(this));
        children = new BehaviorNode[] {
            new CylinderRepositionNode(
                new BehaviorNode[] {
                    new CylinderEscapeNode(null, nodeContext)}, 
                context),
            new CylinderLaserNode(null, nodeContext),
            new CylinderFireballNode(null, nodeContext)};
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
