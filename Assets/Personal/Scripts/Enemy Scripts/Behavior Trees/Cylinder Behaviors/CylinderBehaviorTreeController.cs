using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderBehaviorTreeController : EnemyController
{
    BehaviorNode root;
    BehaviorNode currentNode;
    // Start is called before the first frame update
    protected override void Start()
    {
        CylinderContext treeContext = new CylinderContext();
        treeContext.Actor = this.gameObject;

        root = new CylinderRootNode(null, treeContext);
        currentNode = root;
    }

    // Update is called once per frame
    protected override void Update()
    {
        root.Update();
    }

    protected override void FixedUpdate()
    {
        root.FixedUpdate();
    }
}
