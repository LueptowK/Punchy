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
        root = new CylinderRootNode(this);
        currentNode = root;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
