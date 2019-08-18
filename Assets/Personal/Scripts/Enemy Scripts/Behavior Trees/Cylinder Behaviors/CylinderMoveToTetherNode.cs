using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderMoveToTetherNode : BehaviorNode
{
    new CylinderContext context;

    public CylinderMoveToTetherNode(BehaviorNode[] nodeChildren, CylinderContext nodeContext) : base(nodeChildren, nodeContext)
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

    private void MoveToTether()
    {
        context.Nav.SetDestination(context.Destination);
    }

    //TODO update this behavior to be less naive
    private void TetheredBehavior()
    {
        context.Nav.SetDestination(context.Tether.transform.position + (Vector3)Random.insideUnitCircle * context.TetherRadius);
    }
}
