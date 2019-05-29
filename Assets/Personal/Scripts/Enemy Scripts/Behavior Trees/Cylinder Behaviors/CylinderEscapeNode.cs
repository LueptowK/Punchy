using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderEscapeNode : BehaviorNode
{
    public CylinderEscapeNode(EnemyController enemyController) : base(enemyController)
    {
        controller = enemyController;
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
