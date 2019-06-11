using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoratorInverter : BehaviorNode
{
    public DecoratorInverter(BehaviorNode[] nodeChildren, Dictionary<string, object> nodeContext) : base(nodeChildren, nodeContext)
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
        statusValues status = base.FixedUpdate();
        return status == statusValues.running ? statusValues.running : (status == statusValues.failure ? statusValues.success : statusValues.failure);
    }
}
