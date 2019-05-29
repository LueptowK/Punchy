using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderRepositionNode : BehaviorNode
{
    public CylinderRepositionNode(EnemyController enemyController) : base(enemyController)
    {
        controller = enemyController;
        children = new BehaviorNode[] {
            new CylinderEscapeNode(controller)};
    }

    public override void Update()
    {
        base.Update();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
