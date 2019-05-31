using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorAlwaysFalse : BehaviorNode
{
    public BehaviorAlwaysFalse(EnemyController enemyController) : base(enemyController)
    {
        controller = enemyController;
        children = new BehaviorNode[] {
            new CylinderEscapeNode(controller)};
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
