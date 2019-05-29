using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderRootNode : BehaviorNode
{
    public CylinderRootNode(EnemyController enemyController) : base(enemyController)
    {
        controller = enemyController;
        children = new BehaviorNode[] {
            new CylinderRepositionNode(controller),
            new CylinderLaserNode(controller),
            new CylinderFireballNode(controller)};
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
